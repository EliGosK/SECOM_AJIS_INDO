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
    /// Data object of Function for retrieving debt summary of a billing code.
    /// </summary>
    public partial class doGetBillingCodeDebtSummary
    {
 
        private string _BillingCodeShort;
        public string BillingCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertBillingClientCode(this.BillingCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _BillingCodeShort = value; }
        }

        public string UnpaidAmountString
        {
            get
            {
                return string.Format("{0}", this.UnpaidAmount.HasValue ? this.UnpaidAmount.Value.ToString("#,##0.00") : "0.00");
            }
        }
        public string UnpaidAmountUsdString
        {
            get
            {
                return string.Format("{0}", this.UnpaidAmountUsd.HasValue ? this.UnpaidAmountUsd.Value.ToString("#,##0.00") : "0.00");
            }
        }

        public string NoOfBillingDetailString
        {
            get
            {
                return string.Format("{0}", this.NoOfBillingDetail.HasValue ? this.NoOfBillingDetail.Value.ToString("#,##0") : "0");
            }
        }
 
    }
}
