using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// BucketEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FolderNameWindow : Window
    {
        private OssClient mClient;
        private DescribeBucket mBucket;
        private string mCurrKey;

        public bool EnableButton;
        public bool EnableText;
        public string DirectoryName { get; set; }
        public EventHandler<string> UpdateEventHandler;


        public FolderNameWindow(OssClient c, DescribeBucket b, string k)
        {
            InitializeComponent();
            this.DataContext = this;
            mClient = c;
            mBucket = b;
            mCurrKey = k;
        }

        private void CreatedFolder()
        {
            UpdateEventHandler?.Invoke(this, mCurrKey);
            this.Close();
        }

        private void CreatedFail()
        {
            EnableButton = true;
        }

        private void CreateFolder(object obj)
        {
            string key = obj as string;
            Stream stream = null;
            try
            {
                stream = new MemoryStream();
                mClient.PutObject(mBucket.Name, key, stream);
                Dispatcher.Invoke(new Action(CreatedFolder));
            }
            catch
            {
                Dispatcher.Invoke(new Action(CreatedFail));
            }
            finally
            {
                stream.Close();
            }
            
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            EnableButton = false;
            string key = mCurrKey + DirectoryName + '/';
            Thread t = new Thread(new ParameterizedThreadStart(CreateFolder));
            t.Start(key);
        }
    }
}
