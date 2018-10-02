using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of advance receipt status
    /// </summary>
    public class AdvanceReceiptStatus
    {
        public static string C_INC_ADVANCE_RECEIPT_STATUS_NOT { get; private set; }
        public static string C_INC_ADVANCE_RECEIPT_STATUS_ISSUED { get; private set; }
        public static string C_INC_ADVANCE_RECEIPT_STATUS_REGISTERED { get; private set; }
        public static string C_INC_ADVANCE_RECEIPT_STATUS_PAID { get; private set; }
    }
}