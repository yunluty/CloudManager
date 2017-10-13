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
            if (typeof(string).IsInstanceOfType(value))
            {
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
                else if (status.Equals("doing", StringComparison.CurrentCultureIgnoreCase))
                {
                    string size = parameter as string;
                    if (size != null)
                    {
                        return "images/doing_" + size + ".png";
                    }
                    else
                    {
                        return "images/doing.png";
                    }
                }
                else if (status.Equals("success", StringComparison.CurrentCultureIgnoreCase))
                {
                    string size = parameter as string;
                    if (size != null)
                    {
                        return "images/success_" + size + ".png";
                    }
                    else
                    {
                        return "images/success.png";
                    }
                }
                else if (status.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                {
                    string size = parameter as string;
                    if (size != null)
                    {
                        return "images/error_" + size + ".png";
                    }
                    else
                    {
                        return "images/error.png";
                    }
                }
                else if (status.Equals("legal", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "images/legal.png";
                }
                else if (status.Equals("illegal", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "images/illegal.png";
                }
            }
            else if (typeof(bool).IsInstanceOfType(value))
            {
                bool? legal = value as bool?;
                if (legal == true)
                {
                    return "images/legal.png";
                }
                else
                {
                    return "images/illegal.png";
                }
            }
                
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
