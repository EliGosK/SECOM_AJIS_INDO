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
    /// Do Of billing target for view
    /// </summary>
    [MetadataType(typeof(dtTbt_BillingTargetForView_MetaData))]
    public partial class dtTbt_BillingTargetForView
    {
        [LanguageMapping]
        public string CustTypeName { get; set; }

        [LanguageMapping]
        public string BusinessTypeName { get; set; }

        [LanguageMapping]
        public string Nationality { get; set; }
        
        [LanguageMapping]
        public string CompanyTypeName { get; set; }

        [LanguageMapping]
        public string OfficeName { get; set; }

        public string IssueInvTimeName { get; set; }
        public string InvFormatTypeName { get; set; }
        public string SignatureTypeName { get; set; }
        public string DocLanguageName { get; set; }
        public string ShowDueDateName { get; set; }
        public string IssueReceiptTimingName { get; set; }
        public string ShowAccTypeName { get; set; }
        public string WhtDeductionTypeName { get; set; }
        public string ShowIssueDateName { get; set; }
        public string SeparateInvTypeName { get; set; }

        public string BillingOfficeCodeName { get { return CommonUtil.TextCodeName(this.BillingOfficeCode, OfficeName); } }

        public string FullName
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}",
                String.IsNullOrEmpty(this.FullNameEN) ? "-" : this.FullNameEN,
                String.IsNullOrEmpty(this.FullNameLC) ? "-" : this.FullNameLC);
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
    }
}


namespace SECOM_AJIS.DataEntity.Billing.MetaData
{
    public class dtTbt_BillingTargetForView_MetaData
    {
        [IssueInvTimeMappingAttribute("IssueInvTimeName")]
        public string IssueInvTime { get; set; }

        [InvFormatTypeMappingAttribute("InvFormatTypeName")]
        public string InvFormatType { get; set; }

        [SigTypeMappingAttribute("SignatureTypeName")]
        public string SignatureType { get; set; }

        [DocLanguageMappingAttribute("DocLanguageName")]
        public string DocLanguage { get; set; }

        [ShowDueDateMappingAttribute("ShowDueDateName")]
        public string ShowDueDate { get; set; }

        [IssueRecieptTimeMappingAttribute("IssueReceiptTimingName")]
        public string IssueReceiptTiming { get; set; }

        [ShowBankAccTypeMappingAttribute("ShowAccTypeName")]
        public string ShowAccType { get; set; }

        [DeductTypeMappingAttribute("WhtDeductionTypeName")]
        public string WhtDeductionType { get; set; }

        [ShowIssueDateMappingAttribute("ShowIssueDateName")]
        public string ShowIssueDate { get; set; }

        [SeparateInvTypeMappingAttribute("SeparateInvTypeName")]
        public string SeparateInvType { get; set; }
    }
}