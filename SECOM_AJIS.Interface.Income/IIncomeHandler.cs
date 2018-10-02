using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Income
{
    public interface IIncomeHandler
    {
        #region Receipt
        //ICS010
        /// <summary>
        /// Function for retrieving advance issued receipt information of specific receipt no. (sp_IC_GetAdvanceReceipt)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        doReceipt GetAdvanceReceipt(string receiptNo);
        /// <summary>
        /// Function for retrieving receipt information of specific receipt no. which isn't cancel status. (sp_IC_GetReceipt)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        doReceipt GetReceipt(string receiptNo);
        /// <summary>
        /// Function for retrieving receipt information of specific receipt no. (sp_IC_GetReceiptIncludeCancel)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        doReceipt GetReceiptIncludeCancel(string receiptNo);
        /// <summary>
        /// Function for retrieving receipt information of specific invoice no. and invoice occ (sp_IC_GetReceiptByInvoiceNo)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="invoiceOCC">invoice occ</param>
        /// <returns></returns>
        doReceipt GetReceiptByInvoiceNo(string invoiceNo, int invoiceOCC);
        
        //ICS084
        /// <summary>
        /// Update advacent receipt status of receipt information of specific receipt no. (sp_IC_UpdateAdvanceReceiptMatchPayment)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        bool UpdateAdvanceReceiptMatchPayment(string receiptNo);
        /// <summary>
        /// Update receipt status back when the payment is deleted. (sp_IC_UpdateAdvanceReceiptDeletePayment)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        bool UpdateAdvanceReceiptDeletePayment(string receiptNo);
        /// <summary>
        /// Update receipt status that has been register with a payment. (sp_IC_UpdateAdvanceReceiptRegisterPayment)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        bool UpdateAdvanceReceiptRegisterPayment(string receiptNo);
        
        //ICS050
        /// <summary>
        /// Function for checking whether invoice has been issued a receipt. (sp_IC_CheckInvoiceIssuedReceipt)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="invoiceOCC">invoice occ</param>
        /// <returns></returns>
        bool CheckInvoiceIssuedReceipt(string invoiceNo, int invoiceOCC);

        //ICS060
        /// <summary>
        /// Function for cancel receipt information. (sp_IC_CancelReceipt)
        /// </summary>
        /// <param name="doReceipt">receipt information</param>
        /// <returns></returns>
        bool CancelReceipt(doReceipt doReceipt);


        //ICS050
        /// <summary>
        /// Function for force issue receipt of specific invoice information and receipt date.
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="receiptDate">receipt date</param>
        /// <returns></returns>
        tbt_Receipt ForceIssueReceipt(doInvoice doInvoice, DateTime receiptDate);

        //ICP010
        /// <summary>
        /// Function for issue a receipt for paid invoice (after payment matched) (Used by batch).
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="receiptDate">receipt date</param>
        /// <param name="batchId">batch id</param>
        /// <param name="batchDate">batch datetime</param>
        /// <returns></returns>
        tbt_Receipt IssueReceipt(doInvoice doInvoice, DateTime receiptDate, string batchId, DateTime batchDate, bool isWriteTransLog = true); //Add (isWriteTransLog) by Jutarat A. on 07062013
        /// <summary>
        /// Function for insert receipt information to the system.
        /// </summary>
        /// <param name="receipts">receipt information</param>
        /// <returns></returns>
        List<tbt_Receipt> InsertTbt_Receipt(List<tbt_Receipt> receipts, bool isWriteTransLog = true); //Add (isWriteTransLog) by Jutarat A. on 07062013

        //ICP011
        /// <summary>
        /// Function for issue an advance issued receipt for unpaid invoice. (Used by batch)
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="receiptDate">receipt date</param>
        /// <param name="batchId">batch id</param>
        /// <param name="batchDate">batch datetime</param>
        /// <returns></returns>
        tbt_Receipt IssueAdvanceReceipt(doInvoice doInvoice, DateTime receiptDate, string batchId, DateTime batchDate);
        #endregion

        #region Cretdit Note
        /// <summary>
        /// Function for retrieving credit note information of specific tax invoice no. (sp_IC_GetTbt_CreditNote)
        /// </summary>
        /// <param name="taxInvoiceNo">tax invoice no.</param>
        /// <returns></returns>
        List<tbt_CreditNote> GetTbt_CreditNote(string creditNoteNo);

        /// <summary>
        /// Function for insert credit note information to DB. (sp_IC_InsertTbt_CreditNote)
        /// </summary>
        /// <param name="doTbt_CreditNote">credit note information</param>
        /// <returns></returns>
        int InsertTbt_CreditNote(tbt_CreditNote doTbt_CreditNote);
        /// <summary>
        /// Function for update credit note information to DB. (sp_IC_UpdateTbt_CreditNote)
        /// </summary>
        /// <param name="doTbt_CreditNote">credit note information</param>
        /// <returns></returns>
        int UpdateTbt_CreditNote(tbt_CreditNote doTbt_CreditNote);

        /// <summary>
        /// Function for update Receipt information to DB. (sp_IC_UpdateTbt_Receipt)
        /// </summary>
        /// <param name="doTbt_Receipt">Receipt information</param>
        /// <returns></returns>
        int UpdateTbt_Receipt(tbt_Receipt doTbt_Receipt);

        /// <summary>
        /// Function for checking whether credit can be cancel. (sp_IC_CheckCreditNoteCanCancel)
        /// </summary>
        /// <param name="strCreditNoteNo">credit note no.</param>
        /// <returns></returns>
        bool CheckCreditNoteCanCancel(string strCreditNoteNo);
        /// <summary>
        /// Function for generate credit note no. and save credit note information to DB.
        /// </summary>
        /// <param name="_dotbt_CreditNote">credit note information</param>
        /// <returns></returns>
        tbt_CreditNote RegisterCreditNote(tbt_CreditNote _dotbt_CreditNote);
        /// <summary>
        /// Function for retrieving credit note information of specific tax invoice no. (sp_IC_GetCreditNote)
        /// </summary>
        /// <param name="strTaxInvoiceNo">tax invoice no.</param>
        /// <returns></returns>
        doGetCreditNote GetCreditNote(string strTaxInvoiceNo);

        

        #endregion

        #region Payment
        //ICS010
        /// <summary>
        /// Generate payment transaction no. and save payment information to DB.
        /// </summary>
        /// <param name="payment">payment information</param>
        /// <returns></returns>
        List<tbt_Payment> RegisterPayment(List<tbt_Payment> payment);
        /// <summary>
        /// Function for insert payment information to DB. (sp_IC_InsertTbt_Payment)
        /// </summary>
        /// <param name="xmlTbt_Payment">xml of payment information</param>
        /// <returns></returns>
        List<tbt_Payment> InsertTbt_Payment(string xmlTbt_Payment);
        /// <summary>
        /// Function for retrieving payment trasaction information of specific advance search criteria. (sp_IC_SearchPayment)
        /// </summary>
        /// <param name="condition">advance search criteria information</param>
        /// <returns></returns>
        List<doPayment> SearchPayment(doPaymentSearchCriteria condition);
        /// <summary>
        /// Function for retrieving payment trasaction information of specific payment transaction no. (sp_IC_GetPayment)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <returns></returns>
        tbt_Payment GetPayment(string paymentTransNo);

        //ICS084
        /// <summary>
        /// Update matchable balance of payment transaction of specific payment transaction no. (sp_IC_UpdatePaymentMatchableBalance)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <param name="adjustAmount">adjust amount</param>
        /// <returns></returns>
        bool UpdatePaymentMatchableBalance(string paymentTransNo, decimal adjustAmount, decimal adjustAmountUsd, string MatchableBalanceCurrencyType); 
        /// <summary>
        /// Update bank fee registered flag of payment transaction of specific payment transaction no. (sp_IC_UpdatePaymentBankFeeFlag)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <param name="bankFeeFlag">bank fee registered flag</param>
        /// <returns></returns>
        bool UpdatePaymentBankFeeFlag(string paymentTransNo, bool bankFeeFlag);
        /// <summary>
        /// Update other income registered flag of payment transaction of specific payment transaction no. (sp_IC_UpdatePaymentOtherIncomeFlag)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <param name="otherIncomeFlag">other income registered flag</param>
        /// <returns></returns>
        bool UpdatePaymentOtherIncomeFlag(string paymentTransNo, bool otherIncomeFlag);
        /// <summary>
        /// Update other expense registered flag of payment transaction of specific payment transaction no. (sp_IC_UpdatePaymentOtherExpenseFlag)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <param name="otherExpenseFlag">other expense registered flag</param>
        /// <returns></returns>
        bool UpdatePaymentOtherExpenseFlag(string paymentTransNo, bool otherExpenseFlag);

        //ICS080
        /// <summary>
        /// Delete payment transaction of specific payment information. (sp_IC_DeletePaymentTransaction)
        /// </summary>
        /// <param name="doPayment">payment information</param>
        /// <returns></returns>
        bool DeletePaymentTransaction(tbt_Payment doPayment);
        #endregion

        #region Unpaid Invoice
        /// <summary>
        /// Function for retrieving unpaid invoice information of specific invoice no.(sp_IC_GetUnpaidInvoice)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <returns></returns>
        List<doUnpaidInvoice> GetUnpaidInvoice(string invoiceNo);
        /// <summary>
        /// Function for retrieving unpaid invoice information of specific billing target code. (sp_IC_GetUnpaidInvoiceByBillingTarget)
        /// </summary>
        /// <param name="billingTargetCode">billing target code</param>
        /// <returns></returns>
        List<doUnpaidInvoice> GetUnpaidInvoiceByBillingTarget(string billingTargetCode);
        #endregion

        #region Unpaid Billing
        /* Confirmed SA on 30 Mar 2012 -> Not use
        List<doUnpaidBillingTarget> GetBillingTargetByCode(string billingTargetCode);
        List<doUnpaidBillingTarget> GetBillingTargetByInvoiceNo(string invoiceNo);
        List<doUnpaidBillingTarget> GetBillingTargetByBillingCode(string billingCode);
        List<doUnpaidBillingTarget> GetBillingTargetByReceiptNo(string receiptNo);
        */
        /// <summary>
        /// Function for retrieving unpaid billing target information of specific search criteria. (sp_IC_SearchUnpaidBillingTarget)
        /// </summary>
        /// <param name="doSearch">search criteria</param>
        /// <returns></returns>
        List<doUnpaidBillingTarget> SearchUnpaidBillingTarget(doUnpaidBillingTargetSearchCriteria doSearch);
        List<doGetUnpaidBillingTargetByCodeWithExchange> SearchUnpaidBillingTargetWithExchange(doUnpaidBillingTargetSearchCriteria doSearch);
        /// <summary>
        /// Function for retrieving unpaid billing target information of specific billing code. (sp_IC_GetUnpaidBillingTargetByBillingCode)
        /// </summary>
        /// <param name="billingCode">billing code</param>
        /// <returns></returns>
        List<doUnpaidBillingTarget> GetUnpaidBillingTargetByBillingCode(string billingCode);
        List<doGetUnpaidBillingTargetByCodeWithExchange> GetUnpaidBillingTargetByBillingCodeWithExchange(string billingCode);
        /// <summary>
        /// Function for retrieving unpaid billing target information of specific billing target code. (sp_IC_GetUnpaidBillingTargetByCode)
        /// </summary>
        /// <param name="billingTargetCode">billing target code</param>
        /// <returns></returns>
        List<doUnpaidBillingTarget> GetUnpaidBillingTargetByCode(string billingTargetCode);
        List<doGetUnpaidBillingTargetByCodeWithExchange> GetUnpaidBillingTargetByCodeWithExchange(string billingTargetCode);
        /// <summary>
        /// Function for retrieving billing target information by customer code (sp_IC_GetUnpaidBillingTargetByCustomerCode)
        /// </summary>
        /// <param name="customerCode">Customer Code</param>
        /// <returns></returns>
        List<doUnpaidBillingTarget> GetUnpaidBillingTargetByCustomerCode(string customerCode); //Add by Jutarat A. on 09042013
        List<doGetUnpaidBillingTargetByCodeWithExchange> GetUnpaidBillingTargetByCustomerCodeWithExchange(string customerCode); //Add by Jutarat A. on 09042013
        /// <summary>
        /// Function for retrieving unpaid billing target information of specific invoice no. (sp_IC_GetUnpaidBillingTargetByInvoiceNo)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <returns></returns>
        List<doUnpaidBillingTarget> GetUnpaidBillingTargetByInvoiceNo(string invoiceNo);
        List<doGetUnpaidBillingTargetByCodeWithExchange> GetUnpaidBillingTargetByInvoiceNoWithExchange(string invoiceNo);
        /// <summary>
        /// Function for retrieving unpaid billing target information of specific receipt no. (sp_IC_GetUnpaidBillingTargetByReceiptNo)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        List<doUnpaidBillingTarget> GetUnpaidBillingTargetByReceiptNo(string receiptNo);
        List<doGetUnpaidBillingTargetByCodeWithExchange> GetUnpaidBillingTargetByReceiptNoWithExchange(string receiptNo);

        /// <summary>
        /// Function for retrieving unpaid billing detail information of specific billing target code. (sp_IC_GetUnpaidBillingDetailByBillingTarget)
        /// </summary>
        /// <param name="billingTargetCode"></param>
        /// <returns></returns>
        List<doUnpaidBillingDetail> GetUnpaidBillingDetailByBillingTarget(string billingTargetCode);
        /// <summary>
        /// Function for retrieving unpaid billing detail information of specific invoice no. and invoice occ (sp_IC_GetUnpaidBillingDetailByInvoice)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="invoiceOCC">invoice occ</param>
        /// <returns></returns>
        List<doUnpaidBillingDetail> GetUnpaidBillingDetailByInvoice(string invoiceNo, int? invoiceOCC);
        #endregion

        #region Matching Payment / Cancel
        //ICS080: View
        /// <summary>
        /// Function all matched invoice for given payment transaction
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <returns></returns>
        List<tbt_Invoice> GetInvoicePaymentMatchingList(string paymentTransNo);

        //ICS084: Matching
        /// <summary>
        /// Function for retrieving payment matching result of a payment transaction from tbt_MatchPaymentHeader and tbt_MatchPaymentDetail. (sp_IC_GetPaymentMatchingResult, sp_IC_GetPaymentMatchingResult_Detail)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <returns></returns>
        List<doPaymentMatchingResult> GetPaymentMatchingResult(string paymentTransNo);
        /// <summary>
        /// Function to retrieve description to show in viewing payment matching result. (sp_IC_GetPaymentMatchingDesc)
        /// </summary>
        /// <param name="valueCode">payment matching code</param>
        /// <returns></returns>
        doPaymentMatchingDesc GetPaymentMatchingDesc(string valueCode);
        /// <summary>
        /// Function for checking whether an invoice is matching to only refund payment. (sp_IC_CheckAllMatchingToRefund)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="invoiceOCC">invoice occ</param>
        /// <returns></returns>
        bool CheckAllMatchingToRefund(string invoiceNo, int invoiceOCC);
        /// <summary>
        /// Match payment and invoice(s). Can match payment to multiple invoices. Match payment according to amount user input.
        /// </summary>
        /// <param name="matchHeader">match payment header</param>
        /// <param name="payment">payment information</param>
        /// <returns></returns>
        bool MatchPaymentInvoices(doMatchPaymentHeader matchHeader, tbt_Payment payment);

        /// <summary>
        /// (สำหรับหน้าที่สามารถแปลงสกุลเงินได้) Match payment and invoice(s). Can match payment to multiple invoices. Match payment according to amount user input.
        /// </summary>
        /// <param name="matchHeader"></param>
        /// <param name="payment"></param>
        /// <param name="FirstPaymentAmountCurrencyType"></param>
        /// <returns></returns>
        bool MatchPaymentInvoices(doMatchPaymentHeader matchHeader, tbt_Payment payment, string FirstPaymentAmountCurrencyType);

        //ICS090: Cancel
        /// <summary>
        /// Function for retrieving payment matching header and detail of an invoice. (sp_IC_GetMatchPaymentHeaderByInvoice, sp_IC_GetMatchPaymentHeaderByInvoice_Detail)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="invoiceOCC">invoice occ</param>
        /// <returns></returns>
        List<doMatchPaymentHeader> GetMatchPaymentHeaderByInvoice(string invoiceNo, int invoiceOCC);
        /// <summary>
        /// Function for update match payment header information to DB. (sp_IC_UpdateTbt_MatchPaymentHeader)
        /// </summary>
        /// <param name="xmlTbt_MatchPaymentHeader">xml of match payment header</param>
        /// <returns></returns>
        int UpdateTbt_MatchPaymentHeader(string xmlTbt_MatchPaymentHeader);
        /// <summary>
        /// Function for update match payment detail information to DB. (sp_IC_UpdateTbt_MatchPaymentDetail)
        /// </summary>
        /// <param name="xmlTbt_MatchPaymentDetail">xml of match payment detail</param>
        /// <returns></returns>
        int UpdateTbt_MatchPaymentDetail(string xmlTbt_MatchPaymentDetail);
        /// <summary>
        /// Cancel payment matching information of specific invoice information and cancel information
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="correctionReason">correction reason</param>
        /// <param name="approveNo">approve no.</param>
        /// <returns></returns>
        bool CancelPaymentMatching(doInvoice doInvoice, string correctionReason,string approveNo);
        /// <summary>
        /// Function to retrieve description for the payment status.
        /// </summary>
        /// <param name="paymentStatus">payment status</param>
        /// <returns></returns>
        doMiscTypeCode GetpaymentStatusDesc(string paymentStatus);


        /// <summary>
        /// Update Encashed flag of payment transaction of specific payment transaction no. (sp_IC_UpdatePaymentEncashFlag)
        /// </summary>
        bool UpdatePaymentEncashFlag(doPaymentEncashParam param);

        /// <summary>
        /// Function for checking whether invoice has been issued a receipt. (sp_IC_CheckInvoiceIssuedReceipt)
        /// </summary>
        byte CheckAllPaymentEncashed(string invoiceNo, int invoiceOCC);

        /// <summary>
        /// Function for checking the receipt can be cancel tax invoice. (sp_IC_CheckCancelTaxInvoiceOption)
        /// </summary>
        bool CheckCancelTaxInvoiceOption(string invoiceNo, int invoiceOCC);

        /// <summary>
        /// Function for checking that tax invoice no. can register credit note. (sp_IC_CheckCanRegisterCreditNote)
        /// </summary>
        bool CheckCanRegisterCreditNote(string invoiceNo);

        /// <summary>
        /// Function for get total amount included VAT of all credit note.
        /// </summary>
        decimal GetTotalCreditAmtIncVAT(string invoiceNo);

        /// <summary>
        /// Check the last update row has deposit status is C_DEPOSIT_STATUS_RETURN  //2. (sp_IC_CheckDepositStatusReturn)
        /// </summary>
        bool CheckDepositStatusReturn(string ContractCode);

        /// <summary>
        /// canceling credit note.
        /// </summary>
        bool CancelCreditNote(tbt_BillingBasic _doBillingBasic, doGetCreditNote _doGetCreditNote);

        

        #endregion

        #region Import Payment / Auto payment matching
        //ICS020
        /// <summary>
        /// Function for loading payment content data file.
        /// </summary>
        /// <param name="importID">import id</param>
        /// <param name="csvFilePath">csv file path</param>
        /// <param name="secomBankBranch">SECOM bank/brach id</param>
        /// <param name="paymentType">payment type</param>
        /// <returns></returns>
        int LoadPaymentDataFile(Guid importID, string csvFilePath, int secomBankBranch, string paymentType,string bankCode,string currencyType);
        /// <summary>
        /// Function for insert payment content data to DB. (sp_IC_InsertTbt_tmpImportContent)
        /// </summary>
        /// <param name="tmpImportContents">payment content data</param>
        /// <returns></returns>
        List<tbt_tmpImportContent> InsertTbt_tmpImportContent(List<tbt_tmpImportContent> tmpImportContents);
        /// <summary>
        /// Function for retrieving payment content data of specific import id. (sp_IC_GetTbt_tmpImportContent)
        /// </summary>
        /// <param name="importID">import id</param>
        /// <returns></returns>
        List<tbt_tmpImportContent> GetTbt_tmpImportContent(Guid importID);
        /// <summary>
        /// Function for validating that the import contain all invoice in export file. (sp_IC_ValidateWholeFile)
        /// </summary>
        /// <param name="importID">import id</param>
        /// <returns>The remaining invoice no.</returns>
        List<doValidateWholeFile> ValidateWholeFile(Guid importID);
        /// <summary>
        /// Function for validating business for auto transfer file content. (sp_IC_ValidateAutoTransferContent)
        /// </summary>
        /// <param name="importID">import id</param>
        void ValidateAutoTransferContent(Guid importID, int secomAccountID);
        /// <summary>
        /// Function for validating business for bank transfer file content. (sp_IC_ValidateBankTransferContent)
        /// </summary>
        /// <param name="importID">import id</param>
        void ValidateBankTransferContent(Guid importID);
        /// <summary>
        /// Function for checking that auto transfer has been imported. (sp_IC_CheckAutoTransferFileImported)
        /// </summary>
        /// <param name="importID">import id</param>
        /// <param name="secomAccountID">SECOM account id</param>
        /// <returns></returns>
        bool CheckAutoTransferFileImported(Guid importID, int secomAccountID);
        /// <summary>
        /// Function for insert payment import file data to DB. (sp_IC_InsertTbt_PaymentImportFile)
        /// </summary>
        /// <param name="importFiles">payment import file data</param>
        /// <returns></returns>
        List<tbt_PaymentImportFile> InsertTbt_PaymentImportFile(List<tbt_PaymentImportFile> importFiles);
        /// <summary>
        /// Match payment and invoice. The amount matched is equal to payment amount. Other amount is set automatically.
        /// </summary>
        /// <param name="doPayment">payment information</param>
        /// <param name="doInvoice">invoice information</param>
        /// <returns></returns>
        bool MatchPaymentInvoiceAuto(tbt_Payment doPayment, tbt_Invoice doInvoice);
        #endregion



        #region Debt target
        /// <summary>
        /// Function for retrieving debt target data. (sp_IC_GetDebtTarget)
        /// </summary>
        /// <returns></returns>
        List<doGetDebtTarget> GetDebtTarget();
        /// <summary>
        /// Function for update debt target information to DB. (sp_IC_UpdateTbt_DebtTarget)
        /// </summary>
        /// <param name="dotbt_DebtTarget">debt target information</param>
        /// <returns></returns>
        int UpdateTbt_DebtTarget(tbt_DebtTarget dotbt_DebtTarget);
        #endregion

        #region Debt tracing
        /// <summary>
        /// Function for retrieving debt tracing memo information of specific billing target code, invoice no., invoice occ. (sp_IC_GetDebtTracingMemo)
        /// </summary>
        /// <param name="strBillingTargetCode">billing target code</param>
        /// <param name="strInvoiceNo">invoice no.</param>
        /// <param name="strInvoiceOCC">invoice occ</param>
        /// <returns></returns>
        List<doGetDebtTracingMemo> GetDebtTracingMemoList(string strBillingTargetCode, string strInvoiceNo, int? strInvoiceOCC);
        /// <summary>
        /// Function for insert billing target debt tracing data to DB. (sp_IC_InsertTbt_BillingTargetDebtTracing)
        /// </summary>
        /// <param name="doTbt_BillingTargetDebtTracing">billing target debt tracing information</param>
        /// <returns></returns>
        int InsertTbt_BillingTargetDebtTracing(tbt_BillingTargetDebtTracing doTbt_BillingTargetDebtTracing);
        /// <summary>
        /// Function for insert invoice debt tracing data to DB. (sp_IC_InsertTbt_InvoiceDebtTracing)
        /// </summary>
        /// <param name="doTbt_InvoiceDebtTracing">invoice debt tracing data</param>
        /// <returns></returns>
        int InsertTbt_InvoiceDebtTracing(tbt_InvoiceDebtTracing doTbt_InvoiceDebtTracing);
        #endregion

        #region Dept summary
        /// <summary>
        /// Function for retrieving debt summary data of each billing office. (sp_IC_GetBillingOfficeDebtSummary)
        /// </summary>
        /// <param name="intMonth">month</param>
        /// <param name="intYear">year</param>
        /// <returns></returns>
        List<doGetBillingOfficeDebtSummary> GetBillingOfficeDebtSummaryList(int? intMonth, int? intYear);
        /// <summary>
        /// Function for retrieving debt summary data of each billing target in a billing office. (sp_IC_GetBillingTargetDebtSummaryByOffice)
        /// </summary>
        /// <param name="strBillingTargetCode">billing target code</param>
        /// <param name="intMonth">month</param>
        /// <param name="intYear">year</param>
        /// <returns></returns>
        List<doGetBillingTargetDebtSummaryByOffice> GetBillingTargetDebtSummaryByOfficeList(string strBillingTargetCode, int? intMonth, int? intYear);
        /// <summary>
        /// Function for retrieving unpaid invoices of a billing target. (sp_IC_GetUnpaidInvoiceDebtSummaryByBillingTarget)
        /// </summary>
        /// <param name="strBillingTargetCode">billing target code</param>
        /// <returns></returns>
        List<doGetUnpaidInvoiceDebtSummaryByBillingTarget> GetUnpaidInvoiceDebtSummaryByBillingTargetList(string strBillingTargetCode, string strOfficeCode = null); //Add (strOfficeCode) by Jutarat A. on 10042014
        /// <summary>
        /// Function for retrieving unpaid invoices debt summary by invoice no. (sp_IC_GetUnpaidInvoiceDebtSummaryByInvoiceNo)
        /// </summary>
        /// <param name="strInvoiceNo">invoice no.</param>
        /// <returns></returns>
        List<doGetUnpaidInvoiceDebtSummaryByBillingTarget> GetUnpaidInvoiceDebtSummaryByInvoiceNo(string strInvoiceNo);

        /// <summary>
        /// Function for retrieving debt summary of a billing code. (sp_IC_GetBillingCodeDebtSummary)
        /// </summary>
        /// <param name="strBillingCode">billing code</param>
        /// <returns></returns>
        List<doGetBillingCodeDebtSummary> GetBillingCodeDebtSummaryList(string strBillingCode);

        /// <summary>
        /// Function for retrieving unpaid billing detail of a billing target. (sp_IC_GetUnpaidDetailDebtSummaryByBillingTarget)
        /// </summary>
        /// <param name="strBillingTargetCode">billing target code</param>
        /// <returns></returns>
        List<doGetUnpaidDetailDebtSummary> GetUnpaidDetailDebtSummaryByBillingTargetList(string strBillingTargetCode, string strOfficeCode = null); //Add (strOfficeCode) by Jutarat A. on 11042014
        /// <summary>
        /// Function for retrieving unpaid invoices of a billing target. (sp_IC_GetUnpaidDetailDebtSummaryByInvoice)
        /// </summary>
        /// <param name="strInvoiceNo">invoice no.</param>
        /// <param name="intInvoiceOCC">invoice occ</param>
        /// <returns></returns>
        List<doGetUnpaidDetailDebtSummary> GetUnpaidDetailDebtSummaryByInvoiceList(string strInvoiceNo, int? intInvoiceOCC, string strOfficeCode = null); //Add (strOfficeCode) by Jutarat A. on 11042014
        /// <summary>
        /// Function for retrieving unpaid billing target of a billing code. (sp_IC_GetUnpaidDetailDebtSummaryByBillingCode)
        /// </summary>
        /// <param name="strBillingCode">billing code</param>
        /// <returns></returns>
        List<doGetUnpaidDetailDebtSummary> GetUnpaidDetailDebtSummaryByBillingCodeList(string strBillingCode);
        #endregion

        #region Money collection info
        /// <summary>
        /// Function for insert money collection info to DB. (sp_IC_InsertTbt_MoneyCollectionInfo)
        /// </summary>
        /// <param name="dotbt_MoneyCollectionInfo"></param>
        /// <returns></returns>
        int CreateTbt_MoneyCollectionInfo(tbt_MoneyCollectionInfo dotbt_MoneyCollectionInfo);
        /// <summary>
        /// Function for retrieving money collection management information. (sp_IC_GetMoneyCollectionManagementInfo)
        /// </summary>
        /// <param name="expectedCollectDateFrom">from expected collection date</param>
        /// <param name="expectedCollectDateTo">to expected collection date</param>
        /// <param name="collectionAreaCode"></param>
        /// <returns></returns>
        List<doGetMoneyCollectionManagementInfo> GetMoneyCollectionManagementInfoList(DateTime? expectedCollectDateFrom, DateTime? expectedCollectDateTo, string collectionAreaCode);
        /// <summary>
        /// Delete money collection information of specific receipt no. (sp_IC_DeleteTbt_MoneyCollectionInfo)
        /// </summary>
        /// <param name="ReceiptNo">receipt no.</param>
        /// <returns></returns>
        List<tbt_MoneyCollectionInfo> DeleteTbt_MoneyCollectionInfo(string ReceiptNo);

        /// <summary>
        /// Function for retrieving money collection information of specific receipt no. (sp_IC_GetTbt_MoneyCollectionInfo)
        /// </summary>
        /// <param name="strReceiptNo">receipt no.</param>
        /// <returns></returns>
        List<tbt_MoneyCollectionInfo> GetTbt_MoneyCollectionInfo(string strReceiptNo);
        #endregion

        #region Revenue
        /// <summary>
        /// Function for insert revenue information to DB. (sp_IC_InsertTbt_Revenue)
        /// </summary>
        /// <param name="doTbt_Revenue">revenue information</param>
        /// <returns></returns>
        int InsertTbt_Revenue(tbt_Revenue doTbt_Revenue);
        /// <summary>
        /// Generate revenue no. and save revenue information to DB.
        /// </summary>
        /// <param name="_dotbt_Revenue"></param>
        /// <returns></returns>
        tbt_Revenue RegisterRevenue(tbt_Revenue _dotbt_Revenue);
        #endregion

        #region Reg content
        /// <summary>
        /// Function to retrieve description to show in credit note/revenue register. (sp_IC_GetRegContent)
        /// </summary>
        /// <param name="strRegContentCode">reg content code</param>
        /// <returns></returns>
        List<doGetRegContent> GetRegContent(string strRegContentCode);
        #endregion

        #region Miscellaneous
        /// <summary>
        /// Function for retrieving the working day no. of a date. (sp_IC_GetWorkingDayNoOfMonth)
        /// </summary>
        /// <param name="getNextWorkingDay">working datetime</param>
        /// <returns></returns>
        int GetWorkingDayNoOfMonth(DateTime getNextWorkingDay);
        #endregion

        List<doPaymentForWHT> SearchPaymentForWHT(doPaymentForWHTSearchCriteria param);
        string GenerateWHTNo(DateTime? matchingdate);
        List<tbt_IncomeWHT> InsertTbt_IncomeWHT(List<tbt_IncomeWHT> wht);
        List<tbt_IncomeWHT> UpdateTbt_IncomeWHT(List<tbt_IncomeWHT> wht);
        List<tbt_IncomeWHT> GetTbt_IncomeWHT(string WHTNo);
        List<tbt_Payment> UpdateWHTNoToPayment(string WHTNo, List<string> lstPaymentTransNo, string UpdateBy, DateTime UpdateDate);
        List<doMatchWHTDetail> GetMatchWHTDetail(string wHTNo, string c_CURRENCY_LOCAL, string c_CURRENCY_US);
        List<doIncomeWHT> SearchIncomeWHT(doIncomeWHTSearchCriteria param);
        List<doWHTReportForAccount> GetWHTReportForAccount(DateTime? yearMonthFrom, DateTime? yearMonthTo);
        List<doWHTReportForIMS> GetWHTReportForIMS(DateTime? yearMonthFrom, DateTime? yearMonthTo);
        doWHTYearMonth GetWHTYearMonth();
        List<doDebtTracingPermission> GetDebtTracingPermission(string empNo);
        List<doDebtTracingCustList> GetDebtTracingCustList(string empNo, doDebtTracingCustListSearchCriteria param);
        List<doReturnedCheque> GetReturnedCheque(string billingTargetCode);

        List<doDebtTracingInvoiceList> GetDebtTracingInvoiceList(string billingTargetCode, string serviceTypeCode, string debtTracingSubStatus, string empNo);
        List<doDebtTracingInvoiceDetail> GetDebtTracingInvoiceDetail(string billingTargetCode, string invoiceNo, int? invoiceOCC);
        List<doTbm_DebtTracingPermission> GetTbm_DebtTracingPermission(string empNo);
        List<tbt_DebtTracingHistory> GetTbt_DebtTracingHistory(string billingTargetCode);
        List<tbt_DebtTracingHistoryDetail> GetTbt_DebtTracingHistoryDetail(int? historyID);
        List<tbt_DebtTracingHistory> InsertTbt_DebtTracingHistory(List<tbt_DebtTracingHistory> doInsertList);
        List<tbt_DebtTracingHistoryDetail> InsertTbt_DebtTracingHistoryDetail(List<tbt_DebtTracingHistoryDetail> doInsertList);
        List<tbt_DebtTracingCustCondition> GetTbt_DebtTracingCustCondition(string billingTargetCode);
        List<tbt_DebtTracingCustCondition> InsertTbt_DebtTracingCustCondition(List<tbt_DebtTracingCustCondition> doInsertList);
        List<tbt_DebtTracingCustCondition> UpdateTbt_DebtTracingCustCondition(List<tbt_DebtTracingCustCondition> doUpdateList);
        List<doDebtTracingHistory> GetDebtTracingHistory(string billingTargetCode);
        List<doGetMatchGroupNamePayment> getMatchGroupNameCbo(DateTime? paymentDate, string empno);
        List<doGetICR050> GetListIRC050(doMatchRReport doMatchReport);
        void SaveDebtTracingInput(doDebtTracingInput input, bool isHQUser);
    }
}
