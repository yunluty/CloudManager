using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
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
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeInstancesResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeHealthStatusResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeLoadBalancerAttributeResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeMasterSlaveServerGroupsResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeVServerGroupsResponse;

namespace CloudManager
{
    /// <summary>
    /// SLBServersPage.xaml 的交互逻辑
    /// </summary>
    public partial class SLBServersPage : Page
    {
        private string mAki, mAks;
        private ObservableCollection<DescribeInstance> mBackendServers;
        private ObservableCollection<DescribeInstance> mNotAddedServers;
        private ObservableCollection<SLBListener> mListeners;

        public delegate void DelegateGot(object obj);

        public SLBServersPage()
        {
            InitializeComponent();
        }

        public SLBServersPage(string aki, string aks)
        {
            InitializeComponent();
            mAki = aki;
            mAks = aks;
            mBackendServers = new ObservableCollection<DescribeInstance>();
            BackendServers.DataContext = mBackendServers;
            mNotAddedServers = new ObservableCollection<DescribeInstance>();
            NotAddedServers.DataContext = mNotAddedServers;
            mListeners = new ObservableCollection<SLBListener>();
            Listeners.DataContext = mListeners;
        }

        private DescribeLoadBalancer _mBalancer;
        public DescribeLoadBalancer mBalancer
        {
            get { return _mBalancer; }
            set
            {
                _mBalancer = value;
                Thread t = new Thread(GetAttribute);
                t.Start();
            }
        }

        private void GotBackendServers(object obj)
        {
            ObservableCollection<DescribeInstance> instances = obj as ObservableCollection<DescribeInstance>;
            if (instances != null)
            {
                mBackendServers = instances;
                BackendServers.ItemsSource = mBackendServers;
                BackendServers.DataContext = mBackendServers;
            }
        }

        private void GotNotAddedServers(object obj)
        {
            ObservableCollection<DescribeInstance> instances = obj as ObservableCollection<DescribeInstance>;
            if (instances != null)
            {
                mNotAddedServers = instances;
                NotAddedServers.ItemsSource = mNotAddedServers;
                NotAddedServers.DataContext = mNotAddedServers;
            }
        }

        private void GetServers(DefaultAcsClient client, DescribeLoadBalancerAttributeResponse response)
        {
            ObservableCollection<DescribeInstance> addedServers = new ObservableCollection<DescribeInstance>();
            foreach (DescribeLoadBalancerAttribute_BackendServer s in response.BackendServers)
            {
                DescribeInstanceAttributeRequest request1 = new DescribeInstanceAttributeRequest();
                request1.InstanceId = s.ServerId;
                DescribeInstanceAttributeResponse reponse1 = client.GetAcsResponse(request1);
                DescribeInstance instance = new DescribeInstance();
                instance.Weight = s.Weight;
                instance.InstanceId = reponse1.InstanceId;
                instance.InstanceName = reponse1.InstanceName;
                instance.RegionId = reponse1.RegionId;
                instance.ZoneId = reponse1.ZoneId;
                instance.PublicIpAddress = reponse1.PublicIpAddress;
                instance.InnerIpAddress = reponse1.InnerIpAddress;
                instance.Status = reponse1.Status;
                addedServers.Add(instance);
            }
            Dispatcher.Invoke(new DelegateGot(GotBackendServers), addedServers);

            DescribeInstancesRequest request2 = new DescribeInstancesRequest();
            DescribeInstancesResponse response2 = client.GetAcsResponse(request2);
            ObservableCollection<DescribeInstance> notAddedServers = new ObservableCollection<DescribeInstance>();
            if (response2.Instances.Count > 0)
            {
                foreach (DescribeInstances_Instance i in response2.Instances)
                {
                    bool find = false;
                    foreach (DescribeInstance instance in addedServers)
                    {
                        if (instance.InstanceId == i.InstanceId)
                        {
                            find = true;
                        }
                    }
                    if (!find)
                    {
                        notAddedServers.Add(new DescribeInstance(i));
                    }
                }
            }
            Dispatcher.Invoke(new DelegateGot(GotNotAddedServers), notAddedServers);
        }

        private void GotListeners(object obj)
        {
            ObservableCollection<SLBListener> listeners = obj as ObservableCollection<SLBListener>;
            if (listeners != null)
            {
                mListeners = listeners;
                Listeners.ItemsSource = mListeners;
                Listeners.DataContext = mListeners;
            }
        }

        private void GetListeners(DefaultAcsClient client, DescribeLoadBalancerAttributeResponse response)
        {
            ObservableCollection<SLBListener> listeners = new ObservableCollection<SLBListener>();

            foreach (DescribeLoadBalancerAttribute_ListenerPortAndProtocol p in response.ListenerPortsAndProtocol)
            {
                SLBListener l = null;

                if (p.ListenerProtocol.Equals("tcp"))
                {
                    DescribeLoadBalancerTCPListenerAttributeRequest request = new DescribeLoadBalancerTCPListenerAttributeRequest();
                    request.LoadBalancerId = response.LoadBalancerId;
                    request.ListenerPort = p.ListenerPort;
                    DescribeLoadBalancerTCPListenerAttributeResponse listener = client.GetAcsResponse(request);
                    l = new SLBListener(listener);
                }
                else if (p.ListenerProtocol.Equals("http"))
                {
                    DescribeLoadBalancerHTTPListenerAttributeRequest request = new DescribeLoadBalancerHTTPListenerAttributeRequest();
                    request.LoadBalancerId = response.LoadBalancerId;
                    request.ListenerPort = p.ListenerPort;
                    DescribeLoadBalancerHTTPListenerAttributeResponse listener = client.GetAcsResponse(request);
                    l = new SLBListener(listener);
                }
                else if (p.ListenerProtocol.Equals("https"))
                {
                    DescribeLoadBalancerHTTPSListenerAttributeRequest request = new DescribeLoadBalancerHTTPSListenerAttributeRequest();
                    request.LoadBalancerId = response.LoadBalancerId;
                    request.ListenerPort = p.ListenerPort;
                    DescribeLoadBalancerHTTPSListenerAttributeResponse listener = client.GetAcsResponse(request);
                    l = new SLBListener(listener);
                }
                else if (p.ListenerProtocol.Equals("udp"))
                {
                    DescribeLoadBalancerUDPListenerAttributeRequest request = new DescribeLoadBalancerUDPListenerAttributeRequest();
                    request.LoadBalancerId = response.LoadBalancerId;
                    request.ListenerPort = p.ListenerPort;
                    DescribeLoadBalancerUDPListenerAttributeResponse listener = client.GetAcsResponse(request);
                    l = new SLBListener(listener);
                }

                if (l != null)
                {
                    listeners.Add(l);
                }
            }

            if (listeners.Count > 0)
            {
                DescribeHealthStatusRequest request = new DescribeHealthStatusRequest();
                request.LoadBalancerId = response.LoadBalancerId;
                DescribeHealthStatusResponse status = client.GetAcsResponse(request);

                /*DescribeVServerGroupsRequest vrequest = new DescribeVServerGroupsRequest();
                vrequest.LoadBalancerId = response.LoadBalancerId;
                vrequest.RegionId = response.RegionId;
                DescribeVServerGroupsResponse vgroups = client.GetAcsResponse(vrequest);

                DescribeMasterSlaveServerGroupsRequest describe = new DescribeMasterSlaveServerGroupsRequest();
                describe.RegionId = response.RegionId;
                describe.LoadBalancerId = response.LoadBalancerId;
                DescribeMasterSlaveServerGroupsResponse msgroups = client.GetAcsResponse(describe);*/

                foreach (SLBListener l in listeners)
                {
                    foreach (DescribeHealthStatus_BackendServer server in status.BackendServers)
                    {
                        if (server.ListenerPort == l.ListenerPort)
                        {
                            l.HealthStatus = server.ServerHealthStatus;
                        }
                    }

                    /*if (l.VServerGroupId != null)
                    {
                        foreach (DescribeVServerGroups_VServerGroup group in vgroups.VServerGroups)
                        {
                            if (l.VServerGroupId == group.VServerGroupId)
                            {
                                l.ServerGroupName = "[虚拟] " + group.VServerGroupName;
                            }
                        }
                    }
                    else if (l.MasterSlaveServerGroupId != null)
                    {
                        foreach (DescribeMasterSlaveServerGroups_MasterSlaveServerGroup group in msgroups.MasterSlaveServerGroups)
                        {
                            if (l.VServerGroupId == group.MasterSlaveServerGroupId)
                            {
                                l.ServerGroupName = "[主备] " + group.MasterSlaveServerGroupName;
                            }
                        }
                    }
                    else
                    {
                        l.ServerGroupName = "无";
                    }*/
                }
            }

            Dispatcher.Invoke(new DelegateGot(GotListeners), listeners);
        }

        private void GetAttribute()
        {
            IClientProfile profile = DefaultProfile.GetProfile(mBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeLoadBalancerAttributeRequest request = new DescribeLoadBalancerAttributeRequest();
            request.LoadBalancerId = mBalancer.LoadBalancerId;
            try
            {
                DescribeLoadBalancerAttributeResponse response = client.GetAcsResponse(request);
                GetServers(client, response);
                GetListeners(client, response);
            }
            catch (ClientException)
            {
            }
        }

        private void RemovedServers()
        {
            AddedSelectAll.IsChecked = false;
            mBackendServers.Clear();

            Thread t = new Thread(GetAttribute);
            t.Start();
        }

        private void RemoveServers(object obj)
        {
            IClientProfile profile = DefaultProfile.GetProfile(mBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            RemoveBackendServersRequest request = new RemoveBackendServersRequest();
            request.LoadBalancerId = mBalancer.LoadBalancerId;
            request.BackendServers = obj as string;
            try
            {
                RemoveBackendServersResponse response = client.GetAcsResponse(request);
                Dispatcher.Invoke(new Action(RemovedServers));
            }
            catch (Exception)
            {
            }
        }

        private void RemoveSingle_Click(object sender, RoutedEventArgs e)
        {
            DescribeInstance instance = (sender as Button).DataContext as DescribeInstance;
            string server = "[\"" + instance.InstanceId + "\"]";
            Thread t = new Thread(new ParameterizedThreadStart(RemoveServers));
            t.Start(server);
        }

        private void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            string servers = "[";
            for (int i = 0; i < mBackendServers.Count; i++)
            {
                if (i != 0)
                {
                    servers += ',';
                }

                DescribeInstance instance = mBackendServers[i];
                if (instance.Checked)
                {
                    servers += "\"";
                    servers += instance.InstanceId;
                    servers += "\"";
                }
            }
            servers += "]";
            Thread t = new Thread(new ParameterizedThreadStart(RemoveServers));
            t.Start(servers);
        }

        private void AddedServers()
        {
            NotAddedSelectAll.IsChecked = false;
            mNotAddedServers.Clear();

            Thread t = new Thread(GetAttribute);
            t.Start();
        }

        private void AddServers(object obj)
        {
            IClientProfile profile = DefaultProfile.GetProfile(mBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            AddBackendServersRequest request = new AddBackendServersRequest();
            request.LoadBalancerId = mBalancer.LoadBalancerId;
            request.BackendServers = obj as string;
            try
            {
                AddBackendServersResponse response = client.GetAcsResponse(request);
                Dispatcher.Invoke(new Action(AddedServers));
            }
            catch
            {
            }
        }

        private void AddSingle_Click(object sender, RoutedEventArgs e)
        {
            DescribeInstance instance = (sender as Button).DataContext as DescribeInstance;
            string server = "[{\"ServerId\":\""+ instance.InstanceId + "\",\"Weight\":\"100\"}]";
            Thread t = new Thread(new ParameterizedThreadStart(AddServers));
            t.Start(server);
        }

        private void AddSelected_Click(object sender, RoutedEventArgs e)
        {
            string servers = "[";
            for (int i = 0; i < mNotAddedServers.Count; i++)
            {
                if (i != 0)
                {
                    servers += ',';
                }

                DescribeInstance instance = mNotAddedServers[i];
                if (instance.Checked)
                {
                    servers += "{\"ServerId\":\"";
                    servers += instance.InstanceId;
                    servers += "\",\"Weight\":\"100\"}";
                }
            }
            servers += "]";
            Thread t = new Thread(AddServers);
            t.Start(servers);
        }

        private void AddedSelect_Checked(object sender, RoutedEventArgs e)
        {
            bool allChecked = true;
            foreach (DescribeInstance i in mBackendServers)
            {
                if (!i.Checked)
                {
                    allChecked = false;
                    break;
                }
            }
            if (allChecked == true)
            {
                AddedSelectAll.IsChecked = true;
            }
        }

        private void AddedSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            AddedSelectAll.IsChecked = false;
        }

        private void AddedSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (AddedSelectAll.IsChecked == true)
            {
                foreach (DescribeInstance i in mBackendServers)
                {
                    i.Checked = true;
                }
            }
            else
            {
                foreach (DescribeInstance i in mBackendServers)
                {
                    i.Checked = false;
                }
            }
        }

        private void NotAddedSelect_Checked(object sender, RoutedEventArgs e)
        {
            bool allChecked = true;
            foreach (DescribeInstance i in mNotAddedServers)
            {
                if (!i.Checked)
                {
                    allChecked = false;
                    break;
                }
            }
            if (allChecked == true)
            {
                NotAddedSelectAll.IsChecked = true;
            }
        }

        private void NotAddedSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            NotAddedSelectAll.IsChecked = false;
        }

        private void NotAddedSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (NotAddedSelectAll.IsChecked == true)
            {
                foreach (DescribeInstance i in mNotAddedServers)
                {
                    i.Checked = true;
                }
            }
            else
            {
                foreach (DescribeInstance i in mNotAddedServers)
                {
                    i.Checked = false;
                }
            }
        }

        private void ListenersSelect_Checked(object sender, RoutedEventArgs e)
        {
            bool allChecked = true;
            foreach (SLBListener i in mListeners)
            {
                if (!i.Checked)
                {
                    allChecked = false;
                    break;
                }
            }
            if (allChecked == true)
            {
                ListenersSelectAll.IsChecked = true;
            }
        }

        private void ListenersSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            ListenersSelectAll.IsChecked = false;
        }

        private void StartListeners(object obj)
        {
            List<int?> ports = obj as List<int?>;
            IClientProfile profile = DefaultProfile.GetProfile(mBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            StartLoadBalancerListenerRequest request = new StartLoadBalancerListenerRequest();
            foreach (int? port in ports)
            {
                try
                {
                    request.LoadBalancerId = mBalancer.LoadBalancerId;
                    request.ListenerPort = port;
                    StartLoadBalancerListenerResponse response = client.GetAcsResponse(request);
                }
                catch
                {
                }
            }
            GetAttribute();
        }

        private void StartListeners_Click(object sender, RoutedEventArgs e)
        {
            List<int?> ports = new List<int?>();
            foreach (SLBListener l in mListeners)
            {
                if (l.Checked)
                {
                    ports.Add(l.ListenerPort);
                }
            }
            Thread t = new Thread(new ParameterizedThreadStart(StartListeners));
            t.Start(ports);
        }

        private void StopListeners(object obj)
        {
            List<int?> ports = obj as List<int?>;
            IClientProfile profile = DefaultProfile.GetProfile(mBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            StopLoadBalancerListenerRequest request = new StopLoadBalancerListenerRequest();
            foreach (int? port in ports)
            {
                try
                {
                    request.LoadBalancerId = mBalancer.LoadBalancerId;
                    request.ListenerPort = port;
                    StopLoadBalancerListenerResponse response = client.GetAcsResponse(request);
                }
                catch
                {
                }
            }
        }

        private void StopListeners_Click(object sender, RoutedEventArgs e)
        {
            List<int?> ports = new List<int?>();
            foreach (SLBListener l in mListeners)
            {
                if (l.Checked)
                {
                    ports.Add(l.ListenerPort);
                }
            }
            Thread t = new Thread(new ParameterizedThreadStart(StopListeners));
            t.Start(ports);
        }

        private void DeleteListeners(object obj)
        {
            List<int?> ports = obj as List<int?>;
            IClientProfile profile = DefaultProfile.GetProfile(mBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DeleteLoadBalancerListenerRequest request = new DeleteLoadBalancerListenerRequest();
            foreach (int? port in ports)
            {
                try
                {
                    request.LoadBalancerId = mBalancer.LoadBalancerId;
                    request.ListenerPort = port;
                    DeleteLoadBalancerListenerResponse response = client.GetAcsResponse(request);
                }
                catch
                {
                }
            }
            GetAttribute();
        }

        private void DeleteListeners_Click(object sender, RoutedEventArgs e)
        {
            List<int?> ports = new List<int?>();
            foreach (SLBListener l in mListeners)
            {
                if (l.Checked)
                {
                    ports.Add(l.ListenerPort);
                }
            }
            Thread t = new Thread(new ParameterizedThreadStart(DeleteListeners));
            t.Start(ports);
        }

        private void AddListener_Click(object sender, RoutedEventArgs e)
        {
            AddListenerWindow win = new AddListenerWindow();
            win.ShowDialog();
        }

        private void ListenersSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (ListenersSelectAll.IsChecked == true)
            {
                foreach (SLBListener l in mListeners)
                {
                    l.Checked = true;
                }
            }
            else
            {
                foreach (SLBListener l in mListeners)
                {
                    l.Checked = false;
                }
            }
        }
    }
}
