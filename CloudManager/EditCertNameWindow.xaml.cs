using Aliyun.Acs.Core;
using Aliyun.Acs.Slb.Model.V20140515;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace CloudManager
{
    /// <summary>
    /// EditCertNameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditCertNameWindow : WindowBase
    {
        private DefaultAcsClient mClient;
        private DescribeCertificate mCertificate;

        public EventHandler<string> UpdateEventHandler;
        public delegate void DelegateGot(object obj);

        public EditCertNameWindow(DefaultAcsClient c, DescribeCertificate cert)
        {
            InitializeComponent();

            mClient = c;
            mCertificate = cert;
            this.DataContext = mCertificate;
        }

        private void SetNameSuccess(object obj)
        {
            string name = obj as string;
            UpdateEventHandler?.Invoke(mCertificate, name);
            this.Close();
        }

        private void SetNameFail()
        {
        }

        private void SetCertificateName(object obj)
        {
            string name = obj as string;
            try
            {
                if (mCertificate.CertificateType.Equals("ServerCertificate"))
                {
                    SetServerCertificateNameRequest request = new SetServerCertificateNameRequest();
                    request.ServerCertificateId = mCertificate.CertificateId;
                    request.ServerCertificateName = name;
                    SetServerCertificateNameResponse response = mClient.GetAcsResponse(request);
                }
                else if (mCertificate.CertificateType.Equals("CACertificate"))
                {
                    SetCACertificateNameRequest request = new SetCACertificateNameRequest();
                    request.CACertificateId = mCertificate.CertificateId;
                    request.CACertificateName = name;
                    SetCACertificateNameResponse response = mClient.GetAcsResponse(request);
                }
                Dispatcher.Invoke(new DelegateGot(SetNameSuccess), name);
            }
            catch
            {
                Dispatcher.Invoke(new Action(SetNameFail));
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            string name = String.Copy(CertificateName.Text);
            Thread t = new Thread(new ParameterizedThreadStart(SetCertificateName));
            t.Start(name);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
