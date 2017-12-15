using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CloudManager
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (typeof(string).IsInstanceOfType(value))
            {
                string status = value as string;
                if (status.Equals("Running") || status.Equals("FTP") || status.Equals("NAS"))
                {
                    return Visibility.Visible;
                }
                else if (status.Equals("TCP"))
                {
                    string tcp = parameter as string;
                    if (tcp != null && tcp.Contains("TCP"))
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else if (status.Equals("HTTP"))
                {
                    string http = parameter as string;
                    if (http != null && http.Contains("HTTP") && !http.Equals("HTTPS"))
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else if (status.Equals("HTTPS"))
                {
                    string https = parameter as string;
                    if (https != null && https.Contains("HTTPS"))
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else if (status.Equals("UDP"))
                {
                    string udp = parameter as string;
                    if (udp != null && udp.Contains("UDP"))
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else if (status.Equals("insert"))
                {
                    string insert = parameter as string;
                    if (insert != null && insert.Contains("Time"))
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else if (status.Equals("server"))
                {
                    string udp = parameter as string;
                    if (udp != null && udp.Contains("Cookie"))
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else if (status.Equals("MX"))
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else if (typeof(int).IsInstanceOfType(value))
            {
                int? count = value as int?;
                if (count > 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else if (typeof(bool).IsInstanceOfType(value))
            {
                bool? show = value as bool?;
                if (show == true)
                { 
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else if (typeof(DownUpLoadTaskPage.TaskStatus).IsInstanceOfType(value))
            {
                DownUpLoadTaskPage.TaskStatus status = (DownUpLoadTaskPage.TaskStatus)value;
                if (status == DownUpLoadTaskPage.TaskStatus.Running)
                {
                    string type = parameter as string;
                    if (type != null && type.Contains("Running"))
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else if (status == DownUpLoadTaskPage.TaskStatus.Finished)
                {
                    string type = parameter as string;
                    if (type != null && type.Contains("Finished"))
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
