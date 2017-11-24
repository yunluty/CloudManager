using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CloudManager
{
    class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status != null)
            {
                if (status.Equals("Running", StringComparison.CurrentCultureIgnoreCase)
                || status.Equals("active", StringComparison.CurrentCultureIgnoreCase)
                || status.Equals("normal", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "Green";
                }
                else if (status.Equals("abnormal", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "Red";
                }
            }
            return "#F68300";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
