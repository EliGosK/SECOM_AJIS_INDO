﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
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
using System.Linq;

namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {
        #region Authority
        /// <summary>
        /// Check screen permission
        /// </summary>
        /// <param name="param">ScreenParameter</param>
        /// <returns></returns>
        public ActionResult IVS100_Authority(IVS100_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IInventoryHandler handInven = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                if (comh.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_REPAIR_REQUEST, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
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
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS100_ScreenParameter>("IVS100", param, res);
        }

        /// <summary>
        /// Initial Screen
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS100")]
        public ActionResult IVS100()
        {

            ViewBag.DestLocation = IVS100_GetDestinationLocation();

            return View();
        }
        #endregion

        /// <summary>
        /// Search instrument information
        /// </summary>
        /// <param name="Cond">Search condition object</param>
        /// <returns></returns>
        public ActionResult IVS100_SearchInventoryInstrument(IVS100SearchCond Cond)
        {
            IInventoryDocumentHandler handler = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;

            ObjectResultData res = new ObjectResultData();
            try
            {    //Valid Cond

                ValidatorUtil.BuildErrorMessage(res, this, new object[] { Cond });
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                IVS100_ScreenParameter param = GetScreenObject<IVS100_ScreenParameter>();

                List<dtSearchInstrumentListResult> lstResult =

                    InvH.SearchInventoryInstrumentList(param.office.OfficeCode,
                    Cond.SourceLoc,
                    Cond.InstArea,
                    ShelfType.C_INV_SHELF_TYPE_NORMAL,
                    null,
                    null,
                    Cond.InstName,
                    Cond.InstCode,
                    new string[] { InstrumentArea.C_INV_AREA_SE_LENDING_DEMO }
                  );

                if (lstResult.Count == 0)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
                else if (lstResult.Count > 1000)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4004);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                res.ResultData = CommonUtil.ConvertToXml<dtSearchInstrumentListResult>(lstResult, "inventory\\IVS100_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Initial instrument grid control
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS100_InstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS100_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial transfer stock-out grid control
        /// </summary>
        /// <returns></returns>
        public ActionResult TransferStockInstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS100_TransferStock", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Get destination location
        /// </summary>
        /// <returns></returns>
        public string IVS100_GetDestinationLocation()
        {
            ObjectResultData res = new ObjectResultData();
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            List<tbs_MiscellaneousTypeCode> ResMisc = new List<tbs_MiscellaneousTypeCode>();
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode(MiscType.C_INV_LOC);

                CommonUtil.MappingObjectLanguage<tbs_MiscellaneousTypeCode>(list);

                ResMisc = (from c in list
                           where (c.ValueCode == InstrumentLocation.C_INV_LOC_REPAIR_REQUEST)
                           select c).ToList<tbs_MiscellaneousTypeCode>();

                if (ResMisc != null && ResMisc.Count > 0)
                    return ResMisc[0].ValueDisplay;
                else
                    return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Validate Register transfer stock-out repairing instrument data
        /// </summary>
        /// <param name="Cond">Register condition object</param>
        /// <returns></returns>
        public ActionResult IVS100_cmdReg(IVS100RegisterCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS100_ScreenParameter prm = GetScreenObject<IVS100_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS100INST>();
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_REPAIR_REQUEST, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //Valid Cond
                //6.1.1
                if (prm.ElemInstrument.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4006);
                    return Json(res);
                }

                prm.ElemInstrument = Cond.StockInInstrument;
                prm.Memo = Cond.Memo;
                prm.Location = Cond.SourceLoc;
                prm.ApproveNo = Cond.ApproveNo;
                prm.RepairSubContractor = Cond.RepairSubContractor;

                //6.1.2 
                foreach (IVS100INST i in prm.ElemInstrument)
                {
                    if (i.StockOutQty <= 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4030, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                    }

                }
                //6.1.3
                if (!string.IsNullOrEmpty(prm.Memo) && prm.Memo.Replace(" ", "").Contains("\n\n\n\n"))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4022, null, new string[] { "memo" });
                    return Json(res);
                }
                //6.1.4
                bool isError = false;
                foreach (IVS100INST i in prm.ElemInstrument)
                {
                    doCheckTransferQty doCheck = new doCheckTransferQty();
                    doCheck.OfficeCode = prm.office.OfficeCode;
                    doCheck.LocationCode = prm.Location;
                    doCheck.AreaCode = i.AreaCode;
                    doCheck.ShelfNo = i.ShelfNo;
                    doCheck.InstrumentCode = i.InstrumentCode;
                    doCheck.TransferQty = i.StockOutQty;
                    doCheckTransferQtyResult doCheckResult = InvH.CheckTransferQty(doCheck);

                    i.InstrumentQty = doCheckResult.CurrentyQty;

                    if (doCheckResult.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        isError = true;
                    }
                    else if (doCheckResult.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        isError = true;
                    }
                }

                res.ResultData = prm.ElemInstrument;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Register transfer stock-out repairing instrument data
        /// </summary>
        /// <param name="Con">Register condition object</param>
        /// <returns></returns>
        public ActionResult IVS100_cmdConfirm(IVS100RegisterCond Con)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {       //Check Suspend
                IVS100_ScreenParameter prm = GetScreenObject<IVS100_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS100INST>();

                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_REPAIR_REQUEST, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                //prm.ElemInstrument = (from x in prm.ElemInstrument
                //                      orderby x.InstrumentCode, x.AreaCode
                //                      select x).ToList();

                foreach (IVS100INST i in prm.ElemInstrument)
                {
                    doCheckTransferQty Cond = new doCheckTransferQty();
                    Cond.OfficeCode = prm.office.OfficeCode;
                    Cond.LocationCode = prm.Location;
                    Cond.AreaCode = i.AreaCode;
                    Cond.ShelfNo = i.ShelfNo;
                    Cond.InstrumentCode = i.InstrumentCode;
                    Cond.TransferQty = i.StockOutQty;

                    doCheckTransferQtyResult TransferQtyResult = InvH.CheckTransferQty(Cond);

                    i.InstrumentQty = TransferQtyResult.CurrentyQty;

                    //8.2.1 
                    if (TransferQtyResult.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        res.ResultData = prm.ElemInstrument;
                        i.IsError = true;
                        return Json(res);
                    }
                    else if (TransferQtyResult.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        res.ResultData = prm.ElemInstrument;
                        i.IsError = true;
                        return Json(res);
                    }
                }

                string strInventorySlipNo = string.Empty;

                using (TransactionScope scope = new TransactionScope())
                {

                    //8.3 
                    doRegisterTransferInstrumentData data = new doRegisterTransferInstrumentData();
                    data.SlipId = SlipID.C_INV_SLIPID_REPAIR_REQUEST;

                    tbt_InventorySlip InvSlip = new tbt_InventorySlip();
                    InvSlip.SlipNo = null;
                    InvSlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER;
                    InvSlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_REPAIR_REQUEST;
                    InvSlip.InstallationSlipNo = null;
                    InvSlip.ProjectCode = null;
                    InvSlip.PurchaseOrderNo = null;
                    InvSlip.ContractCode = null;
                    InvSlip.SlipIssueDate = DateTime.Now;
                    InvSlip.StockInDate = null;
                    InvSlip.StockOutDate = DateTime.Now;
                    InvSlip.SourceLocationCode = prm.Location;
                    InvSlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_REPAIRING;
                    InvSlip.SourceOfficeCode = prm.office.OfficeCode;
                    InvSlip.DestinationOfficeCode = prm.office.OfficeCode;
                    InvSlip.ApproveNo = prm.ApproveNo;
                    InvSlip.Memo = prm.Memo;
                    InvSlip.StockInFlag = null;
                    InvSlip.DeliveryOrderNo = null;
                    InvSlip.ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                    InvSlip.RepairSubcontractor = prm.RepairSubContractor;
                    InvSlip.InstallationCompleteFlag = null;
                    InvSlip.ContractStartServiceFlag = null;
                    InvSlip.CustomerAcceptanceFlag = null;
                    // doSlip.ProjectCompleteFlag=null;
                    InvSlip.PickingListNo = null;
                    InvSlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    InvSlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    InvSlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    InvSlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    data.InventorySlip = InvSlip;
                    data.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();

                    int iRunNo = 1;
                    foreach (IVS100INST i in prm.ElemInstrument)
                    {
                        tbt_InventorySlipDetail SlipDetail = new tbt_InventorySlipDetail();
                        SlipDetail.SlipNo = null;
                        SlipDetail.RunningNo = iRunNo;
                        SlipDetail.InstrumentCode = i.InstrumentCode;
                        SlipDetail.SourceAreaCode = i.AreaCode;
                        SlipDetail.DestinationAreaCode = i.AreaCode;
                        SlipDetail.SourceShelfNo = i.ShelfNo;
                        SlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                        SlipDetail.TransferQty = i.StockOutQty;
                        SlipDetail.NotInstalledQty = null;
                        SlipDetail.RemovedQty = null;
                        SlipDetail.UnremovableQty = null;
                        SlipDetail.InstrumentAmount = null;

                        data.lstTbt_InventorySlipDetail.Add(SlipDetail);
                        iRunNo++;
                    }

                    //8.4
                    strInventorySlipNo = InvH.RegisterTransferInstrument(data);

                    //8.5
                    if (InvH.CheckNewInstrument(strInventorySlipNo) == 1)
                    {
                        //8.5.1
                        List<doGroupNewInstrument> groupNewInstrument = InvH.GetGroupNewInstrument(strInventorySlipNo);
                        foreach (doGroupNewInstrument i in groupNewInstrument)
                        {
                            //8.5.2
                            i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_REPAIR_REQUEST;
                            //8.5.3
                            #region Monthly Price @ 2015
                            decimal decMovingAveragePrice = InvH.GetMonthlyAveragePrice(i.Instrumentcode, InvSlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                            #endregion
                            InvH.UpdateAccountTransferNewInstrument(i, Convert.ToDecimal(decMovingAveragePrice));
                        }
                    }
                    //8.6
                    int blnCheckSecondhandInstrument = InvH.CheckSecondhandInstrument(strInventorySlipNo);
                    if (blnCheckSecondhandInstrument == 1)
                    {
                        //8.6.1
                        List<doGroupSecondhandInstrument> GroupSecondHandInstrument = InvH.GetGroupSecondhandInstrument(strInventorySlipNo);
                        foreach (doGroupSecondhandInstrument i in GroupSecondHandInstrument)
                        {
                            //8.6.2
                            i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_REPAIR_REQUEST;
                            InvH.UpdateAccountTransferSecondhandInstrument(i);
                        }
                    }

                    //8.7
                    int blnCheckSampleInstrument = InvH.CheckSampleInstrument(strInventorySlipNo);
                    if (blnCheckSampleInstrument == 1)
                    {
                        //8.7.1
                        List<doGroupSampleInstrument> GroupSampleInstrument = InvH.GetGroupSampleInstrument(strInventorySlipNo);
                        foreach (doGroupSampleInstrument i in GroupSampleInstrument)
                        {
                            //8.7.2
                            i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_REPAIR_REQUEST;
                            InvH.UpdateAccountTransferSampleInstrument(i, null);
                        }
                    }

                    scope.Complete();
                }

                //8.8.1
                IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                string strReportPath = handlerInventoryDocument.GenerateIVR070FilePath(strInventorySlipNo, prm.office.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                prm.slipNoReportPath = strReportPath;
                prm.slipNo = strInventorySlipNo;

                res.ResultData = strInventorySlipNo;

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Update row index to add instrument data
        /// </summary>
        /// <param name="Cond">instrument data</param>
        /// <returns></returns>
        public ActionResult IVS100_UpdateRowIDElem(IVS100INST Cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IVS100_ScreenParameter prm = GetScreenObject<IVS100_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS100INST>();

                foreach (IVS100INST i in prm.ElemInstrument)
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
        /// Initial screen parameter
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS100_InitParam()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS100_ScreenParameter prm = GetScreenObject<IVS100_ScreenParameter>();
                prm.ElemInstrument = new List<IVS100INST>();
                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Check instrument data before add instrument
        /// </summary>
        /// <param name="Cond">instrument data</param>
        /// <returns></returns>
        public ActionResult IVS100_beforeAddElem(IVS100INST Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS100_ScreenParameter prm = GetScreenObject<IVS100_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS100INST>();


                foreach (IVS100INST i in prm.ElemInstrument)
                {
                    if (i.InstrumentCode == Cond.InstrumentCode && i.AreaCode == Cond.AreaCode)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4005);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }

                prm.ElemInstrument.Add(Cond);

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
        /// Check instrument data before remove instrument
        /// </summary>
        /// <param name="InstrumentCode">Remove selecedt instrument code</param>
        /// <param name="AreaCode">Remove selected  Area code</param>
        /// <returns></returns>
        public ActionResult IVS100_DelElem(string InstrumentCode, string AreaCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS100_ScreenParameter prm = GetScreenObject<IVS100_ScreenParameter>();

                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS100INST>();

                for (int i = 0; i < prm.ElemInstrument.Count; i++)
                    if (prm.ElemInstrument[i].InstrumentCode == InstrumentCode && prm.ElemInstrument[i].AreaCode == AreaCode)
                    {
                        prm.ElemInstrument.RemoveAt(i);
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
        /// Check exists slip report
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS100_CheckExistFile()
        {
            try
            {
                IVS100_ScreenParameter param = GetScreenObject<IVS100_ScreenParameter>();
                string path = param.slipNoReportPath;
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
        /// Download slip report
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS100_DownloadPdfAndWriteLog()
        {
            try
            {

                IVS100_ScreenParameter param = GetScreenObject<IVS100_ScreenParameter>();
                string fileName = param.slipNoReportPath;

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = param.slipNo,
                    DocumentCode = ReportID.C_INV_REPORT_ID_REPAIR_REQUEST, // IVR070
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