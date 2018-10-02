using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for payment matching status
    /// </summary>
    public class PaymentMatchingStatus
    {
        public static string C_PAYMENT_MATCHING_FULL { get; private set; }
        public static string C_PAYMENT_MATCHING_PARTIAL { get; private set; }
        public static string C_PAYMENT_MATCHING_PARTIAL_TO_FULL { get; private set; }
    }
}
