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
    /// <summary>
    /// Do Of view billing basic list
    /// </summary>
    public partial class dtViewBillingBasicList
    {       
        [LanguageMapping]
        public string OfficeName { get; set; }
      
        CommonUtil cm = new CommonUtil();
        public string Name
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}",
                String.IsNullOrEmpty(this.NameEN) ? "-" : this.NameEN,
                String.IsNullOrEmpty(this.NameLC) ? "-" : this.NameLC);
            }
        }

        public string Address
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}",
                String.IsNullOrEmpty(this.AddressEN) ? "-" : this.AddressEN,
                String.IsNullOrEmpty(this.AddressLC) ? "-" : this.AddressLC);
            }
        }
       
        public string BillingCode
        {
            get
            {
                return string.Format("{0}-{1}", ContractCode, BillingOCC);
                //return cm.ConvertBillingTargetCode(BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string BillingCode_Short
        {
            get
            {
                return string.Format("{0}-{1}", cm.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT), BillingOCC);                
            }
        }

        public string ContractCode_Short
        {
            get
            {
                return cm.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string BillingCodeTaxIDNo
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}",
                this.BillingCode_Short,
                String.IsNullOrEmpty(this.IDNo) ? "-" : this.IDNo);
            }
        }

    }
}


