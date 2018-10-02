using System;
using System.Collections.Generic;
using System.IO;
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


namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {

        #region Authority

        /// <summary>
        /// - Check user permission for screen IVS010.<br />
        /// - Check system suspending.<br />
        /// - Check freezed data.<br />
        /// - Check started stock checking.<br />
        /// - Get inventory head office.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS010_Authority(IVS010_ScreenParameter param)
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
                IInventoryHandler handInven = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_STOCKIN, FunctionID.C_FUNC_ID_OPERATE))
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

            return InitialScreenEnvironment<IVS010_ScreenParameter>("IVS010", param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS010")]
        public ActionResult IVS010()
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

            return View();
        }
        #endregion

        /// <summary>
        /// Check does user had permission to special stock in.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS010_CanSpeacialStockIn()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_STOCKIN, FunctionID.C_FUNC_ID_SPECIAL_STOCK_IN))
                    res.ResultData = true;
                else
                    res.ResultData = false;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Check does user can add new stock in.
        /// </summary>
        /// <returns></returns>
        public ActionResult CanAdd()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                res.ResultData = (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_STOCKIN, FunctionID.C_FUNC_ID_ADD));

                //  res.ResultData = true;
                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Search purchase order data.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult searchPurchaseOrder(doPurchaseOrderSearchCond cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {


                ValidatorUtil.BuildErrorMessage(res, this, new[] { cond });
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }


                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doPurchaseOrder> dtPurchaseOrder = InvH.GetPurchaserOrderForMaintain(cond);

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                MiscTypeMappingList lstMiscMap = new MiscTypeMappingList();
                lstMiscMap.AddMiscType(dtPurchaseOrder.ToArray());
                hand.MiscTypeMappingList(lstMiscMap);
                CommonUtil.MappingObjectLanguage<doPurchaseOrder>(dtPurchaseOrder);


                res.ResultData = CommonUtil.ConvertToXml<doPurchaseOrder>(dtPurchaseOrder, "Inventory\\IVS010_PurchaseOrder", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Get config for Purchase Order table.
        /// </summary>
        /// <returns></returns>
        public ActionResult PurchaseOrderGrid()
        {
            ObjectResultData res = new ObjectResultData();
            try
            { return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS010_PurchaseOrder", CommonUtil.GRID_EMPTY_TYPE.SEARCH)); }
            catch (Exception ex)
            { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Get config for Special Stock-in table.
        /// </summary>
        /// <returns></returns>
        public ActionResult SpecialGrid()
        {
            ObjectResultData res = new ObjectResultData();
            try
            { return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS010_SpecialStock", CommonUtil.GRID_EMPTY_TYPE.INSERT)); }
            catch (Exception ex)
            { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Get config for Instrument List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult InstrumentGrid()
        {
            ObjectResultData res = new ObjectResultData();
            try
            { return Json(CommonUtil.ConvertToXml<object>(null, "Inventory\\IVS010_Instrument", CommonUtil.GRID_EMPTY_TYPE.INSERT)); }
            catch (Exception ex)
            { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Check before add special stock in.<br />
        /// - Check require field.<br />
        /// - Check exist instrument.
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public ActionResult checkSpecialAdd(doStockInstrumentCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ValidatorUtil.BuildErrorMessage(res, this, new[] { Cond });
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                IInstrumentMasterHandler InsH = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<tbm_Instrument> lstInst = InsH.GetTbm_Instrument(Cond.InstrumentCode.Trim());
                if (lstInst.Count > 0)
                    res.ResultData = true;
                else
                    res.ResultData = false;


                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Get instrument name from given InstrumentCode.
        /// </summary>
        /// <param name="InstrumentCode"></param>
        /// <returns></returns>
        public ActionResult getInstrumentName(string InstrumentCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                IInventoryHandler invH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                IInstrumentMasterHandler InsH = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<tbm_Instrument> lstInst = InsH.GetTbm_Instrument(InstrumentCode.Trim());

                if (lstInst == null || lstInst.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4037);
                }
                else if (lstInst[0].InstrumentTypeCode != InstrumentType.C_INST_TYPE_GENERAL && lstInst[0].InstrumentTypeCode != InstrumentType.C_INST_TYPE_MATERIAL)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4123, new string[] { lstInst[0].InstrumentCode });
                }
                else if (invH.CheckInstrumentExpansion(lstInst[0].InstrumentCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4124, new string[] { lstInst[0].InstrumentCode });
                }
                else
                {
                    StockInInstrumentSpecial instrument = new StockInInstrumentSpecial();
                    instrument.InstrumentName = lstInst[0].InstrumentName;
                    instrument.InstrumentCode = lstInst[0].InstrumentCode;
                    res.ResultData = instrument;
                }

                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Get purchase order detail.
        /// </summary>
        /// <param name="purchaserOrder"></param>
        /// <returns></returns>
        public ActionResult getPurchasOrderDetail(string purchaserOrder)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doPurchaseOrderDetail> doPurchaseDetail = InvH.GetPurchaseOrderDetailForRegisterStockIn(purchaserOrder);
                CommonUtil.MappingObjectLanguage<doPurchaseOrderDetail>(doPurchaseDetail);
                res.ResultData = CommonUtil.ConvertToXml<doPurchaseOrderDetail>(doPurchaseDetail, "Inventory\\IVS010_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Get purchase order detail for register stock-in.
        /// </summary>
        /// <param name="purchaserOrder"></param>
        /// <returns></returns>
        public ActionResult getSinglePurchasOrderDetail(string purchaserOrder)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doPurchaseOrderDetail> doPurchaseDetail = InvH.GetPurchaseOrderDetailForRegisterStockIn(purchaserOrder);
                CommonUtil.MappingObjectLanguage<doPurchaseOrderDetail>(doPurchaseDetail);
                return Json(doPurchaseDetail);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Save operation to database in case Purchase.<br />
        /// - Create inventory slip.<br />
        /// - Update purchase order.<br />
        /// - Register inventory current.<br />
        /// - Generate report IVR010 .
        /// </summary>
        /// <returns></returns>
        public ActionResult cmdConfirmPurchase()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }

                string strInventorySlipNo = "";
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                        IVS010_ScreenParameter param = GetScreenObject<IVS010_ScreenParameter>();
                        doOffice office = param.office;
                        strInventorySlipNo = InvH.GenerateInventorySlipNo(office.OfficeCode, SlipID.C_INV_SLIPID_STOCK_IN);
                        tbt_InventorySlip InventorySlip = new tbt_InventorySlip();
                        InventorySlip.SlipNo = strInventorySlipNo;
                        InventorySlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
                        InventorySlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKIN_PURCHASE;
                        InventorySlip.InstallationSlipNo = null;
                        InventorySlip.ProjectCode = null;
                        InventorySlip.PurchaseOrderNo = param.RegPurchaseData.PurchaseOrderNo;
                        InventorySlip.ContractCode = null;
                        InventorySlip.SlipIssueDate = null;
                        InventorySlip.StockInDate = param.RegPurchaseData.StockInDate; //Monthly Price @ 2015
                        InventorySlip.StockOutDate = null;
                        InventorySlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_VENDER;
                        InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                        InventorySlip.SourceOfficeCode = office.OfficeCode;
                        InventorySlip.DestinationOfficeCode = office.OfficeCode;
                        InventorySlip.ApproveNo = param.RegPurchaseData.ApproveNo;
                        InventorySlip.StockInFlag = StockInType.C_INV_STOCKIN_TYPE_PURCHASE;
                        InventorySlip.RegisterAssetFlag = RegisterAssetFlag.C_INV_REGISTER_ASSET_UNREGISTER;
                        InventorySlip.Memo = param.RegPurchaseData.Memo;
                        InventorySlip.DeliveryOrderNo = param.RegPurchaseData.SupplierDeliveryOrderNo;
                        InventorySlip.ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                        InventorySlip.RepairSubcontractor = null;
                        InventorySlip.InstallationCompleteFlag = null;
                        InventorySlip.ContractStartServiceFlag = null;
                        InventorySlip.CustomerAcceptanceFlag = null;
                        // InventorySlip.PickingListSlipNo = null;
                        InventorySlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        InventorySlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        InventorySlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        InventorySlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        InventorySlip.VoucherID = param.RegPurchaseData.VoucherNo;
                        InventorySlip.VoucherDate = param.RegPurchaseData.VoucherDate;

                        List<tbt_InventorySlip> lst = new List<tbt_InventorySlip>();
                        lst.Add(InventorySlip);
                        List<tbt_InventorySlip> lstResult = InvH.InsertTbt_InventorySlip(CommonUtil.ConvertToXml_Store<tbt_InventorySlip>(lst));
                        //TODO: (Akat) Nattapong Create log
                        int InstRuningNo = 1;
                        foreach (StockInIntrument i in param.RegPurchaseData.StockInInstrument)
                        {
                            #region Monthly Price @ 2015
                            if (i.InstrumentArea.Contains(':'))
                            {
                                i.InstrumentArea = i.InstrumentArea.Substring(0, i.InstrumentArea.IndexOf(':'));
                            }
							#endregion

                            if (i.NewReceiveQty > 0)
                            {
                                tbt_InventorySlipDetail InventorySlipDetail = new tbt_InventorySlipDetail();
                                InventorySlipDetail.SlipNo = strInventorySlipNo;
                                InventorySlipDetail.RunningNo = InstRuningNo++;
                                InventorySlipDetail.InstrumentCode = i.InstrumentCode;
                                InventorySlipDetail.SourceAreaCode = i.InstrumentArea;
                                InventorySlipDetail.DestinationAreaCode = i.InstrumentArea;
                                InventorySlipDetail.SourceShelfNo = ShelfNo.C_INV_SHELF_STOCK_IN_DEFAULT; //Monthly Price @ 2015
                                InventorySlipDetail.DestinationShelfNo = ShelfNo.C_INV_SHELF_STOCK_IN_DEFAULT; //Monthly Price @ 2015
                                InventorySlipDetail.CurrentQty = null;
                                InventorySlipDetail.TransferQty = i.NewReceiveQty;
                                InventorySlipDetail.NotInstalledQty = null;
                                InventorySlipDetail.UninstalledQty = null;
                                InventorySlipDetail.RemovedQty = null;
                                InventorySlipDetail.UnremovableQty = null;
                                InventorySlipDetail.InstrumentAmount = null;
                                // InventorySlipDetail.PickingNo = null;

                                List<tbt_InventorySlipDetail> lstSlipDetail = new List<tbt_InventorySlipDetail>();
                                lstSlipDetail.Add(InventorySlipDetail);
                                List<tbt_InventorySlipDetail> InsertResult = InvH.InsertTbt_InventorySlipDetail(CommonUtil.ConvertToXml_Store<tbt_InventorySlipDetail>(lstSlipDetail));
                            }
                        }

                        //InvH.UpdateTbt_PurchaseOrdersfv
                        tbt_PurchaseOrder PurchaseOrder = InvH.GetTbt_PurchaseOrder(param.RegPurchaseData.PurchaseOrderNo)[0];



                        //int sumRemainQty = (from c in param.RegPurchaseData.StockInInstrument select c.RemainQty).Sum();
                        //int sumReceiveQty = (from c in param.RegPurchaseData.StockInInstrument select c.NewReceiveQty).Sum();
                        //if (sumRemainQty == sumReceiveQty)
                        //    PurchaseOrder.PurchaseOrderStatus = PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_COMPLETE_RECEIVE;
                        //else if (sumRemainQty > sumReceiveQty && sumReceiveQty > 0)
                        //    PurchaseOrder.PurchaseOrderStatus = PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_PARTIAL_RECEIVE;

                        PurchaseOrder.PurchaseOrderStatus = param.PurchaseOrderStatus;
                        PurchaseOrder.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        PurchaseOrder.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        List<tbt_PurchaseOrder> lstPorder = new List<tbt_PurchaseOrder>();
                        lstPorder.Add(PurchaseOrder);
                        InvH.UpdateTbt_PurchaseOrder(CommonUtil.ConvertToXml_Store<tbt_PurchaseOrder>(lstPorder));

                        List<tbt_InventoryCurrent> lstInvCurUpdate = new List<tbt_InventoryCurrent>();
                        List<tbt_InventoryCurrent> lstInvCurInsert = new List<tbt_InventoryCurrent>();
                        
                        foreach (StockInIntrument i in param.RegPurchaseData.StockInInstrument)
                        {
                            tbt_PurchaseOrderDetail PorderDetail = InvH.GetTbt_PurchaseOrderDetail(param.RegPurchaseData.PurchaseOrderNo, i.InstrumentCode)[0];
                            PorderDetail.ReceiveQty += i.NewReceiveQty;
                            PorderDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            PorderDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                            List<tbt_PurchaseOrderDetail> lstPorderDetail = new List<tbt_PurchaseOrderDetail>();
                            lstPorderDetail.Add(PorderDetail);
                            InvH.UpdateTbt_PurchaseOrderDetail(CommonUtil.ConvertToXml_Store<tbt_PurchaseOrderDetail>(lstPorderDetail));

                            var lstInvCurrent = InvH.GetTbt_InventoryCurrent(
                                param.office.OfficeCode, 
                                InstrumentLocation.C_INV_LOC_INSTOCK, 
                                i.InstrumentArea, 
                                ShelfNo.C_INV_SHELF_STOCK_IN_DEFAULT, //Monthly Price @ 2015
                                i.InstrumentCode
                            );

                            tbt_InventoryCurrent InvenCurrent;

                            if (lstInvCurrent == null || lstInvCurrent.Count <= 0)
                            {
                                InvenCurrent = new tbt_InventoryCurrent();

                                InvenCurrent.OfficeCode = param.office.OfficeCode;
                                InvenCurrent.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                InvenCurrent.AreaCode = i.InstrumentArea;
                                InvenCurrent.ShelfNo = ShelfNo.C_INV_SHELF_STOCK_IN_DEFAULT; //Monthly Price @ 2015
                                InvenCurrent.InstrumentCode = i.InstrumentCode;
                                InvenCurrent.InstrumentQty = i.NewReceiveQty;
                                InvenCurrent.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                InvenCurrent.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                InvenCurrent.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                InvenCurrent.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                lstInvCurInsert.Add(InvenCurrent);
                            }
                            else
                            {
                                InvenCurrent = lstInvCurrent[0];
                                
                                InvenCurrent.InstrumentQty = InvenCurrent.InstrumentQty.GetValueOrDefault(0) + i.NewReceiveQty;
                                InvenCurrent.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                InvenCurrent.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                lstInvCurUpdate.Add(InvenCurrent);
                            }

                            #region Monthly Price @ 2015
                            if (i.InstrumentArea == InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                            {

                                doGroupSampleInstrument GroupSample = new doGroupSampleInstrument();
                                GroupSample.SourceOfficeCode = param.office.OfficeCode;
                                GroupSample.DestinationOfficeCode = param.office.OfficeCode;
                                GroupSample.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                GroupSample.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                GroupSample.ContractCode = null;
                                GroupSample.ProjectCode = null;
                                GroupSample.Instrumentcode = i.InstrumentCode;
                                GroupSample.TransferQty = i.NewReceiveQty;
                                GroupSample.ObjectID = ScreenID.C_INV_SCREEN_ID_STOCKIN;
                                InvH.UpdateAccountTransferSampleInstrument(GroupSample, null);
                            }
                            else
                            {
                                doGroupNewInstrument GroupNew = new doGroupNewInstrument();
                                GroupNew.SourceOfficeCode = param.office.OfficeCode;
                                GroupNew.DestinationOfficeCode = param.office.OfficeCode;
                                GroupNew.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                GroupNew.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                GroupNew.ProjectCode = null;
                                GroupNew.ContractCode = null;
                                GroupNew.Instrumentcode = i.InstrumentCode;
                                GroupNew.TransferQty = i.NewReceiveQty;
                                
                                var price = InvH.GetMonthlyAveragePrice(i.InstrumentCode, InventorySlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                                GroupNew.UnitPrice = price;
                                GroupNew.ObjectID = ScreenID.C_INV_SCREEN_ID_STOCKIN;

                                bool blnUpdate = InvH.UpdateAccountTransferNewInstrument(GroupNew, price);
                            }
                            #endregion

                        }

                        if (lstInvCurUpdate.Count > 0)
                        {
                            InvH.UpdateTbt_InventoryCurrent(lstInvCurUpdate);
                        }

                        if (lstInvCurInsert.Count > 0)
                        {
                            InvH.InsertTbt_InventoryCurrent(lstInvCurInsert);
                        }

                        //Genereate Invenotry Slip Report
                        var srvInvDoc = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                        srvInvDoc.GenerateIVR010FilePath(strInventorySlipNo, null, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
               
                        scope.Complete();
                        res.ResultData = strInventorySlipNo;
                        return Json(res);
                    }
                    catch (Exception ex)
                    {
                        res.AddErrorMessage(ex);
                        return Json(res);
                    }
                }
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Save operation to database in case special stock-in.<br />
        /// - Create inventory slip.<br />
        /// - Register inventory current.<br />
        /// - Generate report IVR010 .
        /// </summary>
        /// <returns></returns>
        public ActionResult cmdConfirmSpecial()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {

                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                } 

                IVS010_ScreenParameter param = GetScreenObject<IVS010_ScreenParameter>();
                string strInventorySlipNo = "";
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                        doOffice office = param.office;
                        strInventorySlipNo = InvH.GenerateInventorySlipNo(office.OfficeCode, SlipID.C_INV_SLIPID_STOCK_IN);

                        tbt_InventorySlip invSlip = new tbt_InventorySlip();
                        invSlip.SlipNo = strInventorySlipNo;
                        invSlip.SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
                        invSlip.TransferTypeCode = TransferType.C_INV_TRANSFERTYPE_STOCKIN_SPECIAL;
                        invSlip.InstallationSlipNo = null;
                        invSlip.ProjectCode = null;
                        invSlip.PurchaseOrderNo = null;
                        invSlip.ContractCode = null;
                        invSlip.SlipIssueDate = null;
                        invSlip.StockInDate = param.RegSpecialData.StockInDate; //Monthly Price @ 2015
                        invSlip.StockOutDate = null;
                        invSlip.SourceLocationCode = InstrumentLocation.C_INV_LOC_SPECIAL;
                        invSlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                        invSlip.SourceOfficeCode = param.office.OfficeCode;
                        invSlip.DestinationOfficeCode = param.office.OfficeCode;
                        invSlip.ApproveNo = param.RegSpecialData.ApproveNo;
                        invSlip.StockInFlag = StockInType.C_INV_STOCKIN_TYPE_SPECIAL;
                        invSlip.RegisterAssetFlag = RegisterAssetFlag.C_INV_REGISTER_ASSET_UNREGISTER;
                        invSlip.Memo = param.RegSpecialData.Memo;
                        invSlip.DeliveryOrderNo = param.RegSpecialData.SupplierDeliveryOrderNo;
                        invSlip.ShelfType = ShelfType.C_INV_SHELF_TYPE_NORMAL;
                        invSlip.RepairSubcontractor = null;
                        invSlip.InstallationCompleteFlag = null;
                        invSlip.ContractStartServiceFlag = null;
                        invSlip.CustomerAcceptanceFlag = null;
                        // invSlip.PickingListSlipNo = null;
                        invSlip.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        invSlip.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        invSlip.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        invSlip.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        invSlip.VoucherID = param.RegSpecialData.VoucherNo;
                        invSlip.VoucherDate = param.RegSpecialData.VoucherDate;

                        List<tbt_InventorySlip> lstInvSlip = new List<tbt_InventorySlip>();
                        lstInvSlip.Add(invSlip);
                        InvH.InsertTbt_InventorySlip(CommonUtil.ConvertToXml_Store<tbt_InventorySlip>(lstInvSlip));

                        int num = 1;
                        List<tbt_InventorySlipDetail> lstInvSlipDet = new List<tbt_InventorySlipDetail>();
                        foreach (StockInInstrumentSpecial i in param.RegSpecialData.StockInstrumentSPC)
                        {
                            #region Monthly Price @ 2015
                            if (i.InstrumentArea.Contains(':'))
                            {
                                i.InstrumentArea = i.InstrumentArea.Substring(0, i.InstrumentArea.IndexOf(':'));
                            }
                            #endregion

                            tbt_InventorySlipDetail InvSlipDet = new tbt_InventorySlipDetail();
                            InvSlipDet.SlipNo = strInventorySlipNo;
                            InvSlipDet.RunningNo = num++;
                            InvSlipDet.InstrumentCode = i.InstrumentCode;
                            InvSlipDet.SourceAreaCode = i.InstrumentArea;
                            InvSlipDet.DestinationAreaCode = i.InstrumentArea;
                            InvSlipDet.SourceShelfNo = ShelfNo.C_INV_SHELF_STOCK_IN_DEFAULT; //Monthly Price @ 2015
                            InvSlipDet.DestinationShelfNo = ShelfNo.C_INV_SHELF_STOCK_IN_DEFAULT; //Monthly Price @ 2015
                            InvSlipDet.CurrentQty = null;
                            InvSlipDet.TransferQty = i.StockInQty;
                            InvSlipDet.NotInstalledQty = null;
                            InvSlipDet.UninstalledQty = null;
                            InvSlipDet.RemovedQty = null;
                            InvSlipDet.UnremovableQty = null;
                            InvSlipDet.InstrumentAmount = null;
                            //      InvSlipDet.PickingListNo = null;
                            lstInvSlipDet.Add(InvSlipDet);
                        }
                        InvH.InsertTbt_InventorySlipDetail(CommonUtil.ConvertToXml_Store<tbt_InventorySlipDetail>(lstInvSlipDet));

                        List<tbt_InventoryCurrent> lstInvCurUpdate = new List<tbt_InventoryCurrent>();
                        List<tbt_InventoryCurrent> lstInvCurInsert = new List<tbt_InventoryCurrent>();
                        
                        foreach (StockInInstrumentSpecial i in param.RegSpecialData.StockInstrumentSPC)
                        {
                            var lstInvCurrent = InvH.GetTbt_InventoryCurrent(
                                param.office.OfficeCode,
                                InstrumentLocation.C_INV_LOC_INSTOCK,
                                i.InstrumentArea,
                                ShelfNo.C_INV_SHELF_STOCK_IN_DEFAULT, //Monthly Price @ 2015
                                i.InstrumentCode
                            );

                            tbt_InventoryCurrent invCur;

                            if (lstInvCurrent == null || lstInvCurrent.Count <= 0)
                            {
                                invCur = new tbt_InventoryCurrent();

                                invCur.OfficeCode = param.office.OfficeCode;
                                invCur.LocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                invCur.AreaCode = i.InstrumentArea;
                                invCur.ShelfNo = ShelfNo.C_INV_SHELF_STOCK_IN_DEFAULT; //Monthly Price @ 2015
                                invCur.InstrumentCode = i.InstrumentCode;
                                invCur.InstrumentQty = i.StockInQty;
                                invCur.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                invCur.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                invCur.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                invCur.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                lstInvCurInsert.Add(invCur);
                            }
                            else
                            {
                                invCur = lstInvCurrent[0];

                                invCur.InstrumentQty = invCur.InstrumentQty.GetValueOrDefault(0) + i.StockInQty;
                                invCur.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                invCur.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                lstInvCurUpdate.Add(invCur);
                            }

                            #region Monthly Price @ 2015
                            if (i.InstrumentArea == InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                            {

                                doGroupSampleInstrument GroupSample = new doGroupSampleInstrument();
                                GroupSample.SourceOfficeCode = param.office.OfficeCode;
                                GroupSample.DestinationOfficeCode = param.office.OfficeCode;
                                GroupSample.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                GroupSample.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                GroupSample.ContractCode = null;
                                GroupSample.ProjectCode = null;
                                GroupSample.Instrumentcode = i.InstrumentCode;
                                GroupSample.TransferQty = i.StockInQty;
                                GroupSample.ObjectID = ScreenID.C_INV_SCREEN_ID_STOCKIN;
                                InvH.UpdateAccountTransferSampleInstrument(GroupSample, null);
                            }
                            else
                            {
                                doGroupNewInstrument GroupNew = new doGroupNewInstrument();
                                GroupNew.SourceOfficeCode = param.office.OfficeCode;
                                GroupNew.DestinationOfficeCode = param.office.OfficeCode;
                                GroupNew.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                GroupNew.DestinationLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                                GroupNew.ProjectCode = null;
                                GroupNew.ContractCode = null;
                                GroupNew.Instrumentcode = i.InstrumentCode;
                                GroupNew.TransferQty = i.StockInQty;

                                var price = InvH.GetMonthlyAveragePrice(i.InstrumentCode, invSlip.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                                GroupNew.UnitPrice = price;
                                GroupNew.ObjectID = ScreenID.C_INV_SCREEN_ID_STOCKIN;

                                bool blnUpdate = InvH.UpdateAccountTransferNewInstrument(GroupNew, price);
                            }
                            #endregion
                        }

                        if (lstInvCurUpdate.Count > 0)
                        {
                            InvH.UpdateTbt_InventoryCurrent(lstInvCurUpdate);
                        }

                        if (lstInvCurInsert.Count > 0)
                        {
                            InvH.InsertTbt_InventoryCurrent(lstInvCurInsert);
                        }

                        //Genereate Invenotry Slip Report
                        var srvInvDoc = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                        srvInvDoc.GenerateIVR010FilePath(strInventorySlipNo, null, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                        scope.Complete();
                        res.ResultData = strInventorySlipNo;
                        return Json(res);
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }

            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Validate data before save operation case Purchase.<br />
        /// - Check system suspending.<br />
        /// - Check permission.<br />
        /// - Check require field.<br />
        /// - Check memo.<br />
        /// - Check summary of receive quantity.<br />
        /// - Check receive and remain quantity.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult cmdRegPurchase(doRegStockInPurchase Cond)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                
                // Check user's authority again for every command.
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_STOCKIN, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                ValidatorUtil.BuildErrorMessage(res, this, new[] { Cond });
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                if (Cond.Memo != null && Cond.Memo.Replace(" ", "").Contains("\n\n\n\n"))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4022
                        , null
                        , new string[] { "DetMemo" });
                    return Json(res);
                }

                int SumReceiveQty = (from c in Cond.StockInInstrument select c.NewReceiveQty).Sum();
                if (SumReceiveQty <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4035);
                    return Json(res);
                }

                //var lstErrorRcvQty = new List<string>();
                //var lstErrorInstArea = new List<string>();
                //foreach (StockInIntrument i in Cond.StockInInstrument)
                //{
                //    if (i.NewReceiveQty > i.RemainQty && !lstErrorRcvQty.Contains(i.NewReceiveQtyID))
                //    {
                //        lstErrorRcvQty.Add(i.NewReceiveQtyID);
                //    }
                //    if (i.NewReceiveQty > 0 && string.IsNullOrEmpty(i.InstrumentArea) && !lstErrorInstArea.Contains(i.InstrumentAreaID))
                //    {
                //        lstErrorInstArea.Add(i.InstrumentAreaID);
                //    }
                //}
                //if (lstErrorRcvQty.Count() > 0)
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4034, null, lstErrorRcvQty.ToArray());
                //}

                //if (lstErrorInstArea.Count() > 0)
                //{
                //    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;    
                //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4119, null, lstErrorInstArea.ToArray());
                //}

                foreach (var d in Cond.StockInInstrument)
                {
                    if (d.NewReceiveQty > d.RemainQty)
                    {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4034
                        , new string[] { d.InstrumentCode }
                        , new string[] { d.NewReceiveQtyID });
                    }

                    if (d.NewReceiveQty > 0 && string.IsNullOrEmpty(d.InstrumentArea))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4119
                            , new string[] { d.InstrumentCode }
                            , new string[] { d.InstrumentAreaID });
                    }
                }

                if (res.IsError)
                {
                    return Json(res);
                }

                IVS010_ScreenParameter param = GetScreenObject<IVS010_ScreenParameter>();

                int sumRemainQty = (from c in Cond.StockInInstrument select c.RemainQty).Sum();
                int sumReceiveQty = (from c in Cond.StockInInstrument select c.NewReceiveQty).Sum();
                if (sumRemainQty == sumReceiveQty)
                    param.PurchaseOrderStatus = PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_COMPLETE_RECEIVE;
                else if (sumRemainQty > sumReceiveQty && sumReceiveQty > 0)
                    param.PurchaseOrderStatus = PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_PARTIAL_RECEIVE;

                Cond.StockInInstrument = Cond.StockInInstrument.Where(d => d.NewReceiveQty > 0).ToList();
                
                param.RegPurchaseData = Cond;
                UpdateScreenObject(param);
                res.ResultData = true;
                return Json(res);
            }
            catch (Exception ex) 
            { 
                res.AddErrorMessage(ex); 
                return Json(res); 
            }
        }

        /// <summary>
        /// Validate data before save operation case Special stock in.<br />
        /// - Check system suspending.<br />
        /// - Check require field.<br />
        /// - Check memo.<br />
        /// - Check summary of receive quantity.<br />
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult cmdRegSpecial(doRegStockInSpecial Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                bool result = false;
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                } 

                ValidatorUtil.BuildErrorMessage(res, this, new[] { Cond });
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                if (Cond.Memo != null && Cond.Memo.Replace(" ", "").Contains("\n\n\n\n"))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4022
                        , null
                        , new string[] { "SpcMemo" });
                    return Json(res);
                }

                int SumReceiveQty = (from c in Cond.StockInstrumentSPC select c.StockInQty).Sum();
                
                if (SumReceiveQty <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4035, null, 
                        Cond.StockInstrumentSPC.Select(d => d.StockInQtyID).ToArray()
                    );
                    return Json(res);
                }

                var arrQtyError = (
                    from d in Cond.StockInstrumentSPC
                    where d.StockInQty <= 0
                    select d.StockInQtyID
                ).ToArray();

                if (arrQtyError != null && arrQtyError.Length > 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4040, null, arrQtyError);
                    return Json(res);
                }

                IVS010_ScreenParameter param = GetScreenObject<IVS010_ScreenParameter>();
                param.RegSpecialData = Cond;
                UpdateScreenObject(param);
                result = true;
                res.ResultData = result;
                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Cancel all operation.
        /// </summary>
        /// <returns></returns>
        public ActionResult cmdReset()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS010_ScreenParameter param = GetScreenObject<IVS010_ScreenParameter>();
                param.RegPurchaseData = new doRegStockInPurchase();
                param.RegSpecialData = new doRegStockInSpecial();
                return Json(true);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Get document from share path.
        /// </summary>
        /// <param name="strInvSlipNo"></param>
        /// <returns></returns>
        public ActionResult IVS010_DownloadDocument(string strInvSlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                var lstDocs = srvCommon.GetTbt_DocumentList(strInvSlipNo, ReportID.C_INV_REPORT_ID_STOCKIN, ConfigName.C_CONFIG_DOC_OCC_DEFAULT);

                if (lstDocs == null || lstDocs.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                    res.ResultData = null;
                }
                else
                {
                    // Edit by Narupon W. 9 May 2012
                    //string path = ReportUtil.GetGeneratedReportPath(lstDocs[0].FilePath);
                    string path = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, lstDocs[0].FilePath);

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
        /// Send document to client and write log.
        /// </summary>
        /// <param name="strInvSlipNo"></param>
        /// <returns></returns>
        public ActionResult IVS010_DownloadPdfAndWriteLog(string strDocumentNo, string documentOCC, string strDocumentCode, string fileName)
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

    }

}


