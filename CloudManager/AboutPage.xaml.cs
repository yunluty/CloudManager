using CloudManager.Activation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// AboutPage.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : Page
    {
        public const long TICKSBYSECOND = 10000000L;
        public MainWindow mMainWindow { get; set; }

        public AboutPage()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            string version = assembly.GetName().Version.ToString();
            Version.Content = version;
            AssemblyCopyrightAttribute copyrightAttribute = (AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute));
            CopyRight.Text = copyrightAttribute.Copyright;
        }

        private void Activate_Click(object sender, RoutedEventArgs e)
        {
            ActivationWindow win = new ActivationWindow();
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.ActivationEvent += UpdateExpireDate;
            win.ShowDialog();
        }

        private void UpdateExpireDate(object sender, int time)
        {
            var period = new TimeSpan(time * TICKSBYSECOND);
            Days.Content = period.TotalDays.ToString();
        }
    }
}
