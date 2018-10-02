using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
namespace SECOM_AJIS.DataEntity.Billing
{
    /// <summary>
    /// Do Of credit card
    /// </summary>
  public  class doCreditCard
    {
        public string ContractCode { get; set; }
        public string BillingOCC { get; set; }
        public string BillingClientCode { get; set; }
        public string BillingClientNameEN { get; set; }
        public string BillingClientNameLC { get; set; }
        public string CreditCardCompanyCode { get; set; }
        public string CreditCardCompanyName { get; set; }
        public string CreditCardType { get; set; }
        public string CreditCardTypeName { get; set; }
        public string CreditCardNo { get; set; }
        public string CardName { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }   
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
