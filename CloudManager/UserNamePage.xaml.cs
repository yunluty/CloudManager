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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CloudManager
{
    /// <summary>
    /// UserNamePage.xaml 的交互逻辑
    /// </summary>
    public partial class UserNamePage : Page
    {
        public string mUserName { get; set; }
        public string mPhoneNumber { get; set; }
        public INextCallBack mNext { get; set; }

        public UserNamePage()
        {
            InitializeComponent();
            UserName.DataContext = this;
        }

        private void DoGetPhoneNumber(object obj)
        {
            string name = (string)obj;
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Dispatcher.Invoke(new DelegateGot(GotPhoneNumber), "13482845956");
        }

        private delegate void DelegateGot(object obj);

        private void GotPhoneNumber(object obj)
        {
            mPhoneNumber = (string)obj;
            mNext.NextPage(this);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            string name = mUserName;
            Thread t = new Thread(new ParameterizedThreadStart(DoGetPhoneNumber));
            t.Start(name);
        }
    }
}
