using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace CloudManager
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : WindowBase
    {
        private LoginedData mLoginedData;

        public LoginWindow()
        {
            InitializeComponent();
            mLoginedData = new LoginedData();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            string user = SettingsUtils.GetSetting("userName");
            if (!string.IsNullOrEmpty(user))
            {
                Username.Text = user;
                string pwd = mLoginedData.GetPasswordByUser(user);
                if (!string.IsNullOrEmpty(pwd))
                {
                    Password.Password = pwd;
                    Remember.IsChecked = true;
                    string value = SettingsUtils.GetSetting("isAutoLogin");
                    bool check = false;
                    if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out check) && check)
                    {
                        Auto.IsChecked = true;
                        //StartLogin();
                    }
                }
            }
        }

        private void DoLogin()
        {
            //模拟登录过程等待
            Thread.Sleep(TimeSpan.FromSeconds(1));
            Dispatcher.Invoke(new Action(LoginSuccess));
        }

        private void LoginSuccess()
        {
            var user = Username.Text;
            var pwd = Password.Password;

            SettingsUtils.UpdateSetting("userName", user);

            if (Remember.IsChecked == true)
            {
                mLoginedData.SaveLoginedData(user, pwd);
                if (Auto.IsChecked == true)
                {
                    SettingsUtils.UpdateSetting("isAutoLogin", true.ToString());
                }
            }
            else
            {
                mLoginedData.SaveLoginedData(user);
            }

            Window win = new AccessWindow();
            win.Show();
            Close();
        }

        private void LoginFail(int cause)
        {
            Password.Password = "";
            Message.Text = "你输入的账号或者密码不正确";
            EnableControl();
        }

        private void DisableControls()
        {
            Username.IsEnabled = false;
            Password.IsEnabled = false;
            Auto.IsEnabled = false;
            Remember.IsEnabled = false;
            LoginButton.IsEnabled = false;
            LoginButton.Content = "登录中...";
        }

        private void EnableControl()
        {
            Username.IsEnabled = true;
            Password.IsEnabled = true;
            Auto.IsEnabled = true;
            Remember.IsEnabled = true;
            LoginButton.IsEnabled = true;
            LoginButton.Content = "登录";
        }

        private void StartLogin()
        {
            DisableControls();
            Thread t = new Thread(DoLogin);
            t.Start();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Username.Text))
            {
                Message.Text = "请输入账号后再登录";
                return;
            }

            if (string.IsNullOrEmpty(Password.Password))
            {
                Message.Text = "请输入密码后再登录";
                return;
            }

            Message.Text = "";
            StartLogin();
        }

        private void AutoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Remember.IsChecked == false)
            {
                Remember.IsChecked = true;
            }
        }

        private void RememberCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Auto.IsChecked == true)
            {
                Auto.IsChecked = false;
            }
        }

        private void Username_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (UserHint.Visibility == Visibility.Visible)
            {
                UserHint.Visibility = Visibility.Hidden;
            }
        }

        private void Username_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (UserHint.Visibility != Visibility.Visible && string.IsNullOrEmpty(Username.Text))
            {
                UserHint.Visibility = Visibility.Visible;
            }
        }

        private void Password_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (PasswordHint.Visibility == Visibility.Visible)
            {
                PasswordHint.Visibility = Visibility.Hidden;
            }
        }

        private void Password_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (PasswordHint.Visibility != Visibility.Visible && string.IsNullOrEmpty(Password.Password))
            {
                PasswordHint.Visibility = Visibility.Visible;
            }
        }

        private void Username_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Username.Text))
            {
                UserHint.Visibility = Visibility.Hidden;
            }
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Password.Password))
            {
                PasswordHint.Visibility = Visibility.Hidden;
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow win = new SignUpWindow();
            win.ShowDialog();
        }

        private void Forgot_Click(object sender, RoutedEventArgs e)
        {
            ForgotWindow win = new ForgotWindow();
            win.ShowDialog();
        }
    }
}
