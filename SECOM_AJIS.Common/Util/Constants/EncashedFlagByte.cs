using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of encash flag as byte
    /// </summary>
    public class EncashedFlagByte
    {

        public static byte C_PAYMENT_ENCASHED_FLAG_DEFAULT
        {
            get
            {
                return byte.Parse(EncashedFlag.C_PAYMENT_ENCASHED_FLAG_DEFAULT);
            }
        }

        public static byte C_PAYMENT_ENCASHED_FLAG_ENCASHED
        {
            get
            {
                return byte.Parse(EncashedFlag.C_PAYMENT_ENCASHED_FLAG_ENCASHED);
            }
        }

        public static byte C_PAYMENT_ENCASHED_FLAG_RETURNED
        {
            get
            {
                return byte.Parse(EncashedFlag.C_PAYMENT_ENCASHED_FLAG_RETURNED);
            }
        }
    }
}
