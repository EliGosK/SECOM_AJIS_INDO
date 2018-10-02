using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for payment status
    /// </summary>
    public class PaymentStatus
    {
        public static string C_PAYMENT_STATUS_DETAIL_BANK_COLLECT { get; private set; }
        public static string C_PAYMENT_STATUS_INV_BANK_COLLECT { get; private set; }
        public static string C_PAYMENT_STATUS_GEN_AUTO_CREDIT { get; private set; }
        public static string C_PAYMENT_STATUS_INV_AUTO_CREDIT { get; private set; }
        public static string C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT { get; private set; }
        public static string C_PAYMENT_STATUS_BANK_PAID { get; private set; }
        public static string C_PAYMENT_STATUS_AUTO_PAID { get; private set; }
        public static string C_PAYMENT_STATUS_COUNTER_BAL { get; private set; }
        public static string C_PAYMENT_STATUS_CANCEL { get; private set; }
        public static string C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK { get; private set; }
        public static string C_PAYMENT_STATUS_CASH_PAID { get; private set; }
        public static string C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED { get; private set; }
        public static string C_PAYMENT_STATUS_BILLING_EXEMPTION { get; private set; }
        public static string C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK { get; private set; }
        public static string C_PAYMENT_STATUS_CHEQUE_PAID { get; private set; }
        public static string C_PAYMENT_STATUS_NOTE_MATCHED { get; private set; }
        public static string C_PAYMENT_STATUS_NOTE_MATCHED_RETURN { get; set; }
        public static string C_PAYMENT_STATUS_NOTE_FAIL { get; private set; }
        public static string C_PAYMENT_STATUS_NOTE_ENCASHED { get; private set; }
        public static string C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK { get; private set; }
        public static string C_PAYMENT_STATUS_CASHIER_PAID { get; private set; }
        public static string C_PAYMENT_STATUS_POST_MATCHED { get; private set; }
        public static string C_PAYMENT_STATUS_POST_MATCHED_RETURN { get; set; }
        public static string C_PAYMENT_STATUS_POST_FAIL { get; private set; }
        public static string C_PAYMENT_STATUS_POST_ENCASHED { get; private set; }
        public static string C_PAYMENT_STATUS_AUTO_FAIL_BANK_PAID { get; private set; }
        public static string C_PAYMENT_STATUS_NOTE_FAIL_BANK_PAID { get; private set; }
        public static string C_PAYMENT_STATUS_POST_FAIL_BANK_PAID { get; private set; }
        public static string C_PAYMENT_STATUS_PARTIALLY_PAID { get; private set; }
        public static string C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN { get; private set; }
        public static string C_PAYMENT_STATUS_PARTIALLY_PAID_CN { get; private set; }
        public static string C_PAYMENT_STATUS_PAID_WITH_PARTIAL_REFUND { get; private set; }
        public static string C_PAYMENT_STATUS_REFUND_PAID { get; private set; }

    }
}
