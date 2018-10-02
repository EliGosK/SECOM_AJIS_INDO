using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Common;
using System.IO;

using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.DataEntity.Common
{
    public interface IDocumentHandler
    {
        /// <summary>
        /// Set template path to use template with old company name.
        /// </summary>
        /// <param name="isUse"></param>
        void UseOldCompanyTemplate(bool isUse);

        /// <summary>
        /// Get data of Tbm_DocumentTemplate
        /// </summary>
        /// <param name="pchrServiceTypeCode"></param>
        /// <param name="bReportFlag"></param>
        /// <returns></returns>
        List<tbm_DocumentTemplate> GetTbm_DocumentTemplate(string pchrServiceTypeCode, bool? bReportFlag = null);
        /// <summary>
        /// Get document template data by document code
        /// </summary>
        /// <param name="pDocumentCode"></param>
        /// <returns></returns>
        List<tbm_DocumentTemplate> GetDocumentTemplateByDocumentCode(string pDocumentCode);
        /// <summary>
        /// Get document type data list
        /// </summary>
        /// <param name="C_DOCUMENT_TYPE"></param>
        /// <param name="ObjectIdList"></param>
        /// <returns></returns>
        List<dtDocumentType> GetDocumentTypeDataList(string C_DOCUMENT_TYPE, string ObjectIdList);
        /// <summary>
        /// Get document no. name by document code
        /// </summary>
        /// <param name="pchrDocumentCode"></param>
        /// <returns></returns>
        List<doDocumentNoName> GetDocumentNoNameByDocumentCode(string pchrDocumentCode);

        /// <summary>
        /// Getting document data list
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<dtDocumentData> GetDocumentDataList(doDocumentDataCondition cond, bool isCheckOfficeCode = true); //Modify (Add isCheckOfficeCode) by Jutarat A. on 11112013 

        List<dtDocumentData> GetDocumentDataListByInventorySlipNo(string inventorySlipNo);

        List<dtDocumentData> GetDocumentDataListByDocumentCode(string documentNo, string documentCode, string documentOCC);

        //---- Generate document process ----
        /// <summary>
        /// To generate document (common method)
        /// </summary>
        /// <param name="doDoc"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        Stream GenerateDocument(doDocumentDataGenerate doDoc, string owner_pwd = null);
        /// <summary>
        /// To generate document (common method)
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <param name="doCover"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        Stream GenerateDocument(doDocumentDataGenerate doMainDoc, doDocumentDataGenerate doCover, string owner_pwd = null);
        /// <summary>
        /// To generate document (common method)
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <param name="lstSlaveDoc"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        Stream GenerateDocument(doDocumentDataGenerate doMainDoc, List<doDocumentDataGenerate> lstSlaveDoc, string owner_pwd = null);
        Stream GenerateDocumentISR110(doDocumentDataGenerate doMainDoc, string owner_pwd = null);
        //---- Generate document process ----
        /// <summary>
        /// To generate document return file path (common method)
        /// </summary>
        /// <param name="doDoc"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        string GenerateDocumentFilePath(doDocumentDataGenerate doDoc, string owner_pwd = null, bool isManageDocument = false);
        /// <summary>
        /// To generate document return file path (common method)
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <param name="doCover"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        string GenerateDocumentFilePath(doDocumentDataGenerate doMainDoc, doDocumentDataGenerate doCover, string owner_pwd = null);
        /// <summary>
        /// To generate document return file path (common method)
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <param name="lstSlaveDoc"></param>
        /// <param name="owner_pwd"></param>
        /// <returns></returns>
        string GenerateDocumentFilePath(doDocumentDataGenerate doMainDoc, List<doDocumentDataGenerate> lstSlaveDoc, string owner_pwd = null, bool isReuseRptDoc = false); //Modify by Jutarat A. on 13112012 (Add isReuseRptDoc)


        //---- Generate document process ----
        /// <summary>
        /// To generate document without encrypt
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <returns></returns>
        string GenerateDocumentWithoutEncrypt(doDocumentDataGenerate doMainDoc);
        /// <summary>
        /// To generate document without encrypt
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <param name="doCover"></param>
        /// <returns></returns>
        string GenerateDocumentWithoutEncrypt(doDocumentDataGenerate doMainDoc, doDocumentDataGenerate doCover);
        /// <summary>
        /// To generate document without encrypt
        /// </summary>
        /// <param name="doMainDoc"></param>
        /// <param name="lstSlaveDoc"></param>
        /// <returns></returns>
        string GenerateDocumentWithoutEncrypt(doDocumentDataGenerate doMainDoc, List<doDocumentDataGenerate> lstSlaveDoc);

        /// <summary>
        /// To insert data to tbt_DocumentList
        /// </summary>
        /// <param name="doDoc"></param>
        /// <param name="initialDownloadCount"></param>
        void InsertDocumentList(doDocumentDataGenerate doDoc, int initialDownloadCount = 1);
        /// <summary>
        /// Get document name data list
        /// </summary>
        /// <param name="pchvDocumentType"></param>
        /// <param name="pObjectIDList"></param>
        /// <returns></returns>
        List<dtDocumentNameDataList> GetDocumentNameDataList(string pchvDocumentType, string pObjectIDList);

        /// <summary>
        /// To insert document report to tbt_DocumentReports
        /// </summary>
        /// <param name="pDocumentNo"></param>
        /// <param name="pDocumentOCC"></param>
        /// <param name="pDocumentCode"></param>
        /// <param name="pFileBinary"></param>
        /// <param name="pCreateDate"></param>
        /// <param name="pCreateBy"></param>
        /// <returns></returns>
        List<tbt_DocumentReports> InsertDocumentReports(string pDocumentNo, string pDocumentOCC, string pDocumentCode, byte[] pFileBinary, Nullable<System.DateTime> pCreateDate, string pCreateBy);
        /// <summary>
        /// Get data of tbt_DocumentReports
        /// </summary>
        /// <param name="pDocumentNo"></param>
        /// <param name="pDocumentOCC"></param>
        /// <param name="pDocumentCode"></param>
        /// <returns></returns>
        List<tbt_DocumentReports> GetDocumentReportsList(string pDocumentNo, string pDocumentOCC, string pDocumentCode);
        /// <summary>
        /// Get report file stream from document report path
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Stream GetDocumentReportFileStream(string fileName);
        /// <summary>
        /// To check report is exist in tbt_DocumentReports
        /// </summary>
        /// <param name="pDocumentNo"></param>
        /// <param name="pDocumentOCC"></param>
        /// <param name="pDocumentCode"></param>
        /// <returns></returns>
        List<Nullable<int>> IsExistReport(string pDocumentNo, string pDocumentOCC, string pDocumentCode);

        /// <summary>
        /// Get report tempalte path (.rpt)
        /// </summary>
        /// <param name="documentCode"></param>
        /// <returns></returns>
        List<doReportTemplatePath> GetReportTemplatePath(string documentCode);

        /// <summary>
        /// Get Document list for Prining by Issue date Or (Issue date and ManagementNo)
        /// </summary>
        /// <param name="c_DOCUMENT_TYPE_INCOME"></param>
        /// <param name="c_DOCUMENT_TYPE_COMMON"></param>
        /// <param name="datIssueDate"></param>
        /// <param name="intManagementNoFrom"></param>
        /// <param name="intManagementNoTo"></param>
        /// <returns></returns>
        List<dtDocumentListForPrining> GetDocumentListForPrining(string c_DOCUMENT_TYPE_INCOME, string c_DOCUMENT_TYPE_COMMON, Nullable<System.DateTime> datIssueDate, Nullable<int> intManagementNoFrom, Nullable<int> intManagementNoTo);
        
       
        /// <summary>
        /// Generate report CMR020 - Account data of carry-over & Profit  (CSV format) to the server
        /// </summary>
        /// <param name="reportYear">year of data</param>
        /// <param name="reportMonth">month of data</param>
        /// <param name="productType">product type</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <returns></returns>
        string GenerateCMR020FilePath(string reportYear, string reportMonth, string productType, string strEmpNo, DateTime dtDateTime);
        string GenerateCMR020FilePath(string reportYear, string reportMonth, string productType, string strEmpNo, DateTime dtDateTime, bool isInsertDocumentList);

        /// <summary>
        /// Generate text report document (common method)
        /// </summary>
        /// <param name="doDocument">Document's information.</param>
        /// <param name="dtGenerateDate">Document generation date.</param>
        /// <param name="strFileName">Document file name.</param>
        /// <param name="strContent">Text content for document.</param>
        /// <returns></returns>
        string GenerateTextReportFilePath(doDocumentDataGenerate doDocument, DateTime dtGenerateDate, string strFileName, string strContent);

        /// <summary>
        /// Clear document template cache.
        /// </summary>
        void ClearReportCache();


        string GetReIssueInvoice(string DocNo, string UpdateBy);
        string GetReIssueTaxInvoice(string DocNo, string UpdateBy);
        string GetReIssueCreditNote(string DocNo, string UpdateBy);
        string GetReIssueReceipt(string DocNo, string UpdateBy);

    }
}
