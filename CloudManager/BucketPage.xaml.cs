using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// BucketPage.xaml 的交互逻辑
    /// </summary>
    public partial class BucketPage : Page
    {
        private delegate void DelegateGot(object obj);
        private DescribeOSSObject mRootDirectory;
        private DescribeOSSObject mCurrDirectory;
        private DescribeOSSObject mLastDirectory;

        public BucketPage()
        {
            InitializeComponent();
        }

        private DescribeBucket _mBucket;
        public DescribeBucket mBucket
        {
            get { return _mBucket; }
            set
            {
                _mBucket = value;
                ClearFileList();
                Thread t1 = new Thread(new ParameterizedThreadStart(GetSettings));
                t1.Start(value);
                Thread t2 = new Thread(new ParameterizedThreadStart(GetObjects));
                t2.Start(value);
            }
        }

        private void GotSettings(object obj)
        {
            OverView.DataContext = mBucket;
        }

        private void GetSettings(object obj)
        {
            DescribeBucket bucket = obj as DescribeBucket;
            string endpoint = "http://" + bucket.InternetEndPoint;
            OssClient client = new OssClient(endpoint, App.AKI, App.AKS);
            AccessControlList acl = client.GetBucketAcl(bucket.Name); //读写权限
            bucket.ACL = acl.ACL.ToString();

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
            if (mRootDirectory != null)
            {
                mRootDirectory.ChildObjects.Clear();
                FileList.ItemsSource = mRootDirectory.ChildObjects;
            }
        }

        private void GotObjects(object obj)
        {
            mRootDirectory = obj as DescribeOSSObject;
            mCurrDirectory = mRootDirectory;
            FileList.ItemsSource = mCurrDirectory.ChildObjects;
            FileManager.DataContext = mCurrDirectory;
        }

        private void GetObjects(object obj)
        {
            DescribeBucket bucket = obj as DescribeBucket;
            Dictionary<string, DescribeOSSObject> dictionary = new Dictionary<string, DescribeOSSObject>();
            DescribeOSSObject root = new DescribeOSSObject(bucket.Name);
            dictionary.Add(root.Key, root);

            string endpoint = "http://" + bucket.InternetEndPoint;
            OssClient client = new OssClient(endpoint, App.AKI, App.AKS);
            bool isTruncated = false;
            do
            {
                try
                {
                    ObjectListing listing = client.ListObjects(bucket.Name);
                    foreach (OssObjectSummary s in listing.ObjectSummaries)
                    {
                        DescribeOSSObject o = new DescribeOSSObject(s);
                        if (dictionary.ContainsKey(o.Directory))
                        {
                            dictionary[o.Directory].ChildObjects.Add(o);
                            o.ParentObject = dictionary[o.Directory];
                        }
                        if (o.ObjectType == DescribeOSSObject.OSSObjectType.Directory)
                        {
                            if (!dictionary.ContainsKey(o.Key))
                            {
                                dictionary.Add(o.Key, o);
                            }
                        }
                    }
                    isTruncated = listing.IsTruncated;
                }
                catch
                {

                }
            } while (isTruncated);

            Dispatcher.Invoke(new DelegateGot(GotObjects), root);
        }

        private void FileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView view = sender as ListView;
            DescribeOSSObject obj = view.SelectedItem as DescribeOSSObject;

            if (obj.ObjectType == DescribeOSSObject.OSSObjectType.Directory)
            {
                mCurrDirectory = obj;
                view.ItemsSource = obj.ChildObjects;
                FileManager.DataContext = mCurrDirectory;
                mLastDirectory = mCurrDirectory;
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrDirectory.ParentObject != null)
            {
                mCurrDirectory = mCurrDirectory.ParentObject;
                FileList.ItemsSource = mCurrDirectory.ChildObjects;
                FileManager.DataContext = mCurrDirectory;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (mLastDirectory != null && mLastDirectory != mCurrDirectory)
            {
                int index = -1;
                string key = null;

                if (mCurrDirectory.Key == mCurrDirectory.BucketName)
                {
                    index = mLastDirectory.Key.IndexOf('/');
                    key = mLastDirectory.Key.Substring(0, index + 1);
                }
                else
                {
                    if (mLastDirectory.Key.Contains(mCurrDirectory.Key))
                    {
                        string next = mLastDirectory.Key.Substring(mCurrDirectory.Key.Length);
                        index = next.IndexOf('/');
                        next = next.Substring(0, index + 1);
                        key = mCurrDirectory.Key + next;
                    }
                }

                foreach (DescribeOSSObject obj in mCurrDirectory.ChildObjects)
                {
                    if (key != null && key == obj.Key)
                    {
                        mCurrDirectory = obj;
                        FileList.ItemsSource = mCurrDirectory.ChildObjects;
                        FileManager.DataContext = mCurrDirectory;
                        break;
                    }
                }
            }
        }

        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateDirectory_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void DeleteObject(object para)
        {
            DescribeOSSObject obj = para as DescribeOSSObject;
            DescribeBucket bucket = mBucket;
            if (obj.BucketName != bucket.Name)
            {
                return;
            }

            string endpoint = "http://" + bucket.InternetEndPoint;
            OssClient client = new OssClient(endpoint, App.AKI, App.AKS);
            try
            {
                client.DeleteObject(bucket.Name, obj.Key);
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

        }

        private void HttpHeader_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
