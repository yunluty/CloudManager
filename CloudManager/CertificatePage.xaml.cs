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
    public partial class CertificatePage : Page
    {
        private string mAki, mAks;
        private static List<DescribeRegions_Region> mRegions;
        private ObservableCollection<DescribeCertificate> mCertificates = new ObservableCollection<DescribeCertificate>();
        private delegate void DelegateGot(object obj);

        public MainWindow mMainWindow { get; set; }


        public CertificatePage()
        {
            InitializeComponent();

            mAki = App.AKI;
            mAks = App.AKS;
            mRegions = App.REGIONS;
            CertificatesList.ItemsSource = mCertificates;

            Thread t = new Thread(GetCertificates);
            t.Start();
        }

        private void GotCertificates(object obj)
        {
            mCertificates = obj as ObservableCollection<DescribeCertificate>;
            CertificatesList.ItemsSource = mCertificates;
        }

        private void ModifyCertificateName_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteCertificate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GetCertificates()
        {
            ObservableCollection<DescribeCertificate> certs = new ObservableCollection<DescribeCertificate>();
            foreach (DescribeRegions_Region region in mRegions)
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
                        certs.Add(cert);
                    }
                    DescribeCACertificatesResponse resp2 = client.GetAcsResponse(r2);
                    foreach (DescribeCACertificates_CACertificate c in resp2.CACertificates)
                    {
                        DescribeCertificate cert = new DescribeCertificate(c);
                        cert.RegionLocalName = region.LocalName;
                        certs.Add(cert);
                    }
                }
                catch
                {
                }
            }
            Dispatcher.Invoke(new DelegateGot(GotCertificates), certs);
        }
    }
}
