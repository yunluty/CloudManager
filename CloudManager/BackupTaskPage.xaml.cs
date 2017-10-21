using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CloudManager
{
    /// <summary>
    /// BackupTaskPage.xaml 的交互逻辑
    /// </summary>
    public partial class BackupTaskPage : Page
    {
        private ObservableCollection<BackupTask> mRunningTasks = new ObservableCollection<BackupTask>();
        private ObservableCollection<BackupTask> mCompeleteTasks = new ObservableCollection<BackupTask>();

        public delegate void DelegateResult(object obj);
        public string TaskType { get; set; }

        public BackupTaskPage()
        {
            InitializeComponent();
            RunningList.ItemsSource = mRunningTasks;
            CompleteList.ItemsSource = mCompeleteTasks;
        }

        public void AddNewTask(BackupTask task)
        {
            mRunningTasks.Add(task);
            Thread t = new Thread(StartDownload);
            t.Start(task);
        }

        private void StartDownload(object obj)
        {
            BackupTask task = obj as BackupTask;
            HttpDownload(task);
        }

        private void DownloadSuccess(object obj)
        {
            BackupTask task = obj as BackupTask;
            task.Status = "Success";
            task.CompleteTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            mRunningTasks.Remove(task);
            mCompeleteTasks.Add(task);
        }

        private void DownloadFail(object obj)
        {
            BackupTask task = obj as BackupTask;
            task.Status = "Failed";
        }

        private void HttpDownload(BackupTask task)
        {
            string path = task.SavePath + '\\' + task.FileName;
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

                byte[] bytes = new byte[2048];
                int size = rs.Read(bytes, 0, 2048);
                while (size > 0)
                {
                    fs.Write(bytes, 0, size);
                    task.DownloadSize += size;
                    Thread.Sleep(10);
                    size = rs.Read(bytes, 0, 2048);
                }

                Dispatcher.Invoke(new DelegateResult(DownloadSuccess), task);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new DelegateResult(DownloadFail), task);
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
    }
}
