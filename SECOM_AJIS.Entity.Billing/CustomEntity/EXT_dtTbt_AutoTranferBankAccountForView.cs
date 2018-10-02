using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Billing.MetaData;

namespace SECOM_AJIS.DataEntity.Billing
{
    [MetadataType(typeof(dtTbt_AutoTransferBankAccountForView_MetaData))]
    public partial class dtTbt_AutoTransferBankAccountForView
    {

        public string BillingCode_Short
        {
            get
            {
                CommonUtil cm = new CommonUtil();
                return cm.ConvertBillingCode(this.BillingCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }

        }

        public string BillingClientCode_Short
        {
            get
            {
                CommonUtil cm = new CommonUtil();
                return cm.ConvertBillingClientCode(this.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }

        }

        public string AccountTypeName { get; set; }
        public string LastestResultName { get; set; }

        public string AccountNo_ForView
        {
            get
            {

                string str = string.Empty;

                this.AccountNo = this.AccountNo.Replace("-", "");

                // XXX-X-XXXXX-X  => (10 digits)
                if (this.AccountNo.Length == 10)
                {
                    str = string.Format("{0}-{1}-{2}-{3}", this.AccountNo.Substring(0, 3), this.AccountNo.Substring(3, 1), this.AccountNo.Substring(4, 5), this.AccountNo.Substring(9, 1));
                }
                else
                {
                    str = this.AccountNo;
                }

                return str;
            }
        }
    }
}


namespace SECOM_AJIS.DataEntity.Billing.MetaData
{
    public class dtTbt_AutoTransferBankAccountForView_MetaData
    {
        [AccountTypeMappingAttribute("AccountTypeName")]
        public string AccountType { get; set; }

        [AutoTransferResultMappingAttribute("LastestResultName")]
        public string LastestResult { get; set; }
    }
}