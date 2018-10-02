using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for paymentmethod type
    /// </summary>
  public  class PaymentMethodType
    {
        public static string C_PAYMENT_METHOD_AUTO_TRANFER { get; private set; }
        public static string C_PAYMENT_METHOD_BANK_TRANSFER { get; private set; }
        public static string C_PAYMENT_METHOD_CREDIT_CARD { get; private set; }
        public static string C_PAYMENT_METHOD_MESSENGER { get; private set; }

    }
}
