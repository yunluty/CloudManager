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
using System.Windows.Shapes;
using System.Windows.Threading;

using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Ecs.Model.V20140526;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse;
using CloudManager.Activation;

namespace CloudManager
{
    /// <summary>
    /// AccessWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AccessWindow : WindowBase
    {
        private string mRegion = "cn-shanghai";
        private delegate void DelegateDone(object obj);


        public AccessWindow()
        {
            InitializeComponent();
        }

        private void AccessButton_Click(object sender, RoutedEventArgs e)
        {
            Message.Text = "";
            AccessButton.IsEnabled = false;
            AccessCloud();
        }

        private void AccessCloud()
        {
            string aki = String.Copy(AKI.Text);
            string aks = String.Copy(AKS.Text);
            IClientProfile profile = DefaultProfile.GetProfile(mRegion, aki, aks);
            DefaultAcsClient client = new DefaultAcsClient(profile);

            DoLoadingWork(win =>
            {
                DescribeInstancesRequest request = new DescribeInstancesRequest();
                DescribeInstancesResponse response = client.GetAcsResponse(request);

                Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsRequest rr = new Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsRequest();
                Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse rs = client.GetAcsResponse(rr);
                List<DescribeRegions_Region> regions = new List<DescribeRegions_Region>();
                regions.AddRange(rs.Regions);
                App.AKI = aki;
                App.AKS = aks;
                App.REGIONS = regions;

                //Thread.Sleep(TimeSpan.FromSeconds(10));

                int time = ActivationApi.GetKeyLife(aki);
                Dispatcher.Invoke(() =>
                {
                    AccessButton.IsEnabled = true;
                    if (time > ActivationApi.RunCondition)
                    {
                        StartMainWindow();
                    }
                    else
                    {
                        StartActivationWindow();
                    }
                });
            },
            ex =>
            {
                Dispatcher.Invoke(new DelegateDone(AccessFail), ex);
            });
        }

        private void StartMainWindow()
        {
            Window win = new MainWindow();
            win.Show();
            this.Close();
        }

        private void StartActivationWindow()
        {
            ActivationWindow win = new ActivationWindow();
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = this;
            win.ShowDialog();
            if (ActivationApi.KeyLife > ActivationApi.RunCondition)
            {
                StartMainWindow();
            }
            else
            {
                AccessButton.IsEnabled = true;
            }
        }

        private void AccessFail(object obj)
        {
            string message;
            if (typeof(ServerException).IsInstanceOfType(obj))
            {
                //Server Exception
                message = ExceptionMessage.GetErrorMessage((ServerException)obj);
            }
            else if (typeof(ClientException).IsInstanceOfType(obj))
            {
                //Client Exception
                message = ExceptionMessage.GetErrorMessage((ClientException)obj);
            }
            else
            {
                message = ((Exception)obj).Message;
            }
            Message.Text = message;
            AccessButton.IsEnabled = true;
        }
    }
}
