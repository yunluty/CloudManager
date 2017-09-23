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

namespace CloudManager
{
    /// <summary>
    /// AddListenerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddListenerWindow : Window
    {
        private bool mConfigured;

        public AddListenerWindow()
        {
            InitializeComponent();
        }

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            mConfigured = !mConfigured;
            if (mConfigured)
            {
                Unlimited.Visibility = Visibility.Collapsed;
                BandWidth.Visibility = Visibility.Visible;
                BandM.Visibility = Visibility.Visible;
                Configure.Content = "取消";
            }
            else
            {
                Unlimited.Visibility = Visibility.Visible;
                BandWidth.Visibility = Visibility.Collapsed;
                BandM.Visibility = Visibility.Collapsed;
                Configure.Content = "配置";
            }
        }

        private void BandWidth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void AddListener_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    class Schedulers
    {
        public string SchedulerId { get; set; }
        public string SchedulerName { get; set; }
    }
}
