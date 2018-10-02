using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;


namespace SECOM_AJIS.JobScheduler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Injectors.ServiceRegister.Initial();
                Common.Util.ConstantUtil.InitialConstants();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                Application.Run(new frmJobMonitor());
            }
            catch (Exception ex)
            {
                Program.WriteErrorLog(ex);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Program.WriteErrorLog(e.Exception);
        }

        private static void WriteErrorLog(Exception ex)
        {
            if (ex == null)
                return;

            try
            {
                ErrorLogger.WriteErrorLog(DateTime.Now, ex);
            }
            catch { }
            try
            {
                CommonUtil.WriteWindowLog(EventType.C_EVENT_TYPE_ERROR, ex.ToString(), EventID.C_EVENT_ID_INTERNAL_ERROR);
            }
            catch { }
        }
    }
}
