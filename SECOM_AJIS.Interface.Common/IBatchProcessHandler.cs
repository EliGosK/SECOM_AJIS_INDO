using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Common
{
    public interface IBatchProcessHandler
    {
        /// <summary>
        /// Run batch process in batch process list
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        List<int?> RunBatch(doBatchProcess data);

        /// <summary>
        /// Run all batch process in batch process list
        /// </summary>
        /// <param name="pEmpNo"></param>
        /// <param name="pBatchDate"></param>
        /// <returns></returns>
        int RunBatchAll(string pEmpNo, DateTime? pBatchDate);

        /// <summary>
        /// Run process in process list
        /// </summary>
        /// <param name="doBatchProcess"></param>
        void RunProcess(doBatchProcess doBatchProcess);

        /// <summary>
        /// Run all process in process list
        /// </summary>
        void RunProcessAll();

        /// <summary>
        /// Update Batch Result
        /// <returns></returns>
        List<Nullable<int>> UpdateBatchResult(string vBatchCode, string vBatchStatus, string vBatchLastResult, Nullable<int> vTotal, Nullable<int> vComplete, Nullable<int> vFailed, string vBatchUser);
        void BatchUpdateStatus(SECOM_AJIS.Common.Util.doBatchProcessResult result);
        void BatchUpdateResult(SECOM_AJIS.Common.Util.doBatchProcessResult result);
        int InsertTbt_BatchLog(Nullable<System.DateTime> pBatchDate, string pBatchCode, string pErrorMessage, Nullable<bool> pErrorFlag, string pBatchUser);
    }
}
