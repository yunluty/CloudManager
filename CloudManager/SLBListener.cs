using Aliyun.Acs.Slb.Model.V20140515;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudManager
{
    public class SLBListener : INotifyPropertyChanged
    {
        public SLBListener(DescribeLoadBalancerTCPListenerAttributeResponse tcp)
        {
            Protocol = "TCP";
            ListenerPort = tcp.ListenerPort;
            BackendServerPort = tcp.BackendServerPort;
            Bandwidth = tcp.Bandwidth;
            Status = tcp.Status;
            Scheduler = tcp.Scheduler;
            VServerGroupId = tcp.VServerGroupId;
            MasterSlaveServerGroupId = tcp.MasterSlaveServerGroupId;
            PersistenceTimeout = tcp.PersistenceTimeout;
            EstablishedTimeout = tcp.EstablishedTimeout;
            HealthCheckType = tcp.HealthCheckType;
            HealthCheckDomain = tcp.HealthCheckDomain;
            HealthCheckConnectPort = tcp.HealthCheckConnectPort;
            HealthCheckURI = tcp.HealthCheckURI;
            HealthCheckHttpCode = tcp.HealthCheckHttpCode;
            HealthCheckTimeout = tcp.HealthCheckConnectTimeout;
            HealthCheckInterval = tcp.HealthCheckInterval;
            UnhealthyThreshold = tcp.UnhealthyThreshold;
            HealthyThreshold = tcp.HealthyThreshold;
        }

        public SLBListener(DescribeLoadBalancerHTTPListenerAttributeResponse http)
        {
            Protocol = "HTTP";
            ListenerPort = http.ListenerPort;
            BackendServerPort = http.BackendServerPort;
            Bandwidth = http.Bandwidth;
            Status = http.Status;
            Scheduler = http.Scheduler;
            VServerGroupId = http.VServerGroupId;
            StickySession = http.StickySession;
            StickySessionType = http.StickySessionType;
            Cookie = http.Cookie;
            CookieTimeout = http.CookieTimeout;
            Gzip = http.Gzip;
            XForwardedFor = http.XForwardedFor;
            XForwardedFor_SLBID = http.XForwardedFor_SLBID;
            XForwardedFor_SLBIP = http.XForwardedFor_SLBIP;
            XForwardedFor_proto = http.XForwardedFor_proto;
            HealthCheck = http.HealthCheck;
            HealthCheckDomain = http.HealthCheckDomain;
            HealthCheckConnectPort = http.HealthCheckConnectPort;
            HealthCheckURI = http.HealthCheckURI;
            HealthCheckHttpCode = http.HealthCheckHttpCode;
            HealthCheckTimeout = http.HealthCheckTimeout;
            HealthCheckInterval = http.HealthCheckInterval;
            UnhealthyThreshold = http.UnhealthyThreshold;
            HealthyThreshold = http.HealthyThreshold;
        }

        public SLBListener(DescribeLoadBalancerHTTPSListenerAttributeResponse https)
        {
            Protocol = "HTTPS";
            ListenerPort = https.ListenerPort;
            BackendServerPort = https.BackendServerPort;
            Bandwidth = https.Bandwidth;
            Status = https.Status;
            Scheduler = https.Scheduler;
            VServerGroupId = https.VServerGroupId;
            ServerCertificateId = https.ServerCertificateId;
            CACertificateId = https.CACertificateId;
            StickySession = https.StickySession;
            StickySessionType = https.StickySessionType;
            Cookie = https.Cookie;
            CookieTimeout = https.CookieTimeout;
            Gzip = https.Gzip;
            XForwardedFor = https.XForwardedFor;
            XForwardedFor_SLBID = https.XForwardedFor_SLBID;
            XForwardedFor_SLBIP = https.XForwardedFor_SLBIP;
            XForwardedFor_proto = https.XForwardedFor_proto;
            HealthCheck = https.HealthCheck;
            HealthCheckDomain = https.HealthCheckDomain;
            HealthCheckConnectPort = https.HealthCheckConnectPort;
            HealthCheckURI = https.HealthCheckURI;
            HealthCheckHttpCode = https.HealthCheckHttpCode;
            HealthCheckTimeout = https.HealthCheckTimeout;
            HealthCheckInterval = https.HealthCheckInterval;
            UnhealthyThreshold = https.UnhealthyThreshold;
            HealthyThreshold = https.HealthyThreshold;
        }

        public SLBListener(DescribeLoadBalancerUDPListenerAttributeResponse udp)
        {
            Protocol = "UDP";
            ListenerPort = udp.ListenerPort;
            BackendServerPort = udp.BackendServerPort;
            Bandwidth = udp.Bandwidth;
            Status = udp.Status;
            Scheduler = udp.Scheduler;
            VServerGroupId = udp.VServerGroupId;
            MasterSlaveServerGroupId = udp.MasterSlaveServerGroupId;
            PersistenceTimeout = udp.PersistenceTimeout;
            HealthCheckConnectPort = udp.HealthCheckConnectPort;
            HealthCheckTimeout = udp.HealthCheckConnectTimeout;
            HealthCheckInterval = udp.HealthCheckInterval;
            UnhealthyThreshold = udp.UnhealthyThreshold;
            HealthyThreshold = udp.HealthyThreshold;
            HealthCheckReq = udp.HealthCheckReq;
            HealthCheckExp = udp.HealthCheckExp;
        }

        public string RegionId { get; set; }
        public string LoadBalancerId { get; set; }

        private string _Protocol;
        public string Protocol
        {
            get { return _Protocol; }
            set
            {
                _Protocol = value;
                notifyPropertyChanged("Protocol");
            }
        }

        private int? _ListenerPort;
        public int? ListenerPort
        {
            get { return _ListenerPort; }
            set
            {
                _ListenerPort = value;
                notifyPropertyChanged("ListenerPort");
            }
        }

        private int? _BackendServerPort;
        public int? BackendServerPort
        {
            get { return _BackendServerPort; }
            set
            {
                _BackendServerPort = value;
                notifyPropertyChanged("BackendServerPort");
            }
        }

        private int? _Bandwidth;
        public int? Bandwidth
        {
            get { return _Bandwidth; }
            set
            {
                _Bandwidth = value;
                notifyPropertyChanged("Bandwidth");
            }
        }

        private string _Status;
        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                notifyPropertyChanged("Status");
            }
        }

        private string _Scheduler;
        public string Scheduler
        {
            get { return _Scheduler; }
            set
            {
                _Scheduler = value;
                notifyPropertyChanged("Scheduler");
            }
        }

        private string _HealthStatus;
        public string HealthStatus
        {
            get { return _HealthStatus; }
            set
            {
                _HealthStatus = value;
                notifyPropertyChanged("HealthStatus");
            }
        }

        private bool _Checked;
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                _Checked = value;
                notifyPropertyChanged("Checked");
            }
        }

        public string VServerGroupId { get; set; }
        public string MasterSlaveServerGroupId { get; set; }
        public string ServerGroupName { get; set; }
        public string ServerGroupNameShow { get; set; }
        public string ServerCertificateId { get; set; }
        public string CACertificateId { get; set; }
        public int? EstablishedTimeout { get; set; }
        public int? PersistenceTimeout { get; set; }
        public string StickySession { get; set; }
        public string StickySessionType { get; set; }
        public string Cookie { get; set; }
        public int? CookieTimeout { get; set; }
        public string Gzip { get; set; }
        public string XForwardedFor { get; set; }
        public string XForwardedFor_SLBID { get; set; }
        public string XForwardedFor_SLBIP { get; set; }
        public string XForwardedFor_proto { get; set; }
        public string HealthCheckType { get; set; }
        public string HealthCheck { get; set; }
        public string HealthCheckDomain { get; set; }
        public int? HealthCheckConnectPort { get; set; }
        public string HealthCheckURI { get; set; }
        public string HealthCheckHttpCode { get; set; }
        public int? HealthCheckTimeout { get; set; }
        public int? HealthCheckInterval { get; set; }
        public int? UnhealthyThreshold { get; set; }
        public int? HealthyThreshold { get; set; }
        public string HealthCheckExp { get; set; }
        public string HealthCheckReq { get; set; }
        public ObservableCollection<DescribeRule> Rules { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void notifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
