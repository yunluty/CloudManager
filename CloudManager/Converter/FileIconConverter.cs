using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CloudManager.Converter
{
    class FileIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as DescribeOSSObject.OSSObjectType?;

            if (type == null)
            {
                return null;
            }

            if (type == DescribeOSSObject.OSSObjectType.Directory)
            {
                return "images/folder.png";
            }
            else if (type == DescribeOSSObject.OSSObjectType.ImageFile)
            {
                return "images/image_file.png";
            }
            else if (type == DescribeOSSObject.OSSObjectType.TxtFile)
            {
                return "images/text_file.png";
            }
            else if (type == DescribeOSSObject.OSSObjectType.ExeFile)
            {
                return "images/exe_file.png";
            }
            else if (type == DescribeOSSObject.OSSObjectType.RarFile)
            {
                return "images/rar_file.png";
            }
            else if (type == DescribeOSSObject.OSSObjectType.DocFile)
            {
                return "images/word_file.png";
            }
            else if (type == DescribeOSSObject.OSSObjectType.XlsFile)
            {
                return "images/excle_file.png";
            }
            else if (type == DescribeOSSObject.OSSObjectType.MusicFile)
            {
                return "images/music_file.png";
            }
            else if (type == DescribeOSSObject.OSSObjectType.PdfFile)
            {
                return "images/pdf_file.png";
            }
            else if (type == DescribeOSSObject.OSSObjectType.PptFile)
            {
                return "images/ppt_file.png";
            }
            else
            {
                return "images/unknown_file.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
