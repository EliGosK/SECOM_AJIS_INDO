using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for payment method
    /// </summary>
    public class PaymentMethod
    {
        public static string C_PAYMENT_METHOD_BANK_TRANSFER { get; private set; }
        public static string C_PAYMENT_METHOD_AUTO_TRANSFER { get; private set; }
        public static string C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER { get; private set; }
        public static string C_PAYMENT_METHOD_MESSENGER_TRANSFER { get; private set; }
    }
}
