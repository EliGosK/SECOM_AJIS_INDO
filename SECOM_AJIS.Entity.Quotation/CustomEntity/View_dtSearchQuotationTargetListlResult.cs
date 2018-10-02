using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class View_dtSearchQuotationTargetListlResult : dtSearchQuotationTargetListResult
    {
        public  string QuotationTargetCode_short
        {
            get
            {
                if (!CommonUtil.IsNullOrEmpty(base.QuotationTargetCode))
                {
                    return new CommonUtil().ConvertQuotationTargetCode(base.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                }
                return base.QuotationTargetCode;
            
            }
           
        }
        public string ContPurData
        {
            get
            {
                return "(1) " + (CommonUtil.IsNullOrEmpty(this.CustCode) ? "-" : new CommonUtil().ConvertCustCode(this.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT)) +
                    "<br>(2) " + (CommonUtil.IsNullOrEmpty(this.CustFullNameEN) ? "-" : this.CustFullNameEN) +
                    "<br>(3) " + (CommonUtil.IsNullOrEmpty(this.CustFullNameLC) ? "-" : this.CustFullNameLC);

            }

        }

        public string OfficeData
        {
            get
            {
                return "(1) " + (CommonUtil.IsNullOrEmpty(this.QuotationOfficeName) ? "-" : this.QuotationOfficeName) +
                            "<br>(2) " + (CommonUtil.IsNullOrEmpty(this.OperationOfficeName) ? "-" : this.OperationOfficeName);
            }
        }

        public string SiteData
        {
            get
            {
                return "(1) " + (CommonUtil.IsNullOrEmpty(this.SiteCode) ? "-" : new CommonUtil().ConvertSiteCode(this.SiteCode, CommonUtil.CONVERT_TYPE.TO_SHORT)) +
                      "<br>(2) " + (CommonUtil.IsNullOrEmpty(this.SiteNameEN) ? "-" : this.SiteNameEN) +
                    "<br>(3) " + (CommonUtil.IsNullOrEmpty(this.SiteNameLC) ? "-" : this.SiteNameLC);
            }
        }

    }
}
