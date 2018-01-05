using CloudManager.Activation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using CloudManager.Domain;
using System.Collections.ObjectModel;

namespace CloudManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        private ObservableCollection<MenuNumberInfo> mTaskMenuInfos;
        private ECSPage mECSPage;
        private SLBPage mSLBPage;       
        private RDSPage mRDSPage;
        private DownUpLoadTaskPage mBackupPage;
        private BucketPage mBucketPage;
        private CertificatePage mCertificatePage;
        private AboutPage mAboutPage;
        private DomainPage mDomainPage;
        private System.Windows.Forms.NotifyIcon mNotifyIcon;

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

            CreateTaskMenuInfo();
            mBackupPage = new DownUpLoadTaskPage();
            mBackupPage.UpdateTaskNumber += UpdateTaskNumber;

            mAboutPage = new AboutPage();
            mAboutPage.mMainWindow = this;

            mDomainPage = new DomainPage();
            mDomainPage.mMainWindow = this;

            SettingMenuList.SelectedIndex = 0;
        }

        private void CreateTaskMenuInfo()
        {
            mTaskMenuInfos = new ObservableCollection<MenuNumberInfo>();
            mTaskMenuInfos.Add(new MenuNumberInfo() { Name = "正在进行", Id = "Running", Number = 0 });
            mTaskMenuInfos.Add(new MenuNumberInfo() { Name = "已完成", Id = "Finished", Number = 0 });
            TaskMenuList.ItemsSource = mTaskMenuInfos;
        }

        private void UpdateTaskNumber(string type, int number)
        {
            try
            {
                foreach (MenuNumberInfo info in mTaskMenuInfos)
                {
                    if (info.Id == type)
                    {
                        info.Number = number;
                        break;
                    }
                }
            }
            catch
            {
            }
        }

        private void Menus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox menuList = sender as ListBox;
            if (menuList.SelectedIndex == -1)
            {
                return;
            }

            MenuNumberInfo menu = menuList.SelectedItem as MenuNumberInfo;
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
            else if (id.Equals("Domains"))
            {
                Process.Content = mDomainPage;
            }
            else if (id.Equals("Running"))
            {
                Process.Content = mBackupPage;
                mBackupPage.TaskType = DownUpLoadTaskPage.TaskStatus.Running;
            }
            else if (id.Equals("Finished"))
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

            if (DomainMenuList != list)
            {
                DomainMenuList.SelectedIndex = -1;
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

        /*protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = false;
        }*/
    }

    public class MenuInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class MenuNumberInfo : INotifyPropertyChanged
    {
        private int number;

        public string Name { get; set; }
        public string Id { get; set; }
        public int Number
        {
            get { return number; }
            set
            {
                number = value;
                NotifyPropertyChanged("Number");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
