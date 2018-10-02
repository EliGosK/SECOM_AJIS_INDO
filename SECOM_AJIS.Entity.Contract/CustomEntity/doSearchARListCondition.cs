using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doSearchARListCondition
    {
        public string QuotationTargetCode { get; set; }
        public string ContractTargetPurchaserName { get; set; }
        public string ContractCode { get; set; }
        public string UserCode { get; set; }
        public string ContractOfficeCode { get; set; }
        public string OperationOfficeCode { get; set; }
        public string ContractStatus { get; set; }
        public string ContractType { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGroupName { get; set; }
        public string SiteName { get; set; }
        public string ProjectName { get; set; }
        public string RequestNo { get; set; }
        public string ApproveNo { get; set; }
        public string ARTitle { get; set; }
        public string ARType { get; set; }
        public string ARStatusHandling { get; set; }
        public string ARStatusComplete { get; set; }
        public string AROfficeCode { get; set; }
        public string SpecfyPeriod { get; set; }
        public DateTime? SpecifyPeriodFrom { get; set; }
        public DateTime? SpecifyPeriodTo { get; set; }
        public string Requester { get; set; }
        public string Approver { get; set; }
        public string Auditor { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{

}

