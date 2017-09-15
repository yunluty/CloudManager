
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeLoadBalancersResponse;

namespace CloudManager
{
    public class DescribeLoadBalancer : INotifyPropertyChanged
    {
        private List<DescribeRegions_Region> mRegions;
        public DescribeLoadBalancer(DescribeLoadBalancers_LoadBalancer b, List<DescribeRegions_Region> r)
        {
            mRegions = r;
            CreateTimeStamp = b.CreateTimeStamp;
            CreateTime = b.CreateTime;
            InternetChargeType = b.InternetChargeType;
            SlaveZoneId = b.SlaveZoneId;
            MasterZoneId = b.MasterZoneId;
            NetworkType = b.NetworkType;
            RegionId = b.RegionId;
            AddressType = b.AddressType;
            Address = b.Address;
            LoadBalancerStatus = b.LoadBalancerStatus;
            LoadBalancerId = b.LoadBalancerId;
            LoadBalancerName = b.LoadBalancerName;
            PayType = b.PayType;
            ResourceGroupId = b.ResourceGroupId;
        }

        public long? CreateTimeStamp { get; set; }
        private string _CreateTime;
        public string CreateTime
        {
            get { return _CreateTime; }
            set
            {
                DateTime dt;
                DateTime.TryParse(value, out dt);
                _CreateTime = dt.ToString("yyyy-MM-dd hh:mm:ss");
                notifyPropertyChanged("CreationTime");
            }
        }
        private string _InternetChargeType;
        public string InternetChargeType
        {
            get { return _InternetChargeType; }
            set
            {
                _InternetChargeType = value;
                //InternetChargeTypeLang = value;
            }
        }
        /*private string _InternetChargeTypeLang;
        public string InternetChargeTypeLang
        {
            get { return _InternetChargeTypeLang; }
            set
            {
                if (value == null)
                {
                    _InternetChargeTypeLang = "-";
                }
                else if (value.Equals("PayByBandwidth"))
                {
                    _InternetChargeTypeLang = "按固定带宽";
                }
                else
                {
                    _InternetChargeTypeLang = "按使用流量";
                }
                notifyPropertyChanged("InternetChargeTypeLang");
            }
        }*/
        private string GetZoneName(string zone)
        {
            int index = zone.LastIndexOf('-');
            string area = zone.Substring(index + 1);
            return "可用区" + ' ' + area.ToUpper();
        }
        private string _SlaveZoneId;
        public string SlaveZoneId
        {
            get { return _SlaveZoneId; }
            set
            {
                _SlaveZoneId = value;
                SlaveZoneName = value;
            }
        }
        private string _SlaveZoneName;
        public string SlaveZoneName
        {
            get { return _SlaveZoneName; }
            set
            {
                _SlaveZoneName = GetRegionName(value) + ' ' + GetZoneName(value);
                notifyPropertyChanged("SlaveZoneName");
            }
        }
        private string _MasterZoneId;
        public string MasterZoneId
        {
            get { return _MasterZoneId; }
            set
            {
                _MasterZoneId = value;
                MasterZoneName = value;
            }
        }
        private string _MasterZoneName;
        public string MasterZoneName
        {
            get { return _MasterZoneName; }
            set
            {
                _MasterZoneName = GetRegionName(value) + ' ' + GetZoneName(value);
                notifyPropertyChanged("MasterZoneName");
            }
        }
        private string _NetworkType;
        public string NetworkType
        {
            get { return _NetworkType; }
            set
            {
                _NetworkType = value;
                NetworkTypeName = value;
            }
        }
        private string _NetworkTypeName;
        public string NetworkTypeName
        {
            get { return _NetworkTypeName; }
            set
            {
                if (value.Equals("classic"))
                {
                    _NetworkTypeName = "经典网络";
                }
                else
                {
                    _NetworkTypeName = "专有网络";
                }
                notifyPropertyChanged("NetworkTypeName");
            }
        }
        private string _RegionId;
        public string RegionId
        {
            get { return _RegionId; }
            set
            {
                _RegionId = value;
                RegionName = value;
            }
        }
        private string GetRegionName(string region)
        {
            foreach (DescribeRegions_Region r in mRegions)
            {
                if (r.RegionId.Contains(region))
                {
                    return r.LocalName;
                }
            }
            return null;
        }
        private string _RegionName;
        public string RegionName
        {
            get { return _RegionName; }
            set
            {
                _RegionName = GetRegionName(value);
                notifyPropertyChanged("RegionName");
            }
        }
        private string _AddressType;
        public string AddressType
        {
            get { return _AddressType; }
            set
            {
                _AddressType = value;
                AddressTypeName = value;
            }
        }
        private string _AddressTypeName;
        public string AddressTypeName
        {
            get { return _AddressTypeName; }
            set
            {
                if (value.Equals("internet"))
                {
                    _AddressTypeName = "公网";
                }
                else
                {
                    _AddressTypeName = "私网";
                }
                notifyPropertyChanged("AddressTypeName");
            }
        }
        private string _Address;
        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                notifyPropertyChanged("Address");
            }
        }
        private string _LoadBalancerStatus;
        public string LoadBalancerStatus
        {
            get { return _LoadBalancerStatus; }
            set
            {
                _LoadBalancerStatus = value;
                notifyPropertyChanged("LoadBalancerStatus");
            }
        }

        private string _LoadBalanceName;
        public string LoadBalancerName
        {
            get { return _LoadBalanceName; }
            set
            {
                _LoadBalanceName = value;
                if (value == null)
                {
                    LoadBalanceTitle = LoadBalancerId;
                }
                else
                {
                    LoadBalanceTitle = value;
                }
                notifyPropertyChanged("LoadBalancerName");
            }
        }
        private string _LoadBalanceId;
        public string LoadBalancerId
        {
            get { return _LoadBalanceId; }
            set
            {
                _LoadBalanceId = value;
                notifyPropertyChanged("LoadBalancerId");
            }
        }
        private string _LoadBalanceTitle;
        public string LoadBalanceTitle
        {
            get { return _LoadBalanceTitle; }
            set
            {
                _LoadBalanceTitle = value;
            }
        }
        private string _PayType;
        public string PayType
        {
            get { return _PayType; }
            set
            {
                _PayType = value;
                PayTypeName = value;
            }
        }
        private string _PayTypeName;
        public string PayTypeName
        {
            get { return _PayTypeName; }
            set
            {
                if (value.Equals("PrePay"))
                {
                    _PayTypeName = "预付费";
                }
                else
                {
                    _PayTypeName = "按量付费";
                }
                notifyPropertyChanged("PayTypeName");
            }
        }
        private string _ResourceGroupId;
        public string ResourceGroupId
        {
            get { return _ResourceGroupId; }
            set
            {
                _ResourceGroupId = value;
                notifyPropertyChanged("ResourceGroupId");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void notifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
