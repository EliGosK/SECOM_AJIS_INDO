using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class doGetBillingCodeInfo
    {
        public string ContractCodeShortFormat
        {
            get
            {
                //return only contract code, no have occ
                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
    }
}
