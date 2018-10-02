using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of AR search period
    /// </summary>
    public class ARSearchPeriod
    {
        public static string C_AR_SEARCH_PERIOD_APPROVE_DATE { get; private set; }
        public static string C_AR_SEARCH_PERIOD_DUEDATE { get; private set; }
        public static string C_AR_SEARCH_PERIOD_REQUEST_DATE { get; private set; }
        public static string C_AR_SEARCH_PERIOD_LASTACTION_DATE { get; set; }
    }
}
