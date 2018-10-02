using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of contract status
    /// </summary>
    public class ContractStatus
    {
        public static string C_CONTRACT_STATUS_AFTER_START { get; private set; }
        public static string C_CONTRACT_STATUS_BEF_START { get; private set; }
        public static string C_CONTRACT_STATUS_CANCEL { get; private set; }
        public static string C_CONTRACT_STATUS_END { get; private set; }
        public static string C_CONTRACT_STATUS_FIXED_CANCEL { get; private set; }
        public static string C_CONTRACT_STATUS_STOPPING { get; private set; }

    }
}
