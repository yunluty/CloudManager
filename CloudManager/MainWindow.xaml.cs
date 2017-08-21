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

namespace CloudManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string mAki, mAks;
        List<string> mRegionIds = new List<string>();
        List<DescribeInstances_Instance> mECSInstances = new List<DescribeInstances_Instance>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Thread t = new Thread(GetRegions);
            t.Start();
        }

        public MainWindow(string aki, string aks)
        {
            InitializeComponent();
            mAki = aki;
            mAks = aks;
        }

        private void GetRegions()
        {
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient();
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
                DefaultAcsClient client = new DefaultAcsClient();
                DescribeInstancesRequest request = new DescribeInstancesRequest();
                try
                {
                    DescribeInstancesResponse response = client.GetAcsResponse(request);
                    if (response.Instances.Count > 0)
                    {
                        mECSInstances.AddRange(response.Instances);
                    }
                }
                catch (ClientException)
                {
                    continue;
                }
            }

            Dispatcher.Invoke(new Action(GotInstances));
        }

        private void ECSList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DescribeInstances_Instance instance = ECSList.SelectedItem as DescribeInstances_Instance;
            
        }

        private void GotInstances()
        {
            ECSList.ItemsSource = mECSInstances; //Display the ESCs list

        }
    }
}
