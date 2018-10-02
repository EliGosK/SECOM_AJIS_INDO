using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SECOM_AJIS.DataEntity.Income;

namespace SECOM_AJIS.DataEntity.Income
{
    public interface IIncomeDocumentHandler
    {
        #region Get data
        /// <summary>
        /// Retrieve receipt report information of specific receipt no.
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        List<doRptReceipt> GetRptReceipt(string receiptNo);
        /// <summary>
        /// Retrieve credit note report information of specific credit note no.
        /// </summary>
        /// <param name="creditNoteNo">credit note no.</param>
        /// <returns></returns>
        List<doRptCreditNote> GetRptCreditNote(string creditNoteNo);
        #endregion

        #region PDF report stream
        /// <summary>
        /// Genereate PDF stream of ICR_010 receipt report of specific receipt no. to display on web screen.
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <returns></returns>
        Stream GenerateICR010(string receiptNo, string strEmpNo, DateTime dtDateTime);
        /// <summary>
        /// Genereate PDF stream of ICR_020 credit note report of specific credit note no. to display on web screen.
        /// </summary>
        /// <param name="creditNoteNo">credit note no.</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <returns></returns>
        Stream GenerateICR020(string creditNoteNo, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate PDF stream ICR030 Debt Tracing Notice1
        /// </summary>
        /// <param name="strDocumentNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateICR030(string strDocumentNo, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate PDF stream ICR040 Debt Tracing Notice2
        /// </summary>
        /// <param name="strDocumentNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateICR040(string strDocumentNo, string strEmpNo, DateTime dtDateTime);
        
        //Merge blr020, icr010 report
        /// <summary>
        /// Genereate PDF stream of BLR_020 tax invoice report merge with ICR_010 receipt report of specific tax invoice no., receipt no. to display on web screen.
        /// </summary>
        /// <param name="taxInvoiceNo">tax invoice no.</param>
        /// <param name="receiptNo">receipt no.</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <returns></returns>
        Stream GenerateBLR020_ICR010(string taxInvoiceNo, string receiptNo, string strEmpNo, DateTime dtDateTime);
        #endregion

        #region PDF report file
        /// <summary>
        /// Genereate PDF file of ICR_010 receipt report of specific receipt no. on shared report folder
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <param name="isMerge">is generate pdf report for merge with other pdf report</param>
        /// <returns></returns>
        string GenerateICR010FilePath(string receiptNo, string strEmpNo, DateTime dtDateTime, bool isMerge = false);
        /// <summary>
        /// Genereate PDF file of ICR_020 credit note report of specific credit note no. on shared report folder
        /// </summary>
        /// <param name="creditNoteNo">credit note no.</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <param name="isMerge">is generate pdf report for merge with other pdf report</param>
        /// <returns></returns>
        string GenerateICR020FilePath(string creditNoteNo, string strEmpNo, DateTime dtDateTime, bool isMerge = false);
        
        /// <summary>
        /// Generate PDF stream ICR030 Debt Tracing Notice1
        /// </summary>
        /// <param name="strDocumentNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateICR030FilePath(string strDocumentNo, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate PDF stream ICR040 Debt Tracing Notice2
        /// </summary>
        /// <param name="strDocumentNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateICR040FilePath(string strDocumentNo, string strEmpNo, DateTime dtDateTime);

        //Merge blr020, icr010 report
        /// <summary>
        /// Genereate PDF file of BLR_020 tax invoice report merge with ICR_010 receipt report of specific tax invoice no. and receipt no. on shared report folder
        /// </summary>
        /// <param name="taxInvoiceNo">tax invoice no.</param>
        /// <param name="receiptNo">receipt no.</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <returns></returns>
        string GenerateBLR020_ICR010FilePath(string taxInvoiceNo, string receiptNo, string strEmpNo, DateTime dtDateTime);
        #endregion

        string GenerateICS130ForAccount(List<doWHTReportForAccount> data, DateTime? period);
        string GenerateICS130ForIMS(List<doWHTReportForIMS> data, DateTime? periodFrom, DateTime? periodTo);

        string GenerateICR050Report(List<doGetICR050> data, doMatchRReport paramSearch);
    }
}
