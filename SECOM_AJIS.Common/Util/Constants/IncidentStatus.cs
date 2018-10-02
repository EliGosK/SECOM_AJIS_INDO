using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for incident status
    /// </summary>
    public class IncidentStatus
    {
        public static string C_INCIDENT_STATUS_NEW_REGISTER { get; private set; }
        public static string C_INCIDENT_STATUS_CONTROL_CHIEF_UNREGISTERED { get; private set; }
        public static string C_INCIDENT_STATUS_HAVE_REPLY_FROM_CHIEF { get; private set; }
        public static string C_INCIDENT_STATUS_HAVE_UNREAD_INSTRUCTION { get; private set; }
        public static string C_INCIDENT_STATUS_INCIDENT_CHIEF_RESPONDING { get; private set; }
        public static string C_INCIDENT_STATUS_REPORT { get; private set; }
        public static string C_INCIDENT_STATUS_RESPONDING { get; private set; }
        public static string C_INCIDENT_STATUS_WAIT_FOR_COMPLETE_APPROVAL { get; private set; }
        public static string C_INCIDENT_STATUS_WAIT_FOR_INSTRUCTION { get; private set; }
        public static string C_INCIDENT_STATUS_COMPLETE { get; private set; }
    }
}
