using Aliyun.Acs.Slb.Model.V20140515;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace CloudManager
{
    /// <summary>
    /// AddListenerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddListenerWindow : Window
    {
        private AddListenerParams mParams = new AddListenerParams();
        public DescribeLoadBalancer mBalancer;

        public AddListenerWindow()
        {
            InitializeComponent();
            BindingDataContext();
        }

        public AddListenerWindow(DescribeLoadBalancer balancer)
        {
            InitializeComponent();
            BindingDataContext();
            mBalancer = balancer;
        }

        private void BindingDataContext()
        {
            this.DataContext = mParams;
            mParams.RegionId = mBalancer.RegionId;
            mParams.LoadBalancerId = mBalancer.LoadBalancerId;
            mParams.AutoStart = true;
            mParams.ShowGroup = Visibility.Collapsed;
            mParams.ShowKeppSession = Visibility.Collapsed;
            mParams.ShowUnlimit = Visibility.Visible;
            mParams.ShowConfigure = Visibility.Collapsed;
        }

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            mParams.ConfigureBand = !mParams.ConfigureBand;
            if (mParams.ConfigureBand)
            {
                mParams.ShowUnlimit = Visibility.Collapsed;
                mParams.ShowConfigure = Visibility.Visible;
                Configure.Content = "取消";
            }
            else
            {
                mParams.ShowUnlimit = Visibility.Visible;
                mParams.ShowConfigure = Visibility.Collapsed;
                Configure.Content = "配置";
            }
        }

        private void DoAddListener(object obj)
        {
            AddListenerParams para = obj as AddListenerParams;
            switch (para.Protocol)
            {
                case "TCP":
                    int value;
                    CreateLoadBalancerTCPListenerRequest request = new CreateLoadBalancerTCPListenerRequest();
                    request.LoadBalancerId = para.LoadBalancerId;
                    request.RegionId = para.RegionId; 
                    int.TryParse(para.ForPort, out value);
                    request.ListenerPort = value;
                    int.TryParse(para.BackPort, out value);
                    request.BackendServerPort = value;
                    if (para.ConfigureBand)
                    {
                        int.TryParse(para.BandWidth, out value);
                        request.Bandwidth = value;
                    }
                    else
                    {
                        request.Bandwidth = -1;
                    }
                    request.Scheduler = para.Scheduler;
                    if (para.UseServerGroup)
                    {
                        if (para.MServerGroup)
                        {
                            request.MasterSlaveServerGroupId = para.ServerGroupId;
                        }
                        else if (para.VServerGroup)
                        {
                            request.VServerGroupId = para.ServerGroupId;
                        }
                    }
                    int.TryParse(para.PersistenceTimeout, out value);
                    request.PersistenceTimeout = value;
                    int.TryParse(para.EstablishedTimeout, out value);
                    request.EstablishedTimeout = value;
                    break;

                case "HTTP":
                    break;
                case "HTTPS":
                    break;
                case "UDP":
                    break;
                default:
                    return;
            }
        }

        private void AddListener_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(DoAddListener));
            t.Start(mParams);
        }

        private void UseServerGroup_Checked(object sender, RoutedEventArgs e)
        {
            mParams.ShowGroup = Visibility.Visible;
        }

        private void UseServerGroup_Unchecked(object sender, RoutedEventArgs e)
        {
            mParams.ShowGroup = Visibility.Collapsed;
        }

        private void KeepSession_Checked(object sender, RoutedEventArgs e)
        {
            mParams.ShowKeppSession = Visibility.Visible;
        }

        private void KeepSession_Unchecked(object sender, RoutedEventArgs e)
        {
            mParams.ShowKeppSession = Visibility.Collapsed;
        }

        private void Digital_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void VServeGroup_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void MServerGroup_Checked(object sender, RoutedEventArgs e)
        {

        }
    }

    class Schedulers
    {
        public string SchedulerId { get; set; }
        public string SchedulerName { get; set; }
    }

    class AddListenerParams : INotifyPropertyChanged
    {
        public string RegionId { get; set; }

        public string LoadBalancerId { get; set; }

        private string _Protocol;
        public string Protocol
        {
            get { return _Protocol; }
            set
            {
                _Protocol = value;
                NotifyPropertyChanged("Protocol");
            }
        }

        private string _ForPort;
        public string ForPort
        {
            get { return _ForPort; }
            set
            {
                _ForPort = value;
                NotifyPropertyChanged("ForPort");
            }
        }

        private string _BackPort;
        public string BackPort
        {
            get { return _BackPort; }
            set
            {
                _BackPort = value;
                NotifyPropertyChanged("BackPort");
            }
        }

        private string _BandWidth;
        public string BandWidth
        {
            get { return _BandWidth; }
            set
            {
                _BandWidth = value;
                NotifyPropertyChanged("BandWidth");
            }
        }

        private bool _ConfigureBand;
        public bool ConfigureBand
        {
            get { return _ConfigureBand; }
            set
            {
                _ConfigureBand = value;
                NotifyPropertyChanged("ConfigureBand");
            }
        }

        private Visibility _ShowUnlimit;
        public Visibility ShowUnlimit
        {
            get { return _ShowUnlimit; }
            set
            {
                _ShowUnlimit = value;
                NotifyPropertyChanged("ShowUnlimit");
            }
        }

        private Visibility _ShowConfigure;
        public Visibility ShowConfigure
        {
            get { return _ShowConfigure; }
            set
            {
                _ShowConfigure = value;
                NotifyPropertyChanged("ShowConfigure");
            }
        }

        private string _Scheduler;
        public string Scheduler
        {
            get { return _Scheduler; }
            set
            {
                _Scheduler = value;
                NotifyPropertyChanged("Scheduler");
            }
        }

        private bool _UseServerGroup;
        public bool UseServerGroup
        {
            get { return _UseServerGroup; }
            set
            {
                _UseServerGroup = value;
                NotifyPropertyChanged("UseServerGroup");
            }
        }

        private Visibility _ShowGroup;
        public Visibility ShowGroup
        {
            get { return _ShowGroup; }
            set
            {
                _ShowGroup = value;
                NotifyPropertyChanged("ShowGroup");
            }
        }

        private bool _VServerGroup;
        public bool VServerGroup
        {
            get { return _VServerGroup; }
            set
            {
                _VServerGroup = value;
                NotifyPropertyChanged("VServerGroup");
            }
        }

        private bool _MServerGroup;
        public bool MServerGroup
        {
            get { return _MServerGroup; }
            set
            {
                _MServerGroup = value;
                NotifyPropertyChanged("MServerGroup");
            }
        }

        private string _ServerGroupId;
        public string ServerGroupId
        {
            get { return _ServerGroupId; }
            set
            {
                _ServerGroupId = value;
                NotifyPropertyChanged("ServerGroupId");
            }
        }
 
        private bool _AutoStart;
        public bool AutoStart
        {
            get { return _AutoStart; }
            set
            {
                _AutoStart = value;
                NotifyPropertyChanged("AutoStart");
            }
        }

        private bool _KeepSession;
        public bool KeepSession
        {
            get { return _KeepSession; }
            set
            {
                _KeepSession = value;
                NotifyPropertyChanged("KeepSession");
            }
        }

        private Visibility _ShowKeppSession;
        public Visibility ShowKeppSession
        {
            get { return _ShowKeppSession; }
            set
            {
                _ShowKeppSession = value;
                NotifyPropertyChanged("ShowKeppSession");
            }
        }

        private string _PersistenceTimeout;
        public string PersistenceTimeout
        {
            get { return _PersistenceTimeout; }
            set
            {
                _PersistenceTimeout = value;
                NotifyPropertyChanged("PersistenceTimeout");
            }
        }

        private string _EstablishedTimeout;
        public string EstablishedTimeout
        {
            get { return _EstablishedTimeout; }
            set
            {
                _EstablishedTimeout = value;
                NotifyPropertyChanged("EstablishedTimeout");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
