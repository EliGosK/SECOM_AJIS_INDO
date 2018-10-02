


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
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Inventory.Models;
using System.Transactions;
using System.IO;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {
        private const string NEW_INSTRUMENT_FOR_SAMPLE = "0";
        private const string NEW_INSTRUMENT_FOR_SALE = "1";
        private const string NEW_INSTRUMENT_FOR_RENTAL = "2";
        private const string SECONDHAND_INSTRUMENT_FOR_RENTAL = "3";
        private const string NEW_INSTRUMENT_FOR_DEMO = "4";
        private const string LENDING_SECONDHAND_INSTRUMENT_FOR_DEMO = "5";
        private const string HANDINGSECOND_INSTRUMENT_FOR_DEMO = "6";

        #region Authority
        /// <summary>
        /// Check screen permission
        /// </summary>
        /// <param name="param">ScreenParameter</param>
        /// <returns></returns>
        public ActionResult IVS080_Authority(IVS080_ScreenParameter param)
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
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_WITHIN_WH, FunctionID.C_FUNC_ID_OPERATE))
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
                if (IvHeadOffice.Count > 0)
                {
                    param.HeadOfficeCode = IvHeadOffice[0].OfficeCode;
                }

                List<doOffice> SrinakarinOffice = handInven.GetInventorySrinakarinOffice();
                if (SrinakarinOffice.Count > 0)
                {
                    param.SrinakarinOfficeCode = SrinakarinOffice[0].OfficeCode;
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS080_ScreenParameter>("IVS080", param, res);
        }

        /// <summary>
        /// Initial Screen
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS080")]
        public ActionResult IVS080()
        {
            IVS080_ScreenParameter param = GetScreenObject<IVS080_ScreenParameter>();
            ViewBag.HeadOfficeCode = param.HeadOfficeCode;
            ViewBag.SrinakarinOfficeCode = param.SrinakarinOfficeCode;

            ViewBag.Location = IVS080_GetLocation();

            var srvInv = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            var today = DateTime.Today;
            var endoflastmonth = today.AddDays(-today.Day);
            var dateof5businessday = srvInv.GetNextBusinessDateByDefaultOffset(endoflastmonth);

            if (dateof5businessday >= today)
            {
                var beginoflastmonth = endoflastmonth.AddDays(1).AddMonths(-1);
                ViewBag.MinDate = (beginoflastmonth - today).TotalDays;
            }
            else
            {
                var beginofthismonth = endoflastmonth.AddDays(1);
                ViewBag.MinDate = (beginofthismonth - today).TotalDays;
            }
            ViewBag.MaxDate = 0;

            return View();
        }
        #endregion

        /// <summary>
        /// Search inventory instrument
        /// </summary>
        /// <param name="Cond">Search condition object</param>
        /// <returns></returns>
        public ActionResult IVS080_SearchInventoryInstrument(IVS080SearchCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {    //Valid Cond
                ValidatorUtil.BuildErrorMessage(res, this);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                IVS080_ScreenParameter param = GetScreenObject<IVS080_ScreenParameter>();

                List<dtSearchInstrumentListResult> lstResult =

                    InvH.SearchInventoryInstrumentList(Cond.Office,
                    InstrumentLocation.C_INV_LOC_INSTOCK,
                    Cond.SourceArea,
                    ShelfType.C_INV_SHELF_TYPE_NORMAL,
                    Cond.ShelfNoFrom,
                    Cond.ShelfNoTo,
                    Cond.InstName,
                    Cond.InstCode
                  );

                if (lstResult.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
                else if (lstResult.Count > 1000)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4004);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                res.ResultData = CommonUtil.ConvertToXml<dtSearchInstrumentListResult>(lstResult, "inventory\\IVS080_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        public ActionResult IVS080_InstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS080_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial transfer instrument grid control
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS080_TransferStockInstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS080_TransferStock", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Get location
        /// </summary>
        /// <returns></returns>
        public string IVS080_GetLocation()
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
                           where (c.ValueCode == InstrumentLocation.C_INV_LOC_INSTOCK)
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
        /// Validate register transfer instrument data
        /// </summary>
        /// <param name="Cond">Register condition object</param>
        /// <returns></returns>
        public ActionResult IVS080_cmdReg(IVS080RegisterCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS080_ScreenParameter prm = GetScreenObject<IVS080_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS080INST>();
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                //9.1
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_WITHIN_WH, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //9.2
                if (InvH.CheckStartedStockChecking() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    return Json(res);
                }

                if (Cond.TransferDate == null)
                {
                    res.AddErrorMessage(
                        MessageUtil.MODULE_INVENTORY, 
                        "IVS080", 
                        MessageUtil.MODULE_COMMON, 
                        MessageUtil.MessageList.MSG0007, 
                        new string[] { "lblTransferDate" }, 
                        new string[] { "txtTransferDate" }
                    );
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                prm.Memo = Cond.Memo;
                prm.ApproveNo = Cond.ApproveNo;
                prm.Office = Cond.Office;
                prm.SourceArea = Cond.SourceArea;
                prm.DestinationArea = Cond.DestinationArea;
                prm.ContractCode = Cond.ContractCode;
                prm.TransferDate = Cond.TransferDate;

                //9.3
                bool chkApprove = false;
                if (prm.SourceArea == NEW_INSTRUMENT_FOR_SAMPLE && prm.DestinationArea == LENDING_SECONDHAND_INSTRUMENT_FOR_DEMO)
                    chkApprove = true;
                else if (prm.SourceArea == NEW_INSTRUMENT_FOR_SALE && prm.DestinationArea == NEW_INSTRUMENT_FOR_DEMO)
                    chkApprove = true;
                else if (prm.SourceArea == NEW_INSTRUMENT_FOR_RENTAL && prm.DestinationArea == NEW_INSTRUMENT_FOR_DEMO)
                    chkApprove = true;
                else if (prm.SourceArea == SECONDHAND_INSTRUMENT_FOR_RENTAL && (prm.DestinationArea == LENDING_SECONDHAND_INSTRUMENT_FOR_DEMO || prm.DestinationArea == HANDINGSECOND_INSTRUMENT_FOR_DEMO))
                    chkApprove = true;
                else if (prm.SourceArea == NEW_INSTRUMENT_FOR_DEMO && (prm.DestinationArea == NEW_INSTRUMENT_FOR_SALE || prm.DestinationArea == NEW_INSTRUMENT_FOR_RENTAL || prm.DestinationArea == LENDING_SECONDHAND_INSTRUMENT_FOR_DEMO))
                    chkApprove = true;
                else if (prm.SourceArea == HANDINGSECOND_INSTRUMENT_FOR_DEMO && (prm.DestinationArea == SECONDHAND_INSTRUMENT_FOR_RENTAL || prm.DestinationArea == LENDING_SECONDHAND_INSTRUMENT_FOR_DEMO))
                    chkApprove = true;

                if (chkApprove)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }

                if (prm.SourceArea == NEW_INSTRUMENT_FOR_DEMO && prm.DestinationArea == LENDING_SECONDHAND_INSTRUMENT_FOR_DEMO)
                {
                    if (string.IsNullOrEmpty(prm.ContractCode))
                    {
                        ValidatorUtil validator = new ValidatorUtil(this);
                        validator.AddErrorMessage(MessageUtil.MODULE_INVENTORY,
                            ScreenID.C_INV_SCREEN_ID_WITHIN_WH,
                            MessageUtil.MODULE_COMMON,
                            MessageUtil.MessageList.MSG0007,
                            "MsgIdContractCode",
                            "lblContractCode",
                            "ContractCode");

                        ValidatorUtil.BuildErrorMessage(res, validator);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                    else
                    {
                        var billH = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                        var commonutil = new CommonUtil();
                        var billingbasic = billH.GetTbt_BillingBasic(commonutil.ConvertContractCode(prm.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG), null);

                        if (billingbasic == null || billingbasic.Count <= 0)
                        {
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0105, new string[] { prm.ContractCode });
                            return Json(res);
                        }
                        else
                        {
                            prm.ContractCode = billingbasic[0].ContractCode;
                        }
                    }
                }

                //9.4.1
                if (prm.ElemInstrument.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4006);
                    return Json(res);
                }

                prm.ElemInstrument = Cond.StockInInstrument;

                //9.4.2
                foreach (IVS080INST i in prm.ElemInstrument)
                {
                    if (string.IsNullOrEmpty(i.DestinationShelfNo))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY,
                                        ScreenID.C_INV_SCREEN_ID_WITHIN_WH,
                                        MessageUtil.MODULE_COMMON,
                                        MessageUtil.MessageList.MSG0007,
                                        new string[] { "headerDestinationShelfNo" },
                                        new string[] { i.DestShelfNo_id });

                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    }

                    if (i.TransferQty <= 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4029, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                    }

                }

                //9.4.3
                if (!string.IsNullOrEmpty(prm.Memo) && prm.Memo.Replace(" ", "").Contains("\n\n\n\n"))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4022, null, new string[] { "memo" });
                    return Json(res);
                }

                //9.4.4
                bool isError = false;
                foreach (IVS080INST i in prm.ElemInstrument)
                {
                    List<tbt_InventoryCurrent> doTbt_InventoryCurrent = InvH.GetTbt_InventoryCurrent(prm.Office, InstrumentLocation.C_INV_LOC_INSTOCK, prm.SourceArea, i.SourceShelfNo, i.InstrumentCode);

                    if (doTbt_InventoryCurrent.Count <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        isError = true;
                        i.InstrumentQty = 0;
                    }
                    else if (doTbt_InventoryCurrent[0].InstrumentQty < i.TransferQty)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        isError = true;
                        i.InstrumentQty = doTbt_InventoryCurrent[0].InstrumentQty != null ? Convert.ToInt32(doTbt_InventoryCurrent[0].InstrumentQty) : 0;
                    }
                    else
                    {
                        i.InstrumentQty = doTbt_InventoryCurrent[0].InstrumentQty != null ? Convert.ToInt32(doTbt_InventoryCurrent[0].InstrumentQty) : 0;
                    }

                    //9.4.5
                    if (prm.Office == prm.HeadOfficeCode)
                    {
                        //9.4.6
                        doGetShelfCurrentData doGetSourceShelf = new doGetShelfCurrentData();
                        doGetSourceShelf.OfficeCode = prm.Office;
                        doGetSourceShelf.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                        doGetSourceShelf.ShelfNo = i.DestinationShelfNo;
                        doGetSourceShelf.InstrumentCode = null;

                        List<doShelfCurrentData> doShelfCurrentData = InvH.GetShelfCurrentData(doGetSourceShelf);

                        if (doShelfCurrentData.Count <= 0 && !string.IsNullOrEmpty(i.DestinationShelfNo))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4048, new string[] { i.DestinationShelfNo }, new string[] { i.DestShelfNo_id });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            isError = true;
                        }

                        //9.4.7
                        if (doShelfCurrentData.Count > 0 && doShelfCurrentData[0].ShelfType == ShelfType.C_INV_SHELF_TYPE_PROJECT)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4104, new string[] { i.DestinationShelfNo }, new string[] { i.DestShelfNo_id });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            isError = true;
                        }

                        //9.4.8.1
                        if (!string.IsNullOrEmpty(i.DestinationShelfNo) && doShelfCurrentData.Count > 0 && i.SourceShelfNo != i.DestinationShelfNo && doShelfCurrentData[0].ShelfType == ShelfType.C_INV_SHELF_TYPE_NORMAL
                            && (doShelfCurrentData[0].AreaCode != prm.DestinationArea || doShelfCurrentData[0].InstrumentCode != i.InstrumentCode))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4105, new string[] { i.DestinationShelfNo }, new string[] { i.DestShelfNo_id });
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            isError = true;
                        }

                        //9.4.8.2
                        if (!string.IsNullOrEmpty(i.DestinationShelfNo) && doShelfCurrentData.Count > 0 && i.SourceShelfNo != i.DestinationShelfNo && doShelfCurrentData[0].ShelfType == ShelfType.C_INV_SHELF_TYPE_EMPTY)
                        {
                            IMasterHandler IMasH = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                            //Modify by Jutarat A. on 09042013
                            //if (IMasH.CheckShelfDuplicateArea(i.InstrumentCode, prm.DestinationArea))
                            string strDuplicateShelfNoList = IMasH.GetShelfNoDuplicateArea(i.InstrumentCode, prm.DestinationArea);
                            if (String.IsNullOrEmpty(strDuplicateShelfNoList) == false)
                            {
                                //res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4106, new string[] { i.DestinationShelfNo }, new string[] { i.DestShelfNo_id });
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4106, new string[] { i.DestinationShelfNo, strDuplicateShelfNoList }, new string[] { i.DestShelfNo_id });
                                
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                isError = true;
                            }
                            //End Modify
                        }

                        //9.4.8.3
                        if (i.SourceShelfNo == i.DestinationShelfNo)
                        {
                            if (i.InstrumentQty != i.TransferQty)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4107, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                isError = true;
                            }

                            doGetShelfCurrentData doGetDestShelf = new doGetShelfCurrentData();
                            doGetDestShelf.OfficeCode = prm.Office;
                            doGetDestShelf.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                            doGetDestShelf.AreaCode = prm.DestinationArea;
                            doGetDestShelf.ShelfNo = null;
                            doGetDestShelf.InstrumentCode = i.InstrumentCode;

                            List<doShelfCurrentData> doShelfCurrentDataDest = InvH.GetShelfForChecking(doGetDestShelf);

                            if (doShelfCurrentData.Count > 0 && doShelfCurrentDataDest.Count > 0 && doShelfCurrentDataDest[0].InstrumentQty > 0)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4108, new string[] { i.DestinationShelfNo }, new string[] { i.DestShelfNo_id });
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                isError = true;
                            }
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
        /// Register transfer instrument data
        /// </summary>
        /// <param name="Con">Register condition object</param>
        /// <returns></returns>
        public ActionResult IVS080_cmdConfirm(IVS080RegisterCond Con)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS080_ScreenParameter prm = GetScreenObject<IVS080_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS080INST>();

                //10.1
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_WITHIN_WH, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                //10.2
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                if (InvH.CheckStartedStockChecking() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    return Json(res);
                }

                foreach (IVS080INST i in prm.ElemInstrument)
                {
                    //10.3.1
                    List<tbt_InventoryCurrent> doTbt_InventoryCurrent = InvH.GetTbt_InventoryCurrent(prm.Office, InstrumentLocation.C_INV_LOC_INSTOCK, prm.SourceArea, i.SourceShelfNo, i.InstrumentCode);

                    if (doTbt_InventoryCurrent.Count <= 0)
                    {
                        i.InstrumentQty = 0;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        res.ResultData = prm.ElemInstrument;
                        i.IsError = true;
                        return Json(res);
                    }
                    else if (doTbt_InventoryCurrent[0].InstrumentQty < i.TransferQty)
                    {
                        i.InstrumentQty = doTbt_InventoryCurrent[0].InstrumentQty != null ? Convert.ToInt32(doTbt_InventoryCurrent[0].InstrumentQty) : 0;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                        res.ResultData = prm.ElemInstrument;
                        i.IsError = true;
                        return Json(res);
                    }

                    //10.3.2
                    if (prm.Office == prm.HeadOfficeCode)
                    {
                        //10.3.3
                        doGetShelfCurrentData doGetSourceShelf = new doGetShelfCurrentData();
                        doGetSourceShelf.OfficeCode = prm.Office;
                        doGetSourceShelf.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                        doGetSourceShelf.InstrumentCode = null;
                        doGetSourceShelf.ShelfNo = i.DestinationShelfNo;

                        List<doShelfCurrentData> doShelfCurrentData = InvH.GetShelfCurrentData(doGetSourceShelf);

                        if (doShelfCurrentData.Count <= 0)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4048, new string[] { i.DestinationShelfNo }, new string[] { i.DestShelfNo_id });
                            res.ResultData = prm.ElemInstrument;
                            i.IsError = true;
                            return Json(res);
                        }

                        //10.3.4
                        if (doShelfCurrentData[0].ShelfType == ShelfType.C_INV_SHELF_TYPE_PROJECT)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4104, new string[] { i.DestinationShelfNo }, new string[] { i.DestShelfNo_id });
                            res.ResultData = prm.ElemInstrument;
                            i.IsError = true;
                            return Json(res);
                        }

                        //10.3.5.1
                        if (i.SourceShelfNo != i.DestinationShelfNo && doShelfCurrentData[0].ShelfType == ShelfType.C_INV_SHELF_TYPE_NORMAL
                            && (doShelfCurrentData[0].AreaCode != prm.DestinationArea || doShelfCurrentData[0].InstrumentCode != i.InstrumentCode))
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4105, new string[] { i.DestinationShelfNo }, new string[] { i.DestShelfNo_id });
                            res.ResultData = prm.ElemInstrument;
                            i.IsError = true;
                            return Json(res);
                        }

                        //10.3.5.2
                        if (i.SourceShelfNo != i.DestinationShelfNo && doShelfCurrentData[0].ShelfType == ShelfType.C_INV_SHELF_TYPE_EMPTY)
                        {
                            IMasterHandler IMasH = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                            if (IMasH.CheckShelfDuplicateArea(i.InstrumentCode, prm.DestinationArea))
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4106, new string[] { i.DestinationShelfNo }, new string[] { i.DestShelfNo_id });
                                res.ResultData = prm.ElemInstrument;
                                i.IsError = true;
                                return Json(res);
                            }
                        }

                        //10.3.5.3
                        if (i.SourceShelfNo == i.DestinationShelfNo)
                        {
                            if (i.InstrumentQty != i.TransferQty)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4107, new string[] { i.InstrumentCode }, new string[] { i.StockOutQty_id });
                                res.ResultData = prm.ElemInstrument;
                                i.IsError = true;
                                return Json(res);
                            }

                            doGetShelfCurrentData doGetDestShelf = new doGetShelfCurrentData();
                            doGetDestShelf.OfficeCode = prm.Office;
                            doGetDestShelf.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                            doGetDestShelf.AreaCode = prm.DestinationArea;
                            doGetDestShelf.ShelfNo = null;
                            doGetDestShelf.InstrumentCode = i.InstrumentCode;

                            List<doShelfCurrentData> doShelfCurrentDataDest = InvH.GetShelfForChecking(doGetDestShelf);

                            if (doShelfCurrentData.Count > 0 && doShelfCurrentDataDest.Count > 0 && doShelfCurrentDataDest[0].InstrumentQty > 0)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4108, new string[] { i.DestinationShelfNo }, new string[] { i.DestShelfNo_id });
                                res.ResultData = prm.ElemInstrument;
                                i.IsError = true;
                                return Json(res);
                            }
                        }
                    }
                }

                //10.4
                string strInventorySlipNo = string.Empty;

                doRegisterTransferInstrumentData data = new doRegisterTransferInstrumentData();
                //10.4.1
                data.SlipId = SlipID.C_INV_SLIPID_TRANSFER_AREA;
                //10.4.2
                tbt_InventorySlip InvSlip = new tbt_InventorySlip();
                InvSlip.SlipNo = null;
                InvSlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
                InvSlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_TRANSFER_AREA;
                InvSlip.InstallationSlipNo = null;
                InvSlip.ProjectCode = null;
                InvSlip.PurchaseOrderNo = null;
                InvSlip.ContractCode = prm.ContractCode;
                InvSlip.SlipIssueDate = DateTime.Now;
                InvSlip.StockInDate = prm.TransferDate;
                InvSlip.StockOutDate = prm.TransferDate;
                InvSlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                InvSlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                InvSlip.SourceOfficeCode = prm.Office;
                InvSlip.DestinationOfficeCode = prm.Office;
                InvSlip.ApproveNo = prm.ApproveNo;
                InvSlip.Memo = prm.Memo;
                InvSlip.StockInFlag = null;
                InvSlip.DeliveryOrderNo = null;
                InvSlip.ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                InvSlip.RepairSubcontractor = null;
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

                //10.4.3
                int iRunNo = 1;
                foreach (IVS080INST i in prm.ElemInstrument)
                {
                    tbt_InventorySlipDetail SlipDetail = new tbt_InventorySlipDetail();
                    SlipDetail.SlipNo = null;
                    SlipDetail.RunningNo = iRunNo;
                    SlipDetail.InstrumentCode = i.InstrumentCode;
                    SlipDetail.SourceAreaCode = prm.SourceArea;
                    SlipDetail.DestinationAreaCode = prm.DestinationArea;
                    if (prm.Office != prm.HeadOfficeCode)
                        SlipDetail.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    else
                        SlipDetail.SourceShelfNo = i.SourceShelfNo;
                    if (prm.Office != prm.HeadOfficeCode)
                        SlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    else
                        SlipDetail.DestinationShelfNo = i.DestinationShelfNo;
                    SlipDetail.TransferQty = i.TransferQty;
                    SlipDetail.NotInstalledQty = null;
                    SlipDetail.RemovedQty = null;
                    SlipDetail.UnremovableQty = null;
                    SlipDetail.InstrumentAmount = null;

                    data.lstTbt_InventorySlipDetail.Add(SlipDetail);
                    iRunNo++;
                }

                IShelfMasterHandler hand = ServiceContainer.GetService<IShelfMasterHandler>() as IShelfMasterHandler;

                //10.5
                using (TransactionScope scope = new TransactionScope())
                {
                    //10.6
                    strInventorySlipNo = InvH.RegisterTransferInstrument(data);

                    //10.7
                    if (prm.Office == prm.HeadOfficeCode || prm.Office == prm.SrinakarinOfficeCode)
                    {
                        //10.7.1
                        foreach (IVS080INST i in prm.ElemInstrument)
                        {
                            if (prm.Office == prm.HeadOfficeCode)
                            {
                                doGetShelfCurrentData doGetDestShelf = new doGetShelfCurrentData();
                                doGetDestShelf.OfficeCode = prm.Office;
                                doGetDestShelf.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                doGetDestShelf.ShelfNo = i.DestinationShelfNo;
                                doGetDestShelf.InstrumentCode = null;

                                List<doShelfCurrentData> doShelfCurrentDataDest = InvH.GetShelfCurrentData(doGetDestShelf);

                                //10.7.1.1
                                if (doShelfCurrentDataDest.Count > 0 && doShelfCurrentDataDest[0].ShelfType == ShelfType.C_INV_SHELF_TYPE_EMPTY)
                                {
                                    List<tbm_Shelf> GetTbm_Shelf = hand.GetTbm_Shelf(i.DestinationShelfNo);

                                    if (GetTbm_Shelf.Count > 0)
                                    {
                                        GetTbm_Shelf[0].ShelfTypeCode = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                                        GetTbm_Shelf[0].AreaCode = prm.DestinationArea;
                                        GetTbm_Shelf[0].InstrumentCode = i.InstrumentCode;
                                        GetTbm_Shelf[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                        GetTbm_Shelf[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                        List<tbm_Shelf> UpdateTbm_Shelf = hand.UpdateShelf(GetTbm_Shelf[0]);

                                        if (UpdateTbm_Shelf == null || UpdateTbm_Shelf.Count <= 0)
                                        {
                                            return Json(res);
                                        }
                                    }
                                }

                                //10.7.1.2
                                if (i.SourceShelfNo == i.DestinationShelfNo)
                                {
                                    #region R2
                                    doGetShelfCurrentData doGetShelf = new doGetShelfCurrentData();
                                    doGetShelf.OfficeCode = prm.Office;
                                    doGetShelf.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                    doGetShelf.AreaCode = prm.DestinationArea;
                                    doGetShelf.ShelfNo = null;
                                    doGetShelf.InstrumentCode = i.InstrumentCode;

                                    List<doShelfCurrentData> doShelfCurrentData = InvH.GetShelfForChecking(doGetShelf);

                                    #endregion R2

                                    if (doShelfCurrentData.Count > 0)
                                    {
                                        List<tbm_Shelf> GetTbm_Shelf = hand.GetTbm_Shelf(doShelfCurrentData[0].ShelfNo);

                                        if (GetTbm_Shelf.Count > 0)
                                        {
                                            GetTbm_Shelf[0].ShelfTypeCode = ShelfType.C_INV_SHELF_TYPE_EMPTY;
                                            GetTbm_Shelf[0].AreaCode = null;
                                            GetTbm_Shelf[0].InstrumentCode = null;
                                            GetTbm_Shelf[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                            GetTbm_Shelf[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                            List<tbm_Shelf> UpdateTbm_Shelf = hand.UpdateShelf(GetTbm_Shelf[0]);

                                            if (UpdateTbm_Shelf == null || UpdateTbm_Shelf.Count <= 0)
                                            {
                                                return Json(res);
                                            }

                                            #region R2
                                            List<tbt_InventoryCurrent> lstInventoryCurrent = InvH.DeleteTbt_InventoryCurrent(prm.Office, InstrumentLocation.C_INV_LOC_INSTOCK, prm.DestinationArea, doShelfCurrentData[0].ShelfNo, i.InstrumentCode);

                                            if (lstInventoryCurrent == null || lstInventoryCurrent.Count <= 0)
                                            {
                                                //throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_CURRENT });
                                            }
                                            #endregion R2

                                        }
                                    }

                                    List<tbm_Shelf> Get_Shelf = hand.GetTbm_Shelf(i.DestinationShelfNo);

                                    if (Get_Shelf.Count > 0)
                                    {
                                        Get_Shelf[0].AreaCode = prm.DestinationArea;
                                        Get_Shelf[0].InstrumentCode = i.InstrumentCode;
                                        Get_Shelf[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                        Get_Shelf[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                        List<tbm_Shelf> UpdateTbm_Shelf = hand.UpdateShelf(Get_Shelf[0]);

                                        if (UpdateTbm_Shelf == null || UpdateTbm_Shelf.Count <= 0)
                                        {
                                            return Json(res);
                                        }
                                    }

                                    InvH.DeleteTbt_InventoryCurrent(prm.Office, InstrumentLocation.C_INV_LOC_INSTOCK, prm.SourceArea, i.SourceShelfNo, i.InstrumentCode);
                                }
                            }

                            //10.7.1.3
                            if ((prm.SourceArea == InstrumentArea.C_INV_AREA_NEW_SAMPLE || prm.SourceArea == InstrumentArea.C_INV_AREA_NEW_DEMO) && prm.DestinationArea == InstrumentArea.C_INV_AREA_SE_LENDING_DEMO)
                            {
                                //10.7.1.4
                                decimal movingAvePrice = 0;
                                decimal? InstrumentPrice = null;

                                if (prm.SourceArea != InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                                {
                                    List<tbt_AccountInstock> doTbt_AccountInStock = InvH.GetTbt_AccountInStock(i.InstrumentCode, InstrumentLocation.C_INV_LOC_INSTOCK, prm.Office);

                                    if (doTbt_AccountInStock.Count > 0 && doTbt_AccountInStock[0].MovingAveragePrice != null)
                                        movingAvePrice = Convert.ToDecimal(doTbt_AccountInStock[0].MovingAveragePrice);

                                    InstrumentPrice = doTbt_AccountInStock[0].MovingAveragePrice;
                                }
                                else
                                {
                                    List<tbt_AccountSampleInstock> doTbt_AccountSampleInStock = InvH.GetTbt_AccountSampleInStock(i.InstrumentCode, InstrumentLocation.C_INV_LOC_INSTOCK, prm.Office);

                                    if (doTbt_AccountSampleInStock.Count > 0 && doTbt_AccountSampleInStock[0].MovingAveragePrice != null)
                                        movingAvePrice = Convert.ToDecimal(doTbt_AccountSampleInStock[0].MovingAveragePrice);

                                    InstrumentPrice = doTbt_AccountSampleInStock[0].MovingAveragePrice;
                                }


                                //10.7.1.5
                                doInsertDepreciationData doInsertDepreciationData = new doInsertDepreciationData();
                                doInsertDepreciationData.ContractCode = null;
                                doInsertDepreciationData.InstrumentCode = i.InstrumentCode;
                                doInsertDepreciationData.StartYearMonth = prm.TransferDate.Value.ToString("yyyyMM");
                                doInsertDepreciationData.MovingAveragePrice = movingAvePrice;
                                if (prm.SourceArea == InstrumentArea.C_INV_AREA_NEW_SAMPLE || prm.SourceArea == InstrumentArea.C_INV_AREA_NEW_DEMO)
                                {
                                    if (prm.DestinationArea == InstrumentArea.C_INV_AREA_SE_LENDING_DEMO)
                                    {
                                        doInsertDepreciationData.StartType = InventoryStartType.C_INV_STARTTYPE_CHANGEAREA;
                                    }
                                }

                                string strLotNo = InvH.InsertDepreciationData(doInsertDepreciationData);

                                //10.7.1.6
                                doGroupNewInstrument doGroupNewInstrument = new doGroupNewInstrument();
                                doGroupNewInstrument.SourceOfficeCode = prm.Office;
                                doGroupNewInstrument.DestinationOfficeCode = prm.Office;
                                doGroupNewInstrument.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                doGroupNewInstrument.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                doGroupNewInstrument.ProjectCode = null;
                                doGroupNewInstrument.ContractCode = null;
                                doGroupNewInstrument.Instrumentcode = i.InstrumentCode;
                                doGroupNewInstrument.TransferQty = i.TransferQty;
                                if (prm.SourceArea == InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                                    doGroupNewInstrument.ObjectID = InstrumentArea.C_INV_AREA_NEW_SAMPLE;
                                else
                                    doGroupNewInstrument.ObjectID = null;
                                doGroupNewInstrument.LotNo = strLotNo;

                                //10.7.1.7
                                #region Monthly Price @ 2015
                                //decimal decMovingAveragePrice = InvH.CalculateMovingAveragePrice(doGroupNewInstrument);
                                decimal decMovingAveragePrice = InvH.GetMonthlyAveragePrice(doGroupNewInstrument.Instrumentcode, InvSlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                                #endregion

                                //10.7.1.8
                                bool blnUpdate = false;

                                if (prm.SourceArea != InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                                    blnUpdate = InvH.UpdateAccountTransferNewInstrument(doGroupNewInstrument, Convert.ToDecimal(decMovingAveragePrice));
                                else
                                {
                                    doGroupSampleInstrument doGroupSampleInstrument = CommonUtil.CloneObject<doGroupNewInstrument, doGroupSampleInstrument>(doGroupNewInstrument);
                                    blnUpdate = InvH.UpdateAccountTransferSampleInstrument(doGroupSampleInstrument, Convert.ToDecimal(decMovingAveragePrice));
                                }

                                if (!blnUpdate)
                                {
                                    return Json(res);
                                }

                                //10.7.1.9
                                tbt_AccountStockMoving doTbt_AccountStockMoving = new tbt_AccountStockMoving();
                                doTbt_AccountStockMoving.MovingNo = 0;
                                doTbt_AccountStockMoving.SlipNo = strInventorySlipNo;
                                doTbt_AccountStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_TRANSFER_AREA;
                                doTbt_AccountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                                doTbt_AccountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTALLED;
                                doTbt_AccountStockMoving.SourceLocationCode = doGroupNewInstrument.SourceLocationCode;
                                doTbt_AccountStockMoving.DestinationLocationCode = doGroupNewInstrument.DestinationLocationCode;
                                doTbt_AccountStockMoving.InstrumentCode = doGroupNewInstrument.Instrumentcode;
                                doTbt_AccountStockMoving.InstrumentQty = doGroupNewInstrument.TransferQty;
                                doTbt_AccountStockMoving.InstrumentPrice = InstrumentPrice;
                                doTbt_AccountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                doTbt_AccountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                doTbt_AccountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                doTbt_AccountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                List<tbt_AccountStockMoving> lstTbt_AccountStock = new List<tbt_AccountStockMoving>();
                                lstTbt_AccountStock.Add(doTbt_AccountStockMoving);

                                List<tbt_AccountStockMoving> lstdoTbt_AccountStockMoving = InvH.InsertAccountStockMoving(lstTbt_AccountStock);

                                if (lstdoTbt_AccountStockMoving == null || lstdoTbt_AccountStockMoving.Count <= 0)
                                {
                                    return Json(res);
                                }
                            }
                        }
                    }


                    // Generate report  C_INV_REPORT_ID_TRANSFER_WITHIN_WH = IVR050
                    IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                    string strSlipNoReportPath = handlerInventoryDocument.GenerateIVR050FilePath(strInventorySlipNo, prm.Office, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                    prm.slipNoReportPath = strSlipNoReportPath;
                    prm.slipNo = strInventorySlipNo;

                    //10.9
                    scope.Complete();
                }

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
        public ActionResult IVS080_UpdateRowIDElem(IVS080INST Cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IVS080_ScreenParameter prm = GetScreenObject<IVS080_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS080INST>();

                foreach (IVS080INST i in prm.ElemInstrument)
                {
                    //if (Cond.InstrumentCode == i.InstrumentCode && Cond.AreaCode == i.AreaCode)
                    //{
                    //    i.row_id = Cond.row_id;
                    //    break;
                    //}
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
        public ActionResult IVS080_InitParam()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS080_ScreenParameter prm = GetScreenObject<IVS080_ScreenParameter>();
                prm.ElemInstrument = new List<IVS080INST>();
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
        /// <param name="OfficeCode">Office code</param>
        /// <returns></returns>
        public ActionResult IVS080_beforeAddElem(IVS080INST Cond, string OfficeCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS080_ScreenParameter prm = GetScreenObject<IVS080_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS080INST>();


                foreach (IVS080INST i in prm.ElemInstrument)
                {
                    if (i.InstrumentCode == Cond.InstrumentCode && i.ShelfNo == Cond.ShelfNo)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4005);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }

                if (OfficeCode != prm.HeadOfficeCode)
                {
                    Cond.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                }
                else
                {
                    Cond.DestinationShelfNo = "";
                }

                prm.ElemInstrument.Add(Cond);

                UpdateScreenObject(prm);

                res.ResultData = Cond;
                return Json(res);
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
        /// <param name="ShelfNo">Remove selected  shelf no</param>
        /// <returns></returns>
        public ActionResult IVS080_DelElem(string InstrumentCode, string ShelfNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS080_ScreenParameter prm = GetScreenObject<IVS080_ScreenParameter>();

                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS080INST>();

                for (int i = 0; i < prm.ElemInstrument.Count; i++)
                    if (prm.ElemInstrument[i].InstrumentCode == InstrumentCode && prm.ElemInstrument[i].ShelfNo == ShelfNo)
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
        public ActionResult IVS080_CheckExistFile()
        {
            try
            {
                IVS080_ScreenParameter param = GetScreenObject<IVS080_ScreenParameter>();
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
        public ActionResult IVS080_DownloadPdfAndWriteLog()
        {
            try
            {

                IVS080_ScreenParameter param = GetScreenObject<IVS080_ScreenParameter>();
                string fileName = param.slipNoReportPath;

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = param.slipNo,
                    DocumentCode = ReportID.C_INV_REPORT_ID_TRANSFER_WITHIN_WH, // IVR050
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