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


namespace CloudManager
{
    /// <summary>
    /// AccessWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AccessWindow : Window
    {
        string mAki;
        string mAks;
        string mRegion = "cn-shanghai";

        public AccessWindow()
        {
            InitializeComponent();
        }

        private void AccessButton_Click(object sender, RoutedEventArgs e)
        {
            mAki = AKI.Text;
            mAks = AKS.Text;

            Thread t = new Thread(new ParameterizedThreadStart(AccessCloud));
            string[] s = new string[2] { mAki, mAks };
            t.Start(s);
        }

        private void AccessCloud(object ob)
        {
            string[] s = (string[])ob;
            IClientProfile profile = DefaultProfile.GetProfile(mRegion, s[0], s[1]);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeInstancesRequest request = new DescribeInstancesRequest();

            try
            {
                DescribeInstancesResponse response = client.GetAcsResponse(request);
                Dispatcher.Invoke(AccessSuccess);
            }
            catch (ServerException ex)
            {
                //MessageBox.Show(ex.ToString());
                Dispatcher.Invoke(new delegateAccessFail(AccessFail), ex);
            }
            catch (ClientException ex)
            {
                //MessageBox.Show(ex.ToString());
                Dispatcher.Invoke(new delegateAccessFail(AccessFail), ex);
            }
        }

        private void AccessSuccess()
        {
            Window win = new MainWindow(mAki, mAks);
            win.Show();
            Close();
        }

        public delegate void delegateAccessFail(object obj);//Define delegate

        private void AccessFail(object obj)
        {
            if (typeof(ServerException).IsInstanceOfType(obj))
            {
                //Server Exception
            }
            else if (typeof(ClientException).IsInstanceOfType(obj))
            {
                //Client Exception
            }
        }
    }
}
