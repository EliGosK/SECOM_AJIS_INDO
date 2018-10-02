using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Inventory.Models;
using System.Transactions;
using System.IO;


namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {
        #region Authority

        /// <summary>
        /// - Check system suspending.<br />
        /// - Check user permission for screen IVS060.<br />
        /// - Check freezed data.<br />
        /// - Check started stock checking.<br />
        /// - Get inventory head office.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS060_Authority(IVS060_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (comh.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_TRANSFER_OFFICE, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                IInventoryHandler handInven = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                if (handInven.CheckFreezedData() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4002);
                    return Json(res);
                }
                if (handInven.CheckStartedStockChecking() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    return Json(res);
                }

                List<doOffice> IvHeadOffice = handInven.GetInventoryHeadOffice();

                if (IvHeadOffice.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4016);
                    return Json(res);
                }

                param.office = IvHeadOffice[0];

                List<doOffice> SrinakarinOffice = handInven.GetInventorySrinakarinOffice();
                if (SrinakarinOffice.Count > 0)
                {
                    param.SrinakarinOfficeCode = SrinakarinOffice[0];
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS060_ScreenParameter>("IVS060", param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS060")]
        public ActionResult IVS060()
        {
            IVS060_ScreenParameter param = GetScreenObject<IVS060_ScreenParameter>();
            ViewBag.HeadOfficeCode = param.office.OfficeCode;
            ViewBag.SrinakarinOfficeCode = param.SrinakarinOfficeCode.OfficeCode;

            ViewBag.Eliminate = InstrumentLocation.C_INV_LOC_ELIMINATION;
            ViewBag.PreEliminate = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
            ViewBag.Total = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INVENTORY, ScreenID.C_INV_SCREEN_ID_PRE_ELIMINATION, "lblTotalAmountOfTransferAsset");

            return View();
        }
        #endregion

        /// <summary>
        /// Search inventory instrument.
        /// </summary>
        /// <param name="Cond"></param>
        /// <param name="SourceOffice"></param>
        /// <param name="IntAreaComboData"></param>
        /// <returns></returns>
        public ActionResult IVS060_SearchInventoryInstrumentList(IVS060_SearchInstCond Cond, string SourceOffice, string IntAreaComboData)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS060_ScreenParameter prm = GetScreenObject<IVS060_ScreenParameter>();

                if (prm.lstTransferInstrument == null)
                    prm.lstTransferInstrument = new List<IVS060INST>();

                ValidatorUtil.BuildErrorMessage(res, this, new object[] { Cond });
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                if (Cond.SourceOffice == Cond.DestinationOffice)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4014, null, new string[] { "SourceOffice", "DestinationOffice" });
                    return Json(res);
                }

                prm.SourceOffice = SourceOffice;
                prm.DestinationOffice = Cond.DestinationOffice;

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                doSearchInstrumentListCondition SearchCond = new doSearchInstrumentListCondition();
                SearchCond.OfficeCode = Cond.SourceOffice;
                SearchCond.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                if (Cond.AreaCode == null)
                    SearchCond.AreaCode = IntAreaComboData;
                else
                    SearchCond.AreaCode = Cond.AreaCode;
                SearchCond.ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                SearchCond.Instrumentcode = Cond.InstrumentCode;
                SearchCond.InstrumentName = Cond.InstrumentName;
                List<dtSearchInstrumentListResult> lstResult = InvH.SearchInventoryInstrumentList(SearchCond);

                if (lstResult.Count <= 0) {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                }

                UpdateScreenObject(prm);
                res.ResultData = CommonUtil.ConvertToXml<dtSearchInstrumentListResult>(lstResult, "Inventory\\IVS060_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        #region init
        /// <summary>
        /// Initial screen parameter.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS060_InitParam()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS060_ScreenParameter prm = GetScreenObject<IVS060_ScreenParameter>();
                prm.lstTransferInstrument = new List<IVS060INST>();
                prm.Memo = "";
                prm.SourceOffice = "";
                prm.DestinationOffice = "";
                prm.PrevInstTransferArr = new string[][] { };
                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Get config for Transfer Instrument table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS060_TransGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS060_Transfer", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get config for Instrument List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS060_InstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS060_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }
        #endregion

        /// <summary>
        /// Validate before add instrument.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult IVS060_CheckBeforeAdd(IVS060INST cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS060_ScreenParameter prm = GetScreenObject<IVS060_ScreenParameter>();
                if (prm.lstTransferInstrument == null)
                    prm.lstTransferInstrument = new List<IVS060INST>();

                foreach (IVS060INST i in prm.lstTransferInstrument)
                {
                    if (i.InstrumentCode == cond.InstrumentCode && i.AreaCode == cond.AreaCode)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4005);
                        return Json(res);
                    }
                }
                prm.lstTransferInstrument.Add(cond);
                UpdateScreenObject(prm);
                res.ResultData = true;
                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Remove selected instrument from table.
        /// </summary>
        /// <param name="InstrumentCode"></param>
        /// <param name="AreaCode"></param>
        /// <returns></returns>
        public ActionResult IVS060_DelElem(string InstrumentCode, string AreaCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS060_ScreenParameter prm = GetScreenObject<IVS060_ScreenParameter>();

                if (prm.lstTransferInstrument == null)
                    prm.lstTransferInstrument = new List<IVS060INST>();

                for (int i = 0; i < prm.lstTransferInstrument.Count; i++)
                    if (prm.lstTransferInstrument[i].InstrumentCode == InstrumentCode && prm.lstTransferInstrument[i].AreaCode == AreaCode)
                    {
                        prm.lstTransferInstrument.RemoveAt(i);
                        break;
                    }

                UpdateScreenObject(prm);
                res.ResultData = true;

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Validate before confirm screen.<br />
        /// - Check require field.<br />
        /// - Check instrument list not empty.<br />
        /// - Check memo.
        /// - Check quantity.
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns></returns>
        public ActionResult IVS060_cmdReg(IVS060TransferDO transfer)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS060_ScreenParameter prm = GetScreenObject<IVS060_ScreenParameter>();

                if (prm.lstTransferInstrument == null)
                    prm.lstTransferInstrument = new List<IVS060INST>();

                if (prm.lstTransferInstrument.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4006);
                    return Json(res);
                }

                if (transfer.memo != null)
                {
                    if (transfer.memo.Replace(" ", "").Contains("\n\n\n\n"))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4022, null , new string[] {"memo"});
                        return Json(res);
                    }
                }
                prm.Memo = transfer.memo;                

                bool isError = false;

                List<string> controls = new List<string>();
                bool hasTransfer = true;
                foreach (var i in transfer.transferQTYList) {
                    if (!i.transferQTY.HasValue || i.transferQTY.Value == 0)
                    {
                        controls.Add(i.controlID);
                        hasTransfer = false;
                    }
                    if (hasTransfer)
                    {
                        foreach (var instrument in prm.lstTransferInstrument)
                        {
                            if (instrument.InstrumentCode == i.instrumentCode && instrument.AreaCode == i.areaCode)
                            {
                                instrument.TransferInstrumentQty = i.transferQTY.Value;
                                instrument.TransQtyID = i.controlID;
                                continue;
                            }
                        }
                    }
                }
                if (controls.Count > 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(
                        MessageUtil.MODULE_INVENTORY,
                        ScreenID.C_INV_SCREEN_ID_TRANSFER_OFFICE,
                        MessageUtil.MODULE_INVENTORY,
                        MessageUtil.MessageList.MSG4029,
                        new string[] { "headerTransferInstrumentQuantity" },
                        controls.ToArray());

                    isError = true;
                    //return Json(res);
                }

                
                foreach (IVS060INST i in prm.lstTransferInstrument)
                {
                    doCheckTransferQty checkQty = new doCheckTransferQty();
                    checkQty.OfficeCode = prm.SourceOffice;
                    checkQty.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                    checkQty.AreaCode = i.AreaCode;
                    checkQty.ShelfNo = i.ShelfNo;
                    checkQty.InstrumentCode = i.InstrumentCode;
                    checkQty.TransferQty = i.TransferInstrumentQty;
                    IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    doCheckTransferQtyResult resCheckQty = InvH.CheckTransferQty(checkQty);
                    i.InstrumentQty = resCheckQty.CurrentyQty;

                    if (resCheckQty.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.InstrumentCode }, new string[] { i.TransQtyID });
                        isError = true;
                    }
                    else if (resCheckQty.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.InstrumentCode }, new string[] { i.TransQtyID });
                        isError = true;
                    }
                }

                if (isError)
                {
                    UpdateScreenObject(prm);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = prm.lstTransferInstrument;
                    return Json(res);
                }

                UpdateScreenObject(prm);
                res.ResultData = prm.lstTransferInstrument;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Update row id of instrument in screen.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS060_UpdateRowIDTransfer(IVS060INST Cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IVS060_ScreenParameter prm = GetScreenObject<IVS060_ScreenParameter>();
                if (prm.lstTransferInstrument == null)
                    prm.lstTransferInstrument = new List<IVS060INST>();

                foreach (IVS040INST i in prm.lstTransferInstrument)
                {
                    if (Cond.InstrumentCode == i.InstrumentCode && Cond.AreaCode == i.AreaCode)
                    {
                        i.row_id = Cond.row_id;
                        break;
                    }
                }
                UpdateScreenObject(prm);

                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Register pre-elimination.<br />
        /// - Check system suspending.<br />
        /// - Check permission.<br />
        /// - Check quantity.<br />
        /// - Insert inventory slip.<br />
        /// - Update account transfer new/second hand/sample instrument.<br />
        /// - Generate report.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS060_cmdConfirm()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {       //Check Suspend

                IVS060_ScreenParameter prm = GetScreenObject<IVS060_ScreenParameter>();
                if (prm.lstTransferInstrument == null)
                    prm.lstTransferInstrument = new List<IVS060INST>();
                foreach (IVS060INST i in prm.lstTransferInstrument)
                    i.IsError = false;

                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_TRANSFER_OFFICE, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                //8.2.1
                foreach (IVS040INST i in prm.lstTransferInstrument)
                {
                    doCheckTransferQty Cond = new doCheckTransferQty();
                    Cond.OfficeCode = prm.SourceOffice;
                    Cond.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                    Cond.AreaCode = i.AreaCode;
                    Cond.ShelfNo = i.ShelfNo;
                    Cond.InstrumentCode = i.InstrumentCode;
                    Cond.TransferQty = i.TransferInstrumentQty;

                    doCheckTransferQtyResult TransferQtyResult = InvH.CheckTransferQty(Cond);
                    i.InstrumentQty = TransferQtyResult.CurrentyQty;
                    
                    if (TransferQtyResult.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.InstrumentCode }, new string[] { i.TransQtyID });
                         res.ResultData = prm.lstTransferInstrument;
                        i.IsError = true;
                        return Json(res);
                    }
                    else if (TransferQtyResult.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.InstrumentCode }, new string[] { i.TransQtyID });
                        res.ResultData = prm.lstTransferInstrument;
                        i.IsError = true;
                        return Json(res);
                    }
                }

                string strInventorySlipNo = null;
                using (TransactionScope scope = new TransactionScope())
                {
                    //8.3
                    doRegisterTransferInstrumentData data = new doRegisterTransferInstrumentData();
                    data.SlipId = SlipID.C_INV_SLIPID_TRANSFER_OFFICE;
                    IOfficeMasterHandler OffH = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                    bool blnInventoryHeadOfficeFlag = OffH.CheckInventoryHeadOffice(prm.DestinationOffice);
                    string strDestinationShelfNo = "";
                    if (blnInventoryHeadOfficeFlag)
                        strDestinationShelfNo = ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF;
                    else
                        strDestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;

                    data.InventorySlip = new tbt_InventorySlip();
                    tbt_InventorySlip InvSlip = new tbt_InventorySlip();
                    InvSlip.SlipNo = null;
                    InvSlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER;
                    InvSlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_TRANSFER_OFFICE;
                    InvSlip.InstallationSlipNo = null;
                    InvSlip.ProjectCode = null;
                    InvSlip.PurchaseOrderNo = null;
                    InvSlip.ContractCode = null;
                    InvSlip.SlipIssueDate = DateTime.Now;
                    InvSlip.StockInDate = null;
                    InvSlip.StockOutDate = DateTime.Now;
                    InvSlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                    InvSlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                    InvSlip.SourceOfficeCode = prm.SourceOffice;
                    InvSlip.DestinationOfficeCode = prm.DestinationOffice;
                    InvSlip.ApproveNo = null;
                    InvSlip.Memo = prm.Memo;
                    InvSlip.StockInFlag = null;
                    InvSlip.DeliveryOrderNo = null;
                    InvSlip.ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                    InvSlip.RepairSubcontractor = null;
                    InvSlip.InstallationCompleteFlag = null;
                    InvSlip.ContractStartServiceFlag = null;
                    InvSlip.CustomerAcceptanceFlag = null;
                    //InvSlip.ProjectCompleteFlag=null;
                    InvSlip.PickingListNo = null;
                    InvSlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    InvSlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    InvSlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    InvSlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    data.InventorySlip = InvSlip;

                    data.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
                    int iRunNo = 1;
                    foreach (IVS040INST i in prm.lstTransferInstrument)
                    {
                        tbt_InventorySlipDetail SlipDetail = new tbt_InventorySlipDetail();
                        SlipDetail.SlipNo = null;
                        SlipDetail.RunningNo = iRunNo;
                        SlipDetail.InstrumentCode = i.InstrumentCode;
                        SlipDetail.SourceAreaCode = i.AreaCode;
                        SlipDetail.DestinationAreaCode = i.AreaCode;
                        SlipDetail.SourceShelfNo = i.ShelfNo;
                        SlipDetail.DestinationShelfNo = strDestinationShelfNo;
                        SlipDetail.TransferQty = i.TransferInstrumentQty;
                        SlipDetail.NotInstalledQty = null;
                        SlipDetail.RemovedQty = null;
                        SlipDetail.UnremovableQty = null;
                        SlipDetail.InstrumentAmount = null;

                        data.lstTbt_InventorySlipDetail.Add(SlipDetail);
                        iRunNo++;
                    }
                    strInventorySlipNo = InvH.RegisterTransferInstrument(data);
                    if (InvH.CheckNewInstrument(strInventorySlipNo) == 1)
                    {
                        //8.5.1
                        List<doGroupNewInstrument> groupNewInstrument = InvH.GetGroupNewInstrument(strInventorySlipNo);
                        foreach (doGroupNewInstrument i in groupNewInstrument)
                        {
                            i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_TRANSFER;
                            #region Monthly Price @ 2015
                            //decimal decMovingAveragePrice = InvH.CalculateMovingAveragePrice(i);
                            var decMovingAveragePrice = InvH.GetMonthlyAveragePrice(i.Instrumentcode, InvSlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                            #endregion
                            InvH.UpdateAccountTransferNewInstrument(i, null);
                        }
                    }

                    //8.6
                    int blnCheckSecondhandInstrument = InvH.CheckSecondhandInstrument(strInventorySlipNo);
                    if (blnCheckSecondhandInstrument == 1)
                    {
                        List<doGroupSecondhandInstrument> GroupSecondHandInstrument = InvH.GetGroupSecondhandInstrument(strInventorySlipNo);
                        foreach (doGroupSecondhandInstrument i in GroupSecondHandInstrument)
                        {
                            i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_TRANSFER;
                            InvH.UpdateAccountTransferSecondhandInstrument(i);
                        }
                    }

                    //8.7
                    int blnCheckSampleInstrument = InvH.CheckSampleInstrument(strInventorySlipNo);
                    if (blnCheckSampleInstrument == 1)
                    {
                        List<doGroupSampleInstrument> GroupSampleInstrument = InvH.GetGroupSampleInstrument(strInventorySlipNo);
                        foreach (doGroupSampleInstrument i in GroupSampleInstrument)
                        {
                            i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_TRANSFER;
                            InvH.UpdateAccountTransferSampleInstrument(i, null);
                        }
                    }

                    scope.Complete();
                } //end transaction scope

                //8.8
                IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                string reportPath = handlerInventoryDocument.GenerateIVR040FilePath(strInventorySlipNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                prm.slipNo = strInventorySlipNo;
                prm.reportFilePath = reportPath;
                UpdateScreenObject(prm);

                res.ResultData = strInventorySlipNo;

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Check is report file exist.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS060_CheckExistFile()
        {
            try
            {
                IVS060_ScreenParameter prm = GetScreenObject<IVS060_ScreenParameter>();
                string path = prm.reportFilePath;
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
        public ActionResult IVS060_DownloadPdfAndWriteLog()
        {
            try
            {
                IVS060_ScreenParameter prm = GetScreenObject<IVS060_ScreenParameter>();
                string fileName = prm.reportFilePath;

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = prm.slipNo,
                    DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT,
                    DocumentCode = ReportID.C_INV_REPORT_ID_TRANSFER_OFFICE,
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