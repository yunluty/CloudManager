using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Ecs.Model.V20140526;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeInstancesResponse;

namespace CloudManager
{
    /// <summary>
    /// PasswordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordWindow : WindowBase
    {
        string mAki, mAks, mPassword;
        DescribeInstance mCurrInstance;
        bool mLegal, mMatch;
        string mErrMessage;
        public delegate void PassValuesHandler(object sender, DescribeInstance i);
        public event PassValuesHandler PassValuesEvent;

        public PasswordWindow(DescribeInstance instance)
        {
            InitializeComponent();
            mAki = App.AKI;
            mAks = App.AKS;
            mCurrInstance = instance;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Message.Text = "";
            mPassword = Password1.Password;
            DoResetPassword();
        }

        private void DoResetPassword()
        {
            DoLoadingWork(win =>
            {
                if (mCurrInstance == null)
                {
                    return;
                }
                string regionId = mCurrInstance.RegionId;
                string instanceId = mCurrInstance.InstanceId;
                IClientProfile profile = DefaultProfile.GetProfile(regionId, mAki, mAks);
                DefaultAcsClient client = new DefaultAcsClient(profile);
                ModifyInstanceAttributeRequest request = new ModifyInstanceAttributeRequest();
                request.InstanceId = instanceId;
                request.Password = mPassword;
                ModifyInstanceAttributeResponse response = client.GetAcsResponse(request);
                Dispatcher.Invoke(new Action(ResetSuccess));
            },
            ex =>
            {
                Dispatcher.Invoke(new DelegateResetFail(ResetFail), ex);
            });
            
        }

        private void ResetSuccess()
        {
            if (MessageBox.Show("登录密码重置成功，是否立即重启实例", "重启实例", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                PassValuesEvent(this, mCurrInstance);
            }
            Close();
        }

        private bool CheckPassword(string password, out string message)
        {
            //Regex regex1 = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,30}$");//包含数字，字母，大写字母
            //Regex regex2 = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=([\x21-\x7e]+)[^a-zA-Z0-9]).{8,30}$");//包含数字，字母，符号
            //Regex regex3 = new Regex(@"^(?=.*\d)(?=.*[A-Z])(?=([\x21-\x7e]+)[^a-zA-Z0-9]).{8,30}$");//包含数字，大写字母，符号
            //Regex regex4 = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=([\x21-\x7e]+)[^a-zA-Z0-9]).{8,30}$");//包含字母，大写字母，符号
            Regex regex5 = new Regex(@"^(?![a-zA-Z]+$)(?![A-Z0-9]+$)(?![\x21-\x29\x3a-\x60\x7b-\x7e]+$)(?![a-z0-9]+$)(?![\x21-\x29\x3a-\x40\x5b-\x7e]+$)(?![\x21-\x40\x5b-\x60\x7b-\x7e]+$)[\x21-\x7e]{8,30}$");
            if (password.Length < 8)
            {
                message = "输入密码的字符数少于8个";
                return false;
            }

            //if (regex1.IsMatch(password) || regex2.IsMatch(password) || regex3.IsMatch(password) || regex4.IsMatch(password))
            if (regex5.IsMatch(password))
            {
                message = "";
                return true;
            }
            else
            {
                message = "输入的密码不符合登录密码规则";
                return false;
            }
        }

        private void Password2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            mMatch = Password2.Password.Equals(Password1.Password);
            ShowMessage();
        }

        private void Password1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            mLegal = CheckPassword(Password1.Password, out mErrMessage);
            mMatch = Password2.Password.Equals(Password1.Password);
            ShowMessage();
        }

        private void ShowMessage()
        {
            if (mLegal && mMatch)
            {
                Message.Text = "";
                ResetButton.IsEnabled = true;
            }
            else
            {
                if (mLegal)
                {
                    Message.Text = "两次输入的密码不一致，请重新输入一致的密码";
                }
                else
                {
                    Message.Text = mErrMessage;
                }
                ResetButton.IsEnabled = false;
            }
        }

        public delegate void DelegateResetFail(object obj);//Define delegate

        private void ResetFail(Object obj)
        {
            Message.Text = "重置密码失败，请重试";
        }
    }
}
