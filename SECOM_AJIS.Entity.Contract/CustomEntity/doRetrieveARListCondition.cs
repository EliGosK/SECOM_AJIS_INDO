using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doRetrieveARListCondition
    {
        public string ARRelevantType { get; set; }
        public string CustomerCode { get; set; }
        public string UserCode { get; set; }
        public string SiteCode { get; set; }
        public string ContractCode { get; set; }
        public string ProjectCode { get; set; }
        public string QuotationCode { get; set; }
        public string ARType { get; set; }
        public DateTime? DuedateDeadline { get; set; }
        public string ARStatus { get; set; }
    }
}
