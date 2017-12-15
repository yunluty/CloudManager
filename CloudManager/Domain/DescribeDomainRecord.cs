using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aliyun.Acs.Alidns.Model.V20150109.DescribeDomainRecordsResponse;

namespace CloudManager.Domain
{
    public class DescribeDomainRecord : INotifyPropertyChanged
    {
        public DescribeDomainRecord(DescribeDomain domain)
        {
            Domain = domain;
            DomainName = domain.DomainName;
            DomainSuffix = '.' + DomainName;
            Type = "A";
            Line = "default";
            Priority = 10;
            TTL = 600;
        }

        public DescribeDomainRecord(Record r)
        {
            DomainName = r.DomainName;
            DomainSuffix = '.' + DomainName;
            RecordId = r.RecordId;
            RR = r.RR;
            Type = r.Type;
            Value = r.Value;
            TTL = r.TTL;
            Priority = r.Priority;
            Line = r.Line;
            Status = r.Status;
            Locked = r.Locked;
            Weight = r.Weight;
        }

        public DescribeDomainRecord(DescribeDomainRecord r)
        {
            DomainName = r.DomainName;
            DomainSuffix = r.DomainSuffix;
            RecordId = r.RecordId;
            RR = r.RR;
            Type = r.Type;
            Value = r.Value;
            TTL = r.TTL;
            Priority = r.Priority;
            Line = r.Line;
            Status = r.Status;
            Locked = r.Locked;
            Weight = r.Weight;
        }

        private string rR;

        private string type;

        private string value_;

        private long? tTL;

        private long? priority;

        private string line;

        private string status;

        private bool? locked;

        private int? weight;

        public string DomainName { get; }

        public string DomainSuffix { get; }

        public string RecordId { get; }

        public string RR
        {
            get
            {
                return rR;
            }
            set
            {
                rR = value;
                NotifyPropertyChanged("RR");
            }
        }

        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                NotifyPropertyChanged("Type");
            }
        }

        public string Value
        {
            get
            {
                return value_;
            }
            set
            {
                value_ = value;
                NotifyPropertyChanged("Value");
            }
        }

        public long? TTL
        {
            get
            {
                return tTL;
            }
            set
            {
                tTL = value;
                if (value == 10 * 60)
                {
                    TTLStr = "10分钟";
                }
                else if (value == 30 * 60)
                {
                    TTLStr = "30分钟";
                }
                else if (value == 60 * 60)
                {
                    TTLStr = "1小时";
                }
                else if (value == 12 * 60 * 60)
                {
                    TTLStr = "12小时";
                }
                else if (value == 24 * 60 * 60)
                {
                    TTLStr = "1天";
                }
            }
        }

        private string tTTLStr;
        public string TTLStr
        {
            get { return tTTLStr; }
            set
            {
                tTTLStr = value;
                NotifyPropertyChanged("TTLStr");
            }
        }

        public long? Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
                NotifyPropertyChanged("Priority");
            }
        }

        public string Line
        {
            get
            {
                return line;
            }
            set
            {
                line = value;
                switch (value)
                {
                    case "default":
                        LineStr = "默认";
                        break;

                    case "telecom":
                        LineStr = "中国电信";
                        break;

                    case "unicom":
                        LineStr = "中国联通";
                        break;

                    case "mobile":
                        LineStr = "中国移动";
                        break;

                    case "edu":
                        LineStr = "中国教育网";
                        break;

                    case "oversea":
                        LineStr = "世界";
                        break;

                    case "baidu":
                        LineStr = "百度";
                        break;

                    case "biying":
                        LineStr = "必应";
                        break;

                    case "google":
                        LineStr = "谷歌";
                        break;
                }
            }
        }

        private string lineStr;
        public string LineStr
        {
            get { return lineStr; }
            set
            {
                lineStr = value;
                NotifyPropertyChanged("LineStr");
            }
        }

        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }

        public bool? Locked
        {
            get
            {
                return locked;
            }
            set
            {
                locked = value;
                NotifyPropertyChanged("Locked");
            }
        }

        public int? Weight
        {
            get
            { return weight; }
            set
            {
                weight = value;
                NotifyPropertyChanged("Weight");
            }
        }

        public DescribeDomain Domain { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
