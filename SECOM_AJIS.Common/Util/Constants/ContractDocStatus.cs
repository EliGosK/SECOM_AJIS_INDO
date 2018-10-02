using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of contract document status
    /// </summary>
    public class ContractDocStatus
    {
        public static string C_CONTRACT_DOC_STATUS_COLLECTED { get; private set; }
        public static string C_CONTRACT_DOC_STATUS_ISSUED { get; private set; }
        public static string C_CONTRACT_DOC_STATUS_NOT_ISSUED { get; private set; }
        public static string C_CONTRACT_DOC_STATUS_NOT_USED { get; private set; }
    }
}
