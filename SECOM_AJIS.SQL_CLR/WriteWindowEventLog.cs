using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


public class WindowEventLog
{
    public bool WriteWindowEventLog(string source, string strLogType, string logDetail)
    {
        bool result = false;
        string logName = "Application";


        EventLog objLog = new EventLog();

        try
        {
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, logName);
            }

            objLog.Source = source;
            objLog.Log = logName;

            if (strLogType.Trim().ToUpper() == "ERROR")
            {
                objLog.WriteEntry(logDetail, EventLogEntryType.Error);
            }
            else if (strLogType.Trim().ToUpper() == "WARNING")
            {
                objLog.WriteEntry(logDetail, EventLogEntryType.Warning);
            }
            else
            {
                objLog.WriteEntry(logDetail, EventLogEntryType.Information);
            }

            result = true;

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return result;
    }
}



