using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse;

namespace CloudManager
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string AKI;
        public static string AKS;
        public static List<DescribeRegions_Region> REGIONS;

        /*public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            RegisterEvents();
            base.OnStartup(e);
        }

        private void RegisterEvents()
        {
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                MessageBox.Show(args.Exception.Message);
                args.SetObserved();
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) => MessageBox.Show("Unhandled exception.");
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Error encountered! Please contact support." + Environment.NewLine + e.Exception.Message);
            //Shutdown(1);
            e.Handled = true;
        }*/
    }
}
