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
    public partial class EditRecordWindow : Window
    {
        private DescribeDomainRecord mRecord;
        private DefaultAcsClient mClient;

        public EditRecordWindow(DefaultAcsClient client, DescribeDomain domain)
        {
            InitializeComponent();
            mClient = client;
            mRecord = new DescribeDomainRecord(domain);
            this.DataContext = mRecord;
        }

        public EditRecordWindow(DefaultAcsClient client, DescribeDomainRecord record)
        {
            InitializeComponent();
            mClient = client;
            mRecord = new DescribeDomainRecord(record);
            this.DataContext = mRecord;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                try
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
                catch
                {
                }
            });
        }

        
    }

    class SimpleComboBoxItem
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
