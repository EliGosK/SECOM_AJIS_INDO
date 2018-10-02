using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class doMaintenanceCompletionReport
    {
        public string ContractCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        
        private string _imageSignatureFullPath = string.Empty;
        public string ImageSignatureFullPath
        {
            get
            {
                if (String.IsNullOrEmpty(ImageSignaturePath) == false)
                    _imageSignatureFullPath = ImageSignaturePath;

                return PathUtil.GetPathValue(PathUtil.PathName.ImageSignaturePath, _imageSignatureFullPath); // ReportUtil.GetImageSignaturePath(_imageSignatureFullPath); //string.Format("{0}bin/{1}", CommonUtil.WebPath, ImageSignaturePath);
            }
        }
    }
}
