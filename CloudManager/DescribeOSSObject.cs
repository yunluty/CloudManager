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
                ObjectType = OSSObjectType.File;
            }
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
            File = 0,
            Directory = 1
        }
    }
}
