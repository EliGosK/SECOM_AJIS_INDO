using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net.Mail;

using System.Transactions;

using System.Security.Cryptography;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Common
{
    public class BatchProcessHandler : BizCMDataEntities, IBatchProcessHandler
    {
        /// <summary>
        /// Process - run batch (common function)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<int?> RunBatch(doBatchProcess data)
        {
            List<int?> result = new List<int?>();
            try
            {
                //using (TransactionScope t = new TransactionScope())
                //{
                result = base.RunBatch(data.BatchCode,
                                        data.BatchName,
                                        data.BatchDescription,
                                        data.BatchLastResult,
                                        data.BatchStatus,
                                        data.Total,
                                        data.Complete,
                                        data.Failed,
                                        data.BatchJobName,
                                        data.BatchDate,
                                        data.BatchUser,
                                        BatchStatus.C_BATCH_STATUS_FAILED,
                                        BatchStatus.C_BATCH_STATUS_PROCESSING,
                                        BatchStatus.C_BATCH_STATUS_SUCCEEDED,
                                        EventType.C_EVENT_TYPE_INFORMATION,
                                        LogMessage.C_LOG_NIGHT_BATCH_ERROR);

                //t.Complete();  // commit

                //}


                return result;


            }
            catch (Exception)
            {
                throw;
            }


        }
        /// <summary>
        /// Process - execute process (common function)
        /// </summary>
        /// <param name="doBatchProcess"></param>
        public void RunProcess(doBatchProcess doBatchProcess)
        {

            try
            {

                ILogHandler handLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                handLog.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION
                                        , "Night batch of " + doBatchProcess.BatchName + " is started", EventID.C_EVENT_ID_BATCH_START);
                base.UpdateBatchResult(
                       doBatchProcess.BatchCode
                    , BatchStatus.C_BATCH_STATUS_PROCESSING
                    , null
                    , 0
                    , 0
                    , 0
                    , doBatchProcess.BatchUser);

                BatchCallBackDel callback = new BatchCallBackDel(this.BatchUpdateStatus);

                BatchProcessRunAll_Result activeBatch = CommonUtil.CloneObject<doBatchProcess, BatchProcessRunAll_Result>(doBatchProcess);
                BatchProcessUtil.RunProcess(activeBatch, callback);
            }
            catch (Exception ex)
            {
                ILogHandler handLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                handLog.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION
                                        , "Night batch of " + doBatchProcess.BatchName + " is Error", EventID.C_EVENT_ID_BATCH_ERROR);
                base.UpdateBatchResult(
                       doBatchProcess.BatchCode
                    , BatchStatus.C_BATCH_STATUS_FAILED
                    , null
                    , 0
                    , 0
                    , 0
                    , doBatchProcess.BatchUser);
                base.InsertTbt_BatchLog(doBatchProcess.BatchDate, doBatchProcess.BatchCode, ex.Message, FlagType.C_FLAG_ON, doBatchProcess.BatchUser);
            }

        }

        /// <summary>
        /// Process - update batch status
        /// </summary>
        /// <param name="result"></param>
        public void BatchUpdateStatus(SECOM_AJIS.Common.Util.doBatchProcessResult result)
        {
            base.UpdateBatchResult(
                    result.BatchCode
                    , result.BatchStatus
                    , null
                    , result.Total
                    , result.Complete
                    , result.Failed
                    , result.BatchUser);

            ILogHandler handLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            if (result.BatchStatus == BatchStatus.C_BATCH_STATUS_SUCCEEDED)
            {
                string message = "Night batch of " + result.BatchName + " is finished";
                base.InsertTbt_BatchLog(result.BatchDate, result.BatchCode, message, FlagType.C_FLAG_OFF, result.BatchUser);
                handLog.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, message, EventID.C_EVENT_ID_BATCH_FINISH);
            }

            else if (result.BatchStatus == BatchStatus.C_BATCH_STATUS_FAILED)
            {
                string message = "Night batch of " + result.BatchName + " is finished with error : \n\n" + result.ErrorMessage;
                base.InsertTbt_BatchLog(result.BatchDate, result.BatchCode, message, FlagType.C_FLAG_ON, result.BatchUser);
                handLog.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, message, EventID.C_EVENT_ID_BATCH_ERROR);
            }
            else
            {
                string message = "Night batch of " + result.BatchName + " is processing";
                base.InsertTbt_BatchLog(result.BatchDate, result.BatchCode, message, FlagType.C_FLAG_OFF, result.BatchUser);
                handLog.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, message, EventID.C_EVENT_ID_BATCH_START);
            }




        }
        /// <summary>
        /// Process - update batch result
        /// </summary>
        /// <param name="result"></param>
        public void BatchUpdateResult(SECOM_AJIS.Common.Util.doBatchProcessResult result)
        {
            bool flag;
            flag = (result.BatchStatus == BatchStatus.C_BATCH_STATUS_FAILED);
            if (!string.IsNullOrEmpty(result.BatchName))
            {
                ILogHandler handLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                if (result.BatchStatus == BatchStatus.C_BATCH_STATUS_FAILED)
                    handLog.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, LogMessage.C_LOG_NIGHT_BATCH_ERROR, EventID.C_EVENT_ID_BATCH_ERROR);
                else
                    handLog.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, LogMessage.C_LOG_NIGHT_BATCH_FINISH, EventID.C_EVENT_ID_BATCH_FINISH);
            }

            if (result.BatchCode != null)
            {
                base.UpdateBatchResult(
                   result.BatchCode
                , result.BatchStatus
                , null
                , result.Total
                , result.Complete
                , result.Failed
                , result.BatchUser);

                //base.InsertTbt_BatchLog(result.BatchDate, result.BatchCode, result.BatchName, flag, result.BatchUser);

                // == add by Narupon ==

                ILogHandler handLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                if (result.BatchStatus == BatchStatus.C_BATCH_STATUS_SUCCEEDED)
                {
                    string message = "Night batch of " + result.BatchName + " is finished";
                    base.InsertTbt_BatchLog(result.BatchDate, result.BatchCode, message, FlagType.C_FLAG_OFF, result.BatchUser);
                    handLog.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, message, EventID.C_EVENT_ID_BATCH_FINISH);
                }

                else if (result.BatchStatus == BatchStatus.C_BATCH_STATUS_FAILED)
                {
                    string message = "Night batch of " + result.BatchName + " is finished with error : \n\n" + result.ErrorMessage;
                    base.InsertTbt_BatchLog(result.BatchDate, result.BatchCode, message, FlagType.C_FLAG_ON, result.BatchUser);
                    handLog.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, message, EventID.C_EVENT_ID_BATCH_ERROR);
                }
                else
                {
                    string message = "Night batch of " + result.BatchName + " is processing";
                    base.InsertTbt_BatchLog(result.BatchDate, result.BatchCode, message, FlagType.C_FLAG_OFF, result.BatchUser);
                    handLog.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, message, EventID.C_EVENT_ID_BATCH_START);
                }

                // == (end) add by Narupon ==
            }
            else
            {
                base.UpdateBatchResult(
                   null
                , BatchStatus.C_BATCH_STATUS_FAILED
                , null
                , -1
                , -1
                , -1
                , result.BatchUser);
            }

        }
        /// <summary>
        /// Process - execute all batch process
        /// </summary>
        public void RunProcessAll()
        {
            ILogHandler handLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            handLog.WriteWindowLog(EventType.C_EVENT_TYPE_INFORMATION, LogMessage.C_LOG_NIGHT_BATCH_START, EventID.C_EVENT_ID_BATCH_START);

            List<GetBatchProcessRunAll_Result> list = new List<GetBatchProcessRunAll_Result>();
            List<BatchProcessRunAll_Result> jobList = new List<BatchProcessRunAll_Result>();
            list = base.GetBatchProcessRunAll();
            DateTime? batchDate = DateTime.Now;
            foreach (GetBatchProcessRunAll_Result p in list)
            {
                BatchProcessRunAll_Result r = CommonUtil.CloneObject<GetBatchProcessRunAll_Result, BatchProcessRunAll_Result>(p);
                r.BatchUser = CommonUtil.dsTransData.dtUserData.EmpNo;
                r.BatchDate = batchDate;
                jobList.Add(r);

                // Narupon 10-Sep-2012

                base.UpdateBatchResult(
                   p.BatchCode
                   , BatchStatus.C_BATCH_STATUS_BE_PROCESSED
                   , null
                   , 0
                   , 0
                   , 0
                   , CommonUtil.dsTransData.dtUserData.EmpNo);
            }
           
            BatchWriteLogDel callback = new BatchWriteLogDel(this.BatchUpdateResult);
            BatchProcessUtil.RunBatchAll(jobList, callback, DateTime.Now);
        }
    }
}
