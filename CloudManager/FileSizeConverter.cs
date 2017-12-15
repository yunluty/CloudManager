using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CloudManager
{
    class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long? size = value as long?;
            if (size != null)
            {
                if (size >= 1024 * 1024 * 1024)
                {
                    return ((double)size / (1024 * 1024 * 1024)).ToString("0.00") + " GB";
                }
                else if (size >= 1024 * 1024)
                {
                    return ((double)size / (1024 * 1024)).ToString("0.00") + " MB";
                }
                else if (size >= 1024)
                {
                    return ((double)size / (1024)).ToString("0.00") + " KB";
                }
                else if (size > 0)
                {
                    return size + " B";
                }
                else if (size == 0)
                {
                    return "0";
                }
                else
                {
                    return "-";
                }
            }
            else
            {
                return "-";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
