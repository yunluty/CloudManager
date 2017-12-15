using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudManager
{
    public class DescribeOSSObject
    {
        public DescribeOSSObject(DescribeBucket bucket)
        {
            Bucket = bucket;
            BucketName = bucket.Name;
            Key = "";
            Path = bucket.Name;
            ObjectType = OSSObjectType.Directory;
            Name = bucket.Name;
        }

        public DescribeOSSObject(OssObjectSummary summary)
        {
            BucketName = summary.BucketName;
            Key = summary.Key;
            GetObjectType();
            GetPath();
            Size = summary.Size;
            CreatTime = (summary.LastModified.ToLocalTime()).ToString("yyyy-MM-dd HH:mm:ss");
        }

        public DescribeOSSObject(string bucketName, string key)
        {
            BucketName = bucketName;
            Key = key;
            GetObjectType();
            GetPath();
            Size = -1; //For directory, set the size less than zero
            CreatTime = "-";
        }

        public string BucketName { get; }
        public DescribeBucket Bucket { get; set; }
        public string Key { get; }
        public string Path { get; set; }
        public OSSObjectType ObjectType { get; set; }
        public string Name { get; set; }
        public long Size { get; }
        public string CreatTime { get; }
        public ObservableCollection<DescribeOSSObject> ChildObjects { get; set; }
        public DescribeOSSObject ParentObject { get; set; }

        private void GetObjectType()
        {
            if (Key[Key.Length - 1] == '/')
            {
                ObjectType = OSSObjectType.Directory;
            }
            else
            {
                ObjectType = GetFileType(Key);
            }
        }

        private OSSObjectType GetFileType(string key)
        {
            int index = key.LastIndexOf('.');
            if (index > 0)
            {
                string ext = key.Substring(index + 1);
                
                if (ext.Equals("exe", StringComparison.CurrentCultureIgnoreCase))
                {
                    return OSSObjectType.ExeFile;
                }
                else if (ext.Equals("jpg", StringComparison.CurrentCultureIgnoreCase)
                    || ext.Equals("png", StringComparison.CurrentCultureIgnoreCase)
                    || ext.Equals("bmp", StringComparison.CurrentCultureIgnoreCase))
                {
                    return OSSObjectType.ImageFile;
                }
                else if (ext.Equals("txt", StringComparison.CurrentCultureIgnoreCase))
                {
                    return OSSObjectType.TxtFile;
                }
                else if (ext.Equals("exe", StringComparison.CurrentCultureIgnoreCase))
                {
                    return OSSObjectType.ExeFile;
                }
                else if (ext.Equals("rar", StringComparison.CurrentCultureIgnoreCase)
                    || ext.Equals("zip", StringComparison.CurrentCultureIgnoreCase))
                {
                    return OSSObjectType.RarFile;
                }
                else if (ext.Equals("doc", StringComparison.CurrentCultureIgnoreCase)
                    || ext.Equals("docx", StringComparison.CurrentCultureIgnoreCase))
                {
                    return OSSObjectType.DocFile;
                }
                else if (ext.Equals("xls", StringComparison.CurrentCultureIgnoreCase)
                    || ext.Equals("xlsx", StringComparison.CurrentCultureIgnoreCase))
                {
                    return OSSObjectType.XlsFile;
                }
                else if (ext.Equals("mp3", StringComparison.CurrentCultureIgnoreCase)
                    || ext.Equals("mid", StringComparison.CurrentCultureIgnoreCase)
                    || ext.Equals("wav", StringComparison.CurrentCultureIgnoreCase))
                {
                    return OSSObjectType.MusicFile;
                }
                else if (ext.Equals("pdf", StringComparison.CurrentCultureIgnoreCase))
                {
                    return OSSObjectType.PdfFile;
                }
                else if (ext.Equals("ppt", StringComparison.CurrentCultureIgnoreCase)
                    || ext.Equals("pptx", StringComparison.CurrentCultureIgnoreCase))
                {
                    return OSSObjectType.PptFile;
                }
            }
            
            return OSSObjectType.Unknown;
        }

        private void GetPath()
        {
            string path = null;
            if (ObjectType == OSSObjectType.Directory)
            {
                path = Key.Substring(0, Key.Length - 1); //Remove the last '/'
            }
            else
            {
                path = Key;
            }
            path = BucketName + '/' + path;

            int index = path.LastIndexOf('/');
            Name = path.Substring(index + 1);
            Path = path;
        }

        public enum OSSObjectType
        {
            Directory = 0,
            Unknown = 1,
            ImageFile = 2,
            ExeFile = 3,
            TxtFile = 4,
            RarFile = 5,
            DocFile = 6,
            XlsFile = 7,
            MusicFile = 8,
            PdfFile = 9,
            PptFile = 10
        }
    }
}
