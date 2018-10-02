using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for log message
    /// </summary>
    public class LogMessage
    {
        public static string C_LOG_NIGHT_BATCH_ERROR { get; private set; }
        public static string C_LOG_NIGHT_BATCH_FINISH { get; private set; }
        public static string C_LOG_NIGHT_BATCH_START { get; private set; }
        public static string C_LOG_REPORT_NOT_FOUND { get; private set; }
        public static string C_LOG_SYSTEM_RESUME { get; private set; }
        public static string C_LOG_SYSTEM_SUSPEND { get; private set; }

        public static string C_LOG_INVALID_USER { get; private set; }

    }
}
