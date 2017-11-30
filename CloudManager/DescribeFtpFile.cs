using FluentFTP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudManager
{
    class DescribeFtpObject
    {
        public DescribeFtpObject()
        {
            Name = "/";
            Path = "/";
        }

        public DescribeFtpObject(FtpListItem item)
        {
            Name = item.Name;
            Path = item.Name;
            Size = item.Size;
            ModifiedTime = item.Modified.ToString("yyyy-MM-dd HH:mm:ss");
            if (item.Type == FtpFileSystemObjectType.File)
            {
                ObjectType = FtpObjectType.File;
            }
            else if (item.Type == FtpFileSystemObjectType.Directory)
            {
                ObjectType = FtpObjectType.Directory;
            }
            else if (item.Type == FtpFileSystemObjectType.Link)
            {
                ObjectType = FtpObjectType.Link;
            }
        }

        private DescribeFtpObject parentObject;

        public FtpClient Client { get; set; }
        public string Name { get; }
        public string Path { get; set; }
        public long Size { get; }
        public string ModifiedTime { get; }
        public FtpObjectType ObjectType { get; set; }
        public ObservableCollection<DescribeFtpObject> ChildObjects { get; set; }
        public DescribeFtpObject ParentObject
        {
            get { return parentObject; }
            set
            {
                parentObject = value;
                if (value.Path.IndexOf('/') == value.Path.Length - 1)
                {
                    Path = value.Path + Name;
                }
                else
                {
                    Path = value.Path + '/' + Name;
                }
            }
        }

        public enum FtpObjectType
        {
            File = 0,
            Directory = 1,
            Link = 2
        }
    }
}
