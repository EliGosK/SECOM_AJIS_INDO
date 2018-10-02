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

    [MetadataType(typeof(dtBillingTargetData_MetaData))]
    public partial class dtBillingTargetData
    {
        [LanguageMapping]
        public string OfficeName { get; set; }

        public string CustTypeName { set; get; }

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
        public string BillingTargetCode_Short
        {
            get
            {
                return cm.ConvertBillingTargetCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string BillingClientCode_Short
        {
            get
            {
                return cm.ConvertBillingClientCode(this.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string OfficeName_Text
        {
            get
            {
                return CommonUtil.IsNullOrEmpty(this.OfficeName) ? "-" : this.OfficeName;
            }
        }

        public int? NoOfBillingBasic_Numeric
        {
            get
            {
                return CommonUtil.IsNullOrEmpty(this.NoOfBillingBasic) ? 0 : this.NoOfBillingBasic;
            }
        }
    }
}

namespace SECOM_AJIS.DataEntity.Billing.MetaData
{
    public class dtBillingTargetData_MetaData
    {
        [CustTypeMappingAttribute("CustTypeName")]
        public string CustTypeCode { get; set; }


    }
}




