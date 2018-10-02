using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.DataEntity.Billing
{
    public interface IBillingDocumentHandler
    {
        /// <summary>
        /// Get data for generate Invoice Report
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="c_SHOW_DUEDATE"></param>
        /// <param name="c_SHOW_DUEDATE_7"></param>
        /// <param name="c_SHOW_DUEDATE_30"></param>
        /// <returns></returns>
        List<dtRptInvoice> GetRptInvoice(string invoiceNo, string c_SHOW_DUEDATE, string c_SHOW_DUEDATE_7, string c_SHOW_DUEDATE_30,string C_SHOW_DUEDATE_14, string C_SHOW_DUEDATE_NONE);
        /// <summary>
        /// Get data for generate Tax Invoice Report
        /// </summary>
        /// <param name="taxInvoiceNo"></param>
        /// <param name="c_SHOW_DUEDATE"></param>
        /// <param name="c_SHOW_DUEDATE_7"></param>
        /// <param name="c_SHOW_DUEDATE_30"></param>
        /// <returns></returns>
        List<dtRptTaxInvoice> GetRptTaxInvoice(string taxInvoiceNo, string c_SHOW_DUEDATE, string c_SHOW_DUEDATE_7, string c_SHOW_DUEDATE_30, string C_SHOW_DUEDATE_14, string C_SHOW_DUEDATE_NONE);
       /// <summary>
        /// Get data for generate Payment Form Report
       /// </summary>
       /// <param name="invoiceNo"></param>
       /// <returns></returns>
        List<dtRptPaymentForm> GetRptPaymentForm(string invoiceNo);

        /// <summary>
        /// Generate report BLR010
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateBLR010(string strInvoiceNo, string strEmpNo, DateTime dtDateTime);
        /// <summary>
        /// Generate report BLR020
        /// </summary>
        /// <param name="strTaxInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        // Comment by Jirawat Jannet
        //Stream GenerateBLR020(string strTaxInvoiceNo, string strEmpNo, DateTime dtDateTime);
        /// <summary>
        /// Generate report BLR030
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        // Comment by Jirawat Jannet
        //Stream GenerateBLR030(string strInvoiceNo, string strEmpNo, DateTime dtDateTime);
        /// <summary>
        /// Generate report BLR010 and BLR020
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strTaxInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        Stream GenerateBLR010_BLR020(string strInvoiceNo,string strTaxInvoiceNo, string strEmpNo, DateTime dtDateTime);

        /// <summary>
        /// Generate report BLR010
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <param name="isMerge"></param>
        /// <returns></returns>
        string GenerateBLR010FilePath(string strInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false);
        /// <summary>
        /// Generate report BLR020
        /// </summary>
        /// <param name="strTaxInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <param name="isMerge"></param>
        /// <returns></returns>
        // Comment by Jirawat Jannet
        //string GenerateBLR020FilePath(string strTaxInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false);
        /// <summary>
        /// Generate report BLR030
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        // Comment by Jirawat Jannet
        //string GenerateBLR030FilePath(string strInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false);
        /// <summary>
        /// Generate report BLR010 and BLR020
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strTaxInvoiceNo"></param>
        /// <param name="strEmpNo"></param>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        string GenerateBLR010_BLR020FilePath(string strInvoiceNo, string strTaxInvoiceNo,string strEmpNo, DateTime dtDateTime);

        Stream GenerateBLR020(string strInvoiceNo, string strEmpNo, DateTime dtDateTime);
        string GenerateBLR020FilePath(string strInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false);

        Stream GenerateBLR030(string strInvoiceNo, string strEmpNo, DateTime dtDateTime);
        string GenerateBLR030FilePath(string strInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false);

        Stream GenerateBLR040(string strInvoiceNo, string strEmpNo, DateTime dtDateTime);
        string GenerateBLR040FilePath(string strInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge = false);
        Stream GenerateBLR050(string strInvoiceNo, string strEmpNo, DateTime dtDateTime);
        string GenerateBLR050FilePath(string strInvoiceNo, string strEmpNo, DateTime dtDateTime, bool isUseForMerge);
    }
}
