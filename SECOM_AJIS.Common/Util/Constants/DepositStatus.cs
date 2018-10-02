using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for deposit status
    /// </summary>
    public class DepositStatus
    {
        public static string C_DEPOSIT_STATUS_ISSUE_INVOICE { get; private set; }
        public static string C_DEPOSIT_STATUS_PAYMENT { get; private set; }
        public static string C_DEPOSIT_STATUS_RETURN { get; private set; }
        public static string C_DEPOSIT_STATUS_SLIDE { get; private set; }
        public static string C_DEPOSIT_STATUS_REVENUE { get; private set; }
        
        public static string C_DEPOSIT_STATUS_CANCEL_PAYMENT { get; private set; }
        public static string C_DEPOSIT_STATUS_CANCEL_SLIDE { get; private set; }
        
        public static string C_DEPOSIT_STATUS_CANCEL { get; private set; }

        public static string C_DEPOSIT_STATUS_CANCEL_REFUND { get; private set; }

    }
}
