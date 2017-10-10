using Aliyun.Acs.Core;
using Aliyun.Acs.Slb.Model.V20140515;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static Aliyun.Acs.Slb.Model.V20140515.DescribeCACertificatesResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeMasterSlaveServerGroupsResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeServerCertificatesResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeVServerGroupsResponse;

namespace CloudManager
{
    /// <summary>
    /// ListenerBasePage.xaml 的交互逻辑
    /// </summary>
    public partial class ListenerBasePage : Page
    {
        private AddListenerParams mParams;
        private DefaultAcsClient mClient;
        
        private ObservableCollection<DescribeServerCertificates_ServerCertificate> mServerCertificates;
        private ObservableCollection<DescribeCACertificates_CACertificate> mCACertificates;

        public AddListenerWindow mOwner { get; set; }
        public delegate void DelegateGot(object obj);

        private ObservableCollection<ServerGroup> _mVServerGroups;
        public ObservableCollection<ServerGroup> mVServerGroups
        {
            get { return _mVServerGroups; }
            set
            {
                _mVServerGroups = value;
                if (mParams.VServerGroup)
                {
                    ServerGroupId.ItemsSource = mVServerGroups;
                    ServerGroupId.SelectedIndex = 0;
                }
                if (mParams.UseVServerGroup)
                {
                    VServerGroupId.ItemsSource = mVServerGroups;
                    VServerGroupId.SelectedIndex = 0;
                }
            }
        }

        private ObservableCollection<ServerGroup> _mMServerGroups;
        public ObservableCollection<ServerGroup> mMServerGroups
        {
            get { return _mMServerGroups; }
            set
            {
                _mMServerGroups = value;
                if (mParams.MServerGroup)
                {
                    ServerGroupId.ItemsSource = mMServerGroups;
                    ServerGroupId.SelectedIndex = 0;
                }
            }
        }

        public ListenerBasePage(DefaultAcsClient c, AddListenerParams p)
        {
            InitializeComponent();
            mClient = c;
            mParams = p;
            DataContext = mParams;
            PreLoadSources();
        }

        private void PreLoadSources()
        {
            Thread t3 = new Thread(GetCertificates);
            t3.Start();
        }

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            mParams.ConfigureBand = !mParams.ConfigureBand;
        }

        private void Digital_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void GotServerCertificates(object obj)
        {
            mServerCertificates = obj as ObservableCollection<DescribeServerCertificates_ServerCertificate>;
            ServerCertificateId.ItemsSource = mServerCertificates;
            ServerCertificateId.SelectedIndex = 0;
        }

        private void GotCACertificates(object obj)
        {
            mCACertificates = obj as ObservableCollection<DescribeCACertificates_CACertificate>;
            CACertificateId.ItemsSource = mCACertificates;
            CACertificateId.SelectedIndex = 0;
        }

        private void GetCertificates()
        {
            DescribeServerCertificatesRequest r1 = new DescribeServerCertificatesRequest();
            try
            {
                DescribeServerCertificatesResponse s1 = mClient.GetAcsResponse(r1);
                ObservableCollection<DescribeServerCertificates_ServerCertificate> cs = new ObservableCollection<DescribeServerCertificates_ServerCertificate>();
                foreach (DescribeServerCertificates_ServerCertificate c in s1.ServerCertificates)
                {
                    cs.Add(c);
                }
                Dispatcher.Invoke(new DelegateGot(GotServerCertificates), cs);
            }
            catch
            {

            }

            DescribeCACertificatesRequest r2 = new DescribeCACertificatesRequest();
            try
            {
                DescribeCACertificatesResponse s2 = mClient.GetAcsResponse(r2);       
                ObservableCollection<DescribeCACertificates_CACertificate> cs = new ObservableCollection<DescribeCACertificates_CACertificate>();
                foreach (DescribeCACertificates_CACertificate c in s2.CACertificates)
                {
                    cs.Add(c);
                }
                Dispatcher.Invoke(new DelegateGot(GotCACertificates), cs);
            }
            catch
            { 
            } 
        }

        private void VServeGroup_Checked(object sender, RoutedEventArgs e)
        {
            if (mVServerGroups != null)
            {
                ServerGroupId.ItemsSource = mVServerGroups;
                ServerGroupId.SelectedIndex = 0;
            }
        }

        private void MServerGroup_Checked(object sender, RoutedEventArgs e)
        {
            if (mMServerGroups != null)
            {
                ServerGroupId.ItemsSource = mMServerGroups;
                ServerGroupId.SelectedIndex = 0;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (mOwner != null)
            {
                mOwner.NextPage();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (mOwner != null)
            {
                mOwner.Cancel();
            }
        }

        private void UseVServerGroup_Checked(object sender, RoutedEventArgs e)
        {
            if (mVServerGroups != null)
            {
                VServerGroupId.ItemsSource = mVServerGroups;
                VServerGroupId.SelectedIndex = 0;
            }
        }
    }
}
