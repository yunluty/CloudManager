using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CloudManager
{
    class StatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = (string)value;

            if (status.Equals("Running", StringComparison.CurrentCultureIgnoreCase)
                || status.Equals("active", StringComparison.CurrentCultureIgnoreCase))
            {
                return "运行中";
            }
            else if (status.Equals("Stopped", StringComparison.CurrentCultureIgnoreCase) 
                || status.Equals("inactive", StringComparison.CurrentCultureIgnoreCase))
            {
                return "已停止";
            }
            else if (status.Equals("Starting", StringComparison.CurrentCultureIgnoreCase))
            {
                return "启动中";
            }
            else if (status.Equals("Stopping", StringComparison.CurrentCultureIgnoreCase))
            {
                return "停止中";
            }
            else if (status.Equals("locked", StringComparison.CurrentCultureIgnoreCase))
            {
                return "已锁定";
            }
            else if (status.Equals("configuring", StringComparison.CurrentCultureIgnoreCase))
            {
                return "配置中";
            }
            else if (status.Equals("normal", StringComparison.CurrentCultureIgnoreCase))
            {
                return "正常";
            }
            else if (status.Equals("abnormal", StringComparison.CurrentCultureIgnoreCase))
            {
                return "异常";
            }
            else if (status.Equals("wrr", StringComparison.CurrentCultureIgnoreCase))
            {
                return "加权轮询";
            }
            else if (status.Equals("wlc", StringComparison.CurrentCultureIgnoreCase))
            {
                return "加权最小连接数";
            }
            else if (status.Equals("rr", StringComparison.CurrentCultureIgnoreCase))
            {
                return "轮询";
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
