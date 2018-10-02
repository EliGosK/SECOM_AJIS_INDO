using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for incident permission type
    /// </summary>
    public class IncidentPermissionType
    {
        public static string C_INCIDENT_PERMISSION_TYPE_GENERAL { get; private set; }
        public static string C_INCIDENT_PERMISSION_TYPE_CONTROL_CHIEF { get; private set; }
        public static string C_INCIDENT_PERMISSION_TYPE_CHIEF { get; private set; }
        public static string C_INCIDENT_PERMISSION_TYPE_CORRESPONDENT { get; private set; }
        public static string C_INCIDENT_PERMISSION_TYPE_ASSISTANT { get; private set; }

        public static string C_INCIDENT_PERMISSION_TYPE_BELONG_SAME_OFFICE { get; private set; }
        public static string C_INCIDENT_PERMISSION_TYPE_BELONG_SAME_BRANCH { get; private set; }
        public static string C_INCIDENT_PERMISSION_TYPE_BELONG_HQ { get; private set; }

        public static string C_INCIDENT_PERMISSION_TYPE_OFFICE_CHIEF { get; private set; }
        public static string C_INCIDENT_PERMISSION_TYPE_RELATED_OFFICE { get; private set; }
        public static string C_INCIDENT_PERMISSION_TYPE_PROJECT_MANAGER { get; private set; }
        public static string C_INCIDENT_PERMISSION_TYPE_EXECUTIVE { get; private set; }
    }
}
