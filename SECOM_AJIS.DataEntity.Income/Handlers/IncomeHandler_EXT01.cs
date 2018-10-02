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
using System.Data.Objects;



namespace SECOM_AJIS.DataEntity.Income
{
    public partial class IncomeHandler : BizICDataEntities, IIncomeHandler
    {
        #region Receipt
        //ICS010
        /// <summary>
        /// Function for retrieving advance issued receipt information of specific receipt no. (sp_IC_GetAdvanceReceipt)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        public doReceipt GetAdvanceReceipt(string receiptNo)
        {
            doReceipt result = base.GetAdvanceReceipt(receiptNo).FirstOrDefault();
            //Mapping Language
            if (result != null)
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    result.BillingClientName = result.BillingClientNameEN;
                    result.BillingClientAddress = result.BillingClientAddressEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    result.BillingClientName = result.BillingClientNameEN;
                    result.BillingClientAddress = result.BillingClientAddressEN;
                }
                else
                {
                    result.BillingClientName = result.BillingClientNameLC;
                    result.BillingClientAddress = result.BillingClientAddressLC;
                }
            }
            return result;
        }
        /// <summary>
        /// Function for retrieving receipt information of specific receipt no. which isn't cancel status. (sp_IC_GetReceipt)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        public doReceipt GetReceipt(string receiptNo)
        {
            doReceipt result = base.GetReceipt(receiptNo,CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US).FirstOrDefault();
            //Mapping Language
            if (result != null)
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    result.BillingClientName = result.BillingClientNameEN;
                    result.BillingClientAddress = result.BillingClientAddressEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    result.BillingClientName = result.BillingClientNameEN;
                    result.BillingClientAddress = result.BillingClientAddressEN;
                }
                else
                {
                    result.BillingClientName = result.BillingClientNameLC;
                    result.BillingClientAddress = result.BillingClientAddressLC;
                }
            }
            return result;
        }
        /// <summary>
        /// Function for retrieving receipt information of specific invoice no. and invoice occ (sp_IC_GetReceiptByInvoiceNo)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="invoiceOCC">invoice occ</param>
        /// <returns></returns>
        public doReceipt GetReceiptByInvoiceNo(string invoiceNo, int invoiceOCC)
        {
            doReceipt result = base.GetReceiptByInvoiceNo(invoiceNo, invoiceOCC, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US).FirstOrDefault();
            //Mapping Language
            if (result != null)
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    result.BillingClientName = result.BillingClientNameEN;
                    result.BillingClientAddress = result.BillingClientAddressEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    result.BillingClientName = result.BillingClientNameEN;
                    result.BillingClientAddress = result.BillingClientAddressEN;
                }
                else
                {
                    result.BillingClientName = result.BillingClientNameLC;
                    result.BillingClientAddress = result.BillingClientAddressLC;
                }
            }
            return result;
        }
        /// <summary>
        /// Function for retrieving receipt information of specific receipt no. (sp_IC_GetReceiptIncludeCancel)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        public doReceipt GetReceiptIncludeCancel(string receiptNo)
        {
            doReceipt result = base.GetReceiptIncludeCancel(receiptNo).FirstOrDefault();
            //Mapping Language
            if (result != null)
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    result.BillingClientName = result.BillingClientNameEN;
                    result.BillingClientAddress = result.BillingClientAddressEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    result.BillingClientName = result.BillingClientNameEN;
                    result.BillingClientAddress = result.BillingClientAddressEN;
                }
                else
                {
                    result.BillingClientName = result.BillingClientNameLC;
                    result.BillingClientAddress = result.BillingClientAddressLC;
                }
            }
            return result;
        }

        //ICS084
        /// <summary>
        /// Update advacent receipt status of receipt information of specific receipt no. (sp_IC_UpdateAdvanceReceiptMatchPayment)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        public bool UpdateAdvanceReceiptMatchPayment(string receiptNo)
        {
            List<tbt_Receipt> result = base.UpdateAdvanceReceiptMatchPayment(receiptNo, AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_PAID
                , CommonUtil.dsTransData.dtUserData.EmpNo
                , CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                );
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_RECEIPT,
                    TableData = CommonUtil.ConvertToXml(result)
                };
                logHand.WriteTransactionLog(logData);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Update receipt status back when the payment is deleted. (sp_IC_UpdateAdvanceReceiptDeletePayment)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        public bool UpdateAdvanceReceiptDeletePayment(string receiptNo)
        {
            List<tbt_Receipt> result = base.UpdateAdvanceReceiptDeletePayment(receiptNo, AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_ISSUED
                , CommonUtil.dsTransData.dtUserData.EmpNo
                , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_RECEIPT,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(result);
                logHand.WriteTransactionLog(logData);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Update receipt status that has been register with a payment. (sp_IC_UpdateAdvanceReceiptRegisterPayment)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        public bool UpdateAdvanceReceiptRegisterPayment(string receiptNo)
        {
            List<tbt_Receipt> result = base.UpdateAdvanceReceiptRegisterPayment(receiptNo, AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_REGISTERED
                ,CommonUtil.dsTransData.dtUserData.EmpNo
                ,CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                );
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_RECEIPT,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(result);
                logHand.WriteTransactionLog(logData);
                return true;
            }
            return false;
        }

        //ICS050
        /// <summary>
        /// Function for checking whether invoice has been issued a receipt. (sp_IC_CheckInvoiceIssuedReceipt)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="invoiceOCC">invoice occ</param>
        /// <returns></returns>
        public bool CheckInvoiceIssuedReceipt(string invoiceNo, int invoiceOCC)
        {
            ObjectParameter paramIsIssued = new ObjectParameter("IsIssued", false);
            base.CheckInvoiceIssuedReceipt(invoiceNo, (int?)invoiceOCC, paramIsIssued);
            bool isIssued = Convert.ToBoolean(paramIsIssued.Value);
            return isIssued;
        }

        //ICS060
        /// <summary>
        /// Function for cancel receipt information. (sp_IC_CancelReceipt)
        /// </summary>
        /// <param name="doReceipt">receipt information</param>
        /// <returns></returns>
        public bool CancelReceipt(doReceipt doReceipt)
        {
            //Validate
            if (doReceipt == null || string.IsNullOrEmpty(doReceipt.ReceiptNo))
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG0007, null);
            }
            doReceipt doReceiptDB = this.GetReceipt(doReceipt.ReceiptNo);
            if (doReceipt.UpdateDate != doReceiptDB.UpdateDate)
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG0019, null);
            }

            //Cancel receipt
            List<tbt_Receipt> result = base.CancelReceipt(doReceipt.ReceiptNo
                , CommonUtil.dsTransData.dtUserData.EmpNo
                , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_RECEIPT,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(result);
                logHand.WriteTransactionLog(logData);
                return true;
            }

            return false;
        }

        //ICP010 
        //Comment by phumipat 4/May/2012 : No need logging for batch process
        //public override List<tbt_Receipt> InsertTbt_Receipt(string xmlTbt_Receipt)
        //{
        //    ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
        //    List<tbt_Receipt> saved = base.InsertTbt_Receipt(xmlTbt_Receipt);
        //    doTransactionLog logData = new doTransactionLog()
        //    {
        //        TransactionType = doTransactionLog.eTransactionType.Insert,
        //        TableName = TableName.C_TBL_NAME_RECEIPT,
        //        TableData = null
        //    };
        //    logData.TableData = CommonUtil.ConvertToXml(saved);
        //    logHand.WriteTransactionLog(logData);
        //    return saved;
        //}

        /// <summary>
        /// Function for insert receipt information to the system.
        /// </summary>
        /// <param name="receipts">receipt information</param>
        /// <returns></returns>
        public List<tbt_Receipt> InsertTbt_Receipt(List<tbt_Receipt> receipts, bool isWriteTransLog = true) //Add (isWriteTransLog) by Jutarat A. on 07062013
        {
            //Modify by Jutarat A. on 30052013
            //return InsertTbt_Receipt(CommonUtil.ConvertToXml_Store<tbt_Receipt>(receipts));
            List<tbt_Receipt> insertList = InsertTbt_Receipt(CommonUtil.ConvertToXml_Store<tbt_Receipt>(receipts));
            if (insertList != null && insertList.Count > 0)
            {
                if (isWriteTransLog) //Add (isWriteTransLog) by Jutarat A. on 07062013
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_RECEIPT;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }
            }
            return insertList;
            //End Modify
        }


        //ICS050
        /// <summary>
        /// Function for force issue receipt of specific invoice information and receipt date.
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="receiptDate">receipt date</param>
        /// <returns></returns>
        public tbt_Receipt ForceIssueReceipt(doInvoice doInvoice, DateTime receiptDate)
        {
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;

            //Genereate receipt no
            string receiptNo = billingHandler.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_RECEIPT
                , CommonUtil.dsTransData.dtUserData.EmpNo
                , receiptDate/*CommonUtil.dsTransData.dtOperationData.ProcessDateTime*/); //Edit by Patcharee T. For get Reciept no. in month of receiptDate 11-Jun-2013

            //Determine advacen receipt status
            string advanceReceiptStatus = string.Empty;
            if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN)
            {
                advanceReceiptStatus = AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_ISSUED;
            }
            else
            {
                advanceReceiptStatus = AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_NOT;
            }

            decimal? ReceiptAmt = (doInvoice.InvoiceAmount ?? 0) + (doInvoice.VatAmount ?? 0);

            //Prepare receipt 
            tbt_Receipt doReceipt = new tbt_Receipt()
            {
                ReceiptNo = receiptNo,
                ReceiptDate = receiptDate,
                BillingTargetCode = doInvoice.BillingTargetCode,
                InvoiceNo = doInvoice.InvoiceNo,
                InvoiceOCC = doInvoice.InvoiceOCC,
                ReceiptAmount = doInvoice.InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL ? ReceiptAmt : null,
                ReceiptAmountUsd = doInvoice.InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_US ? ReceiptAmt : null,
                ReceiptAmountCurrencyType = doInvoice.InvoiceAmountCurrencyType,
                AdvanceReceiptStatus = advanceReceiptStatus,
                ReceiptIssueFlag = true,
                CancelFlag = false,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
            };
            List<tbt_Receipt> receipts = new List<tbt_Receipt>();
            receipts.Add(doReceipt);
            List<tbt_Receipt> result = this.InsertTbt_Receipt(receipts);
            if (result != null && result.Count > 0)
            {
                //Update receipt no. to taxinvoice 
                bool isSuccess = billingHandler.UpdateReceiptNo(doInvoice.InvoiceNo, doInvoice.InvoiceOCC, receiptNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                if (isSuccess)
                {
                    //Success
                    return result[0];
                }
            }

            //All fail, No insert data
            return null;
        }

        //ICP010
        /// <summary>
        /// Function for issue a receipt for paid invoice (after payment matched) (Used by batch).
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="receiptDate">receipt date</param>
        /// <param name="batchId">batch id</param>
        /// <param name="batchDate">batch datetime</param>
        /// <returns></returns>
        public tbt_Receipt IssueReceipt(doInvoice doInvoice, DateTime receiptDate, string batchId, DateTime batchDate, bool isWriteTransLog = true) //Add (isWriteTransLog) by Jutarat A. on 07062013
        {
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            //call running no for batch process
            string receiptNo = billingHandler.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_RECEIPT, batchId, receiptDate/*batchDate*/); //Edit by Patcharee T. For issue Reciept no. in month of receiptDate 11-Jun-2013

            //Determin receipt issue flag
            bool receiptIssueFlag = doInvoice.IssueReceiptTiming == IssueRecieptTime.C_ISSUE_REC_TIME_NOT_ISSUE ? false : true;

            tbt_Receipt doReceipt = new tbt_Receipt()
            {
                ReceiptNo = receiptNo,
                ReceiptDate = receiptDate,
                BillingTargetCode = doInvoice.BillingTargetCode,
                InvoiceNo = doInvoice.InvoiceNo,
                InvoiceOCC = doInvoice.InvoiceOCC,
                ReceiptAmount = (doInvoice.PaidAmountIncVat ?? 0) + (doInvoice.RegisteredWHTAmount ?? 0),
                AdvanceReceiptStatus = AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_NOT,
                ReceiptIssueFlag = receiptIssueFlag,
                CancelFlag = false,
                CreateDate = batchDate,
                CreateBy = batchId,
                UpdateDate = batchDate,
                UpdateBy = batchId
            };
            List<tbt_Receipt> receipts = new List<tbt_Receipt>();
            receipts.Add(doReceipt);
            List<tbt_Receipt> result = this.InsertTbt_Receipt(receipts, isWriteTransLog); //Add (isWriteTransLog) by Jutarat A. on 07062013
            if (result != null && result.Count > 0)
            {
                bool isSuccess = billingHandler.UpdateReceiptNo(doInvoice.InvoiceNo, doInvoice.InvoiceOCC, receiptNo,batchId,batchDate);
                //Success
                if (isSuccess)
                    return result[0];
            }

            //all fail, No insert data
            return null;
        }

        //ICP011
        /// <summary>
        /// Function for issue an advance issued receipt for unpaid invoice. (Used by batch)
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="receiptDate">receipt date</param>
        /// <param name="batchId">batch id</param>
        /// <param name="batchDate">batch datetime</param>
        /// <returns></returns>
        public tbt_Receipt IssueAdvanceReceipt(doInvoice doInvoice, DateTime receiptDate, string batchId, DateTime batchDate)
        {
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            
            //Genereate receipt no
            string receiptNo = billingHandler.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_RECEIPT, batchId, receiptDate/*batchDate*/); //Edit by Patcharee T. For issue Reciept no. in month of receiptDate 11-Jun-2013

            //Determine advacen receipt status
            string advanceReceiptStatus = string.Empty;
            if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN)
            {
                advanceReceiptStatus = AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_ISSUED;
            }
            else
            {
                advanceReceiptStatus = AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_NOT;
            }

            //Determin receipt issue flag
            bool receiptIssueFlag = doInvoice.IssueReceiptTiming == IssueRecieptTime.C_ISSUE_REC_TIME_NOT_ISSUE ? false : true;
            
            //Prepare receipt
            tbt_Receipt doReceipt = new tbt_Receipt()
            {
                ReceiptNo = receiptNo,
                ReceiptDate = receiptDate,
                BillingTargetCode = doInvoice.BillingTargetCode,
                InvoiceNo = doInvoice.InvoiceNo,
                InvoiceOCC = doInvoice.InvoiceOCC,
                ReceiptAmount = (doInvoice.InvoiceAmount ?? 0) + (doInvoice.VatAmount ?? 0),
                AdvanceReceiptStatus = advanceReceiptStatus,
                ReceiptIssueFlag = receiptIssueFlag,
                CancelFlag = false,
                CreateDate = batchDate,
                CreateBy = batchId,
                UpdateDate = batchDate,
                UpdateBy = batchId
            };
            List<tbt_Receipt> receipts = new List<tbt_Receipt>();
            receipts.Add(doReceipt);
            List<tbt_Receipt> result = this.InsertTbt_Receipt(receipts, false); //Modify (Add isWriteTransLog) by Jutarat A. on 27022014
            if (result != null && result.Count > 0)
            {
                //Update receipt no. to taxinvoice 
                bool isSuccess = billingHandler.UpdateReceiptNo(doInvoice.InvoiceNo, doInvoice.InvoiceOCC, receiptNo, batchId, batchDate);
                if (isSuccess)
                {
                    //Success
                    return result[0];
                }
            }

            //All fail, No insert data
            return null;
        }

        /// <summary>
        /// Function for checking the receipt can be cancel tax invoice. (sp_IC_CheckCancelTaxInvoiceOption)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="invoiceOCC">invoice occ</param>
        /// <returns></returns>
        public bool CheckCancelTaxInvoiceOption(string invoiceNo, int invoiceOCC)
        {
            ObjectParameter paramIsCancel = new ObjectParameter("IsCancelTaxInvoiceOption", false);
            base.CheckCancelTaxInvoiceOption(invoiceNo, (int?)invoiceOCC, paramIsCancel);
            bool IsCancelTaxInvoiceOption = Convert.ToBoolean(paramIsCancel.Value);
            return IsCancelTaxInvoiceOption;
        }

        /// <summary>
        /// Function for checking that tax invoice no. can register credit note. (sp_IC_CheckCanRegisterCreditNote)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <returns></returns>
        public bool CheckCanRegisterCreditNote(string invoiceNo)
        {
            ObjectParameter paramIsCanRegisterCN = new ObjectParameter("IsCanRegisterCN", false);
            base.CheckCanRegisterCreditNote(invoiceNo, paramIsCanRegisterCN);
            bool IsCanRegisterCN = Convert.ToBoolean(paramIsCanRegisterCN.Value);
            return IsCanRegisterCN;
        }

        /// <summary>
        /// Function for get total amount included VAT of all credit note. (sp_IC_GetTotalCreditAmtIncVAT)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <returns></returns>
        public decimal GetTotalCreditAmtIncVAT(string invoiceNo)
        {
            decimal? result = base.GetTotalCreditAmtIncVAT(invoiceNo,CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US).FirstOrDefault();
            return result ?? 0;
        }

        /// <summary>
        /// Check the last update row has deposit status is C_DEPOSIT_STATUS_RETURN  //2. (sp_IC_CheckDepositStatusReturn)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <returns></returns>
        public bool CheckDepositStatusReturn(string ContractCode)
        {
            ObjectParameter paramIsStatusReturn = new ObjectParameter("IsStatusReturn", false);
            base.CheckDepositStatusReturn(ContractCode, paramIsStatusReturn);
            bool IsStatusReturn = Convert.ToBoolean(paramIsStatusReturn.Value);
            return IsStatusReturn;
        }


        /// <summary>
        /// canceling credit note.
        /// </summary>
        public bool CancelCreditNote(tbt_BillingBasic _doBillingBasic, doGetCreditNote _doGetCreditNote)
        {
            
               
            try
            {
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                decimal CNAmountExcVAT = _doGetCreditNote.CreditAmountIncVAT - (_doGetCreditNote.CreditVATAmount ?? 0);
                decimal CNAmountExcVATUsd = _doGetCreditNote.CreditAmountIncVATUsd - (_doGetCreditNote.CreditVATAmountUsd ?? 0);

                //Add by Pachara S. 15032017
                string CNAmountExcVATCurrencyType;
                if (_doGetCreditNote.CreditAmountIncVATCurrencyType == null)
                    CNAmountExcVATCurrencyType = _doGetCreditNote.CreditVATAmountCurrencyType;
                else
                    CNAmountExcVATCurrencyType = _doGetCreditNote.CreditAmountIncVATCurrencyType;

                decimal? decBalanceDeposit = 0;
                decimal? decBalanceDepositUsd = 0;
                string decBalanceDepositCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                billingHandler.UpdateBalanceDepositOfBillingBasic(_doBillingBasic.ContractCode, _doBillingBasic.BillingOCC
                    , CNAmountExcVAT, CNAmountExcVATUsd, "1", out decBalanceDeposit, out decBalanceDepositUsd, out decBalanceDepositCurrencyType); // add by Jirawat Jannet @2016-10-05 ยังไม่ถูก แก้ด้วย

                if (decBalanceDeposit < 0) // Incase of null is not happen
                {
                    return false;
                }
                else
                {
                    bool blnSuccess = billingHandler.InsertDepositFeeCancelRefund(_doBillingBasic.ContractCode, _doBillingBasic.BillingOCC, _doGetCreditNote.CreditNoteNo, CNAmountExcVAT, CNAmountExcVATUsd, CNAmountExcVATCurrencyType);
                    return blnSuccess;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
                

           
        }

        // End Add

        #endregion

        #region Payment
        /// <summary>
        /// Function for insert payment information to DB. (sp_IC_InsertTbt_Payment)
        /// </summary>
        /// <param name="xmlTbt_Payment">xml of payment information</param>
        /// <returns></returns>
        public override List<tbt_Payment> InsertTbt_Payment(string xmldoPayment)
        {
            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            List<tbt_Payment> saved = base.InsertTbt_Payment(xmldoPayment);
            if (saved != null && saved.Count > 0) //Add by Jutarat A. on 30052013
            {
                doTransactionLog logData = new doTransactionLog()
                            {
                                TransactionType = doTransactionLog.eTransactionType.Insert,
                                TableName = TableName.C_TBL_NAME_PAYMENT,
                                TableData = null
                            };
                logData.TableData = CommonUtil.ConvertToXml(saved);
                logHand.WriteTransactionLog(logData);
            }
            return saved;
        }
        /// <summary>
        /// Update receipt status that has been register with a payment. (sp_IC_UpdateAdvanceReceiptRegisterPayment)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        public List<tbt_Payment> RegisterPayment(List<tbt_Payment> payments)
        {
            IBillingHandler handler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            string paymentTranNo = string.Empty;
            foreach (var item in payments)
            {
                paymentTranNo = handler.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_PAYMENT_TRANS);

                item.PaymentTransNo = paymentTranNo;
                item.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                item.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                item.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                item.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            }

            List<tbt_Payment> result = this.InsertTbt_Payment(CommonUtil.ConvertToXml_Store<tbt_Payment>(payments));
            if (result.Count == payments.Count)
            {
                foreach (var item in result)
                {
                    if (!CommonUtil.IsNullOrEmpty(item.RefAdvanceReceiptNo))
                    {
                        if (!UpdateAdvanceReceiptRegisterPayment(item.RefAdvanceReceiptNo))
                        {
                            //Fail
                            return null;
                        }
                        
                    }
                }
            }
            else
            {
                //Fail
                return null;
            }
            //Success
            return result;
        }
        /// <summary>
        /// Function for retrieving payment trasaction information of specific advance search criteria. (sp_IC_SearchPayment)
        /// </summary>
        /// <param name="condition">advance search criteria information</param>
        /// <returns></returns>
        public List<doPayment> SearchPayment(doPaymentSearchCriteria condition)
        {
            List<doPayment> result = base.SearchPayment(condition.PaymentType
                , condition.Status
                , condition.SECOMAccountID
                , condition.PaymentTransNo
                , condition.Payer
                , condition.PaymentDateFrom
                , condition.PaymentDateTo
                , condition.MatchableBalanceFrom
                , condition.MatchableBalanceTo
                , condition.InvoiceNo
                , condition.ReceiptNo
                , condition.SendingBank
                , MiscType.C_PAYMENT_TYPE
                , condition.MatchRGroupName
                , condition.EmpNo
                , CurrencyUtil.C_CURRENCY_LOCAL
                , CurrencyUtil.C_CURRENCY_US
                , condition.MatchableBalancCurrencyType
                );
            if (result != null && result.Count > 0)
            {
                CommonUtil.MappingObjectLanguage<doPayment>(result);
            }
            return result;
        }
        /// <summary>
        /// Function for retrieving payment trasaction information of specific payment transaction no. (sp_IC_GetPayment)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <returns></returns>
        public tbt_Payment GetPayment(string paymentTransNo)
        {
            return base.GetPayment(paymentTransNo, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US).FirstOrDefault();
        }

        //ICS084
        /// <summary>
        /// Update matchable balance of payment transaction of specific payment transaction no. (sp_IC_UpdatePaymentMatchableBalance)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <param name="adjustAmount">adjust amount</param>
        /// <returns></returns>
        public bool UpdatePaymentMatchableBalance(string paymentTransNo, decimal adjustAmount,decimal adjustAmountUsd, string MatchableBalanceCurrencyType)
        {
            List<tbt_Payment> result = base.UpdatePaymentMatchableBalance(paymentTransNo, adjustAmount
                , CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, adjustAmountUsd, MatchableBalanceCurrencyType);
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_PAYMENT,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(result);
                logHand.WriteTransactionLog(logData);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Update bank fee registered flag of payment transaction of specific payment transaction no. (sp_IC_UpdatePaymentBankFeeFlag)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <param name="bankFeeFlag">bank fee registered flag</param>
        /// <returns></returns>
        public bool UpdatePaymentBankFeeFlag(string paymentTransNo, bool bankFeeFlag)
        {
            List<tbt_Payment> result = base.UpdatePaymentBankFeeFlag(paymentTransNo, bankFeeFlag
               , CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_PAYMENT,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(result);
                logHand.WriteTransactionLog(logData);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Update other income registered flag of payment transaction of specific payment transaction no. (sp_IC_UpdatePaymentOtherIncomeFlag)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <param name="otherIncomeFlag">other income registered flag</param>
        /// <returns></returns>
        public bool UpdatePaymentOtherIncomeFlag(string paymentTransNo, bool otherIncomeFlag)
        {
            List<tbt_Payment> result = base.UpdatePaymentOtherIncomeFlag(paymentTransNo, otherIncomeFlag
               , CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_PAYMENT,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(result);
                logHand.WriteTransactionLog(logData);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Update other expense registered flag of payment transaction of specific payment transaction no. (sp_IC_UpdatePaymentOtherExpenseFlag)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <param name="otherExpenseFlag">other expense registered flag</param>
        /// <returns></returns>
        public bool UpdatePaymentOtherExpenseFlag(string paymentTransNo, bool otherExpenseFlag) 
        {
            List<tbt_Payment> result = base.UpdatePaymentOtherExpenseFlag(paymentTransNo, otherExpenseFlag
               , CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_PAYMENT,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(result);
                logHand.WriteTransactionLog(logData);
                return true;
            }
            return false;
        }

        //ICS080
        /// <summary>
        /// Delete payment transaction of specific payment information. (sp_IC_DeletePaymentTransaction)
        /// </summary>
        /// <param name="doPayment">payment information</param>
        /// <returns></returns>
        public bool DeletePaymentTransaction(tbt_Payment doPayment)
        {
            tbt_Payment dbPayment = GetPayment(doPayment.PaymentTransNo);
            if (dbPayment == null || dbPayment.UpdateDate != doPayment.UpdateDate)
                throw new ApplicationException("MSG0019");

            tbt_Payment deleted = base.DeletePaymentTransaction(doPayment.PaymentTransNo,
                CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime).FirstOrDefault();

            //Add by Jutarat A. on 28022013
            //Insert Log
            if (deleted != null)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Delete;
                logData.TableName = TableName.C_TBL_NAME_PAYMENT;

                List<tbt_Payment> deletedList = new List<tbt_Payment>();
                deletedList.Add(deleted);
                logData.TableData = CommonUtil.ConvertToXml(deletedList);

                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }
            //End Add

            if (deleted != null)
                return true;       //Success

            return false;
        }

        // Add By Sommai P., Oct 29, 2013
        /// <summary>
        /// Update Encashed flag of payment transaction of specific payment transaction no. (sp_IC_UpdatePaymentEncashFlag)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <param name="bankFeeFlag">Eencashed flag</param>
        /// <returns></returns>
        public bool UpdatePaymentEncashFlag(doPaymentEncashParam param)
        {
            List<tbt_Payment> result = base.UpdatePaymentEncashFlag(
                param.PaymentTransNo,
                param.EncashedFlagByte, 
                param.ChequeReturnDate, 
                param.ChequeReturnReason, 
                param.ChequeReturnRemark, 
                param.ChequeEncashRemark,
                CommonUtil.dsTransData.dtUserData.EmpNo, 
                CommonUtil.dsTransData.dtOperationData.ProcessDateTime
            );
            if (result != null && result.Count > 0)
            {
                //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                //{
                //    try
                //    {
                        ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                        doTransactionLog logData = new doTransactionLog()
                        {
                            TransactionType = doTransactionLog.eTransactionType.Update,
                            TableName = TableName.C_TBL_NAME_PAYMENT,
                            TableData = null
                        };
                        logData.TableData = CommonUtil.ConvertToXml(result);
                        logHand.WriteTransactionLog(logData);
                        //scope.Complete();
                        return true;
                    //}
                    //catch
                    //{
                    //    scope.Dispose();
                    //    return false;
                    //}
                //}
            }
            return false;
        }

        /// <summary>
        /// Function for checking whether invoice has been issued a receipt. (sp_IC_CheckInvoiceIssuedReceipt)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="invoiceOCC">invoice occ</param>
        /// <returns></returns>
        public byte CheckAllPaymentEncashed(string invoiceNo, int invoiceOCC)
        {
            ObjectParameter paramEncashedFlag = new ObjectParameter("EncashedFlag", false);

            //base.CheckAllPaymentEncashed(invoiceNo, (int?)invoiceOCC, paramIsEncashed);
            base.CheckAllPaymentEncashed(invoiceNo, (int?)invoiceOCC, PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE, PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED, paramEncashedFlag);

            byte encashedflag = Convert.ToByte(paramEncashedFlag.Value);
            return encashedflag;
        }

        // End Add
        #endregion
    }
}
