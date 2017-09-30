using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aliyun.Acs.Rds.Model.V20140815.DescribeDBInstancesResponse;

namespace CloudManager
{
    class DescribeDBInstance : INotifyPropertyChanged
    {
        public DescribeDBInstance(DBInstance d)
        {
            DBInstanceId = d.DBInstanceId;
            DBInstanceDescription = d.DBInstanceDescription;
        }

        public List<ReadOnlyDBInstanceId> ReadOnlyDBInstanceIds { get; set; }
        public string VpcId { get; set; }
        public string MasterInstanceId { get; set; }
        public string TempDBInstanceId { get; set; }
        public string GuardDBInstanceId { get; set; }
        public string EngineVersion { get; set; }
        public string CreateTime { get; set; }
        public bool? MutriORsignle { get; set; }
        public string ZoneId { get; set; }
        public string InstanceNetworkType { get; set; }
        public LockModeEnum? LockMode { get; set; }
        public string LockReason { get; set; }
        public DBInstanceNetTypeEnum? DBInstanceNetType { get; set; }
        public int? InsId { get; set; }
        public string DBInstanceId { get; set; }
        public ConnectionModeEnum? ConnectionMode { get; set; }
        public string PayType { get; set; }
        public DBInstanceTypeEnum? DBInstanceType { get; set; }
        private string _DBInstanceDescription;
        public string DBInstanceDescription
        {
            get { return _DBInstanceDescription; }
            set
            {
                if (value == null)
                {
                    _DBInstanceDescription = DBInstanceId;
                }
                else
                {
                    _DBInstanceDescription = value;
                }
                NotifyPropertyChanged("DBInstanceDescription");
            }
        }
        public string ExpireTime { get; set; }
        public DBInstanceStatusEnum? DBInstanceStatus { get; set; }
        public string Engine { get; set; }
        public string RegionId { get; set; }

        public enum LockModeEnum
        {
            LockByExpiration = 0,
            LockByRestoration = 1,
            LockReadInstanceByDiskQuota = 2,
            ManualLock = 3,
            LockByDiskQuota = 4,
            Unlock = 5
        }
        public enum DBInstanceTypeEnum
        {
            Shared = 0,
            Guard = 1,
            Primary = 2,
            Readonly = 3,
            Temp = 4
        }
        public enum DBInstanceStatusEnum
        {
            TempDBInstanceCreating = 0,
            Running = 1,
            Deleting = 2,
            DBInstanceClassChanging = 3,
            Creating = 4,
            Rebooting = 5,
            GuardSwitching = 6,
            LingSwitching = 7,
            Transing = 8,
            TransingToOthers = 9,
            ImportingFromOthers = 10,
            EngineVersionUpgrading = 11,
            DBInstanceNetTypeChanging = 12,
            Importing = 13,
            Restoring = 14
        }
        public enum DBInstanceNetTypeEnum
        {
            Intranet = 0,
            Internet = 1
        }
        public enum ConnectionModeEnum
        {
            Standard = 0,
            Safe = 1
        }

        public class ReadOnlyDBInstanceId
        {
            public string DBInstanceId { get; set; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
