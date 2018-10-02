using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doSearchContractDocCondition
    { 
        public string DocStatus { get; set; }
        public string ContractCode { get; set; }
        public string QuotationTargetCode { get; set; }
        public string ProjectCode { get; set; }
        public string OCC { get; set; }
        public string Alphabet { get; set; }
        public string ContractOfficeCode { get; set; }
        public string OperationOfficeCode { get; set; }
        public string ContractOfficeCodeAuthority { get; set; }
        public string OperationOfficeCodeAuthority { get; set; }
        public string NegotiationStaffEmpNo { get; set; }
        public string NegotiationStaffEmpName { get; set; }
        public string DocumentCode { get; set; }
    }
}
