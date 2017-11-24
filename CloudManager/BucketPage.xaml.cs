using Aliyun.OSS;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static CloudManager.DownUploadTask;
using static CloudManager.DescribeOSSObject;

namespace CloudManager
{
    /// <summary>
    /// BucketPage.xaml 的交互逻辑
    /// </summary>
    public partial class BucketPage : Page
    {
        private string mAki, mAks;
        private ObservableCollection<DescribeBucket> mSelBuckets = new ObservableCollection<DescribeBucket>();
        private DescribeBucket mSelBucket;
        private delegate void DelegateGot(object obj);
        private DescribeOSSObject mRootDirectory;
        private DescribeOSSObject mCurrDirectory;
        //private DescribeOSSObject mLastDirectory;
        private string mDeepestKey;
        private string mUploadFolder;

        //public delegate void BackupTaskHandler(object sender, BackupTask task);
        //public event BackupTaskHandler BackupTaskEvent;
        public EventHandler<DownUploadTask> BackupTaskEvent;
        public MainWindow mMainWindow { get; set; }


        public BucketPage()
        {
            InitializeComponent();

            mAki = App.AKI;
            mAks = App.AKS;
            Thread t = new Thread(GetBuckets);
            t.Start();
        }

        private void GotBuckets(object obj)
        {
            mSelBuckets = obj as ObservableCollection<DescribeBucket>;
            BucketList.ItemsSource = mSelBuckets;
            SelectDefaultIndex();
        }

        private void GetBuckets()
        {
            ObservableCollection<DescribeBucket> buckets = new ObservableCollection<DescribeBucket>();
            string endpoint = "http://oss-cn-hangzhou.aliyuncs.com";
            OssClient client = new OssClient(endpoint, mAki, mAks);
            foreach (Bucket b in client.ListBuckets())
            {
                DescribeBucket bucket = new DescribeBucket(b);
                buckets.Add(bucket);
            }
            Dispatcher.Invoke(new DelegateGot(GotBuckets), buckets);
        }

        private void SelectDefaultIndex()
        {
            if (BucketList.SelectedIndex == -1)
            {
                BucketList.SelectedIndex = 0;
            }
        }

        private void BucketList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BucketList.SelectedIndex == -1)
            {
                return;
            }

            SetBucket(BucketList.SelectedItem as DescribeBucket);
        }

        private void SetBucket(DescribeBucket bucket)
        {
            mSelBucket = bucket;
            Thread t = new Thread(new ParameterizedThreadStart(GetSettings));
            t.Start(mSelBucket);

            ClearFileList();
            mRootDirectory = new DescribeOSSObject(mSelBucket);
            mCurrDirectory = null;
            mDeepestKey = mRootDirectory.Key;
            StartGetObjects(mRootDirectory);
        }

        private void GotSettings(object obj)
        {
            OverView.DataContext = mSelBucket;
        }

        private void GetSettings(object obj)
        {
            DescribeBucket bucket = obj as DescribeBucket;
            string endpoint = "http://" + bucket.InternetEndPoint;
            OssClient client = new OssClient(endpoint, mAki, mAks);

            try
            {
                AccessControlList acl = client.GetBucketAcl(bucket.Name); //读写权限
                bucket.ACL = acl.ACL.ToString();
            }
            catch
            {
                bucket.ACL = "Private";
            }

            try
            {
                IList<CORSRule> crules = client.GetBucketCors(bucket.Name); //跨域共享
                bucket.CORSRule = "Enabled";
            }
            catch
            {
                bucket.CORSRule = "Disabled";
            }

            try
            {
                IList<LifecycleRule> lrules = client.GetBucketLifecycle(bucket.Name); //生命周期
                bucket.LifecycleRule = "Enabled";
            }
            catch
            {
                bucket.LifecycleRule = "Disabled";
            }

            try
            {
                BucketLoggingResult logging = client.GetBucketLogging(bucket.Name); //日志管理
                if (logging.TargetBucket != null || logging.TargetPrefix != null)
                {
                    bucket.Logging = "Enabled";
                }
                else
                {
                    bucket.Logging = "Disabled";
                }
            }
            catch
            {
                bucket.Logging = "Disabled";
            }

            try
            {
                RefererConfiguration referer = client.GetBucketReferer(bucket.Name); //防盗链
                if (referer.RefererList.Referers != null)
                {
                    bucket.Referer = "Enabled";
                }
                else
                {
                    bucket.Referer = "Disabled";
                }
            }
            catch
            {
                bucket.Referer = "Disabled";
            }

            try
            {
                BucketWebsiteResult website = client.GetBucketWebsite(bucket.Name); //静态页面
                if (website.IndexDocument != null || website.ErrorDocument != null)
                {
                    bucket.Website = "Enabled";
                }
                else
                {
                    bucket.Website = "Disabled";
                }
            }
            catch
            {
                bucket.Website = "Disabled";
            }

            Dispatcher.Invoke(new DelegateGot(GotSettings), bucket);
        }

        private void ClearFileList()
        {
            if (mCurrDirectory != null && mCurrDirectory.ChildObjects != null)
            {
                mCurrDirectory.ChildObjects.Clear();
            }
        }

        private void GotObjects(object obj)
        {
            DescribeOSSObject directory = obj as DescribeOSSObject;
            if (mCurrDirectory != null && mCurrDirectory != directory)
            {
                mCurrDirectory.ChildObjects.Clear();
            }
            mCurrDirectory = directory;
            FileList.ItemsSource = mCurrDirectory.ChildObjects;
            FileManager.DataContext = mCurrDirectory;
            if (!mDeepestKey.Contains(mCurrDirectory.Key))
            {
                mDeepestKey = mCurrDirectory.Key;
            }
        }

        private void GetObjects(object obj)
        {
            DescribeOSSObject directory = obj as DescribeOSSObject;
            DescribeBucket bucket = directory.Bucket;
            string endpoint = "http://" + bucket.InternetEndPoint;
            OssClient client = new OssClient(endpoint, mAki, mAks);
            bool isTruncated = false;
            ObservableCollection<DescribeOSSObject> childs = new ObservableCollection<DescribeOSSObject>();

            do
            {
                try
                {
                    /*Dictionary<string, DescribeOSSObject> dictionary = new Dictionary<string, DescribeOSSObject>();
                    if (directory.Key == directory.BucketName) //Bucket root
                    {
                        listing = client.ListObjects(bucket.Name);
                        foreach (OssObjectSummary s in listing.ObjectSummaries)
                        {
                            bool ischild = false;
                            if (s.Key.IndexOf('/') < 0 || s.Key.IndexOf('/') == s.Key.Length - 1)
                            {
                                ischild = true;
                            }

                            if (ischild)
                            {
                                DescribeOSSObject o = new DescribeOSSObject(s);
                                o.Bucket = bucket;
                                o.ParentObject = directory;
                                childs.Add(o);
                            }
                        }
                    }
                    else*/
                    {
                        var request = new ListObjectsRequest(bucket.Name)
                        {
                            Prefix = directory.Key,
                            Delimiter = "/"
                        };
                        var result = client.ListObjects(request);

                        foreach (string prefix in result.CommonPrefixes)
                        {
                            DescribeOSSObject o = new DescribeOSSObject(result.BucketName, prefix);
                            o.Bucket = bucket;
                            o.ParentObject = directory;
                            childs.Add(o);
                        }

                        foreach (OssObjectSummary s in result.ObjectSummaries)
                        {
                            /*bool ischild = false;
                            string next = s.Key.Substring(directory.Key.Length);

                            if (next == null || next.Length == 0)
                            {
                                continue;
                            }

                            int index = next.IndexOf('/');
                            DescribeOSSObject o = null;
                            if (index < 0 || index == next.Length - 1) //子文件或者子文件夹
                            {
                                ischild = true;
                                o = new DescribeOSSObject(s);
                                //一般不会发生，因为子文件夹一般都是在子文件夹下一级对象出现，这里为了防止意外
                                if (dictionary.ContainsKey(o.Key))
                                {
                                    foreach (DescribeOSSObject child in childs)
                                    {
                                        if (child.Key == o.Key)
                                        {
                                            childs.Remove(child);
                                        }
                                    } 
                                }
                            }
                            else
                            {
                                /*子文件夹下一级的对象，获得这个子文件夹的key，确认是否已经添加过这个key的子文件夹，
                                  因为子文件夹的key在下一级对象的key的前面，如果没有添加可以认为在bucket中这个子文件不存在，
                                  如果不存在，在文件列表中创建一个虚拟文件夹，key在bucket中实际不存在。*/
                                /*next = next.Substring(0, index + 1);
                                string key = directory.Key + next;
                                if (!dictionary.ContainsKey(key))
                                {
                                    ischild = true;
                                    o = new DescribeOSSObject(s.BucketName, key);
                                }
                            }

                            if (ischild)
                            {
                                o.Bucket = bucket;
                                o.ParentObject = directory;
                                childs.Add(o);
                                dictionary.Add(o.Key, o);
                            }*/
                            if (s.Key != directory.Key)
                            {
                                DescribeOSSObject o = new DescribeOSSObject(s);
                                o.Bucket = bucket;
                                o.ParentObject = directory;
                                childs.Add(o);
                            }
                        }

                        isTruncated = result.IsTruncated;
                    }
                }
                catch
                {

                }
            } while (isTruncated);

            directory.ChildObjects = childs;
            Dispatcher.Invoke(new DelegateGot(GotObjects), directory);
        }

        private void StartGetObjects(DescribeOSSObject obj)
        {
            Thread t = new Thread(GetObjects);
            t.Start(obj);
        }

        private void FileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView view = sender as ListView;
            DescribeOSSObject obj = view.SelectedItem as DescribeOSSObject;
            if (obj.ObjectType == DescribeOSSObject.OSSObjectType.Directory)
            {
                StartGetObjects(obj);
                //mCurrDirectory = obj;
                //view.ItemsSource = obj.ChildObjects;
                //FileManager.DataContext = mCurrDirectory;
                //mLastDirectory = mCurrDirectory;
            }
            else
            {
                DownloadObject(obj);
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrDirectory.ParentObject != null)
            {
                StartGetObjects(mCurrDirectory.ParentObject);
                //mCurrDirectory = mCurrDirectory.ParentObject;
                //FileList.ItemsSource = mCurrDirectory.ChildObjects;
                //FileManager.DataContext = mCurrDirectory;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (mDeepestKey != null && mDeepestKey != mCurrDirectory.Key)
            {
                int index = -1;
                string key = null;

                /*if (mCurrDirectory.Key == mCurrDirectory.BucketName)
                {
                    index = mLastDirectory.Key.IndexOf('/');
                    key = mLastDirectory.Key.Substring(0, index + 1);
                }
                else*/
                {
                    if (mDeepestKey.Contains(mCurrDirectory.Key))
                    {
                        string next = mDeepestKey.Substring(mCurrDirectory.Key.Length);
                        index = next.IndexOf('/');
                        next = next.Substring(0, index + 1);
                        key = mCurrDirectory.Key + next;
                    }
                }

                foreach (DescribeOSSObject obj in mCurrDirectory.ChildObjects)
                {
                    if (key != null && key == obj.Key)
                    {
                        StartGetObjects(obj);
                        break;
                    }
                }
            }
        }

        private bool IsDirectory(string path)
        {
            FileAttributes attributes = File.GetAttributes(path);
            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return true;
            }

            return false;
        }

        private DownUploadTask CreateOssTask(DescribeBucket bucket, string path, string key)
        {
            DownUploadTask task = new DownUploadTask();
            task.InstanceType = "OSS";
            task.InstanceName = bucket.Name;
            task.Instance = bucket;
            task.FilePath = path;
            if (key == bucket.Name)
            {
                task.URL = "";
            }
            else
            {
                task.URL = key;
            } 
            return task;
        } 

        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = "选择上传文件";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string key = mCurrDirectory.Key + dialog.FileName.Substring(dialog.FileName.LastIndexOf('\\') + 1);
                DownUploadTask task = CreateOssTask(mSelBucket, dialog.FileName, key);
                task.FileName = key;
                task.FileType = FileTypeMode.File;
                task.TaskType = TaskTypeMode.Upload;
                BackupTaskEvent?.Invoke(this, task);
            }
        }

        private void DispatchUploadTask(string path)
        {
            if (IsDirectory(path))
            {
                DownUploadTask task = null;
                string key = mCurrDirectory.Key + path.Substring(mUploadFolder.LastIndexOf('\\') + 1).Replace('\\', '/') + '/';
                task = CreateOssTask(mSelBucket, path, key);
                task.FileName = key;
                task.FileType = FileTypeMode.Directory;
                task.TaskType = TaskTypeMode.Upload;
                BackupTaskEvent?.Invoke(this, task);

                foreach (string directory in Directory.GetDirectories(path))
                {
                    DispatchUploadTask(directory);
                }

                foreach (string file in Directory.GetFiles(path))
                {
                    key = mCurrDirectory.Key + file.Substring(mUploadFolder.LastIndexOf('\\') + 1).Replace('\\', '/');
                    task = CreateOssTask(mSelBucket, file, key);
                    task.FileName = key;
                    task.FileType = FileTypeMode.File;
                    task.TaskType = TaskTypeMode.Upload;
                    BackupTaskEvent?.Invoke(this, task);
                }
            }
        }

        private void UploadFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = "选择上传文件夹";
            dialog.IsFolderPicker = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                mUploadFolder = dialog.FileName;
                DispatchUploadTask(dialog.FileName);
            }
        }

        private void DispatchDownloadTask(object obj)
        {
            DownUploadTask task = obj as DownUploadTask;
            BackupTaskEvent?.Invoke(this, task);
        }

        private void GetDownloadObjects(object obj)
        {
            object[] para = obj as object[];
            DescribeOSSObject directory = para[0] as DescribeOSSObject;
            string path = para[1] as string;
            DescribeBucket bucket = directory.Bucket;
            string endpoint = "http://" + bucket.InternetEndPoint;
            OssClient client = new OssClient(endpoint, mAki, mAks);
            try
            {
                ObjectListing listing = client.ListObjects(bucket.Name, directory.Key);
                foreach (OssObjectSummary s in listing.ObjectSummaries)
                {
                    DescribeOSSObject o = new DescribeOSSObject(s);
                    string realPath = path + '\\' + (directory.Name + o.Path.Substring(directory.Path.Length)).Replace('/', '\\');
                    DownUploadTask task = CreateOssTask(bucket, realPath, o.Key);
                    task.FileName = o.Key;
                    if (o.ObjectType == OSSObjectType.Directory)
                    {
                        task.FileType = FileTypeMode.Directory;
                        if (!Directory.Exists(realPath))
                        {
                            Directory.CreateDirectory(realPath);
                        }
                    }
                    else
                    {
                        task.FileType = FileTypeMode.File;
                    }
                    task.TaskType = TaskTypeMode.Download;
                    Dispatcher.Invoke(new DelegateGot(DispatchDownloadTask), task);
                }
            }
            catch
            {
            }
        }

        private void DownloadObject(DescribeOSSObject obj)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "选择保存位置";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (obj.ObjectType == OSSObjectType.File)
                {
                    string path = dialog.FileName + '\\' + obj.Name;
                    DownUploadTask task = CreateOssTask(mSelBucket, path, obj.Key);
                    task.FileName = obj.Key;
                    task.FileType = FileTypeMode.File;
                    task.TaskType = TaskTypeMode.Download;
                    BackupTaskEvent?.Invoke(this, task);
                }
                else
                {
                    Thread t = new Thread(new ParameterizedThreadStart(GetDownloadObjects));
                    object[] para = new object[2];
                    para[0] = obj;
                    para[1] = dialog.FileName;
                    t.Start(para);
                }
            }
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            DescribeOSSObject obj = FileList.SelectedItem as DescribeOSSObject;
            if (obj == null)
            {
                return;
            }

            DownloadObject(obj);
        }
        
        private void DeleteObject(object para)
        {
            DescribeOSSObject obj = para as DescribeOSSObject;
            DescribeBucket bucket = mSelBucket;
            if (obj.BucketName != bucket.Name)
            {
                return;
            }

            string endpoint = "http://" + bucket.InternetEndPoint;
            OssClient client = new OssClient(endpoint, mAki, mAks);
            try
            {
                if (obj.ObjectType == OSSObjectType.Directory)
                {
                    var listing = client.ListObjects(bucket.Name, obj.Key);
                    var keys = new List<string>();
                    foreach (OssObjectSummary s in listing.ObjectSummaries)
                    {
                        keys.Add(s.Key);
                    }
                    var request = new DeleteObjectsRequest(bucket.Name, keys, false);
                    client.DeleteObjects(request);
                }
                else
                {
                    client.DeleteObject(bucket.Name, obj.Key);
                }
                Dispatcher.Invoke(new DelegateGot(DeletedObject), obj);
            }
            catch
            {
            }
        }

        private void DeletedObject(object para)
        {
            DescribeOSSObject obj = para as DescribeOSSObject;
            if (obj != null && obj.ParentObject != null)
            {
                obj.ParentObject.ChildObjects.Remove(obj);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DescribeOSSObject obj = FileList.SelectedItem as DescribeOSSObject;
            if (obj == null)
            {
                return;
            }

            Thread t = new Thread(new ParameterizedThreadStart(DeleteObject));
            t.Start(obj);
        }

        private void GetUrl_Click(object sender, RoutedEventArgs e)
        {
            DescribeOSSObject obj = FileList.SelectedItem as DescribeOSSObject;
            if (obj == null)
            {
                return;
            }

            string endpoint = "http://" + mSelBucket.InternetEndPoint;
            OssClient client = new OssClient(endpoint, mAki, mAks);
            GetUrlWindow win = new GetUrlWindow(client, mSelBucket, obj);
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.ShowDialog();
        }

        private void HttpHeader_Click(object sender, RoutedEventArgs e)
        {
            DescribeOSSObject obj = FileList.SelectedItem as DescribeOSSObject;
            if (obj == null)
            {
                return;
            }

            string endpoint = "http://" + mSelBucket.InternetEndPoint;
            OssClient client = new OssClient(endpoint, mAki, mAks);
            HttpHeaderWindow win = new HttpHeaderWindow(client, mSelBucket, obj);
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.ShowDialog();
        }

        private void CreateFolder_Click(object sender, RoutedEventArgs e)
        {
            string endpoint = "http://" + mSelBucket.InternetEndPoint;
            OssClient client = new OssClient(endpoint, mAki, mAks);
            EditNameWindow win = new EditNameWindow(client, mSelBucket, mCurrDirectory.Key);
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.Owner = mMainWindow;
            win.UpdateEventHandler += UpdateDirectory;
            win.ShowDialog();
        }

        private void UpdateDirectory(object sender, string key)
        {
            if (mCurrDirectory.Key == key)
            {
                StartGetObjects(mCurrDirectory);
            }
        }
    }
}
