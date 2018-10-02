using CSI.WindsorHelper;
using Quartz;
using Quartz.Impl;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SECOM_AJIS.BatchService
{
    public class JobHandler: IJobListener
    {
        private IScheduler _scheduler = null;
        private List<tbs_BatchQueue> _lstBatchQueue = new List<tbs_BatchQueue>();

        public bool IsFinished = false;

        public JobHandler()
        {
            JobScheduler.Injectors.ServiceRegister.Initial();           
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            _scheduler = schedFact.GetScheduler();
            _scheduler.AddJobListener(this);
            _scheduler.Start();
        }


        public void sendMail()
        {
            ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            doEmailProcess dtEmail = new doEmailProcess();
            dtEmail.MailFrom = "secom-sys-info@secom.co.jp";
            dtEmail.MailFromAlias = "test";
            dtEmail.MailTo = "h382785@secom.co.jp";
            dtEmail.Message = "This is test send email process from SECOM" + Environment.NewLine + Environment.NewLine
                            + "これは、テストがセコムからのメール送信処理です。" + Environment.NewLine + Environment.NewLine
                            + "Time stamp :" + DateTime.Now.ToString()
                            ;

            dtEmail.Subject = "SECOM-AJIS Test send mail";

            handler.SendMail(dtEmail);
        }

        public void Execute(DateTime nextRunFrom, DateTime nextRunTo)
        {
            ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            ErrorLogger.WriteErrorLog(DateTime.Now, new ApplicationException(string.Format("Executing jobs for the time between {0} and {1}", nextRunFrom.ToString("yyyy/MM/dd HH:mm:ss"), nextRunTo.ToString("yyyy/MM/dd HH:mm:ss"))));
            //sendMail();
                 
            //処理対象データがあれば処理を実行      
            try
            {                
                LoadBatchQueue(nextRunFrom, nextRunTo);

                while (!IsFinished)
                {
                   
                    var remainings = commonHandler.GetTbs_BatchQueue(null, nextRunFrom, nextRunTo).Where(q => q.EndTime == null).ToList();

                    ErrorLogger.WriteErrorLog(DateTime.Now, new ApplicationException("残り件数：" + remainings.Count()));

                    ErrorLogger.WriteErrorLog(DateTime.Now, new ApplicationException(remainings.Count.ToString()));
                    if (remainings.Count() > 0) {
                        Thread.Sleep(10000);
                    } else
                    {
                        IsFinished = true;
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


            // 処理対象データの生成（常時１日後のデータを作成する）
            DateTime nextSettingFrom = nextRunFrom.AddDays(1);
            DateTime nextSettingTo = nextRunTo.AddDays(1);
            var settings = commonHandler.GetTbs_BatchQueue(null, nextSettingFrom, nextSettingTo).ToList();
            if (settings.Count < 1)
            {
                try
                {
                    new ApplicationDataservice().GenerateBatchQueue();
                }
                catch (Exception ex)
                {
                    ErrorLogger.WriteErrorLog(DateTime.Now, new ApplicationException("バッチキューのデータ生成に失敗しました。", ex));
                }
            }
        }



        private void LoadBatchQueue(DateTime nextRunFrom, DateTime nextRunTo)
        {
            ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            var lstAllQueue = commonHandler.GetTbs_BatchQueue(null, nextRunFrom, nextRunTo).OrderBy(q => q.NextRun).ThenBy(q => q.RunId).ToList();
            
            foreach (var queueProxy in lstAllQueue)
            {
                var queue = CommonUtil.CloneObject<tbs_BatchQueue, tbs_BatchQueue>(queueProxy);

                JobDetail jobDetail = SequentialProcessJob.CreateJobDetail(queue, new QueueStatusUpdate(this.QueueStatusUpdate));

                var scheduledQueue = _lstBatchQueue.Where(q => q.RunId == queue.RunId).FirstOrDefault();
                if (scheduledQueue != null)
                {
                    _lstBatchQueue.Remove(scheduledQueue);
                }

                if (queue.Status == "W")
                {
                    Trigger trg = new SimpleTrigger(jobDetail.Name, null, queue.NextRun.ToUniversalTime(), null, 0, TimeSpan.Zero);

                    if (_scheduler.GetJobDetail(jobDetail.Name, jobDetail.Group) != null)
                    {
                        _scheduler.DeleteJob(jobDetail.Name, jobDetail.Group);
                    }

                    _scheduler.ScheduleJob(jobDetail, trg);
                }

                _lstBatchQueue.Add(queue);
            }

            var lstCompleted = _lstBatchQueue.Where(q => q.Status == "C").ToList();
            foreach (var queue in lstCompleted)
            {
                _lstBatchQueue.Remove(queue);
            }

        }
        private void QueueStatusUpdate(tbs_BatchQueue queue)
        {
            //int idx = bsBatchQueue.IndexOf(queue);
            //if (idx > -1)
            //{
            //    bsBatchQueue.ResetItem(idx);
            //}
            
        }

        #region IJobListener
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
        #endregion
    }

}
