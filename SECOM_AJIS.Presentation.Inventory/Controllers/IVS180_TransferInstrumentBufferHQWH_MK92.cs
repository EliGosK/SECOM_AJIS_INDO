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
        /// - Check user permission for screen IVS180.<br />
        /// - Check freezed data.<br />
        /// - Check started stock checking.<br />
        /// - Get inventory head office.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS180_Authority(IVS180_ScreenParameter param)
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
                if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_TRANSFER_BUFFER, FunctionID.C_FUNC_ID_OPERATE) == false)
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

            return InitialScreenEnvironment<IVS180_ScreenParameter>("IVS180", param, res);
        }

        /// <summary>
        /// Get config for Instrument List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS180_InitialSearchResultGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS180_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get config for Transfer Instrument table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS180_InitialAdjustmentDetailGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS180_AdjustmentDetail", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS180")]
        public ActionResult IVS180()
        {
            IVS180_ScreenParameter param = GetScreenObject<IVS180_ScreenParameter>();

            ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            param.Miscellaneous = CommonUtil.ConvertObjectbyLanguage<doMiscTypeCode, doMiscTypeCode>(param.Miscellaneous, "ValueDisplay");
            ViewBag.SourceLocation = handlerCommon.GetMiscDisplayValue(param.Miscellaneous, MiscType.C_INV_LOC, InstrumentLocation.C_INV_LOC_BUFFER);
            ViewBag.DestinationLocation = handlerCommon.GetMiscDisplayValue(param.Miscellaneous, MiscType.C_INV_LOC, InstrumentLocation.C_INV_LOC_BUFFER);

            IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            List<tbt_InventoryCheckingSchedule> checkingSchedule = handlerInventory.GetLastCheckingSchedule();

            ViewBag.EnableLastChecking = 0;

            if (checkingSchedule.Count > 0 && CheckingStatus.C_INV_CHECKING_STATUS_IMPLEMENTING.Equals(checkingSchedule[0].CheckingStatus))
            {
                ViewBag.CheckingYearMonth = checkingSchedule[0].CreateDate.HasValue ? checkingSchedule[0].CreateDate.Value.ToString("dd-MMM-yyyy") : DateTime.Now.ToString("dd-MMM-yyyy");
                if (checkingSchedule[0].CreateDate.HasValue)
                    ViewBag.EnableLastChecking = 1;
            }
            else
            {
                ViewBag.CheckingYearMonth = DateTime.Now.ToString("dd-MMM-yyyy");
            }

            return View();
        }

        /// <summary>
        /// Get destination location name from source locatin code.
        /// </summary>
        /// <param name="LocationCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult IVS180_GetLocation(string LocationCode)
        {

            ObjectResultData res = new ObjectResultData();
            try
            {
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<string> list = new List<string>();
                List<doMiscTypeCode> dtMisc = new List<doMiscTypeCode>();
                List<doMiscTypeCode> ResMisc = new List<doMiscTypeCode>();
                list.Add(MiscType.C_INV_LOC);
                dtMisc = handlerCommon.GetMiscTypeCodeListByFieldName(list);

                string Destination = string.Empty;

                if (LocationCode == InstrumentLocation.C_INV_LOC_INSTOCK)
                {
                    ResMisc = (from c in dtMisc
                               where (c.ValueCode == InstrumentLocation.C_INV_LOC_INSTOCK)
                               select c).ToList<doMiscTypeCode>();

                    // Get Destination Location name
                    var dest = (from p in dtMisc where p.ValueCode == InstrumentLocation.C_INV_LOC_BUFFER select p).ToList<doMiscTypeCode>();
                    if (dest.Count > 0)
                    {
                        Destination = dest[0].ValueDisplay;
                    }
                }
                else if (LocationCode == InstrumentLocation.C_INV_LOC_BUFFER)
                {
                    ResMisc = (from c in dtMisc
                               where (c.ValueCode == InstrumentLocation.C_INV_LOC_INSTOCK ||
                                   c.ValueCode == InstrumentLocation.C_INV_LOC_BUFFER)
                               select c).ToList<doMiscTypeCode>();

                    // Get Destination Location name
                    var dest = (from p in dtMisc where p.ValueCode == InstrumentLocation.C_INV_LOC_INSTOCK select p).ToList<doMiscTypeCode>();
                    if (dest.Count > 0)
                    {
                        Destination = dest[0].ValueDisplay;
                    }
                }

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(ResMisc, "ValueCodeDisplay", "ValueCode", false);

                List<object> resultList = new List<object>();
                resultList.Add(cboModel);
                resultList.Add(Destination);

                return Json(resultList);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);

            }
        }

        /// <summary>
        /// Search inventory instrument.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="sourceLocationCode"></param>
        /// <returns></returns>
        public ActionResult IVS180_SearchResponse(doSearchInstrumentListCondition cond, string sourceLocationCode)
        {

            List<dtSearchInstrumentListResult> list = new List<dtSearchInstrumentListResult>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                //Validate #1
                doSearchInstrument searchCri = CommonUtil.CloneObject<doSearchInstrumentListCondition, doSearchInstrument>(cond);

                if (CommonUtil.IsNullAllField(searchCri))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }

                IInventoryHandler handler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                IVS180_ScreenParameter param = GetScreenObject<IVS180_ScreenParameter>();

                // Keep SourceLocationCode and DestinationLocation
                param.SourceLocationCode = sourceLocationCode;
                if (sourceLocationCode == InstrumentLocation.C_INV_LOC_BUFFER)
                {
                    param.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                }
                else
                {
                    param.DestinationLocationCode = InstrumentLocation.C_INV_LOC_BUFFER;
                }

                if (param.HeaderOffice.Count > 0)
                {
                    cond.OfficeCode = param.HeaderOffice[0].OfficeCode;
                    cond.ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                }

                cond.ExcludeAreaCode = new List<string>() { InstrumentArea.C_INV_AREA_SE_LENDING_DEMO };

                list = handler.SearchInventoryInstrumentList(cond);

                foreach (var item in list)
                {
                    item.LocationCode = cond.LocationCode;
                }

            }
            catch (Exception ex)
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                res.AddErrorMessage(ex);
            }

            res.ResultData = CommonUtil.ConvertToXml<dtSearchInstrumentListResult>(list, "Inventory\\IVS180_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
            return Json(res);


        }

        /// <summary>
        /// Validate before register.
        /// - Check system suspending.<br />
        /// - Check user permission.<br />
        /// - Check approve no.<br />
        /// - Check transfer date.<br />
        /// - Check list not empty.<br />
        /// - Check memo.<br />
        /// - Check quantity.<br />
        /// - Calculate total amount.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult IVS180_Register(IVS180_RegisterData data)
        {

            IVS180_ScreenParameter param = GetScreenObject<IVS180_ScreenParameter>();

            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<object> resultList = new List<object>();
            List<string> HiglightRows = new List<string>();
            string strResult = "0";
            decimal? totalAmount = 0;

            // Default Currency "Rp."
            for (int i=0; i<data.Detail.Count; i++)
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

            try
            {
                // Is suspend ?
                ICommonHandler handlerCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
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
                    string lblApproveNo = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INVENTORY, "IVS180", "lblApproveNo");

                    label.Add(lblApproveNo);
                    controlsName.Add("ApproveNo");

                }
                if (CommonUtil.IsNullOrEmpty(data.Header.TransferDate))
                {
                    string lblTransferDate = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INVENTORY, "IVS180", "lblTransferDate");

                    label.Add(lblTransferDate);
                    controlsName.Add("TransferDate");
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

                foreach (var item in data.Detail)
                {
                    if (param.SourceLocationCode == InstrumentLocation.C_INV_LOC_BUFFER &&
                        param.DestinationLocationCode == InstrumentLocation.C_INV_LOC_INSTOCK &&
                        (item.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || item.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO))
                    {
                        bool blnIsError = handlerInventory.CheckTransferFromBuffer(InstrumentLocation.C_INV_LOC_INSTOCK, item.Instrumentcode);
                        if (blnIsError)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4131, new string[] { item.Instrumentcode }, null);
                            return Json(res);
                        }
                    }
                }

                //Validate #3
                List<string> lstWarningAtControls = new List<string>();
                foreach (var item in data.Detail)
                {
                    if (item.FixedStockQty <= 0 || item.FixedStockQty == null)
                    {
                        lstWarningAtControls.Add(item.txtStockAdjQtyID);
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4030, new string[] { item.Instrumentcode }, lstWarningAtControls.ToArray());
                    }
                }
                if (lstWarningAtControls.Count > 0)
                {
                    countBusinessWaringing++;
                }

                //Business validate      
                foreach (var item in data.Detail)
                {
                    doCheckTransferQty doCheck = new doCheckTransferQty()
                    {
                        OfficeCode = param.HeaderOffice[0].OfficeCode,
                        LocationCode = item.LocationCode,
                        AreaCode = item.AreaCode,
                        ShelfNo = item.ShelfNo,
                        InstrumentCode = item.Instrumentcode,
                        TransferQty = item.FixedStockQty.HasValue ? item.FixedStockQty.Value : 0
                    };

                    doCheckTransferQtyResult result = handlerInventory.CheckTransferQty(doCheck);
                    if (result.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtStockAdjQtyID });
                        countBusinessWaringing++;
                    }
                    else if (result.OverQtyFlag == true)
                    {
                        if (param.SourceLocationCode == InstrumentLocation.C_INV_LOC_BUFFER)
                        {
                            HiglightRows.Add(item.row_id);
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtStockAdjQtyID });
                            countBusinessWaringing++;
                        }

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
                IVS180_CountAccumulateSumQty(data.Detail);

                string strInstrumentLocation = string.Empty;
                string strLocationcode = string.Empty;
                int tmpCount = 0;
                foreach (var d in data.Detail)
                {
                    // get location code
                    strLocationcode = d.LocationCode;
                    if (d.LocationCode == InstrumentLocation.C_INV_LOC_BUFFER)
                    {
                        var cur = handlerInventory.GetTbt_InventoryCurrent(param.HeaderOffice[0].OfficeCode, d.LocationCode, d.AreaCode, d.ShelfNo, d.Instrumentcode);
                        if (cur.Count > 0)
                        {
                            if (cur[0].InstrumentQty == 0)
                            {
                                strLocationcode = InstrumentLocation.C_INV_LOC_INSTOCK;
                            }
                        }
                    }

                    // get FIFO
                    if (d.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || d.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO)
                    {
                        decimal? instPrice;

                        if (param.SourceLocationCode == InstrumentLocation.C_INV_LOC_BUFFER)
                        {
                            instPrice = handlerInventory.GetLIFOInstrumentPrice(
                                param.HeaderOffice[0].OfficeCode,
                                strLocationcode,
                                d.Instrumentcode,
                                d.FixedStockQty,
                                d.AccumulateSumQty,
                                SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL,
                                SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            ).decTransferAmount;

                            string tmpCurrency = handlerInventory.GetLIFOInstrumentPrice(
                                param.HeaderOffice[0].OfficeCode,
                                strLocationcode,
                                d.Instrumentcode,
                                d.FixedStockQty,
                                d.AccumulateSumQty,
                                SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL,
                                SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            ).TransferAmountCurrencyType;

                            if(tmpCurrency != null)
                            {
                                data.Detail[tmpCount].AdjustAmountCurrencyType = tmpCurrency;
                            }
                        }
                        else
                        {
                            instPrice = handlerInventory.GetFIFOInstrumentPrice(
                                d.FixedStockQty,
                                param.HeaderOffice[0].OfficeCode,
                                strLocationcode,
                                d.Instrumentcode,
                                d.AccumulateSumQty
                            ).decTransferAmount;

                            string tmpCurrency = handlerInventory.GetFIFOInstrumentPrice(
                                d.FixedStockQty,
                                param.HeaderOffice[0].OfficeCode,
                                strLocationcode,
                                d.Instrumentcode,
                                d.AccumulateSumQty
                            ).decTransferAmountCurrencyType;

                            if(tmpCurrency != null)
                            {
                                data.Detail[tmpCount].AdjustAmountCurrencyType = tmpCurrency;
                            }
                        }

                        d.AdjustAmount = instPrice.GetValueOrDefault(0);
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
                    data.Detail[tmpCount].AdjustAmount = d.AdjustAmount;
                    totalAmount += d.AdjustAmount;
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
            resultList.Add(HiglightRows.ToArray());
            resultList.Add(data.Detail.ToArray());
            resultList.Add(totalAmount.Value.ToString("N2"));

            res.ResultData = resultList.ToArray();
            return Json(res);


        }

        /// <summary>
        /// Register transfer instrument buffer.<br />
        /// - Check system suspending.<br />
        /// - Check quantity.<br />
        /// - Register transfer instrument.<br />
        /// - Update account transfer new/second hand/sample instrument.<br />
        /// - Generate report.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS180_Confirm()
        {
            string slipNo = string.Empty;
            string slipNoReportPath = string.Empty;
            IVS180_ScreenParameter param = GetScreenObject<IVS180_ScreenParameter>();
            IVS180_RegisterData data = new IVS180_RegisterData();
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

                foreach (var item in data.Detail)
                {
                    if (param.SourceLocationCode == InstrumentLocation.C_INV_LOC_BUFFER &&
                        param.DestinationLocationCode == InstrumentLocation.C_INV_LOC_INSTOCK &&
                        (item.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || item.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO))
                    {
                        bool blnIsError = handlerInventory.CheckTransferFromBuffer(InstrumentLocation.C_INV_LOC_INSTOCK, item.Instrumentcode);
                        if (blnIsError)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4131, new string[] { item.Instrumentcode }, null);
                            return Json(res);
                        }
                    }
                }

                //Business validate
                foreach (var item in data.Detail)
                {
                    doCheckTransferQty doCheck = new doCheckTransferQty()
                    {
                        OfficeCode = param.HeaderOffice[0].OfficeCode,
                        LocationCode = item.LocationCode,
                        AreaCode = item.AreaCode,
                        ShelfNo = item.ShelfNo,
                        InstrumentCode = item.Instrumentcode,
                        TransferQty = item.FixedStockQty.HasValue ? item.FixedStockQty.Value : 0
                    };


                    doCheckTransferQtyResult result = handlerInventory.CheckTransferQty(doCheck);
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
                        if (param.SourceLocationCode == InstrumentLocation.C_INV_LOC_INSTOCK)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtStockAdjQtyID });

                            resultList.Add(data.Detail.ToArray());
                            res.ResultData = resultList.ToArray();
                            return Json(res);
                        }

                    }


                }

                // Prepare data for Register
                doRegisterTransferInstrumentData dsRegisterData = new doRegisterTransferInstrumentData();

                dsRegisterData.SlipId = SlipID.C_INV_SLIPID_BUFFER;
                dsRegisterData.InventorySlip = new tbt_InventorySlip()
                {
                    SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE,
                    TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_TRANSFER_BUFFER,
                    SlipIssueDate = DateTime.Now,
                    StockInDate = data.Header.TransferDate,
                    StockOutDate = data.Header.TransferDate,
                    SourceLocationCode = param.SourceLocationCode,
                    DestinationLocationCode = param.DestinationLocationCode,
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
                        SourceShelfNo = (param.SourceLocationCode == InstrumentLocation.C_INV_LOC_BUFFER ? ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION : data.Detail[i].ShelfNo),
                        DestinationShelfNo = (param.DestinationLocationCode == InstrumentLocation.C_INV_LOC_INSTOCK ? ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF : ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION),
                        TransferQty = data.Detail[i].FixedStockQty
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

                        //10.5
                        List<tbt_InventoryCheckingSchedule> doTbt_InventoryCheckingSchedule = handlerInventory.GetLastCheckingSchedule();

                        if (doTbt_InventoryCheckingSchedule.Count > 0)
                        {
                            if (doTbt_InventoryCheckingSchedule[0].CheckingStatus.Equals(CheckingStatus.C_INV_CHECKING_STATUS_IMPLEMENTING))
                            {
                                foreach (tbt_InventorySlipDetail i in dsRegisterData.lstTbt_InventorySlipDetail)
                                {
                                    string strShelfNo = string.Empty;
                                    int? intTransferQty = 0;

                                    //10.5.1
                                    if (param.DestinationLocationCode == InstrumentLocation.C_INV_LOC_INSTOCK)
                                    {
                                        strShelfNo = ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF;
                                        intTransferQty = i.TransferQty;
                                    }
                                    else
                                    {
                                        strShelfNo = i.SourceShelfNo;
                                        intTransferQty = 0 - i.TransferQty;
                                    }

                                    //10.5.2
                                    List<tbt_InventoryCheckingTemp> doTbt_InventoryCurrent = handlerInventory.GetTbt_InventoryCheckingTemp(doTbt_InventoryCheckingSchedule[0].CheckingYearMonth, InstrumentLocation.C_INV_LOC_INSTOCK, param.HeaderOffice[0].OfficeCode, strShelfNo, i.SourceAreaCode, i.InstrumentCode);

                                    if (doTbt_InventoryCurrent.Count <= 0)
                                    {
                                        tbt_InventoryCheckingTemp doTbt_InventoryCheckingTemp = new tbt_InventoryCheckingTemp();
                                        doTbt_InventoryCheckingTemp.CheckingYearMonth = doTbt_InventoryCheckingSchedule[0].CheckingYearMonth;
                                        doTbt_InventoryCheckingTemp.OfficeCode = param.HeaderOffice[0].OfficeCode;
                                        doTbt_InventoryCheckingTemp.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                        doTbt_InventoryCheckingTemp.AreaCode = i.SourceAreaCode;
                                        doTbt_InventoryCheckingTemp.ShelfNo = strShelfNo;
                                        doTbt_InventoryCheckingTemp.InstrumentCode = i.InstrumentCode;
                                        doTbt_InventoryCheckingTemp.InstrumentQty = intTransferQty;
                                        doTbt_InventoryCheckingTemp.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                        doTbt_InventoryCheckingTemp.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                        doTbt_InventoryCheckingTemp.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                        doTbt_InventoryCheckingTemp.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                        List<tbt_InventoryCheckingTemp> lstTempInsert = new List<tbt_InventoryCheckingTemp>();
                                        lstTempInsert.Add(doTbt_InventoryCheckingTemp);

                                        List<tbt_InventoryCheckingTemp> lstTbt_InventoryCheckingTemp = handlerInventory.InsertTbt_InventoryCheckingTemp(lstTempInsert);

                                        if (lstTbt_InventoryCheckingTemp.Count <= 0)
                                        {
                                            return Json(res);
                                        }
                                    }
                                    else
                                    {
                                        doTbt_InventoryCurrent[0].InstrumentQty = (doTbt_InventoryCurrent[0].InstrumentQty ?? 0) + intTransferQty;
                                        doTbt_InventoryCurrent[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                        doTbt_InventoryCurrent[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                        List<tbt_InventoryCheckingTemp> lstTbt_InventoryCheckingTemp = handlerInventory.UpdateTbt_InventoryCheckingTemp(doTbt_InventoryCurrent);

                                        if (lstTbt_InventoryCheckingTemp.Count <= 0)
                                        {
                                            return Json(res);
                                        }
                                    }
                                } // End Loop
                            } // End doTbt_InventoryCheckingSchedule.CheckingStatus = 1
                        }

                        //--------------- Update related table --------------------

                        // Check New instrument
                        int bCheckingNewInstrument = handlerInventory.CheckNewInstrument(slipNo_result);
                        var currentStock = handlerInventory.GetTbt_InventoryCurrent(param.HeaderOffice[0].OfficeCode, InstrumentLocation.C_INV_LOC_BUFFER, null, null, null);

                        string[] AreaCodeList = {InstrumentArea.C_INV_AREA_NEW_SALE , 
                                         InstrumentArea.C_INV_AREA_NEW_RENTAL ,
                                         InstrumentArea.C_INV_AREA_NEW_DEMO };
                        currentStock = (from p in currentStock where AreaCodeList.Contains(p.AreaCode) select p).ToList<tbt_InventoryCurrent>();

                        if (bCheckingNewInstrument == 1)
                        {
                            List<doGroupNewInstrument> dtGroupNewInstrument = handlerInventory.GetGroupNewInstrument(slipNo_result);
                            foreach (var item in dtGroupNewInstrument)
                            {
                                bool swapFlag = false;
                                var isExist = currentStock.Any(m => m.InstrumentCode == item.Instrumentcode);

                                if (!isExist && item.SourceLocationCode == InstrumentLocation.C_INV_LOC_BUFFER)
                                {
                                    item.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                    swapFlag = true;
                                }

                                #region Monthly Price @ 2015
                                //decimal decMovingAveragePrice = handlerInventory.CalculateMovingAveragePrice(item);
                                decimal decMovingAveragePrice = handlerInventory.GetMonthlyAveragePrice(item.Instrumentcode, dsRegisterData.InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                                #endregion

                                if (swapFlag)
                                {
                                    item.SourceLocationCode = InstrumentLocation.C_INV_LOC_BUFFER;
                                }

                                handlerInventory.UpdateAccountTransferNewInstrument(item, decMovingAveragePrice);
                            }
                        }


                        // Check secondhand instrument 
                        int bCheckSeconhandInstrument = handlerInventory.CheckSecondhandInstrument(slipNo_result);
                        if (bCheckSeconhandInstrument == 1)
                        {
                            List<doGroupSecondhandInstrument> dtGroupSecondhandInstrument = handlerInventory.GetGroupSecondhandInstrument(slipNo_result);
                            foreach (var item in dtGroupSecondhandInstrument)
                            {
                                handlerInventory.UpdateAccountTransferSecondhandInstrumentIVS180(item);
                            }
                        }


                        // Check sample instrument 
                        int bCheckSampleInstrument = handlerInventory.CheckSampleInstrument(slipNo_result);
                        if (bCheckSampleInstrument == 1)
                        {
                            List<doGroupSampleInstrument> dtGroupSampleInstrument = handlerInventory.GetGroupSampleInstrument(slipNo_result);
                            foreach (var item in dtGroupSampleInstrument)
                            {
                                handlerInventory.UpdateAccountTransferSampleInstrument(item, null);
                            }
                        }

                        // Generate inventory slip report   // C_INV_REPORT_ID_TRANSFER_BUFFER = IVR120

                        IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                        slipNoReportPath = handlerInventoryDocument.GenerateIVR120FilePath(slipNo_result, param.HeaderOffice[0].OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

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

                res.ResultData = slipNo; // Success ! return slip no.]
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
        public ActionResult IVS180_CheckExistFile()
        {
            try
            {
                IVS180_ScreenParameter param = GetScreenObject<IVS180_ScreenParameter>();
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
        public ActionResult IVS180_DownloadPdfAndWriteLog()
        {
            try
            {

                IVS180_ScreenParameter param = GetScreenObject<IVS180_ScreenParameter>();
                string fileName = param.SlipNoReportPath;

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = param.SlipNo,
                    DocumentCode = ReportID.C_INV_REPORT_ID_TRANSFER_BUFFER, // IVR120
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
        /// Register transfer instrument buffer.<br />
        /// - Check transfer from buffer.<br />
        /// - Check fix stock quantity.<br />
        /// - Check quantity.<br />
        /// - Get accumulate sum quantity.<br />
        /// - Calculate total amount.
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public ActionResult IVS180_CalculateAmount(List<IVS180_DetailData> detail)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            List<string> HilightRows = new List<string>();

            try
            {
                IVS180_ScreenParameter param = GetScreenObject<IVS180_ScreenParameter>();
                IInventoryHandler handlerInventory = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                // Default Currency "Rp."
                for (int i=0; i<detail.Count; i++)
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

                foreach (var item in detail)
                {
                    if (param.SourceLocationCode == InstrumentLocation.C_INV_LOC_BUFFER &&
                        param.DestinationLocationCode == InstrumentLocation.C_INV_LOC_INSTOCK &&
                        (item.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || item.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO))
                    {
                        bool blnIsError = handlerInventory.CheckTransferFromBuffer(InstrumentLocation.C_INV_LOC_INSTOCK, item.Instrumentcode);
                        if (blnIsError)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4131, new string[] { item.Instrumentcode }, null);
                            return Json(res);
                        }
                    }
                }

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
                        LocationCode = item.LocationCode,
                        AreaCode = item.AreaCode,
                        ShelfNo = item.ShelfNo,
                        InstrumentCode = item.Instrumentcode,
                        TransferQty = item.FixedStockQty.HasValue ? item.FixedStockQty.Value : 0
                    };

                    doCheckTransferQtyResult result = handlerInventory.CheckTransferQty(doCheck);
                    if (result.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtStockAdjQtyID });
                        countBusinessWaringing++;
                    }
                    else if (result.OverQtyFlag == true)
                    {
                        if (param.SourceLocationCode == InstrumentLocation.C_INV_LOC_BUFFER)
                        {
                            HilightRows.Add(item.row_id);
                        }
                        else
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008
                                            , new string[] { item.Instrumentcode }
                                            , new string[] { item.txtStockAdjQtyID });
                            countBusinessWaringing++;
                        }

                    }


                    item.InstrumentQty = result.CurrentyQty;
                }
                if (countBusinessWaringing > 0)
                {
                    return Json(res);
                }

                // Calculate stock adjust amount
                IVS180_CountAccumulateSumQty(detail); // count accumualteSumQty (ที่ไม่รวมตัวเองเข้าไปด้วย) of second hand group

                string strInstrumentLocation = string.Empty;


                decimal? totalAmount = 0;
                //tmp Count detailList
                int tmpCount = 0;
                string strLocationcode = string.Empty;
                foreach (var d in detail)
                {
                    // get location code
                    strLocationcode = d.LocationCode;
                    if (d.LocationCode == InstrumentLocation.C_INV_LOC_BUFFER)
                    {
                        var cur = handlerInventory.GetTbt_InventoryCurrent(param.HeaderOffice[0].OfficeCode, d.LocationCode, d.AreaCode, d.ShelfNo, d.Instrumentcode);
                        if (cur.Count > 0)
                        {
                            if (cur[0].InstrumentQty == 0)
                            {
                                strLocationcode = InstrumentLocation.C_INV_LOC_INSTOCK;
                            }
                        }
                    }

                    // get FIFO
                    if (d.AreaCode == InstrumentArea.C_INV_AREA_SE_RENTAL || d.AreaCode == InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO)
                    {
                        decimal? instPrice;

                        if (param.SourceLocationCode == InstrumentLocation.C_INV_LOC_BUFFER)
                        {
                            instPrice = handlerInventory.GetLIFOInstrumentPrice(
                                param.HeaderOffice[0].OfficeCode,
                                strLocationcode,
                                d.Instrumentcode,
                                d.FixedStockQty,
                                d.AccumulateSumQty,
                                SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL,
                                SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            ).decTransferAmount;

                            string tmpCurrency = handlerInventory.GetLIFOInstrumentPrice(
                                param.HeaderOffice[0].OfficeCode,
                                strLocationcode,
                                d.Instrumentcode,
                                d.FixedStockQty,
                                d.AccumulateSumQty,
                                SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL,
                                SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US
                            ).TransferAmountCurrencyType;

                            if(tmpCurrency != null)
                            {
                                detail[tmpCount].AdjustAmountCurrencyType = tmpCurrency;
                            }
                        }
                        else
                        {
                            instPrice = handlerInventory.GetFIFOInstrumentPrice(
                                d.FixedStockQty,
                                param.HeaderOffice[0].OfficeCode,
                                strLocationcode,
                                d.Instrumentcode,
                                d.AccumulateSumQty
                            ).decTransferAmount;

                            string tmpCurrency = handlerInventory.GetFIFOInstrumentPrice(
                                d.FixedStockQty,
                                param.HeaderOffice[0].OfficeCode,
                                strLocationcode,
                                d.Instrumentcode,
                                d.AccumulateSumQty
                            ).decTransferAmountCurrencyType;

                            if(tmpCurrency != null)
                            {
                                detail[tmpCount].AdjustAmountCurrencyType = tmpCurrency;
                            }

                        }

                        d.AdjustAmount = Convert.ToDecimal(instPrice);
                    }
                    else if (d.AreaCode == InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                    {
                        d.AdjustAmount = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                    }
                    else
                    {
                        #region R2
                        doCalPriceCondition doCalulatePrice = handlerInventory.GetMovingAveragePriceCondition(param.HeaderOffice[0].OfficeCode,
                                                                                           null,
                                                                                           null,
                                                                                           d.Instrumentcode,
                                                                                           new string[] { strLocationcode },
                                                                                           null
                                                                                         );
                        #endregion

                        if (doCalulatePrice != null)
                        {
                            d.AdjustAmount = Convert.ToDecimal(doCalulatePrice.MovingAveragePrice) * d.FixedStockQty;

                            if (doCalulatePrice.MovingAveragePriceCurrencyType != null)
                            {
                                detail[tmpCount].AdjustAmountCurrencyType = doCalulatePrice.MovingAveragePriceCurrencyType;
                            }
                        }
                    }
                    detail[tmpCount].AdjustAmount = d.AdjustAmount;
                    totalAmount += d.AdjustAmount;
                    tmpCount++;
                }

                List<object> resultList = new List<object>();
                resultList.Add(detail.ToArray());
                resultList.Add(totalAmount.Value.ToString("N2"));
                resultList.Add(HilightRows.ToArray());

                return Json(resultList.ToArray());
            }
            catch (Exception ex)
            {

                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        private void IVS180_CountAccumulateSumQty(List<IVS180_DetailData> detail)
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


