using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for send to billing timing
    /// </summary>
    public class SendToBillingTiming
    {
        public static int C_CONTRACT_TIMING_APPROVE { get; private set; }
        public static int C_CONTRACT_TIMING_BEFORE_NEW_INSTALL { get; private set; }
        public static int C_CONTRACT_TIMING_BEFORE_START { get; private set; }
        public static int C_CONTRACT_TIMING_START { get; private set; }
        public static int C_CONTRACT_TIMING_AFTER_START_CHANGE_PLAN { get; private set; }
        public static int C_CONTRACT_TIMING_AFTER_START_CHANGE_DURING_STOP { get; private set; }
        public static int C_CONTRACT_TIMING_AFTER_START_CHANGE_FEE { get; private set; }

    }
}
