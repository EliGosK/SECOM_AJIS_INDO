using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace SECOM_AJIS.JobScheduler
{
    public static class ErrorLogger
    {
        public static void WriteErrorLog(DateTime dt, Exception ex)
        {
            try
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                Exception logEx = ex;

                while (logEx.InnerException != null)
                {
                    logEx = logEx.InnerException;
                }
                var logEventInfo = new LogEventInfo(LogLevel.Error, "databaselog", "logMessage");
                logEventInfo.Properties["ErrorDescription"] = logEx.Message;
                logEventInfo.Properties["CreateDate"] = dt;
                logEventInfo.Properties["UserID"] = "SCHEDULER";
                logEventInfo.Exception = ex;

                logger.Log(logEventInfo);
            }
            catch { }
        }
    }
}
