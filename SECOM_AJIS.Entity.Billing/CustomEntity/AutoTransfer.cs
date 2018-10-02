using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
namespace SECOM_AJIS.DataEntity.Billing
{
    /// <summary>
    /// Do Of Autotransfer
    /// </summary>
  public  class AutoTransfer
    {
        public string ContractCode { get; set; }
        public string BillingOCC { get; set; }
        public string BillingClientCode { get; set; }
        public string BillingClientNameEN { get; set; }
        public string BillingClientNameLC { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BankBranchCode { get; set; }
        public string BankBranchName { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string AccountTypeName { get; set; }
        public string AutoTransferDate { get; set; }
        public string LastestResult { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }

        CommonUtil cm = new CommonUtil();
        public string BillingCode_Short
        {
            get
            {
                if (CommonUtil.IsNullOrEmpty(this.ContractCode) || CommonUtil.IsNullOrEmpty(this.BillingOCC))
                {
                    return null;
                }
                else
                {
                    return string.Format("{0}-{1}",
                    this.ContractCode, this.BillingOCC);
                }
            }
        }      
    }
}
