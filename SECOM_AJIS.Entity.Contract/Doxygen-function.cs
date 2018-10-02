namespace SECOM_AJIS.DataEntity.Contract
{
	partial class BizCTDataEntities
	{
		/// \fn public virtual List<AutoRenewProcess_Result> AutoRenewProcessBatch()
		/// \brief (Call stored procedure: sp_CT_AutoRenewProcessBatch).

		/// \fn public virtual List<CheckCancelContractBeforeStartService_Result> CheckCancelContractBeforeStartService(string strContractCode, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_CheckCancelContractBeforeStartService).

		/// \fn public virtual List<CheckCanReplaceInstallSlip_Result> CheckCanReplaceInstallSlip(string strContractCode, string c_SALE_CHANGE_TYPE_NEW_SALE, string c_SALE_CHANGE_TYPE_ADD_SALE)
		/// \brief (Call stored procedure: sp_CT_CheckCanReplaceInstallSlip).

		/// \fn public virtual List<CheckCP12_Result> CheckCP12(string strContractCode, string strLastImplementOCC, string c_RENTAL_CHANGE_TYPE_PLAN_CHANGE)
		/// \brief (Call stored procedure: sp_CT_CheckCP12).

		/// \fn public virtual List<tbt_RelationType> CheckRelationType(string pContractCode, string pC_CONTRACT_STATUS_END, string pC_CONTRACT_STATUS_CANCEL, string pC_CONTRACT_STATUS_FIXED_CANCEL, string pQuotationTargetCode, string pRelationType)
		/// \brief (Call stored procedure: sp_CT_CheckRelationType).

		/// \fn public virtual List<tbt_BillingTemp> DeleteAllOneTimeFee(string contractCode, string oCC, string c_BILLING_TYPE_INSTALLATION_FEE, string c_BILLING_TYPE_DEPOSIT_FEE, string pContractCode, string pOCC, string pBillingOCC, string pBillingTargetRunningNo, string pBillingClientCode, string pBillingTargetCode, string pBillingOfficeCode, string pBillingType, string pBillingTiming, Nullable<decimal> pBillingAmt, string pPayMethod, Nullable<int> pBillingCycle, string pCalDailyFeeStatus, string pSendFlag, Nullable<System.DateTime> pProcessDateTime, string pEmpNo)
		/// \brief (Call stored procedure: sp_CT_DeleteAllOneTimeFee).

		/// \fn public virtual List<tbt_BillingTemp> DeleteAllSendData(string pContractCode, string pC_BILLINGTEMP_FLAG_KEEP)
		/// \brief (Call stored procedure: sp_CT_DeleteAllSendData).

		/// \fn public virtual List<tbt_MaintenanceCheckup> DeleteMACheckup(string pContractCode, Nullable<System.DateTime> pMaintenanceDate, Nullable<bool> pDeleteFlag)
		/// \brief (Call stored procedure: sp_CT_DeleteMACheckup).

		/// \fn public virtual List<tbt_MaintenanceCheckupDetails> DeleteMACheckupDetail(string pContractCode, Nullable<System.DateTime> pMaintenanceDate, Nullable<bool> pDeleteFlag)
		/// \brief (Call stored procedure: sp_CT_DeleteMACheckupDetail).

		/// \fn public virtual List<tbt_ARRole> DeleteTbt_ARRole(Nullable<int> pARRoleID)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_ARRole).

		/// \fn public virtual List<tbt_BillingTemp> DeleteTbt_BillingTemp_ByContractCode(string pContractCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_BillingTemp_ByContractCode).

		/// \fn public virtual List<tbt_BillingTemp> DeleteTbt_BillingTemp_ByContractCodeOCC(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_BillingTemp_ByContractCodeOCC).

		/// \fn public virtual List<tbt_BillingTemp> DeleteTbt_BillingTemp_ByContractCodeOCCBillingClientCodeBillingOfficeCode(string contractCode, string oCC, string billingClientCode, string billingOfficeCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_BillingTemp_ByContractCodeOCCBillingClientCodeBillingOfficeCode).

		/// \fn public virtual List<tbt_BillingTemp> DeleteTbt_BillingTemp_ByKey(string pContractCode, string pOCC, Nullable<int> pSequenceNo)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_BillingTemp_ByKey).

		/// \fn public virtual List<tbt_CancelContractMemo> DeleteTbt_CancelContractMemo_ByKey(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_CancelContractMemo_ByKey).

		/// \fn public virtual List<tbt_CancelContractMemoDetail> DeleteTbt_CancelContractMemoDetail_ByContractCodeOCC(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_CancelContractMemoDetail_ByContractCodeOCC).

		/// \fn public virtual List<tbt_ContractEmail> DeleteTbt_ContractEmail(Nullable<int> pContractEmailID)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_ContractEmail).

		/// \fn public virtual List<tbt_ContractEmail> DeleteTbt_ContractEmail_UnsentContractEmail(string paramContractCode, string paramEmailType, Nullable<bool> paramFlag)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_ContractEmail_UnsentContractEmail).

		/// \fn public virtual List<tbt_DraftRentalBillingTarget> DeleteTbt_DraftRentalBillingTarget(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_DraftRentalBillingTarget).

		/// \fn public virtual List<tbt_DraftRentalEmail> DeleteTbt_DraftRentalEmail(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_DraftRentalEmail).

		/// \fn public virtual List<tbt_DraftRentalInstrument> DeleteTbt_DraftRentalInstrument(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_DraftRentalInstrument).

		/// \fn public virtual List<tbt_DraftRentalOperationType> DeleteTbt_DraftRentalOperationType(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_DraftRentalOperationType).

		/// \fn public virtual List<tbt_DraftRentalSentryGuardDetails> DeleteTbt_DraftRentalSentryGuardDetails(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_DraftRentalSentryGuardDetails).

		/// \fn public virtual List<tbt_DraftSaleBillingTarget> DeleteTbt_DraftSaleBillingTarget(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_DraftSaleBillingTarget).

		/// \fn public virtual List<tbt_DraftSaleEmail> DeleteTbt_DraftSaleEmail(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_DraftSaleEmail).

		/// \fn public virtual List<tbt_DraftSaleInstrument> DeleteTbt_DraftSaleInstrument(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_DraftSaleInstrument).

		/// \fn public virtual List<tbt_IncidentRole> DeleteTbt_IncidentRole(Nullable<int> pIncidentRoleID)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_IncidentRole).

		/// \fn public virtual List<tbt_ProjectExpectedInstrumentDetails> DeleteTbt_ProjectExpectedInstrumentDetails(string strProjectCode, string instrumentCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_ProjectExpectedInstrumentDetails).

		/// \fn public virtual List<tbt_ProjectOtherRalatedCompany> DeleteTbt_ProjectOtherRalatedCompany(string strProjectCode, Nullable<int> sequenceNo)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_ProjectOtherRalatedCompany).

		/// \fn public virtual List<tbt_ProjectSupportStaffDetails> DeleteTbt_ProjectSupportStaffDetails(string strProjectCode, string empNo)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_ProjectSupportStaffDetails).

		/// \fn public virtual List<tbt_ProjectSystemDetails> DeleteTbt_ProjectSystemDetails(string strProjectCode, string productCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_ProjectSystemDetails).

		/// \fn public virtual List<tbt_RelationType> DeleteTbt_RelationType(string pchrContractCode)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_RelationType).

		/// \fn public virtual List<tbt_RelationType> DeleteTbt_RelationType_ByContractCodeOCC(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_RelationType_ByContractCodeOCC).

		/// \fn public virtual List<tbt_RentalBEDetails> DeleteTbt_RentalBEDetails_ByKey(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_RentalBEDetails_ByKey).

		/// \fn public virtual List<tbt_RentalInstrumentDetails> DeleteTbt_RentalInstrumentDetails_ByContractCodeOCC(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_RentalInstrumentDetails_ByContractCodeOCC).

		/// \fn public virtual List<tbt_RentalInstSubcontractor> DeleteTbt_RentalInstSubContractor_ByContractCodeOCC(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_RentalInstSubContractor_ByContractCodeOCC).

		/// \fn public virtual List<tbt_RentalMaintenanceDetails> DeleteTbt_RentalMaintenanceDetails_ByKey(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_RentalMaintenanceDetails_ByKey).

		/// \fn public virtual List<tbt_RentalOperationType> DeleteTbt_RentalOperationType_ByKey(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_RentalOperationType_ByKey).

		/// \fn public virtual List<tbt_RentalSecurityBasic> DeleteTbt_RentalSecurityBasic_ByKey(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_RentalSecurityBasic_ByKey).

		/// \fn public virtual List<tbt_RentalSentryGuard> DeleteTbt_RentalSentryGuard_ByKey(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_RentalSentryGuard_ByKey).

		/// \fn public virtual List<tbt_RentalSentryGuardDetails> DeleteTbt_RentalSentryguardDetails_ByContractCodeOCC(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_RentalSentryguardDetails_ByContractCodeOCC).

		/// \fn public virtual List<tbt_SaleInstrumentDetails> DeleteTbt_SaleInstrumentDetails_ByContractCodeOCC(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_DeleteTbt_SaleInstrumentDetails_ByContractCodeOCC).

		/// \fn public virtual int EditDraftRentalContract(string pQuotationTargetCode, string pProductTypeCode, string pGUID, string pScreenID, Nullable<System.DateTime> pCreateDate, string pCreateBy, string pC_PROD_TYPE_ONLINE, string pC_PROD_TYPE_BE, string pC_PROD_TYPE_SG, string pC_PROD_TYPE_MA, string xml_DraftRentalContract, string xml_DraftRentalBillingTarget, string xml_DraftRentalEmail, string xml_DraftRentalOperationType, string xml_RelationType, string xml_DraftRentalInstrument, string xml_DraftRentalBEDetails, string xml_DraftRentalSentryGuard, string xml_DraftRentalSentryGuardDetails, string xml_DraftRentalMaintenanceDetails)
		/// \brief (Call stored procedure: sp_CT_EditDraftRentalContract).

		/// \fn public virtual List<doMaintenanceRelationType> GenerateMaintenanceRelationType(string xml0, Nullable<bool> pBeforeStartFlag)
		/// \brief (Call stored procedure: sp_CT_GenerateMaintenanceRelationType).

		/// \fn public virtual List<dtAR> GetARData(string pRequestNo, string c_AR_RELEVANT_TYPE_CONTRACT, string c_AR_RELEVANT_TYPE_CUSTOMER, string c_AR_RELEVANT_TYPE_SITE, string c_AR_RELEVANT_TYPE_PROJECT, string c_AR_TYPE, string c_AR_STATUS)
		/// \brief (Call stored procedure: sp_CT_GetARData).

		/// \fn public virtual List<dtARDepartmentChief> GetARDepartmentChief(string pRequestNo, Nullable<bool> c_FLAG_ON)
		/// \brief (Call stored procedure: sp_CT_GetARDepartmentChief).

		/// \fn public virtual List<dtARListCTS370> GetARList(string aRRelavantType, string custCode, string siteCode, string contractCode, string quotationTargetCode, string projectCode, string aRType, Nullable<System.DateTime> duedateDeadline, string aRStatus, string c_AR_ROLE_APPROVER, string c_AR_ROLE_REQUESTER, string c_AR_ROLE_AUDITOR, string c_AR_TYPE, string c_AR_STATUS, string c_AR_RELEVANT_TYPE_CUSTOMER, string c_AR_RELEVANT_TYPE_SITE, string c_AR_RELEVANT_TYPE_CONTRACT, string c_AR_RELEVANT_TYPE_PROJECT, string c_AR_RELEVANT_TYPE_QUOTATION, string c_AR_SEARCH_STATUS_COMPLETE, string c_AR_SEARCH_STATUS_HANDLING, string c_AR_STATUS_INSTRUCTED, string c_AR_STATUS_REJECTED, string c_AR_STATUS_APPROVED, string c_DEADLINE_TIME_TYPE, string c_CUST_ROLE_TYPE_CONTRACT_TARGET, Nullable<bool> c_FLAG_ON)
		/// \brief (Call stored procedure: sp_CT_GetARList).

		/// \fn public virtual List<dtARList> GetARListByRole(string aRStatus, string specfyPeriod, Nullable<System.DateTime> specifyPeriodFrom, Nullable<System.DateTime> specifyPeriodTo, string empNo, Nullable<int> aRRole, Nullable<System.DateTime> currentdate, string c_AR_RELEVANT_TYPE_CONTRACT, string c_AR_RELEVANT_TYPE_QUOTATION, string c_AR_RELEVANT_TYPE_SITE, string c_CUST_ROLE_TYPE_REAL_CUST, string c_AR_ROLE_APPROVER, string c_AR_ROLE_AUDITOR, string c_AR_ROLE_REQUESTER, string c_AR_TYPE, string c_AR_STATUS, string c_DEADLINE_TIME_TYPE, string c_AR_SEARCH_STATUS_COMPLETE, string c_AR_SEARCH_STATUS_HANDLING, string c_AR_SEARCH_PERIOD_REQUEST_DATE, string c_AR_SEARCH_PERIOD_APPROVE_DATE, string c_AR_SEARCH_PERIOD_DUEDATE, string c_AR_STATUS_INSTRUCTED, string c_AR_STATUS_REJECTED, string c_AR_STATUS_APPROVED)
		/// \brief (Call stored procedure: sp_CT_GetARListByRole).

		/// \fn public virtual List<dtAROccContract> GetAROccurringContract(string strSiteCode, string c_AR_RELEVANT_TYPE_CONTRACT)
		/// \brief (Call stored procedure: sp_CT_GetAROccurringContract).

		/// \fn public virtual List<dtAROccSite> GetAROccurringSite(string strCustCode, string c_AR_RELEVANT_TYPE_SITE)
		/// \brief (Call stored procedure: sp_CT_GetAROccurringSite).

		/// \fn public virtual List<dtAROfficeChief> GetAROfficeChief(string pRequestNo, Nullable<bool> c_FLAG_ON)
		/// \brief (Call stored procedure: sp_CT_GetAROfficeChief).

		/// \fn public virtual List<dtARRole> GetARRoleData(string pRequestNo, string c_AR_ROLE_TYPE, string c_INCIDENT_ROLE_CHIEF_OF_RELATED_Office)
		/// \brief (Call stored procedure: sp_CT_GetARRoleData).

		/// \fn public virtual List<doBillingTempBasic> GetBillingBasicData(string pContractCode, string pOCC, string pBillingType, string pBillingTiming, string pC_BILLING_TYPE_SALE_PRICE, string pC_BILLINGTEMP_FLAG_KEEP)
		/// \brief (Call stored procedure: sp_CT_GetBillingBasicData).

		/// \fn public virtual List<doBillingTempDetail> GetBillingDetailData(string pContractCode, string pOCC, string pBillingType, string pBillingTiming, string pC_BILLINGTEMP_FLAG_KEEP)
		/// \brief (Call stored procedure: sp_CT_GetBillingDetailData).

		/// \fn public virtual List<doBillingTargetDetail> GetBillingTargetDetailByContractCode(string strContractCode)
		/// \brief (Call stored procedure: sp_CT_GetBillingTargetDetailByContractCode).

		/// \fn public virtual List<tbt_BillingTemp> GetBillingTargetForEditing(string strContractCode, string strOCC, string c_BILLING_TYPE_CONTRACT_FEE, string c_BILLING_TYPE_INSTALLATION_FEE, string c_BILLING_TYPE_MAINTENANCE_FEE, string c_BILLING_TYPE_STOP_FEE, string c_BILLING_TYPE_SALE_PRICE)
		/// \brief (Call stored procedure: sp_CT_GetBillingTempForEditing).

		/// \fn public virtual List<dtBillingTempChangeFeeData> GetBillingTempForChangeFee(string strContractCode, string c_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
		/// \brief (Call stored procedure: sp_CT_GetBillingTempForChangeFee).

		/// \fn public virtual List<dtTbt_BillingTempForSP> GetBillingTempForChangePlan_Edit(string strContractCode, string strOCC, string c_CONTRACT_BILLING_TYPE_CONTRACT_FEE, string c_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_CON, string c_CONTRACT_BILLING_TYPE_MAINTENANCE_FEE_ONE, string c_CONTRACT_BILLING_TYPE_INSTALLATION_FEE, string c_CONTRACT_BILLING_TYPE_DEPOSIT_FEE, string c_BILLING_TIMING_APPROVE_CONTRACT)
		/// \brief (Call stored procedure: sp_CT_GetBillingTempForChangePlan_Edit).

		/// \fn public virtual List<dtTbt_BillingTempForSP> GetBillingTempForChangePlan_New(string strContractCode, string strOCC, string c_CONTRACT_BILLING_TYPE_CONTRACT_FEE)
		/// \brief (Call stored procedure: sp_CT_GetBillingTempForChangePlan_New).

		/// \fn public virtual List<dtChangedCustHistDetail> GetChangedCustHistDetail(string pchvContractCode, Nullable<int> pintSequenceNo, string pchvC_CONTRACT_SIGNER_TYPE, string pchvC_CUST_STATUS, string pchvC_CUST_TYPE, string pchvC_FINANCIAL_MARKET_TYPE, string pchvC_CHANGE_NAME_REASON_TYPE)
		/// \brief (Call stored procedure: sp_CT_GetChangedCustHistDetail).

		/// \fn public virtual List<dtChangedCustHistList> GetChangedCustHistList(string pchvContractCode, string pchrOCC, string pchrCSCustCode, string pchrRCCustCode, string pchrSiteCode, string pchvC_CONTRACT_SIGNER_TYPE, string pchvC_CHANGE_NAME_REASON_TYPE)
		/// \brief (Call stored procedure: sp_CT_GetChangedCustHistList).

		/// \fn public virtual List<dtChangedCustHistList2> GetChangedCustHistList2(string pContractCode, string pOCC, string pCSCustCode, string pRCCustCode, string pSiteCode)
		/// \brief (Call stored procedure: sp_CT_GetChangedCustHistList2).

		/// \fn public virtual List<dtContractBranchName> GetContractBranchName(string pchvLiveSearch)
		/// \brief (Call stored procedure: sp_CT_GetContractBranchName).

		/// \fn public virtual List<Nullable<int>> GetContractCounterNo(string strContractCode, string strLastOCC)
		/// \brief (Call stored procedure: sp_CT_GetContractCounterNo).

		/// \fn public virtual List<dtContractData> GetContractDataForSearch(string pchrCustomerCode, string pchvnCustomerName, string pchvBranchName, string pchvnAddress, string pchvnAlley, string pchvnRoad, string pchvnSubDistrict, string pchrProvinceCode, string pchrDistrictCode, string pchrZipCode, string pchvSALE_CHANGE_TYPE_NEW_SALE)
		/// \brief (Call stored procedure: sp_CT_GetContractDataForSearch).

		/// \fn public virtual List<doProjectContractDetail> GetContractDetailList(string pProjectCode, string pC_DOC_AUDIT_RESULT)
		/// \brief (Call stored procedure: sp_CT_GetContractDetailList).

		/// \fn public virtual List<dtContractDoc> GetContractDocDataList(string strContractCode, string strQuotationTargetCode, string strOccAlphabet, string strOfficeCode, Nullable<int> cModuleIdContract, string cARStatus, Nullable<bool> cFlagOn)
		/// \brief (Call stored procedure: sp_CT_GetContractDocDataList).

		/// \fn public virtual List<dtContractDocument> GetContractDocDataListForView(string pContractCode, string pOCC, string pContractOfficeCode_List, string pOperationOfficeCode_List)
		/// \brief (Call stored procedure: sp_CT_GetContractDocDataListForView).

		/// \fn public virtual List<dtContractDocHeader> GetContractDocHeader(string pContractCode, string pQuotationTargetCode, string pOCC_Alphabet, string pContractDocOCC)
		/// \brief (Call stored procedure: sp_CT_GetContractDocHeader).

		/// \fn public virtual List<tbt_ContractDocument> GetContractDocHeaderByContractCode(string pContractCode, string pOCC, string pchrContractDocOCC)
		/// \brief (Call stored procedure: sp_CT_GetContractDocHeaderByContractCode).

		/// \fn public virtual List<tbt_ContractDocument> GetContractDocHeaderByQuotationCode(string pchrQuotationTargetCode, string pchrAlphabet, string pchrContractDocOCC)
		/// \brief (Call stored procedure: sp_CT_GetContractDocHeaderByQuotationCode).

		/// \fn public virtual List<string> GetContractDocOCC(string strCode, string strOCC)
		/// \brief (Call stored procedure: sp_CT_GetContractDocOCC).

		/// \fn public virtual List<tbt_ContractEmail> GetContractEmailByContractCodeOCC(string contractCode, string oCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ContractEmailByContractCodeOCC).

		/// \fn public virtual List<doContractAutoRenew> GetContractExpireNextMonth(string pC_CONTRACT_STATUS_AFTER_START, Nullable<System.DateTime> batchDate)
		/// \brief (Call stored procedure: sp_CT_GetContractExpireNextMonth).

		/// \fn public virtual List<doContractHeader> GetContractHeaderData(string xml0)
		/// \brief (Call stored procedure: sp_CT_GetContractHeaderData).

		/// \fn public virtual List<dtContractList> GetContractListForSearchInfo(string pchrRoleTypeContractTarget, string pchrRoleTypePurchaser, string pchrRoleTypeRealCustomer, string pchrServiceTypeCode, string pchrCustomerCode, string pchrGroupCode, string pchrSiteCode, string pchvContractCode, string pchvUserCode, string pchvPlanCode, string pchvProjectCode, string pchrnCustomerName, string pchrnBranchName, string pchrnGroupName, string pchrCustomerStatus, string pchrCustomerTypeCode, string pchrCompanyTypeCode, string pchrnIDNo, string pchrRegionCode, string pchrBusinessTypeCode, string pchrnCust_Address, string pchrnCust_Alley, string pchrnCust_Road, string pchrnCust_SubDistrict, string pchrCust_ProvinceCode, string pchrCust_DistrictCode, string pchrCust_ZipCode, string pchrnCust_PhoneNo, string pchrnCust_FaxNo, string pchrnSiteName, string pchrnSite_Address, string pchrnSite_Alley, string pchrnSite_Road, string pchrnSite_SubDistrict, string pchrSite_ProvinceCode, string pchrSite_DistrictCode, string pchrSite_ZipCode, string pchrnSite_PhoneNo, Nullable<System.DateTime> pdtmOperationDate_From, Nullable<System.DateTime> pdtmOperationDate_To, Nullable<System.DateTime> pdtmCustAcceptDate_From, Nullable<System.DateTime> pdtmCustAcceptDate_To, Nullable<System.DateTime> pdtmInstallationCompleteDate_From, Nullable<System.DateTime> pdtmInstallationCompleteDate_To, string pchvContractOfficeCode, string pchvdsTransDataOfficeCode, string pchvOperationOfficeCode, string pchvSalesmanEmpNo1, string pchvSalesmanEmpName1, string pchrProductCode, string pchrChangeTypeCode, string pchrProcessManageStatusCode, string pchrStartTypeCode, string pchvC_RENTAL_CHANGE_TYPE, string pchvC_SALE_CHANGE_TYPE, string pchvC_SALE_PROC_MANAGE_STATUS, string pchrC_CUST_ROLE_TYPE_CONTRACT_TARGET, string pchrC_CUST_ROLE_TYPE_REAL_CUST, string pchrC_CUST_ROLE_TYPE_PURCHASER, Nullable<bool> pbitC_FLAG_ON, string pchrC_SERVICE_TYPE_RENTAL, string pchrC_SERVICE_TYPE_SALE, string pchrC_CONTRACT_STATUS_BEF_START, string pchrC_CONTRACT_STATUS_CANCEL, string pchrC_CONTRACT_STATUS_END, string c_SALE_CHANGE_TYPE_NEW_SALE, string c_CUST_TYPE_JURISTIC)
		/// \brief (Call stored procedure: sp_CT_GetContractListForSearchInfo).

		/// \fn public virtual List<dtContractListGrp> GetContractListForViewCustGrp(string strGroupCode, string strCustRoleType, string strProductTypeCode)
		/// \brief (Call stored procedure: sp_CT_GetContractListForViewCustGrp).

		/// \fn public virtual List<dtContractListGrp> GetContractListForViewCustGrp_CT_Rental(string strGroupCode, string strCONTRACT_PREFIX, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetContractListForViewCustGrp_CT_Rental).

		/// \fn public virtual List<dtContractListGrp> GetContractListForViewCustGrp_CT_Sale(string strGroupCode, string strCONTRACT_PREFIX, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetContractListForViewCustGrp_CT_Sale).

		/// \fn public virtual List<dtContractListGrp> GetContractListForViewCustGrp_R_Rental(string strGroupCode, string strCONTRACT_PREFIX, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetContractListForViewCustGrp_R_Rental).

		/// \fn public virtual List<dtContractListGrp> GetContractListForViewCustGrp_R_Sale(string strGroupCode, string strCONTRACT_PREFIX, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetContractListForViewCustGrp_R_Sale).

		/// \fn public virtual List<dtContractSignerType> GetContractSignerType(string c_CONTRACT_SIGNER_TYPE, string contractCode, string oCC)
		/// \brief (Call stored procedure: sp_CT_GetContractSignerType).

		/// \fn public virtual List<dtContractsSameSite> GetContractsListForViewSite(string pSiteCode, string pC_RENTAL_CHANGE_TYPE, string pC_SALE_CHANGE_TYPE, string pC_SERVICE_TYPE_RENTAL, string pC_SALE_CHANGE_TYPE_NEW_SALE)
		/// \brief (Call stored procedure: sp_CT_GetContractsListForViewSite).

		/// \fn public virtual List<dtContractsSameSite> GetContractsSameSiteList(string pSiteCode, string pC_RENTAL_CHANGE_TYPE, string pC_SALE_CHANGE_TYPE, string pC_SERVICE_TYPE_RENTAL, string pContractCode)
		/// \brief (Call stored procedure: sp_CT_GetContractsSameSiteList).

		/// \fn public virtual List<dtContractTargetInfoByRelated> GetContractTargetInfoByRelated(string pRelatedContractCode, string pRelationType, string pRelatedOCC, string pC_PROD_TYPE_SALE, string pC_PROD_TYPE_AL, string pC_PROD_TYPE_ONLINE, string pC_PROD_TYPE_RENTAL_SALE, string pRelatedProductTypeCode, string pC_RELATION_TYPE_MA, string pC_RELATION_TYPE_SALE)
		/// \brief (Call stored procedure: sp_CT_GetContractTargetInfoByRelated).

		/// \fn public virtual List<dtCustomerList> GetCustomerListForSearchInfo(string pchrCustomerCode, string pchrRoleTypeContractTarget, string pchrRoleTypePurchaser, string pchrRoleTypeRealCustomer, string pchrGroupCode, string pchrnCustomerName, string pchrnGroupName, string pchrCustomerStatus, string pchrCustomerTypeCode, string pchrCompanyTypeCode, string pchrnIDNo, string pchrRegionCode, string pchrBusinessTypeCode, string pchrnCust_Address, string pchrnCust_Alley, string pchrnCust_Road, string pchrnCust_SubDistrict, string pchrCust_ProvinceCode, string pchrCust_DistrictCode, string pchrCust_ZipCode, string pchrnCust_PhoneNo, string pchrnCust_FaxNo, string pchrC_CUST_ROLE_TYPE_CONTRACT_TARGET, string pchrC_CUST_ROLE_TYPE_REAL_CUST, string pchrC_CUST_ROLE_TYPE_PURCHASER, Nullable<bool> pbitC_FLAG_ON, string pchrC_SERVICE_TYPE_RENTAL, string pchrC_SERVICE_TYPE_SALE, string pchrC_CONTRACT_STATUS_BEF_START, string c_CUST_TYPE_JURISTIC)
		/// \brief (Call stored procedure: sp_CT_GetCustomerListForSearchInfo).

		/// \fn public virtual List<dtCustomerListGrp> GetCustomerListForViewCustGrp(string strGroupCode, string strCustRoleType, string strProductTypeCode)
		/// \brief (Call stored procedure: sp_CT_GetCustomerListForViewCustGrp).

		/// \fn public virtual List<dtCustomerListGrp> GetCustomerListForViewCustGrp_CT_Rental(string strGroupCode, string strCONTRACT_PREFIX, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetCustomerListForViewCustGrp_CT_Rental).

		/// \fn public virtual List<dtCustomerListGrp> GetCustomerListForViewCustGrp_CT_Sale(string strGroupCode, string strCONTRACT_PREFIX, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetCustomerListForViewCustGrp_CT_Sale).

		/// \fn public virtual List<dtCustomerListGrp> GetCustomerListForViewCustGrp_R_Rental(string strGroupCode, string strCONTRACT_PREFIX, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetCustomerListForViewCustGrp_R_Rental).

		/// \fn public virtual List<dtCustomerListGrp> GetCustomerListForViewCustGrp_R_Sale(string strGroupCode, string strCONTRACT_PREFIX, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetCustomerListForViewCustGrp_R_Sale).

		/// \fn public virtual List<doDraftRentalContractInformation> GetDraftRentalContractInformation(string strQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetDraftRentalContractInformation).

		/// \fn public virtual List<dtGroupList> GetGroupListForSearchCustGrp(string pchvGroupCode, string pchrnGroupName, string pchvOfficeCode, string pchvEmpNo, Nullable<int> pintNumOfCustFrom, Nullable<int> pintNumOfCustTo, Nullable<int> pintNumOfSiteFrom, Nullable<int> pintNumOfSiteTo, string c_CONTRACT_STATUS_BEF_START, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_AFTER_START, Nullable<bool> c_FLAG_ON)
		/// \brief (Call stored procedure: sp_CT_GetGroupListForSearchCustGrp).

		/// \fn public virtual List<dtGroupSummary> GetGroupSummaryForViewCustGrp(string strGroupCode, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetGroupSummaryForViewCustGrp).

		/// \fn public virtual List<dtIncident> GetIncidentData(Nullable<int> incidentID, string c_INCIDENT_RELEVANT_TYPE_CONTRACT, string c_INCIDENT_RELEVANT_TYPE_CUSTOMER, string c_INCIDENT_RELEVANT_TYPE_SITE, string c_INCIDENT_RELEVANT_TYPE_PROJECT, string c_INCIDENT_TYPE, string c_REASON_TYPE)
		/// \brief (Call stored procedure: sp_CT_GetIncidentData).

		/// \fn public virtual List<string> GetIncidentDepartmentChief(Nullable<int> incidentID, Nullable<bool> c_FLAG_ON)
		/// \brief (Call stored procedure: sp_CT_GetIncidentDepartmentChief).

		/// \fn public virtual List<dtIncidentHistory> GetIncidentHistoryData(Nullable<int> incidentID, string c_INCIDENT_INTERACTION_TYPE)
		/// \brief (Call stored procedure: sp_CT_GetIncidentHistoryData).

		/// \fn public virtual List<dtIncidentListCTS320> GetIncidentList(string incidentRelevantType, string customerCode, string siteCode, string contractCode, string projectCode, string incidentType, Nullable<System.DateTime> duedateDeadline, string incidentStatus, string c_INCIDENT_ROLE_CONTROL_CHIEF, string c_INCIDENT_ROLE_CHIEF, string c_INCIDENT_ROLE_CORRESPONDENT, string c_INCIDENT_ROLE_ASSISTANT, string c_INCIDENT_TYPE, string c_INCIDENT_RELEVANT_TYPE_CUSTOMER, string c_INCIDENT_RELEVANT_TYPE_SITE, string c_INCIDENT_STATUS, string c_INCIDENT_STATUS_COMPLETE, string c_INCIDENT_SEARCH_STATUS_COMPLETE, string c_INCIDENT_SEARCH_STATUS_HANDLING, string c_INCIDENT_RELEVANT_TYPE_CONTRACT, string c_DEADLINE_TIME_TYPE, string c_FLAG_ON, Nullable<bool> isSearchByCustomer, Nullable<bool> isSearchBySite, Nullable<bool> isSearchByContract, Nullable<bool> isSearchByProject)
		/// \brief (Call stored procedure: sp_CT_GetIncidentList).

		/// \fn public virtual List<dtIncidentList> GetIncidentListByRole(string incidentRole, string empNo, Nullable<System.DateTime> dueDate, string incidentStatus, string c_INCIDENT_SEARCH_STATUS_COMPLETE, string c_INCIDENT_SEARCH_STATUS_HANDLING, string c_INCIDENT_ROLE_CONTROL_CHIEF, string c_INCIDENT_ROLE_CHIEF, string c_INCIDENT_ROLE_CORRESPONDENT, string c_INCIDENT_ROLE_ASSISTANT, string c_INCIDENT_RELEVANT_TYPE_CONTRACT, string c_INCIDENT_RELEVANT_TYPE_SITE, string c_INCIDENT_STATUS, string c_INCIDENT_STATUS_COMPLETE, string c_INCIDENT_TYPE, string c_DEADLINE_TIME_TYPE)
		/// \brief (Call stored procedure: sp_CT_GetIncidentListByRole).

		/// \fn public virtual List<dtIncidentOccContract> GetIncidentOccurringContract(string strSiteCode, string c_INCIDENT_RELEVANT_TYPE_CONTRACT)
		/// \brief (Call stored procedure: sp_CT_GetIncidentOccurringContract).

		/// \fn public virtual List<dtIncidentOccSite> GetIncidentOccurringSite(string strCustCode, string c_INCIDENT_RELEVANT_TYPE_SITE)
		/// \brief (Call stored procedure: sp_CT_GetIncidentOccurringSite).

		/// \fn public virtual List<string> GetIncidentOfficeChief(Nullable<int> incidentID, Nullable<bool> c_FLAG_ON)
		/// \brief (Call stored procedure: sp_CT_GetIncidentOfficeChief).

		/// \fn public virtual List<dtIncidentRole> GetIncidentRoleData(Nullable<int> incidentID, string c_INCIDENT_ROLE_TYPE, string c_INCIDENT_ROLE_CHIEF_OF_RELATED_OFFICE)
		/// \brief (Call stored procedure: sp_CT_GetIncidentRoleData).

		/// \fn public virtual List<RPTInstrumentCheckupDo> GetInstrument(string contractCode, string occ)
		/// \brief (Call stored procedure: sp_CT_GetInstrumentList).

		/// \fn public virtual List<dtInstrumentAdditionalInstalled> GetInstrumentAdditionalInstalled(string strContractCode)
		/// \brief (Call stored procedure: sp_CT_GetInstrumentAdditionalInstalled).

		/// \fn public virtual List<dtInstrumentInstalledBefore> GetInstrumentInstalledBefore(string strContractCode)
		/// \brief (Call stored procedure: sp_CT_GetInstrumentInstalledBefore).

		/// \fn public virtual List<tbt_CancelContractMemo> GetLastCancelContractMemo(string strContractCode, Nullable<bool> isQuotation)
		/// \brief (Call stored procedure: sp_CT_GetLastCancelContractMemo).

		/// \fn public virtual List<Nullable<int>> GetLastContractCounterNo(string strContractCode)
		/// \brief (Call stored procedure: sp_CT_GetLastContractCounterNo).

		/// \fn public virtual List<string> GetLastImplementedOCCs(string strContractCode)
		/// \brief (Call stored procedure: sp_CT_GetLastImplementedOCC).

		/// \fn public virtual List<string> GetLastMACheckupNo()
		/// \brief (Call stored procedure: sp_CT_GetLastMACheckupNo).

		/// \fn public virtual List<Nullable<System.DateTime>> GetLastMaintenanceDate(string contractCode, string productCode)
		/// \brief (Call stored procedure: sp_CT_GetLastMaintenanceDate).

		/// \fn public virtual List<string> GetLastOCCs(string strContractCode)
		/// \brief (Call stored procedure: sp_CT_GetLastOCC).

		/// \fn public virtual List<string> GetLastUnimplementedOCCs(string strContractCode)
		/// \brief (Call stored procedure: sp_CT_GetLastUnimplementedOCC).

		/// \fn public virtual List<string> GetMAContractCodeOf(string strMATargetContractCode, string strRelationType)
		/// \brief (Call stored procedure: sp_CT_GetMAContractCodeOf).

		/// \fn public virtual List<doCreateMASchedule> GetMAforCreateScheduleByMA(string pContractCode, string pC_RELATION_TYPE_MA, Nullable<bool> pLatestOCCFlag)
		/// \brief (Call stored procedure: sp_CT_GetMAforCreateScheduleByMA).

		/// \fn public virtual List<doCreateMAScheduleDetail> GetMAforCreateScheduleDetailByMA(string pContractCode, string pC_RELATION_TYPE_MA, Nullable<bool> pLatestOCCFlag)
		/// \brief (Call stored procedure: sp_CT_GetMAforCreateScheduleDetailByMA).

		/// \fn public virtual List<dtMaintCheckUpResultList> GetMaintCheckUpResultList(string pContractCode, string pMATargetContractCode, string pProductCode)
		/// \brief (Call stored procedure: sp_CT_GetMaintCheckUpResultList).

		/// \fn public virtual List<dtMaintContractTargetInfoByRelated> GetMaintContractTargetInfoByRelated(string pRelatedContractCode, string pC_MA_TARGET_PROD_TYPE, string pC_MA_TYPE, string pC_MA_FEE_TYPE, string pC_RELATION_TYPE_MA, string pRelatedOCC, string pC_PROD_TYPE_SALE, string pC_PROD_TYPE_AL, string pC_PROD_TYPE_ONLINE, string pC_PROD_TYPE_RENTAL_SALE, string pRelatedProductTypeCode)
		/// \brief (Call stored procedure: sp_CT_GetMaintContractTargetInfoByRelated).

		/// \fn public virtual List<doMaintenanceCheckupInformation> GetMaintenanceCheckupInformation(string pContractCode, string pProductCode, Nullable<System.DateTime> pInstructionDate)
		/// \brief (Call stored procedure: sp_CT_GetMaintenanceCheckupInformation).

		/// \fn public virtual List<dtGetMaintenanceCheckupList> GetMaintenanceCheckupList(string xml_MaintenanceCheckup)
		/// \brief (Call stored procedure: sp_CT_GetMaintenanceCheckupList).

		/// \fn public virtual List<RPTMACheckupSlipDo> GetMaintenanceCheckupSlipReport(string paramContractCode, string paramProductCode, Nullable<System.DateTime> paramInstructionDate, string paramContractStatusAfterStart)
		/// \brief (Call stored procedure: sp_CT_GetMaintenanceCheckupSlipReport).

		/// \fn public virtual List<dtGetMaintenanceTargetContract> GetMaintenanceTargetContract(string pContractCode, Nullable<bool> pLastestOCCFlag)
		/// \brief (Call stored procedure: sp_CT_GetMaintenanceTargetContract).

		/// \fn public virtual List<Nullable<System.DateTime>> GetMaxUpdateDateOfMATargetContract(string paramMAContractCode, string paramOCC, string c_RENTAL_CHANGE_TYPE_CHANGE_NAME, string c_RENTAL_CHANGE_TYPE_MOVE_INSTRU)
		/// \brief (Call stored procedure: sp_CT_GetMaxUpdateDateOfMATargetContract).

		/// \fn public virtual List<string> GetNextImplementedOCC(string paramContractCode, string paramOCC, Nullable<bool> paramFLAGON)
		/// \brief (Call stored procedure: sp_CT_GetNextImplementedOCC).

		/// \fn public virtual List<string> GetPreviousImplementedOCC(string paramContractCode, string paramOCC, Nullable<bool> paramFLAGON)
		/// \brief (Call stored procedure: sp_CT_GetPreviousImplementedOCC).

		/// \fn public virtual List<string> GetPreviousOCC(string strContractCode, string strCurrentOCC)
		/// \brief (Call stored procedure: sp_CT_GetPreviousOCC).

		/// \fn public virtual List<string> GetPreviousUnimplementedOCC(string paramContractCode, string paramOCC, Nullable<bool> paramFLAGOFF)
		/// \brief (Call stored procedure: sp_CT_GetPreviousUnimplementedOCC).

		/// \fn public virtual List<dtProjectData> GetProjectDataForSearch(string pchrProjectCode, string pchvContractCode, string pchnvProductCode, string pchnvProjectName, string pchnvProjectAddress, string pchnvPJPurchaseName, string pchnvOwner1Name, string pchnvCompanyName, string pchnvOtherProjectRelatedPersonName, string pchnvHeadSalesmanEmpName, string pchnvProjectManagerEmpName)
		/// \brief (Call stored procedure: sp_CT_GetProjectDataForSearch).

		/// \fn public virtual List<dtProjectForInstall> GetProjectForInstall(string strProjectCode)
		/// \brief (Call stored procedure: sp_CT_GetProjectForInstall).

		/// \fn public virtual List<string> GetProjectStatus(string strProjectCode)
		/// \brief (Call stored procedure: sp_CT_GetProjectStatus).

		/// \fn public virtual List<doProjectBranch> GetProjectStockOutBranch(string projectCode)
		/// \brief (Call stored procedure: sp_CT_GetProjectStockOutBranch).

		/// \fn public virtual List<dtRelatedContract> GetRelatedContractList(string pchrRelationType, string pchvstrContractCode, string pchrOCC)
		/// \brief (Call stored procedure: sp_CT_GetRelatedContractList).

		/// \fn public virtual List<dtRelatedOfficeChief> GetReleatedContractOfCustAR(string requestNo, Nullable<bool> c_FLAG_ON)
		/// \brief (Call stored procedure: sp_CT_GetReleatedContractOfCustAR).

		/// \fn public virtual List<dtContractOfAllSite> GetReleatedContractOfCustIncident(string strSiteCodeList, Nullable<bool> c_FLAG_ON)
		/// \brief (Call stored procedure: sp_CT_GetReleatedContractOfCustIncident).

		/// \fn public virtual List<dtRentalContractBasicForInstall> GetRentalContractBasicForInstall(string strContractCode, string buildingType)
		/// \brief (Call stored procedure: sp_CT_GetRentalContractBasicForInstall).

		/// \fn public virtual List<dtRentalContractBasicForView> GetRentalContractBasicForView(string strContractCode, string strUserCode, string c_RENTAL_CHANGE_TYPE)
		/// \brief (Call stored procedure: sp_CT_GetRentalContractBasicForView).

		/// \fn public virtual List<doRentalContractBasicInformation> GetRentalContractBasicInformation(string strContractCode)
		/// \brief (Call stored procedure: sp_CT_GetRentalContractBasicInformation).

		/// \fn public virtual List<doRentalContractDataForFlowMenu> GetRentalContractDataForFlowMenu(string contractCode)
		/// \brief (Call stored procedure: sp_CT_GetRentalContractDataForFlowMenu).

		/// \fn public virtual List<dtRentalHistoryDigest> GetRentalHistoryDigestList(string pchvContractCode, string xml_SelChangeType, string xml_selIncidentARtype, string pC_RENTAL_CHANGE_TYPE, string pC_INCIDENT_TYPE, string pC_AR_TYPE, string pC_DOC_AUDIT_RESULT)
		/// \brief (Call stored procedure: sp_CT_GetRentalHistoryDigestList).

		/// \fn public virtual List<doRentalSecurityBasicInformation> GetRentalSecurityBasicInformation(string strContractCode, string occ)
		/// \brief (Call stored procedure: sp_CT_GetRentalSecurityBasicInformation).

		/// \fn public virtual List<RPTCancelContractMemoDo> GetRptCancelContractMemoData(Nullable<int> iDocID, Nullable<bool> bFlagOn)
		/// \brief (Call stored procedure: sp_CT_GetRptCancelContractMemoData).

		/// \fn public virtual List<RPTCancelContractMemoDetailDo> GetRptCancelContractMemoDetailData(Nullable<int> iDocID)
		/// \brief (Call stored procedure: sp_CT_GetRptCancelContractMemoDetailData).

		/// \fn public virtual List<RPTChangeFeeMemoDo> GetRptChangeFeeMemoData(Nullable<int> iDocID, Nullable<bool> bFlagOn)
		/// \brief (Call stored procedure: sp_CT_GetRptChangeFeeMemoData).

		/// \fn public virtual List<RPTChangeMemoDo> GetRptChangeMemoData(Nullable<int> iDocID, Nullable<bool> bFlagOn)
		/// \brief (Call stored procedure: sp_CT_GetRptChangeMemoData).

		/// \fn public virtual List<RPTChangeNoticeDo> GetRptChangeNoticeData(Nullable<int> iDocID, Nullable<bool> bFlagOn)
		/// \brief (Call stored procedure: sp_CT_GetRptChangeNoticeData).

		/// \fn public virtual List<RPTConfirmCurrInstMemoDo> GetRptConfirmCurrentInstrumentMemoData(Nullable<int> iDocID, Nullable<bool> bFlagOn)
		/// \brief (Call stored procedure: sp_CT_GetRptConfirmCurrentInstrumentMemoData).

		/// \fn public virtual List<RPTContractReportDo> GetRptContractReportData(Nullable<int> iDocID, Nullable<bool> bFlagOn, string cPaymentMethod, string cBillingTiming)
		/// \brief (Call stored procedure: sp_CT_GetRptContractReportData).

		/// \fn public virtual List<RPTCoverLetterDo> GetRptCoverLetterData(Nullable<int> iDocID, string cPaymentMethod)
		/// \brief (Call stored procedure: sp_CT_GetRptCoverLetterData).

		/// \fn public virtual List<RPTInstrumentDetailDo> GetRptInstrumentDetailData(Nullable<int> iDocID)
		/// \brief (Call stored procedure: sp_CT_GetRptInstrumentDetailData).

		/// \fn public virtual List<dtSaleBasic> GetSaleBasicForInstall(string strContractCode, string buildingType)
		/// \brief (Call stored procedure: sp_CT_GetSaleBasicForInstall).

		/// \fn public virtual List<dtSaleContractBasicForView> GetSaleContractBasicForView(string strContractCode, string c_SALE_PROC_MANAGE_STATUS, string c_SALE_TYPE, string c_SALE_CHANGE_TYPE, Nullable<bool> fLAG_ON)
		/// \brief (Call stored procedure: sp_CT_GetSaleContractBasicForView).

		/// \fn public virtual List<doSaleContractBasicInformation> GetSaleContractBasicInformation(string strContractCode, Nullable<bool> isLastOCCFlag, string strOCC)
		/// \brief (Call stored procedure: sp_CT_GetSaleContractBasicInformation).

		/// \fn public virtual List<doSaleContractDataForFlowMenu> GetSaleContractDataForFlowMenu(string contractCode)
		/// \brief (Call stored procedure: sp_CT_GetSaleContractDataForFlowMenu).

		/// \fn public virtual List<doGetSaleDataForIssueInvoice> GetSaleDataForIssueInvoice(string contractCode, string oCC, string c_BILLING_TIMING_ACCEPTANCE, string c_BILLINGTEMP_FLAG_KEEP)
		/// \brief (Call stored procedure: sp_CT_GetSaleDataForIssueInvoice).

		/// \fn public virtual List<dtSaleHistoryDigest> GetSaleHistoryDigestList(string pchvContractCode, string xml_SelChangeType, string xml_selIncidentARtype, string pC_SALE_CHANGE_TYPE, string pC_INCIDENT_TYPE, string pC_AR_TYPE)
		/// \brief (Call stored procedure: sp_CT_GetSaleHistoryDigestList).

		/// \fn public virtual List<dtSaleInstruDetailListForView> GetSaleInstruDetailListForView(string pContractCode, string pOCC, string pInstrumentTypeCode)
		/// \brief (Call stored procedure: sp_CT_GetSaleInstruDetailListForView).

		/// \fn public virtual List<dsSaleInstrumentDetails> GetSaleInstrumentDetails(string pchrContractCode, string pchrOCC, string c_INST_TYPE_GENERAL, string c_SALE_CHANGE_TYPE_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetSaleInstrumentDetails).

		/// \fn public virtual List<string> GetSaleLastOCC(string pContractCode)
		/// \brief (Call stored procedure: sp_CT_GetSaleLastOCC).

		/// \fn public virtual List<doServiceProductTypeCode> GetServiceProductTypeCode(string strCode, string c_SERVICE_TYPE_PROJECT)
		/// \brief (Call stored procedure: sp_CT_GetServiceProductTypeCode).

		/// \fn public virtual List<dsGetSiteContractList> GetSiteContractList(string pContractCode, Nullable<bool> pLastestOCCFlag)
		/// \brief (Call stored procedure: sp_CT_GetSiteContractList).

		/// \fn public virtual List<dtSiteList> GetSiteListForCustInfo(string pchvCustomerCode, string pchrCustomerRole, string pchrC_CUST_ROLE_TYPE_CONTRACT_TARGET, string pchrC_CUST_ROLE_TYPE_REAL_CUST, string pchrC_CUST_ROLE_TYPE_PURCHASER, Nullable<bool> pbitC_FLAG_ON, string pchrC_SERVICE_TYPE_RENTAL, string pchrC_RENTAL_CHANGE_TYPE_END_CONTRACT, string pchrC_RENTAL_CHANGE_TYPE_CANCEL, string pchrC_SERVICE_TYPE_SALE, string pchrC_CONTRACT_STATUS_CANCEL, string pchrC_CONTRACT_STATUS_END, string pchrC_CONTRACT_STATUS_BEF_START, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetSiteListForCustInfo).

		/// \fn public virtual List<dtsiteListGrp> GetSiteListForViewCustGrp(string strGroupCode, string strCustRoleType, string strProductTypeCode)
		/// \brief (Call stored procedure: sp_CT_GetSiteListForViewCustGrp).

		/// \fn public virtual List<dtsiteListGrp> GetSiteListForViewCustGrp_CT_Rental(string strGroupCode, string strCONTRACT_PREFIX, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetSiteListForViewCustGrp_CT_Rental).

		/// \fn public virtual List<dtsiteListGrp> GetSiteListForViewCustGrp_CT_Sale(string strGroupCode, string strCONTRACT_PREFIX, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetSiteListForViewCustGrp_CT_Sale).

		/// \fn public virtual List<dtsiteListGrp> GetSiteListForViewCustGrp_R_Rental(string strGroupCode, string strCONTRACT_PREFIX, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetSiteListForViewCustGrp_R_Rental).

		/// \fn public virtual List<dtsiteListGrp> GetSiteListForViewCustGrp_R_Sale(string strGroupCode, string strCONTRACT_PREFIX, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_FIXED_CANCEL)
		/// \brief (Call stored procedure: sp_CT_GetSiteListForViewCustGrp_R_Sale).

		/// \fn public virtual List<tbm_SubContractor> GetTbm_SubContractor(string pSubContractorCode)
		/// \brief (Call stored procedure: sp_CT_GetTbm_SubContractor).

		/// \fn public virtual List<dtTbs_ARApproveNoRunningNo> GetTbs_ARApproveNoRunningNo(string pYear, string pPrefix)
		/// \brief (Call stored procedure: sp_CT_GetTbs_ARApproveNoRunningNo).

		/// \fn public virtual List<tbs_ARPermissionConfiguration> GetTbs_ARPermissionConfiguration(string strPermissionType)
		/// \brief (Call stored procedure: sp_CT_GetTbs_ARPermissionConfiguration).

		/// \fn public virtual List<dtTbs_ARRunningNo> GetTbs_ARRunningNo(string pOffice, string pYear, string pPrefix)
		/// \brief (Call stored procedure: sp_CT_GetTbs_ARRunningNo).

		/// \fn public virtual List<tbs_ARTypePattern> GetTbs_ARTypePattern(string strARType, string strARTitleType)
		/// \brief (Call stored procedure: sp_CT_GetTbs_ARTypePattern).

		/// \fn public virtual List<tbs_ARTypeTitle> GetTbs_ARTypeTitle(string strARType, string strARTitleType)
		/// \brief (Call stored procedure: sp_CT_GetTbs_ARTypeTitle).

		/// \fn public virtual List<tbs_ContractDocTemplate> GetTbs_ContractDocTemplate(string pDocumentCode)
		/// \brief (Call stored procedure: sp_CT_GetTbs_ContractDocTemplate).

		/// \fn public virtual List<tbs_IncidentPermissionConfiguration> GetTbs_IncidentPermissionConfiguration(string incidentRole)
		/// \brief (Call stored procedure: sp_CT_GetTbs_IncidentPermissionConfiguration).

		/// \fn public virtual List<tbs_IncidentReasonType> GetTbs_IncidentReasonType(string strIncidentType)
		/// \brief (Call stored procedure: sp_CT_GetTbs_IncidentReasonType).

		/// \fn public virtual List<dtTbs_IncidentRunningNo> GetTbs_IncidentRunningNo(string pOffice, string pYear, string pPrefix)
		/// \brief (Call stored procedure: sp_CT_GetTbs_IncidentRunningNo).

		/// \fn public virtual List<dtTbs_IncidentTypePattern> GetTbs_IncidentTypePattern(string strIncidentType)
		/// \brief (Call stored procedure: sp_CT_GetTbs_IncidentTypePattern).

		/// \fn public virtual List<tbt_AR> GetTbt_AR(string pRequestNo)
		/// \brief (Call stored procedure: sp_CT_GetTbt_AR).

		/// \fn public virtual List<tbt_ARFeeAdjustment> GetTbt_ARFeeAdjustment(string pRequestNo)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ARFeeAdjustment).

		/// \fn public virtual List<tbt_ARHistory> GetTbt_ARHistory(string pRequestNo)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ARHistory).

		/// \fn public virtual List<tbt_ARHistoryDetail> GetTbt_ARHistoryDetail(Nullable<int> pARHistoryID)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ARHistoryDetail).

		/// \fn public virtual List<tbt_ARRole> GetTbt_ARRole(string pRequestNo)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ARRole).

		/// \fn public virtual List<tbt_BillingTemp> GetTbt_BillingTemp(string strContractCode, string strOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_BillingTemp).

		/// \fn public virtual List<dtTbt_BillingTempListForView> GetTbt_BillingTempListForView(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_BillingTempListForView).

		/// \fn public virtual List<tbt_CancelContractMemo> GetTbt_CancelContractMemo(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_CancelContractMemo).

		/// \fn public virtual List<tbt_CancelContractMemoDetail> GetTbt_CancelContractMemoDetail(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_CancelContractMemoDetail).

		/// \fn public virtual List<dtTbt_CancelContractMemoDetailForView> GetTbt_CancelContractMemoDetailForView(string contractCode, string oCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_CancelContractMemoDetailForView).

		/// \fn public virtual List<tbt_ContractDocument> GetTbt_ContractDocument(Nullable<int> pDocID)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ContractDocument).

		/// \fn public virtual List<tbt_ContractEmail> GetTbt_ContractEmail(Nullable<int> pContractEmailID)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ContractEmail).

		/// \fn public virtual List<tbt_ContractEmail> GetTbt_ContractEmailByContractCodeOCC(string contractCode, string oCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ContractEmailByContractCodeOCC).

		/// \fn public virtual List<tbt_DocCancelContractMemo> GetTbt_DocCancelContractMemo(Nullable<int> pDocID)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DocCancelContractMemo).

		/// \fn public virtual List<tbt_DocCancelContractMemoDetail> GetTbt_DocCancelContractMemoDetail(Nullable<int> pDocID)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DocCancelContractMemoDetail).

		/// \fn public virtual List<tbt_DocChangeFeeMemo> GetTbt_DocChangeFeeMemo(Nullable<int> pDocID)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DocChangeFeeMemo).

		/// \fn public virtual List<tbt_DocChangeMemo> GetTbt_DocChangeMemo(Nullable<int> pDocID)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DocChangeMemo).

		/// \fn public virtual List<tbt_DocChangeNotice> GetTbt_DocChangeNotice(Nullable<int> pDocID)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DocChangeNotice).

		/// \fn public virtual List<tbt_DocConfirmCurrentInstrumentMemo> GetTbt_DocConfirmCurrentInstrumentMemo(Nullable<int> pDocID)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DocConfirmCurrentInstrumentMemo).

		/// \fn public virtual List<tbt_DocContractReport> GetTbt_DocContractReport(Nullable<int> pDocID)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DocContractReport).

		/// \fn public virtual List<tbt_DraftRentalBEDetails> GetTbt_DraftRentalBEDetails(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftRentalBEDetails).

		/// \fn public virtual List<tbt_DraftRentalBillingTarget> GetTbt_DraftRentalBillingTarget(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftRentalBillingTarget).

		/// \fn public virtual List<tbt_DraftRentalContract> GetTbt_DraftRentalContract(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftRentalContract).

		/// \fn public virtual List<tbt_DraftRentalEmail> GetTbt_DraftRentalEmail(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftRentalEmail).

		/// \fn public virtual List<tbt_DraftRentalInstrument> GetTbt_DraftRentalInstrument(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftRentalInstrument).

		/// \fn public virtual List<tbt_DraftRentalMaintenanceDetails> GetTbt_DraftRentalMaintenanceDetails(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftRentalMaintenanceDetails).

		/// \fn public virtual List<tbt_DraftRentalOperationType> GetTbt_DraftRentalOperationType(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftRentalOperationType).

		/// \fn public virtual List<tbt_DraftRentalSentryGuard> GetTbt_DraftRentalSentryGuard(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftRentalSentryGuard).

		/// \fn public virtual List<tbt_DraftRentalSentryGuardDetails> GetTbt_DraftRentalSentryGuardDetails(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftRentalSentryGuardDetails).

		/// \fn public virtual List<tbt_DraftSaleBillingTarget> GetTbt_DraftSaleBillingTarget(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftSaleBillingTarget).

		/// \fn public virtual List<tbt_DraftSaleContract> GetTbt_DraftSaleContract(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftSaleContract).

		/// \fn public virtual List<tbt_DraftSaleEmail> GetTbt_DraftSaleEmail(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftSaleEmail).

		/// \fn public virtual List<tbt_DraftSaleInstrument> GetTbt_DraftSaleInstrument(string pchrQuotationTargetCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_DraftSaleInstrument).

		/// \fn public virtual List<tbt_RelationType> GetTbt_GetContractLinkageRelation(string paramContractCode, string paramOCC, string paramRelationType)
		/// \brief (Call stored procedure: sp_CT_GetContractLinkageRelation).

		/// \fn public virtual List<tbt_Incident> GetTbt_Incident(Nullable<int> pIncidentID)
		/// \brief (Call stored procedure: sp_CT_GetTbt_Incident).

		/// \fn public virtual List<tbt_IncidentHistoryDetail> GetTbt_IncidentHistoryDetail(Nullable<int> strIncidentHistoryID)
		/// \brief (Call stored procedure: sp_CT_GetTbt_IncidentHistoryDetail).

		/// \fn public virtual List<tbt_IncidentRole> GetTbt_IncidentRole(Nullable<int> incidentID, string empNo)
		/// \brief (Call stored procedure: sp_CT_GetTbt_IncidentRole).

		/// \fn public virtual List<tbt_MaintenanceCheckup> GetTbt_MaintenanceCheckup(string pContractCode, string pProductCode, Nullable<System.DateTime> pInstructionDate)
		/// \brief (Call stored procedure: sp_CT_GetTbt_MaintenanceCheckup).

		/// \fn public virtual List<tbt_MaintenanceCheckupDetails> GetTbt_MaintenanceCheckupDetails(string pContractCode, string pProductCode, Nullable<System.DateTime> pInstructionDate, string pMATargetContractCode, string pMATargetOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_MaintenanceCheckupDetails).

		/// \fn public virtual List<tbt_Project> GetTbt_Project(string pProjectCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_Project).

		/// \fn public virtual List<dtTbt_ProjectExpectedInstrumentDetailsForView> GetTbt_ProjectExpectedInstrumentDetailsForView(string strProjectCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ProjectExpectedInstrumentDetailsForView).

		/// \fn public virtual List<tbt_Project> GetTbt_ProjectForViewSQL(string strProjectCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ProjectForview).

		/// \fn public virtual List<tbt_ProjectOtherRalatedCompany> GetTbt_ProjectOtherRalatedCompanyForView(string strProjectCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ProjectOtherRalatedCompanyForView).

		/// \fn public virtual List<dtTbt_ProjectPurchaserCustomerForView> GetTbt_ProjectPurchaserCustomerForView(string strProjectCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ProjectPurchaserCustomerForView).

		/// \fn public virtual List<dtTbt_ProjectStockoutBranchIntrumentDetailForView> GetTbt_ProjectStockoutBranchIntrumentDetailForView(string strProjectCode, Nullable<int> iBranchNo)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ProjectStockoutBranchIntrumentDetailForView).

		/// \fn public virtual List<tbt_ProjectStockoutBranchIntrumentDetails> GetTbt_ProjectStockoutBranchIntrumentDetails(string strProjectCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ProjectStockoutBranchIntrumentDetails).

		/// \fn public virtual List<tbt_ProjectStockoutInstrument> GetTbt_ProjectStockoutInstrument(string pProjectCode, string pInstrumentCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ProjectStockoutInstrument).

		/// \fn public virtual List<dtTbt_ProjectStockoutIntrumentForView> GetTbt_ProjectStockoutIntrumentForView(string strProjectCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ProjectStockoutIntrumentForView).

		/// \fn public virtual List<tbt_ProjectStockOutMemo> GetTbt_ProjectStockoutMemoForView(string projectCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ProjectStockoutMemoForView).

		/// \fn public virtual List<dtTbt_ProjectSupportStaffDetailForView> GetTbt_ProjectSupportStaffDetailForView(string strProjectCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ProjectSupportStaffDetailForView).

		/// \fn public virtual List<dtTbt_ProjectSystemDetailForView> GetTbt_ProjectSystemDetailForView(string strProjectCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_ProjectSystemDetailForView).

		/// \fn public virtual List<tbt_RelationType> GetTbt_RelationType(string pContractCode, string pOCC, string pRelatedContractCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RelationType).

		/// \fn public virtual List<tbt_RentalBEDetails> GetTbt_RentalBEDetails(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalBEDetails).

		/// \fn public virtual List<dtTbt_RentalBEDetailsForView> GetTbt_RentalBEDetailsForView(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalBEDetails).

		/// \fn public virtual List<tbt_RentalContractBasic> GetTbt_RentalContractBasic(string pContractCode, string pUserCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalContractBasic).

		/// \fn public virtual List<dtTbt_RentalContractBasicForView> GetTbt_RentalContractBasicForView(string pchvContractCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalContractBasicForView).

		/// \fn public virtual List<tbt_RentalInstrumentDetails> GetTbt_RentalInstrumentDetails(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalInstrumentDetails).

		/// \fn public virtual List<dtTbt_RentalInstrumentDetailsListForView> GetTbt_RentalInstrumentDetailsListForView(string pContractCode, string pOCC, string pInstrumentCode, string pInstrumentTypeCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalInstrumentDetailsListForView).

		/// \fn public virtual List<tbt_RentalInstSubcontractor> GetTbt_RentalInstSubContractor(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalInstSubContractor).

		/// \fn public virtual List<dtTbt_RentalInstSubContractorListForView> GetTbt_RentalInstSubContractorListForView(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalInstSubContractorListForView).

		/// \fn public virtual List<tbt_RentalMaintenanceDetails> GetTbt_RentalMaintenanceDetails(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalMaintenanceDetails).

		/// \fn public virtual List<dtTbt_RentalMaintenanceDetailsForView> GetTbt_RentalMaintenanceDetailsForView(string contractCode, string oCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalMaintenanceDetailsForView).

		/// \fn public virtual List<tbt_RentalOperationType> GetTbt_RentalOperationType(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalOperationType).

		/// \fn public virtual List<dtTbt_RentalOperationTypeListForView> GetTbt_RentalOperationTypeListForView(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalOperationTypeListForView).

		/// \fn public virtual List<tbt_RentalSecurityBasic> GetTbt_RentalSecurityBasic(string pchvContractCode, string pchrOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalSecurityBasic).

		/// \fn public virtual List<dtTbt_RentalSecurityBasicForView> GetTbt_RentalSecurityBasicForView(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalSecurityBasicForView).

		/// \fn public virtual List<tbt_RentalSentryGuard> GetTbt_RentalSentryGuard(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalSentryGuard).

		/// \fn public virtual List<tbt_RentalSentryGuardDetails> GetTbt_RentalSentryGuardDetails(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalSentryGuardDetails).

		/// \fn public virtual List<dtTbt_RentalSentryGuardDetailsListForView> GetTbt_RentalSentryGuardDetailsListForView(string pContractCode, string pOCC, Nullable<int> pSequenceNo)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalSentryGuardDetailsListForView).

		/// \fn public virtual List<dtTbt_RentalSentryGuardForView> GetTbt_RentalSentryGuardForView(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_RentalSentryGuardForView).

		/// \fn public virtual List<tbt_SaleBasic> GetTbt_SaleBasic(string pchvContractCode, string pchrOCC, Nullable<bool> pbLastOCCFlag)
		/// \brief (Call stored procedure: sp_CT_GetTbt_SaleBasic).

		/// \fn public virtual List<dtTbt_SaleBasicForView> GetTbt_SaleBasicForView(string pchvContractCode, string pchrOCC, Nullable<bool> pLatestOCCFlag)
		/// \brief (Call stored procedure: sp_CT_GetTbt_SaleBasicForView).

		/// \fn public virtual List<tbt_SaleInstrumentDetails> GetTbt_SaleInstrumentDetails(string pContractCode, string pOCC)
		/// \brief (Call stored procedure: sp_CT_GetTbt_SaleInstrumentDetails).

		/// \fn public virtual List<dtTbt_SaleInstSubcontractorListForView> GetTbt_SaleInstSubcontractorListForView(string pContractCode, string pOCC, string pSubcontractorCode)
		/// \brief (Call stored procedure: sp_CT_GetTbt_SaleInstSubcontractorListForView).

		/// \fn public virtual List<tbt_ContractEmail> GetUnsentNotifyEmail(string pEmailType, Nullable<bool> pSendFlag)
		/// \brief (Call stored procedure: sp_CT_GetUnsentNotifyEmail).

		/// \fn public virtual int InsertEntireContract(string pContractCode, string pGUID, string pScreenID, Nullable<System.DateTime> pCreateDate, string pCreateBy, Nullable<System.DateTime> pLastUpdateDate, string pC_PROD_TYPE_BE, string pC_PROD_TYPE_SG, string pC_PROD_TYPE_MA, string xml_RentalContractBasic, string xml_RentalSecurityBasic, string xml_RentalBEDetails, string xml_RentalInstrumentDetails, string xml_RentalSentryGuard, string xml_RentalSentryGuardDetails, string xml_CancelContractMemoDetail, string xml_RentalOperationType, string xml_RentalMaintenanceDetails, string xml_RelationType)
		/// \brief (Call stored procedure: sp_CT_InsertEntireContract).

		/// \fn public virtual List<tbs_ARApproveNoRunningNo> InsertTbs_ARApproveNoRunningNo(string pYear, string pPrefix, Nullable<int> pRunningNo, string pUser)
		/// \brief (Call stored procedure: sp_CT_InsertTbs_ARApproveNoRunningNo).

		/// \fn public virtual List<tbs_ARRunningNo> InsertTbs_ARRunningNo(string pOffice, string pYear, string pPrefix, Nullable<int> pRunningNo, string pUser)
		/// \brief (Call stored procedure: sp_CT_InsertTbs_ARRunningNo).

		/// \fn public virtual List<tbs_ContractDocTemplate> InsertTbs_ContractDocTemplate(string xml_ContractDocTemplate)
		/// \brief (Call stored procedure: sp_CT_InsertTbs_ContractDocTemplate).

		/// \fn public virtual List<tbs_IncidentRunningNo> InsertTbs_IncidentRunningNo(string pOffice, string pYear, string pPrefix, Nullable<int> pRunningNo, string pUser)
		/// \brief (Call stored procedure: sp_CT_InsertTbs_IncidentRunningNo).

		/// \fn public virtual List<tbt_AR> InsertTbt_AR(string xml_AR)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_AR).

		/// \fn public virtual List<tbt_ARFeeAdjustment> InsertTbt_ARFeeAdjustment(string xml_ARFeeAdjustment)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ARFeeAdjustment).

		/// \fn public virtual List<tbt_ARHistory> InsertTbt_ARHistory(string xml_ARHistory)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ARHistory).

		/// \fn public virtual List<tbt_ARHistoryDetail> InsertTbt_ARHistoryDetail(string xml_ARHistoryDetail)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ARHistoryDetail).

		/// \fn public virtual List<tbt_ARRole> InsertTbt_ARRole(string xml_ARRole)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ARRole).

		/// \fn public virtual List<tbt_BillingTemp> InsertTbt_BillingTemp(string pContractCode, string pOCC, string pBillingOCC, string pBillingTargetRunningNo, string pBillingClientCode, string pBillingTargetCode, string pBillingOfficeCode, string pBillingType, Nullable<int> pCreditTerm, string pBillingTiming, Nullable<decimal> pBillingAmt, string pPayMethod, Nullable<int> pBillingCycle, string pCalDailyFeeStatus, string pSendFlag, Nullable<System.DateTime> pProcessDateTime, string pEmpNo)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_BillingTemp).

		/// \fn public virtual List<tbt_CancelContractMemo> InsertTbt_CancelContractMemo(string xml_CancelContractMemo)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_CancelContractMemo).

		/// \fn public virtual List<tbt_CancelContractMemoDetail> InsertTbt_CancelContractMemoDetail(string xml_CancelContractMemoDetail)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_CancelContractMemoDetail).

		/// \fn public virtual List<tbt_ContractCustomerHistory> InsertTbt_ContractCustomerHistory(string xml_ContractCustomerHistory)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ContractCustomerHistory).

		/// \fn public virtual List<tbt_ContractDocument> InsertTbt_ContractDocument(string xml_ContractDocument)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ContractDocument).

		/// \fn public virtual List<tbt_ContractEmail> InsertTbt_ContractEmail(string xml_ContractEmail)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ContractEmail).

		/// \fn public virtual List<tbt_DocCancelContractMemo> InsertTbt_DocCancelContractMemo(string xml_DocCancelContractMemo)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DocCancelContractMemo).

		/// \fn public virtual List<tbt_DocCancelContractMemoDetail> InsertTbt_DocCancelContractMemoDetail(string xml_DocCancelContractMemoDetail)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DocCancelContractMemoDetail).

		/// \fn public virtual List<tbt_DocChangeFeeMemo> InsertTbt_DocChangeFeeMemo(string xml_DocChangeFeeMemo)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DocChangeFeeMemo).

		/// \fn public virtual List<tbt_DocChangeMemo> InsertTbt_DocChangeMemo(string xml_DocChangeMemo)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DocChangeMemo).

		/// \fn public virtual List<tbt_DocChangeNotice> InsertTbt_DocChangeNotice(string xml_DocChangeNotice)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DocChangeNotice).

		/// \fn public virtual List<tbt_DocConfirmCurrentInstrumentMemo> InsertTbt_DocConfirmCurrentInstrumentMemo(string xml_DocConfirmCurrentInstrumentMemo)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DocConfirmCurrentInstrumentMemo).

		/// \fn public virtual List<tbt_DocContractReport> InsertTbt_DocContractReport(string xml_DocContractReport)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DocContractReport).

		/// \fn public virtual List<tbt_DocInstrumentDetails> InsertTbt_DocInstrumentDetails(string xml_DocInstrumentDetails)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DocInstrumentDetails).

		/// \fn public virtual List<tbt_DraftRentalBEDetails> InsertTbt_DraftRentalBEDetails(string xml_DraftRentalBEDetails)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftRentalBEDetails).

		/// \fn public virtual List<tbt_DraftRentalBillingTarget> InsertTbt_DraftRentalBillingTarget(string xml_DraftRentalBillingTarget)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftRentalBillingTarget).

		/// \fn public virtual List<tbt_DraftRentalContract> InsertTbt_DraftRentalContract(string xml_DraftRentalContract)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftRentalContract).

		/// \fn public virtual List<tbt_DraftRentalEmail> InsertTbt_DraftRentalEmail(string xml)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftRentalEmail).

		/// \fn public virtual List<tbt_DraftRentalInstrument> InsertTbt_DraftRentalInstrument(string xml)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftRentalInstrument).

		/// \fn public virtual List<tbt_DraftRentalMaintenanceDetails> InsertTbt_DraftRentalMaintenanceDetails(string xml)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftRentalMaintenanceDetails).

		/// \fn public virtual List<tbt_DraftRentalOperationType> InsertTbt_DraftRentalOperationType(string xml)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftRentalOperationType).

		/// \fn public virtual List<tbt_DraftRentalSentryGuard> InsertTbt_DraftRentalSentryGuard(string xml)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftRentalSentryGuard).

		/// \fn public virtual List<tbt_DraftRentalSentryGuardDetails> InsertTbt_DraftRentalSentryGuardDetails(string xml)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftRentalSentryGuardDetails).

		/// \fn public virtual List<tbt_DraftSaleBillingTarget> InsertTbt_DraftSaleBillingTarget(string xml)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftSaleBillingTarget).

		/// \fn public virtual List<tbt_DraftSaleContract> InsertTbt_DraftSaleContract(string xml)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftSaleContract).

		/// \fn public virtual List<tbt_DraftSaleEmail> InsertTbt_DraftSaleEmail(string xml)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftSaleEmail).

		/// \fn public virtual List<tbt_DraftSaleInstrument> InsertTbt_DraftSaleInstrument(string xml)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_DraftSaleInstrument).

		/// \fn public virtual List<tbt_Incident> InsertTbt_Incident(string xml_Incident)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_Incident).

		/// \fn public virtual List<tbt_IncidentHistory> InsertTbt_IncidentHistory(string xml_IncidentHistory)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_IncidentHistory).

		/// \fn public virtual List<tbt_IncidentHistoryDetail> InsertTbt_IncidentHistoryDetail(string xml_IncidentHistoryDetail)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_IncidentHistoryDetail).

		/// \fn public virtual List<tbt_IncidentRole> InsertTbt_IncidentRole(string xml_IncidentRole)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_IncidentRole).

		/// \fn public virtual List<tbt_MaintenanceCheckup> InsertTbt_MaintenanceCheckup(string xml_MaintenanceCheckup)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_MaintenanceCheckup).

		/// \fn public virtual List<tbt_MaintenanceCheckupDetails> InsertTbt_MaintenanceCheckupDetails(string xml_MaintenanceCheckupDetails)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_MaintenanceCheckupDetails).

		/// \fn public virtual List<tbt_Project> InsertTbt_Project(string xml_doTbt_Project)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_Project).

		/// \fn public virtual List<tbt_ProjectExpectedInstrumentDetails> InsertTbt_ProjectExpectedInstrumentDetail(string xml_dotbt_ProjectExpectedInstrumentDetails)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ProjectExpectedInstrumentDetail).

		/// \fn public virtual List<tbt_ProjectOtherRalatedCompany> InsertTbt_ProjectOtherRalatedCompany(string xml_dotbt_ProjectOtherRalatedCompany)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ProjectOtherRalatedCompany).

		/// \fn public virtual List<tbt_ProjectPurchaserCustomer> InsertTbt_ProjectPurchaserCustomer(string xml_Tbt_ProjectPurchaserCustomer)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ProjectPurchaserCustomer).

		/// \fn public virtual List<tbt_ProjectStockoutBranchIntrumentDetails> InsertTbt_ProjectStockoutBranchIntrumentDetails(string xML_Tbt_ProjectStockoutBranchIntrumentDetails)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ProjectStockoutBranchIntrumentDetails).

		/// \fn public virtual List<tbt_ProjectStockoutInstrument> InsertTbt_ProjectStockoutInstrument(string xml_doTbtProjectStockoutInstrument)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ProjectStockoutInstrument).

		/// \fn public virtual List<tbt_ProjectStockOutMemo> InsertTbt_ProjectStockOutMemo(string xml_StockOutMemo)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ProjectStockOutMemo).

		/// \fn public virtual List<tbt_ProjectSupportStaffDetails> InsertTbt_ProjectSupportStaffDetail(string xml_Tbt_ProjectSupportStaffDetail)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ProjectSupportStaffDetail).

		/// \fn public virtual List<tbt_ProjectSystemDetails> InsertTbt_ProjectSystemDetail(string xml_Tbt_ProjectSystemDetail)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_ProjectSystemDetail).

		/// \fn public virtual List<tbt_RelationType> InsertTbt_RelationType(string xml_RelationType)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_RelationType).

		/// \fn public virtual List<tbt_RentalBEDetails> InsertTbt_RentalBEDetails(string xml_RentalBEDetails)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_RentalBEDetails).

		/// \fn public virtual List<tbt_RentalContractBasic> InsertTbt_RentalContractBasic(string xml_RentalContractBasic)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_RentalContractBasic).

		/// \fn public virtual List<tbt_RentalInstrumentDetails> InsertTbt_RentalInstrumentDetails(string xml_RentalInstrumentDetails)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_RentalInstrumentDetails).

		/// \fn public virtual List<tbt_RentalInstSubcontractor> InsertTbt_RentalInstSubContractor(string pContractCode, string pOCC, string pSubcontractorCode, Nullable<System.DateTime> pProcessDateTime, string pUser)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_RentalInstSubcontractor).

		/// \fn public virtual List<tbt_RentalMaintenanceDetails> InsertTbt_RentalMaintenanceDetails(string xml_RentalMaintenanceDetails)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_RentalMaintenanceDetails).

		/// \fn public virtual List<tbt_RentalOperationType> InsertTbt_RentalOperationType(string pContractCode, string pOCC, string pOperationTypeCode, Nullable<System.DateTime> pProcessDateTime, string pEmpNo)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_RentalOperationType).

		/// \fn public virtual List<tbt_RentalSecurityBasic> InsertTbt_RentalSecurityBasic(string xml_RentalSecurityBasic)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_RentalSecurityBasic).

		/// \fn public virtual List<tbt_RentalSentryGuard> InsertTbt_RentalSentryGuard(string xml_RentalSentryGuard)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_RentalSentryGuard).

		/// \fn public virtual List<tbt_RentalSentryGuardDetails> InsertTbt_RentalSentryGuardDetails(string xml_RentalSentryGuardDetails)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_RentalSentryGuardDetails).

		/// \fn public virtual List<tbt_SaleBasic> InsertTbt_SaleBasic(string xml_SaleBasic)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_SaleBasic).

		/// \fn public virtual List<tbt_SaleInstrumentDetails> InsertTbt_SaleInstrumentDetails(string xml_SaleInstrumentDetails)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_SaleInstrumentDetails).

		/// \fn public virtual List<tbt_SaleInstSubcontractor> InsertTbt_SaleInstSubcontractor(string pContractCode, string pOCC, string pSubcontractorCode, Nullable<System.DateTime> pDate, string pUser)
		/// \brief (Call stored procedure: sp_CT_InsertTbt_SaleInstSubcontractor).

		/// \fn public virtual List<Nullable<int>> IsAllResultRegistered(string pContractCode, Nullable<System.DateTime> pInstructionDate)
		/// \brief (Call stored procedure: sp_CT_IsAllResultRegistered).

		/// \fn public virtual List<Nullable<bool>> IsCompleteRemoveAll(string strContractCode, string c_RENTAL_INSTALLATION_TYPE_REMOVE_ALL)
		/// \brief (Call stored procedure: sp_CT_IsCompleteRemoveAll).

		/// \fn public virtual List<Nullable<bool>> IsContractDocExist(string strContractCode, string strQuotationTargetCode, string strOCCAlphabet, string strDocOCC)
		/// \brief (Call stored procedure: sp_CT_IsContractDocExist).

		/// \fn public virtual List<Nullable<bool>> IsContractExist(string strContractCode)
		/// \brief (Call stored procedure: sp_CT_IsContractExist).

		/// \fn public virtual List<Nullable<int>> IsLastResultToRegister(string pContractCode, string pProductCode, Nullable<System.DateTime> pInstructionDate)
		/// \brief (Call stored procedure: sp_CT_IsLastResultToRegister).

		/// \fn public virtual List<Nullable<bool>> IsProjectExist(string strProjectCode)
		/// \brief (Call stored procedure: sp_CT_IsProjectExist).

		/// \fn public virtual List<Nullable<int>> IsSomeResultRegistered(string pContractCode, Nullable<System.DateTime> pInstructionDate)
		/// \brief (Call stored procedure: sp_CT_IsSomeResultRegistered).

		/// \fn public virtual List<Nullable<int>> IsUsedCustomer(string pchrCustCode)
		/// \brief (Call stored procedure: sp_CT_IsUsedCustomer).

		/// \fn public virtual List<Nullable<int>> IsUsedSite(string pchrSiteCode)
		/// \brief (Call stored procedure: sp_CT_IsUsedSite).

		/// \fn public virtual List<dtSearchMACheckupResult> SearchAlarmPeriodMaintenance(string pProductName, string pSiteName, string pEmployeeName, string pC_PROD_TYPE_AL, string pC_PROD_TYPE_RENTAL_SALE, string pOperationOfficeCode, Nullable<System.DateTime> pInstructionDateFrom, Nullable<System.DateTime> pInstructionDateTo, string pUserCode, string pContractCode, string pMACheckupNo, Nullable<bool> pHasCheckupResult, Nullable<bool> pHaveInstrumentMalfunction, Nullable<bool> pNeedToContactSalesman)
		/// \brief (Call stored procedure: sp_CT_SearchAlarmPeriodMaintenance).

		/// \fn public virtual List<dtARList> SearchARList(string customerName, string projectName, string requestNo, string approveNo, string aRTitleType, string aRType, string aRStatusHandling, string aRStatusComplete, string aROfficeCode, string specfyPeriod, Nullable<System.DateTime> specifyPeriodFrom, Nullable<System.DateTime> specifyPeriodTo, string requester, string approver, string auditor, string contractTargetPurchaserName, string siteName, string customerGroupName, string contractCode, string userCode, string quotationTargetCode, string contractOfficeCode, string operationOfficeCode, string contractStatus, string contractType, string c_AR_ROLE_APPROVER, string c_AR_ROLE_REQUESTER, string c_AR_ROLE_AUDITOR, string c_AR_TYPE, string c_AR_STATUS, string c_DEADLINE_TIME_TYPE, string c_AR_SEARCH_PERIOD_REQUEST_DATE, string c_AR_SEARCH_PERIOD_APPROVE_DATE, string c_AR_SEARCH_PERIOD_DUEDATE, string c_AR_RELEVANT_TYPE_CUSTOMER, string c_AR_RELEVANT_TYPE_SITE, string c_AR_RELEVANT_TYPE_PROJECT, string c_CUST_ROLE_TYPE_CONTRACT_TARGET, string c_CUST_ROLE_TYPE_REAL_CUST, string c_AR_STATUS_INSTRUCTED, string c_AR_STATUS_REJECTED, string c_AR_STATUS_APPROVED)
		/// \brief (Call stored procedure: sp_CT_SearchARList).

		/// \fn public virtual List<dtContractDocumentList> SearchContractDocument(string strDocStatus, string strContractCode, string strQuotationTargetCode, string strProjectCode, string strOcc, string strAlphabet, string strContractOfficeCode, string strOperationOfficeCode, string strContractOfficeCodeAuthority, string strOperationOfficeCodeAuthority, string strNegotiationStaffEmpNo, string strNegotiationStaffEmpName, string strDocumentCode, string cDocStatus, string cDocAuditResult, Nullable<bool> cFlagOn)
		/// \brief (Call stored procedure: sp_CT_SearchContractDocument).

		/// \fn public virtual List<dtSearchDraftContractResult> SearchDraftContractList(string pchvQuotationTargetCode, string pchrAlphabet, Nullable<System.DateTime> pdtmRegistrationDateFrom, Nullable<System.DateTime> pdtmRegistrationDateTo, string pchvSalesman1Code, string pchvnSaleman1Name, string pchvnContractTargetName, string pchrnSiteName, string pchvnContractOfficeCode, string pchvnOperationOfficeCode, string pchrApproveContractStatus, Nullable<System.DateTime> pdtmApproveDateFrom, Nullable<System.DateTime> pdtmApproveDateTo)
		/// \brief (Call stored procedure: sp_CT_SearchDraftContractList).

		/// \fn public virtual List<dtIncidentList> SearchIncidentList(string incidentNo, string incidentTitle, string incidentType, string incidentStatusHandling, string incidentStatusComplete, string incidentOfficeCode, string specfyPeriod, Nullable<System.DateTime> specifyPeriodFrom, Nullable<System.DateTime> specifyPeriodTo, string registrant, string controlChief, string correspondent, string chief, string assistant, string contractTargetPurchaserName, string siteName, string customerGroupName, string contractCode, string userCode, string contractOfficeCode, string operationOfficeCode, string contractStatus, string contractType, string customerName, string projectName, string c_INCIDENT_ROLE_CONTROL_CHIEF, string c_INCIDENT_ROLE_CHIEF, string c_INCIDENT_ROLE_CORRESPONDENT, string c_INCIDENT_ROLE_ASSISTANT, string c_INCIDENT_TYPE, string c_INCIDENT_STATUS, string c_DEADLINE_TIME_TYPE, string c_INCIDENT_SEARCH_PERIOD_OCCURRING, string c_INCIDENT_SEARCH_PERIOD_DUEDATE, string c_INCIDENT_SEARCH_PERIOD_COMPLETE, string c_INCIDENT_RELEVANT_TYPE_CONTRACT, string c_INCIDENT_RELEVANT_TYPE_CUSTOMER, string c_INCIDENT_RELEVANT_TYPE_PROJECT, string c_INCIDENT_RELEVANT_TYPE_SITE, string c_INCIDENT_STATUS_COMPLETE)
		/// \brief (Call stored procedure: sp_CT_SearchIncidentList).

		/// \fn public virtual List<dtSearchMACheckupResult> SearchSaleMaintenance(string pProductName, string pSiteName, string pEmployeeName, string pOperationOfficeCode, Nullable<System.DateTime> pInstructionDateFrom, Nullable<System.DateTime> pInstructionDateTo, string pUserCode, string pContractCode, string pMACheckupNo, Nullable<bool> pHasCheckupResult, Nullable<bool> pHaveInstrumentMalfunction, Nullable<bool> pNeedToContactSalesman, Nullable<bool> pFLAG_ON, string pC_PROD_TYPE_MA)
		/// \brief (Call stored procedure: sp_CT_SearchSaleMaintenance).

		/// \fn public virtual List<dtSearchSaleWarrantyExpireResult> SearchSaleWarrantyExpireList(Nullable<bool> isMaintenanceContractFlag, Nullable<System.DateTime> dtExpireWarrantyFrom, Nullable<System.DateTime> dtExpireWarrantyTo, string strOperationOfficeCode, string strSaleContractOfficeCode, string pC_CONTRACT_STATUS_BEF_START, string pC_CONTRACT_STATUS_AFTER_START, string pC_RELATION_TYPE_MA, string pC_SALE_CHANGE_TYPE_NEW_SALE, string pC_SALE_CHANGE_TYPE_ADD_SALE)
		/// \brief (Call stored procedure: sp_CT_SearchSaleWarrantyExpireList).

		/// \fn public virtual List<dtSearchMACheckupResult> SearchSeparatedMaintenance(string pProductName, string pSiteName, string pEmployeeName, string pProductTypeCode, string pOperationOfficeCode, Nullable<System.DateTime> pInstructionDateFrom, Nullable<System.DateTime> pInstructionDateTo, string pUserCode, string pContractCode, string pMACheckupNo, Nullable<bool> pHasCheckupResult, Nullable<bool> pHaveInstrumentMalfunction, Nullable<bool> pNeedToContactSalesman)
		/// \brief (Call stored procedure: sp_CT_SearchSeparatedMaintenance).

		/// \fn public virtual int SetNotUsedStatus(string pContractDoc, string pOCC, string pC_CONTRACT_DOC_STATUS_NOT_USED, string pC_CONTRACT_DOC_STATUS_COLLECTED, Nullable<bool> pIsRecursive, string pRef)
		/// \brief (Call stored procedure: sp_CT_SetNotUsedStatus).

		/// \fn public virtual List<doSummaryFee> SumFeeUnimplementData(string strContractCode)
		/// \brief (Call stored procedure: sp_CT_SumFeeUnimplementData).

		/// \fn public virtual List<dtSummaryAR> SummaryAR(Nullable<System.DateTime> dateFrom, Nullable<System.DateTime> dateTo, Nullable<System.DateTime> currentDate, string c_AR_STATUS_INSTRUCTED, string c_AR_STATUS_REJECTED, string c_AR_STATUS_APPROVED)
		/// \brief (Call stored procedure: sp_CT_SummaryAR).

		/// \fn public virtual List<dtSummaryIncident> SummaryIncident(Nullable<System.DateTime> dateFrom, Nullable<System.DateTime> dateTo, Nullable<System.DateTime> currentdate, string c_INCIDENT_STATUS_WAIT_FOR_INSTRUCTION, string c_INCIDENT_STATUS_COMPLETE, string c_INCIDENT_TYPE_CANCEL, string c_INCIDENT_TYPE_COMPLAIN)
		/// \brief (Call stored procedure: sp_CT_SummaryIncident).

		/// \fn public virtual List<doUpdateAutoRenew> UpdateAutoRenew(string contractCode, string oCC, Nullable<System.DateTime> calContractEndDate, Nullable<System.DateTime> updateDate, string updateBy)
		/// \brief (Call stored procedure: sp_CT_UpdateAutoRenew).

		/// \fn public virtual List<tbt_AR> UpdateContractCode(string pQuotationTargetCode, string pContractCode, Nullable<System.DateTime> pProcessingDateTime, string pEmpNo)
		/// \brief (Call stored procedure: sp_CT_UpdateContractCode).

		/// \fn public virtual List<tbt_ContractDocument> UpdateDocumentStatus(string strDocStatus, Nullable<int> iDocID, Nullable<System.DateTime> updateDate, string updateBy)
		/// \brief (Call stored procedure: sp_CT_UpdateDocumentStatus).

		/// \fn public virtual List<tbt_Project> UpdateProjectStatus(string strProjectCode, string c_ProjectStatus, Nullable<System.DateTime> lastCompleteDate, Nullable<System.DateTime> cancelDate, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_CT_UpdateProjectStatus).

		/// \fn public virtual List<tbt_BillingTemp> UpdateSendFlag(string pContractCode, Nullable<int> pSequenceNo, string pC_BILLINGTEMP_FLAG_SENT, Nullable<System.DateTime> pProcessingDateTime, string pEmpNo)
		/// \brief (Call stored procedure: sp_CT_UpdateSendFlag).

		/// \fn public virtual List<tbt_RentalContractBasic> UpdateSummaryFields(string strContractCode, string strLastChangeType, Nullable<System.DateTime> dateLastChangeImplementDate)
		/// \brief (Call stored procedure: sp_CT_UpdateSummaryFields).

		/// \fn public virtual List<tbs_ARApproveNoRunningNo> UpdateTbs_ARApproveNoRunningNo(string pYear, string pPrefix, Nullable<int> pRunningNo, string pUpdateBy)
		/// \brief (Call stored procedure: sp_CT_UpdateTbs_ARApproveNoRunningNo).

		/// \fn public virtual List<tbs_ARRunningNo> UpdateTbs_ARRunningNo(string pOfficeNo, string pYear, string pPrefix, Nullable<int> pRunningNo, string pUpdateBy)
		/// \brief (Call stored procedure: sp_CT_UpdateTbs_ARRunningNo).

		/// \fn public virtual List<tbs_IncidentRunningNo> UpdateTbs_IncidentRunningNo(string pOfficeNo, string pYear, string pPrefix, Nullable<int> pRunningNo, string pUpdateBy)
		/// \brief (Call stored procedure: sp_CT_UpdateTbs_IncidentRunningNo).

		/// \fn public virtual List<tbt_AR> UpdateTbt_AR(string xmlAR)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_AR).

		/// \fn public virtual List<tbt_ARRole> UpdateTbt_ARRole(string xml_doTbtARRole)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_ARRole).

		/// \fn public virtual List<tbt_BillingTemp> UpdateTbt_BillingTemp_ByBillingClientAndOffice(string pBillingOCC, string pBillingTargetCode, string pContractCode, string pBillingClientCode, string pBillingOfficeCode, Nullable<System.DateTime> pProcessingDateTime, string pEmpNo)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_BillingTemp_ByBillingClientAndOffice).

		/// \fn public virtual List<tbt_BillingTemp> UpdateTbt_BillingTemp_ByBillingTarget(string pNewBillingClientCode, string pNewBillingOfficeCode, string pNewBillingTargetCode, string pOldBillingClientCode, string pOldBillingOfficeCode, string pOldBillingTargetCode, string pContractCode, Nullable<System.DateTime> pProcessingDateTime, string pEmpNo, string pC_BILLINGTEMP_FLAG_KEEP)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_BillingTemp_ByBillingTarget).

		/// \fn public virtual List<tbt_BillingTemp> UpdateTbt_BillingTemp_ByKey(string pBillingOCC, string pBillingTargetRunningNo, string pBillingClientCode, string pBillingTargetCode, string pBillingOfficeCode, string pBillingType, Nullable<int> pCreditTerm, string pBillingTiming, Nullable<decimal> pBillingAmt, string pPayMethod, Nullable<int> pBillingCycle, string pCalDailyFeeStatus, string pSendFlag, string pContractCode, string pOCC, Nullable<int> pSequenceNo, Nullable<System.DateTime> pProcessingDateTime, string pEmpNo)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_BillingTemp_ByKey).

		/// \fn public virtual List<tbt_BillingTemp> UpdateTbt_BillingTempByKeyXML(string xml_doTbtUpdateTbtBillingTemp)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_BillingTempByKeyXML).

		/// \fn public virtual List<tbt_CancelContractMemoDetail> UpdateTbt_CancelContractMemoDetail(string xml_doTbtCancelContractMemoDetail)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_CancelContractMemoDetail).

		/// \fn public virtual List<tbt_ContractDocument> UpdateTbt_ContractDocument(string xml)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_ContractDocument).

		/// \fn public virtual List<tbt_ContractEmail> UpdateTbt_ContractEmail(string xml_doTbtContractEmail)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_ContractEmail).

		/// \fn public virtual List<tbt_DraftRentalBEDetails> UpdateTbt_DraftRentalBEDetails(string xml)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_DraftRentalBEDetails).

		/// \fn public virtual List<tbt_DraftRentalContract> UpdateTbt_DraftRentalContract(string xml)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_DraftRentalContract).

		/// \fn public virtual List<tbt_DraftRentalMaintenanceDetails> UpdateTbt_DraftRentalMaintenanceDetails(string xml)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_DraftRentalMaintenanceDetails).

		/// \fn public virtual List<tbt_DraftRentalSentryGuard> UpdateTbt_DraftRentalSentryGuard(string xml)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_DraftRentalSentryGuard).

		/// \fn public virtual List<tbt_DraftSaleContract> UpdateTbt_DraftSaleContract(string xml)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_DraftSaleContract).

		/// \fn public virtual List<tbt_Incident> UpdateTbt_Incident(string xmlIncident)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_Incident).

		/// \fn public virtual List<tbt_IncidentRole> UpdateTbt_IncidentRole(string xml_doTbtIncidentRole)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_IncidentRole).

		/// \fn public virtual List<tbt_MaintenanceCheckup> UpdateTbt_MaintenanceCheckup(string xml_doTbtMaintenanceCheckup)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_MaintenanceCheckup).

		/// \fn public virtual List<tbt_MaintenanceCheckupDetails> UpdateTbt_MaintenanceCheckupDetails(string xml_doTbtMaintenanceCheckupDetails)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_MaintenanceCheckupDetails).

		/// \fn public virtual List<tbt_Project> UpdateTbt_ProjectData(string xmlTbt_Project)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_Project).

		/// \fn public virtual List<tbt_ProjectExpectedInstrumentDetails> UpdateTbt_ProjectExpectedInstrumentDetails(string strProjectCode, string strInstrumentCode, Nullable<int> intInstrumentQty, string updateBy, Nullable<System.DateTime> updateDate)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_ProjectExpectedInstrumentDetails).

		/// \fn public virtual List<tbt_ProjectOtherRalatedCompany> UpdateTbt_ProjectOtherRalatedCompany(string strProjectCode, Nullable<int> sequenceNo, string companyName, string name, string telNo, string remark)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_ProjectOtherRalatedCompany).

		/// \fn public virtual List<tbt_ProjectPurchaserCustomer> UpdateTbt_ProjectPurchaseCustomer(string xmlTbt_ProjectPurchaseCustomer)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_ProjectPurchaseCustomer).

		/// \fn public virtual List<tbt_ProjectStockoutBranchIntrumentDetails> UpdateTbt_ProjectStockoutBranchIntrumentDetails(string strProjectCode, Nullable<int> branchNo, Nullable<int> assignBranchQty, string updateBy, Nullable<System.DateTime> updateDate, string instrumentCode)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_ProjectStockoutBranchIntrumentDetails).

		/// \fn public virtual List<tbt_ProjectStockoutInstrument> UpdateTbt_ProjectStockoutInstrument(string xml_doTbtProjectStockoutInstrument)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_ProjectStockoutInstrument).

		/// \fn public virtual List<tbt_RentalContractBasic> UpdateTbt_RentalContractBasic(string xml_doTbtRentalContractBasic)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_RentalContractBasic).

		/// \fn public virtual List<tbt_RentalSecurityBasic> UpdateTbt_RentalSecurityBasic(string xml_doTbtRentalSecurityBasic)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_RentalSecurityBasic).

		/// \fn public virtual List<tbt_RentalSentryGuard> UpdateTbt_RentalSentryGuard(string xml_doTbtRentalSentryGuard)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_RentalSentryGuard).

		/// \fn public virtual List<tbt_SaleBasic> UpdateTbt_SaleBasic(string xml_doTbtSaleBasic)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_SaleBasic).

		/// \fn public virtual List<tbt_SaleInstrumentDetails> UpdateTbt_SaleInstrumentDetails(string xml_doTbt_SaleInstrumentDetails)
		/// \brief (Call stored procedure: sp_CT_UpdateTbt_SaleInstrumentDetails).


	}
}

