using Aliyun.OSS;
using Aliyun.OSS.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CloudManager.DownUploadTask;

namespace CloudManager
{
    /// <summary>
    /// BackupTaskPage.xaml 的交互逻辑
    /// </summary>
    public partial class DownUpLoadTaskPage : Page, INotifyPropertyChanged
    {
        private static int DOWNLOAD_SIZE = 4096;

        private string mAki, mAks;
        private ObservableCollection<DownUploadTask> mRunningTasks = new ObservableCollection<DownUploadTask>();
        private ObservableCollection<DownUploadTask> mFinishedTasks = new ObservableCollection<DownUploadTask>();

        public delegate void DelegateResult(object obj);

        private TaskStatus taskType;
        public TaskStatus TaskType
        {
            get { return taskType; }
            set
            {
                taskType = value;
                NotifyPropertyChanged("TaskType");
            }
        }

        public DownUpLoadTaskPage()
        {
            InitializeComponent();

            mAki = App.AKI;
            mAks = App.AKS;
            RunningList.ItemsSource = mRunningTasks;
            FinishedList.ItemsSource = mFinishedTasks;
            RunningList.DataContext = this;
            FinishedList.DataContext = this;
        }

        public void AddNewTask(DownUploadTask task)
        {
            mRunningTasks.Add(task);
            StartTask(task);
            
        }

        private void StartTask(DownUploadTask task)
        {
            Thread t = null;
            switch (task.InstanceType)
            {
                case "RDS":
                    task.Status = "Downloading";
                    t = new Thread(new ParameterizedThreadStart(HttpDownload));
                    break;

                case "OSS":
                    if (task.TaskType == DownUploadTask.TaskTypeMode.Upload)
                    {
                        task.Status = "Uploading";
                        t = new Thread(new ParameterizedThreadStart(OssUpload));
                    }
                    else if (task.TaskType == DownUploadTask.TaskTypeMode.Download)
                    {
                        task.Status = "Downloading";
                        t = new Thread(new ParameterizedThreadStart(OssDownload));
                    }
                    break;
            }
            t.Start(task);
        }

        private void TaskSuccess(object obj)
        {
            DownUploadTask task = obj as DownUploadTask;
            task.Status = "Success";
            task.CompleteTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            mRunningTasks.Remove(task);
            mFinishedTasks.Add(task);
        }

        private void TaskFail(object obj)
        {
            DownUploadTask task = obj as DownUploadTask;
            task.Status = "Failed";
            task.CompleteTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void OssUpload(object obj)
        {
            DownUploadTask task = obj as DownUploadTask;
            DescribeBucket bucket = task.Instance as DescribeBucket;
            string endpoint = "http://" + bucket.InternetEndPoint;
            OssClient client = new OssClient(endpoint, mAki, mAks);
            string key = task.URL;
            Stream stream = null;
            try
            {
                if (task.FileType == FileTypeMode.Directory)
                {
                    stream = new MemoryStream();
                    client.PutObject(bucket.Name, key, stream);
                }
                else
                {

                    stream = File.Open(task.FilePath, FileMode.Open);
                    var request = new PutObjectRequest(bucket.Name, key, stream);
                    request.StreamTransferProgress += task.StreamTransferProgress;
                    client.PutObject(request);
                }
                Dispatcher.Invoke(new DelegateResult(TaskSuccess), task);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new DelegateResult(TaskFail), task);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        private void OssDownload(object obj)
        {
            DownUploadTask task = obj as DownUploadTask;
            DescribeBucket bucket = task.Instance as DescribeBucket;
            string endpoint = "http://" + bucket.InternetEndPoint;
            OssClient client = new OssClient(endpoint, mAki, mAks);
            string path = task.FilePath;
            string key = task.URL;
            Stream stream = null;
            FileStream fs = null;
            try
            {
                if (task.FileType == FileTypeMode.Directory)
                {
                    if (!Directory.Exists(task.FilePath))
                    {
                        Directory.CreateDirectory(task.FilePath);
                    }
                }
                else
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    fs = File.Open(path, FileMode.Create);
                    var request = new GetObjectRequest(bucket.Name, key);
                    request.StreamTransferProgress += task.StreamTransferProgress;
                    var o = client.GetObject(request);
                    stream = o.Content;
                    byte[] bytes = new byte[DOWNLOAD_SIZE];
                    int size = 0;
                    while ((size = stream.Read(bytes, 0, DOWNLOAD_SIZE)) > 0)
                    {
                        fs.Write(bytes, 0, size);
                    }
                }     
                Dispatcher.Invoke(new DelegateResult(TaskSuccess), task);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new DelegateResult(TaskFail), task);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }

                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        private void HttpDownload(object obj)
        {
            DownUploadTask task = obj as DownUploadTask;
            string path = task.FilePath;
            FileStream fs = null;
            HttpWebRequest request = null;
            Stream rs = null;
            WebResponse response = null;

            if (File.Exists(path))
            {
                File.Delete(path); //Redownload
            }

            try
            {
                fs = new FileStream(path, FileMode.Create);

                request = WebRequest.Create(task.URL) as HttpWebRequest;
                request.ReadWriteTimeout = 2000;
                request.Timeout = 5000;
                response = request.GetResponse();
                rs = response.GetResponseStream();

                task.TotalSize = response.ContentLength;
                task.DownloadSize = 0;

                byte[] bytes = new byte[DOWNLOAD_SIZE];
                int size = 0;
                while ((size = rs.Read(bytes, 0, DOWNLOAD_SIZE)) > 0)
                { 
                    fs.Write(bytes, 0, size);
                    task.DownloadSize += size;
                    Thread.Sleep(10);
                }

                Dispatcher.Invoke(new DelegateResult(TaskSuccess), task);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new DelegateResult(TaskFail), task);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }

                if (rs != null)
                {
                    rs.Close();
                }
                
                if (request != null)
                {
                    request.Abort();
                }

                if (response != null)
                {
                    response.Close();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public enum TaskStatus
        {
            Running,
            Finished
        }
    }
}
