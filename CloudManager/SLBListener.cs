using Aliyun.Acs.Slb.Model.V20140515;
using System;
using System.Collections.Generic;
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
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        public void notifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
