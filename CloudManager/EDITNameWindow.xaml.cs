using Aliyun.Acs.Core;
using Aliyun.Acs.Rds.Model.V20140815;
using Aliyun.Acs.Slb.Model.V20140515;
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
    public partial class EditNameWindow : WindowBase
    {
        private OssClient mOssClient;
        private DescribeBucket mBucket;
        private string mCurrKey;
        private DefaultAcsClient mAcsClient;
        private DescribeLoadBalancer mBalancer;
        private EditingType mEditingType;
        private DescribeDBInstance mDBInstance;
        
        public string EditName { get; set; }
        public string TextTile { get; set; }
        public string TextTips { get; set; }
        public EventHandler<string> UpdateEventHandler;
        public delegate void DelegateGot(object obj);


        public EditNameWindow(OssClient c, DescribeBucket b, string k)
        {
            InitializeComponent();

            mOssClient = c;
            mBucket = b;
            mCurrKey = k;
            TextTile = "文件夹名称:";
            TextTips = "长度限制为1-254个字符\n不要以「/」打头，不要出现连续的「/」，不允许出现名为「..」的子目录";
            EditNameBox.MaxLength = 254;
            mEditingType = EditingType.BucketFolderName;
            this.Title = "新建文件夹";
            this.DataContext = this;
        }

        public EditNameWindow(DefaultAcsClient c, DescribeLoadBalancer b)
        {
            InitializeComponent();

            mAcsClient = c;
            mBalancer = b;
            TextTile = "名称:";
            TextTips = "长度限制为1-80个字符\n允许包含中文、字母、数字、'-'、'/'、'.'、'_'这些字符";
            EditNameBox.MaxLength = 80;
            mEditingType = EditingType.BlancerName;
            this.Title = "编辑负载均衡名称";
            this.DataContext = this;
        }

        public EditNameWindow(DefaultAcsClient c, DescribeDBInstance i)
        {
            InitializeComponent();

            mAcsClient = c;
            mDBInstance = i;
            TextTile = "名称:";
            TextTips = "长度限制为2-64个字符";
            EditNameBox.MaxLength = 64;
            mEditingType = EditingType.DBInstanceName;
            this.Title = "编辑数据库实例名称";
            this.DataContext = this;
        }

        private void SetDBInstanceNameSuccess(object obj)
        {
            string name = obj as string;
            UpdateEventHandler?.Invoke(mDBInstance, name);
            this.Close();
        }

        private void SetDBInstanceNameFail()
        {
        }

        private void SetDBInstanceName(string name)
        {
            DoLoadingWork(win =>
            {
                ModifyDBInstanceDescriptionRequest request = new ModifyDBInstanceDescriptionRequest();
                request.DBInstanceId = mDBInstance.DBInstanceId;
                request.DBInstanceDescription = name;
                ModifyDBInstanceDescriptionResponse response = mAcsClient.GetAcsResponse(request);
                Dispatcher.Invoke(new DelegateGot(SetDBInstanceNameSuccess), name);
            },
            ex =>
            {
                Dispatcher.Invoke(new Action(SetDBInstanceNameFail));
            });
        }

        private void SetNameSuccess(object obj)
        {
            string name = obj as string;
            UpdateEventHandler?.Invoke(mBalancer, name);
            this.Close();
        }

        private void SetNameFail()
        {
        }

        private void SetBalancerName(string name)
        {
            DoLoadingWork(win =>
            {
                SetLoadBalancerNameRequest request = new SetLoadBalancerNameRequest();
                request.LoadBalancerId = mBalancer.LoadBalancerId;
                request.LoadBalancerName = name;
                SetLoadBalancerNameResponse response = mAcsClient.GetAcsResponse(request);
                Dispatcher.Invoke(new DelegateGot(SetNameSuccess), name);
            },
            ex =>
            {
                Dispatcher.Invoke(new Action(SetNameFail));
            });
        }

        private void CreatedFolder()
        {
            UpdateEventHandler?.Invoke(this, mCurrKey);
            this.Close();
        }

        private void CreatedFail()
        {
        }

        private void CreateFolder(string key)
        {
            Stream stream = new MemoryStream();
            DoLoadingWork(win =>
            {
                mOssClient.PutObject(mBucket.Name, key, stream);
                stream.Close();
                Dispatcher.Invoke(new Action(CreatedFolder));
            },
            ex =>
            {
                stream.Close();
                Dispatcher.Invoke(new Action(CreatedFail));
            });
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (mEditingType == EditingType.BucketFolderName)
            {
                string key = mCurrKey + EditName + '/';
                CreateFolder(key);
            }
            else if (mEditingType == EditingType.BlancerName)
            {
                string name = String.Copy(EditName);
                SetBalancerName(name);
            }
            else if (mEditingType == EditingType.DBInstanceName)
            {
                string name = String.Copy(EditName);
                SetDBInstanceName(name);
            }
        }

        enum EditingType
        {
            BucketFolderName,
            BlancerName,
            DBInstanceName,
            CertificateName
        }

        private void EditNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (mEditingType == EditingType.DBInstanceName)
            {
                if (textBox.Text.Length > 1)
                {
                }
            }
            else if (textBox.Text.Length > 0)
            {
            }
        }
    }
}
