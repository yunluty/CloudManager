using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace CloudManager
{
    /// <summary>
    /// ResetPasswordPage.xaml 的交互逻辑
    /// </summary>
    public partial class ResetPasswordPage : Page
    {
        public string mPassword { get; set; }
        public INextCallBack mNext { get; set; }

        public ResetPasswordPage()
        {
            InitializeComponent();
        }

        private void CheckPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            mNext.NextPage(this);
        }
    }
}
