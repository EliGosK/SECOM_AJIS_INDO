using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for incident received method
    /// </summary>
    public class IncidentReceivedMethod
    {
        public static string C_INCIDENT_RECEIVED_METHOD_DOCUMENT { get; private set; }
        public static string C_INCIDENT_RECEIVED_METHOD_OTHER { get; private set; }
        public static string C_INCIDENT_RECEIVED_METHOD_TELEPHONE { get; private set; }
        public static string C_INCIDENT_RECEIVED_METHOD_VISIT { get; private set; }
    }
}