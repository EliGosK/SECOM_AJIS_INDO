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
    public partial class RPTChangeNoticeDo
    {
        public string ContractCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string DocNoShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(DocNo, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        //private static byte[] m_ImageSignature = null;
        //public byte[] ImageSignature
        //{
        //    get
        //    {
        //        if (String.IsNullOrEmpty(ImageSignaturePath) == false)
        //        {
        //            string path = string.Format("{0}bin/{1}", CommonUtil.WebPath, ImageSignaturePath);
        //            FileStream fs = new FileStream(path, FileMode.Open);
        //            BinaryReader br = new BinaryReader(fs);
        //            int length = (int)br.BaseStream.Length;
        //            m_ImageSignature = new byte[length];
        //            m_ImageSignature = br.ReadBytes(length);
        //            br.Close();
        //            fs.Close();                
        //        }

        //        return m_ImageSignature;
        //    }
        //}

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
