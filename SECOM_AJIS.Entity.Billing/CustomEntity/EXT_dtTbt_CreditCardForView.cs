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
  [MetadataType(typeof(dtTbt_CreditCardForView_MetaData))]
    public partial class dtTbt_CreditCardForView
    {
        public string ContractCode_Short
        {
            get
            {
                CommonUtil cm = new CommonUtil();
                return cm.ConvertBillingCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }

        }
        public string BillingCode { get { return string.Format("{0}-{1}" ,this.ContractCode ,this.BillingOCC); } }
        public string BillingCode_short { get { return string.Format("{0}-{1}", this.ContractCode_Short, this.BillingOCC); } }    
        public string BillingClientCode_Short
        {
            get
            {
                CommonUtil cm = new CommonUtil();
                return cm.ConvertBillingClientCode(this.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }

        }
        public string ExpireDate
        {
            get
            {
               
                return Convert.ToInt32(ExpMonth).ToString("00") + "/" + ExpYear ;
            }

        }
        public string CreditCardTypeName { set; get; }

        [LanguageMapping] 
        public string CreditCardCompanyName { set; get; }

        public string CreditCardNo_ForView
        {
            get
            {

                string str = string.Empty;

                this.CreditCardNo = this.CreditCardNo.Replace("-", "");

                // XXXX-XXXX-XXXX-XXXX  => (16 digits)
                if (this.CreditCardNo.Length == 16)
                {
                    str = string.Format("{0}-{1}-{2}-{3}", this.CreditCardNo.Substring(0, 4), this.CreditCardNo.Substring(4, 4), this.CreditCardNo.Substring(8, 4), this.CreditCardNo.Substring(12, 4));
                }
                else
                {
                    str = this.CreditCardNo;
                }

                return str;
            }
        }
    }
}


namespace SECOM_AJIS.DataEntity.Billing.MetaData
{
    public class dtTbt_CreditCardForView_MetaData
    {
        [CreditCardTypeMappingAttribute("CreditCardTypeName")]
        public string CreditCardType { get; set; }

        //[CreditCardCompanyMappingAttribute("CreditCardCompanyName")]
        //public string CreditCardCompanyCode { get; set; }
    }
}