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
            if (status.Equals("Running") || status.Equals("active"))
            {
                return "运行中";
            }
            else if (status.Equals("Stopped") || status.Equals("inactive"))
            {
                return "已停止";
            }
            else if (status.Equals("Starting"))
            {
                return "启动中";
            }
            else if (status.Equals("Stopping"))
            {
                return "停止中";
            }
            else if (status.Equals("locked"))
            {
                return "已锁定";
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
