using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public partial class doSearchQuotationListCondition
    {
        public string QuotationTargetCode { get; set; }
        public string Alphabet { get; set; }
        public string ProductTypeCode { get; set; }
        public string LockStatus { get; set; }
        public string QuotationOfficeCode { get; set; }
        public string OperationOfficeCode { get; set; }
        public string ContractTargetCode { get; set; }
        public string ContractTargetName { get; set; }
        public string ContractTargetAddr { get; set; }
        public string ContractTransferStatus { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string SiteAddr { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public DateTime? QuotationDateFrom { get; set; }
        public DateTime? QuotationDateTo { get; set; }
        public string ServiceTypeCode { get; set; }
        public string TargetCodeTypeCode { get; set; }
   

    }
}
