


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
        public ActionResult IVS090_Authority(IVS090_ScreenParameter param)
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
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_SPECIAL_STOCKOUT, FunctionID.C_FUNC_ID_OPERATE))
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
                
                param.IsSpecialOutMaterial = CheckUserPermission(ScreenID.C_INV_SCREEN_ID_SPECIAL_STOCKOUT, FunctionID.C_FUNC_ID_SPECIAL_STOCK_OUT_MATERIAL);

                var cfgAreaCode = comh.GetSystemConfig(ConfigName.C_CONFIG_SPECIAL_STOCKOUT_MATERIAL).FirstOrDefault();
                param.SpecialOutMaterialAreaCode = (cfgAreaCode == null ? null : cfgAreaCode.ConfigValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                
                List<doMiscTypeCode> lstAreaCode = comh.GetMiscTypeCodeListByFieldName(new List<string>() { MiscType.C_INV_AREA });
                var qAvailableAreaCode = (
                    from c in lstAreaCode
                    where c.ValueCode != InstrumentArea.C_INV_AREA_SE_LENDING_DEMO
                    select c
                );

                if (param.IsSpecialOutMaterial)
                {
                    qAvailableAreaCode = qAvailableAreaCode.Where(d =>
                        param.SpecialOutMaterialAreaCode != null
                        && param.SpecialOutMaterialAreaCode.Contains(d.ValueCode)
                    );
                }

                param.AvailableAreaCode = qAvailableAreaCode.Select(d => d.ValueCode).ToArray();
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS090_ScreenParameter>("IVS090", param, res);
        }

        /// <summary>
        /// Initial Screen
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS090")]
        public ActionResult IVS090()
        {
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

            tbs_MiscellaneousTypeCode m = IVS090_GetSourceLocationLocation();

            ViewBag.SourceLocation = m.ValueDisplay;
            ViewBag.SourceLocationCode = m.ValueCode;

            ViewBag.DestLocation = IVS090_GetDestinationLocation();

            ViewBag.Total = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INVENTORY, ScreenID.C_INV_SCREEN_ID_SPECIAL_STOCKOUT, "lblTotalAmountOfTransferAsset");

            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            var sParam = this.GetScreenObject<IVS090_ScreenParameter>();

            ViewBag.IsSpecialOutMaterial = sParam.IsSpecialOutMaterial;
            ViewBag.SpecialOutMaterialAreaCode = sParam.SpecialOutMaterialAreaCode;

            return View();
        }
        #endregion

        /// <summary>
        /// Search inventory instrument
        /// </summary>
        /// <param name="Cond">Search condition object</param>
        /// <returns></returns>
        public ActionResult IVS090_SearchInventoryInstrument(IVS090SearchCond Cond)
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

                IVS090_ScreenParameter param = GetScreenObject<IVS090_ScreenParameter>();

                List<dtSearchInstrumentListResult> lstResult = InvH.SearchInventoryInstrumentList(
                    param.office.OfficeCode,
                    InstrumentLocation.C_INV_LOC_INSTOCK,
                    (Cond.InstArea ?? CommonUtil.CreateCSVString(param.AvailableAreaCode)),
                    ShelfType.C_INV_SHELF_TYPE_NORMAL,
                    null,
                    null,
                    Cond.InstName,
                    Cond.InstCode,
                    new string[] { InstrumentArea.C_INV_AREA_SE_LENDING_DEMO }
                );

                if (lstResult.Count <= 0)
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

                res.ResultData = CommonUtil.ConvertToXml<dtSearchInstrumentListResult>(lstResult, "inventory\\IVS090_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        public ActionResult IVS090_InstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS090_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Initial special stock-out instrument grid control
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS090_TransferStockInstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS090_TransferStock", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Get destination location
        /// </summary>
        /// <returns></returns>
        public string IVS090_GetDestinationLocation()
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
                           where (c.ValueCode == InstrumentLocation.C_INV_LOC_SPECIAL)
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
        /// Get source location
        /// </summary>
        /// <returns></returns>
        public tbs_MiscellaneousTypeCode IVS090_GetSourceLocationLocation()
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
                    return ResMisc[0];
                else
                    return new tbs_MiscellaneousTypeCode();
            }
            catch (Exception)
            {
                return new tbs_MiscellaneousTypeCode();
            }
        }

        /// <summary>
        /// Validate register special stock-out instrument data
        /// </summary>
        /// <param name="Cond">Register condition object</param>
        /// <returns></returns>
        public ActionResult IVS090_cmdReg(IVS090RegisterCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                IVS090_ScreenParameter prm = GetScreenObject<IVS090_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS090INST>();
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                if (comh.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_SPECIAL_STOCKOUT, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //prm.ElemInstrument = Cond.StockInInstrument;
                for (int i = 0; i < Cond.StockInInstrument.Count; i++)
                {
                    if(Cond.StockInInstrument[i].row_id == prm.ElemInstrument[i].row_id)
                    {
                        prm.ElemInstrument[i].StockOutQty = Cond.StockInInstrument[i].StockOutQty;
                    }
                }

                prm.Memo = Cond.Memo;
                prm.StockOutDate = Cond.StockOutDate;
                prm.Location = Cond.SourceLoc;
                prm.ApproveNo = Cond.ApproveNo;

                //Valid Cond
                //6.1.1
                if (string.IsNullOrEmpty(prm.ApproveNo))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY,
                                        ScreenID.C_INV_SCREEN_ID_SPECIAL_STOCKOUT,
                                        MessageUtil.MODULE_INVENTORY,
                                        MessageUtil.MessageList.MSG4112,
                                        new string[] { "lblApproveNo" },
                                        new string[] { "ApproveNo" });

                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                //6.2.1
                if (prm.ElemInstrument == null || prm.ElemInstrument.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4006);
                    return Json(res);
                }

                //6.2.2
                if (!string.IsNullOrEmpty(prm.Memo) && prm.Memo.Replace(" ", "").Contains("\n\n\n\n"))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4022, null, new string[] { "memo" });
                    return Json(res);
                }

                //6.3
                foreach (IVS090INST i in prm.ElemInstrument)
                {
                    if (i.StockOutQty <= 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4030, new string[] { i.Instrumentcode }, new string[] { i.StockOutQty_id });
                    }

                }

                bool isError = false;
                foreach (IVS090INST i in prm.ElemInstrument)
                {
                    doCheckTransferQty doCheck = new doCheckTransferQty();
                    doCheck.OfficeCode = prm.office.OfficeCode;
                    doCheck.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                    doCheck.AreaCode = i.AreaCode;
                    doCheck.ShelfNo = i.ShelfNo;
                    doCheck.InstrumentCode = i.Instrumentcode;
                    doCheck.TransferQty = i.StockOutQty;
                    doCheckTransferQtyResult doCheckResult = InvH.CheckTransferQty(doCheck);

                    i.InstrumentQty = doCheckResult.CurrentyQty;

                    if (doCheckResult.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.Instrumentcode }, new string[] { i.StockOutQty_id });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        isError = true;
                    }
                    else if (doCheckResult.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.Instrumentcode }, new string[] { i.StockOutQty_id });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        isError = true;
                    }
                }
                if (isError)
                {
                    res.ResultData = prm.ElemInstrument;
                    return Json(res);
                }

                CalculateTransferAmount(prm);

                res.ResultData = prm.ElemInstrument;

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Register special stock-out instrument data
        /// </summary>
        /// <param name="Con">Register condition object</param>
        /// <returns></returns>
        public ActionResult IVS090_cmdConfirm(IVS090RegisterCond Con)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (ModelState.IsValid == false)
                {
                    ValidatorUtil.BuildErrorMessage(res, this);
                    if (res.IsError)
                        return Json(res);
                }

                IVS090_ScreenParameter prm = GetScreenObject<IVS090_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS090INST>();

                //prm.ElemInstrument = Cond.StockInInstrument;
                for (int i = 0; i < Con.StockInInstrument.Count; i++)
                {
                    if (Con.StockInInstrument[i].row_id == prm.ElemInstrument[i].row_id)
                    {
                        prm.ElemInstrument[i].StockOutQty = Con.StockInInstrument[i].StockOutQty;
                    }
                }
                prm.Location = Con.SourceLoc;
                prm.ApproveNo = Con.ApproveNo;
                prm.Memo = Con.Memo;
                prm.StockOutDate = Con.StockOutDate;

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                if (comh.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_SPECIAL_STOCKOUT, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                foreach (IVS090INST i in prm.ElemInstrument)
                {
                    doCheckTransferQty Cond = new doCheckTransferQty();
                    Cond.OfficeCode = prm.office.OfficeCode;
                    Cond.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                    Cond.AreaCode = i.AreaCode;
                    Cond.ShelfNo = i.ShelfNo;
                    Cond.InstrumentCode = i.Instrumentcode;
                    Cond.TransferQty = i.StockOutQty;

                    doCheckTransferQtyResult TransferQtyResult = InvH.CheckTransferQty(Cond);

                    i.InstrumentQty = TransferQtyResult.CurrentyQty;

                    //8.4.1 
                    if (TransferQtyResult.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.Instrumentcode }, new string[] { i.StockOutQty_id });
                        res.ResultData = prm.ElemInstrument;
                        i.IsError = true;
                        return Json(res);
                    }
                    else if (TransferQtyResult.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.Instrumentcode }, new string[] { i.StockOutQty_id });
                        res.ResultData = prm.ElemInstrument;
                        i.IsError = true;
                        return Json(res);
                    }
                }

                string strInventorySlipNo = string.Empty;

                using (TransactionScope scope = new TransactionScope())
                {

                    //8.5.1
                    doRegisterTransferInstrumentData registerTransferInstrument = new doRegisterTransferInstrumentData();

                    registerTransferInstrument.SlipId = SlipID.C_INV_SLIPID_STOCKOUT_SPECIAL;
                    registerTransferInstrument.InventorySlip = new tbt_InventorySlip();
                    registerTransferInstrument.InventorySlip.SlipNo = null;
                    registerTransferInstrument.InventorySlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
                    registerTransferInstrument.InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_SPECIAL;
                    registerTransferInstrument.InventorySlip.InstallationSlipNo = null;
                    registerTransferInstrument.InventorySlip.ProjectCode = null;
                    registerTransferInstrument.InventorySlip.PurchaseOrderNo = null;
                    registerTransferInstrument.InventorySlip.ContractCode = null;
                    registerTransferInstrument.InventorySlip.SlipIssueDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    registerTransferInstrument.InventorySlip.StockInDate = prm.StockOutDate;
                    registerTransferInstrument.InventorySlip.StockOutDate = prm.StockOutDate;
                    registerTransferInstrument.InventorySlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                    registerTransferInstrument.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_SPECIAL;
                    registerTransferInstrument.InventorySlip.SourceOfficeCode = prm.office.OfficeCode;
                    registerTransferInstrument.InventorySlip.DestinationOfficeCode = prm.office.OfficeCode;
                    registerTransferInstrument.InventorySlip.ApproveNo = prm.ApproveNo;
                    registerTransferInstrument.InventorySlip.Memo = prm.Memo;
                    registerTransferInstrument.InventorySlip.StockInFlag = null;
                    registerTransferInstrument.InventorySlip.DeliveryOrderNo = null;
                    registerTransferInstrument.InventorySlip.ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                    registerTransferInstrument.InventorySlip.RepairSubcontractor = null;
                    registerTransferInstrument.InventorySlip.InstallationCompleteFlag = null;
                    registerTransferInstrument.InventorySlip.ContractStartServiceFlag = null;
                    registerTransferInstrument.InventorySlip.CustomerAcceptanceFlag = null;
                    //registerTransferInstrument.InventorySlip.ProjectCompleteFlag = null;
                    registerTransferInstrument.InventorySlip.PickingListNo = null;
                    registerTransferInstrument.InventorySlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    registerTransferInstrument.InventorySlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    registerTransferInstrument.InventorySlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    registerTransferInstrument.InventorySlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    //8.5.2
                    registerTransferInstrument.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();

                    int iRunNo = 1;
                    string tmpTotalCurrencyType = "";
                    foreach (IVS090INST i in prm.ElemInstrument)
                    {
                        tbt_InventorySlipDetail SlipDetail = new tbt_InventorySlipDetail();
                        SlipDetail.SlipNo = null;
                        SlipDetail.RunningNo = iRunNo;
                        SlipDetail.InstrumentCode = i.Instrumentcode;
                        SlipDetail.SourceAreaCode = i.AreaCode;
                        SlipDetail.DestinationAreaCode = i.AreaCode;
                        SlipDetail.SourceShelfNo = i.ShelfNo;
                        SlipDetail.DestinationShelfNo = i.ShelfNo;
                        SlipDetail.TransferQty = i.StockOutQty;
                        SlipDetail.NotInstalledQty = null;
                        SlipDetail.RemovedQty = null;
                        SlipDetail.UnremovableQty = null;
                        if(i.TransferAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                        {
                            SlipDetail.InstrumentAmount = i.TransferAmount;
                            SlipDetail.InstrumentAmountUsd = null;
                            SlipDetail.InstrumentAmountCurrencyType = i.TransferAmountCurrencyType;

                            tmpTotalCurrencyType = i.TransferAmountCurrencyType;
                        }
                        else
                        {
                            SlipDetail.InstrumentAmount = null;
                            SlipDetail.InstrumentAmountUsd = i.TransferAmount;
                            SlipDetail.InstrumentAmountCurrencyType = i.TransferAmountCurrencyType;

                            tmpTotalCurrencyType = i.TransferAmountCurrencyType;
                        }
                        
                        registerTransferInstrument.lstTbt_InventorySlipDetail.Add(SlipDetail);
                        iRunNo++;
                    }

                    //8.6
                    strInventorySlipNo = InvH.RegisterTransferInstrument(registerTransferInstrument);

                    //8.7
                    if (InvH.CheckNewInstrument(strInventorySlipNo) == 1)
                    {
                        //8.7.1
                        List<doGroupNewInstrument> doGroupNewInstrument = InvH.GetGroupNewInstrument(strInventorySlipNo);

                        //8.7.3
                        foreach (doGroupNewInstrument i in doGroupNewInstrument)
                        {
                            #region Monthly Price @ 2015
                            //decimal decMovingAveragePrice = InvH.CalculateMovingAveragePrice(i);
                            decimal decMovingAveragePrice = InvH.GetMonthlyAveragePrice(i.Instrumentcode, registerTransferInstrument.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                            #endregion
                            bool blnUpdate = InvH.UpdateAccountTransferNewInstrument(i, Convert.ToDecimal(decMovingAveragePrice));

                            if (!blnUpdate)
                                return Json(res);

                            List<tbt_AccountInstock> doTbt_AccountInstock = InvH.GetTbt_AccountInStock(i.Instrumentcode, InstrumentLocation.C_INV_LOC_INSTOCK, prm.office.OfficeCode);

                            //8.7.3.1
                            tbt_AccountStockMoving doTbt_AccoutStockMoving = new tbt_AccountStockMoving();
                            doTbt_AccoutStockMoving.SlipNo = strInventorySlipNo;
                            doTbt_AccoutStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_SPECIAL;
                            doTbt_AccoutStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                            doTbt_AccoutStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_SPECIAL;
                            doTbt_AccoutStockMoving.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                            doTbt_AccoutStockMoving.DestinationLocationCode = InstrumentLocation.C_INV_LOC_SPECIAL;
                            doTbt_AccoutStockMoving.InstrumentCode = i.Instrumentcode;
                            doTbt_AccoutStockMoving.InstrumentQty = i.TransferQty;
                            if (tmpTotalCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                            {
                                doTbt_AccoutStockMoving.InstrumentPrice = Convert.ToDecimal(decMovingAveragePrice);
                                doTbt_AccoutStockMoving.InstrumentPriceUsd = null;
                                doTbt_AccoutStockMoving.InstrumentPriceCurrencyType = tmpTotalCurrencyType;
                            }
                            else
                            {
                                doTbt_AccoutStockMoving.InstrumentPrice = null;
                                doTbt_AccoutStockMoving.InstrumentPriceUsd = Convert.ToDecimal(decMovingAveragePrice);
                                doTbt_AccoutStockMoving.InstrumentPriceCurrencyType = tmpTotalCurrencyType;
                            }
                            doTbt_AccoutStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            doTbt_AccoutStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            doTbt_AccoutStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            doTbt_AccoutStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            List<tbt_AccountStockMoving> lstTbt_AccoutStockMoving = new List<tbt_AccountStockMoving>();
                            lstTbt_AccoutStockMoving.Add(doTbt_AccoutStockMoving);

                            List<tbt_AccountStockMoving> lstAccountResult = InvH.InsertTbt_AccountStockMoving(lstTbt_AccoutStockMoving);

                            if (lstAccountResult == null)
                                return Json(res);
                        }
                    }

                    //8.8
                    int blnCheckSecondhandInstrument = InvH.CheckSecondhandInstrument(strInventorySlipNo);
                    if (blnCheckSecondhandInstrument == 1)
                    {
                        //8.8.1
                        List<doGroupSecondhandInstrument> GroupSecondHandInstrument = InvH.GetGroupSecondhandInstrument(strInventorySlipNo);
                        foreach (doGroupSecondhandInstrument i in GroupSecondHandInstrument)
                        {
                            //8.8.1.1
                            bool blnUpdate = InvH.UpdateAccountTransferSecondhandInstrument(i);

                            if (!blnUpdate)
                                return Json(res);

                            //8.8.1.2
                            decimal SumTransferringAmount = 0;
                            foreach (IVS090INST k in prm.ElemInstrument)
                            {
                                if (i.Instrumentcode == k.Instrumentcode && (k.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || k.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO))
                                    SumTransferringAmount += k.TransferAmount;
                                    tmpTotalCurrencyType = k.TransferAmountCurrencyType;
                            }

                            //8.8.1.3
                            tbt_AccountStockMoving doTbt_AccoutStockMoving = new tbt_AccountStockMoving();
                            doTbt_AccoutStockMoving.SlipNo = strInventorySlipNo;
                            doTbt_AccoutStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_SPECIAL;
                            doTbt_AccoutStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTALLED;
                            doTbt_AccoutStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_SPECIAL;
                            doTbt_AccoutStockMoving.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                            doTbt_AccoutStockMoving.DestinationLocationCode = InstrumentLocation.C_INV_LOC_SPECIAL;
                            doTbt_AccoutStockMoving.InstrumentCode = i.Instrumentcode;
                            doTbt_AccoutStockMoving.InstrumentQty = i.TransferQty;
                            if(tmpTotalCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                            {
                                doTbt_AccoutStockMoving.InstrumentPrice = SumTransferringAmount / i.TransferQty;
                                doTbt_AccoutStockMoving.InstrumentPriceUsd = null;
                                doTbt_AccoutStockMoving.InstrumentPriceCurrencyType = tmpTotalCurrencyType;
                            }
                            else
                            {
                                doTbt_AccoutStockMoving.InstrumentPrice = null;
                                doTbt_AccoutStockMoving.InstrumentPriceUsd = SumTransferringAmount / i.TransferQty;
                                doTbt_AccoutStockMoving.InstrumentPriceCurrencyType = tmpTotalCurrencyType;
                            }
                            doTbt_AccoutStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            doTbt_AccoutStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            doTbt_AccoutStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            doTbt_AccoutStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            List<tbt_AccountStockMoving> lstTbt_AccoutStockMoving = new List<tbt_AccountStockMoving>();
                            lstTbt_AccoutStockMoving.Add(doTbt_AccoutStockMoving);

                            List<tbt_AccountStockMoving> lstAccountResult = InvH.InsertTbt_AccountStockMoving(lstTbt_AccoutStockMoving);

                            if (lstAccountResult == null)
                                return Json(res);
                        }
                    }

                    //8.9
                    int blnCheckSampleInstrument = InvH.CheckSampleInstrument(strInventorySlipNo);
                    if (blnCheckSampleInstrument == 1)
                    {
                        //8.9.1
                        List<doGroupSampleInstrument> GroupSampleInstrument = InvH.GetGroupSampleInstrument(strInventorySlipNo);
                        foreach (doGroupSampleInstrument i in GroupSampleInstrument)
                        {
                            //8.9.1.1
                            bool blnUpdate = InvH.UpdateAccountTransferSampleInstrument(i, null);
                            if (!blnUpdate)
                                return Json(res);

                            tbt_AccountStockMoving doTbt_AccoutStockMoving = new tbt_AccountStockMoving();
                            doTbt_AccoutStockMoving.SlipNo = strInventorySlipNo;
                            doTbt_AccoutStockMoving.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_SPECIAL;
                            doTbt_AccoutStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                            doTbt_AccoutStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_SPECIAL;
                            doTbt_AccoutStockMoving.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                            doTbt_AccoutStockMoving.DestinationLocationCode = InstrumentLocation.C_INV_LOC_SPECIAL;
                            doTbt_AccoutStockMoving.InstrumentCode = i.Instrumentcode;
                            doTbt_AccoutStockMoving.InstrumentQty = i.TransferQty;
                            doTbt_AccoutStockMoving.InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                            if (tmpTotalCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                            {
                                doTbt_AccoutStockMoving.InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                                doTbt_AccoutStockMoving.InstrumentPriceUsd = null;
                                doTbt_AccoutStockMoving.InstrumentPriceCurrencyType = tmpTotalCurrencyType;
                            }
                            else
                            {
                                doTbt_AccoutStockMoving.InstrumentPrice = null;
                                doTbt_AccoutStockMoving.InstrumentPriceUsd = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                                doTbt_AccoutStockMoving.InstrumentPriceCurrencyType = tmpTotalCurrencyType;
                            }
                            doTbt_AccoutStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            doTbt_AccoutStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            doTbt_AccoutStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            doTbt_AccoutStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            List<tbt_AccountStockMoving> lstTbt_AccoutStockMoving = new List<tbt_AccountStockMoving>();
                            lstTbt_AccoutStockMoving.Add(doTbt_AccoutStockMoving);

                            List<tbt_AccountStockMoving> lstAccountResult = InvH.InsertTbt_AccountStockMoving(lstTbt_AccoutStockMoving);

                            if (lstAccountResult == null)
                                return Json(res);
                        }
                    }

                    scope.Complete();
                }

                //8.10.1
                IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                string strSlipNoReportPath = handlerInventoryDocument.GenerateIVR060FilePath(strInventorySlipNo, prm.office.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                prm.SlipNoReportPath = strSlipNoReportPath;
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
        public ActionResult IVS090_UpdateRowIDElem(IVS090INST Cond)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                IVS090_ScreenParameter prm = GetScreenObject<IVS090_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS090INST>();

                foreach (IVS090INST i in prm.ElemInstrument)
                {
                    if (Cond.Instrumentcode == i.Instrumentcode && Cond.AreaCode == i.AreaCode)
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
        public ActionResult IVS090_InitParam()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS090_ScreenParameter prm = GetScreenObject<IVS090_ScreenParameter>();
                prm.ElemInstrument = new List<IVS090INST>();
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
        public ActionResult IVS090_beforeAddElem(IVS090INST Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS090_ScreenParameter prm = GetScreenObject<IVS090_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS090INST>();


                foreach (IVS090INST i in prm.ElemInstrument)
                {
                    if (i.Instrumentcode == Cond.Instrumentcode && i.AreaCode == Cond.AreaCode)
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
                if(Cond.TransferAmountCurrencyType == null)
                {
                    Cond.TransferAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
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
        /// <param name="Instrumentcode">Remove selecedt instrument code</param>
        /// <param name="AreaCode">Remove selected  Area code</param>
        /// <returns></returns>
        public ActionResult IVS090_DelElem(string Instrumentcode, string AreaCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS090_ScreenParameter prm = GetScreenObject<IVS090_ScreenParameter>();

                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS090INST>();

                for (int i = 0; i < prm.ElemInstrument.Count; i++)
                    if (prm.ElemInstrument[i].Instrumentcode == Instrumentcode && prm.ElemInstrument[i].AreaCode == AreaCode)
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
        public ActionResult IVS090_CheckExistFile()
        {
            try
            {
                IVS090_ScreenParameter param = GetScreenObject<IVS090_ScreenParameter>();
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
        /// Download slip report
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS090_DownloadPdfAndWriteLog()
        {
            try
            {

                IVS090_ScreenParameter param = GetScreenObject<IVS090_ScreenParameter>();
                string fileName = param.SlipNoReportPath;

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = param.slipNo,
                    DocumentCode = ReportID.C_INV_REPORT_ID_SPECIAL_STOCKOUT, // IVR060
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

        /// <summary>
        /// Calculate transferring amount
        /// </summary>
        /// <param name="Cond">instrument data</param>
        /// <param name="SourceLoc">Source location</param>
        /// <returns></returns>
        public ActionResult IVS090_Calculate(List<IVS090INST> Cond, string SourceLoc)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS090_ScreenParameter prm = GetScreenObject<IVS090_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS090INST>();

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                bool isError = false;
                //5.2
                for (int i = 0; i < Cond.Count; i++)
                {
                    foreach (IVS090INST k in prm.ElemInstrument)
                    {
                        if (Cond[i].Instrumentcode == k.Instrumentcode &&
                            Cond[i].AreaCode == k.AreaCode)
                        {
                            k.StockOutQty = Cond[i].StockOutQty;
                            k.StockOutQty_id = Cond[i].StockOutQty_id;

                            if (k.StockOutQty <= 0)
                            {
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4030, new string[] { k.Instrumentcode }, new string[] { k.StockOutQty_id });
                                isError = true;
                                continue;
                            }

                            doCheckTransferQty doCheck = new doCheckTransferQty();
                            doCheck.OfficeCode = prm.office.OfficeCode;
                            doCheck.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                            doCheck.AreaCode = k.AreaCode;
                            doCheck.ShelfNo = k.ShelfNo;
                            doCheck.InstrumentCode = k.Instrumentcode;
                            doCheck.TransferQty = k.StockOutQty;
                            doCheckTransferQtyResult doCheckResult = InvH.CheckTransferQty(doCheck);

                            k.InstrumentQty = doCheckResult.CurrentyQty;

                            if (doCheckResult.OverQtyFlag == null)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { k.Instrumentcode }, new string[] { k.StockOutQty_id });
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                isError = true;
                            }
                            else if (doCheckResult.OverQtyFlag == true)
                            {
                                res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { k.Instrumentcode }, new string[] { k.StockOutQty_id });
                                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                                isError = true;
                            }
                        }
                    }
                }

                if (isError)
                {
                    res.ResultData = prm.ElemInstrument;
                    return Json(res);
                }

                //5.3
                CalculateTransferAmount(prm);

                UpdateScreenObject(prm);

                res.ResultData = prm.ElemInstrument;

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        private void CalculateTransferAmount(IVS090_ScreenParameter prm)
        {
            try
            {
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                List<IVS090INST> PrevInstTransferArr = new List<IVS090INST>();

                foreach (IVS090INST k in prm.ElemInstrument)
                {
                    int stockqty = 0;

                    if (k.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || k.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO)
                    {
                        IVS090INST inv = PrevInstTransferArr.Find(delegate(IVS090INST match)
                        {
                            return (match.Instrumentcode == k.Instrumentcode);
                        });

                        if (inv != null)
                        {
                            inv.StockOutQty += k.StockOutQty;
                            stockqty = inv.StockOutQty;
                        }
                        else
                        {
                            PrevInstTransferArr.Add(k);
                            stockqty = k.StockOutQty;
                        }

                        stockqty -= k.StockOutQty;

                        doFIFOInstrumentPrice MovingAveragePrice = InvH.GetFIFOInstrumentPrice(k.StockOutQty, prm.office.OfficeCode, InstrumentLocation.C_INV_LOC_INSTOCK, k.Instrumentcode, stockqty);

                        if (MovingAveragePrice.decTransferAmount != null)
                        {
                            k.TransferAmount = Convert.ToDecimal(MovingAveragePrice.decTransferAmount);
                            k.TransferAmountCurrencyType = MovingAveragePrice.decTransferAmountCurrencyType;
                        }
                        else
                            k.TransferAmount = 0;
                    }
                    else if (k.AreaCode == InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                    {
                        k.TransferAmount = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                        k.TransferAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    }
                    else
                    {
                        doCalPriceCondition CalPrice = InvH.GetMovingAveragePriceCondition(prm.office.OfficeCode, null, null, k.Instrumentcode, new string[] { InstrumentLocation.C_INV_LOC_INSTOCK }, null);

                        if (CalPrice != null && CalPrice.MovingAveragePrice != null)
                        {
                            k.TransferAmount = Convert.ToDecimal(CalPrice.MovingAveragePrice) * k.StockOutQty;
                            k.TransferAmountCurrencyType = CalPrice.MovingAveragePriceCurrencyType;
                        }
                        else
                            k.TransferAmount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}