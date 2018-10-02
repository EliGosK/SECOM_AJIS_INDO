using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Net.Sockets;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class BillingTempHandler : BizCTDataEntities, IBillingTempHandler
    {
        /// <summary>
        /// Insert billing temp data
        /// </summary>
        /// <param name="billing"></param>
        /// <returns></returns>
        public List<tbt_BillingTemp> InsertBillingTemp(tbt_BillingTemp billing)
        {
            List<tbt_BillingTemp> insertedList = null;
            try
            {
                //Validate data input
                //1.1	ContractCode and OCC are required.
                //1.3   BillingType, BillingAmount, PaymentMethod are required.
                #region validate input
                ApplicationErrorException.CheckMandatoryField<tbt_BillingTemp, doBillingTempData_Insert>(billing);

                //1.2	At least one of (BillingOCC, BillingTargetCode, BillingClientCode and BillingOfficeCode)
                bool bBillingOCC = true;
                bool bBillingTargetCode = true;
                bool bBillingClientOfficeCode = true;
                List<String> fieldList = new List<string>();

                //BillingOCC
                if (CommonUtil.IsNullOrEmpty(billing.BillingOCC))
                {
                    bBillingOCC = false;
                    //fieldList.Add("BillingOCC");
                }

                //BillingTargetCode
                if (CommonUtil.IsNullOrEmpty(billing.BillingTargetCode))
                {
                    bBillingTargetCode = false;
                    //fieldList.Add("BillingTargetCode");
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
                    //fieldList.Add("BillingOfficeCode");
                    //fieldList.Add("BillingClientCode");
                }

                bool bResult = bBillingOCC || bBillingTargetCode || bBillingClientOfficeCode;
                if (!bResult)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, "At least one of BillingOCC, BillingTargetCode, BillingClientCode and BillingOfficeCode");
                }
                #endregion

                //Insert billing temp data to DB
                insertedList = base.InsertTbt_BillingTemp(billing.ContractCode
                                                , billing.OCC
                                                , billing.BillingOCC
                                                , billing.BillingTargetRunningNo
                                                , billing.BillingClientCode
                                                , billing.BillingTargetCode
                                                , billing.BillingOfficeCode
                                                , billing.BillingType
                                                , billing.CreditTerm
                                                , billing.BillingTiming
                                                , billing.BillingAmt
                                                , billing.PayMethod
                                                , billing.BillingCycle
                                                , billing.CalDailyFeeStatus
                                                , billing.SendFlag
                                                , CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                                                , CommonUtil.dsTransData.dtUserData.EmpNo
                                                , billing.DocLanguage
                                                , billing.BillingAmtUsd
                                                , billing.BillingAmtCurrencyType); //Add by Jutarat A. on 18122013

                //Insert Log
                if (insertedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_TEMP;
                    logData.TableData = CommonUtil.ConvertToXml(insertedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return insertedList;
        }

        /// <summary>
        /// Update billing temp data by billing client code and office
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingClientCode"></param>
        /// <param name="strBillingOfficeCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="strBillingTargetCode"></param>
        /// <returns></returns>
        public List<tbt_BillingTemp> UpdateBillingTempByBillingClientAndOffice(string ContractCode, string BillingClientCode, string BillingOfficeCode, string BillingOCC, string BillingTargetCode)
        {
            try
            {
                //Validate data input
                //1.1	All parameters IN are required.
                #region validate data
                doBillingTempData_UpdateBillingClientAndOffice updateDo = new doBillingTempData_UpdateBillingClientAndOffice();
                updateDo.ContractCode = ContractCode;
                updateDo.BillingClientCode = BillingClientCode;
                updateDo.BillingOfficeCode = BillingOfficeCode;
                updateDo.BillingOCC = BillingOCC;
                updateDo.BillingTargetCode = BillingTargetCode;
                ApplicationErrorException.CheckMandatoryField(updateDo);
                #endregion

                //Update data to DB
                List<tbt_BillingTemp> updatedList
                    = base.UpdateTbt_BillingTemp_ByBillingClientAndOffice(
                        BillingOCC, BillingTargetCode, ContractCode, BillingClientCode, BillingOfficeCode,
                        CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        CommonUtil.dsTransData.dtUserData.EmpNo);

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_TEMP;
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
        /// For change billing target from CTS130: CP-16 change customer name and address
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOldBillingClientCode"></param>
        /// <param name="strOldBillingOfficeCode"></param>
        /// <param name="strOldBillingTargetCode"></param>
        /// <param name="strNewBillingClientCode"></param>
        /// <param name="strNewBillingOfficeCode"></param>
        /// <param name="strNewBillingTargetCode"></param>
        /// <returns></returns>
        public List<tbt_BillingTemp> UpdateBillingTempByBillingTarget(string ContractCode, string strOldBillingClientCode, string strOldBillingOfficeCode, string strOldBillingTargetCode, string strNewBillingClientCode, string strNewBillingOfficeCode, string strNewBillingTargetCode)
        {
            try
            {
                //Validate data input
                //1.1	ContractCode is required.
                doBillingTempData billing = new doBillingTempData();
                billing.ContractCode = ContractCode;
                ApplicationErrorException.CheckMandatoryField(billing);

                //Update data to DB
                List<tbt_BillingTemp> updatedList
                    = base.UpdateTbt_BillingTemp_ByBillingTarget(
                        strNewBillingClientCode, strNewBillingOfficeCode, strNewBillingTargetCode,
                        strOldBillingClientCode, strOldBillingOfficeCode, strOldBillingTargetCode, ContractCode,
                        CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                        CommonUtil.dsTransData.dtUserData.EmpNo,
                        BillingTemp.C_BILLINGTEMP_FLAG_KEEP);

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_TEMP;
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
        /// Update billing temp data by key
        /// </summary>
        /// <param name="billing"></param>
        /// <returns></returns>
        public List<tbt_BillingTemp> UpdateBillingTempByKey(tbt_BillingTemp billing)
        {
            try
            {
                //Validate data input
                //1.1	ContractCode, OCC and SequenceNo are required
                //1.3	BillingType, BillingAmount, PaymentMethod are required.
                #region validate input
                ApplicationErrorException.CheckMandatoryField<tbt_BillingTemp, doBillingTempData_Update>(billing);

                //1.2	At least one of (BllingOCC, BillingTargetCode, BillingClientCode)
                List<String> fieldList = new List<string>();

                if (CommonUtil.IsNullOrEmpty(billing.BillingOCC))
                {
                    fieldList.Add("BillingOCC");
                }

                if (CommonUtil.IsNullOrEmpty(billing.BillingTargetCode))
                {
                    fieldList.Add("BillingTargetCode");
                }

                //Modify by Jutarat A. on 13112013
                /*if (CommonUtil.IsNullOrEmpty(billing.BillingClientCode))
                {
                    //fieldList.Add("BillingClientCode");
                }*/
                if (CommonUtil.IsNullOrEmpty(billing.BillingClientCode) && CommonUtil.IsNullOrEmpty(billing.BillingOfficeCode))
                {
                    if (CommonUtil.IsNullOrEmpty(billing.BillingClientCode))
                        fieldList.Add("BillingClientCode");

                    if (CommonUtil.IsNullOrEmpty(billing.BillingOfficeCode))
                        fieldList.Add("BillingOfficeCode");
                    
                }
                //End Modify

                if (fieldList.Count >= 3) //== 3 //Modify by Jutarat A. on 13112013
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, fieldList.ToArray<String>());
                }
                #endregion

                //Update data to DB
                List<tbt_BillingTemp> updatedList
                    = base.UpdateTbt_BillingTemp_ByKey(billing.BillingOCC
                                                        , billing.BillingTargetRunningNo
                                                        , billing.BillingClientCode
                                                        , billing.BillingTargetCode
                                                        , billing.BillingOfficeCode
                                                        , billing.BillingType
                                                        , billing.CreditTerm
                                                        , billing.BillingTiming
                                                        , billing.BillingAmt
                                                        , billing.PayMethod
                                                        , billing.BillingCycle
                                                        , billing.CalDailyFeeStatus
                                                        , billing.SendFlag
                                                        , billing.ContractCode
                                                        , billing.OCC
                                                        , billing.SequenceNo
                                                        , CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                                                        , CommonUtil.dsTransData.dtUserData.EmpNo
                                                        , billing.BillingAmtCurrencyType
                                                        , billing.BillingAmtUsd);

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_TEMP;
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
        /// Delete billing temp data by contract code
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        public List<tbt_BillingTemp> DeleteBillingTempByContractCode(string ContractCode)
        {
            try
            {
                //Validate data input
                //1.1	ContractCode is required.
                doBillingTempData billing = new doBillingTempData();
                billing.ContractCode = ContractCode;
                ApplicationErrorException.CheckMandatoryField(billing);

                //Delete data from DB
                List<tbt_BillingTemp> deletedList
                    = base.DeleteTbt_BillingTemp_ByContractCode(ContractCode);

                //Insert Log
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete billing temp data by contract code and occ
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        public List<tbt_BillingTemp> DeleteBillingTempByContractCodeOCC(string ContractCode, string OCC)
        {
            try
            {
                //Validate data input
                //1.1	ContractCode and OCC are required.
                doBillingTempDataCheckContractOCC billing = new doBillingTempDataCheckContractOCC();
                billing.ContractCode = ContractCode;
                billing.OCC = OCC;
                ApplicationErrorException.CheckMandatoryField(billing);

                //Delete data from DB
                List<tbt_BillingTemp> deletedList
                    = base.DeleteTbt_BillingTemp_ByContractCodeOCC(ContractCode, OCC);

                //Insert Log
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Delete billing temp data by key
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <param name="iSequenceNo"></param>
        /// <returns></returns>
        public List<tbt_BillingTemp> DeleteBillingTempByKey(string ContractCode, string OCC, int iSequenceNo)
        {
            try
            {
                //Validate data input
                //1.1	ContractCode, OCC and SequenceNo are required.
                doBillingTempDataCheckContractSequence billing = new doBillingTempDataCheckContractSequence();
                billing.ContractCode = ContractCode;
                billing.OCC = OCC;
                billing.SequenceNo = iSequenceNo;
                ApplicationErrorException.CheckMandatoryField(billing);

                //Delete data from DB
                List<tbt_BillingTemp> deletedList
                    = base.DeleteTbt_BillingTemp_ByKey(ContractCode, OCC, iSequenceNo);

                //Insert Log
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get billing basic data from billing temp 
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="OCC"></param>
        /// <param name="BillingTypeList"></param>
        /// <param name="BillingTimingList"></param>
        /// <returns></returns>
        public List<doBillingTempBasic> GetBillingBasicData(string ContractCode, string OCC, List<string> BillingTypeList, List<string> BillingTimingList)
        {
            try
            {
                //Validate data input
                //1.1	ContractCode is required.
                doBillingTempData billing = new doBillingTempData();
                billing.ContractCode = ContractCode;
                ApplicationErrorException.CheckMandatoryField(billing);

                string strBillingType = null;
                if (BillingTypeList != null)
                    strBillingType = CommonUtil.CreateCSVString(BillingTypeList);

                string strBillingTiming = null;
                if (BillingTimingList != null)
                    strBillingTiming = CommonUtil.CreateCSVString(BillingTimingList);

                //Get data from DB
                List<doBillingTempBasic> basicList
                    = base.GetBillingBasicData(
                        ContractCode, OCC, strBillingType, strBillingTiming
                        //, SECOM_AJIS.Common.Util.ConstantValue.ContractBillingType.C_CONTRACT_BILLING_TYPE_SALE_PRICE
                        , SECOM_AJIS.Common.Util.ConstantValue.ContractBillingType.C_CONTRACT_BILLING_TYPE_PRODUCT_PRICE
                        , BillingTemp.C_BILLINGTEMP_FLAG_KEEP
                        , SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL);

                return basicList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get billing detail data from billing temp
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="OCC"></param>
        /// <param name="BillingTypeList"></param>
        /// <param name="BillingTimingList"></param>
        /// <returns></returns>
        public List<doBillingTempDetail> GetBillingDetailData(string ContractCode, string OCC, List<string> BillingTypeList, List<string> BillingTimingList)
        {
            try
            {
                //Validate data input
                //1.1	ContractCode is required.
                doBillingTempData billing = new doBillingTempData();
                billing.ContractCode = ContractCode;
                ApplicationErrorException.CheckMandatoryField(billing);

                string strBillingType = null;
                if (BillingTypeList != null)
                    strBillingType = CommonUtil.CreateCSVString(BillingTypeList);

                string strBillingTiming = null;
                if (BillingTimingList != null)
                    strBillingTiming = CommonUtil.CreateCSVString(BillingTimingList);

                //Get data from DB
                List<doBillingTempDetail> detailList
                    = base.GetBillingDetailData(
                        ContractCode, OCC, strBillingType, strBillingTiming, BillingTemp.C_BILLINGTEMP_FLAG_KEEP);

                return detailList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///// <summary>
        ///// Get billing temp from specified fee type
        ///// </summary>
        ///// <param name="contractCode"></param>
        ///// <param name="occ"></param>
        ///// <param name="billingType"></param>
        ///// <param name="billingTiming"></param>
        ///// <returns></returns>
        //public List<tbt_BillingTemp> GetFee(string contractCode, string occ, string billingType, string billingTiming)
        //{
        //    try
        //    {
        //        //ApplicationErrorException.CheckMandatoryField(billing);
        //        return new List<tbt_BillingTemp>();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// Update flag in billing temp when already send to billing module
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="SequenceNo"></param>
        /// <param name="OCC"></param>
        /// <returns></returns>
        public List<tbt_BillingTemp> UpdateSendFlag(string ContractCode, int SequenceNo, string OCC) //Add OCC by Jutarat A. on 05102012
        {
            try
            {
                //Validate data input
                //1.1	ContractCode is required.
                doBillingTempData billing = new doBillingTempData();
                billing.ContractCode = ContractCode;
                ApplicationErrorException.CheckMandatoryField(billing);

                //2.	Update billing temp data.
                //Update data to DB
                List<tbt_BillingTemp> updatedList = base.UpdateSendFlag(ContractCode, SequenceNo, OCC, //Add OCC by Jutarat A. on 05102012
                                                                        BillingTemp.C_BILLINGTEMP_FLAG_SENT, 
                                                                        CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                                                                        CommonUtil.dsTransData.dtUserData.EmpNo);

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_TEMP;
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
        /// Delete data in billing temp when data is send
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        public List<tbt_BillingTemp> DeleteAllSendData(string ContractCode)
        {
            try
            {
                //Validate data input
                //1.1	ContractCode is required.
                doBillingTempData billing = new doBillingTempData();
                billing.ContractCode = ContractCode;
                ApplicationErrorException.CheckMandatoryField(billing);

                //Delete data from DB
                List<tbt_BillingTemp> deletedList = base.DeleteAllSendData(ContractCode, BillingTemp.C_BILLINGTEMP_FLAG_KEEP);

                //Insert Log
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
