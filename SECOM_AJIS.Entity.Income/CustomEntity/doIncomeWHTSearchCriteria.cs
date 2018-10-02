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
    /// Data object of Search criteria for searching Income WHT.
    /// </summary>
    public partial class doIncomeWHTSearchCriteria
    {
        public string WHTNo { get; set; }
        public string PaymentTransNo { get; set; }
        public string PayerName { get; set; }
        public string VATRegistantName { get; set; }
        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }
    }
}


