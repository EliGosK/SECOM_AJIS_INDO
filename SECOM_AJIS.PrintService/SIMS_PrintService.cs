using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.PrintService
{
    partial class SIMS_PrintService : ServiceBase
    {
        JobHandler handler = new JobHandler();

        DateTime nextRunfrom = DateTime.Now;
        DateTime nextRunTo;

        public SIMS_PrintService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.timer.Interval = ServiceSettings.Default.interval;
            this.timer.Enabled = true;
            this.timer.Start();
        }

        protected override void OnStop()
        {
            // TODO: サービスを停止するのに必要な終了処理を実行するコードをここに追加します。
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            nextRunTo = nextRunfrom.AddMilliseconds(this.timer.Interval);
            try
            {
                ErrorLogger.WriteErrorLog(DateTime.Now, new ApplicationException("Timer Tick!!"));
                handler.IsFinished = false;
                this.timer.Stop();
                handler.Execute(nextRunfrom, nextRunTo);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteErrorLog(DateTime.Now, new ApplicationException("Timer Tick Error!!"));
                ErrorLogger.WriteErrorLog(DateTime.Now, ex);
            }
            finally
            {
                this.timer.Start();
            }

        }
    }
}
