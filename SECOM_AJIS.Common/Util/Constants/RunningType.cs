using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for running type
    /// </summary>
    public class RunningType
    {
        public static string C_RUNNING_TYPE_INVOICE { get; private set; }
        public static string C_RUNNING_TYPE_TAX_INVOICE { get; private set; }
        public static string C_RUNNING_TYPE_RECEIPT { get; private set; }
        public static string C_RUNNING_TYPE_CREDIT_NOTE { get; private set; }

        public static string C_RUNNING_TYPE_REVENUE { get; private set; }
        public static string C_RUNNING_TYPE_PAYMENT_TRANS { get; private set; }
        public static string C_RUNNING_TYPE_MATCH_PAYMENT { get; private set; }

        public static string C_RUNNING_TYPE_DEBT_TRACING_NOTICE1 { get; set; }
        public static string C_RUNNING_TYPE_DEBT_TRACING_NOTICE2 { get; set; }
    }
}
