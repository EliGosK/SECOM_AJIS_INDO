using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;

using System.Transactions;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class ContractBatchProcessHandler : BizCTDataEntities
    {
        public doBatchProcessResult BatchAutoRenewProcess(string UserId, DateTime BatchDate)
        {
            //BatchDate = Convert.ToDateTime("2555-02-04");
            doBatchProcessResult result = new doBatchProcessResult();
            RentralContractHandler rcHandler = new RentralContractHandler();
            MaintenanceHandler mHandler = new MaintenanceHandler();
            List<doContractAutoRenew> doContractAutoRenewList = rcHandler.GetContractExpireNextMonth(BatchDate);

            // Initial value of doBatchProcessResult
            result.Result = FlagType.C_FLAG_ON;
            result.BatchStatus = null;
            result.Total = doContractAutoRenewList.Count;
            result.Complete = 0;
            result.Failed = 0;
            result.ErrorMessage = string.Empty;
            result.BatchUser = UserId;

            //DateTime dtEndDateOfNextMonth = (new DateTime(BatchDate.Year, BatchDate.Month + 2, 1)).AddDays(-1); //Add by Jutarat A. on 09052013
            DateTime dtEndDateOfNextMonth = (new DateTime(BatchDate.Year, BatchDate.Month, 1)).AddMonths(2).AddDays(-1); //Add by Jutarat A. on 09052013

            foreach (var doContractAutoRenew in doContractAutoRenewList)
            {             

                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        //================= Update implement data in rental security basic table ==============
                        DateTime tmpDate = Convert.ToDateTime(doContractAutoRenew.CalContractEndDate).AddMonths(Convert.ToInt32(doContractAutoRenew.AutoRenewMonth));

                        //Add by Jutarat A. on 09052013
                        if (doContractAutoRenew.AutoRenewMonth != null && doContractAutoRenew.AutoRenewMonth > 0)
                        {
                            while (tmpDate <= dtEndDateOfNextMonth)
                            {
                                tmpDate = tmpDate.AddMonths(Convert.ToInt32(doContractAutoRenew.AutoRenewMonth));
                            }
                        }
                        //End Add

                        base.UpdateAutoRenew(doContractAutoRenew.ContractCode, doContractAutoRenew.OCC, tmpDate, BatchDate, ProcessID.C_PROCESS_ID_AUTO_RENEW);
                        
                        //================= Update unimplement data in rental security basic table ============
                        string strUnimplementOCC = string.Empty;
                        strUnimplementOCC = rcHandler.GetLastUnimplementedOCC(doContractAutoRenew.ContractCode);
                        base.UpdateAutoRenew(doContractAutoRenew.ContractCode, strUnimplementOCC, tmpDate, BatchDate, ProcessID.C_PROCESS_ID_AUTO_RENEW);

                        //if(doContractAutoRenew.ProductTypeCode == ProductType.C_PROD_TYPE_MA  )
                        if (doContractAutoRenew.ProductTypeCode == ProductType.C_PROD_TYPE_MA
                            || doContractAutoRenew.ProductTypeCode == ProductType.C_PROD_TYPE_AL
                            || doContractAutoRenew.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE) //Modify by Jutarat A. on 11042013
                        {
                            //mHandler.GenerateMaintenanceSchedule(doContractAutoRenew.ContractCode,GenerateMAProcessType.C_GEN_MA_TYPE_RE_CREATE);
                            mHandler.GenerateMaintenanceSchedule(doContractAutoRenew.ContractCode, GenerateMAProcessType.C_GEN_MA_TYPE_RE_CREATE, BatchDate, ProcessID.C_PROCESS_ID_AUTO_RENEW, false); //Modify by Jutarat A. on 09052013 (Add createDate, createBy, isWriteTransLog)
                        }

                        scope.Complete();
                        result.Complete++;
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        result.Failed++;
                        result.ErrorMessage += string.Format("Error: {0} {1}\n", ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
                    }
                }
            }
            result.Result = result.Failed > 0 ? FlagType.C_FLAG_OFF : FlagType.C_FLAG_ON;

            return result;

        }   
    }


    // == CTP130 BatchAutoRenew ==
    public class CTP130_BatchAutoRenew : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            ContractBatchProcessHandler batch = new ContractBatchProcessHandler();
            return batch.BatchAutoRenewProcess(UserId, BatchDate);
        }
    }

    // == CTP060 SendNotifyEmailForChangeFee ==
    public class CTP060_SendNotifyEmailForChangeFee : IBatchProcess
    {

        #region IBatchProcess Members

        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {

            IContractHandler handler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
            return handler.SendNotifyEmailForChangeFee(UserId, BatchDate);
        }

        #endregion
    }
    
    //public class AutoRenewProcess : IBatchProcess
    //{

    //    #region IBatchProcess Members

    //    public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
    //    {
    //        doBatchProcessResult result;
    //        try
    //        {
    //            IContractHandler handler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
    //            List<AutoRenewProcess_Result> list = handler.AutoRenewProcessBatch();
    //            AutoRenewProcess_Result spResult = list[0];
    //            result = CommonUtil.CloneObject<AutoRenewProcess_Result, doBatchProcessResult>(spResult);
    //            result.BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED;
    //        }
    //        catch (Exception)
    //        {
    //            result = new doBatchProcessResult();
    //            result.BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED;
    //        }


    //        return result;
    //    }

    //    #endregion
    //}
}
