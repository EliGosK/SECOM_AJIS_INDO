namespace SECOM_AJIS.DataEntity.Billing
{
	partial class BizBLDataEntities
	{
		/// \fn public virtual List<doBLS050GetBillingBasic> BLS050_GetBillingBasic(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_BLS050_GetBillingBasic).

		/// \fn public virtual List<doBLS050GetBillingDetailForCancelList> BLS050_GetBillingDetailForCancelList(string contractCode, string billingOCC, string paymentStatus)
		/// \brief (Call stored procedure: sp_BL_BLS050_GetBillingDetailForCancelList).

		/// \fn public virtual List<doBLS050GetTbt_BillingTargetForView> BLS050_GetTbt_BillingTargetForView(string billingTargetCode)
		/// \brief (Call stored procedure: sp_BL_BLS050_GetTbt_BillingTargetForView).

		/// \fn public virtual List<tbt_TaxInvoice> CancelTaxInvoice(string taxInvoiceNo, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_BL_CancelTaxInvoice).

		/// \fn public virtual int CheckExistReceiptForInvoice(string invoiceNo, Nullable<int> invoiceOCC, ObjectParameter iS_EXIST)
		/// \brief (Call stored procedure: sp_BL_CheckExistReceiptForInvoice).

		/// \fn public virtual int CheckInvoiceIssuedTaxInvoice(string invoiceNo, Nullable<int> invoiceOCC, ObjectParameter iS_ISSUED)
		/// \brief (Call stored procedure: sp_BL_CheckInvoiceIssuedTaxInvoice).

		/// \fn public virtual List<doCheckInvoiceSameAccount> CheckInvoiceSameAccount(string invoiceNo, Nullable<int> invoiceOCC, string paymentMethod)
		/// \brief (Call stored procedure: sp_BL_CheckInvoiceSameAccount).

		/// \fn public virtual List<doCheckInvoiceSameAccountNo> CheckInvoiceSameAccountNo_AUTO_TRANFER(string invoiceNo, Nullable<int> invoiceOCC)
		/// \brief (Call stored procedure: Sp_BL_CheckInvoiceSameAccountNo_AUTO_TRANFER).

		/// \fn public virtual List<doCheckInvoiceSameAccountNo> CheckInvoiceSameAccountNo_CREDIT_CARD(string invoiceNo, Nullable<int> invoiceOCC)
		/// \brief (Call stored procedure: Sp_BL_CheckInvoiceSameAccountNo_CREDIT_CARD).

		/// \fn public virtual int CheckTaxInvoiceIssued(string invoiceNo, Nullable<int> invoiceOCC, ObjectParameter isIssued)
		/// \brief (Call stored procedure: sp_BL_CheckTaxInvoiceIssued).

		/// \fn public virtual List<tbt_AutoTransferBankAccount> DeleteTbt_autoTransferBankAccountdata(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_DeleteTbt_AutoTransferBankAccount).

		/// \fn public virtual List<tbt_BillingDetail> DeleteTbt_BillingDetailData(string contractCode, string billingOCC, Nullable<int> billingDetailNo)
		/// \brief (Call stored procedure: sp_BL_DeleteTbt_BillingDetail).

		/// \fn public virtual List<tbt_BillingTypeDetail> DeleteTbt_BillingTypeDetailData(string contractCode, string billingOCC, string billingTypeCode)
		/// \brief (Call stored procedure: sp_BL_DeleteTbt_BillingTypeDetail).

		/// \fn public virtual List<tbt_CreditCard> DeleteTbt_CreditCard(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_DeleteTbt_CreditCard).

		/// \fn public virtual List<tbt_Invoice> DeleteTbt_Invoice(string invoiceNo, Nullable<int> invoiceOCC)
		/// \brief (Call stored procedure: sp_BL_DeleteTbt_Invoice).

		/// \fn public virtual List<string> GenerateBillingTargetNo(string billingClientCode)
		/// \brief (Call stored procedure: sp_BL_GetMaxBillingTargetNo).

		/// \fn public virtual List<dtAutoTransferBankAccountForView> GetAutoTransferBankAccountForView(string contractCode, string billingOCC, string pC_ACCOUNT_TYPE, string pC_SHOW_AUTO_TRANSFER_RESULT)
		/// \brief (Call stored procedure: sp_BL_GetAutoTransferBankAccountForView).

		/// \fn public virtual List<Nullable<System.DateTime>> GetAutoTransferDate(string bankCode, string autoTransferDate)
		/// \brief (Call stored procedure: sp_BL_GetAutoTransferDate).

		/// \fn public virtual List<Nullable<decimal>> GetBalanceDepositByBillingCode(string strContractCode, string strBillingOCC)
		/// \brief (Call stored procedure: sp_BL_GetBalanceDepositByBillingCode).

		/// \fn public virtual List<doGetBankAutoTransferDateForGen> GetBankAutoTransferDateForGen()
		/// \brief (Call stored procedure: sp_BL_GetBankAutoTransferDateForGen).

		/// \fn public virtual List<doTbt_BillingBasic> GetBillingBasic(string contractCode, string billingOCC, string billingTargetCode, string billingClientCode, string billingOfficeCode)
		/// \brief (Call stored procedure: sp_BL_GetBillingBasic).

		/// \fn public virtual List<dtBillingBasicForRentalList> GetBillingBasicForRentalList(string contractCode)
		/// \brief (Call stored procedure: sp_BL_GetBillingBasicForRentalList).

		/// \fn public virtual List<BillingBasicList> GetBillingBasicList(string contractCode, string c_CUST_TYPE)
		/// \brief (Call stored procedure: sp_BL_GetBillingBasicList).

		/// \fn public virtual List<doGetBillingCodeInfo> GetBillingCodeInfo(string strBillingCode)
		/// \brief (Call stored procedure: sp_BL_GetBillingCodeInfo).

		/// \fn public virtual List<dtBillingContract> GetBillingContract(string contractCode)
		/// \brief (Call stored procedure: sp_BL_GetBillingContract).

		/// \fn public virtual List<tbt_BillingDetail> GetBillingDetailAutoTransferList(string contractCode, string billingOCC, string pC_PAYMENT_STATUS_INV_AUTO_CREDIT)
		/// \brief (Call stored procedure: sp_BL_GetBillingDetailAutoTransferList).

		/// \fn public virtual List<dtBillingDetailByProcess> GetBillingDetailByProcess(string c_PAYMENT_METHOD_AUTO_TRANFER, string c_PAYMENT_METHOD_BANK_TRANSFER, string c_PAYMENT_METHOD_CREDIT_CARD, string c_PAYMENT_METHOD_MESSENGER, string c_PAYMENT_STATUS_INV_BANK_COLLECT, string c_PAYMENT_STATUS_DETAIL_AUTO_CREDIT, string c_BILLING_TYPE_SERVICE, string c_BILLING_TYPE_MA, string c_BILLING_TYPE_SG, string c_BILLING_TYPE_DURING_STOP_SERVICE, string c_BILLING_TYPE_DURING_STOP_MA, string c_BILLING_TYPE_DURING_STOP_SG, string c_BILLING_TYPE_GROUP_CONTINUES, Nullable<System.DateTime> batchDate)
		/// \brief (Call stored procedure: sp_BL_GetBillingDetailByProcess).

		/// \fn public virtual List<doGetBillingDetailContinues> GetBillingDetailContinues(string contractCode, string billingOCC, string paymentStatus, string c_BILLING_TYPE_GROUP_CONTINUES)
		/// \brief (Call stored procedure: sp_BL_GetBillingDetailContinuesList).

		/// \fn public virtual List<doGetBillingDetailForCancel> GetBillingDetailForCancel(string contractCode, string billingOCC, string paymentStatus)
		/// \brief (Call stored procedure: sp_BL_GetBillingDetailForCancelList).

		/// \fn public virtual List<doBillingDetail> GetBillingDetailForCombine(string billingTargetCode)
		/// \brief (Call stored procedure: sp_BL_GetBillingDetailForCombine).

		/// \fn public virtual List<dtBillingDetailForCreateInvoice> GetBillingDetailForCreateInvoice(string c_CUST_TYPE_JURISTIC, string c_PAYMENT_STATUS_CANCEL, string c_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED, string c_PAYMENT_STATUS_NOTE_FAIL, string c_PAYMENT_STATUS_POST_FAIL, string c_SEP_INV_CONTRACT_CODE_ASCE, string c_SEP_INV_SORT_ASCE, string c_SEP_INV_SORT_DESC, string c_SEP_INV_SAME_TYPE_CONTRACT_CODE_ASCE, string c_SEP_INV_SAME_TYPE_SORT_ASCE, string c_SEP_INV_SAME_TYPE_SORT_DESC, string c_BILLING_TYPE_GROUP_CONTINUES, string c_BILLING_TYPE_GROUP_DEPOSIT, string c_SEP_INV_EACH_CONTRACT, Nullable<System.DateTime> batchDate)
		/// \brief (Call stored procedure: sp_BL_GetBillingDetailForCreateInvoice).

		/// \fn public virtual List<Nullable<int>> GetBillingDetailNo(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetMaxBillingDetailNo).

		/// \fn public virtual List<doGetBillingDetailOfInvoice> GetBillingDetailOfInvoice(string invoiceNo, Nullable<int> invoiceOCC, string paymentStatus)
		/// \brief (Call stored procedure: sp_BL_GetBillingDetailOfInvoiceList).

		/// \fn public virtual List<tbt_BillingDetail> GetBillingDetailPartialFeeList(string contractCode, string billingOCC, string billingTypeCode, string paymentStatus)
		/// \brief (Call stored procedure: sp_BL_GetBillingDetailPartialFeeList).

		/// \fn public virtual List<doTbt_MonthlyBillingHistoryList> GetBillingHistoryList(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetBillingHistoryList).

		/// \fn public virtual List<tbt_MonthlyBillingHistory> GetBillingHistoryPeriodList(string contractCode, string billingOCC, Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
		/// \brief (Call stored procedure: sp_BL_GetBillingHistoryPeriodList).

		/// \fn public virtual List<string> GetBillingOCC(string contractCode)
		/// \brief (Call stored procedure: sp_BL_GetMaxBillingOCC).

		/// \fn public virtual List<BillingTypeDetail> GetBillingTypeDetailContinues(string contractCode, string billingOCC, string c_BILLING_TYPE_GROUP_CONTINUES)
		/// \brief (Call stored procedure: sp_BL_GetBillingTypeDetailContinues).

		/// \fn public virtual List<doBillingTypeDetailList> GetBillingTypeDetailList(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetBillingTypeDetailList).

		/// \fn public virtual List<dtCreditCardForView> GetCreditCardForView(string contractCode, string billingOCC, string pC_CREDIT_CARD_TYPE)
		/// \brief (Call stored procedure: sp_BL_GetCreditCardForView).

		/// \fn public virtual List<tbt_DepositFee> GetDepositFee(string contractCode, string billingOCC, string invoiceNo, string depositStatus)
		/// \brief (Call stored procedure: sp_BL_GetDepositFee).

		/// \fn public virtual List<dtDownloadAutoTransferBankFile> GetDownloadAutoTransferBankFile(Nullable<int> secomAccountID, Nullable<System.DateTime> autoTransferDateFrom, Nullable<System.DateTime> autoTransferDateTo, Nullable<System.DateTime> generateDateFrom, Nullable<System.DateTime> generateDateTo)
		/// \brief (Call stored procedure: sp_BL_GetDownloadAutoTransferBankFile).

		/// \fn public virtual List<tbt_ExportAutoTransfer> GetExportAutoTransfer(string bankCode, Nullable<System.DateTime> autoTransferDate)
		/// \brief (Call stored procedure: sp_BL_GetExportAutoTransfer).

		/// \fn public virtual List<tbt_MonthlyBillingHistory> GetFirstBillingHistory(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetFirstBillingHistory).

		/// \fn public virtual List<doInvoice> GetInvoice(string invoiceNo)
		/// \brief (Call stored procedure: sp_BL_GetInvoice).

		/// \fn public virtual List<doGetInvoiceForGenBankFile> GetInvoiceForGenBankFile(string bankCode, string autoTransferDate, string invoicePaymentStatus)
		/// \brief (Call stored procedure: sp_BL_GetInvoiceForGenBankFile).

		/// \fn public virtual List<string> GetInvoiceOCC(string invoiceNo)
		/// \brief (Call stored procedure: sp_BL_GetMaxInvoiceOCC).

		/// \fn public virtual List<tbt_Invoice> GetInvoiceOfChangeDate(string contractCode, string billingOCC, Nullable<System.DateTime> changeFeeDate)
		/// \brief (Call stored procedure: sp_BL_GetInvoiceOfChangeDate).

		/// \fn public virtual List<string> GetInvoiceRunningNo(string runningType, string year, string month, string operateBy, Nullable<System.DateTime> operateDate)
		/// \brief (Call stored procedure: sp_BL_GetInvoiceRunningNo).

		/// \fn public virtual List<doGetInvoiceWithBillingClientName> GetInvoiceWithBillingClientName(string invoiceNo)
		/// \brief (Call stored procedure: sp_BL_GetInvoiceWithBillingClientName).

		/// \fn public virtual List<tbt_MonthlyBillingHistory> GetLastBillingHistory(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetLastBillingHistory).

		/// \fn public virtual List<Nullable<System.DateTime>> GetLastWorkingDay(Nullable<System.DateTime> checkingDate)
		/// \brief (Call stored procedure: sp_BL_GetLastWorkingDay).

		/// \fn public virtual List<tbt_DepositFee> GetLatestDepositFee(string strContracCode, string strBillingOCC)
		/// \brief (Call stored procedure: sp_BL_GetLatestDepositFee).

		/// \fn public virtual List<Nullable<System.DateTime>> GetNextAutoTransferDate(string bankCode, string autoTransferDate)
		/// \brief (Call stored procedure: sp_BL_GetNextAutoTransferDate).

		/// \fn public virtual List<doRefundInfo> GetRefundInfo(string paymentTransNo)
		/// \brief (Call stored procedure: sp_BL_GetRefundInfo).

		/// \fn public virtual List<dtRptInvoice> GetRptInvoice(string invoiceNo, string c_SHOW_DUEDATE, string c_SHOW_DUEDATE_7, string c_SHOW_DUEDATE_30)
		/// \brief (Call stored procedure: sp_BL_GetRptInvoice).

		/// \fn public virtual List<dtRptInvoiceDetail> GetRptInvoiceDetail(string invoiceNo)
		/// \brief (Call stored procedure: sp_BL_GetRptInvoiceDetail).

		/// \fn public virtual List<dtRptInvoiceHeader> GetRptInvoiceHeader(string invoiceNo, string c_SHOW_DUEDATE, string c_SHOW_DUEDATE_7, string c_SHOW_DUEDATE_30)
		/// \brief (Call stored procedure: sp_BL_GetRptInvoiceHeader).

		/// \fn public virtual List<dtRptPaymentForm> GetRptPaymentForm(string invoiceNo)
		/// \brief (Call stored procedure: sp_BL_GetRptPaymentForm).

		/// \fn public virtual List<dtRptTaxInvoice> GetRptTaxInvoice(string taxInvoiceNo, string c_SHOW_DUEDATE, string c_SHOW_DUEDATE_7, string c_SHOW_DUEDATE_30)
		/// \brief (Call stored procedure: sp_BL_GetRptTaxInvoice).

		/// \fn public virtual List<doTax> GetTaxCharged(string contractCode, string billingOCC, string billingTypeCode, Nullable<System.DateTime> invoiceDate)
		/// \brief (Call stored procedure: sp_BL_GetTaxCharged).

		/// \fn public virtual List<tbt_TaxInvoice> GetTaxInvoice(string invoiceNo, Nullable<int> invoiceOCC)
		/// \brief (Call stored procedure: sp_BL_GetTaxInvoice).

		/// \fn public virtual List<tbt_TaxInvoice> GetTaxInvoiceByInvoiceNo(string invoiceNo, Nullable<int> invoiceOCC)
		/// \brief (Call stored procedure: sp_BL_GetTaxInvoiceByInvoiceNo).

		/// \fn public virtual List<doGetTaxInvoiceForIC> GetTaxInvoiceForIC(string strTaxInvoiceNo)
		/// \brief (Call stored procedure: sp_BL_GetTaxInvoiceForIC).

		/// \fn public virtual List<tbt_AutoTransferBankAccount> GetTbt_AutoTransferBankAccount(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetTbt_AutoTransferBankAccount).

		/// \fn public virtual List<dtTbt_AutoTransferBankAccountForView> GetTbt_AutoTransferBankAccountForView(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetTbt_AutoTransferBankAccountForView).

		/// \fn public virtual List<tbt_BillingBasic> GetTbt_BillingBasic(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetTbt_BillingBasic).

		/// \fn public virtual List<dtTbt_BillingBasicForView> GetTbt_BillingBasicForView(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetTbt_BillingBasicForView).

		/// \fn public virtual List<tbt_BillingBasic> GetTbt_BillingBasicListData(string contractCode)
		/// \brief (Call stored procedure: sp_BL_GetTbt_BillingBasicList).

		/// \fn public virtual List<tbt_BillingDetail> GetTbt_BillingDetailData(string contractCode, string billingOCC, Nullable<int> billingDetailNo)
		/// \brief (Call stored procedure: sp_BL_GetTbt_BillingDetail).

		/// \fn public virtual List<tbt_BillingDetail> GetTbt_BillingDetailOfInvoice(string invoiceNo, Nullable<int> invoiceOCC, string c_PAYMENT_STATUS_CANCEL, string c_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED, string c_PAYMENT_STATUS_NOTE_FAIL, string c_PAYMENT_STATUS_POST_FAIL)
		/// \brief (Call stored procedure: sp_BL_GetTbt_BillingDetailOfInvoice).

		/// \fn public virtual List<tbt_BillingTarget> GetTbt_BillingTarget(string billingTargetCode, string billingClientCode, string billingOfficeCode)
		/// \brief (Call stored procedure: sp_BL_GetTbt_BillingTarget).

		/// \fn public virtual List<dtTbt_BillingTargetForView> GetTbt_BillingTargetForView(string billingTargetCode, string c_CUST_TYPE)
		/// \brief (Call stored procedure: sp_BL_GetTbt_BillingTargetForView).

		/// \fn public virtual List<tbt_BillingTypeDetail> GetTbt_BillingTypeDetail(string contractCode, string billingOCC, string billingTypeCode)
		/// \brief (Call stored procedure: sp_BL_GetTbt_BillingTypeDetail).

		/// \fn public virtual List<tbt_CreditCard> GetTbt_CreditCard(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetTbt_CreditCard).

		/// \fn public virtual List<dtTbt_CreditCardForView> GetTbt_CreditCardForView(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetTbt_CreditCardForView).

		/// \fn public virtual List<tbt_DepositFee> GetTbt_Depositfee(string contractCode, string billingOCC, Nullable<int> depositFeeNo)
		/// \brief (Call stored procedure: sp_BL_GetTbt_Depositfee).

		/// \fn public virtual List<tbt_ExportAutoTransfer> GetTbt_ExportAutoTransfer(Nullable<int> exportAutoTransferID)
		/// \brief (Call stored procedure: sp_BL_GetTbt_ExportAutoTransfer).

		/// \fn public virtual List<tbt_Invoice> GetTbt_Invoice(string invoiceNo, Nullable<int> invoiceOCC)
		/// \brief (Call stored procedure: sp_BL_GetTbt_Invoice).

		/// \fn public virtual List<tbt_TaxInvoice> GetTbt_TaxInvoice(string taxInvoiceNo)
		/// \brief (Call stored procedure: sp_BL_GetTbt_TaxInvoice).

		/// \fn public virtual List<doGetUnpaidInvoiceData> GetUnpaidInvoiceData(string invoiceNo, string invoicePaymentStatus)
		/// \brief (Call stored procedure: sp_BL_GetUnpaidInvoiceData).

		/// \fn public virtual List<dtViewBillingBasic> GetViewBillingBasic(string strContractCode, string strBillingOCC, string strBillingClientCode, string strBillingTargetCode, string strBillingCilentname, string strAddress)
		/// \brief (Call stored procedure: sp_BL_GetViewBillingBasic).

		/// \fn public virtual List<dtViewBillingBasicList> GetViewBillingBasicList(string billingClientCode, string billingTargetCode, string billingClientName, string address, string invoiceNo)
		/// \brief (Call stored procedure: sp_BL_GetViewBillingBasicList).

		/// \fn public virtual List<dtViewBillingDetailList> GetViewBillingDetailList(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetViewBillingDetailList).

		/// \fn public virtual List<dtViewBillingDetailList> GetViewBillingDetailListByTargetCode(string billingTargetCode)
		/// \brief (Call stored procedure: sp_BL_GetViewBillingDetailListByTargetCode).

		/// \fn public virtual List<dtViewBillingDetailListOfLastInvoiceOCC> GetViewBillingDetailListOfLastInvoiceOCC(string invoiceNo, string billingClientCode, string billingTargetCode, string billingCilentname, string address)
		/// \brief (Call stored procedure: sp_BL_GetViewBillingDetailListOfLastInvoiceOcc).

		/// \fn public virtual List<dtViewBillingInvoiceListOfLastInvoiceOcc> GetViewBillingInvoiceListOfLastInvoiceOcc(string billingClientCode, string billingTargetCode, string billingCilentname, string address, string invoiceNo)
		/// \brief (Call stored procedure: sp_BL_GetViewBillingInvoiceListOfLastInvoiceOcc).

		/// \fn public virtual List<dtViewBillingOccList> GetViewBillingOccList(string strContractCode)
		/// \brief (Call stored procedure: sp_BL_GetViewBillingOccList).

		/// \fn public virtual List<dtBillingTargetData> GetViewBillingTargetDataForSearch(string billingCilentCode, string billingOfficeCode, string billingCilentname, string custTypeCode, string companyTypeCode, string regionCode, string businessTypeCode, string address, string phoneNo)
		/// \brief (Call stored procedure: sp_BL_GetViewBillingTargetDataForSearch).

		/// \fn public virtual List<doBillingTargetList> GetViewBillingTargetList(string billingClientCode, string billingTargetCode, string billingClientName, string address, string invoiceNo)
		/// \brief (Call stored procedure: sp_BL_GetViewBillingTargetList).

		/// \fn public virtual List<dtViewDepositDetailInformation> GetViewDepositDetailInformation(string contractCode, string billingOCC)
		/// \brief (Call stored procedure: sp_BL_GetViewDepositDetailInformation).

		/// \fn public virtual List<tbt_AutoTransferBankAccount> InsertTbt_AutoTransferBankAccount(string xml_DocCancelContractMemoDetail)
		/// \brief (Call stored procedure: sp_BL_InsertTbt_AutoTransferBankAccount).

		/// \fn public virtual List<tbt_BillingBasic> InsertTbt_BillingBasicData(string xml_BillingBasic)
		/// \brief (Call stored procedure: sp_BL_InsertTbt_BillingBasic).

		/// \fn public virtual List<tbt_BillingDetail> InsertTbt_BillingDetail(string xml_BillingDetail)
		/// \brief (Call stored procedure: sp_BL_InsertTbt_BillingDetail).

		/// \fn public virtual List<tbt_BillingTarget> InsertTbt_BillingTargetData(string xml_BillingTarget)
		/// \brief (Call stored procedure: sp_BL_InsertTbt_BillingTarget).

		/// \fn public virtual List<tbt_BillingTypeDetail> InsertTbt_BillingTypeDetailData(string xml_BillingTypeDetail)
		/// \brief (Call stored procedure: sp_BL_InsertTbt_BillingTypeDetail).

		/// \fn public virtual List<tbt_CreditCard> InsertTbt_CreditCard(string xml_DocCancelContractMemoDetail)
		/// \brief (Call stored procedure: sp_BL_InsertTbt_CreditCard).

		/// \fn public virtual List<tbt_DepositFee> InsertTbt_Depositfee(string xmlTbt_Depositfee)
		/// \brief (Call stored procedure: sp_BL_InsertTbt_Depositfee).

		/// \fn public virtual List<tbt_ExportAutoTransfer> InsertTbt_ExportAutoTransfer(string bankCode, string bankBranchCode, Nullable<System.DateTime> generateDate, Nullable<System.DateTime> autoTransferDate, string filePath, string fileName, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
		/// \brief (Call stored procedure: sp_BL_InsertTbt_ExportAutoTransfer).

		/// \fn public virtual List<tbt_Invoice> InsertTbt_Invoice(string xmlTbt_Invoice)
		/// \brief (Call stored procedure: sp_BL_InsertTbt_Invoice).

		/// \fn public virtual List<tbt_MonthlyBillingHistory> InsertTbt_MonthlyBillingHistory(string xml_MonthlyBillingHistory)
		/// \brief (Call stored procedure: sp_BL_InsertTbt_MonthlyBillingHistory).

		/// \fn public virtual List<tbt_TaxInvoice> InsertTbt_TaxInvoice(string xmlTbt_TaxInvoice)
		/// \brief (Call stored procedure: sp_BL_InsertTbt_TaxInvoice).

		/// \fn public virtual List<tbt_BillingDetail> RegisterBillingDetailExemption(string invoiceNo, Nullable<int> invoiceOCC, string c_PAYMENT_STATUS_BILLING_EXEMPTION, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_BL_RegisterBillingDetailExemption).

		/// \fn public virtual List<tbt_Invoice> RegisterInvoiceExemption(string invoiceNo, Nullable<int> invoiceOCC, string c_PAYMENT_STATUS_BILLING_EXEMPTION, string exemptApproveNo, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_BL_RegisterInvoiceExemption).

		/// \fn public virtual List<tbt_AutoTransferBankAccount> UpdateAutoTransferAccountLastResult(string invoiceNo, string lastestResult, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_BL_UpdateAutoTransferAccountLastResult).

		/// \fn public virtual List<tbt_BillingBasic> UpdateBalanceDepositOfBillingBasic(string contractCode, string billingOCC, Nullable<decimal> adjustAmount, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_BL_UpdateBalanceDepositOfBillingBasic).

		/// \fn public virtual int UpdateBillingDetailByBatch(string xml_tbt_BillingDetail)
		/// \brief (Call stored procedure: sp_BL_UpdateBillingDetailByBatch).

		/// \fn public virtual List<tbt_Invoice> UpdateInvoiceCorrectionReason(string invoiceNo, Nullable<int> invoiceOCC, string correctReason, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_BL_UpdateInvoiceCorrectionReason).

		/// \fn public virtual List<tbt_BillingBasic> UpdateMonthlyBillingAmount(string xml_doTbtBillingBasic)
		/// \brief (Call stored procedure: sp_BL_UpdateMonthlyBillingAmount).

		/// \fn public virtual List<tbt_TaxInvoice> UpdateReceiptNo(string invoiceNo, Nullable<int> invoiceOCC, string receiptNo, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_BL_UpdateReceiptNo).

		/// \fn public virtual List<tbt_DepositFee> UpdateReceiptNoDepositFee(string invoiceNo, string receiptNo, string updateBy, Nullable<System.DateTime> updateDate, string c_DEPOSIT_STATUS_PAYMENT)
		/// \brief (Call stored procedure: sp_BL_UpdateReceiptNoDepositFee).

		/// \fn public virtual List<tbt_AutoTransferBankAccount> UpdateTbt_AutoTransferBankAccountData(string xml_doTbtAutoTransferBankAccount)
		/// \brief (Call stored procedure: sp_BL_UpdateTbt_AutoTransferBankAccount).

		/// \fn public virtual List<tbt_BillingBasic> UpdateTbt_BillingBasicData(string xml_doTbtBillingBasic)
		/// \brief (Call stored procedure: sp_BL_UpdateTbt_BillingBasic).

		/// \fn public virtual List<tbt_BillingDetail> UpdateTbt_BillingDetailData(string xml_doTbt_BillingDetail)
		/// \brief (Call stored procedure: sp_BL_UpdateTbt_BillingDetail).

		/// \fn public virtual List<tbt_BillingTarget> UpdateTbt_BillingTarget(string xml_doTbtBillingTarget)
		/// \brief (Call stored procedure: sp_BL_UpdateTbt_BillingTarget).

		/// \fn public virtual List<tbt_BillingTypeDetail> UpdateTbt_BillingTypeDetailData(string xml_doTbt_BillingTypeDetail)
		/// \brief (Call stored procedure: sp_BL_UpdateTbt_BillingTypeDetail).

		/// \fn public virtual List<tbt_CreditCard> UpdateTbt_CreditCard(string xml_dotbt_CreditCard)
		/// \brief (Call stored procedure: sp_BL_UpdateTbt_CreditCard).

		/// \fn public virtual List<tbt_DepositFee> UpdateTbt_Depositfee(string xml_doTbt_Depositfee)
		/// \brief (Call stored procedure: sp_BL_UpdateTbt_Depositfee).

		/// \fn public virtual List<tbt_Invoice> UpdateTbt_Invoice(string xml_doTbt_Invoice)
		/// \brief (Call stored procedure: sp_BL_UpdateTbt_Invoice).

		/// \fn public virtual List<tbt_MonthlyBillingHistory> UpdateTbt_MonthlyBillingHistoryData(string xml_doTbtMonthlyBillingHistory)
		/// \brief (Call stored procedure: sp_BL_UpdateTbt_MonthlyBillingHistory).

		/// \fn public virtual List<tbt_TaxInvoice> UpdateTbt_TaxInvoice(string xml_doTbt_TaxInvoice)
		/// \brief (Call stored procedure: sp_BL_UpdateTbt_TaxInvoice).


	}
}

