using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of batch status
    /// </summary>
    public class BatchStatus
    {
        public static string C_BATCH_STATUS_BE_PROCESSED { get; private set; }
        public static string C_BATCH_STATUS_FAILED { get; private set; }
        public static string C_BATCH_STATUS_PROCESSING { get; private set; }
        public static string C_BATCH_STATUS_SUCCEEDED { get; private set; }

    }
}
