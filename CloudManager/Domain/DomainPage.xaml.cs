using Aliyun.Acs.Alidns.Model.V20150109;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Domain.Model.V20160511;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Aliyun.Acs.Alidns.Model.V20150109.DescribeDomainRecordsResponse;
using static Aliyun.Acs.Domain.Model.V20160511.QueryDomainListResponse;

namespace CloudManager.Domain
{
    /// <summary>
    /// DomainPage.xaml 的交互逻辑
    /// </summary>
    public partial class DomainPage : PageBase
    {
        private DescribeDomain mSelDomain;
        private DefaultAcsClient mClient;
        private ObservableCollection<DescribeDomain> mDomains = new ObservableCollection<DescribeDomain>();
        private ObservableCollection<DescribeDomainRecord> mRecords = new ObservableCollection<DescribeDomainRecord>();

        public MainWindow mMainWindow { get; set; }

        public DomainPage()
        {
            InitializeComponent();

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", App.AKI, App.AKS);
            mClient = new DefaultAcsClient(profile);
        }

        protected override void RefreshPage()
        {
            GetDomainList();
        }

        private void GetDomainList()
        {
            DoLoadingWork("正在加载域名", page =>
            {
                var nextPage = false;
                var domains = new ObservableCollection<DescribeDomain>();
                var count = 0;
                do
                {
                    QueryDomainListRequest request = new QueryDomainListRequest();
                    request.PageNum = ++count;
                    request.PageSize = 100;
                    try
                    {
                        QueryDomainListResponse response = mClient.GetAcsResponse(request);
                        nextPage = response.NextPage == true;
                        foreach (QueryDomainList_Domain d in response.Data)
                        {
                            DescribeDomain domain = new DescribeDomain(d);
                            GetDomainRecords(domain);
                            domains.Add(domain);
                        }
                    }
                    catch
                    {

                    }
                } while (nextPage);

                Dispatcher.Invoke(() =>
                {
                    mDomains = domains;
                    DomainList.ItemsSource = mDomains;
                    SelectDefaultIndex(DomainList);
                    HideInitPage(domains);
                });
            },
            ex =>
            {
            });
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }

        private void SelectDefaultIndex(ListBox list)
        {
            if (list.Items.Count > 0 && list.SelectedIndex == -1)
            {
                list.SelectedIndex = 0;
            }
        }

        private void GetDomainInfo(DescribeDomain domain)
        {
            Task.Run(() =>
            {
                int errorCount = 0;
                try
                {
                    QueryDomainBySaleIdRequest request = new QueryDomainBySaleIdRequest();
                    request.SaleId = domain.SaleId;
                    QueryDomainBySaleIdResponse response = mClient.GetAcsResponse(request);
                    domain.SetDomainInfo(response);
                    GetWhoisInfoRequest r2 = new GetWhoisInfoRequest();
                    r2.AcceptFormat = Aliyun.Acs.Core.Http.FormatType.JSON;
                    r2.DomainName = domain.DomainName;
                    do
                    {
                        if (errorCount != 0)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(30));
                        }

                        try
                        {
                            GetWhoisInfoResponse resp2 = mClient.GetAcsResponse(r2);
                            domain.SetWhoisInfo(resp2);
                            break;
                        }
                        catch
                        {
                            if (domain.Registrar == null)
                            {
                                errorCount++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    } while (errorCount < 2);
                }
                catch
                {
                }

                Dispatcher.Invoke(() =>
                {
                    if (domain == mSelDomain)
                    {
                        //Information.DataContext = mSelDomain;
                        SetDNSAndDomainStatus();
                    }
                });
            });
        }

        private void SetDNSAndDomainStatus()
        {
            DomainStatus.Children.Clear();
            DNSList.Children.Clear();
            if (mSelDomain.DomainStatusList != null
                && mSelDomain.DomainStatusList.Count > 0)
            {
                foreach (string status in mSelDomain.DomainStatusList)
                {
                    Label label = new Label();
                    label.Content = status;
                    DomainStatus.Children.Add(label);
                }
            }

            if (mSelDomain.DNSList != null
                && mSelDomain.DNSList.Count > 0)
            {
                foreach (string dns in mSelDomain.DNSList)
                {
                    Label label = new Label();
                    label.Content = dns;
                    DNSList.Children.Add(label);
                }
            }
        }

        private void DomainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedIndex == -1)
            {
                return;
            }

            mSelDomain = (sender as ListBox).SelectedItem as DescribeDomain;
            Information.DataContext = mSelDomain;
            if (mSelDomain.Registrar == null || mSelDomain.DomainStatusList == null)
            {
                GetDomainInfo(mSelDomain);
            }
            else
            {
                SetDNSAndDomainStatus();
            }
            RecordList.ItemsSource = mSelDomain.Records;
        }

        private void GetDomainRecords(DescribeDomain domain)
        {
            var nextPage = false;
            var sum = 0L;
            var count = 0;
            var records = new ObservableCollection<DescribeDomainRecord>();
            do
            {
                try
                {
                    DescribeDomainRecordsRequest request = new DescribeDomainRecordsRequest();
                    request.DomainName = domain.DomainName;
                    request.PageNumber = ++count;
                    request.PageSize = 100;
                    DescribeDomainRecordsResponse response = mClient.GetAcsResponse(request);
                    foreach (Record r in response.DomainRecords)
                    {
                        DescribeDomainRecord record = new DescribeDomainRecord(r);
                        record.Domain = domain;
                        records.Add(record);
                    }
                    sum += response.DomainRecords.Count;
                    nextPage = response.TotalCount > sum;
                }
                catch
                {
                }
            } while (nextPage);
            domain.Records = records;
        }

        private void RefreshRecords(DescribeDomain domain)
        {
            DoLoadingWork(page =>
            {
                GetDomainRecords(domain);
                Dispatcher.Invoke(() =>
                {
                    if (mSelDomain == domain)
                    {
                        RecordList.ItemsSource = domain.Records;
                    }
                });
            },
            ex =>
            {
            });
        }

        private void ModifyRecord_Click(object sender, RoutedEventArgs e)
        {
            DescribeDomainRecord record = (sender as Button).DataContext as DescribeDomainRecord;
            EditRecordWindow win = new EditRecordWindow(mClient, record);
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.ShowDialog();
            if (win.UpdateRecords)
            {
                RefreshRecords(mSelDomain);
            }
        }

        private void PauseDomainRecord(DescribeDomainRecord record)
        {
            DoLoadingWork(page =>
            {
                SetDomainRecordStatusRequest request = new SetDomainRecordStatusRequest();
                request.RecordId = record.RecordId;
                if (record.Status.Equals("Enable", StringComparison.CurrentCultureIgnoreCase))
                {
                    request.Status = "Disable";
                }
                else
                {
                    request.Status = "Enable";
                }
                SetDomainRecordStatusResponse response = mClient.GetAcsResponse(request);
                record.Status = response.Status;
            },
            ex =>
            {
            });
        }

        private void SetRecordStatus_Click(object sender, RoutedEventArgs e)
        {
            PauseDomainRecord((sender as Button).DataContext as DescribeDomainRecord);
        }

        private void DeleteDomainRecord(DescribeDomainRecord record)
        {
            DoLoadingWork(page =>
            {
                DeleteDomainRecordRequest request = new DeleteDomainRecordRequest();
                request.RecordId = record.RecordId;
                DeleteDomainRecordResponse response = mClient.GetAcsResponse(request);
                Dispatcher.Invoke(() =>
                {
                    record.Domain.Records.Remove(record);
                });
            },
            ex =>
            {
            });
        }

        private void DeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            DeleteDomainRecord((sender as Button).DataContext as DescribeDomainRecord);
        }

        private void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            EditRecordWindow win = new EditRecordWindow(mClient, mSelDomain);
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.ShowDialog();
            if (win.UpdateRecords)
            {
                RefreshRecords(mSelDomain);
            }
        }
    }
}
