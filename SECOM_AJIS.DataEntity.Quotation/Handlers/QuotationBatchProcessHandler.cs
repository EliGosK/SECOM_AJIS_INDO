using System;
using System.Collections.Generic;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using System.Linq;

using System.Transactions;


namespace SECOM_AJIS.DataEntity.Quotation
{
    public class QuotationBatchProcessHandler : BizQUDataEntities
    {
        #region Batch Process

        /// <summary>
        /// Process for delete quotation data
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="BatchDate"></param>
        /// <returns></returns>
        public doBatchProcessResult QUP010_DeleteQuotationProcess(string UserId, DateTime BatchDate)
        {
            doBatchProcessResult result = new doBatchProcessResult();

            List<SECOM_AJIS.DataEntity.Quotation.dtBatchProcessResult> deleteResult = new List<SECOM_AJIS.DataEntity.Quotation.dtBatchProcessResult>();

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    deleteResult = base.DeleteQuotation(FlagType.C_FLAG_ON, FlagType.C_FLAG_OFF);

                    scope.Complete();

                }
                catch (Exception ex)
                {
                    scope.Dispose();
                }

            }


            // Clone
            if (deleteResult.Count > 0)
            {
                result.Total = (deleteResult[0].Total ?? 0);
                result.Complete = (deleteResult[0].Complete ?? 0);
                result.Failed = (deleteResult[0].Failed ?? 0);
                result.ErrorMessage = deleteResult[0].ErrorMessage;
                result.BatchUser = UserId;
                result.Result = (deleteResult[0].Failed ?? 0) == 0;
 

            }


            return result;

        }

        #endregion
    }

    #region Class process

    #region QUP010_DeleteQuotation

    /// <summary>
    /// DO for delete quotation
    /// </summary>
    public class QUP010_DeleteQuotation : IBatchProcess
    {
        /// <summary>
        /// Working process
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="BatchDate"></param>
        /// <returns></returns>
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            QuotationBatchProcessHandler batch = new QuotationBatchProcessHandler();
            return batch.QUP010_DeleteQuotationProcess(UserId, BatchDate);
        }
    }

    #endregion

    #endregion
}
