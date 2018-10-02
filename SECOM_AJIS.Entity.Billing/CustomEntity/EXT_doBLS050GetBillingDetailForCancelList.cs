using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class doBLS050GetBillingDetailForCancelList
    {
        public string BillingTypeNameString
        {
            get
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return this.BillingTypeNameEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return this.BillingTypeNameJP;
                }
                else
                {
                    return this.BillingTypeNameLC;
                }
            }
        }

        public string PaymentStatusNameString
        {
            get
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return this.PaymentStatusNameEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return this.PaymentStatusNameJP;
                }
                else
                {
                    return this.PaymentStatusNameLC;
                }
            }
        }

    }
}
