﻿using System;
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

namespace CloudManager
{
    /// <summary>
    /// SignUpWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SignUpWindow : WindowBase
    {
        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UserName.Text.Length > 4)
            {
                UserNameLegal.Source = new BitmapImage(new Uri("images/legal.png", UriKind.Relative));
                UserNameLegal.Visibility = Visibility.Visible;
            }
            else if (UserName.Text.Length > 0)
            {
                UserNameLegal.Source = new BitmapImage(new Uri("images/illegal.png", UriKind.Relative));
                UserNameLegal.Visibility = Visibility.Visible;
            }
            else
            {
                UserNameLegal.Visibility = Visibility.Hidden;
            }
        }

        private void PhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PhoneNumber.Text.Length > 10)
            {
                PhoneNumberLegal.Source = new BitmapImage(new Uri("images/legal.png", UriKind.Relative));
                PhoneNumber.Visibility = Visibility.Visible;
            }
            else if (PhoneNumber.Text.Length > 0)
            {
                PhoneNumberLegal.Source = new BitmapImage(new Uri("images/illegal.png", UriKind.Relative));
                PhoneNumberLegal.Visibility = Visibility.Visible;
            }
            else
            {
                PhoneNumberLegal.Visibility = Visibility.Hidden;
            }
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            /*Regex regex1 = new Regex(@"^(?=.*\d)(?=.*[a-zA-Z]){6,20}$");
              Regex regex2 = new Regex(@"^(?=.*\d)(?=([\x21-\x7e]+)[^a-zA-Z0-9]){6,20}$");
              Regex regex3 = new Regex(@"^(?=.*[a-zA-Z])(?=([\x21-\x7e]+)[^a-zA-Z0-9]){6,20}$");*/
            Regex regex = new Regex(@"^(?![a-zA-Z]+$)(?![0-9]+$)(?![\x21-\x29\x3a-\x40\x5b-\x60\x7b-\x7e])[\x21-\x7e]{6,20}$");

            if (Password.Password.Length > 5 && regex.IsMatch(Password.Password))
            {
                
                PasswordLegal.Source = new BitmapImage(new Uri("images/legal.png", UriKind.Relative));
                PasswordLegal.Visibility = Visibility.Visible;
            }
            else if (Password.Password.Length > 0)
            {
                PasswordLegal.Source = new BitmapImage(new Uri("images/illegal.png", UriKind.Relative));
                PasswordLegal.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordLegal.Visibility = Visibility.Hidden;
            }
        }

        private void CheckPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (CheckPassword.Password.Length > 0)
            {
                if (CheckPassword.Password.Equals(Password.Password))
                {
                    CheckPasswordLegal.Source = new BitmapImage(new Uri("images/legal.png", UriKind.Relative));
                    CheckPasswordLegal.Visibility = Visibility.Visible;
                }
                else
                {
                    CheckPasswordLegal.Source = new BitmapImage(new Uri("images/illegal.png", UriKind.Relative));
                    CheckPasswordLegal.Visibility = Visibility.Visible;
                }
            }
            else
            {
                CheckPasswordLegal.Visibility = Visibility.Hidden;
            }
        }

        private void PhoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void DoSignUp()
        {

        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(DoSignUp);
            t.Start();
        }

        private void GotCode()
        {

        }

        private void DoGetCode(object obj)
        {
            string number = (string)obj;
        }

        private void GetCode_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(DoGetCode));
            t.Start(PhoneNumber.Text);
        }

        private void Code_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
