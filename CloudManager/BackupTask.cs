using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudManager
{
    public class BackupTask
    {
        public BackupLocation Location;
        public int Hour;
        public int Minute;

        public BackupTask()
        {
            LocationStr = "Local";
            HourStr = "01";
            MinuteStr = "00";
        }

        private string locationstr;
        public string LocationStr
        {
            get { return locationstr; }
            set
            {
                locationstr = value;
            }
        }
        public string Ip { get; set; }
        public string Path { get; set; }
        private string hourstr;
        public string HourStr
        {
            get { return hourstr; }
            set
            {
                hourstr = value;
            }
        }
        public string MinuteStr
        {
            get;
            set;
        }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public enum BackupLocation
        {
            Local,
            FTP,
            NAS
        }
    }
}
