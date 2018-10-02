


using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.IO;
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
        /// - Check user permission for screen IVS040.<br />
        /// - Get inventory head office.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS260_Authority(IVS260_ScreenParameter param)
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
                
                if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_MAINTAIN_PURCHASE_ORDER, FunctionID.C_FUNC_ID_OPERATE))
                {
                    param.ViewOnlyMode = false;
                }                
                else if (CheckUserPermission(ScreenID.C_INV_SCREEN_ID_MAINTAIN_PURCHASE_ORDER, FunctionID.C_FUNC_ID_VIEW))
                {
                    param.ViewOnlyMode = true;
                }
                else
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
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doSystemConfig> config = ComH.GetSystemConfig(ConfigName.C_VAT_THB);
                param.office = IvHeadOffice[0];

                param.m_VatTHB = Convert.ToDecimal(config[0].ConfigValue); //Add by Jutarat A. on 31102013

                List<doSystemConfig> configWht = ComH.GetSystemConfig(ConfigName.C_WHT);
                param.m_WHT = Convert.ToDecimal(configWht[0].ConfigValue);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS260_ScreenParameter>("IVS260", param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS260")]
        public ActionResult IVS260()
        {
            IVS260_ScreenParameter prm = GetScreenObject<IVS260_ScreenParameter>(); //Add by Jutarat A. on 31102013

            ViewBag.C_PURCHASE_ORDER_TYPE_DOMESTIC = PurchaseOrderType.C_PURCHASE_ORDER_TYPE_DOMESTIC; //Add by Jutarat A. on 06112013 

            ViewBag.CancelPurchaseOrder = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INVENTORY, "IVS260", "lblBtnCancelPurchaseOrder");
            ViewBag.WaitToReceive = PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_WAIT_TO_RECEIVE;
            ViewBag.CompleteReceive = PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_COMPLETE_RECEIVE;

            ViewBag.CurrencyTHB = CurrencyType.C_CURRENCY_TYPE_THB; //Add by Jutarat A. on 31102013
            ViewBag.VatTHB = prm.m_VatTHB; //Add by Jutarat A. on 31102013
            ViewBag.WHT = prm.m_WHT;

            ViewBag.ViewOnlyMode = (prm.ViewOnlyMode ? "true" : "false");

            return View();
        }

        /// <summary>
        /// Get config for purchase order table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS260_intiPorderGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS260_PurchaseOrder", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Get config for instrument table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS260_intiInstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS260_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        #endregion

        /// <summary>
        /// Get purchase order status from screen parameter.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS260_GetPurchaseOrderStatus()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS260_ScreenParameter prm = GetScreenObject<IVS260_ScreenParameter>();
                if (prm.lstPOrderDetail == null)
                    prm.lstPOrderDetail = new List<doPurchaseOrderDetail>();

                if (prm.lstPOrderDetail.Count > 0)
                {
                    //Modify by Jutarat A. on 31102013
                    //res.ResultData = prm.lstPOrderDetail[0];
                    IVS260_MaintainPurchaseOrderData resultData = new IVS260_MaintainPurchaseOrderData();

                    string strPurchaseOrderNo = prm.lstPOrderDetail[0].PurchaseOrderNo;
                    List<doPurchaseOrder> lstPurchaseOrder = (from t in prm.lstPurchaseOrder
                                                              where t.PurchaseOrderNo == strPurchaseOrderNo
                                                              select t).ToList<doPurchaseOrder>();
                    if (lstPurchaseOrder != null && lstPurchaseOrder.Count > 0)
                    {
                        resultData.doPurchaseOrderData = lstPurchaseOrder[0];
                        resultData.doPOrderDetailData = prm.lstPOrderDetail[0];
                    }

                    res.ResultData = resultData;
                    //End Modify

                    prm.doPurchaseOrderData = resultData.doPurchaseOrderData; //Add by Jutarat A. on 11112013
                }

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Search purchase order by condition.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS260_SearchPurcahseOrder(doPurchaseOrderSearchCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS260_ScreenParameter prm = GetScreenObject<IVS260_ScreenParameter>();
                prm.lstPurchaseOrder = new List<doPurchaseOrder>();
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { Cond });
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }


                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doPurchaseOrder> lstPurchaseOrder = InvH.GetPurchaserOrderForMaintain(Cond);

                if (lstPurchaseOrder.Count <= 0)
                {
                    //res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
                else if (lstPurchaseOrder.Count > 1000)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4004);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

                prm.lstPurchaseOrder = lstPurchaseOrder;
                UpdateScreenObject(prm);
                res.ResultData = CommonUtil.ConvertToXml<doPurchaseOrder>(lstPurchaseOrder, "inventory\\IVS260_PurchaseOrder", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Get detail of purchase order.
        /// </summary>
        /// <param name="PurchaseOrderNo"></param>
        /// <returns></returns>
        public ActionResult IVS260_GetPurchaseOrderDetail(string PurchaseOrderNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS260_ScreenParameter prm = GetScreenObject<IVS260_ScreenParameter>();
                if (prm.lstPOrderDetail == null)
                    prm.lstPOrderDetail = new List<doPurchaseOrderDetail>();
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doPurchaseOrderDetail> lstPurchaseOrderDetail = InvH.GetPurchaseOrderDetailForMaintain(PurchaseOrderNo);

                foreach (doPurchaseOrderDetail data in lstPurchaseOrderDetail)
                {
                    data.IsShowRemove = false;
                }

                prm.lstPOrderDetail = lstPurchaseOrderDetail;
                UpdateScreenObject(prm);

                if (lstPurchaseOrderDetail.Count == 1 && string.IsNullOrEmpty(lstPurchaseOrderDetail[0].InstrumentCode))
                    res.ResultData = CommonUtil.ConvertToXml<doPurchaseOrderDetail>(new List<doPurchaseOrderDetail>(), "inventory\\IVS260_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                else
                    res.ResultData = CommonUtil.ConvertToXml<doPurchaseOrderDetail>(lstPurchaseOrderDetail, "inventory\\IVS260_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Cancel selected purchase order.<br />
        /// - Check exist purchase order.<br />
        /// - Delete Purchase Order from database.
        /// </summary>
        /// <param name="PurchaseOrderNo"></param>
        /// <returns></returns>
        public ActionResult IVS260_CancelPurchaseOrder(string PurchaseOrderNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    List<tbt_PurchaseOrder> lstPurchaseOrder = InvH.GetTbt_PurchaseOrder(PurchaseOrderNo);
                    if (lstPurchaseOrder.Count > 0)
                    {
                        if (lstPurchaseOrder[0].PurchaseOrderStatus != PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_WAIT_TO_RECEIVE)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4028);
                            return Json(res);
                        }
                    }

                    ////7.3.1
                    List<tbt_PurchaseOrderDetail> doPurchaseOrderDetail = InvH.DeleteTbt_PurchaseOrderDetail(PurchaseOrderNo);

                    if (doPurchaseOrderDetail.Count <= 0)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_PURCHASE_DETAIL });
                    }

                    ////7.3.3
                    List<tbt_PurchaseOrder> doPurchaseOrder = InvH.DeleteTbt_PurchaseOrder(PurchaseOrderNo);

                    if (doPurchaseOrder.Count <= 0)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_PURCHASE });
                    }

                    scope.Complete();
                    res.ResultData = true;

                }
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Validate before add instrument.<br />
        /// - Validate require field.<br />
        /// - Check quantity.<br />
        /// - Check instrument code.<br />
        /// - Check is exist instrument.<br />
        /// - Check cannot add more than 15 instruments.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS260_ValidateAddInst(doAddInstrument260 Cond) //Add by Jutarat A. on 01112013
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ValidatorUtil.BuildErrorMessage(res, this, null);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                if (Cond.OrderQty.Value <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4020);
                    return Json(res);
                }
                if ((!CommonUtil.IsNullOrEmpty(Cond.InstrumentCode)) && CommonUtil.IsNullOrEmpty(Cond.dtNewInstrument))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0082, new string[] { Cond.InstrumentCode }, new string[] { "InstrumentCode" });
                    return Json(res);
                }
                if (CommonUtil.IsNullOrEmpty(Cond.Unit))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4145, null, new string[] { "Unit" });
                    return Json(res);
                }
                List<doPurchaseOrderDetail> exist = (from c in Cond.InstrumentData where c.InstrumentCode == Cond.InstrumentCode select c).ToList<doPurchaseOrderDetail>();
                if (exist.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4038);
                    return Json(res);
                }
                //if (Cond.InstrumentData.Count >= 15)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4021);
                //    return Json(res);
                //}

                IInstrumentMasterHandler InsH = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<tbm_Instrument> lstInst = InsH.GetTbm_Instrument(Cond.InstrumentCode);

                if (lstInst[0].LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4140, new string[] { lstInst[0].InstrumentCode });
                    return Json(res);
                }

                Cond.Amount = (Cond.UnitPrice ?? 0) * (Cond.OrderQty ?? 0);

                res.ResultData = Cond;

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
                return Json(res);
            }
        }

        /// <summary>
        /// Validate before register.<br />
        /// - Check system suspending.<br />
        /// - Check user permission.<br />
        /// - Check quantity.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS260_cmdRegister(doSpecifyPOrder260 Cond) //(List<doPurchaseOrderDetail> Cond) //Modify by Jutarat A. on 04112013
        {
            ObjectResultData res = new ObjectResultData();
            try
            {   //Check Suspend
                IVS260_ScreenParameter prm = GetScreenObject<IVS260_ScreenParameter>();

                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_MAINTAIN_PURCHASE_ORDER, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                //if (Cond == null || Cond.Count <= 0)
                if (Cond.InstrumentData == null || Cond.InstrumentData.Count <= 0) //Modify by Jutarat A. on 04112013
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4006);
                    return Json(res);
                }

                //Add by Jutarat A. on 04112013
                if (Cond.PurchaseOrderType != PurchaseOrderType.C_PURCHASE_ORDER_TYPE_DOMESTIC)
                {
                    ValidatorUtil.BuildErrorMessage(res, this, null);
                    if (res.IsError)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }
                else
                {
                    doSpecifyPOrder260_Domes Porder = CommonUtil.CloneObject<doSpecifyPOrder260, doSpecifyPOrder260_Domes>(Cond);
                    ValidatorUtil.BuildErrorMessage(res, new object[] { Porder }, null);
                    if (res.IsError)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }

                if (CommonUtil.IsNullOrEmpty(Cond.Memo))
                    Cond.Memo = "";

                if (Cond.Memo.Replace(" ", "").Contains("\n\n\n\n"))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4022);
                    res.ResultData = "4022";
                    return Json(res);

                }

                if (Cond.TotalAmount > Convert.ToDecimal(CommonValue.C_MAX_AMOUNT))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4132);
                    return Json(res);
                }

                decimal totalAmtBeforeDisc = Cond.InstrumentData.Sum(d => d.Amount ?? 0);
                if (Cond.Discount > totalAmtBeforeDisc)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4144, null, new string[] { "Discount" });
                    return Json(res);
                }

                if ((Cond.TotalAmount - Cond.WHT + Cond.Vat) < 0 && Cond.TotalAmount > 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4114, null, new string[] { "Vat", "WHT" });
                    return Json(res);
                }
                //End Add

                //8.1.2
                #region R2
                bool chkModifyAllNull = true;
                foreach (doPurchaseOrderDetail i in Cond.InstrumentData) //Cond //Modify by Jutarat A. on 04112013
                {
                    if (i.ModifyOrderQty < i.ReceiveQty)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4025, null, new string[] { i.ModifyOrderQtyID });
                        return Json(res);
                    }

                    if (prm.doPurchaseOrderData != null && prm.doPurchaseOrderData.PurchaseOrderStatus != PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_WAIT_TO_RECEIVE) //Add by Jutarat A. on 04112013
                    {
                        if (i.ModifyOrderQty > i.FirstOrderQty)
                        {
                            res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4026, null, new string[] { i.ModifyOrderQtyID });
                            return Json(res);
                        }
                    }

                    //Add by Jutarat A. on 19112013
                    if (i.IsShowRemove == true && (i.ModifyOrderQty == null || i.ModifyOrderQty <= 0))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4020, null, new string[] { i.ModifyOrderQtyID });
                        return Json(res);
                    }
                    //End Add

                    //Modify by Jutarat A. on 19112013
                    /*if (i.ModifyOrderQty != null && i.RemainQty != 0)
                        chkModifyAllNull = false;*/
                    if (i.RemainQty != 0)
                    {
                        if (i.IsShowRemove == false)
                        {
                            var PorderDetailLst = (from c in prm.lstPOrderDetail where c.InstrumentCode == i.InstrumentCode select c).First();

                            if (i.InstrumentName != PorderDetailLst.InstrumentName
                                || i.Memo != PorderDetailLst.Memo
                                || i.UnitPrice != PorderDetailLst.UnitPrice
                                || i.ModifyOrderQty != PorderDetailLst.ModifyOrderQty)
                            {
                                chkModifyAllNull = false;
                            }
                        }
                        else
                        {
                            chkModifyAllNull = false;
                        }

                    }
                    //End Modify

                    if (string.IsNullOrEmpty(i.Unit))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4145, null, new string[] { i.UnitCtrlID });
                        return Json(res);
                    }
                }

                //Add by Jutarat A. on 19112013
                if (Cond.PurchaseOrderType != prm.doPurchaseOrderData.PurchaseOrderType
                    || Cond.TransportType != prm.doPurchaseOrderData.TransportType
                    || Cond.AdjustDueDate != prm.doPurchaseOrderData.ShippingDate
                    || Cond.Currency != prm.doPurchaseOrderData.Currency
                    || Cond.Memo != prm.doPurchaseOrderData.Memo
                    || Cond.Discount != prm.doPurchaseOrderData.Discount
                    || Cond.WHT != prm.doPurchaseOrderData.WHT
                    || Cond.Vat != prm.doPurchaseOrderData.Vat)
                {
                    chkModifyAllNull = false;
                }
                //End Add

                if (chkModifyAllNull)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4134, null, null);
                    return Json(res);
                }
                #endregion

                prm.lstInstrumentInGrid = Cond.InstrumentData; //Cond //Modify by Jutarat A. on 04112013
                prm.doSpecifyPOrder = Cond; //Add by Jutarat A. on 06112013

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
        /// Validate before register.<br />
        /// - Unit price more than 0.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS260_ValidateRegis() //Add by Jutarat A. on 06112013
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS260_ScreenParameter prm = GetScreenObject<IVS260_ScreenParameter>();
                if (prm.doSpecifyPOrder.InstrumentData == null)
                    prm.doSpecifyPOrder.InstrumentData = new List<doPurchaseOrderDetail>();

                foreach (doPurchaseOrderDetail i in prm.doSpecifyPOrder.InstrumentData)
                {
                    if (i.UnitPrice == 0)
                    {
                        return Json(res);
                    }

                    if (CommonUtil.IsNullOrEmpty(i.Unit))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4145, null, new string[] { i.UnitCtrlID });
                        return Json(res);
                    }
                }

                res.ResultData = true;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Confirm operation.<br />
        /// - Check system suspending.<br />
        /// - Check user permission.<br />
        /// - Check status.<br />
        /// - Update purchase order.<br />
        /// - Generate report.
        /// </summary>
        /// <param name="PurchaseOrderNo"></param>
        /// <returns></returns>
        public ActionResult IVS260_cmdConfirm(string PurchaseOrderNo)
        {
            ObjectResultData res = new ObjectResultData();

            try
            {
                //Check Suspend
                IVS260_ScreenParameter prm = GetScreenObject<IVS260_ScreenParameter>();
                if (prm.lstInstrumentInGrid == null)
                    prm.lstInstrumentInGrid = new List<doPurchaseOrderDetail>();

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doSystemConfig> config = ComH.GetSystemConfig(ConfigName.C_VAT_THB);
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_MAINTAIN_PURCHASE_ORDER, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                List<tbt_PurchaseOrder> lstPurchaseOrder = InvH.GetTbt_PurchaseOrder(PurchaseOrderNo);
                tbt_PurchaseOrder PurchaseOrder = new tbt_PurchaseOrder();
                if (lstPurchaseOrder.Count > 0)
                    PurchaseOrder = lstPurchaseOrder[0];

                doPurchaseOrder oldPO = new doPurchaseOrder();
                oldPO = (from c in prm.lstPurchaseOrder where c.PurchaseOrderNo == PurchaseOrderNo select c).First();

                //10.2.2
                if (DateTime.Compare(Convert.ToDateTime(oldPO.UpdateDate), Convert.ToDateTime(PurchaseOrder.UpdateDate)) != 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                    return Json(res);
                }
                //10.2.3
                if (PurchaseOrder.PurchaseOrderStatus == PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_COMPLETE_RECEIVE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4032);
                    return Json(res);
                }

                //10.3
                using (TransactionScope scope = new TransactionScope())
                {

                    //10.4.1
                    int iModifyQty = 0;
                    int iSumModifyQty = 0;
                    int iReceiveQty = 0;
                    int iSumReceiveQty = 0;
                    decimal dcVat = 0;
                    decimal dcAmount = 0;
                    #region R2
                    foreach (doPurchaseOrderDetail i in prm.lstInstrumentInGrid)
                    {
                        iModifyQty = (i.ModifyOrderQty ?? 0);
                        iReceiveQty = (i.ReceiveQty ?? 0);

                        if (i.ModifyOrderQty == null)
                            iModifyQty = (i.FirstOrderQty ?? 0);

                        iSumModifyQty += iModifyQty;
                        iSumReceiveQty += iReceiveQty;

                        dcAmount += (iModifyQty * (i.UnitPrice ?? 0));

                    }

                    if (iSumModifyQty == iSumReceiveQty)
                    {
                        PurchaseOrder.PurchaseOrderStatus = PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_COMPLETE_RECEIVE;
                    }
                    else if (iSumReceiveQty == 0)
                    {
                        PurchaseOrder.PurchaseOrderStatus = PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_WAIT_TO_RECEIVE;
                    }
                    else
                    {
                        PurchaseOrder.PurchaseOrderStatus = PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_PARTIAL_RECEIVE;
                    }

                    #endregion

                    //Modify by Jutarat A. on 11112013
                    //PurchaseOrder.Amount = dcAmount;
                    //if (PurchaseOrder.PurhcaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_DOMESTIC
                    //        || (PurchaseOrder.PurhcaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_SECOM && PurchaseOrder.Currency == CurrencyType.C_CURRENCY_TYPE_THB))
                    //{
                    //    PurchaseOrder.Vat = ((dcAmount * Convert.ToDecimal(config[0].ConfigValue)) / 100);
                    //}
                    PurchaseOrder.Amount = prm.doSpecifyPOrder.TotalAmount;
                    if (prm.doSpecifyPOrder.PurchaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_DOMESTIC
                            || (prm.doSpecifyPOrder.PurchaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_SECOM && prm.doSpecifyPOrder.Currency == CurrencyType.C_CURRENCY_TYPE_THB))
                    {
                        PurchaseOrder.Vat = prm.doSpecifyPOrder.Vat;
                    }
                    PurchaseOrder.Discount = prm.doSpecifyPOrder.Discount;
                    PurchaseOrder.WHT = prm.doSpecifyPOrder.WHT;
                    //End Modify

                    //Add by Jutarat A. on 11112013
                    PurchaseOrder.PurhcaseOrderType = prm.doSpecifyPOrder.PurchaseOrderType;
                    PurchaseOrder.TransportType = prm.doSpecifyPOrder.TransportType;
                    PurchaseOrder.ShippingDate = prm.doSpecifyPOrder.AdjustDueDate;
                    PurchaseOrder.Currency = prm.doSpecifyPOrder.Currency;
                    PurchaseOrder.Memo = prm.doSpecifyPOrder.Memo;
                    //End Add

                    PurchaseOrder.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    PurchaseOrder.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    List<tbt_PurchaseOrder> doPurchaseOrder = InvH.UpdateTbt_PurchaseOrder(CommonUtil.ConvertToXml_Store<tbt_PurchaseOrder>(lstPurchaseOrder));

                    if (doPurchaseOrder.Count <= 0)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_PURCHASE });
                    }

                    //10.4.3                     
                    List<tbt_PurchaseOrderDetail> originPorderDetail = InvH.GetTbt_PurchaseOrderDetail(PurchaseOrderNo, null);

                    //Modifyby Jutarat A. on 11112013
                    /*List<tbt_PurchaseOrderDetail> tmpPorderDetailForUpdate = null;
                    foreach (doPurchaseOrderDetail i in prm.lstInstrumentInGrid)
                    {
                        if (i.RemainQty != 0)
                        {
                            tmpPorderDetailForUpdate = new List<tbt_PurchaseOrderDetail>();
                            tbt_PurchaseOrderDetail PorderDetail = null;
                            var tmpPorderDetailLst = (from c in originPorderDetail where c.InstrumentCode == i.InstrumentCode select c);

                            if (tmpPorderDetailLst.Count() == 1 && i.ModifyOrderQty != null)
                            {
                                PorderDetail = tmpPorderDetailLst.First();

                                var PorderDetailLst = (from c in prm.lstPOrderDetail where c.InstrumentCode == i.InstrumentCode select c).First();

                                //10.2.5
                                if (DateTime.Compare(Convert.ToDateTime(PorderDetail.UpdateDate), Convert.ToDateTime(PorderDetailLst.UpdateDate)) != 0)
                                {
                                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                                    return Json(res);
                                }
                                PorderDetail.ModifyOrderQty = i.ModifyOrderQty;
                                PorderDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                PorderDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                //tmpPorderDetailForUpdate.Add(CommonUtil.CloneObject<doPurchaseOrderDetail, tbt_PurchaseOrderDetail>(i));
                                tmpPorderDetailForUpdate.Add(PorderDetail);
                                List<tbt_PurchaseOrderDetail> doPurchaseOrderDetail = InvH.UpdateTbt_PurchaseOrderDetail(tmpPorderDetailForUpdate);

                                if (doPurchaseOrderDetail.Count <= 0)
                                {
                                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_PURCHASE_DETAIL });
                                }
                            }
                        }
                    }*/
                    List<tbt_PurchaseOrderDetail> tmpPorderDetailForUpdate = new List<tbt_PurchaseOrderDetail>();
                    List<tbt_PurchaseOrderDetail> tmpPorderDetailForInsert = new List<tbt_PurchaseOrderDetail>();

                    foreach (doPurchaseOrderDetail i in prm.doSpecifyPOrder.InstrumentData)
                    {
                        if (i.RemainQty != 0)
                        {
                            tbt_PurchaseOrderDetail PorderDetail = null;
                            var tmpPorderDetailLst = (from c in originPorderDetail where c.InstrumentCode == i.InstrumentCode select c);

                            if (tmpPorderDetailLst.Count() == 1)
                            {
                                PorderDetail = tmpPorderDetailLst.First();
                                var PorderDetailLst = (from c in prm.lstPOrderDetail where c.InstrumentCode == i.InstrumentCode select c).First();

                                if (i.InstrumentName != PorderDetailLst.InstrumentName
                                    || i.Memo != PorderDetailLst.Memo
                                    || i.UnitPrice != PorderDetailLst.UnitPrice
                                    || i.ModifyOrderQty != PorderDetailLst.ModifyOrderQty
                                    || i.Unit != PorderDetailLst.Unit
                                    || i.DetailAmount != PorderDetailLst.DetailAmount)
                                {
                                    //10.2.5
                                    if (DateTime.Compare(Convert.ToDateTime(PorderDetail.UpdateDate), Convert.ToDateTime(PorderDetailLst.UpdateDate)) != 0)
                                    {
                                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                                        return Json(res);
                                    }

                                    PorderDetail.InstrumentName = i.InstrumentName;
                                    PorderDetail.OriginalUnitPrice = i.OriginalUnitPrice;
                                    PorderDetail.UnitPrice = i.UnitPrice;
                                    PorderDetail.Memo = i.Memo;

                                    if (i.ModifyOrderQty != PorderDetailLst.ModifyOrderQty)
                                    {
                                        if (prm.doPurchaseOrderData.PurchaseOrderStatus == PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_WAIT_TO_RECEIVE)
                                        {
                                            PorderDetail.FirstOrderQty = i.ModifyOrderQty;
                                            PorderDetail.ModifyOrderQty = null;
                                        }
                                        else
                                        {
                                            PorderDetail.ModifyOrderQty = i.ModifyOrderQty;
                                        }
                                    }

                                    PorderDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    PorderDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                    PorderDetail.Unit = i.Unit;
                                    PorderDetail.Amount = i.DetailAmount;

                                    tmpPorderDetailForUpdate.Add(PorderDetail);
                                }

                            }
                            else
                            {
                                PorderDetail = new tbt_PurchaseOrderDetail();
                                PorderDetail.PurchaseOrderNo = prm.doPurchaseOrderData.PurchaseOrderNo;
                                PorderDetail.InstrumentCode = i.InstrumentCode;
                                PorderDetail.InstrumentName = i.InstrumentName;
                                PorderDetail.OriginalUnitPrice = i.OriginalUnitPrice;
                                PorderDetail.UnitPrice = i.UnitPrice;
                                PorderDetail.Memo = i.Memo;
                                PorderDetail.FirstOrderQty = i.ModifyOrderQty;
                                PorderDetail.ReceiveQty = 0;

                                if (prm.doPurchaseOrderData != null && prm.doPurchaseOrderData.PurchaseOrderStatus != PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_WAIT_TO_RECEIVE)
                                    PorderDetail.ModifyOrderQty = i.ModifyOrderQty;

                                PorderDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                PorderDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                PorderDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                PorderDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                PorderDetail.Unit = i.Unit;
                                PorderDetail.Amount = i.DetailAmount;

                                tmpPorderDetailForInsert.Add(PorderDetail);
                            }

                        }
                    }

                    if (tmpPorderDetailForUpdate.Count > 0)
                    {
                        List<tbt_PurchaseOrderDetail> doPurchaseOrderDetail = InvH.UpdateTbt_PurchaseOrderDetail(tmpPorderDetailForUpdate);
                        if (doPurchaseOrderDetail == null || doPurchaseOrderDetail.Count == 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_PURCHASE_DETAIL });
                        }
                    }

                    if (tmpPorderDetailForInsert.Count > 0)
                    {
                        List<tbt_PurchaseOrderDetail> doPurchaseOrderDetail = InvH.InsertTbt_PurchaseOrderDetail(tmpPorderDetailForInsert);
                        if (doPurchaseOrderDetail == null || doPurchaseOrderDetail.Count == 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0110);
                        }
                    }
                    //End Modify

                    var invDocSrv = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                    //if (PurchaseOrder.PurhcaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_SECOM)
                    //{
                    //    //var strReportPath = invDocSrv.GenerateIVR190FilePath(PurchaseOrderNo, prm.office.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                    //    var strReportPath = invDocSrv.GenerateIVR192FilePath(PurchaseOrderNo, prm.office.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                    //    res.ResultData = new
                    //    {
                    //        PurchaseOrderNo = PurchaseOrderNo,
                    //        ReportID = ReportID.C_INV_REPORT_ID_PURCHASE_ORDER_CHN
                    //    };
                    //}
                    //else if (PurchaseOrder.PurhcaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_DOMESTIC)
                    //{
                    //    var strReportPath = invDocSrv.GenerateIVR191FilePath(PurchaseOrderNo, prm.office.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                    //    res.ResultData = new
                    //    {
                    //        PurchaseOrderNo = PurchaseOrderNo,
                    //        ReportID = ReportID.C_INV_REPORT_ID_PURCHASE_ORDER_DOM
                    //    };
                    //}

                    var strReportPath = invDocSrv.GenerateIVR192FilePath(PurchaseOrderNo, prm.office.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                    res.ResultData = new
                    {
                        PurchaseOrderNo = PurchaseOrderNo,
                        ReportID = ReportID.C_INV_REPORT_ID_PURCHASE_ORDER
                    };

                    scope.Complete();

                }
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
        public ActionResult IVS260_InitParam()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS260_ScreenParameter prm = GetScreenObject<IVS260_ScreenParameter>();
                prm.lstInstrumentInGrid = new List<doPurchaseOrderDetail>();
                prm.lstPOrderDetail = new List<doPurchaseOrderDetail>();
                prm.lstPurchaseOrder = new List<doPurchaseOrder>();

                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Download report.
        /// </summary>
        /// <param name="strPurchaseOrderNo"></param>
        /// <param name="strReportID"></param>
        /// <returns></returns>
        public ActionResult IVS260_DownloadDocument(string strPurchaseOrderNo, string strReportID)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                var lstDocs = srvCommon.GetTbt_DocumentList(strPurchaseOrderNo, strReportID).OrderByDescending(d => d.DocumentOCC).FirstOrDefault();

                if (lstDocs == null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                    res.ResultData = null;
                }
                else
                {
                    string path = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, lstDocs.FilePath);  //ReportUtil.GetGeneratedReportPath(lstDocs.FilePath);

                    if (System.IO.File.Exists(path) == true)
                    {
                        res.ResultData = lstDocs;
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
        /// Download report and write log.
        /// </summary>
        /// <param name="strDocumentNo"></param>
        /// <param name="documentOCC"></param>
        /// <param name="strDocumentCode"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult IVS260_DownloadPdfAndWriteLog(string strDocumentNo, string documentOCC, string strDocumentCode, string fileName)
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

        public ActionResult IVS260_PrepareDownloadDocument(string strPurchaseOrderNo, string strReportID)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS260_ScreenParameter prm = GetScreenObject<IVS260_ScreenParameter>();
                var srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                var lstDocs = srvCommon.GetTbt_DocumentList(strPurchaseOrderNo, strReportID).OrderByDescending(d => d.DocumentOCC).FirstOrDefault();

                if (lstDocs == null)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                    res.ResultData = false;
                }
                else
                {
                    string path = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, lstDocs.FilePath);  //ReportUtil.GetGeneratedReportPath(lstDocs.FilePath);

                    if (System.IO.File.Exists(path) == true)
                    {
                        prm.PreparedDownloadDocument = lstDocs;
                        res.ResultData = true;
                    }
                    else
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                        res.ResultData = false;
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

        public ActionResult IVS260_DownloadPreparedDocument()
        {
            try
            {
                IVS260_ScreenParameter prm = GetScreenObject<IVS260_ScreenParameter>();
                if (prm.PreparedDownloadDocument == null)
                {
                    return HttpNotFound();
                }

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = prm.PreparedDownloadDocument.DocumentNo,
                    DocumentCode = prm.PreparedDownloadDocument.DocumentCode,
                    DocumentOCC = prm.PreparedDownloadDocument.DocumentOCC,
                    DownloadDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    DownloadBy = CommonUtil.dsTransData.dtUserData.EmpNo
                };

                ILogHandler handlerLog = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                int isSuccess = handlerLog.WriteDocumentDownloadLog(doDownloadLog);

                IDocumentHandler handlerDoc = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                Stream reportStream = handlerDoc.GetDocumentReportFileStream(prm.PreparedDownloadDocument.FilePath);

                prm.PreparedDownloadDocument = null;

                return File(reportStream, "application/pdf");
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }

        /// <summary>
        /// Generate Unit comboitem list 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult IVS260_GetUnit(string id)
        {
            try
            {
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                var lstAll = comh.GetMiscTypeCodeListByFieldName(new List<string>() { MiscType.C_UNIT });
                var lstFilter = lstAll.ToList<doMiscTypeCode>();
                CommonUtil.MappingObjectLanguage<doMiscTypeCode>(lstFilter);

                ComboBoxModel cboModel = new ComboBoxModel();
                cboModel.SetList<doMiscTypeCode>(lstFilter, "ValueDisplay", "ValueCode", true, CommonUtil.eFirstElementType.Select);

                return Json(cboModel);

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