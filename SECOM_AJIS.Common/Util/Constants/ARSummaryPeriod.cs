using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of AR summary period
    /// </summary>
    public class ARSummaryPeriod
    {
        public static string C_AR_SUMMARY_PERIOD_LASTMONTH { get; private set; }
        public static string C_AR_SUMMARY_PERIOD_LASTWEEK { get; private set; }
        public static string C_AR_SUMMARY_PERIOD_THISMONTH { get; private set; }
        public static string C_AR_SUMMARY_PERIOD_THISWEEK { get; private set; }
        public static string C_AR_SUMMARY_PERIOD_TODAY { get; private set; }
    }
}
