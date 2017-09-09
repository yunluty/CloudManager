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
using System.Windows.Threading;

namespace CloudManager
{
    /// <summary>
    /// VerifyPhonePage.xaml 的交互逻辑
    /// </summary>
    public partial class VerifyPhonePage : Page
    {
        private int mCount;
        private DispatcherTimer mTimer;
        private string _mPhoneNumber;
        public string mDisplayNumber { get; set; }
        public string mPhoneCode { get; set; }
        public INextCallBack mNext { get; set; }


        public VerifyPhonePage()
        {
            InitializeComponent();
        }

        public string mPhoneNumber
        { get { return _mPhoneNumber; }
          set
            {
                _mPhoneNumber = value;
                UpdatePhoneNumber();
            }
        }

        private void UpdatePhoneNumber()
        {
            string last4 = mPhoneNumber.Substring(mPhoneNumber.Length - 4);
            mDisplayNumber = last4.PadLeft(mPhoneNumber.Length, '*');
            DisplayNumber.DataContext = this;
        }

        private delegate void DelegateGot(object obj);

        private void GotCode(object obj)
        {
            string code = (string)obj;
        }

        private void DoGetCode()
        {
            Thread.Sleep(TimeSpan.FromSeconds(2)); 
            Dispatcher.Invoke(new DelegateGot(GotCode), "88888");
        }

        private void GetCode_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(DoGetCode);
            t.Start();
            GetCodeCountdown();
        }

        private void UpdateCountdown()
        {
            if (mCount > 0)
            {
                GetCode.IsEnabled = false;
                GetCode.Content = mCount.ToString() + 's';
            }
            else
            {
                GetCode.IsEnabled = true;
                GetCode.Content = "获取验证码";
            }
            
        }

        private void TimerTick(object sender, EventArgs e)
        {
            mCount--;
            if (mCount == 0)
            {
                mTimer.Stop();
            }

            UpdateCountdown();
        }

        private void GetCodeCountdown()
        {
            mCount = 60;
            UpdateCountdown();

            mTimer = new DispatcherTimer();
            mTimer.Interval = new TimeSpan(0, 0, 1);
            mTimer.Tick += new EventHandler(TimerTick);
            mTimer.Start();
        }

        private void Code_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            mNext.NextPage(this);
        }
    }
}
