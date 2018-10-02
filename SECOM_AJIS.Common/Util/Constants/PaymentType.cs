using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for payment type
    /// </summary>
    public class PaymentType
    {
        public static string C_PAYMENT_TYPE_BANK_TRANSFER { get; private set; }
        public static string C_PAYMENT_TYPE_CASH { get; private set; }
        public static string C_PAYMENT_TYPE_CHEQUE_NORMAL { get; private set; }
        public static string C_PAYMENT_TYPE_CASHIER_CHEQUE { get; private set; }
        public static string C_PAYMENT_TYPE_PROMISSORY_NOTE { get; private set; }
        public static string C_PAYMENT_TYPE_CHEQUE_POST_DATED { get; private set; }
        public static string C_PAYMENT_TYPE_AUTO_TRANSFER { get; private set; }
        public static string C_PAYMENT_TYPE_CREDITNOTE_REFUND { get; private set; }
        public static string C_PAYMENT_TYPE_CREDITNOTE_DECREASED { get; private set; }
    }
}
