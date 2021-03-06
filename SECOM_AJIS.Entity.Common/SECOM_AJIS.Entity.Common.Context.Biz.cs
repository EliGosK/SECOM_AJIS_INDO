//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SECOM_AJIS.DataEntity.Common
{
    public partial class BizCMDataEntities
    {
        #region Methods
    		public virtual List<tbs_ProductType> GetTbs_ProductType(string pchrServiceTypeCode, string pchrProductTypeCode)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTbs_ProductType(pchrServiceTypeCode, pchrProductTypeCode).ToList();
    		}
    		public virtual List<dtDocumentType> GetDocumentTypeDataList(string c_DOCUMENT_TYPE, string pObjectIDList)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetDocumentTypeDataList(c_DOCUMENT_TYPE, pObjectIDList).ToList();
    		}
    		public virtual List<Nullable<int>> WriteTransactionLog(string gUID, string screenID, string tableName, string transactionData, Nullable<System.DateTime> createDate, string createBy)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.WriteTransactionLog(gUID, screenID, tableName, transactionData, createDate, createBy).ToList();
    		}
    		public virtual List<Nullable<int>> UpdateSystemStatus(Nullable<bool> bSuspendFlag, Nullable<bool> bManualFlag, string pUpdateBy, string pSuspendResumeSystemJobName)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.UpdateSystemStatus(bSuspendFlag, bManualFlag, pUpdateBy, pSuspendResumeSystemJobName).ToList();
    		}
    		public virtual List<doSystemStatus> CM_GetSystemStatus()
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.sp_CM_GetSystemStatus().ToList();
    		}
    		public virtual List<tbt_PurgeLog> DeleteLog(Nullable<System.DateTime> monthYear, string empNo)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.DeleteLog(monthYear, empNo).ToList();
    		}
    		public virtual List<dtDocumentData> GetDocumentDataList(string pchvDocumentType, string pchvDocumentCode, Nullable<System.DateTime> pdatGenerateDateFrom, Nullable<System.DateTime> pdatGenerateDateTo, Nullable<int> pdatMonth, Nullable<int> pdatYear, string pchrContractOfficeCode, string pchrOperationOfficeCode, string pchrBillingOfficeCode, string pchrIssueOfficeCode, string pchrDocumentNo, string pchrQuotationTargetCode, string pchrAlphabet, string pchrProjectCode, string pchrContractCode, string pchrOCC, string pchrBillingTargetCode, string pchvInstrumentCode, string pC_DOCUMENT_TYPE_CONTRACT, string pC_DOCUMENT_TYPE_MA, string pC_DOCUMENT_TYPE_INSTALLATION, string pC_DOCUMENT_TYPE_INVENTORY, string pC_DOCUMENT_TYPE_INCOME, string pOfficeCodeList, string pC_DOCUMENT_TYPE_COMMON, string pLocationCode)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetDocumentDataList(pchvDocumentType, pchvDocumentCode, pdatGenerateDateFrom, pdatGenerateDateTo, pdatMonth, pdatYear, pchrContractOfficeCode, pchrOperationOfficeCode, pchrBillingOfficeCode, pchrIssueOfficeCode, pchrDocumentNo, pchrQuotationTargetCode, pchrAlphabet, pchrProjectCode, pchrContractCode, pchrOCC, pchrBillingTargetCode, pchvInstrumentCode, pC_DOCUMENT_TYPE_CONTRACT, pC_DOCUMENT_TYPE_MA, pC_DOCUMENT_TYPE_INSTALLATION, pC_DOCUMENT_TYPE_INVENTORY, pC_DOCUMENT_TYPE_INCOME, pOfficeCodeList, pC_DOCUMENT_TYPE_COMMON, pLocationCode).ToList();
    		}
    		public virtual List<Nullable<int>> UpdateSystemConfig(string configName, string configValue, string empNo, string pSuspendResumeSystemJobName)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.UpdateSystemConfig(configName, configValue, empNo, pSuspendResumeSystemJobName).ToList();
    		}
    		public virtual int WriteWindowLog(string eventType, string strMessage)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.WriteWindowLog(eventType, strMessage);
    		}
    		public virtual int WriteErrorLog(string screenID, string desc, string detail, Nullable<System.DateTime> modifiedDate, string modifiedBy)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.WriteErrorLog(screenID, desc, detail, modifiedDate, modifiedBy);
    		}
    		public virtual int WriteDocumentDownloadLog(string pDocumentNo, string pDocumentCode, Nullable<System.DateTime> processDateTime, string empNo, string pDocumentOCC)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.WriteDocumentDownloadLog(pDocumentNo, pDocumentCode, processDateTime, empNo, pDocumentOCC);
    		}
    		public virtual List<doMiscTypeCode> GetMiscTypeCodeList(string xml0)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetMiscTypeCodeList(xml0).ToList();
    		}
    		public virtual List<tbm_DocumentTemplate> GetTbm_DocumentTemplate(string pchvDocumentType, Nullable<bool> bReportFlag)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTbm_DocumentTemplate(pchvDocumentType, bReportFlag).ToList();
    		}
    		public virtual List<tbm_Supplier> GetTbm_SupplierCode()
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTbm_SupplierCode().ToList();
    		}
    		public virtual List<doReportTemplatePath> GetReportTemplatePath(string documentCode)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetReportTemplatePath(documentCode).ToList();
    		}
    		public virtual List<doRunningNo> GetNextRunningCode(string nameCode, Nullable<bool> isLockRow)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetNextRunningCode(nameCode, isLockRow).ToList();
    		}
    		public virtual List<doSystemConfig> GetSystemConfig(string configName)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetSystemConfig(configName).ToList();
    		}
    		public virtual List<dtDocumentList> InsertDocumentList(string documentNo, string documentOCC, string documentCode, string contractCode, string oCC, string quotationTargetCode, string alphabet, string projectCode, string billingTargetCode, string instrumentCode, string contractOfficeCode, string operationOfficeCode, string billingOfficeCode, string issueOfficeCode, Nullable<System.DateTime> generateDate, Nullable<int> reportMonth, Nullable<int> reportYear, string filePath, Nullable<int> downloadCount, Nullable<int> managementNo, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy, string locationCode, Nullable<int> minManagementNo, Nullable<int> maxManagementNo)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.InsertDocumentList(documentNo, documentOCC, documentCode, contractCode, oCC, quotationTargetCode, alphabet, projectCode, billingTargetCode, instrumentCode, contractOfficeCode, operationOfficeCode, billingOfficeCode, issueOfficeCode, generateDate, reportMonth, reportYear, filePath, downloadCount, managementNo, createDate, createBy, updateDate, updateBy, locationCode, minManagementNo, maxManagementNo).ToList();
    		}
    		public virtual List<doDocumentNoName> GetDocumentNoNameByDocumentCode(string pchrDocumentCode)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetDocumentNoNameByDocumentCode(pchrDocumentCode).ToList();
    		}
    		public virtual List<doOperationType> GetOperationTypeList(string pcharC_OPERATION_TYPE)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetOperationTypeList(pcharC_OPERATION_TYPE).ToList();
    		}
    		public virtual List<dtBatchProcess> GetBatchProcessDataList(string pC_CONFIG_SUSPEND_FLAG, string pC_BATCH_STATUS, string pC_BATCH_LAST_RESULT, string pC_BATCH_STATUS_PROCESSING, Nullable<bool> pC_FLAG_ON, Nullable<bool> pC_FLAG_OFF)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetBatchProcessDataList(pC_CONFIG_SUSPEND_FLAG, pC_BATCH_STATUS, pC_BATCH_LAST_RESULT, pC_BATCH_STATUS_PROCESSING, pC_FLAG_ON, pC_FLAG_OFF).ToList();
    		}
    		public virtual List<Nullable<int>> RunBatch(string pBatchCode, string pBatchName, string pBatchDescription, string pBatchLastResult, string pBatchStatus, Nullable<int> pTotal, Nullable<int> pComplete, Nullable<int> pFailed, string pBatchJobName, Nullable<System.DateTime> pBatchDate, string pBatchUser, string pC_BATCH_STATUS_FAILED, string pC_BATCH_STATUS_PROCESSING, string pC_BATCH_STATUS_SUCCEEDED, string pC_EVENT_TYPE_INFORMATION, string pC_LOG_NIGHT_BATCH_ERROR)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.RunBatch(pBatchCode, pBatchName, pBatchDescription, pBatchLastResult, pBatchStatus, pTotal, pComplete, pFailed, pBatchJobName, pBatchDate, pBatchUser, pC_BATCH_STATUS_FAILED, pC_BATCH_STATUS_PROCESSING, pC_BATCH_STATUS_SUCCEEDED, pC_EVENT_TYPE_INFORMATION, pC_LOG_NIGHT_BATCH_ERROR).ToList();
    		}
    		public virtual List<string> IsSystemSuspending(string pcharC_CONFIG_SUSPEND_FLAG)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.IsSystemSuspending(pcharC_CONFIG_SUSPEND_FLAG).ToList();
    		}
    		public virtual List<Nullable<int>> KeepHistory_ef(string pchrEmpNo, string pchrLogType)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.KeepHistory_ef(pchrEmpNo, pchrLogType).ToList();
    		}
    		public virtual List<dtMonthYear> GetLogMonthYear()
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetLogMonthYear().ToList();
    		}
    		public virtual List<doMisPurge> GetMisPurge(string purgeStatus)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetMisPurge(purgeStatus).ToList();
    		}
    		public virtual List<dtTPL> GetTbt_Purgelog(Nullable<System.DateTime> pPurgeMonthYear)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTbt_Purgelog(pPurgeMonthYear).ToList();
    		}
    		public virtual List<dtUserPermission> RefreshPermissionData(string pEmpNo, string xml_dtBelonging)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.RefreshPermissionData(pEmpNo, xml_dtBelonging).ToList();
    		}
    		public virtual List<dtDocumentNameDataList> GetDocumentNameDataList(string pchvDocumentType, string pObjectIDList)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetDocumentNameDataList(pchvDocumentType, pObjectIDList).ToList();
    		}
    		public virtual List<doEmpCodeName> GetEmpCodeName()
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetEmpCodeName().ToList();
    		}
    		public virtual int DeleteAttachFileByID(Nullable<int> attachFileID)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.DeleteAttachFileByID(attachFileID);
    		}
    		public virtual int DeleteAttachFileByRelatedID(string relatedID)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.DeleteAttachFileByRelatedID(relatedID);
    		}
    		public virtual List<tbt_AttachFile> GetAttachFile(string relatedID)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetAttachFile(relatedID).ToList();
    		}
    		public virtual List<tbt_AttachFile> InsertAttachFile(string relatedID, string fileName, string fileType, Nullable<int> fileSize, string filePath, Nullable<bool> uploadCompleteFlag, Nullable<System.DateTime> createDate, string createBy, Nullable<System.DateTime> updateDate, string updateBy)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.InsertAttachFile(relatedID, fileName, fileType, fileSize, filePath, uploadCompleteFlag, createDate, createBy, updateDate, updateBy).ToList();
    		}
    		public virtual List<dtTotalAttachFileSize> GetTotalAttachFileSize(string relatedID)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTotalAttachFileSize(relatedID).ToList();
    		}
    		public virtual int UpdateFlagAttachFile(string relatedID, string newRelatedID, string newFileName, string filePath, Nullable<int> attachFileID)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.UpdateFlagAttachFile(relatedID, newRelatedID, newFileName, filePath, attachFileID);
    		}
    		public virtual List<tbt_DocumentReports> GetDocumentReportsList(string pDocumentNo, string pDocumentOCC, string pDocumentCode)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetDocumentReportsList(pDocumentNo, pDocumentOCC, pDocumentCode).ToList();
    		}
    		public virtual List<tbt_DocumentReports> InsertDocumentReports(string pDocumentNo, string pDocumentOCC, string pDocumentCode, byte[] pFileBinary, Nullable<System.DateTime> pCreateDate, string pCreateBy)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.InsertDocumentReports(pDocumentNo, pDocumentOCC, pDocumentCode, pFileBinary, pCreateDate, pCreateBy).ToList();
    		}
    		public virtual List<Nullable<int>> IsExistReport(string pDocumentNo, string pDocumentOCC, string pDocumentCode)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.IsExistReport(pDocumentNo, pDocumentOCC, pDocumentCode).ToList();
    		}
    		public virtual int RunBatchAll(string pEmpNo, Nullable<System.DateTime> pBatchDate)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.RunBatchAll(pEmpNo, pBatchDate);
    		}
    		public virtual int RemoveAttachFile(Nullable<int> attachFileId, string relateId, Nullable<bool> uploadCompleteFlag)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.RemoveAttachFile(attachFileId, relateId, uploadCompleteFlag);
    		}
    		public virtual List<dtAttachFileNameID> GetAttachFileName(string relatedID, Nullable<int> attachFileID, Nullable<bool> uploadCompleteFlag)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetAttachFileName(relatedID, attachFileID, uploadCompleteFlag).ToList();
    		}
    		public virtual List<tbt_AttachFile> GetTbt_AttachFile(string relatedID, Nullable<int> attachFileID, Nullable<bool> uploadCompleteFlag)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTbt_AttachFile(relatedID, attachFileID, uploadCompleteFlag).ToList();
    		}
    		public virtual int GetTbt_EmailTemplate(string strEmailTemplateName)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTbt_EmailTemplate(strEmailTemplateName);
    		}
    		public virtual List<tbt_DocumentList> GetTbt_DocumentList(string pDocumentNo, string pDocumentCode, string pDocumentOCC)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTbt_DocumentList(pDocumentNo, pDocumentCode, pDocumentOCC).ToList();
    		}
    		public virtual List<tbt_AttachFile> CopyAttachFile(string relatedID, string newRelatedID, string module)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.CopyAttachFile(relatedID, newRelatedID, module).ToList();
    		}
    		public virtual List<tbm_DocumentTemplate> GetDocumentTemplateByDocumentCode(string pDocumentCode)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetDocumentTemplateByDocumentCode(pDocumentCode).ToList();
    		}
    		public virtual List<dtAttachFileTemporary> InsertTbt_AttachFileTemporary(Nullable<int> pAttachFileID, string pRelatedID, string pFileName, string pFileType, Nullable<int> pFileSize, string pFilePath, Nullable<bool> pUploadCompleteFlag, Nullable<System.DateTime> pCreateDate, string pCreateBy, Nullable<System.DateTime> pUpdateDate, string pUpdateBy, string pSessionID)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.InsertTbt_AttachFileTemporary(pAttachFileID, pRelatedID, pFileName, pFileType, pFileSize, pFilePath, pUploadCompleteFlag, pCreateDate, pCreateBy, pUpdateDate, pUpdateBy, pSessionID).ToList();
    		}
    		public virtual int BackUpAttachFile(Nullable<int> pAttachFileID, string pSessionID)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.BackUpAttachFile(pAttachFileID, pSessionID);
    		}
    		public virtual List<dtAttachFileTemporary> GetTbt_AttachFileTemporary(string pSessionID)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTbt_AttachFileTemporary(pSessionID).ToList();
    		}
    		public virtual int RemoveAttachFileTemporary(string pSessionID, Nullable<bool> pIsDeleteRealAttachFile)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.RemoveAttachFileTemporary(pSessionID, pIsDeleteRealAttachFile);
    		}
    		public virtual List<dtAttachFileForGridView> GetAttachFileForGridView(string pRelatedID)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetAttachFileForGridView(pRelatedID).ToList();
    		}
    		public virtual List<dtGetAccumulateFileSize> GetAccumulateFileSize(string pRelatedID, string pSession)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetAccumulateFileSize(pRelatedID, pSession).ToList();
    		}
    		public virtual List<Nullable<int>> UpdateBatchResult(string vBatchCode, string vBatchStatus, string vBatchLastResult, Nullable<int> vTotal, Nullable<int> vComplete, Nullable<int> vFailed, string vBatchUser)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.UpdateBatchResult(vBatchCode, vBatchStatus, vBatchLastResult, vTotal, vComplete, vFailed, vBatchUser).ToList();
    		}
    		public virtual List<GetBatchProcessRunAll_Result> GetBatchProcessRunAll()
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetBatchProcessRunAll().ToList();
    		}
    		public virtual int InsertTbt_BatchLog(Nullable<System.DateTime> pBatchDate, string pBatchCode, string pErrorMessage, Nullable<bool> pErrorFlag, string pBatchUser)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.InsertTbt_BatchLog(pBatchDate, pBatchCode, pErrorMessage, pErrorFlag, pBatchUser);
    		}
    		public virtual List<dtDocumentListForPrining> GetDocumentListForPrining(string c_DOCUMENT_TYPE_INCOME, string c_DOCUMENT_TYPE_COMMON, Nullable<System.DateTime> datIssueDate, Nullable<int> intManagementNoFrom, Nullable<int> intManagementNoTo)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetDocumentListForPrining(c_DOCUMENT_TYPE_INCOME, c_DOCUMENT_TYPE_COMMON, datIssueDate, intManagementNoFrom, intManagementNoTo).ToList();
    		}
    		public virtual List<dtIssueListData> GetTmpIssueListData()
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTmpIssueListData().ToList();
    		}
    		public virtual List<dtBillingOffice> GetBillingOffice(string documentType, Nullable<System.DateTime> datBatchDate)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetBillingOffice(documentType, datBatchDate).ToList();
    		}
    		public virtual List<Nullable<int>> GetMaxManagementNo(string documentType, Nullable<System.DateTime> batchDate)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetMaxManagementNo(documentType, batchDate).ToList();
    		}
    		public virtual List<dtIssueListData> GetIssueList(string documentType, string documentCode, Nullable<System.DateTime> batchDate, string billingOfficeCode, Nullable<int> intMaxManagementNo)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetIssueList(documentType, documentCode, batchDate, billingOfficeCode, intMaxManagementNo).ToList();
    		}
    		public virtual List<Nullable<int>> GetNextIssueListNo(string nameCode)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetNextIssueListNo(nameCode).ToList();
    		}
    		public virtual List<tbt_DocumentList> UpdateManageNo(string xml)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.UpdateManageNo(xml).ToList();
    		}
    		public virtual List<doPopupSubMenuList> GetPopupSubMenuList(string popupSubmenuID)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetPopupSubMenuList(popupSubmenuID).ToList();
    		}
    		public virtual List<dtAccountDataOfCarryOverAndProfit> GetAccountDataOfCarryOverAndProfit(Nullable<System.DateTime> startTargetDate, Nullable<System.DateTime> endTargetDate, Nullable<System.DateTime> fiveBusinessDate, string productTypeCode, string c_PROD_TYPE_AL, string c_PROD_TYPE_ONLINE, string c_PROD_TYPE_BE, string c_PROD_TYPE_SG, string c_PROD_TYPE_MA, string c_PROD_TYPE_RENTAL_SALE, string c_GROUP_PRODUCT_TYPE_N, string c_GROUP_PRODUCT_TYPE_SG, string c_GROUP_PRODUCT_TYPE_MA, string c_BILLING_TYPE_SERVICE, string c_RENTAL_CHANGE_TYPE)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetAccountDataOfCarryOverAndProfit(startTargetDate, endTargetDate, fiveBusinessDate, productTypeCode, c_PROD_TYPE_AL, c_PROD_TYPE_ONLINE, c_PROD_TYPE_BE, c_PROD_TYPE_SG, c_PROD_TYPE_MA, c_PROD_TYPE_RENTAL_SALE, c_GROUP_PRODUCT_TYPE_N, c_GROUP_PRODUCT_TYPE_SG, c_GROUP_PRODUCT_TYPE_MA, c_BILLING_TYPE_SERVICE, c_RENTAL_CHANGE_TYPE).ToList();
    		}
    		public virtual List<tbt_ManageCarryOverProfit> GetTbt_ManageCarryOverProfit(string reportYear, string reportMonth, string productType)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTbt_ManageCarryOverProfit(reportYear, reportMonth, productType).ToList();
    		}
    		public virtual List<dtBusinessDateForAccountDataOfCarryOverAndProfit> GetBusinessDateForAccountDataOfCarryOverAndProfitProcess(Nullable<System.DateTime> batchDate)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetBusinessDateForAccountDataOfCarryOverAndProfitProcess(batchDate).ToList();
    		}
    		public virtual List<tbt_ManageCarryOverProfit> InsertTbt_ManageCarryOverProfit(string xmlTbt_ManageCarryOverProfit)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.InsertTbt_ManageCarryOverProfit(xmlTbt_ManageCarryOverProfit).ToList();
    		}
    		public virtual List<dtRptAccountDataOfCarryOverAndProfit> GetRptAccountDataOfCarryOverAndProfit(string reportYear, string reportMonth, string productType)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetRptAccountDataOfCarryOverAndProfit(reportYear, reportMonth, productType).ToList();
    		}
    		public virtual int DeleteAttachFileTemporaryByFileName(string pSessionID, string pFileName)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.DeleteAttachFileTemporaryByFileName(pSessionID, pFileName);
    		}
    		public virtual int UpdateBillingHistoryOfManageCarryOverProfit(string reportYear, string reportMonth, string productType, Nullable<System.DateTime> updateDate, string updateBy)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.UpdateBillingHistoryOfManageCarryOverProfit(reportYear, reportMonth, productType, updateDate, updateBy);
    		}
    		public virtual List<doYearOfCarryOverProfit> GetYearOfCarryOverProfit()
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetYearOfCarryOverProfit().ToList();
    		}
    		public virtual List<doResultManageCarryOverProfitForEdit> GetManageCarryOverProfitForEdit(string reportYear, string reportMonth, string productType, string contractCode, string billingOCC, string c_CURRENCY_LOCAL, string c_CURRENCY_US)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetManageCarryOverProfitForEdit(reportYear, reportMonth, productType, contractCode, billingOCC, c_CURRENCY_LOCAL, c_CURRENCY_US).ToList();
    		}
    		public virtual int UpdateTbtManageCarryOverProfit(string reportYear, string reportMonth, string contractCode, string billingOCC, Nullable<decimal> receiveAmount, Nullable<decimal> receiveAmountUsd, string receiveAmountCurrencyType, Nullable<decimal> incomeRentalFee, Nullable<decimal> incomeRentalFeeUsd, string incomeRentalFeeCurrencyType, Nullable<decimal> accumulatedReceiveAmount, Nullable<decimal> accumulatedReceiveAmountUsd, string accumulatedReceiveAmountCurrencyType, Nullable<decimal> accumulatedUnpaid, Nullable<decimal> accumulatedUnpaidUsd, string accumulatedUnpaidCurrencyType, Nullable<decimal> incomeVat, Nullable<decimal> incomeVatUsd, string incomeVatCurrencyType, Nullable<decimal> unpaidPeriod, Nullable<System.DateTime> incomeDate, string updateBy)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.UpdateTbtManageCarryOverProfit(reportYear, reportMonth, contractCode, billingOCC, receiveAmount, receiveAmountUsd, receiveAmountCurrencyType, incomeRentalFee, incomeRentalFeeUsd, incomeRentalFeeCurrencyType, accumulatedReceiveAmount, accumulatedReceiveAmountUsd, accumulatedReceiveAmountCurrencyType, accumulatedUnpaid, accumulatedUnpaidUsd, accumulatedUnpaidCurrencyType, incomeVat, incomeVatUsd, incomeVatCurrencyType, unpaidPeriod, incomeDate, updateBy);
    		}
    		public virtual List<dtDocumentData> GetDocumentDataListByInventorySlipNo(string inventorySlipNo)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetDocumentDataListByInventorySlipNo(inventorySlipNo).ToList();
    		}
    		public virtual List<tbs_BatchQueue> GetTbs_BatchQueue(Nullable<int> runId, Nullable<System.DateTime> nextRunFrom, Nullable<System.DateTime> nextRunTo)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTbs_BatchQueue(runId, nextRunFrom, nextRunTo).ToList();
    		}
    		public virtual List<tbs_BatchQueue> UpdateTbs_BatchQueue(string xml_doTbsBatchQueue)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.UpdateTbs_BatchQueue(xml_doTbsBatchQueue).ToList();
    		}
    		public virtual List<dtDocumentData> GetDocumentDataListByDocumentCode(string documentNo, string documentCode, string documentOCC)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetDocumentDataListByDocumentCode(documentNo, documentCode, documentOCC).ToList();
    		}
    		public virtual List<dtReIssueInvoice> CM_ReIssue_Invoice(string docNo, string updateBy)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.sp_CM_ReIssue_Invoice(docNo, updateBy).ToList();
    		}
    		public virtual List<dtReIssueReceipt> CM_ReIssue_Receipt(string docNo, string updateBy)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.sp_CM_ReIssue_Receipt(docNo, updateBy).ToList();
    		}
    		public virtual List<dtReIssueCreditNote> CM_ReIssue_CreditNote(string docNo, string updateBy)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.sp_CM_ReIssue_CreditNote(docNo, updateBy).ToList();
    		}
    		public virtual List<dtReIssueTaxInvoice> CM_ReIssue_TaxInvoice(string docNo, string updateBy)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.sp_CM_ReIssue_TaxInvoice(docNo, updateBy).ToList();
    		}
    		public virtual List<sp_CM_GetTbs_DocumentOutput_Result> GetTbs_DocumentOutput(string documentCode, Nullable<int> documentCodeSeq, Nullable<System.DateTime> startDay)
    		{
    			CMDataEntities context = new CMDataEntities();
    			return context.GetTbs_DocumentOutput(documentCode, documentCodeSeq, startDay).ToList();
    		}

        #endregion

    }
}
