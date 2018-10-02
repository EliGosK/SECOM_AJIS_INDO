using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;


public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void sp_WriteWindowsEventLog(string source, string strLogType, string logDetail)
    {
        // Put your code here

        SqlPipe sp;
        sp = SqlContext.Pipe;
        String s = "Result : ";

        bool result = false;

        WindowEventLog log = new WindowEventLog();

        try
        {
            result = log.WriteWindowEventLog(source, strLogType, logDetail);
        }
        catch (Exception ex)
        {
            s += ex.Message;
        }
        

        if (result)
        {
            s += "OK";
        }
        
       
        sp.Send(s);
    }
};
