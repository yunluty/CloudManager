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
        public DescribeOSSObject(string bucketName)
        {
            BucketName = bucketName;
            Key = bucketName;
            ObjectType = OSSObjectType.Directory;
            Name = bucketName;
            ChildObjects = new ObservableCollection<DescribeOSSObject>();
        }

        public DescribeOSSObject(OssObjectSummary summary)
        {
            BucketName = summary.BucketName;
            Key = summary.Key;
            GetObjectType();
            GetDirectory();
            Size = summary.Size;
            CreatTime = summary.LastModified.ToString("yyyy-MM-dd hh:mm:ss");
            if (ObjectType == OSSObjectType.Directory)
            {
                ChildObjects = new ObservableCollection<DescribeOSSObject>();
            }
        }

        public string BucketName { get; }
        public string Key { get; }
        public OSSObjectType ObjectType { get; set; }
        public string Name { get; set; }
        public string Directory { get; set; }
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
                ObjectType = OSSObjectType.File;
            }
        }

        private void GetDirectory()
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

            int index = path.LastIndexOf('/');
            if (index >= 0)
            {
                Name = path.Substring(index + 1);
                Directory = path.Substring(0, index + 1);
            }
            else
            {
                Name = path;
                Directory = BucketName;
            }
        }

        public enum OSSObjectType
        {
            File = 0,
            Directory = 1
        }
    }
}
