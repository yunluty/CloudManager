using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CloudManager
{
    class StatusImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            string status = (string)value;

            if (status.Equals("Running", StringComparison.CurrentCultureIgnoreCase) 
                || status.Equals("active", StringComparison.CurrentCultureIgnoreCase))
            {
                return "images/running.png";
            }
            else if (status.Equals("Stopped") 
                || status.Equals("inactive", StringComparison.CurrentCultureIgnoreCase))
            {
                return "images/stopped.png";
            }
            else if (status.Equals("Starting", StringComparison.CurrentCultureIgnoreCase)
                || status.Equals("configuring", StringComparison.CurrentCultureIgnoreCase))
            {
                return "images/starting.png";
            }
            else if (status.Equals("Stopping", StringComparison.CurrentCultureIgnoreCase))
            {
                return "images/stopping.png";
            }
            else if (status.Equals("abnormal", StringComparison.CurrentCultureIgnoreCase))
            {
                return "images/abnormal.png";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
