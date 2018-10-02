using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using CSI.WindsorHelper;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;


namespace SECOM_AJIS.DataEntity.Inventory
{
    // Using by Akat
    partial class InventoryHandler : BizIVDataEntities, IInventoryHandler
    {
        #region Private
        private int GetIntNullableVal(int? i, int defaultVal = 0)
        {
            return i.HasValue ? i.Value : defaultVal;
        }
        private decimal GetDecimalNullableVal(decimal? i, decimal defaultVal = 0)
        {
            return i.HasValue ? i.Value : defaultVal;
        }

        #endregion

        public List<doOffice> GetInventoryHeadOffice()
        {
            try
            {
                //Modified by Non A. 15/Feb/2012 : HeadOffice should always return only 1 doOffice.
                var lstTmp = base.GetInventoryHeadOffice(InventoryHeadOffice.C_OFFICLOGISTIC_HEAD);
                if (lstTmp != null && lstTmp.Capacity > 1)
                {
                    for (int i = 1; i < lstTmp.Count; i++)
                    {
                        lstTmp.RemoveAt(i);
                    }
                }
                CommonUtil.MappingObjectLanguage<doOffice>(lstTmp);
                return lstTmp;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<doOffice> GetInventorySrinakarinOffice()
        {
            try
            {
                return base.GetInventorySrinakarinOffice(OfficeCode.C_INV_OFFICE_SNR);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<doPurchaseOrder> GetPurchaserOrderForMaintain(doPurchaseOrderSearchCond cond)
        {
            try
            {
                List<doPurchaseOrder> lst = base.GetPurchaserOrderForMaintain(MiscType.C_PURCHASE_ORDER_STATUS, MiscType.C_TRANSPORT_TYPE, cond.PurchaseOrderNo, cond.PurchaseOrderStatus, cond.SupplierCode, cond.TransportType, cond.SupplierName, cond.POIssueDateFrom, cond.POIssueDateTo, cond.SearchExpectedDeliveryDateFrom, cond.SearchExpectedDeliveryDateTo, cond.SearchInstrumentCode);
                CommonUtil.MappingObjectLanguage<doPurchaseOrder>(lst);
                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public List<doPurchaseOrderDetail> GetPurchaseOrderDetailForRegisterStockIn(string strPurchaseOrderNo)
        {
            try
            {
                return base.GetPurchaseOrderDetailForRegisterStockIn(strPurchaseOrderNo, MiscType.C_CURRENCY_TYPE, MiscType.C_PURCHASE_ORDER_STATUS, MiscType.C_TRANSPORT_TYPE);
            }
            catch (Exception)
            {

                throw;
            }

        }
        public string GenerateInventorySlipNo(string officeCode, string SlipId)
        {
            //DateTime now = DateTime.Now;
            //string strYear = now.ToString("yyyy");
            //string strMonth = now.ToString("MM");
            //string strRunningNo = null;
            //List<tbs_InventoryRunningSlipNo> lstRunningNo = GetTbs_InventorySlipRunningNo(strMonth, strYear, officeCode, SlipId);
            //if (lstRunningNo.Count > 0 && (!CommonUtil.IsNullOrEmpty(lstRunningNo[0].RunningNo)))
            //{
            //    strRunningNo = (Convert.ToInt16(lstRunningNo[0].RunningNo) + 1).ToString("000");
            //    if (String.Compare(strRunningNo, SlipID.C_INV_SLIP_NO_MAXIMUM) > 0) // C_INV_SLIP_NO_MAXIMUM = 999;  // int.Parse(strRunningNo) > int.Parse(SlipID.C_INV_SLIP_NO_MAXIMUM)
            //    {
            //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4010);
            //    }
            //    else
            //    {
            //        List<tbs_InventoryRunningSlipNo> lst = UpdateTbs_InventorySlipRunningNo(strMonth, strYear, officeCode, SlipId, strRunningNo.ToString());
            //    }
            //}
            //else
            //{
            //    strRunningNo = SlipID.C_INV_SLIP_NO_MINIMUM;
            //    List<tbs_InventoryRunningSlipNo> lst = InsertTbs_InventorySlipRunningNo(strRunningNo, strMonth, strYear, officeCode, SlipId);

            //}
            //string NewSlipNo = officeCode + SlipId + strYear + strMonth + strRunningNo;
            //return NewSlipNo;

            try
            {
                var lstInvSlipNo = base.GenerateInventorySlipNo(SlipID.C_INV_SLIP_NO_MAXIMUM, SlipID.C_INV_SLIP_NO_MINIMUM, officeCode, SlipId);
                if (lstInvSlipNo.Count > 0)
                {
                    return lstInvSlipNo[0];
                }
                else
                {
                    throw new ApplicationException("Unable to generate new Inventory Slip No");
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message != null && (ex.InnerException.Message == MessageUtil.MessageList.MSG4010.ToString()))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4010);
                }
                else
                {
                    throw ex;
                }
            }
        }
        public List<doInventorySlipDetailList> GetInventorySlipDetailForSearch(string slipNo)
        {
            try
            {
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                List<doInventorySlipDetailList> lst = new List<doInventorySlipDetailList>();
                lst = base.GetInventorySlipDetailForSearch(slipNo, MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_REGISTER_ASSET, MiscType.C_INV_AREA, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);

                MiscTypeMappingList lstMiscMap = new MiscTypeMappingList();
                lstMiscMap.AddMiscType(lst.ToArray());
                hand.MiscTypeMappingList(lstMiscMap);
                CommonUtil.MappingObjectLanguage<doInventorySlipDetailList>(lst);



                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<doInventorySlipList> GetInventorySlipForSearch(doInventorySlipSearchCondition Cond)
        {
            try
            {
                List<doInventorySlipList> lst = base.GetInventorySlipForSearch(Cond.SlipNo, Cond.PurchaseOrderNo,
                       Cond.StockInFlag, Cond.DeliveryOrderNo, Cond.StockInDateFrom, Cond.StockInDateTo, Cond.RegisterAssetFlag, Cond.Memo, MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_REGISTER_ASSET, StockInType.C_INV_STOCKIN_TYPE_PURCHASE, StockInType.C_INV_STOCKIN_TYPE_SPECIAL);
                CommonUtil.MappingObjectLanguage<doInventorySlipList>(lst);

                //  List<doInventorySlip> lst = new List<doInventorySlip>();

                return lst;

            }
            catch (Exception)
            {

                throw;
            }

        }
        public List<doInventorySlipDetailList> GetInventorySlipDetailForRegister(string slipNo)
        {
            try
            {
                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doInventorySlipDetailList> lst = base.GetInventorySlipDetailForRegister(slipNo, MiscType.C_INV_STOCKIN_TYPE, MiscType.C_INV_REGISTER_ASSET, MiscType.C_INV_AREA, RegisterAssetFlag.C_INV_REGISTER_ASSET_UNREGISTER, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);

                MiscTypeMappingList lstMiscMap = new MiscTypeMappingList();
                lstMiscMap.AddMiscType(lst.ToArray());
                hand.MiscTypeMappingList(lstMiscMap);
                CommonUtil.MappingObjectLanguage<doInventorySlipDetailList>(lst);
                return lst;

            }
            catch (Exception) { throw; }


        }
        public bool UpdateInventoryCurrentDestination(List<tbt_InventoryCurrent> doInventoryCurrent)
        {
            try
            {
                //bool blnUpdate = false;

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<tbt_InventoryCurrent> dtInventoryCurrent = InvH.GetTbt_InventoryCurrent(doInventoryCurrent[0].OfficeCode, doInventoryCurrent[0].LocationCode,
                    doInventoryCurrent[0].AreaCode, doInventoryCurrent[0].ShelfNo, doInventoryCurrent[0].InstrumentCode);

                if (dtInventoryCurrent.Count > 0)
                {
                    dtInventoryCurrent = InvH.UpdateTbt_InventoryCurrent(doInventoryCurrent);
                    if (dtInventoryCurrent.Count < 0)
                        return false;
                }
                else
                {
                    dtInventoryCurrent = InvH.InsertTbt_InventoryCurrent(doInventoryCurrent);
                    if (dtInventoryCurrent.Count < 0)
                        return false;
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }


        }
        public List<tbt_AccountSampleInprocess> InsertTbt_AccountSampleInProcess(List<tbt_AccountSampleInprocess> lstDoAccount)
        {
            try
            {
                List<tbt_AccountSampleInprocess> lst = base.InsertTbt_AccountSampleInProcess(CommonUtil.ConvertToXml_Store<tbt_AccountSampleInprocess>(lstDoAccount));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_ACC_SAMPLE_INPROCESS,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountSampleInprocess> InsertTbt_AccountSampleInProcess_NoLog(List<tbt_AccountSampleInprocess> lstDoAccount)
        {
            try
            {
                List<tbt_AccountSampleInprocess> lst = base.InsertTbt_AccountSampleInProcess(CommonUtil.ConvertToXml_Store<tbt_AccountSampleInprocess>(lstDoAccount));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountSampleInprocess> UpdateTbt_AccountSampleInProcess(List<tbt_AccountSampleInprocess> lstDoAccount)
        {
            try
            {
                List<tbt_AccountSampleInprocess> lst = base.UpdateTbt_AccountSampleInProcess(CommonUtil.ConvertToXml_Store<tbt_AccountSampleInprocess>(lstDoAccount));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_ACC_SAMPLE_INPROCESS,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountSampleInprocess> UpdateTbt_AccountSampleInProcess_NoLog(List<tbt_AccountSampleInprocess> lstDoAccount)
        {
            try
            {
                List<tbt_AccountSampleInprocess> lst = base.UpdateTbt_AccountSampleInProcess(CommonUtil.ConvertToXml_Store<tbt_AccountSampleInprocess>(lstDoAccount));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountSampleInstock> InsertTbt_AccountSampleInStock(List<tbt_AccountSampleInstock> lstInStock)
        {
            try
            {
                List<tbt_AccountSampleInstock> lst = base.InsertTbt_AccountSampleInStock(CommonUtil.ConvertToXml_Store<tbt_AccountSampleInstock>(lstInStock));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_ACC_SAMPLE_INSTOCK,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountSampleInstock> InsertTbt_AccountSampleInStock_NoLog(List<tbt_AccountSampleInstock> lstInStock)
        {
            try
            {
                List<tbt_AccountSampleInstock> lst = base.InsertTbt_AccountSampleInStock(CommonUtil.ConvertToXml_Store<tbt_AccountSampleInstock>(lstInStock));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountSampleInstock> UpdateTbt_AccountSampleInStock(List<tbt_AccountSampleInstock> lstInStock)
        {
            try
            {
                List<tbt_AccountSampleInstock> lst = base.UpdateTbt_AccountSampleInStock(CommonUtil.ConvertToXml_Store<tbt_AccountSampleInstock>(lstInStock));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_ACC_SAMPLE_INSTOCK,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tbt_AccountSampleInstock> UpdateTbt_AccountSampleInStock_NoLog(List<tbt_AccountSampleInstock> lstInStock)
        {
            try
            {
                List<tbt_AccountSampleInstock> lst = base.UpdateTbt_AccountSampleInStock(CommonUtil.ConvertToXml_Store<tbt_AccountSampleInstock>(lstInStock));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override List<tbt_PurchaseOrderDetail> DeleteTbt_PurchaseOrderDetail(string purchaseOrderNo)
        {
            try
            {
                List<tbt_PurchaseOrderDetail> list = base.DeleteTbt_PurchaseOrderDetail(purchaseOrderNo);

                #region Log

                if (list.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Delete,
                        TableName = TableName.C_TBL_NAME_INV_PURCHASE_DETAIL,
                        TableData = CommonUtil.ConvertToXml(list)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_PurchaseOrderDetail> DeleteTbt_PurchaseOrderDetail_NoLog(string purchaseOrderNo)
        {
            try
            {
                List<tbt_PurchaseOrderDetail> list = base.DeleteTbt_PurchaseOrderDetail(purchaseOrderNo);

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<tbt_PurchaseOrder> DeleteTbt_PurchaseOrder(string purchaseOrderNo)
        {
            try
            {
                List<tbt_PurchaseOrder> list = base.DeleteTbt_PurchaseOrder(purchaseOrderNo);

                #region Log

                if (list.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Delete,
                        TableName = TableName.C_TBL_NAME_INV_PURCHASE,
                        TableData = CommonUtil.ConvertToXml(list)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_PurchaseOrder> DeleteTbt_PurchaseOrder_NoLog(string purchaseOrderNo)
        {
            try
            {
                List<tbt_PurchaseOrder> list = base.DeleteTbt_PurchaseOrder(purchaseOrderNo);

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Akat K. CHECKED 2012-03-02
        public bool UpdateAccountTransferSampleInstrument(doGroupSampleInstrument doGroupSample, decimal? decMovingAveragePrice)
        {
            #region // New WIP concept @ 24-Feb-2015
            int? tmp;
            return this.UpdateAccountTransferSampleInstrument(doGroupSample, decMovingAveragePrice, out tmp);
            #endregion
        }

        public bool UpdateAccountTransferSampleInstrument(doGroupSampleInstrument doGroupSample, decimal? decMovingAveragePrice, out int? intReturnInprocess)
        {
            try
            {
                intReturnInprocess = null; // New WIP concept @ 24-Feb-2015

                if (doGroupSample.DestinationLocationCode == InstrumentLocation.C_INV_LOC_RETURNED
                    && (
                        doGroupSample.SourceLocationCode == InstrumentLocation.C_INV_LOC_WIP
                        || doGroupSample.SourceLocationCode == InstrumentLocation.C_INV_LOC_UNOPERATED_WIP
                        || doGroupSample.SourceLocationCode == InstrumentLocation.C_INV_LOC_PROJECT_WIP
                        || doGroupSample.SourceLocationCode == InstrumentLocation.C_INV_LOC_PARTIAL_OUT     // - Add by Nontawat L. on 09-Jul-2014
                    )
                )
                {
                    doGroupSample.DestinationLocationCode = InstrumentLocation.C_INV_LOC_WAITING_RETURN;
                }

                if (InstrumentLocation.C_INV_LOC_PARTIAL_OUT.Equals(doGroupSample.SourceLocationCode) ||
                    //InstrumentLocation.C_INV_LOC_PROJECT_WIP.Equals(doGroupSample.SourceLocationCode) ||      //Edited by Phasupong 15/12/2015
                    InstrumentLocation.C_INV_LOC_WIP.Equals(doGroupSample.SourceLocationCode) ||
                    InstrumentLocation.C_INV_LOC_UNOPERATED_WIP.Equals(doGroupSample.SourceLocationCode))
                {
                    IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                    if (doGroupSample.TransferType == false)
                    {
                        doGroupSample.TransferQty = doGroupSample.TransferQty * -1;
                    }

                    string contractCode = null;
                    if (InstrumentLocation.C_INV_LOC_PROJECT_WIP.Equals(doGroupSample.SourceLocationCode))
                    {
                        contractCode = doGroupSample.ProjectCode;
                    }
                    else
                    {
                        contractCode = doGroupSample.ContractCode;
                    }

                    List<tbt_AccountSampleInprocess> lstTbt_AccountSampleInProcess = GetTbt_AccountSampleInProcess(doGroupSample.SourceLocationCode, contractCode, doGroupSample.Instrumentcode);
                    if (lstTbt_AccountSampleInProcess.Count > 0)
                    {
                        lstTbt_AccountSampleInProcess[0].LocationCode = doGroupSample.SourceLocationCode;
                        //lstTbt_AccountSampleInProcess[0].ContractCode = contractCode;     
                        lstTbt_AccountSampleInProcess[0].ContractCode = doGroupSample.ContractCode;     //Edited by Phasupong 15/12/2015
                        lstTbt_AccountSampleInProcess[0].InstrumentCode = doGroupSample.Instrumentcode;
                        lstTbt_AccountSampleInProcess[0].ProjectCode = doGroupSample.ProjectCode;
                        lstTbt_AccountSampleInProcess[0].InstrumentQty = GetIntNullableVal(lstTbt_AccountSampleInProcess[0].InstrumentQty) - GetIntNullableVal(doGroupSample.TransferQty);
                        lstTbt_AccountSampleInProcess[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        lstTbt_AccountSampleInProcess[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                        List<tbt_AccountSampleInprocess> lstDoAccount = new List<tbt_AccountSampleInprocess>();

                        lstDoAccount.Add(lstTbt_AccountSampleInProcess[0]);
                        List<tbt_AccountSampleInprocess> doTbt_AccountSampleInProcess = this.UpdateTbt_AccountSampleInProcess(lstDoAccount);
                        if (doTbt_AccountSampleInProcess.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_SAMPLE_INPROCESS });
                        }
                    }
                }
                #region // New WIP concept @ 24-Feb-2015
                else if (InstrumentLocation.C_INV_LOC_WAITING_RETURN.Equals(doGroupSample.SourceLocationCode))
                {
                    int transferQty = doGroupSample.TransferQty ?? 0;

                    if (transferQty > 0)
                    {
                        List<tbt_AccountSampleInprocess> oldInProcess = GetTbt_AccountSampleInProcess(doGroupSample.SourceLocationCode, doGroupSample.ContractCode, doGroupSample.Instrumentcode);
                        if (oldInProcess.Count > 0 && (oldInProcess[0].InstrumentQty ?? 0) > 0)
                        {
                            oldInProcess[0].LocationCode = doGroupSample.SourceLocationCode;
                            oldInProcess[0].ContractCode = doGroupSample.ContractCode;
                            oldInProcess[0].InstrumentCode = doGroupSample.Instrumentcode;

                            int inProcessQty = (oldInProcess[0].InstrumentQty ?? 0);
                            if (inProcessQty >= transferQty)
                            {
                                oldInProcess[0].InstrumentQty = inProcessQty - transferQty;
                                transferQty = 0;
                                intReturnInprocess = transferQty;
                            }
                            else
                            {
                                oldInProcess[0].InstrumentQty = 0;
                                transferQty -= inProcessQty;
                                intReturnInprocess = inProcessQty;
                            }

                            oldInProcess[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            oldInProcess[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            List<tbt_AccountSampleInprocess> NewInProcess = new List<tbt_AccountSampleInprocess>();
                            NewInProcess.Add(oldInProcess[0]);
                            List<tbt_AccountSampleInprocess> lstbt_AccountInprocess = this.UpdateTbt_AccountSampleInProcess(NewInProcess);
                            if (lstbt_AccountInprocess == null || lstbt_AccountInprocess.Count <= 0)
                            {
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INPROCESS });
                            }

                        }

                        if (transferQty > 0)
                        {
                            List<tbt_AccountSampleInstock> accountSampleInstockList = GetTbt_AccountSampleInStock(doGroupSample.Instrumentcode, doGroupSample.SourceLocationCode, doGroupSample.SourceOfficeCode);
                            if (accountSampleInstockList.Count > 0 && (accountSampleInstockList[0].InstrumentQty ?? 0) > 0)
                            {
                                accountSampleInstockList[0].OfficeCode = doGroupSample.SourceOfficeCode;
                                accountSampleInstockList[0].LocationCode = doGroupSample.SourceLocationCode;
                                accountSampleInstockList[0].InstrumentCode = doGroupSample.Instrumentcode;

                                int inStockQty = (accountSampleInstockList[0].InstrumentQty ?? 0);
                                accountSampleInstockList[0].InstrumentQty = inStockQty - transferQty;

                                accountSampleInstockList[0].MovingAveragePrice = decMovingAveragePrice;
                                accountSampleInstockList[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                accountSampleInstockList[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                List<tbt_AccountSampleInstock> newAccountSampleInstockList = new List<tbt_AccountSampleInstock>();
                                newAccountSampleInstockList.Add(accountSampleInstockList[0]);
                                List<tbt_AccountSampleInstock> resultUpdate = UpdateTbt_AccountSampleInStock(newAccountSampleInstockList);

                                if (resultUpdate == null || resultUpdate.Count <= 0)
                                {
                                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTOCK });
                                }
                            }
                        }
                    }
                }
                #endregion
                //else if (doGroupSample.ObjectID != ScreenID.C_INV_SCREEN_ID_STOCKIN_ASSET)
                else if (doGroupSample.ObjectID != ScreenID.C_INV_SCREEN_ID_STOCKIN) //Monthly Price @ 2015
                {
                    List<tbt_AccountSampleInstock> lstAccInStock = GetTbt_AccountSampleInStock(doGroupSample.Instrumentcode, doGroupSample.SourceLocationCode, doGroupSample.SourceOfficeCode);
                    if (lstAccInStock.Count > 0)
                    {
                        lstAccInStock[0].InstrumentQty = GetIntNullableVal(lstAccInStock[0].InstrumentQty) - GetIntNullableVal(doGroupSample.TransferQty);
                        lstAccInStock[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        lstAccInStock[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        List<tbt_AccountSampleInstock> newLstInstock = new List<tbt_AccountSampleInstock>(); newLstInstock.Add(lstAccInStock[0]);
                        List<tbt_AccountSampleInstock> doTbt_AccountSampleInstock = this.UpdateTbt_AccountSampleInStock(newLstInstock);
                        if (doTbt_AccountSampleInstock.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_SAMPLE_INSTOCK });
                        }
                    }
                    else
                    {
                        tbt_AccountSampleInstock doTbt_AccountSampleInStock = new tbt_AccountSampleInstock();
                        doTbt_AccountSampleInStock.OfficeCode = doGroupSample.SourceOfficeCode;
                        doTbt_AccountSampleInStock.LocationCode = doGroupSample.SourceLocationCode;
                        doTbt_AccountSampleInStock.InstrumentCode = doGroupSample.Instrumentcode;
                        doTbt_AccountSampleInStock.InstrumentQty = GetIntNullableVal(doTbt_AccountSampleInStock.InstrumentQty) - doGroupSample.TransferQty;
                        doTbt_AccountSampleInStock.MovingAveragePrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                        doTbt_AccountSampleInStock.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doTbt_AccountSampleInStock.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doTbt_AccountSampleInStock.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doTbt_AccountSampleInStock.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        List<tbt_AccountSampleInstock> newLstInstock = new List<tbt_AccountSampleInstock>(); newLstInstock.Add(doTbt_AccountSampleInStock);
                        List<tbt_AccountSampleInstock> doTbt_AccountSampleInstock = this.InsertTbt_AccountSampleInStock(newLstInstock);
                        if (doTbt_AccountSampleInstock.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_SAMPLE_INSTOCK });
                        }
                    }
                }

                if (InstrumentLocation.C_INV_LOC_ELIMINATION.Equals(doGroupSample.DestinationLocationCode) ||
                    InstrumentLocation.C_INV_LOC_SPECIAL.Equals(doGroupSample.DestinationLocationCode) ||
                    InstrumentLocation.C_INV_LOC_LOSS.Equals(doGroupSample.DestinationLocationCode) ||
                    InstrumentLocation.C_INV_LOC_SOLD.Equals(doGroupSample.DestinationLocationCode))
                {
                    return true;
                }

                //2.1
                if (InstrumentLocation.C_INV_LOC_PARTIAL_OUT.Equals(doGroupSample.DestinationLocationCode) ||
                        //InstrumentLocation.C_INV_LOC_PROJECT_WIP.Equals(doGroupSample.DestinationLocationCode) ||     //Edited by Phasupong 15/12/2015
                        InstrumentLocation.C_INV_LOC_WIP.Equals(doGroupSample.DestinationLocationCode) ||
                        InstrumentLocation.C_INV_LOC_UNOPERATED_WIP.Equals(doGroupSample.DestinationLocationCode) ||
                        InstrumentLocation.C_INV_LOC_WAITING_RETURN.Equals(doGroupSample.DestinationLocationCode) // New WIP concept @ 24-Feb-2015
                )
                {
                    string contractCode = null;
                    if (InstrumentLocation.C_INV_LOC_PROJECT_WIP.Equals(doGroupSample.DestinationLocationCode))
                    {
                        contractCode = doGroupSample.ProjectCode;
                    }
                    else
                    {
                        contractCode = doGroupSample.ContractCode;
                    }

                    List<tbt_AccountSampleInprocess> doTbt_AccountSampleInProcess = GetTbt_AccountSampleInProcess(doGroupSample.DestinationLocationCode, contractCode, doGroupSample.Instrumentcode);
                    if (doTbt_AccountSampleInProcess.Count <= 0)
                    {
                        tbt_AccountSampleInprocess accountSample = new tbt_AccountSampleInprocess();
                        accountSample.LocationCode = doGroupSample.DestinationLocationCode;
                        //accountSample.ContractCode = contractCode;        //edited by Phasupong 15/12/2015
                        accountSample.ContractCode = doGroupSample.ContractCode;        //edited by Phasupong 15/12/2015
                        accountSample.InstrumentCode = doGroupSample.Instrumentcode;
                        accountSample.ProjectCode = doGroupSample.ProjectCode;
                        accountSample.InstrumentQty = doGroupSample.TransferQty;
                        accountSample.MovingAveragePrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                        accountSample.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        accountSample.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        accountSample.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        accountSample.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountSampleInprocess> NewAccInProcess = new List<tbt_AccountSampleInprocess>();
                        NewAccInProcess.Add(accountSample);
                        List<tbt_AccountSampleInprocess> lstInsertAccSample = this.InsertTbt_AccountSampleInProcess(NewAccInProcess);

                        if (lstInsertAccSample.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_SAMPLE_INPROCESS });
                        }
                    }
                    else
                    {
                        doTbt_AccountSampleInProcess[0].ProjectCode = doGroupSample.ProjectCode;
                        doTbt_AccountSampleInProcess[0].InstrumentQty = GetIntNullableVal(doTbt_AccountSampleInProcess[0].InstrumentQty) + GetIntNullableVal(doGroupSample.TransferQty);
                        doTbt_AccountSampleInProcess[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doTbt_AccountSampleInProcess[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        List<tbt_AccountSampleInprocess> NewAccInProcess = new List<tbt_AccountSampleInprocess>();
                        NewAccInProcess.Add(doTbt_AccountSampleInProcess[0]);
                        List<tbt_AccountSampleInprocess> lstInsertAccSample = this.UpdateTbt_AccountSampleInProcess(NewAccInProcess);

                        if (lstInsertAccSample.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_SAMPLE_INPROCESS });
                        }
                    }
                }

                else if (!CommonUtil.IsNullOrEmpty(doGroupSample.LotNo))
                {
                    List<tbt_AccountInstalled> accountInstallList = GetTbt_AccountInstalled(doGroupSample.DestinationOfficeCode, doGroupSample.DestinationLocationCode, doGroupSample.Instrumentcode, doGroupSample.LotNo);
                    if (accountInstallList.Count <= 0)
                    {
                        tbt_AccountInstalled accountInstall = new tbt_AccountInstalled();
                        accountInstall.OfficeCode = doGroupSample.DestinationOfficeCode;
                        accountInstall.LocationCode = doGroupSample.DestinationLocationCode;
                        accountInstall.LotNo = doGroupSample.LotNo;
                        accountInstall.InstrumentCode = doGroupSample.Instrumentcode;
                        accountInstall.InstrumentQty = doGroupSample.TransferQty;
                        accountInstall.AccquisitionCost = decMovingAveragePrice;
                        accountInstall.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        accountInstall.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        accountInstall.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        accountInstall.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        List<tbt_AccountInstalled> newAccountInstall = new List<tbt_AccountInstalled>();
                        newAccountInstall.Add(accountInstall);
                        List<tbt_AccountInstalled> resultInsert = InsertTbt_AccountInstalled(newAccountInstall);
                        if (resultInsert.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }
                    else
                    {
                        accountInstallList[0].InstrumentQty = GetIntNullableVal(accountInstallList[0].InstrumentQty) + GetIntNullableVal(doGroupSample.TransferQty);
                        accountInstallList[0].AccquisitionCost = decMovingAveragePrice;
                        accountInstallList[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        accountInstallList[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        List<tbt_AccountInstalled> updateAccountInstall = new List<tbt_AccountInstalled>();
                        updateAccountInstall.Add(accountInstallList[0]);
                        List<tbt_AccountInstalled> resultUpdate = UpdateTbt_AccountInstalled(updateAccountInstall);
                        if (resultUpdate.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }
                }

                //2.3
                else
                {
                    List<tbt_AccountSampleInstock> doTbt_AccountSampleInStock = GetTbt_AccountSampleInStock(doGroupSample.Instrumentcode, doGroupSample.DestinationLocationCode, doGroupSample.DestinationOfficeCode);
                    if (doTbt_AccountSampleInStock.Count <= 0)
                    {
                        tbt_AccountSampleInstock Newtbt_AccountSampleInstock = new tbt_AccountSampleInstock();

                        Newtbt_AccountSampleInstock.OfficeCode = doGroupSample.DestinationOfficeCode;
                        Newtbt_AccountSampleInstock.LocationCode = doGroupSample.DestinationLocationCode;
                        Newtbt_AccountSampleInstock.InstrumentCode = doGroupSample.Instrumentcode;
                        Newtbt_AccountSampleInstock.InstrumentQty = doGroupSample.TransferQty;
                        Newtbt_AccountSampleInstock.MovingAveragePrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                        Newtbt_AccountSampleInstock.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        Newtbt_AccountSampleInstock.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        Newtbt_AccountSampleInstock.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        Newtbt_AccountSampleInstock.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountSampleInstock> NewLstAccSampleInStock = new List<tbt_AccountSampleInstock>();
                        NewLstAccSampleInStock.Add(Newtbt_AccountSampleInstock);
                        List<tbt_AccountSampleInstock> lsttbt_AccountSampleInstock = this.InsertTbt_AccountSampleInStock(NewLstAccSampleInStock);
                        if (lsttbt_AccountSampleInstock.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_SAMPLE_INSTOCK });
                        }
                    }
                    else
                    {
                        doTbt_AccountSampleInStock[0].OfficeCode = doGroupSample.DestinationOfficeCode;
                        doTbt_AccountSampleInStock[0].LocationCode = doGroupSample.DestinationLocationCode;
                        doTbt_AccountSampleInStock[0].InstrumentCode = doGroupSample.Instrumentcode;
                        doTbt_AccountSampleInStock[0].InstrumentQty = GetIntNullableVal(doTbt_AccountSampleInStock[0].InstrumentQty) + GetIntNullableVal(doGroupSample.TransferQty);
                        doTbt_AccountSampleInStock[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doTbt_AccountSampleInStock[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountSampleInstock> newLstSampleStock = new List<tbt_AccountSampleInstock>();
                        newLstSampleStock.Add(doTbt_AccountSampleInStock[0]);
                        List<tbt_AccountSampleInstock> lsttbt_AccountSampleInstock = this.UpdateTbt_AccountSampleInStock(newLstSampleStock);
                        if (lsttbt_AccountSampleInstock.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_SAMPLE_INSTOCK });
                        }
                    }
                }

                if (!CommonUtil.IsNullOrEmpty(doGroupSample.LotNo))
                {
                    UpdateMovingAveragePriceForInstalled(decMovingAveragePrice,
                                                        doGroupSample.Instrumentcode,
                                                        doGroupSample.LotNo,
                                                        ConfigName.C_CONFIG_SCRAP_VALUE,
                                                        CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                                        CommonUtil.dsTransData.dtUserData.EmpNo,
                                                        CommonUtil.dsTransData.dtOperationData.GUID,
                                                        CommonUtil.dsTransData.dtTransHeader.ScreenID);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Obsolete("use GetMonthlyAveragePrice() instead", true)]
        public decimal CalculateMovingAveragePrice(doGroupNewInstrument doGroupNew)
        {
            try
            {
                decimal DecTransferMovingAveragePrice = doGroupNew.UnitPrice;
                decimal decMovingAveragePrice = 0;
                List<tbt_AccountInstock> doTbt_AccountInStock = new List<tbt_AccountInstock>();
                List<doCalPriceCondition> lstCalPriceCond = new List<doCalPriceCondition>();
                if (doGroupNew.ObjectID != ScreenID.C_INV_SCREEN_ID_STOCKIN_ASSET)
                {
                    if (doGroupNew.ObjectID == InstrumentArea.C_INV_AREA_NEW_SAMPLE)
                        DecTransferMovingAveragePrice = InventoryConfig.C_INV_DEFAULT_SAMPLE_AMOUNT;
                    else if (InstrumentLocation.C_INV_LOC_PROJECT_WIP.Equals(doGroupNew.SourceLocationCode))
                    {
                        List<tbt_AccountInprocess> doTbt_AccountInProcess = GetTbt_AccountInProcess(doGroupNew.SourceLocationCode, doGroupNew.ProjectCode, doGroupNew.Instrumentcode);
                        if (doTbt_AccountInProcess.Count > 0)
                            DecTransferMovingAveragePrice = Convert.ToDecimal(doTbt_AccountInProcess[0].MovingAveragePrice);
                    }
                    else if (InstrumentLocation.C_INV_LOC_WIP.Equals(doGroupNew.SourceLocationCode) ||
                        InstrumentLocation.C_INV_LOC_PARTIAL_OUT.Equals(doGroupNew.SourceLocationCode) ||
                        InstrumentLocation.C_INV_LOC_UNOPERATED_WIP.Equals(doGroupNew.SourceLocationCode))
                    {
                        List<tbt_AccountInprocess> doTbt_AccountInProcess = GetTbt_AccountInProcess(doGroupNew.SourceLocationCode, doGroupNew.ContractCode, doGroupNew.Instrumentcode);
                        if (doTbt_AccountInProcess.Count > 0)
                            DecTransferMovingAveragePrice = Convert.ToDecimal(doTbt_AccountInProcess[0].MovingAveragePrice);
                    }
                    else
                    {
                        doTbt_AccountInStock = GetTbt_AccountInStock(doGroupNew.Instrumentcode, doGroupNew.SourceLocationCode, doGroupNew.SourceOfficeCode);
                        if (doTbt_AccountInStock.Count > 0)
                            DecTransferMovingAveragePrice = Convert.ToDecimal(doTbt_AccountInStock[0].MovingAveragePrice);
                    }
                }


                // Get destination instrument data
                if (CommonUtil.IsNullOrEmpty(doGroupNew.LotNo) == false)
                {
                    lstCalPriceCond = base.GetMovingAveragePriceCondition(doGroupNew.DestinationOfficeCode,
                        null,
                        null,
                        doGroupNew.Instrumentcode,
                        "," + doGroupNew.DestinationLocationCode + ",",
                        doGroupNew.LotNo,
                        SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL,
                        SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                }
                else if (doGroupNew.DestinationLocationCode == InstrumentLocation.C_INV_LOC_REPAIR_REQUEST)
                {
                    string strArrayLocationCode = CommonUtil.CreateCSVString(new string[]{ InstrumentLocation.C_INV_LOC_REPAIR_REQUEST,
                                                                                            InstrumentLocation.C_INV_LOC_REPAIRING ,
                                                                                            InstrumentLocation.C_INV_LOC_REPAIR_RETURN });
                    lstCalPriceCond = base.GetMovingAveragePriceCondition(doGroupNew.DestinationOfficeCode, null, null, doGroupNew.Instrumentcode, strArrayLocationCode, null, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);

                }
                else if (doGroupNew.DestinationLocationCode == InstrumentLocation.C_INV_LOC_INSTOCK && String.IsNullOrEmpty(doGroupNew.LotNo))
                {
                    string strArrayLocationCode = CommonUtil.CreateCSVString(new string[]{InstrumentLocation.C_INV_LOC_INSTOCK ,
                                                                                          InstrumentLocation.C_INV_LOC_TRANSFER});
                    lstCalPriceCond = base.GetMovingAveragePriceCondition(null, null, null, doGroupNew.Instrumentcode, strArrayLocationCode, null, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                }
                else if (doGroupNew.DestinationLocationCode == InstrumentLocation.C_INV_LOC_WIP ||
                        doGroupNew.DestinationLocationCode == InstrumentLocation.C_INV_LOC_PARTIAL_OUT ||
                        doGroupNew.DestinationLocationCode == InstrumentLocation.C_INV_LOC_PROJECT_WIP)
                {
                    string strArrayLocationCode = CommonUtil.CreateCSVString(new string[]{InstrumentLocation.C_INV_LOC_WIP ,
                                                                                            InstrumentLocation.C_INV_LOC_PARTIAL_OUT ,
                                                                                            InstrumentLocation.C_INV_LOC_PROJECT_WIP,
                                                                                            InstrumentLocation.C_INV_LOC_UNOPERATED_WIP});
                    lstCalPriceCond = base.GetMovingAveragePriceCondition(doGroupNew.DestinationOfficeCode, doGroupNew.ContractCode, doGroupNew.ProjectCode, doGroupNew.Instrumentcode, strArrayLocationCode, null, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                }
                //else if (CommonUtil.IsNullOrEmpty(doGroupNew.LotNo) == false)
                //{
                //    lstCalPriceCond = base.GetMovingAveragePriceCondition(doGroupNew.DestinationOfficeCode,
                //        null,
                //        null,
                //        doGroupNew.Instrumentcode,
                //        "," + doGroupNew.DestinationLocationCode + ",",
                //        doGroupNew.LotNo);
                //}
                else
                {
                    lstCalPriceCond = base.GetMovingAveragePriceCondition(doGroupNew.DestinationOfficeCode,
                        null,
                        null,
                        doGroupNew.Instrumentcode,
                        "," + doGroupNew.DestinationLocationCode + ",",
                        null,
                        SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL,
                        SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                }


                //3.0
                if (lstCalPriceCond != null && lstCalPriceCond.Count > 0)
                {
                    if (doGroupNew.ObjectID == ScreenID.C_INV_SCREEN_ID_STOCKIN_ASSET)
                    {

                        decimal s_Amount = Math.Abs(Convert.ToDecimal(doGroupNew.TransferQty) * Convert.ToDecimal(doGroupNew.UnitPrice));
                        decimal d_Amount = Math.Abs(Convert.ToDecimal(lstCalPriceCond[0].InstrumentQty) * Convert.ToDecimal(lstCalPriceCond[0].MovingAveragePrice));
                        decimal s_qty = Math.Abs(Convert.ToDecimal(doGroupNew.TransferQty));
                        decimal d_qty = Math.Abs(Convert.ToDecimal(lstCalPriceCond[0].InstrumentQty));

                        // decMovingAveragePrice = (s_Amount + d_Amount) / (s_qty + d_qty)
                        #region //R2
                        decMovingAveragePrice = Math.Abs(Math.Round((s_Amount + d_Amount) / (s_qty + d_qty), 4, MidpointRounding.AwayFromZero));
                        #endregion
                    }
                    else
                    {

                        decimal s_Amount = Math.Abs(Convert.ToDecimal(doGroupNew.TransferQty) * Convert.ToDecimal(DecTransferMovingAveragePrice));
                        decimal d_Amount = Math.Abs(Convert.ToDecimal(lstCalPriceCond[0].InstrumentQty) * Convert.ToDecimal(lstCalPriceCond[0].MovingAveragePrice));
                        decimal s_qty = Math.Abs(Convert.ToDecimal(doGroupNew.TransferQty));
                        decimal d_qty = Math.Abs(Convert.ToDecimal(lstCalPriceCond[0].InstrumentQty));

                        // decMovingAveragePrice = (s_Amount + d_Amount) / (s_qty + d_qty)
                        #region //R2
                        decMovingAveragePrice = Math.Abs(Math.Round((s_Amount + d_Amount) / (s_qty + d_qty), 4, MidpointRounding.AwayFromZero));
                        #endregion
                    }
                }
                else
                {
                    #region //R2
                    decMovingAveragePrice = Math.Round(DecTransferMovingAveragePrice, 4, MidpointRounding.AwayFromZero);
                    #endregion
                }

                return decMovingAveragePrice;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<tbt_AccountInprocess> InsertTbt_AccountInProcess(List<tbt_AccountInprocess> newLstInProcess)
        {
            try
            {
                List<tbt_AccountInprocess> lst = base.InsertTbt_AccountInProcess(CommonUtil.ConvertToXml_Store<tbt_AccountInprocess>(newLstInProcess));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_ACC_INPROCESS,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountInprocess> InsertTbt_AccountInProcess_NoLog(List<tbt_AccountInprocess> newLstInProcess)
        {
            try
            {
                List<tbt_AccountInprocess> lst = base.InsertTbt_AccountInProcess(CommonUtil.ConvertToXml_Store<tbt_AccountInprocess>(newLstInProcess));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountInprocess> UpdateTbt_AccountInProcess(List<tbt_AccountInprocess> NewInProcess)
        {
            try
            {
                List<tbt_AccountInprocess> lst = base.UpdateTbt_AccountInProcess(CommonUtil.ConvertToXml_Store<tbt_AccountInprocess>(NewInProcess));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_ACC_INPROCESS,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountInprocess> UpdateTbt_AccountInProcess_NoLog(List<tbt_AccountInprocess> NewInProcess)
        {
            try
            {
                List<tbt_AccountInprocess> lst = base.UpdateTbt_AccountInProcess(CommonUtil.ConvertToXml_Store<tbt_AccountInprocess>(NewInProcess));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountInstock> InsertTbt_AccountInStock(List<tbt_AccountInstock> newAccountInStock)
        {
            try
            {
                List<tbt_AccountInstock> lst = base.InsertTbt_AccountInStock(CommonUtil.ConvertToXml_Store<tbt_AccountInstock>(newAccountInStock));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_ACC_INSTOCK,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountInstock> InsertTbt_AccountInStock_NoLog(List<tbt_AccountInstock> newAccountInStock)
        {
            try
            {
                List<tbt_AccountInstock> lst = base.InsertTbt_AccountInStock(CommonUtil.ConvertToXml_Store<tbt_AccountInstock>(newAccountInStock));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountInstock> UpdateTbt_AccountInStock(List<tbt_AccountInstock> newAccountInStock)
        {
            try
            {
                List<tbt_AccountInstock> lst = base.UpdateTbt_AccountInStock(CommonUtil.ConvertToXml_Store<tbt_AccountInstock>(newAccountInStock));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_ACC_INSTOCK,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_AccountInstock> UpdateTbt_AccountInStock_NoLog(List<tbt_AccountInstock> newAccountInStock)
        {
            try
            {
                List<tbt_AccountInstock> lst = base.UpdateTbt_AccountInStock(CommonUtil.ConvertToXml_Store<tbt_AccountInstock>(newAccountInStock));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Akat K. CHECKED 2012-03-01
        public bool UpdateAccountTransferNewInstrument(doGroupNewInstrument doGroupNew, decimal? decMovingAveragePrice)
        {
            #region // New WIP concept @ 24-Feb-2015
            int? tmp;
            return this.UpdateAccountTransferNewInstrument(doGroupNew, decMovingAveragePrice, out tmp);
            #endregion
        }

        public bool UpdateAccountTransferNewInstrument(doGroupNewInstrument doGroupNew, decimal? decMovingAveragePrice, out int? intReturnInprocess)
        {
            intReturnInprocess = null;

            if (doGroupNew == null)
            {
                return false;
            }

            try
            {
                if (doGroupNew.DestinationLocationCode == InstrumentLocation.C_INV_LOC_RETURNED
                    && (
                        doGroupNew.SourceLocationCode == InstrumentLocation.C_INV_LOC_WIP
                        || doGroupNew.SourceLocationCode == InstrumentLocation.C_INV_LOC_UNOPERATED_WIP
                        || doGroupNew.SourceLocationCode == InstrumentLocation.C_INV_LOC_PROJECT_WIP          
                        || doGroupNew.SourceLocationCode == InstrumentLocation.C_INV_LOC_PARTIAL_OUT            // - Add by Nontawat L. on 09-Jul-2014: Phase4: 3.44
                    )
                )
                {
                    doGroupNew.DestinationLocationCode = InstrumentLocation.C_INV_LOC_WAITING_RETURN;
                }

                if (CommonUtil.IsNullOrEmpty(decMovingAveragePrice) || decMovingAveragePrice == 0)
                {
                    if (InstrumentLocation.C_INV_LOC_PROJECT_WIP.Equals(doGroupNew.SourceLocationCode))
                    {
                        /*-----------------Edited by Phasupong 14/12/2015----------------*/
                        //List<tbt_AccountInprocess> oldInProcess = GetTbt_AccountInProcess(doGroupNew.SourceLocationCode, doGroupNew.ProjectCode, doGroupNew.Instrumentcode);
                        //if (oldInProcess.Count > 0)
                        //{
                        //    decMovingAveragePrice = oldInProcess[0].MovingAveragePrice.HasValue ? oldInProcess[0].MovingAveragePrice.Value : 0;
                        //}
                        
                        List<tbt_AccountInstock> oldInStock = GetTbt_AccountInStock(doGroupNew.Instrumentcode, doGroupNew.SourceLocationCode, doGroupNew.SourceOfficeCode);
                        if (oldInStock.Count > 0)
                        {
                            decMovingAveragePrice = oldInStock[0].MovingAveragePrice.HasValue ? oldInStock[0].MovingAveragePrice.Value : 0;
                        }
                        /*----------------------------END--------------------------------*/
                    }
                    else if (InstrumentLocation.C_INV_LOC_WIP.Equals(doGroupNew.SourceLocationCode) ||
                        InstrumentLocation.C_INV_LOC_PARTIAL_OUT.Equals(doGroupNew.SourceLocationCode) ||
                        //InstrumentLocation.C_INV_LOC_PROJECT_WIP.Equals(doGroupNew.SourceLocationCode) ||
                        InstrumentLocation.C_INV_LOC_UNOPERATED_WIP.Equals(doGroupNew.SourceLocationCode))
                    {
                        List<tbt_AccountInprocess> oldInProcess = GetTbt_AccountInProcess(doGroupNew.SourceLocationCode, doGroupNew.ContractCode, doGroupNew.Instrumentcode);
                        if (oldInProcess.Count > 0)
                        {
                            decMovingAveragePrice = oldInProcess[0].MovingAveragePrice.HasValue ? oldInProcess[0].MovingAveragePrice.Value : 0;
                        }
                    }
                    else if (InstrumentLocation.C_INV_LOC_TRANSFER.Equals(doGroupNew.DestinationLocationCode) ||
                        InstrumentLocation.C_INV_LOC_INSTOCK.Equals(doGroupNew.DestinationLocationCode) ||
                        InstrumentLocation.C_INV_LOC_REPAIR_RETURN.Equals(doGroupNew.DestinationLocationCode) ||
                        InstrumentLocation.C_INV_LOC_REPAIRING.Equals(doGroupNew.DestinationLocationCode))
                    {
                        List<tbt_AccountInstock> doTbt_AccountInstock = GetTbt_AccountInStock(doGroupNew.Instrumentcode, doGroupNew.SourceLocationCode, doGroupNew.SourceOfficeCode);
                        if (doTbt_AccountInstock.Count > 0)
                        {
                            decMovingAveragePrice = doTbt_AccountInstock[0].MovingAveragePrice.HasValue ? doTbt_AccountInstock[0].MovingAveragePrice.Value : 0;
                        }
                    }
                    else
                    {
                        List<tbt_AccountInstalled> oldInstalled = base.GetTbt_AccountInstalled(doGroupNew.SourceOfficeCode, doGroupNew.SourceLocationCode, doGroupNew.Instrumentcode, null);
                        if (oldInstalled.Count > 0)
                        {
                            decMovingAveragePrice = GetDecimalNullableVal(oldInstalled[0].AccquisitionCost);
                        }
                    }
                }

                // Update transfer account stock data to source
                if (InstrumentLocation.C_INV_LOC_PARTIAL_OUT.Equals(doGroupNew.SourceLocationCode) ||
                    InstrumentLocation.C_INV_LOC_WIP.Equals(doGroupNew.SourceLocationCode) ||
                    InstrumentLocation.C_INV_LOC_UNOPERATED_WIP.Equals(doGroupNew.SourceLocationCode))
                {
                    List<tbt_AccountInprocess> oldInProcess = GetTbt_AccountInProcess(doGroupNew.SourceLocationCode, doGroupNew.ContractCode, doGroupNew.Instrumentcode);
                    if (oldInProcess.Count > 0)
                    {
                        oldInProcess[0].LocationCode = doGroupNew.SourceLocationCode;
                        oldInProcess[0].ContractCode = doGroupNew.ContractCode;
                        oldInProcess[0].InstrumentCode = doGroupNew.Instrumentcode;
                        //ProjectCode Not changed 
                        oldInProcess[0].InstrumentQty = GetIntNullableVal(oldInProcess[0].InstrumentQty) - GetIntNullableVal(doGroupNew.TransferQty);
                        //MovingAveragePrice Not changed
                        //CreateDate Not changed
                        //CreatedBy	Not changed
                        oldInProcess[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        oldInProcess[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;


                        List<tbt_AccountInprocess> NewInProcess = new List<tbt_AccountInprocess>();
                        NewInProcess.Add(oldInProcess[0]);
                        List<tbt_AccountInprocess> lstbt_AccountInprocess = this.UpdateTbt_AccountInProcess(NewInProcess);
                        if (lstbt_AccountInprocess == null || lstbt_AccountInprocess.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INPROCESS });
                        }

                    }
                }
                #region // New WIP concept @ 24-Feb-2015
                else if (InstrumentLocation.C_INV_LOC_WAITING_RETURN.Equals(doGroupNew.SourceLocationCode))
                {
                    int transferQty = doGroupNew.TransferQty ?? 0;

                    if (transferQty > 0)
                    {
                        List<tbt_AccountInprocess> oldInProcess = GetTbt_AccountInProcess(doGroupNew.SourceLocationCode, doGroupNew.ContractCode, doGroupNew.Instrumentcode);
                        if (oldInProcess.Count > 0 && (oldInProcess[0].InstrumentQty ?? 0) > 0)
                        {
                            oldInProcess[0].LocationCode = doGroupNew.SourceLocationCode;
                            oldInProcess[0].ContractCode = doGroupNew.ContractCode;
                            oldInProcess[0].InstrumentCode = doGroupNew.Instrumentcode;

                            int inProcessQty = (oldInProcess[0].InstrumentQty ?? 0);
                            if (inProcessQty >= transferQty)
                            {
                                oldInProcess[0].InstrumentQty = inProcessQty - transferQty;
                                intReturnInprocess = (intReturnInprocess ?? 0) + transferQty;
                                transferQty = 0;
                            }
                            else
                            {
                                oldInProcess[0].InstrumentQty = 0;
                                intReturnInprocess = (intReturnInprocess ?? 0) + inProcessQty;
                                transferQty -= inProcessQty;
                            }

                            oldInProcess[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                            oldInProcess[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                            List<tbt_AccountInprocess> NewInProcess = new List<tbt_AccountInprocess>();
                            NewInProcess.Add(oldInProcess[0]);
                            List<tbt_AccountInprocess> lstbt_AccountInprocess = this.UpdateTbt_AccountInProcess(NewInProcess);
                            if (lstbt_AccountInprocess == null || lstbt_AccountInprocess.Count <= 0)
                            {
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INPROCESS });
                            }

                        }

                        if (transferQty > 0)
                        {
                            List<tbt_AccountInstock> accountInstockList = GetTbt_AccountInStock(doGroupNew.Instrumentcode, doGroupNew.SourceLocationCode, doGroupNew.SourceOfficeCode);
                            if (accountInstockList.Count > 0 && (accountInstockList[0].InstrumentQty ?? 0) > 0)
                            {
                                accountInstockList[0].OfficeCode = doGroupNew.SourceOfficeCode;
                                accountInstockList[0].LocationCode = doGroupNew.SourceLocationCode;
                                accountInstockList[0].InstrumentCode = doGroupNew.Instrumentcode;

                                int inStockQty = (accountInstockList[0].InstrumentQty ?? 0);
                                accountInstockList[0].InstrumentQty = inStockQty - transferQty;

                                accountInstockList[0].MovingAveragePrice = decMovingAveragePrice;
                                accountInstockList[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                accountInstockList[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                                List<tbt_AccountInstock> newAccountInstockList = new List<tbt_AccountInstock>();
                                newAccountInstockList.Add(accountInstockList[0]);
                                List<tbt_AccountInstock> resultUpdate = UpdateTbt_AccountInStock(newAccountInstockList);

                                if (resultUpdate == null || resultUpdate.Count <= 0)
                                {
                                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTOCK });
                                }
                            }
                        }
                    }
                }
                #endregion
                else if (InstrumentLocation.C_INV_LOC_PROJECT_WIP.Equals(doGroupNew.SourceLocationCode))
                {
                    /*------------------------Edited by Phasupong------------------------*/
                    //List<tbt_AccountInprocess> oldInProcess = GetTbt_AccountInProcess(doGroupNew.SourceLocationCode, doGroupNew.ProjectCode, doGroupNew.Instrumentcode);
                    //if (oldInProcess.Count > 0)
                    //{
                    //    oldInProcess[0].LocationCode = doGroupNew.SourceLocationCode;
                    //    oldInProcess[0].ContractCode = doGroupNew.ProjectCode;
                    //    oldInProcess[0].InstrumentCode = doGroupNew.Instrumentcode;
                    //    oldInProcess[0].InstrumentQty = GetIntNullableVal(oldInProcess[0].InstrumentQty) - GetIntNullableVal(doGroupNew.TransferQty);
                    //    oldInProcess[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    oldInProcess[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    //    List<tbt_AccountInprocess> NewInProcess = new List<tbt_AccountInprocess>();
                    //    NewInProcess.Add(oldInProcess[0]);
                    //    List<tbt_AccountInprocess> lstbt_AccountInprocess = this.UpdateTbt_AccountInProcess(NewInProcess);
                    //    if (lstbt_AccountInprocess == null || lstbt_AccountInprocess.Count <= 0)
                    //    {
                    //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INPROCESS });
                    //    }
                    //}

                    List<tbt_AccountInstock> oldInStock = GetTbt_AccountInStock(doGroupNew.Instrumentcode, doGroupNew.SourceLocationCode, doGroupNew.SourceOfficeCode);
                    if (oldInStock.Count > 0)
                    {
                        oldInStock[0].LocationCode = doGroupNew.SourceLocationCode;
                        oldInStock[0].OfficeCode = doGroupNew.SourceOfficeCode;
                        oldInStock[0].InstrumentCode = doGroupNew.Instrumentcode;
                        oldInStock[0].InstrumentQty = GetIntNullableVal(oldInStock[0].InstrumentQty) - GetIntNullableVal(doGroupNew.TransferQty);
                        oldInStock[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        oldInStock[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountInstock> NewInStock = new List<tbt_AccountInstock>();
                        NewInStock.Add(oldInStock[0]);
                        List<tbt_AccountInstock> lstbt_AccountInStock = this.UpdateTbt_AccountInStock(NewInStock);
                        if (lstbt_AccountInStock == null || lstbt_AccountInStock.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTOCK });
                        }
                    }
                    /*-----------------------------End-------------------------------*/
                }
                //else if (doGroupNew.ObjectID != ScreenID.C_INV_SCREEN_ID_STOCKIN_ASSET)
                else if (doGroupNew.ObjectID != ScreenID.C_INV_SCREEN_ID_STOCKIN) //Monthly Price @ 2015
                {
                    if (doGroupNew.TransferType == false)
                    {
                        doGroupNew.TransferQty = doGroupNew.TransferQty * -1;
                    }

                    List<tbt_AccountInstock> oldAccountInStock = GetTbt_AccountInStock(doGroupNew.Instrumentcode, doGroupNew.SourceLocationCode, doGroupNew.SourceOfficeCode);

                    if (oldAccountInStock.Count > 0)
                    {
                        oldAccountInStock[0].InstrumentQty = GetIntNullableVal(oldAccountInStock[0].InstrumentQty) - GetIntNullableVal(doGroupNew.TransferQty);
                        oldAccountInStock[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        oldAccountInStock[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountInstock> newAccountInStock = new List<tbt_AccountInstock>();
                        newAccountInStock.Add(oldAccountInStock[0]);
                        List<tbt_AccountInstock> lstbt_AccountInStock = UpdateTbt_AccountInStock(newAccountInStock);

                        if (lstbt_AccountInStock == null || lstbt_AccountInStock.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTOCK });
                        }
                    }
                    else
                    {
                        tbt_AccountInstock doTbt_AccountInStock = new tbt_AccountInstock();
                        doTbt_AccountInStock.OfficeCode = doGroupNew.SourceOfficeCode;
                        doTbt_AccountInStock.LocationCode = doGroupNew.SourceLocationCode;
                        doTbt_AccountInStock.InstrumentCode = doGroupNew.Instrumentcode;
                        doTbt_AccountInStock.InstrumentQty = GetIntNullableVal(doTbt_AccountInStock.InstrumentQty) - GetIntNullableVal(doGroupNew.TransferQty);
                        doTbt_AccountInStock.MovingAveragePrice = decMovingAveragePrice;
                        doTbt_AccountInStock.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doTbt_AccountInStock.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doTbt_AccountInStock.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doTbt_AccountInStock.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountInstock> newAccountInStock = new List<tbt_AccountInstock>();
                        newAccountInStock.Add(doTbt_AccountInStock);
                        List<tbt_AccountInstock> lstbt_AccountInStock = InsertTbt_AccountInStock(newAccountInStock);

                        if (lstbt_AccountInStock == null || lstbt_AccountInStock.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTOCK });
                        }
                    }
                }


                // Check Destination Location (send out system)
                if (InstrumentLocation.C_INV_LOC_ELIMINATION.Equals(doGroupNew.DestinationLocationCode) ||
                    InstrumentLocation.C_INV_LOC_SPECIAL.Equals(doGroupNew.DestinationLocationCode) ||
                    InstrumentLocation.C_INV_LOC_LOSS.Equals(doGroupNew.DestinationLocationCode) ||
                    InstrumentLocation.C_INV_LOC_SOLD.Equals(doGroupNew.DestinationLocationCode)
                    )
                {
                    return true;
                }


                // 4. Update transfer account status data to destination
                // Update in WIP Group location
                if (InstrumentLocation.C_INV_LOC_PARTIAL_OUT.Equals(doGroupNew.DestinationLocationCode) ||
                    InstrumentLocation.C_INV_LOC_WIP.Equals(doGroupNew.DestinationLocationCode) ||
                    InstrumentLocation.C_INV_LOC_UNOPERATED_WIP.Equals(doGroupNew.DestinationLocationCode) ||
                    InstrumentLocation.C_INV_LOC_WAITING_RETURN.Equals(doGroupNew.DestinationLocationCode) // New WIP concept @ 24-Feb-2015
                    )
                {
                    List<tbt_AccountInprocess> doTbt_AccountInProcess = GetTbt_AccountInProcess(doGroupNew.DestinationLocationCode, doGroupNew.ContractCode, doGroupNew.Instrumentcode);
                    if (doTbt_AccountInProcess.Count <= 0)
                    {
                        // Insert
                        tbt_AccountInprocess newInPs = new tbt_AccountInprocess();
                        newInPs.LocationCode = doGroupNew.DestinationLocationCode;
                        newInPs.ContractCode = doGroupNew.ContractCode;
                        newInPs.InstrumentCode = doGroupNew.Instrumentcode;
                        newInPs.ProjectCode = doGroupNew.ProjectCode;
                        newInPs.InstrumentQty = doGroupNew.TransferQty;
                        newInPs.MovingAveragePrice = decMovingAveragePrice;
                        newInPs.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        newInPs.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        newInPs.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        newInPs.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountInprocess> newLstInProcess = new List<tbt_AccountInprocess>();
                        newLstInProcess.Add(newInPs);
                        List<tbt_AccountInprocess> resultInsert = InsertTbt_AccountInProcess(newLstInProcess);
                        if (resultInsert.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INPROCESS });
                        }
                    }
                    else
                    {
                        // Update 
                        doTbt_AccountInProcess[0].InstrumentQty = GetIntNullableVal(doTbt_AccountInProcess[0].InstrumentQty) + GetIntNullableVal(doGroupNew.TransferQty);
                        doTbt_AccountInProcess[0].MovingAveragePrice = decMovingAveragePrice;
                        doTbt_AccountInProcess[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doTbt_AccountInProcess[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        List<tbt_AccountInprocess> NewInProcess = new List<tbt_AccountInprocess>();
                        NewInProcess.Add(doTbt_AccountInProcess[0]);
                        List<tbt_AccountInprocess> resultUpdate = UpdateTbt_AccountInProcess(NewInProcess);
                        if (resultUpdate.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INPROCESS });
                        }
                    }
                }
                // Update in Project WIP location (INSTOCK -> PROJECT WIP) --- 4.2
                else if (InstrumentLocation.C_INV_LOC_PROJECT_WIP.Equals(doGroupNew.DestinationLocationCode))
                {
                    /*---------------------Edited by Phasupong 14/12/2015---------------------------*/
                    //List<tbt_AccountInprocess> doTbt_AccountInProcess = GetTbt_AccountInProcess(doGroupNew.DestinationLocationCode, doGroupNew.ProjectCode, doGroupNew.Instrumentcode);

                    //if (doTbt_AccountInProcess.Count <= 0)
                    //{
                    //    // Insert
                    //    tbt_AccountInprocess newInPs = new tbt_AccountInprocess();
                    //    newInPs.LocationCode = doGroupNew.DestinationLocationCode;
                    //    newInPs.ContractCode = doGroupNew.ProjectCode;
                    //    newInPs.InstrumentCode = doGroupNew.Instrumentcode;
                    //    newInPs.ProjectCode = doGroupNew.ProjectCode;
                    //    newInPs.InstrumentQty = doGroupNew.TransferQty;
                    //    newInPs.MovingAveragePrice = decMovingAveragePrice;
                    //    newInPs.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    newInPs.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    //    newInPs.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    newInPs.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    //    List<tbt_AccountInprocess> newLstInProcess = new List<tbt_AccountInprocess>();
                    //    newLstInProcess.Add(newInPs);
                    //    List<tbt_AccountInprocess> lstbt_AccountInStock = InsertTbt_AccountInProcess(newLstInProcess);

                    //    if (lstbt_AccountInStock == null || lstbt_AccountInStock.Count <= 0)
                    //    {
                    //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INPROCESS });
                    //    }
                    //}
                    //else
                    //{
                    //    // Update
                    //    doTbt_AccountInProcess[0].InstrumentQty = GetIntNullableVal(doTbt_AccountInProcess[0].InstrumentQty) + GetIntNullableVal(doGroupNew.TransferQty);
                    //    doTbt_AccountInProcess[0].MovingAveragePrice = decMovingAveragePrice;
                    //    doTbt_AccountInProcess[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    //    doTbt_AccountInProcess[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                    //    List<tbt_AccountInprocess> NewInProcess = new List<tbt_AccountInprocess>();
                    //    NewInProcess.Add(doTbt_AccountInProcess[0]);
                    //    List<tbt_AccountInprocess> lstbt_AccountInStock = UpdateTbt_AccountInProcess(NewInProcess);

                    //    if (lstbt_AccountInStock == null || lstbt_AccountInStock.Count <= 0)
                    //    {
                    //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INPROCESS });
                    //    }
                    //}

                    List<tbt_AccountInstock> doTbt_AccountInStock = GetTbt_AccountInStock(doGroupNew.Instrumentcode, doGroupNew.DestinationLocationCode, doGroupNew.DestinationOfficeCode);

                    if (doTbt_AccountInStock.Count <= 0)
                    {
                        // Insert
                        tbt_AccountInstock newInPs = new tbt_AccountInstock();
                        newInPs.OfficeCode = doGroupNew.DestinationOfficeCode;
                        newInPs.LocationCode = doGroupNew.DestinationLocationCode;
                        newInPs.InstrumentCode = doGroupNew.Instrumentcode;
                        newInPs.InstrumentQty = doGroupNew.TransferQty;
                        newInPs.MovingAveragePrice = decMovingAveragePrice;
                        newInPs.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        newInPs.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        newInPs.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        newInPs.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountInstock> newLstInStock = new List<tbt_AccountInstock>();
                        newLstInStock.Add(newInPs);
                        List<tbt_AccountInstock> lstbt_AccountInStock = InsertTbt_AccountInStock(newLstInStock);

                        if (lstbt_AccountInStock == null || lstbt_AccountInStock.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTOCK });
                        }
                    }
                    else
                    {
                        // Update
                        doTbt_AccountInStock[0].OfficeCode = doGroupNew.DestinationOfficeCode;
                        doTbt_AccountInStock[0].LocationCode = doGroupNew.DestinationLocationCode;
                        doTbt_AccountInStock[0].InstrumentCode = doGroupNew.Instrumentcode;
                        doTbt_AccountInStock[0].InstrumentQty = GetIntNullableVal(doTbt_AccountInStock[0].InstrumentQty) + GetIntNullableVal(doGroupNew.TransferQty);
                        doTbt_AccountInStock[0].MovingAveragePrice = decMovingAveragePrice;
                        doTbt_AccountInStock[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doTbt_AccountInStock[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountInstock> NewInStock = new List<tbt_AccountInstock>();
                        NewInStock.Add(doTbt_AccountInStock[0]);
                        List<tbt_AccountInstock> lstbt_AccountInStock = UpdateTbt_AccountInStock(NewInStock);

                        if (lstbt_AccountInStock == null || lstbt_AccountInStock.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTOCK });
                        }
                    }
                    /*------------------------------------End---------------------------------------*/
                }

                // Update in user location
                else if (!String.IsNullOrEmpty(doGroupNew.LotNo))
                {
                    List<tbt_AccountInstalled> doTbt_AccountInstalled = GetTbt_AccountInstalled(doGroupNew.DestinationOfficeCode, doGroupNew.DestinationLocationCode, doGroupNew.Instrumentcode, doGroupNew.LotNo);
                    if (doTbt_AccountInstalled.Count <= 0)
                    {
                        // Insert
                        tbt_AccountInstalled newAcc = new tbt_AccountInstalled();
                        newAcc.OfficeCode = doGroupNew.DestinationOfficeCode;
                        newAcc.LocationCode = doGroupNew.DestinationLocationCode;
                        newAcc.LotNo = doGroupNew.LotNo;
                        newAcc.InstrumentCode = doGroupNew.Instrumentcode;
                        newAcc.InstrumentQty = doGroupNew.TransferQty;
                        newAcc.AccquisitionCost = decMovingAveragePrice;
                        newAcc.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        newAcc.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        newAcc.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        newAcc.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountInstalled> newLstInStalled = new List<tbt_AccountInstalled>();
                        newLstInStalled.Add(newAcc);
                        List<tbt_AccountInstalled> lstbt_AccountInstall = InsertTbt_AccountInstalled(newLstInStalled);

                        if (lstbt_AccountInstall == null || lstbt_AccountInstall.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }
                    else
                    {
                        // Update
                        doTbt_AccountInstalled[0].InstrumentQty = GetIntNullableVal(doTbt_AccountInstalled[0].InstrumentQty) + GetIntNullableVal(doGroupNew.TransferQty);
                        doTbt_AccountInstalled[0].AccquisitionCost = decMovingAveragePrice;
                        doTbt_AccountInstalled[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doTbt_AccountInstalled[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountInstalled> NewInStall = new List<tbt_AccountInstalled>();
                        NewInStall.Add(doTbt_AccountInstalled[0]);
                        List<tbt_AccountInstalled> lstbt_AccountInstall = UpdateTbt_AccountInstalled(NewInStall);

                        if (lstbt_AccountInstall == null || lstbt_AccountInstall.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }
                }
                // 4.4 Update in other location
                else
                {
                    List<tbt_AccountInstock> accountInstockList = GetTbt_AccountInStock(doGroupNew.Instrumentcode, doGroupNew.DestinationLocationCode, doGroupNew.DestinationOfficeCode);
                    if (accountInstockList.Count <= 0)
                    {
                        tbt_AccountInstock accountInstock = new tbt_AccountInstock();
                        accountInstock.OfficeCode = doGroupNew.DestinationOfficeCode;
                        accountInstock.LocationCode = doGroupNew.DestinationLocationCode;
                        accountInstock.InstrumentCode = doGroupNew.Instrumentcode;
                        accountInstock.InstrumentQty = doGroupNew.TransferQty;
                        accountInstock.MovingAveragePrice = decMovingAveragePrice;
                        accountInstock.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        accountInstock.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        accountInstock.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        accountInstock.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountInstock> newAccountInstockList = new List<tbt_AccountInstock>();
                        newAccountInstockList.Add(accountInstock);
                        List<tbt_AccountInstock> resulInsert = InsertTbt_AccountInStock(newAccountInstockList);

                        if (resulInsert == null || resulInsert.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTOCK });
                        }
                    }
                    else
                    {
                        accountInstockList[0].OfficeCode = doGroupNew.DestinationOfficeCode;
                        accountInstockList[0].LocationCode = doGroupNew.DestinationLocationCode;
                        accountInstockList[0].InstrumentCode = doGroupNew.Instrumentcode;
                        accountInstockList[0].InstrumentQty = GetIntNullableVal(accountInstockList[0].InstrumentQty) + GetIntNullableVal(doGroupNew.TransferQty);
                        accountInstockList[0].MovingAveragePrice = decMovingAveragePrice;
                        accountInstockList[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        accountInstockList[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_AccountInstock> newAccountInstockList = new List<tbt_AccountInstock>();
                        newAccountInstockList.Add(accountInstockList[0]);
                        List<tbt_AccountInstock> resultUpdate = UpdateTbt_AccountInStock(newAccountInstockList);

                        if (resultUpdate == null || resultUpdate.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTOCK });
                        }
                    }
                }

                // 5. Update moving averag price to reference table

                if (InstrumentLocation.C_INV_LOC_REPAIR_REQUEST.Equals(doGroupNew.DestinationLocationCode))
                {
                    bool canUpdate = UpdateMovingAveragePriceForRepairingGroup(decMovingAveragePrice,
                                                                doGroupNew.DestinationOfficeCode,
                                                                doGroupNew.Instrumentcode);
                    if (!canUpdate)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTOCK });
                    }
                }
                else if (InstrumentLocation.C_INV_LOC_INSTOCK.Equals(doGroupNew.DestinationLocationCode) && string.IsNullOrEmpty(doGroupNew.LotNo))
                {
                    bool canUpdate = UpdateMovingAveragePriceForInStockGroup(decMovingAveragePrice,
                                                                doGroupNew.Instrumentcode);
                    if (!canUpdate)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTOCK });
                    }
                }
                else if (InstrumentLocation.C_INV_LOC_PARTIAL_OUT.Equals(doGroupNew.DestinationLocationCode) ||
                            //InstrumentLocation.C_INV_LOC_PROJECT_WIP.Equals(doGroupNew.DestinationLocationCode) ||        //Edited by Phasupong 14/12/2015
                            InstrumentLocation.C_INV_LOC_WIP.Equals(doGroupNew.DestinationLocationCode)
                    )
                {
                    bool canUpdate = UpdateMovingAveragePriceForWIPGroup(doGroupNew.ProjectCode,
                                                        decMovingAveragePrice,
                                                        doGroupNew.Instrumentcode,
                                                        doGroupNew.ContractCode);
                    if (!canUpdate)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTOCK });
                    }
                }
                else if (!string.IsNullOrEmpty(doGroupNew.LotNo))
                {
                    UpdateMovingAveragePriceForInstalled(decMovingAveragePrice,
                                                            doGroupNew.Instrumentcode,
                                                            doGroupNew.LotNo,
                                                            ConfigName.C_CONFIG_SCRAP_VALUE,
                                                            CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                                            CommonUtil.dsTransData.dtUserData.EmpNo,
                                                            CommonUtil.dsTransData.dtOperationData.GUID,
                                                            CommonUtil.dsTransData.dtTransHeader.ScreenID);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool UpdateMovingAveragePriceForWIPGroup(string projectCode, decimal? decMovingAveragePrice, string instrumentcode, string contractCode)
        {
            try
            {
                List<tbt_AccountInprocess> updateResult = UpdateMovingAveragePriceForWIPGroup(projectCode,
                                    decMovingAveragePrice,
                                    instrumentcode,
                                    contractCode,
                                    InstrumentLocation.C_INV_LOC_PARTIAL_OUT,
                                    InstrumentLocation.C_INV_LOC_PROJECT_WIP,
                                    InstrumentLocation.C_INV_LOC_WIP,
                                    InstrumentLocation.C_INV_LOC_UNOPERATED_WIP);
                #region Log

                if (updateResult.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_ACC_INPROCESS,
                        TableData = CommonUtil.ConvertToXml(updateResult)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                    return true;
                }

                #endregion

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateMovingAveragePriceForInStockGroup(decimal? decMovingAveragePrice, string instrumentcode)
        {
            try
            {
                List<tbt_AccountInstock> updateResult = UpdateMovingAveragePriceForInStockGroup(decMovingAveragePrice,
                                                         InstrumentLocation.C_INV_LOC_INSTOCK,
                                                         InstrumentLocation.C_INV_LOC_TRANSFER,
                                                         instrumentcode);
                #region Log

                if (updateResult.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_ACC_INSTOCK,
                        TableData = CommonUtil.ConvertToXml(updateResult)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                    return true;
                }

                #endregion

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateMovingAveragePriceForRepairingGroup(decimal? decMovingAveragePrice, string officeCode, string instrumentcode)
        {
            try
            {
                List<tbt_AccountInstock> updateResult = UpdateMovingAveragePriceForRepairingGroup(InstrumentLocation.C_INV_LOC_REPAIR_REQUEST,
                                                          InstrumentLocation.C_INV_LOC_REPAIRING,
                                                          InstrumentLocation.C_INV_LOC_REPAIR_RETURN,
                                                          decMovingAveragePrice,
                                                          officeCode,
                                                          instrumentcode);
                #region Log

                if (updateResult.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_ACC_INSTOCK,
                        TableData = CommonUtil.ConvertToXml(updateResult)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                    return true;
                }

                #endregion

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tbt_InventoryCurrent> UpdateTbt_InventoryCurrent(List<tbt_InventoryCurrent> lstCurrent)
        {
            try
            {
                List<tbt_InventoryCurrent> lst = base.UpdateTbt_InventoryCurrent(CommonUtil.ConvertToXml_Store<tbt_InventoryCurrent>(lstCurrent));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_CURRENT,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_InventoryCurrent> UpdateTbt_InventoryCurrent_NoLog(List<tbt_InventoryCurrent> lstCurrent)
        {
            try
            {
                List<tbt_InventoryCurrent> lst = base.UpdateTbt_InventoryCurrent(CommonUtil.ConvertToXml_Store<tbt_InventoryCurrent>(lstCurrent));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<tbt_InventoryCurrent> UpdateTbt_InventoryCurrent(string xmlTbt_InventoryCurrent)
        {
            try
            {
                List<tbt_InventoryCurrent> lst = base.UpdateTbt_InventoryCurrent(xmlTbt_InventoryCurrent);

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_CURRENT,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_InventoryCurrent> InsertTbt_InventoryCurrent(List<tbt_InventoryCurrent> lstCurrent)
        {
            try
            {
                List<tbt_InventoryCurrent> lst = base.InsertTbt_InventoryCurrent(CommonUtil.ConvertToXml_Store<tbt_InventoryCurrent>(lstCurrent));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_CURRENT,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_InventoryCurrent> InsertTbt_InventoryCurrent_NoLog(List<tbt_InventoryCurrent> lstCurrent)
        {
            try
            {
                List<tbt_InventoryCurrent> lst = base.InsertTbt_InventoryCurrent(CommonUtil.ConvertToXml_Store<tbt_InventoryCurrent>(lstCurrent));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_InventorySlip> InsertTbt_InventorySlip(List<tbt_InventorySlip> lstSlip)
        {
            try
            {
                List<tbt_InventorySlip> lst = base.InsertTbt_InventorySlip(CommonUtil.ConvertToXml_Store<tbt_InventorySlip>(lstSlip));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_SLIP,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    logData.TableData = CommonUtil.ConvertToXml(lst);

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_InventorySlip> InsertTbt_InventorySlip_NoLog(List<tbt_InventorySlip> lstSlip)
        {
            try
            {
                List<tbt_InventorySlip> lst = base.InsertTbt_InventorySlip(CommonUtil.ConvertToXml_Store<tbt_InventorySlip>(lstSlip));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<tbt_InventorySlip> InsertTbt_InventorySlip(string xmlTbt_InventorySlilp)
        {
            try
            {
                List<tbt_InventorySlip> lst = base.InsertTbt_InventorySlip(xmlTbt_InventorySlilp);

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_SLIP,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    logData.TableData = CommonUtil.ConvertToXml(lst);

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_InventorySlip> UpdateTbt_InventorySlip(List<tbt_InventorySlip> lstSlip)
        {
            try
            {
                //Add by Jutarat A. on 30052013
                if (lstSlip != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_InventorySlip data in lstSlip)
                    {
                        //Check whether this record is the most updated data
                        List<tbt_InventorySlip> tmpSlip = GetTbt_InventorySlip(data.SlipNo);
                        if (tmpSlip.Count > 0)
                        {
                            if (DateTime.Compare(tmpSlip[0].UpdateDate.GetValueOrDefault(), data.UpdateDate.GetValueOrDefault()) != 0)
                            {
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                            }
                        }

                        //set updateDate and updateBy
                        data.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        data.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }
                //End Add

                List<tbt_InventorySlip> lst = base.UpdateTbt_InventorySlip(CommonUtil.ConvertToXml_Store<tbt_InventorySlip>(lstSlip));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_SLIP,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_InventorySlip> UpdateTbt_InventorySlip_NoLog(List<tbt_InventorySlip> lstSlip)
        {
            try
            {
                List<tbt_InventorySlip> lst = base.UpdateTbt_InventorySlip(CommonUtil.ConvertToXml_Store<tbt_InventorySlip>(lstSlip));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_InventorySlipDetail> GetTbt_InventorySlipDetail(string slipNo)
        {
            try
            {
                return base.GetTbt_InventorySlipDetail(slipNo, null);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public override List<tbt_InventorySlipDetail> GetTbt_InventorySlipDetail(string slipNo, Int32? RunningNo)
        {
            try
            {
                return base.GetTbt_InventorySlipDetail(slipNo, RunningNo);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<tbt_InventorySlipDetail> InsertTbt_InventorySlipDetail(List<tbt_InventorySlipDetail> lstSlipDetail)
        {
            try
            {
                string xml = CommonUtil.ConvertToXml_Store<tbt_InventorySlipDetail>(lstSlipDetail);
                List<tbt_InventorySlipDetail> lst = base.InsertTbt_InventorySlipDetail(xml);

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_SLIP_DETAIL,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_InventorySlipDetail> InsertTbt_InventorySlipDetail_NoLog(List<tbt_InventorySlipDetail> lstSlipDetail)
        {
            try
            {
                List<tbt_InventorySlipDetail> lst = base.InsertTbt_InventorySlipDetail(CommonUtil.ConvertToXml_Store<tbt_InventorySlipDetail>(lstSlipDetail));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<tbt_InventorySlipDetail> InsertTbt_InventorySlipDetail(string xmlTbt_InventorySlipDetail)
        {
            try
            {
                List<tbt_InventorySlipDetail> lst = base.InsertTbt_InventorySlipDetail(xmlTbt_InventorySlipDetail);

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_SLIP_DETAIL,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GeneratePurchaseOrderNo(string strRegionCode)
        {
            //try
            //{
            //    int iRunningNo = 0;
            //    string strRunningNo = string.Empty;
            //    string strYear = DateTime.Now.ToString("yy");
            //    string strMonth = DateTime.Now.Month.ToString("00");

            //    List<tbs_PurchaseOrderRunningNo> dtTbs_PurchaseOrderRunningNo = GetTbs_PurchaseOrderRunningNo(strYear, strMonth, NationCode.C_INV_NATION_CODE_NOT_JAPAN);
            //    if (dtTbs_PurchaseOrderRunningNo.Count > 0)
            //    {
            //        iRunningNo = (int.Parse(dtTbs_PurchaseOrderRunningNo[0].RunningNo) + 1);
            //        strRunningNo = iRunningNo.ToString("000");
            //        if (iRunningNo > int.Parse(SlipID.C_INV_SLIP_NO_MAXIMUM))
            //            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4076);
            //        else if (iRunningNo <= int.Parse(SlipID.C_INV_SLIP_NO_MAXIMUM))
            //            UpdateTbs_PurchaseOrderRunningNo(strYear, strMonth, NationCode.C_INV_NATION_CODE_NOT_JAPAN, strRunningNo);
            //    }
            //    else
            //    {
            //        strRunningNo = SlipID.C_INV_SLIP_NO_MINIMUM;
            //        tbs_PurchaseOrderRunningNo PorderNo = new tbs_PurchaseOrderRunningNo();
            //        PorderNo.YearCode = strYear;
            //        PorderNo.MonthCode = strMonth;
            //        PorderNo.NationCodeCode = NationCode.C_INV_NATION_CODE_NOT_JAPAN;
            //        PorderNo.RunningNo = strRunningNo;
            //    }
            //    string strPurchaseOrderNo = "AP" + NationCode.C_INV_NATION_CODE_NOT_JAPAN + strYear + strMonth + strRunningNo;
            //    return strPurchaseOrderNo;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

            try
            {
                string strNCode = "";
                if (strRegionCode == RegionCode.C_REGION_CODE_JAPAN)
                    strNCode = NationCode.C_INV_NATION_CODE_JAPAN;
                else
                    strNCode = NationCode.C_INV_NATION_CODE_NOT_JAPAN;

                var lstPurchaseOrderNo = base.GeneratePurchaseOrderNo(SlipID.C_INV_SLIP_NO_MAXIMUM, SlipID.C_INV_SLIP_NO_MINIMUM, strNCode);
                if (lstPurchaseOrderNo.Count > 0)
                {
                    return lstPurchaseOrderNo[0];
                }
                else
                {
                    throw new ApplicationException("Unable to generate new Purchase Order No");
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message != null && (ex.InnerException.Message == MessageUtil.MessageList.MSG4076.ToString()))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INVENTORY, MessageUtil.MessageList.MSG4076);
                }
                else
                {
                    throw ex;
                }
            }
        }
        // Akat K. CHECKED 2012-03-02
        public doCheckTransferQtyResult CheckTransferQty(doCheckTransferQty Cond)
        {
            try
            {

                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<tbt_InventoryCurrent> dtTbt_InventoryCurrent = InvH.GetTbt_InventoryCurrent(Cond.OfficeCode, Cond.LocationCode, Cond.AreaCode, Cond.ShelfNo, Cond.InstrumentCode);

                doCheckTransferQtyResult Result = new doCheckTransferQtyResult();
                if (dtTbt_InventoryCurrent.Count <= 0)
                {
                    Result.OverQtyFlag = null;
                    Result.CurrentyQty = 0;
                }
                else if (dtTbt_InventoryCurrent[0].InstrumentQty < Cond.TransferQty)
                {
                    Result.OverQtyFlag = true;
                    //Result.CurrentyQty = Convert.ToInt16(dtTbt_InventoryCurrent[0].InstrumentQty);
                    if (dtTbt_InventoryCurrent[0].InstrumentQty.HasValue)
                    {
                        Result.CurrentyQty = dtTbt_InventoryCurrent[0].InstrumentQty.Value;
                    }
                    else
                    {
                        Result.CurrentyQty = 0;
                    }
                }
                else
                {
                    Result.OverQtyFlag = false;
                    //Result.CurrentyQty = Convert.ToInt16(dtTbt_InventoryCurrent[0].InstrumentQty);
                    if (dtTbt_InventoryCurrent[0].InstrumentQty.HasValue)
                    {
                        Result.CurrentyQty = dtTbt_InventoryCurrent[0].InstrumentQty.Value;
                    }
                    else
                    {
                        Result.CurrentyQty = 0;
                    }
                }
                return Result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public doCheckTransferQtyResult CheckTransferQtyIVS190(doCheckTransferQty Cond)
        {
            try
            {
                IInventoryHandler InvH = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                List<tbt_InventoryCurrent> dtTbt_InventoryCurrent = InvH.GetTbt_InventoryCurrent(Cond.OfficeCode, Cond.LocationCode, Cond.AreaCode, Cond.ShelfNo, Cond.InstrumentCode);

                doCheckTransferQtyResult Result = new doCheckTransferQtyResult();
                if (dtTbt_InventoryCurrent.Count <= 0)
                {
                    Result.OverQtyFlag = null;
                    Result.CurrentyQty = 0;
                }
                else if (Math.Abs(GetIntNullableVal(dtTbt_InventoryCurrent[0].InstrumentQty)) < Cond.TransferQty)
                {
                    Result.OverQtyFlag = true;
                    if (dtTbt_InventoryCurrent[0].InstrumentQty.HasValue)
                    {
                        Result.CurrentyQty = dtTbt_InventoryCurrent[0].InstrumentQty.Value;
                    }
                    else
                    {
                        Result.CurrentyQty = 0;
                    }
                }
                else
                {
                    Result.OverQtyFlag = false;
                    if (dtTbt_InventoryCurrent[0].InstrumentQty.HasValue)
                    {
                        Result.CurrentyQty = Math.Abs(dtTbt_InventoryCurrent[0].InstrumentQty.Value);
                    }
                    else
                    {
                        Result.CurrentyQty = 0;
                    }
                }
                return Result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<tbt_PurchaseOrder> InsertTbt_PurchaseOrder(List<tbt_PurchaseOrder> lstPurchase)
        {
            try
            {
                List<tbt_PurchaseOrder> lst = base.InsertTbt_PurchaseOrder(CommonUtil.ConvertToXml_Store<tbt_PurchaseOrder>(lstPurchase));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_PURCHASE,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_PurchaseOrder> InsertTbt_PurchaseOrder_NoLog(List<tbt_PurchaseOrder> lstPurchase)
        {
            try
            {
                List<tbt_PurchaseOrder> lst = base.InsertTbt_PurchaseOrder(CommonUtil.ConvertToXml_Store<tbt_PurchaseOrder>(lstPurchase));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_PurchaseOrderDetail> InsertTbt_PurchaseOrderDetail(List<tbt_PurchaseOrderDetail> lstPurchaseDetail)
        {
            try
            {
                List<tbt_PurchaseOrderDetail> lst = base.InsertTbt_PurchaseOrderDetail(CommonUtil.ConvertToXml_Store<tbt_PurchaseOrderDetail>(lstPurchaseDetail));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_PURCHASE_DETAIL,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_PurchaseOrderDetail> InsertTbt_PurchaseOrderDetail_Nolog(List<tbt_PurchaseOrderDetail> lstPurchaseDetail)
        {
            try
            {
                List<tbt_PurchaseOrderDetail> lst = base.InsertTbt_PurchaseOrderDetail(CommonUtil.ConvertToXml_Store<tbt_PurchaseOrderDetail>(lstPurchaseDetail));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Akat K. CHECKED 2012-03-02
        public List<dtSearchInstrumentListResult> SearchInventoryInstrumentList(string officeCode, string locationCode, string areaCodeStr, string shelfType, string startShelfNo, string endShelfNo, string instrumentName, string instrumentCode, string[] excludeAreaCode = null)
        {
            try
            {
                string strArrayExcludeAreaCode = (excludeAreaCode == null ? null : CommonUtil.CreateCSVString(excludeAreaCode));
                List<dtSearchInstrumentListResult> lst = base.SearchInventoryInstrumentList(MiscType.C_INV_AREA, officeCode, locationCode, areaCodeStr, shelfType, endShelfNo, instrumentName, instrumentCode, startShelfNo, ConfigName.C_INV_AREA_SHORT, ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF, ShelfNo.C_INV_SHELF_NO_NOT_PRICE, strArrayExcludeAreaCode);
                CommonUtil.MappingObjectLanguage<dtSearchInstrumentListResult>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }


        }
        public List<dtSearchInstrumentListResult> SearchInventoryInstrumentList(doSearchInstrumentListCondition Cond)
        {
            try
            {

                string strAreaCode = null;
                if (!string.IsNullOrEmpty(Cond.AreaCode))
                {
                    strAreaCode = Cond.AreaCode;
                }
                if (!CommonUtil.IsNullOrEmpty(Cond.AreaCodeList))
                {
                    if (Cond.AreaCodeList.Count > 0)
                    {
                        strAreaCode = "," + string.Join(",", Cond.AreaCodeList) + ",";
                    }
                }

                string strArrayExcludeAreaCode = (Cond.ExcludeAreaCode == null ? null : CommonUtil.CreateCSVString(Cond.ExcludeAreaCode));

                List<dtSearchInstrumentListResult> lst = base.SearchInventoryInstrumentList(MiscType.C_INV_AREA, Cond.OfficeCode
                    , Cond.LocationCode
                    , strAreaCode
                    , Cond.ShelfType
                    , Cond.EndShelfNo,
                    Cond.InstrumentName,
                    Cond.Instrumentcode,
                    Cond.StartShelfNo,
                    ConfigName.C_INV_AREA_SHORT,
                    ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF,
                    ShelfNo.C_INV_SHELF_NO_NOT_PRICE,
                    strArrayExcludeAreaCode);

                CommonUtil.MappingObjectLanguage<dtSearchInstrumentListResult>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }


        }
        public List<dtSearchInstrumentListResult> SearchInventoryInstrumentListIVS190(doSearchInstrumentListConditionIVS190 Cond)
        {
            try
            {

                string strAreaCode = null;
                if (!string.IsNullOrEmpty(Cond.AreaCode))
                {
                    strAreaCode = Cond.AreaCode;
                }
                if (!CommonUtil.IsNullOrEmpty(Cond.AreaCodeList))
                {
                    if (Cond.AreaCodeList.Count > 0)
                    {
                        strAreaCode = "," + string.Join(",", Cond.AreaCodeList) + ",";
                    }
                }


                string strTransferType = "0";

                if (Cond.TransferType == true)
                    strTransferType = "1";


                List<dtSearchInstrumentListResult> lst = base.SearchInventoryInstrumentListIVS190(MiscType.C_INV_AREA, ConfigName.C_INV_AREA_SHORT
                    , Cond.OfficeCode
                    , Cond.LocationCode
                    , strAreaCode
                    , Cond.ShelfType
                    , Cond.StartShelfNo
                    , Cond.EndShelfNo
                    , Cond.InstrumentName
                    , Cond.Instrumentcode
                    , ShelfNo.C_INV_SHELF_NO_NOT_MOVE_SHELF
                    , ShelfNo.C_INV_SHELF_NO_NOT_PRICE
                    , strTransferType
                    , ConfigName.C_CONFIG_WILDCARD
                    , InstrumentArea.C_INV_AREA_SE_LENDING_DEMO);

                CommonUtil.MappingObjectLanguage<dtSearchInstrumentListResult>(lst);

                return lst;
            }
            catch (Exception)
            {
                throw;
            }


        }
        // Akat K. CHECKED 2012-03-02
        public doFIFOInstrumentPrice GetFIFOInstrumentPrice(int? strTransferQty, string strOfficeCode, string strLocationCode, string strInstrumentCode, int? strPrevInstrumentQty)
        {
            try
            {
                List<doFIFOInstrumentPrice> res =  base.GetFIFOInstrumentPrice(strTransferQty, strOfficeCode, strLocationCode, strInstrumentCode, strPrevInstrumentQty, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                return res[0];
            }
            catch (Exception)
            {
                throw;
            }

        }
        // Akat K. CHECKED 2012-03-02
        public doCalPriceCondition GetMovingAveragePriceCondition(string strOfficeCode, string strContractCode, string strProjectCode, string strInstrumentCode, string[] strLocationCode, string strLotNo)
        {
            try
            {

                string strArrayLoc = CommonUtil.CreateCSVString(strLocationCode);

                //foreach (string i in strLocationCode)
                //    strArrayLoc += i + ",";
                //if (strArrayLoc.Trim() == "")
                //    strArrayLoc = null;


                List<doCalPriceCondition> lst = base.GetMovingAveragePriceCondition(strOfficeCode, strContractCode, strProjectCode, strInstrumentCode, strArrayLoc, strLotNo, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                if (lst.Count > 0)
                    return lst[0];
                else
                    return new doCalPriceCondition();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string RegisterTransferInstrument(doRegisterTransferInstrumentData Cond)
        {
            try
            {
                string strInventorySlipNo;

                if (Cond.InventorySlip.TransferTypeCode != TransferType.C_INV_TRANSFERTYPE_PROJECT_COMPLETE)
                {
                    strInventorySlipNo = GenerateInventorySlipNo(Cond.InventorySlip.SourceOfficeCode, Cond.SlipId);
                }
                else
                {
                    strInventorySlipNo = Cond.InventorySlip.ProjectCode;
                }

                Cond.InventorySlip.SlipNo = strInventorySlipNo;
                int runningNumber = 1;

                var lstInstockLoc = this.GetInventoryStockLocations();
                string accountcode = lstInstockLoc.Contains(Cond.InventorySlip.SourceLocationCode) 
                    ? InventoryAccountCode.C_INV_ACCOUNT_CODE_INSTOCK 
                    : InventoryAccountCode.C_INV_ACCOUNT_CODE_INPROCESS;

                foreach (tbt_InventorySlipDetail i in Cond.lstTbt_InventorySlipDetail)
                {
                    i.SlipNo = strInventorySlipNo;
                    i.RunningNo = runningNumber;

                    var avgprice = this.GetMonthlyAveragePrice(i.InstrumentCode, Cond.InventorySlip.SlipIssueDate, accountcode, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
                    i.InstrumentAmount = i.TransferQty * avgprice;

                    runningNumber++;
                }

                List<tbt_InventorySlip> lst = new List<tbt_InventorySlip>();
                lst.Add(Cond.InventorySlip);
                //4.1
                List<tbt_InventorySlip> doSlip = InsertTbt_InventorySlip(lst);
                if (doSlip.Count <= 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_SLIP });
                }

                List<tbt_InventorySlipDetail> doSlipDetail = InsertTbt_InventorySlipDetail(Cond.lstTbt_InventorySlipDetail);

                if (doSlipDetail.Count <= 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_SLIP_DETAIL });
                }

                //5

                if (CommonUtil.dsTransData.dtTransHeader.ScreenID == ScreenID.C_INV_SCREEN_ID_REPAIR_REQUEST)
                {
                    Cond.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_REPAIR_REQUEST;
                }
                else if (CommonUtil.dsTransData.dtTransHeader.ScreenID == ScreenID.C_INV_SCREEN_ID_REPAIR_RETURN)
                {
                    Cond.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_REPAIR_RETURN;
                }
                else if (CommonUtil.dsTransData.dtTransHeader.ScreenID == ScreenID.C_INV_SCREEN_ID_TRANSFER_OFFICE)
                {
                    Cond.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_TRANSFER;
                    foreach (tbt_InventorySlipDetail d in Cond.lstTbt_InventorySlipDetail)
                        d.DestinationShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;

                    // The Yellow hi-light
                }
                else if (Cond.InventorySlip.DestinationLocationCode == InstrumentLocation.C_INV_LOC_RETURNED)
                {
                    if (Cond.InventorySlip.SourceLocationCode == InstrumentLocation.C_INV_LOC_WIP ||
                        Cond.InventorySlip.SourceLocationCode == InstrumentLocation.C_INV_LOC_UNOPERATED_WIP ||
                        Cond.InventorySlip.SourceLocationCode == InstrumentLocation.C_INV_LOC_PROJECT_WIP)
                    {
                        Cond.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_WAITING_RETURN;
                    }
                    else if (Cond.InventorySlip.SourceLocationCode == InstrumentLocation.C_INV_LOC_USER)
                    {
                        Cond.InventorySlip.DestinationLocationCode = InstrumentLocation.C_INV_LOC_RETURN_WIP;
                    }
                }

                //5.2
                //    List<tbt_InventoryCurrent> CurrentForUpdate = new List<tbt_InventoryCurrent>();
                List<tbt_InventoryCurrent> lstOriginCurrent = GetTbt_InventoryCurrent(Cond.InventorySlip.SourceOfficeCode, Cond.InventorySlip.SourceLocationCode, null, null, null);
                foreach (tbt_InventorySlipDetail i in Cond.lstTbt_InventorySlipDetail)
                {
                    if (Cond.InventorySlip.TransferType == false)
                    {
                        i.TransferQty = i.TransferQty * -1;
                    }

                    List<tbt_InventoryCurrent> matchCurrent = (from c in lstOriginCurrent
                                                               where c.AreaCode == i.SourceAreaCode && c.ShelfNo == i.SourceShelfNo
                                                                   && string.Compare(c.InstrumentCode, i.InstrumentCode, true) == 0 //Compare with ignorecase.
                                                               select c).ToList<tbt_InventoryCurrent>();
                    if (matchCurrent.Count > 0)
                    {
                        matchCurrent[0].InstrumentQty = GetIntNullableVal(matchCurrent[0].InstrumentQty) - GetIntNullableVal(i.TransferQty);
                        matchCurrent[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        matchCurrent[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        List<tbt_InventoryCurrent> CurrentForUpdate = new List<tbt_InventoryCurrent>();
                        CurrentForUpdate.Add(matchCurrent[0]);
                        List<tbt_InventoryCurrent> doTbt_InventoryCurrent = UpdateTbt_InventoryCurrent(CurrentForUpdate);

                        if (doTbt_InventoryCurrent.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_CURRENT });
                        }
                    }
                    else
                    {
                        tbt_InventoryCurrent doTbt_InventoryCurrent = new tbt_InventoryCurrent();
                        doTbt_InventoryCurrent.OfficeCode = Cond.InventorySlip.SourceOfficeCode;
                        doTbt_InventoryCurrent.LocationCode = Cond.InventorySlip.SourceLocationCode;
                        doTbt_InventoryCurrent.AreaCode = i.SourceAreaCode;
                        doTbt_InventoryCurrent.ShelfNo = i.SourceShelfNo;
                        doTbt_InventoryCurrent.InstrumentCode = i.InstrumentCode;
                        doTbt_InventoryCurrent.InstrumentQty = 0 - i.TransferQty;
                        doTbt_InventoryCurrent.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doTbt_InventoryCurrent.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        doTbt_InventoryCurrent.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        doTbt_InventoryCurrent.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        List<tbt_InventoryCurrent> CurrentForInsert = new List<tbt_InventoryCurrent>();
                        CurrentForInsert.Add(doTbt_InventoryCurrent);
                        List<tbt_InventoryCurrent> doTbt_InventoryCurrentResult = InsertTbt_InventoryCurrent(CurrentForInsert);

                        if (doTbt_InventoryCurrentResult.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_CURRENT });
                        }

                    }

                    // The Yellow hi-light
                    if (Cond.InventorySlip.DestinationLocationCode == InstrumentLocation.C_INV_LOC_ELIMINATION
                        || Cond.InventorySlip.DestinationLocationCode == InstrumentLocation.C_INV_LOC_SPECIAL
                        || Cond.InventorySlip.DestinationLocationCode == InstrumentLocation.C_INV_LOC_LOSS
                        || Cond.InventorySlip.DestinationLocationCode == InstrumentLocation.C_INV_LOC_SOLD)
                    {
                        continue;
                    }

                    string destLocation = Cond.InventorySlip.DestinationLocationCode;
                    if (Cond.InventorySlip.TransferTypeCode == TransferType.C_INV_TRANSFERTYPE_CANCEL_INSTALLATION)
                    {
                        destLocation = InstrumentLocation.C_INV_LOC_WAITING_RETURN;
                    }

                    List<tbt_InventoryCurrent> DestCurrent = GetTbt_InventoryCurrent(Cond.InventorySlip.DestinationOfficeCode, destLocation, i.DestinationAreaCode, i.DestinationShelfNo, i.InstrumentCode);
                    if (DestCurrent.Count <= 0)
                    {
                        tbt_InventoryCurrent Current = new tbt_InventoryCurrent();
                        Current.OfficeCode = Cond.InventorySlip.DestinationOfficeCode;
                        Current.LocationCode = destLocation;
                        Current.AreaCode = i.DestinationAreaCode;
                        Current.ShelfNo = i.DestinationShelfNo;
                        Current.InstrumentCode = i.InstrumentCode;
                        Current.InstrumentQty = i.TransferQty;
                        Current.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        Current.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        Current.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        Current.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        List<tbt_InventoryCurrent> CurrentForInsert = new List<tbt_InventoryCurrent>();
                        CurrentForInsert.Add(Current);
                        List<tbt_InventoryCurrent> doTbt_InventoryCurrent = InsertTbt_InventoryCurrent(CurrentForInsert);

                        if (doTbt_InventoryCurrent.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_CURRENT });
                        }
                    }
                    else
                    {
                        DestCurrent[0].InstrumentQty = GetIntNullableVal(DestCurrent[0].InstrumentQty) + GetIntNullableVal(i.TransferQty);
                        DestCurrent[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        DestCurrent[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_InventoryCurrent> DestCurrentForUpdate = new List<tbt_InventoryCurrent>();
                        DestCurrentForUpdate.Add(DestCurrent[0]);
                        List<tbt_InventoryCurrent> doTbt_InventoryCurrent = UpdateTbt_InventoryCurrent(DestCurrentForUpdate);

                        if (doTbt_InventoryCurrent.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_CURRENT });
                        }
                    }
                }

                return strInventorySlipNo;
            }
            catch (Exception)
            {

                throw;
            }

        }

        // Akat K. CHECKED 2012-03-01
        public int CheckNewInstrument(string slipNo)
        {
            try
            {
                int result = 0;
                List<int?> list = base.CheckNewInstrument(slipNo,
                                        InstrumentArea.C_INV_AREA_NEW_SALE,
                                        InstrumentArea.C_INV_AREA_NEW_RENTAL,
                                        InstrumentArea.C_INV_AREA_NEW_DEMO);

                if (list.Count > 0)
                {
                    result = list[0].HasValue ? list[0].Value : 0;
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Akat K. CHECKED 2012-03-01
        public List<doGroupNewInstrument> GetGroupNewInstrument(string slipNo)
        {
            try
            {
                return base.GetGroupNewInstrument(InstrumentArea.C_INV_AREA_NEW_SALE, InstrumentArea.C_INV_AREA_NEW_RENTAL, InstrumentArea.C_INV_AREA_NEW_DEMO, slipNo);
            }
            catch (Exception)
            {
                throw;
            }

        }

        // Akat K. CHECKED 2012-03-01
        public int CheckSecondhandInstrument(string strInventorySlipNo)
        {
            try
            {
                int result = 0;
                var list = base.CheckSecondhandInstrument(strInventorySlipNo, InstrumentArea.C_INV_AREA_SE_RENTAL, InstrumentArea.C_INV_AREA_SE_LENDING_DEMO, InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO);
                if (list.Count > 0)
                {
                    result = list[0].HasValue ? list[0].Value : 0;
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }

        // Akat K. CHECKED 2012-03-01
        public List<doGroupSecondhandInstrument> GetGroupSecondhandInstrument(string strInventorySlipNo)
        {
            try
            {
                return base.GetGroupSecondhandInstrument(strInventorySlipNo, InstrumentArea.C_INV_AREA_SE_RENTAL, InstrumentArea.C_INV_AREA_SE_HANDLING_DEMO, InstrumentArea.C_INV_AREA_SE_LENDING_DEMO);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Akat K. CHECKED 2012-03-01
        public bool UpdateAccountTransferSecondhandInstrument(doGroupSecondhandInstrument doGroupSecondhand)
        {
            try
            {
                if (doGroupSecondhand.DestinationLocationCode == InstrumentLocation.C_INV_LOC_RETURNED)
                {
                    if (doGroupSecondhand.SourceLocationCode == InstrumentLocation.C_INV_LOC_WIP
                        || doGroupSecondhand.SourceLocationCode == InstrumentLocation.C_INV_LOC_UNOPERATED_WIP
                        || doGroupSecondhand.SourceLocationCode == InstrumentLocation.C_INV_LOC_PROJECT_WIP
                        || doGroupSecondhand.SourceLocationCode == InstrumentLocation.C_INV_LOC_PARTIAL_OUT         // - Add by Nontawat L. on 09-Jul-2014: Phase4: 3.44
                        )
                    {
                        doGroupSecondhand.DestinationLocationCode = InstrumentLocation.C_INV_LOC_WAITING_RETURN;
                    }
                    else if (doGroupSecondhand.SourceLocationCode == InstrumentLocation.C_INV_LOC_USER)
                    {

                        doGroupSecondhand.DestinationLocationCode = InstrumentLocation.C_INV_LOC_RETURN_WIP;
                    }
                }

                bool swapBit = false;
                if (InstrumentLocation.C_INV_LOC_BUFFER.Equals(doGroupSecondhand.SourceLocationCode)
                    && (!doGroupSecondhand.TransferQty.HasValue || doGroupSecondhand.TransferQty.Value == 0))
                {
                    swapBit = true;
                    doGroupSecondhand.SourceLocationCode = InstrumentLocation.C_INV_LOC_INSTOCK;
                }

                List<doFIFOInstrument> FifoInstrument = GetFIFOInstrument(doGroupSecondhand.SourceOfficeCode, doGroupSecondhand.SourceLocationCode, doGroupSecondhand.Instrumentcode);
                if (swapBit)
                {
                    doGroupSecondhand.SourceLocationCode = InstrumentLocation.C_INV_LOC_BUFFER;
                }

                //1.1
                int iTransferQty = Convert.ToInt32(doGroupSecondhand.TransferQty);
                int iSourceQty = 0;
                int iDestinationQty = 0;
                List<tbt_AccountInstalled> lstDestMatchAcc = GetTbt_AccountInstalled(doGroupSecondhand.DestinationOfficeCode, doGroupSecondhand.DestinationLocationCode, doGroupSecondhand.Instrumentcode, null);
                List<tbt_AccountInstalled> lstSrcMatchAcc = GetTbt_AccountInstalled(doGroupSecondhand.SourceOfficeCode, doGroupSecondhand.SourceLocationCode, doGroupSecondhand.Instrumentcode, null);

                foreach (doFIFOInstrument i in FifoInstrument)
                {
                    if (iTransferQty <= 0)
                    {
                        break;
                    }

                    if (i.InstrumentQty == 0)
                    {
                        continue;
                    }

                    if (i.InstrumentQty <= iTransferQty)
                    {
                        iSourceQty = 0;
                        iDestinationQty = Convert.ToInt32(i.InstrumentQty);
                    }
                    else
                    {
                        iSourceQty = Convert.ToInt32(i.InstrumentQty) - iTransferQty;
                        //iDestinationQty = Convert.ToInt32(i.InstrumentQty);
                        iDestinationQty = iTransferQty;
                    }

                    if (doGroupSecondhand.TransferType == false)
                    {
                        iSourceQty = iSourceQty * -1;
                    }

                    iTransferQty = iTransferQty - Convert.ToInt32(i.InstrumentQty);

                    List<tbt_AccountInstalled> matchSrcAcc = (from c in lstSrcMatchAcc where c.LotNo == i.LotNo select c).ToList<tbt_AccountInstalled>();
                    if (matchSrcAcc.Count > 0)
                    {
                        matchSrcAcc[0].OfficeCode = doGroupSecondhand.SourceOfficeCode;
                        matchSrcAcc[0].LocationCode = doGroupSecondhand.SourceLocationCode;
                        matchSrcAcc[0].LotNo = i.LotNo;
                        matchSrcAcc[0].InstrumentCode = doGroupSecondhand.Instrumentcode;
                        matchSrcAcc[0].InstrumentQty = iSourceQty;

                        matchSrcAcc[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        matchSrcAcc[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        List<tbt_AccountInstalled> lstForUpdate = new List<tbt_AccountInstalled>();
                        lstForUpdate.Add(matchSrcAcc[0]);
                        List<tbt_AccountInstalled> resultUpdate = UpdateTbt_AccountInstalled(lstForUpdate);
                        if (resultUpdate.Count == 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }

                    if (InstrumentLocation.C_INV_LOC_ELIMINATION.Equals(doGroupSecondhand.DestinationLocationCode)
                        || InstrumentLocation.C_INV_LOC_SPECIAL.Equals(doGroupSecondhand.DestinationLocationCode)
                        || InstrumentLocation.C_INV_LOC_LOSS.Equals(doGroupSecondhand.DestinationLocationCode)
                        || InstrumentLocation.C_INV_LOC_SOLD.Equals(doGroupSecondhand.DestinationLocationCode))
                    {
                        continue;
                    }

                    //1.1.3
                    List<tbt_AccountInstalled> matchDestAcc = (from c in lstDestMatchAcc where c.LotNo == i.LotNo select c).ToList<tbt_AccountInstalled>();
                    if (matchDestAcc.Count <= 0)
                    {
                        tbt_AccountInstalled AccForInsert = new tbt_AccountInstalled();
                        AccForInsert.OfficeCode = doGroupSecondhand.DestinationOfficeCode;
                        AccForInsert.LocationCode = doGroupSecondhand.DestinationLocationCode;
                        AccForInsert.LotNo = i.LotNo;
                        AccForInsert.InstrumentCode = doGroupSecondhand.Instrumentcode;
                        AccForInsert.InstrumentQty = iDestinationQty;
                        AccForInsert.AccquisitionCost = i.AccquisitionCost;
                        AccForInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        AccForInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        AccForInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        AccForInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        matchDestAcc.Add(AccForInsert);
                        List<tbt_AccountInstalled> resultInsert = InsertTbt_AccountInstalled(matchDestAcc);
                        if (resultInsert.Count == 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }
                    else
                    {
                        matchDestAcc[0].OfficeCode = doGroupSecondhand.DestinationOfficeCode;
                        matchDestAcc[0].LocationCode = doGroupSecondhand.DestinationLocationCode;
                        matchDestAcc[0].LotNo = i.LotNo;
                        matchDestAcc[0].InstrumentCode = doGroupSecondhand.Instrumentcode;
                        matchDestAcc[0].InstrumentQty = matchDestAcc[0].InstrumentQty + iDestinationQty;
                        matchDestAcc[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        matchDestAcc[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        List<tbt_AccountInstalled> lstForUpDestAcc = new List<tbt_AccountInstalled>();
                        lstForUpDestAcc.Add(matchDestAcc[0]);
                        List<tbt_AccountInstalled> resultUpdate = UpdateTbt_AccountInstalled(lstForUpDestAcc);
                        if (resultUpdate.Count == 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_ACC_INSTALLED });
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<tbt_AccountInstalled> UpdateTbt_AccountInstalled(List<tbt_AccountInstalled> lstAcc)
        {
            try
            {
                List<tbt_AccountInstalled> lst = base.UpdateTbt_AccountInstalled(CommonUtil.ConvertToXml_Store<tbt_AccountInstalled>(lstAcc));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_ACC_INSTALLED,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<tbt_AccountInstalled> UpdateTbt_AccountInstalled_NoLog(List<tbt_AccountInstalled> lstAcc)
        {
            try
            {
                List<tbt_AccountInstalled> lst = base.UpdateTbt_AccountInstalled(CommonUtil.ConvertToXml_Store<tbt_AccountInstalled>(lstAcc));

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<tbt_AccountInstalled> InsertTbt_AccountInstalled(List<tbt_AccountInstalled> lstAcc)
        {
            try
            {
                List<tbt_AccountInstalled> lst = base.InsertTbt_AccountInstalled(CommonUtil.ConvertToXml_Store<tbt_AccountInstalled>(lstAcc));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_ACC_INSTALLED,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<tbt_AccountInstalled> InsertTbt_AccountInstalled_NoLog(List<tbt_AccountInstalled> lstAcc)
        {
            try
            {
                List<tbt_AccountInstalled> lst = base.InsertTbt_AccountInstalled(CommonUtil.ConvertToXml_Store<tbt_AccountInstalled>(lstAcc));

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Akat K. CHECKED 2012-03-02
        public int CheckSampleInstrument(string strInventorySlipNo)
        {
            try
            {
                int result = 0;
                var list = base.CheckSampleInstrument(strInventorySlipNo, InstrumentArea.C_INV_AREA_NEW_SAMPLE);
                if (list.Count > 0)
                {
                    result = list[0].HasValue ? list[0].Value : 0;
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }
        // Akat K. CHECKED 2012-03-02
        public List<doInventorySlip> SearchInventorySlip(string strInventorySlipNo)
        {
            try
            {
                List<doInventorySlip> lst = base.SearchInventorySlip(MiscType.C_INV_LOC, strInventorySlipNo);
                CommonUtil.MappingObjectLanguage<doInventorySlip>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Akat K. CHECKED 2012-03-02
        public List<doInventorySlipDetail> SearchInventorySlipDetail(string strInventorySlipNo)
        {
            try
            {
                List<doInventorySlipDetail> lst = base.SearchInventorySlipDetail(MiscType.C_INV_AREA, strInventorySlipNo);
                CommonUtil.MappingObjectLanguage<doInventorySlipDetail>(lst);
                return lst;

            }
            catch (Exception)
            {
                throw;
            }
        }

        // Akat K. CHECKED 01-03-2012
        public bool RegisterReceiveInstrument(string strInventorySlipNo, string strMemo, string strApproveNo)
        {
            //1
            List<tbt_InventorySlip> lstSlip = this.GetTbt_InventorySlip(strInventorySlipNo);
            List<tbt_InventorySlipDetail> lstSlipDetail = base.GetTbt_InventorySlipDetail(strInventorySlipNo, null);

            //2
            if (lstSlip != null && lstSlip.Count > 0)
            {
                if (CommonUtil.dsTransData.dtTransHeader.ScreenID == ScreenID.C_INV_SCREEN_ID_REPAIR_REQUEST_RECEIVE)
                {
                    lstSlip[0].SourceLocationCode = InstrumentLocation.C_INV_LOC_REPAIR_REQUEST;

                    #region R2
                    foreach (tbt_InventorySlipDetail i in lstSlipDetail)
                        i.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    #endregion
                }
                else if (CommonUtil.dsTransData.dtTransHeader.ScreenID == ScreenID.C_INV_SCREEN_ID_REPAIR_RETURN_RECEIVE)
                    lstSlip[0].SourceLocationCode = InstrumentLocation.C_INV_LOC_REPAIR_RETURN;
                else if (CommonUtil.dsTransData.dtTransHeader.ScreenID == ScreenID.C_INV_SCREEN_ID_TRANSFER_OFFICE_RECEIVE)
                {
                    //IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                    //if (!handler.CheckInventoryHeadOffice(lstSlip[0].DestinationOfficeCode))
                    //{
                    //    foreach (tbt_InventorySlipDetail i in lstSlipDetail)
                    //        i.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;
                    //}

                    foreach (tbt_InventorySlipDetail i in lstSlipDetail)
                        i.SourceShelfNo = ShelfNo.C_INV_SHELF_NO_OTHER_LOCATION;

                    lstSlip[0].SourceLocationCode = InstrumentLocation.C_INV_LOC_TRANSFER;
                    lstSlip[0].SourceOfficeCode = lstSlip[0].DestinationOfficeCode;
                }
                else if (CommonUtil.dsTransData.dtTransHeader.ScreenID == ScreenID.C_INV_SCREEN_ID_RECEIVE_RETURN)
                {
                    if (InstrumentLocation.C_INV_LOC_UNOPERATED_WIP.Equals(lstSlip[0].SourceLocationCode)
                        || InstrumentLocation.C_INV_LOC_WIP.Equals(lstSlip[0].SourceLocationCode)
                        || InstrumentLocation.C_INV_LOC_PROJECT_WIP.Equals(lstSlip[0].SourceLocationCode))
                    {
                        lstSlip[0].SourceLocationCode = InstrumentLocation.C_INV_LOC_WAITING_RETURN;
                    }
                    else if (InstrumentLocation.C_INV_LOC_USER.Equals(lstSlip[0].SourceLocationCode))
                    {
                        lstSlip[0].SourceLocationCode = InstrumentLocation.C_INV_LOC_RETURN_WIP;
                    }
                }
            }

            if (lstSlip != null && lstSlip.Count > 0)
            {
                foreach (tbt_InventorySlipDetail i in lstSlipDetail)
                {
                    List<tbt_InventoryCurrent> lstSourceCurrent = GetTbt_InventoryCurrent(lstSlip[0].SourceOfficeCode, lstSlip[0].SourceLocationCode, i.SourceAreaCode, i.SourceShelfNo, i.InstrumentCode);
                    if (lstSourceCurrent.Count > 0)
                    {
                        lstSourceCurrent[0].InstrumentQty = lstSourceCurrent[0].InstrumentQty - i.TransferQty;
                        lstSourceCurrent[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        lstSourceCurrent[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        //3.1.1;
                        List<tbt_InventoryCurrent> lstdoTbt_InventoryCurrent = UpdateTbt_InventoryCurrent(lstSourceCurrent);
                        //3.1.2
                        if (lstdoTbt_InventoryCurrent == null || lstdoTbt_InventoryCurrent.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_CURRENT });
                        }
                    }

                    //3.2.1
                    List<tbt_InventoryCurrent> lstDestinationCurrent = GetTbt_InventoryCurrent(lstSlip[0].DestinationOfficeCode, lstSlip[0].DestinationLocationCode, i.DestinationAreaCode, i.DestinationShelfNo, i.InstrumentCode);

                    //3.3.1
                    if (lstDestinationCurrent.Count <= 0)
                    {
                        tbt_InventoryCurrent InvenCurr = new tbt_InventoryCurrent();
                        InvenCurr.OfficeCode = lstSlip[0].DestinationOfficeCode;
                        InvenCurr.LocationCode = lstSlip[0].DestinationLocationCode;
                        InvenCurr.AreaCode = i.DestinationAreaCode;
                        InvenCurr.ShelfNo = i.DestinationShelfNo;
                        InvenCurr.InstrumentCode = i.InstrumentCode;
                        InvenCurr.InstrumentQty = i.TransferQty;
                        InvenCurr.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        InvenCurr.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        InvenCurr.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        InvenCurr.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_InventoryCurrent> lst = new List<tbt_InventoryCurrent>();
                        lst.Add(InvenCurr);

                        List<tbt_InventoryCurrent> lstResult = InsertTbt_InventoryCurrent(lst);
                        if (lstResult == null || lstResult.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_CURRENT });
                        }
                    }
                    else
                    {
                        lstDestinationCurrent[0].InstrumentQty = lstDestinationCurrent[0].InstrumentQty + i.TransferQty;
                        lstDestinationCurrent[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        lstDestinationCurrent[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        List<tbt_InventoryCurrent> lstResult = UpdateTbt_InventoryCurrent(lstDestinationCurrent);
                        if (lstResult == null || lstResult.Count <= 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_CURRENT });
                        }
                    }
                }
            }

            //4
            List<tbt_InventorySlip> lstUpdateSlip = GetTbt_InventorySlip(strInventorySlipNo);
            if (lstUpdateSlip.Count > 0)
            {
                lstUpdateSlip[0].SlipStatus = InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE;
                lstUpdateSlip[0].StockInDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                //Comment by Jutarat A. on 30052013 (Set at UpdateTbt_InventorySlip())
                //lstUpdateSlip[0].UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //lstUpdateSlip[0].UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                //End Comment

                if (!string.IsNullOrEmpty(strMemo))
                    lstUpdateSlip[0].Memo = strMemo;
                if (!string.IsNullOrEmpty(strApproveNo))
                    lstUpdateSlip[0].ApproveNo = strApproveNo;
            }

            List<tbt_InventorySlip> doTbt_InventorySlip = UpdateTbt_InventorySlip(lstUpdateSlip);
            if (doTbt_InventorySlip == null || doTbt_InventorySlip.Count <= 0)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0148, new string[] { TableName.C_TBL_NAME_INV_SLIP });
            }

            return true;
        }
        // Akat K. CHECKED 2012-03-02
        public List<doGroupSampleInstrument> GetGroupSampleInstrument(string strInventorySlipNo)
        {
            try
            {
                return base.GetGroupSampleInstrument(strInventorySlipNo, InstrumentArea.C_INV_AREA_NEW_SAMPLE);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<tbt_AccountStockMoving> InsertAccountStockMoving(List<tbt_AccountStockMoving> lstStock)
        {
            try
            {
                // Do not insert data when DestinationLocationCode = InstrumentLocation.C_INV_LOC_WAITING_RETURN
                var lstFilter = lstStock.Where(d => d.DestinationLocationCode != InstrumentLocation.C_INV_LOC_WAITING_RETURN).ToList();
                // Return original data if no data will be inserted.
                if (lstFilter.Count == 0)
                {
                    return lstStock;
                }
                string xml = CommonUtil.ConvertToXml_Store<tbt_AccountStockMoving>(lstFilter);

                List<tbt_AccountStockMoving> lst = base.InsertAccountStockMoving(xml);

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<tbt_AccountStockMoving> InsertAccountStockMoving_NoLog(List<tbt_AccountStockMoving> lstStock)
        {
            try
            {
                // Do not insert data when DestinationLocationCode = InstrumentLocation.C_INV_LOC_WAITING_RETURN
                var lstFilter = lstStock.Where(d => d.DestinationLocationCode != InstrumentLocation.C_INV_LOC_WAITING_RETURN).ToList();
                // Return original data if no data will be inserted.
                if (lstFilter.Count == 0)
                {
                    return lstStock;
                }
                string xml = CommonUtil.ConvertToXml_Store<tbt_AccountStockMoving>(lstFilter);

                List<tbt_AccountStockMoving> lst = base.InsertAccountStockMoving(xml);

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<doOffice> GetInventoryOffice()
        {
            List<doOffice> lst = base.GetInventoryOffice(LogisticFunction.C_OFFICELOGISTIC_NONE);
            CommonUtil.MappingObjectLanguage<doOffice>(lst);
            return lst;
        }
        public override List<doOffice> GetAuthorityOffice(string EmpNo)
        {
            List<doOffice> lst = base.GetAuthorityOffice(EmpNo);
            CommonUtil.MappingObjectLanguage<doOffice>(lst);
            return lst;
        }
        // Akat K. CHECKED 2012-03-02
        int IInventoryHandler.CheckFreezedData()
        {
            try
            {
                return Convert.ToInt32(base.CheckFreezedData()[0]);
            }
            catch (Exception)
            {

                throw;
            }

        }
        // Akat K. CHECKED 2012-03-02
        int IInventoryHandler.CheckStartedStockChecking()
        {
            try
            {
                return Convert.ToInt32(base.CheckStartedStockChecking()[0]);
            }
            catch (Exception)
            {

                throw;
            }

        }

        int IInventoryHandler.CheckImplementStockChecking()
        {
            try
            {
                return Convert.ToInt32(base.CheckImplementStockChecking()[0]);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public List<doResultGetReturnSlip> GetReturnedSlip(string strInstallationSlipNo)
        {
            try
            {
                List<doResultGetReturnSlip> lst = base.GetReturnedSlip(strInstallationSlipNo, MiscType.C_SALE_INSTALL_TYPE, MiscType.C_RENTAL_INSTALL_TYPE, SlipStatus.C_SLIP_STATUS_WAIT_FOR_RETURN);
                CommonUtil.MappingObjectLanguage<doResultGetReturnSlip>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<doResultReturnInstrument> GetReturnInstrumentByInstallationSlip(string strInstallationSlipNo)
        {
            try
            {
                List<doResultReturnInstrument> lst = base.GetReturnInstrumentByInstallationSlip(strInstallationSlipNo, InstrumentLocation.C_INV_LOC_RETURNED, InstrumentLocation.C_INV_LOC_ELIMINATION, InventorySlipStatus.C_INV_SLIP_STATUS_TRANSFER, MiscType.C_INV_AREA);
                CommonUtil.MappingObjectLanguage<doResultReturnInstrument>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<doTbt_InventorySlipDetailForView> GetTbt_InventorySlipDetailForView(string strInventorySlipNo)
        {
            try
            {
                List<doTbt_InventorySlipDetailForView> lst = base.GetTbt_InventorySlipDetailForView(strInventorySlipNo, MiscType.C_INV_AREA);
                CommonUtil.MappingObjectLanguage<doTbt_InventorySlipDetailForView>(lst);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<tbt_InventorySlip> GetTbt_InventorySlipForReceiveReturn(string strInstallationSlipNo)
        {
            try
            {
                List<tbt_InventorySlip> lst = base.GetTbt_InventorySlipForReceiveReturn(strInstallationSlipNo, InstrumentLocation.C_INV_LOC_RETURNED, InventorySlipStatus.C_INV_SLIP_STATUS_COMPLETE);
                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<doPurchaseOrderDetail> GetPurchaseOrderDetailForMaintain(string strPurchaseOrder)
        {
            List<doPurchaseOrderDetail> lst = base.GetPurchaseOrderDetailForMaintain(MiscType.C_CURRENCY_TYPE, MiscType.C_PURCHASE_ORDER_STATUS, MiscType.C_TRANSPORT_TYPE, strPurchaseOrder);
            CommonUtil.MappingObjectLanguage<doPurchaseOrderDetail>(lst);
            return lst;
        }

        public List<tbt_PurchaseOrderDetail> UpdateTbt_PurchaseOrderDetail(List<tbt_PurchaseOrderDetail> lstPurchase)
        {
            try
            {
                List<tbt_PurchaseOrderDetail> lst = base.UpdateTbt_PurchaseOrderDetail(CommonUtil.ConvertToXml_Store<tbt_PurchaseOrderDetail>(lstPurchase));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_PURCHASE_DETAIL,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<tbt_PurchaseOrderDetail> UpdateTbt_PurchaseOrderDetail_NoLog(List<tbt_PurchaseOrderDetail> lstPurchase)
        {
            try
            {
                List<tbt_PurchaseOrderDetail> lst = base.UpdateTbt_PurchaseOrderDetail(CommonUtil.ConvertToXml_Store<tbt_PurchaseOrderDetail>(lstPurchase));

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override List<tbt_PurchaseOrderDetail> UpdateTbt_PurchaseOrderDetail(string xmlPurchasOrderDetail)
        {
            try
            {
                List<tbt_PurchaseOrderDetail> lst = base.UpdateTbt_PurchaseOrderDetail(xmlPurchasOrderDetail);

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_PURCHASE_DETAIL,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override List<tbt_PurchaseOrder> UpdateTbt_PurchaseOrder(string xmlPurchaseOrder)
        {
            try
            {
                List<tbt_PurchaseOrder> lst = base.UpdateTbt_PurchaseOrder(xmlPurchaseOrder);

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_PURCHASE,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        List<tbt_AccountInstock> IInventoryHandler.GetTbt_AccountInStock(string instrumentCode, string locationCode, string officecode)
        {
            try
            {
                return base.GetTbt_AccountInStock(instrumentCode, locationCode, officecode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<tbt_AccountStockMoving> InsertTbt_AccountStockMoving(List<tbt_AccountStockMoving> lstStock)
        {
            try
            {
                List<tbt_AccountStockMoving> lst = base.InsertTbt_AccountStockMoving(CommonUtil.ConvertToXml_Store<tbt_AccountStockMoving>(lstStock));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_ACC_STOCK_MOVING,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<tbt_AccountStockMoving> InsertTbt_AccountStockMoving_NoLog(List<tbt_AccountStockMoving> lstStock)
        {
            try
            {
                List<tbt_AccountStockMoving> lst = base.InsertTbt_AccountStockMoving(CommonUtil.ConvertToXml_Store<tbt_AccountStockMoving>(lstStock));

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<doShelfCurrentData> GetShelfCurrentData(doGetShelfCurrentData doGetShelf)
        {
            try
            {
                return base.GetShelfCurrentData(doGetShelf.OfficeCode, doGetShelf.LocationCode, doGetShelf.ShelfNo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<doShelfCurrentData> GetShelfForChecking(doGetShelfCurrentData doGetShelf)
        {
            try
            {
                return base.GetShelfForChecking(doGetShelf.OfficeCode, doGetShelf.LocationCode, doGetShelf.AreaCode, doGetShelf.ShelfNo, doGetShelf.InstrumentCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<dtSearchInstrumentListResult> SearchInventoryInstrumentListAllShelf(doSearchInstrumentListCondition doCondition)
        {
            List<dtSearchInstrumentListResult> lst = base.SearchInventoryInstrumentListAllShelf(doCondition.OfficeCode, doCondition.LocationCode, doCondition.AreaCode, doCondition.StartShelfNo, doCondition.EndShelfNo, doCondition.Instrumentcode, doCondition.InstrumentName, MiscType.C_INV_AREA, ConfigName.C_CONFIG_WILDCARD, ShelfNo.C_INV_SHELF_NO_NOT_PRICE, InstrumentArea.C_INV_AREA_SE_LENDING_DEMO);

            CommonUtil.MappingObjectLanguage<dtSearchInstrumentListResult>(lst);
            return lst;
        }

        public int GetMovingNumber()
        {
            var lstMovingNo = base.GetMovingNo();
            if (lstMovingNo.Count == 0)
            {
                return 1;
            }
            else
            {
                return lstMovingNo[0].Value;
            }
        }

        public List<doResultGetSumPartialStockOutList> GetSumPartialStockOutList(string strContractCode)
        {
            return base.GetSumPartialStockOutList(InventorySlipStatus.C_INV_SLIP_STATUS_PARTIAL, strContractCode);
        }

        public Nullable<DateTime> GetLastBusinessDate(Nullable<DateTime> date)
        {
            var lstDate = base.GetLastBusinessDate_(date);
            if (lstDate.Count > 0)
            {
                return lstDate[0];
            }
            else
            {
                return null;
            }
        }

        #region Tbt_InventoryBooking

        //public List<tbt_InventoryBooking> GetTbt_InventoryBooking(string strContractCode)
        //{
        //    try
        //    {
        //        return base.GetTbt_InventoryBooking(strContractCode);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public void InsertTbt_InventoryBooking(List<tbt_InventoryBooking> lstBooking)
        {
            if (lstBooking == null || lstBooking.Count <= 0)
            {
                throw new ArgumentException("lstBooking cannot be null or empty.", "lstBooking");
            }

            string strXmlBooking = CommonUtil.ConvertToXml_Store<tbt_InventoryBooking>(lstBooking);
            var lstUpdated = base.InsertTbt_InventoryBooking(strXmlBooking);
            if (lstUpdated.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                logData.TableName = TableName.C_TBL_NAME_INV_BOOKING;
                logData.TableData = CommonUtil.ConvertToXml(lstUpdated);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }
        }

        public void UpdateTbt_InventoryBooking(List<tbt_InventoryBooking> lstBooking)
        {
            if (lstBooking == null || lstBooking.Count <= 0)
            {
                throw new ArgumentException("lstBooking cannot be null or empty.", "lstBooking");
            }

            string strXmlBooking = CommonUtil.ConvertToXml_Store<tbt_InventoryBooking>(lstBooking);
            var lstUpdated = base.UpdateTbt_InventoryBooking(strXmlBooking);
            if (lstUpdated.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_INV_BOOKING;
                logData.TableData = CommonUtil.ConvertToXml(lstUpdated);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }
        }

        public void DeleteTbt_InventoryBookingWithLog(string strContractCode)
        {
            var lstUpdated = base.DeleteTbt_InventoryBooking(strContractCode);
            if (lstUpdated.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                logData.TableName = TableName.C_TBL_NAME_INV_BOOKING;
                logData.TableData = CommonUtil.ConvertToXml(lstUpdated);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }
        }

        #endregion

        #region Tbt_InventoryBookingDetail

        //public List<tbt_InventoryBookingDetail> GetTbt_InventoryBookingDetail(string strContractCode, string strInstrumentCode)
        //{
        //    try
        //    {
        //        return base.GetTbt_InventoryBookingDetail(strContractCode, strInstrumentCode);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public void InsertTbt_InventoryBookingDetail(List<tbt_InventoryBookingDetail> lstBookingDtl)
        {
            if (lstBookingDtl == null || lstBookingDtl.Count <= 0)
            {
                throw new ArgumentException("lstBookingDtl cannot be null or empty.", "lstBookingDtl");
            }

            string strXmlBookingDtl = CommonUtil.ConvertToXml_Store<tbt_InventoryBookingDetail>(lstBookingDtl);
            var lstUpdated = base.InsertTbt_InventoryBookingDetail(strXmlBookingDtl);
            if (lstUpdated.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                logData.TableName = TableName.C_TBL_NAME_INV_BOOKING_DETAIL;
                logData.TableData = CommonUtil.ConvertToXml(lstUpdated);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }
        }

        public void UpdateTbt_InventoryBookingDetail(List<tbt_InventoryBookingDetail> lstBookingDtl)
        {
            if (lstBookingDtl == null || lstBookingDtl.Count <= 0)
            {
                throw new ArgumentException("lstBookingDtl cannot be null or empty.", "lstBookingDtl");
            }

            string strXmlUpdate = CommonUtil.ConvertToXml_Store<tbt_InventoryBookingDetail>(lstBookingDtl);
            var lstUpdated = base.UpdateTbt_InventoryBookingDetail(strXmlUpdate);
            if (lstUpdated.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_INV_BOOKING_DETAIL;
                logData.TableData = CommonUtil.ConvertToXml(lstUpdated);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }
        }

        public void DeleteTbt_InventoryBookingDetailWithLog(string strContractCode, string strInstrumentCode)
        {
            var lstUpdated = base.DeleteTbt_InventoryBookingDetail(strContractCode, strInstrumentCode);
            if (lstUpdated.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                logData.TableName = TableName.C_TBL_NAME_INV_BOOKING_DETAIL;
                logData.TableData = CommonUtil.ConvertToXml(lstUpdated);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }
        }

        #endregion

        public List<tbt_InventoryCheckingSlip> InsertTbt_InventoryCheckingSlip(List<tbt_InventoryCheckingSlip> lstInvChkSlip, bool blnLogging = true)
        {
            var result = base.InsertTbt_InventoryCheckingSlip(CommonUtil.ConvertToXml_Store<tbt_InventoryCheckingSlip>(lstInvChkSlip));

            //Insert Log
            if (result.Count > 0 && blnLogging)
            {

                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_INV_CHECKING_SLIP;
                logData.TableData = CommonUtil.ConvertToXml(result);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return result;
        }
        public List<tbt_InventoryCheckingSlipDetail> InsertTbt_InventoryCheckingSlipDetail(List<tbt_InventoryCheckingSlipDetail> lstInvChkSlipDtl, bool blnLogging = true)
        {
            var result = base.InsertTbt_InventoryCheckingSlipDetail(CommonUtil.ConvertToXml_Store<tbt_InventoryCheckingSlipDetail>(lstInvChkSlipDtl));

            //Insert Log
            if (result.Count > 0 && blnLogging)
            {

                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_INV_CHECKING_SLIP_DETAIL;
                logData.TableData = CommonUtil.ConvertToXml(result);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return result;
        }

        public List<tbt_InventorySlip> GetTbt_InventorySlip(string slipNo)
        {
            return base.GetTbt_InventorySlip(slipNo, null);
        }

        //public List<tbt_InventorySlip> GetTbt_InventorySlip(string slipNo, string installationSlipNo) {
        //    return base.GetTbt_InventorySlip(slipNo, installationSlipNo);
        //}

        public new bool IsEmptyShelf(string ShelfNo)
        {
            List<doIsEmptyShelfResult> result = base.IsEmptyShelf(ShelfNo);
            if (result.Count > 0)
            {
                return Convert.ToBoolean(result[0].Column1);
            }
            else
            {
                return true;
            }

        }

        public List<tbt_InventoryProjectWIP> InsertTbt_InventoryProjectWIP(List<tbt_InventoryProjectWIP> lstNewData)
        {
            if (lstNewData == null || lstNewData.Count <= 0)
            {
                throw new ArgumentException("lstNewData cannot be null or empty.", "lstNewData");
            }

            string strXmlNewData = CommonUtil.ConvertToXml_Store<tbt_InventoryProjectWIP>(lstNewData);
            var lstUpdated = base.InsertTbt_InventoryProjectWIP(strXmlNewData);
            if (lstUpdated.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                logData.TableName = TableName.C_TBL_NAME_INV_PROJECT_WIP;
                logData.TableData = CommonUtil.ConvertToXml(lstUpdated);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lstUpdated;
        }

        public List<tbt_InventoryProjectWIP> UpdateTbt_InventoryProjectWIP(List<tbt_InventoryProjectWIP> lstNewData)
        {
            if (lstNewData == null || lstNewData.Count <= 0)
            {
                throw new ArgumentException("lstNewData cannot be null or empty.", "lstNewData");
            }

            List<tbt_InventoryProjectWIP> lstUpdated = new List<tbt_InventoryProjectWIP>();
            foreach (var objNewData in lstNewData)
            {
                lstUpdated.AddRange(base.UpdateTbt_InventoryProjectWIP(objNewData.ProjectCode, objNewData.AreaCode, objNewData.InstrumentCode, objNewData.InstrumentQty));
            }

            if (lstUpdated.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_INV_PROJECT_WIP;
                logData.TableData = CommonUtil.ConvertToXml(lstUpdated);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lstUpdated;
        }

        public List<tbt_InventorySlipDetail> UpdateTbt_InventorySlipDetail(List<tbt_InventorySlipDetail> lstSlip)
        {
            try
            {
                List<tbt_InventorySlipDetail> lst = base.UpdateTbt_InventorySlipDetail(CommonUtil.ConvertToXml_Store<tbt_InventorySlipDetail>(lstSlip));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_SLIP_DETAIL,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<tbt_InventorySlipDetail> UpdateTbt_InventorySlipDetail_NoLog(List<tbt_InventorySlipDetail> lstSlip)
        {
            try
            {
                List<tbt_InventorySlipDetail> lst = base.UpdateTbt_InventorySlipDetail(CommonUtil.ConvertToXml_Store<tbt_InventorySlipDetail>(lstSlip));

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckInstrumentExpansion(string strInstrumentCode)
        {
            IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            var lstResult = handler.GetTbm_InstrumentExpansion(strInstrumentCode, null);
            if (lstResult != null && lstResult.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override List<tbt_AccountSampleInstock> GetTbt_AccountSampleInStock(string instrumentCode, string locationCode, string officeCode)
        {
            return base.GetTbt_AccountSampleInStock(instrumentCode, locationCode, officeCode);
        }

        public override List<tbt_InventoryCurrent> DeleteTbt_InventoryCurrent(string officeCode, string locationCode, string areaCode, string shelfNo, string instrumentCode)
        {
            try
            {
                List<tbt_InventoryCurrent> list = base.DeleteTbt_InventoryCurrent(officeCode, locationCode, areaCode, shelfNo, instrumentCode);

                #region Log

                if (list.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Delete,
                        TableName = TableName.C_TBL_NAME_INV_CURRENT,
                        TableData = CommonUtil.ConvertToXml(list)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tbt_InventoryCurrent> DeleteTbt_InventoryCurrent_NoLog(string officeCode, string locationCode, string areaCode, string shelfNo, string instrumentCode)
        {
            try
            {
                List<tbt_InventoryCurrent> list = base.DeleteTbt_InventoryCurrent(officeCode, locationCode, areaCode, shelfNo, instrumentCode);

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public override List<tbt_InventorySlipDetail> DeleteTbt_InventorySlipDetail(string slipNo)
        {
            try
            {
                List<tbt_InventorySlipDetail> list = base.DeleteTbt_InventorySlipDetail(slipNo);

                #region Log

                if (list.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Delete,
                        TableName = TableName.C_TBL_NAME_INV_SLIP_DETAIL,
                        TableData = CommonUtil.ConvertToXml(list)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override List<tbt_InventorySlip> DeleteTbt_InventorySlip(string slipNo)
        {
            try
            {
                List<tbt_InventorySlip> list = base.DeleteTbt_InventorySlip(slipNo);

                #region Log

                if (list.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Delete,
                        TableName = TableName.C_TBL_NAME_INV_SLIP,
                        TableData = CommonUtil.ConvertToXml(list)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region R2

        public override List<tbt_InventoryCheckingTemp> GetTbt_InventoryCheckingTemp(string strCheckingYearMonth, string strLocationCode, string strOfficeCode, string strShelfNo, string strAreaCode, string strInstrumentCode)
        {
            return base.GetTbt_InventoryCheckingTemp(strCheckingYearMonth, strLocationCode, strOfficeCode, strShelfNo, strAreaCode, strInstrumentCode);
        }

        public List<tbt_InventoryCheckingTemp> InsertTbt_InventoryCheckingTemp(List<tbt_InventoryCheckingTemp> lstCheckingTemp)
        {
            try
            {
                List<tbt_InventoryCheckingTemp> lst = base.InsertTbt_InventoryCheckingTemp(CommonUtil.ConvertToXml_Store<tbt_InventoryCheckingTemp>(lstCheckingTemp));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Insert,
                        TableName = TableName.C_TBL_NAME_INV_CHECKING_TMP,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<tbt_InventoryCheckingTemp> UpdateTbt_InventoryCheckingTemp(List<tbt_InventoryCheckingTemp> lstCheckingTemp)
        {
            try
            {
                List<tbt_InventoryCheckingTemp> lst = base.UpdateTbt_InventoryCheckingTemp(CommonUtil.ConvertToXml_Store<tbt_InventoryCheckingTemp>(lstCheckingTemp));

                #region Log

                if (lst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog()
                    {
                        TransactionType = doTransactionLog.eTransactionType.Update,
                        TableName = TableName.C_TBL_NAME_INV_CHECKING_TMP,
                        TableData = CommonUtil.ConvertToXml(lst)
                    };

                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                #endregion

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion R2

        public doLIFOInstrumentPrice GetLIFOInstrumentPrice(string strOfficeCode, string strLocationCode, string strInstrumentCode, int? intTransferQty, int? intPrevInstrumentQty, string c_CURRENCY_LOCAL, string c_CURRENCY_US)
        {
            try
            {
                List<doLIFOInstrumentPrice> res = base.GetLIFOInstrumentPrice(strOfficeCode, strLocationCode, strInstrumentCode, intTransferQty, intPrevInstrumentQty, c_CURRENCY_LOCAL, c_CURRENCY_US);
                return res[0];
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override List<tbt_InventoryCheckingSchedule> UpdateTbt_InventoryCheckingSchedule(string xml)
        {
            List<tbt_InventoryCheckingSchedule> lstUpdated = base.UpdateTbt_InventoryCheckingSchedule(xml);

            if (lstUpdated.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Update;
                logData.TableName = TableName.C_TBL_NAME_INV_CHECKING_SCHEDULE;
                logData.TableData = CommonUtil.ConvertToXml(lstUpdated);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lstUpdated;
        }

        public override List<tbt_InventoryCheckingSchedule> InsertTbt_InventoryCheckingSchedule(string xml)
        {
            List<tbt_InventoryCheckingSchedule> lstUpdated = base.InsertTbt_InventoryCheckingSchedule(xml);

            if (lstUpdated.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                logData.TableName = TableName.C_TBL_NAME_INV_CHECKING_SCHEDULE;
                logData.TableData = CommonUtil.ConvertToXml(lstUpdated);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return lstUpdated;
        }

        public List<dtInReportHeader> GetStockReport_InReport_Header(doIVS280SearchCondition param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }
            var lst = base.GetStockReport_InReport_Header(param.ReportType, param.SlipNoStart, param.SlipNoEnd, param.StockInDateStart, param.StockInDateEnd);
            CommonUtil.MappingObjectLanguage(lst);
            return lst;
        }

        public List<dtOutReportHeader> GetStockReport_OutReport_Header(doIVS281SearchCondition param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }
            var lst = base.GetStockReport_OutReport_Header(param.ReportType, param.SlipNoStart, param.SlipNoEnd, param.StockOutDateStart, param.StockOutDateEnd, param.ContractCode, param.OperateDateStart, param.OperateDateEnd, param.CustName);
            CommonUtil.MappingObjectLanguage(lst);
            return lst;
        }

        public List<dtReturnReportHeader> GetStockReport_ReturnReport_Header(doIVS282SearchCondition param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }
            var lst = base.GetStockReport_ReturnReport_Header(param.ReportType, param.SlipNoStart, param.SlipNoEnd, param.ReturnDateStart, param.ReturnDateEnd, param.ContractCode, param.OperateDateStart, param.OperateDateEnd, param.CustName);
            CommonUtil.MappingObjectLanguage(lst);
            return lst;
        }

        public List<dtInprocessToInstallReport> GetStockReport_InprocessToInstall(doIVS284SearchCondition param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }
            var lst = base.GetStockReport_InprocessToInstall(
                param.ReportType,
                (param.ContractCodeSelected != null ? string.Join(",", param.ContractCodeSelected) : param.ContractCode),
                param.YearMonth
            );
            CommonUtil.MappingObjectLanguage(lst);
            return lst;
        }

        public List<dtInprocessToInstallReportDetail> GetStockReport_InprocessToInstall_Detail(doIVS284SearchCondition param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }
            var lst = base.GetStockReport_InprocessToInstall_Detail(
                param.ReportType,
                (param.ContractCodeSelected != null ? string.Join(",", param.ContractCodeSelected) : param.ContractCode),
                param.YearMonth
            );
            CommonUtil.MappingObjectLanguage(lst);
            return lst;
        }

        public List<dtInProcessReport> GetStockReport_InProcess(doIVS286SearchCondition param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }
            var lst = base.GetStockReport_InProcess(
                param.ReportType,
                (param.ContractCodeSelected != null ? string.Join(",", param.ContractCodeSelected) : param.ContractCode),
                (string.IsNullOrEmpty(param.ProcessDate) ? null : param.ProcessDate.Replace("-", ""))
            );
            CommonUtil.MappingObjectLanguage(lst);
            return lst;
        }

        public List<dtInProcessReportDetail> GetStockReport_InProcess_Detail(doIVS286SearchCondition param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }
            var lst = base.GetStockReport_InProcess_Detail(
                param.ReportType,
                (param.ContractCodeSelected != null ? string.Join(",", param.ContractCodeSelected) : param.ContractCode),
                (string.IsNullOrEmpty(param.ProcessDate) ? null : param.ProcessDate.Replace("-", ""))
            );
            CommonUtil.MappingObjectLanguage(lst);
            return lst;
        }

        public List<dtChangeAreaReportHeader> GetStockReport_ChangeArea_Header(doIVS288SearchCondition param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }
            var lst = base.GetStockReport_ChangeArea_Header(param.SlipNoStart, param.SlipNoEnd, param.ContractCode, param.TransferDateStart, param.TransferDateEnd, param.SourceAreaCode, param.DestinationAreaCode);
            CommonUtil.MappingObjectLanguage(lst);
            return lst;
        }

        public List<dtEliminateReportHeader> GetStockReport_Eliminate_Header(doIVS289SearchCondition param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }
            var lst = base.GetStockReport_Eliminate_Header(param.SlipNoStart, param.SlipNoEnd, param.TransferDateStart, param.TransferDateEnd, param.TransferType);
            CommonUtil.MappingObjectLanguage(lst);
            return lst;
        }

        public List<dtBufferLossReportHeader> GetStockReport_BufferLoss_Header(doIVS290SearchCondition param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }
            var lst = base.GetStockReport_BufferLoss_Header(param.SlipNoStart, param.SlipNoEnd, param.TransferDateStart, param.TransferDateEnd);
            CommonUtil.MappingObjectLanguage(lst);
            return lst;
        }

        public new string GenerateVoucherID(DateTime? stockInDate)
        {
            var lstResult = base.GenerateVoucherID(stockInDate);
            if (lstResult.Count > 0)
            {
                return lstResult[0];
            }
            else
            {
                throw new ApplicationException("Unable to generate new Voucher ID");
            }
        }
        public List<doSearchReceiveSlipResult> SearchReceiveSlip(doIVS030SearchCondition param)
        {
            var lstResult = base.SearchReceiveSlip(
                param.ContractCode, 
                param.CompleteDateFrom, 
                param.CompleteDateTo, 
                param.InstallationSlipNo, 
                param.SubContractorName, 
                param.ProjectCode
            );
            return lstResult.ToList();
        }

        #region Monthly Price @ 2015
        public new decimal GetMonthlyAveragePrice(string instrumentCode, DateTime? yearmonth, string accountCode, string c_CURRENCY_LOCAL,string c_CURRENCY_US)
        {
            var lstResult = base.GetMonthlyAveragePrice(instrumentCode, yearmonth, accountCode, c_CURRENCY_LOCAL, c_CURRENCY_US);
            if (lstResult != null && lstResult.Count > 0)
                return lstResult[0].UnitPrice ?? 0;
            else
                return 0;
        }

        public List<string> GetInventoryStockLocations()
        {
            List<string> lstLocation = new List<string>();
            var common = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var cfg = common.GetSystemConfig(ConfigName.C_CONFIG_INV_STOCK_LOC);
            if (cfg != null && cfg.Count > 0)
            {
                lstLocation.AddRange(cfg[0].ConfigValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            return lstLocation;
        }

        public List<string> GetInventoryWIPLocations()
        {
            List<string> lstLocation = new List<string>();
            var common = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var cfg = common.GetSystemConfig(ConfigName.C_CONFIG_INV_WIP_LOC);
            if (cfg != null && cfg.Count > 0)
            {
                lstLocation.AddRange(cfg[0].ConfigValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            return lstLocation;
        }
        #endregion

        public DateTime? GetPreviousBusinessDateByDefaultOffset(DateTime? referencedate)
        {
            var common = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var cfg = common.GetSystemConfig(ConfigName.C_CONFIG_ACCOUNTING_BUSINESS_DAYS).FirstOrDefault();
            int offset = 3;
            if (cfg != null)
            {
                if (!int.TryParse(cfg.ConfigValue, out offset))
                {
                    offset = 3;
                }
            }
            var result = base.GetBusinessDateByOffset(referencedate, -offset);
            return result.FirstOrDefault();
        }

        public DateTime? GetNextBusinessDateByDefaultOffset(DateTime? referencedate)
        {
            var common = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            var cfg = common.GetSystemConfig(ConfigName.C_CONFIG_ACCOUNTING_BUSINESS_DAYS).FirstOrDefault();
            int offset = 3;
            if (cfg != null)
            {
                if (!int.TryParse(cfg.ConfigValue, out offset))
                {
                    offset = 3;
                }
            }
            var result = base.GetBusinessDateByOffset(referencedate, offset);
            return result.FirstOrDefault();
        }

        public DateTime? GetBusinessDateByOffset(DateTime? date, int offset)
        {
            var result = base.GetBusinessDateByOffset(date, offset);
            return result.FirstOrDefault();
        }
    }
}
