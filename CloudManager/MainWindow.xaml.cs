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
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeInstanceStatusResponse;
using System.Net;
using System.Collections.ObjectModel;

namespace CloudManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string mAki, mAks;
        private List<string> mRegionIds = new List<string>();
        private ObservableCollection<DescribeInstance> mECSInstances = new ObservableCollection<DescribeInstance>();
        private DescribeInstance mSelInstance;
        private ECSPage mECSPage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ECSList.ItemsSource = mECSInstances; //Display the ESCs list
            Thread t = new Thread(GetRegions);
            t.Start();
        }

        public MainWindow(string aki, string aks)
        {
            InitializeComponent();
            mAki = aki;
            mAks = aks;
            mECSPage = new ECSPage(aki, aks);
        }

        private void GetRegions()
        {
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeRegionsRequest request = new DescribeRegionsRequest();
            try
            {
                DescribeRegionsResponse response = client.GetAcsResponse(request);
                foreach (DescribeRegions_Region region in response.Regions)
                {
                    // Save all region IDs
                    mRegionIds.Add(region.RegionId);
                }
            }
            catch (Exception)
            {
            }

            Dispatcher.Invoke(new Action(GotRegions));
        }

        private void GotRegions()
        {
            Thread t = new Thread(GetInstances);
            t.Start();
        }

        private void GetInstances()
        {
            foreach (string id in mRegionIds)
            {
                IClientProfile profile = DefaultProfile.GetProfile(id, mAki, mAks);
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
                            Dispatcher.Invoke(new DelegateGotInstance(GotInstance), instance);
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

        private delegate void DelegateGotInstance(DescribeInstance instance);

        private void GotInstance(DescribeInstance instance)
        {
            mECSInstances.Add(instance);
        }

        private void ECSList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mSelInstance = ECSList.SelectedItem as DescribeInstance;
            if (Information.Content != mECSPage)
            {
                Information.Navigate(mECSPage);
            }
            mECSPage.mInstance = mSelInstance;
            /*if (mSelInstance.Status.CompareTo("Running") == 0)
            {
                Stop.Content = "停止";
            }
            else if (mSelInstance.Status.CompareTo("Stopped") == 0)
            {
                Stop.Content = "启动";
            }*/
        }

        private void RDSList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OSSList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SLBList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DomainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
