using RestSharp;
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
using System.Windows.Shapes;

namespace CloudManager.Activation
{
    /// <summary>
    /// ActivationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ActivationWindow : Window
    {
        public class ResponseLife
        {
            public int Time { get; set; }
        }

        private RestClient mRestClient = new RestClient("http://39.108.229.176/");


        public ActivationWindow()
        {
            InitializeComponent();
            
        }

        private void Activate_Click(object sender, RoutedEventArgs e)
        {
            string aki = String.Copy(App.AKI);
            string key = String.Copy(ActivationKey.Text);

            ErrorInfo.Text = "";
            Task.Run(() =>
            {
                try
                {
                    int time = ActivationApi.ActivateKey(aki, key);
                    Dispatcher.Invoke(() =>
                    {
                        this.Close();
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ErrorInfo.Text = ex.Message;
                    });
                }
            });
        }
    }
}
