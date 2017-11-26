using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Ecs.Model.V20140526;
using Aliyun.Acs.Rds.Model.V20140815;
using Aliyun.Acs.Slb.Model.V20140515;
using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeInstancesResponse;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse;
using static Aliyun.Acs.Rds.Model.V20140815.DescribeDBInstancesResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeLoadBalancersResponse;

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

        private delegate void DelegateGot(object obj);


        public MainWindow()
        {
            InitializeComponent();

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
    }

    public class MenuInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
