using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Ecs.Model.V20140526;
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
using static Aliyun.Acs.Slb.Model.V20140515.DescribeLoadBalancerAttributeResponse;
using static Aliyun.Acs.Slb.Model.V20140515.RemoveBackendServersResponse;

namespace CloudManager
{
    /// <summary>
    /// SLBPage.xaml 的交互逻辑
    /// </summary>
    public partial class SLBPage : Page
    {
        private string mAki, mAks;

        public MainWindow mMainWindow { get; set; }

        public SLBPage()
        {
            InitializeComponent();
        }

        public SLBPage(string aki, string aks)
        {
            InitializeComponent();
            mAki = aki;
            mAks = aks;
        }

        private DescribeLoadBalancer _mBalancer;
        public DescribeLoadBalancer mBalancer
        {
            get { return _mBalancer; }
            set
            {
                _mBalancer = value;
                Information.DataContext = value;
            }
        }
    }
}
