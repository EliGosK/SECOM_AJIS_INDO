//*********************************
// Create by: Natthavat S.
// Create date: 02/FEB/2012
// Update date: 02/FEB/2012
//*********************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Presentation.Inventory.Models;

namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {
        #region Authority

        /// <summary>
        /// Checking user's permission.
        /// </summary>
        /// <param name="param">Screen's parameter.</param>
        /// <returns>Return ActionResult of the screen.</returns>
        public ActionResult IVS270_Authority(IVS270_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {

                ICommonHandler srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (srvCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    //res.ResultData = MessageUtil.MessageList.MSG0049.ToString();
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_PROJECT_STOCKOUT, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    //res.ResultData = MessageUtil.MessageList.MSG0053.ToString();
                    return Json(res);
                }

                IInventoryHandler srvInv = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                if (srvInv.CheckFreezedData() == 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4002);
                    //res.ResultData = MessageUtil.MessageList.MSG4002.ToString();
                    return Json(res);
                }

                if (srvInv.CheckStartedStockChecking() == 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    //res.ResultData = MessageUtil.MessageList.MSG4003.ToString();
                    return Json(res);
                }

                List<doOffice> lstHeadOffice = srvInv.GetInventoryHeadOffice();
                if (lstHeadOffice.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4016);
                    return Json(res);
                }

            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS270_ScreenParameter>("IVS270", param, res);
        }

        #endregion

        #region Action

        /// <summary>
        /// Initialize screen.
        /// </summary>
        /// <returns>Return ActionResult of the screen.</returns>
        [Initialize("IVS270")]
        public ActionResult IVS270()
        {
            var srvInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            var lstHQ = srvInventory.GetInventoryHeadOffice();
            if (lstHQ.Count > 0)
            {
                ViewBag.InvHeadOfficeCode = lstHQ[0].OfficeCode;
                ViewBag.InvHeadOfficeName = lstHQ[0].OfficeName;
            }

            var srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var objLocation = srvCommon.GetMiscTypeCodeListByFieldName(
                new List<string>() { MiscType.C_INV_LOC }
            ).First(
                p => p.ValueCode == InstrumentLocation.C_INV_LOC_INSTOCK
            );

            if (objLocation != null)
            {
                ViewBag.LocationCode = objLocation.ValueCode;
                ViewBag.LocationName = objLocation.ValueDisplay;
            }

            return View();
        }

        /// <summary>
        /// Get data for initialize instrument result grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data for initialize instrument result grid.</returns>
        public ActionResult IVS270_InitialInstrumentResult()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, @"Inventory\IVS270_InstrumentResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get data for initialize stock-out detail grid.
        /// </summary>
        /// <returns>Return ActionResult of JSON data for initialize stock-out detail grid.</returns>
        public ActionResult IVS270_InitialStockOutDetail()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, @"Inventory\IVS270_StockOutDetail", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Retrive project information.
        /// </summary>
        /// <param name="strProjectCode">Project Code.</param>
        /// <returns>Return ActionResult of JSON data contains project information.</returns>
        public ActionResult IVS270_GetProjectInformation(string strProjectCode)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (string.IsNullOrEmpty(strProjectCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4066, 
                        null, new string[] { "txtProjectCode" });
                    res.ResultData = this.IVS270_GetProjectInformation_CreateResult(false, null);
                }
                else
                {
                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                    var lstProjInfo = service.GetProjectInformation(strProjectCode);
                    if (lstProjInfo.Count <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4067,
                            new string[] { strProjectCode }, new string[] { "txtProjectCode" });
                        res.ResultData = this.IVS270_GetProjectInformation_CreateResult(false, null);
                    }
                    else if (lstProjInfo[0].ProjectStatus != ProjectStatus.C_PROJECT_STATUS_PROCESSING)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4070, 
                            null, new string[] { "txtProjectCode" });
                        res.ResultData = this.IVS270_GetProjectInformation_CreateResult(false, null);
                    }
                    else
                    {
                        res.ResultData = this.IVS270_GetProjectInformation_CreateResult(true, lstProjInfo[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }

        /// <summary>
        /// Search inventory instrument list for register stock-out.
        /// </summary>
        /// <param name="param">DO of searching parameter.</param>
        /// <returns>Return ActionResult of JSON data contains instrument list.</returns>
        public ActionResult IVS270_SearchInventoryInstrumentList(doSearchInstrumentListCondition param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (CommonUtil.IsNullAllField(param, "OfficeCode", "LocationCode", "AreaCodeList"))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else
                {
                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    if (string.IsNullOrEmpty(param.AreaCode))
                    {
                        param.AreaCodeList = new List<string>() { 
                            InstrumentArea.C_INV_AREA_NEW_SAMPLE, 
                            InstrumentArea.C_INV_AREA_NEW_SALE, 
                            InstrumentArea.C_INV_AREA_NEW_RENTAL, 
                            InstrumentArea.C_INV_AREA_SE_RENTAL 
                        };
                    }
                    var lst = service.SearchInventoryInstrumentList(param);
                    res.ResultData = CommonUtil.ConvertToXml<dtSearchInstrumentListResult>(lst, @"Inventory\IVS270_InstrumentResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                }
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        
        /// <summary>
        /// Calculate amount for register stock-out.
        /// </summary>
        /// <param name="param">List of instrument data for calculate amount.</param>
        /// <returns>Return ActionResult of JSON data contains calculated amount of parameter's instrument.</returns>
        public ActionResult IVS270_CalculateAmount(List<IVS270_CalculateAmountParam> param)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            // Default Currency "Rp."
            for (int i = 0; i < param.Count; i++)
            {
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                param[i].Currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                if (param[i].StockOutAmountCurrencyType == null)
                {
                    param[i].StockOutAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                }
            }

            try
            {
                if (param == null || param.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4068);
                    res.ResultData = this.IVS270_CalculateAmount_CreateResult(false, null);
                    return Json(res);
                }

                this.IVS270_CalculateAmountProcess(param, res);

                if (res.MessageList != null && res.MessageList.Count > 0)
                {
                    res.ResultData = IVS270_CalculateAmount_CreateResult(false, param);
                    return Json(res);
                }

                this.IVS270_UpdateAmount(param);

                res.ResultData = this.IVS270_CalculateAmount_CreateResult(true, param);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                res.ResultData = this.IVS270_CalculateAmount_CreateResult(false, null);
            }

            return Json(res);
        }

        /// <summary>
        /// Registering project stock-out.
        /// </summary>
        /// <param name="param">DO of registering parameter.</param>
        /// <returns>Return ActionResult of registration result.</returns>
        public ActionResult IVS270_RegisterProjectStockOut(IVS270_RegisterProjectStockOutParam param)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            // Default Currency "Rp."
            for (int i = 0; i < param.Details.Count; i++)
            {
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                param.Details[i].Currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                if (param.Details[i].StockOutAmountCurrencyType == null)
                {
                    param.Details[i].StockOutAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                }
            }

            try
            {
                ICommonHandler srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (srvCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.ResultData = this.IVS270_RegisterProjectStockOut_CreateResult(false, null);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_PROJECT_STOCKOUT, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    res.ResultData = this.IVS270_ConfirmProjectStockOut_CreateResult(false, null);
                    return Json(res);
                }

                if (string.IsNullOrEmpty(param.ProjectCode) || !param.IsRetrievePressed.Value)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4066);
                    res.ResultData = this.IVS270_RegisterProjectStockOut_CreateResult(false, null);
                    return Json(res);
                }

                if (param.Details == null || param.Details.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4068);
                    res.ResultData = this.IVS270_RegisterProjectStockOut_CreateResult(false, null);
                    return Json(res);
                }
                else
                {
                    param.Details = param.Details.OrderBy(d => d.InputOrder).ToList();
                }


                if (param.Memo != null && param.Memo.Replace(" ", "").Contains("\n\n\n\n"))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4022
                        , null
                        , new string[] { "txtMemo" });
                    res.ResultData = this.IVS270_RegisterProjectStockOut_CreateResult(false, null);
                    return Json(res);
                }

                this.IVS270_CalculateAmountProcess(param.Details, res);

                if (res.MessageList != null && res.MessageList.Count > 0)
                {
                    res.ResultData = this.IVS270_RegisterProjectStockOut_CreateResult(false, param.Details);
                    return Json(res);
                }

                this.IVS270_UpdateAmount(param.Details);

                res.ResultData = this.IVS270_RegisterProjectStockOut_CreateResult(true, param.Details);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                res.ResultData = this.IVS270_RegisterProjectStockOut_CreateResult(false, null);
                return Json(res);
            }
        }

        /// <summary>
        /// Confirming project stock-out.
        /// </summary>
        /// <param name="param">DO of confirming parameter.</param>
        /// <returns>Return ActionResult of confirmation result.</returns>
        public ActionResult IVS270_ConfirmProjectStockOut(IVS270_RegisterProjectStockOutParam param)
        {

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION; //All message during confirmation process must be information dialog box.

            try
            {
                ICommonHandler srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (srvCommon.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.ResultData = this.IVS270_ConfirmProjectStockOut_CreateResult(false, null);
                    return Json(res);
                }

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_PROJECT_STOCKOUT, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    res.ResultData = this.IVS270_ConfirmProjectStockOut_CreateResult(false, null);
                    return Json(res);
                }

                if (param.Details == null || param.Details.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4068);
                    res.ResultData = this.IVS270_RegisterProjectStockOut_CreateResult(false, null);
                    return Json(res);
                }
                else
                {
                    param.Details = param.Details.OrderBy(d => d.InputOrder).ToList();
                }
                
                var srvInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                this.IVS270_CalculateAmountProcess(param.Details, res);

                if (res.MessageList != null && res.MessageList.Count > 0)
                {
                    res.ResultData = this.IVS270_ConfirmProjectStockOut_CreateResult(false, null);
                    return Json(res);
                }

                var lstHQ = srvInventory.GetInventoryHeadOffice();
                int intDtlRunningNo = 0;

                //11.3	Set value in dsRegisterTransferInstrumentData for register transfer instrument
                doRegisterTransferInstrumentData objRegister = new doRegisterTransferInstrumentData()
                {
                    SlipId = SlipID.C_INV_SLIPID_STOCK_OUT,
                    InventorySlip = new tbt_InventorySlip()
                    {
                        SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE,
                        TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PROJECT,
                        ProjectCode = param.ProjectCode,
                        SlipIssueDate = DateTime.Now,
                        StockOutDate = DateTime.Now,
                        SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK,
                        DestinationLocationCode = InstrumentLocation.C_INV_LOC_PROJECT_WIP,
                        SourceOfficeCode = lstHQ[0].OfficeCode,
                        DestinationOfficeCode = lstHQ[0].OfficeCode,
                        Memo = param.Memo,
                        CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                        UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                    },
                    lstTbt_InventorySlipDetail = (
                        from p in param.Details
                        select new tbt_InventorySlipDetail()
                        {
                            RunningNo = ++intDtlRunningNo,
                            InstrumentCode = p.InstrumentCode,
                            SourceAreaCode = p.AreaCode,
                            DestinationAreaCode = p.AreaCode,
                            SourceShelfNo = p.ShelfNo,
                            DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION,
                            TransferQty = p.StockOutQty
                        }
                    ).ToList<tbt_InventorySlipDetail>()
                };

                string strInvSlipNo = null;

                using (var scope = new TransactionScope())
                {
                    //11.4	Insert data to tbt_InventoryProjectWIP
                    foreach (var objDtl in objRegister.lstTbt_InventorySlipDetail){
                        var lstProjWIP = srvInventory.GetTbt_InventoryProjectWIP(param.ProjectCode, objDtl.SourceAreaCode, objDtl.InstrumentCode);
                        
                        if (lstProjWIP == null || lstProjWIP.Count <= 0)
                        {
                            var lstResult = srvInventory.InsertTbt_InventoryProjectWIP(new List<tbt_InventoryProjectWIP>() {
                                new tbt_InventoryProjectWIP() {
                                    ProjectCode = param.ProjectCode,
                                    AreaCode = objDtl.SourceAreaCode,
                                    InstrumentCode = objDtl.InstrumentCode,
                                    InstrumentQty = objDtl.TransferQty
                                }
                            });

                            if (lstResult == null || lstResult.Count <= 0)
                            {
                                scope.Dispose();
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                res.AddErrorMessage(new Exception("Failed to call InsertTbt_InventoryProjectWIP()"));
                                res.ResultData = this.IVS270_ConfirmProjectStockOut_CreateResult(false, null);
                                return Json(res);
                            }
                        } 
                        else 
                        {
                            var lstResult = srvInventory.UpdateTbt_InventoryProjectWIP(new List<tbt_InventoryProjectWIP>() {
                                new tbt_InventoryProjectWIP() {
                                    ProjectCode = param.ProjectCode,
                                    AreaCode = objDtl.SourceAreaCode,
                                    InstrumentCode = objDtl.InstrumentCode,
                                    InstrumentQty = objDtl.TransferQty
                                }
                            });

                            if (lstResult == null || lstResult.Count <= 0)
                            {
                                scope.Dispose();
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                res.AddErrorMessage(new Exception("Failed to call UpdateTbt_InventoryProjectWIP()"));
                                res.ResultData = this.IVS270_ConfirmProjectStockOut_CreateResult(false, null);
                                return Json(res);
                            }
                        }
                    }

                    //11.5	Update inventory transfer data
                    strInvSlipNo = srvInventory.RegisterTransferInstrument(objRegister);

                    //11.6	Check there are new instrument
                    int intCheckNewInstrument = srvInventory.CheckNewInstrument(strInvSlipNo);
                    if (intCheckNewInstrument == 1)
                    {
                        //11.6.1	Prepare data for update new instrument to account stock data
                        var lstNewGroup = srvInventory.GetGroupNewInstrument(strInvSlipNo);
                        foreach (var objNewGroup in lstNewGroup)
                        {
                            #region Monthly Price @ 2015
                            //var decAvgPrice = srvInventory.CalculateMovingAveragePrice(objNewGroup);
                            var decAvgPrice = srvInventory.GetMonthlyAveragePrice(objNewGroup.Instrumentcode, objRegister.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                            #endregion
                            var bUpdated = srvInventory.UpdateAccountTransferNewInstrument(objNewGroup, decAvgPrice);
                            if (!bUpdated)
                            {
                                scope.Dispose();
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                res.AddErrorMessage(new Exception("Failed to call UpdateAccountTransferNewInstrument()"));
                                res.ResultData = this.IVS270_ConfirmProjectStockOut_CreateResult(false, null);
                                return Json(res);
                            }

                            var objAccStkMov = new tbt_AccountStockMoving()
                            {
                                //MovingNo = 0,
                                SlipNo  = strInvSlipNo,
                                TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PROJECT,
                                SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                                DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK,
                                DestinationLocationCode = InstrumentLocation.C_INV_LOC_PROJECT_WIP,
                                InstrumentCode = objNewGroup.Instrumentcode,
                                InstrumentQty = objNewGroup.TransferQty,
                                InstrumentPrice = decAvgPrice,
                                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                            };
                            //srvInventory.InsertTbt_AccountStockMoving(new List<tbt_AccountStockMoving>() { objAccStkMov });       //Edited by Phasupong 14/12/2015

                        }
                    }

                    //11.7	Check there are secondhand instrument
                    int intCheckSecondHand = srvInventory.CheckSecondhandInstrument(strInvSlipNo);
                    if (intCheckSecondHand == 1)
                    {
                        var qTotalAmountByInstrument = (
                            from d in param.Details
                            where (d.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || d.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO)
                            group d by new { InstrumentCode = d.InstrumentCode } into gInstrument
                            select new {
                                InstrumentCode = gInstrument.Key.InstrumentCode,
                                TotalAmount = gInstrument.Sum(dg => dg.StockOutAmount)
                            }
                        );

                        //11.7.1	Prepare data for update secondhand instrument to account stock data
                        var lstNewGroup = srvInventory.GetGroupSecondhandInstrument(strInvSlipNo);
                        int tmpCount = 0;
                        foreach (var objNewGroup in lstNewGroup)
                        {
                            //11.7.1.1	Update secondhand instrument to Account DB
                            var bUpdated = srvInventory.UpdateAccountTransferSecondhandInstrument(objNewGroup);
                            if (!bUpdated)
                            {
                                scope.Dispose();
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                res.AddErrorMessage(new Exception("Failed to call UpdateAccountTransferSecondhandInstrument()"));
                                res.ResultData = this.IVS270_ConfirmProjectStockOut_CreateResult(false, null);
                                return Json(res);
                            }

                            var decTotalAmount = qTotalAmountByInstrument
                                .Where(d => d.InstrumentCode == objNewGroup.Instrumentcode)
                                .Select(d => d.TotalAmount)
                                .FirstOrDefault();

                            if (param.Details[tmpCount].StockOutAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                            {
                                var objAccStkMov = new tbt_AccountStockMoving()
                                {
                                    //MovingNo = 0,
                                    SlipNo = strInvSlipNo,
                                    TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PROJECT,
                                    SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTALLED,
                                    DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                    SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK,
                                    DestinationLocationCode = InstrumentLocation.C_INV_LOC_PROJECT_WIP,
                                    InstrumentCode = objNewGroup.Instrumentcode,
                                    InstrumentQty = objNewGroup.TransferQty,
                                    InstrumentPrice = decTotalAmount / objNewGroup.TransferQty,
                                    InstrumentPriceUsd = null,
                                    InstrumentPriceCurrencyType = param.Details[tmpCount].StockOutAmountCurrencyType,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                                };
                                srvInventory.InsertTbt_AccountStockMoving(new List<tbt_AccountStockMoving>() { objAccStkMov });
                            }
                            else
                            {
                                var objAccStkMov = new tbt_AccountStockMoving()
                                {
                                    //MovingNo = 0,
                                    SlipNo = strInvSlipNo,
                                    TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PROJECT,
                                    SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTALLED,
                                    DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                    SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK,
                                    DestinationLocationCode = InstrumentLocation.C_INV_LOC_PROJECT_WIP,
                                    InstrumentCode = objNewGroup.Instrumentcode,
                                    InstrumentQty = objNewGroup.TransferQty,
                                    InstrumentPrice = null,
                                    InstrumentPriceUsd = decTotalAmount / objNewGroup.TransferQty,
                                    InstrumentPriceCurrencyType = param.Details[tmpCount].StockOutAmountCurrencyType,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                                };
                                srvInventory.InsertTbt_AccountStockMoving(new List<tbt_AccountStockMoving>() { objAccStkMov });
                            }

                            tmpCount++;
                        }
                    }

                    //11.8	Check there are sample instrument
                    int intCheckSample = srvInventory.CheckSampleInstrument(strInvSlipNo);
                    if (intCheckSample == 1)
                    {
                        //11.8.1	Prepare data for update sample instrument to account stock data
                        var lstNewGroup = srvInventory.GetGroupSampleInstrument(strInvSlipNo);
                        foreach (var objNewGroup in lstNewGroup)
                        {
                            //11.8.1.1	Update sample instrument to Account DB
                            var bUpdated = srvInventory.UpdateAccountTransferSampleInstrument(objNewGroup, InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT);
                            if (!bUpdated)
                            {
                                scope.Dispose();
                                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                                res.AddErrorMessage(new Exception("Failed to call UpdateAccountTransferSampleInstrument()"));
                                res.ResultData = this.IVS270_ConfirmProjectStockOut_CreateResult(false, null);
                                return Json(res);
                            }

                            var objAccStkMov = new tbt_AccountStockMoving()
                            {
                                //MovingNo = 0,
                                SlipNo = strInvSlipNo,
                                TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKOUT_PROJECT,
                                SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                                DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS,
                                SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK,
                                DestinationLocationCode = InstrumentLocation.C_INV_LOC_PROJECT_WIP,
                                InstrumentCode = objNewGroup.Instrumentcode,
                                InstrumentQty = objNewGroup.TransferQty,
                                InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT,
                                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                            };
                            //srvInventory.InsertTbt_AccountStockMoving(new List<tbt_AccountStockMoving>() { objAccStkMov });       //Edited by Phasupong 14/12/2015

                        }
                    }

                    #region //R2
                    try
                    {
                        IProjectHandler projSrv = ServiceContainer.GetService<IProjectHandler>() as IProjectHandler;
                        List<doInstrument> lstDoInst = (
                            from d in param.Details
                            group d by new { InstrumentCode = d.InstrumentCode } into g
                            select new doInstrument
                            {
                                InstrumentCode = g.Key.InstrumentCode,
                                InstrumentQty = g.Sum(d => d.StockOutQty.GetValueOrDefault(0))
                            }
                        ).ToList();
                        projSrv.UpdateStockOutInstrument(param.ProjectCode, lstDoInst, param.Memo);
                    }
                    catch (Exception e)
                    {
                        scope.Dispose();
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(new Exception("Failed to call UpdateStockOutInstrument()", e));
                        res.ResultData = this.IVS270_ConfirmProjectStockOut_CreateResult(false, null);
                        return Json(res);
                    }
                    #endregion

                    //Genereate inventory slip report  // ReportID.C_INV_REPORT_ID_STOCKOUT = IVR180
                    IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                    string strSlipNoReportPath = handlerInventoryDocument.GenerateIVR180FilePath(strInvSlipNo, ViewBag.InvHeadOfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                    scope.Complete();
                }

                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION_OK;
                res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0046);
                res.ResultData = this.IVS270_ConfirmProjectStockOut_CreateResult(true, strInvSlipNo);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                res.ResultData = this.IVS270_ConfirmProjectStockOut_CreateResult(false, null);
                return Json(res);
            }
        }

        /// <summary>
        /// Get document's data for downloading.
        /// </summary>
        /// <param name="strInvSlipNo">Inventory Slip No.</param>
        /// <returns>Return ActionResult of document's data.</returns>
        public ActionResult IVS270_DownloadDocument(string strInvSlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                var lstDocs = srvCommon.GetTbt_DocumentList(strInvSlipNo, ReportID.C_INV_REPORT_ID_STOCKOUT, ConfigName.C_CONFIG_DOC_OCC_DEFAULT);

                if (lstDocs == null || lstDocs.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                    res.ResultData = null;
                }
                else
                {
                    string path = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, lstDocs[0].FilePath);// ReportUtil.GetGeneratedReportPath(lstDocs[0].FilePath);

                    if (System.IO.File.Exists(path) == true)
                    {
                        res.ResultData = lstDocs[0];
                    }
                    else
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                        res.ResultData = null;
                    }
                }
                return Json(res);

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Download document.
        /// </summary>
        /// <param name="strDocumentNo">Document No.</param>
        /// <param name="documentOCC">Document OCC.</param>
        /// <param name="strDocumentCode">Document Code.</param>
        /// <param name="fileName">File Name.</param>
        /// <returns>Return ActionResult of file's stream.</returns>
        public ActionResult IVS270_DownloadPdfAndWriteLog(string strDocumentNo, string documentOCC, string strDocumentCode, string fileName)
        {
            try
            {
                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = strDocumentNo,
                    DocumentCode = strDocumentCode,
                    DocumentOCC = documentOCC,
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

        #endregion

        #region Method
        /// <summary>
        /// Calculate amount of stock-out data.
        /// </summary>
        /// <param name="param">List of stock-out instrument data.</param>
        /// <param name="res">ObjectResultData of current session.</param>
        private void IVS270_CalculateAmountProcess(List<IVS270_CalculateAmountParam> param, ObjectResultData res)
        {

            var srvInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

            // Check Current Stock
            foreach (var p in param)
            {
                if (!p.StockOutQty.HasValue || p.StockOutQty.Value <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4030
                        , new string[] { p.InstrumentCode }
                        , new string[] { p.StockOutQtyCtrlID });
                    continue;
                }

                var objChkResult = srvInventory.CheckTransferQty(new doCheckTransferQty()
                {
                    AreaCode = p.AreaCode,
                    InstrumentCode = p.InstrumentCode,
                    LocationCode = p.LocationCode,
                    OfficeCode = p.OfficeCode,
                    ShelfNo = p.ShelfNo,
                    TransferQty = p.StockOutQty.Value
                });

                p.CurrentStockQty = objChkResult.CurrentyQty;
                if (!objChkResult.OverQtyFlag.HasValue)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009
                        , new string[] { p.InstrumentCode }
                        , new string[] { p.StockOutQtyCtrlID });
                    continue;
                }
                else if (objChkResult.OverQtyFlag.Value)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008
                        , new string[] { p.InstrumentCode }
                        , new string[] { p.StockOutQtyCtrlID });
                    continue;
                }
            }

        }

        /// <summary>
        /// Update amount of stock-out data.
        /// </summary>
        /// <param name="param">List of instrument data used to update amount.</param>
        private void IVS270_UpdateAmount(List<IVS270_CalculateAmountParam> param)
        {
            // Calculate Amount
            var srvInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            Dictionary<string, int> dictPrevStockOut = new Dictionary<string, int>();

            int tmpCount = 0;
            foreach (var p in param)
            {
                if (p.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || p.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO)
                {
                    int intPrevTransferQty = 0;
                    if (dictPrevStockOut.ContainsKey(p.InstrumentCode))
                    {
                        intPrevTransferQty = dictPrevStockOut[p.InstrumentCode];
                        dictPrevStockOut[p.InstrumentCode] += p.StockOutQty.Value;
                    }
                    else
                    {
                        dictPrevStockOut.Add(p.InstrumentCode, p.StockOutQty.Value);
                    }

                    var decAmount = srvInventory.GetFIFOInstrumentPrice(p.StockOutQty, p.OfficeCode, p.LocationCode, p.InstrumentCode, intPrevTransferQty).decTransferAmount;

                    string tmpCurrency = srvInventory.GetFIFOInstrumentPrice(p.StockOutQty, p.OfficeCode, p.LocationCode, p.InstrumentCode, intPrevTransferQty).decTransferAmountCurrencyType;
                    if(tmpCurrency != null)
                    {
                        param[tmpCount].StockOutAmountCurrencyType = tmpCurrency;
                    }

                    p.StockOutAmount = Convert.ToDecimal(decAmount);
                }
                else if (p.AreaCode != InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                {
                    var objPrice = srvInventory.GetMovingAveragePriceCondition(p.OfficeCode, null, null, p.InstrumentCode
                        , new string[] { InstrumentLocation.C_INV_LOC_INSTOCK }, null);
                    if (objPrice == null || objPrice.MovingAveragePrice == null)
                    {
                        throw new ApplicationException("Unable to get price");
                    }
                    p.StockOutAmount = objPrice.MovingAveragePrice * p.StockOutQty.Value;

                    string tmpCurrency = srvInventory.GetMovingAveragePriceCondition(p.OfficeCode, null, null, p.InstrumentCode
                        , new string[] { InstrumentLocation.C_INV_LOC_INSTOCK }, null).MovingAveragePriceCurrencyType;
                    if (tmpCurrency != null)
                    {
                        param[tmpCount].StockOutAmountCurrencyType = tmpCurrency;
                    }

                }
                else
                {
                    p.StockOutAmount = 0;

                }
                tmpCount++;
            }
        }

        /// <summary>
        /// Create result data for IVS270_GetProjectInformation controller.
        /// </summary>
        /// <param name="bIsSuccess">Boolean, indicate the controller's successful.</param>
        /// <param name="objProjInfo">Object contains project information.</param>
        /// <returns>Return object of result data.</returns>
        private object IVS270_GetProjectInformation_CreateResult(bool bIsSuccess, doProjectInformation objProjInfo)
        {
            return new
            {
                IsSuccess = bIsSuccess,
                ProjectInformation = objProjInfo
            };
        }

        /// <summary>
        /// Create result data for IVS270_CalculateAmount controller.
        /// </summary>
        /// <param name="bIsSuccess">Boolean, indicate the controller's successful.</param>
        /// <param name="lstResultData">Object contains calculated amount data.</param>
        /// <returns>Return object of result data.</returns>
        private object IVS270_CalculateAmount_CreateResult(bool bIsSuccess, List<IVS270_CalculateAmountParam> lstResultData)
        {
            int intTotalQty = 0;
            //decimal decTotalAmount = 0;
            decimal TotalAmount = 0;
            if (lstResultData != null)
            {
                foreach (var p in lstResultData)
                {
                    intTotalQty = (p.StockOutQty.HasValue ? p.StockOutQty.Value : 0);
                    //intTotalQty += (p.StockOutQty.HasValue ? p.StockOutQty.Value : 0);
                    //decTotalAmount += (p.StockOutAmount.HasValue ? p.StockOutAmount.Value : 0);

                    TotalAmount += intTotalQty * p.UnitPrice;
                }
            }

            return new
            {
                IsSuccess = bIsSuccess,
                ResultData = lstResultData,
                TotalQty = intTotalQty,
                TotalQtyText = intTotalQty.ToString("#,##0"),
                TotalAmount = TotalAmount
            };
        }

        /// <summary>
        /// Create result data for IVS270_RegisterProjectStockOut controller.
        /// </summary>
        /// <param name="bIsSuccess">Boolean, indicate the controller's successful.</param>
        /// <param name="lstResultData">List of registering instrument data.</param>
        /// <returns>Return object of result data.</returns>
        private object IVS270_RegisterProjectStockOut_CreateResult(bool bIsSuccess, List<IVS270_CalculateAmountParam> lstResultData)
        {
            int intTotalQty = 0;
            //decimal decTotalAmount = 0;
            decimal TotalAmount = 0;
            if (lstResultData != null)
            {
                foreach (var p in lstResultData)
                {
                    intTotalQty = (p.StockOutQty.HasValue ? p.StockOutQty.Value : 0);
                    //intTotalQty += (p.StockOutQty.HasValue ? p.StockOutQty.Value : 0);
                    //decTotalAmount += (p.StockOutAmount.HasValue ? p.StockOutAmount.Value : 0);

                    TotalAmount += intTotalQty * p.UnitPrice;
                }
            }

            return new
            {
                IsSuccess = bIsSuccess,
                ResultData = lstResultData,
                TotalQty = intTotalQty,
                TotalQtyText = intTotalQty.ToString("#,##0"),
                TotalAmount = TotalAmount
            };
        }

        /// <summary>
        /// Create result data for IVS270_RegisterProjectStockOut controller.
        /// </summary>
        /// <param name="bIsSuccess">Boolean, indicate the controller's successful.</param>
        /// <param name="strInvSlipNo">Inventory Slip No.</param>
        /// <returns>Return object of result data.</returns>
        private object IVS270_ConfirmProjectStockOut_CreateResult(bool bIsSuccess, string strInvSlipNo)
        {
            return new
            {
                IsSuccess = bIsSuccess, 
                InvSlipNo = strInvSlipNo
            };
        }

        #endregion
    }
}
