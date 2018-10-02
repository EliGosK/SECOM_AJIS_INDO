using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class doBLS050GetBillingBasic
    {

        public string ContractCodeShort
        {
            get
            {
                return new CommonUtil().ConvertBillingCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string BillingTargetCodeShort
        {
            get
            {
                return new CommonUtil().ConvertBillingTargetCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string PaymentMethodNameString
        {
            get
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return this.PaymentMethodNameEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return this.PaymentMethodNameJP;
                }
                else
                {
                    return this.PaymentMethodNameLC;
                }
            }
        }

    }
}
