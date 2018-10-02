using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// Debt Tracing Sub Status
    /// </summary>
    public class DebtTracingSubStatus
    {
        public static string C_DEBT_TRACE_SUBSTATUS_HQ { get; private set; }
        public static string C_DEBT_TRACE_SUBSTATUS_HQ_OUTSTANDING { get; private set; }
        public static string C_DEBT_TRACE_SUBSTATUS_HQ_PENDING_CALL { get; private set; }
        public static string C_DEBT_TRACE_SUBSTATUS_HQ_NOTDUE { get; private set; }
        public static string C_DEBT_TRACE_SUBSTATUS_HQ_PENDING_MATCH { get; private set; }
        public static string C_DEBT_TRACE_SUBSTATUS_BR { get; private set; }
        public static string C_DEBT_TRACE_SUBSTATUS_BR_OUTSTANDING { get; private set; }
        public static string C_DEBT_TRACE_SUBSTATUS_BR_PENDING_CALL { get; private set; }
        public static string C_DEBT_TRACE_SUBSTATUS_BR_NOTDUE { get; private set; }
        public static string C_DEBT_TRACE_SUBSTATUS_BR_PENDING_MATCH { get; private set; }
        public static string C_DEBT_TRACE_SUBSTATUS_LAWSUIT { get; private set; }
    }
}
