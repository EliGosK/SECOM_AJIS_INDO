using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class doBillingTypeDetailList
    {
        [LanguageMapping]
        public string BillingTypeName { get; set; }

        public string BillingType
        {
            get
            {
                return CommonUtil.TextCodeName(this.BillingTypeCode, this.BillingTypeName);
            }
        }

        public string BillingTypeBLS050
        {
            get
            {
                return CommonUtil.TextCodeName(this.BillingTypeCode, this.InvoiceDescriptionBLS050);
            }
        }
        // fix to this value
        public string InvoiceDescriptionBLS050
        {
            get
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN ||
                    CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return this.InvoiceDescriptionEN;
                }else
                {
                    return this.InvoiceDescriptionLC;
                }

            }
        }
        public string InvoiceDescription
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}",
                String.IsNullOrEmpty(this.InvoiceDescriptionEN) ? "-" : this.InvoiceDescriptionEN,
                String.IsNullOrEmpty(this.InvoiceDescriptionLC) ? "-" : this.InvoiceDescriptionLC);
            }
        }
    }
}
