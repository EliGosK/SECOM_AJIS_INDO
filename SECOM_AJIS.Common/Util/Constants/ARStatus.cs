using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of AR status
    /// </summary>
    public class ARStatus
    {
        public static string C_AR_STATUS_APPROVED { get; private set; }
        public static string C_AR_STATUS_AUDITING { get; private set; }
        public static string C_AR_STATUS_INSTRUCTED { get; private set; }
        public static string C_AR_STATUS_REJECTED { get; private set; }
        public static string C_AR_STATUS_RETURNED_REQUEST { get; private set; }
        public static string C_AR_STATUS_WAIT_FOR_APPROVAL { get; private set; }
    }
}
