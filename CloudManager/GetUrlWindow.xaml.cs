using Aliyun.OSS;
using System;
using System.Collections.Generic;
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
    /// GetUrlWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GetUrlWindow : Window
    {
        private OssClient mClient;
        private DescribeBucket mBucket;
        private DescribeOSSObject mObject;

        public string ObjectName { get; set; }
        public long Period { get; set; }
        public string URL { get; set; }


        public GetUrlWindow(OssClient c, DescribeBucket b, DescribeOSSObject o)
        {
            InitializeComponent();
            this.DataContext = this;
            mClient = c;
            mBucket = b;
            mObject = o;

            Period = 3600;
            ObjectName = o.Name;

            Thread t = new Thread(new ParameterizedThreadStart(GetUrl));
            t.Start(Period);
        }

        private void GetUrl(object obj)
        {
            long? period = obj as long?;
            GeneratePresignedUriRequest request = new GeneratePresignedUriRequest(mBucket.Name, mObject.Key, SignHttpMethod.Get)
            {
                Expiration = DateTime.Now.AddSeconds((double)period)
            };
            var uri = mClient.GeneratePresignedUri(request);
            URL = uri.ToString();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(URL);
        }
    }
}
