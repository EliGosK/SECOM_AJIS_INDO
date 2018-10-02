using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for payment status search
    /// </summary>
    public class PaymentStatusSearch
    {
        public static string C_PAYMENT_STATUS_SEARCH_ALL { get; private set; }
        public static string C_PAYMENT_STATUS_SEARCH_ALL_MATCHED_PAYMENT { get; private set; }
        public static string C_PAYMENT_STATUS_SERACH_HAVE_UNMATCHED_PAYMENT { get; private set; }
        public static string C_PAYMENT_STATUS_SERACH_ALL_UNMATCHED_PAYMENT { get; private set; }
        public static string C_PAYMENT_STATUS_SERACH_PARTIAL_UNMATCHED_PAYMENT { get; private set; }
    }
}
