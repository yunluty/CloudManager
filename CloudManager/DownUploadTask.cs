using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudManager
{
    public class DownUploadTask : EventArgs, INotifyPropertyChanged
    {
        public string InstanceType { get; set; }

        public string InstanceID { get; set; }

        public string InstanceName { get; set; }

        public object Instance { get; set; }

        public string URL { get; set; }

        public string FilePath { get; set; }

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

        public FileTypeMode FileType { get; set; }

        private long totalsize;
        public long TotalSize
        {
            get { return totalsize; }
            set
            {
                totalsize = value;
                NotifyPropertyChanged("TotalSize");
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

        public TaskTypeMode TaskType { get; set; }

        public string CompleteTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StreamTransferProgress(object sender, StreamTransferProgressArgs args)
        {
            TotalSize = args.TotalBytes;
            DownloadSize = args.TransferredBytes;
        }

        public enum TaskTypeMode
        {
            Upload = 0,
            Download = 1
        }

        public enum FileTypeMode
        {
            File = 0,
            Directory = 1
        }
    }
}
