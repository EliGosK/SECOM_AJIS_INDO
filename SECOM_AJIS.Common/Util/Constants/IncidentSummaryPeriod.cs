using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for incident summary period
    /// </summary>
    public class IncidentSummaryPeriod
    {
        public static string C_INCIDENT_SUMMARY_PERIOD_LASTMONTH { get; private set; }
        public static string C_INCIDENT_SUMMARY_PERIOD_LASTWEEK { get; private set; }
        public static string C_INCIDENT_SUMMARY_PERIOD_THISMONTH { get; private set; }
        public static string C_INCIDENT_SUMMARY_PERIOD_THISWEEK { get; private set; }
        public static string C_INCIDENT_SUMMARY_PERIOD_TODAY { get; private set; }
    }
}
