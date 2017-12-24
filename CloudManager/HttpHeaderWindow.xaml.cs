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
using System.Windows.Shapes;

namespace CloudManager
{
    /// <summary>
    /// HttpHeaderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HttpHeaderWindow : WindowBase
    {
        private OssClient mClient;
        private DescribeBucket mBucket;
        private DescribeOSSObject mObject;
        private ObjectHttpHeader mHttpHeader = new ObjectHttpHeader();
        private ObservableCollection<ObjectParameter> mParameters = new ObservableCollection<ObjectParameter>();
        private delegate void DelegateGot(object obj);

        public HttpHeaderWindow(OssClient c, DescribeBucket b, DescribeOSSObject o)
        {
            InitializeComponent();
            mClient = c;
            mBucket = b;
            mObject = o;
        }

        private void HttpHeaderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GetHttpHeader();
        }

        private void GotHttpHeader(object obj)
        {
            ObjectMetadata metadata = obj as ObjectMetadata;

            var httpdata = metadata.HttpMetadata;
            if (httpdata.ContainsKey("Content-Type"))
            {
                mHttpHeader.Type = httpdata["Content-Type"] as string;
            }
            if (httpdata.ContainsKey("Content-Encoding"))
            {
                mHttpHeader.Encoding = httpdata["Content-Encoding"] as string;
            }
            if (httpdata.ContainsKey("Cache-Control"))
            {
                mHttpHeader.Control = httpdata["Cache-Control"] as string;
            }
            if (httpdata.ContainsKey("Content-Disposition"))
            {
                mHttpHeader.Disposition = httpdata["Content-Disposition"] as string;
            }
            if (httpdata.ContainsKey("Content-Language"))
            {
                mHttpHeader.Language = httpdata["Content-Language"] as string;
            }
            if (httpdata.ContainsKey("Expires"))
            {
                mHttpHeader.Expires = httpdata["Expires"] as string;
            }

            foreach (var userdata in metadata.UserMetadata)
            {
                ObjectParameter para = new ObjectParameter();
                para.Name = userdata.Key;
                para.Value = userdata.Value;
                mParameters.Add(para);
            }

            if (mParameters.Count == 0)
            {
                ObjectParameter para = new ObjectParameter();
                mParameters.Add(para);
            }

            this.HttpHeader.DataContext = mHttpHeader;
            this.UserData.ItemsSource = mParameters;
        }

        private void GetHttpHeader()
        {
            DoLoadingWork(win =>
            {
                ObjectMetadata meta = mClient.GetObjectMetadata(mBucket.Name, mObject.Key);
                Dispatcher.Invoke(new DelegateGot(GotHttpHeader), meta);
            },
            ex =>
            {
                //TODO:
            });
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ObjectParameter para = (sender as Button).DataContext as ObjectParameter;
            mParameters.Remove(para);
        }

        private void AddUserData_Click(object sender, RoutedEventArgs e)
        {
            ObjectParameter para = new ObjectParameter();
            mParameters.Add(para);
        }

        private void ModifiedHttpHeader()
        {
            this.Close();
        }

        private void ModifyHttpHeader()
        {
            DoLoadingWork(win =>
            {
                ObjectMetadata meta = new ObjectMetadata();
                if (!String.IsNullOrEmpty(mHttpHeader.Type))
                {
                    meta.AddHeader("Content-Type", mHttpHeader.Type);
                }
                if (!String.IsNullOrEmpty(mHttpHeader.Encoding))
                {
                    meta.AddHeader("Content-Encoding", mHttpHeader.Encoding);
                }
                if (!String.IsNullOrEmpty(mHttpHeader.Control))
                {
                    meta.AddHeader("Cache-Control", mHttpHeader.Control);
                }
                if (!String.IsNullOrEmpty(mHttpHeader.Disposition))
                {
                    meta.AddHeader("Content-Disposition", mHttpHeader.Disposition);
                }
                if (!String.IsNullOrEmpty(mHttpHeader.Language))
                {
                    meta.AddHeader("Content-Language", mHttpHeader.Language);
                }
                if (!String.IsNullOrEmpty(mHttpHeader.Expires))
                {
                    meta.AddHeader("Expires", mHttpHeader.Expires);
                }

                foreach (ObjectParameter p in mParameters)
                {
                    meta.AddHeader("x-oss-meta-" + p.Name, p.Value);
                }

                mClient.ModifyObjectMeta(mBucket.Name, mObject.Key, meta);
                Dispatcher.Invoke(new Action(ModifiedHttpHeader));
            },
            ex =>
            {
                //TODO:
            });
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            ModifyHttpHeader();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class ObjectHttpHeader
    {
        public string Type { get; set; }
        public string Encoding { get; set; }
        public string Control { get; set; }
        public string Disposition { get; set; }
        public string Language { get; set; }
        public string Expires { get; set; }
    }

    public class ObjectParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
