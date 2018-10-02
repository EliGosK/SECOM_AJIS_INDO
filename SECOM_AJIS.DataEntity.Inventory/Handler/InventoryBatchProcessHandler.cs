using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;
using System.Transactions;


namespace SECOM_AJIS.DataEntity.Inventory
{

    public class IVP020_UpdateCalculateDepreciation : IBatchProcess
    {
        #region IBatchProcess Members

        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            const string BatchUserID = "IVP020";
            try
            {
                return (new InventoryHandler()).UpdateCalculateDepreciation(BatchUserID, BatchDate);
            }
            catch (Exception ex)
            {
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = ex.Message, //msg.Message,
                    BatchUser = BatchUserID,
                    BatchDate = BatchDate
                };
            }
        }

        #endregion
    }

    public class IVP050_FreezeInStrumentDataForStockCheckingProcess : IBatchProcess
    {
        #region IBatchProcess Members

        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            const string BatchUserID = "IVP050";
            try
            {
                return (new InventoryHandler()).FreezeInstrumentDataForStockCheckingProcess(BatchUserID, BatchDate);
            }
            catch (Exception ex)
            {
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = ex.Message, //msg.Message,
                    BatchUser = BatchUserID,
                    BatchDate = BatchDate
                };
            }
        }

        #endregion
    }

    public class IVP130_GenerateBlankCheckingSlip : IBatchProcess
    {
        #region IBatchProcess Members

        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            const string BatchUserID = "IVP130";
            try
            {
                return (new InventoryHandler()).GenerateBlankCheckingSlip(BatchUserID, BatchDate);
            }
            catch (Exception ex)
            {
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = ex.Message, //msg.Message,
                    BatchUser = BatchUserID,
                    BatchDate = BatchDate
                };
            }
        }

        #endregion
    }

    public class IVP070_GenerateSummaryInventoryInOutReport : IBatchProcess
    {
        #region IBatchProcess Members

        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            //Test SwtIVP070
            //BatchDate = new DateTime(2012, 3, 1, 18, 00, 00); //Case2
            //BatchDate = new DateTime(2012, 3, 30, 18, 00, 00); //Case3

            const string BatchUserID = "IVP070";
            try
            {
                return (new InventoryHandler()).GenerateSummaryInventoryInOutReport(UserId, BatchDate);
            }
            catch (Exception ex)
            {
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = ex.Message, //msg.Message,
                    BatchUser = BatchUserID,
                    BatchDate = BatchDate
                };
            }
        }

        #endregion

    }

    public class IVP080_GenerateSummaryInventoryAssetReport : IBatchProcess
    {
        #region IBatchProcess Members

        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            //Test SwtIVP080
            //BatchDate = new DateTime(2012, 5, 31);

            const string BatchUserID = "IVP080";
            try
            {
                return (new InventoryHandler()).GenerateInventorySummaryAsset(UserId, BatchDate);
            }
            catch (Exception ex)
            {
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = ex.Message, //msg.Message,
                    BatchUser = BatchUserID,
                    BatchDate = BatchDate
                };
            }
        }

        #endregion
    }

    public class IVP100_GenerateInventoryAccountData : IBatchProcess
    {
        #region IBatchProcess Members

        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            const string BatchUserID = "IVP100";
            try
            {
                return (new InventoryHandler()).GenerateInventoryAccountData(UserId, BatchDate);
            }
            catch (Exception ex)
            {
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = ex.Message, //msg.Message,
                    BatchUser = BatchUserID,
                    BatchDate = BatchDate
                };
            }
        }

        #endregion
    }

    public class IVP140_UpdateMonthlyAveragePrice : IBatchProcess
    {
        #region IBatchProcess Members

        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            const string BatchUserID = "IVP140";
            try
            {
                return (new InventoryHandler()).UpdateMonthlyAveragePrice(BatchUserID, null, BatchDate);
            }
            catch (Exception ex)
            {
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = ex.Message, //msg.Message,
                    BatchUser = BatchUserID,
                    BatchDate = BatchDate
                };
            }
        }

        #endregion
    }

    public class IVP150_FreezeInprocess : IBatchProcess
    {
        #region IBatchProcess Members

        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            const string BatchUserID = "IVP150";
            try
            {
                return (new InventoryHandler()).FreezeInprocess(BatchUserID, null, BatchDate);
            }
            catch (Exception ex)
            {
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = ex.Message, //msg.Message,
                    BatchUser = BatchUserID,
                    BatchDate = BatchDate
                };
            }
        }

        #endregion
    }

}
