namespace SECOM_AJIS.DataEntity.Quotation
{
	partial class BizQUDataEntities
	{
		/// \fn public virtual List<tbt_QuotationInstrumentDetails> ConvertQuotationParentToChildInstrument(string pQuotationTargetCode, string pAlphabet)
		/// \brief (Call stored procedure: sp_QU_ConvertQuotationParentToChildInstrument).

		/// \fn public virtual List<Nullable<int>> CountQuotationBasicSQL(string pchvQuotationTargetCode)
		/// \brief (Call stored procedure: sp_QU_CountQuotationBasic).

		/// \fn public virtual List<dtBatchProcessResult> DeleteQuotation(Nullable<bool> pbit_C_FLAG_ON, Nullable<bool> pbit_C_FLAG_OFF)
		/// \brief (Call stored procedure: sp_QU_DeleteQuotation).

		/// \fn public virtual List<doBeatGuardDetail> GetBeatGuardDetail(string pchrQuotationTargetCode, string pchrAlphabet, string pcharC_NUM_OF_DATE)
		/// \brief (Call stored procedure: sp_QU_GetBeatGuardDetail).

		/// \fn public virtual List<doDefaultFacility> GetDefaultFacility(string pchrProductCode, string c_LINE_UP_TYPE_STOP_SALE, string c_LINE_UP_TYPE_LOGICAL_DELETE, string c_INST_TYPE_MONITOR)
		/// \brief (Call stored procedure: sp_QU_GetDefaultFacility).

		/// \fn public virtual List<doDefaultInstrument> GetDefaultInstrument(string pchrProductCode, string pchrProductTypeCode, string c_LINE_UP_TYPE, string c_LINE_UP_TYPE_STOP_SALE, string c_LINE_UP_TYPE_LOGICAL_DELETE, string c_PROD_TYPE_SALE, string c_INST_TYPE_GENERAL, string c_EXPANSION_TYPE_PARENT, Nullable<bool> blnSaleFlag, Nullable<bool> blnRentalFlag)
		/// \brief (Call stored procedure: sp_QU_GetDefaultInstrument).

		/// \fn public virtual List<doFacilityDetail> GetFacilityDetail(string pchrQuotationTargetCode, string pchrAlphabet)
		/// \brief (Call stored procedure: sp_QU_GetFacilityDetail).

		/// \fn public virtual List<doInstrumentDetail> GetInstrumentDetail(string pchr_C_LINE_UP_TYPE, string pchr_C_PROD_TYPE_SALE, string pchrQuotationTargetCode, string pchrAlphabet, string pchrProductTypeCode)
		/// \brief (Call stored procedure: sp_QU_GetInstrumentDetail).

		/// \fn public virtual List<tbt_QuotationBasic> GetQuotationBasicData(string pchvQuotationTargetCode, string pchrAlphabet)
		/// \brief (Call stored procedure: sp_QU_GetQuotationBasicData).

		/// \fn public virtual List<doQuotationCustomer> GetQuotationCustomer(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_QU_GetQuotationCustomer).

		/// \fn public virtual List<doQuotationOperationType> GetQuotationOperationType(string pchrQuotationTargetCode, string pchrAlphabet, string pcharC_OPERATION_TYPE)
		/// \brief (Call stored procedure: sp_QU_GetQuotationOperationType).

		/// \fn public virtual List<doQuotationSite> GetQuotationSite(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_QU_GetQuotationSite).

		/// \fn public virtual List<doQuotationTarget> GetQuotationTarget(string pchr_C_ACQUISITION_TYPE, string pchr_C_MOTIVATION_TYPE, string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_QU_GetQuotationTarget).

		/// \fn public virtual List<doSentryGuardDetail> GetSentryGuardDetail(string pchrQuotationTargetCode, string pchrAlphabet, string pcharC_SG_TYPE)
		/// \brief (Call stored procedure: sp_QU_GetSentryGuardDetail).

		/// \fn public virtual List<tbt_QuotationBeatGuardDetails> GetTbt_QuotationBeatGuardDetails(string quotationTargetCode, string alphabet)
		/// \brief (Call stored procedure: sp_QU_GetTbt_QuotationBeatGuardDetails).

		/// \fn public virtual List<tbt_QuotationCustomer> GetTbt_QuotationCustomer(string quotationTargetCode)
		/// \brief (Call stored procedure: sp_QU_GetTbt_QuotationCustomer).

		/// \fn public virtual List<tbt_QuotationFacilityDetails> GetTbt_QuotationFacilityDetails(string quotationTargetCode, string alphabet)
		/// \brief (Call stored procedure: sp_QU_GetTbt_QuotationFacilityDetails).

		/// \fn public virtual List<tbt_QuotationInstrumentDetails> GetTbt_QuotationInstrumentDetails(string quotationTargetCode, string alphabet)
		/// \brief (Call stored procedure: sp_QU_GetTbt_QuotationInstrumentDetails).

		/// \fn public virtual List<tbt_QuotationMaintenanceLinkage> GetTbt_QuotationMaintenanceLinkage(string quotationTargetCode, string alphabet)
		/// \brief (Call stored procedure: sp_QU_GetTbt_QuotationMaintenanceLinkage).

		/// \fn public virtual List<tbt_QuotationOperationType> GetTbt_QuotationOperationType(string quotationTargetCode, string alphabet)
		/// \brief (Call stored procedure: sp_QU_GetTbt_QuotationOperationType).

		/// \fn public virtual List<tbt_QuotationSentryGuardDetails> GetTbt_QuotationSentryGuardDetails(string quotationTargetCode, string alphabet)
		/// \brief (Call stored procedure: sp_QU_GetTbt_QuotationSentryGuardDetails).

		/// \fn public virtual List<tbt_QuotationSite> GetTbt_QuotationSite(string quotationTargetCode)
		/// \brief (Call stored procedure: sp_QU_GetTbt_QuotationSite).

		/// \fn public virtual List<tbt_QuotationTarget> GetTbt_QuotationTarget(string quotationTargetCode, string serviceTypeCode, string targetCodeTypeCode)
		/// \brief (Call stored procedure: sp_QU_GetTbt_QuotationTarget).

		/// \fn public virtual List<tbt_QuotationBasic> InsertQuotationBasic(string xml_doTbtQuotationBasic)
		/// \brief (Call stored procedure: sp_QU_InsertQuotationBasic).

		/// \fn public virtual List<tbt_QuotationBeatGuardDetails> InsertQuotationBeatGuardDetails(string xml_doTbt_QuotationSite)
		/// \brief (Call stored procedure: sp_QU_InsertQuotationBeatGuardDetails).

		/// \fn public virtual List<tbt_QuotationCustomer> InsertQuotationCustomer(string xml_doTbt_QuotationCustomer)
		/// \brief (Call stored procedure: sp_QU_InsertQuotationCustomer).

		/// \fn public virtual List<tbt_QuotationFacilityDetails> InsertQuotationFacilityDetails(string pchvQuotationTargetCode, string pchrAlphabet, string pchvFacilityCode, Nullable<int> pintFacilityQty, Nullable<System.DateTime> pdtmCreateDate, string pchvCreateBy, Nullable<System.DateTime> pdtmUpdateDate, string pchvUpdateBy)
		/// \brief (Call stored procedure: sp_QU_InsertQuotationFacilityDetails).

		/// \fn public virtual List<tbt_QuotationInstrumentDetails> InsertQuotationInstrumentDetails(string pchvQuotationTargetCode, string pchrAlphabet, string pchvInstrumentCode, Nullable<int> pintInstrumentQty, Nullable<int> pintAddedQty, Nullable<int> pintRemovedQty, Nullable<System.DateTime> pdtmCreateDate, string pchvCreateBy, Nullable<System.DateTime> pdtmUpdateDate, string pchvUpdateBy)
		/// \brief (Call stored procedure: sp_QU_InsertQuotationInstrumentDetails).

		/// \fn public virtual List<tbt_QuotationMaintenanceLinkage> InsertQuotationMaintenanceLinkage(string pchrQuotationTargetCode, string pchrAlphabet, string pchrContractCode, Nullable<System.DateTime> pdtCreateDate, string pchrCreateBy, Nullable<System.DateTime> pdtUpdateDate, string pchrUpdateBy)
		/// \brief (Call stored procedure: sp_QU_InsertQuotationMaintenanceLinkage).

		/// \fn public virtual List<tbt_QuotationOperationType> InsertQuotationOperationType(string pchvQuotationTargetCode, string pchrAlphabet, string pchrOperationTypeCode, Nullable<System.DateTime> pdtmCreateDate, string pchvCreateBy, Nullable<System.DateTime> pdtmUpdateDate, string pchvUpdateBy)
		/// \brief (Call stored procedure: sp_QU_InsertQuotationOperationType).

		/// \fn public virtual List<tbt_QuotationSentryGuardDetails> InsertQuotationSentryGuardDetails(string pchrQuotationTargetCode, string pchrAlphabet, Nullable<int> pintRunningNo, string pchrSentryGuardTypeCode, Nullable<decimal> pdecNumOfDate, Nullable<System.TimeSpan> ptSecurityStartTime, Nullable<System.TimeSpan> ptSecurityFinishTime, Nullable<decimal> pdecWorkHourPerMonth, Nullable<decimal> pdecCostPerHour, Nullable<int> pintNumOfSentryGuard, Nullable<System.DateTime> pdtCreateDate, string pchrCreateBy, Nullable<System.DateTime> pdtUpdateDate, string pdtUpdateBy)
		/// \brief (Call stored procedure: sp_QU_InsertQuotationSentryGuardDetails).

		/// \fn public virtual List<tbt_QuotationSite> InsertQuotationSite(string xml_doTbt_QuotationSite)
		/// \brief (Call stored procedure: sp_QU_InsertQuotationSite).

		/// \fn public virtual List<tbt_QuotationTarget> InsertQuotationTarget(string xml_QuotationTarget)
		/// \brief (Call stored procedure: sp_QU_InsertQuotationTarget).

		/// \fn public virtual List<Nullable<int>> IsUsedCustomer(string pchrCustCode)
		/// \brief (Call stored procedure: sp_QU_IsUsedCustomer).

		/// \fn public virtual List<Nullable<int>> IsUsedSite(string pchrSiteCode)
		/// \brief (Call stored procedure: sp_QU_IsUsedSite).

		/// \fn public virtual List<tbt_QuotationBasic> LockAll(string pchr_C_LOCK_STATUS_LOCK, string pchr_C_LOCK_STATUS_UNLOCK, Nullable<System.DateTime> pdtmProcessDateTime, string pchrEmpNo, string pchvQuotationTargetCode)
		/// \brief (Call stored procedure: sp_QU_LockAll).

		/// \fn public virtual List<tbt_QuotationBasic> LockBackward(string pchr_C_LOCK_STATUS_LOCK, string pchr_C_LOCK_STATUS_UNLOCK, Nullable<System.DateTime> pdtmProcessDateTime, string pchrEmpNo, string pchvQuotationTargetCode, string pchrAlphabet)
		/// \brief (Call stored procedure: sp_QU_LockBackward).

		/// \fn public virtual List<tbt_QuotationBasic> LockIndividual(string pchvnQuotationTargetCode, string pchrAlphabet, string pchrC_LOCK_STATUS_LOCK, string pchr_C_LOCK_STATUS_UNLOCK, Nullable<System.DateTime> pdatProcessDateTime, string pchvEmpno)
		/// \brief (Call stored procedure: sp_QU_LockIndividual).

		/// \fn public virtual List<dtSearchQuotationListResult> SearchQuotationList(string pC_CUST_PART_TYPE_CONTRACT_TARGET, string pQuotationTargetCode, string pAlphabet, string pProductTypeCode, string pLockStatus, string pQuotationOfficeCode, string pOperationOfficeCode, string pContractTargetCode, string pContractTargetName, string pContractTargetAddr, string pSiteCode, string pSiteName, string pSiteAddr, string pEmpNo, string pEmpName, Nullable<System.DateTime> pQuotationDateFrom, Nullable<System.DateTime> pQuotationDateTo, string pServiceTypeCode, string pTargetCodeTypeCode, string pContractTransferStatus)
		/// \brief (Call stored procedure: sp_QU_SearchQuotationList).

		/// \fn public virtual List<dtSearchQuotationTargetListResult> SearchQuotationTargetList(string quotationTargetCode, string productTypeCode, string quotationOfficeCode, string operationOfficeCode, string contractTargetCode, string contractTargetName, string contractTargetAddr, string siteCode, string siteName, string siteAddr, string empNo, string empName, Nullable<System.DateTime> quotationDateFrom, Nullable<System.DateTime> quotationDateTo, string c_CUST_PART_TYPE_CONTRACT_TARGET, string c_TARGET_CODE_TYPE_QTN_CODE, string c_CONTRACT_TRANS_STATUS_CONTRACT_APP, string c_TARGET_CODE_TYPE_CONTRACT_CODE, string xmlOfficeData)
		/// \brief (Call stored procedure: sp_QU_SearchQuotationTargetList).

		/// \fn public virtual List<tbt_QuotationBasic> UpdateQuotationBasic(Nullable<System.DateTime> pdatProcessDateTime, string pchvEmpno, string pchrAlphabet, string pchrContractTransferStatus, string pchvnQuotationTargetCode)
		/// \brief (Call stored procedure: sp_QU_UpdateQuotationBasic).

		/// \fn public virtual List<tbt_QuotationTarget> UpdateQuotationTarget(string pchrQuotationOfficeCode, string pchrLastAlphabet, string pchrContractTransferStatus, string pchrContractCode, Nullable<System.DateTime> pchrTransferDate, string pchrTransferAlphabet, Nullable<System.DateTime> pdtmUpdateDate, string pchvUpdateBy, string pchvQuotationTargetCode)
		/// \brief (Call stored procedure: sp_QU_UpdateQuotationTarget).


	}
}

