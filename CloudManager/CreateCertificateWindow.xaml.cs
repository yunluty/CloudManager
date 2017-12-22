using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Slb.Model.V20140515;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse;

namespace CloudManager
{
    /// <summary>
    /// CreateCertificateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreateCertificateWindow : WindowBase
    {
        private List<CertificateRegion> CertRegions = new List<CertificateRegion>();
        /*{
            new CertificateRegion() { LocalName = "华北 1", RegionId = "cn-qingdao"},
            new CertificateRegion() { LocalName = "华北 2", RegionId = "cn-beijing"},
            new CertificateRegion() { LocalName = "华东 1", RegionId = "cn-hangzhou"},
            new CertificateRegion() { LocalName = "华东 2", RegionId = "cn-shanghai"},
            new CertificateRegion() { LocalName = "华南 1", RegionId = "cn-shenzhen"},
            new CertificateRegion() { LocalName = "香港", RegionId = "cn-hongkong"},
            new CertificateRegion() { LocalName = "亚太东南 1 (新加坡)", RegionId = "ap-southeast-1"},
            new CertificateRegion() { LocalName = "美国东部 1 (弗吉尼亚)", RegionId = "us-east-1"},
            new CertificateRegion() { LocalName = "美国西部 1 (硅谷)", RegionId = "us-west-1"}
        };*/
        private List<CheckBox> mCheckBoxs = new List<CheckBox>();
        private CreateCertParameter mParameter = new CreateCertParameter() { IsServer = true };

        public EventHandler<List<string>> RefreshEvent;

        public CreateCertificateWindow()
        {
            InitializeComponent();

            InitializeRegions();
            this.DataContext = mParameter;
            InitializeCheckBox();
        }

        private void InitializeRegions()
        {
            foreach (DescribeRegions_Region region in App.REGIONS)
            {
                CertificateRegion r = new CertificateRegion() { LocalName = region.LocalName, RegionId = region.RegionId };
                CertRegions.Add(r);
            }
        }

        /*private void GetCheckBox(DependencyObject obj)
        {
            DependencyObject child = null;
            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is CheckBox)
                {
                    mCheckBoxs.Add((CheckBox)child);
                }
                else
                {
                    GetCheckBox(child);
                }
            }
        }*/

        private void InitializeCheckBox()
        {
            StackPanel panel = null;
            int count = 0;
            int panelCount = 0;
            for (int i = 0; i < CertRegions.Count; i++)
            {
                CheckBox check = new CheckBox();
                if (count > 0)
                {
                    check.Margin = new Thickness(10, 0, 0, 0);
                }
                check.SetBinding(CheckBox.ContentProperty, new Binding("LocalName"));
                check.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsCheck") { Mode = BindingMode.TwoWay });
                check.DataContext = CertRegions[i];
                if (panel == null)
                {
                    panel = new StackPanel() { Orientation = Orientation.Horizontal };
                    if (panelCount > 0)
                    {
                        panel.Margin = new Thickness(0, 5, 0, 0);
                    }
                    panelCount++;
                }
                panel.Children.Add(check);
                count++;

                if ((panelCount == 1 && count > 5)
                    || (panelCount == 2 && count > 3)
                    || (panelCount > 2 && count > 2)
                    || i == CertRegions.Count - 1) //TODO:可以使用Width来判断
                {
                    RegionCheck.Children.Add(panel);
                    panel = null;
                    count = 0;
                }
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            string aki = App.AKI;
            string aks = App.AKS;
            List<string> regionIds = new List<string>();
            foreach (CertificateRegion region in CertRegions)
            {
                if (region.IsCheck)
                {
                    regionIds.Add(region.RegionId);
                }
            }
            bool isServer = mParameter.IsServer;
            string cert = null;
            string key = null;
            if (isServer)
            {
                cert = mParameter.ServerCertificate == null ? null : String.Copy(mParameter.ServerCertificate).Replace("\r", "");
                key = mParameter.PrivateKey == null ? null : String.Copy(mParameter.PrivateKey).Replace("\r", "");
            }
            else
            {
                cert = mParameter.CACertificate == null ? null : String.Copy(mParameter.CACertificate).Replace("\r", "");
            }
            string name = mParameter.CertName == null ? null : mParameter.CertName;

            Task.Run(() =>
            {
                Parallel.ForEach(regionIds, (id) =>
                {
                    IClientProfile profile = DefaultProfile.GetProfile(id, aki, aks);
                    DefaultAcsClient client = new DefaultAcsClient(profile);
                    try
                    {
                        if (isServer)
                        {
                            UploadServerCertificateRequest request = new UploadServerCertificateRequest();
                            request.ServerCertificate = cert;
                            request.PrivateKey = key;
                            request.ServerCertificateName = name;
                            UploadServerCertificateResponse response = client.GetAcsResponse(request);
                        }
                        else
                        {
                            UploadCACertificateRequest request = new UploadCACertificateRequest();
                            request.CACertificate = cert;
                            request.CACertificateName = name;
                            UploadCACertificateResponse response = client.GetAcsResponse(request);
                        }
                    }
                    catch
                    {
                    }
                });

                Dispatcher.Invoke(() =>
                {
                    RefreshEvent?.Invoke(this, regionIds);
                    this.Close();
                });
            });
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Example1_Click(object sender, RoutedEventArgs e)
        {
            mParameter.ServerCertificate = @"-----BEGIN CERTIFICATE-----
MIIDRjCCAq+gAwIBAgIJAJn3ox4K13PoMA0GCSqGSIb3DQEBBQUAMHYxCzAJBgNV
BAYTAkNOMQswCQYDVQQIEwJCSjELMAkGA1UEBxMCQkoxDDAKBgNVBAoTA0FMSTEP
MA0GA1UECxMGQUxJWVVOMQ0wCwYDVQQDEwR0ZXN0MR8wHQYJKoZIhvcNAQkBFhB0
ZXN0QGhvdG1haWwuY29tMB4XDTE0MTEyNDA2MDQyNVoXDTI0MTEyMTA2MDQyNVow
djELMAkGA1UEBhMCQ04xCzAJBgNVBAgTAkJKMQswCQYDVQQHEwJCSjEMMAoGA1UE
ChMDQUxJMQ8wDQYDVQQLEwZBTElZVU4xDTALBgNVBAMTBHRlc3QxHzAdBgkqhkiG
9w0BCQEWEHRlc3RAaG90bWFpbC5jb20wgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJ
AoGBAM7SS3e9+Nj0HKAsRuIDNSsS3UK6b+62YQb2uuhKrp1HMrOx61WSDR2qkAnB
coG00Uz38EE+9DLYNUVQBK7aSgLP5M1Ak4wr4GqGyCgjejzzh3DshUzLCCy2rook
KOyRTlPX+Q5l7rE1fcSNzgepcae5i2sE1XXXzLRIDIvQxcspAgMBAAGjgdswgdgw
HQYDVR0OBBYEFBdy+OuMsvbkV7R14f0OyoLoh2z4MIGoBgNVHSMEgaAwgZ2AFBdy
+OuMsvbkV7R14f0OyoLoh2z4oXqkeDB2MQswCQYDVQQGEwJDTjELMAkGA1UECBMC
QkoxCzAJBgNVBAcTAkJKMQwwCgYDVQQKEwNBTEkxDzANBgNVBAsTBkFMSVlVTjEN
MAsGA1UEAxMEdGVzdDEfMB0GCSqGSIb3DQEJARYQdGVzdEBob3RtYWlsLmNvbYIJ
AJn3ox4K13PoMAwGA1UdEwQFMAMBAf8wDQYJKoZIhvcNAQEFBQADgYEAY7KOsnyT
cQzfhiiG7ASjiPakw5wXoycHt5GCvLG5htp2TKVzgv9QTliA3gtfv6oV4zRZx7X1
Ofi6hVgErtHaXJheuPVeW6eAW8mHBoEfvDAfU3y9waYrtUevSl07643bzKL6v+Qd
DUBTxOAvSYfXTtI90EAxEG/bJJyOm5LqoiA=
-----END CERTIFICATE-----";
        }

        private void Example2_Click(object sender, RoutedEventArgs e)
        {
            mParameter.PrivateKey = @"-----BEGIN RSA PRIVATE KEY-----
MIICXAIBAAKBgQDO0kt3vfjY9BygLEbiAzUrEt1Cum/utmEG9rroSq6dRzKzsetV
kg0dqpAJwXKBtNFM9/BBPvQy2DVFUASu2koCz+TNQJOMK+BqhsgoI3o884dw7IVM
ywgstq6KJCjskU5T1/kOZe6xNX3Ejc4HqXGnuYtrBNV118y0SAyL0MXLKQIDAQAB
AoGAfe3NxbsGKhN42o4bGsKZPQDfeCHMxayGp5bTd10BtQIE/ST4BcJH+ihAS7Bd
6FwQlKzivNd4GP1MckemklCXfsVckdL94e8ZbJl23GdWul3v8V+KndJHqv5zVJmP
hwWoKimwIBTb2s0ctVryr2f18N4hhyFw1yGp0VxclGHkjgECQQD9CvllsnOwHpP4
MdrDHbdb29QrobKyKW8pPcDd+sth+kP6Y8MnCVuAKXCKj5FeIsgVtfluPOsZjPzz
71QQWS1dAkEA0T0KXO8gaBQwJhIoo/w6hy5JGZnrNSpOPp5xvJuMAafs2eyvmhJm
Ev9SN/Pf2VYa1z6FEnBaLOVD6hf6YQIsPQJAX/CZPoW6dzwgvimo1/GcY6eleiWE
qygqjWhsh71e/3bz7yuEAnj5yE3t7Zshcp+dXR3xxGo0eSuLfLFxHgGxwQJAAxf8
9DzQ5NkPkTCJi0sqbl8/03IUKTgT6hcbpWdDXa7m8J3wRr3o5nUB+TPQ5nzAbthM
zWX931YQeACcwhxvHQJBAN5mTzzJD4w4Ma6YTaNHyXakdYfyAWrOkPIWZxfhMfXe
DrlNdiysTI4Dd1dLeErVpjsckAaOW/JDG5PCSwkaMxk=
-----END RSA PRIVATE KEY-----";
        }

        private void Example3_Click(object sender, RoutedEventArgs e)
        {
            mParameter.CACertificate = @"-----BEGIN CERTIFICATE-----
MIID3zCCAsegAwIBAgIJAN8DgpVccyUgMA0GCSqGSIb3DQEBBQUAMFIxCzAJBgNV
BAYTAkNOMREwDwYDVQQIEwhaaGVqaWFuZzERMA8GA1UEBxMISGFuZ3pob3UxDzAN
BgNVBAoTBkFsaXl1bjEMMAoGA1UECxMDU0xCMCAXDTE2MDYwMTA5Mzk0MFoYDzMw
MTUxMDAzMDkzOTQwWjBSMQswCQYDVQQGEwJDTjERMA8GA1UECBMIWmhlamlhbmcx
ETAPBgNVBAcTCEhhbmd6aG91MQ8wDQYDVQQKEwZBbGl5dW4xDDAKBgNVBAsTA1NM
QjCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALtBUHUlLckNMIhpc5Aj
PVozeNw45Hwx2JAnIOor2pjpN/fUUjN9sCOdeTI5mjE5bSLIRh6EXhCsnPmkWbEQ
Gy6jRwrYrlr4ki8pGBwZtTu3kJdm4S9XDH+WBHH9bMM06fz2qoNEuhv5oN/dmTve
Lw1Ml6YfHkP7+68kbk3cqHDP4cNviWygWoNh76Y8Gt5BUGYP6POxY0nYfdrZ7qvc
9ReQC9djEzQwlKAR20XkR5Jg8iTPOHy3NuFAIzIAx7qH/1/ueIPdDvj8I0K4R3Mi
PKJvfxE4XA1v0l6ASHsQ2bA8HZAjR2DqhAivbgTW5IOupjvl75sE+pLmFr81jL1m
nq8CAwEAAaOBtTCBsjAdBgNVHQ4EFgQUTd2UNlxkwchdw7kd15V244I6xpUwgYIG
A1UdIwR7MHmAFE3dlDZcZMHIXcO5HdeVduOCOsaVoVakVDBSMQswCQYDVQQGEwJD
TjERMA8GA1UECBMIWmhlamlhbmcxETAPBgNVBAcTCEhhbmd6aG91MQ8wDQYDVQQK
EwZBbGl5dW4xDDAKBgNVBAsTA1NMQoIJAN8DgpVccyUgMAwGA1UdEwQFMAMBAf8w
DQYJKoZIhvcNAQEFBQADggEBABX5haA+IAmw0Z6rj0g7d6AY8qYjSzGtX+SffTgS
Gtn1kgzb0Yh7smdOgiyaY4jq0xD8/igzL3sylxt6zGwL87BcJ72OxDRiz1i/JPd1
KXWCySvsgK4tPF43UCrrw39PYnKl5TGaIkO5voCKf6NlhZmHfklN5FKk86WbPbHb
87ZFkfom9zdeGgLAGij2vFKXLWf/EcehhNzGy5ubEGrQBP669kxWeW4OWTiee8T0
bUNu0bNJb6t6DMXo7a/mlYgK02BRaGtjYuiaVcljAKMiH1oCAknoIspKpEGjKCoo
jjkf3CP4H6Hwyn6lg2yPOUM3gjc/6OjHOfuqBsJMIR5oOZo=
-----END CERTIFICATE-----";
        }
    }

    public class CertificateRegion
    {
        public string LocalName { get; set; }
        public string RegionId { get; set; }
        public bool IsCheck { get; set; }
    }

    public class CreateCertParameter : INotifyPropertyChanged
    {
        public string CertName { get; set; }
        private bool _IsServer;
        public bool IsServer
        {
            get { return _IsServer; }
            set
            {
                _IsServer = value;
                NotifyPropertyChanged("IsServer");
            }
        }
        private bool _IsCA;
        public bool IsCA
        {
            get { return _IsCA; }
            set
            {
                _IsCA = value;
                NotifyPropertyChanged("IsCA");
            }
        }
        public string _ServerCertificate;
        public string ServerCertificate
        {
            get { return _ServerCertificate; }
            set
            {
                _ServerCertificate = value;
                NotifyPropertyChanged("ServerCertificate");
            }
        }
        public string privatekey;
        public string PrivateKey
        {
            get { return privatekey; }
            set
            {
                privatekey = value;
                NotifyPropertyChanged("PrivateKey");
            }
        }
        public string _CACertificate;
        public string CACertificate
        {
            get { return _CACertificate; }
            set
            {
                _CACertificate = value;
                NotifyPropertyChanged("CACertificate");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
