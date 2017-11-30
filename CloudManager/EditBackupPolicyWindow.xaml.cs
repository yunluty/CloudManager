using Aliyun.Acs.Core;
using Aliyun.Acs.Rds.Model.V20140815;
using System;
using System.Threading;
using System.Windows;

namespace CloudManager
{
    /// <summary>
    /// EditBackupPolicy.xaml 的交互逻辑
    /// </summary>
    public partial class EditBackupPolicyWindow : Window
    {
        private DBBackupPolicy mPolicy;
        private DefaultAcsClient mClient;


        public EditBackupPolicyWindow(DBBackupPolicy policy, DefaultAcsClient client)
        {
            InitializeComponent();
            mPolicy = new DBBackupPolicy(policy);
            this.DataContext = mPolicy;
            mClient = client;
        }

        private void ModifiedPolicy()
        {

            this.Close();
        }

        private void ModifyPolicy(object obj)
        {
            DBBackupPolicy policy = obj as DBBackupPolicy;
            ModifyBackupPolicyRequest request = new ModifyBackupPolicyRequest();
            request.DBInstanceId = policy.DBInstanceId;
            request.BackupRetentionPeriod = policy.BackupRetentionPeriod;
            request.PreferredBackupPeriod = policy.GetBackupPeriod();
            request.PreferredBackupTime = policy.GetBackupTimeUTC();
            if (policy.Enable)
            {
                request.BackupLog = "Enable";
            }
            else
            {
                request.BackupLog = "Disable";
            }
            request.LogBackupRetentionPeriod = policy.LogBackupRetentionPeriod;
            try
            {
                ModifyBackupPolicyResponse response = mClient.GetAcsResponse(request);
                Dispatcher.Invoke(new Action(ModifiedPolicy));
            }
            catch (Exception ex)
            {

            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ParameterizedThreadStart(ModifyPolicy));
            t.Start(mPolicy);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
