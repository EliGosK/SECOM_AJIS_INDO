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
    public partial class doGetBillingDetailOfInvoice
    {

        private string _ContractCodeShort;
        public string ContractCodeShort
        {
            get
            {

                CommonUtil c = new CommonUtil();
                return c.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _ContractCodeShort = value; }
        }

        [LanguageMapping]
        public string InvoiceDescription { set; get; }

        public string BillingAmountCurrencyTypeName { get; set; }
    }
}
