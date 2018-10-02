using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Inventory.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Transactions;
using System.IO;


namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {

        /// <summary>
        /// - Check system suspending.<br />
        /// - Check user permission for screen IVS140.<br />
        /// - Check freezed data.<br />
        /// - Check started stock checking.<br />
        /// - Get inventory head office.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS140_Authority(IVS140_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // Is suspend ?
                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check permission
                if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_CHECKING_RETURN, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                // Check freezed data
                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                if (handlerInventory.CheckFreezedData() == 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4002);
                    return Json(res);
                }

                // Check for the stock is started
                if (handlerInventory.CheckStartedStockChecking() == 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    return Json(res);
                }

                //Get Inventory head office
                List<doOffice> headerOffice = handlerInventory.GetInventoryHeadOffice();
                param.HeaderOffice = headerOffice;

                if (headerOffice == null || headerOffice.Count <= 0 || CommonUtil.IsNullOrEmpty(headerOffice[0].OfficeCode))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4016);
                    return Json(res);
                }
                

                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<string> listFieldName = new List<string>();
                listFieldName.Add(MiscType.C_INV_LOC);
                List<doMiscTypeCode> listMisc = handlerCommon.GetMiscTypeCodeListByFieldName(listFieldName); // This result has language mapping already

                param.Miscellaneous = listMisc;

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS140_ScreenParameter>("IVS140", param, res);
        }

        /// <summary>
        /// Get config for instrument list table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS140_InitialSearchResultGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS140_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get config for checking instrument list table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS140_InitialAdjustmentDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS140_AdjustmentDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS140")]
        public ActionResult IVS140()
        {
            IVS140_ScreenParameter param = GetScreenObject<IVS140_ScreenParameter>();

            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            param.Miscellaneous = CommonUtil.ConvertObjectbyLanguage<doMiscTypeCode, doMiscTypeCode>(param.Miscellaneous, "ValueDisplay");
            ViewBag.SourceLocation = handlerCommon.GetMiscDisplayValue(param.Miscellaneous, MiscType.C_INV_LOC, InstrumentLocation.C_INV_LOC_RETURNED);
            ViewBag.DestinationLocation = handlerCommon.GetMiscDisplayValue(param.Miscellaneous, MiscType.C_INV_LOC, InstrumentLocation.C_INV_LOC_INSTOCK);

            return View();
        }

        /// <summary>
        /// Search instrument in location returned w/h.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS140_SearchResponse(doSearchInstrumentListCondition cond)
        {

            List<dtSearchInstrumentListResult> list = new List<dtSearchInstrumentListResult>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                //Validate #1
                if (CommonUtil.IsNullAllField(cond))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }

                // Business validate

                IVS140_ScreenParameter param = GetScreenObject<IVS140_ScreenParameter>();

                if (param != null)
                {
                    // Prepare search condition
                    cond.OfficeCode = param.HeaderOffice[0].OfficeCode;
                    cond.LocationCode = InstrumentLocation.C_INV_LOC_RETURNED;
                    cond.ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                    cond.ExcludeAreaCode = new List<string>() { InstrumentArea.C_INV_AREA_SE_LENDING_DEMO };

                    IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    list = handlerInventory.SearchInventoryInstrumentList(cond);
                }

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtSearchInstrumentListResult>(list, "Inventory\\IVS140_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);


        }

        // Register click
        //[PermissionOperationAttibute("","")]
        /// <summary>
        /// Validate before register.<br />
        /// - Check system suspending.<br />
        /// - Check user permission.<br />
        /// - Check detail not empty.<br />
        /// - Check memo.<br />
        /// - Check quantity.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult IVS140_Register(IVS140_RegisterData data)
        {

            IVS140_ScreenParameter param = GetScreenObject<IVS140_ScreenParameter>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                // Is suspend ?
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handlerCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check permission
                if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_CHECKING_RETURN, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //Validate #1
                if (data.Detail == null || data.Detail.Count == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4006);
                    return Json(res);
                }

                //Validate #2
                data.Header.Memo = data.Header.Memo == null ? "" : data.Header.Memo;
                if (data.Header.Memo.Replace(" ", "").Contains("\n\n\n\n"))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4022, null, new string[] { "Memo" });
                    return Json(res);
                }

                //Validate #3
                List<string> lstWarningAtControls = new List<string>();
                foreach (var item in data.Detail)
                {
                    if (item.FixedReturnQty.GetValueOrDefault(0) <= 0)
                    {
                        lstWarningAtControls.Add(item.txtFixedReturnQtyID);
                    }
                }
                if (lstWarningAtControls.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4071, null, lstWarningAtControls.ToArray());
                    return Json(res);
                }

                List<dtSearchInstrumentListResult> lstUpdatedCurrent = new List<dtSearchInstrumentListResult>();

                //Business validate
                int countBusinessWaringing = 0;
                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                foreach (var item in data.Detail)
                {
                    doCheckTransferQty doCheck = new doCheckTransferQty()
                    {
                        OfficeCode = param.HeaderOffice[0].OfficeCode,
                        LocationCode = InstrumentLocation.C_INV_LOC_RETURNED,
                        AreaCode = item.AreaCode,
                        ShelfNo = item.ShelfNo,
                        InstrumentCode = item.Instrumentcode,
                        TransferQty = item.FixedReturnQty.HasValue ? item.FixedReturnQty.Value : 0
                    };

                    doCheckTransferQtyResult result = handlerInventory.CheckTransferQty(doCheck);

                    item.InstrumentQty = result.CurrentyQty;
                    
                    if (result.OverQtyFlag == null)
                    {
                        item.InstrumentQty = 0;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtFixedReturnQtyID });
                        countBusinessWaringing++;
                    }
                    else if (result.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtFixedReturnQtyID });
                        countBusinessWaringing++;
                    }

                }

                if (countBusinessWaringing > 0)
                {
                    res.ResultData = new
                    {
                        IsSuccess = false,
                        Data = data
                    };
                    return Json(res);
                }

                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = new
            {
                IsSuccess = true,
                Data = data
            };
            return Json(res);
        }

        /// <summary>
        /// Register checking returned instrument.<br />
        /// - Check system suspending.<br />
        /// - Check quantity.<br />
        /// - Register transfer instrument.<br />
        /// - Update account transfer new/second hand/sample instrument.<br />
        /// - Generate report.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS140_Confirm()
        {
            string slipNo = string.Empty;
            string slipNoReportPath = string.Empty;
            IVS140_ScreenParameter param = GetScreenObject<IVS140_ScreenParameter>();
            IVS140_RegisterData data = new IVS140_RegisterData();
            if (param != null)
            {
                data = param.RegisterData;
            }

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                // Is suspend ?
                if (handlerCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                //Business validate
                //int countBusinessWaringing = 0;
                foreach (var item in data.Detail)
                {
                    doCheckTransferQty doCheck = new doCheckTransferQty()
                    {
                        OfficeCode = param.HeaderOffice[0].OfficeCode,
                        LocationCode = InstrumentLocation.C_INV_LOC_RETURNED,
                        AreaCode = item.AreaCode,
                        ShelfNo = item.ShelfNo,
                        InstrumentCode = item.Instrumentcode,
                        TransferQty = item.FixedReturnQty.HasValue ? item.FixedReturnQty.Value : 0
                    };


                    doCheckTransferQtyResult result = handlerInventory.CheckTransferQty(doCheck);

                    item.InstrumentQty = result.CurrentyQty;

                    if (result.OverQtyFlag == null)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtFixedReturnQtyID });
                        res.ResultData = new
                        {
                            IsSuccess = false,
                            Data = data
                        };
                        return Json(res);
                        //countBusinessWaringing++;
                    }
                    else if (result.OverQtyFlag == true)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtFixedReturnQtyID });
                        res.ResultData = new
                        {
                            IsSuccess = false,
                            Data = data
                        };
                        return Json(res);
                        //countBusinessWaringing++;
                    }
                }
                //if (countBusinessWaringing > 0)
                //{
                //    return Json(res);
                //}


                // Prepare data for Register
                doRegisterTransferInstrumentData dsRegisterData = new doRegisterTransferInstrumentData();

                dsRegisterData.SlipId = SlipID.C_INV_SLIPID_CHECKING_RETURNED;
                dsRegisterData.InventorySlip = new tbt_InventorySlip()
                {
                    SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE,
                    TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_CHECKING_RETURNED,
                    SlipIssueDate = DateTime.Now,
                    StockInDate = DateTime.Now,
                    StockOutDate = DateTime.Now,
                    SourceLocationCode = InstrumentLocation.C_INV_LOC_RETURNED,
                    DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK,
                    SourceOfficeCode = param.HeaderOffice[0].OfficeCode,
                    DestinationOfficeCode = param.HeaderOffice[0].OfficeCode,
                    ApproveNo = data.Header.ApproveNo,
                    Memo = data.Header.Memo,
                    ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL,
                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                };

                dsRegisterData.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
                tbt_InventorySlipDetail detail;
                for (int i = 0; i < data.Detail.Count; i++)
                {
                    detail = new tbt_InventorySlipDetail()
                    {
                        RunningNo = (i + 1),
                        InstrumentCode = data.Detail[i].Instrumentcode,
                        SourceAreaCode = data.Detail[i].AreaCode,
                        DestinationAreaCode = data.Detail[i].AreaCode,
                        SourceShelfNo = data.Detail[i].ShelfNo,
                        DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF,
                        TransferQty = data.Detail[i].FixedReturnQty
                    };

                    dsRegisterData.lstTbt_InventorySlipDetail.Add(detail);
                }

                // TODO: (Narupon) Uncomment for use TransactionScope

                // Save data to database..
                using (TransactionScope scope = new TransactionScope())
                {

                    try
                    {
                        // Register Inventory Tranfer data
                        string slipNo_result = handlerInventory.RegisterTransferInstrument(dsRegisterData);
                        slipNo = slipNo_result;
                        param.SlipNo = slipNo_result;

                        // Check New instrument -- 8.5
                        int bCheckingNewInstrument = handlerInventory.CheckNewInstrument(slipNo_result);
                        if (bCheckingNewInstrument == 1)
                        {
                            List<doGroupNewInstrument> dtGroupNewInstrument = handlerInventory.GetGroupNewInstrument(slipNo_result);
                            foreach (var item in dtGroupNewInstrument)
                            {
                                #region Monthly Price @ 2015
                                //decimal decMovingAveragePrice = handlerInventory.CalculateMovingAveragePrice(item);
                                decimal decMovingAveragePrice = handlerInventory.GetMonthlyAveragePrice(item.Instrumentcode, dsRegisterData.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                                #endregion
                                handlerInventory.UpdateAccountTransferNewInstrument(item, decMovingAveragePrice);
                            }
                        }

                        // Check secondhand instrument -- 8.6
                        int bCheckSeconhandInstrument = handlerInventory.CheckSecondhandInstrument(slipNo_result);
                        if (bCheckSeconhandInstrument == 1)
                        {
                            List<doGroupSecondhandInstrument> dtGroupSecondhandInstrument = handlerInventory.GetGroupSecondhandInstrument(slipNo_result);
                            foreach (var item in dtGroupSecondhandInstrument)
                            {
                                handlerInventory.UpdateAccountTransferSecondhandInstrument(item);
                            }
                        }


                        // Check sample instrument -- 8.6
                        int bCheckSampleInstrument = handlerInventory.CheckSampleInstrument(slipNo_result);
                        if (bCheckSampleInstrument == 1)
                        {
                            List<doGroupSampleInstrument> dtGroupSampleInstrument = handlerInventory.GetGroupSampleInstrument(slipNo_result);
                            foreach (var item in dtGroupSampleInstrument)
                            {
                                handlerInventory.UpdateAccountTransferSampleInstrument(item, null);
                            }
                        }

                        // Generate inventory slip report  // ReportID.C_INV_REPORT_ID_CHECKING_RETURNED = IVR090
                        IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                        slipNoReportPath = handlerInventoryDocument.GenerateIVR090FilePath(slipNo_result, param.HeaderOffice[0].OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);


                        scope.Complete(); // Commit transtion.
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        throw ex;
                    }

                }
                // TODO: (Narupon) Uncomment for use TransactionScope

                param.SlipNoReportPath = slipNoReportPath;

                res.ResultData = new
                {
                    IsSuccess = true,
                    Data = data,
                    SlipNo = slipNo
                };
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);

        }

        /// <summary>
        /// Check is report file exist.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS140_CheckExistFile()
        {
            try
            {
                IVS140_ScreenParameter param = GetScreenObject<IVS140_ScreenParameter>();
                string path = param.SlipNoReportPath;
                if (System.IO.File.Exists(path) == true)
                {
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }
            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Download report and write log.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS140_DownloadPdfAndWriteLog()
        {
            try
            {

                IVS140_ScreenParameter param = GetScreenObject<IVS140_ScreenParameter>();
                string fileName = param.SlipNoReportPath;

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = param.SlipNo,
                    DocumentCode = ReportID.C_INV_REPORT_ID_CHECKING_RETURNED, // IVR090
                    DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT,
                    DownloadDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    DownloadBy = CommonUtil.dsTransData.dtUserData.EmpNo
                };


                ILogHandler handlerLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                int isSuccess = handlerLog.WriteDocumentDownloadLog(doDownloadLog);

                IDocumentHandler handlerDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                Stream reportStream = handlerDoc.GetDocumentReportFileStream(fileName);

                return File(reportStream, "application/pdf");


            }
            catch (Exception ex)
            {
                ObjectResultData res = new ObjectResultData();
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }
    }

}


