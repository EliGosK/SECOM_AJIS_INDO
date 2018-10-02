using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of billing invoice format type
    /// </summary>
    public class EncashedFlag
    {

        public static string C_PAYMENT_ENCASHED_FLAG_DEFAULT { get; private set; }
        public static string C_PAYMENT_ENCASHED_FLAG_ENCASHED { get; private set; }
        public static string C_PAYMENT_ENCASHED_FLAG_RETURNED { get; private set; }

    }
}
