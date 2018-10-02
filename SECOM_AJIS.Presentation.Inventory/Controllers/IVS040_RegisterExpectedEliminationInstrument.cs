
using System;
using System.Collections.Generic;
using System.Collections;
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


namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {
        #region Authority

        /// <summary>
        /// - Check system suspending.<br />
        /// - Check user permission for screen IVS040.<br />
        /// - Check freezed data.<br />
        /// - Check started stock checking.<br />
        /// - Get inventory head office.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS040_Authority(IVS040_ScreenParameter param)
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
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_PRE_ELIMINATION, FunctionID.C_FUNC_ID_OPERATE))
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
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS040_ScreenParameter>("IVS040", param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS040")]
        public ActionResult IVS040()
        {
            ViewBag.Total = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INVENTORY, ScreenID.C_INV_SCREEN_ID_PRE_ELIMINATION, "lblTotalAmountOfTransferAsset");

            ViewBag.LOC_InStock = InstrumentLocation.C_INV_LOC_INSTOCK;
            ViewBag.LOC_PreEliminate = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
            ViewBag.LOC_Return = InstrumentLocation.C_INV_LOC_RETURNED;
            return View();
        }
        #endregion

        /// <summary>
        /// Search instrument list.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult SearchInventoryInstrumentList(IVS040SearchCond Cond)
        {
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
                IVS040_ScreenParameter param = GetScreenObject<IVS040_ScreenParameter>();
                List<dtSearchInstrumentListResult> lstResult =
                    InvH.SearchInventoryInstrumentList(param.office.OfficeCode,
                    Cond.SourceLoc,
                    Cond.AreaCode,
                    ShelfType.C_INV_SHELF_TYPE_NORMAL,
                    null,
                    null,
                    Cond.InstrumentName,
                    Cond.InstrumentCode,
                    new string[] { InstrumentArea.C_INV_AREA_SE_LENDING_DEMO }
                  );

                if (lstResult.Count > 1000)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4004);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                param.DestinationLocation = Cond.DestinationLoc;

                res.ResultData = CommonUtil.ConvertToXml<dtSearchInstrumentListResult>(lstResult, "Inventory\\IVS040_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Get config for Instrument Pre-elimination table.
        /// </summary>
        /// <returns></returns>
        public ActionResult EliminateInstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS040_Elem", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Get config for Instrument Pre-elimination Confirm table.
        /// </summary>
        /// <returns></returns>
        public ActionResult EliminateInstConfirmGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS040_ElemConfirm", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Get config for Instrument List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS040_InstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS040_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get Pre-elimination Location for combobox.
        /// </summary>
        /// <param name="SourceLoc"></param>
        /// <returns></returns>
        public ActionResult IVS040_GetPreEliminationLocationCbo(string SourceLoc)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> dtMisc = new List<doMiscTypeCode>();

                if (SourceLoc == InstrumentLocation.C_INV_LOC_INSTOCK || SourceLoc == InstrumentLocation.C_INV_LOC_RETURNED)
                {
                    doMiscTypeCode misc = new doMiscTypeCode();
                    misc.FieldName = MiscType.C_INV_LOC;
                    misc.ValueCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
                    dtMisc.Add(misc);
                }
                else if (SourceLoc == InstrumentLocation.C_INV_LOC_PRE_ELIMINATION)
                {
                    doMiscTypeCode misc = new doMiscTypeCode();
                    misc.FieldName = MiscType.C_INV_LOC;
                    misc.ValueCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                    dtMisc.Add(misc);
                    misc = new doMiscTypeCode();
                    misc.FieldName = MiscType.C_INV_LOC;
                    misc.ValueCode = InstrumentLocation.C_INV_LOC_RETURNED;
                    dtMisc.Add(misc);
                }
                else
                {
                    doMiscTypeCode misc = new doMiscTypeCode();
                    misc.FieldName = MiscType.C_INV_LOC;
                    misc.ValueCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                    dtMisc.Add(misc);
                    misc = new doMiscTypeCode();
                    misc.FieldName = MiscType.C_INV_LOC;
                    misc.ValueCode = InstrumentLocation.C_INV_LOC_RETURNED;
                    dtMisc.Add(misc);
                    misc = new doMiscTypeCode();
                    misc.FieldName = MiscType.C_INV_LOC;
                    misc.ValueCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
                    dtMisc.Add(misc);
                }

                List<doMiscTypeCode> ResMisc = new List<doMiscTypeCode>();

                ResMisc = comh.GetMiscTypeCodeList(dtMisc);

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(ResMisc, "ValueCodeDisplay", "ValueCode");
                res.ResultData = cboModel;

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        //public ActionResult IVS040_GetSourceLocationCbo(string SourceLoc)
        //{
        //    ObjectResultData res = new ObjectResultData();
        //    try
        //    {
        //        ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        List<string> lst = new List<string>();
        //        List<doMiscTypeCode> dtMisc = new List<doMiscTypeCode>();
        //        List<doMiscTypeCode> ResMisc = new List<doMiscTypeCode>();
        //        lst.Add(MiscType.C_INV_LOC);
        //        dtMisc = comh.GetMiscTypeCodeListByFieldName(lst);

        //        if (SourceLoc == InstrumentLocation.C_INV_LOC_INSTOCK || SourceLoc == InstrumentLocation.C_INV_LOC_RETURNED)
        //            ResMisc = (from c in dtMisc
        //                       where (c.ValueCode == InstrumentLocation.C_INV_LOC_INSTOCK ||
        //                           c.ValueCode == InstrumentLocation.C_INV_LOC_RETURNED)
        //                           && c.ValueCode != SourceLoc
        //                       select c).ToList<doMiscTypeCode>();
        //        else if (SourceLoc != null)
        //            ResMisc = (from c in dtMisc
        //                       where (c.ValueCode == InstrumentLocation.C_INV_LOC_INSTOCK ||
        //                           c.ValueCode == InstrumentLocation.C_INV_LOC_RETURNED || c.ValueCode == InstrumentLocation.C_INV_LOC_PRE_ELIMINATION) &&
        //                           (c.ValueCode != SourceLoc)
        //                       select c).ToList<doMiscTypeCode>();
        //        else
        //            ResMisc = dtMisc;

        //        //res.ResultData = CommonUtil.CommonComboBox<doMiscTypeCode>("BLANKID", ResMisc, "ValueCodeDisplay", "ValueCode", null, true).ToString();
        //        ComboBoxModel cboModel = new ComboBoxModel();
        //        cboModel.SetList<doMiscTypeCode>(ResMisc, "ValueCodeDisplay", "ValueCode");
        //        res.ResultData = cboModel;

        //        return Json(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        res.AddErrorMessage(ex); return Json(res);
        //    }
        //}

        /// <summary>
        /// Validate before confirm screen.<br />
        /// - Check require field.<br />
        /// - Check instrument list not empty.<br />
        /// - Check memo.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS040_cmdReg(IVS040RegisterCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS040_ScreenParameter prm = GetScreenObject<IVS040_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS040INST>();
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                if (CommonUtil.IsNullOrEmpty(Cond.ApproveNo))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, "IVS040", MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4112, new string[] { "lblApproveNo" }, new string[] { "ApproveNo" });
                    return Json(res);
                }
                prm.ApproveNo = Cond.ApproveNo;

                if (prm.ElemInstrument.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4006);
                    return Json(res);
                }
                //6.2.2 
                foreach (IVS040INST i in prm.ElemInstrument)
                    i.IsError = false;

                if (Cond.Memo != null && Cond.Memo.Replace(" ", "").Contains("\n\n\n\n"))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4022);
                    return Json(res);
                }
                prm.Memo = Cond.Memo;
                prm.Location = Cond.SourceLoc;

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
        /// Send pre-eliminate instrument to confirm screen.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS040_GetEliminateListForConfirm(IVS040RegisterCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS040_ScreenParameter prm = GetScreenObject<IVS040_ScreenParameter>();
                //List<IVS040INST> lstResult = (
                //        from row in prm.ElemInstrument
                //        orderby row.InstrumentCode, row.AreaCode
                //        select row
                //    ).ToList<IVS040INST>();
                //res.ResultData = CommonUtil.ConvertToXml<IVS040INST>(lstResult, "Inventory\\IVS040_ElemConfirm", CommonUtil.GRID_EMPTY_TYPE.SEARCH);

                res.ResultData = CommonUtil.ConvertToXml<IVS040INST>(prm.ElemInstrument, "Inventory\\IVS040_ElemConfirm", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Get invalid instrument.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS040_GetElemError()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS040_ScreenParameter prm = GetScreenObject<IVS040_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS040INST>();

                List<IVS040INST> lstError = (from c in prm.ElemInstrument where c.IsError == true select c).ToList<IVS040INST>();
                res.ResultData = lstError;
                return Json(res);
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
        public ActionResult IVS040_cmdConfirm()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {       //Check Suspend
                IVS040_ScreenParameter prm = GetScreenObject<IVS040_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS040INST>();
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_PRE_ELIMINATION, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                bool isError = false;
                foreach (IVS040INST i in prm.ElemInstrument)
                {
                    doCheckTransferQty Cond = new doCheckTransferQty();
                    Cond.OfficeCode = prm.office.OfficeCode;
                    Cond.LocationCode = prm.Location;
                    Cond.AreaCode = i.AreaCode;
                    Cond.ShelfNo = i.ShelfNo;
                    Cond.InstrumentCode = i.InstrumentCode;
                    Cond.TransferQty = i.TransferInstrumentQty;

                    doCheckTransferQtyResult TransferQtyResult = InvH.CheckTransferQty(Cond);
                    i.InstrumentQty = TransferQtyResult.CurrentyQty;

                    //8.2.1 

                    if (TransferQtyResult.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.InstrumentCode }, new string[] { i.TransQtyID });
                        //res.ResultData = i.InstrumentCode + "," + i.row_id;
                        isError = true;
                    }
                    else if (TransferQtyResult.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.InstrumentCode }, new string[] { i.TransQtyID });
                        //res.ResultData = i.InstrumentCode + "," + i.row_id;
                        isError = true;
                    }
                }
                if (isError)
                {
                    //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = prm.ElemInstrument;
                    return Json(res);
                }

                string strInventorySlipNo = null;
                using (TransactionScope scope = new TransactionScope())
                {
                    //8.3 
                    doRegisterTransferInstrumentData data = new doRegisterTransferInstrumentData();
                    data.SlipId = SlipID.C_INV_SLIPID_PRE_ELIMINATE;
                    data.InventorySlip = new tbt_InventorySlip();
                    data.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
                    tbt_InventorySlip InvSlip = new tbt_InventorySlip();
                    InvSlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
                    InvSlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_PRE_ELIMINATION;
                    InvSlip.SlipIssueDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    InvSlip.StockInDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    InvSlip.StockOutDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    InvSlip.SourceLocationCode = prm.Location;
                    //InvSlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
                    InvSlip.DestinationLocationCode = prm.DestinationLocation;
                    InvSlip.SourceOfficeCode = prm.office.OfficeCode;
                    InvSlip.DestinationOfficeCode = prm.office.OfficeCode;
                    InvSlip.ApproveNo = prm.ApproveNo;
                    InvSlip.Memo = prm.Memo;
                    InvSlip.ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                    InvSlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    InvSlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    InvSlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    InvSlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    data.InventorySlip = InvSlip;

                    data.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
                    int iRunNo = 1;
                    List<IVS040INST> sortedList = (
                            from row in prm.ElemInstrument
                            orderby row.InstrumentCode, row.AreaCode
                            select row
                        ).ToList<IVS040INST>();

                    foreach (IVS040INST i in sortedList)
                    {
                        tbt_InventorySlipDetail SlipDetail = new tbt_InventorySlipDetail();
                        SlipDetail.RunningNo = iRunNo;
                        SlipDetail.InstrumentCode = i.InstrumentCode;
                        SlipDetail.SourceAreaCode = i.AreaCode;
                        SlipDetail.DestinationAreaCode = i.AreaCode;
                        SlipDetail.SourceShelfNo = i.ShelfNo;
                        if (InstrumentLocation.C_INV_LOC_INSTOCK == prm.DestinationLocation)
                        {
                            SlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF;
                        }
                        else
                        {
                            SlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                        }
                        //SlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                        SlipDetail.TransferQty = i.TransferInstrumentQty;

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
                            //i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
                            #region Monthly Price @ 2015
                            //decimal decMovingAveragePrice = InvH.CalculateMovingAveragePrice(i);
                            var decMovingAveragePrice = InvH.GetMonthlyAveragePrice(i.Instrumentcode, InvSlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                            #endregion
                            InvH.UpdateAccountTransferNewInstrument(i, Convert.ToDecimal(decMovingAveragePrice));
                        }
                    }

                    //8.6
                    int blnCheckSecondhandInstrument = InvH.CheckSecondhandInstrument(strInventorySlipNo);
                    if (blnCheckSecondhandInstrument == 1)
                    {
                        List<doGroupSecondhandInstrument> GroupSecondHandInstrument = InvH.GetGroupSecondhandInstrument(strInventorySlipNo);
                        foreach (doGroupSecondhandInstrument i in GroupSecondHandInstrument)
                        {
                            //i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
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
                            //i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
                            InvH.UpdateAccountTransferSampleInstrument(i, null);
                        }
                    }

                    scope.Complete();
                } //end transaction scope

                //8.8
                IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                string reportPath = handlerInventoryDocument.GenerateIVR020FilePath(strInventorySlipNo, prm.office.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                prm.slipNo = strInventorySlipNo;
                prm.reportFilePath = reportPath;
                UpdateScreenObject(prm);

                res.ResultData = strInventorySlipNo;

                return Json(res);
            }
            catch (Exception ex)
            {
                res.ResultData = "toregister";
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Update data in table.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS040_UpdateRowIDElem(IVS040INST Cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IVS040_ScreenParameter prm = GetScreenObject<IVS040_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS040INST>();

                foreach (IVS040INST i in prm.ElemInstrument)
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
        /// Initial screen parameter.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS040_InitParam()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS040_ScreenParameter prm = GetScreenObject<IVS040_ScreenParameter>();
                prm.ElemInstrument = new List<IVS040INST>();
                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Validate before add instrument.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS040_beforeAddElem(IVS040INST Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS040_ScreenParameter prm = GetScreenObject<IVS040_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS040INST>();


                foreach (IVS040INST i in prm.ElemInstrument)
                {
                    if (i.InstrumentCode == Cond.InstrumentCode && i.AreaCode == Cond.AreaCode)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4005);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }

                }
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                Cond.Currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                // Default Currency "Rp." Pachara S. 12102016
                Cond.TransferAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;

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
        /// Remove selected instrument from table.
        /// </summary>
        /// <param name="InstrumentCode"></param>
        /// <param name="AreaCode"></param>
        /// <returns></returns>
        public ActionResult IVS040_DelElem(string InstrumentCode, string AreaCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS040_ScreenParameter prm = GetScreenObject<IVS040_ScreenParameter>();

                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS040INST>();

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
        /// Calculate price.<br />
        /// - Check is empty.<br />
        /// - Check quantity.<br />
        /// - If second hand get price by FIFO lot.<br />
        /// - If sample use default.<br />
        /// - If new get average price.
        /// </summary>
        /// <param name="Cond"></param>
        /// <param name="SourceLoc"></param>
        /// <param name="RegisterPress"></param>
        /// <returns></returns>
        public ActionResult IVS040_Calculate(List<IVS040INST> Cond, string SourceLoc, bool RegisterPress)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (Cond == null || Cond.Count <= 0)
                {
                    if (RegisterPress)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4006);
                    }
                    else
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4068);
                    }
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                IVS040_ScreenParameter prm = GetScreenObject<IVS040_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS040INST>();
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                //int previousQty = 0;
                Hashtable preTransQty = new Hashtable();
                bool isError = false;

                // Check transfer empty instrument
                //List<IVS040INST> zeroList = (
                //    from row in Cond
                //    where row.TransferInstrumentQty <= 0
                //    select row
                //).ToList<IVS040INST>();
                //foreach (var i in zeroList) {
                //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4030, new string[] { i.InstrumentCode }, new string[] { i.TransQtyID });
                //    isError = true;
                //}
                //if (isError) {
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.ResultData = prm.ElemInstrument;
                //    return Json(res);
                //}
                //isError = false;

                for (int i = 0; i < Cond.Count; i++)
                {
                    if (Cond[i].TransferInstrumentQty <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4030, new string[] { Cond[i].InstrumentCode }, new string[] { Cond[i].TransQtyID });
                        isError = true;
                        continue;
                    }

                    foreach (IVS040INST k in prm.ElemInstrument)
                    {
                        if (Cond[i].InstrumentCode == k.InstrumentCode &&
                            Cond[i].AreaCode == k.AreaCode)
                        {
                            doCheckTransferQty checkCon = new doCheckTransferQty();
                            checkCon.OfficeCode = prm.office.OfficeCode;
                            checkCon.LocationCode = SourceLoc;
                            checkCon.AreaCode = k.AreaCode;
                            checkCon.ShelfNo = k.ShelfNo;
                            checkCon.InstrumentCode = k.InstrumentCode;
                            checkCon.TransferQty = Cond[i].TransferInstrumentQty;
                            doCheckTransferQtyResult checkResult = InvH.CheckTransferQty(checkCon);
                            k.InstrumentQty = checkResult.CurrentyQty;

                            if (checkResult.OverQtyFlag == null)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { k.InstrumentCode }, new string[] { Cond[i].TransQtyID });
                                isError = true;
                                break;
                            }
                            else if (checkResult.OverQtyFlag == true)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { k.InstrumentCode }, new string[] { Cond[i].TransQtyID });
                                isError = true;
                                break;
                            }

                            break;
                        }
                    }
                }

                if (isError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = prm.ElemInstrument;
                    return Json(res);
                }

                for (int i = 0; i < Cond.Count; i++)
                {
                    foreach (IVS040INST k in prm.ElemInstrument)
                    {
                        if (Cond[i].InstrumentCode == k.InstrumentCode &&
                            Cond[i].AreaCode == k.AreaCode)
                        {
                            k.TransQtyID = Cond[i].TransQtyID;
                            k.TransferInstrumentQty = Cond[i].TransferInstrumentQty;
                            if (k.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || k.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO)
                            {
                                int previousQty = 0;
                                if (preTransQty.ContainsKey(k.InstrumentCode))
                                {
                                    previousQty = ((int)preTransQty[k.InstrumentCode]);
                                    preTransQty[k.InstrumentCode] = previousQty + k.TransferInstrumentQty;
                                }
                                else
                                {
                                    preTransQty[k.InstrumentCode] = k.TransferInstrumentQty;
                                }

                                doFIFOInstrumentPrice instrumentPrice = InvH.GetFIFOInstrumentPrice(k.TransferInstrumentQty, prm.office.OfficeCode, SourceLoc, k.InstrumentCode, previousQty);

                                if (instrumentPrice.decTransferAmount == null)
                                {
                                    k.TransferAmount = 0;
                                }
                                else
                                {
                                    k.TransferAmount = instrumentPrice.decTransferAmount.Value;
                                    k.TransferAmountCurrencyType = instrumentPrice.decTransferAmountCurrencyType;
                                }
                                //k.TransferAmount = Convert.ToDecimal(InvH.GetFIFOInstrumentPrice(k.TransferInstrumentQty, prm.office.OfficeCode, SourceLoc, k.InstrumentCode, previousQty));
                            }
                            else if (k.AreaCode == InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                            {
                                k.TransferAmount = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                                k.TransferAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                            }
                            else
                            {
                                doCalPriceCondition CalPrice = InvH.GetMovingAveragePriceCondition(prm.office.OfficeCode, null, null, k.InstrumentCode, new string[] { SourceLoc }, null);
                                k.TransferAmount = Convert.ToDecimal(CalPrice.MovingAveragePrice * k.TransferInstrumentQty);
                                if (CalPrice.MovingAveragePriceCurrencyType != null)
                                {
                                    k.TransferAmountCurrencyType = CalPrice.MovingAveragePriceCurrencyType;
                                }
                            }
                            break;
                        }
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
        /// Check is report file exist.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS040_CheckExistFile()
        {
            try
            {
                IVS040_ScreenParameter prm = GetScreenObject<IVS040_ScreenParameter>();
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
        public ActionResult IVS040_DownloadPdfAndWriteLog()
        {
            try
            {
                IVS040_ScreenParameter prm = GetScreenObject<IVS040_ScreenParameter>();
                string fileName = prm.reportFilePath;

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = prm.slipNo,
                    DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT,
                    DocumentCode = ReportID.C_INV_REPORT_ID_PRE_ELIMINATION,
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
