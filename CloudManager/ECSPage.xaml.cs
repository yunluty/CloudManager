using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Ecs.Model.V20140526;
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
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeInstanceStatusResponse;

namespace CloudManager
{
    /// <summary>
    /// ECSPage.xaml 的交互逻辑
    /// </summary>
    public partial class ECSPage : Page
    {
        private string mAki, mAks;
        private DescribeInstance _mInstance;

        public MainWindow mMainWindow { get; set; }

        public ECSPage()
        {
            InitializeComponent();
        }

        public ECSPage(string aki, string aks)
        {
            InitializeComponent();
            mAki = aki;
            mAks = aks;
        }

        public DescribeInstance mInstance
        {
            get { return _mInstance; }
            set
            {
                _mInstance = value;
                Information.DataContext = value;
            }
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

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Thread t;
            if (mInstance.Status.CompareTo("Running") == 0)
            {
                t = new Thread(new ParameterizedThreadStart(DoStopInstance));
                mInstance.Status = "Stopping";
            }
            else
            {
                t = new Thread(new ParameterizedThreadStart(DoStartInstance));
                mInstance.Status = "Starting";
            }
            t.Start(mInstance);
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
            mInstance.Status = "Stopping";
            t.Start(mInstance);
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            PasswordWindow win = new PasswordWindow(mAki, mAks, mInstance);
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.PassValuesEvent += new PasswordWindow.PassValuesHandler(ReceivedReboot);
            win.ShowDialog();
        }

        private void ReceivedReboot(object sender, DescribeInstance instance)
        {
            Thread t = new Thread(new ParameterizedThreadStart(DoRebootInstance));
            instance.Status = "Stopping";
            t.Start(instance);
        }
    }
}
