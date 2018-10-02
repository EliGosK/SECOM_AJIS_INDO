
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
        /// - Check user permission for screen IVS070.<br />
        /// - Check freezed data.<br />
        /// - Check started stock checking.<br />
        /// - Get inventory head office.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult IVS070_Authority(IVS070_ScreenParameter param)
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
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_TRANSFER_OFFICE_RECEIVE, FunctionID.C_FUNC_ID_OPERATE))
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

            return InitialScreenEnvironment<IVS070_ScreenParameter>("IVS070", param, res);
        }

        /// <summary>
        /// Initial screen.
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS070")]
        public ActionResult IVS070()
        {
            ViewBag.Total = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_INVENTORY, ScreenID.C_INV_SCREEN_ID_PRE_ELIMINATION, "lblTotalAmountOfTransferAsset");

            return View();
        }
        #endregion

        /// <summary>
        /// Validate before register.<br />
        /// - Check system suspending.<br />
        /// - Check user permission for screen IVS070.<br />
        /// - Check slip status.<br />
        /// - Check quantity.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS070_cmdConfirm(IVS070RegisterCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS070_ScreenParameter prm = GetScreenObject<IVS070_ScreenParameter>();
                string SlipNo = prm.SlipNo;
                string SourceOffice = prm.SourceOffice;
                string DestinationOffice = prm.DestinationOffice;
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.ResultData = MessageUtil.MessageList.MSG0049;
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_TRANSFER_OFFICE_RECEIVE, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    res.ResultData = MessageUtil.MessageList.MSG0053;
                    return Json(res);
                }

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<tbt_InventorySlip> dtTbt_InventorySlip = InvH.GetTbt_InventorySlip(SlipNo);

                tbt_InventorySlip InvenSlip = dtTbt_InventorySlip[0];
                //4.2.2

                if (InvenSlip.SlipStatus == InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4012, new string[] { SlipNo });
                    res.ResultData = MessageUtil.MessageList.MSG4012;
                    return Json(res);
                }

                //4.2.3
                foreach (IVS070INST i in Cond.StockInInstrument)
                {
                    doCheckTransferQty checkQty = new doCheckTransferQty();
                    checkQty.OfficeCode = DestinationOffice;
                    checkQty.LocationCode = InstrumentLocation.C_INV_LOC_TRANSFER;
                    checkQty.AreaCode = i.DestinationAreaCode;
                    checkQty.ShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    checkQty.InstrumentCode = i.InstrumentCode;
                    checkQty.TransferQty = Convert.ToInt32(i.TransferQty);
                    doCheckTransferQtyResult qtyResult = InvH.CheckTransferQty(checkQty);

                    if (qtyResult.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.InstrumentCode });
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.ResultData = i.row_id;
                        return Json(res);
                    }
                    else if (qtyResult.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.InstrumentCode });
                        res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                        res.ResultData = i.row_id;
                        return Json(res);
                    }
                }

                //4.3 Return to screen and show message
                //res.ResultData = prm.SlipNo;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Register receive between warehouse and branch.<br />
        /// - Register receive instrument.<br />
        /// - Update account transfer new/second hand/sample instrument.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS070_cmdConfirm_part2()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {//4.5.1 
                IVS070_ScreenParameter prm = GetScreenObject<IVS070_ScreenParameter>();
                string SlipNo = prm.SlipNo;
                
                using (TransactionScope scope = new TransactionScope())
                {

                    IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    InvH.RegisterReceiveInstrument(SlipNo, string.Empty);
                    //4.6.4
                    if (InvH.CheckNewInstrument(SlipNo) == 1)
                    {
                        List<doGroupNewInstrument> newInstGroup = InvH.GetGroupNewInstrument(SlipNo);
                        foreach (doGroupNewInstrument i in newInstGroup)
                        {
                            i.SourceLocationCode = InstrumentLocation.C_INV_LOC_TRANSFER;
                            i.SourceOfficeCode = i.DestinationOfficeCode;
                            InvH.UpdateAccountTransferNewInstrument(i, null);
                        }
                    }
                    //4.7
                    if (InvH.CheckSecondhandInstrument(SlipNo) == 1)
                    {
                        List<doGroupSecondhandInstrument> secondHandInst = InvH.GetGroupSecondhandInstrument(SlipNo);
                        foreach (doGroupSecondhandInstrument i in secondHandInst)
                        {
                            i.SourceLocationCode = InstrumentLocation.C_INV_LOC_TRANSFER;
                            i.SourceOfficeCode = i.DestinationOfficeCode;
                            InvH.UpdateAccountTransferSecondhandInstrument(i);
                        }
                    }
                    //4.8              
                    if (InvH.CheckSampleInstrument(SlipNo) == 1)
                    {
                        List<doGroupSampleInstrument> sampleInst = InvH.GetGroupSampleInstrument(SlipNo);
                        foreach (doGroupSampleInstrument i in sampleInst)
                        {
                            i.SourceLocationCode = InstrumentLocation.C_INV_LOC_TRANSFER;
                            i.SourceOfficeCode = i.DestinationOfficeCode;
                            InvH.UpdateAccountTransferSampleInstrument(i, null);
                        }
                    }

                    scope.Complete();
                } //end transaction scope
                res.ResultData = true;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Search inventory slip.<br />
        /// - Slip No. must have value.<br />
        /// - Check exist slip no.<br />
        /// - Check user can receive this slip.<br />
        /// - Check transfer type.<br />
        /// - Check slip status.
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public ActionResult IVS070_SearchInventorySlip(IVS070_RetrieveCond Cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS070_ScreenParameter prm = GetScreenObject<IVS070_ScreenParameter>();
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                ValidatorUtil.BuildErrorMessage(res, this, null);
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                List<doInventorySlip> dtInventorySlip = InvH.SearchInventorySlip(Cond.SlipNo);

                //2.3.3
                if (dtInventorySlip.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4001, null, new string[] { "SlipNo" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                else
                {
                    string officeCode = dtInventorySlip[0].DestinationOfficeCode;

                    List<UserBelongingData> officeDo = CommonUtil.dsTransData.dtUserBelongingData;

                    List<UserBelongingData> lst = (from x in officeDo
                     where x.OfficeCode == officeCode
                     select x).ToList();

                    if (lst.Count == 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4126, null, new string[] { "SlipNo" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }
               
                //2.3.1
                if (dtInventorySlip.Count > 0 && dtInventorySlip[0].TransferTypeCode != TransferType.C_INV_TRANSFERTYPE_TRANSFER_OFFICE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4018, null, new string[] { "SlipNo" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                //2.3.2
                if (dtInventorySlip.Count > 0 && dtInventorySlip[0].SlipStatus == InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4012, new string[] { Cond.SlipNo }, new string[] { "SlipNo" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }
                res.ResultData = dtInventorySlip[0];
                //CommonUtil.ConvertToXml<doInventorySlip>(dtInventorySlip, "Inventory\\IVS070", CommonUtil.GRID_EMPTY_TYPE.SEARCH);

                prm.DestinationOffice = dtInventorySlip[0].DestinationOfficeCode;
                prm.SourceOffice = dtInventorySlip[0].SourceOfficeCode;
                prm.SlipNo = Cond.SlipNo;
                
                UpdateScreenObject(prm);                
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Get config for instrument grid.
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS070_InstGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS070", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Search slip detail.
        /// </summary>
        /// <param name="SlipNo"></param>
        /// <returns></returns>
        public ActionResult IVS070_SearchInventorySlipDetail(string SlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS070_ScreenParameter prm = GetScreenObject<IVS070_ScreenParameter>();
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doInventorySlipDetail> dtInventorySlipDetail = InvH.SearchInventorySlipDetail(SlipNo);

                res.ResultData = CommonUtil.ConvertToXml<doInventorySlipDetail>(dtInventorySlipDetail, "Inventory\\IVS070", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                prm.lstInventoryDetail = dtInventorySlipDetail;
                UpdateScreenObject(prm);
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
        public ActionResult IVS070_initParam()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS070_ScreenParameter prm = GetScreenObject<IVS070_ScreenParameter>();
                prm.SourceOffice = null;
                prm.DestinationOffice = null;
                prm.SlipNo = null;
                prm.Memo = null;
                prm.lstInventoryDetail = new List<doInventorySlipDetail>();
                return Json(true);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

    }
}