using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doSearchDraftContractCondition
    {

        public string QuotationCode { get; set; }
        public string Alphabet { get; set; }
        public DateTime? RegistrationDateFrom { get; set; }
        public DateTime? RegistrationDateTo { get; set; }
        public string Salesman1Code { get; set; }
        public string Salesman1Name { get; set; }
        public string ContractTargetName { get; set; }
        public string SiteName { get; set; }
        public string ContractOfficeCode { get; set; }
        public string OperationOfficeCode { get; set; }
        public string ApproveContractStatus { get; set; }
        public DateTime? ApproveDateFrom { get; set; }
        public DateTime? ApproveDateTo { get; set; }

    }
}
