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

using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Ecs.Model.V20140526;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeInstancesResponse;
using System.Net;
using System.Collections.ObjectModel;
using Aliyun.Acs.Slb.Model.V20140515;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeLoadBalancersResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeZonesResponse;

namespace CloudManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string mAki, mAks;
        private List<DescribeRegions_Region> mRegions = new List<DescribeRegions_Region>();
        private ObservableCollection<DescribeInstance> mECSInstances = new ObservableCollection<DescribeInstance>();
        private DescribeInstance mSelInstance;
        private ECSPage mECSPage;
        private ObservableCollection<DescribeLoadBalancer> mLoadBalancers = new ObservableCollection<DescribeLoadBalancer>();
        private DescribeLoadBalancer mSelBalancer;
        private SLBPage mSLBPage;
        private SLBServersPage mSLBServersPage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ECSList.ItemsSource = mECSInstances; //Display the ECSs list
            SLBList.ItemsSource = mLoadBalancers;
            Thread t = new Thread(GetRegions);
            t.Start();
        }

        public MainWindow(string aki, string aks)
        {
            InitializeComponent();
            mAki = aki;
            mAks = aks;
            mECSPage = new ECSPage(aki, aks);
            mSLBPage = new SLBPage(aki, aks);
            mSLBServersPage = new SLBServersPage(aki, aks);
        }

        private void GetRegions()
        {
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsRequest request = new Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsRequest();
            try
            {
                Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse response = client.GetAcsResponse(request);
                mRegions.AddRange(response.Regions);
            }
            catch (Exception)
            {
            }

            Dispatcher.Invoke(new Action(GotRegions));
        }

        private void GotRegions()
        {
            Thread t1 = new Thread(GetInstances);
            t1.Start();
            Thread t2 = new Thread(GetLoadBalancers);
            t2.Start();
        }

        private void GetInstances()
        {
            foreach (DescribeRegions_Region region in mRegions)
            {
                IClientProfile profile = DefaultProfile.GetProfile(region.RegionId, mAki, mAks);
                DefaultAcsClient client = new DefaultAcsClient(profile);
                DescribeInstancesRequest request = new DescribeInstancesRequest();
                try
                {
                    DescribeInstancesResponse response = client.GetAcsResponse(request);
                    if (response.Instances.Count > 0)
                    {
                        foreach (DescribeInstances_Instance i in response.Instances)
                        {
                            DescribeInstance instance = new DescribeInstance(i);
                            Dispatcher.Invoke(new DelegateGot(GotInstances), instance);
                        }
                    }
                }
                catch (ClientException)
                {
                    continue;
                }
                catch (WebException)
                {
                    continue;
                }
            }
        }

        private delegate void DelegateGot(object obj);

        private void GotInstances(object obj)
        {
            DescribeInstance instance = obj as DescribeInstance;
            if (instance != null)
            {
                mECSInstances.Add(instance);
            }
        }

        private void GetLoadBalancers()
        {
            foreach (DescribeRegions_Region region in mRegions)
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
                            DescribeLoadBalancer balancer = new DescribeLoadBalancer(b, mRegions);
                            Dispatcher.Invoke(new DelegateGot(GotLoadBalancers), balancer);
                        }
                    }
                }
                catch (ClientException)
                {
                    continue;
                }
                catch (WebException)
                {
                    continue;
                }
            }
        }

        private void GotLoadBalancers(object obj)
        {
            DescribeLoadBalancer balancer = obj as DescribeLoadBalancer;
            if (balancer != null)
            {
                mLoadBalancers.Add(balancer);
            }
        }

        private void ECSList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ECSList.SelectedIndex == -1)
            {
                return;
            }
            mSelInstance = ECSList.SelectedItem as DescribeInstance;
            if (Information.Content != mECSPage)
            {
                Information.Content = mECSPage;
            }

            Process.Content = null;
            
            mECSPage.mInstance = mSelInstance;
            /*if (mSelInstance.Status.CompareTo("Running") == 0)
            {
                Stop.Content = "停止";
            }
            else if (mSelInstance.Status.CompareTo("Stopped") == 0)
            {
                Stop.Content = "启动";
            }*/
            ECSList.SelectedIndex = -1;
        }

        private void RDSList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void OSSList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SLBList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SLBList.SelectedIndex == -1)
            {
                return;
            }

            mSelBalancer = SLBList.SelectedItem as DescribeLoadBalancer;
            if (Information.Content != mSLBPage)
            {
                Information.Content = mSLBPage;
            }

            if (Information.Content != mSLBServersPage)
            {
                Process.Content = mSLBServersPage;
            }
            mSLBPage.mBalancer = mSelBalancer;
            mSLBServersPage.mBalancer = mSelBalancer;
            SLBList.SelectedIndex = -1;
        }

        private void DomainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
