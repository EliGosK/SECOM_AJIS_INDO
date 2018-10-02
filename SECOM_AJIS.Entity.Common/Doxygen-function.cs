namespace SECOM_AJIS.DataEntity.Common
{
	partial class BizCMDataEntities
	{
		/// \fn public virtual int BackUpAttachFile(Nullable<int> pAttachFileID, string pSessionID)
		/// \brief (Call stored procedure: sp_CM_BackUpAttachFile).

		/// \fn public virtual List<tbt_AttachFile> CopyAttachFile(string relatedID, string newRelatedID, string module)
		/// \brief (Call stored procedure: sp_CM_CopyAttachFile).

		/// \fn public virtual int DeleteAttachFileByID(Nullable<int> attachFileID)
		/// \brief (Call stored procedure: sp_CM_DeleteAttachFileByID).

		/// \fn public virtual int DeleteAttachFileByRelatedID(string relatedID)
		/// \brief (Call stored procedure: sp_CM_DeleteAttachFileByRelatedID).

		/// \fn public virtual List<tbt_PurgeLog> DeleteLog(Nullable<System.DateTime> monthYear, string empNo)
		/// \brief (Call stored procedure: sp_CM_DeleteLog).

		/// \fn public virtual List<dtGetAccumulateFileSize> GetAccumulateFileSize(string pRelatedID, string pSession)
		/// \brief (Call stored procedure: sp_CM_GetAccumulateFileSize).

		/// \fn public virtual List<tbt_AttachFile> GetAttachFile(string relatedID)
		/// \brief (Call stored procedure: sp_CM_GetAttachFile).

		/// \fn public virtual List<dtAttachFileForGridView> GetAttachFileForGridView(string pRelatedID)
		/// \brief (Call stored procedure: sp_CM_GetAttachFileForGridView).

		/// \fn public virtual List<dtAttachFileNameID> GetAttachFileName(string relatedID, Nullable<int> attachFileID, Nullable<bool> uploadCompleteFlag)
		/// \brief (Call stored procedure: sp_CM_getAttachFileName).

		/// \fn public virtual List<dtBatchProcess> GetBatchProcessDataList(string pC_CONFIG_SUSPEND_FLAG, string pC_BATCH_STATUS, string pC_BATCH_LAST_RESULT, string pC_BATCH_STATUS_PROCESSING, Nullable<bool> pC_FLAG_ON, Nullable<bool> pC_FLAG_OFF)
		/// \brief (Call stored procedure: sp_CM_GetBatchProcessDataList).

		/// \fn public virtual List<GetBatchProcessRunAll_Result> GetBatchProcessRunAll()
		/// \brief (Call stored procedure: sp_CM_GetBatchProcessRunAll).

		/// \fn public virtual List<dtBillingOffice> GetBillingOffice(string documentType, Nullable<System.DateTime> datBatchDate)
		/// \brief (Call stored procedure: sp_CM_GetBillingOffice).

		/// \fn public virtual List<dtDocumentData> GetDocumentDataList(string pchvDocumentType, string pchvDocumentCode, Nullable<System.DateTime> pdatGenerateDateFrom, Nullable<System.DateTime> pdatGenerateDateTo, Nullable<int> pdatMonth, Nullable<int> pdatYear, string pchrContractOfficeCode, string pchrOperationOfficeCode, string pchrBillingOfficeCode, string pchrIssueOfficeCode, string pchrDocumentNo, string pchrQuotationTargetCode, string pchrAlphabet, string pchrProjectCode, string pchrContractCode, string pchrOCC, string pchrBillingTargetCode, string pchvInstrumentCode, string pC_DOCUMENT_TYPE_CONTRACT, string pC_DOCUMENT_TYPE_MA, string pC_DOCUMENT_TYPE_INSTALLATION, string pC_DOCUMENT_TYPE_INVENTORY, string pC_DOCUMENT_TYPE_INCOME, string pOfficeCodeList, string pC_DOCUMENT_TYPE_COMMON, string pLocationCode)
		/// \brief (Call stored procedure: sp_CM_GetDocumentDataList).

		/// \fn public virtual List<dtDocumentListForPrining> GetDocumentListForPrining(string c_DOCUMENT_TYPE_INCOME, string c_DOCUMENT_TYPE_COMMON, Nullable<System.DateTime> datIssueDate, Nullable<int> intManagementNoFrom, Nullable<int> intManagementNoTo)
		/// \brief (Call stored procedure: sp_CM_GetDocumentListForPrining).

		/// \fn public virtual List<dtDocumentNameDataList> GetDocumentNameDataList(string pchvDocumentType, string pObjectIDList)
		/// \brief (Call stored procedure: sp_CM_GetDocumentNameDataList).

		/// \fn public virtual List<doDocumentNoName> GetDocumentNoNameByDocumentCode(string pchrDocumentCode)
		/// \brief (Call stored procedure: sp_CM_GetDocumentNoNameByDocumentCode).

		/// \fn public virtual List<tbt_DocumentReports> GetDocumentReportsList(string pDocumentNo, string pDocumentOCC, string pDocumentCode)
		/// \brief (Call stored procedure: sp_CM_GetDocumentReportsList).

		/// \fn public virtual List<tbm_DocumentTemplate> GetDocumentTemplateByDocumentCode(string pDocumentCode)
		/// \brief (Call stored procedure: sp_CM_GetDocumentTemplateByDocumentCode).

		/// \fn public virtual List<dtDocumentType> GetDocumentTypeDataList(string c_DOCUMENT_TYPE, string pObjectIDList)
		/// \brief (Call stored procedure: sp_CM_GetDocumentTypeDataList).

		/// \fn public virtual List<doEmpCodeName> GetEmpCodeName()
		/// \brief (Call stored procedure: sp_CM_GetEmpCodeName).

		/// \fn public virtual List<dtIssueListData> GetIssueList(string documentType, string documentCode, Nullable<System.DateTime> batchDate, string billingOfficeCode, Nullable<int> intMaxManagementNo)
		/// \brief (Call stored procedure: sp_CM_GetIssueList).

		/// \fn public virtual List<dtMonthYear> GetLogMonthYear()
		/// \brief (Call stored procedure: sp_CM_GetLogMonthYear).

		/// \fn public virtual List<Nullable<int>> GetMaxManagementNo(string documentType, Nullable<System.DateTime> batchDate)
		/// \brief (Call stored procedure: sp_CM_GetMaxmanagementNo).

		/// \fn public virtual List<doMiscTypeCode> GetMiscTypeCodeList(string xml0)
		/// \brief (Call stored procedure: sp_CM_GetMiscTypeCodeList).

		/// \fn public virtual List<doMisPurge> GetMisPurge(string purgeStatus)
		/// \brief (Call stored procedure: sp_CM_GetMisPurge).

		/// \fn public virtual List<Nullable<int>> GetNextIssueListNo(string nameCode)
		/// \brief (Call stored procedure: sp_CM_GetNextIssueListNo).

		/// \fn public virtual List<doRunningNo> GetNextRunningCode(string nameCode)
		/// \brief (Call stored procedure: sp_CM_GetNextRunningCode).

		/// \fn public virtual List<doPopupSubMenuList> GetPopupSubMenuList(string popupSubmenuID)
		/// \brief (Call stored procedure: sp_CM_GetPopupSubMenuList).

		/// \fn public virtual List<doReportTemplatePath> GetReportTemplatePath(string documentCode)
		/// \brief (Call stored procedure: sp_CM_GetReportTemplatePath).

		/// \fn public virtual List<doSystemConfig> GetSystemConfig(string configName)
		/// \brief (Call stored procedure: sp_CM_GetSystemConfig).

		/// \fn public virtual List<tbm_DocumentTemplate> GetTbm_DocumentTemplate(string pchvDocumentType, Nullable<bool> bReportFlag)
		/// \brief (Call stored procedure: sp_CM_GetTbm_DocumentTemplate).

		/// \fn public virtual List<tbm_Supplier> GetTbm_SupplierCode()
		/// \brief (Call stored procedure: sp_CM_GetTbm_SupplierCode).

		/// \fn public virtual List<tbs_ProductType> GetTbs_ProductType(string pchrServiceTypeCode, string pchrProductTypeCode)
		/// \brief (Call stored procedure: sp_CM_GetTbs_ProductType).

		/// \fn public virtual List<tbt_AttachFile> GetTbt_AttachFile(string relatedID, Nullable<int> attachFileID, Nullable<bool> uploadCompleteFlag)
		/// \brief (Call stored procedure: sp_CM_GetTbt_AttachFile).

		/// \fn public virtual List<dtAttachFileTemporary> GetTbt_AttachFileTemporary(string pSessionID)
		/// \brief (Call stored procedure: sp_CM_GetTbt_AttachFileTemporary).

		/// \fn public virtual List<tbt_DocumentList> GetTbt_DocumentList(string pDocumentNo, string pDocumentCode, string pDocumentOCC)
		/// \brief (Call stored procedure: sp_CM_GetTbt_DocumentList).

		/// \fn public virtual int GetTbt_EmailTemplate(string strEmailTemplateName)
		/// \brief (Call stored procedure: sp_CM_GetTbt_EmailTemplate).

		/// \fn public virtual List<dtTPL> GetTbt_Purgelog(Nullable<System.DateTime> pPurgeMonthYear)
		/// \brief (Call stored procedure: sp_CM_GetTbt_Purgelog).

		/// \fn public virtual List<dtIssueListData> GetTmpIssueListData()
		/// \brief (Call stored procedure: sp_CM_GetTmpIssueListData).

		/// \fn public virtual List<dtTotalAttachFileSize> GetTotalAttachFileSize(string relatedID)
		/// \brief (Call stored procedure: sp_CM_GetTotalAttachFileSize).

		/// \fn public virtual List<tbt_AttachFile> InsertAttachFile(string relatedID, string fileName, string fileType, Nullable<int> fileSize, string filePath, Nullable<bool> uploadCompleteFlag, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
		/// \brief (Call stored procedure: sp_CM_InsertAttachFile).

		/// \fn public virtual List<dtDocumentList> InsertDocumentList(string documentNo, string documentOCC, string documentCode, string contractCode, string oCC, string quotationTargetCode, string alphabet, string projectCode, string billingTargetCode, string instrumentCode, string contractOfficeCode, string operationOfficeCode, string billingOfficeCode, string issueOfficeCode, Nullable<System.DateTime> generateDate, Nullable<int> reportMonth, Nullable<int> reportYear, string filePath, Nullable<int> downloadCount, Nullable<int> managementNo, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy, string locationCode, Nullable<int> minManagementNo, Nullable<int> maxManagementNo)
		/// \brief (Call stored procedure: sp_CM_InsertDocumentList).

		/// \fn public virtual List<tbt_DocumentReports> InsertDocumentReports(string pDocumentNo, string pDocumentOCC, string pDocumentCode, byte[] pFileBinary, Nullable<System.DateTime> pCreateDate, string pCreateBy)
		/// \brief (Call stored procedure: sp_CM_InsertDocumentReports).

		/// \fn public virtual int InsertTbt_BatchLog(Nullable<System.DateTime> pBatchDate, string pBatchCode, string pErrorMessage, Nullable<bool> pErrorFlag, string pBatchUser)
		/// \brief (Call stored procedure: sp_CM_InsertTbt_BatchLog).

		/// \fn public virtual List<Nullable<int>> IsExistReport(string pDocumentNo, string pDocumentOCC, string pDocumentCode)
		/// \brief (Call stored procedure: sp_CM_IsExistReport).

		/// \fn public virtual List<string> IsSystemSuspending(string pcharC_CONFIG_SUSPEND_FLAG)
		/// \brief (Call stored procedure: sp_CM_IsSystemSuspending).

		/// \fn public virtual List<Nullable<int>> KeepHistory_ef(string pchrEmpNo, string pchrLogType)
		/// \brief (Call stored procedure: sp_CM_KeepHistory).

		/// \fn public virtual List<dtUserPermission> RefreshPermissionData(string pEmpNo, string xml_dtBelonging)
		/// \brief (Call stored procedure: sp_CM_RefreshPermissionData).

		/// \fn public virtual int RemoveAttachFile(Nullable<int> attachFileId, string relateId, Nullable<bool> uploadCompleteFlag)
		/// \brief (Call stored procedure: sp_CM_RemoveAttachFile).

		/// \fn public virtual int RemoveAttachFileTemporary(string pSessionID, Nullable<bool> pIsDeleteRealAttachFile)
		/// \brief (Call stored procedure: sp_CM_RemoveAttachFileTemporary).

		/// \fn public virtual List<Nullable<int>> RunBatch(string pBatchCode, string pBatchName, string pBatchDescription, string pBatchLastResult, string pBatchStatus, Nullable<int> pTotal, Nullable<int> pComplete, Nullable<int> pFailed, string pBatchJobName, Nullable<System.DateTime> pBatchDate, string pBatchUser, string pC_BATCH_STATUS_FAILED, string pC_BATCH_STATUS_PROCESSING, string pC_BATCH_STATUS_SUCCEEDED, string pC_EVENT_TYPE_INFORMATION, string pC_LOG_NIGHT_BATCH_ERROR)
		/// \brief (Call stored procedure: sp_CM_RunBatch).

		/// \fn public virtual int RunBatchAll(string pEmpNo, Nullable<System.DateTime> pBatchDate)
		/// \brief (Call stored procedure: sp_CM_RunBatchAll).

		/// \fn return context.sp_CM_GetSystemStatus().ToList();
		/// \brief (Call stored procedure: sp_CM_GetSystemStatus).

		/// \fn public virtual List<Nullable<int>> UpdateBatchResult(string vBatchCode, string vBatchStatus, string vBatchLastResult, Nullable<int> vTotal, Nullable<int> vComplete, Nullable<int> vFailed, string vBatchUser)
		/// \brief (Call stored procedure: sp_CM_UpdateBatchResultCommon).

		/// \fn public virtual int UpdateFlagAttachFile(string relatedID, string newRelatedID, string newFileName, string filePath, Nullable<int> attachFileID)
		/// \brief (Call stored procedure: sp_CM_UpdateFlagAttachFile).

		/// \fn public virtual List<tbt_DocumentList> UpdateManageNo(string xml)
		/// \brief (Call stored procedure: sp_CM_UpdateManageNo).

		/// \fn public virtual List<Nullable<int>> UpdateSystemConfig(string configName, string configValue, string empNo, string pSuspendResumeSystemJobName)
		/// \brief (Call stored procedure: sp_CM_UpdateSystemConfig).

		/// \fn public virtual List<Nullable<int>> UpdateSystemStatus(Nullable<bool> bSuspendFlag, Nullable<bool> bManualFlag, string pUpdateBy, string pSuspendResumeSystemJobName)
		/// \brief (Call stored procedure: sp_CM_UpdateSystemStatus).

		/// \fn public virtual int WriteDocumentDownloadLog(string pDocumentNo, string pDocumentCode, Nullable<System.DateTime> processDateTime, string empNo, string pDocumentOCC)
		/// \brief (Call stored procedure: sp_CM_WriteDocumentDownloadLog).

		/// \fn public virtual int WriteErrorLog(string screenID, string desc, string detail, Nullable<System.DateTime> modifiedDate, string modifiedBy)
		/// \brief (Call stored procedure: sp_CM_WriteErrorLog).

		/// \fn public virtual List<Nullable<int>> WriteTransactionLog(string gUID, string screenID, string tableName, string transactionData, Nullable<System.DateTime> createDate, string createBy)
		/// \brief (Call stored procedure: sp_CM_WriteTransactionLog).

		/// \fn public virtual int WriteWindowLog(string eventType, string strMessage)
		/// \brief (Call stored procedure: sp_CM_WriteWindowLog).


	}
}

