using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeCACertificatesResponse;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeServerCertificatesResponse;

namespace CloudManager
{
    public class DescribeCertificate : INotifyPropertyChanged
    {
        private string certificateName;

        public DescribeCertificate(DescribeServerCertificates_ServerCertificate c)
        {
            CertificateType = "ServerCertificate";
            CertificateId = c.ServerCertificateId;
            CertificateName = c.ServerCertificateName;
            RegionId = c.RegionId;
            Fingerprint = c.Fingerprint;
        }
        
        public DescribeCertificate(DescribeCACertificates_CACertificate c)
        {
            CertificateType = "CACertificate";
            CertificateId = c.CACertificateId;
            CertificateName = c.CACertificateName;
            RegionId = c.RegionId;
            Fingerprint = c.Fingerprint;
        }

        public string CertificateId { get; set; }
        public string CertificateName
        {
            get { return certificateName; }
            set
            {
                certificateName = value;
                NotifyPropertyChanged("CertificateName");
            }
        }
        public string CertificateType { get; set; }
        public string RegionId { get; set; }
        public string RegionLocalName { get; set; }
        public string Fingerprint { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
