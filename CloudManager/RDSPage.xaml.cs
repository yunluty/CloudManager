using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Rds.Model.V20140815;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static Aliyun.Acs.Rds.Model.V20140815.DescribeBackupsResponse;
using static Aliyun.Acs.Rds.Model.V20140815.DescribeDBInstanceAttributeResponse;
using static Aliyun.Acs.Rds.Model.V20140815.DescribeParametersResponse;
using static Aliyun.Acs.Rds.Model.V20140815.DescribeParameterTemplatesResponse;

namespace CloudManager
{
    /// <summary>
    /// RDSPage.xaml 的交互逻辑
    /// </summary>
    public partial class RDSPage : Page
    {
        private string mAki, mAks;
        private ObservableCollection<DBParameter> mParameters;
        private ObservableCollection<DBBackup> mBackups;
        private DBBackupPolicy mPolicy;

        public MainWindow mMainWindow { get; set; }
        public delegate void DelegateGot(object obj);
        public delegate void BackupTaskHandler(object sender, BackupTask task);
        public event BackupTaskHandler BackupTaskEvent;


        public RDSPage()
        {
            InitializeComponent();
        }

        public RDSPage(string aki, string aks)
        {
            InitializeComponent();
            mAki = aki;
            mAks = aks;
        }

        private DescribeDBInstance _mDBInstance;
        public DescribeDBInstance mDBInstance
        {
            get { return _mDBInstance; }
            set
            {
                _mDBInstance = value;
                Thread t1 = new Thread(new ParameterizedThreadStart(GetAttribute));
                t1.Start(value);
                Thread t2 = new Thread(new ParameterizedThreadStart(GetParameters));
                t2.Start(value);
                Thread t3 = new Thread(new ParameterizedThreadStart(GetBackups));
                t3.Start(value);
            }
        }

        private void GotAttribute(object obj)
        {
            DescribeDBInstanceAttribute_DBInstanceAttribute attribute = obj as DescribeDBInstanceAttribute_DBInstanceAttribute;
            mDBInstance.ConnectionString = attribute.ConnectionString;
            mDBInstance.Port = attribute.Port;
            mDBInstance.DBInstanceClassType = attribute.DBInstanceClassType;
            mDBInstance.DBInstanceCPU = attribute.DBInstanceCPU;
            mDBInstance.DBInstanceMemory = attribute.DBInstanceMemory;
            mDBInstance.MaxIOPS = attribute.MaxIOPS;
            mDBInstance.MaxConnections = attribute.MaxConnections;
            mDBInstance.DBInstanceClass = attribute.DBInstanceClass;
            mDBInstance.MaintainTime = attribute.MaintainTime;
            mDBInstance.DBInstanceStorage = attribute.DBInstanceStorage;
            RDSInfo.DataContext = mDBInstance;
        }

        private void GetAttribute(object obj)
        {
            DescribeDBInstance db = obj as DescribeDBInstance;
            IClientProfile profile = DefaultProfile.GetProfile(db.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeDBInstanceAttributeRequest request = new DescribeDBInstanceAttributeRequest();
            request.DBInstanceId = db.DBInstanceId;
            try
            {
                DescribeDBInstanceAttributeResponse response = client.GetAcsResponse(request);
                Dispatcher.Invoke(new DelegateGot(GotAttribute), response.Items[0]);
            }
            catch (Exception ex)
            {

            }
        }

        private void GotParameters(object obj)
        {
            mParameters = obj as ObservableCollection<DBParameter>;
            Parameters.ItemsSource = mParameters;
        }

        private void ModifyParameters(object obj)
        {
            int count = 0;
            bool forcerestart = false;
            string parameters = "{";
            ObservableCollection<DBParameter> modify = obj as ObservableCollection<DBParameter>;

            foreach (DBParameter p in modify)
            {
                if (p.ForceRestart)
                {
                    forcerestart = true;
                }
                if (count != 0)
                {
                    parameters += ",";
                }
                parameters += "\"" + p.Name + "\":\"" + p.OperationValue + "\"";
                count++;
            }
            parameters += "}";

            IClientProfile profile = DefaultProfile.GetProfile(mDBInstance.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            ModifyParameterRequest request = new ModifyParameterRequest();
            request.DBInstanceId = mDBInstance.DBInstanceId;
            request.Parameters = parameters;
            request.Forcerestart = forcerestart;
            try
            {
                ModifyParameterResponse response = client.GetAcsResponse(request);
                foreach (DBParameter p in modify)
                {
                    p.RunningValue = p.OperationValue;
                    p.Changed = false;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<DBParameter> modify = new ObservableCollection<DBParameter>();

            foreach (DBParameter p in mParameters)
            {
                if (p.Changed == true)
                {
                    modify.Add(p);
                    break;
                }
            }

            if (modify.Count > 0)
            {
                Thread t = new Thread(ModifyParameters);
                t.Start(modify);
            }
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            foreach (DBParameter p in mParameters)
            {
                if (p.Changed == true)
                {
                    p.OperationValue = p.RunningValue;
                }
            }
        }

        private void EditValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            BindingExpression be = tb.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
            /*DBParameter para = tb.DataContext as DBParameter;
            if (tb.Text != para.RunningValue)
            {
                Regex regex = new Regex(para.CheckingCode);
                if (regex.IsMatch(tb.Text))
                {
                    para.LegalValue = true;
                }
                else
                {
                    para.LegalValue = false;
                }
                para.Changed = true;
            }
            else
            {
                para.Changed = false;
            }*/
        }

        private void GetParameters(object obj)
        {
            DescribeDBInstance db = obj as DescribeDBInstance;
            IClientProfile profile = DefaultProfile.GetProfile(db.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeParameterTemplatesRequest r1 = new DescribeParameterTemplatesRequest();
            r1.Engine = db.Engine;
            r1.EngineVersion = db.EngineVersion;
            DescribeParametersRequest r2 = new DescribeParametersRequest();
            r2.DBInstanceId = db.DBInstanceId;
            try
            {
                DescribeParameterTemplatesResponse resp1 = client.GetAcsResponse(r1);
                DescribeParametersResponse resp2 = client.GetAcsResponse(r2);
                ObservableCollection<DBParameter> parameters = new ObservableCollection<DBParameter>();
                foreach (DescribeParameterTemplates_TemplateRecord record in resp1.Parameters)
                {
                    bool boolValue;
                    DBParameter para = new DBParameter();
                    para.DBInstanceId = db.DBInstanceId;
                    para.Name = record.ParameterName;
                    para.DefaultValue = record.ParameterValue;
                    bool.TryParse(record.ForceModify, out boolValue);
                    para.ForceModify = boolValue;
                    bool.TryParse(record.ForceRestart, out boolValue);
                    para.ForceRestart = boolValue;
                    para.CheckingCode = record.CheckingCode;
                    para.Description = record.ParameterDescription;

                    foreach (DescribeParameters_DBInstanceParameter running in resp2.RunningParameters)
                    {
                        if (para.Name.Equals(running.ParameterName))
                        {
                            para.RunningValue = running.ParameterValue;
                            para.OperationValue = running.ParameterValue;
                            resp2.RunningParameters.Remove(running);
                            break;
                        }
                    }
                    parameters.Add(para);
                }
                Dispatcher.Invoke(new DelegateGot(GotParameters), parameters);
            }
            catch (Exception ex)
            {
            }
        }

        private void GotBackups(object obj)
        {
            mBackups = obj as ObservableCollection<DBBackup>;
            Backups.ItemsSource = mBackups;
        }

        private string GetFileNameByUrl(string url)
        {
            int start = url.LastIndexOf('/') + 1;
            int end = url.IndexOf('?');
            return url.Substring(start, end - start);
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            DBBackup backup = (sender as Button).DataContext as DBBackup;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "选择保存位置";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                BackupTask task = new BackupTask();
                task.InstanceType = "RDS";
                task.InstanceID = mDBInstance.DBInstanceId;
                task.InstanceName = mDBInstance.DBInstanceDescription;
                task.URL = backup.BackupDownloadURL;
                task.SavePath = dialog.FileName;
                task.FileName = GetFileNameByUrl(task.URL);
                task.FileType = "File";
                BackupTaskEvent?.Invoke(this, task);
            }
        }

        private void DownloadURL_Click(object sender, RoutedEventArgs e)
        {
            DBBackup backup = (sender as Button).DataContext as DBBackup;
            Clipboard.SetText(backup.BackupDownloadURL);
        }

        private void IntranetDownloadURL_Click(object sender, RoutedEventArgs e)
        {
            DBBackup backup = (sender as Button).DataContext as DBBackup;
            Clipboard.SetText(backup.BackupIntranetDownloadURL);
        }

        private void GetBackups(object obj)
        {
            DescribeDBInstance db = obj as DescribeDBInstance;
            IClientProfile profile = DefaultProfile.GetProfile(db.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeBackupsRequest r1 = new DescribeBackupsRequest();
            r1.AcceptFormat = Aliyun.Acs.Core.Http.FormatType.JSON;
            r1.DBInstanceId = db.DBInstanceId;
            DateTime end = DateTime.UtcNow;
            r1.EndTime = end.ToString("yyy-MM-ddThh:mmZ");
            DateTime start = end.AddDays(-7);
            r1.StartTime = start.ToString("yyy-MM-ddThh:mmZ");
            try
            {
                DescribeBackupsResponse resp1 = client.GetAcsResponse(r1);
                ObservableCollection<DBBackup> backups = new ObservableCollection<DBBackup>();
                foreach (DescribeBackups_Backup b in resp1.Items)
                {
                    DBBackup backup = new DBBackup(b);
                    backups.Add(backup);
                }
                Dispatcher.Invoke(new DelegateGot(GotBackups), backups);

                GetBackupPolicy(db);
            }
            catch (Exception ex)
            {

            }
        }

        private void GotBackupPolicy(object obj)
        {
            mPolicy = obj as DBBackupPolicy;
            BackupPolicy.DataContext = mPolicy;
        }

        private void GetBackupPolicy(object obj)
        {
            DescribeDBInstance db = obj as DescribeDBInstance;
            IClientProfile profile = DefaultProfile.GetProfile(db.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            DescribeBackupPolicyRequest request = new DescribeBackupPolicyRequest();
            request.DBInstanceId = db.DBInstanceId;
            try
            {
                DescribeBackupPolicyResponse response = client.GetAcsResponse(request);
                DBBackupPolicy policy = new DBBackupPolicy(response);
                policy.DBInstanceId = db.DBInstanceId;
                Dispatcher.Invoke(new DelegateGot(GotBackupPolicy), policy);
            }
            catch (Exception ex)
            {
            }
        }

        private void EditPolicy_Click(object sender, RoutedEventArgs e)
        {
            IClientProfile profile = DefaultProfile.GetProfile(mDBInstance.RegionId, mAki, mAks);
            DefaultAcsClient client = new DefaultAcsClient(profile);
            EditBackupPolicyWindow win = new EditBackupPolicyWindow(mPolicy, client);
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.ShowDialog();
        }
    }

    public class DBParameter : INotifyPropertyChanged
    {
        public string DBInstanceId { get; set; }
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        private string _RunningValue;
        public string RunningValue
        {
            get { return _RunningValue; }
            set
            {
                _RunningValue = value;
                NotifyPropertyChanged("RunningValue");
            }
        }
        private string _OperationValue;
        public string OperationValue
        {
            get { return _OperationValue; }
            set
            {
                _OperationValue = value;
                if (RunningValue != value)
                {
                    try
                    {
                        Regex regex = new Regex(CheckingCode);
                        if (regex.IsMatch(value))
                        {
                            LegalValue = true;
                        }
                        else
                        {
                            LegalValue = false;
                        }
                    }
                    catch 
                    {
                    }
                    Changed = true;
                }
                else
                {
                    Changed = false;
                }
                NotifyPropertyChanged("OperationValue");
            }
        }
        public bool ForceModify { get; set; }
        public bool ForceRestart { get; set; }
        public string CheckingCode { get; set; }
        public string Description { get; set; }
        private bool? _LegalValue;
        public bool? LegalValue
        {
            get { return _LegalValue; }
            set
            {
                _LegalValue = value;
                NotifyPropertyChanged("LegalValue");
            }
        }
        private bool? _Changed;
        public bool? Changed
        {
            get { return _Changed; }
            set
            {
                _Changed = value;
                NotifyPropertyChanged("Changed");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DBBackup : INotifyPropertyChanged
    {
        public DBBackup(DescribeBackups_Backup b)
        {
            BackupId = b.BackupId;
            DBInstanceId = b.DBInstanceId;
            BackupStatus = b.BackupStatus;
            BackupStartTime = b.BackupStartTime;
            BackupEndTime = b.BackupEndTime;
            BackupType = b.BackupType;
            BackupMode = b.BackupMode;
            BackupMethod = b.BackupMethod;
            BackupDownloadURL = b.BackupDownloadURL;
            BackupIntranetDownloadURL = b.BackupIntranetDownloadURL;
            BackupLocation = b.BackupLocation;
            BackupExtractionStatus = b.BackupExtractionStatus;
            BackupScale = b.BackupScale;
            BackupDBNames = b.BackupDBNames;
            TotalBackupSize = b.TotalBackupSize;
            BackupSize = b.BackupSize;
            HostInstanceID = b.HostInstanceID;
            StoreStatus = b.StoreStatus;
        }

        public string BackupId { get; set; }

        public string DBInstanceId { get; set; }

        public string BackupStatus { get; set; }

        private string backupStartTime;
        public string BackupStartTime
        {
            get { return backupStartTime; }
            set
            {
                DateTime.TryParse(value, out DateTime dt);
                backupStartTime = dt.ToString("yyyy-MM-dd hh:mm:ss");
            }
        }

        private string backupEndTime;
        public string BackupEndTime
        {
            get { return backupEndTime; }
            set
            {
                DateTime.TryParse(value, out DateTime dt);
                backupEndTime = dt.ToString("yyyy-MM-dd hh:mm:ss");
            }
        }

        public string BackupType { get; set; }

        public string BackupMode { get; set; }

        public string BackupMethod { get; set; }

        public string BackupDownloadURL { get; set; }

        public string BackupIntranetDownloadURL { get; set; }

        public string BackupLocation { get; set; }

        public string BackupExtractionStatus { get; set; }

        public string BackupScale { get; set; }

        public string BackupDBNames { get; set; }

        public long? TotalBackupSize { get; set; }

        private long? backupSize;
        public long? BackupSize
        {
            get { return backupSize; }
            set
            {
                backupSize = value;
                if (value >= 1024 * 1024)
                {
                    BackupSizeStr = ((double)value / (1024 * 1024)).ToString("0.00") + " MB";
                }
                else if (value >= 1024)
                {
                    BackupSizeStr = ((double)value / 1024).ToString("0.00") + " KB";
                }
                else
                {
                    BackupSizeStr = value + " B";
                }
            }
        }

        public string BackupSizeStr { get; set; }

        public string HostInstanceID { get; set; }

        public string StoreStatus { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DBBackupPolicy
    {
        public DBBackupPolicy(DescribeBackupPolicyResponse r)
        {
            BackupRetentionPeriod = r.BackupRetentionPeriod.ToString();
            PreferredNextBackupTime = r.PreferredNextBackupTime;
            PreferredBackupTime = r.PreferredBackupTime;
            PreferredBackupPeriod = r.PreferredBackupPeriod;
            BackupLog = r.BackupLog;
            LogBackupRetentionPeriod = r.LogBackupRetentionPeriod.ToString();
        }

        public DBBackupPolicy(DBBackupPolicy p)
        {
            DBInstanceId = p.DBInstanceId;
            BackupRetentionPeriod = p.BackupRetentionPeriod;
            PreferredNextBackupTime = p.PreferredNextBackupTime;
            PreferredBackupTime = p.PreferredBackupTime;
            PreferredBackupPeriod = p.PreferredBackupPeriod;
            BackupLog = p.BackupLog;
            LogBackupRetentionPeriod = p.LogBackupRetentionPeriod;
        }

        private string backupRetentionPeriod;
        private string preferredNextBackupTime;
        private string preferredBackupTime;
        private string preferredBackupPeriod;
        private string backupLog;
        private string logBackupRetentionPeriod;

        public string DBInstanceId { get; set; }

        public string BackupRetentionPeriod
        {
            get
            {
                return backupRetentionPeriod;
            }
            set
            {
                backupRetentionPeriod = value;
            }
        }

        public string PreferredNextBackupTime //Local time
        {
            get
            {
                return preferredNextBackupTime;
            }
            set
            {
                if (value.IndexOf('Z') >= 0 && value.IndexOf('T') >= 0) //UTC time, convert to local time
                {
                    DateTime.TryParse(value, out DateTime dt);
                    preferredNextBackupTime = dt.ToString("yyyy-MM-dd hh:mm:ss");
                }
                else
                {
                    preferredNextBackupTime = value;
                }
            }
        }

        public string PreferredBackupTime //Local time
        {
            get
            {
                return preferredBackupTime;
            }
            set
            {
                if (value.IndexOf('Z') >= 0) //UTC time, convert to local time
                {
                    int index = value.LastIndexOf('-');
                    string start = value.Substring(0, index - 1) + ":00"; //The last char is 'Z'
                    string end = value.Substring(index + 1, value.Length - index - 2) + ":00";
                    TimeZoneInfo info = TimeZoneInfo.Local;

                    TimeSpan.TryParse(start, out TimeSpan t1);
                    TimeSpan.TryParse(end, out TimeSpan t2);
                    t1 += info.BaseUtcOffset;
                    t2 += info.BaseUtcOffset;

                    TimeSpan t3 = TimeSpan.FromDays(1);

                    if (t1 < TimeSpan.FromSeconds(0))
                    {
                        t1 += TimeSpan.FromDays(1);
                    }
                    else if (t1 >= TimeSpan.FromDays(1))
                    {
                        t1 -= TimeSpan.FromDays(1);
                    }

                    if (t2 < TimeSpan.FromSeconds(0))
                    {
                        t2 += TimeSpan.FromDays(1);
                    }
                    else if (t2 >= TimeSpan.FromDays(1))
                    {
                        t2 -= TimeSpan.FromDays(1);
                    }

                    preferredBackupTime = t1.ToString().Substring(0, 5) + '-' + t2.ToString().Substring(0, 5);
                }
                else
                {
                    preferredBackupTime = value;
                }
            }
        }

        public string PreferredBackupPeriod
        {
            get
            {
                return preferredBackupPeriod;
            }
            set
            {
                preferredBackupPeriod = value;
                PreferredBackupPeriodCN = "";
                if (value.Contains("Monday"))
                {
                    Monday = true;
                    PreferredBackupPeriodCN += "周一 ";
                }
                if (value.Contains("Tuesday"))
                {
                    Tuesday = true;
                    PreferredBackupPeriodCN += "周二 ";
                }
                if (value.Contains("Wednesday"))
                {
                    Wednesday = true;
                    PreferredBackupPeriodCN += "周三 ";
                }
                if (value.Contains("Thursday"))
                {
                    Thursday = true;
                    PreferredBackupPeriodCN += "周四 ";
                }
                if (value.Contains("Friday"))
                {
                    Friday = true;
                    PreferredBackupPeriodCN += "周五 ";
                }
                if (value.Contains("Saturday"))
                {
                    Saturday = true;
                    PreferredBackupPeriodCN += "周六 ";
                }
                if (value.Contains("Sunday"))
                {
                    Sunday = true;
                    PreferredBackupPeriodCN += "周日 ";
                }
            }
        }

        public string PreferredBackupPeriodCN { get; set; }

        public string BackupLog
        {
            get
            {
                return backupLog;
            }
            set
            {
                backupLog = value;
                if (value.Equals("Enable", StringComparison.CurrentCultureIgnoreCase))
                {
                    Enable = true; //For Enable button
                    Disable = false; //For Disable button
                }
                else
                {
                    Enable = false;
                    Disable = true;
                }
            }
        }

        public string LogBackupRetentionPeriod
        {
            get
            {
                return logBackupRetentionPeriod;
            }
            set
            {
                logBackupRetentionPeriod = value;
            }
        }

        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public bool Enable { get; set; }
        public bool Disable { get; set; }

        public string GetBackupPeriod()
        {
            string period = "";
            if (Monday)
            {
                period += "Monday,";
            }
            if (Tuesday)
            {
                period += "Tuesday,";
            }
            if (Wednesday)
            {
                period += "Wednesday,";
            }
            if (Thursday)
            {
                period += "Thursday,";
            }
            if (Friday)
            {
                period += "Friday,";
            }
            if (Saturday)
            {
                period += "Saturday,";
            }
            if (Sunday)
            {
                period += "Sunday,";
            }
            period = period.Substring(0, period.Length - 1); //Rmove the last ','
            return period;
        }
        
        public string GetBackupTimeUTC()
        {
            int index = PreferredBackupTime.LastIndexOf('-');
            string start = PreferredBackupTime.Substring(0, index) + ":00"; //The last char is 'Z'
            string end = PreferredBackupTime.Substring(index + 1, PreferredBackupTime.Length - index - 1) + ":00";
            TimeZoneInfo info = TimeZoneInfo.Local;

            TimeSpan.TryParse(start, out TimeSpan t1);
            TimeSpan.TryParse(end, out TimeSpan t2);
            t1 -= info.BaseUtcOffset;
            t2 -= info.BaseUtcOffset;

            if (t1 < TimeSpan.FromSeconds(0))
            {
                t1 += TimeSpan.FromDays(1);
            }
            else if (t1 >= TimeSpan.FromDays(1))
            {
                t1 -= TimeSpan.FromDays(1);
            }

            if (t2 < TimeSpan.FromSeconds(0))
            {
                t2 += TimeSpan.FromDays(1);
            }
            else if (t2 >= TimeSpan.FromDays(1))
            {
                t2 -= TimeSpan.FromDays(1);
            }

            return t1.ToString().Substring(0, 5) + 'Z' + '-' + t2.ToString().Substring(0, 5) + 'Z';
        }
    }
}
