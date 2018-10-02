

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for validation import result
    /// </summary>
    public class ValidationImportResult
    {
        public static string C_PAYMENT_IMPORT_NO_ERROR { get; private set; }
        public static string C_PAYMENT_IMPORT_ERROR_INVALID_INVOICE { get; private set; }
        public static string C_PAYMENT_IMPORT_ERROR_IMPORTED_INVOICE { get; private set; }
        public static string C_PAYMENT_IMPORT_ERROR_PAID_INVOICE { get; private set; }
        public static string C_PAYMENT_IMPORT_ERROR_INVOICE_AMOUNT_UNMATCH { get; private set; }
        public static string C_PAYMENT_IMPORT_ERROR_INCORRECT_STATUS { get; private set; }
        public static string C_PAYMENT_IMPORT_ERROR_PAY_DATE_UNMATCH { get; private set; }
        public static string C_PAYMENT_IMPORT_ERROR_BANK_UNMATCH { get; private set; }
    }
}
