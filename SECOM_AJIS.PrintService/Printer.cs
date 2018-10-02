using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.PrintService
{
    public class Printer
    {
        public static void PrintPDF(string strPathFilename)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = ConfigurationManager.AppSettings["PrintPDFFoxit"];
                //process.StartInfo.FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
                //process.StartInfo.Arguments = "/p " + @strPathFilename;
                process.StartInfo.Arguments = string.Format("/t \"{0}\" \"{1}\"", strPathFilename, ConfigurationManager.AppSettings["PrinterName"]); //Modify by Jutarat A. on 12072013
                //process.StartInfo.Verb = "Open";
                process.StartInfo.Verb = "Print";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                //Modify by Jutarat A. on 12072013
                //process.WaitForExit();
                int intPrintTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["PrintTimeOut"]);
                if (process.WaitForExit(intPrintTimeOut) == false) //Wait a maximum of 1 min for the process to finish
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                        process.Dispose();
                    }

                    throw new Exception("Print Timeout");
                }
                //End Modify
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
