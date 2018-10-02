using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Sockets;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using System.Transactions;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class BillingInterfaceHandler : BizCTDataEntities, IBillingInterfaceHandler
    {
        /// <summary>
        /// Send billing basic data of changing contract fee to billing module 
        /// </summary>
        /// <param name="billingList"></param>
        /// <returns></returns>
        public bool SendBilling_ChangeFee(List<doBillingTempBasic> billingList)
        {
            bool blnProcessResult = false;
            IBillingHandler billHandler;

            try
            {
                if (billingList != null && billingList.Count > 0)
                {
                    //1.	Check mandatory data, Loop all in list of doBillingBasic[]
                    #region validate input
                    foreach (doBillingTempBasic billing in billingList)
                    {
                        //1.1 ContractCode, ContractBillingType, BillingAmount, ChangeFeeDate are required.
                        //ApplicationErrorException.CheckMandatoryField<doBillingTempBasic, doBillingBasicChangeFee>(billing);

                        //1.2	At least one of (BillingOCC, BillingTargetCode, BillingClientCode and BillingOfficeCode)
                        bool bBillingOCC = true;
                        bool bBillingTargetCode = true;
                        bool bBillingClientOfficeCode = true;
                        List<String> fieldList = new List<string>();

                        //BillingOCC
                        if (CommonUtil.IsNullOrEmpty(billing.BillingOCC))
                        {
                            bBillingOCC = false;
                            fieldList.Add("BillingOCC");
                        }

                        //BillingTargetCode
                        if (CommonUtil.IsNullOrEmpty(billing.BillingTargetCode))
                        {
                            bBillingTargetCode = false;
                            fieldList.Add("BillingTargetCode");
                        }

                        //BillingClientCode and BillingOfficeCode
                        try
                        {
                            doBillingBasicCheckBillingClientOfficeCode bClientOfficeCode = new doBillingBasicCheckBillingClientOfficeCode();
                            bClientOfficeCode.ContractCode = billing.ContractCode;
                            bClientOfficeCode.BillingOfficeCode = billing.BillingOfficeCode;
                            bClientOfficeCode.BillingClientCode = billing.BillingClientCode;
                            ApplicationErrorException.CheckMandatoryField(bClientOfficeCode);
                        }
                        catch (Exception)
                        {
                            bBillingClientOfficeCode = false;
                            fieldList.Add("BillingOfficeCode");
                            fieldList.Add("BillingClientCode");
                        }

                        bool bResult = bBillingOCC || bBillingTargetCode || bBillingClientOfficeCode;
                        if (!bResult)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, fieldList.ToArray<String>());
                        }
                    }
                    #endregion

                    //2.    Prepare data object
                    foreach (doBillingTempBasic data in billingList)
                    {
                        data.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_FEE;
                    }

                    //3.	Send billing basic data to billing module
                    billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                    billingList = billHandler.ManageBillingBasic(billingList);

                    blnProcessResult = true;
                }
                else
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "doBillingTempBasic");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        /// <summary>
        /// Send billing basic data of changing name to billing module
        /// </summary>
        /// <param name="billingList"></param>
        /// <returns></returns>
        public bool SendBilling_ChangeName(List<doBillingTempBasic> billingList)
        {
            bool blnProcessResult = false;
            IBillingHandler billHandler;

            try
            {
                if (billingList != null && billingList.Count > 0)
                {
                    //1.	Check mandatory data
                    #region validate input
                    foreach (doBillingTempBasic billing in billingList)
                    {
                        //1.1 ContractCode, BillingOCC
                        //ApplicationErrorException.CheckMandatoryField(billing);
                        ApplicationErrorException.CheckMandatoryField<doBillingTempBasic, doBillingBasicChangeName>(billing);

                        //1.2	At least one of (BillingTargetCode, BillingClientCode and BillingOfficeCode)
                        bool bBillingOCC = true;
                        bool bBillingTargetCode = true;
                        bool bBillingClientOfficeCode = true;
                        List<String> fieldList = new List<string>();

                        //Move to 1.1
                        ////BillingOCC
                        //if (CommonUtil.IsNullOrEmpty(billing.BillingOCC))
                        //{
                        //    bBillingOCC = false;
                        //    fieldList.Add("BillingOCC");
                        //}

                        //BillingTargetCode
                        if (CommonUtil.IsNullOrEmpty(billing.BillingTargetCode))
                        {
                            bBillingTargetCode = false;
                            fieldList.Add("BillingTargetCode");
                        }

                        //BillingClientCode and BillingOfficeCode
                        try
                        {
                            doBillingBasicCheckBillingClientOfficeCode bClientOfficeCode = new doBillingBasicCheckBillingClientOfficeCode();
                            bClientOfficeCode.ContractCode = billing.ContractCode;
                            bClientOfficeCode.BillingOfficeCode = billing.BillingOfficeCode;
                            bClientOfficeCode.BillingClientCode = billing.BillingClientCode;
                            ApplicationErrorException.CheckMandatoryField(bClientOfficeCode);
                        }
                        catch (Exception)
                        {
                            bBillingClientOfficeCode = false;
                            fieldList.Add("BillingOfficeCode");
                            fieldList.Add("BillingClientCode");
                        }

                        bool bResult = bBillingOCC || bBillingTargetCode || bBillingClientOfficeCode;
                        if (!bResult)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, fieldList.ToArray<String>());
                        }
                    }
                    #endregion

                    //2.	Send billing basic data to billing module
                    billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                    billingList = billHandler.ManageBillingBasicForChangeNameAndAddress(billingList);

                    blnProcessResult = true;
                }
                else
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "doBillingTempBasic");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        /// <summary>
        /// Send billing basic data and billing detail of approve contract to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        public bool SendBilling_RentalApprove(string ContractCode)
        {
            bool blnProcessResult = false;

            try
            {
                //1.	Check mandatory data ContractCode
                doSendBillingData sendBill = new doSendBillingData();
                sendBill.ContractCode = ContractCode;
                ApplicationErrorException.CheckMandatoryField(sendBill);

                IBillingTempHandler billTempHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                IRentralContractHandler rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //2.	Get rental contract basic data
                    List<tbt_RentalContractBasic> doTbt_RentalContractBasic = rentralHandler.GetTbt_RentalContractBasic(ContractCode, null);

                    string strProductTypeCode = string.Empty;
                    if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                        strProductTypeCode = doTbt_RentalContractBasic[0].ProductTypeCode;

                    //3.	Get billing basic data of contact fee from billing temp
                    List<doBillingTempBasic> doBillingBasicContractFee = new List<doBillingTempBasic>();
                    if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                    {
                        List<string> BillingTypeList = new List<string>();
                        BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE);
                        BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON);
                        BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE);

                        doBillingBasicContractFee = billTempHandler.GetBillingBasicData(ContractCode, doTbt_RentalContractBasic[0].LastOCC, BillingTypeList, null);

                        var billingBasic = (from t in doBillingBasicContractFee
                                            where t.BillingAmount != 0
                                            select t);

                        doBillingBasicContractFee = billingBasic.ToList<doBillingTempBasic>();
                    }

                    //4.	Get billing basic data of approve phase from billing temp 
                    List<string> BillingTimingList = new List<string>();
                    BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT);

                    List<doBillingTempBasic> doBillingBasicApprovePhase = billTempHandler.GetBillingBasicData(ContractCode, null, null, BillingTimingList);

                    //5.	Prepare data object
                    //5.1	Create data object doBillingBasic
                    List<doBillingTempBasic> doBillingTempBasicList = new List<doBillingTempBasic>();

                    //5.2	Set data  to data object (merge by exclude duplicate)
                    var lstBillingBasic = doBillingBasicContractFee.Union(doBillingBasicApprovePhase).ToList();
                    var uniqueBillingBasic = (from t in lstBillingBasic
                                              group t by new
                                              {
                                                  ContractCode = t.ContractCode,
                                                  BillingClientCode = t.BillingClientCode,
                                                  ContractBillingType = t.ContractBillingType,
                                                  ContractTiming = t.ContractTiming,
                                                  //BillingOfficeCode = t.BillingOfficeCode
                                              } into g
                                              select g.FirstOrDefault());

                    doBillingTempBasicList = uniqueBillingBasic.ToList<doBillingTempBasic>();
                    foreach (doBillingTempBasic data in doBillingTempBasicList)
                    {
                        data.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_APPROVE;
                        if (String.IsNullOrEmpty(strProductTypeCode) == false)
                            data.ProductTypeCode = strProductTypeCode;
                    }

                    //6.
                    if (doBillingTempBasicList != null && doBillingTempBasicList.Count > 0)
                    {
                        //6.1	Send billing basic data to billing module
                        billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                        doBillingTempBasicList = billHandler.ManageBillingBasic(doBillingTempBasicList);

                        foreach (doBillingTempBasic doOutBillingBasic in doBillingTempBasicList)
                        {
                            //6.2	Update billing OCC and billing target code to billing temp
                            billTempHandler.UpdateBillingTempByBillingClientAndOffice(
                                doOutBillingBasic.ContractCode, doOutBillingBasic.BillingClientCode
                                , doOutBillingBasic.BillingOfficeCode, doOutBillingBasic.BillingOCC
                                , doOutBillingBasic.BillingTargetCode);
                        }
                    }

                    //7.	Get billing detail of approve phase from billing temp
                    List<doBillingTempDetail> doBillingDetailList = billTempHandler.GetBillingDetailData(ContractCode, null, null, BillingTimingList);

                    //8.
                    if (doBillingDetailList != null && doBillingDetailList.Count > 0)
                    {
                        //8.1	 Prepare data object
                        //8.1.1.	Set data  to data object 				//For all item in list
                        foreach (doBillingTempDetail doBillingDetail in doBillingDetailList)
                        {
                            doBillingDetail.BillingDate = DateTime.Now;
                            if (String.IsNullOrEmpty(strProductTypeCode) == false)
                                doBillingDetail.ProductTypeCode = strProductTypeCode;
                        }

                        //8.2	 Send billing detail data to billing module
                        billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                        doBillingDetailList = billHandler.ManageBillingDetailByContract(doBillingDetailList);

                        //8.3	 Update send flag in Billing temp
                        foreach (doBillingTempDetail doBillingDetail in doBillingDetailList)
                        {
                            billTempHandler.UpdateSendFlag(doBillingDetail.ContractCode, doBillingDetail.SequenceNo, doBillingDetail.OCC);
                        }
                    }

                    scope.Complete(); //commit trans
                    blnProcessResult = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        /// <summary>
        /// Send billing basic data and billing detail of cancel contract to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="CancelDate"></param>
        /// <param name="doBillingTempBasicCancel"></param>
        /// <param name="doBillingTempDetailCancel"></param>
        /// <param name="blnCompleteInstallFlag"></param>
        /// <returns></returns>
        public bool SendBilling_RentalCancel(string ContractCode, DateTime? CancelDate, doBillingTempBasic doBillingTempBasicCancel, doBillingTempDetail doBillingTempDetailCancel, bool? blnCompleteInstallFlag)
        {
            bool blnProcessResult = false;
            List<string> BillingTypeList;
            List<string> BillingTimingList;

            try
            {
                //1.	Check mandatory data ContractCode and CancelDate
                doSendBillingDataCheckContractCancelDate sendBill = new doSendBillingDataCheckContractCancelDate();
                sendBill.ContractCode = ContractCode;
                sendBill.CancelDate = CancelDate;
                sendBill.CompleteInstallFlag = blnCompleteInstallFlag;
                ApplicationErrorException.CheckMandatoryField(sendBill);

                IBillingTempHandler billTempHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                IRentralContractHandler rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //3.	Copy billing temp basic data  to new data object 
                    List<doBillingTempBasic> doBillingTempBasicList = new List<doBillingTempBasic>();

                    if (doBillingTempBasicCancel != null)
                        doBillingTempBasicList.Add(doBillingTempBasicCancel);

                    //4.	Get rental contract basic data 
                    List<tbt_RentalContractBasic> doTbt_RentalContractBasic = rentralHandler.GetTbt_RentalContractBasic(ContractCode, null);

                    string strProductTypeCode = string.Empty;
                    if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                        strProductTypeCode = doTbt_RentalContractBasic[0].ProductTypeCode;

                    //5.	In case new installation is complete and before start servive, get billing basic data of start service phase from billing temp 
                    if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                    {
                        if (doTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_ON
                            && doTbt_RentalContractBasic[0].LastChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START)
                        {
                            BillingTimingList = new List<string>();
                            BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_START_SERVICE);

                            List<doBillingTempBasic> doBillingTempBasicListStartPhase = billTempHandler.GetBillingBasicData(ContractCode, null, null, BillingTimingList);

                            //Merge data  to data object (merge by exclude duplicate)  
                            doBillingTempBasicList = doBillingTempBasicList.Union(doBillingTempBasicListStartPhase).ToList<doBillingTempBasic>();

                            var uniqueBillingBasic = (from t in doBillingTempBasicList
                                                      group t by new
                                                      {
                                                          ContractCode = t.ContractCode,
                                                          BillingClientCode = t.BillingClientCode,
                                                          ContractBillingType = t.ContractBillingType,
                                                          ContractTiming = t.ContractTiming,
                                                          BillingOfficeCode = t.BillingOfficeCode
                                                      } into g
                                                      select g.FirstOrDefault());
                            doBillingTempBasicList = uniqueBillingBasic.ToList<doBillingTempBasic>();
                        }
                    }

                    //6.
                    if (doBillingTempBasicList != null && doBillingTempBasicList.Count > 0)
                    {
                        //6.1	Set other value in doBillingTempBasicList[]			//For all item in list
                        foreach (doBillingTempBasic doOutBillingBasic in doBillingTempBasicList)
                        {

                            if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                            {
                                //6.1.1.
                                if (doTbt_RentalContractBasic[0].LastChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START)
                                {
                                    if (doTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_ON)
                                    {
                                        doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_NEW_INSTALL;
                                    }
                                    else
                                    {
                                        doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START;
                                    }
                                }
                                else
                                {
                                    //6.1.2.
                                    doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_START;
                                }

                                //6.1.3.
                                doOutBillingBasic.ProductTypeCode = doTbt_RentalContractBasic[0].ProductTypeCode;
                            }
                        }

                        //6.2	Send billing basic data to billing module
                        doBillingTempBasicList = billHandler.ManageBillingBasic(doBillingTempBasicList);

                        foreach (doBillingTempBasic doOutBillingBasic in doBillingTempBasicList)
                        {
                            //Update billing OCC and billing target code to billing temp
                            billTempHandler.UpdateBillingTempByBillingClientAndOffice(
                                doOutBillingBasic.ContractCode, doOutBillingBasic.BillingClientCode
                                , doOutBillingBasic.BillingOfficeCode, doOutBillingBasic.BillingOCC
                                , doOutBillingBasic.BillingTargetCode);
                        }

                        //6.3	Map Billing OCC to billing detail with returned data from Billing module
                        foreach (doBillingTempBasic doOutBillingBasic in doBillingTempBasicList)
                        {
                            if (doOutBillingBasic.ContractBillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE)
                            {
                                if (doBillingTempDetailCancel != null)
                                {
                                    doBillingTempDetailCancel.BillingOCC = doOutBillingBasic.BillingOCC;
                                    break;
                                }
                            }
                        }
                    }

                    //7.	Copy billing detail data  to new data object 
                    List<doBillingTempDetail> doBillingTempDetailList = new List<doBillingTempDetail>();

                    if (doBillingTempDetailCancel != null)
                        doBillingTempDetailList.Add(doBillingTempDetailCancel);

                    //8.	In case new installation is complete and before start servive, get installation fee for billing detail of start service from billing temp
                    if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                    {
                        if (doTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_ON
                            && doTbt_RentalContractBasic[0].LastChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CANCEL_BEFORE_START)
                        {
                            //8.1.1.	Get installation fee for billing detail of start service from billing temp
                            BillingTypeList = new List<string>();
                            BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE);

                            BillingTimingList = new List<string>();
                            BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_START_SERVICE);

                            List<doBillingTempDetail> doBillingTempDetailStartPhase = billTempHandler.GetBillingDetailData(ContractCode, null, BillingTypeList, BillingTimingList);

                            //9.	Set data  to data object (merge by exclude duplicate)
                            doBillingTempDetailList = doBillingTempDetailList.Union(doBillingTempDetailStartPhase).ToList<doBillingTempDetail>();

                            var uniqueBillingDetail = (from t in doBillingTempDetailList
                                                       group t by new
                                                       {
                                                           ContractCode = t.ContractCode,
                                                           ContractBillingType = t.ContractBillingType,
                                                           BillingTiming = t.BillingTiming,
                                                       } into g
                                                       select g.FirstOrDefault());
                            doBillingTempDetailList = uniqueBillingDetail.ToList<doBillingTempDetail>();
                        }
                    }

                    //10.	In case remove installation is complete, get billing detail of complete installation phase
                    if (blnCompleteInstallFlag.Value)
                    {
                        //10.1	 Get billing detail of complete installation from billing temp
                        BillingTimingList = new List<string>();
                        BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION);

                        List<doBillingTempDetail> doBillingTempDetailListCompleteInstall = billTempHandler.GetBillingDetailData(ContractCode, null, null, BillingTimingList);

                        //10.2	 Set data  to data object (merge by exclude duplicate)
                        doBillingTempDetailList = doBillingTempDetailList.Union(doBillingTempDetailListCompleteInstall).ToList<doBillingTempDetail>();

                        var uniqueBillingDetail = (from t in doBillingTempDetailList
                                                   group t by new
                                                   {
                                                       ContractCode = t.ContractCode,
                                                       ContractBillingType = t.ContractBillingType,
                                                       BillingTiming = t.BillingTiming,
                                                   } into g
                                                   select g.FirstOrDefault());
                        doBillingTempDetailList = uniqueBillingDetail.ToList<doBillingTempDetail>();
                    }

                    //11.
                    if (doBillingTempDetailList != null && doBillingTempDetailList.Count > 0)
                    {
                        //11.1	 Prepare data object
                        //11.1.1.	 Set data  to data object 
                        foreach (doBillingTempDetail doOutBillingBasic in doBillingTempDetailList)
                        {
                            doOutBillingBasic.BillingDate = CancelDate.Value;
                            if (String.IsNullOrEmpty(strProductTypeCode) == false)
                                doOutBillingBasic.ProductTypeCode = strProductTypeCode;
                        }

                        //11.2	 Send billing detail data to billing module
                        doBillingTempDetailList = billHandler.ManageBillingDetailByContract(doBillingTempDetailList);

                        //11.4	 Update send flag in Billing temp
                        foreach (doBillingTempDetail doBillingDetail in doBillingTempDetailList)
                        {
                            billTempHandler.UpdateSendFlag(doBillingDetail.ContractCode, doBillingDetail.SequenceNo, doBillingDetail.OCC);
                        }
                    }

                    //11.3	 Call sending cancel contract data to billing module
                    bool blnProcessCancelResult = billHandler.ManageBillingBasicForCancel(ContractCode, CancelDate.Value);

                    //12.	Delete all send billing data in billing temp
                    List<tbt_BillingTemp> doTbt_BillingTemp = billTempHandler.DeleteAllSendData(ContractCode);

                    scope.Complete();
                    blnProcessResult = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        /// <summary>
        /// Send billing basic data and billing detail of change plan contract to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="OCC"></param>
        /// <returns></returns>
        public bool SendBilling_RentalChangePlan(string ContractCode, string OCC)
        {
            bool blnProcessResult = false;
            List<string> BillingTypeList;
            List<string> BillingTimingList;

            try
            {
                //1.	Check mandatory data ContractCode and OCC
                doSendBillingDataCheckContractOCC sendBill = new doSendBillingDataCheckContractOCC();
                sendBill.ContractCode = ContractCode;
                sendBill.OCC = OCC;
                ApplicationErrorException.CheckMandatoryField(sendBill);

                IBillingTempHandler billTempHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                IRentralContractHandler rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //2.	Get rental contract basic data 
                    List<tbt_RentalContractBasic> doTbt_RentalContractBasic = rentralHandler.GetTbt_RentalContractBasic(ContractCode, null);

                    string strProductTypeCode = string.Empty;
                    if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                        strProductTypeCode = doTbt_RentalContractBasic[0].ProductTypeCode;

                    //3.	Create new data object 
                    List<doBillingTempBasic> doBillingTempBasicList = new List<doBillingTempBasic>();

                    //4.	In case of change before new installation, get billing basic data of contact fee from billing temp
                    if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                    {
                        if (
                            (doTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE
                                || doTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA
                                || doTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE
                                || doTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                            
                            || (doTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
                                && (doTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL
                                    || doTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                                )
                            )
                        {
                            //4.1.1.
                            BillingTypeList = new List<string>();
                            BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE);
                            BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON);
                            BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE);

                            List<doBillingTempBasic> doBillingTempBasicContractFeeList = billTempHandler.GetBillingBasicData(ContractCode, OCC, BillingTypeList, null);

                            var billingBasic = (from t in doBillingTempBasicContractFeeList
                                                where (t.BillingAmount > 0 && t.BillingOCC == null) ||
                                                        (t.BillingOCC != null)
                                                select t);

                            doBillingTempBasicContractFeeList = billingBasic.ToList<doBillingTempBasic>();

                            //4.1.2.	Merge data  to data object (merge by exclude duplicate)
                            doBillingTempBasicList = doBillingTempBasicList.Union(doBillingTempBasicContractFeeList).ToList<doBillingTempBasic>();

                            var uniqueBillingBasic = (from t in doBillingTempBasicList
                                                      group t by new
                                                      {
                                                          ContractCode = t.ContractCode,
                                                          BillingClientCode = t.BillingClientCode,
                                                          ContractBillingType = t.ContractBillingType,
                                                          ContractTiming = t.ContractTiming,
                                                          BillingOfficeCode = t.BillingOfficeCode
                                                      } into g
                                                      select g.FirstOrDefault());
                            doBillingTempBasicList = uniqueBillingBasic.ToList<doBillingTempBasic>();
                        }

                        //5.	Get billing basic data of register CP-12 phase from billing temp 
                        BillingTimingList = new List<string>();
                        BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_AFTER_REGISTER);

                        List<doBillingTempBasic> doBillingTempBasicListRegisterPhase = billTempHandler.GetBillingBasicData(ContractCode, doTbt_RentalContractBasic[0].LastOCC, null, BillingTimingList);                            

                        //6.	Merge data object (merge by exclude duplicate)
                        doBillingTempBasicList = doBillingTempBasicList.Union(doBillingTempBasicListRegisterPhase).ToList<doBillingTempBasic>();

                        var uniqueBillingBasic2 = (from t in doBillingTempBasicList
                                                  group t by new
                                                  {
                                                      ContractCode = t.ContractCode,
                                                      BillingClientCode = t.BillingClientCode,
                                                      ContractBillingType = t.ContractBillingType,
                                                      ContractTiming = t.ContractTiming,
                                                      BillingOfficeCode = t.BillingOfficeCode
                                                  } into g
                                                  select g.FirstOrDefault());
                        doBillingTempBasicList = uniqueBillingBasic2.ToList<doBillingTempBasic>();
                    }

                    //7.	Prepare data object
                    //7.1	Set other value in doBillingTempBasicList[]			//**For all item in list
                    foreach (doBillingTempBasic doOutBillingBasic in doBillingTempBasicList)
                    {
                        if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                        {
                            // Product type is alarm or rental-sale
                            if (doTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL
                                    || doTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                            {
                                if (doTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_OFF)
                                {
                                    doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_NEW_INSTALL;
                                }
                                else if (doTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                                {
                                    doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START;
                                }
                                else if (doTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                                {
                                    doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_DURING_STOP;
                                }
                                else
                                {
                                    doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_PLAN;
                                }
                            }
                            //Product type is SG, MA and sale-online
                            else
                            {
                                if (doTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                                {
                                    doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START;
                                }
                                else if (doTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                                {
                                    doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_DURING_STOP;
                                }
                                else
                                {
                                    doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_PLAN;
                                }
                            }
                        }

                        doOutBillingBasic.ProductTypeCode = doTbt_RentalContractBasic[0].ProductTypeCode;

                        if (doTbt_RentalContractBasic[0].LastChangeImplementDate != null)
                            doOutBillingBasic.ChangeFeeDate = doTbt_RentalContractBasic[0].LastChangeImplementDate.Value;
                    }

                    if (doBillingTempBasicList != null && doBillingTempBasicList.Count > 0)
                    {
                        //8.	Send billing basic data to billing module
                        doBillingTempBasicList = billHandler.ManageBillingBasic(doBillingTempBasicList);

                        //9.	Loop btbl as doBillingTempBasic in list of doBillingTempBasicList[]
                        foreach (doBillingTempBasic doOutBillingBasic in doBillingTempBasicList)
                        {
                            //9.1	Update billing OCC and billing target code to billing temp	
                            billTempHandler.UpdateBillingTempByBillingClientAndOffice(
                                doOutBillingBasic.ContractCode, doOutBillingBasic.BillingClientCode
                                , doOutBillingBasic.BillingOfficeCode, doOutBillingBasic.BillingOCC
                                , doOutBillingBasic.BillingTargetCode);
                        }
                    }

                    //10.	Get billing detail of after register phase from billing temp
                    BillingTimingList = new List<string>();
                    BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_AFTER_REGISTER);

                    List<doBillingTempDetail> doBillingTempDetailList = billTempHandler.GetBillingDetailData(ContractCode, null, null, BillingTimingList);

                    //11.
                    if (doBillingTempDetailList != null && doBillingTempDetailList.Count > 0)
                    {
                        //11.1	Prepare data object
                        //11.1.1.	Set data  to data object 
                        foreach (doBillingTempDetail doOutBillingDetail in doBillingTempDetailList)
                        {
                            doOutBillingDetail.BillingDate = DateTime.Now;
                            if (String.IsNullOrEmpty(strProductTypeCode) == false)
                                doOutBillingDetail.ProductTypeCode = strProductTypeCode;
                        }

                        //11.2	Send billing detail data to billing module
                        doBillingTempDetailList = billHandler.ManageBillingDetailByContract(doBillingTempDetailList);

                        //11.3	Update send flag in Billing temp
                        foreach (doBillingTempDetail doOutBillingDetail in doBillingTempDetailList)
                        {
                            billTempHandler.UpdateSendFlag(doOutBillingDetail.ContractCode, doOutBillingDetail.SequenceNo, doOutBillingDetail.OCC);
                        }
                    }

                    //12.	If there is no error from all previous steps
                    scope.Complete(); //commit trans
                    blnProcessResult = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        /// <summary>
        /// Send billing basic data and billing detail of complete installation to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        public bool SendBilling_RentalCompleteInstall(string ContractCode, DateTime CompleteInstallationDate)
        {
            bool blnProcessResult = false;
            List<string> BillingTypeList;
            List<string> BillingTimingList;

            try
            {
                //1.	Check mandatory data ContractCode
                doSendBillingData sendBill = new doSendBillingData();
                sendBill.ContractCode = ContractCode;
                ApplicationErrorException.CheckMandatoryField(sendBill);

                IBillingTempHandler billTempHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                IRentralContractHandler rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //2.	Get rental contract basic data
                    List<tbt_RentalContractBasic> doTbt_RentalContractBasic = rentralHandler.GetTbt_RentalContractBasic(ContractCode, null);

                    string strProductTypeCode = string.Empty;
                    if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                        strProductTypeCode = doTbt_RentalContractBasic[0].ProductTypeCode;

                    //3.	Get billing basic data of complete installation phase from billing temp 
                    BillingTypeList = new List<string>();
                    BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE);
                    BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE);
                    BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE);
                    BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE);

                    BillingTimingList = new List<string>();
                    BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION);

                    List<doBillingTempBasic> doBillingTempBasicList = billTempHandler.GetBillingBasicData(ContractCode, null, BillingTypeList, BillingTimingList);                            

                    //4.	For after start service, get billing basic data of contact fee
                    if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                    {
                        if ((doTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                              && doTbt_RentalContractBasic[0].StartType != StartType.C_START_TYPE_ALTER_START)
                            || doTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                        {
                            //4.1	 
                            BillingTypeList = new List<string>();
                            BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE);
                            BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON);
                            BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE);

                            List<doBillingTempBasic> doBillingTempBasicListContractFee = billTempHandler.GetBillingBasicData(ContractCode, null, BillingTypeList, null);                            

                            //4.2	 Set data  to data object (merge by exclude duplicate)
                            doBillingTempBasicList = doBillingTempBasicList.Union(doBillingTempBasicListContractFee).ToList<doBillingTempBasic>();

                            var uniqueBillingBasic = (from t in doBillingTempBasicList
                                                      group t by new
                                                      {
                                                          ContractCode = t.ContractCode,
                                                          BillingClientCode = t.BillingClientCode,
                                                          ContractBillingType = t.ContractBillingType,
                                                          ContractTiming = t.ContractTiming,
                                                          BillingOfficeCode = t.BillingOfficeCode
                                                      } into g
                                                      select g.LastOrDefault());
                            doBillingTempBasicList = uniqueBillingBasic.ToList<doBillingTempBasic>();
                        }
                    }

                    //5.	Prepare data object
                    //5.1	Set other value in doBillingTempBasicList[]			//**For all item in list
                    foreach (doBillingTempBasic doOutBillingBasic in doBillingTempBasicList)
                    {
                        if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                        {
                            if (doTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                            {
                                //before and after new same process in billing module
                                doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_BEFORE_START;
                            }
                            else if (doTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                            {
                                doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_DURING_STOP;
                            }
                            else //after start service
                            {
                                doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_PLAN;
                            }
                        }

                        doOutBillingBasic.ChangeFeeDate = CompleteInstallationDate;
                        doOutBillingBasic.ProductTypeCode = doTbt_RentalContractBasic[0].ProductTypeCode;
                    }

                    if (doBillingTempBasicList != null && doBillingTempBasicList.Count > 0)
                    {
                        //6.	Send billing basic data to billing module
                        doBillingTempBasicList = billHandler.ManageBillingBasic(doBillingTempBasicList);

                        //7.	Loop btbl as doBillingTempBasic in list of doBillingTempBasicList[]
                        foreach (doBillingTempBasic doOutBillingBasic in doBillingTempBasicList)
                        {
                            //7.1	Update billing OCC and billing target code to billing temp	
                            billTempHandler.UpdateBillingTempByBillingClientAndOffice(
                                doOutBillingBasic.ContractCode, doOutBillingBasic.BillingClientCode
                                , doOutBillingBasic.BillingOfficeCode, doOutBillingBasic.BillingOCC
                                , doOutBillingBasic.BillingTargetCode);
                        }
                    }

                    //8.	Get billing detail of complete installation phase from billing temp
                    BillingTimingList = new List<string>();
                    BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION);

                    List<doBillingTempDetail> doBillingTempDetailList = billTempHandler.GetBillingDetailData(ContractCode, null, null, BillingTimingList);

                    if (doBillingTempDetailList != null && doBillingTempDetailList.Count > 0)
                    {
                        //9.	Prepare data object
                        //9.1	 Set data  to data object 
                        foreach (doBillingTempDetail doOutBillingDetail in doBillingTempDetailList)
                        {
                            doOutBillingDetail.BillingDate = CompleteInstallationDate;

                            if (String.IsNullOrEmpty(strProductTypeCode) == false)
                                doOutBillingDetail.ProductTypeCode = strProductTypeCode;
                        }

                        //10.	Send billing detail data to billing module
                        doBillingTempDetailList = billHandler.ManageBillingDetailByContract(doBillingTempDetailList);

                        //11.	Update send flag in Billing temp
                        foreach (doBillingTempDetail doOutBillingDetail in doBillingTempDetailList)
                        {
                            billTempHandler.UpdateSendFlag(doOutBillingDetail.ContractCode, doOutBillingDetail.SequenceNo, doOutBillingDetail.OCC);
                        }
                    }

                    //12.	If there is no error from all previous steps
                    scope.Complete(); //commit trans
                    blnProcessResult = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        /// <summary>
        /// Send resume service command to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="ResumeDate"></param>
        /// <returns></returns>
        public bool SendBilling_ResumeService(string ContractCode, DateTime? ResumeDate)
        {
            bool blnProcessResult = false;

            try
            {
                //1.	Check mandatory data ContractCode and ResumeDate
                doSendBillingDataCheckContractResumeDate sendBill = new doSendBillingDataCheckContractResumeDate();
                sendBill.ContractCode = ContractCode;
                sendBill.ResumeDate = ResumeDate;
                ApplicationErrorException.CheckMandatoryField(sendBill);

                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                //2.	Call sending resume service data to billing module
                bool blnProcessResumeResult = billHandler.ManageBillingBasicForResume(ContractCode, ResumeDate.Value);

                blnProcessResult = true;
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        /// <summary>
        /// Send billing basic data and billing detail of approve contract to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="SaleOCC"></param>
        /// <returns></returns>
        public bool SendBilling_SaleApprove(string ContractCode, string SaleOCC)
        {
            bool blnProcessResult = false;
            List<string> BillingTypeList;
            List<string> BillingTimingList;

            try
            {
                //1.	Check mandatory data ContractCode and SaleOCC
                doSendBillingDataCheckContractOCC sendBill = new doSendBillingDataCheckContractOCC();
                sendBill.ContractCode = ContractCode;
                sendBill.OCC = SaleOCC;
                ApplicationErrorException.CheckMandatoryField(sendBill);

                IBillingTempHandler billTempHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //2.	Get billing basic data of sale price from billing temp
                    BillingTypeList = new List<string>();
                    //BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE);
                    BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE);
                    BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE);

                    List<doBillingTempBasic> doBillingTempBasicList = billTempHandler.GetBillingBasicData(ContractCode, SaleOCC, BillingTypeList, null);                            

                    //3.	Prepare data object
                    //3.1	Set other value in doBillingTempBasic
                    foreach (doBillingTempBasic doOutBillingBasic in doBillingTempBasicList)
                    {
                        doOutBillingBasic.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_APPROVE;
                        doOutBillingBasic.ProductTypeCode = ProductType.C_PROD_TYPE_SALE;
                    }

                    if (doBillingTempBasicList != null && doBillingTempBasicList.Count > 0)
                    {
                        //4.	Send billing basic data to billing module
                        doBillingTempBasicList = billHandler.ManageBillingBasic(doBillingTempBasicList);

                        foreach (doBillingTempBasic doOutBillingBasic in doBillingTempBasicList)
                        {
                            //5.	Update billing OCC and billing target code to billing temp	
                            billTempHandler.UpdateBillingTempByBillingClientAndOffice(
                                doOutBillingBasic.ContractCode, doOutBillingBasic.BillingClientCode
                                , doOutBillingBasic.BillingOfficeCode, doOutBillingBasic.BillingOCC
                                , doOutBillingBasic.BillingTargetCode);
                        }
                    }

                    //6.	Get billing detail of approve phase from billing temp
                    BillingTimingList = new List<string>();
                    BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT);
                    BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_PARTIAL);

                    List<doBillingTempDetail> doBillingTempDetailList = billTempHandler.GetBillingDetailData(ContractCode, SaleOCC, null, BillingTimingList);

                    //7.
                    if (doBillingTempDetailList != null && doBillingTempDetailList.Count > 0)
                    {
                        List<tbt_SaleBasic> doTbt_SaleBasic = saleHandler.GetTbt_SaleBasic(ContractCode, SaleOCC, null);

                        //7.1	Prepare data object
                        //7.1.1.	Set data  to data object 
                        foreach (doBillingTempDetail doOutBillingDetail in doBillingTempDetailList)
                        {
                            doOutBillingDetail.BillingDate = DateTime.Now;
                            doOutBillingDetail.ProductTypeCode = ProductType.C_PROD_TYPE_SALE;
                            if (doTbt_SaleBasic != null && doTbt_SaleBasic.Count > 0)
                                doOutBillingDetail.ProductCode = doTbt_SaleBasic[0].ProductCode;
                        }

                        //7.2	Send billing detail data to billing module
                        doBillingTempDetailList = billHandler.ManageBillingDetailByContract(doBillingTempDetailList);
                        
                        foreach (doBillingTempDetail doOutBillingDetail in doBillingTempDetailList)
                        {
                            //7.3	Update send flag in Billing temp
                            billTempHandler.UpdateSendFlag(doOutBillingDetail.ContractCode, doOutBillingDetail.SequenceNo, doOutBillingDetail.OCC);
                        }
                    }

                    //8.	If there is no error from all previous steps
                    scope.Complete(); //commit trans
                    blnProcessResult = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        /// <summary>
        /// Send billing detail data of complete installation to billing module
        /// </summary>
        /// <param name="doBillingTempDetailData"></param>
        /// <returns></returns>
        public bool SendBilling_SaleCompleteInstall(doBillingTempDetail doBillingTempDetailData)
        {
            bool blnProcessResult = false;

            try
            {
                //1.	Check mandatory data ContractCode, BillingOCC, BillingType, BillingAmt
                ApplicationErrorException.CheckMandatoryField<doBillingTempDetail, doBillingDetailSaleCompleteInstall>(doBillingTempDetailData);

                ISaleContractHandler saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                //2.	Send billing detail data to billing module
                List<tbt_SaleBasic> doTbt_SaleBasic = saleHandler.GetTbt_SaleBasic(doBillingTempDetailData.ContractCode, null, FlagType.C_FLAG_ON);

                //3.	Prepare data object
                //3.1	Set data  to data object 
                if (doBillingTempDetailData != null)
                {
                    doBillingTempDetailData.BillingDate = DateTime.Now;
                    doBillingTempDetailData.ProductTypeCode = ProductType.C_PROD_TYPE_SALE;
                    if (doTbt_SaleBasic != null && doTbt_SaleBasic.Count > 0)
                        doBillingTempDetailData.ProductCode = doTbt_SaleBasic[0].ProductCode;

                    //4.	Send billing detail data to billing module
                    List<doBillingTempDetail> doBillingTempDetailList = new List<doBillingTempDetail>();
                    doBillingTempDetailList.Add(doBillingTempDetailData);

                    doBillingTempDetailList = billHandler.ManageBillingDetailByContract(doBillingTempDetailList);

                    blnProcessResult = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        /// <summary>
        /// Send billing detail data of one time to billing module
        /// </summary>
        /// <param name="doBillingTempDetailData"></param>
        /// <returns></returns>
        public bool SendBilling_OnetimeFee(doBillingTempDetail doBillingTempDetailData)
        {
            bool blnProcessResult = false;

            try
            {
                //1.	Check mandatory data ContractCode, BillingOCC, BillingType, BillingAmt
                ApplicationErrorException.CheckMandatoryField<doBillingTempDetail, doBillingDetailSaleCompleteInstall>(doBillingTempDetailData);
                               
                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                if (doBillingTempDetailData != null)
                {
                    List<doBillingTempDetail> doBillingTempDetailList = new List<doBillingTempDetail>();
                    doBillingTempDetailList.Add(doBillingTempDetailData);
                    doBillingTempDetailList = billHandler.ManageBillingDetailByContract(doBillingTempDetailList);

                    blnProcessResult = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        #region No use
        //public void SendBilling_SaleCustAccept(string ContractCode, string SaleOCC)
        //{
        //    try
        //    {
        //        //1.	Check mandatory data ContractCode and SaleOCC
        //        doSendBillingDataCheckContractOCC sendBill = new doSendBillingDataCheckContractOCC();
        //        sendBill.ContractCode = ContractCode;
        //        sendBill.OCC = SaleOCC;
        //        ApplicationErrorException.CheckMandatoryField(sendBill);

        //        using (TransactionScope scope = new TransactionScope())
        //        {
        //            IBillingTempHandler hand = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;

        //            //3.	Get billing detail of customer agreement from billing temp
        //            List<string> BillingTimingList = new List<string>();
        //            BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_ACCEPTANCE);

        //            List<doBillingTempDetail> doBillingDetailList = hand.GetBillingDetailData(ContractCode, SaleOCC, null, BillingTimingList);

        //            if (doBillingDetailList != null && doBillingDetailList.Count > 0)
        //            {
        //                //4.	Prepare data object
        //                //4.1	Set data  to data object 
        //                //    Set doBillingDetail[].BillingDate = NOW() 
        //                //Already set BillingDate = CURRENT_TIMESTAMP from database

        //                //Mim's Todo Call Method
        //                //5.	Send billing detail data to billing module
        //                //Call		BillingHandler.MaintainBillingDetail()		//implement in billing phase
        //                //Parameter	doBillingDetail[]
        //                //Return	blnProcessCreateDetailResult

        //                foreach (doBillingTempDetail doOutBillingDetail in doBillingDetailList)
        //                {
        //                    //6.	Update send flag in Billing temp
        //                    hand.UpdateSendFlag(doOutBillingDetail.ContractCode, doOutBillingDetail.SequenceNo);
        //                }
        //            }

        //            //7.	If there is no error from all previous steps
        //            scope.Complete(); //commit trans
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        #endregion

        /// <summary>
        ///Send start service command to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="StartDate"></param>
        /// <param name="AdjustBillingTermEndDate"></param>
        /// <returns></returns>
        public bool SendBilling_StartService(string ContractCode, DateTime? StartDate, DateTime? AdjustBillingTermEndDate, string strStartType)
        {
            bool blnProcessResult = false;
            List<string> BillingTypeList;
            List<string> BillingTimingList;

            try
            {
                //1.	Check mandatory data ContractCode and StartDate
                doSendBillingDataCheckContractStartDate sendBill = new doSendBillingDataCheckContractStartDate();
                sendBill.ContractCode = ContractCode;
                sendBill.StartDate = StartDate;
                ApplicationErrorException.CheckMandatoryField(sendBill);

                IBillingTempHandler billTempHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                IRentralContractHandler rentralHandler = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                using (TransactionScope scope = new TransactionScope())
                {
                    //2.	Get rental contract basic data
                    List<tbt_RentalContractBasic> doTbt_RentalContractBasic = rentralHandler.GetTbt_RentalContractBasic(ContractCode, null);

                    string strProductTypeCode = string.Empty;
                    if (doTbt_RentalContractBasic != null && doTbt_RentalContractBasic.Count > 0)
                    {
                        strProductTypeCode = doTbt_RentalContractBasic[0].ProductTypeCode;

                        //3.	Get billing basic data of contact fee from billing temp
                        BillingTypeList = new List<string>();
                        BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE);
                        BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON);
                        BillingTypeList.Add(ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE);

                        //3.1 Get last OCC from unimplement data
                        var lastOCC = rentralHandler.GetLastUnimplementedOCC(ContractCode);

                        List<doBillingTempBasic> doBillingTempBasicListContractFee = billTempHandler.GetBillingBasicData(ContractCode, lastOCC, BillingTypeList, null);                            

                        //4.	Get billing basic data of approve phase from billing temp
                        BillingTimingList = new List<string>();
                        BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_START_SERVICE);

                        List<doBillingTempBasic> doBillingTempBasicListStartPhase = billTempHandler.GetBillingBasicData(ContractCode, null, null, BillingTimingList);   
                    
                        //5.	Prepare data object
                        //5.1	Create data object
                        List<doBillingTempBasic> doBillingTempBasicList = new List<doBillingTempBasic>();

                        //5.2	Merge data object (merge by exclude duplicate)
                        var lstBillingBasic = doBillingTempBasicListContractFee.Union(doBillingTempBasicListStartPhase).ToList();
                        var uniqueBillingBasic = (from t in lstBillingBasic
                                                  group t by new
                                                  {
                                                      ContractCode = t.ContractCode,
                                                      BillingClientCode = t.BillingClientCode,
                                                      ContractBillingType = t.ContractBillingType,
                                                      ContractTiming = t.ContractTiming,
                                                      BillingOfficeCode = t.BillingOfficeCode
                                                  } into g
                                                  select g.FirstOrDefault());

                        //5.3	Set other value in doBillingTempBasicList[]				//For all item in list
                        doBillingTempBasicList = uniqueBillingBasic.ToList<doBillingTempBasic>();
                        foreach (doBillingTempBasic data in doBillingTempBasicList)
                        {
                            // case new start but already altearnative start
                            if (!String.IsNullOrEmpty(strStartType))
                            {
                                data.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_AFTER_START_CHANGE_PLAN;
                            }
                            // case new start or alternative start
                            else
                            {
                                data.ContractTiming = SendToBillingTiming.C_CONTRACT_TIMING_START;
                            }
                            data.ProductTypeCode = strProductTypeCode;
                            if (!CommonUtil.IsNullOrEmpty(StartDate))
                                data.ChangeFeeDate = (DateTime)StartDate;
                        }

                        //6.
                        if (doBillingTempBasicList != null && doBillingTempBasicList.Count > 0)
                        {
                            //6.1	Send billing basic data to billing module
                            doBillingTempBasicList = billHandler.ManageBillingBasic(doBillingTempBasicList);

                            //6.2	Loop btbl as doBillingTempBasic in list of doBillingTempBasicList[]
                            foreach (doBillingTempBasic doOutBillingBasic in doBillingTempBasicList)
                            {
                                //6.2.1.	Update billing OCC and billing target code to billing temp	
                                billTempHandler.UpdateBillingTempByBillingClientAndOffice(
                                    doOutBillingBasic.ContractCode, doOutBillingBasic.BillingClientCode
                                    , doOutBillingBasic.BillingOfficeCode, doOutBillingBasic.BillingOCC
                                    , doOutBillingBasic.BillingTargetCode);
                            }
                        }    
                    }

                    //7.	Get billing detail of start service phase from billing temp
                    BillingTimingList = new List<string>();
                    BillingTimingList.Add(BillingTiming.C_BILLING_TIMING_START_SERVICE);

                    List<doBillingTempDetail> doBillingTempDetailList = billTempHandler.GetBillingDetailData(ContractCode, null, null, BillingTimingList);			

                    //8.
                    if (doBillingTempDetailList != null && doBillingTempDetailList.Count > 0)
                    {
                        //8.1	Prepare data object
                        //8.1.1.	Set data  to data object 
                        foreach (doBillingTempDetail doOutBillingDetail in doBillingTempDetailList)
                        {
                            doOutBillingDetail.BillingDate = StartDate.Value;
                            if (String.IsNullOrEmpty(strProductTypeCode) == false)
                                doOutBillingDetail.ProductTypeCode = strProductTypeCode;
                        }

                        //8.2	Send billing detail data to billing module
                        doBillingTempDetailList = billHandler.ManageBillingDetailByContract(doBillingTempDetailList);

                        //8.3	Update send flag in Billing temp
                        foreach (doBillingTempDetail doOutBillingDetail in doBillingTempDetailList)
                        {
                            billTempHandler.UpdateSendFlag(doOutBillingDetail.ContractCode, doOutBillingDetail.SequenceNo, doOutBillingDetail.OCC);
                        }
                    }

                    if (String.IsNullOrEmpty(strStartType))
                    {
                        //8.4	Call sending start service data to billing module
                        bool blnProcessStartResult = billHandler.ManageBillingBasicForStart(ContractCode, StartDate, AdjustBillingTermEndDate);
                    }

                    //9.	If there is no error from all previous steps
                    scope.Complete(); //commit trans
                    blnProcessResult = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        /// <summary>
        /// Send stop service command to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="StopDate"></param>
        /// <param name="StopFee"></param>
        /// <returns></returns>
        public bool SendBilling_StopService(string ContractCode, DateTime? StopDate, decimal? StopFee)
        {
            bool blnProcessResult = false;

            try
            {
                //1.	Check mandatory data ContractCode and StopDate
                doSendBillingDataCheckContractStopDate sendBill = new doSendBillingDataCheckContractStopDate();
                sendBill.ContractCode = ContractCode;
                sendBill.StopDate = StopDate;
                ApplicationErrorException.CheckMandatoryField(sendBill);

                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

                //2.	Call sending resume service data to billing module
                bool blnProcessStopResult = billHandler.ManageBillingBasicForStop(ContractCode, StopDate.Value, StopFee == null ? 0 : StopFee.Value);

                blnProcessResult = true;
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        /// <summary>
        /// Send billing detail when register result base of maintenance contract to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="BillingOCC"></param>
        /// <param name="ResultBaseFee"></param>
        /// <returns></returns>
        public bool SendBilling_MAResultBase(string ContractCode, string BillingOCC, decimal? ResultBaseFee)
        {
            bool blnProcessResult = false;

            try
            {
                //1.	Check mandatory data ContractCode and ResultBaseFee
                doSendBillingDataCheckContractFeeOCC sendBill = new doSendBillingDataCheckContractFeeOCC();
                sendBill.ContractCode = ContractCode;
                sendBill.BillingOCC = BillingOCC;
                sendBill.ResultBaseFee = ResultBaseFee;
                ApplicationErrorException.CheckMandatoryField(sendBill);

                //2.	Check MA result base fee must more than 0
                if (ResultBaseFee <= 0)
                    return true;

                //4.	Create data object for sending
                List<doBillingTempDetail> doBillingTempDetailList = new List<doBillingTempDetail>();
                doBillingTempDetail doBillingTempDetail = new doBillingTempDetail();
                doBillingTempDetail.ContractCode = ContractCode;
                doBillingTempDetail.BillingOCC = BillingOCC;
                doBillingTempDetail.ContractBillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE;
                doBillingTempDetail.BillingDate = DateTime.Now;
                doBillingTempDetail.BillingAmount = ResultBaseFee;
                doBillingTempDetail.PaymentMethod = null;
                doBillingTempDetail.ProductTypeCode = ProductType.C_PROD_TYPE_MA;
                doBillingTempDetail.ProductCode = null;
                doBillingTempDetailList.Add(doBillingTempDetail);

                //5.	Send billing detail data to billing module
                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                doBillingTempDetailList = billHandler.ManageBillingDetailByContract(doBillingTempDetailList);

                blnProcessResult = true;
            }
            catch (Exception)
            {
                throw;
            }

            return blnProcessResult;
        }

        //No use
        //public List<tbt_BillingBasic> GetBillingBasicByContractCode(string ContractCode)
        //{
        //    //mock data
        //    List<tbt_BillingBasic> lst = new List<tbt_BillingBasic>();
        //    return lst;
        //}

        /// <summary>
        /// Getting billing target data from billing module
        /// </summary>
        /// <param name="billingTargetCode"></param>
        /// <returns></returns>
        public List<tbt_BillingTarget> GetBillingTarget(string billingTargetCode)
        {
            IBillingHandler billinghandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            List<tbt_BillingTarget> billingRes = billinghandler.GetTbt_BillingTarget(billingTargetCode, null, null);
            //if (billingRes.Count > 0)
            //{
            //    return billingRes[0];
            //}
            //else
            //{
            //    return null;
            //}
            return billingRes;
        }

        /// <summary>
        /// Getting billing temp from billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="occCode"></param>
        /// <returns></returns>
        public List<tbt_BillingTemp> GetBillingBasicAsBillingTemp(string ContractCode, string occCode)
        {
            List<tbt_BillingTemp> dtTbt_BillingTemp = new List<tbt_BillingTemp>();

            try
            {
                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                List<BillingBasicList> doBillingBasicList = billHandler.GetBillingBasicListData(ContractCode, null);

                if (doBillingBasicList != null)
                {
                    tbt_BillingTemp billingTemp;
                    foreach (BillingBasicList data in doBillingBasicList)
                    {
                        billingTemp = new tbt_BillingTemp();
                        billingTemp.ContractCode = data.ContractCode;
                        billingTemp.BillingOCC = data.BillingOCC;
                        billingTemp.BillingClientCode = data.BillingClientCode;
                        billingTemp.BillingTargetCode = data.BillingTargetCode;
                        billingTemp.BillingOfficeCode = data.BillingOfficeCode;
                        dtTbt_BillingTemp.Add(billingTemp);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dtTbt_BillingTemp;
        }
    }
}
