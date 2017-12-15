using Aliyun.Acs.Domain.Model.V20160511;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aliyun.Acs.Domain.Model.V20160511.GetWhoisInfoResponse;
using static Aliyun.Acs.Domain.Model.V20160511.QueryDomainListResponse;

namespace CloudManager.Domain
{
    public class DescribeDomain : INotifyPropertyChanged
    {
        public DescribeDomain(QueryDomainList_Domain d)
        {
            DomainName = d.DomainName;
            SaleId = d.SaleId;
            CreationDate = d.RegDate;
            ExpirationDate = d.DeadDate;
            DomainAuditStatus = d.DomainAuditStatus;
        }

        public void SetDomainInfo(QueryDomainBySaleIdResponse rs)
        {
            UserId = rs.UserId;
            DomainRegType = rs.DomainRegType;
            EnglishHolder = rs.EnglishHolder;
            ChineseHolder = rs.ChineseHolder;
            EnglishContactPerson = rs.EnglishContactPerson;
            ChineseContactPerson = rs.ChineseContactPerson;
            HolderEmail = rs.HolderEmail;
            TransferOutStatus = rs.TransferOutStatus;
            SafetyLock = rs.SafetyLock;
            TransferLock = rs.TransferLock;
            WhoisProtected = rs.WhoisProtected;
            DNSList = rs.DnsList;
        }

        public void SetWhoisInfo(GetWhoisInfoResponse rs)
        {
            Registrar = rs.Registrar;
            if (rs.DomainStatusList.Count > 0)
            {
                DomainStatusList = new List<string>();
                foreach (GetWhoisInfo_DomainStatus s in rs.DomainStatusList)
                {
                    string status = s.Desc + "(" + s.Status + ")";
                    DomainStatusList.Add(status);
                }
            }
        }

        public string DomainName { get; }
        public string SaleId { get; }
        public string UserId { get; set; }
        private string creationdate;
        public string CreationDate
        {
            get { return creationdate; }
            set
            {
                DateTime date;
                if (DateTime.TryParse(value, out date))
                {
                    creationdate = date.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                }
                NotifyPropertyChanged("CreationDate");
            }
        }
        private string expirationdate;
        public string ExpirationDate
        {
            get { return expirationdate; }
            set
            {
                DateTime date;
                if (DateTime.TryParse(value, out date))
                {
                    expirationdate = date.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                }
                NotifyPropertyChanged("ExpirationDate");
            }
        }
        public string DomainRegType { get; set; }
        private string domainauditstatus;
        public string DomainAuditStatus
        {
            get { return domainauditstatus; }
            set
            {
                switch (value)
                {
                    case "NONAUDIT":
                        domainauditstatus = "未实名认证";
                        break;

                    case "AUDITING":
                        domainauditstatus = "审核中";
                        break;

                    case "SUCCEED":
                        domainauditstatus = "已实名认证";
                        break;

                    case "FAILED":
                        domainauditstatus = "实名认证失败";
                        break;
                }
                NotifyPropertyChanged("DomainAuditStatus");
            }
        }
        private string englishholder;
        public string EnglishHolder
        {
            get { return englishholder; }
            set
            {
                englishholder = value;
                NotifyPropertyChanged("EnglishHolder");
            }
        }
        private string chineseholder;
        public string ChineseHolder
        {
            get { return chineseholder; }
            set
            {
                chineseholder = value;
                NotifyPropertyChanged("ChineseHolder");
            }
        }
        private string englishContactPerson;
        public string EnglishContactPerson
        {
            get { return englishContactPerson; }
            set
            {
                englishContactPerson = value;
                NotifyPropertyChanged("EnglishContactPerson");
            }
        }
        private string chineseContactPerson;
        public string ChineseContactPerson
        {
            get { return chineseContactPerson; }
            set
            {
                chineseContactPerson = value;
                NotifyPropertyChanged("ChineseContactPerson");
            }
        }
        private string holderemail;
        public string HolderEmail
        {
            get { return holderemail; }
            set
            {
                holderemail = value;
                NotifyPropertyChanged("HolderEmail");
            }
        }
        public string TransferOutStatus { get; set; }
        public string SafetyLock { get; set; }
        public string TransferLock { get; set; }
        public bool? WhoisProtected { get; set; }
        public List<string> DNSList { get; set; }
        private string registrar;
        public string Registrar
        {
            get { return registrar; }
            set
            {
                registrar = value;
                NotifyPropertyChanged("Registrar");
            }
        }
        public List<string> DomainStatusList { get; set; }
        public ObservableCollection<DescribeDomainRecord> Records { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
