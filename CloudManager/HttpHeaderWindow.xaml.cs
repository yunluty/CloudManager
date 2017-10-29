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
    public partial class HttpHeaderWindow : Window
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

            Thread t = new Thread(GetHttpHeader);
            t.Start();
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
                mHttpHeader.Type = httpdata["Content-Encoding"] as string;
            }
            if (httpdata.ContainsKey("Content-Control"))
            {
                mHttpHeader.Type = httpdata["Content-Control"] as string;
            }
            if (httpdata.ContainsKey("Content-Disposition"))
            {
                mHttpHeader.Type = httpdata["Content-Disposition"] as string;
            }
            if (httpdata.ContainsKey("Content-Language"))
            {
                mHttpHeader.Type = httpdata["Content-Language"] as string;
            }
            if (httpdata.ContainsKey("Content-Expires"))
            {
                mHttpHeader.Type = httpdata["Content-Expires"] as string;
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
            try
            {
                ObjectMetadata meta = mClient.GetObjectMetadata(mBucket.Name, mObject.Key);
                Dispatcher.Invoke(new DelegateGot(GotHttpHeader), meta);
            }
            catch
            {
            }
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

        private void OK_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

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
