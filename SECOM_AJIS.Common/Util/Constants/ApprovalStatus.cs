using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of approval status
    /// </summary>
    public class ApprovalStatus
    {
        public static string C_APPROVE_STATUS_APPROVED { get; private set; }
        public static string C_APPROVE_STATUS_REJECTED { get; private set; }
        public static string C_APPROVE_STATUS_RETURNED { get; private set; }
        public static string C_APPROVE_STATUS_WAITFORAPPROVE { get; private set; }
    }
}
