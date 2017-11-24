using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Ecs.Model.V20140526;
using FluentFTP;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeInstancesResponse;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeInstanceStatusResponse;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse;

namespace CloudManager
{
    /// <summary>
    /// ECSPage.xaml 的交互逻辑
    /// </summary>
    public partial class ECSPage : Page
    {
        private string mAki, mAks;
        private ObservableCollection<DescribeInstance> mECSInstances = new ObservableCollection<DescribeInstance>();
        private DescribeInstance mSelInstance;
        //private SQLiteConnection mSQLiteConnection;
        //private FtpInfo mFtpInfo;
        //private DescribeFtpObject mCurrDirectory;
        //private string mDeepestPath;
        private delegate void DelegateGot(object obj);

        public MainWindow mMainWindow { get; set; }


        public ECSPage()
        {
            InitializeComponent();
            //ConnectDataBase();
            mAki = App.AKI;
            mAks = App.AKS;
            ECSList.ItemsSource = mECSInstances; //Display the ECSs list
            Thread t1 = new Thread(GetInstances);
            t1.Start();
        }

        private void GetInstances()
        {
            foreach (DescribeRegions_Region region in App.REGIONS)
            {
                IClientProfile profile = DefaultProfile.GetProfile(region.RegionId, mAki, mAks);
                DefaultAcsClient client = new DefaultAcsClient(profile);
                DescribeInstancesRequest request = new DescribeInstancesRequest();
                try
                {
                    DescribeInstancesResponse response = client.GetAcsResponse(request);
                    if (response.Instances.Count > 0)
                    {
                        foreach (DescribeInstances_Instance i in response.Instances)
                        {
                            DescribeInstance instance = new DescribeInstance(i);
                            Dispatcher.Invoke(new DelegateGot(GotInstances), instance);
                        }
                    }
                }
                catch (ClientException)
                {
                    continue;
                }
                catch (WebException)
                {
                    continue;
                }
            }
        }

        private void GotInstances(object obj)
        {
            DescribeInstance instance = obj as DescribeInstance;
            if (instance != null)
            {
                mECSInstances.Add(instance);
                SelectDefaultIndex();
            }
        }

        private void SelectDefaultIndex()
        {
            if (ECSList.SelectedIndex == -1)
            {
                ECSList.SelectedIndex = 0;
            }
        }

        private void ECSList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ECSList.SelectedIndex == -1)
            {
                return;
            }

            SetInstance(ECSList.SelectedItem as DescribeInstance);
            /*if (mSelInstance.Status.CompareTo("Running") == 0)
            {
                Stop.Content = "停止";
            }
            else if (mSelInstance.Status.CompareTo("Stopped") == 0)
            {
                Stop.Content = "启动";
            }*/
        }

        private void SetInstance(DescribeInstance instance)
        {
            mSelInstance = instance;
            Information.DataContext = mSelInstance;
        }

        /*private void ConnectDataBase()
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dir += "\\CloudManager";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var path = dir + "\\cloud.db";
            mSQLiteConnection = new SQLiteConnection("Data Source=" + path);
            SQLiteHelper.ExecuteNonQuery(mSQLiteConnection, 
                "CREATE TABLE IF NOT EXISTS ftpinfo" +
                "(id INTEGER PRIMARY KEY," +
                " host TEXT, port INTEGER," +
                " username TEXT, password TEXT)", null);
        }*/

        /*private void GetFtpInfo(string host)
        {
            object[] paras = new object[1];
            paras[0] = host;
            DataSet data = SQLiteHelper.ExecuteDataSet(mSQLiteConnection, "SELECT * FROM ftpinfo WHERE host = @host", paras);
            DataRowCollection rows = data.Tables[0].Rows;
            if (rows.Count == 0)
            {
                mFtpInfo.Port = 21;
                mFtpInfo.PortStr = mFtpInfo.Port.ToString();
                FTPInfo.DataContext = mFtpInfo;

                FTPInfo.Visibility = Visibility.Visible;
                FtpFiles.Visibility = Visibility.Collapsed;
            }
            else
            {
                var port = rows[0]["port"] as long?;
                mFtpInfo.Port = (int)port;
                mFtpInfo.PortStr = mFtpInfo.Port.ToString();
                mFtpInfo.Username = (string)rows[0]["username"];
                mFtpInfo.Password = (string)rows[0]["password"];
                if (!String.IsNullOrEmpty(mFtpInfo.Username) && !String.IsNullOrEmpty(mFtpInfo.Password))
                {
                    StartGetFtpFiles(null);

                    FTPInfo.Visibility = Visibility.Collapsed;
                    FtpFiles.Visibility = Visibility.Visible;
                }
                else
                {
                    mFtpInfo.PortStr = mFtpInfo.Port.ToString();
                    FTPInfo.DataContext = mFtpInfo;

                    FTPInfo.Visibility = Visibility.Visible;
                    FtpFiles.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void InsertFtpInfo(FtpInfo info)
        {
            object[] paras = new object[4];
            paras[0] = info.Host;
            paras[1] = info.Port;
            paras[2] = info.Username;
            paras[3] = info.Password;
            int result = SQLiteHelper.ExecuteNonQuery(mSQLiteConnection,
                "INSERT into ftpinfo" +
                " (host, port, username, password)" +
                " VALUES (@host, @port, @username, @password)",
                paras);

            FTPInfo.Visibility = Visibility.Collapsed;
            FtpFiles.Visibility = Visibility.Visible;

            if (info.Host == mFtpInfo.Host)
            {
                StartGetFtpFiles(null);
            }
        }

        private void StartGetFtpFiles(DescribeFtpObject obj)
        {
            if (obj == null)
            {
                obj = new DescribeFtpObject();
            }
            
            FtpClient client = new FtpClient(mFtpInfo.Host, mFtpInfo.Port, mFtpInfo.Username, mFtpInfo.Password);
            client.DataConnectionType = FtpDataConnectionType.PORT;
            obj.Client = client;
            Thread t = new Thread(new ParameterizedThreadStart(GetFtpFiles));
            t.Start(obj);
        }*/

        private void GetInstanceStatus(DescribeInstance instance)
        {
            bool goOn = true;
            int count = 0;
            string regionId = instance.RegionId;
            string instanceId = instance.InstanceId;
            IClientProfile profile = DefaultProfile.GetProfile(regionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeInstanceStatusRequest request = new DescribeInstanceStatusRequest();

            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                try
                {
                    DescribeInstanceStatusResponse reponse = client.GetAcsResponse(request);
                    foreach (DescribeInstanceStatus_InstanceStatus status in reponse.InstanceStatuses)
                    {
                        if (status.InstanceId == instanceId)
                        {
                            instance.Status = status.Status;
                            if (status.Status.Equals("Stopped") || status.Status.Equals("Running"))
                            {
                                goOn = false;
                            }
                            break;
                        }
                    }
                }
                catch
                {
                }

                count++;
            } while (goOn && count <= 12);
        }

        private void DoStopInstance(Object obj)
        {
            DescribeInstance instance = obj as DescribeInstance;
            if (instance == null)
            {
                return;
            }

            string regionId = instance.RegionId;
            string instanceId = instance.InstanceId;
            IClientProfile profile = DefaultProfile.GetProfile(regionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            StopInstanceRequest request = new StopInstanceRequest();
            request.InstanceId = instanceId;
            try
            {
                StopInstanceResponse response = client.GetAcsResponse(request);
            }
            catch (ClientException)
            {

            }
            GetInstanceStatus(instance);
        }

        private void DoStartInstance(Object obj)
        {
            DescribeInstance instance = obj as DescribeInstance;
            if (instance == null)
            {
                return;
            }

            string regionId = instance.RegionId;
            string instanceId = instance.InstanceId;
            IClientProfile profile = DefaultProfile.GetProfile(regionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            StartInstanceRequest request = new StartInstanceRequest();
            request.InstanceId = instanceId;
            try
            {
                StartInstanceResponse response = client.GetAcsResponse(request);
            }
            catch (ClientException)
            {
            }
            GetInstanceStatus(instance);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Thread t;
            if (mSelInstance.Status.CompareTo("Running") == 0)
            {
                t = new Thread(new ParameterizedThreadStart(DoStopInstance));
                mSelInstance.Status = "Stopping";
            }
            else
            {
                t = new Thread(new ParameterizedThreadStart(DoStartInstance));
                mSelInstance.Status = "Starting";
            }
            t.Start(mSelInstance);
        }

        private void DoRebootInstance(object obj)
        {
            DescribeInstance instance = obj as DescribeInstance;
            if (instance == null)
            {
                return;
            }

            string regionId = instance.RegionId;
            string instanceId = instance.InstanceId;
            IClientProfile profile = DefaultProfile.GetProfile(regionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            RebootInstanceRequest request = new RebootInstanceRequest();
            request.InstanceId = instanceId;
            try
            {
                RebootInstanceResponse response = client.GetAcsResponse(request);
            }
            catch (ClientException)
            {

            }
            GetInstanceStatus(instance);
        }

        private void Reboot_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(DoRebootInstance));
            mSelInstance.Status = "Stopping";
            t.Start(mSelInstance);
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            PasswordWindow win = new PasswordWindow(mSelInstance);
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.PassValuesEvent += new PasswordWindow.PassValuesHandler(ReceivedReboot);
            win.ShowDialog();
        }

        private void ReceivedReboot(object sender, DescribeInstance instance)
        {
            Thread t = new Thread(new ParameterizedThreadStart(DoRebootInstance));
            instance.Status = "Stopping";
            t.Start(instance);
        }

        /*private void AddFtpInfo_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(CheckFtpInfo));
            t.Start(mFtpInfo);
        }

        private void CheckedFtpInfo(object obj)
        {
            FtpInfo info = obj as FtpInfo;
            InsertFtpInfo(info);
        }

        private void CheckedFtpInfoFail(object obj)
        {
            FtpInfo info = obj as FtpInfo;
        }

        private void CheckFtpInfo(object obj)
        {
            FtpInfo info = obj as FtpInfo;
            FtpClient client = new FtpClient(info.Host, info.Port, info.Username, info.Password);
            try
            {
                client.Connect();
                Dispatcher.Invoke(new DelegateGot(CheckedFtpInfo), info);
            }
            catch
            {
                Dispatcher.Invoke(new DelegateGot(CheckedFtpInfoFail), info);
            }
        }

        private void ClearFileList()
        {
            if (mCurrDirectory != null && mCurrDirectory.ChildObjects != null)
            {
                mCurrDirectory.ChildObjects.Clear();
            }
        }

        private void GotFtpFiles(object obj)
        {
            DescribeFtpObject directory = obj as DescribeFtpObject;
            if (mCurrDirectory != null && mCurrDirectory != directory)
            {
                mCurrDirectory.ChildObjects.Clear();
            }
            mCurrDirectory = directory;
            FtpFilesList.ItemsSource = mCurrDirectory.ChildObjects;
            BackupFiles.DataContext = mCurrDirectory;
            if (!mDeepestPath.Contains(mCurrDirectory.Path))
            {
                mDeepestPath = mCurrDirectory.Path;
            }
        }

        private void GetFtpFiles(object obj)
        {
            DescribeFtpObject parent = obj as DescribeFtpObject;
            try
            {
                FtpListItem[] items = parent.Client.GetListing(parent.Path);
                ObservableCollection<DescribeFtpObject> objects = new ObservableCollection<DescribeFtpObject>();
                foreach (FtpListItem item in items)
                {
                    DescribeFtpObject o = new DescribeFtpObject(item);
                    o.ParentObject = parent;
                    objects.Add(o);
                }
                parent.ChildObjects = objects;
                Dispatcher.Invoke(new DelegateGot(GotFtpFiles), parent);
            }
            catch
            {
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrDirectory.ParentObject != null)
            {
                StartGetFtpFiles(mCurrDirectory.ParentObject);
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (mDeepestPath != null && mDeepestPath != mCurrDirectory.Path)
            {
                int index = -1;
                string path = null;

                if (mDeepestPath.Contains(mCurrDirectory.Path))
                {
                    string next = mDeepestPath.Substring(mCurrDirectory.Path.Length);
                    index = next.IndexOf('/');
                    if (index > 0)
                    {
                        next = next.Substring(0, index);
                    }
                    path = mCurrDirectory.Path + next;
                }

                foreach (DescribeFtpObject obj in mCurrDirectory.ChildObjects)
                {
                    if (path != null && path == obj.Path)
                    {
                        StartGetFtpFiles(obj);
                        break;
                    }
                }
            }
        }

        private void FtpFilesList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListView view = sender as ListView;
            DescribeFtpObject obj = view.SelectedItem as DescribeFtpObject;
            if (obj.ObjectType == DescribeFtpObject.FtpObjectType.Directory)
            {
                StartGetFtpFiles(obj);
            }
        }

        private void CreateBackup_Click(object sender, RoutedEventArgs e)
        {
            BackupTaskWindow win = new BackupTaskWindow();
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = this.mMainWindow;
            win.ShowDialog();
        }*/
    }

    public class FtpInfo
    {
        public string Host { get; set; }
        private string _PortStr;
        public string PortStr
        {
            get { return _PortStr; }
            set
            {
                _PortStr = value;

                int port = 0;
                int.TryParse(value, out port);
                Port = port;
            }
        }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
