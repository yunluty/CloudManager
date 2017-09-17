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
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeInstancesResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeLoadBalancerAttributeResponse;

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
            AddedSelectAll.DataContext = this;
            NotAddedSelectAll.DataContext = this;
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

        private void GetAttribute()
        {
            IClientProfile profile = DefaultProfile.GetProfile(mBalancer.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeLoadBalancerAttributeRequest request = new DescribeLoadBalancerAttributeRequest();
            request.LoadBalancerId = mBalancer.LoadBalancerId;
            try
            {
                DescribeLoadBalancerAttributeResponse response = client.GetAcsResponse(request);
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
            catch
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

        private void AddedSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            AddedSelectAll.IsChecked = false;
        }

        private void NotAddedSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            NotAddedSelectAll.IsChecked = false;
        }

        private void NotAddedSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (NotAddedSelectAll.IsChecked == true)
            {
                CheckBox checkBox = sender as CheckBox;
                foreach (DescribeInstance i in mNotAddedServers)
                {
                    i.Checked = true;
                }
            }
            else
            {
                CheckBox checkBox = sender as CheckBox;
                foreach (DescribeInstance i in mNotAddedServers)
                {
                    i.Checked = false;
                }
            }
        }

        private void AddedSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (AddedSelectAll.IsChecked == true)
            {
                CheckBox checkBox = sender as CheckBox;
                foreach (DescribeInstance i in mBackendServers)
                {
                    i.Checked = true;
                }
            }
            else
            {
                CheckBox checkBox = sender as CheckBox;
                foreach (DescribeInstance i in mBackendServers)
                {
                    i.Checked = false;
                }
            }
        }
    }
}
