using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Security.Cryptography;
using System.IO;

namespace SECOM_AJIS.DataEntity.Common
{
    public interface ICommonHandler
    {
        /// <summary>
        /// Get data of Tbs_ProductType
        /// </summary>
        /// <param name="pchrServiceTypeCode"></param>
        /// <param name="pchrProductTypeCode"></param>
        /// <returns></returns>
        List<tbs_ProductType> GetTbs_ProductType(string pchrServiceTypeCode, string pchrProductTypeCode); 
        /// <summary>
        /// Get system status
        /// </summary>
        /// <returns></returns>
        List<doSystemStatus> GetSystemStatus();
        /// <summary>
        /// Get miscellaneous list
        /// </summary>
        /// <param name="misc"></param>
        /// <returns></returns>
        List<doMiscTypeCode> GetMiscTypeCodeList(List<doMiscTypeCode> misc);
        
        /// <summary>
        /// To mapping miscellaneous value
        /// </summary>
        /// <param name="miscLst"></param>
        void MiscTypeMappingList(MiscTypeMappingList miscLst);
        /// <summary>
        /// Get miscellaneous display value
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="strValueCode"></param>
        /// <returns></returns>
        string GetMiscDisplayValue(string fieldName, string strValueCode);
        /// <summary>
        /// Get miscellaneous display value
        /// </summary>
        /// <param name="listMisc"></param>
        /// <param name="fieldName"></param>
        /// <param name="strValueCode"></param>
        /// <returns></returns>
        string GetMiscDisplayValue(List<doMiscTypeCode> listMisc, string fieldName, string strValueCode);
        /// <summary>
        /// Get miscellaneous list by field name
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        List<doMiscTypeCode> GetMiscTypeCodeListByFieldName(List<string> fieldNames);

        /// <summary>
        /// To generate check digit
        /// </summary>
        /// <param name="strRunningNo"></param>
        /// <returns></returns>
        string GenerateCheckDigit(string strRunningNo);

        /// <summary>
        /// To update system status
        /// </summary>
        /// <param name="bSuspendFlag"></param>
        /// <param name="bManualFlag"></param>
        /// <param name="pUpdateBy"></param>
        /// <returns></returns>
        Boolean UpdateSystemStatus(bool bSuspendFlag, bool bManualFlag, string pUpdateBy);
        /// <summary>
        /// To update system configuration
        /// </summary>
        /// <param name="ConfigName"></param>
        /// <param name="ConfigValue"></param>
        /// <returns></returns>
        bool UpdateSystemConfig(string ConfigName, string ConfigValue);
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="doEmailProc"></param>
        /// <returns></returns>
        DeliveryNotificationOptions SendMail(doEmailProcess doEmailProc);
        /// <summary>
        /// Get supplier code from tbm_Supplier
        /// </summary>
        /// <returns></returns>
        List<tbm_Supplier> GetTbm_SupplierCode();

        /// <summary>
        /// Get next running no.
        /// </summary>
        /// <param name="nameCode"></param>
        /// <returns></returns>
        List<doRunningNo> GetNextRunningCode(string nameCode, bool? isLockRow = false);

        /// <summary>
        /// Get current system status
        /// </summary>
        /// <param name="strConfigName"></param>
        /// <returns></returns>
        string GetSystemStatusValue(string strConfigName);
        /// <summary>
        /// Get operation type list
        /// </summary>
        /// <param name="pcharC_OPERATION_TYPE"></param>
        /// <returns></returns>
        List<doOperationType> GetOperationTypeList(string pcharC_OPERATION_TYPE);
        
        /// <summary>
        /// To check system status is suspend.
        /// </summary>
        /// <returns></returns>
        bool IsSystemSuspending();

        /// <summary>
        /// Get employee code-name list
        /// </summary>
        /// <returns></returns>
        List<doEmpCodeName> GetEmployeeCodeNameList();
        /// <summary>
        /// Get employee display name
        /// </summary>
        /// <param name="lsEmployee"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        string GetEmplyeeDisplayCodeName(List<doEmpCodeName> lsEmployee, string empNo);
        /// <summary>
        /// Get data of Tbt_DocumentList
        /// </summary>
        /// <param name="strDocNo"></param>
        /// <param name="strDocumentCode"></param>
        /// <param name="strDocumentOCC"></param>
        /// <returns></returns>
        List<tbt_DocumentList> GetTbt_DocumentList(string strDocNo, string strDocumentCode, string strDocumentOCC = null);
       /// <summary>
       /// Get system configuration value by configName
       /// </summary>
       /// <param name="configName"></param>
       /// <returns></returns>
        List<doSystemConfig> GetSystemConfig(string configName);

        //List<tbs_EmailTemplate> GetTbt_EmailTemplate(string strEmailTemplateName);


        // --------------------------------------------------------------------
        // Attach File Process (After Revise Version.)
        // Modified By: Narupon W., Natthavat S.
        // Revised Date: 20/02/2012
        // --------------------------------------------------------------------

        // General Method

        /// <summary>
        /// To validate attach file property
        /// </summary>
        /// <param name="fullDocumentName"></param>
        /// <param name="fileSize"></param>
        /// <param name="fileType"></param>
        /// <param name="relatedID"></param>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        bool CanAttachFile(string fullDocumentName, int fileSize, string fileType, string relatedID, string sessionID = null);
        /// <summary>
        /// Get Attach file list for display on grid
        /// </summary>
        /// <param name="RelatedID"></param>
        /// <returns></returns>
        List<dtAttachFileForGridView> GetAttachFileForGridView(string RelatedID);
        /// <summary>
        /// Get attach file stream for download
        /// </summary>
        /// <param name="AttachFileID"></param>
        /// <param name="SessionId"></param>
        /// <returns></returns>
        Stream GetAttachFileForDownload(int AttachFileID, string SessionId);
        /// <summary>
        /// Get data of Tbt_AttachFile
        /// </summary>
        /// <param name="relatedID"></param>
        /// <param name="attachFileID"></param>
        /// <param name="uploadCompleteFlag"></param>
        /// <returns></returns>
        List<tbt_AttachFile> GetTbt_AttachFile(string relatedID, Nullable<int> attachFileID, Nullable<bool> uploadCompleteFlag);

        /// <summary>
        /// Get attach file data by relateID
        /// </summary>
        /// <param name="relatedID"></param>
        /// <returns></returns>
        List<tbt_AttachFile> GetAttachFile(string relatedID);
        //List<dtGetAccumulateFileSize> GetAccumulateFileSize(string pRelatedID, string pSession);

        /// <summary>
        /// Get temporary attach file path
        /// </summary>
        /// <returns></returns>
        string GetTemporaryAttachFilePath();

        // Transaction Method

        /// <summary>
        /// To insert attach data to system (tbt_AttachFile)
        /// </summary>
        /// <param name="relatedID"></param>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <param name="fileSize"></param>
        /// <param name="fileBinary"></param>
        /// <param name="uploadCompleteFlag"></param>
        /// <returns></returns>
        List<tbt_AttachFile> InsertAttachFile(string relatedID, string fileName, string fileType, Nullable<int> fileSize, byte[] fileBinary, Nullable<bool> uploadCompleteFlag);
        /// <summary>
        /// Copy attach file
        /// </summary>
        /// <param name="module"></param>
        /// <param name="relatedID"></param>
        /// <param name="newRelatedID"></param>
        /// <param name="deleteSource"></param>
        /// <returns></returns>
        List<tbt_AttachFile> CopyAttachFile(string module, string relatedID, string newRelatedID, bool deleteSource = false);
        /// <summary>
        /// Delete attch file by ID
        /// </summary>
        /// <param name="attachFileID"></param>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        int DeleteAttachFileByID(Nullable<int> attachFileID, string sessionID);
        /// <summary>
        /// Delete actual attach file by ID
        /// </summary>
        /// <param name="attachFileID"></param>
        /// <returns></returns>
        int DeleteActualAttachFileByID(Nullable<int> attachFileID);
        /// <summary>
        /// Clear temporary uploaded file
        /// </summary>
        /// <param name="relatedID"></param>
        /// <returns></returns>
        int ClearTemporaryUploadFile(string relatedID);
        /// <summary>
        /// To update UpdateCompleteFlag (tbt_AttachFile)
        /// </summary>
        /// <param name="module"></param>
        /// <param name="relatedID"></param>
        /// <param name="newRelatedID"></param>
        /// <returns></returns>
        int UpdateFlagAttachFile(string module, string relatedID, string newRelatedID);

        //Test CMR010
        List<dtIssueListData> GetTmpIssueListData();
        // UnUsed
        //List<dtTotalAttachFileSize> GetTotalAttachFileSize(string relatedID);
        //int RemoveAttachFile(string module, Nullable<int> attachFileId, string relateId, Nu$llable<bool> uploadCompleteFlag);
        //List<dtAttachFileNameID> GetAttachFileName(string relatedID, Nullable<int> attachFileID, Nullable<bool> uploadCompleteFlag);
        //Stream GetAttachFile(string module, string relatedID, Nullable<int> attachFileID);
        //int DeleteAttachFileByRelatedID(string relatedID);

        /// <summary>
        /// Getting popup submenu list (sp_CM_GetPopupSubMenuList)
        /// </summary>
        /// <param name="popupSubmenuID"></param>
        /// <returns></returns>
        List<doPopupSubMenuList> GetPopupSubMenuList(string popupSubmenuID);


        /// <summary>
        /// Get account data of carry-over & Profit (sp_CM_GetAccountDataOfCarryOverAndProfit)
        /// </summary>
        /// <param name="StartTargetDate">start target date</param>
        /// <param name="EndTargetDate">end target date</param>
        /// <param name="FiveBusinessDate">five business date</param>
        /// <param name="ProductTypeCode">product type code</param>
        /// <returns></returns>
        List<dtAccountDataOfCarryOverAndProfit> GetAccountDataOfCarryOverAndProfit(DateTime StartTargetDate, DateTime EndTargetDate, DateTime FiveBusinessDate, string ProductTypeCode);

        /// <summary>
        /// Insert manage account data of carry-over & profit information to the system (sp_CM_InsertTbt_ManageCarryOverProfit)
        /// </summary>
        /// <param name="xmlTbt_ManageCarryOverProfit"></param>
        /// <returns></returns>
        List<tbt_ManageCarryOverProfit> InsertTbt_ManageCarryOverProfit(string xmlTbt_ManageCarryOverProfit);

        void UpdateBillingHistoryOfManageCarryOverProfit(string reportYear, string reportMonth, string productType, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Get manage account data of carry-over & Profit Process (sp_CM_GetTbt_ManageCarryOverProfit)
        /// </summary>
        /// <param name="reportYear">report year</param>
        /// <param name="reportMonth">report month</param>
        /// <returns></returns>
        List<tbt_ManageCarryOverProfit> GetTbt_ManageCarryOverProfit(string reportYear, string reportMonth);
        /// <summary>
        /// Get manage account data of carry-over & Profit Process (sp_CM_GetTbt_ManageCarryOverProfit)
        /// </summary>
        /// <param name="reportYear">report year</param>
        /// <param name="reportMonth">report month</param>
        /// <param name="productType">product type</param>
        /// <returns></returns>
        List<tbt_ManageCarryOverProfit> GetTbt_ManageCarryOverProfit(string reportYear, string reportMonth,string productType);
        /// <summary>
        /// Delete attach file temporary
        /// </summary>
        /// <param name="strSessionID"></param>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        bool DeleteAttachFileTemporaryByFileName(string strSessionID, string strFileName);

        /// <summary>
        /// Validate and allocate Printing process
        /// </summary>
        /// <param name="strPrintingFlag"></param>
        /// <returns></returns>
        bool AllocatePrintingProcess(string strPrintingFlag, string strScreenID, ref string strErrorMessage); //Add by Jutarat A. on 17092013

        /// <summary>
        /// Reset Printing process (set PrintingFlag = C_PRINTING_FLAG_NO_PRINT)
        /// </summary>
        /// <returns></returns>
        bool ResetPrintingProcess(string strScreenID); //Add by Jutarat A. on 17092013

        /// <summary>
        /// Printing PDF
        /// </summary>
        /// <param name="strPathFilename"></param>
        void PrintPDF(string strPathFilename); //Add by Jutarat A. on 17092013

        /// <summary>
        /// Get Year of Carry over and profit (Use by CMS480)
        /// </summary>
        /// <returns></returns>
        List<doYearOfCarryOverProfit> GetYearOfCarryOverProfit();

        /// <summary>
        /// GetManageCarryOverProfitForEdit
        /// </summary>
        /// <param name="reportYear"></param>
        /// <param name="reportMonth"></param>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<doResultManageCarryOverProfitForEdit> GetManageCarryOverProfitForEdit(string reportYear, string reportMonth, string productType, string contractCode, string billingOCC);

        /// <summary>
        /// UpdateTbtManageCarryOverProfit
        /// </summary>
        /// <param name="reportYear"></param>
        /// <param name="reportMonth"></param>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="receiveAmount"></param>
        /// <param name="incomeRentalFee"></param>
        /// <param name="accumulatedReceiveAmount"></param>
        /// <param name="accumulatedUnpaid"></param>
        /// <param name="incomeVat"></param>
        /// <param name="unpaidPeriod"></param>
        /// <param name="incomeDate"></param>
        /// <param name="updateBy"></param>
        /// <returns></returns>
        int UpdateTbtManageCarryOverProfit(string reportYear, string reportMonth, string contractCode, string billingOCC, Nullable<decimal> receiveAmount, Nullable<decimal> receiveAmountUsd, string receiveAmountCurrencyType, Nullable<decimal> incomeRentalFee, Nullable<decimal> incomeRentalFeeUsd, string incomeRentalFeeCurrencyType, Nullable<decimal> accumulatedReceiveAmount, Nullable<decimal> accumulatedReceiveAmountUsd, string accumulatedReceiveAmountCurrencyType, Nullable<decimal> accumulatedUnpaid, Nullable<decimal> accumulatedUnpaidUsd, string accumulatedUnpaidCurrencyType, Nullable<decimal> incomeVat, Nullable<decimal> incomeVatUsd, string incomeVatCurrencyType, Nullable<decimal> unpaidPeriod, Nullable<System.DateTime> incomeDate, string updateBy);
        
        List<tbs_BatchQueue> GetTbs_BatchQueue(Nullable<int> runId, Nullable<System.DateTime> nextRunFrom, Nullable<System.DateTime> nextRunTo);

        List<tbs_BatchQueue> UpdateTbs_BatchQueue(string xml_doTbsBatchQueue);
        string getCurrencyName(string currencyCode, string defaultValue = "", bool isThrowException = false);
        string getCurrencyFullName(string currencyCode, string defaultValue = "", bool isThrowException = false);

        /// <summary>
        /// Get document output: For Report
        /// </summary>
        /// <param name="documentCode"></param>
        /// <param name="documentCodeSeq"></param>
        /// <param name="startDay"></param>
        /// <returns></returns>
        List<sp_CM_GetTbs_DocumentOutput_Result> GetTbs_DocumentOutput(string documentCode, Nullable<int> documentCodeSeq, Nullable<System.DateTime> startDay);


        /// <summary>
        /// ทำเพื่อ test อย่านำไปใช้
        /// </summary>
        decimal? ConvertCurrencyPrice(decimal? price, string fromCurrencyType, string toCurrencyType, DateTime TargetDate, ref double ErrorCode, decimal? defaultPrice = null);
        /// <summary>
        /// ทำเพื่อ test อย่านำไปใช้
        /// </summary>
        decimal ConvertCurrencyPrice(decimal price, string fromCurrencyType, string toCurrencyType, DateTime TargetDate, ref double ErrorCode);
    }
}
