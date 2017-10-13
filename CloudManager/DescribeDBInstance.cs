using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aliyun.Acs.Rds.Model.V20140815.DescribeDBInstancesResponse;

namespace CloudManager
{
    public class DescribeDBInstance : INotifyPropertyChanged
    {
        public DescribeDBInstance(DescribeDBInstances_DBInstance d)
        {
            DBInstanceId = d.DBInstanceId;
            DBInstanceDescription = d.DBInstanceDescription;
            RegionId = d.RegionId;
            ZoneId = d.ZoneId;
            DBInstanceType = d.DBInstanceType;
            DBInstanceStatus = d.DBInstanceStatus;
            VpcId = d.VpcId;
            MasterInstanceId = d.MasterInstanceId;
            TempDBInstanceId = TempDBInstanceId;
            GuardDBInstanceId = d.GuardDBInstanceId;
            Engine = d.Engine;
            EngineVersion = d.EngineVersion;
            CreateTime = d.CreateTime;
            MutriORsignle = d.MutriORsignle;
            ZoneId = d.ZoneId;
            InstanceNetworkType = d.InstanceNetworkType;
            LockMode = d.LockMode;
            LockReason = d.LockReason;
            DBInstanceNetType = d.DBInstanceNetType;
            InsId = d.InsId;
            DBInstanceId = d.DBInstanceId;
            ConnectionMode = d.ConnectionMode;
            PayType = d.PayType;
            ExpireTime = d.ExpireTime;
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
        public string LockMode { get; set; }
        public string LockReason { get; set; }
        public string DBInstanceNetType { get; set; }
        public int? InsId { get; set; }
        public string DBInstanceId { get; set; }
        public string ConnectionMode { get; set; }
        public string PayType { get; set; }
        public string DBInstanceType { get; set; }
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
        public string DBInstanceStatus { get; set; }
        public string Engine { get; set; }
        public string RegionId { get; set; }
        public string AvailabilityValue { get; set; }
        public int? MaxIOPS { get; set; }
        public int? MaxConnections { get; set; }
        public string DBInstanceCPU { get; set; }
        public string IncrementSourceDBInstanceId { get; set; }
        public string SecurityIPList { get; set; }
        public string CreationTime { get; set; }
        public int? AccountMaxQuantity { get; set; }
        public string MaintainTime { get; set; }
        public string ReadDelayTime { get; set; }
        public string DBInstanceClassType { get; set; }
        public int? DBMaxQuantity { get; set; }
        public string Port { get; set; }
        public string ConnectionString { get; set; }
        public string DBInstanceClass { get; set; }
        public int? DBInstanceStorage { get; set; }
        public long? DBInstanceMemory { get; set; }
        public AccountTypeEnum? AccountType { get; set; }
        public SupportUpgradeAccountTypeEnum? SupportUpgradeAccountType { get; set; }


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
        public enum AccountTypeEnum
        {
            Normal = 0,
            Super = 1
        }
        public enum SupportUpgradeAccountTypeEnum
        {
            Yes = 0,
            No = 1
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
