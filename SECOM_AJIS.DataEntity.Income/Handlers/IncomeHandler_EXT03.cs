using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Transactions;

using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.DataEntity.Income
{
    public partial class IncomeHandler : BizICDataEntities, IIncomeHandler
    {
        #region Credit note
        /// <summary>
        /// Function for insert credit note information to DB. (sp_IC_InsertTbt_CreditNote)
        /// </summary>
        /// <param name="doTbt_CreditNote">credit note information</param>
        /// <returns></returns>
        public int InsertTbt_CreditNote(tbt_CreditNote doTbt_CreditNote)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doTbt_CreditNote.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_CreditNote.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doTbt_CreditNote.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_CreditNote.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;


                List<tbt_CreditNote> doInsertList = new List<tbt_CreditNote>();
                doInsertList.Add(doTbt_CreditNote);
                List<tbt_CreditNote> insertList = base.InsertTbt_CreditNote
                    (CommonUtil.ConvertToXml_Store<tbt_CreditNote>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_CREDIT_NOTE;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList.Count;
            }
            catch (Exception Ex)
            {
                throw;
            }
        }
        /// <summary>
        /// Function for update credit note information to DB. (sp_IC_UpdateTbt_CreditNote)
        /// </summary>
        /// <param name="doTbt_CreditNote">credit note information</param>
        /// <returns></returns>
        public int UpdateTbt_CreditNote(tbt_CreditNote doTbt_CreditNote)
        {
            try
            {
                //set updateDate and updateBy
                doTbt_CreditNote.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_CreditNote.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_CreditNote> doUpdateList = new List<tbt_CreditNote>();
                doUpdateList.Add(doTbt_CreditNote);
                List<tbt_CreditNote> updatedList = base.UpdateTbt_CreditNote(CommonUtil.ConvertToXml_Store<tbt_CreditNote>(doUpdateList));

                //Insert Log
                if (updatedList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_CREDIT_NOTE;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Function for update Receipt information to DB. (sp_IC_UpdateTbt_Receipt)
        /// </summary>
        /// <param name="doTbt_Receipt">Receipt information</param>
        /// <returns></returns>
        public int UpdateTbt_Receipt(tbt_Receipt doTbt_Receipt)
        {
            try
            {
                //set updateDate and updateBy
                doTbt_Receipt.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_Receipt.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_Receipt> doUpdateList = new List<tbt_Receipt>();
                doUpdateList.Add(doTbt_Receipt);
                List<tbt_Receipt> updatedList = base.UpdateTbt_Receipt(CommonUtil.ConvertToXml_Store<tbt_Receipt>(doUpdateList));

                //Insert Log
                if (updatedList != null && updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_RECEIPT;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Function for checking whether credit can be cancel. (sp_IC_CheckCreditNoteCanCancel)
        /// </summary>
        /// <param name="strCreditNoteNo">credit note no.</param>
        /// <returns></returns>
        public bool CheckCreditNoteCanCancel(string strCreditNoteNo)
        {
            try
            {

                List<String> result = base.CheckCreditNoteCanCancel(strCreditNoteNo);
                if (result.Count > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Function for generate credit note no. and save credit note information to DB.
        /// </summary>
        /// <param name="_dotbt_CreditNote">credit note information</param>
        /// <returns></returns>
        public tbt_CreditNote RegisterCreditNote(tbt_CreditNote _dotbt_CreditNote)
        {
            try
            {

                IBillingHandler iBillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                _dotbt_CreditNote.CreditNoteNo = iBillingHandler.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_CREDIT_NOTE, CommonUtil.dsTransData.dtUserData.EmpNo, _dotbt_CreditNote.CreditNoteDate == null ? CommonUtil.dsTransData.dtOperationData.ProcessDateTime : _dotbt_CreditNote.CreditNoteDate.Value);

                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                _dotbt_CreditNote.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                _dotbt_CreditNote.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                _dotbt_CreditNote.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                _dotbt_CreditNote.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                _dotbt_CreditNote.CancelFlag = false;

                if (InsertTbt_CreditNote(_dotbt_CreditNote) == 0)
                {
                    return null;
                }
                else
                {
                    return _dotbt_CreditNote;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Function for retrieving credit note information of specific tax invoice no. (sp_IC_GetCreditNote)
        /// </summary>
        /// <param name="strTaxInvoiceNo">tax invoice no.</param>
        /// <returns></returns>
        public doGetCreditNote GetCreditNote(string strTaxInvoiceNo)
        {
            try
            {
                //GetBillingCodeDeptSummary
                List<doGetCreditNote> result = base.GetCreditNote(
                    strTaxInvoiceNo);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion


        #region Debt target
        /// <summary>
        /// Function for retrieving debt target data. (sp_IC_GetDebtTarget)
        /// </summary>
        /// <returns></returns>
        public List<doGetDebtTarget> GetDebtTarget()
        {
            try
            {
                List<doGetDebtTarget> result = base.GetDebtTarget();
                if (result.Count > 0)
                {
                    //Language Mapping
                    CommonUtil.MappingObjectLanguage<doGetDebtTarget>(result);
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Function for update debt target information to DB. (sp_IC_UpdateTbt_DebtTarget)
        /// </summary>
        /// <param name="dotbt_DebtTarget">debt target information</param>
        /// <returns></returns>
        public int UpdateTbt_DebtTarget(tbt_DebtTarget dotbt_DebtTarget)
        {
            try
            {
                //set updateDate and updateBy
                // this functio is speacial beacuse it up/insert not update only

                dotbt_DebtTarget.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dotbt_DebtTarget.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                dotbt_DebtTarget.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dotbt_DebtTarget.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_DebtTarget> _dotbt_DebtTarget = new List<tbt_DebtTarget>();
                _dotbt_DebtTarget.Add(dotbt_DebtTarget);
                List<tbt_DebtTarget> updatedList = base.UpdateTbt_DebtTarget(CommonUtil.ConvertToXml_Store<tbt_DebtTarget>(_dotbt_DebtTarget));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_DEBT_TARGET;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Debt tracing
        /// <summary>
        /// Function for retrieving debt tracing memo information of specific billing target code, invoice no., invoice occ. (sp_IC_GetDebtTracingMemo)
        /// </summary>
        /// <param name="strBillingTargetCode">billing target code</param>
        /// <param name="strInvoiceNo">invoice no.</param>
        /// <param name="strInvoiceOCC">invoice occ</param>
        /// <returns></returns>
        public List<doGetDebtTracingMemo> GetDebtTracingMemoList(string strBillingTargetCode, string strInvoiceNo, int? strInvoiceOCC)
        {
            try
            {
                //GetBillingCodeDeptSummary
                List<doGetDebtTracingMemo> result = base.GetDebtTracingMemo(
                    strBillingTargetCode, strInvoiceNo, strInvoiceOCC);
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Function for insert billing target debt tracing data to DB. (sp_IC_InsertTbt_BillingTargetDebtTracing)
        /// </summary>
        /// <param name="doTbt_BillingTargetDebtTracing">billing target debt tracing information</param>
        /// <returns></returns>
        public int InsertTbt_BillingTargetDebtTracing(tbt_BillingTargetDebtTracing doTbt_BillingTargetDebtTracing)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doTbt_BillingTargetDebtTracing.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_BillingTargetDebtTracing.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doTbt_BillingTargetDebtTracing.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_BillingTargetDebtTracing.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;


                List<tbt_BillingTargetDebtTracing> doInsertList = new List<tbt_BillingTargetDebtTracing>();
                doInsertList.Add(doTbt_BillingTargetDebtTracing);
                List<tbt_BillingTargetDebtTracing> insertList = base.InsertTbt_BillingTargetDebtTracing
                    (CommonUtil.ConvertToXml_Store<tbt_BillingTargetDebtTracing>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_BILLING_TARGET_DEBT_TRACING;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList.Count;
            }
            catch (Exception Ex)
            {
                throw;
            }
        }
        /// <summary>
        /// Function for insert invoice debt tracing data to DB. (sp_IC_InsertTbt_InvoiceDebtTracing)
        /// </summary>
        /// <param name="doTbt_InvoiceDebtTracing">invoice debt tracing data</param>
        /// <returns></returns>
        public int InsertTbt_InvoiceDebtTracing(tbt_InvoiceDebtTracing doTbt_InvoiceDebtTracing)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doTbt_InvoiceDebtTracing.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_InvoiceDebtTracing.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doTbt_InvoiceDebtTracing.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_InvoiceDebtTracing.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;


                List<tbt_InvoiceDebtTracing> doInsertList = new List<tbt_InvoiceDebtTracing>();
                doInsertList.Add(doTbt_InvoiceDebtTracing);
                List<tbt_InvoiceDebtTracing> insertList = base.InsertTbt_InvoiceDebtTracing
                    (CommonUtil.ConvertToXml_Store<tbt_InvoiceDebtTracing>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_INVOICE_DEBT_TRACING;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList.Count;
            }
            catch (Exception Ex)
            {
                throw;
            }
        }
        #endregion

        #region Dept summary
        /// <summary>
        /// Function for retrieving debt summary data of each billing office. (sp_IC_GetBillingOfficeDebtSummary)
        /// </summary>
        /// <param name="intMonth">month</param>
        /// <param name="intYear">year</param>
        /// <returns></returns>
        public List<doGetBillingOfficeDebtSummary> GetBillingOfficeDebtSummaryList(int? intMonth, int? intYear)
        {
            try
            {
                List<doGetBillingOfficeDebtSummary> result = base.GetBillingOfficeDebtSummary
                    (intMonth, intYear);
                if (result.Count > 0)
                {
                    //Language Mapping
                    CommonUtil.MappingObjectLanguage<doGetBillingOfficeDebtSummary>(result);
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Function for retrieving debt summary data of each billing target in a billing office. (sp_IC_GetBillingTargetDebtSummaryByOffice)
        /// </summary>
        /// <param name="strBillingTargetCode">billing target code</param>
        /// <param name="intMonth">month</param>
        /// <param name="intYear">year</param>
        /// <returns></returns>
        public List<doGetBillingTargetDebtSummaryByOffice> GetBillingTargetDebtSummaryByOfficeList(string strOfficeCode, int? intMonth, int? intYear)
        {
            try
            {
                List<doGetBillingTargetDebtSummaryByOffice> result = base.GetBillingTargetDebtSummaryByOffice
                    (strOfficeCode, intMonth, intYear);

                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Function for retrieving unpaid invoices of a billing target. (sp_IC_GetUnpaidInvoiceDebtSummaryByBillingTarget)
        /// </summary>
        /// <param name="strBillingTargetCode">billing target code</param>
        /// <returns></returns>
        public List<doGetUnpaidInvoiceDebtSummaryByBillingTarget> GetUnpaidInvoiceDebtSummaryByBillingTargetList(string strBillingTargetCode, string strOfficeCode = null) //Add (strOfficeCode) by Jutarat A. on 10042014
        {
            try
            {
                List<doGetUnpaidInvoiceDebtSummaryByBillingTarget> result = base.GetUnpaidInvoiceDebtSummaryByBillingTarget
                    (strBillingTargetCode
                    , "PaymentMethodType"
                    , PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                    , PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                    , PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                    , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                    , strOfficeCode); //Add (strOfficeCode) by Jutarat A. on 10042014

                if (result!= null && result.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage<doGetUnpaidInvoiceDebtSummaryByBillingTarget>(result); //Add by Jutarat A. on 26042013
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Function for retrieving unpaid invoices debt summary by invoice no. (sp_IC_GetUnpaidInvoiceDebtSummaryByInvoiceNo)
        /// </summary>
        /// <param name="strInvoiceNo">invoice no.</param>
        /// <returns></returns>
        public List<doGetUnpaidInvoiceDebtSummaryByBillingTarget> GetUnpaidInvoiceDebtSummaryByInvoiceNo(string strInvoiceNo)
        {
            try
            {
                List<doGetUnpaidInvoiceDebtSummaryByBillingTarget> result = base.GetUnpaidInvoiceDebtSummaryByInvoiceNo
                    (strInvoiceNo
                    , "PaymentMethodType"
                    , PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                    , PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                    , PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                    , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN);

                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        
        /// <summary>
        /// Function for retrieving debt summary of a billing code. (sp_IC_GetBillingCodeDebtSummary)
        /// </summary>
        /// <param name="strBillingCode">billing code</param>
        /// <returns></returns>
        public List<doGetBillingCodeDebtSummary> GetBillingCodeDebtSummaryList(string strBillingCode)
        {
            try
            {
                //GetBillingCodeDeptSummary
                List<doGetBillingCodeDebtSummary> result = base.GetBillingCodeDebtSummary(
                    strBillingCode
                    , PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                    , PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                    , PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                    , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN);

                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Function for retrieving unpaid billing detail of a billing target. (sp_IC_GetUnpaidDetailDebtSummaryByBillingTarget)
        /// </summary>
        /// <param name="strBillingTargetCode">billing target code</param>
        /// <returns></returns>
        public List<doGetUnpaidDetailDebtSummary> GetUnpaidDetailDebtSummaryByBillingTargetList(string strBillingTargetCode, string strOfficeCode = null) //Add (strOfficeCode) by Jutarat A. on 11042014
        {
            List<doGetUnpaidDetailDebtSummary> result = base.GetUnpaidDetailDebtSummaryByBillingTarget
                    (strBillingTargetCode
                    , MiscType.C_PAYMENT_STATUS
                    , PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                    , PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                    , PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                    , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                    , strOfficeCode); //Add (strOfficeCode) by Jutarat A. on 11042014
            return result;
        }
        /// <summary>
        /// Function for retrieving unpaid invoices of a billing target. (sp_IC_GetUnpaidDetailDebtSummaryByInvoice)
        /// </summary>
        /// <param name="strInvoiceNo">invoice no.</param>
        /// <param name="intInvoiceOCC">invoice occ</param>
        /// <returns></returns>
        public List<doGetUnpaidDetailDebtSummary> GetUnpaidDetailDebtSummaryByInvoiceList(string strInvoiceNo, int? intInvoiceOCC, string strOfficeCode = null) //Add (strOfficeCode) by Jutarat A. on 11042014
        {
            List<doGetUnpaidDetailDebtSummary> result = base.GetUnpaidDetailDebtSummaryByInvoice
                    (strInvoiceNo
                    , intInvoiceOCC
                    , MiscType.C_PAYMENT_STATUS
                    , PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                    , PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                    , PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                    , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                    , strOfficeCode); //Add (strOfficeCode) by Jutarat A. on 11042014
            return result;
        }
        /// <summary>
        /// Function for retrieving unpaid billing target of a billing code. (sp_IC_GetUnpaidDetailDebtSummaryByBillingCode)
        /// </summary>
        /// <param name="strBillingCode">billing code</param>
        /// <returns></returns>
        public List<doGetUnpaidDetailDebtSummary> GetUnpaidDetailDebtSummaryByBillingCodeList(string strBillingCode)
        {
            List<doGetUnpaidDetailDebtSummary> result = base.GetUnpaidDetailDebtSummaryByBillingCode
                    (strBillingCode
                    , MiscType.C_PAYMENT_STATUS
                    , PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                    , PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                    , PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                    , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                    , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN);
            return result;
        }
        #endregion

        #region Money collection info
        /// <summary>
        /// Function for insert money collection info to DB. (sp_IC_InsertTbt_MoneyCollectionInfo)
        /// </summary>
        /// <param name="dotbt_MoneyCollectionInfo"></param>
        /// <returns></returns>
        public int CreateTbt_MoneyCollectionInfo(tbt_MoneyCollectionInfo dotbt_MoneyCollectionInfo)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                dotbt_MoneyCollectionInfo.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dotbt_MoneyCollectionInfo.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                dotbt_MoneyCollectionInfo.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                dotbt_MoneyCollectionInfo.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_MoneyCollectionInfo> doInsertList = new List<tbt_MoneyCollectionInfo>();
                doInsertList.Add(dotbt_MoneyCollectionInfo);
                List<tbt_MoneyCollectionInfo> insertList = base.InsertTbt_MoneyCollectionInfo
                    (CommonUtil.ConvertToXml_Store<tbt_MoneyCollectionInfo>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_MONEY_COLLECTION_INFO;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Function for retrieving money collection management information. (sp_IC_GetMoneyCollectionManagementInfo)
        /// </summary>
        /// <param name="expectedCollectDateFrom">from expected collection date</param>
        /// <param name="expectedCollectDateTo">to expected collection date</param>
        /// <param name="collectionAreaCode"></param>
        /// <returns></returns>
        public List<doGetMoneyCollectionManagementInfo> GetMoneyCollectionManagementInfoList(DateTime? expectedCollectDateFrom, DateTime? expectedCollectDateTo, string collectionAreaCode)
        {
            try
            {
                List<doGetMoneyCollectionManagementInfo> result = base.GetMoneyCollectionManagementInfo
                    (expectedCollectDateFrom, expectedCollectDateTo, collectionAreaCode);
                if (result.Count > 0)
                {
                    CommonUtil.MappingObjectLanguage<doGetMoneyCollectionManagementInfo>(result);
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Delete money collection information of specific receipt no. (sp_IC_DeleteTbt_MoneyCollectionInfo)
        /// </summary>
        /// <param name="ReceiptNo">receipt no.</param>
        /// <returns></returns>
        public List<tbt_MoneyCollectionInfo> DeleteTbt_MoneyCollectionInfo(string ReceiptNo)
        {
            try
            {
                List<tbt_MoneyCollectionInfo> result = base.DeleteTbt_MoneyCollectionInfo(ReceiptNo);
                //Insert Log
                if (result.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                    logData.TableName = TableName.C_TBL_NAME_MONEY_COLLECTION_INFO;
                    logData.TableData = CommonUtil.ConvertToXml(result);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Function for retrieving money collection information of specific receipt no. (sp_IC_GetTbt_MoneyCollectionInfo)
        /// </summary>
        /// <param name="strReceiptNo">receipt no.</param>
        /// <returns></returns>
        public List<tbt_MoneyCollectionInfo> GetTbt_MoneyCollectionInfo(string strReceiptNo)
        {
            List<tbt_MoneyCollectionInfo> result = base.GetTbt_MoneyCollectionInfo(strReceiptNo);
            if (result != null && result.Count > 0)
            {
                return result;
            }
            return null;
        }
        #endregion

        #region Revenue
        /// <summary>
        /// Function for insert revenue information to DB. (sp_IC_InsertTbt_Revenue)
        /// </summary>
        /// <param name="doTbt_Revenue">revenue information</param>
        /// <returns></returns>
        public int InsertTbt_Revenue(tbt_Revenue doTbt_Revenue)
        {
            try
            {
                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                doTbt_Revenue.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_Revenue.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doTbt_Revenue.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doTbt_Revenue.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;


                List<tbt_Revenue> doInsertList = new List<tbt_Revenue>();
                doInsertList.Add(doTbt_Revenue);
                List<tbt_Revenue> insertList = base.InsertTbt_Revenue
                    (CommonUtil.ConvertToXml_Store<tbt_Revenue>(doInsertList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_REVENUE;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList.Count;
            }
            catch (Exception Ex)
            {
                throw;
            }
        }
        /// <summary>
        /// Generate revenue no. and save revenue information to DB.
        /// </summary>
        /// <param name="_dotbt_Revenue"></param>
        /// <returns></returns>
        public tbt_Revenue RegisterRevenue(tbt_Revenue _dotbt_Revenue)
        {
            try
            {
                IBillingHandler iBillingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                _dotbt_Revenue.RevenueNo = iBillingHandler.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_REVENUE);

                //set CreateDate, CreateBy, UpdateDate and UpdateBy
                _dotbt_Revenue.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                _dotbt_Revenue.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                _dotbt_Revenue.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                _dotbt_Revenue.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                if (InsertTbt_Revenue(_dotbt_Revenue) == 0)
                {
                    return null;
                }
                else
                {
                    return _dotbt_Revenue;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Reg content
        /// <summary>
        /// Function to retrieve description to show in credit note/revenue register. (sp_IC_GetRegContent)
        /// </summary>
        /// <param name="strRegContentCode">reg content code</param>
        /// <returns></returns>
        public List<doGetRegContent> GetRegContent(string strRegContentCode)
        {
            try
            {

                List<doGetRegContent> result = base.GetRegContent("RegContent", strRegContentCode);
                if (result.Count > 0)
                {
                    //Language Mapping
                    CommonUtil.MappingObjectLanguage<doGetRegContent>(result);
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion

        #region Miscellaneous
        /// <summary>
        /// Function for retrieving the working day no. of a date. (sp_IC_GetWorkingDayNoOfMonth)
        /// </summary>
        /// <param name="getNextWorkingDay">working datetime</param>
        /// <returns></returns>
        public int GetWorkingDayNoOfMonth(DateTime getNextWorkingDay)
        {
            List<Int32?> result = base.GetWorkingDayNoOfMonth((DateTime?)getNextWorkingDay);
            if (result != null && result.Count > 0)
            {
                return (int)result[0];
            }
            return 0;
        }
        #endregion
    }
}
