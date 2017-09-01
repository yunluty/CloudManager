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
        string mAki, mAks;
        List<string> mRegionIds = new List<string>();
        ObservableCollection<DescribeInstance> mECSInstances = new ObservableCollection<DescribeInstance>();
        DescribeInstance mSelInstance;

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
            DescribeInstance instance = ECSList.SelectedItem as DescribeInstance;
            Information.DataContext = mSelInstance = instance;
            /*if (mSelInstance.Status.CompareTo("Running") == 0)
            {
                Stop.Content = "停止";
            }
            else if (mSelInstance.Status.CompareTo("Stopped") == 0)
            {
                Stop.Content = "启动";
            }*/
        }

        private void GetInstanceStatus(DescribeInstance instance)
        {
            bool goOn = true;
            int count = 0;
            string regionId = instance.RegionId;
            string instanceId = instance.InstanceId;
            IClientProfile profile = DefaultProfile.GetProfile(regionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeInstanceStatusRequest request = new DescribeInstanceStatusRequest();

            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                try
                {
                    DescribeInstanceStatusResponse reponse = client.GetAcsResponse(request);
                    foreach (DescribeInstanceStatus_InstanceStatus status in reponse.InstanceStatuses)
                    {
                        if (status.InstanceId == instanceId)
                        {
                            instance.Status = status.Status;
                            if (status.Status.Equals("Stopped") || status.Status.Equals("Running"))
                            {
                                goOn = false;
                            }
                            break;
                        }
                    }
                }
                catch
                {

                }

                count++;   
            } while (goOn && count <= 10);
        }

        private void DoStopInstance(Object obj)
        {
            DescribeInstance instance = obj as DescribeInstance;
            if (instance == null)
            {
                return;
            }

            string regionId = instance.RegionId;
            string instanceId = instance.InstanceId;
            IClientProfile profile = DefaultProfile.GetProfile(regionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            StopInstanceRequest request = new StopInstanceRequest();
            request.InstanceId = instanceId;
            try
            {
                StopInstanceResponse response = client.GetAcsResponse(request);
            }
            catch (ClientException)
            {

            }
            GetInstanceStatus(instance);
        }

        private void DoStartInstance(Object obj)
        {
            DescribeInstance instance = obj as DescribeInstance;
            if (instance == null)
            {
                return;
            }

            string regionId = instance.RegionId;
            string instanceId = instance.InstanceId;
            IClientProfile profile = DefaultProfile.GetProfile(regionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            StartInstanceRequest request = new StartInstanceRequest();
            request.InstanceId = instanceId;
            try
            {
                StartInstanceResponse response = client.GetAcsResponse(request);
            }
            catch (ClientException)
            {

            }
            GetInstanceStatus(instance);
        }

        private void DisableInstanceButton()
        {
            Stop.IsEnabled = false;
            Reboot.IsEnabled = false;
            ResetPassword.IsEnabled = false;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Thread t;
            if (mSelInstance.Status.CompareTo("Running") == 0)
            {
                t = new Thread(new ParameterizedThreadStart(DoStopInstance));
                mSelInstance.Status = "Stopping";
            }
            else
            {
                t = new Thread(new ParameterizedThreadStart(DoStartInstance));
                mSelInstance.Status = "Starting";
            }
            t.Start(mSelInstance);
            DisableInstanceButton();
        }

        private void DoRebootInstance(object obj)
        {
            DescribeInstance instance = obj as DescribeInstance;
            if (instance == null)
            {
                return;
            }

            string regionId = instance.RegionId;
            string instanceId = instance.InstanceId;
            IClientProfile profile = DefaultProfile.GetProfile(regionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            RebootInstanceRequest request = new RebootInstanceRequest();
            request.InstanceId = instanceId;
            try
            {
                RebootInstanceResponse response = client.GetAcsResponse(request);
            }
            catch (ClientException)
            {

            }
            GetInstanceStatus(instance);
        }

        private void Reboot_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(DoRebootInstance));
            mSelInstance.Status = "Stopping";
            t.Start(mSelInstance);
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            PasswordWindow win = new PasswordWindow(mAki, mAks, mSelInstance);
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = this;
            win.PassValuesEvent += new PasswordWindow.PassValuesHandler(ReceivedReboot);
            win.ShowDialog();
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

        private void ReceivedReboot(object sender, DescribeInstance instance)
        {
            Thread t = new Thread(new ParameterizedThreadStart(DoRebootInstance));
            instance.Status = "Stopping";
            t.Start(instance);
        }
    }
}
