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
    
    public partial class doBillingTargetList
    {
            
        [LanguageMapping]
        public string OfficeName { get; set; }

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

        public string BillingTargetCode_Short
        {
            get
            {
                CommonUtil cm = new CommonUtil();
                return cm.ConvertBillingTargetCode(BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string BillingTargetCodeTaxIDNo
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}",
                this.BillingTargetCode_Short,
                String.IsNullOrEmpty(this.IDNo) ? "-" : this.IDNo);
            }
        }

    }
}
