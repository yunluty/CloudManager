using Aliyun.Acs.Alidns.Model.V20150109;
using Aliyun.Acs.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CloudManager.Domain
{
    /// <summary>
    /// EditRecordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditRecordWindow : WindowBase
    {
        private DescribeDomainRecord mRecord;
        private DefaultAcsClient mClient;
        private RecordEditType mEditType;
        private bool mUpdateRecords;

        public bool UpdateRecords { get { return mUpdateRecords; } }

        public EditRecordWindow(DefaultAcsClient client, DescribeDomain domain)
        {
            InitializeComponent();
            mClient = client;
            mRecord = new DescribeDomainRecord(domain);
            this.DataContext = mRecord;
            SetValueTipsAndLine(mRecord.Type);
            mEditType = RecordEditType.CreateRecord;
        }

        public EditRecordWindow(DefaultAcsClient client, DescribeDomainRecord record)
        {
            InitializeComponent();
            mClient = client;
            mRecord = new DescribeDomainRecord(record);
            this.DataContext = mRecord;
            SetValueTipsAndLine(mRecord.Type);
            mEditType = RecordEditType.EditRecord;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DoLoadingWork(win =>
            {
                if (mEditType == RecordEditType.CreateRecord)
                {
                    AddDomainRecordRequest request = new AddDomainRecordRequest();
                    request.DomainName = mRecord.DomainName;
                    request.Type = mRecord.Type;
                    request.RR = mRecord.RR;
                    request.Line = mRecord.Line;
                    request.Value = mRecord.Value;
                    if (mRecord.Type.Equals("MX"))
                    {
                        request.Priority = mRecord.Priority;
                    }
                    request.TTL = mRecord.TTL;
                    AddDomainRecordResponse response = mClient.GetAcsResponse(request);
                }
                else
                {
                    UpdateDomainRecordRequest request = new UpdateDomainRecordRequest();
                    request.RecordId = mRecord.RecordId;
                    request.Type = mRecord.Type;
                    request.RR = mRecord.RR;
                    request.Line = mRecord.Line;
                    request.Value = mRecord.Value;
                    if (mRecord.Type.Equals("MX"))
                    {
                        request.Priority = mRecord.Priority;
                    }
                    request.TTL = mRecord.TTL;
                    UpdateDomainRecordResponse response = mClient.GetAcsResponse(request);
                }

                Dispatcher.Invoke(() =>
                {
                    mUpdateRecords = true;
                    this.Close();
                });
            },
            ex =>
            {
                //TODO:
            });
        }

        private void SetValueTipsAndLine(string type)
        {
            switch (type)
            {
                case "A":
                    ValueTips.Text = "A记录的记录值为IPv4形式（如10.10.10.10）";
                    break;

                case "CNAME":
                    ValueTips.Text = "CNAME记录的记录值为域名形式（如abc.example.com）";
                    break;

                case "AAAA":
                    ValueTips.Text = "AAAA记录的记录值为IPv6形式（如ff03:0:0:0:0:0:0:c1）";
                    break;

                case "NS":
                    ValueTips.Text = "NS记录的记录值为域名形式（如ns1.example.com）";
                    break;

                case "MX":
                    ValueTips.Text = "MX记录的记录值为域名形式（如abc.example.com）";
                    break;

                case "SRV":
                    ValueTips.Text = "SRV记录的记录值合法格式为：优先级、空格、权重、空格、端口号、空格、目标地址";
                    break;

                case "TXT":
                    ValueTips.Text = "TXT记录的记录值为字符串形式（只能包含字母、数字、*?-_~=:;.@+^/!”）";
                    break;

                case "CAA":
                    ValueTips.Text = "CAA记录的记录值为字符串形式（只能包含字母、数字、*?-_~=:;.@+^/!”）, 如：0 issue \"ca.example.com\"";
                    break;

                case "REDIRECT_URL":
                    ValueTips.Text = "URL转发记录的值为域名或URL地址";
                    break;

                case "FORWARD_URL":
                    ValueTips.Text = "URL转发记录的值为域名或URL地址";
                    break;
            }

            if (type != null && type.Equals("MX"))
            {
                mRecord.Line = "default";
                LinesComboBox.IsEnabled = false;
            }
            else
            {
                LinesComboBox.IsEnabled = true;
            }
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var type = (sender as ComboBox).SelectedValue as string;
            if (type != null)
            {
                SetValueTipsAndLine(type);
            }
        }

        enum RecordEditType
        {
            CreateRecord = 0,
            EditRecord = 1
        }
    }

    class SimpleComboBoxItem
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
