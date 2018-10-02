namespace SECOM_AJIS.DataEntity.Installation
{
    partial class BizISDataEntities
    {
        /// \fn public virtual List<CheckAllRemoval_Result> CheckAllRemoval(string strContractCode, string c_RENTAL_INSTALL_TYPE_REMOVE_ALL, string c_SALE_INSTALL_TYPE_REMOVE_ALL, string c_INSTALL_STATUS_COMPLETED)
        /// \brief (Call stored procedure: sp_IS_CheckAllRemoval).

        /// \fn public virtual List<CheckCancelContractBeforeStartService_Result> CheckCancelContractBeforeStartService(string strContractCode, string c_CONTRACT_STATUS_END, string c_CONTRACT_STATUS_CANCEL, string c_CONTRACT_STATUS_FIXED_CANCEL)
        /// \brief (Call stored procedure: sp_IS_CheckCancelContractBeforeStartService).

        /// \fn public virtual List<CheckCancelInstallationManagement_Result> CheckCancelInstallationManagement(string strInstallationMaintenanceNo, string c_INSTALL_MANAGE_STATUS_CANCELED)
        /// \brief (Call stored procedure: sp_IS_CheckCancelInstallationManagement).

        /// \fn public virtual List<doCheckInstallationDataToOpenScreen> CheckInstallationDataToOpenScreen(string strCode)
        /// \brief (Call stored procedure: sp_IS_CheckInstallationDataToOpenScreen).

        /// \fn public virtual List<CheckInstallationRegistered_Result> CheckInstallationRegistered(string strContractCode, string c_INSTALL_STATUS_INSTALLATION_NOT_REQUESTED)
        /// \brief (Call stored procedure: sp_IS_CheckInstallationRegistered).

        /// \fn public virtual List<tbt_InstallationAttachFile> DeleteTbt_InstallationAttachFile(Nullable<int> attachFileID, string maintenanceNo, string objectID)
        /// \brief (Call stored procedure: sp_IS_DeleteTbt_InstallationAttachFile).

        /// \fn public virtual List<tbt_InstallationBasic> DeleteTbt_InstallationBasic(string contractProjectCode)
        /// \brief (Call stored procedure: sp_IS_DeleteTbt_InstallationBasic).

        /// \fn public virtual List<tbt_InstallationInstrumentDetails> DeleteTbt_InstallationInstrumentDetail(string contractCode, string instrumentCode)
        /// \brief (Call stored procedure: sp_IS_DeleteTbt_InstallationInstrumentDetail).

        /// \fn public virtual List<tbt_InstallationMemo> DeleteTbt_InstallationMemo(string contractProjectCode, string referenceID, string objectID)
        /// \brief (Call stored procedure: sp_IS_DeleteTbt_InstallationMemo).

        /// \fn public virtual List<tbt_InstallationPOManagement> DeleteTbt_InstallationPOManagement(string maintenanceNo)
        /// \brief (Call stored procedure: sp_IS_DeleteTbt_InstallationPOManagement).

        /// \fn public virtual List<doAllSlipNoSeries> GetAllSlipNoSeries(string pContractProjectCode, string pOCC, string pLatestSlipNo)
        /// \brief (Call stored procedure: sp_IS_GetAllSlipNoSeries).

        /// \fn public virtual List<dtRequestApproveInstallation> GetEmailForApprove()
        /// \brief (Call stored procedure: sp_IS_GetEmailForApprove).

        /// \fn public virtual List<ContractCodeList> GetInstallationBasicContractByProject(string cProjectCode)
        /// \brief (Call stored procedure: sp_IS_GetInstallationBasicContractByProject).

        /// \fn public virtual List<dtInstallation> GetInstallationDataListForCsvFile(string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL, string c_INSTALL_STATUS_COMPLETED, string c_RENTAL_INSTALL_TYPE_REMOVE_ALL, string c_SALE_INSTALL_TYPE_REMOVE_ALL, string c_SALE_INSTALL_TYPE, string c_RENTAL_INSTALL_TYPE, Nullable<bool> slipNoNullFlag, string c_INSTALL_STATUS_NO_INSTALLATION)
        /// \brief (Call stored procedure: sp_IS_GetInstallationDataListForCsvFile).

        /// \fn public virtual List<dtInstallation> GetInstallationDataListForView(string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL, string c_INSTALL_STATUS_COMPLETED, string c_RENTAL_INSTALL_TYPE_REMOVE_ALL, string c_SALE_INSTALL_TYPE_REMOVE_ALL, string c_SALE_INSTALL_TYPE, string c_RENTAL_INSTALL_TYPE, string contractCode, string userCode, string planCode, string slipNo, string installationMaintenanceNo, string operationOfficeCode, string salesmanEmpNo, Nullable<System.DateTime> slipIssueDateFrom, Nullable<System.DateTime> slipIssueDateTo, string contractTargetPurchaserName, string siteCode, string siteName, string siteAddress, string installationStatus, string slipStatus, string managementStatus, Nullable<bool> slipNoNullFlag, Nullable<bool> viewFlag)
        /// \brief (Call stored procedure: sp_IS_GetInstallationDataListForView).

        /// \fn public virtual List<doInstallationDetailForCompleteInstallation> GetInstallationDetailForCompleteInstallation(string pC_INST_TYPE_GENERAL, string pSlipNo)
        /// \brief (Call stored procedure: sp_IS_GetInstallationDetailForCompleteInstallation).

        /// \fn public virtual List<doGetNormalRemovalFee> GetNormalRemovalFee(string pContractCode, string c_RENTAL_INSTALL_TYPE_REMOVE_ALL, string c_SLIP_STATUS_INSTALL_SLIP_CANCELED, string c_SLIP_STATUS_REPLACED)
        /// \brief (Call stored procedure: sp_IS_GetNormalRemovalFee).

        /// \fn public virtual List<doRentalInstrumentdataList> GetRentalInstrumentdataList(string vcContractCode, string vcOCC, string vcSlipNo, string vcInstrumentTyepCode)
        /// \brief (Call stored procedure: sp_IS_GetRentalInstrumentdataList).

        /// \fn public virtual List<RPTAcceptInspecDo> GetRptAcceptInspecNocticeData(string cMaintenanceNo, string cSubcontractorCode, string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL, string c_SERVICE_TYPE_PROJECT)
        /// \brief (Call stored procedure: sp_IS_GetRptAcceptInspecNocticeData).

        /// \fn public virtual List<RPTChangeSlipDo> GetRptChangeSlipData(string pSlipNo, string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL, string c_SALE_INSTALL_TYPE, string c_RENTAL_INSTALL_TYPE)
        /// \brief (Call stored procedure: sp_IS_GetRptChangeSlipData).

        /// \fn public virtual List<RPTDeliveryConfirmDo> GetRptDeliveryConfirmData(string cSlipNo, string c_CONFIG_INSTALL_WARRANTY_COND)
        /// \brief (Call stored procedure: sp_IS_GetRptDeliveryConfirmData).

        /// \fn public virtual List<RPTIECheckSheetDo> GetRptIECheckSheetData(string cMaintenanceNo, string cSubcontractorCode, string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL, string c_SERVICE_TYPE_PROJECT)
        /// \brief (Call stored procedure: sp_IS_GetRptIECheckSheetData).

        /// \fn public virtual List<RPTInstallRequestDo> GetRptInstallationRequestData(string cMaintenanceNo, string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL, string c_SERVICE_TYPE_PROJECT, string c_SALE_INSTALL_TYPE, string c_RENTAL_INSTALL_TYPE, string c_NEW_BLD_MGMT_FLAG, string c_BUILDING_TYPE, string c_PHONE_LINE_OWNER_TYPE)
        /// \brief (Call stored procedure: sp_IS_GetRptInstallationRequestData).

        /// \fn public virtual List<RPTInstallCompleteDo> GetRptInstallCompleteConfirmData(string pSlipNo, string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL, string c_SALE_INSTALL_TYPE, string c_RENTAL_INSTALL_TYPE)
        /// \brief (Call stored procedure: sp_IS_GetRptInstallCompleteConfirmData).

        /// \fn public virtual List<RPTInstallSpecCompleteDo> GetRptInstallSpecCompleteData(string cMaintenanceNo, string cSubcontractorCode, string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL, string c_SERVICE_TYPE_PROJECT)
        /// \brief (Call stored procedure: sp_IS_GetRptInstallSpecCompleteData).

        /// \fn public virtual List<RPTNewRentalSlipDo> GetRptNewRetalSlipData(string pSlipNo)
        /// \brief (Call stored procedure: sp_IS_GetRptNewRentalSlipData).

        /// \fn public virtual List<RPTNewSaleSlipDo> GetRptNewSaleSlipData(string pSlipNo, string c_SALE_INSTALL_TYPE)
        /// \brief (Call stored procedure: sp_IS_GetRptNewSaleSlipData).

        /// \fn public virtual List<RPTPOSubPriceDo> GetRptPOSubPriceData(string cMaintenanceNo, string cSubcontractorCode, string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL, string c_SERVICE_TYPE_PROJECT, string c_SALE_INSTALL_TYPE, string c_RENTAL_INSTALL_TYPE)
        /// \brief (Call stored procedure: sp_IS_GetRptPOSubPriceData).

        /// \fn public virtual List<RPTRemoveSlipDo> GetRptRemoveSlipData(string pSlipNo, string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL)
        /// \brief (Call stored procedure: sp_IS_GetRptRemoveSlipData).

        /// \fn public virtual List<doSaleInstrumentdataList> GetSaleInstrumentdataList(string vcContractCode, string vcOCC, string vcSlipNo, string vcInstrumentTyepCode)
        /// \brief (Call stored procedure: sp_IS_GetSaleInstrumentdataList).

        /// \fn public virtual List<InstallationMARunningNo> GetTbs_InstallationMARunningNo(string officeCode, string prefix, Nullable<int> year)
        /// \brief (Call stored procedure: sp_IS_GetTbs_InstallationMARunningNo).

        /// \fn public virtual List<InstallationSlipRunningNo_Result> GetTbs_InstallationSlipRunningNo(string officeCode, string slipID, string year, string month)
        /// \brief (Call stored procedure: sp_IS_GetTbs_InstallationSlipRunningNo).

        /// \fn public virtual List<tbt_InstallationAttachFile> GetTbt_InstallationAttachFile(Nullable<int> attachFileID, string maintenanceNo, string objectID)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationAttachFile).

        /// \fn public virtual List<tbt_InstallationBasic> GetTbt_InstallationBasic(string pContractProjectCode)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationBasic).

        /// \fn public virtual List<tbt_InstallationEmail> GetTbt_InstallationEmail(string pMaintenanceNo)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationEmail).

        /// \fn public virtual List<tbt_InstallationHistory> GetTbt_InstallationHistory(string contractProjectCode, string maintenanceNo, Nullable<int> historyNo)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationHistory).

        /// \fn public virtual List<dtInstallationHistoryForView> GetTbt_InstallationHistoryForView(string c_SERVICE_TYPE_SALE, string c_SERVICE_TYPE_RENTAL, string c_SALE_INSTALL_TYPE, string c_RENTAL_INSTALL_TYPE, string c_CHANGE_REASON_TYPE_CUSTOMER, string c_CHANGE_REASON_TYPE_SECOM, string c_CUSTOMER_REASON, string c_SECOM_REASON, string contractProjectCode, string maintenanceNo, string slipNo)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationHistoryForView).

        /// \fn public virtual List<tbt_InstallationInstrumentDetails> GetTbt_InstallationInstrumentDetails(string pContractCode, string pInstrumentCode)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationInstrumentDetails).

        /// \fn public virtual List<tbt_InstallationManagement> GetTbt_InstallationManagement(string pMaintenanceNo)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationManagement).

        /// \fn public virtual List<tbt_InstallationMemo> GetTbt_InstallationMemo(string pMaintenanceNo)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationMemo).

        /// \fn public virtual List<dtInstallationMemoForView> GetTbt_InstallationMemoForView(string contractProjectCode)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationMemoForView).

        /// \fn public virtual List<tbt_InstallationPOManagement> GetTbt_InstallationPOManagement(string pMaintenanceNo)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationPOManagement).

        /// \fn public virtual List<dtInstallationPOManagementForView> GetTbt_InstallationPOManagementForView(string pMaintenanceNo)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationPOManagementForView).

        /// \fn public virtual List<tbt_InstallationSlip> GetTbt_InstallationSlip(string pSlipNo)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationSlip).

        /// \fn public virtual List<tbt_InstallationSlipDetails> GetTbt_InstallationSlipDetails(string slipNo, string instrumentCode, string instrumentTypeCode)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationSlipDetails).

        /// \fn public virtual List<dtInstallationSlipDetailsForView> GetTbt_InstallationSlipDetailsForView(string slipNo, string instrumentCode)
        /// \brief (Call stored procedure: sp_IS_GetTbt_InstallationSlipDetailsForView).

        /// \fn public virtual int InsertTbs_InstallationMARunningNo(string officeCode, string prefix, Nullable<int> year, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy, Nullable<int> runningNo)
        /// \brief (Call stored procedure: sp_IS_InsertTbs_InstallationMARunningNo).

        /// \fn public virtual int InsertTbs_InstallationSlipRunningNo(string officeCode, string slipID, string year, string month, Nullable<int> runningNo, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
        /// \brief (Call stored procedure: sp_IS_InsertTbs_InstallationSlipRunningNo).

        /// \fn public virtual List<tbt_InstallationAttachFile> InsertTbt_InstallationAttachFile(string xml_POManagement)
        /// \brief (Call stored procedure: sp_IS_InsertTbt_InstallationAttachFile).

        /// \fn public virtual List<tbt_InstallationBasic> InsertTbt_InstallationBasic(string pContractProjectCode, string pOCC, string pServiceTypeCode, string pInstallationStatus, string pInstallationType, string pPlanCode, string pSlipNo, string pMaintenanceNo, string pOperationOfficeCode, string pSecurityTypeCode, string pChangeReasonTypeCode, Nullable<decimal> pNormalInstallFee, Nullable<decimal> pBillingInstallFee, string pInstallFeeBillingType, Nullable<decimal> pNormalSaleProductPrice, Nullable<decimal> pBillingSalePrice, Nullable<System.DateTime> pInstallationSlipProcessingDate, Nullable<System.DateTime> pInstallationCompleteDate, Nullable<System.DateTime> pInstallationCompleteProcessingDate, string pInstallationBy, string pSalesmanEmpNo1, string pSalesmanEmpNo2, string pApproveNo1, string pApproveNo2, Nullable<System.DateTime> pInstallationStartDate, Nullable<System.DateTime> pInstallationFinishDate, Nullable<decimal> pNormalContractFee, string pBillingOCC, Nullable<System.DateTime> pCreateDate, string pCreateBy, Nullable<System.DateTime> pUpdateDate, string pUpdateBy)
        /// \brief (Call stored procedure: sp_IS_InsertTbt_InstallationBasic).

        /// \fn public virtual List<tbt_InstallationEmail> InsertTbt_InstallationEmail(Nullable<int> emailID, string referenceID, string emailNoticeTarget, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
        /// \brief (Call stored procedure: sp_IS_InsertTbt_InstallationEmail).

        /// \fn public virtual List<tbt_InstallationHistory> InsertTbt_InstallationHistory(string contractProjectCode, string oCC, string serviceTypeCode, string installationStatus, string installationType, string planCode, string slipNo, string maintenanceNo, string operationOfficeCode, string securityTypeCode, string changeReasonTypeCode, Nullable<decimal> normalInstallFee, Nullable<decimal> billingInstallFee, string installFeeBillingType, Nullable<decimal> normalSaleProductPrice, Nullable<decimal> billingSalePrice, Nullable<System.DateTime> installationSlipProcessingDate, Nullable<System.DateTime> installationCompleteDate, Nullable<System.DateTime> installationCompleteProcessingDate, string installationBy, string salesmanEmpNo1, string salesmanEmpNo2, string approveNo1, string approveNo2, Nullable<System.DateTime> installationStartDate, Nullable<System.DateTime> installationFinishDate, Nullable<decimal> normalContractFee, string billingOCC, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
        /// \brief (Call stored procedure: sp_IS_InsertTbt_InstallationHistory).

        /// \fn public virtual List<tbt_InstallationHistoryDetails> InsertTbt_InstallationHistoryDetail(string contractCode, string instrumentCode, string instrumentTypeCode, Nullable<int> contractInstalledQty, Nullable<int> contractRemovedQty, Nullable<int> contractMovedQty, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
        /// \brief (Call stored procedure: sp_IS_InsertTbt_InstallationHistoryDetail).

        /// \fn public virtual List<tbt_InstallationInstrumentDetails> InsertTbt_InstallationInstrumentDetails(string contractCode, string instrumentCode, string instrumentTypeCode, Nullable<int> contractInstalledQty, Nullable<int> contractRemovedQty, Nullable<int> contractMovedQty, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
        /// \brief (Call stored procedure: sp_IS_InsertTbt_InstallationInstrumentDetails).

        /// \fn public virtual List<tbt_InstallationManagement> InsertTbt_InstallationManagement(string maintenanceNo, string contractProjectCode, string managementStatus, Nullable<System.DateTime> proposeInstallStartDate, Nullable<System.DateTime> proposeInstallCompleteDate, string customerStaffBelonging, string customerStaffName, string customerStaffPhoneNo, Nullable<System.DateTime> newPhoneLineOpenDate, string newConnectionPhoneNo, string newPhoneLineOwnerTypeCode, string iEStaffEmpNo1, string iEStaffEmpNo2, Nullable<int> iEManPower, Nullable<decimal> materialFee, string requestMemo, string pOMemo, string changeReasonCode, string changeReasonOther, string changeRequestorCode, string changeRequestorOther, Nullable<bool> newBldMgmtFlag, Nullable<decimal> newBldMgmtCost, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
        /// \brief (Call stored procedure: sp_IS_InsertTbt_InstallationManagement).

        /// \fn public virtual List<tbt_InstallationMemo> InsertTbt_InstallationMemo(Nullable<int> memoID, string contractProjectCode, string referenceID, string objectID, string memo, string officeCode, string departmentCode, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
        /// \brief (Call stored procedure: sp_IS_InsertTbt_InstallationMemo).

        /// \fn public virtual List<tbt_InstallationPOManagement> InsertTbt_InstallationPOManagement(string xml_POManagement)
        /// \brief (Call stored procedure: sp_IS_InsertTbt_InstallationPOManagement).

        /// \fn public virtual List<tbt_InstallationSlip> InsertTbt_InstallationSlip(string slipNo, string serviceTypeCode, string slipStatus, string changeReasonCode, string installationType, string planCode, string causeReason, Nullable<decimal> normalContractFee, Nullable<decimal> normalInstallFee, string installFeeBillingType, Nullable<decimal> billingInstallFee, string billingOCC, string previousSlipNo, string previousSlipStatus, string contractCode, Nullable<System.DateTime> slipIssueDate, string slipIssueOfficeCode, Nullable<System.DateTime> stockOutDate, string stockOutOfficeCode, Nullable<System.DateTime> returnReceiveDate, string returnReceiveOfficeCode, string approveNo1, string approveNo2, string changeContents, Nullable<System.DateTime> expectedInstrumentArrivalDate, string stockOutTypeCode, string slipType, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy, string additionalStockOutOfficeCode, Nullable<bool> slipIssueFlag, string unremoveApproveNo)
        /// \brief (Call stored procedure: sp_IS_InsertTbt_InstallationSlip).

        /// \fn public virtual List<tbt_InstallationSlipDetails> InsertTbt_InstallationSlipDetails(string slipNo, string instrumentCode, string instrumentTypeCode, Nullable<int> contractInstalledQty, Nullable<int> currentStockOutQty, Nullable<int> totalStockOutQty, Nullable<int> addInstalledQty, Nullable<int> returnQty, Nullable<int> addRemovedQty, Nullable<int> notInstalledQty, Nullable<int> moveQty, Nullable<int> mAExchangeQty, Nullable<int> unremovableQty, Nullable<int> returnRemoveQty, Nullable<decimal> instrumentPrice, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy, Nullable<int> partialStockOutQty)
        /// \brief (Call stored procedure: sp_IS_InsertTbt_InstallationSlipDetails).

        /// \fn public virtual List<tbt_InstallationSlipExpansion> InsertTbt_InstallationSlipExpansion(string xml_SlipExpansion)
        /// \brief (Call stored procedure: sp_IS_InsertTbt_InstallationSlipExpansion).

        /// \fn public virtual List<doSearchInstallManagementResult> SearchInstallationManagementList(string c_FLAG_ON, string c_INSTALL_MANAGE_STATUS_CANCELED, string contractCode, string projectCode, string installationType, string iEStaffCode, string subContractorCode, string subcontractorGroupName, Nullable<System.DateTime> proposedInstallationCompleteDateFrom, Nullable<System.DateTime> proposedInstallationCompleteDateTo, Nullable<System.DateTime> installationCompleteDateFrom, Nullable<System.DateTime> installationCompleteDateTo, Nullable<System.DateTime> installationStartDateFrom, Nullable<System.DateTime> installationStartDateTo, Nullable<System.DateTime> installationFinishDateFrom, Nullable<System.DateTime> installationFinishDateTo, string siteName, string siteAddress, string operationOfficeCode, string installationManagementStatus)
        /// \brief (Call stored procedure: sp_IS_SearchInstallationManagementList).

        /// \fn public virtual List<doPrepareCompleteInstallationData> Temp_CompleteInstallation_Rental(string vcContractCode)
        /// \brief (Call stored procedure: sp_IS_Temp_CompleteInstallation_Rental).

        /// \fn public virtual List<doPrepareCompleteInstallationData> Temp_CompleteInstallation_Sale(string vcContractCode)
        /// \brief (Call stored procedure: sp_IS_Temp_CompleteInstallation_Sale).

        /// \fn public virtual int UpdateTbs_InstallationMARunningNo(string officeCode, string prefix, Nullable<int> year, Nullable<int> runningNo, Nullable<System.DateTime> updateDate, string updateBy)
        /// \brief (Call stored procedure: sp_IS_UpdateTbs_InstallationMARunningNo).

        /// \fn public virtual int UpdateTbs_InstallationSlipRunningNo(string officeCode, string slipID, string year, string month, Nullable<int> runningNo, Nullable<System.DateTime> updateDate, string updateBy)
        /// \brief (Call stored procedure: sp_IS_UpdateTbs_InstallationSlipRunningNo).

        /// \fn public virtual List<tbt_InstallationBasic> UpdateTbt_InstallationBasic(string pContractProjectCode, string pOCC, string pServiceTypeCode, string pInstallationStatus, string pInstallationType, string pPlanCode, string pSlipNo, string pMaintenanceNo, string pOperationOfficeCode, string pSecurityTypeCode, string pChangeReasonTypeCode, Nullable<decimal> pNormalInstallFee, Nullable<decimal> pBillingInstallFee, string pInstallFeeBillingType, Nullable<decimal> pNormalSaleProductPrice, Nullable<decimal> pBillingSalePrice, Nullable<System.DateTime> pInstallationSlipProcessingDate, Nullable<System.DateTime> pInstallationCompleteDate, Nullable<System.DateTime> pInstallationCompleteProcessingDate, string pInstallationBy, string pSalesmanEmpNo1, string pSalesmanEmpNo2, string pApproveNo1, string pApproveNo2, Nullable<System.DateTime> pInstallationStartDate, Nullable<System.DateTime> pInstallationFinishDate, Nullable<decimal> pNormalContractFee, string pBillingOCC, Nullable<System.DateTime> pCreateDate, string pCreateBy, Nullable<System.DateTime> pUpdateDate, string pUpdateBy)
        /// \brief (Call stored procedure: sp_IS_UpdateTbt_InstallationBasic).

        /// \fn public virtual List<tbt_InstallationManagement> UpdateTbt_InstallationManagement(string pMaintenanceNo, string pContractProjectCode, string pManagementStatus, Nullable<System.DateTime> pProposeInstallStartDate, Nullable<System.DateTime> pProposeInstallCompleteDate, string pCustomerStaffBelonging, string pCustomerStaffName, string pCustomerStaffPhoneNo, Nullable<System.DateTime> pNewPhoneLineOpenDate, string pNewConnectionPhoneNo, string pNewPhoneLineOwnerTypeCode, string pIEStaffEmpNo1, string pIEStaffEmpNo2, Nullable<int> pIEManPower, Nullable<decimal> pMaterialFee, string pRequestMemo, string pPOMemo, string pChangeReasonCode, string pChangeReasonOther, string pChangeRequestorCode, string pChangeRequestorOther, Nullable<bool> pNewBldMgmtFlag, Nullable<decimal> pNewBldMgmtCost, Nullable<System.DateTime> pCreateDate, string pCreateBy, Nullable<System.DateTime> pUpdateDate, string pUpdateBy)
        /// \brief (Call stored procedure: sp_IS_UpdateTbt_InstallationManagement).

        /// \fn public virtual List<tbt_InstallationPOManagement> UpdateTbt_InstallationPOManagement(string xml_POManagement)
        /// \brief (Call stored procedure: sp_IS_UpdateTbt_InstallationPOManagement).

        /// \fn public virtual List<tbt_InstallationSlip> UpdateTbt_InstallationSlip(string slipNo, string serviceTypeCode, string slipStatus, string changeReasonCode, string installationType, string planCode, string causeReason, Nullable<decimal> normalContractFee, Nullable<decimal> normalInstallFee, string installFeeBillingType, Nullable<decimal> billingInstallFee, string billingOCC, string previousSlipNo, string previousSlipStatus, string contractCode, Nullable<System.DateTime> slipIssueDate, string slipIssueOfficeCode, Nullable<System.DateTime> stockOutDate, string stockOutOfficeCode, Nullable<System.DateTime> returnReceiveDate, string returnReceiveOfficeCode, string approveNo1, string approveNo2, string changeContents, Nullable<System.DateTime> expectedInstrumentArrivalDate, string stockOutTypeCode, string slipType, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy, string additionalStockOutOfficeCode, Nullable<bool> slipIssueFlag, string unremoveApproveNo)
        /// \brief (Call stored procedure: sp_IS_UpdateTbt_InstallationSlip).

        /// \fn public virtual List<tbt_InstallationSlipDetails> UpdateTbt_InstallationSlipDetails(string slipNo, string instrumentCode, string instrumentTypeCode, Nullable<int> contractInstalledQty, Nullable<int> currentStockOutQty, Nullable<int> totalStockOutQty, Nullable<int> addInstalledQty, Nullable<int> returnQty, Nullable<int> addRemovedQty, Nullable<int> notInstalledQty, Nullable<int> moveQty, Nullable<int> mAExchangeQty, Nullable<int> unremovableQty, Nullable<int> returnRemoveQty, Nullable<decimal> instrumentPrice, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy, Nullable<int> partialStockOutQty)
        /// \brief (Call stored procedure: sp_IS_UpdateTbt_InstallationSlipDetails).

        /// \fn public virtual List<string> GetInstallationSlipNoForAcceptant(string pContractProjectCode, string pOCC, string pC_SALE_INSTALL_TYPE_ADD, string pC_SALE_INSTALL_TYPE_NEW)
        /// \brief (Call stored procedure: sp_IS_GetInstallationSlipNoForAcceptant).
    }
}

