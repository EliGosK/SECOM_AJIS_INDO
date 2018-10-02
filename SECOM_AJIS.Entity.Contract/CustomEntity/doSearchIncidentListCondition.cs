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
    public class doSearchIncidentListCondition
    {
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
        public string IncidentNo { get; set; }
        public string IncidentTitle { get; set; }
        public string IncidentType { get; set; }
        public string IncidentStatusHandling { get; set; }
        public string IncidentStatusComplete { get; set; }
        public string IncidentOfficeCode { get; set; }
        public string SpecfyPeriod { get; set; }
        public DateTime? SpecifyPeriodFrom { get; set; }
        public DateTime? SpecifyPeriodTo { get; set; }
        public string Registrant { get; set; }
        public string Correspondent { get; set; }
        public string ControlChief { get; set; }
        public string Chief { get; set; }
        public string Assistant { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{

}

