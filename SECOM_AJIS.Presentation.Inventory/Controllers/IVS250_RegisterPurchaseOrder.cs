


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
        /// - Check system suspending.<br />
        /// - Check user permission for screen IVS250.<br />
        /// - Get inventory head office.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS250_Authority(IVS250_ScreenParameter param)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                GetScreenObject<IVS250_ScreenParameter>();
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (comh.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_REGISTER_PURCHASE_ORDER, FunctionID.C_FUNC_ID_OPERATE))
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

                param.m_VatTHB = Convert.ToDecimal(config[0].ConfigValue);
                param.office = IvHeadOffice[0];

                List<doSystemConfig> configWht = ComH.GetSystemConfig(ConfigName.C_WHT);
                param.m_WHT = Convert.ToDecimal(configWht[0].ConfigValue);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return InitialScreenEnvironment<IVS250_ScreenParameter>("IVS250", param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS250")]
        public ActionResult IVS250()
        {
            IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>(); //Add by Jutarat A. on 31072013
            ViewBag.Domestic = PurchaseOrderType.C_PURCHASE_ORDER_TYPE_DOMESTIC;
            ViewBag.CurrencyTHB = CurrencyType.C_CURRENCY_TYPE_THB;
            ViewBag.VatTHB = prm.m_VatTHB; //Add by Jutarat A. on 31072013
            ViewBag.WHT = prm.m_WHT;
            return View();
        }
        #endregion

        /// <summary>
        /// - Validate require field.<br />
        /// - Search supplier and return.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult SearchSupplier(doSupplierSearch Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>();
                if (prm.Supplier == null)
                    prm.Supplier = new DataEntity.Master.tbm_Supplier();
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { Cond });
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                if (!CommonUtil.IsNullOrEmpty(Cond.SupplierName) && !CommonUtil.IsNullOrEmpty(Cond.SupplierCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4017);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
                ISupplierMasterHandler SupH = ServiceContainer.GetService<ISupplierMasterHandler>() as ISupplierMasterHandler;
                DataEntity.Master.tbm_Supplier doSupplier = SupH.GetSupplier(Cond.SupplierCode, Cond.SupplierName);

                if (string.IsNullOrEmpty(doSupplier.SupplierCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }
                else
                {
                    prm.Supplier = doSupplier;
                }

                res.ResultData = prm.Supplier;
                UpdateScreenObject(prm);

                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
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
        public ActionResult IVS250_ValidateAddInst(doInstrument250 Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>();
                if (prm.lstInstrument == null)
                    prm.lstInstrument = new List<doInstrument250>();

                ValidatorUtil.BuildErrorMessage(res, this, null);
                if(res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                //add new 25/11/2015 adunyarich
                IInventoryHandler invH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                IInstrumentMasterHandler InsH = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                List<tbm_Instrument> lstInst = InsH.GetTbm_Instrument(Cond.InstrumentCode.Trim());

                if (lstInst.FirstOrDefault().LineUpTypeCode == "3")
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4149, new string[] { Cond.InstrumentCode });
                    return Json(res);
                }

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
                List<doInstrument250> exist = (from c in prm.lstInstrument where c.InstrumentCode == Cond.InstrumentCode select c).ToList<doInstrument250>();
                if (exist.Count > 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4038);
                    return Json(res);
                }
                //if (prm.lstInstrument.Count >= 15)
                //{
                //    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4021);
                //    return Json(res);
                //}

                Cond.Amount = (Cond.OrderQty ?? 0) * (Cond.UnitPrice ?? 0);

                prm.lstInstrument.Add(Cond);
                UpdateScreenObject(prm);
                res.ResultData = Cond;

                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Get config for Instrument table.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS250InstrumentGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS250", CommonUtil.GRID_EMPTY_TYPE.INSERT));
        }

        /// <summary>
        /// Calculate Vat.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS250_CalVat()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>();
                if (prm.lstInstrument == null)
                    prm.lstInstrument = new List<doInstrument250>();


                decimal decTotalVat = 0;
                foreach (doInstrument250 i in prm.lstInstrument)
                {
                    decTotalVat += i.Amount;
                }

                decimal vat = ((decTotalVat * prm.m_VatTHB) / 100);
                res.ResultData = vat.ToString("#,##0.00");

                //res.ResultData = ((decTotalVat * prm.m_VatTHB) / 100);

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Validate before register.<br />
        /// - Validate require field.<br />
        /// - Check system suspending.<br />
        /// - Check permission.<br />
        /// - Check purchase order type.<br />
        /// - Check memo.<br />
        /// - Check total amount.<br />
        /// - Check added instrument not empty.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS250_ValidateRegis(doSpecifyPOrder250_Domes Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>();
                if (prm.lstInstrument == null)
                    prm.lstInstrument = new List<doInstrument250>();

                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_REGISTER_PURCHASE_ORDER, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }
                if (Cond.PurchaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_DOMESTIC)
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
                    doSpecifyPOrder250 Porder = CommonUtil.CloneObject<doSpecifyPOrder250_Domes, doSpecifyPOrder250>(Cond);
                    ValidatorUtil.BuildErrorMessage(res, new object[] { Porder }, null);
                    if (res.IsError)
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }
                //14.4.2
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

                decimal totalAmtBeforeDisc = prm.lstInstrument.Sum(d => d.Amount);
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

                if (prm.lstInstrument.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4133);
                    return Json(res);
                }

                foreach (var i in prm.lstInstrument)
                {
                    IInstrumentMasterHandler InsH = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    List<tbm_Instrument> lstInst = InsH.GetTbm_Instrument(i.InstrumentCode);

                    if (lstInst[0].LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4140, new string[] { lstInst[0].InstrumentCode });
                        return Json(res);
                    }
                }

                foreach (var i in Cond.InstrumentData)
                {
                    if (CommonUtil.IsNullOrEmpty(i.Unit))
                    {
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4145, null, new string[] { i.UnitCtrlID });
                        return Json(res);
                    }
                }

                prm.Supplier.BankName = Cond.BankName;
                prm.Supplier.AccountNo = Cond.AccountNo;
                prm.Supplier.AccountName = Cond.AccountName;

                // get data from param to session
                prm.SpecifyPOrder250 = Cond;

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
        public ActionResult IVS250_ValidateRegis_2()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>();
                if (prm.lstInstrument == null)
                    prm.lstInstrument = new List<doInstrument250>();

                foreach (doInstrument250 i in prm.lstInstrument)
                {
                    if (i.UnitPrice == 0)
                    {
                        //res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4024);
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
        /// Register purchase order.<br />
        /// - Check system suspending.<br />
        /// - Check permission.<br />
        /// - Insert new purchase order.<br />
        /// - Generate report.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS250_cmdConfirm()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>();
                if (prm.lstInstrument == null)
                    prm.lstInstrument = new List<doInstrument250>();
                if (prm.SpecifyPOrder250 == null)
                    prm.SpecifyPOrder250 = new doSpecifyPOrder250();
                //Check Suspend
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_REGISTER_PURCHASE_ORDER, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                foreach (var i in prm.lstInstrument)
                {
                    IInstrumentMasterHandler InsH = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                    List<tbm_Instrument> lstInst = InsH.GetTbm_Instrument(i.InstrumentCode);

                    if (lstInst[0].LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4140, new string[] { lstInst[0].InstrumentCode });
                        return Json(res);
                    }
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                        string strPurchaseOrderNo = InvH.GeneratePurchaseOrderNo(prm.Supplier.RegionCode);

                        tbt_PurchaseOrder doPurchaseOrder = new tbt_PurchaseOrder();
                        doPurchaseOrder.PurchaseOrderNo = strPurchaseOrderNo;
                        doPurchaseOrder.PurhcaseOrderType = prm.SpecifyPOrder250.PurchaseOrderType;
                        doPurchaseOrder.PurchaseOrderStatus = PurchaseOrderStatus.C_PURCHASE_ORDER_STATUS_WAIT_TO_RECEIVE;
                        doPurchaseOrder.SupplierCode = prm.SpecifyPOrder250.SupplierCode;
                        doPurchaseOrder.TransportType = prm.SpecifyPOrder250.TransportType;
                        doPurchaseOrder.Currency = prm.SpecifyPOrder250.Currency;
                        doPurchaseOrder.BankName = prm.Supplier.BankName;
                        doPurchaseOrder.AccountNo = prm.Supplier.AccountNo;
                        doPurchaseOrder.AccountName = prm.Supplier.AccountName;
                        doPurchaseOrder.ShippingDate = prm.SpecifyPOrder250.AdjustDueDate;
                        doPurchaseOrder.Amount = prm.SpecifyPOrder250.TotalAmount;
                        if (prm.SpecifyPOrder250.PurchaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_DOMESTIC
                            || (prm.SpecifyPOrder250.PurchaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_SECOM && prm.SpecifyPOrder250.Currency == CurrencyType.C_CURRENCY_TYPE_THB))
                            doPurchaseOrder.Vat = prm.SpecifyPOrder250.Vat;
                        doPurchaseOrder.Memo = prm.SpecifyPOrder250.Memo;
                        doPurchaseOrder.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doPurchaseOrder.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doPurchaseOrder.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doPurchaseOrder.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doPurchaseOrder.WHT = prm.SpecifyPOrder250.WHT;
                        doPurchaseOrder.Discount = prm.SpecifyPOrder250.Discount;

                        List<tbt_PurchaseOrder> lst = new List<tbt_PurchaseOrder>();
                        lst.Add(doPurchaseOrder);
                        List<tbt_PurchaseOrder> dolstPurchaseOrder = InvH.InsertTbt_PurchaseOrder(lst);

                        if (dolstPurchaseOrder.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_PURCHASE });
                        }

                        List<tbt_PurchaseOrderDetail> lstDetail = new List<tbt_PurchaseOrderDetail>();
                        
                        //foreach (doInstrument250 i in prm.lstInstrument)
                        foreach (doInstrument250 i in prm.SpecifyPOrder250.InstrumentData) //Modify by Jutarat A. on 28102013
                        {
                            tbt_PurchaseOrderDetail OrderDetail = new tbt_PurchaseOrderDetail();
                            OrderDetail.PurchaseOrderNo = strPurchaseOrderNo;
                            OrderDetail.InstrumentCode = i.InstrumentCode;
                            OrderDetail.InstrumentName = i.InstrumentName; //Add by Jutarat A. on 28102013
                            OrderDetail.Memo = i.Memo; //Add by Jutarat A. on 28102013
                            OrderDetail.UnitPrice = i.UnitPrice;
                            OrderDetail.FirstOrderQty = i.OrderQty;
                            OrderDetail.ModifyOrderQty = null;
                            OrderDetail.ReceiveQty = 0;
                            OrderDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            OrderDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            OrderDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            OrderDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                            OrderDetail.Unit = i.Unit;
                            OrderDetail.OriginalUnitPrice = i.OriginalUnitPrice;
                            OrderDetail.Amount = i.Amount;
                            lstDetail.Add(OrderDetail);
                        }
                        List<tbt_PurchaseOrderDetail> doPurchaseOrderDetail = InvH.InsertTbt_PurchaseOrderDetail(lstDetail);

                        if (doPurchaseOrderDetail.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_PURCHASE_DETAIL });
                        }

                        //if (prm.SpecifyPOrder250.PurchaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_SECOM)
                        //{
                        //    IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                        //    string reportPath = handlerInventoryDocument.GenerateIVR190FilePath(strPurchaseOrderNo, prm.office.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                        //    prm.slipNo = strPurchaseOrderNo;
                        //    prm.reportFilePath = reportPath;
                        //}
                        //else if (prm.SpecifyPOrder250.PurchaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_DOMESTIC)
                        //{
                        //    IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                        //    string reportPath = handlerInventoryDocument.GenerateIVR191FilePath(strPurchaseOrderNo, prm.office.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                        //    prm.slipNo = strPurchaseOrderNo;
                        //    prm.reportFilePath = reportPath;
                        //}

                        IInventoryDocumentHandler handlerInventoryDocument = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                        string reportPath = handlerInventoryDocument.GenerateIVR192FilePath(strPurchaseOrderNo, prm.office.OfficeCode, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                        prm.slipNo = strPurchaseOrderNo;
                        prm.reportFilePath = reportPath;

                        scope.Complete();
                        res.ResultData = strPurchaseOrderNo;
                        return Json(res);
                    }
                    catch (Exception ex)
                    {
                        res.AddErrorMessage(ex);
                        return Json(res);
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Rempove selected instrument form list.
        /// </summary>
        /// <param name="InstCode"></param>
        /// <returns></returns>
        public ActionResult IVS250_RemoveInst(string InstCode)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>();
                if (prm.lstInstrument == null)
                    prm.lstInstrument = new List<doInstrument250>();
                // bool Exist = false;
                for (int i = 0; i < prm.lstInstrument.Count; i++)
                    if (InstCode == prm.lstInstrument[i].InstrumentCode)
                        prm.lstInstrument.RemoveAt(i);
                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Calculate amount.
        /// </summary>
        /// <param name="InstCode"></param>
        /// <param name="UnitPrice"></param>
        /// <param name="OrderQty"></param>
        /// <returns></returns>
        public ActionResult IVS250_CalculateAmount(string InstCode, decimal? UnitPrice, int? OrderQty, decimal? Amount)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>();
                if (prm.lstInstrument == null)
                    prm.lstInstrument = new List<doInstrument250>();

                decimal decTotalAmount = 0; //Add by Jutarat A. on 01082013
                string strAmount = string.Empty; //Add by Jutarat A. on 01082013
                decimal? decAmount = null;

                foreach (var inst in prm.lstInstrument)
                {
                    if (inst.InstrumentCode == InstCode)
                    {
                        inst.OrderQty = OrderQty;
                        inst.UnitPrice = (UnitPrice ?? 0);
                        if (CommonUtil.IsNullOrEmpty(Amount))
                        {
                            inst.Amount = (inst.OrderQty ?? 0) * (inst.UnitPrice ?? 0);
                        }
                        else
                        {
                            inst.Amount = (Amount ?? 0);
                        }
                        strAmount = inst.Amount_view;
                        decAmount = inst.Amount;
                    }

                    decTotalAmount += inst.Amount;
                }

                //Add by Jutarat A. on 01082013
                decimal decVatTotal = Math.Round(((decTotalAmount * prm.m_VatTHB) / 100), 2);

                doPOrderAmount result = new doPOrderAmount();
                result.Amount = strAmount;
                result.AmountDecimal = decAmount;
                result.TotalAmount = decTotalAmount.ToString("#,##0.00");
                result.Vat = decVatTotal.ToString("#,##0.00");
                res.ResultData = result;

                if (prm.SpecifyPOrder250 != null)
                {
                    prm.SpecifyPOrder250.TotalAmount = decTotalAmount;
                    prm.SpecifyPOrder250.Vat = decVatTotal;
                }
                //End Add

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Calculate total amount.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS250_CalTotalAmount()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>();
                if (prm.lstInstrument == null)
                    prm.lstInstrument = new List<doInstrument250>();
                decimal TotalAmount = 0;
                foreach (doInstrument250 i in prm.lstInstrument)
                {
                    TotalAmount += i.Amount;
                }

                res.ResultData = TotalAmount.ToString("#,##0.00");

                //res.ResultData = TotalAmount;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Reset screen parameter.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS250_resetParam()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>();
                IVS250_ScreenParameter nprm = (IVS250_ScreenParameter)ScreenParameter.ResetScreenParameter(prm);
                UpdateScreenObject(nprm);
                res.ResultData = true;

                //ScreenParameter param = GetScreenObject<object>() as ScreenParameter;
                //if (param != null)
                //{
                //    ScreenParameter nparam = ScreenParameter.ResetScreenParameter(param);
                //    if (nparam != null)
                //    {
                //        nparam.IsLoaded = false;
                //        UpdateScreenObject(nparam);
                //    }
                //}

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Get instrument data for search.
        /// </summary>
        /// <param name="InstrumentCode"></param>
        /// <returns></returns>
        public ActionResult IVS250_GetInstrumentDataForSearch(string InstrumentCode)
        {
            if (!CommonUtil.IsNullOrEmpty(InstrumentCode))
            {
                ObjectResultData res = new ObjectResultData();

                IInstrumentMasterHandler InstHand = ServiceContainer.GetService<IInstrumentMasterHandler>() as IInstrumentMasterHandler;
                doInstrumentSearchCondition Cond = new doInstrumentSearchCondition();
                Cond.InstrumentCode = InstrumentCode;
                Cond.ExpansionType = new List<string>() { ExpansionType.C_EXPANSION_TYPE_CHILD };
                Cond.InstrumentType = new List<string>() { InstrumentType.C_INST_TYPE_GENERAL, InstrumentType.C_INST_TYPE_MATERIAL };
                List<doInstrumentData> dtNewInstrument = InstHand.GetInstrumentDataForSearch(Cond);

                if (dtNewInstrument.Count > 0)
                    return Json(dtNewInstrument[0]);
                else
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4037);
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    return Json(res);
                }

            }
            return Json("");
        }

        /// <summary>
        /// Get instrument name.
        /// </summary>
        /// <param name="InstrumentCode"></param>
        /// <returns></returns>
        public ActionResult IVS250_getInstrumentName(string InstrumentCode)
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
                else if (lstInst[0].LineUpTypeCode == LineUpType.C_LINE_UP_TYPE_LOGICAL_DELETE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4140, new string[] { lstInst[0].InstrumentCode });
                }
                else if (invH.CheckInstrumentExpansion(lstInst[0].InstrumentCode))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4124, new string[] { lstInst[0].InstrumentCode });
                }
                else
                {
                    doInstrumentData dtNewInstrument = CommonUtil.CloneObject<tbm_Instrument, doInstrumentData>(lstInst[0]);
                    res.ResultData = dtNewInstrument;
                }

                return Json(res);
            }
            catch (Exception ex) { res.AddErrorMessage(ex); return Json(res); }
        }

        /// <summary>
        /// Check is report file exist.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS250_CheckExistFile()
        {
            try
            {
                IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>();
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
        public ActionResult IVS250_DownloadPdfAndWriteLog()
        {
            try
            {
                IVS250_ScreenParameter prm = GetScreenObject<IVS250_ScreenParameter>();
                string fileName = prm.reportFilePath;

                doDocumentDownloadLog doDownloadLog = new doDocumentDownloadLog()
                {
                    DocumentNo = prm.slipNo,
                    DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT,
                    DownloadDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    DownloadBy = CommonUtil.dsTransData.dtUserData.EmpNo
                };

                //if (prm.SpecifyPOrder250.PurchaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_SECOM)
                //{
                //    doDownloadLog.DocumentCode = ReportID.C_INV_REPORT_ID_PURCHASE_ORDER_CHN;
                //}
                //else if (prm.SpecifyPOrder250.PurchaseOrderType == PurchaseOrderType.C_PURCHASE_ORDER_TYPE_DOMESTIC)
                //{
                //    doDownloadLog.DocumentCode = ReportID.C_INV_REPORT_ID_PURCHASE_ORDER_DOM;
                //}

                doDownloadLog.DocumentCode = ReportID.C_INV_REPORT_ID_PURCHASE_ORDER;

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
        /// Generate Unit comboitem list 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public ActionResult IVS250_GetUnit(string id)
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