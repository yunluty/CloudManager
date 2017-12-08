using CloudManager.Activation;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace CloudManager
{
    /// <summary>
    /// AboutPage.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : Page
    {
        public MainWindow mMainWindow { get; set; }

        public AboutPage()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            string version = assembly.GetName().Version.ToString();
            Version.Content = version;
            AssemblyCopyrightAttribute copyrightAttribute = (AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute));
            CopyRight.Text = copyrightAttribute.Copyright;
            UpdateKeyLife();
        }

        private void Activate_Click(object sender, RoutedEventArgs e)
        {
            ActivationWindow win = new ActivationWindow();
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.ShowDialog();
            UpdateKeyLife();
        }

        private void UpdateKeyLife()
        {
            var life = new TimeSpan(ActivationApi.KeyLife * ActivationApi.TICKSBYSECOND);
            Days.Content = life.TotalDays.ToString();
            ExpireTime.Content = DateTime.Now.Add(life).ToString("yyyy-MM-dd");
        }
    }
}
