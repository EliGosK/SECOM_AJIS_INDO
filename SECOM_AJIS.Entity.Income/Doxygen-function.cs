namespace SECOM_AJIS.DataEntity.Income
{
	partial class BizICDataEntities
	{
		/// \fn public virtual int BatchDeleteDebtTracing(string c_PAYMENT_STATUS_BANK_PAID, string c_PAYMENT_STATUS_AUTO_PAID, string c_PAYMENT_STATUS_CANCEL, string c_PAYMENT_STATUS_CASH_PAID, string c_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED, string c_PAYMENT_STATUS_BILLING_EXEMPTION, string c_PAYMENT_STATUS_CHEQUE_PAID, string c_PAYMENT_STATUS_NOTE_MATCHED, string c_PAYMENT_STATUS_NOTE_FAIL, string c_PAYMENT_STATUS_CASHIER_PAID, string c_PAYMENT_STATUS_POST_MATCHED, string c_PAYMENT_STATUS_POST_FAIL, string c_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN, string c_PAYMENT_STATUS_PAID_WITH_PARTIAL_REFUND, string c_PAYMENT_STATUS_REFUND_PAID, ObjectParameter totalRow, ObjectParameter completedRow)
		/// \brief (Call stored procedure: sp_IC_BatchDeleteDebtTracing).

		/// \fn public virtual int BatchGenDebtSummary(string batchId, Nullable<System.DateTime> batchDate, string c_BILLING_TYPE_GROUP_CONTINUES, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN, ObjectParameter totalEffectRow)
		/// \brief (Call stored procedure: sp_IC_BatchGenDebtSummary).

		/// \fn public virtual List<tbt_Receipt> CancelReceipt(string receiptNo, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_IC_CancelReceipt).

		/// \fn public virtual int CheckAllMatchingToRefund(string invoiceNo, Nullable<int> invoiceOCC, string c_PAYMENT_TYPE_CN_REFUND, ObjectParameter oUT_ALL_REFUND)
		/// \brief (Call stored procedure: sp_IC_CheckAllMatchingToRefund).

		/// \fn public virtual int CheckAutoTransferFileImported(Nullable<System.Guid> importID, Nullable<int> sECOMAccountID, string c_PAYMENT_TYPE_AUTO_TRANSFER, ObjectParameter iS_IMPORTED)
		/// \brief (Call stored procedure: sp_IC_CheckAutoTransferFileImported).

		/// \fn public virtual List<string> CheckCreditNoteCanCancel(string strCreditNoteNo)
		/// \brief (Call stored procedure: sp_IC_CheckCreditNoteCanCancel).

		/// \fn public virtual int CheckInvoiceIssuedReceipt(string invoiceNo, Nullable<int> invoiceOCC, ObjectParameter isIssued)
		/// \brief (Call stored procedure: sp_IC_CheckInvoiceIssuedReceipt).

		/// \fn public virtual List<tbt_Payment> DeletePaymentTransaction(string paymentTransNo, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_IC_DeletePaymentTransaction).

		/// \fn public virtual List<tbt_MoneyCollectionInfo> DeleteTbt_MoneyCollectionInfo(string receiptNo)
		/// \brief (Call stored procedure: sp_IC_DeleteTbt_MoneyCollectionInfo).

		/// \fn public virtual List<doReceipt> GetAdvanceReceipt(string receiptNo)
		/// \brief (Call stored procedure: sp_IC_GetAdvanceReceipt).

		/// \fn public virtual List<doGetBillingCodeDebtSummary> GetBillingCodeDebtSummary(string billingCode, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetBillingCodeDebtSummary).

		/// \fn public virtual List<doGetBillingOfficeDebtSummary> GetBillingOfficeDebtSummary(Nullable<int> intMonth, Nullable<int> intYear)
		/// \brief (Call stored procedure: sp_IC_GetBillingOfficeDebtSummary).

		/// \fn public virtual List<doGetBillingTargetDebtSummaryByOffice> GetBillingTargetDebtSummaryByOffice(string strBillingTargetCode, Nullable<int> intMonth, Nullable<int> intYear)
		/// \brief (Call stored procedure: sp_IC_GetBillingTargetDebtSummaryByOffice).

		/// \fn public virtual List<doGetCreditNote> GetCreditNote(string creditNoteNo)
		/// \brief (Call stored procedure: sp_IC_GetCreditNote).

		/// \fn public virtual List<doGetDebtTarget> GetDebtTarget()
		/// \brief (Call stored procedure: sp_IC_GetDebtTarget).

		/// \fn public virtual List<doGetDebtTracingMemo> GetDebtTracingMemo(string strBillingTargetCode, string strInvoiceNo, Nullable<int> strInvoiceOCC)
		/// \brief (Call stored procedure: sp_IC_GetDebtTracingMemo).

		/// \fn public virtual List<doInvoiceAdvanceReceipt> GetInvoiceAdvanceReceipt(string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_BILLING_TYPE_GROUP_SALE, string c_ISSUE_REC_TIME_SAME_INV, string c_PAYMENT_METHOD_MESSENGER)
		/// \brief (Call stored procedure: sp_IC_GetInvoiceAdvanceReceipt).

		/// \fn public virtual List<doMatchPaymentHeader> GetMatchPaymentHeaderByInvoice(string invoiceNo, Nullable<int> invoiceOCC)
		/// \brief (Call stored procedure: sp_IC_GetMatchPaymentHeaderByInvoice).

		/// \fn public virtual List<doMatchPaymentDetail> GetMatchPaymentHeaderByInvoiceDetail(string matchId)
		/// \brief (Call stored procedure: sp_IC_GetMatchPaymentHeaderByInvoice_Detail).

		/// \fn public virtual List<doGetMoneyCollectionManagementInfo> GetMoneyCollectionManagementInfo(Nullable<System.DateTime> expectedCollectDateFrom, Nullable<System.DateTime> expectedCollectDateTo, string collectionAreaCode)
		/// \brief (Call stored procedure: sp_IC_GetMoneyCollectionManagementInfo).

		/// \fn public virtual List<doPaidInvoiceNoReceipt> GetPaidInvoiceNoReceipt(string c_PAYMENT_STATUS_BANK_PAID, string c_PAYMENT_STATUS_AUTO_PAID, string c_PAYMENT_STATUS_CASH_PAID, string c_PAYMENT_STATUS_CHEQUE_PAID, string c_PAYMENT_STATUS_CASHIER_PAID, string c_PAYMENT_STATUS_NOTE_MATCHED, string c_PAYMENT_STATUS_POST_MATCHED, string c_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN, string c_PAYMENT_STATUS_PAID_WITH_PARTIAL_REFUND, string c_PAYMENT_STATUS_REFUND_PAID)
		/// \brief (Call stored procedure: sp_IC_GetPaidInvoiceNoReceipt).

		/// \fn public virtual List<tbt_Payment> GetPayment(string paymentTransNo)
		/// \brief (Call stored procedure: sp_IC_GetPayment).

		/// \fn public virtual List<doPaymentMatchingDesc> GetPaymentMatchingDesc(string valueCode, string c_PAYMENT_MATCHING_DESC)
		/// \brief (Call stored procedure: sp_IC_GetPaymentMatchingDesc).

		/// \fn public virtual List<doPaymentMatchingResult> GetPaymentMatchingResult(string paymentTransNo)
		/// \brief (Call stored procedure: sp_IC_GetPaymentMatchingResult).

		/// \fn public virtual List<doPaymentMatchingResultDetail> GetPaymentMatchingResult_Detail(string matchID)
		/// \brief (Call stored procedure: sp_IC_GetPaymentMatchingResult_Detail).

		/// \fn public virtual List<doReceipt> GetReceipt(string receiptNo)
		/// \brief (Call stored procedure: sp_IC_GetReceipt).

		/// \fn public virtual List<doReceipt> GetReceiptByInvoiceNo(string invoiceNo, Nullable<int> invoiceOCC)
		/// \brief (Call stored procedure: sp_IC_GetReceiptByInvoiceNo).

		/// \fn public virtual List<doReceipt> GetReceiptIncludeCancel(string receiptNo)
		/// \brief (Call stored procedure: sp_IC_GetReceiptIncludeCancel).

		/// \fn public virtual List<doGetRegContent> GetRegContent(string c_REG_CONTENT, string strRegContentCode)
		/// \brief (Call stored procedure: sp_IC_GetRegContent).

		/// \fn public virtual List<doRptCreditNote> GetRptCreditNote(string creditNoteNo)
		/// \brief (Call stored procedure: sp_IC_GetRptCreditNote).

		/// \fn public virtual List<doRptReceipt> GetRptReceipt(string receiptNo)
		/// \brief (Call stored procedure: sp_IC_GetRptReceipt).

		/// \fn public virtual List<SECOMBankBranchData> GetSECOMBankBranch()
		/// \brief (Call stored procedure: sp_IC_GetSECOMBankBranch).

		/// \fn public virtual List<tbt_CreditNote> GetTbt_CreditNote(string taxInvoiceNo)
		/// \brief (Call stored procedure: sp_IC_GetTbt_CreditNote).

		/// \fn public virtual List<tbt_MoneyCollectionInfo> GetTbt_MoneyCollectionInfo(string receiptNo)
		/// \brief (Call stored procedure: sp_IC_GetTbt_MoneyCollectionInfo).

		/// \fn public virtual List<tbt_tmpImportContent> GetTbt_tmpImportContent(Nullable<System.Guid> importID)
		/// \brief (Call stored procedure: sp_IC_GetTbt_tmpImportContent).

		/// \fn public virtual List<doUnpaidBillingDetail> GetUnpaidBillingDetailByBillingTarget(string billingTargetCode, string c_PAYMENT_STATUS, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidBillingDetailByBillingTarget).

		/// \fn public virtual List<doUnpaidBillingDetail> GetUnpaidBillingDetailByInvoice(string invoiceNo, Nullable<int> invoiceOCC, string c_PAYMENT_STATUS, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidBillingDetailByInvoice).

		/// \fn public virtual List<doUnpaidBillingTarget> GetUnpaidBillingTargetByBillingCode(string billingCode, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidBillingTargetByBillingCode).

		/// \fn public virtual List<doUnpaidBillingTarget> GetUnpaidBillingTargetByCode(string billingTargetCode, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidBillingTargetByCode).

		/// \fn public virtual List<doUnpaidBillingTarget> GetUnpaidBillingTargetByInvoiceNo(string invoiceNo, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidBillingTargetByInvoiceNo).

		/// \fn public virtual List<doUnpaidBillingTarget> GetUnpaidBillingTargetByReceiptNo(string receiptNo, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidBillingTargetByReceiptNo).

		/// \fn public virtual List<doGetUnpaidDetailDebtSummary> GetUnpaidDetailDebtSummaryByBillingCode(string strBillingCode, string c_PAYMENT_STATUS, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidDetailDebtSummaryByBillingCode).

		/// \fn public virtual List<doGetUnpaidDetailDebtSummary> GetUnpaidDetailDebtSummaryByBillingTarget(string strBillingTargetCode, string c_PAYMENT_STATUS, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidDetailDebtSummaryByBillingTarget).

		/// \fn public virtual List<doGetUnpaidDetailDebtSummary> GetUnpaidDetailDebtSummaryByInvoice(string invoiceNo, Nullable<int> invoiceOCC, string c_PAYMENT_STATUS, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidDetailDebtSummaryByInvoice).

		/// \fn public virtual List<doUnpaidInvoice> GetUnpaidInvoice(string invoiceNo, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidInvoice).

		/// \fn public virtual List<doUnpaidInvoice> GetUnpaidInvoiceByBillingTarget(string billingTargetCode, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidInvoiceByBillingTarget).

		/// \fn public virtual List<doGetUnpaidInvoiceDebtSummaryByBillingTarget> GetUnpaidInvoiceDebtSummaryByBillingTarget(string strBillingTargetCode, string c_PAYMENT_STATUS, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidInvoiceDebtSummaryByBillingTarget).

		/// \fn public virtual List<doGetUnpaidInvoiceDebtSummaryByBillingTarget> GetUnpaidInvoiceDebtSummaryByInvoiceNo(string strInvoiceNo, string c_PAYMENT_STATUS, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_GetUnpaidInvoiceDebtSummaryByInvoiceNo).

		/// \fn public virtual List<Nullable<int>> GetWorkingDayNoOfMonth(Nullable<System.DateTime> getNextWorkingDay)
		/// \brief (Call stored procedure: sp_IC_GetWorkingDayNoOfMonth).

		/// \fn public virtual List<tbt_BillingTargetDebtTracing> InsertTbt_BillingTargetDebtTracing(string xmlTbt_BillingTargetDebtTracing)
		/// \brief (Call stored procedure: sp_IC_InsertTbt_BillingTargetDebtTracing).

		/// \fn public virtual List<tbt_CreditNote> InsertTbt_CreditNote(string xmlTbt_CreditNote)
		/// \brief (Call stored procedure: sp_IC_InsertTbt_CreditNote).

		/// \fn public virtual List<tbt_InvoiceDebtTracing> InsertTbt_InvoiceDebtTracing(string xmlTbt_InvoiceDebtTracing)
		/// \brief (Call stored procedure: sp_IC_InsertTbt_InvoiceDebtTracing).

		/// \fn public virtual List<tbt_MatchPaymentDetail> InsertTbt_MatchPaymentDetail(string xmlTbt_MatchPaymentDetail)
		/// \brief (Call stored procedure: sp_IC_InsertTbt_MatchPaymentDetail).

		/// \fn public virtual List<tbt_MatchPaymentHeader> InsertTbt_MatchPaymentHeader(string xmlTbt_MatchPaymentHeader)
		/// \brief (Call stored procedure: sp_IC_InsertTbt_MatchPaymentHeader).

		/// \fn public virtual List<tbt_MoneyCollectionInfo> InsertTbt_MoneyCollectionInfo(string xml_Tbt_MoneyCollectionInfo)
		/// \brief (Call stored procedure: sp_IC_InsertTbt_MoneyCollectionInfo).

		/// \fn public virtual List<tbt_Payment> InsertTbt_Payment(string xmlTbt_Payment)
		/// \brief (Call stored procedure: sp_IC_InsertTbt_Payment).

		/// \fn public virtual List<tbt_PaymentImportFile> InsertTbt_PaymentImportFile(string xmlTbt_tmpImportContent)
		/// \brief (Call stored procedure: sp_IC_InsertTbt_PaymentImportFile).

		/// \fn public virtual List<tbt_Receipt> InsertTbt_Receipt(string xmlTbt_Receipt)
		/// \brief (Call stored procedure: sp_IC_InsertTbt_Receipt).

		/// \fn public virtual List<tbt_Revenue> InsertTbt_Revenue(string xmlTbt_Revenue)
		/// \brief (Call stored procedure: sp_IC_InsertTbt_Revenue).

		/// \fn public virtual List<tbt_tmpImportContent> InsertTbt_tmpImportContent(string xmlTbt_tmpImportContent)
		/// \brief (Call stored procedure: sp_IC_InsertTbt_tmpImportContent).

		/// \fn public virtual List<tbt_Payment> SearchPayment(string paymentType, string status, Nullable<int> sECOMAccountID, string paymentTransNo, string payer, Nullable<System.DateTime> paymentDateFrom, Nullable<System.DateTime> paymentDateTo, Nullable<decimal> matchableBalanceFrom, Nullable<decimal> matchableBalanceTo, string invoiceNo, string receiptNo, string sendingBankCode, string cONST_PAYMENT_TYPE)
		/// \brief (Call stored procedure: sp_IC_SearchPayment).

		/// \fn public virtual List<doUnpaidBillingTarget> SearchUnpaidBillingTarget(string billingClientName, Nullable<decimal> invoiceAmountFrom, Nullable<decimal> invoiceAmountTo, Nullable<System.DateTime> issueInvoiceDateFrom, Nullable<System.DateTime> issueInvoiceDateTo, Nullable<bool> haveCreditNoteIssued, Nullable<decimal> billingDetailAmountFrom, Nullable<decimal> billingDetailAmountTo, string billingType_ContractFee, string billingType_InstallationFee, string billingType_DepositFee, string billingType_SalePrice, string billingType_OtherFee, string paymentMethod_BankTransfer, string paymentMethod_Messenger, string paymentMethod_AutoTransfer, string paymentMethod_CreditCard, Nullable<int> billingCycle, Nullable<int> lastPaymentDayFrom, Nullable<int> lastPaymentDayTo, Nullable<System.DateTime> expectedPaymentDateFrom, Nullable<System.DateTime> expectedPaymentDateTo, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_GEN_AUTO_CREDIT, string c_PAYMENT_STATUS_INV_AUTO_CREDIT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_SearchUnpaidBillingTarget).

		/// \fn public virtual List<tbt_Receipt> UpdateAdvanceReceiptDeletePayment(string receiptNo, string advanceReceiptStatus, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_IC_UpdateAdvanceReceiptDeletePayment).

		/// \fn public virtual List<tbt_Receipt> UpdateAdvanceReceiptMatchPayment(string receiptNo, string advanceReceiptStatus, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_IC_UpdateAdvanceReceiptMatchPayment).

		/// \fn public virtual List<tbt_Receipt> UpdateAdvanceReceiptRegisterPayment(string receiptNo, string advanceReceiptStatus, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_IC_UpdateAdvanceReceiptRegisterPayment).

		/// \fn public virtual List<tbt_Payment> UpdatePaymentBankFeeFlag(string paymentTransNo, Nullable<bool> bankFeeFlag, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_IC_UpdatePaymentBankFeeFlag).

		/// \fn public virtual List<tbt_Payment> UpdatePaymentMatchableBalance(string paymentTransNo, Nullable<decimal> adjustMatchableBalance, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_IC_UpdatePaymentMatchableBalance).

		/// \fn public virtual List<tbt_Payment> UpdatePaymentOtherExpenseFlag(string paymentTransNo, Nullable<bool> otherExpenseFlag, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_IC_UpdatePaymentOtherExpenseFlag).

		/// \fn public virtual List<tbt_Payment> UpdatePaymentOtherIncomeFlag(string paymentTransNo, Nullable<bool> otherIncomeFlag, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_IC_UpdatePaymentOtherIncomeFlag).

		/// \fn public virtual List<tbt_CreditNote> UpdateTbt_CreditNote(string xmlTbt_CreditNote)
		/// \brief (Call stored procedure: sp_IC_UpdateTbt_CreditNote).

		/// \fn public virtual List<tbt_DebtTarget> UpdateTbt_DebtTarget(string xmlTbt_DebtTarget)
		/// \brief (Call stored procedure: sp_IC_UpdateTbt_DebtTarget).

		/// \fn public virtual List<tbt_MatchPaymentDetail> UpdateTbt_MatchPaymentDetail(string xmlTbt_MatchPaymentDetail)
		/// \brief (Call stored procedure: sp_IC_UpdateTbt_MatchPaymentDetail).

		/// \fn public virtual List<tbt_MatchPaymentHeader> UpdateTbt_MatchPaymentHeader(string xmlTbt_MatchPaymentHeader)
		/// \brief (Call stored procedure: sp_IC_UpdateTbt_MatchPaymentHeader).

		/// \fn public virtual int ValidateAutoTransferContent(Nullable<System.Guid> importID, string c_PAYMENT_IMPORT_ERROR_INVOICE_AMOUNT_UNMATCH, string c_PAYMENT_IMPORT_NO_ERROR)
		/// \brief (Call stored procedure: sp_IC_ValidateAutoTransferContent).

		/// \fn public virtual int ValidateBankTransferContent(Nullable<System.Guid> importID, string c_PAYMENT_IMPORT_ERROR_INVALID_INVOICE, string c_PAYMENT_IMPORT_ERROR_IMPORTED_INVOICE, string c_PAYMENT_IMPORT_ERROR_PAID_INVOICE, string c_PAYMENT_IMPORT_NO_ERROR, string c_INC_PAYMENT_IMPORT, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_FAIL_AUTO_INV_BANK, string c_PAYMENT_STATUS_FAIL_NOTE_INV_BANK, string c_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK, string c_PAYMENT_STATUS_PARTIALLY_PAID, string c_PAYMENT_STATUS_PARTIALLY_PAID_CN)
		/// \brief (Call stored procedure: sp_IC_ValidateBankTransferContent).


	}
}

