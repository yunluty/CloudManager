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
    public partial class EditBackupPolicyWindow : WindowBase
    {
        private DBBackupPolicy mPolicy;
        private DefaultAcsClient mClient;

        public DBBackupPolicy NewPolicy { get; set; }

        public EditBackupPolicyWindow(DBBackupPolicy policy, DefaultAcsClient client)
        {
            InitializeComponent();
            mPolicy = new DBBackupPolicy(policy);
            this.DataContext = mPolicy;
            mClient = client;
        }

        private void ModifiedPolicy()
        {
            NewPolicy = mPolicy;
            if (NewPolicy.Enable) //Set the backup log status
            {
                NewPolicy.BackupLog = "Enable";
            }
            else
            {
                NewPolicy.BackupLog = "Disabled";
            }
            this.Close();
        }

        private void ModifyPolicy(DBBackupPolicy policy)
        {
            DoLoadingWork(win =>
            {
                ModifyBackupPolicyRequest request = new ModifyBackupPolicyRequest();
                request.DBInstanceId = policy.DBInstanceId;
                request.BackupRetentionPeriod = policy.BackupRetentionPeriod;
                request.PreferredBackupTime = policy.GetBackupTimeUTC();
                request.PreferredBackupPeriod = policy.GetBackupPeriod();
                if (policy.Enable)
                {
                    request.BackupLog = "Enable";
                }
                else
                {
                    request.BackupLog = "Disabled";
                }
                request.LogBackupRetentionPeriod = policy.LogBackupRetentionPeriod;
                ModifyBackupPolicyResponse response = mClient.GetAcsResponse(request);
                Dispatcher.Invoke(new Action(ModifiedPolicy));
            },
            ex =>
            {
                //TODO:
            });
            
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            ModifyPolicy(mPolicy);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
