using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using Quartz;
using Quartz.Impl;
using System.Configuration;

namespace SECOM_AJIS.JobScheduler
{
    public partial class frmJobMonitor : Form, IJobListener
    {

        private IScheduler _scheduler = null;

        private DateTime _lastLoadedNextRun = DateTime.Now;
        private List<tbs_BatchQueue> _lstBatchQueue = new List<tbs_BatchQueue>();

        public DateTime LastLoadedNextRun
        {
            get { return _lastLoadedNextRun; }
            set
            {
                _lastLoadedNextRun = value;
                lblLastLoad.Text = "Last Queue Loaded: " + _lastLoadedNextRun.ToString();
            }
        }

        public frmJobMonitor()
        {
            InitializeComponent();
        }

        #region Event's Handlers
        private void frmJobMonitor_Load(object sender, EventArgs e)
        {
            try
            {
                ISchedulerFactory schedFact = new StdSchedulerFactory();
                _scheduler = schedFact.GetScheduler();
                _scheduler.AddJobListener(this);
                _scheduler.Start();

                string version = ConfigurationManager.AppSettings["SecomVersion"];
                if (!string.IsNullOrEmpty(version))
                {
                    lblVersion.Text = version;
                }

                bsBatchQueue.DataSource = _lstBatchQueue;

                DateTime nextRunStart = DateTime.Now;
                DateTime nextRunEnd;
                if (nextRunStart.Minute < 55)
                {
                    nextRunEnd = nextRunStart.Date.AddHours(DateTime.Now.Hour + 1);
                }
                else
                {
                    nextRunEnd = nextRunStart.Date.AddHours(DateTime.Now.Hour + 2);
                }

                this.LoadBatchQueue(nextRunStart, nextRunEnd);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteErrorLog(DateTime.Now, ex);
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmJobMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                const string EXIT_CONFIRM_PHRASE = "Yes";
                var result = Microsoft.VisualBasic.Interaction.InputBox("All running job will be terminated!!\nDo you want to close this application?\n\nType \"" + EXIT_CONFIRM_PHRASE + "\" to close application.", "Exit Confirmation", "", -1, -1);

                if (result != EXIT_CONFIRM_PHRASE)
                {
                    e.Cancel = true;
                    return;
                }

                _scheduler.Shutdown();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteErrorLog(DateTime.Now, ex);
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timLoadBatchQueue_Tick(object sender, EventArgs e)
        {
            try
            {
                timLoadBatchQueue.Enabled = false;

                this.LoadBatchQueue(this.LastLoadedNextRun, this.LastLoadedNextRun.AddHours(1));
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteErrorLog(DateTime.Now, ex);
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

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
            get { return this.Name; }
        }
        #endregion

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

            bsBatchQueue.ResetBindings(false);

            this.LastLoadedNextRun = nextRunTo;

            timLoadBatchQueue.Interval = (int)(nextRunTo.AddMinutes(-5) - DateTime.Now).TotalMilliseconds;
            timLoadBatchQueue.Enabled = true;
        }

        private void QueueStatusUpdate(tbs_BatchQueue queue)
        {
            int idx = bsBatchQueue.IndexOf(queue);
            if (idx > -1)
            {
                bsBatchQueue.ResetItem(idx);
            }
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region No Close Button
        const int CS_NOCLOSE = 0x200;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_NOCLOSE;
                return cp;
            }
        }
        #endregion
    }
}
