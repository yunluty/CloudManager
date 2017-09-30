﻿using System;
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
            if (typeof(string).IsInstanceOfType(value))
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
                else if (status.Equals("doing", StringComparison.CurrentCultureIgnoreCase))
                {
                    string doing = parameter as string;
                    if (doing.Equals("All"))
                    {
                        return "监听配置中";
                    }
                    else if (doing.Equals("Add"))
                    {
                        return "监听创建中";
                    }
                    else if (doing.Equals("Start"))
                    {
                        return "监听启动中";
                    }
                    else if (doing.Equals("Config"))
                    {
                        return "监听配置中";
                    }
                    return null;
                }
                else if (status.Equals("success", StringComparison.CurrentCultureIgnoreCase))
                {
                    string success = parameter as string;
                    if (success.Equals("All"))
                    {
                        return "恭喜，监听配置成功";
                    }
                    else if (success.Equals("Add"))
                    {
                        return "监听创建成功";
                    }
                    else if (success.Equals("Start"))
                    {
                        return "监听启动成功";
                    }
                    else if (success.Equals("Config"))
                    {
                        return "监听配置成功";
                    }
                    return null;
                }
                else if (status.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                {
                    string error = parameter as string;
                    if (error.Equals("All"))
                    {
                        return "抱歉，监听配置失败";
                    }
                    else if (error.Equals("Add"))
                    {
                        return "监听创建失败";
                    }
                    else if (error.Equals("Start"))
                    {
                        return "监听启动失败";
                    }
                    else if (error.Equals("Config"))
                    {
                        return "监听配置失败";
                    }
                    return null;
                }
                else if (status.Equals("on", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "开启";
                }
                else if (status.Equals("off", StringComparison.CurrentCultureIgnoreCase))
                {
                    return "关闭";
                }
                else
                {
                    return null;
                }
            }
            else if (typeof(bool?).IsInstanceOfType(value))
            {
                bool? config = value as bool?;
                if (config == true)
                {
                    return "取消";
                }
                else
                {
                    return "配置";
                }
            }
            else if (typeof(int?).IsInstanceOfType(value))
            {
                int? config = value as int?;
                string type = parameter as string;
                if (type != null && type.Equals("ONFF"))
                {
                    if (config > 0)
                    {
                        return "开启";
                    }
                    else
                    {
                        return "关闭";
                    }
                }
                else
                {
                    if (config > 0)
                    {
                        return config;
                    }
                    else
                    {
                        return "不限制";
                    }
                }
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
