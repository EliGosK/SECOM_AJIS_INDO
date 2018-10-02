using CSI.WindsorHelper;
using Quartz;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.BatchService
{
    public delegate void QueueStatusUpdate(tbs_BatchQueue queue);

    public class SequentialProcessJob : IJob
    {
        public static JobDetail CreateJobDetail(tbs_BatchQueue queue, QueueStatusUpdate callback)
        {
            JobDetail job = new JobDetail(queue.RunId.ToString(), null, typeof(SequentialProcessJob));
            job.JobDataMap["BatchQueue"] = queue;
            job.JobDataMap["QueueStatusUpdateCallBack"] = callback;
            job.Durable = false;
            return job;
        }

        public void Execute(JobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            tbs_BatchQueue batchQueue = dataMap.Get("BatchQueue") as tbs_BatchQueue;
            QueueStatusUpdate queueUpdateCallBack = dataMap.Get("QueueStatusUpdateCallBack") as QueueStatusUpdate;
            if (batchQueue == null)
            {
                return;
            }

            ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ILogHandler logHandler = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            IBatchProcessHandler batchhandler = ServiceContainer.GetService<IBatchProcessHandler>() as IBatchProcessHandler;
            List<string> message = new List<string>();

            batchQueue.Status = "R";
            batchQueue.StartTime = DateTime.Now;
            batchQueue.LastUpdate = DateTime.Now;
            batchQueue.Remark = null;

            commonHandler.UpdateTbs_BatchQueue(CommonUtil.ConvertToXml_Store<tbs_BatchQueue>(new List<tbs_BatchQueue>() { batchQueue }));
            if (queueUpdateCallBack != null)
            {
                queueUpdateCallBack(batchQueue);
            }

            var callback = new BatchCallBackDel(batchhandler.BatchUpdateStatus);

            try
            {
                foreach (string tmpBatchCode in batchQueue.BatchCode.Split(','))
                {
                    var batchCode = tmpBatchCode.Trim();

                    var lstAllBatches = logHandler.GetBatchProcessDataList(
                        ConfigName.C_CONFIG_SUSPEND_FLAG,
                        MiscType.C_BATCH_STATUS,
                        MiscType.C_BATCH_LAST_RESULT,
                        BatchStatus.C_BATCH_STATUS_PROCESSING,
                        FlagType.C_FLAG_ON,
                        // FlagType.C_FLAG_OFF
                        FlagType.C_FLAG_ON
                    );

                    var batchInfo = lstAllBatches.Where(b => b.BatchCode == batchCode).FirstOrDefault();

                    if (batchInfo == null)
                    {
                        message.Add(string.Format("BatchCode: {0} | Action: Skipped | Remark: Missing from sp_CM_GetBatchProcessDataList", batchCode));
                        continue;
                    }

                    if (!(batchInfo.EnableRun ?? false))
                    {
                        message.Add(string.Format("BatchCode: {0} | Action: Skipped | Remark: EnableRun = False", batchCode));
                        continue;
                    }

                    batchQueue.LastUpdate = DateTime.Now;
                    batchQueue.Remark = "Running BatchCode : " + batchCode;

                    commonHandler.UpdateTbs_BatchQueue(CommonUtil.ConvertToXml_Store<tbs_BatchQueue>(new List<tbs_BatchQueue>() { batchQueue }));
                    if (queueUpdateCallBack != null)
                    {
                        queueUpdateCallBack(batchQueue);
                    }

                    message.Add(string.Format("BatchCode: {0} | Action: Start | Remark: " + DateTime.Now.ToString(), batchCode));

                    this.RunProcess(batchInfo, "INITIAL", DateTime.Now, callback);

                    message.Add(string.Format("BatchCode: {0} | Action: Finished | Remark: " + DateTime.Now.ToString(), batchCode));
                }

                batchQueue.Status = "C";
                batchQueue.EndTime = DateTime.Now;
                batchQueue.LastUpdate = DateTime.Now;
                batchQueue.Remark = string.Join("\r\n", message);

                commonHandler.UpdateTbs_BatchQueue(CommonUtil.ConvertToXml_Store<tbs_BatchQueue>(new List<tbs_BatchQueue>() { batchQueue }));
                if (queueUpdateCallBack != null)
                {
                    queueUpdateCallBack(batchQueue);
                }
            }
            catch (Exception ex)
            {
                message.Add("ERROR: " + ex.ToString());

                batchQueue.Status = "E";
                batchQueue.EndTime = DateTime.Now;
                batchQueue.Remark = string.Join("\n", message);

                commonHandler.UpdateTbs_BatchQueue(CommonUtil.ConvertToXml_Store<tbs_BatchQueue>(new List<tbs_BatchQueue>() { batchQueue }));
                if (queueUpdateCallBack != null)
                {
                    queueUpdateCallBack(batchQueue);
                }
            }
        }

        public void RunProcess(dtBatchProcess batchProcess, string batchUser, DateTime batchDate, BatchCallBackDel callback)
        {
            doBatchProcessResult result = new doBatchProcessResult();
            try
            {
                if (callback != null)
                {
                    result.BatchName = batchProcess.BatchName;
                    result.BatchCode = batchProcess.BatchCode;
                    result.BatchStatus = BatchStatus.C_BATCH_STATUS_PROCESSING;
                    result.BatchUser = batchUser;
                    result.BatchDate = batchDate;
                    callback(result);
                }

                string[] batchJobName = batchProcess.BatchJobName.Split(',');
                string assemblyName = batchJobName[0];
                //-----2016.09.26 modify tanaka start
                assemblyName = assemblyName.Replace("_TEST", "");
                //-----2016.09.26 modify tanaka end
                string typeName = assemblyName + '.' + batchJobName[1];

                Assembly assembly = Assembly.Load(assemblyName + ", Version=1.0.0.0, PublicKeyToken=null,Culture=neutral");
                Type type = assembly.GetType(typeName);
                IBatchProcess process = (IBatchProcess)Activator.CreateInstance(type);
                result = process.WorkProcess(batchUser, batchDate);

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
                    result.BatchUser = batchUser;
                result.BatchJobName = batchProcess.BatchCode;
                result.BatchName = batchProcess.BatchName;
                result.BatchCode = batchProcess.BatchCode;
                result.BatchDate = batchDate;
                if (callback != null)
                {
                    callback(result);
                }
            }
        }

    }
}
