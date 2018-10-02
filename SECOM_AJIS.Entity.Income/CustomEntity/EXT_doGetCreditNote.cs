using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Function for retrieving debt target data.
    /// </summary>
    public partial class doGetCreditNote
    {
        public string BillingCodeShort
        {
            get {
                return new CommonUtil().ConvertBillingCode(this.BillingCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
    }
}