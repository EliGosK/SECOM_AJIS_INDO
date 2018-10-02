using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using SECOM_AJIS.Common.Models;
using System.Reflection;
using Quartz.Impl;
using SECOM_AJIS.Common.Util.ConstantValue;


namespace SECOM_AJIS.Common.Util
{
    public delegate void BatchCallBackDel(doBatchProcessResult result);
    public delegate void BatchWriteLogDel(doBatchProcessResult result);

    /// <summary>
    /// Batch process management
    /// </summary>
    public class BatchProcessUtil
    {
        /// <summary>
        /// Method for run process
        /// </summary>
        /// <param name="doBatchProcess"></param>
        /// <param name="callback"></param>
        public static void RunProcess(BatchProcessRunAll_Result doBatchProcess, BatchCallBackDel callback = null)
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();
            try
            {
                string[] batchJobName = doBatchProcess.BatchJobName.Split(',');
                string assemblyName = batchJobName[0];
                string typeName = assemblyName + '.' + batchJobName[1];
                //JobDetail jobDetail = new JobDetail("SECOM_AJIS_" + assemblyName, null, typeof(ProcessJob));
                JobDetail jobDetail = new JobDetail(string.Join(".", doBatchProcess.BatchCode, doBatchProcess.BatchJobName), null, typeof(ProcessJob));
                jobDetail.JobDataMap["assemblyName"] = assemblyName;
                jobDetail.JobDataMap["typeName"] = typeName;
                jobDetail.JobDataMap["doBatchProcess"] = doBatchProcess;
                jobDetail.JobDataMap["Callback"] = callback;

                //Modify by Phoomsak L. 2012-11-15 Change time span from 1 to 12
                //Modify by Phoomsak L. 2012-12-19 Change repeat count from 1 to 0
                Trigger trigger2 = TriggerUtils.MakeImmediateTrigger(0, new TimeSpan(12, 0, 0));
                
                //trigger2.Name = "ExecuteImmediate";
                trigger2.Name = String.Format("ExecuteImmediate_{0}", doBatchProcess.BatchCode); //Modify by Jutarat A. on 26032013

                sched.ScheduleJob(jobDetail, trigger2);
            }
            catch (Exception)
            {
                sched.Shutdown();
                throw;
            }


        }
        /// <summary>
        /// Check job is running
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public static bool CheckJobRunning(string jobName)
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = schedFact.GetScheduler();
            JobDetail jobDetail = sched.GetJobDetail(jobName, null);
            return (jobDetail != null);
        }
        /// <summary>
        /// Run all process in list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        /// <param name="BatchDate"></param>
        public static void RunBatchAll(List<BatchProcessRunAll_Result> list, BatchWriteLogDel callback, DateTime BatchDate)
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();
            try
            {

                JobDetail jobDetail = new JobDetail("SECOM_AJIS_BatchAll", null, typeof(ProcessJobList));
                jobDetail.JobDataMap["BatchList"] = list;
                jobDetail.JobDataMap["UserId"] = list[0].BatchUser;
                jobDetail.JobDataMap["BatchDate"] = BatchDate;
                jobDetail.JobDataMap["WriteLog"] = callback;
                //Modify by Phoomsak L. 2012-11-15 Change time span from 1 to 12
                //Modify by Phoomsak L. 2012-12-19 Change repeat count from 1 to 0
                Trigger trigger2 = TriggerUtils.MakeImmediateTrigger(0, new TimeSpan(12, 0, 0));
                trigger2.Name = "ExecuteBatchImmediate";
                sched.ScheduleJob(jobDetail, trigger2);
            }
            catch (Exception)
            {
                sched.Shutdown();
                throw;
            }


        }
    }
    /// <summary>
    /// DO for process
    /// </summary>
    public class ProcessJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            doBatchProcessResult result = new doBatchProcessResult();
            BatchCallBackDel callback = (BatchCallBackDel)dataMap.Get("Callback");
            BatchProcessRunAll_Result doBatchProcess = (BatchProcessRunAll_Result)dataMap.Get("doBatchProcess");
            try
            {
                if (callback != null)
                {
                    result.BatchName = doBatchProcess.BatchName;
                    result.BatchCode = doBatchProcess.BatchCode;
                    result.BatchStatus = BatchStatus.C_BATCH_STATUS_PROCESSING;
                    result.BatchUser = doBatchProcess.BatchUser;
                    result.BatchDate = doBatchProcess.BatchDate;
                    callback(result);
                }

                string assemblyName = dataMap.GetString("assemblyName");
                string typeName = dataMap.GetString("typeName");

                Assembly assembly = Assembly.Load(assemblyName + ", Version=0.0.0.0, PublicKeyToken=null,Culture=neutral");
                Type type = assembly.GetType(typeName);
                IBatchProcess process = (IBatchProcess)Activator.CreateInstance(type);
                result = process.WorkProcess(doBatchProcess.BatchUser, (DateTime)doBatchProcess.BatchDate);

                // Added by Non A. 30/Mar/2012 : Set BatchStatus only if it's null.
                if (string.IsNullOrWhiteSpace(result.BatchStatus))
                {
                    if (result.Result == true)
                    {
                        result.BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED;
                    }
                    else
                    {
                        result.BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED;
                    }
                }

            }
            catch (Exception ex)
            {
                result.ErrorMessage = string.Format("{0} {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                result.BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED;

                throw ex;
            }
            finally
            {
                if (string.IsNullOrWhiteSpace(result.BatchUser))
                    result.BatchUser = doBatchProcess.BatchUser;
                result.BatchJobName = doBatchProcess.BatchCode;
                result.BatchName = doBatchProcess.BatchName;
                result.BatchCode = doBatchProcess.BatchCode;
                result.BatchDate = doBatchProcess.BatchDate;
                if (callback != null)
                {
                    callback(result);
                }

                bool keepScheduler = dataMap.GetBoolean("KeepScheduler");
                if (!keepScheduler)
                {
                    context.Scheduler.Shutdown();
                }
            }
        }
    }
    /// <summary>
    /// DO for process list
    /// </summary>
    public class ProcessJobList : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            List<BatchProcessRunAll_Result> list = (List<BatchProcessRunAll_Result>)dataMap.Get("BatchList");
            string UserId = dataMap.GetString("UserId");
            DateTime BatchDate = dataMap.GetDateTime("BatchDate");
            BatchWriteLogDel writeLog = (BatchWriteLogDel)dataMap.Get("WriteLog");
            try
            {

                //=== start 'run batch all' ===
                doBatchProcessResult allResult_start = new doBatchProcessResult();
                allResult_start.BatchName = "Run All Batch";
                allResult_start.BatchUser = UserId;

                allResult_start.BatchStatus = BatchStatus.C_BATCH_STATUS_PROCESSING;
                allResult_start.BatchCode = "AL";
                allResult_start.BatchJobName = "";
                allResult_start.BatchDate = DateTime.Now;

                writeLog(allResult_start);


                foreach (BatchProcessRunAll_Result s in list)
                {
                    doBatchProcessResult result = CommonUtil.CloneObject<BatchProcessRunAll_Result, doBatchProcessResult>(s);

                    try
                    {

                        string[] batchJobName = s.BatchJobName.Split(',');
                        string assemblyName = batchJobName[0];
                        string typeName = assemblyName + '.' + batchJobName[1];
                        result.BatchStatus = BatchStatus.C_BATCH_STATUS_PROCESSING;
                        writeLog(result);

                        Assembly assembly = Assembly.Load(assemblyName + ", Version=0.0.0.0, PublicKeyToken=null,Culture=neutral");
                        Type type = assembly.GetType(typeName);

                        IBatchProcess process = (IBatchProcess)Activator.CreateInstance(type);
                        result = process.WorkProcess(UserId, BatchDate);

                        // == add by Narupon ==

                        result.BatchCode = s.BatchCode;
                        result.BatchName = s.BatchName;
                        result.BatchJobName = s.BatchJobName;
                        result.BatchDate = DateTime.Now;


                        if (result.Result)
                        {
                            result.BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED;
                        }
                        else
                        {
                            result.BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED;
                        }


                        // == (end) add by Narupon ==

                        result.BatchUser = UserId;
                        writeLog(result);
                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessage += string.Format("Error: {0} {1}\n", ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
                        result.BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED;
                        writeLog(result);
                    }

                }

                //=== finish 'run batch all' ===
                doBatchProcessResult allResult_finish = new doBatchProcessResult();
                allResult_finish.BatchName = "Run All Batch";
                allResult_finish.BatchUser = UserId;

                allResult_finish.BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED;
                allResult_finish.BatchCode = "AL";
                allResult_finish.BatchJobName = "";
                allResult_finish.BatchDate = DateTime.Now;

                writeLog(allResult_finish);
            }
            catch (Exception ex)
            {
                doBatchProcessResult errorResult = new doBatchProcessResult();
                errorResult.BatchCode = null;
                errorResult.ErrorMessage = ex.Message;
                errorResult.BatchUser = UserId;
                errorResult.BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED;
                writeLog(errorResult);
                throw ex;
            }
            finally
            {
                context.Scheduler.Shutdown();
            }

        }
    }
    /// <summary>
    /// DO for receive result from process
    /// </summary>
    public class doBatchProcessResult
    {
        public bool Result { get; set; }
        public string BatchStatus { get; set; }
        public int Total { get; set; }
        public int Complete { get; set; }
        public int Failed { get; set; }
        public string ErrorMessage { get; set; }
        public string BatchUser { get; set; }
        public string BatchCode { get; set; }
        public string BatchName { get; set; }
        public string BatchJobName { get; set; }
        public DateTime? BatchDate { get; set; }
    }
    /// <summary>
    /// DO for receive result form process in case of run all process
    /// </summary>
    public class BatchProcessRunAll_Result
    {
        #region Primitive Properties

        public string BatchCode
        {
            get;
            set;
        }

        public string BatchName
        {
            get;
            set;
        }

        public string BatchJobName
        {
            get;
            set;
        }

        public int BatchOrder
        {
            get;
            set;
        }

        public string BatchStatus
        {
            get;
            set;
        }

        public string BatchUser { get; set; }
        public DateTime? BatchDate { get; set; }
        #endregion
    }
}
