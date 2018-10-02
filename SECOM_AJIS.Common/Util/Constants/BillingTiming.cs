using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of billing timing
    /// </summary>
    public class BillingTiming
    {
        public static string C_BILLING_TIMING_ACCEPTANCE { get; private set; }
        public static string C_BILLING_TIMING_AFTER_REGISTER { get; private set; }
        public static string C_BILLING_TIMING_APPROVE_CONTRACT { get; private set; }
        public static string C_BILLING_TIMING_COMPLETE_INSTALLATION { get; private set; }
        public static string C_BILLING_TIMING_PARTIAL { get; private set; }
        public static string C_BILLING_TIMING_START_SERVICE { get; private set; }
    }
}
