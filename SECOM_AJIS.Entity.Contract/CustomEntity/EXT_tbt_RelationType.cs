using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class tbt_RelationType
    {
        public string RelatedContractCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertContractCode(this.RelatedContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string ProductName { get; set; }
        public string ProductTypeCode { get; set; }
    }
}
