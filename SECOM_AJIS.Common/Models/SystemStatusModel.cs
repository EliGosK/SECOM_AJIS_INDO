using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util
{
    /// <summary>
    /// DO for system status
    /// </summary>
    [Serializable]
    public static class SystemStatus
    {
        public static bool SuspendFlag;
        public static DateTime SuspendDateTime;
        public static DateTime ResumeDateTime;
    }
}
