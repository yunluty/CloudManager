using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Ecs.Model.V20140526;
using FluentFTP;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeInstanceStatusResponse;

namespace CloudManager
{
    /// <summary>
    /// ECSPage.xaml 的交互逻辑
    /// </summary>
    public partial class ECSPage : Page
    {
        private DescribeInstance _mInstance;
        private SQLiteConnection mSQLiteConnection;
        private FtpInfo mFtpInfo;

        public MainWindow mMainWindow { get; set; }


        public ECSPage(string aki, string aks)
        {
            InitializeComponent();
            ConnectDataBase();
        }

        public DescribeInstance mInstance
        {
            get { return _mInstance; }
            set
            {
                _mInstance = value;
                Information.DataContext = value;

                mFtpInfo = new FtpInfo();
                mFtpInfo.Host = value.PublicIpAddress[0];
                GetFtpInfo(mFtpInfo.Host);
            }
        }

        private void ConnectDataBase()
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
        }

        private void GetFtpInfo(string host)
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
                    Thread t = new Thread(new ParameterizedThreadStart(GetFtpFiles));
                    t.Start(mFtpInfo);
                }
            }
        }

        private void InsertFtpInfo()
        {
            object[] paras = new object[4];
            paras[0] = mFtpInfo.Host;
            paras[1] = mFtpInfo.Port;
            paras[2] = mFtpInfo.Username;
            paras[3] = mFtpInfo.Password;
            int result = SQLiteHelper.ExecuteNonQuery(mSQLiteConnection,
                "INSERT into ftpinfo" +
                " (host, port, username, password)" +
                " VALUES (@host, @port, @username, @password)",
                paras);
            Thread t = new Thread(new ParameterizedThreadStart(GetFtpFiles));
            t.Start(mFtpInfo);
        }

        private void GetInstanceStatus(DescribeInstance instance)
        {
            bool goOn = true;
            int count = 0;
            string regionId = instance.RegionId;
            string instanceId = instance.InstanceId;
            IClientProfile profile = DefaultProfile.GetProfile(regionId, App.AKI, App.AKS);
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
            IClientProfile profile = DefaultProfile.GetProfile(regionId, App.AKI, App.AKS);
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
            IClientProfile profile = DefaultProfile.GetProfile(regionId, App.AKI, App.AKS);
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
            if (mInstance.Status.CompareTo("Running") == 0)
            {
                t = new Thread(new ParameterizedThreadStart(DoStopInstance));
                mInstance.Status = "Stopping";
            }
            else
            {
                t = new Thread(new ParameterizedThreadStart(DoStartInstance));
                mInstance.Status = "Starting";
            }
            t.Start(mInstance);
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
            IClientProfile profile = DefaultProfile.GetProfile(regionId, App.AKI, App.AKS);
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
            mInstance.Status = "Stopping";
            t.Start(mInstance);
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            PasswordWindow win = new PasswordWindow(App.AKI, App.AKS, mInstance);
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

        private void AddFtpInfo_Click(object sender, RoutedEventArgs e)
        {
            InsertFtpInfo();
        }

        private void GetFtpFiles(object obj)
        {
            FtpInfo info = obj as FtpInfo;
            FtpClient client = new FtpClient(info.Host, info.Port, info.Username, info.Password);
            client.DataConnectionType = FtpDataConnectionType.PORT;
            FtpListItem[] items = client.GetListing();
        }
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
