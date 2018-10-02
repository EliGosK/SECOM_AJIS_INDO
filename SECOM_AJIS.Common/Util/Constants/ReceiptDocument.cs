using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for receipt document
    /// </summary>
    public class ReceiptDocument
    {
        public static string C_RECEIPT_HEADER_EN_NORMAL { get; private set; }
        public static string C_RECEIPT_HEADER_TH_NORMAL { get; private set; }

        public static string C_RECEIPT_FIRST_FEE_EN { get; private set; }
        public static string C_RECEIPT_FIRST_FEE_TH { get; private set; }

        public static string C_RECEIPT_DOC_ORIGINAL_CUST_EN { get; private set; }
        public static string C_RECEIPT_DOC_COPY_ACC_EN { get; private set; }
        public static string C_RECEIPT_DOC_ORIGINAL_CUST_TH { get; private set; }
        public static string C_RECEIPT_DOC_COPY_ACC_TH { get; private set; }
    }
}
