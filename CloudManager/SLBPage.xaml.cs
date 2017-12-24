using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Ecs.Model.V20140526;
using Aliyun.Acs.Slb.Model.V20140515;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeHealthStatusResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeLoadBalancerAttributeResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeLoadBalancersResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeMasterSlaveServerGroupsResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeRulesResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeVServerGroupsResponse;

namespace CloudManager
{
    /// <summary>
    /// SLBServersPage.xaml 的交互逻辑
    /// </summary>
    public partial class SLBPage : Page
    {
        private string mAki, mAks;
        private List<DescribeRegions_Region> mRegions;
        private ObservableCollection<DescribeLoadBalancer> mLoadBalancers = new ObservableCollection<DescribeLoadBalancer>();
        private DescribeLoadBalancer mSelBalancer;
        private ObservableCollection<DescribeInstance> mBackendServers;
        private ObservableCollection<DescribeInstance> mNotAddedServers;
        private ObservableCollection<SLBListener> mListeners;
        private SLBListener mSelListener;
        private ObservableCollection<ServerGroup> mVServerGroups;
        private ObservableCollection<ServerGroup> mMServerGroups;

        public MainWindow mMainWindow { get; set; }
        public delegate void DelegateGot(object obj);


        public SLBPage()
        {
            InitializeComponent();

            mAki = App.AKI;
            mAks = App.AKS;
            mRegions = App.REGIONS;
            SLBList.ItemsSource = mLoadBalancers;
            Thread t = new Thread(GetLoadBalancers);
            t.Start();

            mBackendServers = new ObservableCollection<DescribeInstance>();
            Added.DataContext = mBackendServers;
            mNotAddedServers = new ObservableCollection<DescribeInstance>();
            NotAdded.DataContext = mNotAddedServers;
            mListeners = new ObservableCollection<SLBListener>();
            ListenersList.DataContext = mListeners;
        }

        private void GetLoadBalancers()
        {
            ObservableCollection<DescribeLoadBalancer> balancers = new ObservableCollection<DescribeLoadBalancer>();
            Parallel.ForEach(mRegions, (region) =>
            {
                IClientProfile profile = DefaultProfile.GetProfile(region.RegionId, mAki, mAks);
                DefaultAcsClient client = new DefaultAcsClient(profile);
                /*Aliyun.Acs.Slb.Model.V20140515.DescribeZonesRequest zonesRequest = new Aliyun.Acs.Slb.Model.V20140515.DescribeZonesRequest();
                try
                {
                    Aliyun.Acs.Slb.Model.V20140515.DescribeZonesResponse response = client.GetAcsResponse(zonesRequest);
                }
                catch (ClientException)
                {
                    continue;
                }
                catch (WebException)
                {
                    continue;
                }*/
                DescribeLoadBalancersRequest request = new DescribeLoadBalancersRequest();
                try
                {
                    DescribeLoadBalancersResponse response = client.GetAcsResponse(request);
                    if (response.LoadBalancers.Count > 0)
                    {
                        foreach (DescribeLoadBalancers_LoadBalancer b in response.LoadBalancers)
                        {
                            DescribeLoadBalancer balancer = new DescribeLoadBalancer(b, App.REGIONS);
                            balancers.Add(balancer);
                        }
                    }
                }
                catch (ClientException)
                {
                }
                catch (WebException)
                {
                }
            });
            Dispatcher.Invoke(new DelegateGot(GotLoadBalancers), balancers);
        }

        private void GotLoadBalancers(object obj)
        {
            ObservableCollection<DescribeLoadBalancer> balancers = obj as ObservableCollection<DescribeLoadBalancer>;
            mLoadBalancers = balancers;
            SLBList.ItemsSource = mLoadBalancers;
            SelectDefaultIndex(SLBList);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(GetLoadBalancers);
            t.Start();
        }

        private void SelectDefaultIndex(ListBox list)
        {
            if (list.Items.Count > 0 && list.SelectedIndex == -1)
            {
                list.SelectedIndex = 0;
            }
        }

        private void SLBList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SLBList.SelectedIndex == -1)
            {
                return;
            }

            SetBalancer(SLBList.SelectedItem as DescribeLoadBalancer);
        }

        private void SetBalancer(DescribeLoadBalancer balancer)
        {
            mSelBalancer = balancer;
            SLBInfo.DataContext = mSelBalancer;
            Thread t = new Thread(GetAll);
            t.Start();
        }

        private void StartedBalancer(object obj)
        {
            DescribeLoadBalancer balancer = obj as DescribeLoadBalancer;
            balancer.LoadBalancerStatus = "active";
        }

        private void StartBalancer(object obj)
        {
            DescribeLoadBalancer balancer = obj as DescribeLoadBalancer;
            IClientProfile profile = DefaultProfile.GetProfile(balancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            SetLoadBalancerStatusRequest request = new SetLoadBalancerStatusRequest();
            request.LoadBalancerId = balancer.LoadBalancerId;
            request.LoadBalancerStatus = "active";
            try
            {
                SetLoadBalancerStatusResponse response = client.GetAcsResponse(request);
                Dispatcher.Invoke(new DelegateGot(StartedBalancer), balancer);
            }
            catch
            {
            }
        }

        private void StoppedBalancer(object obj)
        {
            DescribeLoadBalancer balancer = obj as DescribeLoadBalancer;
            balancer.LoadBalancerStatus = "inactive";
        }

        private void StopBalancer(object obj)
        {
            DescribeLoadBalancer balancer = obj as DescribeLoadBalancer;
            IClientProfile profile = DefaultProfile.GetProfile(balancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            SetLoadBalancerStatusRequest request = new SetLoadBalancerStatusRequest();
            request.LoadBalancerId = balancer.LoadBalancerId;
            request.LoadBalancerStatus = "inactive";
            try
            {
                SetLoadBalancerStatusResponse response = client.GetAcsResponse(request);
                Dispatcher.Invoke(new DelegateGot(StoppedBalancer), balancer);
            }
            catch
            {
            }
        }

        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            Thread t = null;

            if (mSelBalancer.LoadBalancerStatus.Equals("inactive"))
            {
                t = new Thread(new ParameterizedThreadStart(StartBalancer));
                mSelBalancer.LoadBalancerStatus = "inactivating";
            }
            else if (mSelBalancer.LoadBalancerStatus.Equals("active"))
            {
                t = new Thread(new ParameterizedThreadStart(StopBalancer));
                mSelBalancer.LoadBalancerStatus = "activating";
            }

            if (t != null)
            {
                t.IsBackground = true;
                t.Start(mSelBalancer);
            }
        }

        private void EditName_Click(object sender, RoutedEventArgs e)
        {
            IClientProfile profile = DefaultProfile.GetProfile(mSelBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);

            EditNameWindow win = new EditNameWindow(client, mSelBalancer);
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.UpdateEventHandler += UpdateBalancerName;
            win.ShowDialog();
        }

        private void UpdateBalancerName(object sender, string name)
        {
            DescribeLoadBalancer balancer = sender as DescribeLoadBalancer;
            balancer.LoadBalancerName = name;
        }

        private void GotBackendServers(object obj)
        {
            ObservableCollection<DescribeInstance> instances = obj as ObservableCollection<DescribeInstance>;
            if (instances != null)
            {
                mBackendServers = instances;
                BackendServers.ItemsSource = mBackendServers;
                Added.DataContext = mBackendServers;
            }
        }

        private void GotNotAddedServers(object obj)
        {
            ObservableCollection<DescribeInstance> instances = obj as ObservableCollection<DescribeInstance>;
            if (instances != null)
            {
                mNotAddedServers = instances;
                NotAddedServers.ItemsSource = mNotAddedServers;
                NotAdded.DataContext = mNotAddedServers;
            }
        }

        private void RequestServers(DefaultAcsClient client, DescribeLoadBalancerAttributeResponse response)
        {
            ObservableCollection<DescribeInstance> addedServers = new ObservableCollection<DescribeInstance>();
            Parallel.ForEach(response.BackendServers, (s) =>
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
            });
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
                ListenersList.ItemsSource = mListeners;
                ListenersList.DataContext = mListeners;
                ListenersList.SelectedIndex = 0;
            }
        }

        private void GotVServerGroups(object obj)
        {
            mVServerGroups = obj as ObservableCollection<ServerGroup>;
        }

        private void GotMServerGroups(object obj)
        {
            mMServerGroups = obj as ObservableCollection<ServerGroup>;
        }

        private void RequestListeners(DefaultAcsClient client, DescribeLoadBalancerAttributeResponse response)
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
                    l.LoadBalancerId = response.LoadBalancerId;
                    l.RegionId = response.RegionIdAlias;
                    listeners.Add(l);
                }
            }

            DescribeVServerGroupsRequest vrequest = new DescribeVServerGroupsRequest();
            vrequest.LoadBalancerId = response.LoadBalancerId;
            DescribeVServerGroupsResponse vgroups = client.GetAcsResponse(vrequest);
            ObservableCollection<ServerGroup> vServergroups = new ObservableCollection<ServerGroup>();
            foreach (DescribeVServerGroups_VServerGroup g in vgroups.VServerGroups)
            {
                ServerGroup group = new ServerGroup();
                group.ServerGroupId = g.VServerGroupId;
                group.ServerGroupName = g.VServerGroupName;
                vServergroups.Add(group);
            }
            Dispatcher.Invoke(new DelegateGot(GotVServerGroups), vServergroups);

            DescribeMasterSlaveServerGroupsRequest describe = new DescribeMasterSlaveServerGroupsRequest();
            describe.LoadBalancerId = response.LoadBalancerId;
            DescribeMasterSlaveServerGroupsResponse msgroups = client.GetAcsResponse(describe);
            ObservableCollection<ServerGroup> mServergroups = new ObservableCollection<ServerGroup>();
            foreach (DescribeMasterSlaveServerGroups_MasterSlaveServerGroup g in msgroups.MasterSlaveServerGroups)
            {
                ServerGroup group = new ServerGroup();
                group.ServerGroupId = g.MasterSlaveServerGroupId;
                group.ServerGroupName = g.MasterSlaveServerGroupName;
                mServergroups.Add(group);
            }
            Dispatcher.Invoke(new DelegateGot(GotMServerGroups), mServergroups);

            if (listeners.Count > 0)
            {
                DescribeHealthStatusRequest request = new DescribeHealthStatusRequest();
                request.LoadBalancerId = response.LoadBalancerId;
                DescribeHealthStatusResponse status = client.GetAcsResponse(request);

                foreach (SLBListener l in listeners)
                {
                    if (l == null)
                    {
                        continue;
                    }

                    foreach (DescribeHealthStatus_BackendServer server in status.BackendServers)
                    {
                        if (l.ListenerPort == server.ListenerPort)
                        {
                            l.HealthStatus = server.ServerHealthStatus;
                            break;
                        }
                    }

                    if (l.VServerGroupId != null)
                    {
                        foreach (DescribeVServerGroups_VServerGroup group in vgroups.VServerGroups)
                        {
                            if (l.VServerGroupId == group.VServerGroupId)
                            {
                                l.ServerGroupName = group.VServerGroupName;
                                l.ServerGroupNameShow = "[虚拟] " + group.VServerGroupName;
                                break;
                            }
                        }
                    }
                    else if (l.MasterSlaveServerGroupId != null)
                    {
                        foreach (DescribeMasterSlaveServerGroups_MasterSlaveServerGroup group in msgroups.MasterSlaveServerGroups)
                        {
                            if (l.VServerGroupId == group.MasterSlaveServerGroupId)
                            {
                                l.ServerGroupName = group.MasterSlaveServerGroupName;
                                l.ServerGroupNameShow = "[主备] " + group.MasterSlaveServerGroupName;
                                break;
                            }
                        }
                    }
                    else
                    {
                        l.ServerGroupNameShow = "无";
                    }
                }
            }

            Dispatcher.Invoke(new DelegateGot(GotListeners), listeners);
        }

        private DescribeLoadBalancerAttributeResponse GetAttribute(DefaultAcsClient client)
        {
            
            DescribeLoadBalancerAttributeRequest request = new DescribeLoadBalancerAttributeRequest();
            request.LoadBalancerId = mSelBalancer.LoadBalancerId;
            try
            {
                DescribeLoadBalancerAttributeResponse response = client.GetAcsResponse(request);
                return response;
            }
            catch (ClientException)
            {
                return null;
            }
        }

        private void GetAll()
        {
            IClientProfile profile = DefaultProfile.GetProfile(mSelBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeLoadBalancerAttributeResponse response = GetAttribute(client);
            if (response != null)
            {
                RequestServers(client, response);
                RequestListeners(client, response);
            }
        }

        private void GetServers()
        {
            IClientProfile profile = DefaultProfile.GetProfile(mSelBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeLoadBalancerAttributeResponse response = GetAttribute(client);
            if (response != null)
            {
                RequestServers(client, response);
            }
        }

        private void GetListeners()
        {
            IClientProfile profile = DefaultProfile.GetProfile(mSelBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeLoadBalancerAttributeResponse response = GetAttribute(client);
            if (response != null)
            {
                RequestListeners(client, response);
            }
        }

        private void RemovedServers()
        {
            AddedSelectAll.IsChecked = false;
            mBackendServers.Clear();

            Thread t = new Thread(GetServers);
            t.Start();
        }

        private void RemoveServers(object obj)
        {
            IClientProfile profile = DefaultProfile.GetProfile(mSelBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            RemoveBackendServersRequest request = new RemoveBackendServersRequest();
            request.LoadBalancerId = mSelBalancer.LoadBalancerId;
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

            Thread t = new Thread(GetServers);
            t.Start();
        }

        private void AddServers(object obj)
        {
            IClientProfile profile = DefaultProfile.GetProfile(mSelBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            AddBackendServersRequest request = new AddBackendServersRequest();
            request.LoadBalancerId = mSelBalancer.LoadBalancerId;
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
            IClientProfile profile = DefaultProfile.GetProfile(mSelBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            StartLoadBalancerListenerRequest request = new StartLoadBalancerListenerRequest();
            Parallel.ForEach(ports, (port) =>
            {
                try
                {
                    request.LoadBalancerId = mSelBalancer.LoadBalancerId;
                    request.ListenerPort = port;
                    StartLoadBalancerListenerResponse response = client.GetAcsResponse(request);
                    GetListeners();
                }
                catch
                {
                }
            });
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
            IClientProfile profile = DefaultProfile.GetProfile(mSelBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            StopLoadBalancerListenerRequest request = new StopLoadBalancerListenerRequest();
            Parallel.ForEach(ports, (port) =>
            {
                try
                {
                    request.LoadBalancerId = mSelBalancer.LoadBalancerId;
                    request.ListenerPort = port;
                    StopLoadBalancerListenerResponse response = client.GetAcsResponse(request);
                    GetListeners();
                }
                catch
                {
                }
            });
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
            IClientProfile profile = DefaultProfile.GetProfile(mSelBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DeleteLoadBalancerListenerRequest request = new DeleteLoadBalancerListenerRequest();
            Parallel.ForEach(ports, (port) =>
            {
                try
                {
                    request.LoadBalancerId = mSelBalancer.LoadBalancerId;
                    request.ListenerPort = port;
                    DeleteLoadBalancerListenerResponse response = client.GetAcsResponse(request);
                }
                catch
                {
                }
            });
            GetListeners();
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
            AddListenerWindow win = new AddListenerWindow(mSelBalancer);
            ShowAddListenerWindow(win);
        }

        private void ConfigrueListener_Click(object sender, RoutedEventArgs e)
        {
            SLBListener listener = (sender as Button).DataContext as SLBListener;
            AddListenerWindow win = new AddListenerWindow(mSelBalancer, listener);
            ShowAddListenerWindow(win);
        }

        private void ShowAddListenerWindow(AddListenerWindow win)
        {
            win.mVServerGroups = mVServerGroups;
            win.mMServerGroups = mMServerGroups;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.ShowDialog();
        }

        private void UpdateListeners_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(GetListeners);
            t.Start();
        }

        private void GotRules(object obj)
        {
            ObservableCollection<DescribeRule> rules = obj as ObservableCollection<DescribeRule>;
            ForwardRules.ItemsSource = rules;
        }

        private void GetRules(object obj)
        {
            IClientProfile profile = DefaultProfile.GetProfile(mSelBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            SLBListener listener = obj as SLBListener;
            DescribeRulesRequest request = new DescribeRulesRequest();
            request.LoadBalancerId = listener.LoadBalancerId;
            request.ListenerPort = listener.ListenerPort;
            try
            {
                DescribeRulesResponse response = client.GetAcsResponse(request);
                ObservableCollection<DescribeRule> rules = new ObservableCollection<DescribeRule>();
                foreach (DescribeRules_Rule r in response.Rules)
                {
                    DescribeRule rule = new DescribeRule(r);
                    rule.Listener = listener;
                    foreach (ServerGroup group in mVServerGroups)
                    {
                        if (rule.VServerGroupId == group.ServerGroupId)
                        {
                            rule.VServerGroupName = group.ServerGroupName;
                            break;
                        }
                    }
                    rules.Add(rule);
                }

                listener.Rules = rules;
                Dispatcher.Invoke(new DelegateGot(GotRules), rules);
            }
            catch
            {
            }
        }

        private void ListenersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListenersList.SelectedIndex < 0)
            {
                return;
            }

            mSelListener = ListenersList.SelectedItem as SLBListener;
            ListenerInfo.DataContext = mSelListener;
            ForwardRule.DataContext = mSelListener;
            Thread t = new Thread(new ParameterizedThreadStart(GetRules));
            t.Start(mSelListener);
        }

        private void StartStopListener_Click(object sender, RoutedEventArgs e)
        {
            List<int?> ports = new List<int?>();
            SLBListener l = (sender as Button).DataContext as SLBListener;
            ports.Add(l.ListenerPort);
            if (l.Status.Equals("running", StringComparison.CurrentCultureIgnoreCase))
            {
                Thread t = new Thread(new ParameterizedThreadStart(StopListeners));
                t.Start(ports);
            }
            else if (l.Status.Equals("stopped", StringComparison.CurrentCultureIgnoreCase))
            {
                Thread t = new Thread(new ParameterizedThreadStart(StartListeners));
                t.Start(ports);
            }
        }

        private void DeleteListener_Click(object sender, RoutedEventArgs e)
        {
            List<int?> ports = new List<int?>();
            SLBListener l = (sender as Button).DataContext as SLBListener;
            ports.Add(l.ListenerPort);
            Thread t = new Thread(new ParameterizedThreadStart(DeleteListeners));
            t.Start(ports);
        }

        private void EditRule_Click(object sender, RoutedEventArgs e)
        {
            DescribeRule rule = (sender as Button).DataContext as DescribeRule;
            CreateRuleWindow win = new CreateRuleWindow(mSelListener, rule);
            win.mVServerGroups = mVServerGroups;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.ShowDialog();
        }

        private void DeleteRules(object obj)
        {
            DescribeRule rule = obj as DescribeRule;
            string id = "[\"" + rule.RuleId + "\"]";
            IClientProfile profile = DefaultProfile.GetProfile(mSelBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DeleteRulesRequest request = new DeleteRulesRequest();
            request.RuleIds = id;
            try
            {
                DeleteRulesResponse response = client.GetAcsResponse(request);
                GetRules(rule.Listener);
            }
            catch
            {
            }
        }

        private void DeleteRule_Click(object sender, RoutedEventArgs e)
        {
            DescribeRule rule = (sender as Button).DataContext as DescribeRule;
            Thread t = new Thread(new ParameterizedThreadStart(DeleteRules));
            t.IsBackground = true;
            t.Start(rule);
        }

        private void AddRule_Click(object sender, RoutedEventArgs e)
        {
            CreateRuleWindow win = new CreateRuleWindow(mSelListener);
            win.mVServerGroups = mVServerGroups;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
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
