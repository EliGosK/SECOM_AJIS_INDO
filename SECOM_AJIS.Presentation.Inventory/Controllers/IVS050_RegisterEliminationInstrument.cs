


using System;
using System.Collections;
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


namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {
        #region Authority

        /// <summary>
        /// - Check system suspending.<br />
        /// - Check user permission for screen IVS050.<br />
        /// - Get inventory head office.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS050_Authority(IVS050_ScreenParameter param)
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
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_ELIMINATION, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                IInventoryHandler handInven = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
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

            return InitialScreenEnvironment<IVS050_ScreenParameter>("IVS050", param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS050")]
        public ActionResult IVS050()
        {

            ViewBag.Eliminate = InstrumentLocation.C_INV_LOC_ELIMINATION;
            ViewBag.PreEliminate = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;

            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> lstMiscTypeCode = new List<doMiscTypeCode>();
            doMiscTypeCode MiscTypeCode = new doMiscTypeCode();
            MiscTypeCode.FieldName = MiscType.C_INV_LOC;
            MiscTypeCode.ValueCode = InstrumentLocation.C_INV_LOC_ELIMINATION;
            lstMiscTypeCode.Add(MiscTypeCode);
            MiscTypeCode = new doMiscTypeCode();
            MiscTypeCode.FieldName = MiscType.C_INV_LOC;
            MiscTypeCode.ValueCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
            lstMiscTypeCode.Add(MiscTypeCode);
            List<doMiscTypeCode> MiscLock = hand.GetMiscTypeCodeList(lstMiscTypeCode);
            CommonUtil.MappingObjectLanguage<doMiscTypeCode>(MiscLock);

            foreach (var i in MiscLock)
            {
                if (i.ValueCode == InstrumentLocation.C_INV_LOC_ELIMINATION)
                {
                    ViewBag.LabelEliminate = i.ValueDisplay;
                }
                else if (i.ValueCode == InstrumentLocation.C_INV_LOC_PRE_ELIMINATION)
                {
                    ViewBag.LabelPreEliminate = i.ValueDisplay;
                }
            }

            ViewBag.Total = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INVENTORY, ScreenID.C_INV_SCREEN_ID_PRE_ELIMINATION, "lblTotalAmountOfTransferAsset");

            return View();
        }
        #endregion

        /// <summary>
        /// Get config for Instrument List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS050_InstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS050_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Search instrument list.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS050_SearchInventoryInstrumentList(IVS050SearchCondition Cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { Cond });
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                IVS050_ScreenParameter prm = GetScreenObject<IVS050_ScreenParameter>();
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<dtSearchInstrumentListResult> lstResult =

                    InvH.SearchInventoryInstrumentList(prm.office.OfficeCode,
                    InstrumentLocation.C_INV_LOC_PRE_ELIMINATION,
                    Cond.AreaCode,
                    ShelfType.C_INV_SHELF_TYPE_NORMAL,
                    ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                    ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                    Cond.InstrumentName, Cond.InstrumentCode,
                    new string[] { InstrumentArea.C_INV_AREA_SE_LENDING_DEMO }
                  );

                res.ResultData = CommonUtil.ConvertToXml<dtSearchInstrumentListResult>(lstResult, "Inventory\\IVS050_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
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
        public ActionResult IVS050_InitParam()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS050_ScreenParameter prm = GetScreenObject<IVS050_ScreenParameter>();
                prm.ElemInstrument = new List<IVS040INST>();
                res.ResultData = true;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Udpate data in Eliminate table.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS050_UpdateRowIDElem(IVS040INST Cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IVS050_ScreenParameter prm = GetScreenObject<IVS050_ScreenParameter>();
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
        public ActionResult IVS050_Calculate(List<IVS040INST> Cond, string SourceLoc, bool RegisterPress)
        {
            ObjectResultData res = new ObjectResultData();
            try {
                if (Cond == null || Cond.Count <= 0) {
                    if (RegisterPress) {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4006);
                    } else {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4068);
                    }
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                IVS050_ScreenParameter prm = GetScreenObject<IVS050_ScreenParameter>();
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
                //foreach (var i in zeroList)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4030, new string[] { i.InstrumentCode }, new string[] { i.TransQtyID });
                //    isError = true;
                //}
                //if (isError)
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.ResultData = prm.ElemInstrument;
                //    return Json(res);
                //}
                //isError = false;

                for (int i = 0; i < Cond.Count; i++) {
                    if (Cond[i].TransferInstrumentQty <= 0) {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4030, new string[] { Cond[i].InstrumentCode }, new string[] { Cond[i].TransQtyID });
                        isError = true;
                        continue;
                    }

                    foreach (IVS040INST k in prm.ElemInstrument) {
                        if (Cond[i].InstrumentCode == k.InstrumentCode &&
                            Cond[i].AreaCode == k.AreaCode)
                        {
                            doCheckTransferQty checkCon = new doCheckTransferQty();
                            checkCon.OfficeCode = prm.office.OfficeCode;
                            checkCon.LocationCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
                            checkCon.AreaCode = k.AreaCode;
                            checkCon.ShelfNo = k.ShelfNo;
                            checkCon.InstrumentCode = k.InstrumentCode;
                            checkCon.TransferQty = Cond[i].TransferInstrumentQty;
                            doCheckTransferQtyResult checkResult = InvH.CheckTransferQty(checkCon);
                            k.InstrumentQty = checkResult.CurrentyQty;

                            if (checkResult.OverQtyFlag == null) {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { k.InstrumentCode }, new string[] { Cond[i].TransQtyID });
                                isError = true;
                                break;
                            } else if (checkResult.OverQtyFlag == true) {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { k.InstrumentCode }, new string[] { Cond[i].TransQtyID });
                                isError = true;
                                break;
                            }

                            break;
                        }
                    }
                }

                if (isError) {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.ResultData = prm.ElemInstrument;
                    return Json(res);
                }

                for (int i = 0; i < Cond.Count; i++) {
                    foreach (IVS040INST k in prm.ElemInstrument) {
                        if (Cond[i].InstrumentCode == k.InstrumentCode &&
                            Cond[i].AreaCode == k.AreaCode)
                        {
                            k.TransQtyID = Cond[i].TransQtyID;
                            k.TransferInstrumentQty = Cond[i].TransferInstrumentQty;
                            if (k.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || k.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO)
                            {
                                int previousQty = 0;
                                if (preTransQty.ContainsKey(k.InstrumentCode)) {
                                    previousQty = ((int)preTransQty[k.InstrumentCode]);
                                    preTransQty[k.InstrumentCode] = previousQty + k.TransferInstrumentQty;
                                } else {
                                    preTransQty[k.InstrumentCode] = k.TransferInstrumentQty;
                                }
                                doFIFOInstrumentPrice instrumentPrice = InvH.GetFIFOInstrumentPrice(k.TransferInstrumentQty, prm.office.OfficeCode, InstrumentLocation.C_INV_LOC_PRE_ELIMINATION, k.InstrumentCode, previousQty);
                                if(instrumentPrice.decTransferAmount == null)
                                {
                                    k.TransferAmount = 0;
                                }
                                else
                                {
                                    k.TransferAmount = instrumentPrice.decTransferAmount.Value;
                                    k.TransferAmountCurrencyType = instrumentPrice.decTransferAmountCurrencyType;
                                }
                            } else if (k.AreaCode == InstrumentArea.C_INV_AREA_NEW_SAMPLE) {
                                k.TransferAmount = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                                k.TransferAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                            } else {
                                doCalPriceCondition CalPrice = InvH.GetMovingAveragePriceCondition(prm.office.OfficeCode, null, null, k.InstrumentCode, new string[] { InstrumentLocation.C_INV_LOC_PRE_ELIMINATION }, null);
                                k.TransferAmount = Convert.ToDecimal(CalPrice.MovingAveragePrice * k.TransferInstrumentQty);
                                if(CalPrice.MovingAveragePriceCurrencyType != null)
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
        /// Validate before add instrument.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS050_beforeAddElem(IVS040INST Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS050_ScreenParameter prm = GetScreenObject<IVS050_ScreenParameter>();
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
                // Default Currency "Rp." Pachara S.
                Cond.TransferAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;

                prm.ElemInstrument.Add(Cond);
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
        /// Remove selected instrument from table.
        /// </summary>
        /// <param name="InstrumentCode"></param>
        /// <param name="AreaCode"></param>
        /// <returns></returns>
        public ActionResult IVS050_DelElem(string InstrumentCode, string AreaCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS050_ScreenParameter prm = GetScreenObject<IVS050_ScreenParameter>();

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
        /// Validate before confirm screen.<br />
        /// - Check require field.<br />
        /// - Check instrument list not empty.<br />
        /// - Check memo.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS050_cmdReg(IVS040RegisterCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS050_ScreenParameter prm = GetScreenObject<IVS050_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS040INST>();
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                if (CommonUtil.IsNullOrEmpty(Cond.ApproveNo))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, "IVS050", MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4112, new string[] { "lblApproveNo" }, new string[] { "ApproveNo" });
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
                prm.Location = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;

                UpdateScreenObject(prm);
                return Json(true);
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
        public ActionResult IVS050_GetEliminateListForConfirm(IVS040RegisterCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS050_ScreenParameter prm = GetScreenObject<IVS050_ScreenParameter>();
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
        /// Register Elimination.<br />
        /// - Check system suspending.<br />
        /// - Check quantity.<br />
        /// - Insert inventory slip.<br />
        /// - Update account transfer new/second hand/sample instrument.<br />
        /// - Insert account stock moving.<br />
        /// - Generate report.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS050_cmdConfirm()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {       //Check Suspend
                IVS050_ScreenParameter prm = GetScreenObject<IVS050_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS040INST>();
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_ELIMINATION, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                //8.4.1
                bool isError = false;
                foreach (IVS040INST i in prm.ElemInstrument)
                {
                    doCheckTransferQty Cond = new doCheckTransferQty();
                    Cond.OfficeCode = prm.office.OfficeCode;
                    Cond.LocationCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
                    Cond.AreaCode = i.AreaCode;
                    Cond.ShelfNo = i.ShelfNo;
                    Cond.InstrumentCode = i.InstrumentCode;
                    Cond.TransferQty = i.TransferInstrumentQty;

                    doCheckTransferQtyResult TransferQtyResult = InvH.CheckTransferQty(Cond);
                    i.InstrumentQty = TransferQtyResult.CurrentyQty;

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
                    //8.5
                    doRegisterTransferInstrumentData data = new doRegisterTransferInstrumentData();
                    data.SlipId = SlipID.C_INV_SLIPID_ELIMINATE;

                    data.InventorySlip = new tbt_InventorySlip();
                    data.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
                    tbt_InventorySlip InvSlip = new tbt_InventorySlip();
                    InvSlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
                    InvSlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_ELIMINATION;
                    InvSlip.SlipIssueDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    InvSlip.StockInDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    InvSlip.StockOutDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    InvSlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
                    InvSlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_ELIMINATION;
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
                    foreach (IVS040INST i in prm.ElemInstrument)
                    {
                        tbt_InventorySlipDetail SlipDetail = new tbt_InventorySlipDetail();
                        SlipDetail.SlipNo = null;
                        SlipDetail.RunningNo = iRunNo;
                        SlipDetail.InstrumentCode = i.InstrumentCode;
                        SlipDetail.SourceAreaCode = i.AreaCode;
                        SlipDetail.DestinationAreaCode = i.AreaCode;
                        SlipDetail.SourceShelfNo = i.ShelfNo;
                        SlipDetail.DestinationShelfNo = i.ShelfNo;
                        SlipDetail.TransferQty = i.TransferInstrumentQty;
                        SlipDetail.InstrumentAmount = i.TransferAmount;

                        data.lstTbt_InventorySlipDetail.Add(SlipDetail);
                        iRunNo++;
                    }
                    strInventorySlipNo = InvH.RegisterTransferInstrument(data);

                    List<tbt_AccountStockMoving> listInsertAccStockMoving = new List<tbt_AccountStockMoving>();

                    if (InvH.CheckNewInstrument(strInventorySlipNo) == 1)
                    {
                        //8.7.1
                        List<doGroupNewInstrument> groupNewInstrument = InvH.GetGroupNewInstrument(strInventorySlipNo);
                        foreach (doGroupNewInstrument i in groupNewInstrument)
                        {
                            i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_ELIMINATION;
                            #region Monthly Price @ 2015
                            //decimal decMovingAveragePrice = InvH.CalculateMovingAveragePrice(i);
                            decimal decMovingAveragePrice = InvH.GetMonthlyAveragePrice(i.Instrumentcode, InvSlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                            #endregion
                            bool canUpdate = InvH.UpdateAccountTransferNewInstrument(i, Convert.ToDecimal(decMovingAveragePrice));

                            if (!canUpdate)
                            {
                                scope.Dispose();
                                return Json(res);
                            }

                            List<tbt_AccountInstock> listInstock = InvH.GetTbt_AccountInStock(i.Instrumentcode, InstrumentLocation.C_INV_LOC_PRE_ELIMINATION, prm.office.OfficeCode);
                            if (listInstock.Count <= 0)
                            {
                                scope.Dispose();
                                return Json(res);
                            }
                            tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                            accountStockMoving.SlipNo = strInventorySlipNo;
                            accountStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_ELIMINATION;
                            accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                            accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_ELIMINATE;
                            accountStockMoving.SourceLocationCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
                            accountStockMoving.DestinationLocationCode = InstrumentLocation.C_INV_LOC_ELIMINATION;
                            accountStockMoving.InstrumentCode = i.Instrumentcode;
                            accountStockMoving.InstrumentQty = i.TransferQty;
                            accountStockMoving.InstrumentPrice = listInstock[0].MovingAveragePrice;
                            accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            listInsertAccStockMoving.Add(accountStockMoving);
                        }
                    }

                    //8.8
                    int blnCheckSecondhandInstrument = InvH.CheckSecondhandInstrument(strInventorySlipNo);
                    if (blnCheckSecondhandInstrument == 1)
                    {
                        List<doGroupSecondhandInstrument> GroupSecondHandInstrument = InvH.GetGroupSecondhandInstrument(strInventorySlipNo);
                        foreach (doGroupSecondhandInstrument i in GroupSecondHandInstrument)
                        {
                            i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_ELIMINATION;
                            bool canUpdate = InvH.UpdateAccountTransferSecondhandInstrument(i);

                            if (!canUpdate)
                            {
                                scope.Dispose();
                                return Json(res);
                            }

                            decimal sumTransferAmount = 0;
                            foreach (IVS040INST j in prm.ElemInstrument)
                            {
                                if (i.Instrumentcode == j.InstrumentCode 
                                    && (j.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || j.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO))
                                {
                                    sumTransferAmount += j.TransferAmount;
                                }
                            }

                            tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                            accountStockMoving.SlipNo = strInventorySlipNo;
                            accountStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_ELIMINATION;
                            accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTALLED;
                            accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_ELIMINATE;
                            accountStockMoving.SourceLocationCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
                            accountStockMoving.DestinationLocationCode = InstrumentLocation.C_INV_LOC_ELIMINATION;
                            accountStockMoving.InstrumentCode = i.Instrumentcode;
                            accountStockMoving.InstrumentQty = i.TransferQty;
                            if (sumTransferAmount != 0)
                            {
                                accountStockMoving.InstrumentPrice = sumTransferAmount / i.TransferQty;
                            }
                            else
                            {
                                accountStockMoving.InstrumentPrice = 0;
                            }
                            accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            listInsertAccStockMoving.Add(accountStockMoving);
                        }
                    }

                    //8.9

                    int blnCheckSampleInstrument = InvH.CheckSampleInstrument(strInventorySlipNo);
                    if (blnCheckSampleInstrument == 1)
                    {
                        List<doGroupSampleInstrument> GroupSampleInstrument = InvH.GetGroupSampleInstrument(strInventorySlipNo);
                        foreach (doGroupSampleInstrument i in GroupSampleInstrument)
                        {
                            i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_ELIMINATION;
                            bool canUpdate = InvH.UpdateAccountTransferSampleInstrument(i, null);

                            if (!canUpdate)
                            {
                                scope.Dispose();
                                return Json(res);
                            }

                            tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                            accountStockMoving.SlipNo = strInventorySlipNo;
                            accountStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_ELIMINATION;
                            accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                            accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_ELIMINATE;
                            accountStockMoving.SourceLocationCode = InstrumentLocation.C_INV_LOC_PRE_ELIMINATION;
                            accountStockMoving.DestinationLocationCode = InstrumentLocation.C_INV_LOC_ELIMINATION;
                            accountStockMoving.InstrumentCode = i.Instrumentcode;
                            accountStockMoving.InstrumentQty = i.TransferQty;
                            accountStockMoving.InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                            accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            listInsertAccStockMoving.Add(accountStockMoving);
                        }
                    }

                    //8.10              
                    List<tbt_AccountStockMoving> resultInsert = InvH.InsertAccountStockMoving(listInsertAccStockMoving);
                    if (resultInsert.Count <= 0)
                    {
                        scope.Dispose();
                        return Json(res);
                    }

                    scope.Complete();
                } //end transaction scope

                IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                string reportPath = handlerInventoryDocument.GenerateIVR030FilePath(strInventorySlipNo, prm.office.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
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
        /// Check is report file exist.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS050_CheckExistFile()
        {
            try
            {
                IVS050_ScreenParameter prm = GetScreenObject<IVS050_ScreenParameter>();
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
        public ActionResult IVS050_DownloadPdfAndWriteLog()
        {
            try
            {
                IVS050_ScreenParameter prm = GetScreenObject<IVS050_ScreenParameter>();
                string fileName = prm.reportFilePath;

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = prm.slipNo,
                    DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT,
                    DocumentCode = ReportID.C_INV_REPORT_ID_ELIMINATION,
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