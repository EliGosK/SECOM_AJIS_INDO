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
using SECOM_AJIS.Presentation.Common.Helpers;
using SECOM_AJIS.DataEntity.ExchangeRate.ConstantValue;

namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {

        #region Authority

        /// <summary>
        /// - Check system suspending.<br />
        /// - Check user permission for screen IVS012.<br />
        /// - Check freezed data.<br />
        /// - Check started stock checking.<br />
        /// - Get inventory head office.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS012_Authority(IVS012_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IInventoryHandler handInven = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (comh.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_STOCKIN_ASSET, FunctionID.C_FUNC_ID_OPERATE))
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

            return InitialScreenEnvironment<IVS012_ScreenParameter>("IVS012", param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS012")]
        public ActionResult IVS012()
        {
            IVS012_ScreenParameter param = GetScreenObject<IVS012_ScreenParameter>();

            ViewBag.PreloadSlipNo = param.PreloadSlipNo;

            ViewBag.AreaSample = InstrumentArea.C_INV_AREA_NEW_SAMPLE;
            return View();
        }
        #endregion

        /// <summary>
        /// Search inventory slip detail.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult RetrieveStockInSlip(IVS012_SearchCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {      //Valid Cond
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                ValidatorUtil.BuildErrorMessage(res, this, null);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                IVS012_ScreenParameter param = GetScreenObject<IVS012_ScreenParameter>();
                if (param.lstInventorySlipDetail == null)
                    param.lstInventorySlipDetail = new List<doInventorySlipDetailList>();

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                ICommonHandler commonHandler = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doInventorySlipDetailList> lstSlipDetail = InvH.GetInventorySlipDetailForRegister(Cond.SlipNo);

                if (lstSlipDetail.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    return Json(res);
                }

                for (int i = 0; i < lstSlipDetail.Count; i++)
                {
                    //Default Currency
                    if (lstSlipDetail[i].InstrumentAmountCurrencyType == null)
                    {
                        lstSlipDetail[i].InstrumentAmountCurrencyType = SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL;
                    }

                    //Exchange Rate Currency
                    if(lstSlipDetail[i].InstrumentAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    {
                        decimal amt = lstSlipDetail[i].InstrumentAmount ?? 0;
                        double errorCode = 0;

                        decimal localAmt = commonHandler.ConvertCurrencyPrice(amt, lstSlipDetail[i].InstrumentAmountCurrencyType, CurrencyUtil.C_CURRENCY_LOCAL, DateTime.Now, ref errorCode);

                        lstSlipDetail[i].InstrumentAmount = localAmt;
                    }
                    else
                    {
                        decimal amt = lstSlipDetail[i].InstrumentAmount ?? 0;
                        double errorCode = 0;

                        decimal usdAmt = commonHandler.ConvertCurrencyPrice(amt, lstSlipDetail[i].InstrumentAmountCurrencyType, CurrencyUtil.C_CURRENCY_US, DateTime.Now, ref errorCode);

                        lstSlipDetail[i].InstrumentAmount = usdAmt;
                    }
                }

                // Monthly Price @ 2015 : Allow to register asset anytime until LockFlag = true
                //if (lstSlipDetail.FirstOrDefault().RegisterAssetFlag == "1")
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4146);
                //    return Json(res);
                //}

                param.lstInventorySlipDetail = lstSlipDetail;

                //Add by Jutarat A. on 30052013
                List<tbt_InventorySlip> lstInventorySlip = InvH.GetTbt_InventorySlip(Cond.SlipNo);

                // Monthly Price @ 2015 : Allow to register asset anytime until LockFlag = true
                if (lstInventorySlip.Count > 0 && lstInventorySlip[0].LockFlag == true)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4148);
                    return Json(res);
                }

                param.lstInventorySlip = lstInventorySlip;
                //End Add

                res.ResultData = CommonUtil.ConvertToXml<doInventorySlipDetailList>(lstSlipDetail, "Inventory\\IVS012_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Get header of selected slip.
        /// </summary>
        /// <param name="SlipNo"></param>
        /// <returns></returns>
        public ActionResult GetHeaderSlipDetail(string SlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS012_ScreenParameter param = GetScreenObject<IVS012_ScreenParameter>();
                if (param.lstInventorySlipDetail == null)
                    param.lstInventorySlipDetail = new List<doInventorySlipDetailList>();

                if (param.lstInventorySlipDetail.Count > 0)
                {
                    res.ResultData = param.lstInventorySlipDetail[0];
                }

                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        } // initGrid

        /// <summary>
        /// Get config for Instrument List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS012_InstrumentGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS012_Instrument"));
        }

        /// <summary>
        /// Validate before show confirm screen.<br />
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="SlipNo"></param>
        /// <returns></returns>
        public ActionResult RegisterIvs012(List<doIvs012Inventory> cond, string SlipNo, string VoucherNo, DateTime? VoucherDate)
        {
            ObjectResultData res = new ObjectResultData();
            IVS012_ScreenParameter prm = GetScreenObject<IVS012_ScreenParameter>();
            try
            {

                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //3.1
                if (comh.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                // Requested by user @ 2015/08/04, allowed 0 price.
                //var qErrorCtrls = cond.Where(i =>
                //    (
                //        i.InstrumentArea == InstrumentArea.C_INV_AREA_NEW_SALE 
                //        || i.InstrumentArea == InstrumentArea.C_INV_AREA_NEW_RENTAL 
                //        || i.InstrumentArea == InstrumentArea.C_INV_AREA_NEW_DEMO
                //    ) 
                //    && i.InstrumentTotalPrice <= 0 
                //    && i.InstrumentTotalPriceEnable == true
                //).Select(i => i.txtInstrumentTotalPrice);

                //if (qErrorCtrls.Count() > 0)
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4052, null, qErrorCtrls.ToArray());
                //    return Json(res);
                //}

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<tbt_InventorySlip> lstInventorySlip = InvH.GetTbt_InventorySlip(SlipNo);

                // Monthly Price @ 2015 : Allow to register asset anytime until LockFlag = true
                if (lstInventorySlip.Count > 0 && lstInventorySlip[0].LockFlag == true)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4148);
                    return Json(res);
                }

                prm.SlipNo = SlipNo;
                prm.lstIVS012Inventory = cond;
                prm.VoucherNo = VoucherNo;
                prm.VoucherDate = VoucherDate;

                return Json(true);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Register stock in asset.<br />
        /// - Update inventory current.<br />
        /// - Update inventory slip.<br />
        /// - Update account transfer sample/new instrument.
        /// </summary>
        /// <returns></returns>
        public ActionResult cmdConfirmIVS012()
        {
            IVS012_ScreenParameter prm = GetScreenObject<IVS012_ScreenParameter>();
            List<doIvs012Inventory> Cond = prm.lstIVS012Inventory;
            ObjectResultData res = new ObjectResultData();
            using (TransactionScope scope = new TransactionScope())
            {
                decimal decMovingAveragePrice = 0;
                try
                {

                    ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    if (comh.IsSystemSuspending())
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                        return Json(res);
                    }


                    #region Monthly Price @ 2015 : C_INV_SHELF_NO_NOT_PRICE has no longer been used.
                    ////5.3
                    //List<tbt_InventoryCurrent> SourceCurrent = InvH.GetTbt_InventoryCurrent(prm.office.OfficeCode, InstrumentLocation.C_INV_LOC_INSTOCK, null, ShelfNo.C_INV_SHELF_NO_NOT_PRICE, null);
                    //
                    //foreach (doIvs012Inventory i in Cond)
                    //{
                    //    List<tbt_InventoryCurrent> OriginCurrent = (from c in SourceCurrent where i.InstrumentCode == c.InstrumentCode && i.InstrumentArea == c.AreaCode select c).ToList<tbt_InventoryCurrent>();
                    //
                    //    if (OriginCurrent.Count > 0)
                    //    {
                    //        OriginCurrent[0].InstrumentQty = OriginCurrent[0].InstrumentQty - i.StockInQty;
                    //        OriginCurrent[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //        OriginCurrent[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //        InvH.UpdateTbt_InventoryCurrent(OriginCurrent);
                    //    }
                    //}
                    //
                    //List<tbt_InventoryCurrent> exCurrent = InvH.GetTbt_InventoryCurrent(prm.office.OfficeCode, InstrumentLocation.C_INV_LOC_INSTOCK, null, ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF, null);
                    //
                    //foreach (doIvs012Inventory i in Cond)
                    //{
                    //    var current = exCurrent.Where(c => i.InstrumentCode == c.InstrumentCode && i.InstrumentArea == c.AreaCode);
                    //    if (current.Count() > 0)
                    //    {
                    //        tbt_InventoryCurrent InvCurrent = current.First();
                    //        InvCurrent.InstrumentQty = (InvCurrent.InstrumentQty.HasValue ? InvCurrent.InstrumentQty : 0) + i.StockInQty;
                    //        InvCurrent.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //        InvCurrent.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //
                    //        InvH.UpdateTbt_InventoryCurrent(new List<tbt_InventoryCurrent>(new tbt_InventoryCurrent[] { InvCurrent }));
                    //    }
                    //    else
                    //    {
                    //        tbt_InventoryCurrent InvCurrent = new tbt_InventoryCurrent();
                    //        InvCurrent.OfficeCode = prm.office.OfficeCode;
                    //        InvCurrent.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                    //        InvCurrent.AreaCode = i.InstrumentArea;
                    //        InvCurrent.ShelfNo = ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF;
                    //        InvCurrent.InstrumentCode = i.InstrumentCode;
                    //        InvCurrent.InstrumentQty = i.StockInQty;
                    //        InvCurrent.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //        InvCurrent.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //        InvCurrent.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //        InvCurrent.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //
                    //        InvH.InsertTbt_InventoryCurrent(new List<tbt_InventoryCurrent>(new tbt_InventoryCurrent[] { InvCurrent }));
                    //    }
                    //}
                    #endregion

                    //Modify by Jutarat A. on 30052013
                    //List<tbt_InventorySlip> lstSlip = InvH.GetTbt_InventorySlip(prm.SlipNo);
                    //List<tbt_InventorySlip> lstSlip = prm.lstInventorySlip;
                    List<tbt_InventorySlip> lstSlip = CommonUtil.ClonsObjectList<tbt_InventorySlip,tbt_InventorySlip>(prm.lstInventorySlip); //Modify by Jutarat A. on 31102013
                    if (lstSlip == null || lstSlip.Count == 0)
                        lstSlip = InvH.GetTbt_InventorySlip(prm.SlipNo);
                    //End Modify
                    
                    foreach (tbt_InventorySlip slip in lstSlip)
                    {
                        slip.RegisterAssetFlag = RegisterAssetFlag.C_INV_REGISTER_ASSET_REGISTERED;
                        slip.VoucherID = prm.VoucherNo;
                        slip.VoucherDate = prm.VoucherDate;
                        //Comment by Jutarat A. on 30052013 (Set at UpdateTbt_InventorySlip())
                        //slip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        //slip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        //End Comment
                    }
                    InvH.UpdateTbt_InventorySlip(lstSlip);

                    DateTime Stock_InDate = lstSlip[0].StockInDate.GetValueOrDefault(DateTime.Now);

                    //5.6
                    foreach (doIvs012Inventory i in Cond)
                    {
                        ICommonHandler commonHandler = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        List<tbt_InventorySlipDetail> lstSlipDetail = InvH.GetTbt_InventorySlipDetail(prm.SlipNo, i.RunningNo);

                        if (lstSlipDetail.Count > 0)
                        {
                            lstSlipDetail[0].InstrumentCode = i.InstrumentCode;
                            //lstSlipDetail[0].InstrumentAmount = Convert.ToDecimal(i.StockInUnitPrice * i.StockInQty);
                            double errorCode = 0;
                            if (i.InstrumentAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            {
                                lstSlipDetail[0].InstrumentAmount = null;
                                lstSlipDetail[0].InstrumentAmountUsd = Convert.ToDecimal(i.InstrumentAmountUsd); //Modify by Jutarat A. on 09042013
                                lstSlipDetail[0].InstrumentAmountCurrencyType = i.InstrumentAmountCurrencyType;
                            }
                            else
                            {
                                lstSlipDetail[0].InstrumentAmount = Convert.ToDecimal(i.InstrumentTotalPrice); //Modify by Jutarat A. on 09042013
                                lstSlipDetail[0].InstrumentAmountUsd = commonHandler.ConvertCurrencyPrice(Convert.ToDecimal(i.InstrumentTotalPrice), CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US, Stock_InDate, ref errorCode);
                                lstSlipDetail[0].InstrumentAmountCurrencyType = i.InstrumentAmountCurrencyType;

                                if (errorCode == RateCalcCode.C_ERROR_NO_RATE)
                                {
                                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0334);
                                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING_DIALOG;
                                }
                            }  
                        }

                        List<tbt_InventorySlipDetail> doInventorySlipDetail = InvH.UpdateTbt_InventorySlipDetail(lstSlipDetail);

                        if (doInventorySlipDetail.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_SLIP_DETAIL });
                        }

                        #region Monthly Price @ 2015 : moved to stock-in process (IVS010)
                        //if (i.InstrumentArea == InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                        //{

                        //    doGroupSampleInstrument GroupSample = new doGroupSampleInstrument();
                        //    GroupSample.SourceOfficeCode = prm.office.OfficeCode;
                        //    GroupSample.DestinationOfficeCode = prm.office.OfficeCode;
                        //    GroupSample.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                        //    GroupSample.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                        //    GroupSample.ContractCode = null;
                        //    GroupSample.ProjectCode = null;
                        //    GroupSample.Instrumentcode = i.InstrumentCode;
                        //    GroupSample.TransferQty = i.StockInQty;
                        //    GroupSample.ObjectID = ScreenID.C_INV_SCREEN_ID_STOCKIN_ASSET;
                        //    InvH.UpdateAccountTransferSampleInstrument(GroupSample, null);
                        //}
                        //else
                        //{
                        //    doGroupNewInstrument GroupNew = new doGroupNewInstrument();
                        //    GroupNew.SourceOfficeCode = prm.office.OfficeCode;
                        //    GroupNew.DestinationOfficeCode = prm.office.OfficeCode;
                        //    GroupNew.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                        //    GroupNew.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                        //    GroupNew.ProjectCode = null;
                        //    GroupNew.ContractCode = null;
                        //    GroupNew.Instrumentcode = i.InstrumentCode;
                        //    GroupNew.TransferQty = i.StockInQty;

                        //    //GroupNew.UnitPrice = Convert.ToDecimal(i.StockInUnitPrice);
                        //    GroupNew.UnitPrice = Convert.ToDecimal(i.InstrumentTotalPrice / i.StockInQty); //Modify by Jutarat A. on 09042013

                        //    GroupNew.ObjectID = ScreenID.C_INV_SCREEN_ID_STOCKIN_ASSET;

                        //    decMovingAveragePrice = InvH.CalculateMovingAveragePrice(GroupNew);

                        //    bool blnUpdate = InvH.UpdateAccountTransferNewInstrument(GroupNew, Convert.ToDecimal(decMovingAveragePrice));
                        //}
                        #endregion

                    }

                    scope.Complete();
                    res.ResultData = true;
                    return Json(res);
                }
                catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
            }
        }

        public ActionResult IVS012_GenerateCurrencyNumericTextBox(string id, string currency, string textboxValue, bool enable)
        {
            string html = "";

            if (enable)
            {
                html = TextBoxHelper.NumericTextBoxWithMultipleCurrency(null, id, textboxValue, currency, new { style = "width: 140px;" }).ToString();
            }
            else
            {
                html = TextBoxHelper.NumericTextBoxWithMultipleCurrency(null, id, textboxValue, currency, new { style = "width: 140px;", @readonly=true}).ToString();
            }
            return Json(html);
        }

        // Gen Only Currency
        public ActionResult IVS012_GenerateCurrencyCombobox(string id, string currency)
        {
            string html = ComboBoxHelper.CurrencyCombobox(null, id, currency).ToString();
            return Json(html);
        }
    }
}


