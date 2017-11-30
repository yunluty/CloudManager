using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudManager
{
    public class DescribeBucket
    {
        public static Dictionary<string, string> VPCEndPoints = new Dictionary<string, string>()
        {
            { "oss-cn-hangzhou",    "oss-cn-hangzhou-internal.aliyuncs.com" },
            { "oss-cn-shanghai",    "oss-cn-shanghai-internal.aliyuncs.com" },
            { "oss-cn-qingdao",     "vpc100-oss-cn-qingdao.aliyuncs.com" },
            { "oss-cn-beijing",     "oss-cn-beijing-internal.aliyuncs.com" },
            { "oss-cn-zhangjiakou", "oss-cn-zhangjiakou-internal.aliyuncs.com" },
            { "oss-cn-shenzhen",    "oss-cn-shenzhen-internal.aliyuncs.com" },
            { "oss-cn-hongkong",    "oss-cn-hongkong-internal.aliyuncs.com" },
            { "oss-us-west-1",      "oss-us-west-1-internal.aliyuncs.com" },
            { "oss-us-east-1",      "oss-us-east-1-internal.aliyuncs.com" },
            { "oss-ap-southeast-1", "vpc100-oss-ap-southeast-1.aliyuncs.com" },
            { "oss-ap-southeast-2", "oss-ap-southeast-2-internal.aliyuncs.com" },
            { "oss-ap-northeast-1", "oss-ap-northeast-1-internal.aliyuncs.com" },
            { "oss-eu-central-1",   "oss-eu-central-1-internal.aliyuncs.com" },
            { "oss-me-east-1",      "oss-me-east-1-internal.aliyuncs.com" },
        };

        public DescribeBucket(Bucket b)
        {
            Location = b.Location;
            Name = b.Name;
            CreationTime = b.CreationDate.ToString("yyyy-MM-dd HH:mm");
            InternetEndPoint = Location + ".aliyuncs.com";
            InternalEndPoint = Location + "-internal.aliyuncs.com";
            if (VPCEndPoints.ContainsKey(Location))
            {
                VPCEndPoint = VPCEndPoints[Location];
            }
            InternetDomain = Name + '.' + InternetEndPoint;
            InternalDomain = Name + '.' + InternalEndPoint;
            VPCDomain = Name + '.' + VPCEndPoint;
        }

        public string Location { get; }
        public string Name { get; }
        public string CreationTime { get; }
        public string InternetEndPoint { get; }
        public string InternalEndPoint { get; }
        public string VPCEndPoint { get; }
        public string InternetDomain { get; }
        public string InternalDomain { get; }
        public string VPCDomain { get; }
        public string ACL { get; set; }
        public string CORSRule { get; set; }
        public string LifecycleRule { get; set; }
        public string Logging { get; set; }
        public string Referer { get; set; }
        public string Website { get; set; }
    }
}
