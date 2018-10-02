using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of billing income document prefix
    /// </summary>
    public class BillingIncomeDocPrefix
    {
        public static string C_INVOICE_PREFIX { get; private set; }
        public static string C_TAX_INVOICE_PREFIX { get; private set; }
        public static string C_RECEIPT_PREFIX { get; private set; }
        public static string C_CREDIT_NOTE_PREFIX { get; private set; }

        public static string C_DEBT_TRACING_NOTICE1 { get; set; }
        public static string C_DEBT_TRACING_NOTICE2 { get; set; }
    }
}
