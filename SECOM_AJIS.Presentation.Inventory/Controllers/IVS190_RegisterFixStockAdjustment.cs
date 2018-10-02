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
        /// - Check user permission for screen IVS190.<br />
        /// - Check freezed data.<br />
        /// - Check started stock checking.<br />
        /// - Get inventory head office.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS190_Authority(IVS190_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                // Is suspend ?
                ICommonHandler handler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handler.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check permission
                if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_FIX_ADJUSTMENT, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                // Check freezed data
                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                if (handlerInventory.CheckFreezedData() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4002);
                    return Json(res);
                }

                // Check for the stock is started
                if (handlerInventory.CheckStartedStockChecking() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    return Json(res);
                }

                //Get Inventory head office
                List<doOffice> headerOffice = handlerInventory.GetInventoryHeadOffice();
                param.HeaderOffice = headerOffice;

                if (headerOffice.Count == 0)
                {
                    if (CommonUtil.IsNullOrEmpty(headerOffice[0].OfficeCode))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4016);
                        return Json(res);
                    }
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

            return InitialScreenEnvironment<IVS190_ScreenParameter>("IVS190", param, res);
        }

        /// <summary>
        /// Get config for Instrument table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS190_InitialSearchResultGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS190_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get config for Fix Adjustment Instrument table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS190_InitialAdjustmentDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS190_AdjustmentDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS190")]
        public ActionResult IVS190()
        {
            IVS190_ScreenParameter param = GetScreenObject<IVS190_ScreenParameter>();

            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            param.Miscellaneous = CommonUtil.ConvertObjectbyLanguage<doMiscTypeCode, doMiscTypeCode>(param.Miscellaneous, "ValueDisplay");
            ViewBag.SourceLocation = handlerCommon.GetMiscDisplayValue(param.Miscellaneous, MiscType.C_INV_LOC, InstrumentLocation.C_INV_LOC_BUFFER);
            ViewBag.DestinationLocation = handlerCommon.GetMiscDisplayValue(param.Miscellaneous, MiscType.C_INV_LOC, InstrumentLocation.C_INV_LOC_LOSS);

            var isExist = (from p in CommonUtil.dsTransData.dtUserBelongingData where p.DepartmentCode == DepartmentMaster.C_DEPT_PURCHASE && p.PositionCode == PositionCode.C_POSITION_DIRECTOR select p).Any(); //0002 =  PositionCode.C_POSITION_DIRECTOR

            ViewBag.DisableMinus = !isExist;

            param.DisableMinus = !isExist;

            return View();
        }

        /// <summary>
        /// Search inventory instrument.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult IVS190_SearchResponse(doSearchInstrumentListConditionIVS190 cond, string type)
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

                IInventoryHandler handler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                IVS190_ScreenParameter param = GetScreenObject<IVS190_ScreenParameter>();
                if (param.HeaderOffice.Count > 0)
                {
                    cond.OfficeCode = param.HeaderOffice[0].OfficeCode;
                    cond.LocationCode = InstrumentLocation.C_INV_LOC_BUFFER;
                    cond.ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                    if (param.DisableMinus == true || type == "PLUS")
                        cond.TransferType = true;
                    else
                        cond.TransferType = false;
                }

                list = handler.SearchInventoryInstrumentListIVS190(cond);

                //list = handler.SearchInventoryInstrumentList(cond);

                //if (param.DisableMinus == true || type == "PLUS")
                //{
                //    list = (from p in list where p.InstrumentQty >= 0 select p).ToList<dtSearchInstrumentListResult>();
                //}
                //else if (type == "MINUS")
                //{
                //    list = (from p in list where p.InstrumentQty < 0 select p).ToList<dtSearchInstrumentListResult>();
                //}

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtSearchInstrumentListResult>(list, "Inventory\\IVS190_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);


        }

        /// <summary>
        /// Validate before register.<br />
        /// - Check system suspending.<br />
        /// - Check user permission.<br />
        /// - Check approve no.<br />
        /// - Check list not empty.<br />
        /// - Check memo.<br />
        /// - Check quantity.<br />
        /// - Count accumulate sum quantity.<br />
        /// - Calculate total amount.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult IVS190_Register(IVS190_RegisterData data)
        {

            IVS190_ScreenParameter param = GetScreenObject<IVS190_ScreenParameter>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<object> resultList = new List<object>();
            List<string> HilightRows = new List<string>();

            // Default Currency "Rp."
            for (int i = 0; i < data.Detail.Count; i++)
            {
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                data.Detail[i].Currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                if (data.Detail[i].AdjustAmountCurrencyType == null)
                {
                    data.Detail[i].AdjustAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                }
            }

            string strResult = "0";
            decimal? totalAmount = 0;

            try
            {
                // Is suspend ?
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Check permission
                if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_TRANSFER_BUFFER, FunctionID.C_FUNC_ID_OPERATE) == false)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //Validate
                List<string> label = new List<string>();
                List<string> controlsName = new List<string>();
                if (CommonUtil.IsNullOrEmpty(data.Header.ApproveNo))
                {
                    string lblApproveNo = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INVENTORY, "IVS190", "lblApproveNo");

                    label.Add(lblApproveNo);
                    controlsName.Add("ApproveNo");

                }

                if (label.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, label.ToArray(), controlsName.ToArray());
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
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4022);
                    return Json(res);
                }

                int countBusinessWaringing = 0;

                //Validate #3
                List<string> lstWarningAtControls = new List<string>();
                foreach (var item in data.Detail)
                {
                    if (item.FixedStockQty <= 0 || item.FixedStockQty == null)
                    {
                        lstWarningAtControls.Add(item.txtStockAdjQtyID);
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4071, null, lstWarningAtControls.ToArray());
                    }
                }
                if (lstWarningAtControls.Count > 0)
                {
                    countBusinessWaringing++;
                }

                //Business validate                

                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                foreach (var item in data.Detail)
                {
                    doCheckTransferQty doCheck = new doCheckTransferQty()
                    {
                        OfficeCode = param.HeaderOffice[0].OfficeCode,
                        LocationCode = InstrumentLocation.C_INV_LOC_BUFFER,
                        AreaCode = item.AreaCode,
                        ShelfNo = item.ShelfNo,
                        InstrumentCode = item.Instrumentcode,
                        TransferQty = item.FixedStockQty.HasValue ? item.FixedStockQty.Value : 0
                    };

                    doCheckTransferQtyResult result = handlerInventory.CheckTransferQtyIVS190(doCheck);
                    if (result.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtStockAdjQtyID });
                        countBusinessWaringing++;
                    }
                    else if (result.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtStockAdjQtyID });
                        countBusinessWaringing++;

                    }

                    item.InstrumentQty = result.CurrentyQty;
                }
                if (countBusinessWaringing > 0)
                {
                    resultList.Add(data.Detail.ToArray());
                    res.ResultData = resultList.ToArray();
                    return Json(res);
                }

                // Calculate stock adjust amount
                IVS190_CountAccumulateSumQty(data.Detail); // count accumualteSumQty (ที่ไม่รวมตัวเองเข้าไปด้วย) of second hand group
                int tmpCount = 0; //tmp Count data.DetailList
                foreach (var d in data.Detail)
                {
                    // get FIFO
                    if (d.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || d.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO)
                    {
                        decimal? fifoPrice = handlerInventory.GetFIFOInstrumentPrice(d.FixedStockQty,
                                                           param.HeaderOffice[0].OfficeCode,
                                                           InstrumentLocation.C_INV_LOC_BUFFER,
                                                           d.Instrumentcode,
                                                           d.AccumulateSumQty).decTransferAmount;

                        string tmpCurrency = handlerInventory.GetFIFOInstrumentPrice(d.FixedStockQty,
                                                           param.HeaderOffice[0].OfficeCode,
                                                           InstrumentLocation.C_INV_LOC_BUFFER,
                                                           d.Instrumentcode,
                                                           d.AccumulateSumQty).decTransferAmountCurrencyType;

                        if(tmpCurrency != null)
                        {
                            data.Detail[tmpCount].AdjustAmountCurrencyType = tmpCurrency;
                        }

                        d.AdjustAmount = Convert.ToDecimal(fifoPrice);
                    }
                    else if (d.AreaCode == InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                    {
                        d.AdjustAmount = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                    }
                    else
                    {
                        doCalPriceCondition doCalulatePrice = handlerInventory.GetMovingAveragePriceCondition(param.HeaderOffice[0].OfficeCode,
                                                                                           null,
                                                                                           null,
                                                                                           d.Instrumentcode,
                                                                                           new string[] { InstrumentLocation.C_INV_LOC_BUFFER },
                                                                                           null
                                                                                         );
                        if (doCalulatePrice != null)
                        {
                            d.AdjustAmount = Convert.ToDecimal(doCalulatePrice.MovingAveragePrice) * d.FixedStockQty;
                            if(doCalulatePrice.MovingAveragePriceCurrencyType != null)
                            {
                                data.Detail[tmpCount].AdjustAmountCurrencyType = doCalulatePrice.MovingAveragePriceCurrencyType;
                            }   
                        }
                    }

                    totalAmount += d.AdjustAmount;
                    data.Detail[tmpCount].AdjustAmount = d.AdjustAmount;
                    tmpCount++;
                }


                // Save RegisterData in session
                if (param != null)
                {
                    param.RegisterData = data;
                }

                strResult = "1"; // Success !!

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            resultList.Add(strResult);
            resultList.Add(HilightRows.ToArray());
            resultList.Add(data.Detail.ToArray());
            resultList.Add(totalAmount.Value.ToString("N2"));

            res.ResultData = resultList.ToArray();
            return Json(res);


        }

        /// <summary>
        /// Register fix stock adjustment.<br />
        /// - Check system suspending.<br />
        /// - Check quantity.<br />
        /// - Register transfer instrument.<br />
        /// - Update account transfer new/second hand/sample instrument.<br />
        /// - Update account stock moving.<br />
        /// - Generate report.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult IVS190_Confirm(doSearchInstrumentListCondition cond, string type)
        {
            string slipNo = string.Empty;
            string slipNoReportPath = string.Empty;
            IVS190_ScreenParameter param = GetScreenObject<IVS190_ScreenParameter>();
            IVS190_RegisterData data = new IVS190_RegisterData();
            List<object> resultList = new List<object>();
            if (param != null)
            {
                data = param.RegisterData;
            }


            ObjectResultData res = new ObjectResultData();
            //res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                // Is suspend ?
                if (handlerCommon.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                //Business validate
                foreach (var item in data.Detail)
                {
                    doCheckTransferQty doCheck = new doCheckTransferQty()
                    {
                        OfficeCode = param.HeaderOffice[0].OfficeCode,
                        LocationCode = InstrumentLocation.C_INV_LOC_BUFFER,
                        AreaCode = item.AreaCode,
                        ShelfNo = item.ShelfNo,
                        InstrumentCode = item.Instrumentcode,
                        TransferQty = item.FixedStockQty.HasValue ? item.FixedStockQty.Value : 0
                    };


                    doCheckTransferQtyResult result = handlerInventory.CheckTransferQtyIVS190(doCheck);
                    item.InstrumentQty = result.CurrentyQty;

                    if (result.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtStockAdjQtyID });
                        resultList.Add(data.Detail.ToArray());
                        res.ResultData = resultList.ToArray();
                        return Json(res);
                    }
                    else if (result.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtStockAdjQtyID });
                        resultList.Add(data.Detail.ToArray());
                        res.ResultData = resultList.ToArray();
                        return Json(res);
                    }
                }

                // Prepare data for Register
                doRegisterTransferInstrumentData dsRegisterData = new doRegisterTransferInstrumentData();

                dsRegisterData.SlipId = SlipID.C_INV_SLIPID_FIX_ADJUSTMET;
                dsRegisterData.InventorySlip = new tbt_InventorySlip()
                {
                    SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE,
                    TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_FIX_ADJUSTMENT,
                    SlipIssueDate = DateTime.Now,
                    StockInDate = DateTime.Now,
                    StockOutDate = DateTime.Now,
                    SourceLocationCode = InstrumentLocation.C_INV_LOC_BUFFER,
                    DestinationLocationCode = InstrumentLocation.C_INV_LOC_LOSS,
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

                if (param.DisableMinus == true || type == "PLUS")
                {
                    dsRegisterData.InventorySlip.TransferType = true;
                }
                else if (type == "MINUS")
                {
                    dsRegisterData.InventorySlip.TransferType = false;
                }

                dsRegisterData.lstTbt_InventorySlipDetail = new List<tbt_InventorySlipDetail>();
                tbt_InventorySlipDetail detail;
                for (int i = 0; i < data.Detail.Count; i++)
                {
                    if(data.Detail[i].AdjustAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    {
                        detail = new tbt_InventorySlipDetail()
                        {
                            RunningNo = (i + 1),
                            InstrumentCode = data.Detail[i].Instrumentcode,
                            SourceAreaCode = data.Detail[i].AreaCode,
                            DestinationAreaCode = data.Detail[i].AreaCode,
                            SourceShelfNo = data.Detail[i].ShelfNo,
                            DestinationShelfNo = data.Detail[i].ShelfNo,
                            //TransferQty = data.Detail[i].FixedStockQty
                            InstrumentAmount = data.Detail[i].AdjustAmount,
                            InstrumentAmountUsd = null,
                            InstrumentAmountCurrencyType = data.Detail[i].AdjustAmountCurrencyType
                        };
                    }
                    else
                    {
                        detail = new tbt_InventorySlipDetail()
                        {
                            RunningNo = (i + 1),
                            InstrumentCode = data.Detail[i].Instrumentcode,
                            SourceAreaCode = data.Detail[i].AreaCode,
                            DestinationAreaCode = data.Detail[i].AreaCode,
                            SourceShelfNo = data.Detail[i].ShelfNo,
                            DestinationShelfNo = data.Detail[i].ShelfNo,
                            //TransferQty = data.Detail[i].FixedStockQty
                            InstrumentAmount = null,
                            InstrumentAmountUsd = data.Detail[i].AdjustAmount,
                            InstrumentAmountCurrencyType = data.Detail[i].AdjustAmountCurrencyType
                        };
                    }
                    
                    if (param.DisableMinus == true || type == "PLUS")
                    {
                        //detail.InstrumentAmount = data.Detail[i].AdjustAmount;
                        detail.TransferQty = data.Detail[i].FixedStockQty;
                    }
                    else if (type == "MINUS")
                    {
                        //detail.InstrumentAmount = 0 - data.Detail[i].AdjustAmount;
                        detail.TransferQty = 0 - data.Detail[i].FixedStockQty;
                    }

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



                        //--------------- Update related table --------------------

                        //Count detail data
                        int tmpCount = 0;

                        // Check New instrument
                        int bCheckingNewInstrument = handlerInventory.CheckNewInstrument(slipNo_result);
                        if (bCheckingNewInstrument == 1)
                        {
                            List<doGroupNewInstrument> dtGroupNewInstrument = handlerInventory.GetGroupNewInstrument(slipNo_result);

                            foreach (var item in dtGroupNewInstrument)
                            {
                                if (type == "MINUS")
                                    item.TransferType = false;

                                #region Monthly Price @ 2015
                                //decimal decMovingAveragePrice = handlerInventory.CalculateMovingAveragePrice(item);
                                decimal decMovingAveragePrice = handlerInventory.GetMonthlyAveragePrice(item.Instrumentcode, dsRegisterData.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                                #endregion
                                handlerInventory.UpdateAccountTransferNewInstrument(item, decMovingAveragePrice);

                                // Insert new instrument to Stock Moving
                                var accountInstock = handlerInventory.GetTbt_AccountInStock(item.Instrumentcode, InstrumentLocation.C_INV_LOC_BUFFER, param.HeaderOffice[0].OfficeCode);

                                if (accountInstock.Count > 0)
                                {
                                    // Prepare
                                    List<tbt_AccountStockMoving> listAccountStockMoving = new List<tbt_AccountStockMoving>();
                                    tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving()
                                    {
                                        SlipNo = slipNo_result,
                                        TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_FIX_ADJUSTMENT,
                                        SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                                        DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_ADJUST,
                                        SourceLocationCode = InstrumentLocation.C_INV_LOC_BUFFER,
                                        DestinationLocationCode = InstrumentLocation.C_INV_LOC_LOSS,
                                        InstrumentCode = item.Instrumentcode,
                                        InstrumentQty = item.TransferQty,
                                        CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                        CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                        UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                        UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                                    };

                                    if (param.DisableMinus == true || type == "PLUS")
                                    {
                                        if (data.Detail[tmpCount].AdjustAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        {
                                            accountStockMoving.InstrumentPrice = null;
                                            accountStockMoving.InstrumentPriceUsd = accountInstock[0].MovingAveragePrice;
                                            accountStockMoving.InstrumentPriceCurrencyType = data.Detail[tmpCount].AdjustAmountCurrencyType;
                                        }
                                        else
                                        {
                                            accountStockMoving.InstrumentPrice = accountInstock[0].MovingAveragePrice;
                                            accountStockMoving.InstrumentPriceUsd = null;
                                            accountStockMoving.InstrumentPriceCurrencyType = data.Detail[tmpCount].AdjustAmountCurrencyType;
                                        }
                                        //accountStockMoving.InstrumentPrice = accountInstock[0].MovingAveragePrice;
                                    }
                                    else if (type == "MINUS")
                                    {
                                        //accountStockMoving.InstrumentPrice = 0 - accountInstock[0].MovingAveragePrice;
                                        //accountStockMoving.InstrumentPrice = accountInstock[0].MovingAveragePrice;
                                        if (data.Detail[0].AdjustAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        {
                                            accountStockMoving.InstrumentPrice = null;
                                            accountStockMoving.InstrumentPriceUsd = accountInstock[0].MovingAveragePrice;
                                            accountStockMoving.InstrumentPriceCurrencyType = data.Detail[tmpCount].AdjustAmountCurrencyType;
                                        }
                                        else
                                        {
                                            accountStockMoving.InstrumentPrice = accountInstock[0].MovingAveragePrice;
                                            accountStockMoving.InstrumentPriceUsd = null;
                                            accountStockMoving.InstrumentPriceCurrencyType = data.Detail[tmpCount].AdjustAmountCurrencyType;
                                        }
                                    }

                                    // add
                                    listAccountStockMoving.Add(accountStockMoving);

                                    handlerInventory.InsertTbt_AccountStockMoving(listAccountStockMoving);

                                }
                                tmpCount++;
                            }
                        }

                        // Check secondhand instrument 
                        int bCheckSeconhandInstrument = handlerInventory.CheckSecondhandInstrument(slipNo_result);
                        if (bCheckSeconhandInstrument == 1)
                        {
                            List<doGroupSecondhandInstrument> dtGroupSecondhandInstrument = handlerInventory.GetGroupSecondhandInstrument(slipNo_result);

                            foreach (var item in dtGroupSecondhandInstrument)
                            {
                                if (type == "MINUS")
                                    item.TransferType = false;

                                handlerInventory.UpdateAccountTransferSecondhandInstrumentIVS190(item);

                                decimal? sumTranferAmount = (from p in data.Detail
                                                             where (p.Instrumentcode == item.Instrumentcode)
                                                                 && (p.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || p.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL)
                                                             select p).Sum(m => m.AdjustAmount);

                                // Prepare
                                List<tbt_AccountStockMoving> listAccountStockMoving = new List<tbt_AccountStockMoving>();
                                tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving()
                                {
                                    SlipNo = slipNo_result,
                                    TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_FIX_ADJUSTMENT,
                                    SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTALLED,
                                    DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_ADJUST,
                                    SourceLocationCode = InstrumentLocation.C_INV_LOC_BUFFER,
                                    DestinationLocationCode = InstrumentLocation.C_INV_LOC_LOSS,
                                    InstrumentCode = item.Instrumentcode,
                                    InstrumentQty = item.TransferQty,
                                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                                };

                                if (param.DisableMinus == true || type == "PLUS")
                                {
                                    if (data.Detail[tmpCount].AdjustAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                    {
                                        accountStockMoving.InstrumentPrice = null;
                                        accountStockMoving.InstrumentPriceUsd = Convert.ToDecimal(sumTranferAmount / item.TransferQty);
                                        accountStockMoving.InstrumentPriceCurrencyType = data.Detail[tmpCount].AdjustAmountCurrencyType;
                                    }
                                    else
                                    {
                                        accountStockMoving.InstrumentPrice = Convert.ToDecimal(sumTranferAmount / item.TransferQty);
                                        accountStockMoving.InstrumentPriceUsd = null;
                                        accountStockMoving.InstrumentPriceCurrencyType = data.Detail[tmpCount].AdjustAmountCurrencyType;
                                    }
                                    //accountStockMoving.InstrumentPrice = Convert.ToDecimal(sumTranferAmount / item.TransferQty);
                                }
                                else if (type == "MINUS")
                                {
                                    if (data.Detail[tmpCount].AdjustAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                    {
                                        accountStockMoving.InstrumentPrice = null;
                                        accountStockMoving.InstrumentPriceUsd = 0 - Convert.ToDecimal(sumTranferAmount / item.TransferQty);
                                        accountStockMoving.InstrumentPriceCurrencyType = data.Detail[tmpCount].AdjustAmountCurrencyType;
                                    }
                                    else
                                    {
                                        accountStockMoving.InstrumentPrice = 0 - Convert.ToDecimal(sumTranferAmount / item.TransferQty);
                                        accountStockMoving.InstrumentPriceUsd = null;
                                        accountStockMoving.InstrumentPriceCurrencyType = data.Detail[tmpCount].AdjustAmountCurrencyType;
                                    }
                                    //accountStockMoving.InstrumentPrice = 0 - Convert.ToDecimal(sumTranferAmount / item.TransferQty);
                                }

                                // add
                                listAccountStockMoving.Add(accountStockMoving);

                                handlerInventory.InsertTbt_AccountStockMoving(listAccountStockMoving);
                                tmpCount++;
                            }
                        }


                        // Check sample instrument 
                        int bCheckSampleInstrument = handlerInventory.CheckSampleInstrument(slipNo_result);
                        if (bCheckSampleInstrument == 1)
                        {
                            List<doGroupSampleInstrument> dtGroupSampleInstrument = handlerInventory.GetGroupSampleInstrument(slipNo_result);

                            foreach (var item in dtGroupSampleInstrument)
                            {
                                if (type == "MINUS")
                                {
                                    item.TransferType = false;
                                    item.TransferQty = item.TransferQty * -1;
                                }

                                handlerInventory.UpdateAccountTransferSampleInstrument(item, null);

                                // Prepare
                                List<tbt_AccountStockMoving> listAccountStockMoving = new List<tbt_AccountStockMoving>();
                                if (data.Detail[tmpCount].AdjustAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                {
                                    tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving()
                                    {
                                        SlipNo = slipNo_result,
                                        TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_FIX_ADJUSTMENT,
                                        SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                                        DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_ADJUST,
                                        SourceLocationCode = InstrumentLocation.C_INV_LOC_BUFFER,
                                        DestinationLocationCode = InstrumentLocation.C_INV_LOC_LOSS,
                                        InstrumentCode = item.Instrumentcode,
                                        InstrumentQty = item.TransferQty,
                                        InstrumentPrice = null,
                                        InstrumentPriceUsd = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT,
                                        InstrumentPriceCurrencyType = data.Detail[tmpCount].AdjustAmountCurrencyType,
                                        CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                        CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                        UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                        UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                                    };
                                    // add
                                    listAccountStockMoving.Add(accountStockMoving);
                                    handlerInventory.InsertTbt_AccountStockMoving(listAccountStockMoving);
                                }
                                else
                                {
                                    tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving()
                                    {
                                        SlipNo = slipNo_result,
                                        TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_FIX_ADJUSTMENT,
                                        SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK,
                                        DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_ADJUST,
                                        SourceLocationCode = InstrumentLocation.C_INV_LOC_BUFFER,
                                        DestinationLocationCode = InstrumentLocation.C_INV_LOC_LOSS,
                                        InstrumentCode = item.Instrumentcode,
                                        InstrumentQty = item.TransferQty,
                                        InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT,
                                        InstrumentPriceUsd = null,
                                        InstrumentPriceCurrencyType = data.Detail[tmpCount].AdjustAmountCurrencyType,
                                        CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                        CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                                        UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                        UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                                    };
                                    // add
                                    listAccountStockMoving.Add(accountStockMoving);
                                    handlerInventory.InsertTbt_AccountStockMoving(listAccountStockMoving);
                                }

                                // add
                                //listAccountStockMoving.Add(accountStockMoving);
                                //handlerInventory.InsertTbt_AccountStockMoving(listAccountStockMoving);
                                tmpCount++;
                            }
                        }

                        // Generate inventory slip report   // C_INV_REPORT_ID_SPECIAL_STOCKOUT = IVR060

                        IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                        slipNoReportPath = handlerInventoryDocument.GenerateIVR130FilePath(slipNo_result, param.HeaderOffice[0].OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

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

                res.ResultData = slipNo; // Success ! return slip no.
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
        public ActionResult IVS190_CheckExistFile()
        {
            try
            {
                IVS190_ScreenParameter param = GetScreenObject<IVS190_ScreenParameter>();
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
        public ActionResult IVS190_DownloadPdfAndWriteLog()
        {
            try
            {

                IVS190_ScreenParameter param = GetScreenObject<IVS190_ScreenParameter>();
                string fileName = param.SlipNoReportPath;

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = param.SlipNo,
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
        /// Calculate amount.<br />
        /// - Check quantity.<br />
        /// - Count accumulate sum quantity.<br />
        /// - Calculate total amount.
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public ActionResult IVS190_CalculateAmount(List<IVS190_DetailData> detail)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            //Default Currency detailList
            for(int i=0; i<detail.Count; i++)
            {
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                detail[i].Currencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                    {
                                                        new doMiscTypeCode()
                                                        {
                                                            FieldName = MiscType.C_CURRENCT,
                                                            ValueCode = "%"
                                                        }
                                                    }).ToList();

                if (detail[i].AdjustAmountCurrencyType == null)
                {
                    detail[i].AdjustAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                }
            }

            try
            {
                IVS190_ScreenParameter param = GetScreenObject<IVS190_ScreenParameter>();
                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                List<string> lstWarningAtControls = new List<string>();
                foreach (var item in detail)
                {
                    if (item.FixedStockQty == null || item.FixedStockQty <= 0)
                    {
                        lstWarningAtControls.Add(item.txtStockAdjQtyID);
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4030, new string[] { item.Instrumentcode }, lstWarningAtControls.ToArray());
                    }
                }
                if (lstWarningAtControls.Count > 0)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4030, null, lstWarningAtControls.ToArray());
                    return Json(res);
                }

                //Business validate
                int countBusinessWaringing = 0;


                foreach (var item in detail)
                {
                    doCheckTransferQty doCheck = new doCheckTransferQty()
                    {
                        OfficeCode = param.HeaderOffice[0].OfficeCode,
                        LocationCode = InstrumentLocation.C_INV_LOC_BUFFER,
                        AreaCode = item.AreaCode,
                        ShelfNo = item.ShelfNo,
                        InstrumentCode = item.Instrumentcode,
                        TransferQty = item.FixedStockQty.HasValue ? item.FixedStockQty.Value : 0
                    };

                    doCheckTransferQtyResult result = handlerInventory.CheckTransferQtyIVS190(doCheck);
                    if (result.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtStockAdjQtyID });
                        countBusinessWaringing++;
                    }
                    else if (result.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtStockAdjQtyID });
                        countBusinessWaringing++;

                    }

                    item.InstrumentQty = result.CurrentyQty;
                }
                if (countBusinessWaringing > 0)
                {
                    return Json(res);
                }

                // Calculate stock adjust amount
                IVS190_CountAccumulateSumQty(detail); // count accumualteSumQty (ที่ไม่รวมตัวเองเข้าไปด้วย) of second hand group

                decimal? totalAmount = 0;
                //tmp count detailList
                int tmpCount = 0;
                foreach (var d in detail)
                {
                    // get FIFO
                    if (d.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || d.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO)
                    {
                        decimal? fifoPrice = handlerInventory.GetFIFOInstrumentPrice(d.FixedStockQty,
                                                           param.HeaderOffice[0].OfficeCode,
                                                           InstrumentLocation.C_INV_LOC_BUFFER,
                                                           d.Instrumentcode,
                                                           d.AccumulateSumQty).decTransferAmount;

                        string tmpCurrency = handlerInventory.GetFIFOInstrumentPrice(d.FixedStockQty,
                                                           param.HeaderOffice[0].OfficeCode,
                                                           InstrumentLocation.C_INV_LOC_BUFFER,
                                                           d.Instrumentcode,
                                                           d.AccumulateSumQty).decTransferAmountCurrencyType;

                        if(tmpCurrency != null)
                        {
                            detail[tmpCount].AdjustAmountCurrencyType = tmpCurrency;
                        }

                        d.AdjustAmount = Convert.ToDecimal(fifoPrice);
                    }
                    else if (d.AreaCode == InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                    {
                        d.AdjustAmount = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                    }
                    else
                    {
                        doCalPriceCondition doCalulatePrice = handlerInventory.GetMovingAveragePriceCondition(param.HeaderOffice[0].OfficeCode,
                                                                                           null,
                                                                                           null,
                                                                                           d.Instrumentcode,
                                                                                           new string[] { InstrumentLocation.C_INV_LOC_BUFFER },
                                                                                           null
                                                                                         );
                        if (doCalulatePrice != null)
                        {
                            d.AdjustAmount = Convert.ToDecimal(doCalulatePrice.MovingAveragePrice) * d.FixedStockQty;
                            if(doCalulatePrice.MovingAveragePriceCurrencyType != null)
                            {
                                detail[tmpCount].AdjustAmountCurrencyType = doCalulatePrice.MovingAveragePriceCurrencyType;
                            }  
                        }
                    }

                    totalAmount += d.AdjustAmount;
                    detail[tmpCount].AdjustAmount = d.AdjustAmount;
                    tmpCount++;
                }

                List<object> resultList = new List<object>();
                resultList.Add(detail.ToArray());
                resultList.Add(totalAmount.Value.ToString("N2"));

                return Json(resultList.ToArray());
            }
            catch (Exception ex)
            {

                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        private void IVS190_CountAccumulateSumQty(List<IVS190_DetailData> detail)
        {

            Dictionary<string, int?> acc = new Dictionary<string, int?>();
            foreach (var d in detail)
            {
                if (d.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || d.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO)
                {
                    // count accumualteSumQty of second hand group (โดยไม่รวมตัวเองเข้าไปด้วย) 

                    //d.AccumulateSumQty = acc;
                    //acc += d.FixedStockQty;

                    if (acc.ContainsKey(d.Instrumentcode))
                    {
                        d.AccumulateSumQty = acc[d.Instrumentcode];
                        acc[d.Instrumentcode] += d.FixedStockQty;
                    }
                    else
                    {
                        d.AccumulateSumQty = 0;
                        acc.Add(d.Instrumentcode, d.FixedStockQty);
                    }
                }
            }
        }

    }

}


