using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doRetrieveIncidentListCondition
    {
        public string IncidentRelevantType { get; set; }
        public string CustomerCode { get; set; }
        public string SiteCode { get; set; }
        public string ContractCode { get; set; }
        public string ProjectCode { get; set; }
        public string IncidentType { get; set; }
        public DateTime? DuedateDeadline { get; set; }
        public string IncidentStatus { get; set; }
    }
}
