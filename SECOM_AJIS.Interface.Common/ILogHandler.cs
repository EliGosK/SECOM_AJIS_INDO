using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SECOM_AJIS.Common.Models;


namespace SECOM_AJIS.DataEntity.Common
{
    public interface ILogHandler
    {
        /// <summary>
        /// Get purge log data
        /// </summary>
        /// <param name="pPurgeMonthYear"></param>
        /// <returns></returns>
        List<dtTPL> GetTbt_Purgelog(DateTime? pPurgeMonthYear);
        /// <summary>
        /// Get miscellaneous data list of purge log
        /// </summary>
        /// <param name="purgeStatus"></param>
        /// <returns></returns>
        List<doMisPurge> GetMisPurge(string purgeStatus);
        /// <summary>
        /// Get transaction log year-month
        /// </summary>
        /// <returns></returns>
        List<dtMonthYear> GetLogMonthYear();
        /// <summary>
        /// To write Windows event log
        /// </summary>
        /// <param name="eEventType"></param>
        /// <param name="strMessage"></param>
        void WriteWindowLog(string eEventType, string strMessage,int EventID);
        /// <summary>
        /// To write transaction log
        /// </summary>
        /// <param name="doTrans"></param>
        /// <param name="empNo"></param>
        /// <param name="screenID"></param>
        /// <returns></returns>
        bool WriteTransactionLog(doTransactionLog doTrans, string empNo = null, string screenID = null);
        /// <summary>
        /// Delete log
        /// </summary>
        /// <param name="dtMonthYear"></param>
        /// <returns></returns>
        List<tbt_PurgeLog> DeleteLog(DateTime dtMonthYear);

        /// <summary>
        /// Insert into document list data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        int WriteDocumentDownloadLog(doDocumentDownloadLog cond);
        /// <summary>
        /// Get batch process data list
        /// </summary>
        /// <param name="pC_CONFIG_SUSPEND_FLAG"></param>
        /// <param name="pC_BATCH_STATUS"></param>
        /// <param name="pC_BATCH_LAST_RESULT"></param>
        /// <param name="pC_BATCH_STATUS_PROCESSING"></param>
        /// <param name="pC_FLAG_ON"></param>
        /// <param name="pC_FLAG_OFF"></param>
        /// <returns></returns>
        List<dtBatchProcess> GetBatchProcessDataList(string pC_CONFIG_SUSPEND_FLAG, string pC_BATCH_STATUS, string pC_BATCH_LAST_RESULT, string pC_BATCH_STATUS_PROCESSING, Nullable<bool> pC_FLAG_ON, Nullable<bool> pC_FLAG_OFF);
  
        /// <summary>
        /// Search Foxit process and write window log
        /// </summary>
        /// <param name="objectID"></param>
        void SearchFoxitProcess(string objectID); //Add by Jutarat A. on 20082013
    }
}
