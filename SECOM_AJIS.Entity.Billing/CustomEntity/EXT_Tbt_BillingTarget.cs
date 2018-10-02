using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Billing
{
    /// <summary>
    /// Do Of table billing target
    /// </summary>
    public partial class tbt_BillingTarget
    {
        CommonUtil comUtil = new CommonUtil();

        public string FullNameEN { get; set; }
        public string FullNameLC { get; set; }

        public virtual string BillingTargetCodeShort
        {
            get
            {
                return comUtil.ConvertBillingTargetCode(BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public virtual string BillingClientCodeShort
        {
            get
            {
                return comUtil.ConvertBillingClientCode(BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
    }
}
