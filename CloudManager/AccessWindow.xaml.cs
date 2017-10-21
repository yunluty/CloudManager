﻿using System;
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
        private string mRegion = "cn-shanghai";
        private delegate void DelegateDone(object obj);

        public AccessWindow()
        {
            InitializeComponent();
        }

        private void AccessButton_Click(object sender, RoutedEventArgs e)
        {
            Message.Text = "";
            Thread t = new Thread(new ParameterizedThreadStart(AccessCloud));
            string[] s = new string[2] { AKI.Text, AKS.Text };
            t.Start(s);
        }

        private void AccessCloud(object obj)
        {
            string[] s = obj as string[];
            IClientProfile profile = DefaultProfile.GetProfile(mRegion, s[0], s[1]);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeInstancesRequest request = new DescribeInstancesRequest();

            try
            {
                DescribeInstancesResponse response = client.GetAcsResponse(request);
                Dispatcher.Invoke(new DelegateDone(AccessSuccess), obj);
            }
            catch (ServerException ex)
            {
                //MessageBox.Show(ex.ToString());
                Dispatcher.Invoke(new DelegateDone(AccessFail), ex);
            }
            catch (ClientException ex)
            {
                //MessageBox.Show(ex.ToString());
                Dispatcher.Invoke(new DelegateDone(AccessFail), ex);
            }
        }

        private void AccessSuccess(object obj)
        {
            string[] s = (string[])obj;
            App.AKI = s[0];
            App.AKS = s[1];
            Window win = new MainWindow(s[0], s[1]);
            win.Show();
            Close();
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
                message = "未知错误";
            }
            Message.Text = message;
        }
    }
}
