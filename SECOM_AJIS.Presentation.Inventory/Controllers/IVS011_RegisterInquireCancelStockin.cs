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


namespace SECOM_AJIS.Presentation.Inventory.Controllers
{
    public partial class InventoryController : BaseController
    {

        #region Authority
        /// <summary>
        /// - Check system suspending.<br />
        /// - Check user permission for screen IVS011.<br />
        /// - Check freezed data.<br />
        /// - Check started stock checking.<br />
        /// - Get inventory head office.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS011_Authority(IVS011_ScreenParameter param)
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
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_STOCKIN_CANCEL, FunctionID.C_FUNC_ID_OPERATE))
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

            return InitialScreenEnvironment<IVS011_ScreenParameter>("IVS011", param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS011")]
        public ActionResult IVS011()
        {

            ViewBag.C_INV_REGISTER_ASSET_REGISTERED = RegisterAssetFlag.C_INV_REGISTER_ASSET_REGISTERED;
            ViewBag.C_INV_REGISTER_ASSET_UNREGISTER = RegisterAssetFlag.C_INV_REGISTER_ASSET_UNREGISTER;
            return View();
        }
        #endregion

        /// <summary>
        /// Get config for Slip List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS011_SlipList()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS011_SlipList", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }

        }

        /// <summary>
        /// Get config for Slip Detail List table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS011_SlipDetailList()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS011_SlipDetailList", CommonUtil.GRID_EMPTY_TYPE.VIEW));
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// - Validate require field.<br />
        /// - Search inventory slip.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult SearchStockSlip(doInventorySlipSearchCondition Cond)
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

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doInventorySlipList> lstInvSlip = InvH.GetInventorySlipForSearch(Cond);
                res.ResultData = CommonUtil.ConvertToXml<doInventorySlipList>(lstInvSlip, "Inventory\\IVS011_SlipList", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Get detail of selected slip.
        /// </summary>
        /// <param name="SlipNo"></param>
        /// <returns></returns>
        public ActionResult GetSlipDetail(string SlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doInventorySlipDetailList> lst = InvH.GetInventorySlipDetailForSearch(SlipNo);
                res.ResultData = CommonUtil.ConvertToXml<doInventorySlipDetailList>(lst, "Inventory\\IVS011_SlipDetailList", CommonUtil.GRID_EMPTY_TYPE.SEARCH);

                IVS011_ScreenParameter param = GetScreenObject<IVS011_ScreenParameter>();

                if (param.lstInventory == null)
                    param.lstInventory = new List<doInventorySlipDetailList>();

                param.lstInventory = lst;
                UpdateScreenObject(param);

                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Get header of selected slip.
        /// </summary>
        /// <param name="SlipNo"></param>
        /// <returns></returns>
        public ActionResult GetHeadSlipDetail(string SlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doInventorySlipDetailList> lst = InvH.GetInventorySlipDetailForSearch(SlipNo);
                doInventorySlipDetailList doInvSlipDetail = new doInventorySlipDetailList();

                if (lst.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001, null, null);
                    return Json(res);
                }
                else
                {
                    doInvSlipDetail = lst[0];
                    res.ResultData = doInvSlipDetail;
                    return Json(res);
                }
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Cancel the slip.
        /// - Check system suspending.<br />
        /// - Check user permission.<br />
        /// - Check stock in date.<br />
        /// - Check quantity of instrument.<br />
        /// - Check register asset flag.<br />
        /// - Delete inventory slip detail and inventory slip.<br />
        /// - Update inventory current.<br />
        /// - Update purchase oreder.<br />
        /// </summary>
        /// <param name="SlipNo"></param>
        /// <param name="strStockInType"></param>
        /// <param name="pOrderNo"></param>
        /// <param name="stockInDate"></param>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult CancelSlip(string SlipNo, string strStockInType, string pOrderNo, string stockInDate, IVS011Cancel Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //5.3
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (comh.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_STOCKIN_CANCEL, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //5.4
                if (!string.IsNullOrEmpty(stockInDate))
                {
                    DateTime dStockInDate = Convert.ToDateTime(stockInDate);
                    if (dStockInDate.Month != DateTime.Now.Month)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4044);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }

                //5.5
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                IVS011_ScreenParameter param = GetScreenObject<IVS011_ScreenParameter>();
                if (param.lstInventory == null)
                    param.lstInventory = new List<doInventorySlipDetailList>();

                #region Monthly Price @ 2015
                List<tbt_InventoryCurrent> lstInvCurrentPendingUpdate = new List<tbt_InventoryCurrent>();
                List<tbt_AccountInstock> lstAccInstockPendingUpdate = new List<tbt_AccountInstock>();
                List<tbt_AccountSampleInstock> lstAccSampleInstockPendingUpdate = new List<tbt_AccountSampleInstock>();


                foreach (IVS011INST i in Cond.InstrumentList)
                {
                    int transferqty = i.TransferQty;

                    // Remove qty from inventory current which ShelfNo is default (00)
                    if (transferqty > 0)
                    {
                        List<tbt_InventoryCurrent> doInventoryCurrent = InvH.GetTbt_InventoryCurrent(param.office.OfficeCode, InstrumentLocation.C_INV_LOC_INSTOCK, i.SourceAreaCode, ShelfNo.C_INV_SHELF_STOCK_IN_DEFAULT, i.InstrumentCode);

                        if (doInventoryCurrent.Count > 0 && (doInventoryCurrent[0].InstrumentQty ?? 0) > 0)
                        {
                            if ((doInventoryCurrent[0].InstrumentQty ?? 0) >= transferqty)
                            {
                                doInventoryCurrent[0].InstrumentQty = doInventoryCurrent[0].InstrumentQty - transferqty;
                                transferqty = 0;
                            }
                            else
                            {
                                transferqty = transferqty - (doInventoryCurrent[0].InstrumentQty ?? 0);
                                doInventoryCurrent[0].InstrumentQty = 0;
                            }
                            doInventoryCurrent[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            doInventoryCurrent[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            lstInvCurrentPendingUpdate.Add(doInventoryCurrent[0]);
                        }
                    }

                    // Remove qty from inventory current which ShelfNo is normal shelf
                    if (transferqty > 0)
                    {
                        var shelf = InvH.GetShelfOfArea(i.SourceAreaCode, i.InstrumentCode);
                        if (shelf != null && !string.IsNullOrEmpty(shelf.ShelfNo))
                        {
                            List<tbt_InventoryCurrent> doInventoryCurrent = InvH.GetTbt_InventoryCurrent(param.office.OfficeCode, InstrumentLocation.C_INV_LOC_INSTOCK, i.SourceAreaCode, shelf.ShelfNo, i.InstrumentCode);

                            if (doInventoryCurrent.Count > 0 && (doInventoryCurrent[0].InstrumentQty ?? 0) > 0)
                            {
                                if ((doInventoryCurrent[0].InstrumentQty ?? 0) >= transferqty)
                                {
                                    doInventoryCurrent[0].InstrumentQty = doInventoryCurrent[0].InstrumentQty - transferqty;
                                    transferqty = 0;
                                }
                                else
                                {
                                    transferqty = transferqty - (doInventoryCurrent[0].InstrumentQty ?? 0);
                                    doInventoryCurrent[0].InstrumentQty = 0;
                                }
                                doInventoryCurrent[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                doInventoryCurrent[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                lstInvCurrentPendingUpdate.Add(doInventoryCurrent[0]);
                            }
                        }
                    }

                    // Not enough stock used to remove. Cancelling slip denied.
                    if (transferqty > 0)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4045, new string[] { i.InstrumentCode, i.AreaCodeName }, new string[] { i.row_id });
                        return Json(res);
                    }
                }

                foreach (IVS011INST i in Cond.InstrumentList)
                {
                    if (i.SourceAreaCode == InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                    {
                        // Remove qty from tbt_AccountSampleInStock
                        var accountSampleInStock = InvH.GetTbt_AccountSampleInStock(i.InstrumentCode, InstrumentLocation.C_INV_LOC_INSTOCK, param.office.OfficeCode);

                        // Not enough stock used to remove. Cancelling slip denied.
                        if (accountSampleInStock == null || accountSampleInStock.Count <= 0 || (accountSampleInStock[0].InstrumentQty ?? 0) < i.TransferQty)
                        {
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4045, new string[] { i.InstrumentCode, i.AreaCodeName }, new string[] { i.row_id });
                            return Json(res);
                        }

                        accountSampleInStock[0].InstrumentQty = accountSampleInStock[0].InstrumentQty - i.TransferQty;
                        accountSampleInStock[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        accountSampleInStock[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        lstAccSampleInstockPendingUpdate.Add(accountSampleInStock[0]);
                    }
                    else
                    {
                        // Remove qty from tbt_AccountInStock
                        var accountInStock = InvH.GetTbt_AccountInStock(i.InstrumentCode, InstrumentLocation.C_INV_LOC_INSTOCK, param.office.OfficeCode);

                        // Not enough stock used to remove. Cancelling slip denied.
                        if (accountInStock == null || accountInStock.Count <= 0 || (accountInStock[0].InstrumentQty ?? 0) < i.TransferQty)
                        {
                            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4045, new string[] { i.InstrumentCode, i.AreaCodeName }, new string[] { i.row_id });
                            return Json(res);
                        }

                        accountInStock[0].InstrumentQty = accountInStock[0].InstrumentQty - i.TransferQty;
                        accountInStock[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        accountInStock[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        lstAccInstockPendingUpdate.Add(accountInStock[0]);
                    }
                }
                #endregion

                List<tbt_InventorySlip> tbtSlip = InvH.GetTbt_InventorySlip(SlipNo);
                if (tbtSlip.Count > 0)
                {
                    // Monthly Price @ 2015 : Allow to register asset anytime until LockFlag = true
                    //if (tbtSlip[0].RegisterAssetFlag == RegisterAssetFlag.C_INV_REGISTER_ASSET_REGISTERED)
                    //{
                    //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4046);
                    //    return Json(res);
                    //}
                    if (tbtSlip[0].LockFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4148);
                        return Json(res);
                    }
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        InvH.DeleteTbt_InventorySlipDetail(SlipNo);
                        InvH.DeleteTbt_InventorySlip(SlipNo);

                        #region Monthly Price @ 2015
                        InvH.UpdateTbt_InventoryCurrent(CommonUtil.ConvertToXml_Store<tbt_InventoryCurrent>(lstInvCurrentPendingUpdate));
                        if (lstAccSampleInstockPendingUpdate.Count > 0)
                        {
                            InvH.UpdateTbt_AccountSampleInStock(lstAccSampleInstockPendingUpdate);
                        }
                        if (lstAccInstockPendingUpdate.Count > 0)
                        {
                            InvH.UpdateTbt_AccountInStock(lstAccInstockPendingUpdate);
                        }

                        // Akat K. if stock-in type is Purchase Order then update purchase order
                        if (strStockInType == StockInType.C_INV_STOCKIN_TYPE_PURCHASE)
                        {
                            foreach (doInventorySlipDetailList i in param.lstInventory)
                            {
                                List<tbt_PurchaseOrderDetail> doPurchaseOrderDetail = InvH.GetTbt_PurchaseOrderDetail(pOrderNo, i.InstrumentCode);

                                if (doPurchaseOrderDetail.Count > 0)
                                {
                                    foreach (tbt_PurchaseOrderDetail p in doPurchaseOrderDetail)
                                    {
                                        p.ReceiveQty = p.ReceiveQty - i.TransferQty;
                                        p.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                        p.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    }

                                    InvH.UpdateTbt_PurchaseOrderDetail(CommonUtil.ConvertToXml_Store<tbt_PurchaseOrderDetail>(doPurchaseOrderDetail));
                                }
                            }

                            List<tbt_PurchaseOrder> lstPurchase = InvH.GetTbt_PurchaseOrder(pOrderNo);
                            tbt_PurchaseOrder Porder = lstPurchase[0];
                            Porder.PurchaseOrderNo = pOrderNo;

                            List<tbt_PurchaseOrderDetail> tbt_PorderDetail = InvH.GetTbt_PurchaseOrderDetail(pOrderNo, null);
                            int? sumReceiveQty = (from c in tbt_PorderDetail select c.ReceiveQty).Sum();
                            int? sumModifyOrderQty = (from c in tbt_PorderDetail select c.ModifyOrderQty).Sum();
                            int? sumFirstQty = (from c in tbt_PorderDetail select c.FirstOrderQty).Sum();
                            int? sumTmpQty = 0;

                            if (sumReceiveQty == 0)
                                Porder.PurchaseOrderStatus = PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_WAIT_TO_RECEIVE;
                            else
                            {
                                if (sumModifyOrderQty > 0)
                                    sumTmpQty = sumModifyOrderQty;
                                else
                                    sumTmpQty = sumFirstQty;

                                if (sumTmpQty > sumReceiveQty && sumReceiveQty > 0)
                                    Porder.PurchaseOrderStatus = PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_PARTIAL_RECEIVE;
                            }
                            Porder.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            Porder.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            List<tbt_PurchaseOrder> lstOrder = new List<tbt_PurchaseOrder>();
                            lstOrder.Add(lstPurchase[0]);

                            InvH.UpdateTbt_PurchaseOrder(CommonUtil.ConvertToXml_Store<tbt_PurchaseOrder>(lstOrder));
                        }
                        #endregion

                        scope.Complete();

                        res.ResultData = true;
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

    }
}


