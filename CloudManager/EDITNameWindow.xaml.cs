﻿using Aliyun.Acs.Core;
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

        private void SetDBInstanceName(object obj)
        {
            string name = obj as string;
            ModifyDBInstanceDescriptionRequest request = new ModifyDBInstanceDescriptionRequest();
            request.DBInstanceId = mDBInstance.DBInstanceId;
            request.DBInstanceDescription = name;
            try
            {
                ModifyDBInstanceDescriptionResponse response = mAcsClient.GetAcsResponse(request);
                Dispatcher.Invoke(new DelegateGot(SetDBInstanceNameSuccess), obj);
            }
            catch
            {
                Dispatcher.Invoke(new Action(SetDBInstanceNameFail));
            }
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

        private void SetBalancerName(object obj)
        {
            string name = obj as string;
            SetLoadBalancerNameRequest request = new SetLoadBalancerNameRequest();
            request.LoadBalancerId = mBalancer.LoadBalancerId;
            request.LoadBalancerName = name;
            try
            {
                SetLoadBalancerNameResponse response = mAcsClient.GetAcsResponse(request);
                Dispatcher.Invoke(new DelegateGot(SetNameSuccess), obj);
            }
            catch
            {
                Dispatcher.Invoke(new Action(SetNameFail));
            }
        }

        private void CreatedFolder()
        {
            UpdateEventHandler?.Invoke(this, mCurrKey);
            this.Close();
        }

        private void CreatedFail()
        {
        }

        private void CreateFolder(object obj)
        {
            string key = obj as string;
            Stream stream = null;
            try
            {
                stream = new MemoryStream();
                mOssClient.PutObject(mBucket.Name, key, stream);
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
            if (mEditingType == EditingType.BucketFolderName)
            {
                string key = mCurrKey + EditName + '/';
                Thread t = new Thread(new ParameterizedThreadStart(CreateFolder));
                t.Start(key);
            }
            else if (mEditingType == EditingType.BlancerName)
            {
                string name = String.Copy(EditName);
                Thread t = new Thread(new ParameterizedThreadStart(SetBalancerName));
                t.Start(name);
            }
            else if (mEditingType == EditingType.DBInstanceName)
            {
                string name = String.Copy(EditName);
                Thread t = new Thread(new ParameterizedThreadStart(SetDBInstanceName));
                t.Start(name);
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
