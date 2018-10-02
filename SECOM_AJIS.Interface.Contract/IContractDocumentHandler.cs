using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IContractDocumentHandler
    {
        /// <summary>
        /// To generate contract document occurrence
        /// </summary>
        /// <param name="strCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        string GenerateDocOCC(string strCode, string strOCC);

        /// <summary>
        /// To get contract document header data
        /// </summary>
        /// <param name="pchrQuotationTargetCode"></param>
        /// <param name="pchrAlphabet"></param>
        /// <param name="pchrContractDocOCC"></param>
        /// <returns></returns>
        List<tbt_ContractDocument> GetContractDocHeaderByQuotationCode(string pchrQuotationTargetCode, string pchrAlphabet, string pchrContractDocOCC);

        /// <summary>
        /// To get contract document header data
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <param name="pchrContractDocOCC"></param>
        /// <returns></returns>
        List<tbt_ContractDocument> GetContractDocHeaderByContractCode(string pContractCode, string pOCC, string pchrContractDocOCC);

        /// <summary>
        /// Insert contract document
        /// </summary>
        /// <param name="docLst"></param>
        /// <returns></returns>
        List<tbt_ContractDocument> InsertTbt_ContractDocument(List<tbt_ContractDocument> docLst);

        /// <summary>
        /// Insert document contract report
        /// </summary>
        /// <param name="docLst"></param>
        /// <returns></returns>
        List<tbt_DocContractReport> InsertTbt_DocContractReport(List<tbt_DocContractReport> docLst);

        /// <summary>
        /// Create report for contract document
        /// </summary>
        /// <param name="iDocID"></param>
        /// <param name="strDocNo"></param>
        /// <param name="strDocumentCode"></param>
        /// <returns></returns>
        Stream CreateContractReport(int? iDocID, string strDocNo, string strDocumentCode);

        /// <summary>
        /// Update contract document
        /// </summary>
        /// <param name="docLst"></param>
        /// <returns></returns>
        List<tbt_ContractDocument> UpdateTbt_ContractDocument(List<tbt_ContractDocument> docLst);

        /// <summary>
        /// Update document status
        /// </summary>
        /// <param name="strDocStatus"></param>
        /// <param name="iDocID"></param>
        /// <param name="dLastUpdateDate"></param>
        /// <returns></returns>
        List<tbt_ContractDocument> UpdateDocumentStatus(string strDocStatus, int? iDocID, DateTime? dLastUpdateDate);

        /// <summary>
        /// Get change notice report data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<RPTChangeNoticeDo> GetRptChangeNoticeData(int iDocID);

        /// <summary>
        /// Get change fee memorandum data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<RPTChangeMemoDo> GetRptChangeMemoData(int iDocID);

        /// <summary>
        /// Get contract report data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<RPTContractReportDo> GetRptContractReportData(int iDocID);

        /// <summary>
        /// Get Start/Resume memorandum data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<RPTStartResumeMemoDo> GetRptStartResumeMemoData(int iDocID);

        /// <summary>
        /// Get confirm current instrument memorandum data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<RPTConfirmCurrInstMemoDo> GetRptConfirmCurrentInstrumentMemoData(int iDocID);

        /// <summary>
        /// Get cancel contract memorandum data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<RPTCancelContractMemoDo> GetRptCancelContractMemoData(int iDocID);

        /// <summary>
        /// Get Cancel contract memo detail
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<RPTCancelContractMemoDetailDo> GetRptCancelContractMemoDetail(int iDocID);

        /// <summary>
        /// Get change fee memorandum data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<RPTChangeFeeMemoDo> GetRptChangeFeeMemoData(int iDocID);

        /// <summary>
        /// Get contract cover letter data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<RPTCoverLetterDo> GetRptCoverLetterData(int iDocID);

        /// <summary>
        /// Get instrument detail data
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<RPTInstrumentDetailDo> GetRptInstrumentDetailData(int iDocID);

        /// <summary>
        /// Getting contract document data for issue
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strQuotationTargetCode"></param>
        /// <param name="strOccAlphabet"></param>
        /// <returns></returns>
        List<dtContractDoc> GetContractDocDataList(string strContractCode, string strQuotationTargetCode, string strOccAlphabet);

        /// <summary>
        /// Get contract document data for issue
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strQuotationTargetCode"></param>
        /// <param name="strOccAlphabet"></param>
        /// <returns></returns>
        dsContractDocForIssue GetContractDocForIssue(string strContractCode, string strQuotationTargetCode, string strOccAlphabet);

        /// <summary>
        /// To search contract document data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<dtContractDocumentList> SearchContractDocument(doSearchContractDocCondition cond);
        
        /// <summary>
        /// Get contract document
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<tbt_ContractDocument> GetTbt_ContractDocument(int? iDocID);

        /// <summary>
        /// Get contract document template
        /// </summary>
        /// <param name="strDocumentCode"></param>
        /// <returns></returns>
        List<tbs_ContractDocTemplate> GetTbs_ContractDocTemplate(string strDocumentCode);

        /// <summary>
        /// Get document contract report
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<tbt_DocContractReport> GetTbt_DocContractReport(int? iDocID);

        /// <summary>
        /// Get document change memo
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<tbt_DocChangeMemo> GetTbt_DocChangeMemo(int? iDocID);

        /// <summary>
        /// Get document change notice
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<tbt_DocChangeNotice> GetTbt_DocChangeNotice(int? iDocID);

        /// <summary>
        /// Get document confirm currend instrument memo
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<tbt_DocConfirmCurrentInstrumentMemo> GetTbt_DocConfirmCurrentInstrumentMemo(int? iDocID);

        /// <summary>
        /// Get document cancel contract memo
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<tbt_DocCancelContractMemo> GetTbt_DocCancelContractMemo(int? iDocID);

        /// <summary>
        /// Get document cancel contract memo detail
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<tbt_DocCancelContractMemoDetail> GetTbt_DocCancelContractMemoDetail(int? iDocID);

        /// <summary>
        /// Get document change fee memo
        /// </summary>
        /// <param name="iDocID"></param>
        /// <returns></returns>
        List<tbt_DocChangeFeeMemo> GetTbt_DocChangeFeeMemo(int? iDocID);

        /// <summary>
        /// To contract document status to be not used
        /// </summary>
        /// <param name="pContractDoc"></param>
        /// <param name="pOCC"></param>
        /// <param name="pC_CONTRACT_DOC_STATUS_NOT_USED"></param>
        /// <param name="pC_CONTRACT_DOC_STATUS_COLLECTED"></param>
        /// <param name="pIsRecursive"></param>
        /// <param name="pRef"></param>
        /// <returns></returns>
        int SetNotUsedStatus(string pContractDoc, string pOCC, string pC_CONTRACT_DOC_STATUS_NOT_USED, string pC_CONTRACT_DOC_STATUS_COLLECTED, Nullable<bool> pIsRecursive, string pRef);

        /// <summary>
        /// To check existing in contract document
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strQuotationTargetCode"></param>
        /// <param name="strOCCAlphabet"></param>
        /// <param name="strDocOCC"></param>
        /// <returns></returns>
        bool IsContractDocExist(string strContractCode, string strQuotationTargetCode, string strOCCAlphabet, string strDocOCC);
        
    }
}
