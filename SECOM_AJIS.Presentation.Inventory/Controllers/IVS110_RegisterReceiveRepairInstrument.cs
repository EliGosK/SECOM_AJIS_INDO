


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
        public ActionResult IVS110_Authority(IVS110_ScreenParameter param)
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
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_REPAIR_REQUEST_RECEIVE, FunctionID.C_FUNC_ID_OPERATE))
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

            return InitialScreenEnvironment<IVS110_ScreenParameter>("IVS110", param, res);
        }

        /// <summary>
        /// Initial Screen
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS110")]
        public ActionResult IVS110()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// Retrieve inventory slip information
        /// </summary>
        /// <param name="cond">Search condition object</param>
        /// <returns></returns>
        public ActionResult IVS110_RetrieveRequestInSlip(IVS110SearchCond cond)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                //2.1 Valid Cond
                ValidatorUtil.BuildErrorMessage(res, this, new object[] { cond });
                if (res.IsError)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }

                IVS110_ScreenParameter param = GetScreenObject<IVS110_ScreenParameter>();
                if (param.ElemInstrument == null)
                    param.ElemInstrument = new List<IVS110INST>();

                param.IsError = false;

                //2.2
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doInventorySlip> lstInvenSlip = InvH.SearchInventorySlip(cond.SlipNo);

                //2.3.1
                if (lstInvenSlip.Count <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4001, null, new string[] { "SlipNo" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    param.IsError = true;
                }

                //2.3.2
                if (lstInvenSlip.Count > 0 && lstInvenSlip[0].TransferTypeCode != TransferType.C_INV_TRANSFERTYPE_REPAIR_REQUEST)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4018, null, new string[] { "SlipNo" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    param.IsError = true;
                }

                //2.3.3
                if (lstInvenSlip.Count > 0 && lstInvenSlip[0].SlipStatus == InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4012, new string[] { cond.SlipNo }, new string[] { "SlipNo" });
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    param.IsError = true;
                }               

                if(param.IsError)
                    return Json(res);

                if (lstInvenSlip.Count > 0)
                {
                    doInventorySlip doInven = lstInvenSlip[0];
                    CommonUtil.MappingObjectLanguage(doInven);

                    param.SlipNo = doInven.SlipNo;

                    UpdateScreenObject(param);

                    res.ResultData = doInven;
                }

                return Json(res);
            }
            catch (Exception ex) 
            { 
                res.AddErrorMessage(ex); return Json(res); 
            }
        }

        /// <summary>
        /// Retrieve inventory slip detail information
        /// </summary>
        /// <param name="SlipNo">slip no.</param>
        /// <returns></returns>
        public ActionResult IVS110_RetrieveRequestSlipDetail(string SlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS110_ScreenParameter param = GetScreenObject<IVS110_ScreenParameter>();

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doInventorySlipDetail> lstSlipDetail = InvH.SearchInventorySlipDetail(SlipNo);
   
                CommonUtil.MappingObjectLanguage<doInventorySlipDetail>(lstSlipDetail);

                res.ResultData = CommonUtil.ConvertToXml<doInventorySlipDetail>(lstSlipDetail, "Inventory\\IVS110_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// <param name="SlipNo">slip no.</param>
        /// <returns></returns>
        public ActionResult IVS110_GetHeaderSlipDetail(string SlipNo)
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS110_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Validate Register repair request instrument detail
        /// </summary>
        /// <param name="Con">Register condition object</param>
        /// <returns></returns>
        public ActionResult IVS110_cmdConfirm(IVS110ConfirmCond Con)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS110_ScreenParameter prm = GetScreenObject<IVS110_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS110INST>();

                prm.ElemInstrument = Con.StockInInstrument;

                //4.1
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    res.ResultData = true;
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_REPAIR_REQUEST_RECEIVE, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    res.ResultData = true;
                    return Json(res);
                }

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                //4.2.1
                List<tbt_InventorySlip> lstInventorySlip = InvH.GetTbt_InventorySlip(prm.SlipNo);

                //4.2.2
                if (lstInventorySlip.Count > 0 && lstInventorySlip[0].SlipStatus == InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4012, new string[] { prm.SlipNo });
                    res.ResultData = true;
                    return Json(res);
                }

                //4.2.3
                foreach (IVS110INST i in prm.ElemInstrument)
                {
                    doCheckTransferQty chk = new doCheckTransferQty();
                    chk.OfficeCode = prm.office.OfficeCode;
                    chk.LocationCode = InstrumentLocation.C_INV_LOC_REPAIR_REQUEST;
                    chk.AreaCode = i.DestinationAreaCode;
                    chk.ShelfNo = i.DestinationShelfNo;
                    chk.InstrumentCode = i.InstrumentCode;
                    chk.TransferQty = i.TransferQty == null ? 0 : Convert.ToInt32(i.TransferQty);

                    doCheckTransferQtyResult TransferQtyResult = InvH.CheckTransferQty(chk);

                    //4.2.3 
                    if (TransferQtyResult.OverQtyFlag == null)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4009, new string[] { i.InstrumentCode });
                        res.ResultData = i.row_id;
                        i.IsError = true;
                        return Json(res);
                    }
                    else if (TransferQtyResult.OverQtyFlag == true)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4008, new string[] { i.InstrumentCode });
                        res.ResultData = i.row_id;
                        i.IsError = true;
                        return Json(res);
                    }
                }                

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Register repair request instrument detail
        /// </summary>
        /// <param name="Con">Register condition object</param>
        /// <returns></returns>
        public ActionResult IVS110_cmdConfirm_Cont(IVS110ConfirmCond Con)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS110_ScreenParameter prm = GetScreenObject<IVS110_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS110INST>();

                using (TransactionScope scope = new TransactionScope())
                {

                    IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                    #region Validating Slip Status
                    List<doInventorySlip> lstInvenSlip = InvH.SearchInventorySlip(prm.SlipNo);

                    if (lstInvenSlip.Count <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4001, null, new string[] { "SlipNo" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }

                    if (lstInvenSlip.Count > 0 && lstInvenSlip[0].TransferTypeCode != TransferType.C_INV_TRANSFERTYPE_REPAIR_REQUEST)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4018, null, new string[] { "SlipNo" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }

                    if (lstInvenSlip.Count > 0 && lstInvenSlip[0].SlipStatus == InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4012, new string[] { prm.SlipNo }, new string[] { "SlipNo" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                    #endregion

                    //4.5
                    InvH.RegisterReceiveInstrument(prm.SlipNo, string.Empty);

                    //4.6
                    if (InvH.CheckNewInstrument(prm.SlipNo) == 1)
                    {
                        //4.6.1
                        List<doGroupNewInstrument> lstGroupNewInstrument = InvH.GetGroupNewInstrument(prm.SlipNo);

                        //4.6.2
                        foreach (doGroupNewInstrument i in lstGroupNewInstrument)
                        {
                            i.SourceLocationCode = InstrumentLocation.C_INV_LOC_REPAIR_REQUEST;
                            i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_REPAIRING;

                            //4.6.3
                            InvH.UpdateAccountTransferNewInstrument(i, null);
                        }
                    }

                    //4.7
                    if (InvH.CheckSecondhandInstrument(prm.SlipNo) == 1)
                    {
                        //4.7.1
                        List<doGroupSecondhandInstrument> lstGroupSecondhandInstrument = InvH.GetGroupSecondhandInstrument(prm.SlipNo);

                        //4.7.2
                        foreach (doGroupSecondhandInstrument i in lstGroupSecondhandInstrument)
                        {
                            i.SourceLocationCode = InstrumentLocation.C_INV_LOC_REPAIR_REQUEST;
                            i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_REPAIRING;

                            //4.7.2.1
                            InvH.UpdateAccountTransferSecondhandInstrument(i);
                        }
                    }

                    //4.8
                    if (InvH.CheckSampleInstrument(prm.SlipNo) == 1)
                    {
                        //4.8.1
                        List<doGroupSampleInstrument> lstGroupSampleInstrument = InvH.GetGroupSampleInstrument(prm.SlipNo);

                        //4.8.2
                        foreach (doGroupSampleInstrument i in lstGroupSampleInstrument)
                        {
                            i.SourceLocationCode = InstrumentLocation.C_INV_LOC_REPAIR_REQUEST;
                            i.DestinationLocationCode = InstrumentLocation.C_INV_LOC_REPAIRING;

                            //4.8.2.1
                            InvH.UpdateAccountTransferSampleInstrument(i, null);
                        }
                    }

                    scope.Complete();
                }

                res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4019);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); 
                return Json(res);
            }
        }
    }
}