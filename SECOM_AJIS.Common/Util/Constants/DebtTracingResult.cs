using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// Debt Tracing Result
    /// </summary>
    public class DebtTracingResult
    {
        public static string C_DEBT_TRACE_RESULT_WAIT_MATCH { get; private set; }
        public static string C_DEBT_TRACE_RESULT_POSTPONE { get; private set; }
        public static string C_DEBT_TRACE_RESULT_WAIT_FOR_PAYMENT { get; private set; }
        public static string C_DEBT_TRACE_RESULT_TRANSFER_TO_BRANCH { get; private set; }
        public static string C_DEBT_TRACE_RESULT_TRANSFER_TO_HQ { get; private set; }
        public static string C_DEBT_TRACE_RESULT_LAWSUIT { get; private set; }
    }
}
