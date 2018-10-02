using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Quotation
{
   public class doSearchQuotationTargetListCondition
    {

        private string strQuotationTargetCode; public string QuotationTargetCode { get { return this.strQuotationTargetCode; } set { this.strQuotationTargetCode = value; } }
        private string strProductTypeCode; public string ProductTypeCode { get { return this.strProductTypeCode; } set { this.strProductTypeCode = value; } }
        private string strQuotationOfficeCode; public string QuotationOfficeCode { get { return this.strQuotationOfficeCode; } set { this.strQuotationOfficeCode = value; } }
        private string strOperationOfficeCode; public string OperationOfficeCode { get { return this.strOperationOfficeCode; } set { this.strOperationOfficeCode = value; } }
        private string strContractTargetCode; public string ContractTargetCode { get { return this.strContractTargetCode; } set { this.strContractTargetCode = value; } }
        private string strContractTargetName; public string ContractTargetName { get { return this.strContractTargetName; } set { this.strContractTargetName = value; } }
        private string strContractTargetAddr; public string ContractTargetAddr { get { return this.strContractTargetAddr; } set { this.strContractTargetAddr = value; } }
        private string strSiteCode; public string SiteCode { get { return this.strSiteCode; } set { this.strSiteCode = value; } }
        private string strSiteName; public string SiteName { get { return this.strSiteName; } set { this.strSiteName = value; } }
        private string strSiteAddr; public string SiteAddr { get { return this.strSiteAddr; } set { this.strSiteAddr = value; } }
        private string strStaffCode; public string StaffCode { get { return this.strStaffCode; } set { this.strStaffCode = value; } }
        private string strStaffName; public string StaffName { get { return this.strStaffName; } set { this.strStaffName = value; } }
        private DateTime? datQuotationDateFrom; public DateTime? QuotationDateFrom { get { return this.datQuotationDateFrom; } set { this.datQuotationDateFrom = value; } }
        private DateTime? datQuotationDateTo; public DateTime? QuotationDateTo { get { return this.datQuotationDateTo; } set { this.datQuotationDateTo = value; } } 
           }
}
