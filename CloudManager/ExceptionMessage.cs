using Aliyun.Acs.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudManager
{
    class ExceptionMessage
    {
        public static string GetErrorMessage(ClientException ex)
        {
            switch (ex.ErrorCode)
            {
                case "InvalidAccessKeyId.NotFound":
                    return "Access Key ID找不到，请检查输入的Access Key是否正确";

                case "InvalidAccessKeyId.Inactive":
                    return "Access Key ID被禁用，请检查 AccessKey 是否正常可用";

                case "IncompleteSignature":
                    return "签名不匹配，请检查Access Key ID和Access Key Secret是否正确";

                case "InternalError":
                    return "内部错误，请重试该操作，如果多次重试报错请提交工单";

                case "UnknownError":
                    return "未知错误，请重试该操作，若再出现该错误请提交工单";

                default:
                    break;
            }

            //Unsupported error code, just show error code and error messgae
            return ex.Message;
        }
    }
}
