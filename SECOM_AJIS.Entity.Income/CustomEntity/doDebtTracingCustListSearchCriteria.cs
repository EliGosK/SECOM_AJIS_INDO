using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Search criteria for retrive cust list on Debt Tracing
    /// </summary>
    public partial class doDebtTracingCustListSearchCriteria
    {
        public string BillingOfficeCode { get; set; }
        public string ContractCode { get; set; }
        public string BillingClientName { get; set; }
        public string InvoiceNo { get; set; }
        public bool? FirstFeeFlag { get; set; }
        public bool? ShowLawsuit { get; set; }
        public bool? ShowBranch { get; set; }
        public bool? ShowNotDue { get; set; }
        public bool? ShowPending { get; set; }
        public bool? ShowOutstanding { get; set; }
    }
}


