using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Billing
{
    /// <summary>
    /// Do Of billing detail
    /// </summary>
    public partial class doBillingDetail
    {

        CommonUtil cm = new CommonUtil();

        [LanguageMapping]      
        public string InvoiceDescription { get; set; }

        public string BillingCode_Short
        {
            get
            {
                return String.Format("{0}-{1}", cm.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT), BillingOCC);                            
            }
        }

        public string BillingCode
        {
            get
            {
                return CommonUtil.TextCodeName(ContractCode, BillingOCC);
               // return String.Format("{0}-{1}", ContractCode, BillingOCC);
            }
        }

        public string BillingAmountCurrencyTypeName { get; set; }

        // Additional field (by Narupon W. Mar 20, 2012)
        //public string InvoiceNo { set; get; }
        //public string InvoiceOCC { set; get; }
        //public DateTime? IssueInvDate { set; get; }
        //public bool IssueInvFlag { set; get; }
        //public string BillingTypeGroup { set; get; }
        //public string AdjustBillingAmount { set; get; }
        //public DateTime? BillingStartDate { set; get; }
        //public DateTime? BillingEndDate { set; get; }
        //public string PaymentMethod { set; get; }
        //public string PaymentStatus { set; get; }
        //public DateTime? AutoTransferDate { set; get; }
        //public bool FirstFeeFlag { set; get; }
        //public int DelayedMonth { set; get; }
        //public DateTime? CreateDate { set; get; }
        //public string CreateBy { set; get; }
        //public DateTime? UpdateDate { set; get; }
        //public string UpdateBy { set; get; }
        //public DateTime? StartOperationDate { set; get; }


    }
}
