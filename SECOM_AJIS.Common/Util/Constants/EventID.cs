using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for event type
    /// </summary>
    public class EventID
    { 
        public static int C_EVENT_ID_INTERNAL_ERROR { get; private set; }
        public static int C_EVENT_ID_LOGIN_FAIL { get; private set; }
        public static int C_EVENT_ID_BATCH_START { get; private set; }
        public static int C_EVENT_ID_BATCH_FINISH { get; private set; }
        public static int C_EVENT_ID_BATCH_ERROR { get; private set; }
        public static int C_EVENT_ID_REPORT_TEMPLATE_NOT_FOUND { get; private set; }
        public static int C_EVENT_ID_SUSPEND_RESUME { get; private set; }
        public static int C_EVENT_ID_REPORT_PURGE_LOG { get; private set; }
        public static int C_EVENT_ID_NOTIFY_EMAIL_ERROR { get; private set; }
        

    }
}
