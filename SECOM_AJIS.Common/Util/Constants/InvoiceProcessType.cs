using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for invoice process type
    /// </summary>
    public class InvoiceProcessType
    {
        public static string C_INV_PROCESS_TYPE_SEPARATE { get; private set; }
        public static string C_INV_PROCESS_TYPE_COMBINE { get; private set; }
        public static string C_INV_PROCESS_TYPE_ISSUE_SALE { get; private set; }
    }
}
