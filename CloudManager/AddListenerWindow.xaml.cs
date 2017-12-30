using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Slb.Model.V20140515;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static Aliyun.Acs.Slb.Model.V20140515.DescribeMasterSlaveServerGroupsResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeVServerGroupsResponse;

namespace CloudManager
{
    /// <summary>
    /// AddListenerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddListenerWindow : WindowBase
    {
        private AddListenerParams mParams = new AddListenerParams();
        private DefaultAcsClient mClient;
        private ListenerBasePage mBasePage;
        private ListenerHealthCheckPage mHealthPage;
        private ListenerSubmmitPage mSubmmitPage;
        private bool mUpdateListeners;

        public ObservableCollection<ServerGroup> mVServerGroups { get; set; } //Must
        public ObservableCollection<ServerGroup> mMServerGroups { get; set; }
        public delegate void DelegateGot(object obj);
        public bool UpdateListeners { get { return mUpdateListeners; } }

        public AddListenerWindow(DescribeLoadBalancer balancer)
        {
            InitListenerParams(balancer);
            InitializeComponent();
            IClientProfile profile = DefaultProfile.GetProfile(balancer.RegionId, App.AKI, App.AKS);
            mClient = new DefaultAcsClient(profile);
        }

        public AddListenerWindow(DescribeLoadBalancer balancer, SLBListener listener)
        {
            InitListenerParams(balancer, listener);
            InitializeComponent();
            IClientProfile profile = DefaultProfile.GetProfile(balancer.RegionId, App.AKI, App.AKS);
            mClient = new DefaultAcsClient(profile);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            mBasePage = new ListenerBasePage(mClient, mParams);
            mBasePage.mVServerGroups = mVServerGroups;
            mBasePage.mMServerGroups = mMServerGroups;
            mBasePage.mOwner = this;
            PageContent.Navigate(mBasePage);
        }

        private void InitListenerParams(DescribeLoadBalancer balancer)
        {
            mParams.RegionId = balancer.RegionId;
            mParams.LoadBalancerId = balancer.LoadBalancerId;
            mParams.AddListener = true;
            mParams.Protocol = "TCP";
            mParams.Scheduler = "wrr";
            mParams.AutoStart = true;
            mParams.PersistenceTimeout = 1000;
            mParams.EstablishedTimeout = 900;
            mParams.StickySessionType = "insert";
            mParams.Gzip = true;
            mParams.XForwardedFor = true;
            mParams.TCPCheck = true;
            mParams.HealthCheckURI = "/";
            mParams.http_2xx = true;
            mParams.http_3xx = true;
            mParams.HealthCheckTimeout = 5;
            mParams.HealthCheckInterval = 2;
            mParams.UnhealthyThreshold = 3;
            mParams.HealthyThreshold = 3;
        }

        private void InitListenerParams(DescribeLoadBalancer balancer, SLBListener listener)
        {
            mParams.RegionId = balancer.RegionId;
            mParams.LoadBalancerId = balancer.LoadBalancerId;
            mParams.ConfigureListener = true;
            mParams.Protocol = listener.Protocol.ToUpper();
            mParams.ListenerPort = listener.ListenerPort;
            mParams.BackendServerPort = listener.BackendServerPort;
            mParams.BandWidth = listener.Bandwidth;
            mParams.Scheduler = listener.Scheduler;
            if (mParams.Protocol.Equals("TCP")
                || mParams.Protocol.Equals("UDP"))
            {
                if (listener.VServerGroupId != null)
                {
                    mParams.UseServerGroup = true;
                    mParams.VServerGroup = true;
                    mParams.ServerGroupId = listener.VServerGroupId;
                }
                else if (listener.MasterSlaveServerGroupId != null)
                {
                    mParams.UseServerGroup = true;
                    mParams.MServerGroup = true;
                    mParams.ServerGroupId = listener.MasterSlaveServerGroupId;
                }

                if (listener.PersistenceTimeout > 0)
                {
                    mParams.KeepSession = true;
                    mParams.PersistenceTimeout = listener.PersistenceTimeout;
                }
                else
                {
                    mParams.KeepSession = false;
                    mParams.PersistenceTimeout = 1000;
                }
                
                if (mParams.Protocol.Equals("TCP"))
                {
                    mParams.EstablishedTimeout = listener.EstablishedTimeout;
                    if (listener.HealthCheckType.ToUpper().Equals("TCP"))
                    {
                        mParams.TCPCheck = true;
                    }
                    else if (listener.HealthCheckType.ToUpper().Equals("HTTP"))
                    {
                        mParams.HTTPCheck = true;
                    }
                }
            }
            else if (mParams.Protocol.Equals("HTTP")
                || mParams.Protocol.Equals("HTTPS"))
            {
                if (listener.VServerGroupId != null)
                {
                    mParams.UseVServerGroup = true;
                    mParams.ServerGroupId = listener.VServerGroupId;
                }

                if (mParams.Protocol.Equals("HTTPS"))
                {
                    mParams.ServerCertificateId = listener.ServerCertificateId;
                    if (listener.CACertificateId != null)
                    {
                        mParams.UseTwoWayAuth = true;
                        mParams.CACertificateId = listener.CACertificateId;
                    }
                }

                if (listener.StickySession.Equals("on"))
                {
                    mParams.StickySession = true;
                    mParams.StickySessionType = listener.StickySessionType;
                    mParams.Cookie = listener.Cookie;
                    mParams.CookieTimeout = listener.CookieTimeout;
                }

                if (listener.Gzip.Equals("on"))
                {
                    mParams.Gzip = true;
                }

                if (listener.XForwardedFor.Equals("on"))
                {
                    mParams.XForwardedFor = true;
                }

                if (listener.XForwardedFor_SLBID.Equals("on"))
                {
                    mParams.XForwardedFor_SLBID = true;
                }

                if (listener.XForwardedFor_SLBIP.Equals("on"))
                {
                    mParams.XForwardedFor_SLBIP = true;
                }

                if (listener.XForwardedFor_proto.Equals("on"))
                {
                    mParams.XForwardedFor_proto = true;
                }

                if (listener.HealthCheck.Equals("on"))
                {
                    mParams.HTTPCheckOn = true;
                }
            }

            mParams.HealthCheckPortStr = listener.HealthCheckConnectPort.ToString();

            if (mParams.HTTPCheck || mParams.HTTPCheckOn)
            {
                mParams.HealthCheckDomain = listener.HealthCheckDomain;
                mParams.HealthCheckURI = listener.HealthCheckURI;
                if (listener.HealthCheckHttpCode.Contains("http_2xx"))
                {
                    mParams.http_2xx = true;
                }
                if (listener.HealthCheckHttpCode.Contains("http_3xx"))
                {
                    mParams.http_3xx = true;
                }
                if (listener.HealthCheckHttpCode.Contains("http_4xx"))
                {
                    mParams.http_4xx = true;
                }
                if (listener.HealthCheckHttpCode.Contains("http_5xx"))
                {
                    mParams.http_5xx = true;
                }
            }
            else
            {
                mParams.HealthCheckURI = "/";
                mParams.http_2xx = true;
                mParams.http_3xx = true;
            }

            if (listener.HealthCheckTimeout != null)
            {
                mParams.HealthCheckTimeout = listener.HealthCheckTimeout;
            }
            else
            {
                mParams.HealthCheckTimeout = 5;
            }

            if (listener.HealthCheckInterval != null)
            {
                mParams.HealthCheckInterval = listener.HealthCheckInterval;
            }
            else
            {
                mParams.HealthCheckInterval = 2;
            }

            if (listener.UnhealthyThreshold != null)
            {
                mParams.UnhealthyThreshold = listener.UnhealthyThreshold;
            }
            else
            {
                mParams.UnhealthyThreshold = 3;
            }

            if (listener.HealthyThreshold != null)
            {
                mParams.HealthyThreshold = listener.HealthyThreshold;
            }
            else
            {
                mParams.HealthyThreshold = 3;
            }

            if (mParams.Protocol.Equals("UDP"))
            {
                mParams.HealthCheckReq = listener.HealthCheckReq;
                mParams.HealthCheckExp = listener.HealthCheckExp;
            }
        }

        public void NextPage()
        {
            if (mHealthPage == null)
            {
                mHealthPage = new ListenerHealthCheckPage(mParams);
                mHealthPage.mOwner = this;
            }
            PageContent.Navigate(mHealthPage);
        }

        public void PreviousPage()
        {
            PageContent.Navigate(mBasePage);
        }

        public void Ensure()
        {
            if (mSubmmitPage == null)
            {
                mSubmmitPage = new ListenerSubmmitPage(mClient, mParams);
                mSubmmitPage.mOwner = this;
            }
            PageContent.Navigate(mSubmmitPage);
        }

        public void Cancel()
        {
            this.Close();
        }

        public void NotifyUpdate()
        {
            mUpdateListeners = true;
        }
    }

    class SchedulerType
    {
        public string SchedulerId { get; set; }
        public string SchedulerName { get; set; }
    }

    class StickySessionId
    {
        public string StickySessionType { get; set; }
        public string StickySessionName { get; set; }
    }

    public class ServerGroup
    {
        public string ServerGroupId { get; set; }
        public string ServerGroupName { get; set; }
    }
}
