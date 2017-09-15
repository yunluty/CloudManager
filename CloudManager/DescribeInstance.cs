using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeInstancesResponse;

namespace CloudManager
{
    //也可以使用值转换器，把Aliyun SDK获取的信息转换成可显示的信息
    public class DescribeInstance : INotifyPropertyChanged
    {
        public DescribeInstance()
        {
        }

        public DescribeInstance(DescribeInstances_Instance i)
        {
            InstanceTypeFamily = i.InstanceTypeFamily;
            LocalStorageCapacity = i.LocalStorageCapacity;
            LocalStorageAmount = i.LocalStorageAmount;
            PublicIpAddress = i.PublicIpAddress;
            InnerIpAddress = i.InnerIpAddress;
            InstanceChargeType = i.InstanceChargeType;
            ExpiredTime = i.ExpiredTime;
            CreationTime = i.CreationTime;
            InstanceId = i.InstanceId;
            InstanceName = i.InstanceName;
            Description = i.Description;
            ImageId = i.ImageId;
            OSName = i.OSName;
            OSType = i.OSType;
            RegionId = i.RegionId;
            ZoneId = i.ZoneId;
            InstanceNetworkType = i.InstanceNetworkType;
            InstanceType = i.InstanceType;
            ClusterId = i.ClusterId;
            Memory = i.Memory;
            HostName = i.HostName;
            Status = i.Status;
            SerialNumber = i.SerialNumber;
            InternetChargeType = i.InternetChargeType;
            InternetMaxBandwidthIn = i.InternetMaxBandwidthIn;
            InternetMaxBandwidthOut = i.InternetMaxBandwidthOut;
            VlanId = i.VlanId;
            Cpu = i.Cpu;
        }

        private string _InstanceTypeFamily;
        public string InstanceTypeFamily
        {
            get { return _InstanceTypeFamily; }
            set
            {
                _InstanceTypeFamily = value;
                notifyPropertyChanged("InstanceTypeFamily");
            }
        }

        private long? _LocalStorageCapacity;
        public long? LocalStorageCapacity
        {
            get { return _LocalStorageCapacity; }
            set
            {
                _LocalStorageCapacity = value;
                notifyPropertyChanged("LocalStorageCapacity");
            }
        }

        private int? _LocalStorageAmount;
        public int? LocalStorageAmount
        {
            get { return _LocalStorageAmount; }
            set
            {
                _LocalStorageAmount = value;
                notifyPropertyChanged("LocalStorageAmount");
            }
        }

        private List<string> _PublicIpAddress;
        public List<string> PublicIpAddress
    {
            get { return _PublicIpAddress; }
            set
            {
                _PublicIpAddress = value;
                notifyPropertyChanged("PublicIpAddress");
            }
        }

        private List<string> _InnerIpAddress;
        public List<string> InnerIpAddress
        {
            get { return _InnerIpAddress; }
            set
            {
                _InnerIpAddress = value;
                notifyPropertyChanged("InnerIpAddress");
            }
        }

        private string _InstanceChargeType;
        public string InstanceChargeType
        {
            get { return _InstanceChargeType; }
            set
            {
                _InstanceChargeType = value;
                notifyPropertyChanged("InstanceChargeType");

                InstanceChargeTypeLang = value;
            }
        }

        private string _InstanceChargeTypeLang;
        public string InstanceChargeTypeLang
        {
            get { return _InstanceChargeTypeLang; }
            set
            {
                if (value.Equals("PrePaid"))
                {
                    _InstanceChargeTypeLang = "包年包月";
                }
                else
                {
                    _InstanceChargeTypeLang = "按量付费";
                }
                notifyPropertyChanged("InstanceChargeTypeLang");
            }
        }

        private string _ExpiredTime;
        public string ExpiredTime
        {
            get { return _ExpiredTime; }
            set
            {
                DateTime dt;
                DateTime.TryParse(value, out dt);
                _ExpiredTime = dt.ToString("yyyy-MM-dd hh:mm:ss");
                notifyPropertyChanged("ExpiredTime");
            }
        }

        private string _CreationTime;
        public string CreationTime
        {
            get { return _CreationTime; }
            set
            {
                DateTime dt;
                DateTime.TryParse(value, out dt);
                _CreationTime = dt.ToString("yyyy-MM-dd hh:mm:ss");
                notifyPropertyChanged("CreationTime");
            }
        }

        private string _InstanceId;
        public string InstanceId
        {
            get { return _InstanceId; }
            set
            {
                _InstanceId = value;
                notifyPropertyChanged("InstanceId");
            }
        }

        private string _InstanceName;
        public string InstanceName
        {
            get { return _InstanceName; }
            set
            {
                _InstanceName = value;
                notifyPropertyChanged("InstanceName");
            }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                notifyPropertyChanged("Description");
            }
        }

        private string _ImageId;
        public string ImageId
    {
            get { return _ImageId; }
            set
            {
                _ImageId = value;
                notifyPropertyChanged("ImageId");
            }
        }

        private string _OSName;
        public string OSName
        {
            get { return _OSName; }
            set
            {
                _OSName = value;
                notifyPropertyChanged("OSName");
            }
        }

        private string _OSType;
        public string OSType
        {
            get { return _OSType; }
            set
            {
                _OSType = value;
                notifyPropertyChanged("OSType");
            }
        }

        private string _RegionId;
        public string RegionId
        {
            get { return _RegionId; }
            set
            {
                _RegionId = value;
                notifyPropertyChanged("RegionId");

                RegionLang = value;
            }
        }

        private string GetRegionLang(string region)
        {
            if (region.Contains("cn-qingdao"))
            {
                return "华北 1";
            } else if (region.Contains("cn-beijing"))
            {
                return "华北 2";
            }
            else if (region.Contains("cn-zhangjiakou"))
            {
                return "华北 3";
            }
            else if (region.Contains("cn-hangzhou"))
            {
                return "华东 1";
            }
            else if (region.Contains("cn-shanghai"))
            {
                return "华东 2";
            }
            else if (region.Contains("cn-shenzhen"))
            {
                return "华南 1";
            }
            else
            {
                return "";
            }
        }

        private string GetZoneLang(string zone)
        {
            int index = zone.LastIndexOf('-');
            string area = zone.Substring(index + 1);
            return "可用区" + ' ' + area.ToUpper();
        }

        private string _RegionLang;
        public string RegionLang
        {
            get { return _RegionLang; }
            set
            {
                _RegionLang = GetRegionLang(value);
                notifyPropertyChanged("RegionLang");
            }
        }

        private string _ZoneId;
        public string ZoneId
        {
            get { return _ZoneId; }
            set
            {
                _ZoneId = value;
                ZoneLang = value;
                notifyPropertyChanged("ZoneId");
            }
        }

        private string _ZoneLang;
        public string ZoneLang
        {
            get { return _ZoneLang; }
            set
            {
                _ZoneLang = GetRegionLang(value) + ' ' + GetZoneLang(value);
                notifyPropertyChanged("ZoneLang");
            }
        }

        private string _InstanceNetworkType;
        public string InstanceNetworkType
        {
            get { return _InstanceNetworkType; }
            set
            {
                _InstanceNetworkType = value;
                notifyPropertyChanged("InstanceNetworkType");
            }
        }

        private string _InstanceType;
        public string InstanceType
        {
            get { return _InstanceType; }
            set
            {
                _InstanceType = value;
                notifyPropertyChanged("InstanceType");
            }
        }

        private string _ClusterId;
        public string ClusterId
        {
            get { return _ClusterId; }
            set
            {
                _ClusterId = value;
                notifyPropertyChanged("ClusterId");
            }
        }

        private int? _Memory;
        public int? Memory
        {
            get { return _Memory; }
            set
            {
                _Memory = value;
                notifyPropertyChanged("Memory");

                if (value < 1024)
                {
                    MemoryLang = value.ToString() + "MB";
                }
                else
                {
                    int? r = value / 1024;
                    MemoryLang = r.ToString() + "GB";
                }
            }
        }

        private string _MemoryLang;
        public string MemoryLang
        {
            get { return _MemoryLang; }
            set
            {
                _MemoryLang = value;
                notifyPropertyChanged("MemoryLang");
            }
        }

        private string _HostName;
        public string HostName
        {
            get { return _HostName; }
            set
            {
                _HostName = value;
                notifyPropertyChanged("HostName");
            }
        }

        private string _Status;
        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                notifyPropertyChanged("Status");
            }
        }

        private string _SerialNumber;
        public string SerialNumber
        {
            get { return _SerialNumber; }
            set
            {
                _SerialNumber = value;
                notifyPropertyChanged("SerialNumber");
            }
        }

        private string _InternetChargeType;
        public string InternetChargeType
        {
            get { return _InternetChargeType; }
            set
            {
                _InternetChargeType = value;
                notifyPropertyChanged("InternetChargeType");

                InternetChargeTypeLang = value;
            }
        }

        private string _InternetChargeTypeLang;
        public string InternetChargeTypeLang
        {
            get { return _InternetChargeTypeLang; }
            set
            {
                if (value.Equals("PayByBandwidth"))
                {
                    _InternetChargeTypeLang = "按固定带宽";
                }
                else
                {
                    _InternetChargeTypeLang = "按使用流量";
                }
                notifyPropertyChanged("InternetChargeTypeLang");
            }
        }

        private int? _InternetMaxBandwidthIn;
        public int? InternetMaxBandwidthIn
        {
            get { return _InternetMaxBandwidthIn; }
            set
            {
                _InternetMaxBandwidthIn = value;
                notifyPropertyChanged("InternetMaxBandwidthIn");
            }
        }

        private int? _InternetMaxBandwidthOut;
        public int? InternetMaxBandwidthOut
        {
            get { return _InternetMaxBandwidthOut; }
            set
            {
                _InternetMaxBandwidthOut = value;
                notifyPropertyChanged("InternetMaxBandwidthOut");

                InternetMaxBandwidthOutLang = value.ToString();
            }
        }

        private string _InternetMaxBandwidthOutLang;
        public string InternetMaxBandwidthOutLang
        {
            get { return _InternetMaxBandwidthOutLang; }
            set
            {
                _InternetMaxBandwidthOutLang = value + "Mbps";
                notifyPropertyChanged("InternetMaxBandwidthOutLang");
            }
        }

        private string _VlanId;
        public string VlanId
        {
            get { return _VlanId; }
            set
            {
                _VlanId = value;
                notifyPropertyChanged("VlanId");
            }
        }

        private int? _Cpu;
        public int? Cpu
        {
            get { return _Cpu; }
            set
            {
                _Cpu = value;
                notifyPropertyChanged("Cpu");

                CpuLang = value.ToString();
            }
        }

        private string _CpuLang;
        public string CpuLang
        {
            get { return _CpuLang; }
            set
            {
                _CpuLang = value + "核";
                notifyPropertyChanged("CpuLang");
            }
        }

        private int? _Weight;
        public int? Weight
        {
            get { return _Weight; }
            set
            {
                _Weight = value;
                notifyPropertyChanged("Weight");
            }
        }

        private bool _Checked;
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                _Checked = value;
                notifyPropertyChanged("Checked");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void notifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
