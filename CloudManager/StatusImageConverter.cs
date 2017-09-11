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
            string status = (string)value;
            if (status.Equals("Running"))
            {
                return "images/running.png";
            }
            else if (status.Equals("Stopped"))
            {
                return "images/stopped.png";
            }
            else if (status.Equals("Starting"))
            {
                return "images/starting.png";
            }
            else if (status.Equals("Stopping"))
            {
                return "images/stopping.png";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
