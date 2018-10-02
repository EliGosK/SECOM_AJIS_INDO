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
using SECOM_AJIS.DataEntity.Contract;
using CSI.WindsorHelper;
using System.Transactions;

using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace SECOM_AJIS.DataEntity.Inventory
{

    partial class InventoryHandler : BizIVDataEntities, IInventoryHandler
    {

        #region IVS020
        /// <summary>
        /// Get installation slip data for stock-out.
        /// </summary>
        /// <param name="param">Data Object for search installation slip data.</param>
        /// <returns>List of Data Object of installation slip.</returns>
        public List<doResultInstallationSlipForStockOut> GetInstallationSlipForStockOut(doGetInstallationSlipForStockOut param)
        {

            var lst = base.GetInstallationSlipForStockOut(
                MiscType.C_RENTAL_INSTALL_TYPE,
                MiscType.C_SALE_INSTALL_TYPE,
                SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT,
                ConfigName.C_CONFIG_WILDCARD,
                param.InstallationSlipNo,
                param.ProjectCode,
                param.ContractCode,
                param.ContractTargetName,
                param.SiteName,
                param.ExpectedStockOutDateFrom,
                param.ExpectedStockOutDateTo,
                param.OfficeCode,
                param.InstallationTypeCode
            );
            CommonUtil.MappingObjectLanguage<doResultInstallationSlipForStockOut>(lst);
            return lst;
        }

        /// <summary>
        /// Get installation instrument data for stock-out.
        /// </summary>
        /// <param name="strSlipNo">Installation slip no.</param>
        /// <returns>List of Data Object of installation slip instrument detail.</returns>
        public List<doResultInstallationDetailForStockOut> GetInstallationDetailForStockOut(string strSlipNo)
        {
            var lst = base.GetInstallationDetailForStockOut(
                InstrumentArea.C_INV_AREA_NEW_SALE,
                InstrumentArea.C_INV_AREA_NEW_RENTAL,
                InstrumentArea.C_INV_AREA_NEW_SAMPLE,
                InstrumentArea.C_INV_AREA_SE_RENTAL,
                InstrumentType.C_INST_TYPE_GENERAL,
                strSlipNo
            );
            return lst;
        }

        /// <summary>
        /// Get installation instrument data for checking.
        /// </summary>
        /// <param name="param">Data Object for get installation data for checking.</param>
        /// <returns>List of Data Object of installation for checking.</returns>
        public List<doResultInstallationStockOutForChecking> GetInstallationStockOutForChecking(doGetInstallationStockOutForChecking param)
        {
            var lst = base.GetInstallationStockOutForChecking(
                FunctionLogistic.C_FUNC_LOGISTIC_HQ,
                InstrumentArea.C_INV_AREA_NEW_SALE,
                InstrumentArea.C_INV_AREA_NEW_RENTAL,
                InstrumentArea.C_INV_AREA_NEW_SAMPLE,
                InstrumentArea.C_INV_AREA_SE_RENTAL,
                InstrumentLocation.C_INV_LOC_INSTOCK,
                InstrumentLocation.C_INV_LOC_PROJECT_WIP,
                param.OfficeCode,
                param.SlipNo,
                param.ProjectCode,
                param.InstrumentCode,
                param.SaleShelfNo,
                param.RentalShelfNo,
                param.SampleShelfNo,
                param.SecondShelfNo
            );
            return lst;
        }

        #endregion

        #region IVS021
        /// <summary>
        /// Get installation slip data for partial stock-out.
        /// </summary>
        /// <param name="param">Data Object for search installation slip for partial stock-out.</param>
        /// <returns>List of Data Object of installation slip for partial stock-out.</returns>
        public List<doResultInstallationSlipForStockOut> GetInstallationSlipForPartialStockOut(doGetInstallationSlipForStockOut param)
        {

            var lst = base.GetInstallationSlipForPartialStockOutList(
                MiscType.C_RENTAL_INSTALL_TYPE,
                MiscType.C_SALE_INSTALL_TYPE,
                SlipStatus.C_SLIP_STATUS_NOT_STOCK_OUT,
                ConfigName.C_CONFIG_WILDCARD,
                SlipStatus.C_SLIP_STATUS_PARTIAL_STOCK_OUT,
                param.InstallationSlipNo,
                param.ContractCode,
                param.ContractTargetName,
                param.SiteName,
                param.ExpectedStockOutDateFrom,
                param.ExpectedStockOutDateTo,
                param.OfficeCode,
                param.InstallationTypeCode
            );
            CommonUtil.MappingObjectLanguage<doResultInstallationSlipForStockOut>(lst);
            return lst;
        }

        /// <summary>
        /// Update all contract's inventory slip status to complete.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public List<tbt_InventorySlip> UpdatePartialToCompleteStatus(string strContractCode)
        {
            var lst = base.UpdatePartialToCompleteStatus(InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE, InventorySlipStatus.C_INV_SLIP_STATUS_PARTIAL, strContractCode);

            #region Log

            if (lst.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_INV_SLIP,
                    TableData = CommonUtil.ConvertToXml(lst)
                };

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            #endregion

            return lst;
        }
        #endregion

        #region IVP010
        /// <summary>
        /// New booking.
        /// </summary>
        /// <param name="booking">Data Object for new booking.</param>
        /// <returns>Return True, if function complete successfully. Otherwise, return False.</returns>
        public bool NewBooking(doBooking booking)
        {
            if (booking == null)
            {
                throw new ArgumentException("Booking can't be null.", "booking");
            }

            #region Expand & Group data by child instruments
            var srvMA = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;

            // Transform booking instrument into LINQ object form (1 object : 1 instrument)
            var qParentInstruments = booking.InstrumentCode.Select((inst, i) => new
            {
                InstrumentCode = booking.InstrumentCode[i],
                InstrumentQty = (i < booking.InstrumentQty.Count ? booking.InstrumentQty[i] : 0),
                ChildInstruments = srvMA.GetTbm_InstrumentExpansion(booking.InstrumentCode[i], null),
                InstrumentInfo = srvMA.GetTbm_Instrument(booking.InstrumentCode[i])
            });

            // Expand instrument into child intrument.
            var qGroupedInstruments = (
                from instParent in qParentInstruments
                where (instParent.InstrumentInfo != null
                    && instParent.InstrumentInfo.Count > 0
                    && instParent.InstrumentInfo[0].InstrumentTypeCode == InstrumentType.C_INST_TYPE_GENERAL
                )
                let ChildInstruments = (
                    instParent.ChildInstruments == null || instParent.ChildInstruments.Count <= 0
                    ? new string[] { instParent.InstrumentCode }
                    : instParent.ChildInstruments.Select(c => c.ChildInstrumentCode)
                )
                from strChildInstrumentCode in ChildInstruments
                let instChild = new
                {
                    InstrumentCode = strChildInstrumentCode,
                    InstrumentQty = instParent.InstrumentQty
                }
                group instChild by new { InstrumentCode = instChild.InstrumentCode } into gGroupedChilds
                orderby gGroupedChilds.Key.InstrumentCode
                select new
                {
                    InstrumentCode = gGroupedChilds.Key.InstrumentCode,
                    InstrumentQty = gGroupedChilds.Sum(i => i.InstrumentQty)
                }
            );

            doBooking bookingByChild = new doBooking()
            {
                ContractCode = booking.ContractCode,
                ExpectedStartServiceDate = booking.ExpectedStartServiceDate,
                InstrumentCode = new List<string>(),
                InstrumentQty = new List<int>(),
                blnExistContractCode = booking.blnExistContractCode,
                blnFirstInstallCompleteFlag = booking.blnFirstInstallCompleteFlag
            };

            foreach (var inst in qGroupedInstruments)
            {
                bookingByChild.InstrumentCode.Add(inst.InstrumentCode);
                bookingByChild.InstrumentQty.Add(inst.InstrumentQty);
            }
            #endregion

            bool blnExistContractCode = false;

            var lstExists = base.GetTbt_InventoryBooking(bookingByChild.ContractCode);
            if (lstExists != null && lstExists.Count > 0)
            {
                blnExistContractCode = true;
            }
            else
            {
                string strAreaCode = null;
                if (CommonUtil.dsTransData.dtTransHeader.ScreenID == ScreenID.C_SCREEN_ID_FN99 ||
                    CommonUtil.dsTransData.dtTransHeader.ScreenID == ScreenID.C_SCREEN_ID_CP12_CHANGE_PLAN)
                {
                    strAreaCode = InstrumentArea.C_INV_AREA_NEW_RENTAL;
                }
                else if (CommonUtil.dsTransData.dtTransHeader.ScreenID == ScreenID.C_SCREEN_ID_FQ99 ||
                    CommonUtil.dsTransData.dtTransHeader.ScreenID == ScreenID.C_SCREEN_ID_CQ12_CHANGE_PLAN)
                {
                    strAreaCode = InstrumentArea.C_INV_AREA_NEW_SALE;
                }

                var lstNewInvBooking = new List<tbt_InventoryBooking> { 
                    new tbt_InventoryBooking() {
                        ContractCode = bookingByChild.ContractCode, 
                        ExpectedStartServiceDate = bookingByChild.ExpectedStartServiceDate,
                        CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                        UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                    }
                };

                var lstNewInvBookingDtl = new List<tbt_InventoryBookingDetail>();
                if (bookingByChild.InstrumentCode != null && bookingByChild.InstrumentCode.Count > 0)
                {
                    for (int i = 0; i < bookingByChild.InstrumentCode.Count; i++)
                    {
                        lstNewInvBookingDtl.Add(new tbt_InventoryBookingDetail()
                        {
                            ContractCode = bookingByChild.ContractCode,
                            InstrumentCode = bookingByChild.InstrumentCode[i],
                            AreaCode = strAreaCode,
                            BookingQty = bookingByChild.InstrumentQty[i],
                            StockOutQty = 0,
                            CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                            UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                        });
                    }
                }

                this.InsertTbt_InventoryBooking(lstNewInvBooking);
                this.InsertTbt_InventoryBookingDetail(lstNewInvBookingDtl);

                blnExistContractCode = false;
            }

            return blnExistContractCode;
        }

        /// <summary>
        /// Change expected start service date.
        /// </summary>
        /// <param name="booking">Data Object for change expected start service date.</param>
        /// <returns>Return True, if function complete successfully. Otherwise, return False.</returns>
        public bool ChangeExpectedStartServiceDate(doBooking booking)
        {
            if (booking == null)
            {
                throw new ArgumentException("Booking can't be null.", "booking");
            }

            bool blnExistContractCode = false;

            var lstBooking = base.GetTbt_InventoryBooking(booking.ContractCode);
            if (lstBooking != null && lstBooking.Count > 0)
            {
                for (int i = 0; i < lstBooking.Count; i++)
                {
                    lstBooking[i].ExpectedStartServiceDate = booking.ExpectedStartServiceDate;
                    lstBooking[i].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    lstBooking[i].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }

                this.UpdateTbt_InventoryBooking(lstBooking);

                blnExistContractCode = true;
            }
            else
            {
                blnExistContractCode = false;
            }

            return blnExistContractCode;
        }

        /// <summary>
        /// Cancel booking.
        /// </summary>
        /// <param name="booking">Data Object for cancel booking.</param>
        /// <returns>Return True, if function complete successfully. Otherwise, return False.</returns>
        public bool CancelBooking(doBooking booking)
        {
            bool blnExistContractCode = false;

            var lstExists = base.GetTbt_InventoryBooking(booking.ContractCode);
            if (lstExists != null && lstExists.Count > 0)
            {
                this.DeleteTbt_InventoryBookingDetailWithLog(booking.ContractCode, null);
                this.DeleteTbt_InventoryBookingWithLog(booking.ContractCode);

                blnExistContractCode = true;
            }
            else
            {
                blnExistContractCode = false;
            }

            return blnExistContractCode;
        }

        /// <summary>
        /// Re-booking.
        /// </summary>
        /// <param name="booking">Data Object for re-booking</param>
        /// <returns>Return True, if function complete successfully. Otherwise, return False.</returns>
        public bool Rebooking(doBooking booking)
        {
            if (booking == null)
            {
                throw new ArgumentException("Booking can't be null.", "booking");
            }

            bool blnExistContractCode = false;

            var lstBooking = base.GetTbt_InventoryBooking(booking.ContractCode);
            if (lstBooking != null && lstBooking.Count > 0)
            {
                var lstBookingDtl = base.GetTbt_InventoryBookingDetail(booking.ContractCode, null);
                for (int i = 0; i < lstBookingDtl.Count; i++)
                {
                    lstBookingDtl[i].StockOutQty = 0;
                    lstBookingDtl[i].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    lstBookingDtl[i].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }

                this.UpdateTbt_InventoryBookingDetail(lstBookingDtl);

                blnExistContractCode = true;
            }
            else
            {
                blnExistContractCode = false;
            }

            return blnExistContractCode;
        }

        /// <summary>
        /// Update stock-out instrument.
        /// </summary>
        /// <param name="booking">Data Object for update stock-out instrument.</param>
        /// <returns>Return true, if function complete successfully. Otherwise, return false.</returns>
        public doBooking UpdateStockOutInstrument(doBooking booking)
        {
            // Fix by Natthavat S.
            bool blnFirstInstallCompleteFlag = true;
            var lstInvBooking = this.GetTbt_InventoryBooking(booking.ContractCode);
            if (lstInvBooking != null && lstInvBooking.Count > 0)
            {
                booking.blnExistContractCode = true;

                IRentralContractHandler rentalHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                var lstRental = rentalHand.GetTbt_RentalContractBasic(booking.ContractCode, null);
                if (lstRental.Count > 0)
                {
                    if (lstRental[0].FirstInstallCompleteFlag.HasValue && !lstRental[0].FirstInstallCompleteFlag.Value)
                    {
                        blnFirstInstallCompleteFlag = false;
                    }
                }

                booking.blnFirstInstallCompleteFlag = blnFirstInstallCompleteFlag;

                ILogHandler srvLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                var lstInvBookDtl = new List<tbt_InventoryBookingDetail>();
                if (booking.InstrumentCode != null)
                {
                    for (int i = 0; i < booking.InstrumentCode.Count; i++)
                    {
                        var strInsrumentCode = booking.InstrumentCode[i];

                        var lstInvBookDtlTmp = this.GetTbt_InventoryBookingDetail(booking.ContractCode, strInsrumentCode);
                        if (lstInvBookDtlTmp.Count > 0)
                        {
                            if (lstInvBookDtlTmp[0].BookingQty < booking.InstrumentQty[i])
                            {
                                this.CancelBooking(booking);
                                lstInvBookDtl.Clear();
                                break;
                            }

                            lstInvBookDtlTmp[0].StockOutQty = (lstInvBookDtlTmp[0].StockOutQty ?? 0) + booking.InstrumentQty[i];
                            lstInvBookDtlTmp[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            lstInvBookDtlTmp[0].UpdateDate = DateTime.Now;
                            lstInvBookDtl.Add(lstInvBookDtlTmp[0]);
                        }
                    }
                }

                if (lstInvBookDtl.Count > 0)
                {
                    this.UpdateTbt_InventoryBookingDetail(lstInvBookDtl);
                }
            }
            else
            {
                booking.blnExistContractCode = false;
            }

            return booking;
        }

        #endregion

        #region IVP020
        /// <summary>
        /// Calcuate depreciation and update to depreciation table.
        /// </summary>
        /// <param name="strUserId">Executing user's id.</param>
        /// <param name="dtBatchDate">Executing date.</param>
        /// <returns>Batch result of calculate and update depreciation.</returns>
        public doBatchProcessResult UpdateCalculateDepreciation(string strUserId, DateTime dtBatchDate)
        {
            base.UpdateCalculateDepreciation(dtBatchDate, strUserId);

            return new doBatchProcessResult()
            {
                Result = FlagType.C_FLAG_ON,
                BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED,
                Total = 1,
                Complete = 1,
                Failed = 0,
                ErrorMessage = null,
                BatchUser = strUserId,
                BatchDate = dtBatchDate
            };
        }
        #endregion

        #region IVP050
        /// <summary>
        /// Freeze Instrument Data For Stock Checking Process.
        /// </summary>
        /// <param name="strUserId">Executing user's id.</param>
        /// <param name="dtBatchDate">Executing date.</param>
        /// <returns>Batch result of freeze instrument data for stock checking process.</returns>
        public doBatchProcessResult FreezeInstrumentDataForStockCheckingProcess(string strUserId, DateTime dtBatchDate)
        {
            var lstYearMonth = base.ValidateFreezeInstrumentIVP050();

            ICommonHandler srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var bIsSuspend = srvCommon.IsSystemSuspending();

            if ((lstYearMonth != null && lstYearMonth.Count > 0 && lstYearMonth[0] != null) && bIsSuspend)
            {
                base.FreezeInstrumentDataForStockCheckingProcess(
                    lstYearMonth[0],
                    InstrumentLocation.C_INV_LOC_INSTOCK,
                    InstrumentLocation.C_INV_LOC_PRE_ELIMINATION,
                    InstrumentLocation.C_INV_LOC_REPAIRING,
                    FlagType.C_FLAG_OFF,
                    CheckingStatus.C_INV_CHECKING_STATUS_PREPARING,
                    ShelfNo.C_INV_SHELF_NO_NOT_PRICE
                );
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_ON,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED,
                    Total = 1,
                    Complete = 1,
                    Failed = 0,
                    ErrorMessage = null,
                    BatchUser = strUserId,
                    BatchDate = dtBatchDate
                };
            }
            else
            {
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_ON,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED,
                    Total = 0,
                    Complete = 0,
                    Failed = 0,
                    ErrorMessage = null,
                    BatchUser = strUserId,
                    BatchDate = dtBatchDate
                };
            }

        }
        #endregion

        #region IVP100
        /// <summary>
        /// Generate inventory data for account.
        /// </summary>
        /// <returns>List of Data Object of inventory data for account.</returns>
        public doBatchProcessResult GenerateInventoryAccountData(string strExecUserID, DateTime dtBatchDate)
        {
            const string strBatchUserID = "IVP100";
            try
            {

                DateTime dtToday = DateTime.Today;
                DateTime dtLastDateOfMonth = dtToday.AddMonths(1).AddDays(-dtToday.Day);
                DateTime dtLastBusinessDate = (this.GetLastBusinessDate(dtToday) ?? dtLastDateOfMonth);
                DateTime dtDateGenerate;
                if (dtLastBusinessDate <= dtToday && dtToday <= dtLastDateOfMonth)
                {
                    dtDateGenerate = dtToday;
                }
                else
                {
                    dtDateGenerate = dtToday.AddMonths(-1);
                }

                IInventoryDocumentHandler srvInvDoc = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                bool bResult = srvInvDoc.GenerateInventoryAccountData(dtDateGenerate, strExecUserID, dtBatchDate);

                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_ON,
                    BatchStatus = (bResult ? BatchStatus.C_BATCH_STATUS_SUCCEEDED : BatchStatus.C_BATCH_STATUS_FAILED),
                    Total = 1,
                    Complete = (bResult ? 1 : 0),
                    Failed = (bResult ? 0 : 1),
                    ErrorMessage = (bResult ? null : "Some report hasn't been generated properly."),
                    BatchUser = strBatchUserID,
                    BatchDate = dtBatchDate
                };
            }
            catch (Exception e)
            {
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = e.ToString(), //msg.Message,
                    BatchUser = strBatchUserID,
                    BatchDate = dtBatchDate
                };
            }
        }

        #endregion

        #region IVP130
        /// <summary>
        /// Generate blank checking slip.
        /// </summary>
        /// <param name="strUserId">Executing user's id.</param>
        /// <param name="dtBatchDate">Executing date.</param>
        /// <returns>Batch result of generate blank checking slip process.</returns>
        public doBatchProcessResult GenerateBlankCheckingSlip(string strUserId, DateTime dtBatchDate)
        {
            const string MSG4116 = "System does'not freezed current inventory data.";
            const string MSG4117 = "Created blank checking slip already.";
            const string MSG4118 = "Created blank checking slip fail.";

            var dCurrentDate = DateTime.Today;
            var dLastDateOfMonth = dCurrentDate.AddMonths(1).AddDays(dCurrentDate.Day);
            var dLastBusinessDate = this.GetLastBusinessDate(null);

            string strYearMonth = null;

            if (dCurrentDate >= dLastBusinessDate && dCurrentDate <= dLastDateOfMonth)
            {
                strYearMonth = dCurrentDate.ToString("yyyyMM");
            }
            else
            {
                strYearMonth = dCurrentDate.AddMonths(-1).ToString("yyyyMM");
            }

            if (!this.CheckFreezedDataYearMonth(strYearMonth))
            {
                //MessageModel msg = MessageUtil.GetMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4116, null);
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = MSG4116, //msg.Message,
                    BatchUser = strUserId,
                    BatchDate = dtBatchDate
                };
            }

            if (this.CheckGeneratedCheckingSlip(strYearMonth))
            {
                ////MessageModel msg = MessageUtil.GetMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4117, null);
                //return new doBatchProcessResult
                //{
                //    Result = FlagType.C_FLAG_OFF,
                //    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                //    Total = 1,
                //    Complete = 0,
                //    Failed = 1,
                //    ErrorMessage = MSG4117, //msg.Message,
                //    BatchUser = strUserId,
                //    BatchDate = dtBatchDate
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
                    BatchDate = dtBatchDate
                };
            }

            var lstChkTmp = this.GetCheckingTempForGenSlip(strYearMonth);

            using (var scope = new TransactionScope())
            {
                try
                {
                    foreach (var objChkTemp in lstChkTmp)
                    {
                        var strInventorySlipNo = this.GenerateInventorySlipNo(objChkTemp.OfficeCode, SlipID.C_INV_SLIPID_CHEKCING_INSTRUMENT);
                        var objInvChkSlip = new tbt_InventoryCheckingSlip()
                        {
                            SlipNo = strInventorySlipNo,
                            SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE,
                            CheckingYearMonth = strYearMonth,
                            LocationCode = objChkTemp.LocationCode,
                            OfficeCode = objChkTemp.OfficeCode,
                            CreateDate = dtBatchDate,
                            CreateBy = null, //C_INV_PROCESS_ID_GEN_BLANK_SLIP 
                            UpdateDate = dtBatchDate,
                            UpdateBy = null //C_INV_PROCESS_ID_GEN_BLANK_SLIP 
                        };

                        var lstInserted = this.InsertTbt_InventoryCheckingSlip(new List<tbt_InventoryCheckingSlip>() { objInvChkSlip }, false);
                        if (lstInserted.Count <= 0)
                        {
                            scope.Dispose();

                            //MessageModel msg = MessageUtil.GetMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4118, null);
                            return new doBatchProcessResult()
                            {
                                Result = FlagType.C_FLAG_OFF,
                                BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                                Total = lstChkTmp.Count,
                                Complete = 0,
                                Failed = lstChkTmp.Count,
                                ErrorMessage = MSG4118, //msg.Message,
                                BatchUser = strUserId,
                                BatchDate = dtBatchDate
                            };
                        }

                        List<tbt_InventoryCheckingSlipDetail> lstInvChkDtl = new List<tbt_InventoryCheckingSlipDetail>();
                        int intRunningNo = 0;
                        var lstInstForChking = this.GetInstrumentForChecking(strYearMonth, objChkTemp.LocationCode, objChkTemp.OfficeCode, objChkTemp.AreaCode);
                        foreach (var objInstChkTmp in lstInstForChking)
                        {
                            lstInvChkDtl.Add(new tbt_InventoryCheckingSlipDetail()
                            {
                                SlipNo = strInventorySlipNo,
                                RunningNoInSlip = ++intRunningNo,
                                AreaCode = objInstChkTmp.AreaCode,
                                ShelfNo = objInstChkTmp.ShelfNo,
                                InstrumentCode = objInstChkTmp.InstrumentCode,
                                CheckingQty = null,
                                StockQty = objInstChkTmp.InstrumentQty,
                                CreateDate = dtBatchDate,
                                CreateBy = null, //C_INV_PROCESS_ID_GEN_BLANK_SLIP,
                                UpdateDate = dtBatchDate,
                                UpdateBy = null //C_INV_PROCESS_ID_GEN_BLANK_SLIP
                            });
                        }

                        var lstInsertedInvChkSlipDtl = this.InsertTbt_InventoryCheckingSlipDetail(lstInvChkDtl, false);
                        if (lstInsertedInvChkSlipDtl.Count != lstInstForChking.Count)
                        {
                            scope.Dispose();

                            //MessageModel msg = MessageUtil.GetMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4118, null);
                            return new doBatchProcessResult()
                            {
                                Result = FlagType.C_FLAG_OFF,
                                BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                                Total = lstInstForChking.Count,
                                Complete = 0,
                                Failed = lstInstForChking.Count,
                                ErrorMessage = MSG4118, //msg.Message,
                                BatchUser = strUserId,
                                BatchDate = dtBatchDate
                            };
                        }

                    }//end lstChkTmp

                    List<string> lstGeneratedFile = new List<string>();
                    try
                    {
                        var srvDoc = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

                        var lstInvChkSlip = this.GetCheckingSlipByYearMonth(strYearMonth);
                        foreach (var objInvChkSlip in lstInvChkSlip)
                        {
                            var strFilePath = srvDoc.GenerateIVR100FilePath(objInvChkSlip.SlipNo, objInvChkSlip.OfficeCode, strUserId, dtBatchDate);
                            lstGeneratedFile.Add(strFilePath);
                        }

                        scope.Complete();
                    }
                    catch (Exception)
                    {
                        foreach (var strFilePath in lstGeneratedFile)
                        {
                            File.Delete(strFilePath);
                        }
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    scope.Dispose();

                    //MessageModel msg = MessageUtil.GetMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4118, null);
                    return new doBatchProcessResult()
                    {
                        Result = FlagType.C_FLAG_OFF,
                        BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                        Total = 1,
                        Complete = 0,
                        Failed = 1,
                        ErrorMessage = ex.Message, //msg.Message,
                        BatchUser = strUserId,
                        BatchDate = dtBatchDate
                    };
                }
            }

            return new doBatchProcessResult()
            {
                Result = FlagType.C_FLAG_ON,
                BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED,
                Total = 1,
                Complete = 1,
                Failed = 0,
                ErrorMessage = null,
                BatchUser = strUserId,
                BatchDate = dtBatchDate
            };
        }

        /// <summary>
        /// Check freezed temp data by year month.
        /// </summary>
        /// <param name="strCheckYearMonth">Checking year/month.</param>
        /// <returns>Return true, if alreaedy freezed data. Otherwise, return false.</returns>
        public bool CheckFreezedDataYearMonth(string strCheckYearMonth)
        {
            var lstResult = this.GetTbt_InventoryCheckingSchedule(strCheckYearMonth);
            return (lstResult.Count > 0);
        }

        /// <summary>
        /// Check Generated Checking Slip.
        /// </summary>
        /// <param name="strCheckYearMonth">Checking year/month.</param>
        /// <returns>Return true, if alreaedy genereated checking slip. Otherwise, return false.</returns>
        public bool CheckGeneratedCheckingSlip(string strCheckYearMonth)
        {
            var lstResult = this.GetTbt_InventoryCheckingSlip(null, strCheckYearMonth, null, null);
            return (lstResult.Count > 0);
        }

        /// <summary>
        /// Get Checking Slip By YearMonth.
        /// </summary>
        /// <param name="strCheckingYearMonth">Checking year/month.</param>
        /// <returns>List of Data Object of inventory checking slip.</returns>
        public List<tbt_InventoryCheckingSlip> GetCheckingSlipByYearMonth(string strCheckingYearMonth)
        {
            return base.GetTbt_InventoryCheckingSlip(null, strCheckingYearMonth, null, null);
        }

        /// <summary>
        /// Get Instrument For Checking.
        /// </summary>
        /// <param name="strCheckingYearMonth">Checking year/month.</param>
        /// <param name="strLocationCode">Instrument location code.</param>
        /// <param name="strOfficeCode">Office code.</param>
        /// <returns>List of Data Object of checking instrument.</returns>
        public List<tbt_InventoryCheckingTemp> GetInstrumentForChecking(string strCheckingYearMonth, string strLocationCode, string strOfficeCode, string strAreaCode)
        {
            #region R2

            return this.GetTbt_InventoryCheckingTemp(strCheckingYearMonth, strLocationCode, strOfficeCode, null, strAreaCode, null);

            #endregion
        }
        #endregion

        /// <summary>
        /// Getting data for create report IVR100.
        /// </summary>
        /// <param name="strInventorySlipNo">Inventory slip no.</param>
        /// <returns>List of Data Object of report IVR100.</returns>
        private List<doIVR100> GetIVR100(string strInventorySlipNo)
        {
            return base.GetIVR100(MiscType.C_INV_LOC, MiscType.C_INV_AREA, ConfigName.C_CONFIG_SUSPEND_FLAG, strInventorySlipNo);
        }

        /// <summary>
        /// Getting data for create report IVR110.
        /// </summary>
        /// <param name="strInventorySlipNo">Inventory slip no.</param>
        /// <returns>List of Data Object of report IVR110.</returns>
        private List<doIVR110> GetIVR110(string strInventorySlipNo)
        {
            return base.GetIVR110(MiscType.C_INV_LOC, MiscType.C_INV_AREA, ConfigName.C_CONFIG_SUSPEND_FLAG, strInventorySlipNo);
        }

        #region IVS201
        /// <summary>
        /// Get data for view in IVS201.
        /// </summary>
        /// <param name="param">Data Object for search data.</param>
        /// <returns>List of Data Object for view in IVS201.</returns>
        public List<doResultIVS201> GetIVS201(doGetIVS201 param)
        {
            var strOfficeCodeList = (param.OfficeCode ?? string.Join(",", param.OfficeCodeList.Select(OfficeCode => "\"" + OfficeCode + "\"")));
            var strLocationCodeList = (param.LocationCode ?? string.Join(",", param.LocationCodeList.Select(LocationCode => "\"" + LocationCode + "\"")));
            var lst = base.GetIVS201(MiscType.C_INV_LOC, MiscType.C_INV_AREA, ConfigName.C_INV_AREA_SHORT, ConfigName.C_CONFIG_WILDCARD,
                strOfficeCodeList, strLocationCodeList, param.AreaCode, param.ShelfNoFrom, param.ShelfNoTo, param.InstrumentCode, param.InstrumentName);
            CommonUtil.MappingObjectLanguage<doResultIVS201>(lst);
            return lst;
        }
        #endregion

        #region IVS220
        /// <summary>
        /// Get data for view in IVS220.
        /// </summary>
        /// <param name="param">Data Object for search data.</param>
        /// <returns>List of Data Object for view in IVS220.</returns>
        public List<dtResultIVS220> GetIVS220(doGetIVS220 param)
        {
            var strOfficeCodeList = (param.OfficeCode == null ? "" : string.Join(",", param.OfficeCode.Select(OfficeCode => "\"" + OfficeCode + "\"")));
            var lst = base.GetIVS220(MiscType.C_INV_AREA, ConfigName.C_INV_AREA_SHORT, ConfigName.C_INV_SLIP_PREFIX,
                TransferType.C_INV_TRANSFERTYPE_TRANSFER_AREA, TransferType.C_INV_TRANSFERTYPE_TRANSFER_SHELF, ConfigName.C_CONFIG_WILDCARD,
                InstrumentLocation.C_INV_LOC_WAITING_RETURN, InstrumentLocation.C_INV_LOC_PROJECT_WIP, InstrumentLocation.C_INV_LOC_WIP,
                InstrumentLocation.C_INV_LOC_UNOPERATED_WIP, InstrumentLocation.C_INV_LOC_RETURNED, InstrumentLocation.C_INV_LOC_RETURN_WIP,
                InstrumentLocation.C_INV_LOC_USER, InstrumentLocation.C_INV_LOC_TRANSFER, InstrumentLocation.C_INV_LOC_INSTOCK,
                InstrumentLocation.C_INV_LOC_REPAIR_REQUEST, InstrumentLocation.C_INV_LOC_REPAIRING, InstrumentLocation.C_INV_LOC_REPAIR_RETURN,
                InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE, InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER,
                strOfficeCodeList, param.LocationCode, param.DateFrom,
                param.DateTo, param.InstrumentCode, param.InstrumentName,
                param.InventorySlipNo, param.AreaCode, param.ContractCode, param.SupplierName, param.TransferType);
            CommonUtil.MappingObjectLanguage<dtResultIVS220>(lst);
            return lst;
        }
        #endregion

        #region IVS230
        /// <summary>
        /// Get inventory slip for view in IVS230.
        /// </summary>
        /// <param name="param">Data Object for search data.</param>
        /// <returns>List of Data Object for view in IVS230.</returns>
        public List<dtResultInventorySlipIVS230> GetInventorySlipIVS230(doGetInventorySlipIVS230 param)
        {
            var strOfficeCodeList = (param.OfficeCode == null ? "" : string.Join(",", param.OfficeCode.Select(OfficeCode => "\"" + OfficeCode + "\"")));
            var lst = base.GetInventorySlipIVS230(
                MiscType.C_INV_LOC,
                TransferType.C_INV_TRANSFERTYPE,
                MiscType.C_INV_SLIP_STATUS,
                TransferType.C_INV_TRANSFERTYPE_TRANSFER_AREA,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK03,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK20,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK29,
                param.InventorySlipno,
                param.SlipStatus,
                strOfficeCodeList,
                param.DateFrom,
                param.DateTo,
                param.EmpNo,
                param.ProjectCode,
                param.StockOutType,
                param.ContractCode,
                param.InstrumentCode,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29
            );
            CommonUtil.MappingObjectLanguage<dtResultInventorySlipIVS230>(lst);
            return lst;
        }
        /// <summary>
        /// Get inventory slip detail for view in IVS230.
        /// </summary>
        /// <param name="strInventorySlipNo">Inventory slip no.</param>
        /// <returns>List of Data Object for view in IVS230.</returns>
        public List<dtResultInventorySlipDetail> GetInventorySlipDetail(string strInventorySlipNo)
        {
            var lst = base.GetInventorySlipDetail(MiscType.C_INV_AREA, strInventorySlipNo);
            CommonUtil.MappingObjectLanguage<dtResultInventorySlipDetail>(lst);
            return lst;
        }
        #endregion

        #region IVS240
        /// <summary>
        /// Search installation slip for view in IVS240.
        /// </summary>
        /// <param name="param">Data Object for search data.</param>
        /// <returns>List of Data Object for view in IVS240.</returns>
        public List<dtSearchInstallationSlipResult> SearchInstallationSlip(doSearchInstallationSlipCond param)
        {
            if (string.IsNullOrEmpty(param.OperationOfficeCode))
            {
                if (param.OperationOfficeCodeList != null) //Add by Jutarat A. on 22012013
                    param.OperationOfficeCode = string.Join(",", param.OperationOfficeCodeList) + ",";
            }

            var lst = base.SearchInstallationSlip(
                ConfigName.C_CONFIG_WILDCARD,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29,
                param.InstallationSlipNo,
                param.ExpectedStockOutDateFrom,
                param.ExpectedStockOutDateTo,
                param.ContractCode,
                param.ProjectCode,
                param.OperationOfficeCode,
                param.SubContractorName
            );
            CommonUtil.MappingObjectLanguage<dtSearchInstallationSlipResult>(lst);
            return lst;
        }

        /// <summary>
        /// Get installation stock-out data for generate picking list no.
        /// </summary>
        /// <param name="strInstallationSlipNo">Installation slip no.</param>
        /// <returns>List of Data Object of installation stock-out data.</returns>
        public List<dtStockOutByInstallationSlipResult> GetStockOutByInstallationSlip(string strInstallationSlipNo)
        {
            var lst = base.GetStockOutByInstallationSlip(
                MiscType.C_INV_AREA,
                strInstallationSlipNo
            );
            CommonUtil.MappingObjectLanguage<dtStockOutByInstallationSlipResult>(lst);
            return lst;
        }

        /// <summary>
        /// Generate picking list no.
        /// </summary>
        /// <returns>Return new value of picking ist no.</returns>
        public string GeneratePickingListNo()
        {
            var lstNewNo = base.GeneratePickingListNo(PickingListNo.C_INV_PICKING_NO_MINIMUM, PickingListNo.C_INV_PICKING_NO_MAXIMUM);
            if (lstNewNo.Count > 0)
            {
                return lstNewNo[0];
            }
            else
            {
                throw new ApplicationException("Unable to generate new PickingListNo");
            }
        }
        #endregion

        #region IVS270


        #endregion

        /// <summary>
        /// Update complete stock data for fully partial stock-out.
        /// </summary>
        /// <param name="doComplete">Data Object of installation for update complete.</param>
        /// <returns>Return true, if update data complete successfully. Otherwise, return false.</returns>
        public bool UpdateCompleteStockoutForPartial(doUpdateCompleteStockoutForPartial doComplete)
        {
            var doOffice = this.GetInventoryHeadOffice();

            if (doOffice.Count < 0)
            {
                //Cannot get inventory header office data.
                return false;
            }

            using (TransactionScope scope = new TransactionScope())
            {

                //Set value in dsRegisterTransferInstrumentData for register transfer instrument
                var objRegTnfInstStkOut = new doRegisterTransferInstrumentData()
                {
                    SlipId = SlipID.C_INV_SLIPID_STOCK_OUT,
                    InventorySlip = new tbt_InventorySlip()
                    {
                        SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE,
                        InstallationSlipNo = doComplete.InstallationSlipNo,
                        ContractCode = doComplete.ContractCode,
                        SlipIssueDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        StockOutDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        SourceLocationCode = InstrumentLocation.C_INV_LOC_PARTIAL_OUT,
                        DestinationLocationCode = InstrumentLocation.C_INV_LOC_WIP,
                        SourceOfficeCode = doOffice[0].OfficeCode,
                        DestinationOfficeCode = doOffice[0].OfficeCode,
                        ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL,
                        CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                        UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                    }
                };

                if (doComplete.SlipType == SlipID.C_INV_SLIPID_NEW_SALE)
                {
                    objRegTnfInstStkOut.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_MK20;
                }
                else if (doComplete.SlipType == SlipID.C_INV_SLIPID_NEW_RENTAL)
                {
                    objRegTnfInstStkOut.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_MK03;
                }
                else if (doComplete.SlipType == SlipID.C_INV_SLIPID_CHANGE_INSTALLATION)
                {
                    objRegTnfInstStkOut.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_MK29;
                }

                //7.15.3
                var lstSumPartStockOut = this.GetSumPartialStockOutList(doComplete.ContractCode);

                if (lstSumPartStockOut != null && lstSumPartStockOut.Count > 0) //Add by Jutarat A. on 08072013
                {
                    //7.15.4
                    objRegTnfInstStkOut.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
                    for (int i = 0; i < lstSumPartStockOut.Count; i++)
                    {
                        objRegTnfInstStkOut.lstTbt_InventorySlipDetail.Add(new tbt_InventorySlipDetail()
                        {
                            RunningNo = i + 1,
                            InstrumentCode = lstSumPartStockOut[i].InstrumentCode,
                            SourceAreaCode = lstSumPartStockOut[i].AreaCode,
                            DestinationAreaCode = lstSumPartStockOut[i].AreaCode,
                            SourceShelfNo = lstSumPartStockOut[i].ShelfNo,
                            DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                            TransferQty = lstSumPartStockOut[i].TransferQty
                        });
                    }

                    //Update inventory trasfer data
                    var strInvSlipNo = this.RegisterTransferInstrument(objRegTnfInstStkOut);

                    //Check there are new instrument
                    if (this.CheckNewInstrument(strInvSlipNo) == 1)
                    {
                        //Prepare data for update new instrument to account stock data
                        var lstNewInstGrp = this.GetGroupNewInstrument(strInvSlipNo);

                        foreach (var objNewInst in lstNewInstGrp)
                        {
                            //Calculate moving average price
                            #region Monthly Price @ 2015
                            //var dblMovingAvgPrice = this.CalculateMovingAveragePrice(objNewInst);
                            var dblMovingAvgPrice = this.GetMonthlyAveragePrice(objNewInst.Instrumentcode, objRegTnfInstStkOut.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                            #endregion

                            //Update transfer data to account DB
                            var bUpdated = this.UpdateAccountTransferNewInstrument(objNewInst, (decimal)dblMovingAvgPrice);

                            if (!bUpdated)
                            {
                                scope.Dispose();
                                return false;
                            }
                        }
                    }

                    //Check there are secondhand instrument
                    if (this.CheckSecondhandInstrument(strInvSlipNo) == 1)
                    {
                        //Prepare data for update secondhand instrument to account stock data
                        var lstSecondInstGrp = this.GetGroupSecondhandInstrument(strInvSlipNo);

                        foreach (var objSecondInst in lstSecondInstGrp)
                        {
                            //Update secondhand instrument to Account DB
                            var bUpdated = this.UpdateAccountTransferSecondhandInstrument(objSecondInst);

                            if (!bUpdated)
                            {
                                scope.Dispose();
                                return false;
                            }
                        }
                    }

                    //Check there are sample instrument
                    if (this.CheckSampleInstrument(strInvSlipNo) == 1)
                    {
                        //Prepare data for update sample instrument to account stock data
                        var lstSampleInstGrp = this.GetGroupSampleInstrument(strInvSlipNo);

                        foreach (var objSampleInst in lstSampleInstGrp)
                        {

                            //Update sample instrument to Account DB
                            var bUpdated = this.UpdateAccountTransferSampleInstrument(objSampleInst, null);

                            if (!bUpdated)
                            {
                                scope.Dispose();
                                return false;
                            }
                        }
                    }

                    //Add by Jutarat A. on 08072013
                    var lstUpdatedSlip = this.UpdatePartialToCompleteStatus(doComplete.ContractCode);
                    if (lstUpdatedSlip == null || lstUpdatedSlip.Count <= 0)
                    {
                        scope.Dispose();
                        return false;
                    }
                    //End Add
                }

                scope.Complete(); //Add by Jutarat A. on 14032013
            }

            return true;
        }

        /// <summary>
        /// Get instrument data for complete sales maintenance.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public List<doInstrumentMASale> GetInstrumentForCompleteMASale(string strContractCode)
        {
            return base.GetInstrumentForCompleteMASale(InstrumentArea.C_INV_AREA_NEW_SALE, InstrumentArea.C_INV_AREA_NEW_SAMPLE, InstrumentLocation.C_INV_LOC_WIP, strContractCode);
        }

        /// <summary>
        /// Update complete stock-out for Sales Maintenance.
        /// </summary>
        /// <param name="strInstallationSlipNo"></param>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public bool UpdateCompleteSaleMA(string strInstallationSlipNo, string strContractCode)
        {
            IInstallationHandler srvInstall = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            ISaleContractHandler srvSale = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
            IContractHandler srvContract = ServiceContainer.GetService<IContractHandler>() as IContractHandler;

            using (TransactionScope scope = new TransactionScope())
            {
                //1.	Get Headoffice data
                var lstHeadOffice = this.GetInventoryHeadOffice();

                //2.	Get installation slip details data
                var lstInstallDtl = srvInstall.GetInstallationDetailForCompleteInstallation(strInstallationSlipNo);

                //3.	Get all Instrument from WIP W/H
                var lstInstMASale = this.GetInstrumentForCompleteMASale(strContractCode);

                //4.	Get Contract data
                var objSaleBasic = srvSale.GetTbt_SaleBasic(strContractCode, null, true);

                var tempVar = new
                {
                    InventoryHeadOfficeCode = (lstHeadOffice != null && lstHeadOffice.Count > 0 ? lstHeadOffice[0].OfficeCode : null),
                    ProjectCode = (objSaleBasic != null && objSaleBasic.Count > 0 ? objSaleBasic[0].ProjectCode : null)
                };

                List<tbt_InventorySlipDetail> lstInvSlpDtlForInstalled = new List<tbt_InventorySlipDetail>();
                List<tbt_InventorySlipDetail> lstInvSlpDtlForNotInstalled = new List<tbt_InventorySlipDetail>();

                #region 6.	Insert transfer data to data object
                foreach (var objInstallDtl in lstInstallDtl)
                {
                    var intNotInstalledQty = objInstallDtl.NotInstalledQty.GetValueOrDefault(0) + objInstallDtl.ReturnQty.GetValueOrDefault(0);
                    var intInstalledQty = objInstallDtl.TotalStockOutQty.GetValueOrDefault(0) - intNotInstalledQty;

                    //6.3
                    var qInstrumentMASale = lstInstMASale.Where(i => i.InstrumentCode == objInstallDtl.InstrumentCode);

                    //6.4	Insert to installed instrument instrument to data object
                    if (intInstalledQty > 0)
                    {
                        foreach (var objInstMASale in qInstrumentMASale.OrderBy(i => i.AreaCode))
                        {
                            if (intInstalledQty <= 0)
                            {
                                break;
                            }

                            //6.4.1.2
                            int intTransferQty = 0;
                            if (objInstMASale.InstrumentQty >= intInstalledQty)
                            {
                                intTransferQty = intInstalledQty;
                            }
                            else
                            {
                                intTransferQty = objInstMASale.InstrumentQty.GetValueOrDefault(0);
                            }
                            objInstMASale.InstrumentQty -= intTransferQty;

                            //6.4.1.3
                            intInstalledQty -= intTransferQty;

                            //6.4.1.4
                            lstInvSlpDtlForInstalled.Add(new tbt_InventorySlipDetail()
                            {
                                SlipNo = null,
                                RunningNo = lstInvSlpDtlForInstalled.Count + 1,
                                InstrumentCode = objInstMASale.InstrumentCode,
                                SourceAreaCode = objInstMASale.AreaCode,
                                DestinationAreaCode = objInstMASale.AreaCode,
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
                        foreach (var objInstMASale in qInstrumentMASale.OrderByDescending(i => i.AreaCode))
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
                            TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START,
                            InstallationSlipNo = strInstallationSlipNo,
                            ProjectCode = tempVar.ProjectCode,
                            ContractCode = strContractCode,
                            SlipIssueDate = DateTime.Now,
                            StockInDate = DateTime.Now,
                            StockOutDate = DateTime.Now,
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
                                TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START,
                                SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_LOSSBYMA,
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
                                TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START,
                                SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_LOSSBYMA,
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
                    //7.2.1
                    doRegisterTransferInstrumentData register = new doRegisterTransferInstrumentData()
                    {
                        SlipId = SlipID.C_INV_SLIPID_STOCK_OUT,
                        InventorySlip = new tbt_InventorySlip()
                        {
                            SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER,
                            TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START,
                            InstallationSlipNo = strInstallationSlipNo,
                            ProjectCode = tempVar.ProjectCode,
                            ContractCode = strContractCode,
                            SlipIssueDate = DateTime.Now,
                            StockInDate = DateTime.Now,
                            StockOutDate = DateTime.Now,
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
                                TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START,
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
                                TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START,
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

            return true;
        }

        public doBatchProcessResult UpdateMonthlyAveragePrice(string strUserId, DateTime? updateDate, DateTime dtBatchDate)
        {
            base.UpdateMonthlyAveragePrice(dtBatchDate,
                updateDate ?? DateTime.Now,
                strUserId,
                InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                TransferType.C_INV_TRANSFERTYPE_STOCKIN_PURCHASE,
                TransferType.C_INV_TRANSFERTYPE_STOCKIN_SPECIAL,
                InstrumentArea.C_INV_AREA_NEW_SAMPLE,
                InstrumentArea.C_INV_AREA_NEW_SALE,
                InstrumentArea.C_INV_AREA_NEW_RENTAL,
                InstrumentArea.C_INV_AREA_SE_RENTAL,
                InstrumentArea.C_INV_AREA_NEW_DEMO,
                InstrumentArea.C_INV_AREA_SE_LENDING_DEMO,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK03,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK20,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_NORMAL_MK29,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK03,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK20,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PARTIAL_MK29,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PROJECT,
                TransferType.C_INV_TRANSFERTYPE_CUSTOMER_ACCEPTANCE,
                TransferType.C_INV_TRANSFERTYPE_COMPLETE_AFTER_START,
                TransferType.C_INV_TRANSFERTYPE_START_SERVICE,
                TransferType.C_INV_TRANSFERTYPE_ELIMINATION,
                TransferType.C_INV_TRANSFERTYPE_STOCKOUT_SPECIAL,
                TransferType.C_INV_TRANSFERTYPE_TRANSFER_BUFFER,
                TransferType.C_INV_TRANSFERTYPE_TRANSFER_AREA,
                InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE,
                InstrumentLocation.C_INV_LOC_RETURNED,
                InstrumentLocation.C_INV_LOC_SOLD,
                InstrumentLocation.C_INV_LOC_USER,
                InstrumentLocation.C_INV_LOC_INSTOCK,
                InstrumentLocation.C_INV_LOC_PRE_ELIMINATION,
                ConfigName.C_CONFIG_SCRAP_VALUE
            );

            return new doBatchProcessResult()
            {
                Result = FlagType.C_FLAG_ON,
                BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED,
                Total = 1,
                Complete = 1,
                Failed = 0,
                ErrorMessage = null,
                BatchUser = strUserId,
                BatchDate = dtBatchDate
            };
        }

        public doBatchProcessResult FreezeInprocess(string strUserId, DateTime? updateDate, DateTime dtBatchDate)
        {
            base.FreezeInprocess(dtBatchDate, null, strUserId);

            return new doBatchProcessResult()
            {
                Result = FlagType.C_FLAG_ON,
                BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED,
                Total = 1,
                Complete = 1,
                Failed = 0,
                ErrorMessage = null,
                BatchUser = strUserId,
                BatchDate = dtBatchDate
            };
        }
    }
}


