


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
using SECOM_AJIS.DataEntity.Installation;

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
        public ActionResult IVS030_Authority(IVS030_ScreenParameter param)
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
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_RECEIVE_RETURN, FunctionID.C_FUNC_ID_OPERATE))
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

            return InitialScreenEnvironment<IVS030_ScreenParameter>("IVS030", param, res);
        }

        /// <summary>
        /// Initial screen
        /// </summary>
        /// <returns></returns>
        [Initialize("IVS030")]
        public ActionResult IVS030()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// Retrieve slip no. of return PJ unused instrument
        /// </summary>
        /// <param name="cond">Search condition object</param>
        /// <returns></returns>
        public ActionResult IVS030_RetrieveSlipData(IVS030SearchCond cond)
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

                IVS030_ScreenParameter param = GetScreenObject<IVS030_ScreenParameter>();
                if (param.ElemInstrument == null)
                    param.ElemInstrument = new List<IVS030INST>();

                param.IsError = false;

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                CommonUtil c = new CommonUtil();

                //3.2
                if (cond.SlipSelectType == IVS030SearchCond.SlipType.InstallationSlip)
                {
                    //3.2.1.1                    
                    List<doResultGetReturnSlip> lstInstallationSlip = InvH.GetReturnedSlip(cond.SlipNo);

                    //3.2.1.2
                    if (lstInstallationSlip.Count <= 0)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4094, null, new string[] { "InstallationReturnSlipNo" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        param.IsError = true;
                    }
                    else
                    {
                        res.ResultData = lstInstallationSlip[0];
                        param.ServiceTypeCode = lstInstallationSlip[0].ServiceTypeCode;
                        param.SlipNo = lstInstallationSlip[0].SlipNo;
                        param.SlipSelectType = IVS030_ScreenParameter.SlipType.InstallationSlip;

                        lstInstallationSlip[0].ContractCode = c.ConvertContractCode(lstInstallationSlip[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        param.ContractCode = lstInstallationSlip[0].ContractCode;

                        if (param.ServiceTypeCode == null)
                            param.ServiceTypeCode = "01";
                    }
                }
                else if (cond.SlipSelectType == IVS030SearchCond.SlipType.ProjectReturnSlip)
                {
                    List<tbt_InventorySlip> lstInven = InvH.GetTbt_InventorySlip(cond.SlipNo);

                    if (lstInven.Count == 0 || (lstInven.Count > 0 && lstInven[0].SlipStatus != InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER
                        || lstInven[0].DestinationLocationCode != InstrumentLocation.C_INV_LOC_RETURNED
                        || string.IsNullOrEmpty(lstInven[0].ProjectCode)))
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4095, null, new string[] { "ProjectReturnSlipNo" });
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        param.IsError = true;
                    }
                    else
                    {
                        CommonUtil.MappingObjectLanguage(lstInven[0]);
                        param.SlipNo = lstInven[0].SlipNo;
                        param.SlipSelectType = IVS030_ScreenParameter.SlipType.ProjectReturnSlip;

                        lstInven[0].ContractCode = c.ConvertContractCode(lstInven[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        param.ContractCode = lstInven[0].ContractCode;

                        res.ResultData = lstInven[0];
                    }

                    param.lstInventorySlip = lstInven; //Add by Jutarat A. on 30052013
                }

                UpdateScreenObject(param);

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Retrieve installation slip no.
        /// </summary>
        /// <param name="SlipNo">Slip no.</param>
        /// <returns></returns>
        public ActionResult IVS030_GetReturnInstrumentByInstallationSlip(string SlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS030_ScreenParameter param = GetScreenObject<IVS030_ScreenParameter>();

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doResultReturnInstrument> lstSlipDetail = InvH.GetReturnInstrumentByInstallationSlip(SlipNo);

                List<IVS030INST> nlst = new List<IVS030INST>();

                foreach (doResultReturnInstrument l in lstSlipDetail)
                {
                    nlst.Add(CommonUtil.CloneObject<doResultReturnInstrument, IVS030INST>(l));
                }

                param.ElemInstrument = nlst;

                UpdateScreenObject(param);

                res.ResultData = CommonUtil.ConvertToXml<doResultReturnInstrument>(lstSlipDetail, "inventory\\IVS030_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Get inventory slip detail
        /// </summary>
        /// <param name="SlipNo">Slip no.</param>
        /// <returns></returns>
        public ActionResult IVS030_GetTbt_InventorySlipDetailForView(string SlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS030_ScreenParameter param = GetScreenObject<IVS030_ScreenParameter>();

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<doTbt_InventorySlipDetailForView> lstSlipDetail = InvH.GetTbt_InventorySlipDetailForView(SlipNo);

                foreach (doTbt_InventorySlipDetailForView i in lstSlipDetail)
                    i.NotInstalledQty = i.TransferQty;

                List<IVS030INST> nlst = new List<IVS030INST>();

                foreach (doTbt_InventorySlipDetailForView l in lstSlipDetail)
                {
                    nlst.Add(CommonUtil.CloneObject<doTbt_InventorySlipDetailForView, IVS030INST>(l));
                }

                param.ElemInstrument = nlst;

                res.ResultData = CommonUtil.ConvertToXml<doTbt_InventorySlipDetailForView>(lstSlipDetail, "inventory\\IVS030_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Initial slip detail grid control
        /// </summary>
        /// <param name="SlipNo">Slip no.</param>
        /// <returns></returns>
        public ActionResult IVS030_GetHeaderSlipDetail(string SlipNo)
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS030_Instrument", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        /// <summary>
        /// Validate register receiving returned instrument
        /// </summary>
        /// <param name="Con">Register condition object</param>
        /// <returns></returns>
        public ActionResult IVS030_cmdConfirm(IVS030ConfirmCond Con)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS030_ScreenParameter prm = GetScreenObject<IVS030_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS030INST>();

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                ICommonHandler ComH = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                IInstallationHandler InstallH = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;

                //4.1                
                if (ComH.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_RECEIVE_RETURN, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //4.2
                if (InvH.CheckStartedStockChecking() == 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4003);
                    return Json(res);
                }

                prm.ApproveNo = Con.ApproveNo;
                prm.Memo = Con.Memo;

                //Check Memo
                if (!string.IsNullOrEmpty(prm.Memo) && prm.Memo.Replace(" ", "").Contains("\n\n\n\n"))
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4022, null, new string[] { "Detmemo" });
                    return Json(res);
                }

                //4.3
                //foreach (IVS030INST i in prm.ElemInstrument)
                //{
                //    //4.3.1
                //    if ((i.RemoveQty == null || i.RemoveQty <= 0) && (i.NotInstalledQty == null || i.NotInstalledQty <= 0))
                //    {
                //        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4127, null, null);
                //        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                //        return Json(res);
                //    }
                //}
                // 4.3.1
                if (prm.ElemInstrument.Sum(q => q.RemoveQty) <= 0 && prm.ElemInstrument.Sum(q => q.NotInstalledQty) <= 0)
                {
                    res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4127, null, null);
                    res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                    return Json(res);
                }


                //4.3.2
                if (prm.SlipSelectType == IVS030_ScreenParameter.SlipType.InstallationSlip)
                {
                    tbt_InstallationSlip doTbt_InstallationSlip = InstallH.GetTbt_InstallationSlipData(prm.SlipNo);

                    if (doTbt_InstallationSlip.SlipStatus == SlipStatus.C_SLIP_STATUS_RETURNED)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4097, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }
                else
                {
                    List<tbt_InventorySlip> doTbt_InventorySlip = InvH.GetTbt_InventorySlip(prm.SlipNo);

                    if (doTbt_InventorySlip.Count > 0 && doTbt_InventorySlip[0].SlipStatus == InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE)
                    {
                        res.AddErrorMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4097, null, null);
                        res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;
                        return Json(res);
                    }
                }

                res.ResultData = prm.ElemInstrument;

                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Register receiving returned instrument
        /// </summary>
        /// <param name="Con">Register condition object</param>
        /// <returns></returns>
        public ActionResult IVS030_cmdConfirm_Cont(IVS030ConfirmCond Con)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS030_ScreenParameter prm = GetScreenObject<IVS030_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS030INST>();

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler; 
                IInstallationHandler InstallH = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
                ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                if (comh.IsSystemSuspending())
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0049);
                    return Json(res);
                }
                if (!CheckUserPermission(ScreenID.C_INV_SCREEN_ID_RECEIVE_RETURN, FunctionID.C_FUNC_ID_OPERATE))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0053);
                    return Json(res);
                }

                //4.6
                using (TransactionScope scope = new TransactionScope())
                {
                    List<tbt_InventorySlip> doTbt_InventorySlipForUpdate = null;

                    //Comment by Jutarat A. on 11022013 (Move to check before 4.8.1)
                    ////4.7.1
                    //if (!string.IsNullOrEmpty(prm.ContractCode) && prm.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                    //{
                    //    //4.7.1.1
                    //    bool blnProcessStatus = InstallH.ReceiveReturnInstrument(prm.SlipNo, prm.office.OfficeCode);

                    //    //4.7.1.2
                    //    if (!blnProcessStatus)
                    //    {
                    //        //Rollback
                    //        return Json(res);
                    //    }

                    //    //4.7.1.3
                    //    res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4019);
                    //    return Json(res);
                    //}
                    //End Comment

                    //4.7.1
                    if (!string.IsNullOrEmpty(prm.ContractCode))
                    {
                        doTbt_InventorySlipForUpdate = InvH.GetTbt_InventorySlipForReceiveReturn(prm.SlipNo);
                    }
                    else
                    {
                        List<tbt_InventorySlip> doTbt_InventorySlip = InvH.GetTbt_InventorySlip(prm.SlipNo);

                        doTbt_InventorySlipForUpdate = new List<tbt_InventorySlip>();
                        doTbt_InventorySlipForUpdate.AddRange(doTbt_InventorySlip);
                    }

                    if (doTbt_InventorySlipForUpdate == null)
                        doTbt_InventorySlipForUpdate = new List<tbt_InventorySlip>();


                    //Add by Jutarat A. on 11022013 (Move to check before 4.8.1)
                    //4.7.2
                    if (!string.IsNullOrEmpty(prm.ContractCode) && prm.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE
                        && (doTbt_InventorySlipForUpdate != null && doTbt_InventorySlipForUpdate.Count > 0 
                            && doTbt_InventorySlipForUpdate[0].SourceLocationCode != InstrumentLocation.C_INV_LOC_WIP
                            && doTbt_InventorySlipForUpdate[0].SourceLocationCode != InstrumentLocation.C_INV_LOC_PARTIAL_OUT))
                    {
                        //4.7.2.1
                        bool blnProcessStatus = InstallH.ReceiveReturnInstrument(prm.SlipNo, prm.office.OfficeCode);

                        //4.7.2.2
                        if (!blnProcessStatus)
                        {
                            //Rollback
                            return Json(res);
                        }

                        //4.7.2.3
                        scope.Complete(); //Add by Jutarat A. on 11022013 (Commit)

                        res.ResultData = MessageUtil.GetMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4019);
                        return Json(res);
                    }
                    //End Add

                    //4.8.1
                    foreach (tbt_InventorySlip i in doTbt_InventorySlipForUpdate)
                    {
                        //4.8.2.1
                        if (i.DestinationLocationCode == InstrumentLocation.C_INV_LOC_ELIMINATION)
                        {
                            List<tbt_InventorySlip> lstUpdateSlip = InvH.GetTbt_InventorySlip(i.SlipNo);
                            if (lstUpdateSlip.Count > 0)
                            {
                                lstUpdateSlip[0].SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
                                if(!string.IsNullOrEmpty(prm.Memo))
                                    lstUpdateSlip[0].Memo = prm.Memo;
                                if (!string.IsNullOrEmpty(prm.ApproveNo))
                                    lstUpdateSlip[0].ApproveNo = prm.ApproveNo;
                                lstUpdateSlip[0].StockInDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                //Comment by Jutarat A. on 30052013 (Set at UpdateTbt_InventorySlip())
                                //lstUpdateSlip[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                //lstUpdateSlip[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                //End Comment

                                List<tbt_InventorySlip> doTbt_InventorySlip = InvH.UpdateTbt_InventorySlip(lstUpdateSlip);

                                if(doTbt_InventorySlip == null || doTbt_InventorySlip.Count <= 0)
                                {
                                    //Rollback
                                    return Json(res);
                                }
                            }
                        }
                        else
                        {
                            //4.8.2.2
                            bool blnProcessUpdate = InvH.RegisterReceiveInstrument(i.SlipNo, prm.Memo, prm.ApproveNo);
                            if (!blnProcessUpdate)
                            {
                                //Rollback
                                return Json(res);
                            }
                        }
                    }

                    string strSourceLocationCode = string.Empty;

                    //4.9
                    foreach (tbt_InventorySlip i in doTbt_InventorySlipForUpdate)
                    {
                        //4.9.1
                        if (i.DestinationLocationCode == InstrumentLocation.C_INV_LOC_ELIMINATION)
                        {
                            continue;
                        }

                        //4.9.2
                        if (i.SourceLocationCode == InstrumentLocation.C_INV_LOC_UNOPERATED_WIP 
                            || i.SourceLocationCode == InstrumentLocation.C_INV_LOC_WIP
                            || i.SourceLocationCode == InstrumentLocation.C_INV_LOC_PROJECT_WIP
                            || i.SourceLocationCode == InstrumentLocation.C_INV_LOC_PARTIAL_OUT // New WIP concept @ 24-Feb-2015
                        )
                            strSourceLocationCode = InstrumentLocation.C_INV_LOC_WAITING_RETURN;                        
                        else
                            strSourceLocationCode = InstrumentLocation.C_INV_LOC_RETURN_WIP;  

                        //4.9.3
                        if (InvH.CheckNewInstrument(i.SlipNo) == 1)
                        {
                            //4.9.3.1
                            List<doGroupNewInstrument> doGroupNewInstrument = InvH.GetGroupNewInstrument(i.SlipNo);
                                                        
                            foreach (doGroupNewInstrument groupNewIns in doGroupNewInstrument)
                            {
                                //4.9.3.2
                                groupNewIns.SourceLocationCode = strSourceLocationCode;
                                
                                //4.9.3.3
                                #region Monthly Price @ 2015
                                //decimal decMovingAveragePrice = InvH.CalculateMovingAveragePrice(groupNewIns);
                                var decMovingAveragePrice = InvH.GetMonthlyAveragePrice(groupNewIns.Instrumentcode, i.SlipIssueDate, InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                                #endregion

                                int? intReturnInprocess; // New WIP concept @ 24-Feb-2015
                                bool blnUpdate = InvH.UpdateAccountTransferNewInstrument(groupNewIns, Convert.ToDecimal(decMovingAveragePrice), out intReturnInprocess); // New WIP concept @ 24-Feb-2015

                                if (!blnUpdate)
                                {
                                    //Rollback
                                    return Json(res);
                                }

                                #region // New WIP concept @ 24-Feb-2015
                                if ((intReturnInprocess ?? 0) > 0)
                                {
                                    List<tbt_AccountInprocess> accountInProcessList = InvH.GetTbt_AccountInProcess(
                                        groupNewIns.SourceLocationCode
                                        , groupNewIns.ContractCode ?? groupNewIns.ProjectCode
                                        , groupNewIns.Instrumentcode
                                    );
                                    tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                                    accountStockMoving.SlipNo = i.SlipNo;
                                    accountStockMoving.TransferTypeCode = i.TransferTypeCode;
                                    accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                                    accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                                    accountStockMoving.SourceLocationCode = groupNewIns.SourceLocationCode;
                                    accountStockMoving.DestinationLocationCode = groupNewIns.DestinationLocationCode;
                                    accountStockMoving.InstrumentCode = groupNewIns.Instrumentcode;
                                    accountStockMoving.InstrumentQty = intReturnInprocess;
                                    if (accountInProcessList.Count != 0)
                                    {
                                        accountStockMoving.InstrumentPrice = accountInProcessList[0].MovingAveragePrice.Value;
                                    }
                                    accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                    accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                    List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                                    targetAccountStockMovingList.Add(accountStockMoving);
                                    List<tbt_AccountStockMoving> resultAccountStockMovingList = InvH.InsertAccountStockMoving(targetAccountStockMovingList);
                                    if (resultAccountStockMovingList.Count <= 0)
                                    {
                                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                                    } //end if/else 
                                }
                                #endregion
                            }
                        }

                        //4.9.4
                        if (InvH.CheckSecondhandInstrument(i.SlipNo) == 1)
                        {
                            //4.9.4.1
                            List<doGroupSecondhandInstrument> doGroupSecondhandInstrument = InvH.GetGroupSecondhandInstrument(i.SlipNo);

                            //4.9.4.2
                            foreach (doGroupSecondhandInstrument doGroupSecond in doGroupSecondhandInstrument)
                            {
                                doGroupSecond.SourceLocationCode = strSourceLocationCode;

                                bool blnUpdate = InvH.UpdateAccountTransferSecondhandInstrument(doGroupSecond);

                                if (!blnUpdate)
                                {
                                    //Rollback
                                    return Json(res);
                                }
                            }
                        }

                        //4.9.5
                        if(InvH.CheckSampleInstrument(i.SlipNo) == 1)
                        {
                            //4.9.5.1
                            List<doGroupSampleInstrument> doGroupSampleInstrument = InvH.GetGroupSampleInstrument(i.SlipNo);

                            //4.9.5.2
                            foreach(doGroupSampleInstrument dogroupSample in doGroupSampleInstrument)
                            {
                                dogroupSample.SourceLocationCode = strSourceLocationCode;

                                int? intReturnInprocess; // New WIP concept @ 24-Feb-2015
                                bool blnUpdate = InvH.UpdateAccountTransferSampleInstrument(dogroupSample, null, out intReturnInprocess); // New WIP concept @ 24-Feb-2015

                                if(!blnUpdate)
                                {
                                    //Rollback
                                    return Json(res);
                                }

                                #region // New WIP concept @ 24-Feb-2015
                                if ((intReturnInprocess ?? 0) > 0)
                                {
                                    tbt_AccountStockMoving accountStockMoving = new tbt_AccountStockMoving();
                                    accountStockMoving.SlipNo = i.SlipNo;
                                    accountStockMoving.TransferTypeCode = i.TransferTypeCode;
                                    accountStockMoving.SourceAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;
                                    accountStockMoving.DestinationAccountStockCode = InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK;
                                    accountStockMoving.SourceLocationCode = dogroupSample.SourceLocationCode;
                                    accountStockMoving.DestinationLocationCode = dogroupSample.DestinationLocationCode;
                                    accountStockMoving.InstrumentCode = dogroupSample.Instrumentcode;
                                    accountStockMoving.InstrumentQty = intReturnInprocess;
                                    accountStockMoving.InstrumentPrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                                    accountStockMoving.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    accountStockMoving.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                    accountStockMoving.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                    accountStockMoving.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                    List<tbt_AccountStockMoving> targetAccountStockMovingList = new List<tbt_AccountStockMoving>();
                                    targetAccountStockMovingList.Add(accountStockMoving);
                                    List<tbt_AccountStockMoving> resultAccountStockMovingList = InvH.InsertAccountStockMoving(targetAccountStockMovingList);
                                    if (resultAccountStockMovingList.Count <= 0)
                                    {
                                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING });
                                    } //end if/else 
                                }
                                #endregion
                            }
                        }
                    }

                    //4.10
                    if(prm.SlipSelectType == IVS030_ScreenParameter.SlipType.InstallationSlip)
                    {
                        bool blnProcessStatus = InstallH.ReceiveReturnInstrument(prm.SlipNo, prm.office.OfficeCode);

                        if(!blnProcessStatus)
                        {
                            //Rollback
                            return Json(res);
                        }
                    }

                    //Genereate Invenotry Slip Report
                    var srvInvDoc = ServiceContainer.GetService<IInventoryDocumentHandler>() as IInventoryDocumentHandler;
                    string invslipnolist = string.Join(",", doTbt_InventorySlipForUpdate.Select(d => d.SlipNo));
                    srvInvDoc.GenerateIVR210FilePath(invslipnolist, null, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                    res.ResultData = new {
                        SlipNo = prm.SlipNo,
                        Message = MessageUtil.GetMessage(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4019)
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
        /// Get error in register receiving returned instrument process
        /// </summary>
        /// <returns></returns>
        public ActionResult IVS030_GetElemError()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                IVS030_ScreenParameter prm = GetScreenObject<IVS030_ScreenParameter>();
                if (prm.ElemInstrument == null)
                    prm.ElemInstrument = new List<IVS030INST>();

                List<IVS030INST> lstError = (from c in prm.ElemInstrument where c.IsError == true select c).ToList<IVS030INST>();
                res.ResultData = lstError;
                return Json(res);
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex); return Json(res);
            }
        }

        /// <summary>
        /// Initial search slip result grid control
        /// </summary>
        /// <param name="SlipNo">Slip no.</param>
        /// <returns></returns>
        public ActionResult IVS030_InitialSearchResultGrid()
        {
            return Json(CommonUtil.ConvertToXml<object>(null, "inventory\\IVS030_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH));
        }

        public ActionResult IVS030_SearchSlip(doIVS030SearchCondition searchParam)
        {
            ObjectResultData res = new ObjectResultData();
            res.MessageType = MessageModel.MESSAGE_TYPE.WARNING;

            try
            {
                if (searchParam == null || CommonUtil.IsNullAllField(searchParam))
                {
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0006);
                }
                else
                {
                    IInventoryHandler service = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                    var lst = service.SearchReceiveSlip(searchParam);
                    CommonUtil.MappingObjectLanguage(lst);
                    res.ResultData = CommonUtil.ConvertToXml(lst, "inventory\\IVS030_SearchResult", CommonUtil.GRID_EMPTY_TYPE.SEARCH);
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
        /// Get document from share path.
        /// </summary>
        /// <param name="strInvSlipNo"></param>
        /// <returns></returns>
        public ActionResult IVS030_DownloadDocument(string strInstallationSlipNo)
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                var srvCommon = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                var lstDocs = srvCommon.GetTbt_DocumentList(strInstallationSlipNo, ReportID.C_INV_REPORT_ID_RETURNED, ConfigName.C_CONFIG_DOC_OCC_DEFAULT);

                if (lstDocs == null || lstDocs.Count <= 0)
                {
                    res.MessageType = MessageModel.MESSAGE_TYPE.INFORMATION;
                    res.AddErrorMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0112);
                    res.ResultData = null;
                }
                else
                {
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
        public ActionResult IVS030_DownloadPdfAndWriteLog(string strDocumentNo, string documentOCC, string strDocumentCode, string fileName)
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