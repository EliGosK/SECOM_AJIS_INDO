using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Payment matching result of a payment transaction information.
    /// </summary>
    public partial class doPaymentMatchingResult
    {
        public List<doPaymentMatchingResultDetail> PaymentMatchingResultDetail { get; set; }
    }
}
