using CSI.WindsorHelper;
using Quartz;
using Quartz.Impl;
using Quartz.Util;
using SECOM_AJIS.Common.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.PrintService
{
    public class JobHandler : IJobListener
    {
        public bool IsFinished = false;

        public void Execute(DateTime nextRunFrom, DateTime nextRunTo)
        {
            ErrorLogger.WriteErrorLog(DateTime.Now, new ApplicationException(string.Format("Executing jobs for the time between {0} and {1}", nextRunFrom.ToString("yyyy/MM/dd HH:mm:ss"), nextRunTo.ToString("yyyy/MM/dd HH:mm:ss"))));

            try
            {
                FileInfo[] fileList = PathUtil.GetFilesFromDirectory(ConfigurationManager.AppSettings["TemporaryPath"]);
                if (fileList.Length < 1)
                {
                    return;
                }

                foreach (FileInfo file in fileList)
                {
                    if (file.Extension == ".pdf" && file.Length > 0)
                    {
                        Printer.PrintPDF(file.FullName);
                    }

                    if (!Directory.Exists(ConfigurationManager.AppSettings["OldTemporaryPath"]))
                    {
                        Directory.CreateDirectory(ConfigurationManager.AppSettings["OldTemporaryPath"]);
                    }

                    File.Copy(file.FullName, ConfigurationManager.AppSettings["OldTemporaryPath"] + file.Name);
                    int retryCount = 0;
                    while (retryCount < 3)
                    {
                        if (File.Exists(ConfigurationManager.AppSettings["OldTemporaryPath"] + file.Name))
                        {
                            file.Delete();
                            if (!File.Exists(ConfigurationManager.AppSettings["TemporaryPath"] + file.Name))
                            {
                                break;
                            }
                        }
                        else
                        {
                            File.Copy(file.FullName, ConfigurationManager.AppSettings["OldTemporaryPath"] + file.Name);
                            retryCount++;
                            if (retryCount == 3)
                            {
                                throw new Exception("failed to copy or delete file.");
                            }
                        }
                        
                    }
                }
                
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteErrorLog(DateTime.Now, ex);
            }
            finally
            {
                ErrorLogger.WriteErrorLog(DateTime.Now, new ApplicationException("Finished Executing"));
            }
        }

        void IJobListener.JobExecutionVetoed(JobExecutionContext context)
        {
            return;
        }

        void IJobListener.JobToBeExecuted(JobExecutionContext context)
        {
            return;
        }
        void IJobListener.JobWasExecuted(JobExecutionContext context, JobExecutionException jobException)
        {
            if (jobException != null)
            {
                ErrorLogger.WriteErrorLog(DateTime.Now, jobException);
            }
        }
        string IJobListener.Name
        {
            get { return "BatchService.JobHandler"; }
        }

    }
}
