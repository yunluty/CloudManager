using CloudManager.Activation;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CloudManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ECSPage mECSPage;
        private SLBPage mSLBPage;       
        private RDSPage mRDSPage;
        private DownUpLoadTaskPage mBackupPage;
        private BucketPage mBucketPage;
        private CertificatePage mCertificatePage;
        private AboutPage mAboutPage;

        private delegate void DelegateGot(object obj);


        public MainWindow()
        {
            InitializeComponent();

            AKS.Content = App.AKI;
            ExpireDate.Content = DateTime.Now.Add(new TimeSpan(ActivationApi.KeyLife * ActivationApi.TICKSBYSECOND)).ToString("yyyy-MM-dd");

            mECSPage = new ECSPage();
            mECSPage.mMainWindow = this;

            mSLBPage = new SLBPage();
            mSLBPage.mMainWindow = this;

            mRDSPage = new RDSPage();
            mRDSPage.mMainWindow = this;
            mRDSPage.BackupTaskEvent += DoBackupTask;

            mBucketPage = new BucketPage();
            mBucketPage.BackupTaskEvent += DoBackupTask;
            mBucketPage.mMainWindow = this;

            mCertificatePage = new CertificatePage();
            mCertificatePage.mMainWindow = this;

            mBackupPage = new DownUpLoadTaskPage();

            mAboutPage = new AboutPage();
            mAboutPage.mMainWindow = this;
        }

        private void Menus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox menuList = sender as ListBox;
            if (menuList.SelectedIndex == -1)
            {
                return;
            }

            MenuInfo menu = menuList.SelectedItem as MenuInfo;
            string id = menu.Id;
            
            if (id.Equals("ECSInstances"))
            {
                Process.Content = mECSPage;
            }
            else if (id.Equals("RDSInstances"))
            {
                Process.Content = mRDSPage;
            }
            else if (id.Equals("OSSInstances"))
            {
                Process.Content = mBucketPage;
            }
            else if (id.Equals("SLBInstances"))
            {
                Process.Content = mSLBPage;
            }
            else if (id.Equals("Certificates"))
            {
                Process.Content = mCertificatePage;
            }
            else if (id.Equals("RunningTask"))
            {
                Process.Content = mBackupPage;
                mBackupPage.TaskType = DownUpLoadTaskPage.TaskStatus.Running;
            }
            else if (id.Equals("FinishedTask"))
            {
                Process.Content = mBackupPage;
                mBackupPage.TaskType = DownUpLoadTaskPage.TaskStatus.Finished;
            }
            else if (id.Equals("About"))
            {
                Process.Content = mAboutPage;
            }

            ClearSelectIndex(menuList);
        }

        private void ClearSelectIndex(ListBox list)
        {
            if (ECSMenuList != list)
            {
                ECSMenuList.SelectedIndex = -1;
            }

            if (RDSMenuList != list)
            {
                RDSMenuList.SelectedIndex = -1;
            }

            if (OSSMenuList != list)
            {
                OSSMenuList.SelectedIndex = -1;
            }

            if (SLBMenuList != list)
            {
                SLBMenuList.SelectedIndex = -1;
            }

            if (TaskMenuList != list)
            {
                TaskMenuList.SelectedIndex = -1;
            }

            if (SettingMenuList != list)
            {
                SettingMenuList.SelectedIndex = -1;
            }
        }

        private void DoBackupTask(object obj, DownUploadTask task)
        {
            mBackupPage.AddNewTask(task);
        }

        private void Logoff_Click(object sender, RoutedEventArgs e)
        {
            App.AKI = null;
            App.AKS = null;
            App.REGIONS = null;
            AccessWindow win = new AccessWindow();
            win.Show();
            this.Close();
        }

        private void Website_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.anquan.info");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    public class MenuInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
