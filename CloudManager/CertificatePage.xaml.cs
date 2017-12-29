using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Slb.Model.V20140515;
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
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeCACertificatesResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeServerCertificatesResponse;

namespace CloudManager
{
    /// <summary>
    /// CertificatePage.xaml 的交互逻辑
    /// </summary>
    public partial class CertificatePage : PageBase
    {
        private string mAki, mAks;
        private List<DescribeRegions_Region> mRegions;
        private ObservableCollection<DescribeCertificate> mCertificates = new ObservableCollection<DescribeCertificate>();
        private delegate void DelegateGot(object obj);

        public MainWindow mMainWindow { get; set; }


        public CertificatePage()
        {
            InitializeComponent();
            HideBlankPage = true;

            mAki = App.AKI;
            mAks = App.AKS;
            mRegions = App.REGIONS;
            CertificatesList.ItemsSource = mCertificates;
            this.Loaded += delegate
            {
                if (!Refreshed)
                {
                    RefreshPage();
                }
            };
        }

        protected override void RefreshPage()
        {
            Refreshed = true;
            mCertificates.Clear();
            GetCertificates();
        }

        private void GotCertificates(object obj)
        {
            DescribeCertificate cert = obj as DescribeCertificate;
            mCertificates.Add(cert);
        }

        private void GetCertificates()
        {
            DoLoadingWork(page =>
            {
                //ObservableCollection<DescribeCertificate> certs = new ObservableCollection<DescribeCertificate>();
                Parallel.ForEach(mRegions, (region) =>
                {
                    IClientProfile profile = DefaultProfile.GetProfile(region.RegionId, mAki, mAks);
                    DefaultAcsClient client = new DefaultAcsClient(profile);
                    DescribeServerCertificatesRequest r1 = new DescribeServerCertificatesRequest();
                    DescribeCACertificatesRequest r2 = new DescribeCACertificatesRequest();
                    try
                    {
                        DescribeServerCertificatesResponse resp1 = client.GetAcsResponse(r1);
                        foreach (DescribeServerCertificates_ServerCertificate c in resp1.ServerCertificates)
                        {
                            DescribeCertificate cert = new DescribeCertificate(c);
                            cert.RegionLocalName = region.LocalName;
                            Dispatcher.Invoke(new DelegateGot(GotCertificates), cert);
                            //certs.Add(cert);
                        }
                        DescribeCACertificatesResponse resp2 = client.GetAcsResponse(r2);
                        foreach (DescribeCACertificates_CACertificate c in resp2.CACertificates)
                        {
                            DescribeCertificate cert = new DescribeCertificate(c);
                            cert.RegionLocalName = region.LocalName;
                            Dispatcher.Invoke(new DelegateGot(GotCertificates), cert);
                            //certs.Add(cert);
                        }
                    }
                    catch
                    {
                    }
                });
            },
            ex =>
            {
            });
            
        }

        private void ModifyCertificateName_Click(object sender, RoutedEventArgs e)
        {
            DescribeCertificate cert = (sender as Button).DataContext as DescribeCertificate;
            IClientProfile profile = DefaultProfile.GetProfile(cert.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);

            EditCertNameWindow win = new EditCertNameWindow(client, cert);
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.UpdateEventHandler += UpdateCertificateName;
            win.ShowDialog();
        }

        private void UpdateCertificateName(object sender, string name)
        {
            DescribeCertificate cert = sender as DescribeCertificate;
            cert.CertificateName = name;
        }

        private void DeleteCertificate_Click(object sender, RoutedEventArgs e)
        {
            DescribeCertificate cert = (sender as Button).DataContext as DescribeCertificate;
            Thread t = new Thread(new ParameterizedThreadStart(DeleteCertificate));
            t.Start(cert);
        }

        private void DeleteCertificate(object obj)
        {
            DescribeCertificate cert = obj as DescribeCertificate;
            IClientProfile profile = DefaultProfile.GetProfile(cert.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            try
            {
                if (cert.CertificateType.Equals("ServerCertificate"))
                {
                    DeleteServerCertificateRequest request = new DeleteServerCertificateRequest();
                    request.ServerCertificateId = cert.CertificateId;
                    DeleteServerCertificateResponse response = client.GetAcsResponse(request);
                }
                else if (cert.CertificateType.Equals("CACertificate"))
                {
                    DeleteCACertificateRequest request = new DeleteCACertificateRequest();
                    request.CACertificateId = cert.CertificateId;
                    DeleteCACertificateResponse response = client.GetAcsResponse(request);
                }
                Dispatcher.Invoke(new DelegateGot(DeletedCertificate), cert);
            }
            catch
            {
            }
        }

        private void DeletedCertificate(object obj)
        {
            DescribeCertificate cert = obj as DescribeCertificate;
            mCertificates.Remove(cert);
        }

        private void AddCertificate_Click(object sender, RoutedEventArgs e)
        {
            CreateCertificateWindow win = new CreateCertificateWindow();
            win.RefreshEvent += RefreshCertificates;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.ShowDialog();
        }

        private void RefreshCertificates(object sender, List<string> regions)
        {
            RefreshPage();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshPage();
        }
    }
}
