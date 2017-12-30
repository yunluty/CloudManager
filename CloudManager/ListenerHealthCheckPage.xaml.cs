using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// ListenerHealthCheckPage.xaml 的交互逻辑
    /// </summary>
    public partial class ListenerHealthCheckPage : Page
    {
        private AddListenerParams mParams;

        public AddListenerWindow mOwner { get; set; }

        public ListenerHealthCheckPage()
        {
            InitializeComponent();
        }

        public ListenerHealthCheckPage(AddListenerParams p)
        {
            InitializeComponent();
            mParams = p;
            DataContext = p;
        }

        private void Digital_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (mOwner != null)
            {
                mOwner.PreviousPage();
            }
        }

        private void Submmit_Click(object sender, RoutedEventArgs e)
        {
            if (mOwner != null)
            {
                mOwner.Ensure();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (mOwner != null)
            {
                mOwner.Cancel();
            }
        }

        private void HealthCheckURI_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox.Text.Length == 0)
            {
                textBox.Text += '/';
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (textBox.Text[0] != '/')
            {
                var index = textBox.Text.IndexOf('/');
                if (index > 0)
                {
                    textBox.Text = textBox.Text.Substring(index);
                }
                else
                {
                    textBox.Text = "/";
                }
                textBox.SelectionStart = textBox.Text.Length;
            }
        }
    }
}
