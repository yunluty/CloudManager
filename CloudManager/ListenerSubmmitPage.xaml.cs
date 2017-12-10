using Aliyun.Acs.Core;
using Aliyun.Acs.Slb.Model.V20140515;
using System;
using System.Collections.Generic;
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

namespace CloudManager
{
    /// <summary>
    /// ListenerSubmmitPage.xaml 的交互逻辑
    /// </summary>
    public partial class ListenerSubmmitPage : Page
    {
        private DefaultAcsClient mClient;
        private AddListenerParams mParams;

        public AddListenerWindow mOwner { get; set; }


        public ListenerSubmmitPage()
        {
            InitializeComponent();
        }

        public ListenerSubmmitPage(DefaultAcsClient c, AddListenerParams p)
        {
            InitializeComponent();
            mClient = c;
            mParams = p;
            DataContext = p;
            Submmit();
        }

        private void CreatedListener()
        {
            mParams.AddListenerStatus = "success";
            if (!mParams.AutoStart)
            {
                AllSuccess();
            }
        }

        private void CreatedListenerError()
        {
            mParams.AddListenerStatus = "error";
            mParams.StartListenerStatus = "error";
            mParams.AllStatus = "error";
        }

        private void StartedListener()
        {
            mParams.StartListenerStatus = "success";
            AllSuccess();
        }

        private void StartedListenerError()
        {
            mParams.StartListenerStatus = "error";
            mParams.AllStatus = "error";
        }

        private void AllSuccess()
        {
            mParams.AllStatus = "success";
            mOwner.NotifyUpdate();
        }

        private void DoAddListener(object obj)
        {
            AddListenerParams para = obj as AddListenerParams;

            try
            {
                switch (para.Protocol)
                {
                    case "TCP":
                        {
                            CreateLoadBalancerTCPListenerRequest request = new CreateLoadBalancerTCPListenerRequest();
                            request.LoadBalancerId = para.LoadBalancerId;
                            request.ListenerPort = para.ListenerPort;
                            request.BackendServerPort = para.BackendServerPort;

                            if (para.ConfigureBand)
                            {
                                request.Bandwidth = para.BandWidth;
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

                            if (para.KeepSession)
                            {
                                request.PersistenceTimeout = para.PersistenceTimeout;
                            }

                            request.EstablishedTimeout = para.EstablishedTimeout;
                            if (para.TCPCheck)
                            {
                                request.HealthCheckType = "tcp";
                                request.HealthCheckConnectPort = para.HealthCheckConnectPort;
                            }
                            else if (para.HTTPCheck)
                            {
                                request.HealthCheckType = "http";
                                request.HealthCheckDomain = para.HealthCheckDomain;
                                request.HealthCheckConnectPort = para.HealthCheckConnectPort;
                                request.HealthCheckURI = para.HealthCheckURI;
                                if (mParams.http_2xx)
                                {
                                    request.HealthCheckHttpCode = "http_2xx";
                                }
                                if (mParams.http_3xx)
                                {
                                    request.HealthCheckHttpCode += ",http_3xx";
                                }
                                if (mParams.http_4xx)
                                {
                                    request.HealthCheckHttpCode = ",http_4xx";
                                }
                                if (mParams.http_5xx)
                                {
                                    request.HealthCheckHttpCode = ",http_5xx";
                                }
                            }
                            request.HealthCheckConnectTimeout = para.HealthCheckTimeout;
                            request.HealthCheckInterval = para.HealthCheckInterval;
                            request.UnhealthyThreshold = para.UnhealthyThreshold;
                            request.HealthyThreshold = para.HealthyThreshold;
                            CreateLoadBalancerTCPListenerResponse response = mClient.GetAcsResponse(request);
                        }
                        break;

                    case "HTTP":
                        {
                            CreateLoadBalancerHTTPListenerRequest request = new CreateLoadBalancerHTTPListenerRequest();
                            request.LoadBalancerId = para.LoadBalancerId;
                            request.ListenerPort = para.ListenerPort;
                            request.BackendServerPort = para.BackendServerPort;

                            if (para.ConfigureBand)
                            {
                                request.Bandwidth = para.BandWidth;
                            }
                            else
                            {
                                request.Bandwidth = -1;
                            }

                            request.Scheduler = para.Scheduler;

                            if (para.UseVServerGroup)
                            {
                                request.VServerGroupId = para.ServerGroupId;
                            }

                            if (para.StickySession)
                            {
                                request.StickySession = "on";
                                request.StickySessionType = para.StickySessionType;
                                if (para.StickySessionType.Equals("insert"))
                                {
                                    request.CookieTimeout = para.CookieTimeout;
                                }
                                else if (para.StickySessionType.Equals("server"))
                                {
                                    request.Cookie = para.Cookie;
                                }
                            }
                            else
                            {
                                request.StickySession = "off";
                            }

                            if (para.Gzip)
                            {
                                request.Gzip = "on";
                            }
                            else
                            {
                                request.Gzip = "off";
                            }

                            if (para.XForwardedFor)
                            {
                                request.XForwardedFor = "on";
                            }

                            if (para.XForwardedFor_SLBID)
                            {
                                request.XForwardedFor_SLBID = "on";
                            }

                            if (para.XForwardedFor_SLBIP)
                            {
                                request.XForwardedFor_SLBIP = "on";
                            }

                            if (para.XForwardedFor_proto)
                            {
                                request.XForwardedFor_proto = "on";
                            }

                            if (para.HTTPCheckOn)
                            {
                                request.HealthCheck = "on";
                                request.HealthCheckDomain = para.HealthCheckDomain;
                                request.HealthCheckConnectPort = para.HealthCheckConnectPort;
                                request.HealthCheckURI = para.HealthCheckURI;
                                if (mParams.http_2xx)
                                {
                                    request.HealthCheckHttpCode = "http_2xx";
                                }
                                if (mParams.http_3xx)
                                {
                                    request.HealthCheckHttpCode += ",http_3xx";
                                }
                                if (mParams.http_4xx)
                                {
                                    request.HealthCheckHttpCode = ",http_4xx";
                                }
                                if (mParams.http_5xx)
                                {
                                    request.HealthCheckHttpCode = ",http_5xx";
                                }
                            }
                            else
                            {
                                request.HealthCheck = "off";
                            }
                            request.HealthCheckTimeout = para.HealthCheckTimeout;
                            request.HealthCheckInterval = para.HealthCheckInterval;
                            request.UnhealthyThreshold = para.UnhealthyThreshold;
                            request.HealthyThreshold = para.HealthyThreshold;
                            CreateLoadBalancerHTTPListenerResponse response = mClient.GetAcsResponse(request);
                        }
                        break;

                    case "HTTPS":
                        {
                            CreateLoadBalancerHTTPSListenerRequest request = new CreateLoadBalancerHTTPSListenerRequest();
                            request.LoadBalancerId = para.LoadBalancerId;
                            request.ListenerPort = para.ListenerPort;
                            request.BackendServerPort = para.BackendServerPort;

                            if (para.ConfigureBand)
                            {
                                request.Bandwidth = para.BandWidth;
                            }
                            else
                            {
                                request.Bandwidth = -1;
                            }

                            request.Scheduler = para.Scheduler;

                            if (para.UseVServerGroup)
                            {
                                request.VServerGroupId = para.ServerGroupId;
                            }

                            request.ServerCertificateId = para.ServerCertificateId;

                            if (para.UseTwoWayAuth)
                            {
                                request.CACertificateId = para.CACertificateId;
                            }

                            if (para.StickySession)
                            {
                                request.StickySession = "on";
                                request.StickySessionType = para.StickySessionType;
                                if (para.StickySessionType.Equals("insert"))
                                {
                                    request.CookieTimeout = para.CookieTimeout;
                                }
                                else if (para.StickySessionType.Equals("server"))
                                {
                                    request.Cookie = para.Cookie;
                                }
                            }
                            else
                            {
                                request.StickySession = "off";
                            }

                            if (para.Gzip)
                            {
                                request.Gzip = "on";
                            }
                            else
                            {
                                request.Gzip = "off";
                            }

                            if (para.XForwardedFor)
                            {
                                request.XForwardedFor = "on";
                            }

                            if (para.XForwardedFor_SLBID)
                            {
                                request.XForwardedFor_SLBID = "on";
                            }

                            if (para.XForwardedFor_SLBIP)
                            {
                                request.XForwardedFor_SLBIP = "on";
                            }

                            if (para.XForwardedFor_proto)
                            {
                                request.XForwardedFor_proto = "on";
                            }

                            if (para.HTTPCheckOn)
                            {
                                request.HealthCheck = "on";
                                request.HealthCheckDomain = para.HealthCheckDomain;
                                request.HealthCheckConnectPort = para.HealthCheckConnectPort;
                                request.HealthCheckURI = para.HealthCheckURI;
                                if (mParams.http_2xx)
                                {
                                    request.HealthCheckHttpCode = "http_2xx";
                                }
                                if (mParams.http_3xx)
                                {
                                    request.HealthCheckHttpCode += ",http_3xx";
                                }
                                if (mParams.http_4xx)
                                {
                                    request.HealthCheckHttpCode = ",http_4xx";
                                }
                                if (mParams.http_5xx)
                                {
                                    request.HealthCheckHttpCode = ",http_5xx";
                                }
                            }
                            else
                            {
                                request.HealthCheck = "off";
                            }
                            request.HealthCheckTimeout = para.HealthCheckTimeout;
                            request.HealthCheckInterval = para.HealthCheckInterval;
                            request.UnhealthyThreshold = para.UnhealthyThreshold;
                            request.HealthyThreshold = para.HealthyThreshold;
                            CreateLoadBalancerHTTPSListenerResponse response = mClient.GetAcsResponse(request);
                        }
                        break;

                    case "UDP":
                        {
                            CreateLoadBalancerUDPListenerRequest request = new CreateLoadBalancerUDPListenerRequest();
                            request.LoadBalancerId = para.LoadBalancerId;
                            request.ListenerPort = para.ListenerPort;
                            request.BackendServerPort = para.BackendServerPort;

                            if (para.ConfigureBand)
                            {
                                request.Bandwidth = para.BandWidth;
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

                            if (para.KeepSession)
                            {
                                request.PersistenceTimeout = para.PersistenceTimeout;
                            }

                            request.HealthCheckConnectPort = para.HealthCheckConnectPort;
                            request.HealthCheckConnectTimeout = para.HealthCheckTimeout;
                            request.HealthCheckInterval = para.HealthCheckInterval;
                            request.UnhealthyThreshold = para.UnhealthyThreshold;
                            request.HealthyThreshold = para.HealthyThreshold;
                            request.HealthCheckReq = para.HealthCheckReq;
                            request.HealthCheckExp = para.HealthCheckExp;
                            CreateLoadBalancerUDPListenerResponse response = mClient.GetAcsResponse(request);
                        }
                        break;

                    default:
                        return;
                }

                Dispatcher.Invoke(new Action(CreatedListener));

                if (para.AutoStart)
                {
                    StartLoadBalancerListenerRequest start = new StartLoadBalancerListenerRequest();
                    start.LoadBalancerId = para.LoadBalancerId;
                    start.ListenerPort = para.ListenerPort;
                    try
                    {
                        StartLoadBalancerListenerResponse started = mClient.GetAcsResponse(start);
                        Dispatcher.Invoke(new Action(StartedListener));
                    }
                    catch
                    {
                        Dispatcher.Invoke(new Action(StartedListenerError));
                    }
                }
            }
            catch (Exception)
            {
                Dispatcher.Invoke(new Action(CreatedListenerError));
            }
        }

        private void ConfiguredListener()
        {
            mParams.ConfigureListenerStatus = "success";
            AllSuccess();
        }

        private void ConfiguredListenerError()
        {
            mParams.ConfigureListenerStatus = "error";
            mParams.AllStatus = "error";
        }

        private void DoConfigureListener(object obj)
        {
            AddListenerParams para = obj as AddListenerParams;
            try
            {
                switch (para.Protocol)
                {
                    case "TCP":
                        {
                            SetLoadBalancerTCPListenerAttributeRequest request = new SetLoadBalancerTCPListenerAttributeRequest();
                            request.LoadBalancerId = para.LoadBalancerId;
                            request.ListenerPort = para.ListenerPort;

                            if (para.ConfigureBand)
                            {
                                request.Bandwidth = para.BandWidth;
                            }
                            else
                            {
                                request.Bandwidth = -1;
                            }

                            request.Scheduler = para.Scheduler;

                            if (para.UseServerGroup)
                            {
                                if (para.MServerGroup && para.ServerGroupId != null)
                                {
                                    request.MasterSlaveServerGroup = "on";
                                    request.MasterSlaveServerGroupId = para.ServerGroupId;
                                }
                                else if (para.VServerGroup && para.ServerGroupId != null)
                                {
                                    request.VServerGroup = "on";
                                    request.VServerGroupId = para.ServerGroupId;
                                }
                                else
                                {
                                    request.MasterSlaveServerGroup = "off";
                                    request.VServerGroup = "off";
                                }
                            }
                            else
                            {
                                request.MasterSlaveServerGroup = "off";
                                request.VServerGroup = "off";
                            }

                            if (para.KeepSession)
                            {
                                request.PersistenceTimeout = para.PersistenceTimeout;
                            }

                            request.EstablishedTimeout = para.EstablishedTimeout;
                            if (para.TCPCheck)
                            {
                                request.HealthCheckType = "tcp";
                                request.HealthCheckConnectPort = para.HealthCheckConnectPort;
                            }
                            else if (para.HTTPCheck)
                            {
                                request.HealthCheckType = "http";
                                request.HealthCheckDomain = para.HealthCheckDomain;
                                request.HealthCheckConnectPort = para.HealthCheckConnectPort;
                                request.HealthCheckURI = para.HealthCheckURI;
                                if (mParams.http_2xx)
                                {
                                    request.HealthCheckHttpCode = "http_2xx";
                                }
                                if (mParams.http_3xx)
                                {
                                    request.HealthCheckHttpCode += ",http_3xx";
                                }
                                if (mParams.http_4xx)
                                {
                                    request.HealthCheckHttpCode += ",http_4xx";
                                }
                                if (mParams.http_5xx)
                                {
                                    request.HealthCheckHttpCode += ",http_5xx";
                                }
                            }
                            request.HealthCheckConnectTimeout = para.HealthCheckTimeout;
                            request.HealthCheckInterval = para.HealthCheckInterval;
                            request.UnhealthyThreshold = para.UnhealthyThreshold;
                            request.HealthyThreshold = para.HealthyThreshold;
                            SetLoadBalancerTCPListenerAttributeResponse response = mClient.GetAcsResponse(request);
                        }
                        break;

                    case "HTTP":
                        {
                            SetLoadBalancerHTTPListenerAttributeRequest request = new SetLoadBalancerHTTPListenerAttributeRequest();
                            request.LoadBalancerId = para.LoadBalancerId;
                            request.ListenerPort = para.ListenerPort;

                            if (para.ConfigureBand)
                            {
                                request.Bandwidth = para.BandWidth;
                            }
                            else
                            {
                                request.Bandwidth = -1;
                            }

                            request.Scheduler = para.Scheduler;

                            if (para.UseVServerGroup)
                            {
                                request.VServerGroupId = para.ServerGroupId;
                            }

                            if (para.StickySession)
                            {
                                request.StickySession = "on";
                                request.StickySessionType = para.StickySessionType;
                                if (para.StickySessionType.Equals("insert"))
                                {
                                    request.CookieTimeout = para.CookieTimeout;
                                }
                                else if (para.StickySessionType.Equals("server"))
                                {
                                    request.Cookie = para.Cookie;
                                }
                            }
                            else
                            {
                                request.StickySession = "off";
                            }

                            if (para.Gzip)
                            {
                                request.Gzip = "on";
                            }
                            else
                            {
                                request.Gzip = "off";
                            }

                            if (para.XForwardedFor)
                            {
                                request.XForwardedFor = "on";
                            }

                            if (para.XForwardedFor_SLBID)
                            {
                                request.XForwardedFor_SLBID = "on";
                            }

                            if (para.XForwardedFor_SLBIP)
                            {
                                request.XForwardedFor_SLBIP = "on";
                            }

                            if (para.XForwardedFor_proto)
                            {
                                request.XForwardedFor_proto = "on";
                            }

                            if (para.HTTPCheckOn)
                            {
                                request.HealthCheck = "on";
                                request.HealthCheckDomain = para.HealthCheckDomain;
                                request.HealthCheckConnectPort = para.HealthCheckConnectPort;
                                request.HealthCheckURI = para.HealthCheckURI;
                                if (mParams.http_2xx)
                                {
                                    request.HealthCheckHttpCode = "http_2xx";
                                }
                                if (mParams.http_3xx)
                                {
                                    request.HealthCheckHttpCode += ",http_3xx";
                                }
                                if (mParams.http_4xx)
                                {
                                    request.HealthCheckHttpCode += ",http_4xx";
                                }
                                if (mParams.http_5xx)
                                {
                                    request.HealthCheckHttpCode += ",http_5xx";
                                }
                            }
                            else
                            {
                                request.HealthCheck = "off";
                            }
                            request.HealthCheckTimeout = para.HealthCheckTimeout;
                            request.HealthCheckInterval = para.HealthCheckInterval;
                            request.UnhealthyThreshold = para.UnhealthyThreshold;
                            request.HealthyThreshold = para.HealthyThreshold;
                            SetLoadBalancerHTTPListenerAttributeResponse response = mClient.GetAcsResponse(request);
                        }
                        break;

                    case "HTTPS":
                        {
                            SetLoadBalancerHTTPSListenerAttributeRequest request = new SetLoadBalancerHTTPSListenerAttributeRequest();
                            request.LoadBalancerId = para.LoadBalancerId;
                            request.ListenerPort = para.ListenerPort;

                            if (para.ConfigureBand)
                            {
                                request.Bandwidth = para.BandWidth;
                            }
                            else
                            {
                                request.Bandwidth = -1;
                            }

                            request.Scheduler = para.Scheduler;

                            if (para.UseVServerGroup)
                            {
                                request.VServerGroupId = para.ServerGroupId;
                            }
                            else
                            {
                                request.VServerGroup = "off";
                            }

                            if(para.ServerCertificateId != null)
                            {
                                request.ServerCertificateId = para.ServerCertificateId;
                            }
                            else
                            {
                                request.ServerCertificateId = "";
                            }

                            if (para.UseTwoWayAuth && para.CACertificateId != null)
                            {
                                request.CACertificateId = para.CACertificateId;
                            }
                            else
                            {
                                request.CACertificateId = "";
                            }

                            if (para.StickySession)
                            {
                                request.StickySession = "on";
                                request.StickySessionType = para.StickySessionType;
                                if (para.StickySessionType.Equals("insert"))
                                {
                                    request.CookieTimeout = para.CookieTimeout;
                                }
                                else if (para.StickySessionType.Equals("server"))
                                {
                                    request.Cookie = para.Cookie;
                                }
                            }
                            else
                            {
                                request.StickySession = "off";
                            }

                            if (para.Gzip)
                            {
                                request.Gzip = "on";
                            }
                            else
                            {
                                request.Gzip = "off";
                            }

                            if (para.XForwardedFor)
                            {
                                request.XForwardedFor = "on";
                            }

                            if (para.XForwardedFor_SLBID)
                            {
                                request.XForwardedFor_SLBID = "on";
                            }

                            if (para.XForwardedFor_SLBIP)
                            {
                                request.XForwardedFor_SLBIP = "on";
                            }

                            if (para.XForwardedFor_proto)
                            {
                                request.XForwardedFor_proto = "on";
                            }

                            if (para.HTTPCheckOn)
                            {
                                request.HealthCheck = "on";
                                request.HealthCheckDomain = para.HealthCheckDomain;
                                request.HealthCheckConnectPort = para.HealthCheckConnectPort;
                                request.HealthCheckURI = para.HealthCheckURI;
                                if (mParams.http_2xx)
                                {
                                    request.HealthCheckHttpCode = "http_2xx";
                                }
                                if (mParams.http_3xx)
                                {
                                    request.HealthCheckHttpCode += ",http_3xx";
                                }
                                if (mParams.http_4xx)
                                {
                                    request.HealthCheckHttpCode += ",http_4xx";
                                }
                                if (mParams.http_5xx)
                                {
                                    request.HealthCheckHttpCode += ",http_5xx";
                                }
                            }
                            else
                            {
                                request.HealthCheck = "off";
                            }
                            request.HealthCheckTimeout = para.HealthCheckTimeout;
                            request.HealthCheckInterval = para.HealthCheckInterval;
                            request.UnhealthyThreshold = para.UnhealthyThreshold;
                            request.HealthyThreshold = para.HealthyThreshold;
                            SetLoadBalancerHTTPSListenerAttributeResponse response = mClient.GetAcsResponse(request);
                        }
                        break;

                    case "UDP":
                        {
                            SetLoadBalancerUDPListenerAttributeRequest request = new SetLoadBalancerUDPListenerAttributeRequest();
                            request.LoadBalancerId = para.LoadBalancerId;
                            request.ListenerPort = para.ListenerPort;

                            if (para.ConfigureBand)
                            {
                                request.Bandwidth = para.BandWidth;
                            }
                            else
                            {
                                request.Bandwidth = -1;
                            }

                            request.Scheduler = para.Scheduler;

                            if (para.UseServerGroup)
                            {
                                if (para.MServerGroup && para.ServerGroupId != null)
                                {
                                    request.MasterSlaveServerGroup = "on";
                                    request.MasterSlaveServerGroupId = para.ServerGroupId;
                                }
                                else if (para.VServerGroup && para.ServerGroupId != null)
                                {
                                    request.VServerGroup = "on";
                                    request.VServerGroupId = para.ServerGroupId;
                                }
                                else
                                {
                                    request.MasterSlaveServerGroup = "off";
                                    request.VServerGroup = "off";
                                }
                            }
                            else
                            {
                                request.MasterSlaveServerGroup = "off";
                                request.VServerGroup = "off";
                            }

                            if (para.KeepSession)
                            {
                                request.PersistenceTimeout = para.PersistenceTimeout;
                            }

                            request.HealthCheckConnectPort = para.HealthCheckConnectPort;
                            request.HealthCheckConnectTimeout = para.HealthCheckTimeout;
                            request.HealthCheckInterval = para.HealthCheckInterval;
                            request.UnhealthyThreshold = para.UnhealthyThreshold;
                            request.HealthyThreshold = para.HealthyThreshold;
                            request.HealthCheckReq = para.HealthCheckReq;
                            request.HealthCheckExp = para.HealthCheckExp;
                            SetLoadBalancerUDPListenerAttributeResponse response = mClient.GetAcsResponse(request);
                        }
                        break;
                }
                Dispatcher.Invoke(new Action(ConfiguredListener));
            }
            catch (Exception)
            {
                Dispatcher.Invoke(new Action(ConfiguredListenerError));
            }
        }

        public void Submmit()
        {
            Thread t;
            if (mParams.AddListener)
            {
                mParams.AddListenerStatus = "doing";
                if (mParams.AutoStart)
                {
                    mParams.StartListenerStatus = "doing";
                }
                mParams.AllStatus = "doing";
                t = new Thread(new ParameterizedThreadStart(DoAddListener));
                t.Start(mParams);
            }
            else if (mParams.ConfigureListener)
            {
                mParams.ConfigureListenerStatus = "doing";
                mParams.AllStatus = "doing";
                t = new Thread(new ParameterizedThreadStart(DoConfigureListener));
                t.Start(mParams);
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            mOwner.Close();
        }
    }
}
