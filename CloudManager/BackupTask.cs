using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudManager
{
    public class BackupTask : INotifyPropertyChanged
    {
        public string InstanceType { get; set; }

        public string InstanceID { get; set; }

        public string InstanceName { get; set; }

        public string URL { get; set; }

        public string SavePath { get; set; }

        public string Status { get; set; }

        private int progress;
        public int Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                NotifyPropertyChanged("Progress");
                ProgressPercent = value + "%";
            }
        }

        private string progresspercent;
        public string ProgressPercent
        {
            get { return progresspercent; }
            set
            {
                progresspercent = value;
                NotifyPropertyChanged("ProgressPercent");
            }
        }

        public string FileName { get; set; }

        public string FileType { get; set; }

        private long totalsize;
        public long TotalSize
        {
            get { return totalsize; }
            set
            {
                totalsize = value;
                if (value > 1024 * 1024 * 1024)
                {
                    TotalSizeUnit = ((double)value / (1024 * 1024 * 1024)).ToString("0.00") + " GB";
                }
                else if (value > 1024 * 1024)
                {
                    TotalSizeUnit = ((double)value / (1024 * 1024)).ToString("0.00") + " MB";
                }
                else if (value >= 1024)
                {
                    TotalSizeUnit = ((double)value / 1024).ToString("0.00") + " KB";
                }
                else
                {
                    TotalSizeUnit = value + " B";
                }
            }
        }

        private string totalSizeUnit;
        public string TotalSizeUnit
        {
            get { return totalSizeUnit; }
            set
            {
                totalSizeUnit = value;
                NotifyPropertyChanged("TotalSizeUnit");
            }
        }

        private long downloadSize;
        public long DownloadSize {
            get { return downloadSize; }
            set
            {
                downloadSize = value;
                int percent = (int)((double)value / TotalSize * 100);
                if (TotalSize > 0 && Progress != percent)
                {
                    Progress = percent;
                }
            }
        }

        public string CompleteTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
