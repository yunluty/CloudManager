using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CloudManager
{
    public class AddListenerParams : INotifyPropertyChanged
    {
        public string RegionId { get; set; }

        public string LoadBalancerId { get; set; }

        private bool _AddListener;
        public bool AddListener
        {
            get { return _AddListener; }
            set
            {
                _AddListener = value;
                NotifyPropertyChanged("AddListener");
            }
        }

        private bool _ConfigureListener;
        public bool ConfigureListener
        {
            get { return _ConfigureListener; }
            set
            {
                _ConfigureListener = value;
                NotifyPropertyChanged("ConfigureListener");
            }
        }

        private string _AddListenerStatus;
        public string AddListenerStatus
        {
            get { return _AddListenerStatus; }
            set
            {
                _AddListenerStatus = value;
                NotifyPropertyChanged("AddListenerStatus");
            }
        }

        private string _ConfigureListenerStatus;
        public string ConfigureListenerStatus
        {
            get { return _ConfigureListenerStatus; }
            set
            {
                _ConfigureListenerStatus = value;
                NotifyPropertyChanged("ConfigureListenerStatus");
            }
        }

        private string _StartListenerStatus;
        public string StartListenerStatus
        {
            get { return _StartListenerStatus; }
            set
            {
                _StartListenerStatus = value;
                NotifyPropertyChanged("StartListenerStatus");
            }
        }

        private string _AllStatus;
        public string AllStatus
        {
            get { return _AllStatus; }
            set
            {
                _AllStatus = value;
                NotifyPropertyChanged("AllStatus");
            }
        }

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

        private int? _ListenerPort;
        public int? ListenerPort
        {
            get { return _ListenerPort; }
            set
            {
                _ListenerPort = value;
                NotifyPropertyChanged("ListenerPort");
            }
        }

        private int? _BackendServerPort;
        public int? BackendServerPort
        {
            get { return _BackendServerPort; }
            set
            {
                _BackendServerPort = value;
                NotifyPropertyChanged("BackendServerPort");
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

        private string _BandWidthStr;
        public string BandWidthStr
        {
            get { return _BandWidthStr; }
            set
            {
                _BandWidthStr = value;
                NotifyPropertyChanged("BandWidthStr");

                int.TryParse(value, out int width);
                if (width > 0)
                {
                    BandWidth = width;
                }
                else
                {
                    BandWidth = null;
                }
            }
        }

        private int? _BandWidth;
        public int? BandWidth
        {
            get { return _BandWidth; }
            set
            {
                _BandWidth = value;
                NotifyPropertyChanged("BandWidth");
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

        private bool _UseVServerGroup;
        public bool UseVServerGroup
        {
            get { return _UseVServerGroup; }
            set
            {
                _UseVServerGroup = value;
                NotifyPropertyChanged("UseVServerGroup");
            }
        }

        private bool _UseTwoWayAuth;
        public bool UseTwoWayAuth
        {
            get { return _UseTwoWayAuth; }
            set
            {
                _UseTwoWayAuth = value;
                NotifyPropertyChanged("UseTwoWayAuth");
            }
        }

        private string _ServerCertificateId;
        public string ServerCertificateId
        {
            get { return _ServerCertificateId; }
            set
            {
                _ServerCertificateId = value;
                NotifyPropertyChanged("ServerCertificateId");
            }
        }

        private string _CACertificateId;
        public string CACertificateId
        {
            get { return _CACertificateId; }
            set
            {
                _CACertificateId = value;
                NotifyPropertyChanged("CACertificateId");
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

        private int? _PersistenceTimeout;
        public int? PersistenceTimeout
        {
            get { return _PersistenceTimeout; }
            set
            {
                _PersistenceTimeout = value;
                NotifyPropertyChanged("PersistenceTimeout");
            }
        }

        private int? _EstablishedTimeout;
        public int? EstablishedTimeout
        {
            get { return _EstablishedTimeout; }
            set
            {
                _EstablishedTimeout = value;
                NotifyPropertyChanged("EstablishedTimeout");
            }
        }

        private bool _StickySession;
        public bool StickySession
        {
            get { return _StickySession; }
            set
            {
                _StickySession = value;
                NotifyPropertyChanged("StickySession");
            }
        }

        private string _StickySessionType;
        public string StickySessionType
        {
            get { return _StickySessionType; }
            set
            {
                _StickySessionType = value;
                NotifyPropertyChanged("StickySessionType");
            }
        }

        private int? _CookieTimeout;
        public int? CookieTimeout
        {
            get { return _CookieTimeout; }
            set
            {
                _CookieTimeout = value;
                NotifyPropertyChanged("CookieTimeout");
            }
        }

        private string _Cookie;
        public string Cookie
        {
            get { return _Cookie; }
            set
            {
                _Cookie = value;
                NotifyPropertyChanged("Cookie");
            }
        }

        private bool _Gzip;
        public bool Gzip
        {
            get { return _Gzip; }
            set
            {
                _Gzip = value;
                NotifyPropertyChanged("Gzip");
            }
        }

        private bool _XForwardedFor;
        public bool XForwardedFor
        {
            get { return _XForwardedFor; }
            set
            {
                _XForwardedFor = value;
                NotifyPropertyChanged("XForwardedFor");
            }
        }

        private bool _XForwardedFor_SLBID;
        public bool XForwardedFor_SLBID
        {
            get { return _XForwardedFor_SLBID; }
            set
            {
                _XForwardedFor_SLBID = value;
                NotifyPropertyChanged("XForwardedFor_SLBID");
            }
        }

        private bool _XForwardedFor_SLBIP;
        public bool XForwardedFor_SLBIP
        {
            get { return _XForwardedFor_SLBIP; }
            set
            {
                _XForwardedFor_SLBIP = value;
                NotifyPropertyChanged("XForwardedFor_SLBIP");
            }
        }

        private bool _XForwardedFor_proto;
        public bool XForwardedFor_proto
        {
            get { return _XForwardedFor_proto; }
            set
            {
                _XForwardedFor_proto = value;
                NotifyPropertyChanged("XForwardedFor_proto");
            }
        }

        private bool _TCPCheck;
        public bool TCPCheck
        {
            get { return _TCPCheck; }
            set
            {
                _TCPCheck = value;
                NotifyPropertyChanged("TCPCheck");
            }
        }

        private bool _HTTPCheck;
        public bool HTTPCheck
        {
            get { return _HTTPCheck; }
            set
            {
                _HTTPCheck = value;
                NotifyPropertyChanged("HTTPCheck");
            }
        }

        private bool _HTTPCheckOn;
        public bool HTTPCheckOn
        {
            get { return _HTTPCheckOn; }
            set
            {
                _HTTPCheckOn = value;
                NotifyPropertyChanged("HTTPCheckOn");
            }
        }

        private string _HealthCheckDomain;
        public string HealthCheckDomain
        {
            get { return _HealthCheckDomain; }
            set
            {
                _HealthCheckDomain = value;
                NotifyPropertyChanged("HealthCheckDomain");
            }
        }

        private string _HealthCheckPortStr;
        public string HealthCheckPortStr
        {
            get { return _HealthCheckPortStr; }
            set
            {
                _HealthCheckPortStr = value;
                NotifyPropertyChanged("HealthCheckPortStr");

                int.TryParse(value, out int port);
                if (port > 0)
                {
                    HealthCheckConnectPort = port;
                }
                else
                {
                    HealthCheckConnectPort = null;
                }
            }
        }

        private int? _HealthCheckConnectPort;
        public int? HealthCheckConnectPort
        {
            get { return _HealthCheckConnectPort; }
            set
            {
                _HealthCheckConnectPort = value;
                NotifyPropertyChanged("HealthCheckConnectPort");
            }
        }

        private string _HealthCheckURI;
        public string HealthCheckURI
        {
            get { return _HealthCheckURI; }
            set
            {
                _HealthCheckURI = value;
                NotifyPropertyChanged("HealthCheckURI");
            }
        }

        private bool _http_2xx;
        public bool http_2xx
        {
            get { return _http_2xx; }
            set
            {
                _http_2xx = value;
                NotifyPropertyChanged("http_2xx");
            }
        }

        private bool _http_3xx;
        public bool http_3xx
        {
            get { return _http_3xx; }
            set
            {
                _http_3xx = value;
                NotifyPropertyChanged("http_3xx");
            }
        }

        private bool _http_4xx;
        public bool http_4xx
        {
            get { return _http_4xx; }
            set
            {
                _http_4xx = value;
                NotifyPropertyChanged("http_4xx");
            }
        }

        private bool _http_5xx;
        public bool http_5xx
        {
            get { return _http_5xx; }
            set
            {
                _http_5xx = value;
                NotifyPropertyChanged("http_5xx");
            }
        }

        private int? _HealthCheckTimeout;
        public int? HealthCheckTimeout
        {
            get { return _HealthCheckTimeout; }
            set
            {
                _HealthCheckTimeout = value;
                NotifyPropertyChanged("HealthCheckTimeout");
            }
        }
        private int? _HealthCheckInterval;
        public int? HealthCheckInterval
        {
            get { return _HealthCheckInterval; }
            set
            {
                _HealthCheckInterval = value;
                NotifyPropertyChanged("HealthCheckInterval");
            }
        }

        private int? _UnhealthyThreshold;
        public int? UnhealthyThreshold
        {
            get { return _UnhealthyThreshold; }
            set
            {
                _UnhealthyThreshold = value;
                NotifyPropertyChanged("UnhealthyThreshold");
            }
        }

        private int? _HealthyThreshold;
        public int? HealthyThreshold
        {
            get { return _HealthyThreshold; }
            set
            {
                _HealthyThreshold = value;
                NotifyPropertyChanged("HealthyThreshold");
            }
        }

        private string _HealthCheckReq;
        public string HealthCheckReq
        {
            get { return _HealthCheckReq; }
            set
            {
                _HealthCheckReq = value;
                NotifyPropertyChanged("HealthCheckReq");
            }
        }

        private string _HealthCheckExp;
        public string HealthCheckExp
        {
            get { return _HealthCheckExp; }
            set
            {
                _HealthCheckExp = value;
                NotifyPropertyChanged("HealthCheckExp");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
