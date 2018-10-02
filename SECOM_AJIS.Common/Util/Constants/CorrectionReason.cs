using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of correction reason
    /// </summary>
    public class CorrectionReason
    {
        public static string C_CORRECTION_REASON_MISTAKE { get; private set; }
        public static string C_CORRECTION_REASON_DISHONOR { get; private set; }
        public static string C_CORRECTION_REASON_ENCASH_MISTAKE { get; private set; }
    }
}
