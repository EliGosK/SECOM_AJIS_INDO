using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Quotation.MetaData;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Quotation
{

    [MetadataType(typeof(View_dtSearchQuotationListResult_MetaData))]
    public class View_dtSearchQuotationListResult : dtSearchQuotationListResult
    {
        public string ContTarPurData_View
        {
            get
            {
                return "(1) " + (CommonUtil.IsNullOrEmpty(this.CustCode) ? "-" : new CommonUtil().ConvertCustCode(this.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT)) +
                                  "<br>(2) " + (CommonUtil.IsNullOrEmpty(this.CustFullNameEN) ? "-" : this.CustFullNameEN) +
                                  "<br>(3) " + (CommonUtil.IsNullOrEmpty(this.CustFullNameLC) ? "-" : this.CustFullNameLC);
            }
        }
        public string SiteData_View
        {
            get
            {
                return "(1) " + (CommonUtil.IsNullOrEmpty(this.SiteCode) ? "-" : new CommonUtil().ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT)) +
                    "<br>(2) " + (CommonUtil.IsNullOrEmpty(this.SiteNameEN) ? "-" : this.SiteNameEN) +
                    "<br>(3) " + (CommonUtil.IsNullOrEmpty(this.SiteNameLC) ? "-" : this.SiteNameLC);
            }
        }
        public string Office_View
        {
            get
            {
                return "(1) " + (CommonUtil.IsNullOrEmpty(this.QuotationOfficeName) ? "-" : this.QuotationOfficeName) +
                       "<br>(2) " + (CommonUtil.IsNullOrEmpty(this.OperationOfficeName) ? "-" : this.OperationOfficeName);
            }
        }
        public string Quotation_View
        {
            get
            {
                return (new CommonUtil().ConvertQuotationTargetCode(this.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT)) + "-" + this.Alphabet;
            }
        }

        public string Lock_View { get; set; }

        public string QuotationCode_Short { get { return (new CommonUtil().ConvertQuotationTargetCode(this.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT)); } }
    }
}
namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    public class View_dtSearchQuotationListResult_MetaData
    {
        [LockStatusTypeMapping("Lock_View")]
        public string LockStatus { get; set; }
        [LanguageMapping]
        public string QuotationOfficeName { get; set; }
        [LanguageMapping]
        public string OperationOfficeName { get; set; }
        [LanguageMapping]
        public string EmpFullName { get; set; }
    }

}
