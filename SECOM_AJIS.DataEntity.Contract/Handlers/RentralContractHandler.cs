

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Transactions;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract.Handlers;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Installation;
using SECOM_AJIS.Common.Models.EmailTemplates;
using System.Data;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class RentralContractHandler : BizCTDataEntities, IRentralContractHandler
    {
        /// <summary>
        /// Get rental contract data
        /// </summary>
        /// <returns></returns>
        public dsRentalContractData GetRentalContractData()
        {

            return null;
        }

        /// <summary>
        /// To check first installation flag
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public bool IsFirstInstallComplete(string strContractCode)
        {
            bool bIsComplete = false;
            try
            {
                //1. Get first installation complete flag
                IRentralContractHandler hand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<tbt_RentalContractBasic> contractList = hand.GetTbt_RentalContractBasic(strContractCode, null);

                //1.5.	Create result data
                if (contractList != null && contractList.Count > 0)
                {
                    //Display whether new installation is complete or not.
                    //[0]: Not complete
                    //[1]: Complete
                    bIsComplete = contractList[0].FirstInstallCompleteFlag ?? false;
                }
                //2.	Return bIsComplete
                return bIsComplete;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get rental contract basic information
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strUserCode"></param>
        /// <returns></returns>
        public List<dtRentalContractBasicForView> GetRentalContractBasicForView(string strContractCode, string strUserCode)
        {
            List<dtRentalContractBasicForView> dtRentalContractBasicForViewList = base.GetRentalContractBasicForView(strContractCode, strUserCode, MiscType.C_RENTAL_CHANGE_TYPE);
            if (dtRentalContractBasicForViewList != null)
            {
                CommonUtil.MappingObjectLanguage<dtRentalContractBasicForView>(dtRentalContractBasicForViewList);
            }

            return dtRentalContractBasicForViewList;
        }

        /// <summary>
        /// Getting all part of specified contract for creaing a new occurrence or else
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public dsRentalContractData GetEntireContract(string strContractCode, string strOCC)
        {
            dsRentalContractData dsResult = new dsRentalContractData();
            //doMiscTypeCode doMiscType;

            //ICommonHandler hand;
            //List<doMiscTypeCode> misc;
            //List<doMiscTypeCode> listDoMiscTypeCode;
            ICommonHandler comHandler;
            MiscTypeMappingList miscList;

            try
            {
                //1. Get Contract basic 
                IRentralContractHandler rentalHand = ServiceContainer.GetService<IRentralContractHandler>() as IRentralContractHandler;
                List<tbt_RentalContractBasic> contractList = rentalHand.GetTbt_RentalContractBasic(strContractCode, null);

                comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                if (contractList != null && contractList.Count > 0)
                {
                    //Mapping MiscType
                    miscList = new MiscTypeMappingList();
                    miscList.AddMiscType(contractList.ToArray());
                    comHandler.MiscTypeMappingList(miscList);

                    //1.2.	Keep result in dsRentalContract
                    dsResult.dtTbt_RentalContractBasic = contractList;

                    //2.	Determine OCC
                    if (strOCC == null || strOCC == String.Empty)
                    {
                        strOCC = contractList[0].LastOCC.Trim();
                    }

                    //3.	Get Security basic
                    List<tbt_RentalSecurityBasic> securityList = rentalHand.GetTbt_RentalSecurityBasic(strContractCode, strOCC);
                    dsResult.dtTbt_RentalSecurityBasic = securityList;

                    if (securityList != null && securityList.Count > 0)
                    {
                        miscList = new MiscTypeMappingList();
                        miscList.AddMiscType(securityList.ToArray());
                        comHandler.MiscTypeMappingList(miscList);
                        dsResult.dtTbt_RentalSecurityBasic = securityList;

                        //4.	Get BE detail
                        List<tbt_RentalBEDetails> BEList = rentalHand.GetTbt_RentalBEDetails(strContractCode, strOCC);
                        dsResult.dtTbt_RentalBEDetails = BEList;

                        //5.	Get Instrument detail
                        List<tbt_RentalInstrumentDetails> instrumentList = rentalHand.GetTbt_RentalInstrumentDetails(strContractCode, strOCC);
                        dsResult.dtTbt_RentalInstrumentDetails = instrumentList;

                        //6.	Get Installation sub-contractor
                        List<tbt_RentalInstSubcontractor> instSubList = rentalHand.GetTbt_RentalInstSubContractor(strContractCode, strOCC);
                        dsResult.dtTbt_RentalInstSubcontractor = instSubList;

                        //7.	Get Maintenance detail
                        List<tbt_RentalMaintenanceDetails> maintenanceList = rentalHand.GetTbt_RentalMaintenanceDetails(strContractCode, strOCC);
                        dsResult.dtTbt_RentalMaintenanceDetails = maintenanceList;

                        //8.	Get Operation type
                        List<tbt_RentalOperationType> operationList = rentalHand.GetTbt_RentalOperationType(strContractCode, strOCC);
                        dsResult.dtTbt_RentalOperationType = operationList;

                        //9.	Get Sentry guard
                        List<tbt_RentalSentryGuard> sentryList = rentalHand.GetTbt_RentalSentryGuard(strContractCode, strOCC);
                        dsResult.dtTbt_RentalSentryGuard = sentryList;

                        //10.	Get Sentry guard detail
                        List<tbt_RentalSentryGuardDetails> sentryDetailList = rentalHand.GetTbt_RentalSentryGuardDetails(strContractCode, strOCC);
                        dsResult.dtTbt_RentalSentryGuardDetails = sentryDetailList;

                        //11.	Get Cancel contract memo
                        List<tbt_CancelContractMemo> memoList = rentalHand.GetTbt_CancelContractMemo(strContractCode, strOCC);
                        dsResult.dtTbt_CancelContractMemo = memoList;

                        //12.	Get Cancel contract memo detail
                        List<tbt_CancelContractMemoDetail> memoDetailList = rentalHand.GetTbt_CancelContractMemoDetail(strContractCode, strOCC);

                        //Modify by Jutarat A. on 17/01/2012
                        //For mapping MiscType
                        //if (memoDetailList.Count() != 0)
                        //{
                        //    doMiscType = new doMiscTypeCode();
                        //    misc = new List<doMiscTypeCode>();
                        //    doMiscType.ValueCode = memoDetailList[0].BillingType;
                        //    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_BILLING_TYPE;
                        //    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_CONTRACT_BILLING_TYPE;
                        //    misc.Add(doMiscType);
                        //    hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        //    listDoMiscTypeCode = hand.GetMiscTypeCodeList(misc);
                        //    memoDetailList[0].BillingTypeName = listDoMiscTypeCode[0].ValueDisplay;

                        //    doMiscType = new doMiscTypeCode();
                        //    misc = new List<doMiscTypeCode>();
                        //    doMiscType.ValueCode = memoDetailList[0].HandlingType;
                        //    doMiscType.FieldName = SECOM_AJIS.Common.Util.ConstantValue.MiscType.C_HANDLING_TYPE;
                        //    misc.Add(doMiscType);
                        //    hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                        //    listDoMiscTypeCode = hand.GetMiscTypeCodeList(misc);
                        //    memoDetailList[0].HandlingTypeName = listDoMiscTypeCode[0].ValueDisplay;
                        //}
                        if (memoDetailList != null && memoDetailList.Count > 0)
                        {
                            miscList = new MiscTypeMappingList();
                            miscList.AddMiscType(memoDetailList.ToArray());
                            comHandler.MiscTypeMappingList(miscList);
                        }

                        dsResult.dtTbt_CancelContractMemoDetail = memoDetailList;

                        //13.	Get Relation type
                        List<tbt_RelationType> relationTypeList = GetTbt_RelationType(strContractCode, strOCC, null);
                        dsResult.dtTbt_RelationType = relationTypeList;
                    }
                    else
                    {
                        dsResult = null;
                    }
                }
                else
                {
                    dsResult = null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return dsResult;
        }

        /// <summary>
        /// Getting last occurrence of unimplemented contract. If there is no unimplemented occurrence, return null
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public string GetLastUnimplementedOCC(string strContractCode)
        {
            try
            {
                List<string> lastOCC = base.GetLastUnimplementedOCCs(strContractCode);

                if (lastOCC.Count > 0 && lastOCC[0] != null && lastOCC[0] != string.Empty)
                    return lastOCC[0];
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<dtRelatedContract> GetRelatedContractList(string pchrRelationType, string pchvstrContractCode, string pchrOCC)
        {
            return base.GetRelatedContractList(pchrRelationType, pchvstrContractCode, pchrOCC, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL, SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US);
        }

        /// <summary>
        /// Getting last occurrence of implemented contract. If there is no implemented occurrence, return as null
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public string GetLastImplementedOCC(string strContractCode)
        {
            try
            {
                List<string> lastOCC = base.GetLastImplementedOCCs(strContractCode);

                if (lastOCC.Count > 0 && lastOCC[0] != null && lastOCC[0] != string.Empty)
                    return lastOCC[0];
                else
                    return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Getting previous OCC number
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public string GetPreviousImplementedOCC(string contractCode, string occ)
        {
            try
            {
                List<string> lastImplementOCC = base.GetPreviousImplementedOCC(contractCode, occ, FlagType.C_FLAG_ON);
                if (lastImplementOCC.Count > 0 && lastImplementOCC[0] != null && lastImplementOCC[0] != string.Empty)
                    return lastImplementOCC[0];
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Getting previous OCC number
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public string GetPreviousUnimplementedOCC(string contractCode, string occ)
        {
            try
            {
                List<string> lastUnimplementOCC = base.GetPreviousUnimplementedOCC(contractCode, occ, FlagType.C_FLAG_OFF);
                if (lastUnimplementOCC.Count > 0 && String.IsNullOrEmpty(lastUnimplementOCC[0]) == false)
                    return lastUnimplementOCC[0];
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Getting next OCC number
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public string GetNextImplementedOCC(string contractCode, string occ)
        {
            try
            {
                List<string> nextImplementOCC = base.GetNextImplementedOCC(contractCode, occ, FlagType.C_FLAG_ON);
                if (nextImplementOCC.Count > 0 && String.IsNullOrEmpty(nextImplementOCC[0]) == false)
                    return nextImplementOCC[0];
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To generate contract occurrence
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="bImplementFlag"></param>
        /// <returns></returns>
        public string GenerateContractOCC(string strContractCode, bool bImplementFlag)
        {
            try
            {
                string strOCC = string.Empty;

                //1.	If bImplementFlag = FALSE Then
                if (!bImplementFlag)
                {
                    //1.1.	Call method to get last contract occurrence of implemented rental contract
                    string strLastOCC = this.GetLastUnimplementedOCC(strContractCode);

                    //1.1.1.	If strLastOCC is null Then
                    if (strLastOCC == null)
                        strOCC = OCCType.C_FIRST_UNIMPLEMENTED_SECURITY_OCC;
                    else
                        strOCC = (Int32.Parse(strLastOCC) + 1).ToString();

                    strOCC = strOCC.ToString().PadLeft(4, '0');
                }
                else //2.	If bImplementFlag = TRUE Then
                {
                    //2.1.	Call method to get last contract occurrence unimplemented rental contract
                    string strLastOCC = this.GetLastImplementedOCC(strContractCode);

                    //2.1.1.	Check strLastOCC and set strOCC
                    if (strLastOCC == null)
                        strOCC = OCCType.C_FIRST_IMPLEMENTED_SECURITY_OCC;
                    else
                    {
                        decimal dLastOCC = Decimal.Parse(strLastOCC); //9965
                        decimal dOCC = Math.Round(dLastOCC / 10, 0, MidpointRounding.AwayFromZero); // Round(996.5) => 997
                        int iOCC = (Decimal.ToInt32(dOCC) * 10) - 10; //997*10-10 = 9960
                        strOCC = iOCC.ToString().PadLeft(4, '0');

                        if (strOCC == "0000")
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3023);
                        }
                    }
                }

                //3.	Return strOCC
                return strOCC;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To generate contract counter
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public int GenerateContractCounter(string strContractCode)
        {
            try
            {
                //1.	Call method to get last contract occurrence of unimplemented rental contract
                string strLastOCC = this.GetLastUnimplementedOCC(strContractCode);

                //2.	Get contract counter number
                List<int?> lastCounterNo = base.GetContractCounterNo(strContractCode, strLastOCC);

                //3.	check iLastCounter
                int iCounter = 0;
                if (lastCounterNo.Count <= 0 || lastCounterNo[0] == null)
                {
                    iCounter = 1;
                }
                else if (lastCounterNo[0] == 99)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT, MessageUtil.MessageList.MSG3024);
                }
                else
                {
                    iCounter = lastCounterNo[0].Value + 1;
                }

                //4.	return iCounter
                return iCounter;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Auto renew contract
        /// </summary>
        public void AutoRenew()
        {
            try
            {
                //2.	Get contract from Rental contract DB which end contract date <= next month
                List<doContractAutoRenew> doRentalSecurityBasicList = this.GetContractExpireNextMonth(DateTime.Now);

                //3.	Check getting value
                if (doRentalSecurityBasicList.Count <= 0)
                    return;

                using (TransactionScope scope = new TransactionScope())
                {
                    //4.	Loop for all contract code in doContractAutoRenew[]
                    foreach (doContractAutoRenew doRenew in doRentalSecurityBasicList)
                    {
                        //4.1	Update end contract date with auto renew (month)
                        //4.1.1.	Update implement data in rental security basic table
                        doRenew.CalContractEndDate = doRenew.CalContractEndDate.Value.AddMonths(doRenew.AutoRenewMonth.Value);
                        tbt_RentalSecurityBasic doBasic = CommonUtil.CloneObject<doContractAutoRenew, tbt_RentalSecurityBasic>(doRenew);
                        List<tbt_RentalSecurityBasic> updatedDo = this.UpdateTbt_RentalSecurityBasic(doBasic);

                        //4.1.2.	Update unimplement data in rental security basic table
                        //4.1.2.1.	Set local variable 
                        //4.1.2.2.	Get last unimplemented OCC
                        string strUnimplementOCC = this.GetLastUnimplementedOCC(doBasic.ContractCode);

                        //4.1.2.3.	If strUnimplementOCC is not empty Then
                        if (strUnimplementOCC != null)
                        {
                            doBasic.OCC = strUnimplementOCC;
                            doBasic.UpdateDate = updatedDo[0].UpdateDate;
                            this.UpdateTbt_RentalSecurityBasic(doBasic);
                        }

                        //4.1.3.	In case product type is maintenance, must call generate maintenance schedule
                        //    If doContractAutoRenew[].ProductTypeCode = C_PROD_TYPE_MA  Then
                        if (doRenew.ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                        {
                            IMaintenanceHandler hand = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                            hand.GenerateMaintenanceSchedule(doRenew.ContractCode, GenerateMAProcessType.C_GEN_MA_TYPE_RE_CREATE);
                        }
                    }

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get contract data where expire next month of BatchDate
        /// </summary>
        /// <param name="BatchDate"></param>
        /// <returns></returns>
        public List<doContractAutoRenew> GetContractExpireNextMonth(DateTime BatchDate)
        {
            return base.GetContractExpireNextMonth(ContractStatus.C_CONTRACT_STATUS_AFTER_START, BatchDate);
        }

        //public int UpdateTbt_RentalSecurityBasic(tbt_RentalSecurityBasic dotbt_RentalSecurityBasic)
        //{
        //    List<tbt_RentalSecurityBasic> lst = this.UpdateTbt_RentalSecurityBasicCore(dotbt_RentalSecurityBasic);
        //    return lst.Count;
        //}

        //public tbt_RentalSecurityBasic UpdateTbt_RentalSecurityBasicReturnDoOut(tbt_RentalSecurityBasic dotbt_RentalSecurityBasic)
        //{
        //    List<tbt_RentalSecurityBasic> lst = this.UpdateTbt_RentalSecurityBasicCore(dotbt_RentalSecurityBasic);
        //    if (lst.Count > 0)
        //        return lst[0];
        //    else
        //        return null;
        //}

        /// <summary>
        /// Update rental security basic data
        /// </summary>
        /// <param name="dotbt_RentalSecurityBasic"></param>
        /// <returns></returns>
        public List<tbt_RentalSecurityBasic> UpdateTbt_RentalSecurityBasic(tbt_RentalSecurityBasic dotbt_RentalSecurityBasic)
        {
            try
            {
                //Check whether this record is the most updated data
                List<tbt_RentalSecurityBasic> rList = this.GetTbt_RentalSecurityBasic(dotbt_RentalSecurityBasic.ContractCode, dotbt_RentalSecurityBasic.OCC);
                //if (rList[0].UpdateDate != dotbt_RentalSecurityBasic.UpdateDate)
                if (DateTime.Compare(rList[0].UpdateDate.Value, dotbt_RentalSecurityBasic.UpdateDate.Value) != 0)
                {
                    if (rList[0].UpdateDate.Value.ToLongDateString().Equals(dotbt_RentalSecurityBasic.UpdateDate.Value.ToLongDateString()) == false)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                    }
                }

                //set updateDate and updateBy
                dotbt_RentalSecurityBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dotbt_RentalSecurityBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RentalSecurityBasic> tbt_RentalSecurityBasicList = new List<tbt_RentalSecurityBasic>();
                tbt_RentalSecurityBasicList.Add(dotbt_RentalSecurityBasic);
                List<tbt_RentalSecurityBasic> updatedList = base.UpdateTbt_RentalSecurityBasic(CommonUtil.ConvertToXml_Store<tbt_RentalSecurityBasic>(tbt_RentalSecurityBasicList));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_RNT_SECURITY_BASIC;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update rental security basic core data
        /// </summary>
        /// <param name="dotbt_RentalContractBasic"></param>
        /// <returns></returns>
        public List<tbt_RentalContractBasic> UpdateTbt_RentalContractBasicCore(tbt_RentalContractBasic dotbt_RentalContractBasic)
        {
            try
            {
                //Check whether this record is the most updated data
                List<tbt_RentalContractBasic> rList = this.GetTbt_RentalContractBasic(dotbt_RentalContractBasic.ContractCode, null);
                //if (rList[0].UpdateDate != dotbt_RentalContractBasic.UpdateDate)
                if (DateTime.Compare(rList[0].UpdateDate.Value, dotbt_RentalContractBasic.UpdateDate.Value) != 0)
                {
                    if (rList[0].UpdateDate.Value.ToLongDateString().Equals(dotbt_RentalContractBasic.UpdateDate.Value.ToLongDateString()) == false)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                    }
                }

                //set updateDate and updateBy
                dotbt_RentalContractBasic.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dotbt_RentalContractBasic.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RentalContractBasic> tbt_RentalContractBasicList = new List<tbt_RentalContractBasic>();
                tbt_RentalContractBasicList.Add(dotbt_RentalContractBasic);
                List<tbt_RentalContractBasic> updatedList = base.UpdateTbt_RentalContractBasic(CommonUtil.ConvertToXml_Store<tbt_RentalContractBasic>(tbt_RentalContractBasicList));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_RNT_CONTRACT_BASIC;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Replace the summary fields in contract basic with current OCC.
        /// </summary>
        /// <param name="dsRental"></param>
        /// <returns></returns>
        public dsRentalContractData UpdateSummaryFields(ref dsRentalContractData dsRental)
        {
            try
            {
                /*
                if(dsRental.dtTbt_RentalSecurityBasic[0].ImplementFlag == FlagType.C_FLAG_ON
                    || dsRental.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
                ){
                    dsRental.dtTbt_RentalContractBasic[0].LastOCC = dsRental.dtTbt_RentalSecurityBasic[0].OCC;
                }
                */

                // Natthavat.s
                // Move out of 'if' condition
                dsRental.dtTbt_RentalContractBasic[0].LastChangeImplementDate = dsRental.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;

                if (dsRental.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE ||
                        dsRental.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE ||
                        dsRental.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG ||
                        dsRental.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA ||

                        (((dsRental.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL) ||
                        (dsRental.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)) &&
                        dsRental.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||

                        (((dsRental.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL) ||
                        (dsRental.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)) &&
                        (dsRental.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START ||
                        dsRental.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING) &&
                        dsRental.dtTbt_RentalSecurityBasic[0].ImplementFlag == FlagType.C_FLAG_ON))
                {

                    dsRental.dtTbt_RentalContractBasic[0].LastOCC = dsRental.dtTbt_RentalSecurityBasic[0].OCC;
                    dsRental.dtTbt_RentalContractBasic[0].LastChangeType = dsRental.dtTbt_RentalSecurityBasic[0].ChangeType;
                    //dsRental.dtTbt_RentalContractBasic[0].LastChangeImplementDate = dsRental.dtTbt_RentalSecurityBasic[0].ChangeImplementDate;
                    // Add by Pachara S. 16032017
                    dsRental.dtTbt_RentalContractBasic[0].LastNormalContractFee = dsRental.dtTbt_RentalSecurityBasic[0].NormalContractFee;
                    dsRental.dtTbt_RentalContractBasic[0].LastNormalContractFeeUsd = dsRental.dtTbt_RentalSecurityBasic[0].NormalContractFeeUsd;
                    dsRental.dtTbt_RentalContractBasic[0].LastNormalContractFeeCurrencyType = dsRental.dtTbt_RentalSecurityBasic[0].NormalContractFeeCurrencyType;
                    dsRental.dtTbt_RentalContractBasic[0].LastOrderContractFee = dsRental.dtTbt_RentalSecurityBasic[0].OrderContractFee;
                    dsRental.dtTbt_RentalContractBasic[0].LastOrderContractFeeUsd = dsRental.dtTbt_RentalSecurityBasic[0].OrderContractFeeUsd;
                    dsRental.dtTbt_RentalContractBasic[0].LastOrderContractFeeCurrencyType = dsRental.dtTbt_RentalSecurityBasic[0].OrderContractFeeCurrencyType;
                }

                return dsRental;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update rental contract basic
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        public List<tbt_RentalContractBasic> UpdateTbt_RentalContractBasic(tbt_RentalContractBasic doUpdate)
        {
            try
            {
                //Check whether this record is the most updated data
                List<tbt_RentalContractBasic> rList = this.GetTbt_RentalContractBasic(doUpdate.ContractCode, null);
                //if (rList[0].UpdateDate != doUpdate.UpdateDate)
                if (DateTime.Compare(rList[0].UpdateDate.GetValueOrDefault(), doUpdate.UpdateDate.GetValueOrDefault()) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                //set updateDate and updateBy
                doUpdate.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doUpdate.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RentalContractBasic> doUpdateList = new List<tbt_RentalContractBasic>();
                doUpdateList.Add(doUpdate);
                List<tbt_RentalContractBasic> updatedList = base.UpdateTbt_RentalContractBasic(CommonUtil.ConvertToXml_Store<tbt_RentalContractBasic>(doUpdateList));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_RNT_CONTRACT_BASIC;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update rental sentry guard
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        public List<tbt_RentalSentryGuard> UpdateTbt_RentalSentryGuard(tbt_RentalSentryGuard doUpdate)
        {
            try
            {
                //Check whether this record is the most updated data
                List<tbt_RentalSentryGuard> rList = this.GetTbt_RentalSentryGuard(doUpdate.ContractCode, doUpdate.OCC);
                //if (rList[0].UpdateDate != doUpdate.UpdateDate)
                if (DateTime.Compare(rList[0].UpdateDate.Value, doUpdate.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                //set updateDate and updateBy
                doUpdate.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doUpdate.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RentalSentryGuard> doUpdateList = new List<tbt_RentalSentryGuard>();
                doUpdateList.Add(doUpdate);
                List<tbt_RentalSentryGuard> updatedList = base.UpdateTbt_RentalSentryGuard(CommonUtil.ConvertToXml_Store<tbt_RentalSentryGuard>(doUpdateList));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_RNT_SG;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update cancel contract memo detail
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        public List<tbt_CancelContractMemoDetail> UpdateTbt_CancelContractMemoDetail(tbt_CancelContractMemoDetail doUpdate)
        {
            try
            {
                //Check whether this record is the most updated data
                List<tbt_CancelContractMemoDetail> rList = this.GetTbt_CancelContractMemoDetail(doUpdate.ContractCode, doUpdate.OCC);
                //if (rList[0].UpdateDate != doUpdate.UpdateDate)
                if (DateTime.Compare(rList[0].UpdateDate.Value, doUpdate.UpdateDate.Value) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                //set updateDate and updateBy
                doUpdate.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doUpdate.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_CancelContractMemoDetail> doUpdateList = new List<tbt_CancelContractMemoDetail>();
                doUpdateList.Add(doUpdate);
                List<tbt_CancelContractMemoDetail> updatedList = base.UpdateTbt_CancelContractMemoDetail(CommonUtil.ConvertToXml_Store<tbt_CancelContractMemoDetail>(doUpdateList));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_CAN_ContractMemo_Detail;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// For registering change expected operation date of rental contract
        /// </summary>
        /// <param name="dsRental"></param>
        /// <returns></returns>
        public int RegisterExpectedOperationDate(dsRentalContractData dsRental)
        {
            int iCounter;

            try
            {
                List<tbt_RentalContractBasic> lst = new List<tbt_RentalContractBasic>();

                using (TransactionScope scope = new TransactionScope())
                {
                    //dsRental.dtTbt_RentalSecurityBasic[0].ChangeType = SECOM_AJIS.Common.Util.ConstantValue.RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_EXPECTED_OPR_DATE;
                    iCounter = GenerateContractCounter(dsRental.dtTbt_RentalContractBasic[0].ContractCode);
                    dsRental.dtTbt_RentalSecurityBasic[0].CounterNo = iCounter;
                    lst = this.UpdateTbt_RentalContractBasicCore(dsRental.dtTbt_RentalContractBasic[0]);
                    this.UpdateTbt_RentalSecurityBasic(dsRental.dtTbt_RentalSecurityBasic[0]);

#if !ROUND1
                    //Call update inventory process
                    if (dsRental.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_OFF)
                    {
                        IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
                        invenhandler.ChangeExpectedStartServiceDate(new doBooking()
                        {
                            ContractCode = dsRental.dtTbt_RentalContractBasic[0].ContractCode,
                            ExpectedStartServiceDate = dsRental.dtTbt_RentalSecurityBasic[0].ExpectedOperationDate.GetValueOrDefault()
                        });
                    }
#endif

                    scope.Complete();
                }
                return lst.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete cancel contract memo by key
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public List<tbt_CancelContractMemo> DeleteTbtCancelContractMemoByKey(string contractCode, string occ)
        {
            try
            {
                //Delete data from DB
                List<tbt_CancelContractMemo> deletedList = base.DeleteTbt_CancelContractMemo_ByKey(contractCode, occ);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_CAN_ContractMemo;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete cancel contract memo detail by contract code and occurrence
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public List<tbt_CancelContractMemoDetail> DeleteTbtCancelContractMemoDetailByContractCodeOCC(string contractCode, string occ)
        {
            try
            {
                //Delete data from DB
                List<tbt_CancelContractMemoDetail> deletedList = base.DeleteTbt_CancelContractMemoDetail_ByContractCodeOCC(contractCode, occ);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_CAN_ContractMemo_Detail;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete rental BE detail by key
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public List<tbt_RentalBEDetails> DeleteTbtRentalBEDetailsByKey(string contractCode, string occ)
        {
            try
            {
                //Delete data from DB
                List<tbt_RentalBEDetails> deletedList = base.DeleteTbt_RentalBEDetails_ByKey(contractCode, occ);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_BE_DET;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete rental instrument detail by contract code and occurrence
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public List<tbt_RentalInstrumentDetails> DeleteTbtRentalInstrumentDetailsByContractCodeOCC(string contractCode, string occ)
        {
            try
            {
                //Delete data from DB
                List<tbt_RentalInstrumentDetails> deletedList = base.DeleteTbt_RentalInstrumentDetails_ByContractCodeOCC(contractCode
                    , occ);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_INST;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete rental instrument subcontractor by contract code and occurrence
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public List<tbt_RentalInstSubcontractor> DeleteTbtRentalInstSubContractorByContractCodeOCC(string contractCode, string occ)
        {
            try
            {
                //Delete data from DB
                List<tbt_RentalInstSubcontractor> deletedList = base.DeleteTbt_RentalInstSubContractor_ByContractCodeOCC(contractCode, occ);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_SUBCONT;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete rental security basic by key
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public List<tbt_RentalSecurityBasic> DeleteTbtRentalSecurityBasicByKey(string contractCode, string occ)
        {
            try
            {
                //Delete data from DB
                List<tbt_RentalSecurityBasic> deletedList = base.DeleteTbt_RentalSecurityBasic_ByKey(contractCode, occ);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_SECURITY_BASIC;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete rental sentry guard by key
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public List<tbt_RentalSentryGuard> DeleteTbtRentalSentryGuardByKey(string contractCode, string occ)
        {
            try
            {
                //Delete data from DB
                List<tbt_RentalSentryGuard> deletedList = base.DeleteTbt_RentalSentryGuard_ByKey(contractCode, occ);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_SG;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete rental sentry guard detail by contract code and occurrence
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public List<tbt_RentalSentryGuardDetails> DeleteTbtRentalSentryguardDetailsByContractCodeOCC(string contractCode, string occ)
        {
            try
            {
                //Delete data from DB
                List<tbt_RentalSentryGuardDetails> deletedList = base.DeleteTbt_RentalSentryguardDetails_ByContractCodeOCC(contractCode, occ);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_SG_DET;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete rental operation type by key
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public List<tbt_RentalOperationType> DeleteTbtRentalOperationTypeByKey(string contractCode, string occ)
        {
            try
            {
                //Delete data from DB
                List<tbt_RentalOperationType> deletedList = base.DeleteTbt_RentalOperationType_ByKey(contractCode, occ);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_OPER_TYPE;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete rental maintenance detail by key
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public List<tbt_RentalMaintenanceDetails> DeleteTbtRentalMaintenanceDetailsByKey(string contractCode, string occ)
        {
            try
            {
                //Delete data from DB
                List<tbt_RentalMaintenanceDetails> deletedList = base.DeleteTbt_RentalMaintenanceDetails_ByKey(contractCode, occ);

                //Insert Log
                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_MA;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete all one time fee
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <param name="pBillingOCC"></param>
        /// <param name="pBillingTargetRunningNo"></param>
        /// <param name="pBillingClientCode"></param>
        /// <param name="pBillingTargetCode"></param>
        /// <param name="pBillingOfficeCode"></param>
        /// <param name="pBillingType"></param>
        /// <param name="pBillingTiming"></param>
        /// <param name="pBillingAmt"></param>
        /// <param name="pPayMethod"></param>
        /// <param name="pBillingCycle"></param>
        /// <param name="pCalDailyFeeStatus"></param>
        /// <param name="pSendFlag"></param>
        /// <param name="pProcessDateTime"></param>
        /// <param name="pEmpNo"></param>
        /// <returns></returns>
        public List<tbt_BillingTemp> DeleteAllOneTimeFee(string contractCode, string occ,
        string pContractCode, string pOCC, string pBillingOCC, string pBillingTargetRunningNo, string pBillingClientCode, string pBillingTargetCode, string pBillingOfficeCode, string pBillingType, string pBillingTiming, Nullable<decimal> pBillingAmt, string pPayMethod, Nullable<int> pBillingCycle, string pCalDailyFeeStatus, string pSendFlag, Nullable<System.DateTime> pProcessDateTime, string pEmpNo, decimal pBillingAmtUsd, string pBillingAmtCurrencyType)
        {
            try
            {
                List<tbt_BillingTemp> deletedList = base.DeleteAllOneTimeFee(contractCode, occ, ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE, ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                pContractCode, pOCC, pBillingOCC, pBillingTargetRunningNo, pBillingClientCode, pBillingTargetCode, pBillingOfficeCode, pBillingType, pBillingTiming, pBillingAmt, pPayMethod, pBillingCycle, pCalDailyFeeStatus, pSendFlag, pProcessDateTime, pEmpNo, pBillingAmtUsd, pBillingAmtCurrencyType);

                if (deletedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_TEMP;
                    logData.TableData = CommonUtil.ConvertToXml(deletedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return deletedList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// For cancel unoperated contract
        /// </summary>
        /// <param name="dsRental"></param>
        public void CancelUnoperatedContract(dsRentalContractData dsRental)
        {
            string strLastUnimplementedOCC;
            dsRentalContractData dsRentalContract;

            try
            {
                List<tbt_RentalSecurityBasic> rList = this.GetTbt_RentalSecurityBasic(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC);

                if (rList.Count != 0)
                {
                    //if (rList[0].UpdateDate != dsRental.dtTbt_RentalSecurityBasic[0].UpdateDate)
                    if (DateTime.Compare(rList[0].UpdateDate.Value, dsRental.dtTbt_RentalSecurityBasic[0].UpdateDate.Value) != 0)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                    }

                    using (TransactionScope scope = new TransactionScope())
                    {
                        //DeleteTbtRentalSecurityBasicByKey(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC);
                        //DeleteTbtRentalBEDetailsByKey(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC);
                        //DeleteTbtRentalInstrumentDetailsByContractCodeOCC(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC);
                        //DeleteTbtRentalSentryGuardByKey(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC);
                        //DeleteTbtRentalSentryguardDetailsByContractCodeOCC(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC);
                        //DeleteTbtCancelContractMemoByKey(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC);
                        //DeleteTbtCancelContractMemoDetailByContractCodeOCC(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC);
                        //DeleteTbtRentalOperationTypeByKey(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC);
                        //DeleteTbtRentalInstSubContractorByContractCodeOCC(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC);
                        //DeleteTbtRentalMaintenanceDetailsByKey(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC);

                        DeleteEntireOCC(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC, dsRental.dtTbt_RentalContractBasic[0].UpdateDate.GetValueOrDefault());

                        //Delete billing temp
                        IBillingTempHandler bhandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                        bhandler.DeleteBillingTempByContractCodeOCC(
                            dsRental.dtTbt_RentalSecurityBasic[0].ContractCode,
                            dsRental.dtTbt_RentalSecurityBasic[0].OCC);

                        strLastUnimplementedOCC = GetLastUnimplementedOCC(dsRental.dtTbt_RentalContractBasic[0].ContractCode);
                        dsRentalContract = GetEntireContract(dsRental.dtTbt_RentalContractBasic[0].ContractCode, strLastUnimplementedOCC);
                        UpdateSummaryFields(ref dsRentalContract);
                        UpdateEntireContract(dsRentalContract);

                        scope.Complete();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete entire contract data with specific occurrence
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <param name="dtRentalContractBasicUpdateDate"></param>
        /// <returns></returns>
        public dsRentalContractData DeleteEntireOCC(string strContractCode, string strOCC, DateTime dtRentalContractBasicUpdateDate)
        {
            dsRentalContractData dsResult = new dsRentalContractData();
            try
            {
                //Check whether this record is the most updated data
                List<tbt_RentalContractBasic> rList = this.GetTbt_RentalContractBasic(strContractCode, null);
                //if (rList[0].UpdateDate != dtRentalContractBasicUpdateDate)
                if (DateTime.Compare(rList[0].UpdateDate.Value, dtRentalContractBasicUpdateDate) != 0)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                }

                dsResult.dtTbt_RentalSecurityBasic = this.DeleteTbtRentalSecurityBasicByKey(strContractCode, strOCC);
                dsResult.dtTbt_RentalBEDetails = this.DeleteTbtRentalBEDetailsByKey(strContractCode, strOCC);
                dsResult.dtTbt_RentalInstrumentDetails = this.DeleteTbtRentalInstrumentDetailsByContractCodeOCC(strContractCode, strOCC);
                dsResult.dtTbt_RentalSentryGuard = this.DeleteTbtRentalSentryGuardByKey(strContractCode, strOCC);
                dsResult.dtTbt_RentalSentryGuardDetails = this.DeleteTbtRentalSentryguardDetailsByContractCodeOCC(strContractCode, strOCC);
                dsResult.dtTbt_CancelContractMemo = this.DeleteTbtCancelContractMemoByKey(strContractCode, strOCC);
                dsResult.dtTbt_CancelContractMemoDetail = this.DeleteTbtCancelContractMemoDetailByContractCodeOCC(strContractCode, strOCC);
                dsResult.dtTbt_RentalOperationType = this.DeleteTbtRentalOperationTypeByKey(strContractCode, strOCC);
                dsResult.dtTbt_RentalInstSubcontractor = this.DeleteTbtRentalInstSubContractorByContractCodeOCC(strContractCode, strOCC);
                dsResult.dtTbt_RentalMaintenanceDetails = this.DeleteTbtRentalMaintenanceDetailsByKey(strContractCode, strOCC);
                dsResult.dtTbt_RelationType = this.DeleteTbt_RelationType_ByContractCodeOCC(strContractCode, strOCC);

            }
            catch (Exception)
            {
                throw;
            }
            return dsResult;
        }

        /// <summary>
        /// Insert entire contract data
        /// </summary>
        /// <param name="dsData"></param>
        /// <returns></returns>
        public dsRentalContractData InsertEntireContract(dsRentalContractData dsData)
        {
            dsRentalContractData dsResult = new dsRentalContractData();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    string strContractCode = dsData.dtTbt_RentalContractBasic[0].ContractCode;

                    List<tbt_RentalContractBasic> rList = this.GetTbt_RentalContractBasic(strContractCode, null);
                    if (rList.Count > 0)
                    {
                        //insert Tbt_RentalContractBasic
                        dsResult.dtTbt_RentalContractBasic = this.UpdateTbt_RentalContractBasic(dsData.dtTbt_RentalContractBasic[0]);
                    }
                    else
                        //update Tbt_RentalContractBasic
                        dsResult.dtTbt_RentalContractBasic = this.InsertTbt_RentalContractBasic(dsData.dtTbt_RentalContractBasic[0]);

                    string productTypeCode = dsResult.dtTbt_RentalContractBasic[0].ProductTypeCode;

                    //InsertTbt_RentalSecurityBasic
                    dsResult.dtTbt_RentalSecurityBasic = this.InsertTbt_RentalSecurityBasic(dsData.dtTbt_RentalSecurityBasic[0]);

                    //InsertTbt_RentalBEDetails
                    if (productTypeCode == ProductType.C_PROD_TYPE_BE && dsData.dtTbt_RentalBEDetails != null)
                    {
                        dsResult.dtTbt_RentalBEDetails = new List<tbt_RentalBEDetails>();
                        foreach (tbt_RentalBEDetails beDetail in dsData.dtTbt_RentalBEDetails)
                        {
                            List<tbt_RentalBEDetails> beDetailList = this.InsertTbt_RentalBEDetails(beDetail);
                            dsResult.dtTbt_RentalBEDetails.AddRange(beDetailList);
                        }
                    }

                    //InsertTbt_RentalInstrumentDetails
                    if (dsData.dtTbt_RentalInstrumentDetails != null)
                    {
                        dsResult.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                        foreach (tbt_RentalInstrumentDetails instDetail in dsData.dtTbt_RentalInstrumentDetails)
                        {
                            List<tbt_RentalInstrumentDetails> instDetailList = this.InsertTbt_RentalInstrumentDetails(instDetail);
                            dsResult.dtTbt_RentalInstrumentDetails.AddRange(instDetailList);
                        }
                    }

                    if (productTypeCode == ProductType.C_PROD_TYPE_SG)
                    {
                        //InsertTbt_RentalSentryGuard
                        if (dsData.dtTbt_RentalSentryGuard != null)
                        {
                            dsResult.dtTbt_RentalSentryGuard = new List<tbt_RentalSentryGuard>();
                            foreach (tbt_RentalSentryGuard senGuard in dsData.dtTbt_RentalSentryGuard)
                            {
                                List<tbt_RentalSentryGuard> senGuardList = this.InsertTbt_RentalSentryGuard(senGuard);
                                dsResult.dtTbt_RentalSentryGuard.AddRange(senGuardList);
                            }
                        }

                        //InsertTbt_RentalSentryGuardDetails
                        if (dsData.dtTbt_RentalSentryGuardDetails != null)
                        {
                            dsResult.dtTbt_RentalSentryGuardDetails = new List<tbt_RentalSentryGuardDetails>();
                            foreach (tbt_RentalSentryGuardDetails senGuardDetail in dsData.dtTbt_RentalSentryGuardDetails)
                            {
                                List<tbt_RentalSentryGuardDetails> senGuardDetailList = this.InsertTbt_RentalSentryGuardDetails(senGuardDetail);
                                dsResult.dtTbt_RentalSentryGuardDetails.AddRange(senGuardDetailList);
                            }
                        }
                    }

                    //InsertTbt_CancelContractMemoDetail
                    if (dsData.dtTbt_CancelContractMemoDetail != null)
                    {
                        dsResult.dtTbt_CancelContractMemoDetail = new List<tbt_CancelContractMemoDetail>();
                        foreach (tbt_CancelContractMemoDetail cancel in dsData.dtTbt_CancelContractMemoDetail)
                        {
                            List<tbt_CancelContractMemoDetail> cancelList = this.InsertTbtCancelContractMemoDetail(cancel);
                            dsResult.dtTbt_CancelContractMemoDetail.AddRange(cancelList);
                        }
                    }

                    //InsertTbt_RentalOperationType
                    if (dsData.dtTbt_RentalOperationType != null)
                    {
                        dsResult.dtTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                        foreach (tbt_RentalOperationType oper in dsData.dtTbt_RentalOperationType)
                        {
                            List<tbt_RentalOperationType> operList = this.InsertTbt_RentalOperationType(oper);
                            dsResult.dtTbt_RentalOperationType.AddRange(operList);
                        }
                    }

                    //InsertTbt_RentalInstSubContractor
                    //if (dsData.dtTbt_RentalInstSubcontractor != null)
                    //{
                    //      dsResult.dtTbt_RentalInstSubcontractor = new List<tbt_RentalInstSubcontractor>();
                    //    foreach (tbt_RentalInstSubcontractor subCon in dsData.dtTbt_RentalInstSubcontractor)
                    //    {
                    //        List<tbt_RentalInstSubcontractor> subConList = this.InsertTbt_RentalInstSubContractor(subCon);
                    //        dsResult.dtTbt_RentalInstSubcontractor.AddRange(subConList);
                    //    }                    
                    //}

                    //InsertTbt_RentalMaintenanceDetails
                    if (productTypeCode == ProductType.C_PROD_TYPE_MA && dsData.dtTbt_RentalMaintenanceDetails != null)
                    {
                        dsResult.dtTbt_RentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>();
                        foreach (tbt_RentalMaintenanceDetails main in dsData.dtTbt_RentalMaintenanceDetails)
                        {
                            List<tbt_RentalMaintenanceDetails> mainList = this.InsertTbt_RentalMaintenanceDetails(main);
                            dsResult.dtTbt_RentalMaintenanceDetails.AddRange(mainList);
                        }
                    }

                    //Insert relation type
                    if (dsData.dtTbt_RelationType != null)
                    {
                        dsResult.dtTbt_RelationType = new List<tbt_RelationType>();
                        ICommonContractHandler commonHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                        foreach (tbt_RelationType main in dsData.dtTbt_RelationType)
                        {
                            List<tbt_RelationType> mainList = commonHandler.InsertTbt_RelationType(main);
                            dsResult.dtTbt_RelationType.AddRange(mainList);
                        }
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsResult;
        }

        /// <summary>
        /// Insert entire contract data for CTS010
        /// </summary>
        /// <param name="dsData"></param>
        public void InsertEntireContractForCTS010(dsRentalContractData dsData)
        {
            try
            {
                DateTime updateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                string updateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                DateTime? lastUpdateDate = null;
                using (TransactionScope scope = new TransactionScope())
                {
                    string strContractCode = dsData.dtTbt_RentalContractBasic[0].ContractCode;

                    #region Convert Rental contract basic data to xml

                    foreach (tbt_RentalContractBasic rbs in dsData.dtTbt_RentalContractBasic)
                    {
                        lastUpdateDate = rbs.UpdateDate;

                        rbs.CreateDate = updateDate;
                        rbs.CreateBy = updateBy;
                        rbs.UpdateDate = updateDate;
                        rbs.UpdateBy = updateBy;
                    }
                    string xml_RentalContractBasic = CommonUtil.ConvertToXml_Store<tbt_RentalContractBasic>(dsData.dtTbt_RentalContractBasic);

                    #endregion
                    #region Convert Rental security basic data to xml

                    foreach (tbt_RentalSecurityBasic rbs in dsData.dtTbt_RentalSecurityBasic)
                    {
                        rbs.CreateDate = updateDate;
                        rbs.CreateBy = updateBy;
                        rbs.UpdateDate = updateDate;
                        rbs.UpdateBy = updateBy;
                    }
                    string xml_RentalSecurityBasic = CommonUtil.ConvertToXml_Store<tbt_RentalSecurityBasic>(dsData.dtTbt_RentalSecurityBasic);

                    #endregion
                    #region Convert Rental beatguard details data to xml

                    if (dsData.dtTbt_RentalBEDetails != null)
                    {
                        foreach (tbt_RentalBEDetails rbs in dsData.dtTbt_RentalBEDetails)
                        {
                            rbs.CreateDate = updateDate;
                            rbs.CreateBy = updateBy;
                            rbs.UpdateDate = updateDate;
                            rbs.UpdateBy = updateBy;
                        }
                    }
                    string xml_RentalBEDetails = CommonUtil.ConvertToXml_Store<tbt_RentalBEDetails>(dsData.dtTbt_RentalBEDetails);

                    #endregion
                    #region Convert Rental instrument details data to xml

                    if (dsData.dtTbt_RentalInstrumentDetails != null)
                    {
                        foreach (tbt_RentalInstrumentDetails rbs in dsData.dtTbt_RentalInstrumentDetails)
                        {
                            rbs.CreateDate = updateDate;
                            rbs.CreateBy = updateBy;
                            rbs.UpdateDate = updateDate;
                            rbs.UpdateBy = updateBy;
                        }
                    }
                    string xml_RentalInstrumentDetails = CommonUtil.ConvertToXml_Store<tbt_RentalInstrumentDetails>(dsData.dtTbt_RentalInstrumentDetails);

                    #endregion
                    #region Convert Rental sentry guard data to xml

                    if (dsData.dtTbt_RentalSentryGuard != null)
                    {
                        foreach (tbt_RentalSentryGuard rbs in dsData.dtTbt_RentalSentryGuard)
                        {
                            rbs.CreateDate = updateDate;
                            rbs.CreateBy = updateBy;
                            rbs.UpdateDate = updateDate;
                            rbs.UpdateBy = updateBy;
                        }
                    }
                    string xml_RentalSentryGuard = CommonUtil.ConvertToXml_Store<tbt_RentalSentryGuard>(dsData.dtTbt_RentalSentryGuard);

                    #endregion
                    #region Convert Rental sentry guard details data to xml

                    if (dsData.dtTbt_RentalSentryGuardDetails != null)
                    {
                        foreach (tbt_RentalSentryGuardDetails rbs in dsData.dtTbt_RentalSentryGuardDetails)
                        {
                            rbs.CreateDate = updateDate;
                            rbs.CreateBy = updateBy;
                            rbs.UpdateDate = updateDate;
                            rbs.UpdateBy = updateBy;
                        }
                    }
                    string xml_RentalSentryGuardDetails = CommonUtil.ConvertToXml_Store<tbt_RentalSentryGuardDetails>(dsData.dtTbt_RentalSentryGuardDetails);

                    #endregion
                    #region Convert Cancel contract memo details data to xml

                    if (dsData.dtTbt_CancelContractMemoDetail != null)
                    {
                        foreach (tbt_CancelContractMemoDetail rbs in dsData.dtTbt_CancelContractMemoDetail)
                        {
                            rbs.CreateDate = updateDate;
                            rbs.CreateBy = updateBy;
                            rbs.UpdateDate = updateDate;
                            rbs.UpdateBy = updateBy;
                        }
                    }
                    string xml_CancelContractMemoDetail = CommonUtil.ConvertToXml_Store<tbt_CancelContractMemoDetail>(dsData.dtTbt_CancelContractMemoDetail);

                    #endregion
                    #region Convert Rental operation type data to xml

                    if (dsData.dtTbt_RentalOperationType != null)
                    {
                        foreach (tbt_RentalOperationType rbs in dsData.dtTbt_RentalOperationType)
                        {
                            rbs.CreateDate = updateDate;
                            rbs.CreateBy = updateBy;
                            rbs.UpdateDate = updateDate;
                            rbs.UpdateBy = updateBy;
                        }
                    }
                    string xml_RentalOperationType = CommonUtil.ConvertToXml_Store<tbt_RentalOperationType>(dsData.dtTbt_RentalOperationType);

                    #endregion
                    #region Convert Rental maintenance detail data to xml

                    if (dsData.dtTbt_RentalMaintenanceDetails != null)
                    {
                        foreach (tbt_RentalMaintenanceDetails rbs in dsData.dtTbt_RentalMaintenanceDetails)
                        {
                            rbs.CreateDate = updateDate;
                            rbs.CreateBy = updateBy;
                            rbs.UpdateDate = updateDate;
                            rbs.UpdateBy = updateBy;
                        }
                    }
                    string xml_RentalMaintenanceDetails = CommonUtil.ConvertToXml_Store<tbt_RentalMaintenanceDetails>(dsData.dtTbt_RentalMaintenanceDetails);

                    #endregion
                    #region Convert Relation type data to xml

                    if (dsData.dtTbt_RelationType != null)
                    {
                        foreach (tbt_RelationType rbs in dsData.dtTbt_RelationType)
                        {
                            rbs.CreateDate = updateDate;
                            rbs.CreateBy = updateBy;
                            rbs.UpdateDate = updateDate;
                            rbs.UpdateBy = updateBy;
                        }
                    }
                    string xml_RelationType = CommonUtil.ConvertToXml_Store<tbt_RelationType>(dsData.dtTbt_RelationType);

                    #endregion

                    base.InsertEntireContract(
                        strContractCode,
                        CommonUtil.dsTransData.dtOperationData.GUID,
                        CommonUtil.dsTransData.dtTransHeader.ScreenID,
                        updateDate,
                        updateBy,
                        lastUpdateDate,
                        ProductType.C_PROD_TYPE_BE,
                        ProductType.C_PROD_TYPE_SG,
                        ProductType.C_PROD_TYPE_MA,
                        xml_RentalContractBasic,
                        xml_RentalSecurityBasic,
                        xml_RentalBEDetails,
                        xml_RentalInstrumentDetails,
                        xml_RentalSentryGuard,
                        xml_RentalSentryGuardDetails,
                        xml_CancelContractMemoDetail,
                        xml_RentalOperationType,
                        xml_RentalMaintenanceDetails,
                        xml_RelationType);
                    scope.Complete();
                }
            }
            catch (EntityCommandExecutionException entEx)
            {
                string strErrorMsg = string.Empty;
                if (String.IsNullOrEmpty(entEx.InnerException.Message) == false && entEx.InnerException.Message.Length >= 7)
                    strErrorMsg = entEx.InnerException.Message.Substring(0, 7);

                if (strErrorMsg == "MSG0019")
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                else
                    throw entEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Insert rental contract basic data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_RentalContractBasic> InsertTbt_RentalContractBasic(tbt_RentalContractBasic doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RentalContractBasic> doInsertList = new List<tbt_RentalContractBasic>();
                doInsertList.Add(doInsert);
                List<tbt_RentalContractBasic> insertList = base.InsertTbt_RentalContractBasic(CommonUtil.ConvertToXml_Store<tbt_RentalContractBasic>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_RNT_CONTRACT_BASIC;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert rental security basic data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_RentalSecurityBasic> InsertTbt_RentalSecurityBasic(tbt_RentalSecurityBasic doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RentalSecurityBasic> doInsertList = new List<tbt_RentalSecurityBasic>();
                doInsertList.Add(doInsert);
                List<tbt_RentalSecurityBasic> insertList = base.InsertTbt_RentalSecurityBasic(CommonUtil.ConvertToXml_Store<tbt_RentalSecurityBasic>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_RNT_SECURITY_BASIC;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert rental BE detail data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_RentalBEDetails> InsertTbt_RentalBEDetails(tbt_RentalBEDetails doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RentalBEDetails> doInsertList = new List<tbt_RentalBEDetails>();
                doInsertList.Add(doInsert);
                List<tbt_RentalBEDetails> insertList = base.InsertTbt_RentalBEDetails(CommonUtil.ConvertToXml_Store<tbt_RentalBEDetails>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_RNT_BE_DET;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert retnal instrument detail data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_RentalInstrumentDetails> InsertTbt_RentalInstrumentDetails(tbt_RentalInstrumentDetails doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RentalInstrumentDetails> doInsertList = new List<tbt_RentalInstrumentDetails>();
                doInsertList.Add(doInsert);
                List<tbt_RentalInstrumentDetails> insertList = base.InsertTbt_RentalInstrumentDetails(CommonUtil.ConvertToXml_Store<tbt_RentalInstrumentDetails>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_RNT_INST;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert rental sentry guard data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_RentalSentryGuard> InsertTbt_RentalSentryGuard(tbt_RentalSentryGuard doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RentalSentryGuard> doInsertList = new List<tbt_RentalSentryGuard>();
                doInsertList.Add(doInsert);
                List<tbt_RentalSentryGuard> insertList = base.InsertTbt_RentalSentryGuard(CommonUtil.ConvertToXml_Store<tbt_RentalSentryGuard>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_RNT_SG;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert rental sentry guard detail data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_RentalSentryGuardDetails> InsertTbt_RentalSentryGuardDetails(tbt_RentalSentryGuardDetails doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RentalSentryGuardDetails> doInsertList = new List<tbt_RentalSentryGuardDetails>();
                doInsertList.Add(doInsert);
                List<tbt_RentalSentryGuardDetails> insertList = base.InsertTbt_RentalSentryGuardDetails(CommonUtil.ConvertToXml_Store<tbt_RentalSentryGuardDetails>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_RNT_SG_DET;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// insert cancel contract memo detail data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_CancelContractMemoDetail> InsertTbtCancelContractMemoDetail(tbt_CancelContractMemoDetail doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_CancelContractMemoDetail> doInsertList = new List<tbt_CancelContractMemoDetail>();
                doInsertList.Add(doInsert);
                List<tbt_CancelContractMemoDetail> insertList = base.InsertTbt_CancelContractMemoDetail(CommonUtil.ConvertToXml_Store<tbt_CancelContractMemoDetail>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_CAN_ContractMemo_Detail;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert rental operation type data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_RentalOperationType> InsertTbt_RentalOperationType(tbt_RentalOperationType doInsert)
        {
            try
            {
                List<tbt_RentalOperationType> insertList
                    = base.InsertTbt_RentalOperationType(
                        doInsert.ContractCode, doInsert.OCC, doInsert.OperationTypeCode
                        , CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                        , CommonUtil.dsTransData.dtUserData.EmpNo);

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_RNT_OPER_TYPE;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert rental Instrument subcontractor data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_RentalInstSubcontractor> InsertTbt_RentalInstSubContractor(tbt_RentalInstSubcontractor doInsert)
        {
            try
            {
                List<tbt_RentalInstSubcontractor> insertList
                    = base.InsertTbt_RentalInstSubContractor(
                        doInsert.ContractCode, doInsert.OCC, doInsert.SubcontractorCode
                        , CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                        , CommonUtil.dsTransData.dtUserData.EmpNo);

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_RNT_SUBCONT;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert rental maintenance detail data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_RentalMaintenanceDetails> InsertTbt_RentalMaintenanceDetails(tbt_RentalMaintenanceDetails doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RentalMaintenanceDetails> doInsertList = new List<tbt_RentalMaintenanceDetails>();
                doInsertList.Add(doInsert);
                List<tbt_RentalMaintenanceDetails> insertList = base.InsertTbt_RentalMaintenanceDetails(CommonUtil.ConvertToXml_Store<tbt_RentalMaintenanceDetails>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_RNT_MA;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert relation type data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_RelationType> InsertTbtRelationType(tbt_RelationType doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_RelationType> doInsertList = new List<tbt_RelationType>();
                doInsertList.Add(doInsert);
                List<tbt_RelationType> insertList = base.InsertTbt_RelationType(CommonUtil.ConvertToXml_Store<tbt_RelationType>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_RELATION_TYPE;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update data in case new/add sale and will insert new occurrence in case other installation type.
        /// </summary>
        /// <param name="doComplete"></param>
        public void CompleteInstallation(doCompleteInstallationData doComplete)
        {
            CommonUtil c = new CommonUtil();
            try
            {
                //Validation ContractCode, Installation Type, Installation Slip No.
                ApplicationErrorException.CheckMandatoryField<doCompleteInstallationData, doCompleteRentalCompleteInstallation>(doComplete);

                //2.set variable
                string strContractCode = doComplete.ContractCode;


                //4.Get contract data
                //4.1 Get last OCC and check contract code exists
                //4.1.1 Get last unimplemented OCC
                string strLastOCC = this.GetLastUnimplementedOCC(strContractCode);

                //4.1.2 If unimplement not exist, get last implemented OCC
                if (strLastOCC == null)
                    strLastOCC = this.GetLastImplementedOCC(strContractCode);

                //4.1.3 If strLastOCC is null
                if (strLastOCC == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));

                //4.2 Get entire contract data
                dsRentalContractData dsData = this.GetEntireContract(strContractCode, strLastOCC);
                if (dsData == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));

                #region //========== Set change type 6/7/2012 Teerapong S. ==============
                string strChangeType = null;
                if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL)
                {
                    strChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_INSTRU_DURING_STOP;
                }
                else if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL)
                {
                    strChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_ALL;
                }
                else if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE)
                {
                    strChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_MOVE_INSTRU;
                }
                else if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING)
                {
                    //strChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_MOVE_INSTRU;
                    strChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_WIRING; //Modify by Jutarat A. on 21052013
                }
                else if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE)
                {
                    strChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_EXCHANGE_INSTRU_AT_MA;
                }
                else if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE)
                {
                    strChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_EXCHANGE_INSTRU_AT_MA;
                }
                else if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW)
                {
                    if (dsData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                        strChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP;
                    else
                        strChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE;
                }
                else if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_NEW)
                {
                    strChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_APPROVE;
                }
                #endregion //==============================================================

                //5. Check contract status
                //tbt_RentalContractBasic contractData = dsData.dtTbt_RentalContractBasic[0];
                //tbt_RentalSecurityBasic securityData = dsData.dtTbt_RentalSecurityBasic[0];

                //5.1 In case control status is alternative start or before start service
                if (dsData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START ||
                    (dsData.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START &&
                        dsData.dtTbt_RentalContractBasic[0].StartType == StartType.C_START_TYPE_ALTER_START))
                {
                    //5.1.1 Check complete installation flag : In case of complete installation
                    if (dsData.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_OFF)
                    {
                        //5.1.1.1 Prepare object before save

                        #region prepareObjCompleteInstallation

                        //dsRentalContractData.dtTbt_RentalContractBasic
                        //contractData.ContractCode = strContractCode;
                        //if (contractData.FirstInstallCompleteFlag == FlagType.C_FLAG_OFF)
                        //{
                        //    contractData.FirstInstallCompleteFlag = FlagType.C_FLAG_ON;
                        //    contractData.NewInstallCompleteProcessDate = DateTime.Now;
                        //}
                        //contractData.LastNormalContractFee = doComplete.NormalInstallationFee;
                        //contractData.LastOrderContractFee = doComplete.BillingInstallationFee;

                        //dsData.dtTbt_RentalSecurityBasic
                        //tbt_RentalSecurityBasic rentalSecDo = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(rentalSecurityBasic);
                        //securityData.ContractCode = strContractCode;
                        //securityData.OCC = strLastOCC;
                        //securityData.NormalContractFee = doComplete.NormalInstallationFee;
                        //securityData.OrderContractFee = doComplete.BillingInstallationFee;
                        //securityData.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                        //securityData.InstallationSlipNo = doComplete.InstallationSlipNo;
                        //securityData.InstallationCompleteDate = DateTime.Now;
                        //securityData.InstallationTypeCode = doComplete.InstallationType;
                        //dsData.dtTbt_RentalSecurityBasic[0] = rentalSecDo;
                        #endregion
                        //========= TRS update 09/04/2012 ===============
                        #region prepareData1
                        //this.prepareObjCompleteInstallation(dsData, eCompleteInstallation.BEFORE_START_NOTCOMPLETE, strLastOCC, doComplete);
                        //--------- prepare contractBasic
                        if (dsData.dtTbt_RentalContractBasic != null && dsData.dtTbt_RentalContractBasic.Count > 0)
                        {
                            dsData.dtTbt_RentalContractBasic[0].ContractCode = strContractCode;
                            if (dsData.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == Convert.ToBoolean(CompleteInstallFlag.C_COMPLETE_INSTALL_FLAG_NOTCOMPLETE))
                            {
                                dsData.dtTbt_RentalContractBasic[0].NewInstallCompleteProcessDate = doComplete.InstallationCompleteProcessDate;
                            }
                            if (dsData.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == Convert.ToBoolean(CompleteInstallFlag.C_COMPLETE_INSTALL_FLAG_NOTCOMPLETE))
                            {
                                dsData.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag = Convert.ToBoolean(CompleteInstallFlag.C_COMPLETE_INSTALL_FLAG_COMPLETE);
                            }
                            dsData.dtTbt_RentalContractBasic[0].LastChangeImplementDate = doComplete.InstallationCompleteDate;

                            //--- Add additional deposit fee --
                            if (dsData.dtTbt_RentalSecurityBasic != null && dsData.dtTbt_RentalSecurityBasic.Count > 0 && dsData.dtTbt_RentalSecurityBasic[0].OCC != OCCType.C_FIRST_UNIMPLEMENTED_SECURITY_OCC)
                            {
                                if (dsData.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFee != null)
                                {
                                    dsData.dtTbt_RentalContractBasic[0].NormalDepositFee = dsData.dtTbt_RentalContractBasic[0].NormalDepositFee + dsData.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFee;
                                }
                                if (dsData.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee != null)
                                {
                                    dsData.dtTbt_RentalContractBasic[0].OrderDepositFee = dsData.dtTbt_RentalContractBasic[0].OrderDepositFee + dsData.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee;
                                }
                            }

                            //======== Update by Teerapong S. 6/7/2012 ============
                            if (!CommonUtil.IsNullOrEmpty(strChangeType))
                            {
                                dsData.dtTbt_RentalContractBasic[0].LastChangeType = strChangeType;
                            }
                            //=====================================================
                            //End Add
                        }
                        //----------- prepare Security Basic
                        if (dsData.dtTbt_RentalSecurityBasic != null && dsData.dtTbt_RentalSecurityBasic.Count > 0)
                        {
                            tbt_RentalSecurityBasic tempdoRentalSecurityBasic = new tbt_RentalSecurityBasic();
                            tempdoRentalSecurityBasic = dsData.dtTbt_RentalSecurityBasic[0];
                            tempdoRentalSecurityBasic.ContractCode = doComplete.ContractCode;
                            tempdoRentalSecurityBasic.OCC = strLastOCC;
                            //tempdoRentalSecurityBasic.NormalInstallFee = doComplete.NormalInstallationFee;
                            //if (!CommonUtil.IsNullOrEmpty(doComplete.BillingInstallationFee))
                            //{
                            //    tempdoRentalSecurityBasic.OrderInstallFee = doComplete.BillingInstallationFee;
                            //}
                            tempdoRentalSecurityBasic.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                            tempdoRentalSecurityBasic.InstallationSlipNo = doComplete.InstallationSlipNo;
                            tempdoRentalSecurityBasic.InstallationCompleteDate = doComplete.InstallationCompleteDate;
                            tempdoRentalSecurityBasic.InstallationTypeCode = doComplete.InstallationType;
                            tempdoRentalSecurityBasic.InstallationCompleteEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                            tempdoRentalSecurityBasic.ChangeImplementDate = doComplete.InstallationCompleteDate;

                            dsData.dtTbt_RentalSecurityBasic[0] = tempdoRentalSecurityBasic;

                            //======== Update by Teerapong S. 6/7/2012 ============
                            if (!CommonUtil.IsNullOrEmpty(strChangeType))
                            {
                                dsData.dtTbt_RentalSecurityBasic[0].ChangeType = strChangeType;
                            }
                            //=====================================================
                            //End Add
                        }
                        if (doComplete.doInstrumentDetailsList != null && doComplete.doInstrumentDetailsList.Count > 0)
                        {
                            int i = 0;
                            dsData.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                            tbt_RentalInstrumentDetails tempInstrument;
                            foreach (DataEntity.Contract.doInstrumentDetails dataInstrument in doComplete.doInstrumentDetailsList)
                            {
                                tempInstrument = new tbt_RentalInstrumentDetails();
                                tempInstrument.ContractCode = doComplete.ContractCode;
                                tempInstrument.OCC = strLastOCC;
                                tempInstrument.InstrumentCode = dataInstrument.InstrumentCode;
                                //tempInstrument.InstrumentTypeCode =	InstrumentType.C_INST_TYPE_GENERAL;
                                tempInstrument.InstrumentTypeCode = dataInstrument.InstrumentTypeCode;
                                if (tempInstrument.InstrumentTypeCode == InstrumentType.C_INST_TYPE_GENERAL)
                                {
                                    tempInstrument.InstrumentQty = dataInstrument.InstrumentQty;
                                    tempInstrument.AdditionalInstrumentQty = dataInstrument.AddQty;
                                    tempInstrument.RemovalInstrumentQty = dataInstrument.RemoveQty;
                                }
                                else
                                {
                                    tempInstrument.InstrumentQty = dataInstrument.InstrumentQty + dataInstrument.AddQty - dataInstrument.RemoveQty;
                                    tempInstrument.AdditionalInstrumentQty = 0;
                                    tempInstrument.RemovalInstrumentQty = 0;
                                }
                                dsData.dtTbt_RentalInstrumentDetails.Add(tempInstrument);
                            }
                        }
                        #endregion
                        //===============================================
                        using (TransactionScope scope = new TransactionScope())
                        {
                            //========= TRS update 09/04/2012 ===============
                            ////5.1.1.2 Update data in rental contract basic table
                            //this.UpdateTbt_RentalContractBasic(dsData.dtTbt_RentalContractBasic[0]);

                            ////5.1.1.3 Update data in rental security basic table
                            //this.UpdateTbt_RentalSecurityBasic(dsData.dtTbt_RentalSecurityBasic[0]);
                            this.UpdateEntireContract(dsData);
                            //===============================================
                            //5.1.1.4 Insert data to subcontractor table
                            #region InsertDataToSubcontract
                            //foreach (doSubcontractorDetails subCon in doComplete.doSubcontractorDetailsList)
                            //{
                            //    tbt_RentalInstSubcontractor tbt = new tbt_RentalInstSubcontractor();
                            //    tbt.ContractCode = strContractCode;
                            //    tbt.OCC = strLastOCC;
                            //    tbt.SubcontractorCode = subCon.SubcontractorCode;
                            //    this.InsertTbt_RentalInstSubContractor(tbt);
                            //}
                            #endregion
                            this.InsertDataToSubcontract(doComplete, strLastOCC);

                            //5.1.1.5 Call send data to billing module
                            IBillingInterfaceHandler hand = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                            hand.SendBilling_RentalCompleteInstall(strContractCode, doComplete.InstallationCompleteDate);


                            scope.Complete();
                        }
                    }
                    //5.1.2 Check complete installation falg : in case of not complete installation
                    else if (dsData.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_ON)
                    {
                        //5.1.2.1 Generate new occurence (unimplement)
                        string strNewOCC = this.GenerateContractOCC(strContractCode, FlagType.C_FLAG_OFF);

                        //5.1.2.2 Prepare object before save
                        #region prepareObjCompleteInstallation
                        //dsRentalContractData.dtTbt_RentalContractBasic
                        //contractData.ContractCode = strContractCode;
                        //contractData.LastOCC = strNewOCC;
                        //contractData.LastNormalContractFee = doComplete.NormalInstallationFee;
                        //contractData.LastOrderContractFee = doComplete.BillingInstallationFee;

                        //dsData.dtTbt_RentalSecurityBasic
                        //securityData.ContractCode = strContractCode;
                        //securityData.OCC = strNewOCC;
                        //securityData.NormalContractFee = doComplete.NormalInstallationFee;
                        //securityData.OrderContractFee = doComplete.BillingInstallationFee;
                        //securityData.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                        //securityData.InstallationSlipNo = doComplete.InstallationSlipNo;
                        //securityData.InstallationCompleteDate = DateTime.Now;
                        //securityData.InstallationTypeCode = doComplete.InstallationType;

                        //Todo: (Lieng) Replace OCC in all DO of entire contract with strNewOCC
                        #endregion
                        //========= TRS update 09/04/2012 ==================================
                        //this.prepareObjCompleteInstallation(dsData, eCompleteInstallation.BEFORE_START_COMPLETE, strNewOCC, doComplete);
                        #region prepareData2
                        //this.prepareObjCompleteInstallation(dsData, eCompleteInstallation.BEFORE_START_NOTCOMPLETE, strLastOCC, doComplete);
                        //--------- prepare contractBasic
                        if (dsData.dtTbt_RentalContractBasic != null && dsData.dtTbt_RentalContractBasic.Count > 0)
                        {
                            dsData.dtTbt_RentalContractBasic[0].ContractCode = strContractCode;
                            dsData.dtTbt_RentalContractBasic[0].LastOCC = strNewOCC;
                            dsData.dtTbt_RentalContractBasic[0].LastChangeImplementDate = doComplete.InstallationCompleteDate;

                            //======== Update by Teerapong S. 6/7/2012 ============
                            if (!CommonUtil.IsNullOrEmpty(strChangeType))
                            {
                                dsData.dtTbt_RentalContractBasic[0].LastChangeType = strChangeType;
                            }
                            //=====================================================
                            //End Add
                        }
                        //----------- prepare Security Basic
                        if (dsData.dtTbt_RentalSecurityBasic != null && dsData.dtTbt_RentalSecurityBasic.Count > 0)
                        {
                            tbt_RentalSecurityBasic tempdoRentalSecurityBasic = new tbt_RentalSecurityBasic();
                            tempdoRentalSecurityBasic = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(dsData.dtTbt_RentalSecurityBasic[0]);
                            tempdoRentalSecurityBasic.ContractCode = doComplete.ContractCode;
                            tempdoRentalSecurityBasic.OCC = strNewOCC;
                            tempdoRentalSecurityBasic.NormalInstallFee = doComplete.NormalInstallationFee;
                            if (!CommonUtil.IsNullOrEmpty(doComplete.BillingInstallationFee))
                            {
                                tempdoRentalSecurityBasic.OrderInstallFee = doComplete.BillingInstallationFee;
                                tempdoRentalSecurityBasic.OrderInstallFee_ApproveContract = null;
                                tempdoRentalSecurityBasic.OrderInstallFee_CompleteInstall = doComplete.BillingInstallationFee;
                                tempdoRentalSecurityBasic.OrderInstallFee_StartService = null;
                            }
                            else
                            {
                                tempdoRentalSecurityBasic.OrderInstallFee = null;
                                tempdoRentalSecurityBasic.OrderInstallFee_ApproveContract = null;
                                tempdoRentalSecurityBasic.OrderInstallFee_CompleteInstall = null;
                                tempdoRentalSecurityBasic.OrderInstallFee_StartService = null;
                            }
                            // 2012-10-04 Add by Phoomsak L. CT-266
                            tempdoRentalSecurityBasic.SalesmanEmpNo1 = null;
                            tempdoRentalSecurityBasic.SalesmanEmpNo2 = null;
                            tempdoRentalSecurityBasic.SalesSupporterEmpNo = null;
                            tempdoRentalSecurityBasic.PlanCode = null;
                            //------------------------------------------------
                            tempdoRentalSecurityBasic.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                            tempdoRentalSecurityBasic.InstallationSlipNo = doComplete.InstallationSlipNo;
                            tempdoRentalSecurityBasic.InstallationCompleteDate = doComplete.InstallationCompleteDate;
                            tempdoRentalSecurityBasic.InstallationTypeCode = doComplete.InstallationType;
                            tempdoRentalSecurityBasic.InstallationCompleteEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                            tempdoRentalSecurityBasic.ChangeImplementDate = doComplete.InstallationCompleteDate;
                            tempdoRentalSecurityBasic.QuotationTargetCode = null;
                            tempdoRentalSecurityBasic.QuotationAlphabet = null;
                            // 2012-10-04 Add by Phoomsak L. CT-266
                            tempdoRentalSecurityBasic.NormalAdditionalDepositFee = null;
                            tempdoRentalSecurityBasic.OrderAdditionalDepositFee = null;
                            //---------------------------------------------------------
                            tempdoRentalSecurityBasic.DepositFeeBillingTiming = null;
                            tempdoRentalSecurityBasic.CounterNo = 0;
                            // 2012-10-04 Add by Phoomsak L. CT-266 
                            //tempdoRentalSecurityBasic.AdditionalFee1 = 0;
                            //tempdoRentalSecurityBasic.AdditionalFee2 = 0;
                            //tempdoRentalSecurityBasic.AdditionalFee3 = 0;
                            //tempdoRentalSecurityBasic.AdditionalApproveNo1 = null;
                            //tempdoRentalSecurityBasic.AdditionalApproveNo2 = null;
                            //tempdoRentalSecurityBasic.AdditionalApproveNo3 = null;
                            //tempdoRentalSecurityBasic.MaintenanceFee1 = 0;
                            //tempdoRentalSecurityBasic.MaintenanceFee2 = 0;
                            tempdoRentalSecurityBasic.ApproveNo1 = null;
                            tempdoRentalSecurityBasic.ApproveNo2 = null;
                            tempdoRentalSecurityBasic.ApproveNo3 = null;
                            tempdoRentalSecurityBasic.ApproveNo4 = null;
                            tempdoRentalSecurityBasic.ApproveNo5 = null;

                            dsData.dtTbt_RentalSecurityBasic[0] = tempdoRentalSecurityBasic;

                            //======== Update by Teerapong S. 6/7/2012 ============
                            if (!CommonUtil.IsNullOrEmpty(strChangeType))
                            {
                                dsData.dtTbt_RentalSecurityBasic[0].ChangeType = strChangeType;
                            }
                            dsData.dtTbt_RentalSecurityBasic[0].QuotationTargetCode = null;
                            dsData.dtTbt_RentalSecurityBasic[0].QuotationAlphabet = null;
                            //=====================================================
                            //End Add
                        }
                        if (doComplete.doInstrumentDetailsList != null && doComplete.doInstrumentDetailsList.Count > 0)
                        {

                            dsData.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                            tbt_RentalInstrumentDetails tempInstrument;
                            foreach (DataEntity.Contract.doInstrumentDetails dataInstrument in doComplete.doInstrumentDetailsList)
                            {
                                tempInstrument = new tbt_RentalInstrumentDetails();
                                tempInstrument.ContractCode = doComplete.ContractCode;
                                tempInstrument.OCC = strNewOCC;
                                tempInstrument.InstrumentCode = dataInstrument.InstrumentCode;
                                //tempInstrument.InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;
                                tempInstrument.InstrumentTypeCode = dataInstrument.InstrumentTypeCode;
                                if (tempInstrument.InstrumentTypeCode == InstrumentType.C_INST_TYPE_GENERAL)
                                {
                                    tempInstrument.InstrumentQty = dataInstrument.InstrumentQty;
                                    tempInstrument.AdditionalInstrumentQty = dataInstrument.AddQty;
                                    tempInstrument.RemovalInstrumentQty = dataInstrument.RemoveQty;
                                }
                                else
                                {
                                    tempInstrument.InstrumentQty = dataInstrument.InstrumentQty + dataInstrument.AddQty - dataInstrument.RemoveQty;
                                    tempInstrument.AdditionalInstrumentQty = 0;
                                    tempInstrument.RemovalInstrumentQty = 0;
                                }
                                dsData.dtTbt_RentalInstrumentDetails.Add(tempInstrument);
                            }
                        }
                        //----------- prepare RentalMaintenanceDetails
                        if (dsData.dtTbt_RentalMaintenanceDetails != null && dsData.dtTbt_RentalMaintenanceDetails.Count > 0)
                        {
                            tbt_RentalMaintenanceDetails tempdoRentalMaintenanceDetails = new tbt_RentalMaintenanceDetails();
                            tempdoRentalMaintenanceDetails = CommonUtil.CloneObject<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(dsData.dtTbt_RentalMaintenanceDetails[0]);
                            tempdoRentalMaintenanceDetails.OCC = strNewOCC;
                            dsData.dtTbt_RentalMaintenanceDetails[0] = tempdoRentalMaintenanceDetails;
                        }
                        //----------- prepare RentalOperationType
                        if (dsData.dtTbt_RentalOperationType != null && dsData.dtTbt_RentalOperationType.Count > 0)
                        {
                            tbt_RentalOperationType tempdoRentalOperationType = new tbt_RentalOperationType();
                            tempdoRentalOperationType = CommonUtil.CloneObject<tbt_RentalOperationType, tbt_RentalOperationType>(dsData.dtTbt_RentalOperationType[0]);
                            tempdoRentalOperationType.OCC = strNewOCC;
                            dsData.dtTbt_RentalOperationType[0] = tempdoRentalOperationType;
                        }
                        //----------- prepare RelationType
                        if (dsData.dtTbt_RelationType != null && dsData.dtTbt_RelationType.Count > 0)
                        {
                            tbt_RelationType tempdoRelationType = new tbt_RelationType();
                            tempdoRelationType = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(dsData.dtTbt_RelationType[0]);
                            tempdoRelationType.OCC = strNewOCC;
                            dsData.dtTbt_RelationType[0] = tempdoRelationType;
                        }
                        //-------------- prepare Tbt_CancelContractMemoDetail
                        dsData.dtTbt_CancelContractMemoDetail = null;
                        #endregion
                        //=============================================================
                        using (TransactionScope scope = new TransactionScope())
                        {
                            string strLastUnimplementOCC = GetLastUnimplementedOCC(strContractCode);

                            //5.1.2.3 Insert entire contract data
                            //this.InsertEntireContract(dsData);
                            this.InsertEntireContractForCTS010(dsData); //Modify by Jutarat A. on 19092013

                            //===================== Teerapong 11/10/2012 ===========================
                            //===== 5.1.2.4.In sert data to Billing temp with new OCC =======                           

                            IBillingTempHandler BillingTempHand = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                            List<tbt_BillingTemp> doTbt_BillingTemp = BillingTempHand.GetTbt_BillingTemp(strContractCode, strLastUnimplementOCC);

                            //================= fixed locked primary key ===================
                            List<tbt_BillingTemp> doTbt_BillingTempFiltered = new List<tbt_BillingTemp>();
                            foreach (var dataTemp in doTbt_BillingTemp)
                            {
                                tbt_BillingTemp tempdoTbt_BillingTemp = new tbt_BillingTemp();
                                tempdoTbt_BillingTemp = CommonUtil.CloneObject<tbt_BillingTemp, tbt_BillingTemp>(dataTemp);
                                doTbt_BillingTempFiltered.Add(tempdoTbt_BillingTemp);
                            }
                            //==============================================================

                            doTbt_BillingTempFiltered = (from a in doTbt_BillingTempFiltered where a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE select a).ToList<tbt_BillingTemp>();

                            foreach (var dataTbt_BillingTemp in doTbt_BillingTempFiltered)
                            {
                                dataTbt_BillingTemp.OCC = strNewOCC;
                            }

                            if (!CommonUtil.IsNullOrEmpty(doTbt_BillingTempFiltered))
                            {
                                foreach (var dataTbt_BillingTemp2 in doTbt_BillingTempFiltered)
                                {
                                    List<tbt_BillingTemp> doInsertedTbt_BillingTemp = BillingTempHand.InsertBillingTemp(dataTbt_BillingTemp2);
                                }
                            }
                            //===================================================================
                            //======================================================================

                            //5.1.2.4 Insert data to subcontractor table
                            #region InsertDataToSubcontract
                            //foreach (doSubcontractorDetails subCon in doComplete.doSubcontractorDetailsList)
                            //{
                            //    tbt_RentalInstSubcontractor tbt = new tbt_RentalInstSubcontractor();
                            //    tbt.ContractCode = strContractCode;
                            //    tbt.OCC = strNewOCC;
                            //    tbt.SubcontractorCode = subCon.SubcontractorCode;
                            //    this.InsertTbt_RentalInstSubContractor(tbt);
                            //}
                            #endregion
                            this.InsertDataToSubcontract(doComplete, strNewOCC);

                            //5.1.2.5 Teerapong S. 21/08/2012 5.1.2.5.	C heck need to send billing or not
                            if (!CommonUtil.IsNullOrEmpty(doComplete.BillingOCC) && doComplete.BillingInstallationFee > 0)
                            {
                                doBillingTempDetail doBillingTempDetail = new doBillingTempDetail();
                                doBillingTempDetail.ContractCode = doComplete.ContractCode;
                                doBillingTempDetail.BillingOCC = doComplete.BillingOCC;
                                if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL ||
                                    doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL)
                                {
                                    doBillingTempDetail.ContractBillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE;
                                }
                                else
                                {
                                    doBillingTempDetail.ContractBillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE;
                                }
                                doBillingTempDetail.BillingDate = doComplete.InstallationCompleteDate;
                                doBillingTempDetail.BillingAmount = doComplete.BillingInstallationFee;

                                IBillingInterfaceHandler hand = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                                bool blnProcessResult = hand.SendBilling_OnetimeFee(doBillingTempDetail);
                            }
                            //============================================================================

                            scope.Complete();
                        }
                    }
                }
                //in case contract status is after start service / cancel contract
                else
                {
                    //5.2.1 Generate new occurrence (implement)
                    string strNewOCC = this.GenerateContractOCC(strContractCode, FlagType.C_FLAG_ON);

                    //5.2.2 Get last OCC of unimplement data for using in next step
                    string strUnimplementOCC = this.GetLastUnimplementedOCC(strContractCode);

                    //5.2.3 Check exists unimplement data : In case of exist
                    if (strUnimplementOCC != null)
                    {

                        //5.2.3.1 Get unimplement data
                        dsRentalContractData dsUnimpData = this.GetEntireContract(strContractCode, strUnimplementOCC);
                        if (dsUnimpData == null)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0011, c.ConvertContractCode(strContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT));

                        //tbt_RentalContractBasic contractUnimpData = dsUnimpData.dtTbt_RentalContractBasic[0];
                        //tbt_RentalSecurityBasic securityUnimpData = dsUnimpData.dtTbt_RentalSecurityBasic[0];

                        //5.2.3.2 Prepare object before save
                        //=============== TRS update 09/04/2012 ===============================
                        #region prepareObjCompleteInstallation Unimplement
                        //dsUnimpData.dtTbt_RentalContractBasic
                        //contractUnimpData.ContractCode = strContractCode;
                        //contractUnimpData.LastOCC = strNewOCC;
                        //contractUnimpData.LastNormalContractFee = doComplete.NormalInstallationFee;
                        //contractUnimpData.LastOrderContractFee = doComplete.BillingInstallationFee;

                        ////dsUnimpData.dtTbt_RentalSecurityBasic
                        //securityUnimpData.ContractCode = strContractCode;
                        //securityUnimpData.OCC = strNewOCC;
                        //securityUnimpData.ImplementFlag = FlagType.C_FLAG_ON;
                        //securityUnimpData.NormalContractFee = doComplete.NormalInstallationFee;
                        //securityUnimpData.OrderContractFee = doComplete.BillingInstallationFee;
                        //securityUnimpData.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                        //securityUnimpData.InstallationSlipNo = doComplete.InstallationSlipNo;
                        //securityUnimpData.InstallationCompleteDate = DateTime.Now;
                        //securityUnimpData.InstallationTypeCode = doComplete.InstallationType;

                        ////dsUnimpData.dtTbt_RentalInstrumentDetails
                        //dsUnimpData.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                        //foreach (doInstrumentDetails instDetail in doComplete.doInstrumentDetailsList)
                        //{
                        //    tbt_RentalInstrumentDetails rentalDetailDo = new tbt_RentalInstrumentDetails();
                        //    rentalDetailDo.ContractCode = doComplete.ContractCode;
                        //    rentalDetailDo.OCC = strNewOCC;
                        //    rentalDetailDo.InstrumentCode = instDetail.InstrumentCode;
                        //    rentalDetailDo.InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;
                        //    rentalDetailDo.InstrumentQty = instDetail.AddQty - instDetail.RemoveQty;
                        //    rentalDetailDo.AdditionalInstrumentQty = 0;
                        //    rentalDetailDo.RemovalInstrumentQty = 0;
                        //    dsUnimpData.dtTbt_RentalInstrumentDetails.Add(rentalDetailDo);
                        //}

                        ////Todo: (Lieng) Replace OCC in all DO of entire contract with strNewOCC
                        #endregion
                        //this.prepareObjCompleteInstallation(dsUnimpData, eCompleteInstallation.AFTER_START_UNIMP_EXIST, strNewOCC, doComplete);
                        #region preparedata3
                        //--------- prepare contractBasic
                        if (dsUnimpData.dtTbt_RentalContractBasic != null && dsUnimpData.dtTbt_RentalContractBasic.Count > 0)
                        {
                            dsUnimpData.dtTbt_RentalContractBasic[0].ContractCode = strContractCode;
                            dsUnimpData.dtTbt_RentalContractBasic[0].LastOCC = strNewOCC;
                            dsUnimpData.dtTbt_RentalContractBasic[0].LastChangeImplementDate = doComplete.InstallationCompleteDate;

                            //======== Update by Teerapong S. 6/7/2012 ============
                            if (!CommonUtil.IsNullOrEmpty(strChangeType))
                            {
                                dsUnimpData.dtTbt_RentalContractBasic[0].LastChangeType = strChangeType;
                            }
                            //=====================================================
                            //End Add
                        }
                        //----------- prepare Security Basic
                        if (dsUnimpData.dtTbt_RentalSecurityBasic != null && dsUnimpData.dtTbt_RentalSecurityBasic.Count > 0)
                        {
                            tbt_RentalSecurityBasic tempdoRentalSecurityBasic = new tbt_RentalSecurityBasic();
                            tempdoRentalSecurityBasic = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(dsUnimpData.dtTbt_RentalSecurityBasic[0]);

                            tempdoRentalSecurityBasic.ContractCode = doComplete.ContractCode;
                            tempdoRentalSecurityBasic.OCC = strNewOCC;
                            tempdoRentalSecurityBasic.ImplementFlag = FlagType.C_FLAG_ON;
                            //tempdoRentalSecurityBasic.NormalInstallFee = doComplete.NormalInstallationFee;
                            //if (!CommonUtil.IsNullOrEmpty(doComplete.BillingInstallationFee))
                            //{
                            //    tempdoRentalSecurityBasic.OrderInstallFee = doComplete.BillingInstallationFee;
                            //}
                            tempdoRentalSecurityBasic.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                            tempdoRentalSecurityBasic.InstallationSlipNo = doComplete.InstallationSlipNo;
                            tempdoRentalSecurityBasic.InstallationCompleteDate = doComplete.InstallationCompleteDate;
                            tempdoRentalSecurityBasic.InstallationTypeCode = doComplete.InstallationType;
                            tempdoRentalSecurityBasic.InstallationCompleteEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                            tempdoRentalSecurityBasic.ChangeImplementDate = doComplete.InstallationCompleteDate;

                            // case after start service and MK-29 is registered
                            if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
                               || doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                               || doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                               || doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                               || doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                               || doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                               || doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE)
                            {
                                tempdoRentalSecurityBasic.CompleteChangeOperationDate = doComplete.InstallationCompleteDate;
                                tempdoRentalSecurityBasic.CompleteChangeOperationEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                            }
                            else
                            {
                                tempdoRentalSecurityBasic.CompleteChangeOperationDate = null;
                                tempdoRentalSecurityBasic.CompleteChangeOperationEmpNo = null;
                            }


                            dsUnimpData.dtTbt_RentalSecurityBasic[0] = tempdoRentalSecurityBasic;

                            //======== Update by Teerapong S. 6/7/2012 ============
                            if (!CommonUtil.IsNullOrEmpty(strChangeType))
                            {
                                dsUnimpData.dtTbt_RentalSecurityBasic[0].ChangeType = strChangeType;
                            }
                            //=====================================================
                            //End Add
                        }
                        if (doComplete.doInstrumentDetailsList != null && doComplete.doInstrumentDetailsList.Count > 0)
                        {
                            int i = 0;
                            dsUnimpData.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                            tbt_RentalInstrumentDetails tempInstrument;
                            foreach (DataEntity.Contract.doInstrumentDetails dataInstrument in doComplete.doInstrumentDetailsList)
                            {
                                tempInstrument = new tbt_RentalInstrumentDetails();
                                tempInstrument.ContractCode = doComplete.ContractCode;
                                tempInstrument.OCC = strNewOCC;
                                tempInstrument.InstrumentCode = dataInstrument.InstrumentCode;
                                //tempInstrument.InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;
                                tempInstrument.InstrumentTypeCode = dataInstrument.InstrumentTypeCode;
                                if (tempInstrument.InstrumentTypeCode == InstrumentType.C_INST_TYPE_GENERAL)
                                {
                                    tempInstrument.InstrumentQty = dataInstrument.InstrumentQty;
                                    tempInstrument.AdditionalInstrumentQty = dataInstrument.AddQty;
                                    tempInstrument.RemovalInstrumentQty = dataInstrument.RemoveQty;
                                }
                                else
                                {
                                    tempInstrument.InstrumentQty = dataInstrument.InstrumentQty + dataInstrument.AddQty - dataInstrument.RemoveQty;
                                    tempInstrument.AdditionalInstrumentQty = 0;
                                    tempInstrument.RemovalInstrumentQty = 0;
                                }
                                dsUnimpData.dtTbt_RentalInstrumentDetails.Add(tempInstrument);
                            }
                        }
                        //----------- prepare RentalMaintenanceDetails
                        if (dsUnimpData.dtTbt_RentalMaintenanceDetails != null && dsUnimpData.dtTbt_RentalMaintenanceDetails.Count > 0)
                        {
                            tbt_RentalMaintenanceDetails tempdoRentalMaintenanceDetails = new tbt_RentalMaintenanceDetails();
                            tempdoRentalMaintenanceDetails = CommonUtil.CloneObject<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(dsUnimpData.dtTbt_RentalMaintenanceDetails[0]);
                            tempdoRentalMaintenanceDetails.OCC = strNewOCC;
                            dsUnimpData.dtTbt_RentalMaintenanceDetails[0] = tempdoRentalMaintenanceDetails;
                        }
                        //----------- prepare RentalOperationType
                        if (dsUnimpData.dtTbt_RentalOperationType != null && dsUnimpData.dtTbt_RentalOperationType.Count > 0)
                        {
                            tbt_RentalOperationType tempdoRentalOperationType = new tbt_RentalOperationType();
                            tempdoRentalOperationType = CommonUtil.CloneObject<tbt_RentalOperationType, tbt_RentalOperationType>(dsUnimpData.dtTbt_RentalOperationType[0]);
                            tempdoRentalOperationType.OCC = strNewOCC;
                            dsUnimpData.dtTbt_RentalOperationType[0] = tempdoRentalOperationType;
                        }
                        //----------- prepare RelationType
                        if (dsUnimpData.dtTbt_RelationType != null && dsUnimpData.dtTbt_RelationType.Count > 0)
                        {
                            tbt_RelationType tempdoRelationType = new tbt_RelationType();
                            tempdoRelationType = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(dsUnimpData.dtTbt_RelationType[0]);
                            tempdoRelationType.OCC = strNewOCC;
                            dsUnimpData.dtTbt_RelationType[0] = tempdoRelationType;
                        }
                        //-------------- prepare Tbt_CancelContractMemoDetail
                        dsData.dtTbt_CancelContractMemoDetail = null;
                        #endregion
                        //=====================================================================
                        using (TransactionScope scope = new TransactionScope())
                        {
                            //Update summary field 
                            UpdateSummaryFields(ref dsUnimpData);

                            //5.2.3.3 Insert entire contract data
                            dsRentalContractData dsResult = this.InsertEntireContract(dsUnimpData);

                            //5.2.3.4 Insert data to subcontractor table
                            #region InsertDataToSubcontract
                            //foreach (doSubcontractorDetails subCon in doComplete.doSubcontractorDetailsList)
                            //{
                            //    tbt_RentalInstSubcontractor tbt = new tbt_RentalInstSubcontractor();
                            //    tbt.ContractCode = strContractCode;
                            //    tbt.OCC = strNewOCC;
                            //    tbt.SubcontractorCode = subCon.SubcontractorCode;
                            //    this.InsertTbt_RentalInstSubContractor(tbt);
                            //}
                            #endregion
                            this.InsertDataToSubcontract(doComplete, strNewOCC);

                            //5.2.3.5 Delete Unimplement data
                            this.DeleteEntireOCC(strContractCode, strUnimplementOCC, dsResult.dtTbt_RentalContractBasic[0].UpdateDate.Value);

                            //5.2.3.6 Call send data to billing module
                            IBillingInterfaceHandler hand = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                            hand.SendBilling_RentalCompleteInstall(strContractCode, doComplete.InstallationCompleteDate);

                            //5.2.3.7 Delete all billing temp
                            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                            List<tbt_BillingTemp> dtDeletedTbt_BillingTemp = target.DeleteBillingTempByContractCode(strContractCode);
                            scope.Complete();
                        }
                    }
                    //5.2.4 Check exists unimplement data : In case of NOT exist
                    else
                    {
                        //5.2.4.1 Prepare object before save
                        #region prepareObjCompleteInstallation
                        ////dsData.dtTbt_RentalContractBasic
                        //contractData.ContractCode = strContractCode;
                        //contractData.LastOCC = strNewOCC;
                        //contractData.LastNormalContractFee = doComplete.NormalInstallationFee;
                        //contractData.LastOrderContractFee = doComplete.BillingInstallationFee;

                        ////dsData.dtTbt_RentalSecurityBasic
                        //securityData.ContractCode = strContractCode;
                        //securityData.OCC = strNewOCC;
                        //securityData.ImplementFlag = FlagType.C_FLAG_ON;
                        //securityData.NormalContractFee = doComplete.NormalInstallationFee;
                        //securityData.OrderContractFee = doComplete.BillingInstallationFee;
                        //securityData.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                        //securityData.InstallationSlipNo = doComplete.InstallationSlipNo;
                        //securityData.InstallationCompleteDate = DateTime.Now;
                        //securityData.InstallationTypeCode = doComplete.InstallationType;

                        ////dsData.dtTbt_RentalInstrumentDetails
                        //dsData.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                        //foreach (doInstrumentDetails instDetail in doComplete.doInstrumentDetailsList)
                        //{
                        //    tbt_RentalInstrumentDetails rentalDetailDo = new tbt_RentalInstrumentDetails();
                        //    rentalDetailDo.ContractCode = doComplete.ContractCode;
                        //    rentalDetailDo.OCC = strNewOCC;
                        //    rentalDetailDo.InstrumentCode = instDetail.InstrumentCode;
                        //    rentalDetailDo.InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;
                        //    rentalDetailDo.InstrumentQty = instDetail.AddQty - instDetail.RemoveQty;
                        //    rentalDetailDo.AdditionalInstrumentQty = 0;
                        //    rentalDetailDo.RemovalInstrumentQty = 0;
                        //    dsData.dtTbt_RentalInstrumentDetails.Add(rentalDetailDo);
                        //}


                        ////Todo: (Lieng) Replace OCC in all DO of entire contract with strNewOCC
                        #endregion
                        //================== TRS update 09/04/2012 ======================
                        //this.prepareObjCompleteInstallation(dsData, eCompleteInstallation.AFTER_START_UNIMP_NOTEXIST, strNewOCC, doComplete);
                        #region preparedata4
                        //--------- prepare contractBasic
                        if (dsData.dtTbt_RentalContractBasic != null && dsData.dtTbt_RentalContractBasic.Count > 0)
                        {
                            dsData.dtTbt_RentalContractBasic[0].ContractCode = strContractCode;
                            dsData.dtTbt_RentalContractBasic[0].LastOCC = strNewOCC;
                            dsData.dtTbt_RentalContractBasic[0].LastChangeImplementDate = doComplete.InstallationCompleteDate;

                            //--- Add additional deposit fee --
                            if (dsData.dtTbt_RentalSecurityBasic != null && dsData.dtTbt_RentalSecurityBasic.Count > 0)
                            {
                                if (dsData.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFee != null)
                                {
                                    dsData.dtTbt_RentalContractBasic[0].NormalDepositFee = dsData.dtTbt_RentalContractBasic[0].NormalDepositFee + dsData.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFee;
                                }
                                if (dsData.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee != null)
                                {
                                    dsData.dtTbt_RentalContractBasic[0].OrderDepositFee = dsData.dtTbt_RentalContractBasic[0].OrderDepositFee + dsData.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee;
                                }
                            }
                            //======== Update by Teerapong S. 6/7/2012 ============
                            if (!CommonUtil.IsNullOrEmpty(strChangeType))
                            {
                                dsData.dtTbt_RentalContractBasic[0].LastChangeType = strChangeType;
                            }
                            //=====================================================
                            //End Add
                        }
                        //----------- prepare Security Basic
                        if (dsData.dtTbt_RentalSecurityBasic != null && dsData.dtTbt_RentalSecurityBasic.Count > 0)
                        {
                            tbt_RentalSecurityBasic tempdoRentalSecurityBasic = new tbt_RentalSecurityBasic();
                            tempdoRentalSecurityBasic = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(dsData.dtTbt_RentalSecurityBasic[0]);

                            tempdoRentalSecurityBasic.ContractCode = doComplete.ContractCode;
                            tempdoRentalSecurityBasic.OCC = strNewOCC;
                            tempdoRentalSecurityBasic.ImplementFlag = FlagType.C_FLAG_ON;
                            tempdoRentalSecurityBasic.NormalInstallFee = doComplete.NormalInstallationFee;
                            if (!CommonUtil.IsNullOrEmpty(doComplete.BillingInstallationFee))
                            {
                                tempdoRentalSecurityBasic.OrderInstallFee = doComplete.BillingInstallationFee;
                                tempdoRentalSecurityBasic.OrderInstallFee_ApproveContract = null;
                                tempdoRentalSecurityBasic.OrderInstallFee_CompleteInstall = doComplete.BillingInstallationFee;
                                tempdoRentalSecurityBasic.OrderInstallFee_StartService = null;
                            }
                            else
                            {
                                tempdoRentalSecurityBasic.OrderInstallFee = null;
                                tempdoRentalSecurityBasic.OrderInstallFee_ApproveContract = null;
                                tempdoRentalSecurityBasic.OrderInstallFee_CompleteInstall = null;
                                tempdoRentalSecurityBasic.OrderInstallFee_StartService = null;
                            }
                            // 2012-10-04 Add by Phoomsak L. CT-266
                            tempdoRentalSecurityBasic.SalesmanEmpNo1 = null;
                            tempdoRentalSecurityBasic.SalesmanEmpNo2 = null;
                            tempdoRentalSecurityBasic.SalesSupporterEmpNo = null;
                            tempdoRentalSecurityBasic.PlanCode = null;
                            // ----------------------------------------------------------
                            tempdoRentalSecurityBasic.InstallationCompleteFlag = FlagType.C_FLAG_ON;
                            tempdoRentalSecurityBasic.InstallationSlipNo = doComplete.InstallationSlipNo;
                            tempdoRentalSecurityBasic.InstallationCompleteDate = doComplete.InstallationCompleteDate;
                            tempdoRentalSecurityBasic.InstallationTypeCode = doComplete.InstallationType;
                            tempdoRentalSecurityBasic.InstallationCompleteEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                            tempdoRentalSecurityBasic.ChangeImplementDate = doComplete.InstallationCompleteDate; //Add by Jutarat A. on 23122013

                            tempdoRentalSecurityBasic.QuotationTargetCode = null;
                            tempdoRentalSecurityBasic.QuotationAlphabet = null;
                            // 2012-10-04 Add by Phoomsak L. CT-266
                            tempdoRentalSecurityBasic.NormalAdditionalDepositFee = null;
                            tempdoRentalSecurityBasic.OrderAdditionalDepositFee = null;
                            //---------------------------------------------------------
                            tempdoRentalSecurityBasic.DepositFeeBillingTiming = null;
                            tempdoRentalSecurityBasic.CounterNo = 0;
                            // 2012-10-04 Add by Phoomsak L. CT-266
                            //tempdoRentalSecurityBasic.AdditionalFee1 = 0;
                            //tempdoRentalSecurityBasic.AdditionalFee2 = 0;
                            //tempdoRentalSecurityBasic.AdditionalFee3 = 0;
                            //tempdoRentalSecurityBasic.AdditionalApproveNo1 = null;
                            //tempdoRentalSecurityBasic.AdditionalApproveNo2 = null;
                            //tempdoRentalSecurityBasic.AdditionalApproveNo3 = null;
                            //tempdoRentalSecurityBasic.MaintenanceFee1 = 0;
                            //tempdoRentalSecurityBasic.MaintenanceFee2 = 0;
                            tempdoRentalSecurityBasic.ApproveNo1 = null;
                            tempdoRentalSecurityBasic.ApproveNo2 = null;
                            tempdoRentalSecurityBasic.ApproveNo3 = null;
                            tempdoRentalSecurityBasic.ApproveNo4 = null;
                            tempdoRentalSecurityBasic.ApproveNo5 = null;

                            // case after start service and MK-29 is registered
                            if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGE_WIRING
                               || doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_CHANGEPLAN_AFTER_NEW
                               || doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_PERIODICAL_MAINTENANCE
                               || doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MAINTENANCE_EXCHANGE
                               || doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL
                               || doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_PARTIAL
                               || doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_MOVE)
                            {
                                tempdoRentalSecurityBasic.CompleteChangeOperationDate = doComplete.InstallationCompleteDate;
                                tempdoRentalSecurityBasic.CompleteChangeOperationEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;
                            }
                            else
                            {
                                tempdoRentalSecurityBasic.CompleteChangeOperationDate = null;
                                tempdoRentalSecurityBasic.CompleteChangeOperationEmpNo = null;
                            }

                            dsData.dtTbt_RentalSecurityBasic[0] = tempdoRentalSecurityBasic;

                            ////Add by jutarat A. on 20120622
                            //if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL)
                            //{
                            //    dsData.dtTbt_RentalSecurityBasic[0].ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_INSTRU_DURING_STOP;
                            //}
                            //else if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL)
                            //{
                            //    dsData.dtTbt_RentalSecurityBasic[0].ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_REMOVE_ALL;
                            //}
                            //======== Update by Teerapong S. 6/7/2012 ============
                            if (!CommonUtil.IsNullOrEmpty(strChangeType))
                            {
                                dsData.dtTbt_RentalSecurityBasic[0].ChangeType = strChangeType;
                            }
                            dsData.dtTbt_RentalSecurityBasic[0].QuotationTargetCode = null;
                            dsData.dtTbt_RentalSecurityBasic[0].QuotationAlphabet = null;
                            //=====================================================
                            //End Add
                        }
                        if (doComplete.doInstrumentDetailsList != null && doComplete.doInstrumentDetailsList.Count > 0)
                        {
                            int i = 0;
                            dsData.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                            tbt_RentalInstrumentDetails tempInstrument;
                            foreach (DataEntity.Contract.doInstrumentDetails dataInstrument in doComplete.doInstrumentDetailsList)
                            {
                                tempInstrument = new tbt_RentalInstrumentDetails();
                                tempInstrument.ContractCode = doComplete.ContractCode;
                                tempInstrument.OCC = strNewOCC;
                                tempInstrument.InstrumentCode = dataInstrument.InstrumentCode;
                                //tempInstrument.InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;
                                tempInstrument.InstrumentTypeCode = dataInstrument.InstrumentTypeCode;
                                if (tempInstrument.InstrumentTypeCode == InstrumentType.C_INST_TYPE_GENERAL)
                                {
                                    tempInstrument.InstrumentQty = dataInstrument.InstrumentQty;
                                    tempInstrument.AdditionalInstrumentQty = dataInstrument.AddQty;
                                    tempInstrument.RemovalInstrumentQty = dataInstrument.RemoveQty;
                                }
                                else
                                {
                                    tempInstrument.InstrumentQty = dataInstrument.InstrumentQty + dataInstrument.AddQty - dataInstrument.RemoveQty;
                                    tempInstrument.AdditionalInstrumentQty = 0;
                                    tempInstrument.RemovalInstrumentQty = 0;
                                }
                                dsData.dtTbt_RentalInstrumentDetails.Add(tempInstrument);
                            }
                        }
                        //----------- prepare RentalMaintenanceDetails
                        if (dsData.dtTbt_RentalMaintenanceDetails != null && dsData.dtTbt_RentalMaintenanceDetails.Count > 0)
                        {
                            tbt_RentalMaintenanceDetails tempdoRentalMaintenanceDetails = new tbt_RentalMaintenanceDetails();
                            tempdoRentalMaintenanceDetails = CommonUtil.CloneObject<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(dsData.dtTbt_RentalMaintenanceDetails[0]);
                            tempdoRentalMaintenanceDetails.OCC = strNewOCC;
                            dsData.dtTbt_RentalMaintenanceDetails[0] = tempdoRentalMaintenanceDetails;
                        }
                        //----------- prepare RentalOperationType
                        if (dsData.dtTbt_RentalOperationType != null && dsData.dtTbt_RentalOperationType.Count > 0)
                        {
                            tbt_RentalOperationType tempdoRentalOperationType = new tbt_RentalOperationType();
                            tempdoRentalOperationType = CommonUtil.CloneObject<tbt_RentalOperationType, tbt_RentalOperationType>(dsData.dtTbt_RentalOperationType[0]);
                            tempdoRentalOperationType.OCC = strNewOCC;
                            dsData.dtTbt_RentalOperationType[0] = tempdoRentalOperationType;
                        }
                        //----------- prepare RelationType
                        if (dsData.dtTbt_RelationType != null && dsData.dtTbt_RelationType.Count > 0)
                        {
                            tbt_RelationType tempdoRelationType = new tbt_RelationType();
                            tempdoRelationType = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(dsData.dtTbt_RelationType[0]);
                            tempdoRelationType.OCC = strNewOCC;
                            dsData.dtTbt_RelationType[0] = tempdoRelationType;
                        }
                        //-------------- prepare Tbt_CancelContractMemoDetail
                        dsData.dtTbt_CancelContractMemoDetail = null;
                        #endregion
                        //===============================================================
                        using (TransactionScope scope = new TransactionScope())
                        {
                            //Update summary field 
                            UpdateSummaryFields(ref dsData);

                            //5.2.4.2 Insert entire contract data
                            //this.InsertEntireContract(dsData);
                            this.InsertEntireContractForCTS010(dsData); //Modify by Jutarat A. on 19092013

                            //5.2.4.3 Insert data to subcontractor table
                            #region InsertDataToSubcontract
                            //foreach (doSubcontractorDetails subCon in doComplete.doSubcontractorDetailsList)
                            //{
                            //    tbt_RentalInstSubcontractor tbt = new tbt_RentalInstSubcontractor();
                            //    tbt.ContractCode = strContractCode;
                            //    tbt.OCC = strNewOCC;
                            //    tbt.SubcontractorCode = subCon.SubcontractorCode;
                            //    this.InsertTbt_RentalInstSubContractor(tbt);
                            //}
                            #endregion
                            this.InsertDataToSubcontract(doComplete, strNewOCC);

                            //5.2.4.4 Call send data to billing module
                            IBillingInterfaceHandler billhand = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                            billhand.SendBilling_RentalCompleteInstall(strContractCode, doComplete.InstallationCompleteDate);

                            //5.2.4.5 Delete all billing temp
                            IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                            List<tbt_BillingTemp> dtDeletedTbt_BillingTemp = target.DeleteBillingTempByContractCode(strContractCode);

                            //5.2.4.6 Teerapong S. 21/08/2012 Check need to send billing or not
                            if (!CommonUtil.IsNullOrEmpty(doComplete.BillingOCC) && doComplete.BillingInstallationFee > 0)
                            {
                                doBillingTempDetail doBillingTempDetail = new doBillingTempDetail();
                                doBillingTempDetail.ContractCode = doComplete.ContractCode;
                                doBillingTempDetail.BillingOCC = doComplete.BillingOCC;
                                if (doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL ||
                                    doComplete.InstallationType == RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL)
                                {
                                    doBillingTempDetail.ContractBillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE;
                                }
                                else
                                {
                                    doBillingTempDetail.ContractBillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE;
                                }
                                doBillingTempDetail.BillingDate = doComplete.InstallationCompleteDate;
                                doBillingTempDetail.BillingAmount = doComplete.BillingInstallationFee;

                                IBillingInterfaceHandler hand = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                                bool blnProcessResult = hand.SendBilling_OnetimeFee(doBillingTempDetail);
                            }
                            //============================================================================

                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region CompleteInstallation Private Method

        private enum eCompleteInstallation
        {
            BEFORE_START_NOTCOMPLETE
            ,
            BEFORE_START_COMPLETE
                ,
            AFTER_START_UNIMP_EXIST
                , AFTER_START_UNIMP_NOTEXIST
        }

        /// <summary>
        /// Prepare object complete installation
        /// </summary>
        /// <param name="dsRentalData"></param>
        /// <param name="eCase"></param>
        /// <param name="strOCC"></param>
        /// <param name="doComplete"></param>
        /// <returns></returns>
        private dsRentalContractData prepareObjCompleteInstallation(dsRentalContractData dsRentalData, eCompleteInstallation eCase, string strOCC, doCompleteInstallationData doComplete)
        {
            if (eCompleteInstallation.BEFORE_START_NOTCOMPLETE == eCase)
            {
                prepareObjContractBasic(dsRentalData, strOCC, doComplete, eCase);
                prepareObjSecurityBasic(dsRentalData, strOCC, doComplete, eCase);
            }
            else if (eCompleteInstallation.BEFORE_START_COMPLETE == eCase)
            {
                prepareObjContractBasic(dsRentalData, strOCC, doComplete, eCase);
                prepareObjSecurityBasic(dsRentalData, strOCC, doComplete, eCase);
                prepareObjSetNewOCC(dsRentalData, strOCC, doComplete, eCase);
            }
            else if (eCompleteInstallation.AFTER_START_UNIMP_EXIST == eCase || eCompleteInstallation.AFTER_START_UNIMP_NOTEXIST == eCase)
            {
                prepareObjContractBasic(dsRentalData, strOCC, doComplete, eCase);
                prepareObjSecurityBasic(dsRentalData, strOCC, doComplete, eCase);
                prepareObjInstrumentDetails(dsRentalData, strOCC, doComplete, eCase);
                prepareObjSetNewOCC(dsRentalData, strOCC, doComplete, eCase);
            }

            return dsRentalData;
        }

        /// <summary>
        /// Prepare object contract basic
        /// </summary>
        /// <param name="dsRentalData"></param>
        /// <param name="strOCC"></param>
        /// <param name="doComplete"></param>
        /// <param name="eCase"></param>
        private void prepareObjContractBasic(dsRentalContractData dsRentalData, string strOCC, doCompleteInstallationData doComplete, eCompleteInstallation eCase)
        {
            tbt_RentalContractBasic contractData = CommonUtil.CloneObject<tbt_RentalContractBasic, tbt_RentalContractBasic>(dsRentalData.dtTbt_RentalContractBasic[0]);

            contractData.ContractCode = doComplete.ContractCode;
            if (eCompleteInstallation.BEFORE_START_NOTCOMPLETE == eCase)
            {
                if (contractData.FirstInstallCompleteFlag == FlagType.C_FLAG_OFF)
                {
                    contractData.FirstInstallCompleteFlag = FlagType.C_FLAG_ON;
                    contractData.NewInstallCompleteProcessDate = doComplete.InstallationCompleteProcessDate;
                }
            }
            else
            {
                contractData.LastOCC = strOCC;
            }
            contractData.LastNormalContractFee = doComplete.NormalInstallationFee;
            contractData.LastOrderContractFee = doComplete.BillingInstallationFee;

            dsRentalData.dtTbt_RentalContractBasic[0] = contractData;
        }

        /// <summary>
        /// Prepare object security basic
        /// </summary>
        /// <param name="dsRentalData"></param>
        /// <param name="strOCC"></param>
        /// <param name="doComplete"></param>
        /// <param name="eCase"></param>
        private void prepareObjSecurityBasic(dsRentalContractData dsRentalData, string strOCC, doCompleteInstallationData doComplete, eCompleteInstallation eCase)
        {
            tbt_RentalSecurityBasic securityData = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(dsRentalData.dtTbt_RentalSecurityBasic[0]);
            securityData.ContractCode = doComplete.ContractCode;
            securityData.OCC = strOCC;
            securityData.NormalContractFee = doComplete.NormalInstallationFee;
            securityData.OrderContractFee = doComplete.BillingInstallationFee;
            securityData.InstallationCompleteFlag = FlagType.C_FLAG_ON;
            securityData.InstallationSlipNo = doComplete.InstallationSlipNo;
            securityData.InstallationCompleteDate = doComplete.InstallationCompleteDate;
            securityData.InstallationTypeCode = doComplete.InstallationType;
            securityData.InstallationCompleteEmpNo = CommonUtil.dsTransData.dtUserData.EmpNo;

            if (eCompleteInstallation.AFTER_START_UNIMP_EXIST == eCase || eCompleteInstallation.AFTER_START_UNIMP_NOTEXIST == eCase)
            {
                securityData.ImplementFlag = FlagType.C_FLAG_ON;
            }

            dsRentalData.dtTbt_RentalSecurityBasic[0] = securityData;
        }

        /// <summary>
        /// Prepare object instrument detail
        /// </summary>
        /// <param name="dsRentalData"></param>
        /// <param name="strOCC"></param>
        /// <param name="doComplete"></param>
        /// <param name="eCase"></param>
        private void prepareObjInstrumentDetails(dsRentalContractData dsRentalData, string strOCC, doCompleteInstallationData doComplete, eCompleteInstallation eCase)
        {
            dsRentalData.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
            foreach (doInstrumentDetails instDetail in doComplete.doInstrumentDetailsList)
            {
                tbt_RentalInstrumentDetails rentalDetailDo = new tbt_RentalInstrumentDetails();
                rentalDetailDo.ContractCode = doComplete.ContractCode;
                rentalDetailDo.OCC = strOCC;
                rentalDetailDo.InstrumentCode = instDetail.InstrumentCode;
                rentalDetailDo.InstrumentTypeCode = InstrumentType.C_INST_TYPE_GENERAL;
                rentalDetailDo.InstrumentQty = instDetail.AddQty - instDetail.RemoveQty;
                rentalDetailDo.AdditionalInstrumentQty = 0;
                rentalDetailDo.RemovalInstrumentQty = 0;
                dsRentalData.dtTbt_RentalInstrumentDetails.Add(rentalDetailDo);
            }
        }

        /// <summary>
        /// Prepare object set new occurrence
        /// </summary>
        /// <param name="dsRentalData"></param>
        /// <param name="strOCC"></param>
        /// <param name="doComplete"></param>
        /// <param name="eCase"></param>
        private void prepareObjSetNewOCC(dsRentalContractData dsRentalData, string strOCC, doCompleteInstallationData doComplete, eCompleteInstallation eCase)
        {
            //tbt_RentalBEDetails
            List<tbt_RentalBEDetails> lstBEDetails = CommonUtil.ClonsObjectList<tbt_RentalBEDetails, tbt_RentalBEDetails>(dsRentalData.dtTbt_RentalBEDetails);
            foreach (tbt_RentalBEDetails tbt in lstBEDetails)
            {
                tbt.OCC = strOCC;
            }
            dsRentalData.dtTbt_RentalBEDetails = lstBEDetails;

            //tbt_RentalInstrumentDetails
            List<tbt_RentalInstrumentDetails> lstInstDetails = CommonUtil.ClonsObjectList<tbt_RentalInstrumentDetails, tbt_RentalInstrumentDetails>(dsRentalData.dtTbt_RentalInstrumentDetails);
            foreach (tbt_RentalInstrumentDetails tbt in lstInstDetails)
            {
                tbt.OCC = strOCC;
            }
            dsRentalData.dtTbt_RentalInstrumentDetails = lstInstDetails;

            //tbt_RentalSentryGuard
            List<tbt_RentalSentryGuard> lstSentry = CommonUtil.ClonsObjectList<tbt_RentalSentryGuard, tbt_RentalSentryGuard>(dsRentalData.dtTbt_RentalSentryGuard);
            foreach (tbt_RentalSentryGuard tbt in lstSentry)
            {
                tbt.OCC = strOCC;
            }
            dsRentalData.dtTbt_RentalSentryGuard = lstSentry;

            //tbt_RentalSentryGuardDetails            
            List<tbt_RentalSentryGuardDetails> lstSentryDetails = CommonUtil.ClonsObjectList<tbt_RentalSentryGuardDetails, tbt_RentalSentryGuardDetails>(dsRentalData.dtTbt_RentalSentryGuardDetails);
            foreach (tbt_RentalSentryGuardDetails tbt in lstSentryDetails)
            {
                tbt.OCC = strOCC;
            }
            dsRentalData.dtTbt_RentalSentryGuardDetails = lstSentryDetails;

            //tbt_CancelContractMemoDetail
            List<tbt_CancelContractMemoDetail> lstMemoDetails = CommonUtil.ClonsObjectList<tbt_CancelContractMemoDetail, tbt_CancelContractMemoDetail>(dsRentalData.dtTbt_CancelContractMemoDetail);
            foreach (tbt_CancelContractMemoDetail tbt in lstMemoDetails)
            {
                tbt.OCC = strOCC;
            }
            dsRentalData.dtTbt_CancelContractMemoDetail = lstMemoDetails;

            //tbt_RentalOperationType
            List<tbt_RentalOperationType> lstOprType = CommonUtil.ClonsObjectList<tbt_RentalOperationType, tbt_RentalOperationType>(dsRentalData.dtTbt_RentalOperationType);
            foreach (tbt_RentalOperationType tbt in lstOprType)
            {
                tbt.OCC = strOCC;
            }
            dsRentalData.dtTbt_RentalOperationType = lstOprType;

            //tbt_RentalMaintenanceDetails
            List<tbt_RentalMaintenanceDetails> lstMainDetails = CommonUtil.ClonsObjectList<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(dsRentalData.dtTbt_RentalMaintenanceDetails);
            foreach (tbt_RentalMaintenanceDetails tbt in lstMainDetails)
            {
                tbt.OCC = strOCC;
            }
            dsRentalData.dtTbt_RentalMaintenanceDetails = lstMainDetails;
        }

        /// <summary>
        /// Insert data to subcontract
        /// </summary>
        /// <param name="doComplete"></param>
        /// <param name="strOCC"></param>
        private void InsertDataToSubcontract(doCompleteInstallationData doComplete, string strOCC)
        {
            if (doComplete.doSubcontractorDetailsList != null && doComplete.doSubcontractorDetailsList.Count > 0)
            {
                foreach (doSubcontractorDetails subCon in doComplete.doSubcontractorDetailsList)
                {
                    tbt_RentalInstSubcontractor tbt = new tbt_RentalInstSubcontractor();
                    tbt.ContractCode = doComplete.ContractCode;
                    tbt.OCC = strOCC;
                    tbt.SubcontractorCode = subCon.SubcontractorCode;
                    this.InsertTbt_RentalInstSubContractor(tbt);
                }
            }
        }

        #endregion

        /// <summary>
        /// Replace contract data with quotation data. Using when create contract or change contract.
        /// This method can be run on client.
        /// </summary>
        /// <param name="dsQuotation"></param>
        /// <param name="dsRentalContract"></param>
        /// <param name="needSumInstrumentQty"></param>
        public void MapFromQuotation(dsQuotationData dsQuotation, ref dsRentalContractData dsRentalContract, bool needSumInstrumentQty)
        {
            tbt_RentalSecurityBasic tbtRentalSecurityBasic;
            tbt_QuotationBasic tbtQuotationBasic;
            tbt_QuotationBeatGuardDetails tbtQuotationBeatGuardDetails;
            tbt_RentalBEDetails tbtRentalBEDetails;
            tbt_RentalInstrumentDetails tbtRentalInstrumentDetails;
            tbt_RentalMaintenanceDetails tbtRentalMaintenanceDetails;
            tbt_RentalOperationType tbtRentalOperationType;
            tbt_RentalSentryGuard tbtRentalSentryGuard;
            tbt_RentalSentryGuardDetails tbtRentalSentryGuardDetail;

            ICommonContractHandler commonContractHandler;
            List<string> listContractCode;

            try
            {
                listContractCode = new List<string>();
                commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                //------------------------------------------------------------------------------------------------------------------------

                //1. Update dtTbt_RentalSecurityBasic
                if ((dsRentalContract.dtTbt_RentalSecurityBasic.Count != 0) && dsQuotation.dtTbt_QuotationBasic != null)
                {
                    tbtRentalSecurityBasic = dsRentalContract.dtTbt_RentalSecurityBasic[0];
                    tbtQuotationBasic = dsQuotation.dtTbt_QuotationBasic;
                    tbtRentalSecurityBasic = dsRentalContract.dtTbt_RentalSecurityBasic[0];
                    tbtRentalSecurityBasic.QuotationTargetCode = tbtQuotationBasic.QuotationTargetCode;
                    tbtRentalSecurityBasic.QuotationAlphabet = tbtQuotationBasic.Alphabet;
                    tbtRentalSecurityBasic.SecurityTypeCode = tbtQuotationBasic.SecurityTypeCode;
                    tbtRentalSecurityBasic.ProductCode = tbtQuotationBasic.ProductCode;
                    tbtRentalSecurityBasic.ProductCode = tbtQuotationBasic.ProductCode;
                    tbtRentalSecurityBasic.PhoneLineTypeCode1 = tbtQuotationBasic.PhoneLineTypeCode1;
                    tbtRentalSecurityBasic.PhoneLineOwnerTypeCode1 = tbtQuotationBasic.PhoneLineOwnerTypeCode1;
                    tbtRentalSecurityBasic.PhoneLineTypeCode2 = tbtQuotationBasic.PhoneLineTypeCode2;
                    tbtRentalSecurityBasic.PhoneLineOwnerTypeCode2 = tbtQuotationBasic.PhoneLineOwnerTypeCode2;
                    tbtRentalSecurityBasic.PhoneLineTypeCode3 = tbtQuotationBasic.PhoneLineTypeCode3;
                    tbtRentalSecurityBasic.PhoneLineOwnerTypeCode3 = tbtQuotationBasic.PhoneLineOwnerTypeCode3;
                    tbtRentalSecurityBasic.ContractDurationMonth = tbtQuotationBasic.ContractDurationMonth;
                    tbtRentalSecurityBasic.AutoRenewMonth = tbtQuotationBasic.AutoRenewMonth;
                    tbtRentalSecurityBasic.MaintenanceCycle = tbtQuotationBasic.MaintenanceCycle;
                    tbtRentalSecurityBasic.FireMonitorFlag = tbtQuotationBasic.FireMonitorFlag;
                    tbtRentalSecurityBasic.CrimePreventFlag = tbtQuotationBasic.CrimePreventFlag;
                    tbtRentalSecurityBasic.EmergencyReportFlag = tbtQuotationBasic.EmergencyReportFlag;
                    tbtRentalSecurityBasic.FacilityMonitorFlag = tbtQuotationBasic.FacilityMonitorFlag;
                    //tbtRentalSecurityBasic.SalesmanEmpNo1 = tbtQuotationBasic.SalesmanEmpNo1;
                    //tbtRentalSecurityBasic.SalesmanEmpNo2 = tbtQuotationBasic.SalesmanEmpNo2;
                    tbtRentalSecurityBasic.SalesSupporterEmpNo = tbtQuotationBasic.SalesSupporterEmpNo;
                    tbtRentalSecurityBasic.BuildingTypeCode = tbtQuotationBasic.BuildingTypeCode;
                    tbtRentalSecurityBasic.MainStructureTypeCode = tbtQuotationBasic.MainStructureTypeCode;
                    tbtRentalSecurityBasic.NumOfBuilding = tbtQuotationBasic.NumOfBuilding;
                    tbtRentalSecurityBasic.NumOfFloor = tbtQuotationBasic.NumOfFloor;
                    tbtRentalSecurityBasic.SiteBuildingArea = tbtQuotationBasic.SiteBuildingArea;
                    tbtRentalSecurityBasic.SecurityAreaFrom = tbtQuotationBasic.SecurityAreaFrom;
                    tbtRentalSecurityBasic.SecurityAreaTo = tbtQuotationBasic.SecurityAreaTo;
                    tbtRentalSecurityBasic.PlanCode = tbtQuotationBasic.PlanCode;
                    tbtRentalSecurityBasic.FacilityPassYear = tbtQuotationBasic.FacilityPassYear;
                    tbtRentalSecurityBasic.FacilityPassMonth = tbtQuotationBasic.FacilityPassMonth;
                    tbtRentalSecurityBasic.InsuranceCoverageAmount = tbtQuotationBasic.InsuranceCoverageAmount;
                    tbtRentalSecurityBasic.InsuranceTypeCode = tbtQuotationBasic.InsuranceTypeCode;
                    tbtRentalSecurityBasic.MonthlyInsuranceFee = tbtQuotationBasic.MonthlyInsuranceFee;
                    tbtRentalSecurityBasic.DispatchTypeCode = tbtQuotationBasic.DispatchTypeCode;
                    tbtRentalSecurityBasic.AdditionalFee1 = tbtQuotationBasic.AdditionalFee1;
                    tbtRentalSecurityBasic.AdditionalFee2 = tbtQuotationBasic.AdditionalFee2;
                    tbtRentalSecurityBasic.AdditionalFee3 = tbtQuotationBasic.AdditionalFee3;
                    tbtRentalSecurityBasic.AdditionalApproveNo1 = tbtQuotationBasic.AdditionalApproveNo1;
                    tbtRentalSecurityBasic.AdditionalApproveNo2 = tbtQuotationBasic.AdditionalApproveNo2;
                    tbtRentalSecurityBasic.AdditionalApproveNo3 = tbtQuotationBasic.AdditionalApproveNo3;
                    tbtRentalSecurityBasic.MaintenanceFee1 = tbtQuotationBasic.MaintenanceFee1;
                    tbtRentalSecurityBasic.MaintenanceFee2 = tbtQuotationBasic.MaintenanceFee2;
                    tbtRentalSecurityBasic.SpecialInstallationFlag = tbtQuotationBasic.SpecialInstallationFlag;
                    tbtRentalSecurityBasic.PlannerEmpNo = tbtQuotationBasic.PlannerEmpNo;
                    tbtRentalSecurityBasic.PlanCheckerEmpNo = tbtQuotationBasic.PlanCheckerEmpNo;
                    tbtRentalSecurityBasic.PlanCheckDate = tbtQuotationBasic.PlanCheckDate;
                    tbtRentalSecurityBasic.PlanApproverEmpNo = tbtQuotationBasic.PlanApproverEmpNo;
                    tbtRentalSecurityBasic.PlanApproveDate = tbtQuotationBasic.PlanApproveDate;
                    tbtRentalSecurityBasic.FacilityMemo = tbtQuotationBasic.FacilityMemo;

                }

                //------------------------------------------------------------------------------------------------------------------------

                //2. Update dtTbt_RentalBEDetails by replace data in the row from quotation
                if ((dsRentalContract.dtTbt_RentalBEDetails.Count != 0) && dsQuotation.dtTbt_QuotationBeatGuardDetails != null)
                {
                    tbtRentalBEDetails = dsRentalContract.dtTbt_RentalBEDetails[0];
                    tbtQuotationBeatGuardDetails = dsQuotation.dtTbt_QuotationBeatGuardDetails;

                    tbtRentalBEDetails.NumOfNightTimeWd = tbtQuotationBeatGuardDetails.NumOfNightTimeWd;
                    tbtRentalBEDetails.NumOfDayTimeWd = tbtQuotationBeatGuardDetails.NumOfDayTimeWd;
                    tbtRentalBEDetails.NumOfNightTimeSat = tbtQuotationBeatGuardDetails.NumOfNightTimeSat;
                    tbtRentalBEDetails.NumOfDayTimeSat = tbtQuotationBeatGuardDetails.NumOfDayTimeSat;
                    tbtRentalBEDetails.NumOfNightTimeSun = tbtQuotationBeatGuardDetails.NumOfNightTimeSun;
                    tbtRentalBEDetails.NumOfDayTimeSun = tbtQuotationBeatGuardDetails.NumOfDayTimeSun;
                    tbtRentalBEDetails.NumOfBeatStep = tbtQuotationBeatGuardDetails.NumOfBeatStep;
                    tbtRentalBEDetails.FreqOfGateUsage = tbtQuotationBeatGuardDetails.FreqOfGateUsage;
                    tbtRentalBEDetails.NumOfClockKey = tbtQuotationBeatGuardDetails.NumOfClockKey;
                    tbtRentalBEDetails.NumOfDate = tbtQuotationBeatGuardDetails.NumOfDate;
                    tbtRentalBEDetails.NotifyTime = tbtQuotationBeatGuardDetails.NotifyTime;
                }

                //------------------------------------------------------------------------------------------------------------------------

                //3. Update dtTbt_RentalInstrumentDetails by remove all exiting rows and insert all rows from quotation instead                

                List<tbt_RentalInstrumentDetails> dtTbt_RentalInstrumentDetailsReserve;
                dtTbt_RentalInstrumentDetailsReserve = CommonUtil.ClonsObjectList<tbt_RentalInstrumentDetails, tbt_RentalInstrumentDetails>(dsRentalContract.dtTbt_RentalInstrumentDetails);

                dsRentalContract.dtTbt_RentalInstrumentDetails.Clear();
                if (dsQuotation.dtTbt_QuotationInstrumentDetails != null)
                {
                    for (int i = 0; i <= dsQuotation.dtTbt_QuotationInstrumentDetails.Count() - 1; i++)
                    {
                        //3.1. Update from dtTbt_QuotationInstrumentDetails
                        tbtRentalInstrumentDetails = new tbt_RentalInstrumentDetails();
                        tbtRentalInstrumentDetails.ContractCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
                        tbtRentalInstrumentDetails.OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
                        tbtRentalInstrumentDetails.InstrumentCode = dsQuotation.dtTbt_QuotationInstrumentDetails[i].InstrumentCode;
                        tbtRentalInstrumentDetails.InstrumentTypeCode = SECOM_AJIS.Common.Util.ConstantValue.InstrumentType.C_INST_TYPE_GENERAL;

                        if (needSumInstrumentQty == FlagType.C_FLAG_ON)
                        {
                            tbtRentalInstrumentDetails.InstrumentQty = dsQuotation.dtTbt_QuotationInstrumentDetails[i].InstrumentQty
                                                                        + dsQuotation.dtTbt_QuotationInstrumentDetails[i].AddQty
                                                                        - dsQuotation.dtTbt_QuotationInstrumentDetails[i].RemoveQty;
                            tbtRentalInstrumentDetails.AdditionalInstrumentQty = 0;
                            tbtRentalInstrumentDetails.RemovalInstrumentQty = 0;
                        }
                        else
                        {
                            tbtRentalInstrumentDetails.InstrumentQty = dsQuotation.dtTbt_QuotationInstrumentDetails[i].InstrumentQty;
                            tbtRentalInstrumentDetails.AdditionalInstrumentQty = dsQuotation.dtTbt_QuotationInstrumentDetails[i].AddQty;
                            tbtRentalInstrumentDetails.RemovalInstrumentQty = dsQuotation.dtTbt_QuotationInstrumentDetails[i].RemoveQty;
                        }

                        tbtRentalInstrumentDetails.CreateBy = dsQuotation.dtTbt_QuotationInstrumentDetails[i].CreateBy;
                        tbtRentalInstrumentDetails.CreateDate = dsQuotation.dtTbt_QuotationInstrumentDetails[i].CreateDate.Value;
                        tbtRentalInstrumentDetails.UpdateBy = dsQuotation.dtTbt_QuotationInstrumentDetails[i].UpdateBy;
                        tbtRentalInstrumentDetails.UpdateDate = dsQuotation.dtTbt_QuotationInstrumentDetails[i].UpdateDate;

                        dsRentalContract.dtTbt_RentalInstrumentDetails.Add(tbtRentalInstrumentDetails);
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------

                if (dsRentalContract.dtTbt_RentalInstrumentDetails != null)
                {
                    //3.2. Update from dtTbt_QuotationFacilityDetails
                    for (int i = 0; i <= dsQuotation.dtTbt_QuotationFacilityDetails.Count() - 1; i++)
                    {
                        tbtRentalInstrumentDetails = new tbt_RentalInstrumentDetails();
                        tbtRentalInstrumentDetails.ContractCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
                        tbtRentalInstrumentDetails.OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
                        tbtRentalInstrumentDetails.InstrumentCode = dsQuotation.dtTbt_QuotationFacilityDetails[i].FacilityCode;
                        tbtRentalInstrumentDetails.InstrumentTypeCode = SECOM_AJIS.Common.Util.ConstantValue.InstrumentType.C_INST_TYPE_MONITOR;
                        tbtRentalInstrumentDetails.InstrumentQty = dsQuotation.dtTbt_QuotationFacilityDetails[i].FacilityQty;
                        tbtRentalInstrumentDetails.AdditionalInstrumentQty = 0;
                        tbtRentalInstrumentDetails.RemovalInstrumentQty = 0;
                        tbtRentalInstrumentDetails.CreateBy = dsQuotation.dtTbt_QuotationFacilityDetails[i].CreateBy;
                        tbtRentalInstrumentDetails.CreateDate = dsQuotation.dtTbt_QuotationFacilityDetails[i].CreateDate.Value;
                        tbtRentalInstrumentDetails.UpdateBy = dsQuotation.dtTbt_QuotationFacilityDetails[i].UpdateBy;
                        tbtRentalInstrumentDetails.UpdateDate = dsQuotation.dtTbt_QuotationFacilityDetails[i].UpdateDate;
                        dsRentalContract.dtTbt_RentalInstrumentDetails.Add(tbtRentalInstrumentDetails);
                    }
                }

                if (dsRentalContract.dtTbt_RentalMaintenanceDetails.Count() != 0)
                {
                    //4. Update dtTbt_RentalMaintenanceDetails by replace data in the row from quotation
                    tbtRentalMaintenanceDetails = dsRentalContract.dtTbt_RentalMaintenanceDetails[0];
                    tbtRentalMaintenanceDetails.MaintenanceTargetProductTypeCode = dsQuotation.dtTbt_QuotationBasic.MaintenanceTargetProductTypeCode;
                    tbtRentalMaintenanceDetails.MaintenanceTypeCode = dsQuotation.dtTbt_QuotationBasic.MaintenanceTypeCode;
                    tbtRentalMaintenanceDetails.MaintenanceMemo = dsQuotation.dtTbt_QuotationBasic.MaintenanceMemo;
                }
                //------------------------------------------------------------------------------------------------------------------------

                dsRentalContract.dtTbt_RentalOperationType.Clear();
                if (dsQuotation.dtTbt_QuotationOperationType != null)
                {
                    //5. Update dtTbt_RentalOperationType by remove all exiting rows and insert all rows from quotation instead
                    for (int i = 0; i <= dsQuotation.dtTbt_QuotationOperationType.Count() - 1; i++)
                    {
                        tbtRentalOperationType = new tbt_RentalOperationType();
                        tbtRentalOperationType.ContractCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
                        tbtRentalOperationType.OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
                        tbtRentalOperationType.OperationTypeCode = dsQuotation.dtTbt_QuotationOperationType[i].OperationTypeCode;
                        tbtRentalOperationType.CreateBy = dsQuotation.dtTbt_QuotationOperationType[i].CreateBy;
                        tbtRentalOperationType.CreateDate = dsQuotation.dtTbt_QuotationOperationType[i].CreateDate.Value;
                        tbtRentalOperationType.UpdateBy = dsQuotation.dtTbt_QuotationOperationType[i].UpdateBy;
                        tbtRentalOperationType.UpdateDate = dsQuotation.dtTbt_QuotationOperationType[i].UpdateDate;
                        dsRentalContract.dtTbt_RentalOperationType.Add(tbtRentalOperationType);
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------
                if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG
                )
                {
                    //6. Update dtTbt_RentalSG by replace data in the row from quotation
                    if (dsRentalContract.dtTbt_RentalSentryGuard != null)
                    {
                        if (dsRentalContract.dtTbt_RentalSentryGuard.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalSentryGuard[0].ContractCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
                            dsRentalContract.dtTbt_RentalSentryGuard[0].OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
                            dsRentalContract.dtTbt_RentalSentryGuard[0].SentryGuardFee = dsQuotation.dtTbt_QuotationBasic.SentryGuardFee;
                            dsRentalContract.dtTbt_RentalSentryGuard[0].TotalSentryGuardFee = dsQuotation.dtTbt_QuotationBasic.SentryGuardFee;
                            dsRentalContract.dtTbt_RentalSentryGuard[0].SecurityItemFee = dsQuotation.dtTbt_QuotationBasic.SecurityItemFee;
                            dsRentalContract.dtTbt_RentalSentryGuard[0].OtherItemFee = dsQuotation.dtTbt_QuotationBasic.OtherItemFee;
                            dsRentalContract.dtTbt_RentalSentryGuard[0].SentryGuardAreaTypeCode = dsQuotation.dtTbt_QuotationBasic.SentryGuardAreaTypeCode;
                        }

                        //tbtRentalSentryGuard = new tbt_RentalSentryGuard();
                        //tbtRentalSentryGuard.ContractCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
                        //tbtRentalSentryGuard.OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
                        //tbtRentalSentryGuard.SentryGuardFee = dsQuotation.dtTbt_QuotationBasic.SentryGuardFee;
                        //tbtRentalSentryGuard.TotalSentryGuardFee = dsQuotation.dtTbt_QuotationBasic.SentryGuardFee;
                        //tbtRentalSentryGuard.SecurityItemFee = dsQuotation.dtTbt_QuotationBasic.SecurityItemFee;
                        //tbtRentalSentryGuard.OtherItemFee = dsQuotation.dtTbt_QuotationBasic.OtherItemFee;
                        //tbtRentalSentryGuard.SentryGuardAreaTypeCode = dsQuotation.dtTbt_QuotationBasic.SentryGuardAreaTypeCode;
                        //dsRentalContract.dtTbt_RentalSentryGuard.Add(tbtRentalSentryGuard);
                    }

                    //------------------------------------------------------------------------------------------------------------------------

                    //7. Update dtTbt_RentalSGDetails by remove all exiting rows and insert all rows from quotation instead
                    dsRentalContract.dtTbt_RentalSentryGuardDetails.Clear();
                    if (dsQuotation.dtTbt_QuotationSentryGuardDetails != null)
                    {
                        for (int i = 0; i <= dsQuotation.dtTbt_QuotationSentryGuardDetails.Count() - 1; i++)
                        {
                            tbtRentalSentryGuardDetail = new tbt_RentalSentryGuardDetails();
                            tbtRentalSentryGuardDetail.ContractCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
                            tbtRentalSentryGuardDetail.OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
                            tbtRentalSentryGuardDetail.SequenceNo = dsQuotation.dtTbt_QuotationSentryGuardDetails[i].RunningNo;
                            tbtRentalSentryGuardDetail.SecurityStartTime = dsQuotation.dtTbt_QuotationSentryGuardDetails[i].SecurityStartTime;
                            tbtRentalSentryGuardDetail.SecurityFinishTime = dsQuotation.dtTbt_QuotationSentryGuardDetails[i].SecurityFinishTime;
                            tbtRentalSentryGuardDetail.SentryGuardTypeCode = dsQuotation.dtTbt_QuotationSentryGuardDetails[i].SentryGuardTypeCode;
                            tbtRentalSentryGuardDetail.NumOfDate = dsQuotation.dtTbt_QuotationSentryGuardDetails[i].NumOfDate;
                            tbtRentalSentryGuardDetail.TimeUnitPrice = dsQuotation.dtTbt_QuotationSentryGuardDetails[i].CostPerHour;
                            tbtRentalSentryGuardDetail.WorkHourPerMonth = dsQuotation.dtTbt_QuotationSentryGuardDetails[i].WorkHourPerMonth;
                            tbtRentalSentryGuardDetail.NumOfSentryGuard = dsQuotation.dtTbt_QuotationSentryGuardDetails[i].NumOfSentryGuard;
                            tbtRentalSentryGuardDetail.CreateBy = dsQuotation.dtTbt_QuotationSentryGuardDetails[i].CreateBy;
                            tbtRentalSentryGuardDetail.CreateDate = dsQuotation.dtTbt_QuotationSentryGuardDetails[i].CreateDate.Value;
                            tbtRentalSentryGuardDetail.UpdateBy = dsQuotation.dtTbt_QuotationSentryGuardDetails[i].UpdateBy;
                            tbtRentalSentryGuardDetail.UpdateDate = dsQuotation.dtTbt_QuotationSentryGuardDetails[i].UpdateDate;

                            dsRentalContract.dtTbt_RentalSentryGuardDetails.Add(tbtRentalSentryGuardDetail);
                        }
                    }
                }

                if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE
                )
                {
                    //6. Update dtTbt_RentalSG by replace data in the row from quotation
                    if (dsRentalContract.dtTbt_RentalBEDetails != null)
                    {
                        if (dsRentalContract.dtTbt_RentalBEDetails.Count > 0)
                        {
                            dsRentalContract.dtTbt_RentalBEDetails[0].ContractCode = dsRentalContract.dtTbt_RentalBEDetails[0].ContractCode;
                            dsRentalContract.dtTbt_RentalBEDetails[0].OCC = dsRentalContract.dtTbt_RentalBEDetails[0].OCC;
                            dsRentalContract.dtTbt_RentalBEDetails[0].NumOfNightTimeWd = dsQuotation.dtTbt_QuotationBeatGuardDetails.NumOfNightTimeWd;
                            dsRentalContract.dtTbt_RentalBEDetails[0].NumOfDayTimeWd = dsQuotation.dtTbt_QuotationBeatGuardDetails.NumOfDayTimeWd;
                            dsRentalContract.dtTbt_RentalBEDetails[0].NumOfNightTimeSat = dsQuotation.dtTbt_QuotationBeatGuardDetails.NumOfNightTimeSat;
                            dsRentalContract.dtTbt_RentalBEDetails[0].NumOfDayTimeSat = dsQuotation.dtTbt_QuotationBeatGuardDetails.NumOfDayTimeSat;
                            dsRentalContract.dtTbt_RentalBEDetails[0].NumOfNightTimeSun = dsQuotation.dtTbt_QuotationBeatGuardDetails.NumOfNightTimeSun;
                            dsRentalContract.dtTbt_RentalBEDetails[0].NumOfDayTimeSun = dsQuotation.dtTbt_QuotationBeatGuardDetails.NumOfDayTimeSun;
                            dsRentalContract.dtTbt_RentalBEDetails[0].NumOfBeatStep = dsQuotation.dtTbt_QuotationBeatGuardDetails.NumOfBeatStep;
                            dsRentalContract.dtTbt_RentalBEDetails[0].FreqOfGateUsage = dsQuotation.dtTbt_QuotationBeatGuardDetails.FreqOfGateUsage;
                            dsRentalContract.dtTbt_RentalBEDetails[0].NumOfClockKey = dsQuotation.dtTbt_QuotationBeatGuardDetails.NumOfClockKey;
                            dsRentalContract.dtTbt_RentalBEDetails[0].NumOfDate = dsQuotation.dtTbt_QuotationBeatGuardDetails.NumOfDate;
                            dsRentalContract.dtTbt_RentalBEDetails[0].NotifyTime = dsQuotation.dtTbt_QuotationBeatGuardDetails.NotifyTime;
                        }
                    }

                    //------------------------------------------------------------------------------------------------------------------------
                }

                //8. Update dtTbt_RelationType
                if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                {
                    List<tbt_RelationType> listRelationType = dsRentalContract.dtTbt_RelationType.FindAll(delegate(tbt_RelationType s) { return s.RelationType == RelationType.C_RELATION_TYPE_MA; });

                    foreach (var item in listRelationType)
                    {
                        dsRentalContract.dtTbt_RelationType.Remove(item);
                    }

                    // 2012-09-12 Phoomsak L. add for checking in case don't have data in dtTbt_QuotationMaintenanceLinkage (case customer product)
                    if (dsQuotation.dtTbt_QuotationMaintenanceLinkage != null && dsQuotation.dtTbt_QuotationMaintenanceLinkage.Count > 0)
                    {
                        List<tbt_RelationType> listGenerate;

                        foreach (tbt_QuotationMaintenanceLinkage item in dsQuotation.dtTbt_QuotationMaintenanceLinkage)
                        {
                            listContractCode.Add(item.ContractCode);
                        }

                        if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                        {
                            listGenerate = commonContractHandler.GenerateMaintenanceRelationType(dsQuotation.dtTbt_QuotationMaintenanceLinkage[0].QuotationTargetCode, listContractCode, true);
                        }
                        else
                        {
                            listGenerate = commonContractHandler.GenerateMaintenanceRelationType(dsQuotation.dtTbt_QuotationMaintenanceLinkage[0].QuotationTargetCode, listContractCode, false);
                        }

                        foreach (tbt_RelationType item in listGenerate)
                        {
                            item.OCC = dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC; //Add by Jutarat A. on 04022013
                            dsRentalContract.dtTbt_RelationType.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Replace contract data with quotation data. Using when create contract or change contract.
        /// This method can be run on client.
        /// </summary>
        /// <param name="dsQuotation"></param>
        /// <param name="dsRentalContract"></param>
        public void MapFromQuotation(dsQuotationData dsQuotation, ref dsRentalContractData dsRentalContract)
        {
            this.MapFromQuotation(dsQuotation, ref dsRentalContract, FlagType.C_FLAG_OFF);
        }

        //public bool RegisterChangePlan(dsRentalContractData dsRentalContract, dsQuotationData dsQuotation, List<dtBillingTempChangePlanData> listBillingTemp, List<dtBillingClientData> listBillingClient, bool contractDurationChangeFlag)
        /// <summary>
        /// For register change plan of rental contract
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <param name="dsQuotation"></param>
        /// <param name="newItemList"></param>
        /// <param name="updateItemList"></param>
        /// <param name="deleteItemList"></param>
        /// <param name="contractDurationChangeFlag"></param>
        /// <param name="lastOCCForBilling"></param>
        /// <returns></returns>
        public bool RegisterChangePlan(dsRentalContractData dsRentalContract, dsQuotationData dsQuotation, List<dtBillingTemp_SetItem> newItemList, List<dtBillingTemp_SetItem> updateItemList, List<dtBillingTemp_SetItem> deleteItemList, bool contractDurationChangeFlag, string lastOCCForBilling)
        {
            bool hasInstallation = false;
            bool bLockQuotationResult;
            string strLastOCC = "";
            CommonUtil util = new CommonUtil();
            //string strBillingClientCode = "";

            tbt_RentalContractBasic tbtRentalContractBasic;
            tbt_RentalSecurityBasic tbtRentalSecurityBasic;
            MaintenanceHandler maintenanceHandler;
            //List<string> listDistinctSequence;
            List<string> listBillingTempDistinctSequenceNo;

            //List<dtBillingClientData> listBillingClientFindAll;
            //List<dtBillingTempChangePlanData> listBillingTempFindAll;

            BillingInterfaceHandler billingInterfaceHandler;

            //QuotationHandler quotationHandler;
            IQuotationHandler quotationHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
            IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;
            IInstallationHandler installationhandler = ServiceContainer.GetService<IInstallationHandler>() as IInstallationHandler;
            IBillingMasterHandler billingmasterhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
            IBillingTempHandler billingtemphandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;

            doUpdateQuotationData doUpdateQuotation;
            bool IsUpdateOCC = false;

            try
            {
                var tmpnewItemList = CommonUtil.ClonsObjectList<dtBillingTemp_SetItem, dtBillingTemp_SetItem>(newItemList);
                var tmpupdateItemList = CommonUtil.ClonsObjectList<dtBillingTemp_SetItem, dtBillingTemp_SetItem>(updateItemList);

                using (TransactionScope scope = new TransactionScope())
                {
                    maintenanceHandler = new MaintenanceHandler();
                    billingInterfaceHandler = new BillingInterfaceHandler();
                    //quotationHandler = new QuotationHandler();

                    listBillingTempDistinctSequenceNo = new List<string>();

                    tbtRentalContractBasic = dsRentalContract.dtTbt_RentalContractBasic[0];
                    tbtRentalSecurityBasic = dsRentalContract.dtTbt_RentalSecurityBasic[0];

                    //0. Merge all billingtemp data
                    // Get all billingtemp
                    List<dtBillingTempChangePlanData> deleteList = new List<dtBillingTempChangePlanData>();
                    var allBillingTemp = this.GetBillingTempForChangePlan(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, lastOCCForBilling);
                    foreach (var newItem in tmpnewItemList)
                    {
                        // Create billing client code
                        if (String.IsNullOrEmpty(newItem.BillingClientCode))
                        {
                            tbm_BillingClient dummieClient = new tbm_BillingClient()
                            {
                                AddressEN = newItem.AddressEN,
                                AddressLC = newItem.AddressLC,
                                BillingClientCode = "",
                                BranchNameEN = newItem.BranchNameEN,
                                BranchNameLC = newItem.BranchNameLC,
                                BusinessTypeCode = newItem.BusinessTypeCode,
                                BusinessTypeName = newItem.BusinessType,
                                CustTypeCode = newItem.CustomerType,
                                FullNameEN = newItem.FullNameEN,
                                FullNameLC = newItem.FullNameLC,
                                IDNo = newItem.IDNo,
                                NameEN = newItem.FullNameEN,
                                NameLC = newItem.FullNameLC,
                                Nationality = newItem.Nationality,
                                RegionCode = newItem.RegionCode,
                                PhoneNo = newItem.PhoneNo,
                                CompanyTypeCode = newItem.CompanyTypeCode
                            };

                            var newClientCode = billingmasterhandler.ManageBillingClient(dummieClient);
                            newItem.BillingClientCode = newClientCode;
                        }

                        // Create billing temp and add to list
                        if ((dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                            ((dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (dsRentalContract.dtTbt_RentalContractBasic[0].StartType == StartType.C_START_TYPE_ALTER_START)))
                        {
                            // Contract fee is always use
                            dtBillingTempChangePlanData contractFee = new dtBillingTempChangePlanData()
                            {
                                //BillingAmt = (newItem.BillingContractFee.HasValue) ? newItem.BillingContractFee : 0,
                                
                                BillingClientCode = (newItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : newItem.BillingClientCode,
                                BillingCycle = null,
                                BillingOCC = null,
                                BillingOfficeCode = newItem.BillingOffice,
                                BillingTargetCode = util.ConvertBillingTargetCode(newItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                BillingTiming = null,
                                BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE,
                                ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                PayMethod = null,
                                SequenceNo = -1,
                                SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                            };

                            contractFee.BillingAmtCurrencyType = newItem.BillingContractFeeCurrencyType;
                            if (contractFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                contractFee.BillingAmtUsd = (newItem.BillingContractFee.HasValue) ? newItem.BillingContractFee : 0;
                            else
                                contractFee.BillingAmt = (newItem.BillingContractFee.HasValue) ? newItem.BillingContractFee : 0;

                            allBillingTemp.Add(contractFee);

                            if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                                || (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))
                            {
                                if (newItem.BillingApproveInstallationFee.HasValue)
                                {
                                    dtBillingTempChangePlanData installApproveFee = new dtBillingTempChangePlanData()
                                    {
                                        //BillingAmt = newItem.BillingApproveInstallationFee,

                                        BillingClientCode = (newItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : newItem.BillingClientCode,
                                        BillingCycle = null,
                                        BillingOCC = null,
                                        BillingOfficeCode = newItem.BillingOffice,
                                        BillingTargetCode = util.ConvertBillingTargetCode(newItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                        BillingTiming = BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT,
                                        // Phoomsak L. 2012-09-18 Add condition for setting installation fee (new or change)
                                        BillingType = (dsRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_ON) ? ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE : ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                        //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                        ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                        OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                        PayMethod = null,
                                        SequenceNo = -1,
                                        SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                    };

                                    installApproveFee.BillingAmtCurrencyType = newItem.BillingApproveInstallationFeeCurrencyType;
                                    if (installApproveFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        installApproveFee.BillingAmtUsd = newItem.BillingApproveInstallationFee;
                                    else
                                        installApproveFee.BillingAmt = newItem.BillingApproveInstallationFee;

                                    allBillingTemp.Add(installApproveFee);
                                }

                                if (newItem.BillingCompleteInstallationFee.HasValue)
                                {
                                    dtBillingTempChangePlanData installCompleteFee = new dtBillingTempChangePlanData()
                                    {
                                        //BillingAmt = newItem.BillingCompleteInstallationFee,

                                        BillingClientCode = (newItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : newItem.BillingClientCode,
                                        BillingCycle = null,
                                        BillingOCC = null,
                                        BillingOfficeCode = newItem.BillingOffice,
                                        BillingTargetCode = util.ConvertBillingTargetCode(newItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                        BillingTiming = BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION,
                                        // Phoomsak L. 2012-09-18 Add condition for setting installation fee (new or change)
                                        BillingType = (dsRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_ON) ? ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE : ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                        //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                        ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                        OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                        PayMethod = newItem.PaymentCompleteInstallationFee,
                                        SequenceNo = -1,
                                        SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                    };

                                    installCompleteFee.BillingAmtCurrencyType = newItem.BillingCompleteInstallationFeeCurrencyType;
                                    if (installCompleteFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        installCompleteFee.BillingAmtUsd = newItem.BillingCompleteInstallationFee;
                                    else
                                        installCompleteFee.BillingAmt = newItem.BillingCompleteInstallationFee;

                                    allBillingTemp.Add(installCompleteFee);
                                }

                                if (newItem.BillingStartInstallationFee.HasValue)
                                {
                                    dtBillingTempChangePlanData installStartServiceFee = new dtBillingTempChangePlanData()
                                    {
                                        //BillingAmt = newItem.BillingStartInstallationFee,

                                        BillingClientCode = (newItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : newItem.BillingClientCode,
                                        BillingCycle = null,
                                        BillingOCC = null,
                                        BillingOfficeCode = newItem.BillingOffice,
                                        BillingTargetCode = util.ConvertBillingTargetCode(newItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                        BillingTiming = BillingTiming.C_BILLING_TIMING_START_SERVICE,
                                        // Phoomsak L. 2012-09-18 Add condition for setting installation fee (new or change)
                                        BillingType = (dsRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_ON) ? ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE : ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                        //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                        ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                        OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                        PayMethod = newItem.PaymentStartInstallationFee,
                                        SequenceNo = -1,
                                        SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                    };

                                    installStartServiceFee.BillingAmtCurrencyType = newItem.BillingStartInstallationFeeCurrencyType;
                                    if (installStartServiceFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        installStartServiceFee.BillingAmtUsd = newItem.BillingStartInstallationFee;
                                    else
                                        installStartServiceFee.BillingAmt = newItem.BillingStartInstallationFee;

                                    allBillingTemp.Add(installStartServiceFee);
                                }

                                if (newItem.BillingDepositFee.HasValue)
                                {
                                    dtBillingTempChangePlanData depositFee = new dtBillingTempChangePlanData()
                                    {
                                        //BillingAmt = newItem.BillingDepositFee,

                                        BillingClientCode = (newItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : newItem.BillingClientCode,
                                        BillingCycle = null,
                                        BillingOCC = null,
                                        BillingOfficeCode = newItem.BillingOffice,
                                        BillingTargetCode = util.ConvertBillingTargetCode(newItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                        BillingTiming = dsRentalContract.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming,
                                        BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                                        ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                        OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                        PayMethod = newItem.PaymentDepositFee,
                                        SequenceNo = -1,
                                        SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                    };

                                    depositFee.BillingAmtCurrencyType = newItem.BillingDepositFeeCurrencyType;
                                    if (depositFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        depositFee.BillingAmtUsd = newItem.BillingDepositFee;
                                    else
                                        depositFee.BillingAmt = newItem.BillingDepositFee;

                                    allBillingTemp.Add(depositFee);
                                }
                            }
                            else if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                            {
                                if (newItem.BillingDepositFee.HasValue)
                                {
                                    dtBillingTempChangePlanData depositFee = new dtBillingTempChangePlanData()
                                    {
                                        //BillingAmt = newItem.BillingDepositFee,

                                        BillingClientCode = (newItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : newItem.BillingClientCode,
                                        BillingCycle = null,
                                        BillingOCC = null,
                                        BillingOfficeCode = newItem.BillingOffice,
                                        BillingTargetCode = util.ConvertBillingTargetCode(newItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                        BillingTiming = dsRentalContract.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming,
                                        BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                                        ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                        OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                        PayMethod = newItem.PaymentDepositFee,
                                        SequenceNo = -1,
                                        SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                    };

                                    depositFee.BillingAmtCurrencyType = newItem.BillingDepositFeeCurrencyType;
                                    if (depositFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        depositFee.BillingAmtUsd = newItem.BillingDepositFee;
                                    else
                                        depositFee.BillingAmt = newItem.BillingDepositFee;

                                    allBillingTemp.Add(depositFee);
                                }
                            }
                            else if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                            {

                            }
                            else if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                                || (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE))
                            {

                            }
                        }
                        else if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                            || dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING
                        )
                        {
                            // Contract fee is always use
                            dtBillingTempChangePlanData contractFee = new dtBillingTempChangePlanData()
                            {
                                //BillingAmt = (newItem.BillingContractFee.HasValue) ? newItem.BillingContractFee : 0,

                                //BillingClientCode = util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                BillingClientCode = (newItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : newItem.BillingClientCode,
                                BillingCycle = null,
                                BillingOCC = null,
                                BillingOfficeCode = newItem.BillingOffice,
                                BillingTargetCode = util.ConvertBillingTargetCode(newItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                BillingTiming = null,
                                BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE,
                                ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                PayMethod = null,
                                SequenceNo = -1,
                                SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                            };

                            contractFee.BillingAmtCurrencyType = newItem.BillingContractFeeCurrencyType;
                            if (contractFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                contractFee.BillingAmtUsd = (newItem.BillingContractFee.HasValue) ? newItem.BillingContractFee : 0;
                            else
                                contractFee.BillingAmt = (newItem.BillingContractFee.HasValue) ? newItem.BillingContractFee : 0;

                            allBillingTemp.Add(contractFee);

                            if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                                || (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))
                            {
                                if (newItem.BillingInstallationFee.HasValue)
                                {
                                    dtBillingTempChangePlanData installFee = new dtBillingTempChangePlanData()
                                    {
                                        //BillingAmt = newItem.BillingInstallationFee,

                                        //BillingClientCode = util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                        BillingClientCode = (newItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : newItem.BillingClientCode,
                                        BillingCycle = null,
                                        BillingOCC = null,
                                        BillingOfficeCode = newItem.BillingOffice,
                                        BillingTargetCode = util.ConvertBillingTargetCode(newItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                        BillingTiming = BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION,
                                        // Phoomsak L. 2012-09-18 Add condition for setting installation fee (new or change)
                                        BillingType = (dsRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_ON) ? ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE : ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                        //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                        ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                        OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                        PayMethod = newItem.PaymentInstallationFee,
                                        SequenceNo = -1,
                                        SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                    };

                                    installFee.BillingAmtCurrencyType = newItem.BillingInstallationFeeCurrencyType;
                                    if (installFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        installFee.BillingAmtUsd = newItem.BillingInstallationFee;
                                    else
                                        installFee.BillingAmt = newItem.BillingInstallationFee;

                                    allBillingTemp.Add(installFee);
                                }

                                if (newItem.BillingDepositFee.HasValue)
                                {
                                    dtBillingTempChangePlanData depositFee = new dtBillingTempChangePlanData()
                                    {
                                        //BillingAmt = newItem.BillingDepositFee,

                                        //BillingClientCode = util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                        BillingClientCode = (newItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : newItem.BillingClientCode,
                                        BillingCycle = null,
                                        BillingOCC = null,
                                        BillingOfficeCode = newItem.BillingOffice,
                                        BillingTargetCode = util.ConvertBillingTargetCode(newItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                        BillingTiming = dsRentalContract.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming,
                                        BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                                        ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                        OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                        PayMethod = newItem.PaymentDepositFee,
                                        SequenceNo = -1,
                                        SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                    };

                                    depositFee.BillingAmtCurrencyType = newItem.BillingDepositFeeCurrencyType;
                                    if (depositFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        depositFee.BillingAmtUsd = newItem.BillingDepositFee;
                                    else
                                        depositFee.BillingAmt = newItem.BillingDepositFee;

                                    allBillingTemp.Add(depositFee);
                                }
                            }
                            else if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                            {
                                if (newItem.BillingDepositFee.HasValue)
                                {
                                    dtBillingTempChangePlanData depositFee = new dtBillingTempChangePlanData()
                                    {
                                        //BillingAmt = newItem.BillingDepositFee,

                                        BillingClientCode = (newItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : newItem.BillingClientCode,
                                        BillingCycle = null,
                                        BillingOCC = null,
                                        BillingOfficeCode = newItem.BillingOffice,
                                        BillingTargetCode = util.ConvertBillingTargetCode(newItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                                        BillingTiming = dsRentalContract.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming,
                                        BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                                        ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                        OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                        PayMethod = newItem.PaymentDepositFee,
                                        SequenceNo = -1,
                                        SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                    };

                                    depositFee.BillingAmtCurrencyType = newItem.BillingDepositFeeCurrencyType;
                                    if (depositFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        depositFee.BillingAmtUsd = newItem.BillingDepositFee;
                                    else
                                        depositFee.BillingAmt = newItem.BillingDepositFee;

                                    allBillingTemp.Add(depositFee);
                                }
                            }
                            else if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                            {

                            }
                            else if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                                || (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE))
                            {

                            }
                        }
                    }

                    foreach (var updateItem in tmpupdateItemList)
                    {
                        //DateTime? keepCreatedDate = null;
                        //string keepCreatedBy = null;

                        // Create billing client code
                        if (String.IsNullOrEmpty(updateItem.BillingClientCode))
                        {
                            tbm_BillingClient dummieClient = new tbm_BillingClient()
                            {
                                AddressEN = updateItem.AddressEN,
                                AddressLC = updateItem.AddressLC,
                                BillingClientCode = "",
                                BranchNameEN = updateItem.BranchNameEN,
                                BranchNameLC = updateItem.BranchNameLC,
                                BusinessTypeCode = updateItem.BusinessTypeCode,
                                BusinessTypeName = updateItem.BusinessType,
                                CustTypeCode = updateItem.CustomerType,
                                FullNameEN = updateItem.FullNameEN,
                                FullNameLC = updateItem.FullNameLC,
                                IDNo = updateItem.IDNo,
                                NameEN = updateItem.FullNameEN,
                                NameLC = updateItem.FullNameLC,
                                Nationality = updateItem.Nationality,
                                RegionCode = updateItem.RegionCode,
                                PhoneNo = updateItem.PhoneNo,
                                CompanyTypeCode = updateItem.CompanyTypeCode
                            };

                            var newClientCode = billingmasterhandler.ManageBillingClient(dummieClient);
                            updateItem.BillingClientCode = newClientCode;
                        }

                        // Update billing temp to list
                        var targBillingItemLst = from a in allBillingTemp
                                                 where (a.BillingOCC == updateItem.BillingOCC)
                                                    && (a.BillingOfficeCode == updateItem.BillingOffice)
                                                    && (a.BillingClientCodeShort == updateItem.BillingClientCode)
                                                 select a;

                        var targBillingMasterItemLst = targBillingItemLst.ToList();

                        if (targBillingMasterItemLst.Count() > 0)
                        {
                            // Keep old created date&by
                            //var keepItem = targBillingMasterItemLst.First();
                            //keepCreatedBy = keepItem.CreateBy;
                            //keepCreatedDate = keepItem.CreateDate;

                            //// Remove all match item from list
                            foreach (var targBilling in targBillingMasterItemLst)
                            {
                                // add all item to delete list
                                bool canMod = true;
                                if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                                    || (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))
                                {
                                    // Phoomsak L. 2012-09-18 Add condition for setting installation fee (new or change)
                                    if ((targBilling.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || targBilling.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                                        //if ((targBilling.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE)
                                        && (targBilling.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT))
                                    {
                                        canMod = false;
                                    }
                                }

                                if (canMod == true && targBilling.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                                    canMod = false;

                                if (canMod)
                                {
                                    allBillingTemp.Remove(targBilling);
                                    deleteList.Add(targBilling);
                                }
                            }

                            // Set value from criteria
                            if ((dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                                ((dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (dsRentalContract.dtTbt_RentalContractBasic[0].StartType == StartType.C_START_TYPE_ALTER_START)))
                            {
                                #region ContractFee
                                if (!String.IsNullOrEmpty(updateItem.BillingOCC))
                                {
                                    var modBillingTemp = targBillingMasterItemLst.Where(x => ((x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                                        || (x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON)
                                        || (x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE)));
                                    if (modBillingTemp.Count() == 1)
                                    {
                                        var modBillingItem = modBillingTemp.First();

                                        // Remove from delete item list
                                        deleteList.Remove(modBillingItem);

                                        //modBillingItem.BillingAmt = updateItem.BillingContractFee.GetValueOrDefault();

                                        modBillingItem.BillingAmtCurrencyType = updateItem.BillingContractFeeCurrencyType;
                                        if (modBillingItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        {
                                            modBillingItem.BillingAmt = null;
                                            modBillingItem.BillingAmtUsd = updateItem.BillingContractFee.GetValueOrDefault();                                            
                                        }
                                        else
                                        {
                                            modBillingItem.BillingAmt = updateItem.BillingContractFee.GetValueOrDefault();
                                            modBillingItem.BillingAmtUsd = null;
                                        }

                                        if (String.IsNullOrEmpty(modBillingItem.SendFlag))
                                        {
                                            modBillingItem.SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP;
                                        }
                                        allBillingTemp.Add(modBillingItem);
                                    }
                                    else
                                    {
                                        dtBillingTempChangePlanData contractFee = new dtBillingTempChangePlanData()
                                        {
                                            //BillingAmt = updateItem.BillingContractFee.GetValueOrDefault(),

                                            BillingClientCode = (updateItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(updateItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingClientCode,
                                            BillingCycle = null,
                                            BillingOCC = updateItem.BillingOCC,
                                            BillingOfficeCode = updateItem.BillingOffice,
                                            BillingTargetCode = (updateItem.BillingTargetCode.Length == CommonValue.BILLING_TARGET_CODE_SHORT_DIGIT) ? util.ConvertBillingTargetCode(updateItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingTargetCode,
                                            BillingTiming = null,
                                            BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE,
                                            ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                            OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                            PayMethod = null,
                                            SequenceNo = -1,
                                            SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP,
                                        };

                                        contractFee.BillingAmtCurrencyType = updateItem.BillingContractFeeCurrencyType;
                                        if (contractFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            contractFee.BillingAmtUsd = updateItem.BillingContractFee.GetValueOrDefault();
                                        else
                                            contractFee.BillingAmt = updateItem.BillingContractFee.GetValueOrDefault();

                                        allBillingTemp.Add(contractFee);
                                    }
                                }
                                #endregion

                                #region AL / RENTAL

                                if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                                    || (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))
                                {
                                    #region Install-Approve Fee

                                    //if (updateItem.BillingApproveInstallationFee.HasValue && (updateItem.BillingApproveInstallationFee.Value > 0))
                                    //{
                                    //    var modBillingTemp = targBillingMasterItemLst.Where(x => x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE && x.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT);
                                    //    if (modBillingTemp.Count() == 1)
                                    //    {
                                    //        var modBillingItem = modBillingTemp.First();

                                    //        // Remove from delete item list
                                    //        deleteList.Remove(modBillingItem);

                                    //        modBillingItem.BillingAmt = updateItem.BillingApproveInstallationFee;
                                    //        modBillingItem.PayMethod = null;
                                    //        modBillingItem.SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP;
                                    //        allBillingTemp.Add(modBillingItem);
                                    //    }
                                    //    else
                                    //    {
                                    //        dtBillingTempChangePlanData billingTemp = new dtBillingTempChangePlanData()
                                    //        {
                                    //            BillingAmt = updateItem.BillingApproveInstallationFee,
                                    //            BillingClientCode = (updateItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(updateItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingClientCode,
                                    //            BillingCycle = null,
                                    //            BillingOCC = updateItem.BillingOCC,
                                    //            BillingOfficeCode = updateItem.BillingOffice,
                                    //            BillingTargetCode = (updateItem.BillingTargetCode.Length == CommonValue.BILLING_TARGET_CODE_SHORT_DIGIT) ? util.ConvertBillingTargetCode(updateItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingTargetCode,
                                    //            BillingTiming = BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT,
                                    //            BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                    //            ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                    //            OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                    //            PayMethod = null,
                                    //            SequenceNo = -1,
                                    //            SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP,
                                    //        };
                                    //        allBillingTemp.Add(billingTemp);
                                    //    }
                                    //}

                                    #endregion

                                    #region Install-Complete Fee

                                    if (updateItem.BillingCompleteInstallationFee.HasValue && (updateItem.BillingCompleteInstallationFee.Value > 0))
                                    {
                                        var modBillingTemp = targBillingMasterItemLst.Where(x => (x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE) && x.BillingTiming == BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION);
                                        if (modBillingTemp.Count() == 1)
                                        {
                                            var modBillingItem = modBillingTemp.First();

                                            // Remove from delete item list
                                            deleteList.Remove(modBillingItem);

                                            //modBillingItem.BillingAmt = updateItem.BillingCompleteInstallationFee;

                                            modBillingItem.BillingAmtCurrencyType = updateItem.BillingCompleteInstallationFeeCurrencyType;
                                            if (modBillingItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            {
                                                modBillingItem.BillingAmt = null;
                                                modBillingItem.BillingAmtUsd = updateItem.BillingCompleteInstallationFee;
                                            }
                                            else
                                            {
                                                modBillingItem.BillingAmt = updateItem.BillingCompleteInstallationFee;
                                                modBillingItem.BillingAmtUsd = null;
                                            }

                                            modBillingItem.PayMethod = updateItem.PaymentCompleteInstallationFee;
                                            if (String.IsNullOrEmpty(modBillingItem.SendFlag))
                                            {
                                                modBillingItem.SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP;
                                            }
                                            allBillingTemp.Add(modBillingItem);
                                        }
                                        else
                                        {
                                            dtBillingTempChangePlanData billingTemp = new dtBillingTempChangePlanData()
                                            {
                                                //BillingAmt = updateItem.BillingCompleteInstallationFee,

                                                BillingClientCode = (updateItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(updateItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingClientCode,
                                                BillingCycle = null,
                                                BillingOCC = updateItem.BillingOCC,
                                                BillingOfficeCode = updateItem.BillingOffice,
                                                BillingTargetCode = (updateItem.BillingTargetCode.Length == CommonValue.BILLING_TARGET_CODE_SHORT_DIGIT) ? util.ConvertBillingTargetCode(updateItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingTargetCode,
                                                BillingTiming = BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION,
                                                // Phoomsak L. 2012-09-18 Add condition for setting installation fee (new or change)
                                                BillingType = (dsRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_ON) ? ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE : ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                                //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                                ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                                OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                                PayMethod = updateItem.PaymentCompleteInstallationFee,
                                                SequenceNo = -1,
                                                SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP,
                                            };

                                            billingTemp.BillingAmtCurrencyType = updateItem.BillingCompleteInstallationFeeCurrencyType;
                                            if (billingTemp.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                                billingTemp.BillingAmtUsd = updateItem.BillingCompleteInstallationFee;
                                            else
                                                billingTemp.BillingAmt = updateItem.BillingCompleteInstallationFee;

                                            allBillingTemp.Add(billingTemp);
                                        }
                                    }

                                    #endregion

                                    #region Install-StartService Fee

                                    if (updateItem.BillingStartInstallationFee.HasValue && (updateItem.BillingStartInstallationFee.Value > 0))
                                    {
                                        var modBillingTemp = targBillingMasterItemLst.Where(x => (x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE) && x.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE);
                                        if (modBillingTemp.Count() == 1)
                                        {
                                            var modBillingItem = modBillingTemp.First();

                                            // Remove from delete item list
                                            deleteList.Remove(modBillingItem);

                                            //modBillingItem.BillingAmt = updateItem.BillingStartInstallationFee;

                                            modBillingItem.BillingAmtCurrencyType = updateItem.BillingStartInstallationFeeCurrencyType;
                                            if (modBillingItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            {
                                                modBillingItem.BillingAmt = null;
                                                modBillingItem.BillingAmtUsd = updateItem.BillingStartInstallationFee;
                                            }
                                            else
                                            {
                                                modBillingItem.BillingAmt = updateItem.BillingStartInstallationFee;
                                                modBillingItem.BillingAmtUsd = null;
                                            }

                                            modBillingItem.PayMethod = updateItem.PaymentStartInstallationFee;
                                            if (String.IsNullOrEmpty(modBillingItem.SendFlag))
                                            {
                                                modBillingItem.SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP;
                                            }
                                            allBillingTemp.Add(modBillingItem);
                                        }
                                        else
                                        {
                                            dtBillingTempChangePlanData billingTemp = new dtBillingTempChangePlanData()
                                            {
                                                //BillingAmt = updateItem.BillingStartInstallationFee,

                                                BillingClientCode = (updateItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(updateItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingClientCode,
                                                BillingCycle = null,
                                                BillingOCC = updateItem.BillingOCC,
                                                BillingOfficeCode = updateItem.BillingOffice,
                                                BillingTargetCode = (updateItem.BillingTargetCode.Length == CommonValue.BILLING_TARGET_CODE_SHORT_DIGIT) ? util.ConvertBillingTargetCode(updateItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingTargetCode,
                                                BillingTiming = BillingTiming.C_BILLING_TIMING_START_SERVICE,
                                                // Phoomsak L. 2012-09-18 Add condition for setting installation fee (new or change)
                                                BillingType = (dsRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_ON) ? ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE : ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                                //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                                ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                                OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                                PayMethod = updateItem.PaymentStartInstallationFee,
                                                SequenceNo = -1,
                                                SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP,
                                            };

                                            billingTemp.BillingAmtCurrencyType = updateItem.BillingStartInstallationFeeCurrencyType;
                                            if (billingTemp.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                                billingTemp.BillingAmtUsd = updateItem.BillingStartInstallationFee;
                                            else
                                                billingTemp.BillingAmt = updateItem.BillingStartInstallationFee;

                                            allBillingTemp.Add(billingTemp);
                                        }
                                    }
                                    #endregion

                                    #region Deposit Fee

                                    if (updateItem.BillingDepositFee.HasValue && (updateItem.BillingDepositFee.Value > 0))
                                    {
                                        var modBillingTemp = targBillingMasterItemLst.Where(x => x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE);
                                        if (modBillingTemp.Count() == 1)
                                        {
                                            var modBillingItem = modBillingTemp.First();

                                            // Remove from delete item list
                                            deleteList.Remove(modBillingItem);

                                            //modBillingItem.BillingAmt = updateItem.BillingDepositFee;

                                            modBillingItem.BillingAmtCurrencyType = updateItem.BillingDepositFeeCurrencyType;
                                            if (modBillingItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            {
                                                modBillingItem.BillingAmt = null;
                                                modBillingItem.BillingAmtUsd = updateItem.BillingDepositFee;
                                            }
                                            else
                                            {
                                                modBillingItem.BillingAmt = updateItem.BillingDepositFee;
                                                modBillingItem.BillingAmtUsd = null;
                                            }

                                            modBillingItem.PayMethod = updateItem.PaymentDepositFee;
                                            if (String.IsNullOrEmpty(modBillingItem.SendFlag))
                                            {
                                                modBillingItem.SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP;
                                            }
                                            allBillingTemp.Add(modBillingItem);
                                        }
                                        else
                                        {
                                            dtBillingTempChangePlanData depositFee = new dtBillingTempChangePlanData()
                                            {
                                                //BillingAmt = updateItem.BillingDepositFee,

                                                BillingClientCode = (updateItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(updateItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingClientCode,
                                                BillingCycle = null,
                                                BillingOCC = updateItem.BillingOCC,
                                                BillingOfficeCode = updateItem.BillingOffice,
                                                BillingTargetCode = (updateItem.BillingTargetCode.Length == CommonValue.BILLING_TARGET_CODE_SHORT_DIGIT) ? util.ConvertBillingTargetCode(updateItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingTargetCode,
                                                BillingTiming = dsRentalContract.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming,
                                                BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                                                ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                                OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                                PayMethod = updateItem.PaymentDepositFee,
                                                SequenceNo = -1,
                                                SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP,
                                            };

                                            depositFee.BillingAmtCurrencyType = updateItem.BillingDepositFeeCurrencyType;
                                            if (depositFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                                depositFee.BillingAmtUsd = updateItem.BillingDepositFee;
                                            else
                                                depositFee.BillingAmt = updateItem.BillingDepositFee;

                                            allBillingTemp.Add(depositFee);
                                        }
                                    }
                                    #endregion
                                }
                                #endregion

                                #region Online
                                else if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE ||
                                            dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA ||
                                            dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE ||
                                            dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                                {
                                    #region Deposit Fee

                                    if (updateItem.BillingDepositFee.HasValue && (updateItem.BillingDepositFee.Value > 0))
                                    {
                                        var modBillingTemp = targBillingMasterItemLst.Where(x => x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE);
                                        if (modBillingTemp.Count() == 1)
                                        {
                                            var modBillingItem = modBillingTemp.First();

                                            // Remove from delete item list
                                            deleteList.Remove(modBillingItem);

                                            //modBillingItem.BillingAmt = updateItem.BillingDepositFee;

                                            modBillingItem.BillingAmtCurrencyType = updateItem.BillingDepositFeeCurrencyType;
                                            if (modBillingItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            {
                                                modBillingItem.BillingAmt = null;
                                                modBillingItem.BillingAmtUsd = updateItem.BillingDepositFee;
                                            }
                                            else
                                            {
                                                modBillingItem.BillingAmt = updateItem.BillingDepositFee;
                                                modBillingItem.BillingAmtUsd = null;
                                            }

                                            modBillingItem.PayMethod = updateItem.PaymentDepositFee;
                                            if (String.IsNullOrEmpty(modBillingItem.SendFlag))
                                            {
                                                modBillingItem.SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP;
                                            }
                                            allBillingTemp.Add(modBillingItem);
                                        }
                                        else
                                        {
                                            dtBillingTempChangePlanData depositFee = new dtBillingTempChangePlanData()
                                            {
                                                //BillingAmt = updateItem.BillingDepositFee,

                                                BillingClientCode = (updateItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(updateItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingClientCode,
                                                BillingCycle = null,
                                                BillingOCC = updateItem.BillingOCC,
                                                BillingOfficeCode = updateItem.BillingOffice,
                                                BillingTargetCode = (updateItem.BillingTargetCode.Length == CommonValue.BILLING_TARGET_CODE_SHORT_DIGIT) ? util.ConvertBillingTargetCode(updateItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingTargetCode,
                                                BillingTiming = dsRentalContract.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming,
                                                BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                                                ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                                OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                                PayMethod = updateItem.PaymentDepositFee,
                                                SequenceNo = -1,
                                                SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP,
                                            };

                                            depositFee.BillingAmtCurrencyType = updateItem.BillingDepositFeeCurrencyType;
                                            if (depositFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                                depositFee.BillingAmtUsd = updateItem.BillingDepositFee;
                                            else
                                                depositFee.BillingAmt = updateItem.BillingDepositFee;

                                            allBillingTemp.Add(depositFee);
                                        }
                                    }

                                    #endregion
                                }

                                #endregion
                            }
                            else if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                                || dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING
                            )
                            {
                                if ((!String.IsNullOrEmpty(updateItem.BillingOCC)))
                                {
                                    var modBillingTemp = targBillingMasterItemLst.Where(x => ((x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                                        || (x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON)
                                        || (x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE)));
                                    if (modBillingTemp.Count() == 1)
                                    {
                                        var modBillingItem = modBillingTemp.First();

                                        // Remove from delete item list
                                        deleteList.Remove(modBillingItem);

                                        //modBillingItem.BillingAmt = updateItem.BillingContractFee.GetValueOrDefault();

                                        modBillingItem.BillingAmtCurrencyType = updateItem.BillingContractFeeCurrencyType;
                                        if (modBillingItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                        {
                                            modBillingItem.BillingAmt = null;
                                            modBillingItem.BillingAmtUsd = updateItem.BillingContractFee;
                                        }
                                        else
                                        {
                                            modBillingItem.BillingAmt = updateItem.BillingContractFee;
                                            modBillingItem.BillingAmtUsd = null;
                                        }

                                        if (String.IsNullOrEmpty(modBillingItem.SendFlag))
                                        {
                                            modBillingItem.SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP;
                                        }
                                        allBillingTemp.Add(modBillingItem);
                                    }
                                    else
                                    {
                                        dtBillingTempChangePlanData contractFee = new dtBillingTempChangePlanData()
                                        {
                                            //BillingAmt = updateItem.BillingContractFee.GetValueOrDefault(),

                                            BillingClientCode = (updateItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(updateItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingClientCode,
                                            BillingCycle = null,
                                            BillingOCC = updateItem.BillingOCC,
                                            BillingOfficeCode = updateItem.BillingOffice,
                                            BillingTargetCode = (updateItem.BillingTargetCode.Length == CommonValue.BILLING_TARGET_CODE_SHORT_DIGIT) ? util.ConvertBillingTargetCode(updateItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingTargetCode,
                                            BillingTiming = null,
                                            BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE,
                                            ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                            OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                            PayMethod = null,
                                            SequenceNo = -1,
                                            SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                        };

                                        contractFee.BillingAmtCurrencyType = updateItem.BillingContractFeeCurrencyType;
                                        if (contractFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            contractFee.BillingAmtUsd = updateItem.BillingContractFee.GetValueOrDefault();
                                        else
                                            contractFee.BillingAmt = updateItem.BillingContractFee.GetValueOrDefault();

                                        allBillingTemp.Add(contractFee);
                                    }
                                }

                                if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                                    || (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))
                                {
                                    if (updateItem.BillingInstallationFee.HasValue && (updateItem.BillingInstallationFee.Value > 0))
                                    {
                                        var modBillingTemp = targBillingMasterItemLst.Where(x => (x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE));
                                        if (modBillingTemp.Count() == 1)
                                        {
                                            var modBillingItem = modBillingTemp.First();

                                            // Remove from delete item list
                                            deleteList.Remove(modBillingItem);

                                            //modBillingItem.BillingAmt = updateItem.BillingInstallationFee;

                                            modBillingItem.BillingAmtCurrencyType = updateItem.BillingInstallationFeeCurrencyType;
                                            if (modBillingItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            {
                                                modBillingItem.BillingAmt = null;
                                                modBillingItem.BillingAmtUsd = updateItem.BillingInstallationFee;
                                            }
                                            else
                                            {
                                                modBillingItem.BillingAmt = updateItem.BillingInstallationFee;
                                                modBillingItem.BillingAmtUsd = null;
                                            }

                                            modBillingItem.PayMethod = updateItem.PaymentInstallationFee;
                                            if (String.IsNullOrEmpty(modBillingItem.SendFlag))
                                            {
                                                modBillingItem.SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP;
                                            }
                                            allBillingTemp.Add(modBillingItem);
                                        }
                                        else
                                        {
                                            string tmpBillingTargetCode = null;
                                            string tmpBillingClientCode = null;
                                            if (!String.IsNullOrEmpty(updateItem.BillingTargetCode))
                                            {
                                                tmpBillingTargetCode = (updateItem.BillingTargetCode.Length == CommonValue.BILLING_TARGET_CODE_SHORT_DIGIT) ? util.ConvertBillingTargetCode(updateItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingTargetCode;
                                            }
                                            if (!String.IsNullOrEmpty(updateItem.BillingClientCode))
                                            {
                                                tmpBillingClientCode = (updateItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(updateItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingClientCode;
                                            }

                                            dtBillingTempChangePlanData installFee = new dtBillingTempChangePlanData()
                                            {
                                                //BillingAmt = updateItem.BillingInstallationFee,

                                                BillingClientCode = tmpBillingClientCode,
                                                BillingCycle = null,
                                                BillingOCC = updateItem.BillingOCC,
                                                BillingOfficeCode = updateItem.BillingOffice,
                                                BillingTargetCode = tmpBillingTargetCode,
                                                BillingTiming = BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION,
                                                // Phoomsak L. 2012-09-18 Add condition for setting installation fee (new or change)
                                                BillingType = (dsRentalContract.dtTbt_RentalContractBasic[0].FirstInstallCompleteFlag == FlagType.C_FLAG_ON) ? ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE : ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                                //BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE,
                                                ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                                OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                                PayMethod = updateItem.PaymentInstallationFee,
                                                SequenceNo = -1,
                                                SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                            };

                                            installFee.BillingAmtCurrencyType = updateItem.BillingInstallationFeeCurrencyType;
                                            if (installFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                                installFee.BillingAmtUsd = updateItem.BillingInstallationFee;
                                            else
                                                installFee.BillingAmt = updateItem.BillingInstallationFee;

                                            allBillingTemp.Add(installFee);
                                        }
                                    }

                                    if (updateItem.BillingDepositFee.HasValue && (updateItem.BillingDepositFee.Value > 0))
                                    {
                                        var modBillingTemp = targBillingMasterItemLst.Where(x => x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE);
                                        if (modBillingTemp.Count() == 1)
                                        {
                                            var modBillingItem = modBillingTemp.First();

                                            // Remove from delete item list
                                            deleteList.Remove(modBillingItem);

                                            //modBillingItem.BillingAmt = updateItem.BillingDepositFee;

                                            modBillingItem.BillingAmtCurrencyType = updateItem.BillingDepositFeeCurrencyType;
                                            if (modBillingItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            {
                                                modBillingItem.BillingAmt = null;
                                                modBillingItem.BillingAmtUsd = updateItem.BillingDepositFee;
                                            }
                                            else
                                            {
                                                modBillingItem.BillingAmt = updateItem.BillingDepositFee;
                                                modBillingItem.BillingAmtUsd = null;
                                            }

                                            modBillingItem.PayMethod = updateItem.PaymentDepositFee;
                                            if (String.IsNullOrEmpty(modBillingItem.SendFlag))
                                            {
                                                modBillingItem.SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP;
                                            }
                                            allBillingTemp.Add(modBillingItem);
                                        }
                                        else
                                        {
                                            dtBillingTempChangePlanData depositFee = new dtBillingTempChangePlanData()
                                            {
                                                //BillingAmt = updateItem.BillingDepositFee,

                                                BillingClientCode = (updateItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(updateItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingClientCode,
                                                BillingCycle = null,
                                                BillingOCC = updateItem.BillingOCC,
                                                BillingOfficeCode = updateItem.BillingOffice,
                                                BillingTargetCode = (updateItem.BillingTargetCode.Length == CommonValue.BILLING_TARGET_CODE_SHORT_DIGIT) ? util.ConvertBillingTargetCode(updateItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingTargetCode,
                                                BillingTiming = dsRentalContract.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming,
                                                BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                                                ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                                OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                                PayMethod = updateItem.PaymentDepositFee,
                                                SequenceNo = -1,
                                                SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                            };

                                            depositFee.BillingAmtCurrencyType = updateItem.BillingDepositFeeCurrencyType;
                                            if (depositFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                                depositFee.BillingAmtUsd = updateItem.BillingDepositFee;
                                            else
                                                depositFee.BillingAmt = updateItem.BillingDepositFee;

                                            allBillingTemp.Add(depositFee);
                                        }
                                    }
                                }
                                else if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE ||
                                            dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA ||
                                            dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE ||
                                            dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                                {
                                    if (updateItem.BillingDepositFee.HasValue && (updateItem.BillingDepositFee.Value > 0))
                                    {
                                        var modBillingTemp = targBillingMasterItemLst.Where(x => x.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE);
                                        if (modBillingTemp.Count() == 1)
                                        {
                                            var modBillingItem = modBillingTemp.First();

                                            // Remove from delete item list
                                            deleteList.Remove(modBillingItem);

                                            //modBillingItem.BillingAmt = updateItem.BillingDepositFee;

                                            modBillingItem.BillingAmtCurrencyType = updateItem.BillingDepositFeeCurrencyType;
                                            if (modBillingItem.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                            {
                                                modBillingItem.BillingAmt = null;
                                                modBillingItem.BillingAmtUsd = updateItem.BillingDepositFee;
                                            }
                                            else
                                            {
                                                modBillingItem.BillingAmt = updateItem.BillingDepositFee;
                                                modBillingItem.BillingAmtUsd = null;
                                            }

                                            modBillingItem.PayMethod = updateItem.PaymentDepositFee;
                                            if (String.IsNullOrEmpty(modBillingItem.SendFlag))
                                            {
                                                modBillingItem.SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP;
                                            }
                                            allBillingTemp.Add(modBillingItem);
                                        }
                                        else
                                        {
                                            dtBillingTempChangePlanData depositFee = new dtBillingTempChangePlanData()
                                            {
                                                //BillingAmt = updateItem.BillingDepositFee,

                                                BillingAmtCurrencyType = updateItem.BillingDepositFeeCurrencyType,
                                                BillingClientCode = (updateItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(updateItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingClientCode,
                                                BillingCycle = null,
                                                BillingOCC = updateItem.BillingOCC,
                                                BillingOfficeCode = updateItem.BillingOffice,
                                                BillingTargetCode = (updateItem.BillingTargetCode.Length == CommonValue.BILLING_TARGET_CODE_SHORT_DIGIT) ? util.ConvertBillingTargetCode(updateItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG) : updateItem.BillingTargetCode,
                                                BillingTiming = dsRentalContract.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming,
                                                BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE,
                                                ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                                                OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,
                                                PayMethod = updateItem.PaymentDepositFee,
                                                SequenceNo = -1,
                                                SendFlag = BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                                            };

                                            depositFee.BillingAmtCurrencyType = updateItem.BillingDepositFeeCurrencyType;
                                            if (depositFee.BillingAmtCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                                                depositFee.BillingAmtUsd = updateItem.BillingDepositFee;
                                            else
                                                depositFee.BillingAmt = updateItem.BillingDepositFee;

                                            allBillingTemp.Add(depositFee);
                                        }
                                    }
                                }
                            }

                            //foreach (var targBilling in targBillingItemLst)
                            //{
                            //    allBillingTemp.Remove(targBilling);

                            //    if (targBilling.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                            //    {
                            //        targBilling.BillingAmt = updateItem.BillingContractFee;
                            //    }
                            //    else if (targBilling.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                            //    {
                            //        if (targBilling.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                            //        {
                            //            targBilling.BillingAmt = updateItem.BillingApproveInstallationFee;
                            //        }
                            //        else if (targBilling.BillingTiming == BillingTiming.C_BILLING_TIMING_COMPLETE_INSTALLATION)
                            //        {
                            //            if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                            //            {
                            //                targBilling.BillingAmt = updateItem.BillingCompleteInstallationFee;
                            //                targBilling.PayMethod = updateItem.PaymentCompleteInstallationFee;
                            //            }
                            //            else if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START)
                            //            {
                            //                targBilling.BillingAmt = updateItem.BillingInstallationFee;
                            //                targBilling.PayMethod = updateItem.PaymentInstallationFee;
                            //            }
                            //        }
                            //        else if (targBilling.BillingTiming == BillingTiming.C_BILLING_TIMING_START_SERVICE)
                            //        {
                            //            targBilling.BillingAmt = updateItem.BillingStartInstallationFee;
                            //            targBilling.PayMethod = updateItem.PaymentStartInstallationFee;
                            //        }
                            //    }
                            //    else if (targBilling.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE)
                            //    {
                            //        targBilling.BillingAmt = updateItem.BillingDepositFee;
                            //        targBilling.PayMethod = updateItem.PaymentDepositFee;
                            //    }

                            //    allBillingTemp.Add(targBilling);
                            //}
                        }
                    }

                    foreach (var deleteItem in deleteItemList)
                    {
                        // Delete billing temp from list
                        var targBillingItemTmp = from a in allBillingTemp
                                                 where (a.BillingOCC == deleteItem.BillingOCC)
                                                    && (a.BillingOfficeCode == deleteItem.BillingOfficeCode)
                                                    && (a.BillingClientCodeShort == deleteItem.BillingClientCode)
                                                 select a;

                        var targBillingMasterItem = targBillingItemTmp.ToList();
                        //var targBillingRunItem = CommonUtil.ClonsObjectList<dtBillingTempChangePlanData, dtBillingTempChangePlanData>(targBillingMasterItem);

                        if (targBillingMasterItem.Count() > 0)
                        {
                            foreach (var targBilling in targBillingMasterItem)
                            {
                                deleteList.Add(targBilling);
                                allBillingTemp.Remove(targBilling);
                            }
                        }
                    }


                    //1. Update data for changing
                    DateTime currDate = DateTime.Now;

                    //1.1
                    tbtRentalContractBasic.ChangePlanProcessDate = currDate;

                    //1.2
                    if (tbtRentalContractBasic.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                    {
                        tbtRentalSecurityBasic.ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP;
                    }
                    else
                    {
                        tbtRentalSecurityBasic.ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE;
                    }

                    // Modify 2012-08-30 Phoomsak L. remove condition and set ChangeImplementDate = ExpectedOperationDate on screen always
                    tbtRentalSecurityBasic.ChangeImplementDate = tbtRentalSecurityBasic.ExpectedOperationDate;

                    //if (tbtRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_AL || tbtRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                    //{
                    //    if ((tbtRentalContractBasic.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                    //        ((tbtRentalContractBasic.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (tbtRentalContractBasic.StartType == StartType.C_START_TYPE_ALTER_START)))
                    //    {
                    //        tbtRentalSecurityBasic.ChangeImplementDate = tbtRentalSecurityBasic.ExpectedOperationDate;
                    //    }
                    //    else if (tbtRentalContractBasic.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                    //        || tbtRentalContractBasic.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                    //    {
                    //        tbtRentalSecurityBasic.ChangeImplementDate = tbtRentalSecurityBasic.ExpectedInstallationCompleteDate;
                    //    }
                    //}
                    //else
                    //{
                    //    tbtRentalSecurityBasic.ChangeImplementDate = tbtRentalSecurityBasic.ExpectedOperationDate;
                    //}

                    tbtRentalSecurityBasic.QuotationTargetCode = dsQuotation.dtTbt_QuotationBasic.QuotationTargetCode;
                    tbtRentalSecurityBasic.QuotationAlphabet = dsQuotation.dtTbt_QuotationBasic.Alphabet;

                    //2. In case of dtTbt_RentalContractBasic.ProductTypeCode = C_PROD_TYPE_AL
                    if (tbtRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_AL || tbtRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                    {

                        if ((tbtRentalContractBasic.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                                || tbtRentalContractBasic.ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                            && tbtRentalContractBasic.StartType != StartType.C_START_TYPE_ALTER_START)
                        {
                            strLastOCC = GetLastUnimplementedOCC(tbtRentalContractBasic.ContractCode);
                            if (strLastOCC != null)
                            {
                                UpdateContract(ref dsRentalContract);
                                allBillingTemp = UpdateBillingOCCToBillingTemp(dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC, allBillingTemp);
                                hasInstallation = true;
                                IsUpdateOCC = false;
                            }
                            else
                            {
                                //--- Add by Phoomsak L. 2012-09-03 for clear data before insert new OCC
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag = FlagType.C_FLAG_OFF;
                                //dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationSlipNo = null;
                                //dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteDate = null;
                                //dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteEmpNo = null;
                                //dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationTypeCode = null;

                                InsertContract(ref dsRentalContract, ref allBillingTemp, false);
                                hasInstallation = true;
                                IsUpdateOCC = true;
                            }
                        }
                        else if (((tbtRentalContractBasic.ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                          || ((tbtRentalContractBasic.ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (tbtRentalContractBasic.StartType == StartType.C_START_TYPE_ALTER_START)))
                          || (tbtRentalContractBasic.StartType == StartType.C_START_TYPE_ALTER_START))
                        {
                            //UpdateSummaryFields(ref dsRentalContract);

                            if (dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_OFF)
                            {
                                UpdateContract(ref dsRentalContract);
                                allBillingTemp = UpdateBillingOCCToBillingTemp(dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC, allBillingTemp);
                                hasInstallation = true;
                                IsUpdateOCC = false;
                            }

                            if (dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_ON)
                            {
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].ImplementFlag = Convert.ToBoolean(RentalImplementType.C_RENTAL_IMPLEMENT_FLAG_A);
                                //--- Add by Phoomsak L. 2012-09-03 for clear data before insert new OCC
                                dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag = FlagType.C_FLAG_OFF;
                                //dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationSlipNo = null;
                                //dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteDate = null;
                                //dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteEmpNo = null;
                                //dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationTypeCode = null;

                                InsertContract(ref dsRentalContract, ref allBillingTemp, false);
                                IsUpdateOCC = true;
                                hasInstallation = true;
                            }

                            if (dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag == FlagType.C_FLAG_OFF)
                            {
                                //invenhandler.BookingProcess(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, strNewOcc);
                            }
                        }
                    }

                    //3.
                    if (tbtRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE ||
                        tbtRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_BE ||
                        tbtRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_SG ||
                        tbtRentalContractBasic.ProductTypeCode == ProductType.C_PROD_TYPE_MA
                        )
                    {
                        //UpdateSummaryFields(ref dsRentalContract);

                        if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                            || dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING
                        )
                        {
                            //// Phoomsak L. 2012-08-24 set deposit fee
                            //dsRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFee = dsRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFee + dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFee;
                            //dsRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFee = dsRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFee + dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee;

                            dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag = null;

                            dsRentalContract.dtTbt_RentalSecurityBasic[0].ImplementFlag = Convert.ToBoolean(RentalImplementType.C_RENTAL_IMPLEMENT_FLAG_B);
                            InsertContract(ref dsRentalContract, ref allBillingTemp, true);
                            hasInstallation = false;
                            IsUpdateOCC = true;

                            //if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA
                            //    //&& contractDurationChangeFlag == FlagType.C_FLAG_ON
                            //)
                            if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA
                                || dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL
                                || dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                                && contractDurationChangeFlag == FlagType.C_FLAG_ON) //Modify by Jutarat A. on 11042013
                                maintenanceHandler.GenerateMaintenanceSchedule(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, GenerateMAProcessType.C_GEN_MA_TYPE_RE_CREATE);
                        }

                        if ((dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                            ((dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (dsRentalContract.dtTbt_RentalContractBasic[0].StartType == StartType.C_START_TYPE_ALTER_START)))
                        {
                            //// Phoomsak L. 2012-08-24 set deposit fee
                            //dsRentalContract.dtTbt_RentalContractBasic[0].NormalDepositFee = dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFee;
                            //dsRentalContract.dtTbt_RentalContractBasic[0].OrderDepositFee = dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee;

                            dsRentalContract.dtTbt_RentalSecurityBasic[0].ImplementFlag = Convert.ToBoolean(RentalImplementType.C_RENTAL_IMPLEMENT_FLAG_A);
                            //InsertContract(ref dsRentalContract, ref allBillingTemp, false);
                            UpdateContract(ref dsRentalContract);
                            allBillingTemp = UpdateBillingOCCToBillingTemp(dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC, allBillingTemp);
                            IsUpdateOCC = false;
                            hasInstallation = false;
                        }
                    }

                    // 5. Update billing temp
                    foreach (var delItem in deleteList)
                    {
                        billingtemphandler.DeleteBillingTempByKey(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode
                            , dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC
                            , delItem.SequenceNo);
                    }

                    foreach (var modItem in allBillingTemp)
                    {
                        if (String.IsNullOrEmpty(modItem.CreateBy))
                        {
                            // Add new record
                            if ((((modItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                                || (modItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON)
                                || (modItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE)))

                                || (((modItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || modItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                                || (modItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE))
                                && ((modItem.BillingAmt.GetValueOrDefault() > 0 || modItem.BillingAmtUsd.GetValueOrDefault() > 0))))
                            {
                                if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                                {
                                    if (dsRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
                                    {
                                        //if (modItem.BillingAmt.GetValueOrDefault() > 0)
                                        //{
                                        modItem.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE;
                                        //}
                                    }
                                    else
                                    {
                                        //if (modItem.BillingAmt.GetValueOrDefault() > 0)
                                        //{
                                        modItem.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON;
                                        //}
                                    }
                                }

                                billingtemphandler.InsertBillingTemp(modItem);
                            }
                        }
                        else
                        {
                            // Update record
                            if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                            {
                                if (dsRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED)
                                {
                                    //if (modItem.BillingAmt.GetValueOrDefault() > 0)
                                    //{
                                    modItem.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE;
                                    //}
                                }
                                else
                                {
                                    //if (modItem.BillingAmt.GetValueOrDefault() > 0)
                                    //{
                                    modItem.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON;
                                    //}
                                }
                            }

                            if ((((modItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                                || (modItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON)
                                || (modItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE)))

                                || (((modItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || modItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CHANGE_INSTALLATION_FEE)
                                || (modItem.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE))
                                && ((modItem.BillingAmt.GetValueOrDefault() > 0) || modItem.BillingAmtUsd.GetValueOrDefault() > 0)))
                            {
                                if (IsUpdateOCC)
                                {
                                    billingtemphandler.InsertBillingTemp(modItem);
                                }
                                else
                                {
                                    billingtemphandler.UpdateBillingTempByKey(modItem);
                                }
                            }
                            else
                            {
                                if (!IsUpdateOCC)
                                {
                                    billingtemphandler.DeleteBillingTempByKey(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode
                                        , dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC
                                        , modItem.SequenceNo);
                                }
                            }
                        }
                    }

                    //listDistinctSequence = new List<string>();
                    //foreach (var item in listBillingTemp)
                    //{
                    //    if (item.Status == "ADD")
                    //    {
                    //        if (listDistinctSequence.Contains(item.Sequence) == false)
                    //            listDistinctSequence.Add(item.Sequence);
                    //    }
                    //}

                    //foreach (var item in listDistinctSequence)
                    //{
                    //    if (listBillingTemp.FindAll(delegate(dtBillingTempChangePlanData s) { return s.Sequence == item; })[0].Status == "ADD")
                    //    {
                    //        //เนื่องจากในกรณีที่ในการ save รอบเเรกมีการใส่ Fee บางประเภทเข้ามาเป็น 0 ทำให้ไม่มีการ save ลงไปใน Database ดังนั้นในรอบต่อไปเราไม่จำเป็นที่จะต้องมีการ สร้าง Billing Client ตัวใหม่
                    //        //เเต่ให้นำเอา Billing Client ตัวเดิมที่ Fee ไม่เป็น 0 ในรอบที่หนึ่งมาใช้งานเลย
                    //        if (listBillingTemp.FindAll(delegate(dtBillingTempChangePlanData s) { return s.Sequence == item && s.Status == "Update"; }).Count() != 0)
                    //            strBillingClientCode = listBillingTemp.FindAll(delegate(dtBillingTempChangePlanData s) { return s.Sequence == item && s.Status == "Update"; })[0].BillingClientCode;
                    //        else
                    //            strBillingClientCode = ManageBillingClient();

                    //        listBillingClientFindAll = listBillingClient.FindAll(delegate(dtBillingClientData s) { return s.BillingClientCode == item; });
                    //        if (listBillingClientFindAll != null)
                    //        {
                    //            listBillingClientFindAll[0].BillingClientCode = strBillingClientCode;
                    //            listBillingTempFindAll = listBillingTemp.FindAll(delegate(dtBillingTempChangePlanData s) { return s.Sequence == item; });
                    //            if (listBillingTempFindAll != null)
                    //            {
                    //                foreach (var itemBilling in listBillingTempFindAll)
                    //                {
                    //                    itemBilling.BillingClientCode = strBillingClientCode;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    //foreach (var item in listBillingTemp)
                    //{
                    //    if (item.Status.ToUpper() == "ADD") // 4.1
                    //    {
                    //        if ((((item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                    //            || (item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON)
                    //            || (item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE))
                    //            && (!String.IsNullOrEmpty(item.BillingOCC)))
                    //            || (((item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                    //            || (item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON)
                    //            || (item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE))
                    //            && ((String.IsNullOrEmpty(item.BillingOCC))
                    //            && (item.BillingAmt > 0)))
                    //            || (((item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE)
                    //            || (item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE))
                    //            && (item.BillingAmt > 0)))
                    //        {
                    //            this.InsertTbt_BillingTemp(item.ContractCode, item.OCC, item.BillingOCC, item.BillingTargetRunningNo, item.BillingClientCode,
                    //            item.BillingTargetCode, item.BillingOfficeCode, item.BillingType, item.CreditTerm, item.BillingTiming, item.BillingAmt,
                    //            item.PayMethod, item.BillingCycle, item.CalDailyFeeStatus, item.SendFlag, CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    //            CommonUtil.dsTransData.dtUserData.EmpNo);
                    //        }
                    //    }

                    //    if (item.Status.ToUpper() == "UPDATE") // 4.3
                    //    {
                    //        if (item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE || item.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE && item.BillingAmt > 0)
                    //        {
                    //            //4.Delete the billing temp of one-time fee records
                    //            //this.DeleteAllOneTimeFee(item.ContractCode, item.OCC, item.ContractCode, item.OCC, item.BillingOCC, item.BillingTargetRunningNo, item.BillingClientCode,
                    //            //item.BillingTargetCode, item.BillingOfficeCode, item.BillingType, item.BillingTiming, item.BillingAmt,
                    //            //item.PayMethod, item.BillingCycle, item.CalDailyFeeStatus, item.SendFlag, CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    //            //CommonUtil.dsTransData.dtUserData.EmpNo);
                    //        }
                    //        else
                    //        {
                    //            tbt_BillingTemp tbtBillingTemp = new tbt_BillingTemp();
                    //            tbtBillingTemp.ContractCode = item.ContractCode;
                    //            tbtBillingTemp.OCC = item.OCC;
                    //            tbtBillingTemp.SequenceNo = item.SequenceNo;
                    //            tbtBillingTemp.BillingTargetRunningNo = item.BillingTargetRunningNo;
                    //            tbtBillingTemp.BillingClientCode = item.BillingClientCode;
                    //            tbtBillingTemp.BillingOfficeCode = item.BillingOfficeCode;
                    //            tbtBillingTemp.BillingType = item.BillingType;
                    //            tbtBillingTemp.BillingTiming = item.BillingTiming;
                    //            tbtBillingTemp.BillingAmt = item.BillingAmt;
                    //            tbtBillingTemp.PayMethod = item.PayMethod;
                    //            tbtBillingTemp.BillingCycle = item.BillingCycle;
                    //            tbtBillingTemp.CalDailyFeeStatus = item.CalDailyFeeStatus;
                    //            tbtBillingTemp.SendFlag = item.SendFlag;
                    //            tbtBillingTemp.CreateDate = item.CreateDate;
                    //            tbtBillingTemp.CreateBy = item.CreateBy;
                    //            tbtBillingTemp.UpdateDate = item.UpdateDate;
                    //            tbtBillingTemp.UpdateBy = item.UpdateBy;

                    //            if ((((tbtBillingTemp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                    //            || (tbtBillingTemp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON)
                    //            || (tbtBillingTemp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE))
                    //            && (!String.IsNullOrEmpty(tbtBillingTemp.BillingOCC)))
                    //            || (((tbtBillingTemp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                    //            || (tbtBillingTemp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON)
                    //            || (tbtBillingTemp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE))
                    //            && ((String.IsNullOrEmpty(tbtBillingTemp.BillingOCC))
                    //            && (tbtBillingTemp.BillingAmt > 0)))
                    //            || (((tbtBillingTemp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE)
                    //            || (tbtBillingTemp.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE))
                    //            && (tbtBillingTemp.BillingAmt > 0)))
                    //            {
                    //                this.UpdateTbt_BillingTempByKeyXML(tbtBillingTemp);
                    //            }
                    //            else
                    //            {
                    //                this.DeleteTbt_BillingTemp_ByKey(tbtBillingTemp.ContractCode, tbtBillingTemp.OCC, tbtBillingTemp.SequenceNo);
                    //            }
                    //        }
                    //    }

                    //    if (item.Status.ToUpper() == "DELETE") // 4.2
                    //    {
                    //        this.DeleteTbt_BillingTemp_ByContractCodeOCCBillingClientCodeBillingOfficeCode(item.ContractCode, item.OCC, item.BillingClientCode, item.BillingOfficeCode);
                    //    }
                    //}

                    //6
                    if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE ||
                        dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE ||
                        dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG ||
                        dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA ||
                        ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL
                            || dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE)
                            && ((dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) ||
                            ((dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (dsRentalContract.dtTbt_RentalContractBasic[0].StartType == StartType.C_START_TYPE_ALTER_START)))
                        ))
                    {
                        billingInterfaceHandler.SendBilling_RentalChangePlan(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);
                    }

                    // 6 (add)
                    if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                    {
                        maintenanceHandler.UpdateMADateInSaleContract(DateTime.Now, dsRentalContract);
                    }

                    // call installation module
                    if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                    {
                        installationhandler.GenerateInstallationSlipDoc(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode);
                    }

                    //7
                    bLockQuotationResult = quotationHandler.LockQuotation(dsQuotation.dtTbt_QuotationBasic.QuotationTargetCode,
                                           dsQuotation.dtTbt_QuotationBasic.Alphabet, LockStyle.C_LOCK_STYLE_BACKWARD);

                    doUpdateQuotation = new doUpdateQuotationData();
                    doUpdateQuotation.QuotationTargetCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode;
                    doUpdateQuotation.Alphabet = dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationAlphabet;
                    doUpdateQuotation.ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode;
                    doUpdateQuotation.ActionTypeCode = ActionType.C_ACTION_TYPE_CHANGE;
                    quotationHandler.UpdateQuotationData(doUpdateQuotation);

                    //8 Delete all billing temp
                    if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE ||
                       dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE ||
                       dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG ||
                       dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA) &&
                           dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus != ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    {
                        IBillingTempHandler target = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                        List<tbt_BillingTemp> dtDeletedTbt_BillingTemp = target.DeleteBillingTempByContractCode(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode);
                    }


                    #region Rebooking before first installation complete

                    var installHist = installationhandler.GetTbt_InstallationHistory(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, null, null);
                    var currentBooking = invenhandler.GetTbt_InventoryBooking(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode);

                    if ((installHist == null || installHist.Count <= 0) &&
                        currentBooking != null && currentBooking.Count > 0)
                    {
                        var bookingInstruments = (
                            from qd in dsQuotation.dtTbt_QuotationInstrumentDetails
                            group qd by new { qd.InstrumentCode } into grpByInst
                            orderby grpByInst.Key.InstrumentCode
                            select new
                            {
                                grpByInst.Key.InstrumentCode,
                                InstrumentQty = grpByInst.Sum(d => d.InstrumentQty),
                            }
                        ).ToList();

                        doBooking booking = new doBooking()
                        {
                            ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                            ExpectedStartServiceDate = dsRentalContract.dtTbt_RentalSecurityBasic[0].ExpectedOperationDate ?? currentBooking[0].ExpectedStartServiceDate.Value,
                            blnExistContractCode = true,
                            blnFirstInstallCompleteFlag = true,
                            InstrumentCode = (from inst in bookingInstruments orderby inst.InstrumentCode select inst.InstrumentCode).ToList(),
                            InstrumentQty = (from inst in bookingInstruments orderby inst.InstrumentCode select inst.InstrumentQty ?? 0).ToList()
                        };

                        var previousBooking = invenhandler.GetTbt_InventoryBookingDetail(booking.ContractCode, null);
                        invenhandler.CancelBooking(booking);
                        invenhandler.NewBooking(booking);
                        var newBookingDtl = invenhandler.GetTbt_InventoryBookingDetail(booking.ContractCode, null);
                        foreach (var bookingDtl in newBookingDtl)
                        {
                            bookingDtl.StockOutQty = (
                                from d in previousBooking
                                where d.InstrumentCode == bookingDtl.InstrumentCode
                                select d.StockOutQty
                            ).FirstOrDefault();

                            if ((bookingDtl.BookingQty ?? 0) < (bookingDtl.StockOutQty ?? 0))
                            {
                                bookingDtl.BookingQty = bookingDtl.StockOutQty;
                            }
                        }

                        var lstUpdateBookingDtl = newBookingDtl.Where(d => (d.StockOutQty ?? 0) != 0).ToList();
                        if (lstUpdateBookingDtl != null && lstUpdateBookingDtl.Count > 0)
                        {
                            invenhandler.UpdateTbt_InventoryBookingDetail(lstUpdateBookingDtl);
                        }
                    }
                    #endregion

                    scope.Complete();
                    newItemList = CommonUtil.ClonsObjectList<dtBillingTemp_SetItem, dtBillingTemp_SetItem>(tmpnewItemList);
                    updateItemList = CommonUtil.ClonsObjectList<dtBillingTemp_SetItem, dtBillingTemp_SetItem>(tmpupdateItemList);

                }
                return hasInstallation;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Update billing occurrence to billing temp
        /// </summary>
        /// <param name="OCC"></param>
        /// <param name="billingTemp"></param>
        /// <returns></returns>
        private List<dtBillingTempChangePlanData> UpdateBillingOCCToBillingTemp(string OCC, List<dtBillingTempChangePlanData> billingTemp)
        {
            foreach (var item in billingTemp)
            {
                item.OCC = OCC;
            }

            return billingTemp;
        }

        //public bool RegisterChangeContractFee(dsRentalContractData dsRentalContract, List<dtEmailAddress> listDTEmailAddress, List<dtBillingTempChangePlanData> listDTBillingTempChangePlan, List<dtBillingClientData> listDTBillingClient)

        /// <summary>
        /// For register change contract fee of rental contract
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <param name="listDTEmailAddress"></param>
        /// <param name="newItemList"></param>
        /// <param name="updateItemList"></param>
        /// <param name="deleteItemList"></param>
        /// <param name="changeFeeDate"></param>
        /// <param name="ChangeContractFee"></param>
        /// <returns></returns>
        public int RegisterChangeContractFee(dsRentalContractData dsRentalContract, List<dtEmailAddress> listDTEmailAddress, List<dtBillingTemp_SetItem> newItemList, List<dtBillingTemp_SetItem> updateItemList, List<dtBillingTemp_SetItem> deleteItemList, DateTime? changeFeeDate, decimal? ChangeContractFee)
        {
            int res = -1;
            int iNewCounter;
            string strNewOCC;
            string strBillingClientCode = "";

            decimal? contractFeeBeforeChange;
            IMasterHandler masterHandler;
            ICommonHandler commonHandler;
            IBillingInterfaceHandler billingInterfaceHandler;

            ContractHandler contractHandler;
            CommonContractHandler commonContractHandler;

            List<string> listDistinctSequence;
            List<dtBillingClientData> listBillingClientFindAll;
            List<dtBillingTempChangePlanData> listBillingTempFindAll;

            tbt_BillingTemp tbtBillingTemp;
            List<tbm_Customer> dtCustomer;
            List<doGetTbm_Site> dtSite;
            doNotifyChangeFeeContract doNotifyEmail;

            CommonUtil util = new CommonUtil();

            try
            {
                var tmpnewItemList = CommonUtil.ClonsObjectList<dtBillingTemp_SetItem, dtBillingTemp_SetItem>(newItemList);
                var tmpupdateItemList = CommonUtil.ClonsObjectList<dtBillingTemp_SetItem, dtBillingTemp_SetItem>(updateItemList);

                doNotifyEmail = new doNotifyChangeFeeContract();
                contractHandler = new ContractHandler();
                commonContractHandler = new CommonContractHandler();
                commonHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                billingInterfaceHandler = ServiceContainer.GetService<IBillingInterfaceHandler>() as IBillingInterfaceHandler;
                IBillingMasterHandler billingmasterhandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                using (TransactionScope scope = new TransactionScope())
                {
                    //0.Merge all billingtemp data
                    List<dtBillingTempChangeFeeData> deleteList = new List<dtBillingTempChangeFeeData>();
                    var allBillingTemp = this.GetBillingTempForChangeFee(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode);

                    //Add by Jutarat A. on 03052013
                    if (allBillingTemp != null && allBillingTemp.Count > 0)
                    {
                        foreach (var oldItem in allBillingTemp)
                        {
                            if ((dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                            || (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START))
                            {
                                oldItem.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE;
                            }

                            if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                            {
                                oldItem.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_STOP_FEE;
                            }

                            if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                                && (dsRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED))
                            {
                                oldItem.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE;
                            }

                            if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                                && (dsRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode != MAFeeType.C_MA_FEE_TYPE_RESULT_BASED))
                            {
                                oldItem.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON;
                            }

                            // begin add by jirawat jannet on 2016-11-24
                            // update billing contracy amount from screen edit data

                            var updateData = tmpupdateItemList.Where(m => m.BillingOCC == oldItem.BillingOCC).FirstOrDefault();
                            if (updateData != null)
                            {
                                oldItem.BillingAmtCurrencyType = updateData.BillingContractFeeCurrencyType;
                                if (updateData.BillingContractFeeCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                                {
                                    oldItem.BillingAmt = updateData.BillingContractFee;
                                    oldItem.BillingAmtUsd = null;
                                }
                                else if (updateData.BillingContractFeeCurrencyType == CurrencyUtil.C_CURRENCY_US)
                                {
                                    oldItem.BillingAmt = null;
                                    oldItem.BillingAmtUsd = updateData.BillingContractFee;
                                }
                            }

                            // end add
                        }
                    }
                    //End Add

                    foreach (var newItem in tmpnewItemList)
                    {
                        // Create billing client code
                        if (String.IsNullOrEmpty(newItem.BillingClientCode))
                        {
                            tbm_BillingClient dummieClient = new tbm_BillingClient()
                            {
                                AddressEN = newItem.AddressEN,
                                AddressLC = newItem.AddressLC,
                                BillingClientCode = "",
                                BranchNameEN = newItem.BranchNameEN,
                                BranchNameLC = newItem.BranchNameLC,
                                BusinessTypeCode = newItem.BusinessTypeCode,
                                BusinessTypeName = newItem.BusinessType,
                                CustTypeCode = newItem.CustomerType,
                                FullNameEN = newItem.FullNameEN,
                                FullNameLC = newItem.FullNameLC,
                                IDNo = newItem.IDNo,
                                NameEN = newItem.FullNameEN,
                                NameLC = newItem.FullNameLC,
                                Nationality = newItem.Nationality,
                                RegionCode = newItem.RegionCode,
                                PhoneNo = newItem.PhoneNo,
                                CompanyTypeCode = newItem.CompanyTypeCode
                            };

                            var newClientCode = billingmasterhandler.ManageBillingClient(dummieClient);
                            newItem.BillingClientCode = newClientCode;
                        }

                        dtBillingTempChangeFeeData contractFee = new dtBillingTempChangeFeeData()
                        {
                            //BillingAmt = newItem.BillingContractFee,
                            BillingClientCode = (newItem.BillingClientCode.Length == CommonValue.BILLING_CLIENT_CODE_SHORT_DIGIT) ? util.ConvertBillingClientCode(newItem.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_LONG) : newItem.BillingClientCode,
                            BillingCycle = null,
                            BillingOCC = null,
                            BillingOfficeCode = newItem.BillingOffice,
                            BillingTargetCode = util.ConvertBillingTargetCode(newItem.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG),
                            BillingTiming = null,
                            BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE,
                            ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                            OCC = dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC,
                            PayMethod = null,
                            SequenceNo = -1,
                        };

                        contractFee.BillingAmtCurrencyType = newItem.BillingContractFeeCurrencyType;
                        if (newItem.BillingContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US)
                            contractFee.BillingAmtUsd = newItem.BillingContractFee;
                        else
                            contractFee.BillingAmt = newItem.BillingContractFee;

                        if ((dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                            || (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START))
                        {
                            contractFee.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE;
                        }

                        if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                        {
                            contractFee.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_STOP_FEE;
                        }

                        if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                            && (dsRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED))
                        {
                            contractFee.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE;
                        }

                        if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                            && (dsRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode != MAFeeType.C_MA_FEE_TYPE_RESULT_BASED))
                        {
                            contractFee.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON;
                        }

                        allBillingTemp.Add(contractFee);
                    }

                    foreach (var updateItem in tmpupdateItemList)
                    {
                        // Create billing client code
                        if (String.IsNullOrEmpty(updateItem.BillingClientCode))
                        {
                            tbm_BillingClient dummieClient = new tbm_BillingClient()
                            {
                                AddressEN = updateItem.AddressEN,
                                AddressLC = updateItem.AddressLC,
                                BillingClientCode = "",
                                BranchNameEN = updateItem.BranchNameEN,
                                BranchNameLC = updateItem.BranchNameLC,
                                BusinessTypeCode = updateItem.BusinessTypeCode,
                                BusinessTypeName = updateItem.BusinessType,
                                CustTypeCode = updateItem.CustomerType,
                                FullNameEN = updateItem.FullNameEN,
                                FullNameLC = updateItem.FullNameLC,
                                IDNo = updateItem.IDNo,
                                NameEN = updateItem.FullNameEN,
                                NameLC = updateItem.FullNameLC,
                                Nationality = updateItem.Nationality,
                                RegionCode = updateItem.RegionCode,
                                PhoneNo = updateItem.PhoneNo,
                                CompanyTypeCode = updateItem.CompanyTypeCode
                            };

                            var newClientCode = billingmasterhandler.ManageBillingClient(dummieClient);
                            updateItem.BillingClientCode = newClientCode;
                        }

                        // Update billing temp to list
                        var targBillingItemLst = from a in allBillingTemp
                                                 where (a.BillingOCC == updateItem.BillingOCC)
                                                 select a;

                        if (targBillingItemLst.Count() == 1)
                        {
                            var targBilling = targBillingItemLst.First();
                            allBillingTemp.Remove(targBilling);

                            if ((dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                            || (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START))
                            {
                                targBilling.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE;
                            }

                            if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                            {
                                targBilling.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_STOP_FEE;
                            }

                            if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                                && (dsRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode == MAFeeType.C_MA_FEE_TYPE_RESULT_BASED))
                            {
                                targBilling.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE;
                            }

                            if ((dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                                && (dsRentalContract.dtTbt_RentalMaintenanceDetails[0].MaintenanceFeeTypeCode != MAFeeType.C_MA_FEE_TYPE_RESULT_BASED))
                            {
                                targBilling.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON;
                            }

                            targBilling.BillingAmt = updateItem.BillingContractFee;
                            allBillingTemp.Add(targBilling);
                        }
                    }

                    foreach (var deleteItem in deleteItemList)
                    {
                        // Delete billing temp from list
                        var targBillingItemLst = from a in allBillingTemp
                                                 where (a.BillingOCC == deleteItem.BillingOCC)
                                                 select a;

                        if (targBillingItemLst.Count() == 1)
                        {
                            var targBilling = targBillingItemLst.First();
                            deleteList.Add(targBilling);
                            allBillingTemp.Remove(targBilling);
                        }
                    }

                    //1.Update data for changing
                    if (dsRentalContract.dtTbt_RentalContractBasic != null)
                    {
                        dsRentalContract.dtTbt_RentalContractBasic[0].ChangeFeeProcessDate = DateTime.Now;
                        if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                        {
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE_DURING_STOP;
                        }
                        else
                        {
                            dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE;
                        }
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].ImplementFlag = Convert.ToBoolean(RentalImplementType.C_RENTAL_IMPLEMENT_FLAG_B);
                    }
                    //2.Reset contract for new OCC
                    if (dsRentalContract.dtTbt_RentalSecurityBasic != null)
                    {
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].ExpectedInstallationCompleteDate = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteDate = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationTypeCode = null;

                        //--- Add by Phoomsak L. 2012-09-03 for clear data before insert new OCC 
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationSlipNo = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteEmpNo = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFee = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService = null;

                        //Add by Jutarat A. 03102012
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationAlphabet = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo1 = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].SalesmanEmpNo2 = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].SalesSupporterEmpNo = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFee = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].DepositFeeBillingTiming = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanCode = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].PlannerEmpNo = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanCheckerEmpNo = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanCheckDate = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanApproverEmpNo = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanApproveDate = null;
                        //End Add

                        //Add by Pachara S. 13032017
                        //Merge at 14032017 By Pachara S.
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].DocAuditResult = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].DocAuditResultName = null;
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].DocumentCode = null;
                    }

                    //3.Set OCC and counter
                    if (dsRentalContract.dtTbt_RentalContractBasic != null)
                    {
                        strNewOCC = GenerateContractOCC(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, true);
                        dsRentalContract = ChangeAllOCCInDSRentalContract(ref dsRentalContract, strNewOCC);
                    }

                    iNewCounter = 0; //Fixed to 0 for new OCC
                    foreach (var item in dsRentalContract.dtTbt_RentalSecurityBasic)
                    {
                        item.CounterNo = iNewCounter;
                    }

                    // 4.Insert new contract OCC
                    UpdateSummaryFields(ref dsRentalContract);
                    //InsertEntireContract(dsRentalContract);
                    this.InsertEntireContractForCTS010(dsRentalContract); //Modify by Jutarat A. on 19092013


                    // 5.Update billing temp
                    //List<doBillingTempBasic> billingTempLst = new List<doBillingTempBasic>();

                    List<doBillingTempBasic> billingTempLst = (from a in allBillingTemp
                                                               select new doBillingTempBasic()
                                                               {
                                                                   BillingAmount = a.BillingAmt,
                                                                   BillingAmountUsd = a.BillingAmtUsd,
                                                                   BillingAmountCurrencyType = a.BillingAmtCurrencyType,
                                                                   BillingClientCode = a.BillingClientCode,
                                                                   BillingCycle = a.BillingCycle,
                                                                   BillingOCC = a.BillingOCC,
                                                                   BillingOfficeCode = a.BillingOfficeCode,
                                                                   BillingTargetCode = a.BillingTargetCode,
                                                                   PaymentMethod = a.PayMethod,
                                                                   ContractCode = a.ContractCode,
                                                                   ContractBillingType = a.BillingType,
                                                                   ChangeFeeDate = changeFeeDate.GetValueOrDefault(),
                                                                   ProductTypeCode = dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode
                                                               }).ToList();

                    billingInterfaceHandler.SendBilling_ChangeFee(billingTempLst);

                    //listDistinctSequence = new List<string>();
                    //foreach (var item in listDTBillingTempChangePlan)
                    //{
                    //    if (item.Status == "ADD")
                    //    {
                    //        if (listDistinctSequence.Contains(item.Sequence) == false)
                    //            listDistinctSequence.Add(item.Sequence);
                    //    }
                    //}

                    //foreach (var item in listDistinctSequence)
                    //{
                    //    if (listDTBillingTempChangePlan.FindAll(delegate(dtBillingTempChangePlanData s) { return s.Sequence == item; })[0].Status == "ADD")
                    //    {
                    //        strBillingClientCode = ManageBillingClient();
                    //        listBillingClientFindAll = listDTBillingClient.FindAll(delegate(dtBillingClientData s) { return s.BillingClientCode == item; });
                    //        if (listBillingClientFindAll != null)
                    //        {
                    //            listBillingClientFindAll[0].BillingClientCode = strBillingClientCode;
                    //            listBillingTempFindAll = listDTBillingTempChangePlan.FindAll(delegate(dtBillingTempChangePlanData s) { return s.Sequence == item; });
                    //            if (listBillingTempFindAll != null)
                    //            {
                    //                foreach (var itemBilling in listBillingTempFindAll)
                    //                {
                    //                    itemBilling.BillingClientCode = strBillingClientCode;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    //foreach (var tmpItem in listDTBillingTempChangePlan)
                    //{
                    //    if ((dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START)
                    //        || (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START))
                    //    {
                    //        tmpItem.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_CANCEL_CONTRACT_FEE;
                    //    }
                    //    else if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                    //    {
                    //        tmpItem.BillingType = ContractBillingType.C_CONTRACT_BILLING_TYPE_STOP_FEE;
                    //    }
                    //}

                    //foreach (var item in listDTBillingTempChangePlan)
                    //{
                    //    if (item.Status == "ADD")
                    //    {
                    //        List<doBillingTempBasic> listdoBillingBasic = new List<doBillingTempBasic>();
                    //        doBillingTempBasic doBilling = new doBillingTempBasic();
                    //        doBilling.BillingAmount = item.BillingAmt;
                    //        doBilling.BillingClientCode = item.BillingClientCode;
                    //        doBilling.BillingCycle = item.BillingCycle;
                    //        doBilling.BillingOCC = item.BillingOCC;
                    //        doBilling.BillingOfficeCode = item.BillingOfficeCode;
                    //        doBilling.BillingTargetCode = item.BillingTargetCode;
                    //        doBilling.ContractBillingType = item.BillingType;
                    //        doBilling.CalculationDailyFee = item.CalDailyFeeStatus;
                    //        doBilling.ContractCode = item.ContractCode;
                    //        doBilling.PaymentMethod = item.PayMethod;
                    //        listdoBillingBasic.Add(doBilling);
                    //        billingInterfaceHandler.SendBilling_ChangeFee(listdoBillingBasic);

                    //        //this.InsertTbt_BillingTemp(item.ContractCode, item.OCC, item.BillingOCC, item.BillingTargetRunningNo, item.BillingClientCode,
                    //        //item.BillingTargetCode, item.BillingOfficeCode, item.BillingType, item.BillingTiming, item.BillingAmt,
                    //        //item.PayMethod, item.BillingCycle, item.CalDailyFeeStatus, item.SendFlag, CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    //        //CommonUtil.dsTransData.dtUserData.EmpNo);
                    //    }

                    //    if (item.Status == "Update")
                    //    {
                    //        tbtBillingTemp = new tbt_BillingTemp();
                    //        tbtBillingTemp.ContractCode = item.ContractCode;
                    //        tbtBillingTemp.OCC = item.OCC;
                    //        tbtBillingTemp.SequenceNo = item.SequenceNo;
                    //        tbtBillingTemp.BillingTargetRunningNo = item.BillingTargetRunningNo;
                    //        tbtBillingTemp.BillingClientCode = item.BillingClientCode;
                    //        tbtBillingTemp.BillingOfficeCode = item.BillingOfficeCode;
                    //        tbtBillingTemp.BillingType = item.BillingType;
                    //        tbtBillingTemp.BillingTiming = item.BillingTiming;
                    //        tbtBillingTemp.BillingAmt = item.BillingAmt;
                    //        tbtBillingTemp.PayMethod = item.PayMethod;
                    //        tbtBillingTemp.BillingCycle = item.BillingCycle;
                    //        tbtBillingTemp.CalDailyFeeStatus = item.CalDailyFeeStatus;
                    //        tbtBillingTemp.SendFlag = item.SendFlag;
                    //        tbtBillingTemp.CreateDate = item.CreateDate;
                    //        tbtBillingTemp.CreateBy = item.CreateBy;
                    //        tbtBillingTemp.UpdateDate = item.UpdateDate;
                    //        tbtBillingTemp.UpdateBy = item.UpdateBy;
                    //        this.UpdateTbt_BillingTempByKeyXML(tbtBillingTemp);
                    //    }

                    //    if (item.Status == "Delete")
                    //    {
                    //        this.DeleteTbt_BillingTemp_ByContractCodeOCCBillingClientCodeBillingOfficeCode(item.ContractCode, item.OCC, item.BillingClientCode, item.BillingOfficeCode);
                    //    }
                    //}

                    // Set default result to 2 (all send mail is complete)
                    res = 1;

                    commonContractHandler.DeleteTbt_ContractEmail_UnsentContractEmail(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, ContractEmailType.C_CONTRACT_EMAIL_TYPE_NOTIFY_CHANGE_FEE, FlagType.C_FLAG_OFF);

                    System.Net.Mail.DeliveryNotificationOptions delivery;
                    if (dsRentalContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate != null)
                    {
                        var operationOfficeDat = officehandler.GetTbm_Office(dsRentalContract.dtTbt_RentalContractBasic[0].OperationOfficeCode);
                        var billingOfficeDat = officehandler.GetTbm_Office(allBillingTemp[0].BillingOfficeCode);

                        EmailTemplateUtil mailUtil = new EmailTemplateUtil(EmailTemplateName.C_EMAIL_TEMPLATE_NAME_CHANGE_FEE);
                        masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                        dtCustomer = masterHandler.GetTbm_Customer(dsRentalContract.dtTbt_RentalContractBasic[0].ContractTargetCustCode);
                        dtSite = masterHandler.GetTbm_Site(dsRentalContract.dtTbt_RentalContractBasic[0].SiteCode);
                        contractFeeBeforeChange = GetContractFeeBeforeChange(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC, dsRentalContract);

                        doNotifyEmail.ContractCode = util.ConvertContractCode(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                        doNotifyEmail.ContractTargetNameEN = dtCustomer[0].CustFullNameEN;
                        doNotifyEmail.ContractTargetNameLC = dtCustomer[0].CustFullNameLC;
                        doNotifyEmail.SiteNameEN = dtSite[0].SiteNameEN;
                        doNotifyEmail.SiteNameLC = dtSite[0].SiteNameLC;
                        doNotifyEmail.ChangeDateOfContractFee = CommonUtil.TextDate(dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeImplementDate);
                        doNotifyEmail.ContractFeeBeforeChange = CommonUtil.TextNumeric(contractFeeBeforeChange);

                        doNotifyEmail.ContractFeeAfterChange = CommonUtil.TextNumeric(ChangeContractFee);
                        doNotifyEmail.ReturnToOriginalFeeDate = CommonUtil.TextDate(dsRentalContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate);
                        doNotifyEmail.OperationOfficeEN = operationOfficeDat[0].OfficeNameEN;
                        doNotifyEmail.OperationOfficeLC = operationOfficeDat[0].OfficeNameLC;
                        doNotifyEmail.RegisterChangeEmpNameEN = CommonUtil.dsTransData.dtUserData.EmpFirstNameEN + ' ' + CommonUtil.dsTransData.dtUserData.EmpLastNameEN;
                        doNotifyEmail.RegisterChangeEmpNameLC = CommonUtil.dsTransData.dtUserData.EmpFirstNameLC + ' ' + CommonUtil.dsTransData.dtUserData.EmpLastNameLC;
                        doNotifyEmail.BillingOfficeEN = billingOfficeDat[0].OfficeNameEN;
                        doNotifyEmail.BillingOfficeLC = billingOfficeDat[0].OfficeNameLC;

                        //contractHandler.GenerateNotifyEmail(doNotifyEmail);
                        var mailTemplate = mailUtil.LoadTemplate(doNotifyEmail);


                        TimeSpan s = dsRentalContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate.Value.Date - DateTime.Now.Date;
                        if (s.TotalDays <= NotifyType.C_SEND_NOTIFY_THRESHOLD)
                        {
                            //doEmailProcess doEmail = new doEmailProcess();
                            //foreach (var item in listDTEmailAddress)
                            //{
                            //    doEmail.MailTo += item.EmailAddress + ";";
                            //}

                            ////doEmail.MailTo.Remove(doEmail.MailTo.Count());
                            //doEmail.MailFrom = CommonUtil.dsTransData.dtUserData.EmailAddress;
                            ////doEmail.MailFromAlias = CommonUtil.dsTransData.dtUserData.EmpFirstNameLC + CommonUtil.dsTransData.dtUserData.EmpLastNameLC;
                            //doEmail.MailFromAlias = null;
                            //doEmail.Subject = mailTemplate.TemplateSubject;
                            //doEmail.Message = mailTemplate.TemplateContent;

                            List<string> doEmail = new List<string>();
                            foreach (var item in listDTEmailAddress)
                            {
                                doEmail.Add(item.EmailAddress);
                            }

                            try
                            {
                                //delivery = commonHandler.SendMail(doEmail);
                                SendMail(mailTemplate, doEmail);
                            }
                            catch (Exception)
                            {
                                // error with send mail
                                // set result to 1
                                res = 2;
                            }
                        }
                        else
                        {
                            foreach (var item in listDTEmailAddress)
                            {
                                tbt_ContractEmail mail = new tbt_ContractEmail();
                                mail.ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode;
                                mail.OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
                                mail.EmailType = ContractEmailType.C_CONTRACT_EMAIL_TYPE_NOTIFY_CHANGE_FEE;
                                mail.ToEmpNo = item.EmpNo;
                                mail.EmailFrom = CommonUtil.dsTransData.dtUserData.EmailAddress;
                                mail.EmailSubject = mailTemplate.TemplateSubject;
                                mail.EmailContent = mailTemplate.TemplateContent;
                                mail.SendDate = (dsRentalContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate.Value.AddDays(-NotifyType.C_SEND_NOTIFY_THRESHOLD));
                                mail.SendFlag = FlagType.C_FLAG_OFF;

                                List<tbt_ContractEmail> listMail = new List<tbt_ContractEmail>();
                                listMail.Add(mail);
                                contractHandler.InsertTbt_ContractEmail(listMail);
                            }
                        }
                    }
                    scope.Complete();
                    newItemList = CommonUtil.ClonsObjectList<dtBillingTemp_SetItem, dtBillingTemp_SetItem>(tmpnewItemList);
                    updateItemList = CommonUtil.ClonsObjectList<dtBillingTemp_SetItem, dtBillingTemp_SetItem>(tmpupdateItemList);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return res;
        }

        public class SendMailObject
        {
            public List<doEmailProcess> EmailList { get; set; }
        }
        public void SendMail(doEmailTemplate template, List<string> mailAddress)
        {
            try
            {
                SendMailObject obj = new SendMailObject();
                if (template != null && mailAddress != null)
                {
                    obj.EmailList = new List<doEmailProcess>();
                    foreach (string addr in mailAddress)
                    {
                        doEmailProcess mail = new doEmailProcess()
                        {
                            MailTo = addr,
                            Subject = template.TemplateSubject,
                            Message = template.TemplateContent
                        };
                        obj.EmailList.Add(mail);
                    }
                }

                System.Threading.Thread t = new System.Threading.Thread(SendMail);
                t.Start(obj);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void SendMail(object o)
        {
            try
            {
                SendMailObject obj = o as SendMailObject;
                if (obj == null)
                    return;

                if (obj.EmailList != null)
                {
                    ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                    foreach (doEmailProcess mail in obj.EmailList)
                    {
                        chandler.SendMail(mail);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// For registering instrument modification of rental contract
        /// </summary>
        /// <param name="dsQuotation"></param>
        /// <param name="dsRentalContract"></param>
        public void RegisterModifyInstrument(dsQuotationData dsQuotation, dsRentalContractData dsRentalContract)
        {
            string strNewOCC;
            bool bLockQuotationResult;
            int bUpdateQuotationResult;
            IQuotationHandler quotationHandler;
            //IInventoryInterfaceHandler inventoryInterfaceHandler;
            doUpdateQuotationData doUpdateQuotation;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    quotationHandler = ServiceContainer.GetService<IQuotationHandler>() as IQuotationHandler;
                    IInventoryHandler invenhandler = ServiceContainer.GetService<IInventoryHandler>() as IInventoryHandler;

                    //1.Update data for changing
                    dsRentalContract.dtTbt_RentalContractBasic[0].ChangeRealInvestigationProcessDate = DateTime.Now;

                    if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                    {
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_QTY_DURING_STOP;
                    }
                    else
                    {
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].ChangeType = RentalChangeType.C_RENTAL_CHANGE_TYPE_MODIFY_INSTRUMENT_QTY;
                    }

                    dsRentalContract.dtTbt_RentalSecurityBasic[0].ImplementFlag = Convert.ToBoolean(RentalImplementType.C_RENTAL_IMPLEMENT_FLAG_B);
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode = dsQuotation.dtTbt_QuotationBasic.QuotationTargetCode;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationAlphabet = dsQuotation.dtTbt_QuotationBasic.Alphabet;

                    // Add by Phoomsak L. 2012-09-04 clear data before insert new OCC
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationSlipNo = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteDate = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteEmpNo = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationTypeCode = null;

                    dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalInstallFee = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_ApproveContract = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_CompleteInstall = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderInstallFee_StartService = null;

                    dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFee = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee = null;

                    UpdateSummaryFields(ref dsRentalContract);

                    //2.Generate contract OCC
                    strNewOCC = GenerateContractOCC(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, true);

                    //3.Set new OCC
                    dsRentalContract = ChangeAllOCCInDSRentalContract(ref dsRentalContract, strNewOCC);
                    dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC = strNewOCC;

                    //4.Insert new contract OCC
                    //InsertEntireContract(dsRentalContract);
                    this.InsertEntireContractForCTS010(dsRentalContract); //Modify by Jutarat A. on 19092013

                    //5.Update to quotation module

                    var quotationInstrumentLst = quotationHandler.GetTbt_QuotationInstrumentDetails(new doGetQuotationDataCondition()
                    {
                        QuotationTargetCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode
                        ,
                        Alphabet = dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationAlphabet
                        ,
                        ProductTypeCode = dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode
                    });

                    //5.1
                    bLockQuotationResult = quotationHandler.LockQuotation(dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationAlphabet, LockStyle.C_LOCK_STYLE_ALL);

                    //5.2
                    doUpdateQuotation = new doUpdateQuotationData();
                    doUpdateQuotation.QuotationTargetCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode;
                    doUpdateQuotation.Alphabet = dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationAlphabet;
                    doUpdateQuotation.ContractCode = dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode;
                    doUpdateQuotation.ActionTypeCode = ActionType.C_ACTION_TYPE_CHANGE;
                    bUpdateQuotationResult = quotationHandler.UpdateQuotationData(doUpdateQuotation);

                    //6.Update to inventory module
#if !ROUND1
                    invenhandler.UpdateRealInvestigation(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode
                        , quotationInstrumentLst);
#endif
                    //invenhandler.UpdateInventoryProcess(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode,
                    //dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC,dsRentalContract.dtTbt_QuotationInstrumentDetails);     
                    scope.Complete();

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// For register maintain contract data (CP-33)
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <param name="listRelationType"></param>
        /// <param name="listContractCode"></param>
        /// <param name="isUpdateRemovalFeeToBillingTemp"></param>
        /// <param name="isGenerateMAScheduleAgain"></param>
        public void RegisterCP33(dsRentalContractData dsRentalContract, List<tbt_RelationType> listRelationType, List<string> listContractCode, bool isUpdateRemovalFeeToBillingTemp, bool isGenerateMAScheduleAgain, bool isUpdateBilling) //Modify (Add isUpdateBilling) by Jutarat A. on 18102013
        {
            List<tbt_RelationType> listRelationTypeTemporary;
            List<tbt_RelationType> listMaintenanceRelationType;
            List<tbt_CancelContractMemoDetail> listCancelContractMemoDetail;
            List<tbt_BillingTemp> listBillingTemp;

            ICommonContractHandler commonContractHandler;
            IBillingTempHandler billingTempHandler;
            IMaintenanceHandler maintenanceHandler;
            ISaleContractHandler saleHandler;
            IBillingHandler billHandler; //Add by Jutarat A. on 18102013

            try
            {

                commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                billingTempHandler = ServiceContainer.GetService<IBillingTempHandler>() as IBillingTempHandler;
                maintenanceHandler = ServiceContainer.GetService<IMaintenanceHandler>() as IMaintenanceHandler;
                saleHandler = ServiceContainer.GetService<ISaleContractHandler>() as ISaleContractHandler;
                billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler; //Add by Jutarat A. on 18102013

                //1.Update data for changing
                //1.1.dtTbt_RentalContractBasic reviseContractBasicProcessDate = TODAY()
                dsRentalContract.dtTbt_RentalContractBasic[0].ReviseContractBasicProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime; //DateTime.Now;


                //2.Update relation type
                //If dtTbt_RentalSecurityBasic.ProductTypeCode = C_PROD_TYPE_MA Then
                if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                {
                    //2.1.Delete all rows in dtTbt_RelationType which have RelationType = C_RELATION_TYPE_MA
                    if (dsRentalContract.dtTbt_RelationType != null)
                    {
                        listRelationTypeTemporary = dsRentalContract.dtTbt_RelationType.FindAll(delegate(tbt_RelationType s) { return s.RelationType == RelationType.C_RELATION_TYPE_MA; });
                        foreach (var item in listRelationTypeTemporary)
                        {
                            dsRentalContract.dtTbt_RelationType.Remove(item);
                        }
                    }

                    //2.2.Generate MA relation type
                    listMaintenanceRelationType = commonContractHandler.GenerateMaintenanceRelationType(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, listContractCode);

                    //2.4. Update all dtTbt_RelationType.OCC = dtTbt_RentalSecurityBasic.OCC
                    foreach (var item in listMaintenanceRelationType)
                    {
                        //if (item.ProductTypeCode == ProductType.C_PROD_TYPE_MA) //Add by Jutarat A. on 16082012
                        if (item.RelationType == RelationType.C_RELATION_TYPE_MA) //Modify by Jutarat A. on 29112013
                        {
                            item.OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC;

                            //1.3. Insert new_dtTbt_RelationType to dtEntireContract.dtTbt_RelationType
                            //InsertTbt_RelationType(CommonUtil.ConvertToXml_Store<tbt_RelationType>(listMaintenanceRelationType));
                            dsRentalContract.dtTbt_RelationType.Add(item);
                        }
                    }

                }

                //Add by Jutarat A. on 16082012
                //3.	Update relation type 
                if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)
                {
                    if (dsRentalContract.dtTbt_RelationType != null)
                    {
                        foreach (var item in dsRentalContract.dtTbt_RelationType)
                        {
                            if (item.RelationType == RelationType.C_RELATION_TYPE_SALE)
                            {
                                item.RelatedOCC = saleHandler.GetLastOCC(item.RelatedContractCode);
                            }
                        }
                    }
                }
                //End Add

                //4.Update contract
                //4.1.Update entire contract
                UpdateEntireContract(dsRentalContract);

                //5.Update billing temp
                //If isUpdateRemovalFeeToBillingTemp = TRUE Then
                if (isUpdateRemovalFeeToBillingTemp == true)
                {
                    listCancelContractMemoDetail = dsRentalContract.dtTbt_CancelContractMemoDetail.FindAll(delegate(tbt_CancelContractMemoDetail s) { return s.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE; });
                    listBillingTemp = billingTempHandler.GetFee(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC, ContractBillingType.C_CONTRACT_BILLING_TYPE_REMOVAL_INSTALLATION_FEE, null);

                    if (listCancelContractMemoDetail.Count == 0 && listBillingTemp.Count != 0)
                    {
                        foreach (var item in listBillingTemp)
                        {
                            billingTempHandler.DeleteBillingTempByKey(item.ContractCode, item.OCC, item.SequenceNo);
                        }
                    }

                    if (listCancelContractMemoDetail.Count != 0 && listBillingTemp.Count != 0)
                    {
                        if (listCancelContractMemoDetail[0].FeeAmountCurrencyType != listBillingTemp[0].BillingAmtCurrencyType
                            || listCancelContractMemoDetail[0].FeeAmount != listBillingTemp[0].BillingAmt
                            || listCancelContractMemoDetail[0].FeeAmountUsd != listBillingTemp[0].BillingAmtUsd)
                        {
                            listBillingTemp[0].BillingAmtCurrencyType = listCancelContractMemoDetail[0].FeeAmountCurrencyType;
                            listBillingTemp[0].BillingAmt = listCancelContractMemoDetail[0].FeeAmount;
                            listBillingTemp[0].BillingAmtUsd = listCancelContractMemoDetail[0].FeeAmountUsd;

                            foreach (var item in listBillingTemp)
                            {
                                billingTempHandler.UpdateBillingTempByKey(item);
                            }
                        }
                    }
                }

                //6.Update maintenance contract target
                if (dsRentalContract.dtTbt_RentalContractBasic != null)
                {
                    if (dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
                    {
                        if (dsRentalContract.dtTbt_RentalContractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                        {
                            //maintenanceHandler.update
                            maintenanceHandler.UpdateMADateInSaleContract(
                                CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                dsRentalContract);
                        }
                    }
                }

                //7.Generate maintenance schedule
                if (isGenerateMAScheduleAgain == true)
                {
                    maintenanceHandler.GenerateMaintenanceSchedule(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, GenerateMAProcessType.C_GEN_MA_TYPE_RE_CREATE);
                }

                //Add by Jutarat A. on 18102013
                //8.	Update billing module
                if (isUpdateBilling)
                {
                    //8.1.1.	Get billing basic
                    List<tbt_BillingBasic> dtBillingBasic = billHandler.GetTbt_BillingBasic(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, null);
                    if (dtBillingBasic != null)
                    {
                        List<tbt_MonthlyBillingHistory> dtMonthlyBillingHistory = new List<tbt_MonthlyBillingHistory>();
                        foreach (tbt_BillingBasic data in dtBillingBasic)
                        {
                            //8.1.2.	Set data to all billing basic
                            data.StartOperationDate = dsRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate;

                            //8.1.3.	Update all billing basic
                            billHandler.UpdateTbt_BillingBasic(data);

                            //8.1.4.	Get first monthly billing history
                            tbt_MonthlyBillingHistory dtHistory = billHandler.GetFirstBillingHistoryData(data.ContractCode, data.BillingOCC);
                            if (dtHistory != null)
                            {
                                dtMonthlyBillingHistory.Add(dtHistory);
                            }
                        }

                        if (dtMonthlyBillingHistory != null)
                        {
                            foreach (tbt_MonthlyBillingHistory data in dtMonthlyBillingHistory)
                            {
                                //8.1.5.	Set data to all first monthly billing history
                                data.BillingStartDate = dsRentalContract.dtTbt_RentalContractBasic[0].FirstSecurityStartDate;

                                //8.1.6.	Update all first monthly billing history
                                billHandler.UpdateTbt_MonthlyBillingHistory(data);
                            }
                        }
                    }
                }
                //End Add
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// For register maintain contract data (CP-34) (Correct)
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <param name="isSendNotifyEmail"></param>
        /// <returns></returns>
        public bool RegisterCP34Correct(dsRentalContractData dsRentalContract, bool isSendNotifyEmail)
        {
            try
            {
                bool isFailToSentEmail = false;

                #region Update data for changing

                dsRentalContract.dtTbt_RentalContractBasic[0].ReviseSecurityBasicProcessDate =
                    CommonUtil.dsTransData.dtOperationData.ProcessDateTime; //DateTime.Now;

                #endregion
                #region Update summary fields in contract basic

                if (dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC == dsRentalContract.dtTbt_RentalContractBasic[0].LastOCC)
                {
                    UpdateSummaryFields(ref dsRentalContract);
                }

                #endregion
                #region Update entire contract with specified OCC

                UpdateEntireContract(dsRentalContract);

                #region Update Rental Inst sub contractor

                DeleteTbt_RentalInstSubContractor_ByContractCodeOCC(
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode,
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);

                if (dsRentalContract.dtTbt_RentalInstSubcontractor != null)
                {
                    foreach (tbt_RentalInstSubcontractor inst in dsRentalContract.dtTbt_RentalInstSubcontractor)
                    {
                        InsertTbt_RentalInstSubContractor(inst);
                    }
                }

                #endregion

                #endregion
                #region Send Mail

                if (isSendNotifyEmail)
                {
                    #region Get contract email

                    List<tbt_ContractEmail> listContractEmail = GetContractEmailByContractCodeOCC(
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode,
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);

                    #endregion
                    #region Clear existing notify email

                    ICommonContractHandler commonContract = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                    commonContract.DeleteTbt_ContractEmailUnsentContractEmail(
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode,
                        ContractEmailType.C_CONTRACT_EMAIL_TYPE_NOTIFY_CHANGE_FEE,
                        false);

                    #endregion

                    //if (listContractEmail.Count > 0)
                    if (listContractEmail.Count > 0 && dsRentalContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate != null) //Modify by Jutarat A. on 27012014
                    {
                        #region Generate notify email

                        doEmailTemplate template = GenerateChangeFeeTemplate(dsRentalContract);

                        #endregion

                        // Checking for send immediately when expire within 35 days
                        DateTime? dateReturnToOriginalFeeDateMinusToday;
                        dateReturnToOriginalFeeDateMinusToday = dsRentalContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate;
                        TimeSpan diffResult = dateReturnToOriginalFeeDateMinusToday.Value.Date - DateTime.Now;
                        dateReturnToOriginalFeeDateMinusToday = dateReturnToOriginalFeeDateMinusToday.Value.Date.Add(diffResult);
                        if (diffResult.Days <= NotifyType.C_SEND_NOTIFY_THRESHOLD)
                        {
                            #region Get employee to send email

                            List<tbm_Employee> empLst = new List<tbm_Employee>();
                            foreach (tbt_ContractEmail ce in listContractEmail)
                            {
                                empLst.Add(new tbm_Employee() { EmpNo = ce.ToEmpNo });
                            }

                            string addr = "";
                            IEmployeeMasterHandler empHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                            List<tbm_Employee> rempLst = empHandler.GetEmployeeList(empLst);
                            foreach (tbm_Employee emp in rempLst)
                            {
                                if (addr != "")
                                    addr += ";";
                                addr += emp.EmailAddress;
                            }

                            #endregion
                            #region Send mail

                            doEmailProcess mail = new doEmailProcess()
                            {
                                MailTo = addr,
                                MailFrom = CommonUtil.dsTransData.dtUserData.EmailAddress,
                                MailFromAlias = CommonUtil.TextFullName(
                                                    CommonUtil.dsTransData.dtUserData.EmpFirstNameLC,
                                                    CommonUtil.dsTransData.dtUserData.EmpLastNameLC),
                                Subject = template.TemplateSubject,
                                Message = template.TemplateContent
                            };

                            try
                            {
                                ICommonHandler chandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                                System.Net.Mail.DeliveryNotificationOptions deliveryNotificationOptions = chandler.SendMail(mail);

                                if (deliveryNotificationOptions == System.Net.Mail.DeliveryNotificationOptions.OnFailure)
                                {
                                    isFailToSentEmail = true;
                                }
                            }
                            catch (Exception)
                            {
                                isFailToSentEmail = true;
                            }

                            #endregion
                        }
                        else
                        {
                            SaveEmailForSendLater(listContractEmail, dsRentalContract, template);
                        }
                    }
                }

                #endregion

                return !isFailToSentEmail;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Generate change fee template
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        private doEmailTemplate GenerateChangeFeeTemplate(dsRentalContractData draft)
        {
            try
            {
                CommonUtil cmm = new CommonUtil();

                doChangeFeeEmailObject obj = new doChangeFeeEmailObject();
                obj.ContractCode = cmm.ConvertContractCode(draft.dtTbt_RentalContractBasic[0].ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);

                IMasterHandler masterHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                List<tbm_Customer> listCustomer = masterHandler.GetTbm_Customer(draft.dtTbt_RentalContractBasic[0].ContractTargetCustCode);
                if (listCustomer.Count > 0)
                {
                    obj.ContractTargetNameEN = listCustomer[0].CustFullNameEN;
                    obj.ContractTargetNameLC = listCustomer[0].CustFullNameLC;
                }

                List<doGetTbm_Site> listSite = masterHandler.GetTbm_Site(draft.dtTbt_RentalContractBasic[0].SiteCode);
                if (listSite.Count > 0)
                {
                    obj.SiteNameEN = listSite[0].SiteNameEN;
                    obj.SiteNameLC = listSite[0].SiteNameLC;
                }

                decimal? contractFeeBeforeChange = GetContractFeeBeforeChange(
                    draft.dtTbt_RentalContractBasic[0].ContractCode,
                    draft.dtTbt_RentalSecurityBasic[0].OCC,
                    draft);

                obj.ChangeDateOfContractFee = CommonUtil.TextDate(draft.dtTbt_RentalSecurityBasic[0].ChangeImplementDate);
                obj.ContractFeeBeforeChange = CommonUtil.TextNumeric(contractFeeBeforeChange);
                obj.ContractFeeAfterChange = CommonUtil.TextNumeric(draft.dtTbt_RentalSecurityBasic[0].OrderContractFee);
                obj.ReturnToOriginalFeeDate = CommonUtil.TextDate(draft.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate);
                obj.OperationOffice = draft.dtTbt_RentalContractBasic[0].OperationOfficeCode;
                obj.RegisterChangeEmpNameEN = CommonUtil.TextFullName(CommonUtil.dsTransData.dtUserData.EmpFirstNameEN, CommonUtil.dsTransData.dtUserData.EmpLastNameEN);
                obj.RegisterChangeEmpNameLC = CommonUtil.TextFullName(CommonUtil.dsTransData.dtUserData.EmpFirstNameLC, CommonUtil.dsTransData.dtUserData.EmpLastNameLC);

                EmailTemplateUtil util = new EmailTemplateUtil(EmailTemplateName.C_EMAIL_TEMPLATE_NAME_CHANGE_FEE);
                return util.LoadTemplate(obj);
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// For register maintain contract data (CP-34) (Delete)
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <returns></returns>
        public bool RegisterCP34Delete(dsRentalContractData dsRentalContract)
        {
            ICommonContractHandler commonContractHandler;
            dsRentalContractData dsLastEntireContract;

            string lastOCC;

            try
            {
                commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;
                //1. Delete entire contract with specified OCC
                DeleteEntireOCC(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC, dsRentalContract.dtTbt_RentalContractBasic[0].UpdateDate.Value);

                //2. Get last OCC
                lastOCC = GetLastImplementedOCC(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode);
                if (lastOCC != null)
                {
                    //3. Get entire last contract
                    dsLastEntireContract = GetEntireContract(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, lastOCC);

                    //4. Update summary fields in contract basic
                    dsLastEntireContract = UpdateSummaryFields(ref dsLastEntireContract);

                    //5. Update data for changing
                    dsLastEntireContract.dtTbt_RentalContractBasic[0].ReviseSecurityBasicProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime; //DateTime.Now;
                    dsLastEntireContract.dtTbt_RentalContractBasic[0].LastOCC = dsLastEntireContract.dtTbt_RentalSecurityBasic[0].OCC;
                    dsLastEntireContract.dtTbt_RentalContractBasic[0].LastChangeType = dsLastEntireContract.dtTbt_RentalSecurityBasic[0].ChangeType;
                    dsLastEntireContract.dtTbt_RentalContractBasic[0].LastNormalContractFee = dsLastEntireContract.dtTbt_RentalSecurityBasic[0].NormalContractFee;
                    dsLastEntireContract.dtTbt_RentalContractBasic[0].LastOrderContractFee = dsLastEntireContract.dtTbt_RentalSecurityBasic[0].OrderContractFee;

                    //2. Update entire contract with specified OCC
                    UpdateEntireContract(dsLastEntireContract);
                }

                //6. Delete notify email
                commonContractHandler.DeleteTbt_ContractEmailUnsentContractEmail(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, ContractEmailType.C_CONTRACT_EMAIL_TYPE_NOTIFY_CHANGE_FEE, FlagType.C_FLAG_OFF);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// For register maintain contract data (CP-34) (Insert)
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <returns></returns>
        public bool RegisterCP34Insert(dsRentalContractData dsRentalContract)
        {
            try
            {
                //1. Update data for changing
                //1.1.	dtTbt_RentalContractBasic
                if (dsRentalContract.dtTbt_RentalContractBasic != null && dsRentalContract.dtTbt_RentalContractBasic.Count > 0)
                {
                    dsRentalContract.dtTbt_RentalContractBasic[0].ReviseSecurityBasicProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime; //DateTime.Now;
                }

                //Add by Jutarat A. on 04102012
                //1.2.	dtTbt_RentalSecurityBasic
                if (dsRentalContract.dtTbt_RentalSecurityBasic != null && dsRentalContract.dtTbt_RentalSecurityBasic.Count > 0)
                {
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationTargetCode = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].QuotationAlphabet = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].NormalAdditionalDepositFee = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].OrderAdditionalDepositFee = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationCompleteFlag = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].InstallationSlipNo = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].PlannerEmpNo = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanCheckerEmpNo = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanCheckDate = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanApproverEmpNo = null;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].PlanApproveDate = null;
                }
                //End Add

                //2. Insert entire contract with specified OCC
                //InsertEntireContract(dsRentalContract);
                this.InsertEntireContractForCTS010(dsRentalContract); //Modify by Jutarat A. on 19092013

                #region Update Rental Inst sub contractor

                DeleteTbt_RentalInstSubContractor_ByContractCodeOCC(
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode,
                        dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);

                if (dsRentalContract.dtTbt_RentalInstSubcontractor != null)
                {
                    foreach (tbt_RentalInstSubcontractor inst in dsRentalContract.dtTbt_RentalInstSubcontractor)
                    {
                        InsertTbt_RentalInstSubContractor(inst);
                    }
                }

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save email for sending again later
        /// </summary>
        /// <param name="listdtEmailAddress"></param>
        public void SaveEmailForSendLater(List<tbt_ContractEmail> listdtEmailAddress)
        {
            try
            {
                IContractHandler contractHandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;
                contractHandler.InsertTbt_ContractEmail(listdtEmailAddress);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save email for sending again later
        /// </summary>
        /// <param name="listContractEmail"></param>
        /// <param name="dsRentalContract"></param>
        /// <param name="template"></param>
        public void SaveEmailForSendLater(List<tbt_ContractEmail> listContractEmail, dsRentalContractData dsRentalContract, doEmailTemplate template)
        {
            try
            {
                IContractHandler contractHandler = ServiceContainer.GetService<IContractHandler>() as IContractHandler;

                if (listContractEmail != null)
                {
                    foreach (tbt_ContractEmail ce in listContractEmail)
                    {
                        ce.ContractCode = dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode;
                        ce.OCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
                        ce.EmailType = ContractEmailType.C_CONTRACT_EMAIL_TYPE_NOTIFY_CHANGE_FEE;

                        ce.EmailFrom = CommonUtil.dsTransData.dtUserData.EmailAddress;
                        ce.EmailSubject = template.TemplateSubject;
                        ce.EmailContent = template.TemplateContent;

                        ce.SendDate = dsRentalContract.dtTbt_RentalSecurityBasic[0].ReturnToOriginalFeeDate.Value.AddDays(-(NotifyType.C_SEND_NOTIFY_THRESHOLD));
                        ce.SendFlag = FlagType.C_FLAG_OFF;
                    }

                    contractHandler.InsertTbt_ContractEmail(listContractEmail);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Merge billing client
        /// </summary>
        /// <returns></returns>
        public string ManageBillingClient()
        {
            try
            {
                return "0000000003";
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Update contract data
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <returns></returns>
        public int UpdateContract(ref dsRentalContractData dsRentalContract)
        {
            int iNewCounter;

            try
            {
                iNewCounter = GenerateContractCounter(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode);
                dsRentalContract.dtTbt_RentalSecurityBasic[0].CounterNo = iNewCounter;
                UpdateSummaryFields(ref dsRentalContract);
                UpdateEntireContract(dsRentalContract);

                return iNewCounter;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert contract data
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <param name="dtBillingTemp"></param>
        /// <param name="bImplementFlag"></param>
        /// <returns></returns>
        public int InsertContract(ref dsRentalContractData dsRentalContract, ref List<dtBillingTempChangePlanData> dtBillingTemp, bool bImplementFlag)
        {
            int iNewCounter = 0; //Fixed to 0 for new OCC
            string strNewOCC;
            List<tbt_RentalSecurityBasic> listRentalSecurityBasic;

            try
            {
                listRentalSecurityBasic = dsRentalContract.dtTbt_RentalSecurityBasic;
                ResetContractForNewOCC(ref listRentalSecurityBasic);
                strNewOCC = GenerateContractOCC(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, bImplementFlag);

                // 3.3 Set new OCC
                dsRentalContract = ChangeAllOCCInDSRentalContract(ref dsRentalContract, strNewOCC);

                //-------------------------------------------------------------------

                foreach (var item in dtBillingTemp)
                {
                    item.OCC = strNewOCC;
                }

                if (dsRentalContract.dtTbt_RentalSecurityBasic.Count() != 0 && dsRentalContract.dtTbt_RentalContractBasic.Count() != 0)
                {
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].ImplementFlag = bImplementFlag;
                    dsRentalContract.dtTbt_RentalSecurityBasic[0].CounterNo = iNewCounter;
                    UpdateSummaryFields(ref dsRentalContract);
                    //InsertEntireContract(dsRentalContract);
                    this.InsertEntireContractForCTS010(dsRentalContract); //Modify by Jutarat A. on 19092013
                }

                return iNewCounter;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Reset contract for new occurrence
        /// </summary>
        /// <param name="listRentalSecurityBasic"></param>
        public void ResetContractForNewOCC(ref List<tbt_RentalSecurityBasic> listRentalSecurityBasic)
        {
            try
            {
                foreach (var item in listRentalSecurityBasic)
                {
                    //item.InstallationCompleteFlag = null;
                    item.InstallationCompleteDate = null;
                    item.InstallationTypeCode = null;
                    item.InstallationSlipNo = null;
                    item.InstallationCompleteEmpNo = null;

                    //Merge at 14032017 By Pachara S.
                    item.DocAuditResult = null;
                    item.DocAuditResultName = null;
                    item.DocumentCode = null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update entire contract
        /// </summary>
        /// <param name="dsRentalContract"></param>
        public void UpdateEntireContract(dsRentalContractData dsRentalContract)
        {
            List<tbt_RentalContractBasic> listRentalContractBasic;
            List<tbt_RentalSecurityBasic> listRentalSecurityBasic;
            List<tbt_RentalSentryGuard> listRentalSentryGuard;

            ICommonContractHandler commonContractHandler;

            try
            {
                commonContractHandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;

                //-----------------------------------------------------------------------------------------------
                //Update Tbt_RentalContractBasic
                listRentalContractBasic = this.GetTbt_RentalContractBasic(dsRentalContract.dtTbt_RentalContractBasic[0].ContractCode, null);
                if (listRentalContractBasic.Count > 0)
                    dsRentalContract.dtTbt_RentalContractBasic = this.UpdateTbt_RentalContractBasic(dsRentalContract.dtTbt_RentalContractBasic[0]);
                else
                    dsRentalContract.dtTbt_RentalContractBasic = this.InsertTbt_RentalContractBasic(dsRentalContract.dtTbt_RentalContractBasic[0]);

                //Update Tbt_RentalSecurityBasic
                listRentalSecurityBasic = this.GetTbt_RentalSecurityBasic(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);
                if (listRentalSecurityBasic.Count > 0)
                    dsRentalContract.dtTbt_RentalSecurityBasic = this.UpdateTbt_RentalSecurityBasic(dsRentalContract.dtTbt_RentalSecurityBasic[0]);
                else
                    dsRentalContract.dtTbt_RentalSecurityBasic = this.InsertTbt_RentalSecurityBasic(dsRentalContract.dtTbt_RentalSecurityBasic[0]);

                //Update Tbt_RentalBEDetails
                var delBEDLst = this.DeleteTbt_RentalBEDetails_ByKey(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);

                //Insert Log
                if (delBEDLst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_BE_DET;
                    logData.TableData = CommonUtil.ConvertToXml(delBEDLst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                foreach (var item in dsRentalContract.dtTbt_RentalBEDetails)
                {
                    this.InsertTbt_RentalBEDetails(item);
                }

                //Update Tbt_RentalInstrumentDetails
                var delInstrumentLst = this.DeleteTbt_RentalInstrumentDetails_ByContractCodeOCC(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode
                     , dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);

                //Insert Log
                if (delInstrumentLst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_INST;
                    logData.TableData = CommonUtil.ConvertToXml(delInstrumentLst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                foreach (var item in dsRentalContract.dtTbt_RentalInstrumentDetails)
                {
                    this.InsertTbt_RentalInstrumentDetails(item);
                }

                //Update Tbt_RentalSentryGuard
                listRentalSentryGuard = this.GetTbt_RentalSentryGuard(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);
                if (listRentalSentryGuard.Count > 0)
                {
                    if (dsRentalContract.dtTbt_RentalSentryGuard.Count() != 0)
                        dsRentalContract.dtTbt_RentalSentryGuard = this.UpdateTbt_RentalSentryGuard(dsRentalContract.dtTbt_RentalSentryGuard[0]);
                }
                else
                {
                    if (dsRentalContract.dtTbt_RentalSentryGuard.Count() != 0)
                        dsRentalContract.dtTbt_RentalSentryGuard = this.InsertTbt_RentalSentryGuard(dsRentalContract.dtTbt_RentalSentryGuard[0]);
                }

                //Update Tbt_RentalSentryGuardDetails
                var delRentSecuLst = this.DeleteTbt_RentalSentryguardDetails_ByContractCodeOCC(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);

                //Insert Log
                if (delRentSecuLst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_SG_DET;
                    logData.TableData = CommonUtil.ConvertToXml(delRentSecuLst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                foreach (var item in dsRentalContract.dtTbt_RentalSentryGuardDetails)
                {
                    this.InsertTbt_RentalSentryGuardDetails(item);
                }

                //Update TbtCancelContractMemo
                var delCancContMemoLst = this.DeleteTbt_CancelContractMemo_ByKey(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);

                if (delCancContMemoLst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_CAN_ContractMemo;
                    logData.TableData = CommonUtil.ConvertToXml(delCancContMemoLst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                foreach (var item in dsRentalContract.dtTbt_CancelContractMemo)
                {
                    this.InsertTbtCancelContractMemo(item);
                }

                //Update TbtCancelContractMemoDetail
                var delCancContMemoDetLst = this.DeleteTbt_CancelContractMemoDetail_ByContractCodeOCC(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode
                    , dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);

                if (delCancContMemoDetLst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_CAN_ContractMemo_Detail;
                    logData.TableData = CommonUtil.ConvertToXml(delCancContMemoDetLst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                foreach (var item in dsRentalContract.dtTbt_CancelContractMemoDetail)
                {
                    this.InsertTbtCancelContractMemoDetail(item);
                }

                //Update Tbt_RentalOperationType
                var delRentOpeLst = this.DeleteTbtRentalOperationTypeByKey(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);

                if (delRentOpeLst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_OPER_TYPE;
                    logData.TableData = CommonUtil.ConvertToXml(delRentOpeLst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                foreach (var item in dsRentalContract.dtTbt_RentalOperationType)
                {
                    this.InsertTbt_RentalOperationType(item);
                }

                //Update Tbt_RentalMaintenanceDetails
                var delRentMntDetLst = this.DeleteTbt_RentalMaintenanceDetails_ByKey(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);

                if (delRentMntDetLst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RNT_MA;
                    logData.TableData = CommonUtil.ConvertToXml(delRentMntDetLst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                foreach (var item in dsRentalContract.dtTbt_RentalMaintenanceDetails)
                {
                    this.InsertTbt_RentalMaintenanceDetails(item);
                }

                //Update TbtRelationType
                var delRelTypeLst = this.DeleteTbt_RelationType_ByContractCodeOCC(dsRentalContract.dtTbt_RentalSecurityBasic[0].ContractCode, dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC);

                if (delRelTypeLst.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_RELATION_TYPE;
                    logData.TableData = CommonUtil.ConvertToXml(delRelTypeLst);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                foreach (var item in dsRentalContract.dtTbt_RelationType)
                {
                    this.InsertTbtRelationType(item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update billing temp by key XML
        /// </summary>
        /// <param name="tbtBillingTemp"></param>
        public void UpdateTbt_BillingTempByKeyXML(tbt_BillingTemp tbtBillingTemp)
        {
            try
            {
                //List<tbt_BillingTemp> rList = this.GetBillingDetailData(dsRental.dtTbt_RentalSecurityBasic[0].ContractCode, dsRental.dtTbt_RentalSecurityBasic[0].OCC);

                //if (rList.Count != 0)
                //{
                //if (rList[0].UpdateDate != dsRental.dtTbt_RentalSecurityBasic[0].UpdateDate)
                List<tbt_BillingTemp> listDTBillingTemp = new List<tbt_BillingTemp>();
                listDTBillingTemp.Add(tbtBillingTemp);
                List<tbt_BillingTemp> updatedList = base.UpdateTbt_BillingTempByKeyXML(CommonUtil.ConvertToXml_Store<tbt_BillingTemp>(listDTBillingTemp));

                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the contract fee before change fee
        /// </summary>
        /// <param name="paramContractCode"></param>
        /// <param name="paramOCC"></param>
        /// <param name="dsRentalContract"></param>
        /// <returns></returns>
        public decimal? GetContractFeeBeforeChange(string paramContractCode, string paramOCC, dsRentalContractData dsRentalContract)
        {
            string prevOCC;
            string changeFeeOCC = "";
            List<tbt_RentalSecurityBasic> listRentalSecurityBasic;

            try
            {
                //prevOCC = GetLastImplementedOCC(paramContractCode);

                var rentalObjLst = GetTbt_RentalSecurityBasic(paramContractCode, null).OrderBy(x => x.OCC).ToList();

                for (int i = 0; i < rentalObjLst.Count; i++)
                {
                    if ((rentalObjLst[i].ImplementFlag.GetValueOrDefault() == FlagType.C_FLAG_ON)
                        && ((rentalObjLst[i].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE)
                        || (rentalObjLst[i].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE_DURING_STOP)))
                    {
                        if (((i + 1) < rentalObjLst.Count)
                            && (((rentalObjLst[i + 1].ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE)
                            && (rentalObjLst[i + 1].ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE_DURING_STOP))
                            || ((rentalObjLst[i + 1].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE_DURING_STOP
                                || rentalObjLst[i + 1].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE)
                                && rentalObjLst[i + 1].ReturnToOriginalFeeDate == null)))
                        {
                            if (dsRentalContract.dtTbt_RentalContractBasic[0].ContractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING)
                            {
                                return rentalObjLst[i + 1].ContractFeeOnStop;
                            }
                            else
                            {
                                return rentalObjLst[i + 1].OrderContractFee;
                            }
                        }
                    }
                }

                //while (prevOCC != null && prevOCC != "")
                //{
                //    listRentalSecurityBasic = GetTbt_RentalSecurityBasic(paramContractCode, paramOCC);
                //    if (listRentalSecurityBasic[0].ChangeType == RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE)
                //    {
                //        changeFeeOCC = dsRentalContract.dtTbt_RentalSecurityBasic[0].OCC;
                //        break;
                //    }
                //    else
                //    {
                //        prevOCC = GetPreviousImplementedOCC(paramContractCode, paramOCC, FlagType.C_FLAG_ON)[0];
                //    }
                //}

                //if (changeFeeOCC != "")
                //{
                //    prevOCC = changeFeeOCC;

                //    string tmpOCC = prevOCC;
                //    while (prevOCC != null && prevOCC != "")
                //    {
                //        listRentalSecurityBasic = GetTbt_RentalSecurityBasic(paramContractCode, paramOCC);

                //        if (listRentalSecurityBasic[0].ChangeType != RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_FEE)
                //            return listRentalSecurityBasic[0].OrderContractFee;
                //        else
                //            prevOCC = GetPreviousImplementedOCC(paramContractCode, paramOCC, FlagType.C_FLAG_ON)[0];

                //        if (tmpOCC != prevOCC)
                //            tmpOCC = prevOCC;
                //        else
                //            break;
                //    }
                //}
            }
            catch (Exception)
            {

                throw;
            }

            return null;
        }

        /// <summary>
        /// Change all occurrence in rental contract
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        public dsRentalContractData ChangeAllOCCInDSRentalContract(ref dsRentalContractData dsRentalContract, string occ)
        {
            tbt_CancelContractMemo tbtCancelContractMemo;
            tbt_CancelContractMemoDetail tbtCancelContractMemoDetail;
            tbt_RelationType tbtRelationType;
            tbt_RentalBEDetails tbtRentalBEDetails;
            tbt_RentalInstrumentDetails tbtRentalInstrumentDetails;
            tbt_RentalInstSubcontractor tbtRentalInstSubcontractor;
            tbt_RentalMaintenanceDetails tbtRentalMaintenanceDetails;
            tbt_RentalOperationType tbtRentalOperationType;
            tbt_RentalSecurityBasic tbtRentalSecurityBasic;
            tbt_RentalSentryGuard tbtRentalSentryGuard;
            tbt_RentalSentryGuardDetails tbtRentalSentryGuardDetails;
            tbt_RentalContractBasic tbtRentalContractBasic;

            dsRentalContractData dsRentalContractNew = new dsRentalContractData();
            try
            {
                if (dsRentalContract.dtTbt_CancelContractMemo != null)
                {
                    dsRentalContractNew.dtTbt_CancelContractMemo = new List<tbt_CancelContractMemo>();
                    foreach (var item in dsRentalContract.dtTbt_CancelContractMemo)
                    {
                        tbtCancelContractMemo = CommonUtil.CloneObject<tbt_CancelContractMemo, tbt_CancelContractMemo>(item);
                        tbtCancelContractMemo.OCC = occ;
                        dsRentalContractNew.dtTbt_CancelContractMemo.Remove(item);
                        dsRentalContractNew.dtTbt_CancelContractMemo.Add(tbtCancelContractMemo);
                    }
                }

                if (dsRentalContract.dtTbt_CancelContractMemoDetail != null)
                {
                    dsRentalContractNew.dtTbt_CancelContractMemoDetail = new List<tbt_CancelContractMemoDetail>();
                    foreach (var item in dsRentalContract.dtTbt_CancelContractMemoDetail)
                    {
                        tbtCancelContractMemoDetail = CommonUtil.CloneObject<tbt_CancelContractMemoDetail, tbt_CancelContractMemoDetail>(item);
                        tbtCancelContractMemoDetail.OCC = occ;
                        dsRentalContractNew.dtTbt_CancelContractMemoDetail.Remove(item);
                        dsRentalContractNew.dtTbt_CancelContractMemoDetail.Add(tbtCancelContractMemoDetail);
                    }
                }

                if (dsRentalContract.dtTbt_RelationType != null)
                {
                    dsRentalContractNew.dtTbt_RelationType = new List<tbt_RelationType>();
                    foreach (var item in dsRentalContract.dtTbt_RelationType)
                    {
                        tbtRelationType = CommonUtil.CloneObject<tbt_RelationType, tbt_RelationType>(item);
                        tbtRelationType.OCC = occ;
                        dsRentalContractNew.dtTbt_RelationType.Remove(item);
                        dsRentalContractNew.dtTbt_RelationType.Add(tbtRelationType);
                    }
                }

                if (dsRentalContract.dtTbt_RentalBEDetails != null)
                {
                    dsRentalContractNew.dtTbt_RentalBEDetails = new List<tbt_RentalBEDetails>();
                    foreach (var item in dsRentalContract.dtTbt_RentalBEDetails)
                    {
                        tbtRentalBEDetails = CommonUtil.CloneObject<tbt_RentalBEDetails, tbt_RentalBEDetails>(item);
                        tbtRentalBEDetails.OCC = occ;
                        dsRentalContractNew.dtTbt_RentalBEDetails.Remove(item);
                        dsRentalContractNew.dtTbt_RentalBEDetails.Add(tbtRentalBEDetails);
                    }
                }

                if (dsRentalContract.dtTbt_RentalInstrumentDetails != null)
                {
                    dsRentalContractNew.dtTbt_RentalInstrumentDetails = new List<tbt_RentalInstrumentDetails>();
                    foreach (var item in dsRentalContract.dtTbt_RentalInstrumentDetails)
                    {
                        if (item.AdditionalInstrumentQty == null)
                        {
                            string a = "";
                        }

                        tbtRentalInstrumentDetails = CommonUtil.CloneObject<tbt_RentalInstrumentDetails, tbt_RentalInstrumentDetails>(item);
                        tbtRentalInstrumentDetails.OCC = occ;
                        dsRentalContractNew.dtTbt_RentalInstrumentDetails.Remove(item);
                        dsRentalContractNew.dtTbt_RentalInstrumentDetails.Add(tbtRentalInstrumentDetails);
                    }
                }

                if (dsRentalContract.dtTbt_RentalInstSubcontractor != null)
                {
                    dsRentalContractNew.dtTbt_RentalInstSubcontractor = new List<tbt_RentalInstSubcontractor>();
                    foreach (var item in dsRentalContract.dtTbt_RentalInstSubcontractor)
                    {
                        tbtRentalInstSubcontractor = CommonUtil.CloneObject<tbt_RentalInstSubcontractor, tbt_RentalInstSubcontractor>(item);
                        tbtRentalInstSubcontractor.OCC = occ;
                        dsRentalContractNew.dtTbt_RentalInstSubcontractor.Remove(tbtRentalInstSubcontractor);
                        dsRentalContractNew.dtTbt_RentalInstSubcontractor.Add(tbtRentalInstSubcontractor);
                    }
                }

                if (dsRentalContract.dtTbt_RentalMaintenanceDetails != null)
                {
                    dsRentalContractNew.dtTbt_RentalMaintenanceDetails = new List<tbt_RentalMaintenanceDetails>();
                    foreach (var item in dsRentalContract.dtTbt_RentalMaintenanceDetails)
                    {
                        tbtRentalMaintenanceDetails = CommonUtil.CloneObject<tbt_RentalMaintenanceDetails, tbt_RentalMaintenanceDetails>(item);
                        tbtRentalMaintenanceDetails.OCC = occ;
                        dsRentalContractNew.dtTbt_RentalMaintenanceDetails.Remove(tbtRentalMaintenanceDetails);
                        dsRentalContractNew.dtTbt_RentalMaintenanceDetails.Add(tbtRentalMaintenanceDetails);
                    }
                }

                if (dsRentalContract.dtTbt_RentalOperationType != null)
                {
                    dsRentalContractNew.dtTbt_RentalOperationType = new List<tbt_RentalOperationType>();
                    foreach (var item in dsRentalContract.dtTbt_RentalOperationType)
                    {
                        tbtRentalOperationType = CommonUtil.CloneObject<tbt_RentalOperationType, tbt_RentalOperationType>(item);
                        tbtRentalOperationType.OCC = occ;
                        dsRentalContractNew.dtTbt_RentalOperationType.Remove(tbtRentalOperationType);
                        dsRentalContractNew.dtTbt_RentalOperationType.Add(tbtRentalOperationType);
                    }
                }

                if (dsRentalContract.dtTbt_RentalSecurityBasic != null)
                {
                    dsRentalContractNew.dtTbt_RentalSecurityBasic = new List<tbt_RentalSecurityBasic>();
                    foreach (var item in dsRentalContract.dtTbt_RentalSecurityBasic)
                    {
                        tbtRentalSecurityBasic = CommonUtil.CloneObject<tbt_RentalSecurityBasic, tbt_RentalSecurityBasic>(item);
                        tbtRentalSecurityBasic.OCC = occ;
                        dsRentalContractNew.dtTbt_RentalSecurityBasic.Remove(tbtRentalSecurityBasic);
                        dsRentalContractNew.dtTbt_RentalSecurityBasic.Add(tbtRentalSecurityBasic);
                    }
                }

                if (dsRentalContract.dtTbt_RentalSentryGuard != null)
                {
                    dsRentalContractNew.dtTbt_RentalSentryGuard = new List<tbt_RentalSentryGuard>();
                    foreach (var item in dsRentalContract.dtTbt_RentalSentryGuard)
                    {
                        tbtRentalSentryGuard = CommonUtil.CloneObject<tbt_RentalSentryGuard, tbt_RentalSentryGuard>(item);
                        tbtRentalSentryGuard.OCC = occ;
                        dsRentalContractNew.dtTbt_RentalSentryGuard.Remove(tbtRentalSentryGuard);
                        dsRentalContractNew.dtTbt_RentalSentryGuard.Add(tbtRentalSentryGuard);
                    }
                }

                if (dsRentalContract.dtTbt_RentalSentryGuardDetails != null)
                {
                    dsRentalContractNew.dtTbt_RentalSentryGuardDetails = new List<tbt_RentalSentryGuardDetails>();
                    foreach (var item in dsRentalContract.dtTbt_RentalSentryGuardDetails)
                    {
                        tbtRentalSentryGuardDetails = CommonUtil.CloneObject<tbt_RentalSentryGuardDetails, tbt_RentalSentryGuardDetails>(item);
                        tbtRentalSentryGuardDetails.OCC = occ;
                        dsRentalContractNew.dtTbt_RentalSentryGuardDetails.Remove(tbtRentalSentryGuardDetails);
                        dsRentalContractNew.dtTbt_RentalSentryGuardDetails.Add(tbtRentalSentryGuardDetails);
                    }
                }


                if (dsRentalContract.dtTbt_RentalContractBasic != null)
                {
                    dsRentalContractNew.dtTbt_RentalContractBasic = new List<tbt_RentalContractBasic>();
                    foreach (var item in dsRentalContract.dtTbt_RentalContractBasic)
                    {
                        tbtRentalContractBasic = CommonUtil.CloneObject<tbt_RentalContractBasic, tbt_RentalContractBasic>(item);
                        //tbtRentalContractBasic.LastOCC = occ;
                        dsRentalContractNew.dtTbt_RentalContractBasic.Remove(tbtRentalContractBasic);
                        dsRentalContractNew.dtTbt_RentalContractBasic.Add(tbtRentalContractBasic);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }

            return dsRentalContractNew;
        }

        /// <summary>
        /// Get last cancel contract quotation data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public dsCancelContractQuotation GetLastCancelContractQuotation(string strContractCode)
        {
            dsCancelContractQuotation dsCancelContract = new dsCancelContractQuotation();
            List<tbt_CancelContractMemoDetail> tbt_CancelContractMemoDetailList = new List<tbt_CancelContractMemoDetail>();
            MiscTypeMappingList miscList = new MiscTypeMappingList();

            try
            {
                List<tbt_CancelContractMemo> tbt_CancelContractMemoList = base.GetLastCancelContractMemo(strContractCode, true);
                foreach (tbt_CancelContractMemo dtMemo in tbt_CancelContractMemoList)
                {
                    List<tbt_CancelContractMemoDetail> tbt_CancelContractMemoDetailTemp = base.GetTbt_CancelContractMemoDetail(strContractCode, dtMemo.OCC);
                    foreach (tbt_CancelContractMemoDetail dtMemoDetail in tbt_CancelContractMemoDetailTemp)
                    {
                        tbt_CancelContractMemoDetailList.Add(dtMemoDetail);
                        miscList.AddMiscType(dtMemoDetail);
                    }

                    miscList.AddMiscType(dtMemo);
                }

                ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                comHandler.MiscTypeMappingList(miscList);

                dsCancelContract.dtTbt_CancelContractMemo = tbt_CancelContractMemoList;
                dsCancelContract.dtTbt_CancelContractMemoDetail = tbt_CancelContractMemoDetailList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dsCancelContract;
        }

        /// <summary>
        /// Get contract doc template data
        /// </summary>
        /// <param name="strDocumentCode"></param>
        /// <returns></returns>
        public List<tbs_ContractDocTemplate> GetTbsContractDocTemplate(string strDocumentCode)
        {
            return base.GetTbs_ContractDocTemplate(strDocumentCode);
        }

        /// <summary>
        /// Gete cancel contract quotationdata
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public dsCancelContractQuotation GetCancelContractQuotation(string strContractCode, string strOCC)
        {
            dsCancelContractQuotation dsCancelContract = new dsCancelContractQuotation();
            List<tbt_CancelContractMemo> tbt_CancelContractMemoList = new List<tbt_CancelContractMemo>();
            List<tbt_CancelContractMemoDetail> tbt_CancelContractMemoDetailList = new List<tbt_CancelContractMemoDetail>();
            MiscTypeMappingList miscList = new MiscTypeMappingList();

            try
            {
                tbt_CancelContractMemoList = base.GetTbt_CancelContractMemo(strContractCode, strOCC);
                foreach (tbt_CancelContractMemo dtMemo in tbt_CancelContractMemoList)
                {
                    List<tbt_CancelContractMemoDetail> tbt_CancelContractMemoDetailTemp = base.GetTbt_CancelContractMemoDetail(strContractCode, dtMemo.OCC);
                    foreach (tbt_CancelContractMemoDetail dtMemoDetail in tbt_CancelContractMemoDetailTemp)
                    {
                        tbt_CancelContractMemoDetailList.Add(dtMemoDetail);
                        miscList.AddMiscType(dtMemoDetail);
                    }

                    miscList.AddMiscType(dtMemo);
                }

                ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                comHandler.MiscTypeMappingList(miscList);

                dsCancelContract.dtTbt_CancelContractMemo = tbt_CancelContractMemoList;
                dsCancelContract.dtTbt_CancelContractMemoDetail = tbt_CancelContractMemoDetailList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dsCancelContract;
        }

        /// <summary>
        /// Insert cancel contract memo
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_CancelContractMemo> InsertTbtCancelContractMemo(tbt_CancelContractMemo doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_CancelContractMemo> doInsertList = new List<tbt_CancelContractMemo>();
                doInsertList.Add(doInsert);
                List<tbt_CancelContractMemo> insertList = base.InsertTbt_CancelContractMemo(CommonUtil.ConvertToXml_Store<tbt_CancelContractMemo>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_CAN_ContractMemo;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create contract memo when register cancel contract
        /// </summary>
        /// <param name="dsCancelContract"></param>
        /// <returns></returns>
        public dsCancelContractQuotation CreateCancelContractMemo(dsCancelContractQuotation dsCancelContract)
        {
            dsCancelContractQuotation dsCancelContractResult = new dsCancelContractQuotation(); ;
            List<tbt_CancelContractMemo> cancelContractMemoList;
            List<tbt_CancelContractMemoDetail> cancelContractMemoDetailList;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //1.	Insert cancel contract memo
                    if (dsCancelContract.dtTbt_CancelContractMemo != null)
                    {
                        cancelContractMemoList = new List<tbt_CancelContractMemo>();
                        foreach (tbt_CancelContractMemo data in dsCancelContract.dtTbt_CancelContractMemo)
                        {
                            cancelContractMemoList.AddRange(InsertTbtCancelContractMemo(data));
                        }

                        dsCancelContractResult.dtTbt_CancelContractMemo = cancelContractMemoList;
                    }

                    //2.	Insert cancel contract memo detail
                    if (dsCancelContract.dtTbt_CancelContractMemoDetail != null)
                    {
                        cancelContractMemoDetailList = new List<tbt_CancelContractMemoDetail>();
                        foreach (tbt_CancelContractMemoDetail data in dsCancelContract.dtTbt_CancelContractMemoDetail)
                        {
                            cancelContractMemoDetailList.AddRange(InsertTbtCancelContractMemoDetail(data));
                        }

                        dsCancelContractResult.dtTbt_CancelContractMemoDetail = cancelContractMemoDetailList;
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dsCancelContractResult;
        }

        /// <summary>
        /// Delete cancel contract memo data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOcc"></param>
        /// <returns></returns>
        public bool DeleteCancelContractMemo(string strContractCode, string strOcc)
        {
            bool blnDeleteSuccess = false;

            try
            {
                DeleteTbtCancelContractMemoDetailByContractCodeOCC(strContractCode, strOcc);
                DeleteTbtCancelContractMemoByKey(strContractCode, strOcc);

                blnDeleteSuccess = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return blnDeleteSuccess;
        }

        /// <summary>
        /// Get the billing basic which pay the maintenance fee in result based. Null is returned if there is many billing target or no billing target
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        public tbt_BillingBasic GetBillingBasicForMAResultBasedFeePayment(string ContractCode)
        {
            try
            {
                #region Check mandatory

                doBillingBasicForMAResultBasedFeePayment cond = new doBillingBasicForMAResultBasedFeePayment()
                {
                    ContractCode = ContractCode
                };
                ApplicationErrorException.CheckMandatoryField(cond);

                #endregion

                IBillingHandler billHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                List<tbt_BillingBasic> billingList = billHandler.GetTbt_BillingBasic(ContractCode, null);

                List<tbt_BillingBasic> lst = billingList.FindAll(i => (i.ResultBasedMaintenanceFlag == FlagType.C_FLAG_ON && i.MonthlyBillingAmount > 0));

                if (lst.Count == 1)
                    return lst[0];
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region CreateContractDocData

        /// <summary>
        /// Insert contract document data
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_ContractDocument> InsertTbtContractDocument(tbt_ContractDocument doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_ContractDocument> doInsertList = new List<tbt_ContractDocument>();
                doInsertList.Add(doInsert);
                List<tbt_ContractDocument> insertList = base.InsertTbt_ContractDocument(CommonUtil.ConvertToXml_Store<tbt_ContractDocument>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_CONTRACT_DOC;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert document cancel contract memo
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_DocCancelContractMemo> InsertTbtDocCancelContractMemo(tbt_DocCancelContractMemo doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_DocCancelContractMemo> doInsertList = new List<tbt_DocCancelContractMemo>();
                doInsertList.Add(doInsert);
                List<tbt_DocCancelContractMemo> insertList = base.InsertTbt_DocCancelContractMemo(CommonUtil.ConvertToXml_Store<tbt_DocCancelContractMemo>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DOC_CAN_CONTRACT_MEMO;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert document cancel contract memo detail
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_DocCancelContractMemoDetail> InsertTbtDocCancelContractMemoDetail(tbt_DocCancelContractMemoDetail doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_DocCancelContractMemoDetail> doInsertList = new List<tbt_DocCancelContractMemoDetail>();
                doInsertList.Add(doInsert);
                List<tbt_DocCancelContractMemoDetail> insertList = base.InsertTbt_DocCancelContractMemoDetail(CommonUtil.ConvertToXml_Store<tbt_DocCancelContractMemoDetail>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DOC_CAN_CONTRAL_MEMO_DET;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert document change memo
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_DocChangeMemo> InsertTbtDocChangeMemo(tbt_DocChangeMemo doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_DocChangeMemo> doInsertList = new List<tbt_DocChangeMemo>();
                doInsertList.Add(doInsert);
                List<tbt_DocChangeMemo> insertList = base.InsertTbt_DocChangeMemo(CommonUtil.ConvertToXml_Store<tbt_DocChangeMemo>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DOC_CHANGE_MEMO;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert document change notice
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_DocChangeNotice> InsertTbtDocChangeNotice(tbt_DocChangeNotice doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_DocChangeNotice> doInsertList = new List<tbt_DocChangeNotice>();
                doInsertList.Add(doInsert);
                List<tbt_DocChangeNotice> insertList = base.InsertTbt_DocChangeNotice(CommonUtil.ConvertToXml_Store<tbt_DocChangeNotice>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DOC_CHANGE_NOTICE;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert document change fee memo
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_DocChangeFeeMemo> InsertTbtDocChangeFeeMemo(tbt_DocChangeFeeMemo doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_DocChangeFeeMemo> doInsertList = new List<tbt_DocChangeFeeMemo>();
                doInsertList.Add(doInsert);
                List<tbt_DocChangeFeeMemo> insertList = base.InsertTbt_DocChangeFeeMemo(CommonUtil.ConvertToXml_Store<tbt_DocChangeFeeMemo>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DOC_CHANGE_FREE_MEMO;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert document contract report
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_DocContractReport> InsertTbtDocContractReport(tbt_DocContractReport doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_DocContractReport> doInsertList = new List<tbt_DocContractReport>();
                doInsertList.Add(doInsert);
                List<tbt_DocContractReport> insertList = base.InsertTbt_DocContractReport(CommonUtil.ConvertToXml_Store<tbt_DocContractReport>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DOC_CONTRACT_RPT;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert document confirm current instrument memo
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_DocConfirmCurrentInstrumentMemo> InsertTbtDocConfirmCurrentInstrumentMemo(tbt_DocConfirmCurrentInstrumentMemo doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_DocConfirmCurrentInstrumentMemo> doInsertList = new List<tbt_DocConfirmCurrentInstrumentMemo>();
                doInsertList.Add(doInsert);
                List<tbt_DocConfirmCurrentInstrumentMemo> insertList = base.InsertTbt_DocConfirmCurrentInstrumentMemo(CommonUtil.ConvertToXml_Store<tbt_DocConfirmCurrentInstrumentMemo>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DOC_CONF_CUR_INSTUMENT_MEMO;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert document start memo
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_DocStartMemo> InsertTbtDocStartMemo(tbt_DocStartMemo doInsert) //Add by Jutarat A. on 22042013
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_DocStartMemo> doInsertList = new List<tbt_DocStartMemo>();
                doInsertList.Add(doInsert);
                List<tbt_DocStartMemo> insertList = base.InsertTbt_DocStartMemo(CommonUtil.ConvertToXml_Store<tbt_DocStartMemo>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DOC_START_MEMO;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Insert focument instrument detail
        /// </summary>
        /// <param name="doInsert"></param>
        /// <returns></returns>
        public List<tbt_DocInstrumentDetails> InsertTbtDocInstrumentDetail(tbt_DocInstrumentDetails doInsert)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doInsert.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInsert.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doInsert.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_DocInstrumentDetails> doInsertList = new List<tbt_DocInstrumentDetails>();
                doInsertList.Add(doInsert);
                List<tbt_DocInstrumentDetails> insertList = base.InsertTbt_DocInstrumentDetails(CommonUtil.ConvertToXml_Store<tbt_DocInstrumentDetails>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DOC_INSTUMENT_DET;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create contract doc data
        /// </summary>
        /// <param name="dsContractDoc"></param>
        /// <returns></returns>
        public dsContractDocData CreateContractDocData(dsContractDocData dsContractDoc)
        {
            int intDocID = 0;
            dsContractDocData dsContractDocTemp;
            dsContractDocData dsContractDocResult = new dsContractDocData();
            List<tbt_ContractDocument> contractDocList = new List<tbt_ContractDocument>();
            List<tbt_DocCancelContractMemo> docCancelContractMemoList;
            List<tbt_DocCancelContractMemoDetail> docCancelContractMemoDetailList;
            List<tbt_DocChangeMemo> docChangeMemoList;
            List<tbt_DocChangeNotice> docChangeNoticeList;
            List<tbt_DocChangeFeeMemo> docChangeFeeMemoList;
            List<tbt_DocContractReport> docContractReportList;
            List<tbt_DocConfirmCurrentInstrumentMemo> docConfirmCurrentInstrumentMemoList;
            List<tbt_DocInstrumentDetails> docInstrumentDetailList;
            List<tbt_DocStartMemo> docStartMemoList; //Add by Jutarat A. on 22042013

            try
            {
                dsContractDocTemp = CommonUtil.CloneObject<dsContractDocData, dsContractDocData>(dsContractDoc);

                using (TransactionScope scope = new TransactionScope())
                {
                    //1.	Insert contract document
                    if (dsContractDocTemp.dtTbt_ContractDocument != null)
                    {
                        contractDocList = new List<tbt_ContractDocument>();
                        foreach (tbt_ContractDocument data in dsContractDocTemp.dtTbt_ContractDocument)
                        {
                            contractDocList.AddRange(InsertTbtContractDocument(data));
                        }

                        dsContractDocResult.dtTbt_ContractDocument = contractDocList;
                    }

                    intDocID = contractDocList.Count > 0 ? contractDocList[0].DocID : 0;

                    //2.	Insert cancel contract memo
                    if (dsContractDocTemp.dtTbt_DocCancelContractMemo != null)
                    {
                        docCancelContractMemoList = new List<tbt_DocCancelContractMemo>();
                        foreach (tbt_DocCancelContractMemo data in dsContractDocTemp.dtTbt_DocCancelContractMemo)
                        {
                            data.DocID = intDocID;
                            docCancelContractMemoList.AddRange(InsertTbtDocCancelContractMemo(data));
                        }

                        dsContractDocResult.dtTbt_DocCancelContractMemo = docCancelContractMemoList;
                    }

                    //3.	Insert cancel contract memo detail
                    if (dsContractDocTemp.dtTbt_DocCancelContractMemoDetail != null)
                    {
                        docCancelContractMemoDetailList = new List<tbt_DocCancelContractMemoDetail>();
                        foreach (tbt_DocCancelContractMemoDetail data in dsContractDocTemp.dtTbt_DocCancelContractMemoDetail)
                        {
                            data.DocID = intDocID;
                            docCancelContractMemoDetailList.AddRange(InsertTbtDocCancelContractMemoDetail(data));
                        }

                        dsContractDocResult.dtTbt_DocCancelContractMemoDetail = docCancelContractMemoDetailList;
                    }

                    //4.	Insert change memo
                    if (dsContractDocTemp.dtTbt_DocChangeMemo != null)
                    {
                        docChangeMemoList = new List<tbt_DocChangeMemo>();
                        foreach (tbt_DocChangeMemo data in dsContractDocTemp.dtTbt_DocChangeMemo)
                        {
                            data.DocID = intDocID;
                            docChangeMemoList.AddRange(InsertTbtDocChangeMemo(data));
                        }

                        dsContractDocResult.dtTbt_DocChangeMemo = docChangeMemoList;
                    }

                    //5.	Insert change notice
                    if (dsContractDocTemp.dtTbt_DocChangeNotice != null)
                    {
                        docChangeNoticeList = new List<tbt_DocChangeNotice>();
                        foreach (tbt_DocChangeNotice data in dsContractDocTemp.dtTbt_DocChangeNotice)
                        {
                            data.DocID = intDocID;
                            docChangeNoticeList.AddRange(InsertTbtDocChangeNotice(data));
                        }

                        dsContractDocResult.dtTbt_DocChangeNotice = docChangeNoticeList;
                    }

                    //6.	Insert change fee memo
                    if (dsContractDocTemp.dtTbt_DocChangeFeeMemo != null)
                    {
                        docChangeFeeMemoList = new List<tbt_DocChangeFeeMemo>();
                        foreach (tbt_DocChangeFeeMemo data in dsContractDocTemp.dtTbt_DocChangeFeeMemo)
                        {
                            data.DocID = intDocID;
                            docChangeFeeMemoList.AddRange(InsertTbtDocChangeFeeMemo(data));
                        }

                        dsContractDocResult.dtTbt_DocChangeFeeMemo = docChangeFeeMemoList;
                    }

                    //7.	Insert contract report
                    if (dsContractDocTemp.dtTbt_DocContractReport != null)
                    {
                        docContractReportList = new List<tbt_DocContractReport>();
                        foreach (tbt_DocContractReport data in dsContractDocTemp.dtTbt_DocContractReport)
                        {
                            data.DocID = intDocID;
                            docContractReportList.AddRange(InsertTbtDocContractReport(data));
                        }

                        dsContractDocResult.dtTbt_DocContractReport = docContractReportList;
                    }

                    //8.	Insert confirm current instrument memo
                    if (dsContractDocTemp.dtTbt_DocConfirmCurrentInstrumentMemo != null)
                    {
                        docConfirmCurrentInstrumentMemoList = new List<tbt_DocConfirmCurrentInstrumentMemo>();
                        foreach (tbt_DocConfirmCurrentInstrumentMemo data in dsContractDocTemp.dtTbt_DocConfirmCurrentInstrumentMemo)
                        {
                            data.DocID = intDocID;
                            docConfirmCurrentInstrumentMemoList.AddRange(InsertTbtDocConfirmCurrentInstrumentMemo(data));
                        }

                        dsContractDocResult.dtTbt_DocConfirmCurrentInstrumentMemo = docConfirmCurrentInstrumentMemoList;
                    }

                    //Add by Jutarat A. on 22042013
                    //9.    Insert start memo
                    if (dsContractDocTemp.dtTbt_DocStartMemo != null)
                    {
                        docStartMemoList = new List<tbt_DocStartMemo>();
                        foreach (tbt_DocStartMemo data in dsContractDocTemp.dtTbt_DocStartMemo)
                        {
                            data.DocID = intDocID;
                            docStartMemoList.AddRange(InsertTbtDocStartMemo(data));
                        }

                        dsContractDocResult.dtTbt_DocStartMemo = docStartMemoList;
                    }
                    //End Add

                    //10.	Insert instrument detail
                    if (dsContractDocTemp.dtTbt_DocInstrumentDetail != null)
                    {
                        docInstrumentDetailList = new List<tbt_DocInstrumentDetails>();
                        foreach (tbt_DocInstrumentDetails data in dsContractDocTemp.dtTbt_DocInstrumentDetail)
                        {
                            data.DocID = intDocID;
                            docInstrumentDetailList.AddRange(InsertTbtDocInstrumentDetail(data));
                        }

                        dsContractDocResult.dtTbt_DocInstrumentDetail = docInstrumentDetailList;
                    }

                    scope.Complete();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dsContractDocResult;
        }

        #endregion

        /// <summary>
        /// In case rental contract start service, calculate all fee of umimplement 
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public List<doSummaryFee> SumFeeUnimplementData(string strContractCode)
        {
            try
            {
                List<doSummaryFee> summaryFeeList = base.SumFeeUnimplementData(strContractCode);
                if (summaryFeeList == null)
                    summaryFeeList = new List<doSummaryFee>();

                return summaryFeeList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get rental contract basic information for display on Installation page
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBuildingType"></param>
        /// <returns></returns>
        public dtRentalContractBasicForInstall GetRentalContractBasicDataForInstall(string strContractCode, string strBuildingType)
        {
            try
            {
                List<dtRentalContractBasicForInstall> dtRentalContractBasicData = base.GetRentalContractBasicForInstall(strContractCode, strBuildingType
                                                                                    , RentalChangeType.C_RENTAL_CHANGE_TYPE_ALTERNATIVE_START
                                                                                    , SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL
                                                                                    , SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_US); //Add by Jutarat A. on 15112013
                if (dtRentalContractBasicData.Count != 0)
                {
                    return dtRentalContractBasicData[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check cancel contract before start service
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public bool CheckCancelContractBeforeStartService(string strContractCode)
        {
            try
            {

                if (strContractCode != null)
                {
                    List<CheckCancelContractBeforeStartService_Result> result = base.CheckCancelContractBeforeStartService(strContractCode, ContractStatus.C_CONTRACT_STATUS_END, ContractStatus.C_CONTRACT_STATUS_CANCEL, ContractStatus.C_CONTRACT_STATUS_FIXED_CANCEL);
                    if (result.Count > 0)
                    {
                        return Convert.ToBoolean(result[0].blnCancelContract);
                    }
                    else
                    {
                        return false;

                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check rental contract basic is CP-12 is registered
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strLastImplementOCC"></param>
        /// <returns></returns>
        public bool CheckCP12(string strContractCode, string strLastImplementOCC)
        {
            try
            {

                if (strLastImplementOCC != null)
                {
                    List<CheckCP12_Result> result = base.CheckCP12(strContractCode, strLastImplementOCC, RentalChangeType.C_RENTAL_CHANGE_TYPE_PLAN_CHANGE, RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_INSTRU_DURING_STOP);
                    if (result.Count > 0)
                    {
                        return Convert.ToBoolean(result[0].blnCheckCP12);
                    }
                    else
                    {
                        return false;

                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get max update date of MA target contract for checking with the create date of quotation when register change plan
        /// </summary>
        /// <param name="paramMAContractCode"></param>
        /// <param name="paramOCC"></param>
        /// <returns></returns>
        public List<DateTime?> GetMaxUpdateDateOfMATargetContract(string paramMAContractCode, string paramOCC)
        {
            return base.GetMaxUpdateDateOfMATargetContract(paramMAContractCode
                , paramOCC
                , RentalChangeType.C_RENTAL_CHANGE_TYPE_CHANGE_NAME
                , RentalChangeType.C_RENTAL_CHANGE_TYPE_MOVE_INSTRU);
        }

        /// <summary>
        /// Get billing temp from both billing temp table and billing basic
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public List<dtBillingTempChangePlanData> GetBillingTempForChangePlan(string strContractCode, string strOCC)
        {
            List<tbt_BillingTemp> tempRes = new List<tbt_BillingTemp>();
            List<dtBillingTempChangePlanData> result = new List<dtBillingTempChangePlanData>();
            ICommonContractHandler commoncontracthandler = ServiceContainer.GetService<ICommonContractHandler>() as ICommonContractHandler;

            var contractBasic = GetTbt_RentalContractBasic(strContractCode, null);
            var securituBasic = GetTbt_RentalSecurityBasic(strContractCode, strOCC);

            if ((contractBasic == null) || (contractBasic.Count != 1) || (securituBasic == null) || (securituBasic.Count != 1))
            {
                //throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_CONTRACT
                //    , MessageUtil.MessageList.MSG0105
                //    , new string[] {strContractCode});
                throw new Exception("Unable to get data from billing temp");
            }

            var contractStatus = contractBasic[0].ContractStatus;
            var productTypeCode = contractBasic[0].ProductTypeCode;
            var installationCompleteFlag = securituBasic[0].InstallationCompleteFlag;
            var firstInstallationCompleteFlag = contractBasic[0].FirstInstallCompleteFlag;
            var startType = contractBasic[0].StartType;

            var tmpUnImplOCC = this.GetLastUnimplementedOCC(strContractCode);
            if (!String.IsNullOrEmpty(tmpUnImplOCC) /*&& (installationCompleteFlag.GetValueOrDefault() == FlagType.C_FLAG_OFF)*/)
            {

                var rawRes = this.GetBillingTempForChangePlan_Edit(strContractCode
                    , strOCC
                    , ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE
                    , ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON
                    , ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE
                    , ContractBillingType.C_CONTRACT_BILLING_TYPE_INSTALLATION_FEE
                    , ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE
                    , BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT);

                tempRes = (from a in rawRes
                           select new tbt_BillingTemp
                           {
                               BillingAmt = a.BillingAmt
                               ,
                               BillingAmtUsd = a.BillingAmtUsd
                               ,
                               BillingAmtCurrencyType = a.BillingAmtCurrencyType
                               ,
                               BillingClientCode = a.BillingClientCode
                               ,
                               BillingCycle = a.BillingCycle
                               ,
                               BillingOCC = a.BillingOCC
                               ,
                               BillingOfficeCode = a.BillingOfficeCode
                               ,
                               BillingTargetCode = a.BillingTargetCode
                               ,
                               BillingTargetRunningNo = a.BillingTargetRunningNo
                               ,
                               BillingTiming = a.BillingTiming
                               ,
                               BillingType = a.BillingType
                               ,
                               CalDailyFeeStatus = a.CalDailyFeeStatus
                               ,
                               ContractCode = a.ContractCode
                               ,
                               CreateBy = a.CreateBy
                               ,
                               CreateDate = a.CreateDate
                               ,
                               OCC = a.OCC
                               ,
                               PayMethod = a.PayMethod
                               ,
                               SendFlag = a.SendFlag
                               ,
                               SequenceNo = a.SequenceNo
                               ,
                               UpdateBy = a.UpdateBy
                               ,
                               UpdateDate = a.UpdateDate
                           }).ToList();

                if ((contractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START
                    || (contractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START
                        && startType == StartType.C_START_TYPE_ALTER_START)
                    )
                    && (installationCompleteFlag.GetValueOrDefault() == FlagType.C_FLAG_ON))
                {
                    //var removeLst = from a in tempRes
                    //                where ((a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                    //                    || (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON)
                    //                    || (a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE))
                    //                select a;

                    tempRes.RemoveAll(a => ((a.BillingType != ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
                                        && (a.BillingType != ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON)
                                        && (a.BillingType != ContractBillingType.C_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE)));
                }

                if (/* SET 1 */
                    (   /* SET 1.2 */
                        (securituBasic[0].DepositFeeBillingTiming != BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                        && ( /* SET 1.2.1 */
                            (contractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_SG)
                        || (contractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_MA)
                        || (contractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_BE)
                        || (contractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_ONLINE)))

                    || /* SET 2 */
                        ((contractBasic[0].FirstInstallCompleteFlag.GetValueOrDefault() == FlagType.C_FLAG_ON)
                    && ((contractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_AL)
                    || (contractBasic[0].ProductTypeCode == ProductType.C_PROD_TYPE_RENTAL_SALE))))
                {
                    tempRes.RemoveAll(a => ((a.BillingType == ContractBillingType.C_CONTRACT_BILLING_TYPE_DEPOSIT_FEE)
                                        && (a.BillingTiming == BillingTiming.C_BILLING_TIMING_APPROVE_CONTRACT)
                                        ));
                }
            }
            else if (/*((contractStatus == ContractStatus.C_CONTRACT_STATUS_BEF_START) && (installationCompleteFlag == FlagType.C_FLAG_ON))
                || */((contractStatus == ContractStatus.C_CONTRACT_STATUS_AFTER_START) && (String.IsNullOrEmpty(tmpUnImplOCC)))
                || ((contractStatus == ContractStatus.C_CONTRACT_STATUS_STOPPING) && (String.IsNullOrEmpty(tmpUnImplOCC)))
                )
            {
                var rawRes = this.GetBillingTempForChangePlan_New(strContractCode
                    , strOCC
                    , ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE);
                tempRes = (from a in rawRes
                           select new tbt_BillingTemp
                           {
                               BillingAmt = a.BillingAmt
                               ,
                               BillingClientCode = a.BillingClientCode
                               ,
                               BillingCycle = a.BillingCycle
                               ,
                               BillingOCC = a.BillingOCC
                               ,
                               BillingOfficeCode = a.BillingOfficeCode
                               ,
                               BillingTargetCode = a.BillingTargetCode
                               ,
                               BillingTargetRunningNo = a.BillingTargetRunningNo
                               ,
                               BillingTiming = a.BillingTiming
                               ,
                               BillingType = a.BillingType
                               ,
                               CalDailyFeeStatus = a.CalDailyFeeStatus
                               ,
                               ContractCode = a.ContractCode
                               ,
                               CreateBy = a.CreateBy
                               ,
                               CreateDate = a.CreateDate
                               ,
                               OCC = a.OCC
                               ,
                               PayMethod = a.PayMethod
                               ,
                               SendFlag = a.SendFlag
                               ,
                               SequenceNo = a.SequenceNo
                               ,
                               UpdateBy = a.UpdateBy
                               ,
                               UpdateDate = a.UpdateDate
                           }).ToList();
            }

            // Dummie Test
            //tempRes = GetTbt_BillingTemp(strContractCode, strOCC);

            result = CommonUtil.ClonsObjectList<tbt_BillingTemp, dtBillingTempChangePlanData>(tempRes);

            foreach (var item in result)
            {
                item.DataComeFrom = (item.CreateDate.HasValue) ? 2 : 1;
            }

            return result;
        }

        /// <summary>
        /// Get billing temp from both billing temp table and billing basic
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public List<dtBillingTempChangeFeeData> GetBillingTempForChangeFee(string strContractCode)
        {
            return base.GetBillingTempForChangeFee(strContractCode, ContractBillingType.C_CONTRACT_BILLING_TYPE_CONTRACT_FEE);
        }

        /// <summary>
        /// Getting installation compete date when installation type is remove all.
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        public DateTime? GetRemovalInstallCompleteDate(string contractCode)
        {
            try
            {
                List<DateTime?> lst = base.GetRemovalInstallCompleteDate(contractCode,
                                            RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_ALL,
                                            RentalInstallationType.C_RENTAL_INSTALL_TYPE_REMOVE_DURING_STOP_REMOVE_ALL);
                if (lst.Count > 0)
                    return lst[0];
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<dtQuotationNoData> GetQuotationNo(string quotationTargetCode, string alphabet)
        {
            return base.GetQuotationNo(quotationTargetCode, alphabet);
        }
    }
}
