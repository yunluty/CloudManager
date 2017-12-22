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

namespace CloudManager
{
    /// <summary>
    /// ForgotWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ForgotWindow : WindowBase , INextCallBack
    {
        private string mUserName;
        private string mPhoneNumber;
        private string mPhoneCode;
        private string mPassword;

        public ForgotWindow()
        {
            InitializeComponent();
            UserNamePage page = new UserNamePage();
            page.mNext = this;
            PageContent.Navigate(page);
        }

        public void NextPage(object obj)
        {
            if (typeof(UserNamePage).IsInstanceOfType(obj))
            {
                UserNamePage prePage = (UserNamePage)obj;
                mUserName = prePage.mUserName;
                mPhoneNumber = prePage.mPhoneNumber;

                VerifyPhonePage nextPage = new VerifyPhonePage();
                nextPage.mPhoneNumber = mPhoneNumber;
                nextPage.mNext = this;
                PageContent.Navigate(nextPage);
            }
            else if (typeof(VerifyPhonePage).IsInstanceOfType(obj))
            {
                VerifyPhonePage prePage = (VerifyPhonePage)obj;
                mPhoneCode = prePage.mPhoneCode;

                ResetPasswordPage nextPage = new ResetPasswordPage();
                nextPage.mNext = this;
                PageContent.Navigate(nextPage);
            }
            else if (typeof(ResetPasswordPage).IsInstanceOfType(obj))
            {
                ResetPasswordPage page = (ResetPasswordPage)obj;
                mPassword = page.mPassword;
            }
        }
    }

    public interface INextCallBack
    {
        void NextPage(object obj);
    }
}
