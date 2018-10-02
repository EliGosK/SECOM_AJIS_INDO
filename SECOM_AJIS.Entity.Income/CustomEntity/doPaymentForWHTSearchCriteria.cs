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
    /// Data object of Search criteria for searching payment transaction information for Match WHT.
    /// </summary>
    public partial class doPaymentForWHTSearchCriteria
    {
        public DateTime? PaymentDateFrom { get; set; }
        public DateTime? PaymentDateTo { get; set; }
        public string PaymentTransNo { get; set; }
        public string PayerName { get; set; }
        public string VATRegistantName { get; set; }
        public string InvoiceNo { get; set; }
        public string ContractCode { get; set; }
        public string WHTNo { get; set; }
        public string IDNo { get; set; }
    }
}


