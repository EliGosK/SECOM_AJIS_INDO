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
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Installation;
using CSI.WindsorHelper;
using System.Transactions;
using SECOM_AJIS.DataEntity.Quotation;
using System.IO;

// Using by Akat
namespace SECOM_AJIS.DataEntity.Inventory
{
    partial class InventoryHandler : BizIVDataEntities, IInventoryHandler
    {
        #region IVP080 Generate Inventory Summary Asset

        public doBatchProcessResult GenerateInventorySummaryAsset(string strEmpNo, DateTime dtDateTime)
        {
            ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool bSuspendingStatus = commonHandler.IsSystemSuspending();

            bSuspendingStatus = true;

            if (!bSuspendingStatus)
            {
                //return null;
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = "System is not suspending.",
                    BatchUser = strEmpNo,
                    BatchDate = dtDateTime
                };
            }

            string strYearMonth = this.ValidateGenerateSummary(dtDateTime);
            if (strYearMonth == null)
            {
                //return new doBatchProcessResult()
                //{
                //    Result = FlagType.C_FLAG_OFF,
                //    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                //    Total = 1,
                //    Complete = 0,
                //    Failed = 1,
                //    ErrorMessage = "Summary asset report already generate. All insert data is rollbacked.",
                //    BatchUser = strEmpNo,
                //    BatchDate = dtDateTime
                //};
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_ON,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED,
                    Total = 0,
                    Complete = 0,
                    Failed = 0,
                    ErrorMessage = null,
                    BatchUser = strEmpNo,
                    BatchDate = dtDateTime
                };
            }

            List<dtBatchProcessResult> batchResult = base.GenerateInventorySummaryAsset(
                    strYearMonth,
                    FunctionLogistic.C_FUNC_LOGISTIC_HQ,
                    OfficeCode.C_INV_OFFICE_SNR,
                    TransferType.C_INV_TRANSFERTYPE_STOCKIN_PURCHASE,
                    TransferType.C_INV_TRANSFERTYPE_STOCKIN_SPECIAL,
                    TransferType.C_INV_TRANSFERTYPE_TRANSFER_OFFICE,
                    InstrumentArea.C_INV_AREA_NEW_SAMPLE,
                    InstrumentArea.C_INV_AREA_NEW_SALE,
                    InstrumentArea.C_INV_AREA_NEW_RENTAL,
                    InstrumentArea.C_INV_AREA_NEW_DEMO,
                    InstrumentArea.C_INV_AREA_SE_RENTAL,
                    InstrumentArea.C_INV_AREA_SE_LENDING_DEMO,
                    InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO,
                    InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE,
                    InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER,
                    TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START,
                    TransferType.C_INV_TRANSFERTYPE_COMPLETE_BEFORE_START,
                    TransferType.C_INV_TRANSFERTYPE_COMPLETE_MK30,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK20,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK03,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK29,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_SPECIAL,
                    TransferType.C_INV_TRANSFERTYPE_START_SERVICE,
                    TransferType.C_INV_TRANSFERTYPE_PRE_ELIMINATION,
                    TransferType.C_INV_TRANSFERTYPE_CANCEL_INSTALLATION,
                    TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE,
                    TransferType.C_INV_TRANSFERTYPE_CHECKING_RETURNED,
                    TransferType.C_INV_TRANSFERTYPE_REPAIR_REQUEST,
                    TransferType.C_INV_TRANSFERTYPE_REPAIR_RETURN,
                    TransferType.C_INV_TRANSFERTYPE_TRANSFER_BUFFER,
                    TransferType.C_INV_TRANSFERTYPE_CHANGE_INVESTIGATION,
                    SaleInstallationType.C_SALE_INSTALL_TYPE_ADD,
                    SaleInstallationType.C_SALE_INSTALL_TYPE_NEW,
                    SaleInstallationType.C_SALE_INSTALL_TYPE_MAINTENANCE_EXCHANGE, // 11/Sep/2012 Thanawit S.
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW,
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW,
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW,
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE,
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE,
                    RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP,
                    InstrumentLocation.C_INV_LOC_WAITING_RETURN,
                    InstrumentLocation.C_INV_LOC_RETURNED,
                    InstrumentLocation.C_INV_LOC_INSTOCK,
                    InstrumentLocation.C_INV_LOC_REPAIRING,
                    InstrumentLocation.C_INV_LOC_BUFFER,
                    InstrumentLocation.C_INV_LOC_USER,
                    InstrumentLocation.C_INV_LOC_PROJECT_WIP,  // 11/Sep/2012 Thanawit S.
                    InstrumentLocation.C_INV_LOC_UNOPERATED_WIP,  // 11/Sep/2012 Thanawit S.
                    InstrumentLocation.C_INV_LOC_WIP, // 11/Sep/2012 Thanawit S.
                    FlagType.C_FLAG_OFF,
                    FlagType.C_FLAG_ON
                );

            InventoryDocumentHandler docHand = new InventoryDocumentHandler();
            string filePath = docHand.GenerateIVR150FilePath(strYearMonth, strEmpNo, dtDateTime);

            if (filePath == null)
            {
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = "Generate inventory summary asset fail.",
                    BatchUser = strEmpNo,
                    BatchDate = dtDateTime
                };
            }

            return new doBatchProcessResult()
            {
                Result = FlagType.C_FLAG_ON,
                BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED,
                Total = 1,
                Complete = 1,
                Failed = 0,
                ErrorMessage = null,
                BatchUser = strEmpNo,
                BatchDate = dtDateTime
            };
        }

        public string ValidateGenerateSummary(DateTime dtDateTime)
        {
            DateTime current = dtDateTime;
            //DateTime lastWorkDate = GetLastWorkingDate(dtDateTime);
            DateTime? lastWorkDate = GetLastBusinessDate(dtDateTime);
            DateTime lastDayOfMonth = CommonUtil.LastDayOfMonthFromDateTime(current.Month, current.Year);

            string yearMonth = null;

            if (current.CompareTo(lastWorkDate) >= 0 && current.CompareTo(lastDayOfMonth) <= 0)
            {
                yearMonth = current.ToString("yyyyMM");
            }
            else
            {
                yearMonth = current.AddMonths(-1).ToString("yyyyMM");
            }

            List<bool?> existSummaryAsset = base.CheckExistSummaryAsset(yearMonth);
            if (existSummaryAsset.Count <= 0 || existSummaryAsset[0] == null || !existSummaryAsset[0].HasValue || existSummaryAsset[0].Value)
            {
                yearMonth = null;
            }

            return yearMonth;
        }

        protected bool CheckHoliday(DateTime checkDate)
        {
            IMasterHandler hand = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            List<tbm_Calendar> listCal = hand.GetTbm_Calendar(checkDate);

            if (listCal.Count > 0)
            {
                if (DateType.C_DATE_TYPE_SPECIAL_WORKING.Equals(listCal[0].DateTypeCode))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (checkDate.DayOfWeek == DayOfWeek.Sunday || checkDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //protected DateTime GetLastWorkingDate(DateTime dtDateTime)
        //{
        //    DateTime lastDayOfMonth = CommonUtil.LastDayOfMonthFromDateTime(dtDateTime.Month, dtDateTime.Year);

        //    bool notFound = true;

        //    while (notFound)
        //    {
        //        if (!CheckHoliday(lastDayOfMonth)) //Fix bug by Non A. 29/Mar/2012 : Exit loop only if it's not holiday
        //        {
        //            notFound = false;
        //        }
        //        else
        //        {
        //            lastDayOfMonth = lastDayOfMonth.AddDays(-1);
        //        }
        //    }

        //    return lastDayOfMonth;
        //}

        #endregion

        #region IVP070 Generate Summary Inventory In/Out Report

        public doBatchProcessResult GenerateSummaryInventoryInOutReport(string strUserId, DateTime batchDate)
        {
            ICommonHandler commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            bool bSuspendingStatus = commonHandler.IsSystemSuspending();

            if (!bSuspendingStatus)
            {
                //return null;
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = "System is not suspending.",
                    BatchUser = strUserId,
                    BatchDate = batchDate
                };
            }

            DateTime current = batchDate;
            DateTime? lastBizDate = GetLastBusinessDate(batchDate);
            DateTime lastDayOfMonth = CommonUtil.LastDayOfMonthFromDateTime(current.Month, current.Year);

            DateTime param = DateTime.Now;
            //if (DateTime.Now.CompareTo(lastBizDate) > 0)
            if (current.CompareTo(lastBizDate) >= 0 && current.CompareTo(lastDayOfMonth) <= 0)
            {
                param = batchDate;
            }
            else
            {
                param = batchDate.AddMonths(-1);
            }

            List<bool?> existSummaryInOut = base.CheckExistSummaryInOutHeadOffice(param);
            if (existSummaryInOut.Count <= 0 || existSummaryInOut[0] == null || !existSummaryInOut[0].HasValue || existSummaryInOut[0].Value)
            {
                //return new doBatchProcessResult()
                //{
                //    Result = FlagType.C_FLAG_OFF,
                //    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                //    Total = 4,
                //    Complete = 0,
                //    Failed = 4,
                //    ErrorMessage = "INSERT summary process is failed. All insert data is rollbacked.",
                //    BatchUser = strUserId,
                //    BatchDate = batchDate
                //};
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_ON,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED,
                    Total = 0,
                    Complete = 0,
                    Failed = 0,
                    ErrorMessage = null,
                    BatchUser = strUserId,
                    BatchDate = batchDate
                };
            }

            List<doOffice> IvHeadOffice = GetInventoryHeadOffice();
            List<dtBatchProcessResult> batchResult = null;
            try
            {
                batchResult = base.GenerateSummaryInventoryInOutReport(
                    param,
                    batchDate,
                    InstrumentLocation.C_INV_LOC_BUFFER,
                    InstrumentLocation.C_INV_LOC_INSTOCK,
                    InstrumentLocation.C_INV_LOC_PRE_ELIMINATION,
                    InstrumentLocation.C_INV_LOC_REPAIR_REQUEST,
                    InstrumentLocation.C_INV_LOC_REPAIR_RETURN,
                    InstrumentLocation.C_INV_LOC_RETURN_WIP,
                    InstrumentLocation.C_INV_LOC_RETURNED,
                    InstrumentLocation.C_INV_LOC_SPECIAL,
                    InstrumentLocation.C_INV_LOC_TRANSFER,
                    InstrumentLocation.C_INV_LOC_UNOPERATED_WIP,
                    InstrumentLocation.C_INV_LOC_USER,
                    InstrumentLocation.C_INV_LOC_WAITING_RETURN,
                    InstrumentLocation.C_INV_LOC_WIP,
                    InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE,
                    TransferType.C_INV_TRANSFERTYPE_CANCEL_INSTALLATION,
                    TransferType.C_INV_TRANSFERTYPE_CHANGE_INVESTIGATION,
                    TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START,
                    TransferType.C_INV_TRANSFERTYPE_COMPLETE_BEFORE_START,
                    TransferType.C_INV_TRANSFERTYPE_PROJECT_COMPLETE,
                    TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE,
                    TransferType.C_INV_TRANSFERTYPE_FIX_ADJUSTMENT,
                    TransferType.C_INV_TRANSFERTYPE_PRE_ELIMINATION,
                    TransferType.C_INV_TRANSFERTYPE_RECEIVE_RETURNED,
                    TransferType.C_INV_TRANSFERTYPE_REPAIR_REQUEST,
                    TransferType.C_INV_TRANSFERTYPE_REPAIR_RETURN,
                    TransferType.C_INV_TRANSFERTYPE_START_SERVICE,
                    TransferType.C_INV_TRANSFERTYPE_STOCKIN_PURCHASE,
                    TransferType.C_INV_TRANSFERTYPE_STOCKIN_SPECIAL,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK03,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK20,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK29,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PROJECT,
                    TransferType.C_INV_TRANSFERTYPE_STOCKOUT_SPECIAL,
                    TransferType.C_INV_TRANSFERTYPE_TRANSFER_BUFFER,
                    TransferType.C_INV_TRANSFERTYPE_TRANSFER_OFFICE,
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW,
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_NEW,
                    RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL,
                    SaleInstallationType.C_SALE_INSTALL_TYPE_REMOVE_ALL,
                    FlagType.C_FLAG_OFF,
                    FlagType.C_FLAG_ON,
                    InstrumentLocation.C_INV_LOC_PROJECT_WIP,
                    InstrumentLocation.C_INV_LOC_PARTIAL_OUT,
                    InstrumentLocation.C_INV_LOC_REPAIRING,
                    InstrumentLocation.C_INV_LOC_ELIMINATION
                );
            }
            catch (Exception e)
            {
                throw;
            }

            InventoryDocumentHandler docHand = new InventoryDocumentHandler();
            string filePath = docHand.GenerateIVR140FilePath(param, "IVP140", batchDate);

            return new doBatchProcessResult()
            {
                Result = FlagType.C_FLAG_ON,
                BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED,
                Total = 4,
                Complete = 4,
                Failed = 0,
                ErrorMessage = null,
                BatchUser = strUserId,
                BatchDate = batchDate
            };
        }

        #endregion

        #region IVP030 Update Inventory Process

        public bool UpdateCompleteInstallation(string strInstallationSlipNo, string strContractCode)
        {
            bool blnBeforeOperate = false;
            IInstallationHandler installHand = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            IRentralContractHandler rentalHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;


            //1. Get Headoffice data
            List<doOffice> headOffice = this.GetInventoryHeadOffice();

            //2. Get installation slip details data
            //List<tbt_InstallationSlipDetails> installSlipDetailList = installHand.GetTbt_InstallationSlipDetailsData(strInstallationSlipNo, null);
            List<doInstallationDetailForCompleteInstallation> installSlipDetailList = installHand.GetInstallationDetailForCompleteInstallation(strInstallationSlipNo);

            //3. Get Contract data
            List<tbt_RentalContractBasic> rentalContract = rentalHand.GetTbt_RentalContractBasic(strContractCode, null);

            //4. Get before or after operate
            if (rentalContract.Count <= 0 || (
                rentalContract[0].StartType != StartType.C_START_TYPE_RESUME
                && rentalContract[0].StartType != StartType.C_START_TYPE_NEW_START))
            {
                blnBeforeOperate = true;
            }
            else
            {
                blnBeforeOperate = false;
            } //end if/else


            List<tbt_InventorySlipDetail> invSlipDetailForInstalledList = new List<tbt_InventorySlipDetail>();
            List<tbt_InventorySlipDetail> invSlipDetailForNotInstalledList = new List<tbt_InventorySlipDetail>();
            List<tbt_InventorySlipDetail> invSlipDetailForRemoveList = new List<tbt_InventorySlipDetail>();
            List<tbt_InventorySlipDetail> invSlipDetailForUnremovableList = new List<tbt_InventorySlipDetail>();

            #region 5. Insert transfer data to data object

            foreach (var installSlip in installSlipDetailList)
            {

                //5.1 If doTbt_InstallationSlipDetails[i].TotalStockoutQty > 0 Then Insert to installed instrument and not installed instrument to data object 
                if (installSlip.TotalStockOutQty > 0)
                {
                    //5.1.1	Call InventoryHandler.GetCompleteStockoutInstrument
                    List<doCompleteStockoutInstrument> completeStockoutList = this.GetCompleteStockoutInstrument(
                                                                            strContractCode,
                                                                            installSlip.InstrumentCode,
                                                                            installSlip.TotalStockOutQty,
                                                                            InstrumentArea.C_INV_AREA_NEW_RENTAL,
                                                                            InstrumentArea.C_INV_AREA_NEW_SAMPLE,
                                                                            InstrumentArea.C_INV_AREA_SE_RENTAL,
                                                                            InstrumentLocation.C_INV_LOC_WIP);

                    //5.1.2	Set intInstalledQty
                    int intInstalledQty = installSlip.TotalStockOutQty.Value - (
                        (installSlip.NotInstalledQty.HasValue ? installSlip.NotInstalledQty.Value : 0) +
                        (installSlip.ReturnQty.HasValue ? installSlip.ReturnQty.Value : 0));

                    //5.1.3	Loop in doCompleteStockOutInstrument[]
                    foreach (var completeStockout in completeStockoutList)
                    {
                        if (intInstalledQty <= 0)
                        {
                            break;
                        }

                        int intTransferQty;

                        //5.1.3.2.	set doCompleteStockoutInstrument[j].InstrumentQty and intTransferQty
                        if (completeStockout.InstrumentQty.HasValue && (completeStockout.InstrumentQty >= intInstalledQty))
                        {
                            intTransferQty = intInstalledQty;
                            completeStockout.InstrumentQty = completeStockout.InstrumentQty - intInstalledQty;
                        }
                        else
                        {
                            intTransferQty = completeStockout.InstrumentQty.HasValue ? completeStockout.InstrumentQty.Value : 0;
                            completeStockout.InstrumentQty = 0;
                        }

                        // 5.1.3.3. Set intInstalledQty = intInstalledQty - intTransferQty
                        intInstalledQty = intInstalledQty - intTransferQty;

                        // 5.1.3.4.	Add data in doTbt_InventorySlipDetailForInstalled[] 
                        tbt_InventorySlipDetail invSlipDetail = new tbt_InventorySlipDetail();
                        invSlipDetail.InstrumentCode = completeStockout.InstrumentCode;
                        invSlipDetail.SourceAreaCode = completeStockout.AreaCode;
                        if (blnBeforeOperate)
                        {
                            invSlipDetail.DestinationAreaCode = completeStockout.AreaCode;
                        }
                        else
                        {
                            invSlipDetail.DestinationAreaCode = InstrumentArea.C_INV_AREA_SE_RENTAL;
                        }
                        invSlipDetail.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                        invSlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                        invSlipDetail.TransferQty = intTransferQty;
                        invSlipDetailForInstalledList.Add(invSlipDetail);
                    } //end foreach

                    //5.1.5	Loop in doCompleteStockOutInstrument[]
                    foreach (var completeStockout in completeStockoutList)
                    {

                        //5.1.5.1.	If doCompleteStockOutInsturment[j].InstrumentQty > 0 Then
                        if (completeStockout.InstrumentQty > 0)
                        {

                            //5.1.5.2. Add data in doTbt_InventorySlipDetailForNotInstalled[] 
                            tbt_InventorySlipDetail invSlipDetail = new tbt_InventorySlipDetail();
                            invSlipDetail.InstrumentCode = completeStockout.InstrumentCode;
                            invSlipDetail.SourceAreaCode = completeStockout.AreaCode;
                            invSlipDetail.DestinationAreaCode = completeStockout.AreaCode;
                            invSlipDetail.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                            invSlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                            invSlipDetail.TransferQty = completeStockout.InstrumentQty;
                            invSlipDetail.NotInstalledQty = completeStockout.InstrumentQty;
                            invSlipDetailForNotInstalledList.Add(invSlipDetail);
                        } //end if
                    } //end foreach
                } //end if

                // 5.2	If doTbt_InstallationSlipDetails[i].AddRemovedQty > 0  AND blnBeforeOperate = TRUE Then Insert removed instrument before start service
                if (installSlip.AddRemovedQty.HasValue && installSlip.AddRemovedQty > 0 && blnBeforeOperate)
                {

                    // 5.2.1 Call InventoryHandler.GetCompleteRemoveInstrument
                    List<doCompleteRemoveInstrument> completeRmoveList = this.GetCompleteRemoveInstrument(
                                                                            strContractCode,
                                                                            installSlip.InstrumentCode,
                                                                            installSlip.AddRemovedQty,
                                                                            InstrumentArea.C_INV_AREA_NEW_RENTAL,
                                                                            InstrumentArea.C_INV_AREA_NEW_SAMPLE,
                                                                            InstrumentArea.C_INV_AREA_SE_RENTAL,
                                                                            InstrumentLocation.C_INV_LOC_UNOPERATED_WIP);

                    // 5.2.2 Set intRemoveQty = doTbt_InstallationSlipDetails[i].AddRemoveQty – doTbt_InstallationSlipDetails[i].UnRemovableQty
                    int intRemoveQty = installSlip.AddRemovedQty.Value - (installSlip.UnremovableQty.HasValue ? installSlip.UnremovableQty.Value : 0);


                    // 5.2.3 Loop in doCompleteRemoveInstrument[]
                    foreach (var completeRemove in completeRmoveList)
                    {
                        //5.2.3.1. If intRemoveQty <= 0 Then exit loop
                        if (intRemoveQty <= 0)
                        {
                            break;
                        }

                        int intTransferQty = 0;

                        // 5.2.3.2.	Set intTransferQty and InstrumentQty
                        if (completeRemove.InstrumentQty.HasValue && completeRemove.InstrumentQty >= intRemoveQty)
                        {
                            intTransferQty = intRemoveQty;
                            completeRemove.InstrumentQty = completeRemove.InstrumentQty - intRemoveQty;
                        }
                        else if (completeRemove.InstrumentQty.HasValue && completeRemove.InstrumentQty < intRemoveQty)
                        {
                            intTransferQty = completeRemove.InstrumentQty.Value;
                            completeRemove.InstrumentQty = 0;
                        } //end if/else

                        // 5.2.3.3. Set intRemoveQty = intRemoveQty - intTransferQty
                        intRemoveQty = intRemoveQty - intTransferQty;

                        // 5.2.3.4. Add data in doTbt_InventorySlipDetailForRemove
                        tbt_InventorySlipDetail removed = new tbt_InventorySlipDetail();
                        removed.InstrumentCode = completeRemove.InstrumentCode;
                        removed.SourceAreaCode = completeRemove.AreaCode;
                        removed.DestinationAreaCode = completeRemove.AreaCode;
                        removed.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                        removed.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                        removed.TransferQty = intTransferQty;
                        removed.RemovedQty = intTransferQty;
                        invSlipDetailForRemoveList.Add(removed);
                    } //end foreach
                } //end if

                // 5.3 Else If doTbt_InstallationSlipDetails[i].AddRemovedQty > 0  AND blnBeforeOperate = FALSE Then Insert removed and unremovable instrument after start service
                else if (installSlip.AddRemovedQty.HasValue && installSlip.AddRemovedQty > 0 && !blnBeforeOperate)
                {

                    if (installSlip.AddRemovedQty.GetValueOrDefault(0) - installSlip.UnremovableQty.GetValueOrDefault(0) > 0)
                    {
                        // 5.3.1 Add data in doTbt_InventorySlipDetailForRemove[] 
                        tbt_InventorySlipDetail removed = new tbt_InventorySlipDetail();
                        removed.InstrumentCode = installSlip.InstrumentCode;
                        removed.SourceAreaCode = InstrumentArea.C_INV_AREA_SE_RENTAL;
                        removed.DestinationAreaCode = InstrumentArea.C_INV_AREA_SE_RENTAL;
                        removed.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                        removed.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                        removed.TransferQty = installSlip.AddRemovedQty - (installSlip.UnremovableQty.HasValue ? installSlip.UnremovableQty.Value : 0);
                        removed.RemovedQty = installSlip.AddRemovedQty - (installSlip.UnremovableQty.HasValue ? installSlip.UnremovableQty.Value : 0);
                        invSlipDetailForRemoveList.Add(removed);
                    }

                    if (installSlip.UnremovableQty.GetValueOrDefault(0) > 0)
                    {
                        // 5.3.2 Add data in doTbt_InventorySlipDetailForUnRemovable[] 
                        tbt_InventorySlipDetail unRemove = new tbt_InventorySlipDetail();
                        unRemove.InstrumentCode = installSlip.InstrumentCode;
                        unRemove.SourceAreaCode = InstrumentArea.C_INV_AREA_SE_RENTAL;
                        unRemove.DestinationAreaCode = InstrumentArea.C_INV_AREA_SE_RENTAL;
                        unRemove.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                        unRemove.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                        unRemove.TransferQty = installSlip.UnremovableQty.GetValueOrDefault(0);
                        unRemove.UnremovableQty = installSlip.UnremovableQty.GetValueOrDefault(0);
                        invSlipDetailForUnremovableList.Add(unRemove);
                    }

                } //end if/else
            } //end foreach

            #endregion

            #region 6. Update transfer instrument installed data

            // 6.1 If doTbt_InventorySlipDetailForInstalled[].Rows.Count > 0 Then
            if (invSlipDetailForInstalledList.Count > 0)
            {

                string strTransferTypeCode = null;
                string strDestLocationCode = null;
                // 6.1.1 Set transferType
                if (blnBeforeOperate)
                {
                    strDestLocationCode = InstrumentLocation.C_INV_LOC_UNOPERATED_WIP;
                    strTransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_BEFORE_START;
                }
                else
                {
                    strDestLocationCode = InstrumentLocation.C_INV_LOC_USER;
                    strTransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START;
                } //end if/else

                doRegisterTransferInstrumentData registerTransferInstrument = new doRegisterTransferInstrumentData();

                // 6.1.2 Set doRegisterTransferInstrumentData.SlipId = C_INV_SLIPID_STOCKOUT
                registerTransferInstrument.SlipId = SlipID.C_INV_SLIPID_STOCK_OUT;

                // 6.1.3 Set data in doRegisterTransferInstrumentData.doTbt_InventorySlip as follows: 
                registerTransferInstrument.InventorySlip = new tbt_InventorySlip();
                registerTransferInstrument.InventorySlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
                registerTransferInstrument.InventorySlip.TransferTypeCode = strTransferTypeCode;
                registerTransferInstrument.InventorySlip.InstallationSlipNo = strInstallationSlipNo;
                if (rentalContract.Count != 0)
                {
                    registerTransferInstrument.InventorySlip.ProjectCode = rentalContract[0].ProjectCode;
                } //end if
                registerTransferInstrument.InventorySlip.ContractCode = strContractCode;
                registerTransferInstrument.InventorySlip.SlipIssueDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.StockInDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.StockOutDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_WIP;
                registerTransferInstrument.InventorySlip.DestinationLocationCode = strDestLocationCode;
                if (headOffice.Count != 0)
                {
                    registerTransferInstrument.InventorySlip.SourceOfficeCode = headOffice[0].OfficeCode;
                    registerTransferInstrument.InventorySlip.DestinationOfficeCode = headOffice[0].OfficeCode;
                } //end if
                registerTransferInstrument.InventorySlip.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                registerTransferInstrument.InventorySlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                registerTransferInstrument.InventorySlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                // 6.1.4 Set doRegisterTransferInstrumentData.doTbt_InventorySlipDetail[] = doTbt_InventorySlipDetailForInstalled[]
                registerTransferInstrument.lstTbt_InventorySlipDetail = invSlipDetailForInstalledList;

                // 6.1.5 Update inventory transfer data for installed
                string strInventorySlipNo = this.RegisterTransferInstrument(registerTransferInstrument);

                // 6.1.6 Check there are new instrument
                int blnCheckNewInstrument = this.CheckNewInstrument(strInventorySlipNo);

                // 6.1.7 If blnCheckNewInstrument = 1 Then
                if (blnCheckNewInstrument == 1)
                {

                    //6.1.8	Prepare data for update new instrument to account stock data
                    List<doGroupNewInstrument> groupNewList = this.GetGroupNewInstrument(strInventorySlipNo);

                    foreach (var groupNew in groupNewList)
                    {

                        decimal decMovingAveragePrice = 0;

                        // 6.1.8.1. Check for calculate moving average price
                        if (!blnBeforeOperate)
                        {
                            //6.1.8.1.1. Get moving average price
                            List<tbt_AccountInprocess> accountInprocessList = this.GetTbt_AccountInProcess(
                                                                                groupNew.SourceLocationCode,
                                                                                strContractCode,
                                                                                groupNew.Instrumentcode);

                            if (accountInprocessList.Count > 0)
                            {
                                decMovingAveragePrice = accountInprocessList[0].MovingAveragePrice.Value;
                            } //end if

                            // 6.1.8.1.2. Get lot no 
                            doInsertDepreciationData insertDepreciation = new doInsertDepreciationData();
                            insertDepreciation.ContractCode = strContractCode;
                            insertDepreciation.InstrumentCode = groupNew.Instrumentcode;
                            insertDepreciation.StartYearMonth = DateTime.Now.ToString("yyyyMM");
                            insertDepreciation.MovingAveragePrice = decMovingAveragePrice;
                            string strLotNo = this.InsertDepreciationData(insertDepreciation);

                            // 6.1.8.1.3. Set doGroupNewInstrument[j].LotNo = strLotNo
                            groupNew.LotNo = strLotNo;

                            //6.1.8.1.4. Calculate moving average price
                            #region Monthly Price @ 2015
                            //decMovingAveragePrice = (decimal)this.CalculateMovingAveragePrice(groupNew);
                            decMovingAveragePrice = (decimal)this.GetMonthlyAveragePrice(groupNew.Instrumentcode, registerTransferInstrument.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                            #endregion
                        }
                        else
                        {
                            decMovingAveragePrice = 0;
                        } //end if/else

                        // 6.1.8.2. Update transfer data to Account DB
                        bool blnUpdate = this.UpdateAccountTransferNewInstrument(groupNew, decMovingAveragePrice);

                        // 6.1.8.3.	If blnUpdate <> TRUE Then Rollback Transaction
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if

                        //6.1.8.4. If blnBeforeOperate = FALSE Then Update transfer data to account moving stock
                        if (!blnBeforeOperate)
                        {

                            // 6.1.8.4.1. Call GetTbt_AccountInProcess
                            List<tbt_AccountInprocess> accountInProcessList = this.GetTbt_AccountInProcess(
                                                                                groupNew.SourceLocationCode,
                                                                                groupNew.ContractCode,
                                                                                groupNew.Instrumentcode);

                            // 6.1.8.4.3. Call InventoryHandler.InsertAccountStockMoving
                            tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                            accountStockMoving.SlipNo = strInventorySlipNo;
                            accountStockMoving.TransferTypeCode = strTransferTypeCode;
                            accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                            accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTALLED;
                            accountStockMoving.SourceLocationCode = groupNew.SourceLocationCode;
                            accountStockMoving.DestinationLocationCode = groupNew.DestinationLocationCode;
                            accountStockMoving.InstrumentCode = groupNew.Instrumentcode;
                            accountStockMoving.InstrumentQty = groupNew.TransferQty;
                            if (accountInProcessList.Count != 0)
                            {
                                accountStockMoving.InstrumentPrice = accountInProcessList[0].MovingAveragePrice;
                            } //end if
                            accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                            targetAccountStockMovingList.Add(accountStockMoving);
                            List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                            if (resultAccountStockMovingList.Count <= 0)
                            {
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                            } //end if/else
                        } //end if
                    } //end foreach
                } //end if 6.1.7

                // 6.1.9 Check there are secondhand instrument
                int blnCheckSecondhandInstrument = this.CheckSecondhandInstrument(strInventorySlipNo);
                if (blnCheckSecondhandInstrument == 1)
                {

                    // 6.1.10 Prepare data for update secondhand instrument to account stock data
                    List<doGroupSecondhandInstrument> groupSecondHandList = this.GetGroupSecondhandInstrument(strInventorySlipNo);

                    // 6.1.11 Loop In doGroupSecondhandInstrument[]
                    foreach (var groupSecondhand in groupSecondHandList)
                    {
                        // 6.1.11.1. Update secondhand instrument to Account DB
                        bool blnUpdate = this.UpdateAccountTransferSecondhandInstrument(groupSecondhand);

                        // 6.1.11.2. If blnUpdate <> TRUE Then rollback
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if
                    } //end foreach
                } //end if 6.1.9

                // 6.1.12 Check there are sample instrument
                int blnCheckSampleInstrument = this.CheckSampleInstrument(strInventorySlipNo);
                if (blnCheckSampleInstrument == 1)
                {

                    // 6.1.14 Prepare data for update sample instrument to account stock data
                    List<doGroupSampleInstrument> groupSampleList = this.GetGroupSampleInstrument(strInventorySlipNo);

                    // 6.1.15 Loop In doGroupSampleInstrument[]
                    foreach (var groupSample in groupSampleList)
                    {

                        decimal decMovingAveragePrice = 0;

                        // 6.1.15.1. Check for calculate moving average price
                        if (!blnBeforeOperate)
                        {
                            decMovingAveragePrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;

                            // 6.1.15.1.1. Get lot no 
                            doInsertDepreciationData insertDepreciation = new doInsertDepreciationData();
                            insertDepreciation.ContractCode = strContractCode;
                            insertDepreciation.InstrumentCode = groupSample.Instrumentcode;
                            insertDepreciation.StartYearMonth = DateTime.Now.ToString("yyyyMM");
                            insertDepreciation.MovingAveragePrice = decMovingAveragePrice;
                            string strLotNo = this.InsertDepreciationData(insertDepreciation);

                            // 6.1.15.1.2. Calculate moving average price
                            doGroupNewInstrument groupNew = new doGroupNewInstrument();
                            groupNew.SourceOfficeCode = groupSample.SourceOfficeCode;
                            groupNew.DestinationOfficeCode = groupSample.DestinationOfficeCode;
                            groupNew.SourceLocationCode = groupSample.SourceLocationCode;
                            groupNew.DestinationLocationCode = groupSample.DestinationLocationCode;
                            groupNew.ProjectCode = groupSample.ProjectCode;
                            groupNew.ContractCode = groupSample.ContractCode;
                            groupNew.Instrumentcode = groupSample.Instrumentcode;
                            groupNew.TransferQty = groupSample.TransferQty;
                            groupNew.ObjectID = InstrumentArea.C_INV_AREA_NEW_SAMPLE;
                            groupNew.LotNo = strLotNo;
                            #region Monthly Price @ 2015
                            //decMovingAveragePrice = (decimal)this.CalculateMovingAveragePrice(groupNew);
                            decMovingAveragePrice = (decimal)this.GetMonthlyAveragePrice(groupNew.Instrumentcode, registerTransferInstrument.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                            #endregion

                            groupSample.LotNo = strLotNo;
                        } //end if

                        // 6.1.15.2. Update transfer data to Account DB
                        bool blnUpdate = this.UpdateAccountTransferSampleInstrument(groupSample, decMovingAveragePrice);

                        //6.1.15.3.	If blnUpdate <> TRUE Then rollback
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if

                        // 6.1.15.4. Update transfer data to account moving stock
                        if (!blnBeforeOperate)
                        {

                            //6.1.15.4.2. Call InventoryHandler.InsertAccountStockMoving
                            tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                            accountStockMoving.SlipNo = strInventorySlipNo;
                            accountStockMoving.TransferTypeCode = strTransferTypeCode;
                            accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                            accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTALLED;
                            accountStockMoving.SourceLocationCode = groupSample.SourceLocationCode;
                            accountStockMoving.DestinationLocationCode = groupSample.DestinationLocationCode;
                            accountStockMoving.InstrumentCode = groupSample.Instrumentcode;
                            accountStockMoving.InstrumentQty = groupSample.TransferQty;
                            accountStockMoving.InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                            accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                            targetAccountStockMovingList.Add(accountStockMoving);
                            List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                            if (resultAccountStockMovingList.Count <= 0)
                            {
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                            } //end if/else
                        } //end if
                    } //end foreach
                } //end if 6.1.12
            } //end if  6.1

            #endregion

            #region 7. Update transfer instrument Not installed data
            // 7.1 If doTbt_InventorySlipDetailForNotInstalled[].Rows.Count > 0 Then
            if (invSlipDetailForNotInstalledList.Count > 0)
            {

                //7.1.1	set strTransferTypeCode
                string strTransferTypeCode = null;
                if (blnBeforeOperate)
                {
                    strTransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_BEFORE_START;
                }
                else
                {
                    strTransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START;
                } //end if/else

                doRegisterTransferInstrumentData registerTransferInstrument = new doRegisterTransferInstrumentData();

                //7.1.2	Set doRegisterTransferInstrumentData.SlipId = C_INV_SLIPID_STOCKOUT
                registerTransferInstrument.SlipId = SlipID.C_INV_SLIPID_STOCK_OUT;

                // 7.1.3 Set data in doRegisterTransferInstrumentData.doTbt_InventorySlip
                registerTransferInstrument.InventorySlip = new tbt_InventorySlip();
                registerTransferInstrument.InventorySlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER;
                registerTransferInstrument.InventorySlip.TransferTypeCode = strTransferTypeCode;
                registerTransferInstrument.InventorySlip.InstallationSlipNo = strInstallationSlipNo;
                if (rentalContract.Count > 0)
                {
                    registerTransferInstrument.InventorySlip.ProjectCode = rentalContract[0].ProjectCode;
                }
                registerTransferInstrument.InventorySlip.ContractCode = strContractCode;
                registerTransferInstrument.InventorySlip.SlipIssueDate = DateTime.Now;
                //registerTransferInstrument.InventorySlip.StockInDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.StockOutDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_WIP;
                registerTransferInstrument.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_RETURNED;
                if (headOffice.Count != 0)
                {
                    registerTransferInstrument.InventorySlip.SourceOfficeCode = headOffice[0].OfficeCode;
                    registerTransferInstrument.InventorySlip.DestinationOfficeCode = headOffice[0].OfficeCode;
                }
                registerTransferInstrument.InventorySlip.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                registerTransferInstrument.InventorySlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                registerTransferInstrument.InventorySlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                // 7.1.4 Set doRegisterTransferInstrumentData.doTbt_InventorySlipDetail[] = doTbt_InventorySlipDetailForNotInstalled[]
                registerTransferInstrument.lstTbt_InventorySlipDetail = invSlipDetailForNotInstalledList;

                // 7.1.5 Update inventory transfer data for installed
                string strInventorySlipNo = this.RegisterTransferInstrument(registerTransferInstrument);

                // 7.1.6 Check there are new instrument
                int blnCheckNewInstrument = this.CheckNewInstrument(strInventorySlipNo);

                // 7.1.7 If blnCheckNewInstrument = 1 Then
                if (blnCheckNewInstrument == 1)
                {

                    // 7.1.8 Prepare data for update new instrument to account stock data
                    List<doGroupNewInstrument> groupNewList = this.GetGroupNewInstrument(strInventorySlipNo);
                    foreach (var groupNew in groupNewList)
                    {
                        groupNew.DestinationLocationCode = InstrumentLocation.C_INV_LOC_WAITING_RETURN;

                        decimal decMovingAveragePrice = 0;

                        // 7.1.8.1. Calculate moving average price
                        #region Monthly Price @ 2015
                        //decMovingAveragePrice = (decimal)this.CalculateMovingAveragePrice(groupNew);
                        decMovingAveragePrice = (decimal)this.GetMonthlyAveragePrice(groupNew.Instrumentcode, registerTransferInstrument.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                        #endregion

                        // 7.1.8.2.	Update transfer data to Account DB
                        bool blnUpdate = this.UpdateAccountTransferNewInstrument(groupNew, decMovingAveragePrice);

                        // 7.1.8.3.	If blnUpdate <> TRUE Then rollback
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if

                        #region // New WIP concept @ 24-Feb-2015
                        //// 7.1.8.4. Update transfer data to account moving stock
                        //List<tbt_AccountInprocess> accountInProcessList = this.GetTbt_AccountInProcess(
                        //                                                    groupNew.SourceLocationCode,
                        //                                                    groupNew.ContractCode,
                        //                                                    groupNew.Instrumentcode);

                        //tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                        //accountStockMoving.SlipNo = strInventorySlipNo;
                        //accountStockMoving.TransferTypeCode = strTransferTypeCode;
                        //accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                        //accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                        //accountStockMoving.SourceLocationCode = groupNew.SourceLocationCode;
                        //accountStockMoving.DestinationLocationCode = groupNew.DestinationLocationCode;
                        //accountStockMoving.InstrumentCode = groupNew.Instrumentcode;
                        //accountStockMoving.InstrumentQty = groupNew.TransferQty;
                        //if (accountInProcessList.Count != 0)
                        //{
                        //    accountStockMoving.InstrumentPrice = accountInProcessList[0].MovingAveragePrice;
                        //}
                        //accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                        //targetAccountStockMovingList.Add(accountStockMoving);
                        //List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                        //if (resultAccountStockMovingList.Count <= 0)
                        //{
                        //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                        //} //end if/else 
                        #endregion
                    } //end foreach
                } //end if 7.1.7

                // 7.1.9 Check there are secondhand instrument
                int blnCheckSecondhandInstrument = this.CheckSecondhandInstrument(strInventorySlipNo);
                if (blnCheckSecondhandInstrument == 1)
                {

                    // 7.1.10 Prepare data for update secondhand instrument to account stock data
                    List<doGroupSecondhandInstrument> groupSecondHandList = this.GetGroupSecondhandInstrument(strInventorySlipNo);

                    // 7.1.11 Loop In doGroupSecondhandInstrument[]
                    foreach (var groupSecondHand in groupSecondHandList)
                    {
                        groupSecondHand.DestinationLocationCode = InstrumentLocation.C_INV_LOC_WAITING_RETURN;

                        // 7.1.11.1. Update secondhand instrument to Account DB
                        bool blnUpdate = this.UpdateAccountTransferSecondhandInstrument(groupSecondHand);

                        // 7.1.11.2. If blnUpdate <> TRUE Then rollback
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if
                    } //end foreach
                } //end if 7.1.9

                // 7.1.12 Check there are sample instrument
                int blnCheckSampleInstrument = this.CheckSampleInstrument(strInventorySlipNo);

                // 7.1.13 If blnCheckSampleInstrument = 1 Then
                if (blnCheckSampleInstrument == 1)
                {

                    // 7.1.14 Prepare data for update sample instrument to account stock data
                    List<doGroupSampleInstrument> groupSampleList = this.GetGroupSampleInstrument(strInventorySlipNo);

                    // 7.1.15 Loop In doGroupSampleInstrument[]
                    foreach (var groupSample in groupSampleList)
                    {
                        groupSample.DestinationLocationCode = InstrumentLocation.C_INV_LOC_WAITING_RETURN;

                        // 7.1.15.1. Update transfer data to Account DB
                        bool blnUpdate = this.UpdateAccountTransferSampleInstrument(groupSample, null);
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if

                        #region // New WIP concept @ 24-Feb-2015
                        //// 7.1.15.2. Update transfer data to account moving stock
                        //if (true) //(!blnBeforeOperate)
                        //{

                        //    // 7.1.15.2.2.	Call InventoryHandler.InsertAccountStockMoving
                        //    tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                        //    accountStockMoving.SlipNo = strInventorySlipNo;
                        //    accountStockMoving.TransferTypeCode = strTransferTypeCode;
                        //    accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                        //    accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK; // CHECK
                        //    accountStockMoving.SourceLocationCode = groupSample.SourceLocationCode;
                        //    accountStockMoving.DestinationLocationCode = groupSample.DestinationLocationCode;
                        //    accountStockMoving.InstrumentCode = groupSample.Instrumentcode;
                        //    accountStockMoving.InstrumentQty = groupSample.TransferQty;
                        //    accountStockMoving.InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                        //    accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //    accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //    accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //    accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //    List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                        //    targetAccountStockMovingList.Add(accountStockMoving);
                        //    List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                        //    if (resultAccountStockMovingList.Count <= 0)
                        //    {
                        //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                        //    } //end if/else
                        //} //end if 
                        #endregion
                    } //end foreach
                } //end if 7.1.13
            } //end if 7.1

            #endregion

            #region 9. Update transfer instrument Unremovable data

            // 9.1 If doTbt_InventorySlipDetailForUnRemovable[].Rows.Count > 0 Then
            if (invSlipDetailForUnremovableList.Count > 0)
            {

                doRegisterTransferInstrumentData registerTransferInstrument = new doRegisterTransferInstrumentData();

                // 9.1.1 Set doRegisterTransferInstrumentData.SlipId = C_INV_SLIPID_STOCKOUT
                registerTransferInstrument.SlipId = SlipID.C_INV_SLIPID_STOCK_OUT;

                // 9.1.2 Set data in doRegisterTransferInstrumentData.doTbt_InventorySlip
                registerTransferInstrument.InventorySlip = new tbt_InventorySlip();
                registerTransferInstrument.InventorySlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER;
                registerTransferInstrument.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START;
                registerTransferInstrument.InventorySlip.InstallationSlipNo = strInstallationSlipNo;
                if (rentalContract.Count > 0)
                {
                    registerTransferInstrument.InventorySlip.ProjectCode = rentalContract[0].ProjectCode;
                }
                registerTransferInstrument.InventorySlip.ContractCode = strContractCode;
                registerTransferInstrument.InventorySlip.SlipIssueDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.StockInDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.StockOutDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_USER;
                registerTransferInstrument.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_ELIMINATION;
                if (headOffice.Count != 0)
                {
                    registerTransferInstrument.InventorySlip.SourceOfficeCode = headOffice[0].OfficeCode;
                    registerTransferInstrument.InventorySlip.DestinationOfficeCode = headOffice[0].OfficeCode;
                }
                registerTransferInstrument.InventorySlip.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                registerTransferInstrument.InventorySlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                registerTransferInstrument.InventorySlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                // 9.1.3 Set doRegisterTransferInstrumentData.doTbt_InventorySlipDetail[] = doTbt_InventorySlipDetailForRemove[]
                registerTransferInstrument.lstTbt_InventorySlipDetail = invSlipDetailForUnremovableList;

                // 9.1.4 Update inventory transfer data for installed
                string strInventorySlipNo = this.RegisterTransferInstrument(registerTransferInstrument);

                // 9.1.5 Prepare data for update secondhand instrument to account stock data
                List<doGroupSecondhandInstrument> groupSecondHandList = this.GetGroupSecondhandInstrument(strInventorySlipNo);

                List<tbt_AccountStockMoving> listAccountStockMoving = new List<tbt_AccountStockMoving>();

                // 9.1.6 Loop In doGroupSecondhandInstrument[]
                foreach (var groupSecondHand in groupSecondHandList)
                {
                    // 9.1.6.1.	Get average price of secondhand instrument from ‘specify eliminate instrument list’
                    decimal decAverateAccqusitionCost = this.GetAveragePriceforInstalledInstrument(groupSecondHand);

                    // 9.1.6.2. Update secondhand instrument to Account DB
                    bool blnUpdate = this.UpdateAccountTransferSecondhandInstrument(groupSecondHand);

                    // 9.1.6.3.	If blnUpdate <> TRUE Then rollback
                    if (!blnUpdate)
                    {
                        return false;
                    } //end if

                    tbt_AccountStockMoving stockMoving = new tbt_AccountStockMoving();
                    stockMoving.SlipNo = strInventorySlipNo;
                    stockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START;
                    stockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTALLED;
                    stockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_ELIMINATE;
                    stockMoving.SourceLocationCode = groupSecondHand.SourceLocationCode;
                    stockMoving.DestinationLocationCode = groupSecondHand.DestinationLocationCode;
                    stockMoving.InstrumentCode = groupSecondHand.Instrumentcode;
                    stockMoving.InstrumentQty = groupSecondHand.TransferQty;
                    stockMoving.InstrumentPrice = decAverateAccqusitionCost;
                    stockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    stockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    stockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    stockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    listAccountStockMoving.Add(stockMoving);
                } //end foreach

                // 9.1.6.4.	Update transfer data to account moving stock
                this.InsertTbt_AccountStockMoving(listAccountStockMoving);

            } //end if 9.1

            #endregion

            #region 8. Update transfer instrument Remove data

            // 8.1 If doTbt_InventorySlipDetailForRemove[].Rows.Count > 0 Then
            if (invSlipDetailForRemoveList.Count > 0)
            {

                string strSourceLocationCode = null;
                string strDestinationLocationCode = null;
                string strTransferTypeCode = null;

                // 8.1.1 If blnBeforeOperate = TRUE Then
                if (blnBeforeOperate)
                {
                    strSourceLocationCode = InstrumentLocation.C_INV_LOC_UNOPERATED_WIP;
                    strDestinationLocationCode = InstrumentLocation.C_INV_LOC_WAITING_RETURN;
                    strTransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_BEFORE_START;
                }
                else
                {
                    strSourceLocationCode = InstrumentLocation.C_INV_LOC_USER;
                    strDestinationLocationCode = InstrumentLocation.C_INV_LOC_RETURN_WIP;
                    strTransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START;
                } //end if

                doRegisterTransferInstrumentData registerTransferInstrument = new doRegisterTransferInstrumentData();

                // 8.1.2 et doRegisterTransferInstrumentData.SlipId = C_INV_SLIPID_STOCKOUT
                registerTransferInstrument.SlipId = SlipID.C_INV_SLIPID_STOCK_OUT;

                // 8.1.3 Set data in doRegisterTransferInstrumentData.doTbt_InventorySlip
                registerTransferInstrument.InventorySlip = new tbt_InventorySlip();
                registerTransferInstrument.InventorySlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER;
                registerTransferInstrument.InventorySlip.TransferTypeCode = strTransferTypeCode;
                registerTransferInstrument.InventorySlip.InstallationSlipNo = strInstallationSlipNo;
                if (rentalContract.Count > 0)
                {
                    registerTransferInstrument.InventorySlip.ProjectCode = rentalContract[0].ProjectCode;
                }
                registerTransferInstrument.InventorySlip.ContractCode = strContractCode;
                registerTransferInstrument.InventorySlip.SlipIssueDate = DateTime.Now;
                //registerTransferInstrument.InventorySlip.StockInDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.StockOutDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.SourceLocationCode = strSourceLocationCode;
                registerTransferInstrument.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_RETURNED;
                if (headOffice.Count != 0)
                {
                    registerTransferInstrument.InventorySlip.SourceOfficeCode = headOffice[0].OfficeCode;
                    registerTransferInstrument.InventorySlip.DestinationOfficeCode = headOffice[0].OfficeCode;
                }
                registerTransferInstrument.InventorySlip.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                registerTransferInstrument.InventorySlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                registerTransferInstrument.InventorySlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                // 8.1.4 Set doRegisterTransferInstrumentData.doTbt_InventorySlipDetail[] = doTbt_InventorySlipDetailForUnRemovable[]
                registerTransferInstrument.lstTbt_InventorySlipDetail = invSlipDetailForRemoveList;

                // 8.1.5 Update inventory transfer data for installed 
                string strInventorySlipNo = this.RegisterTransferInstrument(registerTransferInstrument);

                // 8.1.6 Check there are new instrument
                int blnCheckNewInstrument = this.CheckNewInstrument(strInventorySlipNo);

                // 8.1.7 If blnCheckNewInstrument = 1 Then
                if (blnCheckNewInstrument == 1)
                {

                    // 8.1.8 Prepare data for update new instrument to account stock data
                    List<doGroupNewInstrument> groupNewList = this.GetGroupNewInstrument(strInventorySlipNo);
                    foreach (var groupNew in groupNewList)
                    {
                        groupNew.DestinationLocationCode = strDestinationLocationCode;

                        decimal decMovingAveragePrice = 0;

                        // 8.1.8.1.	Calculate moving average price
                        #region Monthly Price @ 2015
                        //decMovingAveragePrice = (decimal)this.CalculateMovingAveragePrice(groupNew);
                        decMovingAveragePrice = (decimal)this.GetMonthlyAveragePrice(groupNew.Instrumentcode, registerTransferInstrument.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                        #endregion

                        // 8.1.8.2. Update transfer data to Account DB
                        bool blnUpdate = this.UpdateAccountTransferNewInstrument(groupNew, decMovingAveragePrice);
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if

                        #region // New WIP concept @ 24-Feb-2015
                        //// 8.1.8.3. Update transfer data to account moving stock
                        //List<tbt_AccountInprocess> accountInProcessList = this.GetTbt_AccountInProcess(
                        //                                                    groupNew.SourceLocationCode,
                        //                                                    groupNew.ContractCode,
                        //                                                    groupNew.Instrumentcode);

                        //tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                        //accountStockMoving.SlipNo = strInventorySlipNo;
                        //accountStockMoving.TransferTypeCode = strTransferTypeCode;
                        //accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                        //accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                        //accountStockMoving.SourceLocationCode = groupNew.SourceLocationCode;
                        //accountStockMoving.DestinationLocationCode = groupNew.DestinationLocationCode;
                        //accountStockMoving.InstrumentCode = groupNew.Instrumentcode;
                        //accountStockMoving.InstrumentQty = groupNew.TransferQty;
                        //if (accountInProcessList.Count != 0)
                        //{
                        //    accountStockMoving.InstrumentPrice = accountInProcessList[0].MovingAveragePrice;
                        //}
                        //accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                        //targetAccountStockMovingList.Add(accountStockMoving);
                        //List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                        //if (resultAccountStockMovingList.Count <= 0)
                        //{
                        //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                        //} //end if/else 
                        #endregion
                    } //end foreach
                } //end if 8.1.7

                // 8.1.9 Check there are secondhand instrument
                int blnCheckSecondhandInstrument = this.CheckSecondhandInstrument(strInventorySlipNo);
                if (blnCheckSecondhandInstrument == 1)
                {

                    // 8.1.10 Prepare data for update secondhand instrument to account stock data
                    List<doGroupSecondhandInstrument> groupSecondHandList = this.GetGroupSecondhandInstrument(strInventorySlipNo);

                    // 8.1.11 Loop In doGroupSecondhandInstrument[]
                    foreach (var groupSecondHand in groupSecondHandList)
                    {
                        groupSecondHand.DestinationLocationCode = strDestinationLocationCode;

                        // 8.1.11.1. Update secondhand instrument to Account DB
                        bool blnUpdate = this.UpdateAccountTransferSecondhandInstrument(groupSecondHand);
                        // 8.1.11.2. If blnUpdate <> TRUE Then rollback
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if
                    } //end foreach
                } //end if 8.1.9

                // 8.1.12 Check there are sample instrument
                int blnCheckSampleInstrument = this.CheckSampleInstrument(strInventorySlipNo);

                // 8.1.13 If blnCheckSampleInstrument = 1 Then
                if (blnCheckSampleInstrument == 1)
                {

                    // 8.1.14 Prepare data for update sample instrument to account stock data
                    List<doGroupSampleInstrument> groupSampleList = this.GetGroupSampleInstrument(strInventorySlipNo);

                    // 8.1.15 Loop In doGroupSampleInstrument[]
                    foreach (var groupSample in groupSampleList)
                    {
                        groupSample.DestinationLocationCode = strDestinationLocationCode;

                        // 8.1.15.1. Update transfer data to Account DB
                        bool blnUpdate = this.UpdateAccountTransferSampleInstrument(groupSample, null);
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if

                        #region // New WIP concept @ 24-Feb-2015
                        //// 8.1.15.2. If blnBeforeOperate = FALSE Update transfer data to account moving stock
                        //if (blnBeforeOperate)
                        //{

                        //    // 8.1.15.2.2. Call InventoryHandler.InsertAccountStockMoving
                        //    tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                        //    accountStockMoving.SlipNo = strInventorySlipNo;
                        //    accountStockMoving.TransferTypeCode = strTransferTypeCode;
                        //    accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                        //    accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                        //    accountStockMoving.SourceLocationCode = groupSample.SourceLocationCode;
                        //    accountStockMoving.DestinationLocationCode = groupSample.DestinationLocationCode;
                        //    accountStockMoving.InstrumentCode = groupSample.Instrumentcode;
                        //    accountStockMoving.InstrumentQty = groupSample.TransferQty;
                        //    accountStockMoving.InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                        //    accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //    accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //    accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //    accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //    List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                        //    targetAccountStockMovingList.Add(accountStockMoving);
                        //    List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                        //    if (resultAccountStockMovingList.Count <= 0)
                        //    {
                        //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                        //    } //end if/else
                        //} //end if 
                        #endregion
                    } //end foreach
                } //end if 8.1.13
            } //end if 8.1

            #endregion

            return true;
        } //end UpdateCompleteInstallation

        public bool UpdateContractStartService(string strContractCode)
        {

            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;

            // 1. Get Headoffice data
            List<doOffice> headOffice = this.GetInventoryHeadOffice();

            // 2. Get instrument detail
            List<doContractUnoperatedInstrument> contractUnoperatedList = this.GetContractUnoperatedInstrument(strContractCode);

            var rentalHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            List<tbt_RentalContractBasic> rentalContract = rentalHand.GetTbt_RentalContractBasic(strContractCode, null);

            var installHand = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            var installhist = installHand.GetTbt_InstallationHistory(strContractCode, null, null);
            string latestinstallslipno = (
                from d in installhist
                orderby d.HistoryNo descending
                select d.SlipNo
            ).FirstOrDefault();

            // 3. Update transfer instrument data
            doRegisterTransferInstrumentData registerTransferInstrument = new doRegisterTransferInstrumentData();

            // 3.1 Set doRegisterTransferInstrumentData.SlipId = C_INV_SLIPID_STOCKOUT
            registerTransferInstrument.SlipId = SlipID.C_INV_SLIPID_STOCK_OUT;

            // 3.2 Set data in doRegisterTransferInstrumentData.doTbt_InventorySlip
            registerTransferInstrument.InventorySlip = new tbt_InventorySlip();
            registerTransferInstrument.InventorySlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
            registerTransferInstrument.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_START_SERVICE;
            registerTransferInstrument.InventorySlip.ProjectCode = (rentalContract.Count > 0 ? rentalContract[0].ProjectCode : null);
            registerTransferInstrument.InventorySlip.ContractCode = strContractCode;
            registerTransferInstrument.InventorySlip.SlipIssueDate = DateTime.Now;
            registerTransferInstrument.InventorySlip.StockInDate = DateTime.Now;
            registerTransferInstrument.InventorySlip.StockOutDate = DateTime.Now;
            registerTransferInstrument.InventorySlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_UNOPERATED_WIP;
            registerTransferInstrument.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_USER;
            if (headOffice.Count != 0)
            {
                registerTransferInstrument.InventorySlip.SourceOfficeCode = headOffice[0].OfficeCode;
                registerTransferInstrument.InventorySlip.DestinationOfficeCode = headOffice[0].OfficeCode;
            }
            registerTransferInstrument.InventorySlip.ContractStartServiceFlag = FlagType.C_FLAG_ON;
            registerTransferInstrument.InventorySlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            registerTransferInstrument.InventorySlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            registerTransferInstrument.InventorySlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            registerTransferInstrument.InventorySlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            registerTransferInstrument.InventorySlip.InstallationSlipNo = latestinstallslipno;

            // 3.3 Set data in doRegisterTransferInstrumentData.doTbt_InventorySlipDetial[] by Loop in doContractWIPInstrument[]
            registerTransferInstrument.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
            tbt_InventorySlipDetail inventorySlipDetail;
            foreach (var contractUnoperated in contractUnoperatedList)
            {
                inventorySlipDetail = new tbt_InventorySlipDetail();
                inventorySlipDetail.InstrumentCode = contractUnoperated.InstrumentCode;
                inventorySlipDetail.SourceAreaCode = contractUnoperated.AreaCode;
                inventorySlipDetail.DestinationAreaCode = InstrumentArea.C_INV_AREA_SE_RENTAL;
                inventorySlipDetail.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                inventorySlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                inventorySlipDetail.TransferQty = contractUnoperated.InstrumentQty;
                registerTransferInstrument.lstTbt_InventorySlipDetail.Add(inventorySlipDetail);
            } //end foreach


            // 3.4 Update inventory transfer data for installed 
            string strInventorySlipNo = this.RegisterTransferInstrument(registerTransferInstrument);

            // 3.5	Check there are new instrument
            int blnCheckNewInstrument = this.CheckNewInstrument(strInventorySlipNo);
            if (blnCheckNewInstrument == 1)
            {

                // 3.5.1 Prepare data for update new instrument to account stock data
                List<doGroupNewInstrument> groupNewList = this.GetGroupNewInstrument(strInventorySlipNo);

                // 3.5.2 Loop In doGroupNewInstrument[]
                foreach (var groupNew in groupNewList)
                {

                    // 3.5.2.1. Get source moving average price
                    List<tbt_AccountInprocess> accountInProcessList = this.GetTbt_AccountInProcess(
                                                                        groupNew.SourceLocationCode,
                                                                        groupNew.ContractCode,
                                                                        groupNew.Instrumentcode);

                    // 3.5.2.2.	Get lot no
                    doInsertDepreciationData insertDepreciation = new doInsertDepreciationData();

                    insertDepreciation.ContractCode = strContractCode;
                    insertDepreciation.InstrumentCode = groupNew.Instrumentcode;
                    insertDepreciation.StartYearMonth = DateTime.Now.ToString("yyyyMM");
                    if (accountInProcessList.Count != 0)
                    {
                        insertDepreciation.MovingAveragePrice = accountInProcessList[0].MovingAveragePrice.Value;
                    }
                    string strLotNo = this.InsertDepreciationData(insertDepreciation);

                    // 3.5.2.3. Set doGroupNewInstrument[i].LotNo = strLotNo
                    groupNew.LotNo = strLotNo;

                    // 3.5.2.4. Calculate moving average price
                    #region Monthly Price @ 2015
                    //decimal decMovingAveragePrice = (decimal)this.CalculateMovingAveragePrice(groupNew);
                    decimal decMovingAveragePrice = (decimal)this.GetMonthlyAveragePrice(groupNew.Instrumentcode, registerTransferInstrument.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                    #endregion

                    // 3.5.2.5. Update transfer data to Account DB
                    bool blnUpdate = this.UpdateAccountTransferNewInstrument(groupNew, decMovingAveragePrice);
                    if (!blnUpdate)
                    {
                        return false;
                    } //end if

                    // 3.5.2.6. Update transfer data to account moving stock
                    tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                    accountStockMoving.SlipNo = strInventorySlipNo;
                    accountStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_START_SERVICE;
                    accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                    accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTALLED;
                    accountStockMoving.SourceLocationCode = groupNew.SourceLocationCode;
                    accountStockMoving.DestinationLocationCode = groupNew.DestinationLocationCode;
                    accountStockMoving.InstrumentCode = groupNew.Instrumentcode;
                    accountStockMoving.InstrumentQty = groupNew.TransferQty;
                    if (accountInProcessList.Count != 0)
                    {
                        accountStockMoving.InstrumentPrice = accountInProcessList[0].MovingAveragePrice.Value;
                    }
                    accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                    targetAccountStockMovingList.Add(accountStockMoving);
                    List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                    if (resultAccountStockMovingList.Count <= 0)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                    } //end if/else
                } //end foreach
            } //end if 3.5

            // 3.6 Check there are secondhand instrument
            int blnCheckSecondhandInstrument = this.CheckSecondhandInstrument(strInventorySlipNo);
            if (blnCheckSecondhandInstrument == 1)
            {

                // 3.6.1 Prepare data for update secondhand instrument to account stock data
                List<doGroupSecondhandInstrument> groupSecondHandList = this.GetGroupSecondhandInstrument(strInventorySlipNo);
                foreach (var groupSecondHand in groupSecondHandList)
                {

                    // 3.6.1.1. Update secondhand instrument to Account DB
                    bool blnUpdate = this.UpdateAccountTransferSecondhandInstrument(groupSecondHand);

                    // 3.6.1.2.	If blnUpdate <> TRUE Then rollback
                    if (!blnUpdate)
                    {
                        return false;
                    } //end if
                } //end foreach
            } //end if 3.6

            // 3.7 Check there are sample instrument
            int blnCheckSampleInstrument = this.CheckSampleInstrument(strInventorySlipNo);
            if (blnCheckSampleInstrument == 1)
            {

                // 3.7.1 Prepare data for update sample instrument to account stock data
                List<doGroupSampleInstrument> groupSampleList = this.GetGroupSampleInstrument(strInventorySlipNo);
                foreach (var groupSample in groupSampleList)
                {

                    // 3.7.1.1. Get lot no
                    doInsertDepreciationData insertDepreciation = new doInsertDepreciationData();
                    insertDepreciation.ContractCode = strContractCode;
                    insertDepreciation.InstrumentCode = groupSample.Instrumentcode;
                    insertDepreciation.StartYearMonth = DateTime.Now.ToString("yyyyMM");
                    insertDepreciation.MovingAveragePrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                    string strLotNo = this.InsertDepreciationData(insertDepreciation);

                    // 3.7.1.2. Set doGroupSampleInstrument[i].LotNo = strLotNo
                    //groupSample.LotNo = strLotNo;

                    // 3.7.1.3. Calculate moving average price
                    doGroupNewInstrument groupNew = new doGroupNewInstrument();
                    groupNew.SourceOfficeCode = groupSample.SourceOfficeCode;
                    groupNew.DestinationOfficeCode = groupSample.DestinationOfficeCode;
                    groupNew.SourceLocationCode = groupSample.SourceLocationCode;
                    groupNew.DestinationLocationCode = groupSample.DestinationLocationCode;
                    groupNew.ProjectCode = groupSample.ProjectCode;
                    groupNew.ContractCode = groupSample.ContractCode;
                    groupNew.Instrumentcode = groupSample.Instrumentcode;
                    groupNew.TransferQty = groupSample.TransferQty;
                    groupNew.ObjectID = InstrumentArea.C_INV_AREA_NEW_SAMPLE;
                    groupNew.LotNo = strLotNo;
                    #region Monthly Price @ 2015
                    //decimal decMovingAveragePrice = (decimal)this.CalculateMovingAveragePrice(groupNew);
                    decimal decMovingAveragePrice = this.GetMonthlyAveragePrice(groupNew.Instrumentcode, registerTransferInstrument.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                    #endregion

                    groupSample.LotNo = strLotNo;
                    // 3.7.1.4. Update sample instrument to Account DB
                    bool blnUpdate = this.UpdateAccountTransferSampleInstrument(groupSample, decMovingAveragePrice);
                    if (!blnUpdate)
                    {
                        return false;
                    } //end if

                    // 3.7.1.5. Update transfer data to account moving stock
                    tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                    accountStockMoving.SlipNo = strInventorySlipNo;
                    accountStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_START_SERVICE;
                    accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                    accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTALLED;
                    accountStockMoving.SourceLocationCode = groupSample.SourceLocationCode;
                    accountStockMoving.DestinationLocationCode = groupSample.DestinationLocationCode;
                    accountStockMoving.InstrumentCode = groupSample.Instrumentcode;
                    accountStockMoving.InstrumentQty = groupSample.TransferQty;
                    accountStockMoving.InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                    accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                    targetAccountStockMovingList.Add(accountStockMoving);
                    List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                    if (resultAccountStockMovingList.Count <= 0)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                    } //end if/else
                } //end foreach
            } //end if 3.7

            return true;
        } //end UpdateContractStartService

        public bool UpdateCompleteProject(string strProjectCode)
        {

            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;

            // 1. Get Headoffice data
            List<doOffice> headOffice = this.GetInventoryHeadOffice();

            // 2. Get instrument detail
            List<tbt_InventoryProjectWIP> inventoryProjectList = base.GetProjectWIPInstrument(strProjectCode);

            // Added by Non A. 23/Jul/2012 : Requested by SA.
            // In-case project hasn't do any stock-out, pass-through this method without any error.
            if (inventoryProjectList == null || inventoryProjectList.Count <= 0)
            {
                return true;
            }

            // 3. Update transfer instrument data

            doRegisterTransferInstrumentData registerTransferInstrument = new doRegisterTransferInstrumentData();

            // 3.1 Set doRegisterTransferInstrumentData.SlipId = C_INV_SLIPID_STOCKOUT
            registerTransferInstrument.SlipId = SlipID.C_INV_SLIPID_STOCK_OUT;

            // 3.2	Set data in doRegisterTransferInstrumentData.doTbt_InventorySlip
            registerTransferInstrument.InventorySlip = new tbt_InventorySlip();
            registerTransferInstrument.InventorySlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER;
            registerTransferInstrument.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_PROJECT_COMPLETE;
            registerTransferInstrument.InventorySlip.ProjectCode = strProjectCode;
            registerTransferInstrument.InventorySlip.SlipIssueDate = DateTime.Now;
            //registerTransferInstrument.InventorySlip.StockInDate = DateTime.Now;
            registerTransferInstrument.InventorySlip.StockOutDate = DateTime.Now;
            registerTransferInstrument.InventorySlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_PROJECT_WIP;
            registerTransferInstrument.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_RETURNED;
            if (headOffice.Count != 0)
            {
                registerTransferInstrument.InventorySlip.SourceOfficeCode = headOffice[0].OfficeCode;
                registerTransferInstrument.InventorySlip.DestinationOfficeCode = headOffice[0].OfficeCode;
            }
            registerTransferInstrument.InventorySlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            registerTransferInstrument.InventorySlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            registerTransferInstrument.InventorySlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            registerTransferInstrument.InventorySlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

            // 3.3 Set data in doRegisterTransferInstrumentData.doTbt_InventorySlipDetial[] by Loop in doTbt_InventoryProjectWIP[]
            registerTransferInstrument.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
            tbt_InventorySlipDetail inventorySlipDetail;
            foreach (var inventoryProject in inventoryProjectList)
            {
                inventorySlipDetail = new tbt_InventorySlipDetail();
                inventorySlipDetail.InstrumentCode = inventoryProject.InstrumentCode;
                inventorySlipDetail.SourceAreaCode = inventoryProject.AreaCode;
                inventorySlipDetail.DestinationAreaCode = inventoryProject.AreaCode;
                inventorySlipDetail.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                inventorySlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                inventorySlipDetail.TransferQty = inventoryProject.InstrumentQty;
                registerTransferInstrument.lstTbt_InventorySlipDetail.Add(inventorySlipDetail);
            } //end foreach

            // 3.4 Update inventory transfer data
            string strInventorySlipNo = this.RegisterTransferInstrument(registerTransferInstrument);

            // 3.5 Updae strock-out from project wip location
            foreach (var inventoryProject in inventoryProjectList)
            {

                // 3.5.1 Call InventoryHandler.UpdateTbt_InventoryProjectWIP
                List<tbt_InventoryProjectWIP> resultUpdate = this.UpdateTbt_InventoryProjectWIP(
                                                                inventoryProject.ProjectCode,
                                                                inventoryProject.AreaCode,
                                                                inventoryProject.InstrumentCode, -inventoryProject.InstrumentQty);

                // 3.5.2 If doTbt_InventoryProjectWIP.Rows.Count <= 0 Then rollback
                if (resultUpdate.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_PROJECT_WIP,
                        TableData = null
                    };
                    logData.TableData = CommonUtil.ConvertToXml(resultUpdate);
                    logHand.WriteTransactionLog(logData);
                }
                else
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_PROJECT_WIP });
                } //end if
            } //end foreach

            // 3.6 Check there are new instrument
            int blnCheckNewInstrument = this.CheckNewInstrument(strInventorySlipNo);
            if (blnCheckNewInstrument == 1)
            {

                // 3.6.1 Prepare data for update new instrument to account stock data
                List<doGroupNewInstrument> groupNewList = this.GetGroupNewInstrument(strInventorySlipNo);

                // 3.6.2 Loop In doGroupNewInstrument[]
                foreach (var groupNew in groupNewList)
                {

                    // 3.6.2.1. Call InventoryHandler.CalculateMovingAveragePrice
                    #region Monthly Price @ 2015
                    //decimal decMovingAveragePrice = (decimal)this.CalculateMovingAveragePrice(groupNew);
                    decimal decMovingAveragePrice = (decimal)this.GetMonthlyAveragePrice(groupNew.Instrumentcode, registerTransferInstrument.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                    #endregion

                    // 3.6.2.2. Call InventoryHandler.UpdateAccountTransferNewInstrument
                    bool blnUpdate = this.UpdateAccountTransferNewInstrument(groupNew, decMovingAveragePrice);

                    // 3.6.2.3.	If blnUpdate <> TRUE Then rollback
                    if (!blnUpdate)
                    {
                        return false;
                    } //end if

                    #region // New WIP concept @ 24-Feb-2015
                    //// 3.6.2.4. Update transfer data to account moving stock
                    //List<tbt_AccountInprocess> accountInProcessList = this.GetTbt_AccountInProcess(
                    //                                                groupNew.SourceLocationCode,
                    //                                                groupNew.ProjectCode,
                    //                                                groupNew.Instrumentcode);
                    //tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                    //accountStockMoving.SlipNo = strInventorySlipNo;
                    //accountStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_PROJECT_COMPLETE;
                    //accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                    //accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                    //accountStockMoving.SourceLocationCode = groupNew.SourceLocationCode;
                    //accountStockMoving.DestinationLocationCode = groupNew.DestinationLocationCode;
                    //accountStockMoving.InstrumentCode = groupNew.Instrumentcode;
                    //accountStockMoving.InstrumentQty = groupNew.TransferQty;
                    //if (accountInProcessList.Count != 0)
                    //{
                    //    accountStockMoving.InstrumentPrice = accountInProcessList[0].MovingAveragePrice.Value;
                    //}
                    //accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                    //targetAccountStockMovingList.Add(accountStockMoving);
                    //List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                    //if (resultAccountStockMovingList.Count <= 0)
                    //{
                    //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                    //} //end if/else 
                    #endregion
                } //end foreach
            } //end if 3.6

            // 3.7 Check there are secondhand instrument
            int blnCheckSecondhandInstrument = this.CheckSecondhandInstrument(strInventorySlipNo);
            if (blnCheckSecondhandInstrument == 1)
            {

                // 3.7.1 Prepare data for update secondhand instrument to account stock data
                List<doGroupSecondhandInstrument> groupSecondHandList = this.GetGroupSecondhandInstrument(strInventorySlipNo);
                foreach (var groupSecondHand in groupSecondHandList)
                {

                    // 3.7.1.1. Update secondhand instrument to Account DB
                    bool blnUpdate = this.UpdateAccountTransferSecondhandInstrument(groupSecondHand);

                    // 3.7.1.2. If blnUpdate <> TRUE Then rollback
                    if (!blnUpdate)
                    {
                        return false;
                    } //end if
                } //end foreach
            } //end if 3.7

            // 3.8 Check there are sample instrument
            int blnCheckSampleInstrument = this.CheckSampleInstrument(strInventorySlipNo);
            if (blnCheckSampleInstrument == 1)
            {

                // 3.8.1 Prepare data for update sample instrument to account stock data
                List<doGroupSampleInstrument> groupSampleList = this.GetGroupSampleInstrument(strInventorySlipNo);
                foreach (var groupSample in groupSampleList)
                {

                    // 3.8.1.1.	Update sample instrument to Account DB
                    bool blnUpdate = this.UpdateAccountTransferSampleInstrument(groupSample, null);

                    // 3.8.1.2.	If blnUpdate <> TRUE Then rollback
                    if (!blnUpdate)
                    {
                        return false;
                    } //end if

                    #region // New WIP concept @ 24-Feb-2015
                    //// 3.8.1.3. Update transfer data to account moving stock
                    //tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                    //accountStockMoving.SlipNo = strInventorySlipNo;
                    //accountStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_PROJECT_COMPLETE;
                    //accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                    //accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                    //accountStockMoving.SourceLocationCode = groupSample.SourceLocationCode;
                    //accountStockMoving.DestinationLocationCode = groupSample.DestinationLocationCode;
                    //accountStockMoving.InstrumentCode = groupSample.Instrumentcode;
                    //accountStockMoving.InstrumentQty = groupSample.TransferQty;
                    //accountStockMoving.InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                    //accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                    //targetAccountStockMovingList.Add(accountStockMoving);
                    //List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                    //if (resultAccountStockMovingList.Count <= 0)
                    //{
                    //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                    //} //end if/else 
                    #endregion
                } //end foreach
            } //end if 3.8

            var invDocSrv = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
            invDocSrv.GenerateIVR180FilePath(strInventorySlipNo,
                (headOffice != null && headOffice.Count > 0 ? headOffice[0].OfficeCode : null),
                CommonUtil.dsTransData.dtUserData.EmpNo,
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime
            );

            return true;
        } //end UpdateCompleteProject

        public bool UpdateCancelInstallation(string strContractCode, string strInstallationSlipNo, string strServiceTypeCode, bool isPartialOut = false)
        {

            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;


            // 1. Validate updated cancel installation
            // 1.1 Call InventoryHandler.CheckUpdatedCancelInstallation
            bool blnUpdatedCancelInstallation = this.CheckUpdatedCancelInstallation(strInstallationSlipNo);

            // 1.2 If blnUpdatedCancelInstallation = TRUE Then return false
            if (blnUpdatedCancelInstallation)
            {
                return false;
            } //end if

            //2. Get Headoffice data
            List<doOffice> headOffice = this.GetInventoryHeadOffice();

            // 3. Get instrument detail
            // 3.1
            List<doContractWIPInstrument> contractWIPList = this.GetContractWIPInstrument(strInstallationSlipNo);
            // 3.2
            List<doContractWIPInstrumentPartial> contractWIPListPartial = this.GetContractWIPInstrumentPartial(strInstallationSlipNo);

            // - Add by Nontawat L. on 09-Jul-2014 : Phase4: 3.44
            // 4. Get Installation Slip detail
            List<tbt_InstallationSlip> installationSlip = this.GetInstallationSlipDetail(strInstallationSlipNo);
            // End add

            if (contractWIPListPartial != null && contractWIPListPartial.Count > 0)
            {
                // 5. Update transfer instrument data
                doRegisterTransferInstrumentData registerTransferInstrument = new doRegisterTransferInstrumentData();

                // 5.1 Set doRegisterTransferInstrumentData.SlipId = C_INV_SLIPID_STOCKOUT
                registerTransferInstrument.SlipId = SlipID.C_INV_SLIPID_STOCK_OUT;

                // 5.2 Set data in doRegisterTransferInstrumentData.doTbt_InventorySlip
                registerTransferInstrument.InventorySlip = new tbt_InventorySlip();
                registerTransferInstrument.InventorySlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
                registerTransferInstrument.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CANCEL_INSTALLATION;
                registerTransferInstrument.InventorySlip.InstallationSlipNo = strInstallationSlipNo;
                registerTransferInstrument.InventorySlip.ContractCode = strContractCode;
                registerTransferInstrument.InventorySlip.SlipIssueDate = DateTime.Now;
                //registerTransferInstrument.InventorySlip.StockInDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.StockOutDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_PARTIAL_OUT;
                registerTransferInstrument.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_WIP;
                if (headOffice.Count != 0)
                {
                    registerTransferInstrument.InventorySlip.SourceOfficeCode = headOffice[0].OfficeCode;
                    registerTransferInstrument.InventorySlip.DestinationOfficeCode = headOffice[0].OfficeCode;
                }
                registerTransferInstrument.InventorySlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                registerTransferInstrument.InventorySlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                // 5.3	Set data in doRegisterTransferInstrumentData.doTbt_InventorySlipDetial[] by Loop in doContractWIPInstrument[]
                registerTransferInstrument.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
                tbt_InventorySlipDetail inventorySlipDetail;
                foreach (var contractWIPPartial in contractWIPListPartial)
                {
                    inventorySlipDetail = new tbt_InventorySlipDetail();
                    inventorySlipDetail.InstrumentCode = contractWIPPartial.InstrumentCode;
                    inventorySlipDetail.SourceAreaCode = contractWIPPartial.AreaCode;
                    inventorySlipDetail.DestinationAreaCode = contractWIPPartial.AreaCode;
                    inventorySlipDetail.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    inventorySlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    inventorySlipDetail.TransferQty = contractWIPPartial.InstrumentQty;
                    inventorySlipDetail.NotInstalledQty = contractWIPPartial.InstrumentQty;
                    registerTransferInstrument.lstTbt_InventorySlipDetail.Add(inventorySlipDetail);
                } //end foreach

                // 5.4 Update inventory transfer data
                string strInventorySlipNo = this.RegisterTransferInstrument(registerTransferInstrument);

                // 5.5 Check there are new instrument
                int blnCheckNewInstrument = this.CheckNewInstrument(strInventorySlipNo);
                if (blnCheckNewInstrument == 1)
                {

                    // 5.5.1 Prepare data for update new instrument to account stock data
                    List<doGroupNewInstrument> groupNewList = this.GetGroupNewInstrument(strInventorySlipNo);

                    // 5.5.2 Loop In doGroupNewInstrument[]
                    foreach (var groupNew in groupNewList)
                    {
                        // 5.5.2.1. Call InventoryHandler.CalculateMovingAveragePrice
                        #region Monthly Price @ 2015
                        //decimal decMovingAveragePrice = (decimal)this.CalculateMovingAveragePrice(groupNew);
                        decimal decMovingAveragePrice = (decimal)this.GetMonthlyAveragePrice(groupNew.Instrumentcode, registerTransferInstrument.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                        #endregion

                        // 5.5.2.2. Call InventoryHandler.UpdateAccountTransferNewInstrument
                        bool blnUpdate = this.UpdateAccountTransferNewInstrument(groupNew, decMovingAveragePrice);

                        // 5.5.2.3.	If blnUpdate <> TRUE Then rollback
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if

                        #region // New WIP concept @ 24-Feb-2015
                        //// 5.5.2.4. Update transfer data to account moving stock
                        //List<tbt_AccountInprocess> accountInProcessList = this.GetTbt_AccountInProcess(
                        //                                                    groupNew.SourceLocationCode,
                        //                                                    groupNew.ContractCode,
                        //                                                    groupNew.Instrumentcode);

                        //tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                        //accountStockMoving.SlipNo = strInventorySlipNo;
                        //accountStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CANCEL_INSTALLATION;
                        //accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                        //accountStockMoving.DestinationLocationCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                        //accountStockMoving.SourceLocationCode = groupNew.SourceLocationCode;
                        //accountStockMoving.DestinationLocationCode = groupNew.DestinationLocationCode;
                        //accountStockMoving.InstrumentCode = groupNew.Instrumentcode;
                        //accountStockMoving.InstrumentQty = groupNew.TransferQty;
                        //if (accountInProcessList.Count != 0)
                        //{
                        //    accountStockMoving.InstrumentPrice = accountInProcessList[0].MovingAveragePrice;
                        //}
                        //accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                        //targetAccountStockMovingList.Add(accountStockMoving);
                        //List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                        //if (resultAccountStockMovingList.Count <= 0)
                        //{
                        //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                        //} //end if/else
                        #endregion
                    } //end foreach
                } //end if 5.5

                // 5.6 Check there are secondhand instrument
                int blnCheckSecondhandInstrument = this.CheckSecondhandInstrument(strInventorySlipNo);
                if (blnCheckSecondhandInstrument == 1)
                {

                    // 5.6.1 Prepare data for update secondhand instrument to account stock data
                    List<doGroupSecondhandInstrument> groupSecondHandList = this.GetGroupSecondhandInstrument(strInventorySlipNo);
                    foreach (var groupSecondHand in groupSecondHandList)
                    {

                        // 5.6.1.1. Update secondhand instrument to Account DB
                        bool blnUpdate = this.UpdateAccountTransferSecondhandInstrument(groupSecondHand);

                        // 5.6.1.2. If blnUpdate <> TRUE Then rollback
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if
                    } //end foreach
                } //end if 5.6

                // 5.7	Check there are sample instrument
                int blnCheckSampleInstrument = this.CheckSampleInstrument(strInventorySlipNo);
                if (blnCheckSampleInstrument == 1)
                {

                    // 5.7.1 Prepare data for update sample instrument to account stock data
                    List<doGroupSampleInstrument> groupSampleList = this.GetGroupSampleInstrument(strInventorySlipNo);
                    foreach (var groupSample in groupSampleList)
                    {

                        // 5.7.1.1. Update sample instrument to Account DB
                        bool blnUpdate = this.UpdateAccountTransferSampleInstrument(groupSample, null);

                        // 5.7.1.2. If blnUpdate <> TRUE Then rollback
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if

                        #region // New WIP concept @ 24-Feb-2015
                        //// 5.7.1.3. Update transfer data to account moving stock
                        //tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                        //accountStockMoving.SlipNo = strInventorySlipNo;
                        //accountStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CANCEL_INSTALLATION;
                        //accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                        //accountStockMoving.DestinationLocationCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                        //accountStockMoving.SourceLocationCode = groupSample.SourceLocationCode;
                        //accountStockMoving.DestinationLocationCode = groupSample.DestinationLocationCode;
                        //accountStockMoving.InstrumentCode = groupSample.Instrumentcode;
                        //accountStockMoving.InstrumentQty = groupSample.TransferQty;
                        //accountStockMoving.InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                        //accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                        //targetAccountStockMovingList.Add(accountStockMoving);
                        //List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                        //if (resultAccountStockMovingList.Count <= 0)
                        //{
                        //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                        //} //end if/else 
                        #endregion

                    } //end foreach
                } //end if 5.7
            }
            
            if (contractWIPList != null && contractWIPList.Count > 0)
            {
                // 6. Update transfer instrument data
                doRegisterTransferInstrumentData registerTransferInstrument = new doRegisterTransferInstrumentData();

                // 6.1 Set doRegisterTransferInstrumentData.SlipId = C_INV_SLIPID_STOCKOUT
                registerTransferInstrument.SlipId = SlipID.C_INV_SLIPID_STOCK_OUT;

                // 6.2 Set data in doRegisterTransferInstrumentData.doTbt_InventorySlip
                registerTransferInstrument.InventorySlip = new tbt_InventorySlip();
                registerTransferInstrument.InventorySlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER;
                registerTransferInstrument.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CANCEL_INSTALLATION;
                registerTransferInstrument.InventorySlip.InstallationSlipNo = strInstallationSlipNo;
                registerTransferInstrument.InventorySlip.ContractCode = strContractCode;
                registerTransferInstrument.InventorySlip.SlipIssueDate = DateTime.Now;
                //registerTransferInstrument.InventorySlip.StockInDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.StockOutDate = DateTime.Now;
                // - Modified by Nontawat L. on 09-Jul-2014 : Phase4: 3.44
                registerTransferInstrument.InventorySlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_WIP;
                registerTransferInstrument.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_RETURNED;
                if (headOffice.Count != 0)
                {
                    registerTransferInstrument.InventorySlip.SourceOfficeCode = headOffice[0].OfficeCode;
                    registerTransferInstrument.InventorySlip.DestinationOfficeCode = headOffice[0].OfficeCode;
                }
                registerTransferInstrument.InventorySlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                registerTransferInstrument.InventorySlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                // 6.3	Set data in doRegisterTransferInstrumentData.doTbt_InventorySlipDetial[] by Loop in doContractWIPInstrument[]
                registerTransferInstrument.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
                tbt_InventorySlipDetail inventorySlipDetail;
                foreach (var contractWIP in contractWIPList)
                {
                    inventorySlipDetail = new tbt_InventorySlipDetail();
                    inventorySlipDetail.InstrumentCode = contractWIP.InstrumentCode;
                    inventorySlipDetail.SourceAreaCode = contractWIP.AreaCode;
                    inventorySlipDetail.DestinationAreaCode = contractWIP.AreaCode;
                    inventorySlipDetail.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    inventorySlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    inventorySlipDetail.TransferQty = contractWIP.InstrumentQty;
                    inventorySlipDetail.NotInstalledQty = contractWIP.InstrumentQty;
                    registerTransferInstrument.lstTbt_InventorySlipDetail.Add(inventorySlipDetail);
                } //end foreach

                // 6.4 Update inventory transfer data
                string strInventorySlipNo = this.RegisterTransferInstrument(registerTransferInstrument);

                // 6.5 Check there are new instrument
                int blnCheckNewInstrument = this.CheckNewInstrument(strInventorySlipNo);
                if (blnCheckNewInstrument == 1)
                {

                    // 6.5.1 Prepare data for update new instrument to account stock data
                    List<doGroupNewInstrument> groupNewList = this.GetGroupNewInstrument(strInventorySlipNo);

                    // 6.5.2 Loop In doGroupNewInstrument[]
                    foreach (var groupNew in groupNewList)
                    {
                        // 6.5.2.1. Call InventoryHandler.CalculateMovingAveragePrice
                        #region Monthly Price @ 2015
                        //decimal decMovingAveragePrice = (decimal)this.CalculateMovingAveragePrice(groupNew);
                        decimal decMovingAveragePrice = (decimal)this.GetMonthlyAveragePrice(groupNew.Instrumentcode, registerTransferInstrument.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                        #endregion

                        // 6.5.2.2. Call InventoryHandler.UpdateAccountTransferNewInstrument
                        bool blnUpdate = this.UpdateAccountTransferNewInstrument(groupNew, decMovingAveragePrice);

                        // 6.5.2.3.	If blnUpdate <> TRUE Then rollback
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if

                        #region // New WIP concept @ 24-Feb-2015
                        //// 6.5.2.4. Update transfer data to account moving stock
                        //List<tbt_AccountInprocess> accountInProcessList = this.GetTbt_AccountInProcess(
                        //                                                    groupNew.SourceLocationCode,
                        //                                                    groupNew.ContractCode,
                        //                                                    groupNew.Instrumentcode);

                        //tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                        //accountStockMoving.SlipNo = strInventorySlipNo;
                        //accountStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CANCEL_INSTALLATION;
                        //accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                        //accountStockMoving.DestinationLocationCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                        //accountStockMoving.SourceLocationCode = groupNew.SourceLocationCode;
                        //accountStockMoving.DestinationLocationCode = groupNew.DestinationLocationCode;
                        //accountStockMoving.InstrumentCode = groupNew.Instrumentcode;
                        //accountStockMoving.InstrumentQty = groupNew.TransferQty;
                        //if (accountInProcessList.Count != 0)
                        //{
                        //    accountStockMoving.InstrumentPrice = accountInProcessList[0].MovingAveragePrice;
                        //}
                        //accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                        //targetAccountStockMovingList.Add(accountStockMoving);
                        //List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                        //if (resultAccountStockMovingList.Count <= 0)
                        //{
                        //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                        //} //end if/else
                        #endregion
                    } //end foreach
                } //end if 6.5

                // 6.6 Check there are secondhand instrument
                int blnCheckSecondhandInstrument = this.CheckSecondhandInstrument(strInventorySlipNo);
                if (blnCheckSecondhandInstrument == 1)
                {

                    // 6.6.1 Prepare data for update secondhand instrument to account stock data
                    List<doGroupSecondhandInstrument> groupSecondHandList = this.GetGroupSecondhandInstrument(strInventorySlipNo);
                    foreach (var groupSecondHand in groupSecondHandList)
                    {

                        // 6.6.1.1. Update secondhand instrument to Account DB
                        bool blnUpdate = this.UpdateAccountTransferSecondhandInstrument(groupSecondHand);

                        // 6.6.1.2. If blnUpdate <> TRUE Then rollback
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if
                    } //end foreach
                } //end if 6.6

                // 6.7	Check there are sample instrument
                int blnCheckSampleInstrument = this.CheckSampleInstrument(strInventorySlipNo);
                if (blnCheckSampleInstrument == 1)
                {

                    // 6.7.1 Prepare data for update sample instrument to account stock data
                    List<doGroupSampleInstrument> groupSampleList = this.GetGroupSampleInstrument(strInventorySlipNo);
                    foreach (var groupSample in groupSampleList)
                    {

                        // 6.7.1.1. Update sample instrument to Account DB
                        bool blnUpdate = this.UpdateAccountTransferSampleInstrument(groupSample, null);

                        // 6.7.1.2. If blnUpdate <> TRUE Then rollback
                        if (!blnUpdate)
                        {
                            return false;
                        } //end if

                        #region // New WIP concept @ 24-Feb-2015
                        //// 6.7.1.3. Update transfer data to account moving stock
                        //tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                        //accountStockMoving.SlipNo = strInventorySlipNo;
                        //accountStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CANCEL_INSTALLATION;
                        //accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                        //accountStockMoving.DestinationLocationCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                        //accountStockMoving.SourceLocationCode = groupSample.SourceLocationCode;
                        //accountStockMoving.DestinationLocationCode = groupSample.DestinationLocationCode;
                        //accountStockMoving.InstrumentCode = groupSample.Instrumentcode;
                        //accountStockMoving.InstrumentQty = groupSample.TransferQty;
                        //accountStockMoving.InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                        //accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                        //targetAccountStockMovingList.Add(accountStockMoving);
                        //List<tbt_AccountStockMoving> resultAccountStockMovingList = this.InsertAccountStockMoving(targetAccountStockMovingList);
                        //if (resultAccountStockMovingList.Count <= 0)
                        //{
                        //    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                        //} //end if/else 
                        #endregion

                    } //end foreach
                } //end if 6.7
            }

            //7.	Update Slip status Partial -> Complete
            if (contractWIPListPartial != null && contractWIPListPartial.Count > 0)
            {
                this.UpdatePartialToCompleteStatus(strContractCode);
            }

            return true;
        } //end UpdateCancelInstallation

        public bool UpdateCustomerAcceptance(string strContractCode, string strOCC, DateTime? dtpAcceptanceDate)
        {

            ISaleContractHandler srvSale = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IInstallationHandler srvInstall = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;

            try
            {
                // 1. Validate updated customer acceptance
                // 1.1 Call InventoryHandler.CheckUpdatedUserAcceptance
                bool blnUpdatedUserAcceptance = this.CheckUpdatedUserAcceptance(strContractCode, dtpAcceptanceDate);

                // 1.2 If blnUpdateUserAcceptance = TRUE Then return false
                //if (blnUpdatedUserAcceptance)
                //{
                //    return false;
                //} //end if

                //2. Get Headoffice data
                List<doOffice> headOffice = this.GetInventoryHeadOffice();

                // 3. Get Contract data
                List<tbt_SaleBasic> lstSaleBasic = srvSale.GetTbt_SaleBasic(strContractCode, strOCC, null);
                var lstHeadOffice = this.GetInventoryHeadOffice();

                var tempVar = new
                {
                    InventoryHeadOfficeCode = (lstHeadOffice != null && lstHeadOffice.Count > 0 ? lstHeadOffice[0].OfficeCode : null),
                    ProjectCode = (lstSaleBasic != null && lstSaleBasic.Count > 0 ? lstSaleBasic[0].ProjectCode : null)
                };

                // 4. Get instrument detail
                List<doSaleInstrument> lstSaleInstruments = this.GetSaleInstrument(strContractCode, strOCC);

                string strInstallationSlipNo = srvInstall.GetInstallationSlipNoForAcceptant(strContractCode, strOCC);
                if (string.IsNullOrEmpty(strInstallationSlipNo))
                {
                    throw new ApplicationException("Unable to get installation slip no.");
                }

                var lstInstallDtl = srvInstall.GetInstallationDetailForCompleteInstallation(strInstallationSlipNo);

                List<tbt_InventorySlipDetail> lstInvSlpDtlForInstalled = new List<tbt_InventorySlipDetail>();
                List<tbt_InventorySlipDetail> lstInvSlpDtlForNotInstalled = new List<tbt_InventorySlipDetail>();

                using (TransactionScope scope = new TransactionScope())
                 {
                #region 6.	Insert transfer data to data object

                foreach (var objInstallDtl in lstInstallDtl)
                    {
                        var intNotInstalledQty = objInstallDtl.NotInstalledQty.GetValueOrDefault(0) + objInstallDtl.ReturnQty.GetValueOrDefault(0);
                        var intInstalledQty = objInstallDtl.TotalStockOutQty.GetValueOrDefault(0) - intNotInstalledQty;

                        //6.3
                        var qSaleInstrument = lstSaleInstruments.Where(i => i.InstrumentCode == objInstallDtl.InstrumentCode);

                        //6.4	Insert to installed instrument instrument to data object
                        if (intInstalledQty > 0)
                        {
                            foreach (var objSaleInst in qSaleInstrument.OrderBy(i => i.AreaCode))
                            {
                                if (intInstalledQty <= 0)
                                {
                                    break;
                                }

                                //6.4.1.2
                                int intTransferQty = 0;
                                if (objSaleInst.InstrumentQty >= intInstalledQty)
                                {
                                    intTransferQty = intInstalledQty;
                                }
                                else
                                {
                                    intTransferQty = objSaleInst.InstrumentQty.GetValueOrDefault(0);
                                }
                                objSaleInst.InstrumentQty -= intTransferQty;

                                //6.4.1.3
                                intInstalledQty -= intTransferQty;

                                //6.4.1.4
                                lstInvSlpDtlForInstalled.Add(new tbt_InventorySlipDetail()
                                {
                                    SlipNo = null,
                                    RunningNo = lstInvSlpDtlForInstalled.Count + 1,
                                    InstrumentCode = objSaleInst.InstrumentCode,
                                    SourceAreaCode = objSaleInst.AreaCode,
                                    DestinationAreaCode = objSaleInst.AreaCode,
                                    SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                                    DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                                    TransferQty = intTransferQty,
                                    NotInstalledQty = null,
                                    RemovedQty = null,
                                    UnremovableQty = null,
                                    InstrumentAmount = null
                                });
                            }
                        }


                        //6.5	Insert to installed instrument instrument to data object
                        if (intNotInstalledQty > 0)
                        {
                            if (CheckExistReturnSaleInstrument(strInstallationSlipNo) == false) //Add by Jutarat A. on 11042013
                            {
                                foreach (var objInstMASale in qSaleInstrument.OrderByDescending(i => i.AreaCode))
                                {
                                    if (intNotInstalledQty <= 0)
                                    {
                                        break;
                                    }

                                    //6.5.1.2
                                    int intTransferQty = 0;
                                    if (objInstMASale.InstrumentQty >= intNotInstalledQty)
                                    {
                                        intTransferQty = intNotInstalledQty;
                                    }
                                    else
                                    {
                                        intTransferQty = objInstMASale.InstrumentQty.GetValueOrDefault(0);
                                    }
                                    objInstMASale.InstrumentQty -= intTransferQty;

                                    //6.5.1.3
                                    intNotInstalledQty -= intTransferQty;

                                    //6.5.1.4
                                    lstInvSlpDtlForNotInstalled.Add(new tbt_InventorySlipDetail()
                                    {
                                        SlipNo = null,
                                        RunningNo = lstInvSlpDtlForNotInstalled.Count + 1,
                                        InstrumentCode = objInstMASale.InstrumentCode,
                                        SourceAreaCode = objInstMASale.AreaCode,
                                        DestinationAreaCode = objInstMASale.AreaCode,
                                        SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                                        DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                                        TransferQty = intTransferQty,
                                        NotInstalledQty = intTransferQty,
                                        RemovedQty = null,
                                        UnremovableQty = null,
                                        InstrumentAmount = null
                                    });
                                }
                            }
                        }

                    }

                    #endregion

                    #region 7.1 Update transfer instrument installed data
                    if (lstInvSlpDtlForInstalled.Count > 0)
                    {
                        //7.1.1
                        doRegisterTransferInstrumentData register = new doRegisterTransferInstrumentData()
                        {
                            SlipId = SlipID.C_INV_SLIPID_STOCK_OUT,
                            InventorySlip = new tbt_InventorySlip()
                            {
                                SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE,
                                TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE,
                                InstallationSlipNo = strInstallationSlipNo,
                                ProjectCode = tempVar.ProjectCode,
                                ContractCode = strContractCode,
                                SlipIssueDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                StockInDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                StockOutDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                SourceLocationCode = InstrumentLocation.C_INV_LOC_WIP,
                                DestinationLocationCode = InstrumentLocation.C_INV_LOC_SOLD,
                                SourceOfficeCode = tempVar.InventoryHeadOfficeCode,
                                DestinationOfficeCode = tempVar.InventoryHeadOfficeCode,
                                InstallationCompleteFlag = FlagType.C_FLAG_ON,
                                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                            },
                            lstTbt_InventorySlipDetail = lstInvSlpDtlForInstalled
                        };

                        //7.1.4	Update inventory transfer data for installed 
                        string strInvSlipNo = this.RegisterTransferInstrument(register);

                        //7.1.5	Check there are new instrument
                        int intCheckNewInsturment = this.CheckNewInstrument(strInvSlipNo);

                        //7.1.6
                        if (intCheckNewInsturment == 1)
                        {
                            var lstGroupNewInstrument = this.GetGroupNewInstrument(strInvSlipNo);
                            foreach (var objInstrument in lstGroupNewInstrument)
                            {
                                //7.1.6.1.	Decrease instrument qty on source
                                List<tbt_AccountInprocess> lstCurrent = this.GetTbt_AccountInProcess(InstrumentLocation.C_INV_LOC_WIP, strContractCode, objInstrument.Instrumentcode);
                                if (lstCurrent == null || lstCurrent.Count <= 0)
                                {
                                scope.Dispose();
                                throw new ApplicationException("Missing instrument data in tbt_AccountInProcess.");
                                }
                                else if (lstCurrent.Count > 1)
                                {
                                    lstCurrent.RemoveRange(1, lstCurrent.Count - 1);
                                }

                                lstCurrent[0].ProjectCode = tempVar.ProjectCode;
                                lstCurrent[0].InstrumentQty = lstCurrent[0].InstrumentQty.GetValueOrDefault(0) - objInstrument.TransferQty;
                                lstCurrent[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                lstCurrent[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                var lstUpdatedAccountInProcess = this.UpdateTbt_AccountInProcess(lstCurrent);

                                //7.1.6.2
                                if (lstUpdatedAccountInProcess == null || lstUpdatedAccountInProcess.Count <= 0)
                                {
                                scope.Dispose();
                                return false;
                                }

                                //7.1.6.4
                                var objNewAccountStockMoving = new tbt_AccountStockMoving()
                                {
                                    SlipNo = strInvSlipNo,
                                    TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE,
                                    SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                    DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_SALE,
                                    SourceLocationCode = InstrumentLocation.C_INV_LOC_WIP,
                                    DestinationLocationCode = InstrumentLocation.C_INV_LOC_SOLD,
                                    InstrumentCode = objInstrument.Instrumentcode,
                                    InstrumentQty = objInstrument.TransferQty,
                                    InstrumentPrice = lstCurrent[0].MovingAveragePrice,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                                };
                                var lstInsertedAccountStockMoving = this.InsertAccountStockMoving(new List<tbt_AccountStockMoving>() { objNewAccountStockMoving });

                                if (lstInsertedAccountStockMoving == null || lstInsertedAccountStockMoving.Count <= 0)
                                {
                                scope.Dispose();
                                return false;
                                }

                            }
                        }

                        //7.1.7
                        int intCheckSampleInsturment = this.CheckSampleInstrument(strInvSlipNo);
                        if (intCheckSampleInsturment == 1)
                        {
                            var lstGroupSampleInstrument = this.GetGroupSampleInstrument(strInvSlipNo);
                            foreach (var objInstrument in lstGroupSampleInstrument)
                            {
                                //7.1.7.1.	Decrease instrument qty on source
                                List<tbt_AccountSampleInprocess> lstCurrent = this.GetTbt_AccountSampleInProcess(InstrumentLocation.C_INV_LOC_WIP, strContractCode, objInstrument.Instrumentcode);
                                if (lstCurrent == null || lstCurrent.Count <= 0)
                                {
                                scope.Dispose();
                                throw new ApplicationException("Missing instrument data in tbt_AccountSampleInProcess.");
                                }
                                else if (lstCurrent.Count > 1)
                                {
                                    lstCurrent.RemoveRange(1, lstCurrent.Count - 1);
                                }

                                lstCurrent[0].ProjectCode = tempVar.ProjectCode;
                                lstCurrent[0].InstrumentQty = lstCurrent[0].InstrumentQty.GetValueOrDefault(0) - objInstrument.TransferQty;
                                lstCurrent[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                lstCurrent[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                var lstUpdatedAccountSampleInProcess = this.UpdateTbt_AccountSampleInProcess(lstCurrent);

                                if (lstUpdatedAccountSampleInProcess == null || lstUpdatedAccountSampleInProcess.Count <= 0)
                                {
                                scope.Dispose();
                                return false;
                                }

                                //7.1.7.2
                                var objNewAccountStockMoving = new tbt_AccountStockMoving()
                                {
                                    SlipNo = strInvSlipNo,
                                    TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE,
                                    SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                    DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_SALE,
                                    SourceLocationCode = InstrumentLocation.C_INV_LOC_WIP,
                                    DestinationLocationCode = InstrumentLocation.C_INV_LOC_SOLD,
                                    InstrumentCode = objInstrument.Instrumentcode,
                                    InstrumentQty = objInstrument.TransferQty,
                                    InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                                };
                                var lstInsertedAccountStockMoving = this.InsertAccountStockMoving(new List<tbt_AccountStockMoving>() { objNewAccountStockMoving });

                                if (lstInsertedAccountStockMoving == null || lstInsertedAccountStockMoving.Count <= 0)
                                {
                                scope.Dispose();
                                return false;
                                }

                            }
                        }
                    }
                    #endregion

                    #region 7.2 Update transfer instrument Not installed data
                    if (lstInvSlpDtlForNotInstalled.Count > 0)
                    {
                        //Comment by Jutarat A. on 24042013 (Not used)
                        //if (CheckExistReturnSaleInstrument(strInstallationSlipNo) == false) //Add by Jutarat A. on 11042013
                        //{
                        //7.2.1
                        doRegisterTransferInstrumentData register = new doRegisterTransferInstrumentData()
                        {
                            SlipId = SlipID.C_INV_SLIPID_STOCK_OUT,
                            InventorySlip = new tbt_InventorySlip()
                            {
                                SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER,
                                TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE,
                                InstallationSlipNo = strInstallationSlipNo,
                                ProjectCode = tempVar.ProjectCode,
                                ContractCode = strContractCode,
                                SlipIssueDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Date,
                                StockInDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Date,
                                StockOutDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Date,
                                SourceLocationCode = InstrumentLocation.C_INV_LOC_WIP,
                                DestinationLocationCode = InstrumentLocation.C_INV_LOC_RETURNED,
                                SourceOfficeCode = tempVar.InventoryHeadOfficeCode,
                                DestinationOfficeCode = tempVar.InventoryHeadOfficeCode,
                                InstallationCompleteFlag = FlagType.C_FLAG_ON,
                                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                            },
                            lstTbt_InventorySlipDetail = lstInvSlpDtlForNotInstalled
                        };

                        //7.2.4	Update inventory transfer data for not installed  
                        string strInvSlipNo = this.RegisterTransferInstrument(register);

                        //7.2.5	Check there are new instrument
                        int intCheckNewInsturment = this.CheckNewInstrument(strInvSlipNo);

                        //7.2.6
                        if (intCheckNewInsturment == 1)
                        {
                            //7.2.7	Prepare data for update new instrument to account stock data
                            var lstGroupNewInstrument = this.GetGroupNewInstrument(strInvSlipNo);
                            foreach (var objInstrument in lstGroupNewInstrument)
                            {
                                //7.2.7.1.	Calculate moving average price
                                #region Monthly Price @ 2015
                                //decimal decMovingAvgPrice = this.CalculateMovingAveragePrice(objInstrument);
                                decimal decMovingAvgPrice = this.GetMonthlyAveragePrice(objInstrument.Instrumentcode, register.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                                #endregion

                                //7.2.7.2.	Update transfer data to Account DB
                                bool blnUpdate = this.UpdateAccountTransferNewInstrument(objInstrument, decMovingAvgPrice);

                                //7.2.7.3
                                if (!blnUpdate)
                                {
                                scope.Dispose();
                                return false;
                                }

                                //7.2.7.4.	Update transfer data to account moving stock
                                List<tbt_AccountInprocess> lstCurrent = this.GetTbt_AccountInProcess(objInstrument.SourceLocationCode, objInstrument.ContractCode, objInstrument.Instrumentcode);
                                if (lstCurrent == null || lstCurrent.Count <= 0)
                                {
                                    throw new ApplicationException("Missing instrument data in tbt_AccountInProcess.");
                                }

                                var objNewAccountStockMoving = new tbt_AccountStockMoving()
                                {
                                    SlipNo = strInvSlipNo,
                                    TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE,
                                    SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                    DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                                    SourceLocationCode = objInstrument.SourceLocationCode,
                                    DestinationLocationCode = objInstrument.DestinationLocationCode,
                                    InstrumentCode = objInstrument.Instrumentcode,
                                    InstrumentQty = objInstrument.TransferQty,
                                    InstrumentPrice = decMovingAvgPrice,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                                };
                                var lstInsertedAccountStockMoving = this.InsertAccountStockMoving(new List<tbt_AccountStockMoving>() { objNewAccountStockMoving });

                                if (lstInsertedAccountStockMoving == null || lstInsertedAccountStockMoving.Count <= 0)
                                {
                                scope.Dispose();
                                return false;
                                }

                            }
                        }

                        //7.2.8	 Check there are sample instrument
                        int intCheckSampleInsturment = this.CheckSampleInstrument(strInvSlipNo);
                        if (intCheckSampleInsturment == 1)
                        {
                            //7.2.10	 Prepare data for update sample instrument to account stock data
                            var lstGroupSampleInstrument = this.GetGroupSampleInstrument(strInvSlipNo);
                            foreach (var objInstrument in lstGroupSampleInstrument)
                            {
                                //7.2.11.1.	Update transfer data to Account DB
                                bool blnUpdate = this.UpdateAccountTransferSampleInstrument(objInstrument, null);
                                if (!blnUpdate)
                                {
                                scope.Dispose();
                                return false;
                                }

                                //7.2.11.2.	Update transfer data to account moving stock
                                var objNewAccountStockMoving = new tbt_AccountStockMoving()
                                {
                                    SlipNo = strInvSlipNo,
                                    TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE,
                                    SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                    DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                                    SourceLocationCode = objInstrument.SourceLocationCode,
                                    DestinationLocationCode = objInstrument.DestinationLocationCode,
                                    InstrumentCode = objInstrument.Instrumentcode,
                                    InstrumentQty = objInstrument.TransferQty,
                                    InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                                };
                                var lstInsertedAccountStockMoving = this.InsertAccountStockMoving(new List<tbt_AccountStockMoving>() { objNewAccountStockMoving });

                                if (lstInsertedAccountStockMoving == null || lstInsertedAccountStockMoving.Count <= 0)
                                {
                                scope.Dispose();
                                return false;
                                }

                            }
                        }
                        //}

                    }
                #endregion

                  scope.Complete();
                 }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        } //end UpdateCustomerAcceptance


        //Add by Jutarat A. on 11042013
        /// <summary>
        /// Update complete installation but no customer acceptance for sale contract
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <param name="dtpAcceptanceDate"></param>
        /// <returns></returns>
        public bool UpdateReturnSaleInstrument(string strContractCode, string strOCC, DateTime? dtpAcceptanceDate)
        {
            bool blnResult = false;
            ISaleContractHandler srvSale = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IInstallationHandler srvInstall = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;

            try
            {
                // 1. Validate updated customer acceptance
                // 1.1 Call InventoryHandler.CheckUpdatedUserAcceptance
                bool blnUpdatedUserAcceptance = this.CheckUpdatedUserAcceptance(strContractCode, dtpAcceptanceDate);

                // 2. Get Headoffice data
                List<doOffice> headOffice = this.GetInventoryHeadOffice();

                // 3. Get Contract data
                List<tbt_SaleBasic> lstSaleBasic = srvSale.GetTbt_SaleBasic(strContractCode, strOCC, null);
                var lstHeadOffice = this.GetInventoryHeadOffice();

                var tempVar = new
                {
                    InventoryHeadOfficeCode = (lstHeadOffice != null && lstHeadOffice.Count > 0 ? lstHeadOffice[0].OfficeCode : null),
                    ProjectCode = (lstSaleBasic != null && lstSaleBasic.Count > 0 ? lstSaleBasic[0].ProjectCode : null)
                };

                // 4. Get instrument detail
                List<doSaleInstrument> lstSaleInstruments = this.GetSaleInstrument(strContractCode, strOCC);

                // 5. Get InstallationSlip data
                string strInstallationSlipNo = srvInstall.GetInstallationSlipNoForAcceptant(strContractCode, strOCC);
                if (string.IsNullOrEmpty(strInstallationSlipNo))
                {
                    throw new ApplicationException("Unable to get installation slip no.");
                }

                var lstInstallDtl = srvInstall.GetInstallationDetailForCompleteInstallation(strInstallationSlipNo);

                List<tbt_InventorySlipDetail> lstInvSlpDtlForNotInstalled = new List<tbt_InventorySlipDetail>();

                using (TransactionScope scope = new TransactionScope())
                {
                #region 6.	Insert transfer data to data object

                foreach (var objInstallDtl in lstInstallDtl)
                    {
                        //6.1
                        var intNotInstalledQty = objInstallDtl.NotInstalledQty.GetValueOrDefault(0) + objInstallDtl.ReturnQty.GetValueOrDefault(0);

                        //6.2
                        var qSaleInstrument = lstSaleInstruments.Where(i => i.InstrumentCode == objInstallDtl.InstrumentCode);

                        //6.3	Insert to installed instrument instrument to data object
                        if (intNotInstalledQty > 0)
                        {
                            foreach (var objInstMASale in qSaleInstrument.OrderByDescending(i => i.AreaCode))
                            {
                                //6.3.1.1
                                if (intNotInstalledQty <= 0)
                                {
                                    break;
                                }

                                //6.3.1.2
                                int intTransferQty = 0;
                                if (objInstMASale.InstrumentQty >= intNotInstalledQty)
                                {
                                    intTransferQty = intNotInstalledQty;
                                }
                                else
                                {
                                    intTransferQty = objInstMASale.InstrumentQty.GetValueOrDefault(0);
                                }
                                objInstMASale.InstrumentQty -= intTransferQty;

                                //6.3.1.3
                                intNotInstalledQty -= intTransferQty;

                                //6.3.1.4
                                lstInvSlpDtlForNotInstalled.Add(new tbt_InventorySlipDetail()
                                {
                                    SlipNo = null,
                                    RunningNo = lstInvSlpDtlForNotInstalled.Count + 1,
                                    InstrumentCode = objInstMASale.InstrumentCode,
                                    SourceAreaCode = objInstMASale.AreaCode,
                                    DestinationAreaCode = objInstMASale.AreaCode,
                                    SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                                    DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                                    TransferQty = intTransferQty,
                                    NotInstalledQty = intTransferQty,
                                    RemovedQty = null,
                                    UnremovableQty = null,
                                    InstrumentAmount = null
                                });
                            }
                        }
                    }

                    #endregion

                    #region 7.1 Update transfer instrument Not installed data
                    if (lstInvSlpDtlForNotInstalled.Count > 0)
                    {
                        //7.1.1
                        doRegisterTransferInstrumentData register = new doRegisterTransferInstrumentData()
                        {
                            SlipId = SlipID.C_INV_SLIPID_STOCK_OUT,
                            InventorySlip = new tbt_InventorySlip()
                            {
                                SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER,
                                TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE,
                                InstallationSlipNo = strInstallationSlipNo,
                                ProjectCode = tempVar.ProjectCode,
                                ContractCode = strContractCode,
                                SlipIssueDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Date,
                                StockInDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Date,
                                StockOutDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Date,
                                SourceLocationCode = InstrumentLocation.C_INV_LOC_WIP,
                                DestinationLocationCode = InstrumentLocation.C_INV_LOC_RETURNED,
                                SourceOfficeCode = tempVar.InventoryHeadOfficeCode,
                                DestinationOfficeCode = tempVar.InventoryHeadOfficeCode,
                                InstallationCompleteFlag = FlagType.C_FLAG_ON,
                                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                            },
                            lstTbt_InventorySlipDetail = lstInvSlpDtlForNotInstalled
                        };

                        //7.1.4	Update inventory transfer data for not installed  
                        string strInvSlipNo = this.RegisterTransferInstrument(register);

                        //7.1.5	Check there are new instrument
                        int intCheckNewInsturment = this.CheckNewInstrument(strInvSlipNo);

                        //7.1.6
                        if (intCheckNewInsturment == 1)
                        {
                            //7.1.7	Prepare data for update new instrument to account stock data
                            var lstGroupNewInstrument = this.GetGroupNewInstrument(strInvSlipNo);
                            foreach (var objInstrument in lstGroupNewInstrument)
                            {
                                //7.1.7.1.	Calculate moving average price
                                #region Monthly Price @ 2015
                                //decimal decMovingAvgPrice = this.CalculateMovingAveragePrice(objInstrument);
                                decimal decMovingAvgPrice = this.GetMonthlyAveragePrice(objInstrument.Instrumentcode, register.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                                #endregion

                                //7.1.7.2.	Update transfer data to Account DB
                                bool blnUpdate = this.UpdateAccountTransferNewInstrument(objInstrument, decMovingAvgPrice);

                                //7.1.7.3
                                if (!blnUpdate)
                                {
                                    scope.Dispose();
                                    return false;
                                }

                                //7.1.7.4.	Update transfer data to account moving stock
                                List<tbt_AccountInprocess> lstCurrent = this.GetTbt_AccountInProcess(objInstrument.SourceLocationCode, objInstrument.ContractCode, objInstrument.Instrumentcode);
                                if (lstCurrent == null || lstCurrent.Count <= 0)
                                {
                                    throw new ApplicationException("Missing instrument data in tbt_AccountInProcess.");
                                }

                                var objNewAccountStockMoving = new tbt_AccountStockMoving()
                                {
                                    SlipNo = strInvSlipNo,
                                    TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE,
                                    SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                    DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                                    SourceLocationCode = objInstrument.SourceLocationCode,
                                    DestinationLocationCode = objInstrument.DestinationLocationCode,
                                    InstrumentCode = objInstrument.Instrumentcode,
                                    InstrumentQty = objInstrument.TransferQty,
                                    InstrumentPrice = decMovingAvgPrice,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                                };
                                var lstInsertedAccountStockMoving = this.InsertAccountStockMoving(new List<tbt_AccountStockMoving>() { objNewAccountStockMoving });

                                if (lstInsertedAccountStockMoving == null || lstInsertedAccountStockMoving.Count <= 0)
                                {
                                scope.Dispose();
                                return false;
                                }

                            }
                        }

                        //7.1.8	 Check there are sample instrument
                        int intCheckSampleInsturment = this.CheckSampleInstrument(strInvSlipNo);
                        if (intCheckSampleInsturment == 1)
                        {
                            //7.1.10	 Prepare data for update sample instrument to account stock data
                            var lstGroupSampleInstrument = this.GetGroupSampleInstrument(strInvSlipNo);
                            foreach (var objInstrument in lstGroupSampleInstrument)
                            {
                                //7.1.11.1.	Update transfer data to Account DB
                                bool blnUpdate = this.UpdateAccountTransferSampleInstrument(objInstrument, null);
                                if (!blnUpdate)
                                {
                                scope.Dispose();
                                return false;
                                }

                                //7.1.11.2.	Update transfer data to account moving stock
                                var objNewAccountStockMoving = new tbt_AccountStockMoving()
                                {
                                    SlipNo = strInvSlipNo,
                                    TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE,
                                    SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                    DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                                    SourceLocationCode = objInstrument.SourceLocationCode,
                                    DestinationLocationCode = objInstrument.DestinationLocationCode,
                                    InstrumentCode = objInstrument.Instrumentcode,
                                    InstrumentQty = objInstrument.TransferQty,
                                    InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                                };
                                var lstInsertedAccountStockMoving = this.InsertAccountStockMoving(new List<tbt_AccountStockMoving>() { objNewAccountStockMoving });

                                if (lstInsertedAccountStockMoving == null || lstInsertedAccountStockMoving.Count <= 0)
                                {
                                scope.Dispose();
                                return false;
                                }

                            }
                        }
                    }
                #endregion

                    scope.Complete();
                 }

                blnResult = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return blnResult;
        }


        public bool UpdateRealInvestigation(string strContractCode, List<tbt_QuotationInstrumentDetails> quotationInstrumentDetailList)
        {
            if (quotationInstrumentDetailList == null || quotationInstrumentDetailList.Count <= 0)
            {
                throw new ArgumentException("Quotation Instrument Detail list cant be null or empty.", "quotationInstrumentDetailList");
            }

            int sumAddQty = 0;
            int removeQty = 0;

            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            IQuotationHandler srvQU = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;

            var childInstruments = srvQU.ConvertQuotationParentToChildInstrument(quotationInstrumentDetailList[0].QuotationTargetCode, quotationInstrumentDetailList[0].Alphabet);

            // 1. Validate instrument exist in Inventory DB
            // 1.1 Loop in doQouInstrument[]
            foreach (var quotationInstrumentDetail in childInstruments)
            {

                // 1.1.1 Call InventoryHandler.CheckExistInstrument
                bool blnExist = this.CheckExistInstrumentInInventory(quotationInstrumentDetail.InstrumentCode);

                // 1.1.2 If blnExist <> TRUE Then throw exception
                if (!blnExist)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4109, new string[] { quotationInstrumentDetail.InstrumentCode });
                } //end if

                if (quotationInstrumentDetail.AddQty.HasValue)
                {
                    sumAddQty += quotationInstrumentDetail.AddQty.Value;
                } //end if

                if (quotationInstrumentDetail.RemoveQty.HasValue)
                {
                    removeQty += quotationInstrumentDetail.RemoveQty.Value;
                } //end if
            } //end foreach

            // 2. Get Headoffice data
            List<doOffice> headOffice = this.GetInventoryHeadOffice();


            #region 3. Update add instrument data

            // If Summary of doQuoInstrument.AddQty > 0 Then
            if (sumAddQty > 0)
            {

                doRegisterTransferInstrumentData registerTransferInstrument = new doRegisterTransferInstrumentData();

                // 3.2 Set doRegisterTransferInstrumentData.SlipId = C_INV_SLIPID_BUFFER
                registerTransferInstrument.SlipId = SlipID.C_INV_SLIPID_BUFFER;

                // 3.3 Set data in doRegisterTransferInstrumentData.doTbt_InventorySlip
                registerTransferInstrument.InventorySlip = new tbt_InventorySlip();

                registerTransferInstrument.InventorySlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
                registerTransferInstrument.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CHANGE_INVESTIGATION;
                registerTransferInstrument.InventorySlip.ContractCode = strContractCode;
                registerTransferInstrument.InventorySlip.SlipIssueDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.StockInDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.StockOutDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_BUFFER;
                registerTransferInstrument.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_USER;
                if (headOffice.Count != 0)
                {
                    registerTransferInstrument.InventorySlip.SourceOfficeCode = headOffice[0].OfficeCode;
                    registerTransferInstrument.InventorySlip.DestinationOfficeCode = headOffice[0].OfficeCode;
                }
                registerTransferInstrument.InventorySlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                //registerTransferInstrument.InventorySlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //registerTransferInstrument.InventorySlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                // 3.4 Set data in doRegisterTransferInstrumentData.doTbt_InventorySlipDetial[] by Loop in doQuoInstrument[]
                registerTransferInstrument.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
                tbt_InventorySlipDetail inventorySlipDetail;
                foreach (var quotationInstrumentDetail in childInstruments)
                {

                    // 3.1	Filter doQuoInstrument Where AddQty > 0
                    if (!quotationInstrumentDetail.AddQty.HasValue || quotationInstrumentDetail.AddQty.Value <= 0)
                    {
                        continue;
                    } //end if

                    if (this.CheckTransferFromBuffer(InstrumentLocation.C_INV_LOC_USER, quotationInstrumentDetail.InstrumentCode))
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4131);
                    }

                    inventorySlipDetail = new tbt_InventorySlipDetail();
                    inventorySlipDetail.InstrumentCode = quotationInstrumentDetail.InstrumentCode;
                    inventorySlipDetail.SourceAreaCode = InstrumentArea.C_INV_AREA_SE_RENTAL;
                    inventorySlipDetail.DestinationAreaCode = InstrumentArea.C_INV_AREA_SE_RENTAL;
                    inventorySlipDetail.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    inventorySlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    inventorySlipDetail.TransferQty = quotationInstrumentDetail.AddQty;
                    registerTransferInstrument.lstTbt_InventorySlipDetail.Add(inventorySlipDetail);
                } //end foreach

                // 3.5 Update inventory transfer data
                string strInventorySlipNo = this.RegisterTransferInstrument(registerTransferInstrument);

                // 3.6	Prepare data for update secondhand instrument to account stock data
                List<doGroupSecondhandInstrument> groupSecondHandList = this.GetGroupSecondhandInstrument(strInventorySlipNo);
                foreach (var groupSecondHand in groupSecondHandList)
                {

                    if (!this.UpdateAccountTransferSecondhandInstrumentIVS180(groupSecondHand))
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                    }

                    //// 3.6.1 Get oldest lot for account transfer
                    //List<tbt_AccountInstalled> oldestLotList = this.GetOldestLot(
                    //                                                    groupSecondHand.SourceOfficeCode,
                    //                                                    InstrumentLocation.C_INV_LOC_USER,
                    //                                                    groupSecondHand.Instrumentcode);

                    //// 3.6.2 Get data from inventory installed table for check existing data
                    //string lotNo = null;
                    //if (oldestLotList.Count != 0)
                    //{
                    //    lotNo = oldestLotList[0].LotNo;
                    //} //end if
                    //List<tbt_AccountInstalled> accountInstallSourceList = this.GetTbt_AccountInstalled(
                    //                                                    groupSecondHand.SourceOfficeCode,
                    //                                                    groupSecondHand.SourceLocationCode,
                    //                                                    groupSecondHand.Instrumentcode,
                    //                                                    lotNo);

                    //// If doTbt_AccountInstalled.Rows.count <= 0 Then Inset new record for source else Update
                    //if (accountInstallSourceList.Count <= 0)
                    //{
                    //    tbt_AccountInstalled accountInstall = new tbt_AccountInstalled();
                    //    accountInstall.OfficeCode = groupSecondHand.SourceOfficeCode;
                    //    accountInstall.LocationCode = groupSecondHand.SourceLocationCode;
                    //    accountInstall.LotNo = lotNo;
                    //    accountInstall.InstrumentCode = groupSecondHand.Instrumentcode;
                    //    accountInstall.InstrumentQty = 0 - groupSecondHand.TransferQty.Value;
                    //    accountInstall.AccquisitionCost = oldestLotList[0].AccquisitionCost;
                    //    accountInstall.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    accountInstall.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //    accountInstall.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    accountInstall.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //    List<tbt_AccountInstalled> insertList = new List<tbt_AccountInstalled>();
                    //    insertList.Add(accountInstall);
                    //    List<tbt_AccountInstalled> insertResult = this.InsertTbt_AccountInstalled(CommonUtil.ConvertToXml_Store<tbt_AccountInstalled>(insertList));

                    //    if (insertResult.Count > 0)
                    //    {
                    //        doTransactionLog logData = new doTransactionLog()
                    //        {
                    //            TransactionType = doTransactionLog.eTransactionType.Insert,
                    //            TableName = TableName.C_TBL_NAME_INV_ACC_INSTALLED,
                    //            TableData = null
                    //        };
                    //        logData.TableData = CommonUtil.ConvertToXml(insertResult);
                    //        logHand.WriteTransactionLog(logData);
                    //    }
                    //    else
                    //    {
                    //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                    //    } //end if/else
                    //}
                    //else
                    //{
                    //    accountInstallSourceList[0].OfficeCode = groupSecondHand.SourceOfficeCode;
                    //    accountInstallSourceList[0].LocationCode = groupSecondHand.SourceLocationCode;
                    //    accountInstallSourceList[0].LotNo = lotNo;
                    //    accountInstallSourceList[0].InstrumentCode = groupSecondHand.Instrumentcode;
                    //    accountInstallSourceList[0].InstrumentQty = accountInstallSourceList[0].InstrumentQty - groupSecondHand.TransferQty;
                    //    accountInstallSourceList[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    accountInstallSourceList[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //    List<tbt_AccountInstalled> updateList = new List<tbt_AccountInstalled>();
                    //    updateList.Add(accountInstallSourceList[0]);
                    //    List<tbt_AccountInstalled> updateResult = this.UpdateTbt_AccountInstalled(CommonUtil.ConvertToXml_Store<tbt_AccountInstalled>(updateList));

                    //    if (updateResult.Count > 0)
                    //    {
                    //        doTransactionLog logData = new doTransactionLog()
                    //        {
                    //            TransactionType = doTransactionLog.eTransactionType.Insert,
                    //            TableName = TableName.C_TBL_NAME_INV_ACC_INSTALLED,
                    //            TableData = null
                    //        };
                    //        logData.TableData = CommonUtil.ConvertToXml(updateResult);
                    //        logHand.WriteTransactionLog(logData);
                    //    }
                    //    else
                    //    {
                    //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                    //    } //end if/else
                    //} //end else

                    //// 3.6.3 Get data from inventory installed table for check existing data
                    //List<tbt_AccountInstalled> accountInstallDestinationList = this.GetTbt_AccountInstalled(
                    //                                                            groupSecondHand.DestinationOfficeCode,
                    //                                                            groupSecondHand.DestinationLocationCode,
                    //                                                            groupSecondHand.Instrumentcode,
                    //                                                            lotNo);

                    //// If doTbt_AccountInstalled.Rows.count <= 0 Then Inset new record for destination ELSE Update
                    //if (accountInstallDestinationList.Count <= 0)
                    //{
                    //    tbt_AccountInstalled accountInstall = new tbt_AccountInstalled();
                    //    accountInstall.OfficeCode = groupSecondHand.DestinationOfficeCode;
                    //    accountInstall.LocationCode = groupSecondHand.DestinationLocationCode;
                    //    accountInstall.LotNo = lotNo;
                    //    accountInstall.InstrumentCode = groupSecondHand.Instrumentcode;
                    //    accountInstall.InstrumentQty = groupSecondHand.TransferQty.Value;
                    //    accountInstall.AccquisitionCost = oldestLotList[0].AccquisitionCost;
                    //    accountInstall.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    accountInstall.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //    accountInstall.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    accountInstall.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //    List<tbt_AccountInstalled> insertList = new List<tbt_AccountInstalled>();
                    //    insertList.Add(accountInstall);
                    //    List<tbt_AccountInstalled> insertResult = this.InsertTbt_AccountInstalled(CommonUtil.ConvertToXml_Store<tbt_AccountInstalled>(insertList));

                    //    if (insertResult.Count > 0)
                    //    {
                    //        doTransactionLog logData = new doTransactionLog()
                    //        {
                    //            TransactionType = doTransactionLog.eTransactionType.Insert,
                    //            TableName = TableName.C_TBL_NAME_INV_ACC_INSTALLED,
                    //            TableData = null
                    //        };
                    //        logData.TableData = CommonUtil.ConvertToXml(insertResult);
                    //        logHand.WriteTransactionLog(logData);
                    //    }
                    //    else
                    //    {
                    //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                    //    } //end if/else
                    //}
                    //else
                    //{
                    //    accountInstallDestinationList[0].OfficeCode = groupSecondHand.DestinationOfficeCode;
                    //    accountInstallDestinationList[0].LocationCode = groupSecondHand.DestinationLocationCode;
                    //    accountInstallDestinationList[0].LotNo = lotNo;
                    //    accountInstallDestinationList[0].InstrumentCode = groupSecondHand.Instrumentcode;
                    //    accountInstallDestinationList[0].InstrumentQty = accountInstallDestinationList[0].InstrumentQty + groupSecondHand.TransferQty;
                    //    accountInstallDestinationList[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    accountInstallDestinationList[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //    List<tbt_AccountInstalled> updateList = new List<tbt_AccountInstalled>();
                    //    updateList.Add(accountInstallDestinationList[0]);
                    //    List<tbt_AccountInstalled> updateResult = this.UpdateTbt_AccountInstalled(CommonUtil.ConvertToXml_Store<tbt_AccountInstalled>(updateList));

                    //    if (updateResult.Count > 0)
                    //    {
                    //        doTransactionLog logData = new doTransactionLog()
                    //        {
                    //            TransactionType = doTransactionLog.eTransactionType.Insert,
                    //            TableName = TableName.C_TBL_NAME_INV_ACC_INSTALLED,
                    //            TableData = null
                    //        };
                    //        logData.TableData = CommonUtil.ConvertToXml(updateResult);
                    //        logHand.WriteTransactionLog(logData);
                    //    }
                    //    else
                    //    {
                    //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                    //    } //end if/else
                    //} //end if/else

                } //end foreach


            } //end if 3.

            #endregion

            # region 4.	Update remove instrument data

            //If Summary of doQuoInstrument.RemoveQty > 0 Then
            if (removeQty > 0)
            {

                doRegisterTransferInstrumentData registerTransferInstrument = new doRegisterTransferInstrumentData();

                // 4.2 Set doRegisterTransferInstrumentData.SlipId = C_INV_SLIPID_BUFFER
                registerTransferInstrument.SlipId = SlipID.C_INV_SLIPID_BUFFER;

                // 4.3 Set data in doRegisterTransferInstrumentData.doTbt_InventorySlip
                registerTransferInstrument.InventorySlip = new tbt_InventorySlip();

                registerTransferInstrument.InventorySlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
                registerTransferInstrument.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CHANGE_INVESTIGATION;
                registerTransferInstrument.InventorySlip.ContractCode = strContractCode;
                registerTransferInstrument.InventorySlip.SlipIssueDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.StockInDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.StockOutDate = DateTime.Now;
                registerTransferInstrument.InventorySlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_USER;
                registerTransferInstrument.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_BUFFER;
                if (headOffice.Count != 0)
                {
                    registerTransferInstrument.InventorySlip.SourceOfficeCode = headOffice[0].OfficeCode;
                    registerTransferInstrument.InventorySlip.DestinationOfficeCode = headOffice[0].OfficeCode;
                }
                registerTransferInstrument.InventorySlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                registerTransferInstrument.InventorySlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                //registerTransferInstrument.InventorySlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //registerTransferInstrument.InventorySlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                // 4.4	Set data in doRegisterTransferInstrumentData.doTbt_InventorySlipDetial[] by Loop in doQuoInstrument[]
                registerTransferInstrument.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
                tbt_InventorySlipDetail inventorySlipDetail = new tbt_InventorySlipDetail();
                foreach (var quotationInstrumentDetail in childInstruments)
                {

                    // 4.1 Filter doQuoInstrument Where Remove > 0
                    if (!quotationInstrumentDetail.RemoveQty.HasValue || quotationInstrumentDetail.RemoveQty.Value <= 0)
                    {
                        continue;
                    } //end if

                    inventorySlipDetail = new tbt_InventorySlipDetail();
                    inventorySlipDetail.InstrumentCode = quotationInstrumentDetail.InstrumentCode;
                    inventorySlipDetail.SourceAreaCode = InstrumentArea.C_INV_AREA_SE_RENTAL;
                    inventorySlipDetail.DestinationAreaCode = InstrumentArea.C_INV_AREA_SE_RENTAL;
                    inventorySlipDetail.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    inventorySlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    inventorySlipDetail.TransferQty = quotationInstrumentDetail.RemoveQty;
                    registerTransferInstrument.lstTbt_InventorySlipDetail.Add(inventorySlipDetail);
                } //end foreach

                // 4.5 Update inventory transfer data
                string strInventorySlipNo = this.RegisterTransferInstrument(registerTransferInstrument);

                // 4.6 Prepare data for update secondhand instrument to account stock data
                List<doGroupSecondhandInstrument> groupSecondHandList = this.GetGroupSecondhandInstrument(strInventorySlipNo);

                foreach (var groupSecondHand in groupSecondHandList)
                {

                    if (!this.UpdateAccountTransferSecondhandInstrument(groupSecondHand))
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                    }

                    //// 4.6.1 Get oldest lot for account transfer
                    //List<tbt_AccountInstalled> oldestLotList = this.GetOldestLot(groupSecondHand.SourceOfficeCode, InstrumentLocation.C_INV_LOC_USER, groupSecondHand.Instrumentcode);
                    //string lotNo = null;
                    //if (oldestLotList.Count != 0)
                    //{
                    //    lotNo = oldestLotList[0].LotNo;
                    //} //end if

                    //// 4.6.2 Update source location
                    //List<tbt_AccountInstalled> oldAccountInstalled = this.GetTbt_AccountInstalled(
                    //                                                    groupSecondHand.SourceOfficeCode,
                    //                                                    groupSecondHand.SourceLocationCode,
                    //                                                    groupSecondHand.Instrumentcode,
                    //                                                    lotNo);
                    //tbt_AccountInstalled accountInstall = oldAccountInstalled[0];
                    //accountInstall.InstrumentQty = groupSecondHand.TransferQty.Value;
                    //accountInstall.AccquisitionCost = oldestLotList[0].AccquisitionCost;
                    //accountInstall.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //accountInstall.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //List<tbt_AccountInstalled> updateList = new List<tbt_AccountInstalled>();
                    //updateList.Add(accountInstall);
                    //List<tbt_AccountInstalled> updateResult = this.UpdateTbt_AccountInstalled(CommonUtil.ConvertToXml_Store<tbt_AccountInstalled>(updateList));

                    //// 4.6.3 Get data from inventory installed table for check existing data
                    //List<tbt_AccountInstalled> accountInstallDestinationList = this.GetTbt_AccountInstalled(
                    //                                    groupSecondHand.DestinationOfficeCode,
                    //                                    groupSecondHand.DestinationLocationCode,
                    //                                    groupSecondHand.Instrumentcode,
                    //                                    lotNo);

                    //// If doTbt_AccountInstalled.Rows.count <= 0 Then Inset new record for destination Else Update
                    //if (accountInstallDestinationList.Count <= 0)
                    //{
                    //    tbt_AccountInstalled insertAccountInstall = new tbt_AccountInstalled();
                    //    if (headOffice.Count != 0)
                    //    {
                    //        insertAccountInstall.OfficeCode = headOffice[0].OfficeCode;
                    //        insertAccountInstall.LocationCode = headOffice[0].OfficeCode;
                    //    }
                    //    insertAccountInstall.LotNo = lotNo;
                    //    insertAccountInstall.InstrumentCode = groupSecondHand.Instrumentcode;
                    //    insertAccountInstall.InstrumentQty = groupSecondHand.TransferQty.Value;
                    //    insertAccountInstall.AccquisitionCost = oldestLotList[0].AccquisitionCost;
                    //    insertAccountInstall.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    insertAccountInstall.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //    insertAccountInstall.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    insertAccountInstall.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //    List<tbt_AccountInstalled> insertList = new List<tbt_AccountInstalled>();
                    //    insertList.Add(insertAccountInstall);
                    //    List<tbt_AccountInstalled> insertResult = this.InsertTbt_AccountInstalled(CommonUtil.ConvertToXml_Store<tbt_AccountInstalled>(insertList));

                    //    if (insertResult.Count > 0)
                    //    {
                    //        doTransactionLog logData = new doTransactionLog()
                    //        {
                    //            TransactionType = doTransactionLog.eTransactionType.Insert,
                    //            TableName = TableName.C_TBL_NAME_INV_ACC_INSTALLED,
                    //            TableData = null
                    //        };
                    //        logData.TableData = CommonUtil.ConvertToXml(insertResult);
                    //        logHand.WriteTransactionLog(logData);
                    //    }
                    //    else
                    //    {
                    //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                    //    } //end if/else
                    //}
                    //else
                    //{
                    //    accountInstallDestinationList[0].OfficeCode = groupSecondHand.DestinationOfficeCode;
                    //    accountInstallDestinationList[0].LocationCode = groupSecondHand.DestinationLocationCode;
                    //    accountInstallDestinationList[0].LotNo = lotNo;
                    //    accountInstallDestinationList[0].InstrumentCode = groupSecondHand.Instrumentcode;
                    //    accountInstallDestinationList[0].InstrumentQty = accountInstallDestinationList[0].InstrumentQty + groupSecondHand.TransferQty;
                    //    accountInstallDestinationList[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    accountInstallDestinationList[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //    updateList = new List<tbt_AccountInstalled>();
                    //    updateList.Add(accountInstallDestinationList[0]);
                    //    updateResult = this.UpdateTbt_AccountInstalled(CommonUtil.ConvertToXml_Store<tbt_AccountInstalled>(updateList));

                    //    if (updateResult.Count > 0)
                    //    {
                    //        doTransactionLog logData = new doTransactionLog()
                    //        {
                    //            TransactionType = doTransactionLog.eTransactionType.Insert,
                    //            TableName = TableName.C_TBL_NAME_INV_ACC_INSTALLED,
                    //            TableData = null
                    //        };
                    //        logData.TableData = CommonUtil.ConvertToXml(updateResult);
                    //        logHand.WriteTransactionLog(logData);
                    //    }
                    //    else
                    //    {
                    //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                    //    } //end if/else
                    //} //end if/else
                } //end foreach
            } //end if 4.

            #endregion

            return true;
        } //end UpdateRealInvestigation

        #endregion

        #region IVP110 Auto Complete Stock Checking

        public doBatchProcessResult AutoCompleteStockChecking()
        {

            // 1. Set dateCurent = Current date from server
            DateTime dateCurrent = DateTime.Now;

            // 2. Get last working date of current month
            //DateTime lastWorkingDate = this.GetLastWorkingDate(dateCurrent);
            DateTime? lastWorkingDate = this.GetLastBusinessDate(dateCurrent);

            // 3. Validate current date is between last working date and last date of current month
            DateTime lastDateOfMonth = CommonUtil.LastDayOfMonthFromDateTime(dateCurrent.Month, dateCurrent.Year);
            string yearMonth = null;

            // 3.1. If dateCurrent >= dateLastWorkingDate AND dateCurrent <= Last date of current month Then
            if (dateCurrent.CompareTo(lastWorkingDate) >= 0 && dateCurrent.CompareTo(lastDateOfMonth) <= 0)
            {
                yearMonth = dateCurrent.ToString("yyyyMM");
            }
            else
            {
                DateTime lastmonth = dateCurrent.AddMonths(-1);
                yearMonth = lastmonth.ToString("yyyyMM");
            } //end if/else
            List<tbt_InventoryCheckingSchedule> checkingSchedule = base.GetTbt_InventoryCheckingSchedule(yearMonth);

            // 4. Validate checking status is impleimenting
            // 4.1. If dtTbt_CheckingSchedule.CheckingStatus = C_INV_CHECKING_STATUS_IMPLEMENTING Then
            if (checkingSchedule.Count > 0)
            {

                if (CheckingStatus.C_INV_CHECKING_STATUS_IMPLEMENTING.Equals(checkingSchedule[0].CheckingStatus))
                {
                    checkingSchedule[0].CheckingStatus = CheckingStatus.C_INV_CHECKING_STATUS_STOPPING;
                    checkingSchedule[0].CheckingFinishDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    checkingSchedule[0].FinishCheckingBy = ProcessID.C_INV_PROCESS_ID_AUTO_COMPLETE_CHECKING;
                    checkingSchedule[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    checkingSchedule[0].UpdateBy = ProcessID.C_INV_PROCESS_ID_AUTO_COMPLETE_CHECKING;

                    List<tbt_InventoryCheckingSchedule> updateList = new List<tbt_InventoryCheckingSchedule>();
                    updateList.Add(checkingSchedule[0]);

                    base.UpdateTbt_InventoryCheckingSchedule(CommonUtil.ConvertToXml_Store<tbt_InventoryCheckingSchedule>(updateList));
                } //end if
            } //end if

            doBatchProcessResult batchResult = new doBatchProcessResult();
            batchResult.Result = FlagType.C_FLAG_ON;
            batchResult.BatchStatus = null;
            batchResult.Total = 1;
            batchResult.Complete = 1;
            batchResult.Failed = 0;
            batchResult.ErrorMessage = null;

            return batchResult;

        } //end AutoCompleteStockChecking

        #endregion

        #region Handler Public method

        public bool CheckExistInstrumentInInventory(string strInstrumentCode)
        {
            List<bool?> exist = base.CheckExistInstrument(strInstrumentCode);

            if (exist != null && exist.Count != 0 && exist[0].HasValue && exist[0].Value)
            {
                return true;
            }

            return false;
        }

        //Add by Jutarat A. on 11042013
        /// <summary>
        /// Check whether sale instrument has already been returned
        /// </summary>
        /// <param name="strInstallationSlipNo"></param>
        /// <returns></returns>
        public bool CheckExistReturnSaleInstrument(string @strInstallationSlipNo)
        {
            List<bool?> exist = base.CheckExistReturnSaleInstrument(@strInstallationSlipNo, TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE, InstrumentLocation.C_INV_LOC_RETURNED);

            if (exist != null && exist.Count != 0 && exist[0].HasValue && exist[0].Value)
            {
                return true;
            }

            return false;
        }
        //End Add

        public List<doContractWIPInstrument> GetContractWIPInstrument(string strInstallationSlipNo)
        {
            return base.GetContractWIPInstrument(
                strInstallationSlipNo,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK03,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK20,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK29
            );
        }

        public List<doContractWIPInstrumentPartial> GetContractWIPInstrumentPartial(string strInstallationSlipNo)
        {
            return base.GetContractWIPInstrumentPartial(
                strInstallationSlipNo,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK03,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK20,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK29
            );
        }

        public List<doSaleInstrument> GetSaleInstrument(string strContractCode, string strOCC)
        {
            return base.GetSaleInstrument(strContractCode,
                strOCC,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK03,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK20,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK29);

        }

        public bool CheckUpdatedUserAcceptance(string contractCode, DateTime? acceptanceDate)
        {
            List<bool?> booleanList = base.CheckUpdatedUserAcceptance(contractCode, acceptanceDate, TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE);
            if (booleanList != null && booleanList.Count != 0 && booleanList[0].HasValue && booleanList[0].Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckUpdatedCancelInstallation(string strInstallationSlipNo)
        {
            List<bool?> existList = base.CheckUpdatedCancelInstallation(strInstallationSlipNo, TransferType.C_INV_TRANSFERTYPE_CANCEL_INSTALLATION);
            if (existList != null && existList.Count != 0 && existList[0].HasValue && existList[0].Value)
            {
                return true;
            }
            return false;
        }

        public List<doContractUnoperatedInstrument> GetContractUnoperatedInstrument(string strContractCode)
        {
            return base.GetContractUnoperatedInstrument(strContractCode, TransferType.C_INV_TRANSFERTYPE_COMPLETE_BEFORE_START, InstrumentLocation.C_INV_LOC_WIP, InstrumentLocation.C_INV_LOC_UNOPERATED_WIP, InstrumentLocation.C_INV_LOC_WAITING_RETURN, InstrumentLocation.C_INV_LOC_RETURNED); //Modify (Add C_INV_LOC_RETURNED) by Jutarat A. 25022014
        }

        public string GenerateLotNo(tbs_LotRunningNo lotRunningNo)
        {
            //string result = LotNo.C_INV_LOT_NO_MINIMUM;
            //List<string> currentLotNo = base.GetLotRunningNo(
            //    lotRunningNo.InstrumentCode, lotRunningNo.DepreciationPeriodForContract, lotRunningNo.StartYearMonth);
            //if (currentLotNo.Count == 0)
            //{
            //    List<string> maxLotRunningNo = base.GetMaxLotRunningNo(lotRunningNo.InstrumentCode);
            //    if (maxLotRunningNo.Count > 0 && !CommonUtil.IsNullOrEmpty(maxLotRunningNo[0]))
            //    {
            //        int intMaxLot = int.Parse(maxLotRunningNo[0]);
            //        result = (intMaxLot + 1).ToString().PadLeft(5, '0');

            //        if (LotNo.C_INV_LOT_NO_MAXIMUM.CompareTo(result) > 0)
            //        {
            //            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4093);
            //        }

            //        base.InsertTbs_LotRunningNo(lotRunningNo.InstrumentCode, lotRunningNo.DepreciationPeriodForContract, lotRunningNo.StartYearMonth, result);
            //    }
            //}
            //else
            //{
            //    result = currentLotNo[0];
            //}

            //return result;

            try
            {
                var lsLotNo = base.GenerateLotNo(LotNo.C_INV_LOT_NO_MAXIMUM, LotNo.C_INV_LOT_NO_MINIMUM, lotRunningNo.InstrumentCode, lotRunningNo.DepreciationPeriodForContract, lotRunningNo.StartYearMonth, lotRunningNo.StartType);
                if (lsLotNo.Count > 0)
                {
                    return lsLotNo[0];
                }
                else
                {
                    throw new ApplicationException("Unable to generate new Lot No");
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message != null && (ex.InnerException.Message == MessageUtil.MessageList.MSG4093.ToString()))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4093);
                }
                else
                {
                    throw ex;
                }
            }

        } //end GenerateLotNo

        public string InsertDepreciationData(doInsertDepreciationData insertDepreciation)
        {
            int depreciationPeriod = 0;
            int depreciationPeriodRevenue = 0;
            string occ = null;
            string result = null;

            IRentralContractHandler rentalHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
            ICommonHandler commonHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            if (!CommonUtil.IsNullOrEmpty(insertDepreciation.ContractCode))
            {
                List<tbt_RentalContractBasic> rentalContractList = rentalHand.GetTbt_RentalContractBasic(insertDepreciation.ContractCode, null);
                if (rentalContractList.Count != 0)
                {
                    depreciationPeriod = rentalContractList[0].DepreciationPeriodContract.HasValue
                                            ? rentalContractList[0].DepreciationPeriodContract.Value : 0;
                    depreciationPeriodRevenue = rentalContractList[0].DepreciationPeriodRevenue.HasValue
                                            ? rentalContractList[0].DepreciationPeriodRevenue.Value : 0;
                    occ = rentalContractList[0].LastOCC;
                }

                if (insertDepreciation.StartType != InventoryStartType.C_INV_STARTTYPE_CHANGEAREA)
                {
                    var newStartYearMonth = this.GetDepreciationStartDate(insertDepreciation.ContractCode).FirstOrDefault();
                    if (newStartYearMonth != null)
                    {
                        insertDepreciation.StartYearMonth = newStartYearMonth.Value.ToString("yyyyMM");
                    }
                }
            }
            else
            {
                string depreciationPeriodConfig = commonHand.GetSystemStatusValue(ConfigName.C_CONFIG_DEPRECIATION_PERIOD_CONTRACT);
                if (!CommonUtil.IsNullOrEmpty(depreciationPeriodConfig))
                {
                    int depConfig = int.Parse(depreciationPeriodConfig);
                    depreciationPeriod = depConfig;
                }

                depreciationPeriodConfig = commonHand.GetSystemStatusValue(ConfigName.C_CONFIG_DEPRECIATION_PERIOD_REVENUE);
                if (!CommonUtil.IsNullOrEmpty(depreciationPeriodConfig))
                {
                    int depConfig = int.Parse(depreciationPeriodConfig);
                    depreciationPeriodRevenue = depConfig;
                }
            }

            List<doDepreciationData> depreciationList = base.GetDepreciationData(insertDepreciation.InstrumentCode, insertDepreciation.StartYearMonth, depreciationPeriod, insertDepreciation.StartType, insertDepreciation.ContractCode, occ);
            if (depreciationList.Count != 0)
            {
                return depreciationList[0].LotRunningNo;
            }

            tbs_LotRunningNo lotRunningNo = new tbs_LotRunningNo();
            lotRunningNo.InstrumentCode = insertDepreciation.InstrumentCode;
            lotRunningNo.DepreciationPeriodForContract = depreciationPeriod.ToString();
            lotRunningNo.StartYearMonth = insertDepreciation.StartYearMonth;
            lotRunningNo.StartType = insertDepreciation.StartType;
            result = this.GenerateLotNo(lotRunningNo);

            string scrapValueConfig = commonHand.GetSystemStatusValue(ConfigName.C_CONFIG_SCRAP_VALUE);
            int scrapValue = 1;
            if (!CommonUtil.IsNullOrEmpty(scrapValueConfig))
            {
                scrapValue = Convert.ToInt32(Decimal.Parse(scrapValueConfig));
            }

            decimal movingAverageMinusScrapValue = insertDepreciation.MovingAveragePrice - scrapValue;
            decimal MonthlyDepreciationAmount = Math.Round((movingAverageMinusScrapValue) / depreciationPeriod, 4, MidpointRounding.AwayFromZero);
            decimal MonthlyDepreciationAmountRevenue = Math.Round((movingAverageMinusScrapValue) / depreciationPeriodRevenue, 4, MidpointRounding.AwayFromZero);
            base.InsertTbt_InventoryDepreciation(
                result,
                insertDepreciation.InstrumentCode,
                insertDepreciation.MovingAveragePrice, // Fix by Natthavat S.
                insertDepreciation.StartYearMonth,
                depreciationPeriod,
                0,
                null,
                0,
                MonthlyDepreciationAmount,
                (movingAverageMinusScrapValue) - Math.Round((MonthlyDepreciationAmount * (depreciationPeriod - 1)), 4, MidpointRounding.AwayFromZero),
                depreciationPeriodRevenue,
                0,
                null,
                0,
                MonthlyDepreciationAmountRevenue,
                (movingAverageMinusScrapValue) - Math.Round((MonthlyDepreciationAmountRevenue * (depreciationPeriodRevenue - 1)), 4, MidpointRounding.AwayFromZero),
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                CommonUtil.dsTransData.dtUserData.EmpNo
                );

            // Fixed bug by Non A. 29/Mar/2012 : return Lot No. rather than null value.
            return result;
        }

        public decimal GetAveragePriceforInstalledInstrument(doGroupSecondhandInstrument groupSecondHand)
        {

            List<doFIFOInstrument> listFIFO = GetFIFOInstrument(groupSecondHand.SourceOfficeCode, groupSecondHand.SourceLocationCode, groupSecondHand.Instrumentcode);

            int intTransferQty = groupSecondHand.TransferQty.Value;

            decimal decTotalAmount = 0;
            decimal decAverateAccqusitionCost = 0;

            foreach (var i in listFIFO)
            {
                int intSourceQty = 0;
                int intDestinationQty = 0;
                if (intTransferQty <= 0)
                {
                    break;
                }

                if (i.InstrumentQty.Value <= intTransferQty)
                {
                    intDestinationQty = i.InstrumentQty.Value;
                }
                else
                {
                    intSourceQty = i.InstrumentQty.Value - intTransferQty;
                    intDestinationQty = intTransferQty;
                }

                decTotalAmount = decTotalAmount + (intDestinationQty * i.AccquisitionCost.Value);
            }

            decAverateAccqusitionCost = Math.Round(decTotalAmount / groupSecondHand.TransferQty.Value, 4, MidpointRounding.AwayFromZero);

            return decAverateAccqusitionCost;
        }

        public doNormalShelfExistCurrent GetNormalShelfExistCurrent(string ShelfNo)
        {
            List<doNormalShelfExistCurrent> listNormalShelf = base.GetNormalShelfExistCurrent(ShelfNo, ShelfType.C_INV_SHELF_TYPE_NORMAL);

            if (listNormalShelf != null && listNormalShelf.Count != 0)
                return listNormalShelf[0];
            return null;
        }


        // - Add by Nontawat L. on 09-Jul-2014 : Phase4:3.44
        public List<tbt_InstallationSlip> GetInstallationSlipDetail(string installationSlipNo)
        {
            IInstallationHandler handler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            return handler.GetTbt_InstallationSlip(installationSlipNo);
        }
        // - End add
        #endregion

    }
}
