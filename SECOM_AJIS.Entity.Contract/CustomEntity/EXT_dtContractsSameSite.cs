using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtContractsSameSite
    {
        public string ContractCode_Short
        {
            get { return new CommonUtil().ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT); }
        }
        public string CustCode_Short
        {
            get { return new CommonUtil().ConvertCustCode(this.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT); }
        }
    }
}



