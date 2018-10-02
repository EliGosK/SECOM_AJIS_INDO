using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for invoice document
    /// </summary>
    public class InvoiceDocument
    {
        public static string C_INVOICE_HEADER_EN_NORMAL { get; private set; }
        public static string C_INVOICE_HEADER_EN_AUTO_TRANS { get; private set; }
        public static string C_INVOICE_HEADER_TH_NORMAL { get; private set; }
        public static string C_INVOICE_HEADER_TH_AUTO_TRANS { get; private set; }

        public static string C_INVOICE_FIRST_FEE_EN { get; private set; }
        public static string C_INVOICE_FIRST_FEE_TH { get; private set; }


        public static string C_INVOICE_DOC_ORIGINAL_CUST_EN { get; private set; }
        public static string C_INVOICE_DOC_COPY_CUST_EN { get; private set; }
        public static string C_INVOICE_DOC_COPY_ACC_EN { get; private set; }
        public static string C_INVOICE_DOC_ORIGINAL_CUST_TH { get; private set; }
        public static string C_INVOICE_DOC_COPY_CUST_TH { get; private set; }
        public static string C_INVOICE_DOC_COPY_ACC_TH { get; private set; }

        public static string C_CONTRACT_NO_LABEL_EN { get; private set; }
        public static string C_CONTRACT_NO_LABEL_TH { get; private set; }

    }
}
