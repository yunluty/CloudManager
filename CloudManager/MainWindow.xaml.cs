using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Ecs.Model.V20140526;
using Aliyun.Acs.Rds.Model.V20140815;
using Aliyun.Acs.Slb.Model.V20140515;
using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeInstancesResponse;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse;
using static Aliyun.Acs.Rds.Model.V20140815.DescribeDBInstancesResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeLoadBalancersResponse;

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
        private ObservableCollection<DescribeDBInstance> mDBInstances = new ObservableCollection<DescribeDBInstance>();
        private DescribeDBInstance mSelDBInstance;
        private RDSPage mRDSPage;
        private BackupTaskPage mBackupPage;
        private BucketPage mBucketPage;
        private ObservableCollection<DescribeBucket> mBuckets = new ObservableCollection<DescribeBucket>();
        private DescribeBucket mSelBucket;

        private delegate void DelegateGot(object obj);


        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(string aki, string aks)
        {
            InitializeComponent();
            mAki = aki;
            mAks = aks;

            ECSList.ItemsSource = mECSInstances; //Display the ECSs list
            SLBList.ItemsSource = mLoadBalancers;
            RDSList.ItemsSource = mDBInstances;
            Thread t = new Thread(GetRegions);
            t.Start();

            mECSPage = new ECSPage(aki, aks);
            mECSPage.mMainWindow = this;
            mSLBPage = new SLBPage(aki, aks);
            mSLBPage.mMainWindow = this;
            mRDSPage = new RDSPage(aki, aks);
            mRDSPage.mMainWindow = this;
            mRDSPage.BackupTaskEvent += new RDSPage.BackupTaskHandler(DoBackupTask);
            mBucketPage = new BucketPage();

            mBackupPage = new BackupTaskPage();
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
            Thread t3 = new Thread(GetDBInstances);
            t3.Start();
            Thread t4 = new Thread(GetBuckets);
            t4.Start();
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

        private void GotDBInstances(object obj)
        {
            DescribeDBInstance instance = obj as DescribeDBInstance;
            if (instance != null)
            {
                mDBInstances.Add(instance);
            }
        }

        private void GetDBInstances()
        {
            foreach (DescribeRegions_Region region in mRegions)
            {
                IClientProfile profile = DefaultProfile.GetProfile(region.RegionId, mAki, mAks);
                DefaultAcsClient client = new DefaultAcsClient(profile);
                DescribeDBInstancesRequest request = new DescribeDBInstancesRequest();
                try
                {
                    DescribeDBInstancesResponse response = client.GetAcsResponse(request);
                    foreach (DescribeDBInstances_DBInstance d in response.Items)
                    {
                        DescribeDBInstance instance = new DescribeDBInstance(d);
                        Dispatcher.Invoke(new DelegateGot(GotDBInstances), instance);
                    }
                }
                catch
                {

                }
            }
        }

        private void GotBuckets(object obj)
        {
            mBuckets = obj as ObservableCollection<DescribeBucket>;
            BucketList.ItemsSource = mBuckets;
        }

        private void GetBuckets()
        {
            ObservableCollection<DescribeBucket> buckets = new ObservableCollection<DescribeBucket>();
            string endpoint = "http://oss-cn-hangzhou.aliyuncs.com";
            OssClient client = new OssClient(endpoint, mAki, mAks);
            foreach (Bucket b in client.ListBuckets())
            {
                DescribeBucket bucket = new DescribeBucket(b);
                buckets.Add(bucket);
            }
            Dispatcher.Invoke(new DelegateGot(GotBuckets), buckets);
        }

        private void ECSList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ECSList.SelectedIndex == -1)
            {
                return;
            }

            if (Process.Content != mECSPage)
            {
                Process.Content = mECSPage;
            }
            mSelInstance = ECSList.SelectedItem as DescribeInstance;
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
            if (RDSList.SelectedIndex == -1)
            {
                return;
            }

            if (Process.Content != mRDSPage)
            {
                Process.Content = mRDSPage;
            }
            mSelDBInstance = RDSList.SelectedItem as DescribeDBInstance;
            mRDSPage.mDBInstance = mSelDBInstance;
            RDSList.SelectedIndex = -1;
        }

        private void BucketList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BucketList.SelectedIndex == -1)
            {
                return;
            }

            if (Process.Content != mBucketPage)
            {
                Process.Content = mBucketPage;
            }
            mSelBucket = BucketList.SelectedItem as DescribeBucket;
            mBucketPage.mBucket = mSelBucket;
        }

        private void BucketList_LostFocus(object sender, RoutedEventArgs e)
        {
            BucketList.SelectedIndex = -1;
        }

        private void SLBList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SLBList.SelectedIndex == -1)
            {
                return;
            }

            mSelBalancer = SLBList.SelectedItem as DescribeLoadBalancer;
            if (Process.Content != mSLBPage)
            {
                Process.Content = mSLBPage;
            }
            mSLBPage.mBalancer = mSelBalancer;
            SLBList.SelectedIndex = -1;
        }

        private void Certificates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DomainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TaskMenus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TaskMenuList.SelectedIndex == -1)
            {
                return;
            }

            if (Process.Content != mBackupPage)
            {
                Process.Content = mBackupPage;
            }

            if (TaskMenuList.SelectedIndex == 0)
            {

            }
        }

        private void DoBackupTask(object obj, BackupTask task)
        {
            mBackupPage.AddNewTask(task);
        }
    }
}
