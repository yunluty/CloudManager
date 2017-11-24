using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static Aliyun.Acs.Ecs.Model.V20140526.DescribeRegionsResponse;

namespace CloudManager
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string AKI;
        public static string AKS;
        public static List<DescribeRegions_Region> REGIONS;
    }
}
