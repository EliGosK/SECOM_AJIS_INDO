using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;

using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;

using System.Transactions;

using CSI.WindsorHelper;
using SECOM_AJIS.Common;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Billing;
using SpreadsheetLight;
using SECOM_AJIS.Presentation.Common.Service;

namespace SECOM_AJIS.DataEntity.Income
{
    public partial class IncomeDocumentHandler : BizICDataEntities, IIncomeDocumentHandler
    {
        #region PDF report stream
        /// <summary>
        /// Genereate PDF stream of ICR_010 receipt report of specific receipt no. to display on web screen.
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <returns></returns>
        public Stream GenerateICR010(string receiptNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateICR010FilePath(receiptNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);
        }
        /// <summary>
        /// Genereate PDF stream of ICR_020 credit note report of specific credit note no. to display on web screen.
        /// </summary>
        /// <param name="creditNoteNo">credit note no.</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <returns></returns>
        public Stream GenerateICR020(string creditNoteNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateICR020FilePath(creditNoteNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);
        }

        //Merge blr020, icr010 report
        /// <summary>
        /// Genereate PDF stream of BLR_020 tax invoice report merge with ICR_010 receipt report of specific tax invoice no., receipt no. to display on web screen.
        /// </summary>
        /// <param name="taxInvoiceNo">tax invoice no.</param>
        /// <param name="receiptNo">receipt no.</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <returns></returns>
        public Stream GenerateBLR020_ICR010(string taxInvoiceNo, string receiptNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateBLR020_ICR010FilePath(taxInvoiceNo, receiptNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);
        }
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
        public string GenerateICR010FilePath(string receiptNo, string strEmpNo, DateTime dtDateTime, bool isMerge = false)
        {
            int MAX_ITEM_PER_PAGE = 15;
            string strFilePath = string.Empty;
            int nSeq = 2;
          //  var doc = DocumentOutput.getDocumentData("ICR010", 1, DateTime.Now.Date);
            List<doRptReceipt> icr010 = base.GetRptReceipt(receiptNo);

            if (icr010.Count == 0)
                return null;

            if (icr010[0].BillingOfficeCode == OfficeCode.C_INV_OFFICE_INDO)
                nSeq = 1;

            var doc = DocumentOutput.getDocumentData("ICR010", nSeq, DateTime.Now.Date);

            List<List<doRptReceipt>> icr010_list = new List<List<doRptReceipt>>();
            icr010_list.Add(icr010.Clone()); // for Original customer
            icr010_list.Add(icr010.Clone()); // for Copy account 

            for (int i = 0; i < icr010_list.Count; i++)
            {
                // Calcualate page   1st  10/p , >= 2nd 10/p
                int totalPage = ((icr010_list[i].Count - 1) / MAX_ITEM_PER_PAGE) + 1;


                for (int j = 0; j < icr010_list[i].Count; j++)
                {

                    // Calcualate page   1st  10/p , >= 2nd 10/p
                    icr010_list[i][j].Page = (j / MAX_ITEM_PER_PAGE) + 1;
                    icr010_list[i][j].TotalPage = totalPage;

                    // Update Copy
                    switch (i)
                    {
                        case 0:
                            icr010_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_EN;// + "\n" + InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_TH; Comment by Jirawat jannet on 2016-11-16
                            break;
                        case 1:
                            icr010_list[i][j].RPT_InvoicePaper = InvoiceDocument.C_INVOICE_DOC_COPY_ACC_EN;// + "\n" + InvoiceDocument.C_INVOICE_DOC_COPY_ACC_TH; Comment by Jirawat jannet on 2016-11-16
                            break;
                    }


                    icr010_list[i][j].RPT_SignatureImageFullPath = doc.ImageSignaturePath;
                    icr010_list[i][j].EmpName = doc.EmpName;
                    icr010_list[i][j].EmpPosition = doc.EmpPosition;
                    icr010_list[i][j].RPT_BillingOfficeName = doc.CompanyName;
                }
            }


            // Prepare List of doDocumentDataGenerate
            List<doDocumentDataGenerate> doMainDoc = new List<doDocumentDataGenerate>();
            List<doDocumentDataGenerate> doSlaveDoc = new List<doDocumentDataGenerate>();

            for (int i = 0; i < icr010_list.Count; i++)
            {
                doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = receiptNo;
                doDocument.DocumentCode = ReportID.C_REPORT_ID_RECEIPT;
                doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                doDocument.DocumentData = icr010_list[i];
                doDocument.EmpNo = strEmpNo;
                doDocument.ProcessDateTime = dtDateTime;
                doDocument.OtherKey.BillingOffice = icr010_list[i][0].BillingOfficeCode;
                doDocument.OtherKey.BillingTargetCode = icr010_list[i][0].BillingTargetCode;

                if (i == 0)
                {
                    doMainDoc.Add(doDocument);
                }
                else
                {
                    doSlaveDoc.Add(doDocument);
                }
            }

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            if (isMerge == false)
            {
                //strFilePath = documentHandler.GenerateDocumentFilePath(doMainDoc[0], doSlaveDoc);
                strFilePath = documentHandler.GenerateDocumentFilePath(doMainDoc[0], doSlaveDoc, null, true); //Modify by Jutarat A. on 21032013 (Add isReuseRptDoc)
            }
            else
            {
                //Without encrypt for merge report
                strFilePath = documentHandler.GenerateDocumentWithoutEncrypt(doMainDoc[0], doSlaveDoc);
            }
            return strFilePath;
        }
        /// <summary>
        /// Genereate PDF file of ICR_020 credit note report of specific credit note no. on shared report folder
        /// </summary>
        /// <param name="creditNoteNo">credit note no.</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <param name="isMerge">is generate pdf report for merge with other pdf report</param>
        /// <returns></returns>
        public string GenerateICR020FilePath(string creditNoteNo, string strEmpNo, DateTime dtDateTime, bool isMerge = false)
        {
            string strFilePath = string.Empty;
            List<doRptCreditNote> icr020 = base.GetRptCreditNote(creditNoteNo);

            if (icr020.Count == 0)
                return null;

            List<List<doRptCreditNote>> icr020_list = new List<List<doRptCreditNote>>();
            icr020_list.Add(icr020.Clone()); // for Original customer
            icr020_list.Add(icr020.Clone()); // for Copy customer
            icr020_list.Add(icr020.Clone()); // for Copy account 
            icr020_list.Add(icr020.Clone()); // for Copy account

            for (int i = 0; i < icr020_list.Count; i++)
            {
                for (int j = 0; j < icr020_list[i].Count; j++)
                {
                    // Calcualate page   1st  12/p , >= 2nd 20/p
                    icr020_list[i][j].Page = (j + 8) / 20;

                    // Update Copy
                    switch (i)
                    {
                        case 0:
                            icr020_list[i][j].RPT_CreditNotePaper = InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_EN + "\n" + InvoiceDocument.C_INVOICE_DOC_ORIGINAL_CUST_TH;
                            break;
                        case 1:
                            icr020_list[i][j].RPT_CreditNotePaper = InvoiceDocument.C_INVOICE_DOC_COPY_CUST_EN + "\n" + InvoiceDocument.C_INVOICE_DOC_COPY_CUST_TH;
                            break;
                        case 2:
                            icr020_list[i][j].RPT_CreditNotePaper = InvoiceDocument.C_INVOICE_DOC_COPY_ACC_EN + "\n" + InvoiceDocument.C_INVOICE_DOC_COPY_ACC_TH;
                            break;
                        case 3:
                            icr020_list[i][j].RPT_CreditNotePaper = InvoiceDocument.C_INVOICE_DOC_COPY_ACC_EN + "\n" + InvoiceDocument.C_INVOICE_DOC_COPY_ACC_TH;
                            break;
                    }
                }
            }


            // Prepare List of doDocumentDataGenerate
            List<doDocumentDataGenerate> doMainDoc = new List<doDocumentDataGenerate>();
            List<doDocumentDataGenerate> doSlaveDoc = new List<doDocumentDataGenerate>();

            for (int i = 0; i < icr020_list.Count; i++)
            {
                doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
                doDocument.DocumentNo = creditNoteNo;
                doDocument.DocumentCode = ReportID.C_REPORT_ID_CREDIT_NOTE;
                doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
                doDocument.DocumentData = icr020_list[i];
                doDocument.EmpNo = strEmpNo;
                doDocument.ProcessDateTime = dtDateTime;
                doDocument.OtherKey.BillingOffice = icr020_list[i][0].BillingOfficeCode;
                doDocument.OtherKey.BillingTargetCode = icr020_list[i][0].BillingTargetCode;

                if (i == 0)
                {
                    doMainDoc.Add(doDocument);
                }
                else
                {
                    doSlaveDoc.Add(doDocument);
                }
            }

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            if (isMerge == false)
            {
                strFilePath = documentHandler.GenerateDocumentFilePath(doMainDoc[0], doSlaveDoc);
            }
            else
            {
                //Without encrypt for merge report
                strFilePath = documentHandler.GenerateDocumentWithoutEncrypt(doMainDoc[0], doSlaveDoc);
            }
            return strFilePath;
        }

        //Merge blr020, icr010 report
        /// <summary>
        /// Genereate PDF file of BLR_020 tax invoice report merge with ICR_010 receipt report of specific tax invoice no. and receipt no. on shared report folder
        /// </summary>
        /// <param name="taxInvoiceNo">tax invoice no.</param>
        /// <param name="receiptNo">receipt no.</param>
        /// <param name="strEmpNo">employee no.</param>
        /// <param name="dtDateTime">process datetime</param>
        /// <returns></returns>
        public string GenerateBLR020_ICR010FilePath(string taxInvoiceNo, string receiptNo, string strEmpNo, DateTime dtDateTime)
        {
            IBillingDocumentHandler billingDocumentHandler = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;
            List<string> mergeList = new List<string>();
            // Comment by Jirawat Jannet
            //string blr020 = billingDocumentHandler.GenerateBLR020FilePath(taxInvoiceNo, strEmpNo, dtDateTime, true);
            //mergeList.Add(blr020);
            string icr010 = GenerateICR010FilePath(receiptNo, strEmpNo, dtDateTime, true);
            mergeList.Add(icr010);


            string mergeOutputFilename = PathUtil.GetTempFileName(".pdf");
            string encryptOutputFileName = PathUtil.GetTempFileName(".pdf");
            bool isSuccess = ReportUtil.MergePDF(mergeList.ToArray(), mergeOutputFilename, true, encryptOutputFileName, null);

            return encryptOutputFileName;
        }
        #endregion

        public string GenerateICS130ForAccount(List<doWHTReportForAccount> data, DateTime? period)
        {
            const string TEMPLATE_NAME = "ICS130_Account.xlsx";
            const string WSNAME_TEMPLATE = "WHTReportForAccount";
            const string WSNAME_Working = "Sheet1";

            const int ROW_RPTHEADER = 1;
            const int COL_RPTHEADER_TITLE = 1;

            const int ROW_HEADER = 2;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 3;
            const int COL_CustomerName = 1;
            const int COL_TAXID = 2;
            const int COL_WHTDocNo = 3;
            const int COL_WHTDocDate = 4;
            const int COL_CurrencyPayment = 5;
            const int COL_PaymentAmount = 6;
            const int COL_WHTAmount = 7;
            const int COL_MatchingAmount = 8;
            const int COL_Diff = 9;
            const int COL_WHTPercent = 10;
            const int COL_Payer = 11;
            const int COL_PaymentDate = 12;

            const int ROW_DTL = 4;

            const int COL_MIN = 1;
            const int COL_MAX = 12;


            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_TEMPLATE);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                doc.CopyCellFromWorksheet(WSNAME_TEMPLATE, ROW_RPTHEADER, COL_MIN, ROW_RPTHEADER, COL_MAX, ROW_RPTHEADER, 1);
                doc.SetRowHeight(ROW_RPTHEADER, docTemp.GetRowHeight(ROW_RPTHEADER));
                doc.MergeWorksheetCells(ROW_RPTHEADER, COL_MIN, ROW_RPTHEADER, COL_MAX);
                doc.SetCellValue(ROW_RPTHEADER, COL_RPTHEADER_TITLE, doc.GetCellValueAsString(ROW_RPTHEADER, COL_RPTHEADER_TITLE));

                doc.CopyCellFromWorksheet(WSNAME_TEMPLATE, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, ROW_HEADER, 1);
                doc.SetRowHeight(ROW_HEADER, docTemp.GetRowHeight(ROW_HEADER));
                doc.MergeWorksheetCells(ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX);
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    (period == null ? "" : period.Value.ToString("MMMM/yyyy").ToUpper())
                ));

                doc.CopyCellFromWorksheet(WSNAME_TEMPLATE, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, ROW_TBLHDR, 1);
                doc.SetRowHeight(ROW_TBLHDR, docTemp.GetRowHeight(ROW_TBLHDR));

                int rowindex = ROW_DTL;
                int lineno = 0;
                foreach (var detail in data)
                {
                    lineno++;

                    if(detail.AmountCurrencyTypeName == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    {
                        doc.CopyCellFromWorksheet(WSNAME_TEMPLATE, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.SetCellValue(rowindex, COL_CustomerName, detail.CustomerName);
                        doc.SetCellValue(rowindex, COL_TAXID, detail.TaxID);
                        doc.SetCellValue(rowindex, COL_WHTDocNo, detail.WHTDocNo);
                        doc.SetCellValue(rowindex, COL_WHTDocDate, detail.WHTDocDate);
                        doc.SetCellValue(rowindex, COL_CurrencyPayment, detail.AmountCurrencyTypeName);
                        doc.SetCellValue(rowindex, COL_PaymentAmount, detail.PaymentAmount ?? 0);
                        doc.SetCellValue(rowindex, COL_WHTAmount, detail.WHTAmountVal ?? 0);
                        doc.SetCellValue(rowindex, COL_MatchingAmount, detail.MatchingAmount);
                        doc.SetCellValue(rowindex, COL_Diff, detail.Diff ?? 0);
                        doc.SetCellValue(rowindex, COL_WHTPercent, detail.WHTPercent);
                        doc.SetCellValue(rowindex, COL_Payer, detail.Payer);
                        doc.SetCellValue(rowindex, COL_PaymentDate, detail.PaymentDate.ConvertTo<DateTime>(false, DateTime.Now));
                    }
                    else
                    {
                        doc.CopyCellFromWorksheet(WSNAME_TEMPLATE, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.SetCellValue(rowindex, COL_CustomerName, detail.CustomerName);
                        doc.SetCellValue(rowindex, COL_TAXID, detail.TaxID);
                        doc.SetCellValue(rowindex, COL_WHTDocNo, detail.WHTDocNo);
                        doc.SetCellValue(rowindex, COL_WHTDocDate, detail.WHTDocDate);
                        doc.SetCellValue(rowindex, COL_CurrencyPayment, detail.AmountCurrencyTypeName);
                        doc.SetCellValue(rowindex, COL_PaymentAmount, detail.PaymentAmountUsd ?? 0);
                        doc.SetCellValue(rowindex, COL_WHTAmount, detail.WHTAmountUsd);
                        doc.SetCellValue(rowindex, COL_MatchingAmount, detail.MatchingAmount);
                        doc.SetCellValue(rowindex, COL_Diff, detail.Diff ?? 0);
                        doc.SetCellValue(rowindex, COL_WHTPercent, detail.WHTPercent);
                        doc.SetCellValue(rowindex, COL_Payer, detail.Payer);
                        doc.SetCellValue(rowindex, COL_PaymentDate, detail.PaymentDate.ConvertTo<DateTime>(false, DateTime.Now));
                    }
                    
                    rowindex++;
                }


                doc.DeleteWorksheet(WSNAME_TEMPLATE);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public string GenerateICS130ForIMS(List<doWHTReportForIMS> data, DateTime? periodFrom, DateTime? periodTo)
        {
            const string TEMPLATE_NAME = "ICS130_IMS.xlsx";
            const string WSNAME_TEMPLATE = "WHTReportForIMS";
            const string WSNAME_Working = "Sheet1";

            const int ROW_RPTHEADER = 1;
            const int COL_RPTHEADER_TITLE = 1;

            const int ROW_HEADER = 2;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 3;
            const int COL_ContractCode = 1;
            const int COL_PaymentTransNo = 2;
            const int COL_InvoiceNo = 3;
            const int COL_TaxInvoiceNo = 4;
            const int COL_TaxID = 5;
            const int COL_CustomerName = 6;
            const int COL_Payer = 7;
            const int COL_PaymentDate = 8;
            const int COL_CurrencyPayment = 9;
            const int COL_PaymentAmount = 10;
            const int COL_ReceivedWHTAmount = 11;
            const int COL_WHTNo = 12;
            const int COL_BillingOffice = 13;
            const int COL_WitholdingDate = 14;

            const int ROW_DTL = 4;

            const int COL_MIN = 1;
            const int COL_MAX = 14;

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");
            using (SLDocument docTemp = new SLDocument(strReportTempletePath))
            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                docTemp.SelectWorksheet(WSNAME_TEMPLATE);
                doc.SelectWorksheet(WSNAME_Working);

                for (int i = COL_MIN; i <= COL_MAX; i++)
                {
                    doc.SetColumnWidth(i, docTemp.GetColumnWidth(i));
                }

                doc.CopyCellFromWorksheet(WSNAME_TEMPLATE, ROW_RPTHEADER, COL_MIN, ROW_RPTHEADER, COL_MAX, ROW_RPTHEADER, 1);
                doc.SetRowHeight(ROW_RPTHEADER, docTemp.GetRowHeight(ROW_RPTHEADER));
                doc.MergeWorksheetCells(ROW_RPTHEADER, COL_MIN, ROW_RPTHEADER, COL_MAX);
                doc.SetCellValue(ROW_RPTHEADER, COL_RPTHEADER_TITLE, doc.GetCellValueAsString(ROW_RPTHEADER, COL_RPTHEADER_TITLE));

                doc.CopyCellFromWorksheet(WSNAME_TEMPLATE, ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX, ROW_HEADER, 1);
                doc.SetRowHeight(ROW_HEADER, docTemp.GetRowHeight(ROW_HEADER));
                doc.MergeWorksheetCells(ROW_HEADER, COL_MIN, ROW_HEADER, COL_MAX);
                var period = string.Format(
                    "{0} - {1}",
                    (periodFrom == null ? "" : periodFrom.Value.ToString("MMMM/yyyy").ToUpper()),
                    (periodTo == null ? "" : periodTo.Value.ToString("MMMM/yyyy").ToUpper())
                );
                doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, string.Format(
                    doc.GetCellValueAsString(ROW_HEADER, COL_HEADER_TITLE),
                    period
                ));

                doc.CopyCellFromWorksheet(WSNAME_TEMPLATE, ROW_TBLHDR, COL_MIN, ROW_TBLHDR, COL_MAX, ROW_TBLHDR, 1);
                doc.SetRowHeight(ROW_TBLHDR, docTemp.GetRowHeight(ROW_TBLHDR));

                int rowindex = ROW_DTL;
                int lineno = 0;
                foreach (var detail in data)
                {
                    lineno++;

                    if(detail.PaymentAmountCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    {
                        doc.CopyCellFromWorksheet(WSNAME_TEMPLATE, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.SetCellValue(rowindex, COL_ContractCode, detail.ContractCode);
                        doc.SetCellValue(rowindex, COL_PaymentTransNo, detail.PaymentTransNo);
                        doc.SetCellValue(rowindex, COL_InvoiceNo, detail.InvoiceNo);
                        doc.SetCellValue(rowindex, COL_TaxInvoiceNo, detail.TaxInvoiceNo);
                        doc.SetCellValue(rowindex, COL_TaxID, detail.TaxID);
                        doc.SetCellValue(rowindex, COL_CustomerName, detail.CustomerName);
                        doc.SetCellValue(rowindex, COL_Payer, detail.Payer);
                        doc.SetCellValue(rowindex, COL_PaymentDate, detail.PaymentDate);
                        doc.SetCellValue(rowindex, COL_CurrencyPayment, detail.PaymentAmountCurrencyTypeName);
                        doc.SetCellValue(rowindex, COL_PaymentAmount, detail.PaymentAmount ?? 0);
                        doc.SetCellValue(rowindex, COL_ReceivedWHTAmount, detail.ReceivedWHTAmount ?? 0);
                        doc.SetCellValue(rowindex, COL_WHTNo, detail.WHTNo);
                        doc.SetCellValue(rowindex, COL_BillingOffice, detail.BillingOffice);
                        if (detail.WitholdingDate != null) doc.SetCellValue(rowindex, COL_WitholdingDate, detail.WitholdingDate.Value);
                    }
                    else
                    {
                        doc.CopyCellFromWorksheet(WSNAME_TEMPLATE, ROW_DTL, COL_MIN, ROW_DTL, COL_MAX, rowindex, 1);
                        doc.SetRowHeight(rowindex, docTemp.GetRowHeight(ROW_DTL));
                        doc.SetCellValue(rowindex, COL_ContractCode, detail.ContractCode);
                        doc.SetCellValue(rowindex, COL_PaymentTransNo, detail.PaymentTransNo);
                        doc.SetCellValue(rowindex, COL_InvoiceNo, detail.InvoiceNo);
                        doc.SetCellValue(rowindex, COL_TaxInvoiceNo, detail.TaxInvoiceNo);
                        doc.SetCellValue(rowindex, COL_TaxID, detail.TaxID);
                        doc.SetCellValue(rowindex, COL_CustomerName, detail.CustomerName);
                        doc.SetCellValue(rowindex, COL_Payer, detail.Payer);
                        doc.SetCellValue(rowindex, COL_PaymentDate, detail.PaymentDate);
                        doc.SetCellValue(rowindex, COL_CurrencyPayment, detail.PaymentAmountCurrencyTypeName);
                        doc.SetCellValue(rowindex, COL_PaymentAmount, detail.PaymentAmountUsd ?? 0);
                        doc.SetCellValue(rowindex, COL_ReceivedWHTAmount, detail.ReceivedWHTAmountUsd ?? 0);
                        doc.SetCellValue(rowindex, COL_WHTNo, detail.WHTNo);
                        doc.SetCellValue(rowindex, COL_BillingOffice, detail.BillingOffice);
                        if (detail.WitholdingDate != null) doc.SetCellValue(rowindex, COL_WitholdingDate, detail.WitholdingDate.Value);
                    }
                    
                    rowindex++;
                }


                doc.DeleteWorksheet(WSNAME_TEMPLATE);

                doc.SaveAs(strOutputPath);
                docTemp.CloseWithoutSaving();
            }

            return strOutputPath;
        }

        public Stream GenerateICR030(string strDocumentNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateICR030FilePath(strDocumentNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }

        public string GenerateICR030FilePath(string strDocumentNo, string strEmpNo, DateTime dtDateTime)
        {
            if (string.IsNullOrEmpty(strDocumentNo))
                return null;

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_REPORT_ID_DEBT_TRACING_NOTICE1);

            var rptdata = base.GetICR030(
                strDocumentNo,
                ReportDocLanguage.C_DOC_LANG_ENG,
                ReportDocLanguage.C_DOC_LANG_LOCAL,
                ShowIssueDate.C_SHOW_ISSUE_DATE_CHRISTIAN,
                ShowIssueDate.C_SHOW_ISSUE_DATE_BUDDHIST
            );

            if (rptdata == null || rptdata.Count == 0)
            {
                return null;
            }

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strDocumentNo;
            doDocument.DocumentCode = ReportID.C_REPORT_ID_DEBT_TRACING_NOTICE1;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = rptdata;

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            doDocument.OtherKey.BillingTargetCode = rptdata[0].BillingTargetCode;

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            }
            doDocument.MainReportParam = listMainReportParam;

            string reportPath = documentHandler.GenerateDocumentFilePath(doDocument);
            return reportPath;
        }

        public Stream GenerateICR040(string strDocumentNo, string strEmpNo, DateTime dtDateTime)
        {
            string strFilePath = GenerateICR040FilePath(strDocumentNo, strEmpNo, dtDateTime);
            IDocumentHandler handlerDocument = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;

            return handlerDocument.GetDocumentReportFileStream(strFilePath);

        }

        public string GenerateICR040FilePath(string strDocumentNo, string strEmpNo, DateTime dtDateTime)
        {
            if (string.IsNullOrEmpty(strDocumentNo))
                return null;

            IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
            List<tbm_DocumentTemplate> dLst = documentHandler.GetDocumentTemplateByDocumentCode(ReportID.C_REPORT_ID_DEBT_TRACING_NOTICE2);

            var rptdata = base.GetICR030(
                strDocumentNo,
                ReportDocLanguage.C_DOC_LANG_ENG,
                ReportDocLanguage.C_DOC_LANG_LOCAL,
                ShowIssueDate.C_SHOW_ISSUE_DATE_CHRISTIAN,
                ShowIssueDate.C_SHOW_ISSUE_DATE_BUDDHIST
            );

            if (rptdata == null || rptdata.Count == 0)
            {
                return null;
            }

            doDocumentDataGenerate doDocument = new doDocumentDataGenerate();
            doDocument.DocumentNo = strDocumentNo;
            doDocument.DocumentCode = ReportID.C_REPORT_ID_DEBT_TRACING_NOTICE2;
            doDocument.DocumentOCC = ConfigName.C_CONFIG_DOC_OCC_DEFAULT;
            doDocument.DocumentData = rptdata;

            // Additional
            doDocument.EmpNo = strEmpNo;
            doDocument.ProcessDateTime = dtDateTime;

            doDocument.OtherKey.BillingTargetCode = rptdata[0].BillingTargetCode;

            List<ReportParameterObject> listMainReportParam = new List<ReportParameterObject>();
            if (dLst.Count > 0)
            {
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "DocVersion", Value = dLst[0].DocumentVersion });
                listMainReportParam.Add(new ReportParameterObject() { ParameterName = "TemplateName", Value = dLst[0].DocumentNameEN });
            }
            doDocument.MainReportParam = listMainReportParam;

            string reportPath = documentHandler.GenerateDocumentFilePath(doDocument);
            return reportPath;
        }

        public string GenerateICR050Report(List<doGetICR050> data, doMatchRReport paramSearch)
        {

            //prepare data
            List<doHeaderReport> listheader = new List<doHeaderReport>();


            #region Parameter
            decimal? POSTTRADE = 0;
            decimal? BANGKOKBANK_01 = 0;
            decimal? KASIKORNBANK_04 = 0;
            decimal? SIAMCOMMERCIALBANK_09 = 0;
            decimal? SIAMCOMMERCIALBANK_10 = 0;
            decimal? SIAMCOMMERCIALBANK_08 = 0;
            decimal? BANKOFAYUDTAYA_02 = 0;
            decimal? KRUNGTHAIBANK_06 = 0;
            decimal? THAIMILITARYBANK_11 = 0;
            decimal? SIAMCITYBANK_07 = 0;
            decimal? TISCOBANK_21 = 0;
            decimal? THANACHARTBANK_12 = 0;
            decimal? UNITEDOVERSEASBANK_15 = 0;
            decimal? UNITEDOVERSEASBANK_16 = 0;
            decimal? UNITEDOVERSEASBANK_13 = 0;
            decimal? UNITEDOVERSEASBANK_14 = 0;
            decimal? BANKTHAI_03 = 0;
            decimal? THEBANKOFTOKYO_18 = 0;
            decimal? SUMITOMOMITSUIBANK_20 = 0;
            decimal? KASIKORNBANK_05 = 0;
            decimal? MIZUHOCORPORATEBANK_19 = 0;
            decimal? DEFERREDREVENUE = 0;

            decimal? WITHOLDINGINCOMETAX = 0;
            decimal? GUARANTEEBIDDING = 0;

            decimal? OTHERRECEIVABLE = 0;
            decimal? BANKINGFEES = 0;
            decimal? MISCELLANEOUSEFEES = 0;
            decimal? MISCELLANEOUSEXPENSES = 0;

            //credit
            decimal? DEFERREDREVENUE_RENTALFEES = 0;
            decimal? DEFERREDREVENUE_SENTRYGUARD = 0;
            decimal? CONTRACTDEPOSIT = 0;
            decimal? AR_INSTALLATIONFEES = 0;
            decimal? AR_SALES = 0;
            decimal? AR_MAINENANCE = 0;
            decimal? OUTPUTTAX = 0;
            decimal? OTHERINCOME = 0;
            decimal? OTHERRECEIVABLE_CR = 0;
            decimal? DEFERREDREVENUE_MERCHANDISE = 0;
            decimal? DEFERREDREVENUE_INSTALLATION = 0;
            decimal? DEFERREDREVENUE_OTHER = 0;
            decimal? totalAll = 0;
            string totalAllCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;

            #endregion

            string paymentTran = string.Empty;


            foreach (var itemsHeader in data)
            {
                if (itemsHeader.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED
                    || itemsHeader.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE)
                {
                    POSTTRADE = POSTTRADE + itemsHeader.PaymentAmount;
                    totalAll = totalAll + itemsHeader.PaymentAmount;
                }
                else if (itemsHeader.PaymentType == PaymentType.C_PAYMENT_TYPE_CASH)
                {

                    DEFERREDREVENUE = DEFERREDREVENUE + itemsHeader.PaymentAmount;
                    totalAll = totalAll + itemsHeader.PaymentAmount;
                }
                else if (itemsHeader.PaymentType == PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER
                        || itemsHeader.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_NORMAL
                        || itemsHeader.PaymentType == PaymentType.C_PAYMENT_TYPE_CASHIER_CHEQUE
                        || itemsHeader.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE
                        || itemsHeader.PaymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
                {

                    if (itemsHeader.SECOMAccountID == SecomAccountID.C_BANGKOK_BANK_LTD_SATHORN)
                    {
                        BANGKOKBANK_01 = BANGKOKBANK_01 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_KASIKORN_BANK_BANG_LAMPHU)
                    {
                        KASIKORNBANK_04 = KASIKORNBANK_04 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_SIAM_COMMERCIAL_BANK_CHIDLOM_C)
                    {
                        SIAMCOMMERCIALBANK_09 = SIAMCOMMERCIALBANK_09 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_SIAM_COMMERCIAL_BANK_CHIDLOM_S)
                    {
                        SIAMCOMMERCIALBANK_10 = SIAMCOMMERCIALBANK_10 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_SIAM_COMMERCIAL_BANK_RAJYINDEE_HADYAI)
                    {
                        SIAMCOMMERCIALBANK_08 = SIAMCOMMERCIALBANK_08 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_BANK_OF_AYUDHYA_NANGLINGEE)
                    {
                        BANKOFAYUDTAYA_02 = BANKOFAYUDTAYA_02 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_KRUNGTHAI_BANK_KLONGTOEY)
                    {
                        KRUNGTHAIBANK_06 = KRUNGTHAIBANK_06 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_THAI_MILITARY_BANK_KLONGTOEY)
                    {
                        THAIMILITARYBANK_11 = THAIMILITARYBANK_11 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_SIAM_CITY_BANK_PETCHBURI)
                    {
                        SIAMCITYBANK_07 = SIAMCITYBANK_07 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_TISCO_BANK_PUBLIC)
                    {
                        TISCOBANK_21 = TISCOBANK_21 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_THANACHART_BANK_TONSON)
                    {
                        THANACHARTBANK_12 = THANACHARTBANK_12 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_UNITED_OVERSEAS_BANK_CHIENGMAI)
                    {
                        UNITEDOVERSEASBANK_15 = UNITEDOVERSEASBANK_15 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_UNITED_OVERSEAS_BANK_THAIWAH)
                    {
                        UNITEDOVERSEASBANK_16 = UNITEDOVERSEASBANK_16 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_UNITED_OVERSEAS_BANK_SRIRACHA)
                    {
                        UNITEDOVERSEASBANK_13 = UNITEDOVERSEASBANK_13 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_UNITED_OVERSEAS_BANK_NRM)
                    {
                        UNITEDOVERSEASBANK_14 = UNITEDOVERSEASBANK_14 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_BANKTHAI_LANGSUAN)
                    {
                        BANKTHAI_03 = BANKTHAI_03 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_THE_BANK_OF_TOKYO)
                    {
                        THEBANKOFTOKYO_18 = THEBANKOFTOKYO_18 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_SUMITOMO_MITSUI_BANKING_CORPORATION)
                    {
                        SUMITOMOMITSUIBANK_20 = SUMITOMOMITSUIBANK_20 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_KASIKORN_BANK_ALLSEASONS_PLACE)
                    {
                        KASIKORNBANK_05 = KASIKORNBANK_05 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }
                    else if (itemsHeader.SECOMAccountID == SecomAccountID.C_MIZUHO_CORPORATE_BANK)
                    {
                        MIZUHOCORPORATEBANK_19 = MIZUHOCORPORATEBANK_19 + itemsHeader.PaymentAmount;
                        totalAll = totalAll + itemsHeader.PaymentAmount;
                    }

                }


                #region conditioncheckDetailCR
                List<doGetICR050_Detail> lstDetail = new List<doGetICR050_Detail>();
                lstDetail = (from x in base.GetICR050_Detail(itemsHeader.PaymentTransNo)
                             select x).ToList();
                string matchID = string.Empty;
                foreach (var itemsDetail in lstDetail)
                {
                    if (matchID != itemsDetail.MatchID)
                    {

                        GUARANTEEBIDDING = 0;
                        if ((itemsDetail.OtherExpenseAmount ?? 0) > 100)
                        {
                            OTHERRECEIVABLE += (itemsDetail.OtherExpenseAmount ?? 0);
                        }
                        else
                        {
                            MISCELLANEOUSEXPENSES += (itemsDetail.OtherExpenseAmount ?? 0);
                        }
                        BANKINGFEES = BANKINGFEES + (itemsDetail.BankFeeAmount != null ? itemsDetail.BankFeeAmount : 0);

                        totalAll = totalAll + (itemsDetail.OtherExpenseAmount != null ? itemsDetail.OtherExpenseAmount : 0);
                        totalAll = totalAll + (itemsDetail.BankFeeAmount != null ? itemsDetail.BankFeeAmount : 0);
                        
                        //cr
                        if ((itemsDetail.OtherIncomeAmount ?? 0) > 100)
                        {
                            //DEFERREDREVENUE_OTHER
                            DEFERREDREVENUE_OTHER += (itemsDetail.OtherIncomeAmount ?? 0);
                        }
                        else
                        {
                            OTHERINCOME += (itemsDetail.OtherIncomeAmount ?? 0);
                        }
                        matchID = itemsDetail.MatchID;
                    }
                    WITHOLDINGINCOMETAX = WITHOLDINGINCOMETAX + (itemsDetail.RegisteredWHTAmount != null ? itemsDetail.RegisteredWHTAmount : 0);

                    totalAll = totalAll + (itemsDetail.RegisteredWHTAmount != null ? itemsDetail.RegisteredWHTAmount : 0);

                    MISCELLANEOUSEFEES = 0;

                    //DEFERREDREVENUE_RENTALFEES
                    if (itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_SERVICE
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_SERVICE
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DURING_STOP_SERVICE)
                    {
                        DEFERREDREVENUE_RENTALFEES = DEFERREDREVENUE_RENTALFEES + itemsDetail.InvoiceAmount;
                    }

                    //DEFERREDREVENUE_SENTRYGUARD
                    if (itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_SG
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_SG
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DURING_STOP_SG)
                    {
                        DEFERREDREVENUE_SENTRYGUARD = DEFERREDREVENUE_SENTRYGUARD + itemsDetail.InvoiceAmount;
                    }

                    //CONTRACTDEPOSIT
                    if (itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
                    {
                        CONTRACTDEPOSIT = CONTRACTDEPOSIT + itemsDetail.InvoiceAmount;
                    }

                    //AR_INSTALLATIONFEES
                    if (itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_INSTALL_SALE
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_INSTALL_ADVANCE
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_CHANGE_INSTALL
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_CHANGE_INSTALL_ADVANCE
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_REMOVAL_INSTALL)
                    {
                        AR_INSTALLATIONFEES = AR_INSTALLATIONFEES + itemsDetail.InvoiceAmount;
                    }


                    //AR_SALES
                    if (itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_CARD
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_REVISE_VAT
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN)
                    {
                        AR_SALES = AR_SALES + itemsDetail.InvoiceAmount + itemsDetail.VatAmount;
                    }

                    //AR_MAINENANCE
                    if (itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_MA
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_MA
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DURING_STOP_MA
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_MA_RESULT_BASE
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_DIFF_MA_RESULT_BASE)
                    {
                        AR_MAINENANCE = AR_MAINENANCE + itemsDetail.InvoiceAmount;
                    }

                    //OUTPUTTAX
                    //if (itemsDetail.BillingTypeCode != BillingType.C_BILLING_TYPE_SALE_PRICE
                    //    && itemsDetail.BillingTypeCode != BillingType.C_BILLING_TYPE_CARD
                    //    && itemsDetail.BillingTypeCode != BillingType.C_BILLING_TYPE_DIFF_REVISE_VAT
                    //    && itemsDetail.BillingTypeCode != BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL
                    //    && itemsDetail.BillingTypeCode != BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE)
                    //{
                    //    if (itemsDetail.TaxInvoiceDate.Value.Year == itemsHeader.PaymentDate.Year && itemsDetail.TaxInvoiceDate.Value.Month == itemsHeader.PaymentDate.Month)
                    //    {

                    //    }
                    //}

                    //OTHERRECEIVABLE_CR
                    if (itemsDetail.BillingTypeCode != BillingType.C_BILLING_TYPE_SALE_PRICE
                        && itemsDetail.BillingTypeCode != BillingType.C_BILLING_TYPE_CARD
                        && itemsDetail.BillingTypeCode != BillingType.C_BILLING_TYPE_DIFF_REVISE_VAT
                        && itemsDetail.BillingTypeCode != BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL
                        && itemsDetail.BillingTypeCode != BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE
                        && itemsDetail.BillingTypeCode != BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN)
                    {
                        //if (itemsDetail.TaxInvoiceDate.Value.Year <= itemsHeader.PaymentDate.Year
                        //    && itemsDetail.TaxInvoiceDate.Value.Month < itemsHeader.PaymentDate.Month
                        //    || itemsDetail.TaxInvoiceDate.Value.Month < itemsHeader.PaymentDate.Month)                       
                        if (
                            (itemsDetail.TaxInvoiceDate.Value.Year == itemsHeader.PaymentDate.Year && itemsDetail.TaxInvoiceDate.Value.Month < itemsHeader.PaymentDate.Month)
                            || (itemsDetail.TaxInvoiceDate.Value.Year < itemsHeader.PaymentDate.Year)
                        )
                        {
                            OTHERRECEIVABLE_CR = OTHERRECEIVABLE_CR + itemsDetail.VatAmount;
                        }
                        else  if (itemsDetail.TaxInvoiceDate.Value.Year == itemsHeader.PaymentDate.Year &&
                            itemsDetail.TaxInvoiceDate.Value.Month == itemsHeader.PaymentDate.Month)
                        {
                            OUTPUTTAX = OUTPUTTAX + itemsDetail.VatAmount;
                        }
                    }


                    //OTHERINCOME
                    if (itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_CONSUMABLE
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_OTHER
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_CANCEL_CONTRACT
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_EMERGENCY
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_ON_CALL)
                    {
                        OTHERINCOME = OTHERINCOME + itemsDetail.InvoiceAmount;
                    }




                    ////DEFERREDREVENUE_MERCHANDISE
                    //if (itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN)
                    //{
                    //    DEFERREDREVENUE_MERCHANDISE = DEFERREDREVENUE_MERCHANDISE + itemsDetail.InvoiceAmount;
                    //}

                    //DEFERREDREVENUE_INSTALLATION
                    if (itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_INSTALL
                        || itemsDetail.BillingTypeCode == BillingType.C_BILLING_TYPE_INSTRUMENT_SETUP)
                    {
                        List<doGetBillingDetail> lstbill = new List<doGetBillingDetail>();
                        lstbill = (from x in base.GetBillingDetail(itemsDetail.InvoiceNo)
                                   select x).ToList();

                        if (lstbill.Count != 0)
                        {
                            foreach (var itemsbilling in lstbill)
                            {

                                string contractprefix = string.Empty;

                                contractprefix = itemsbilling.ContractCode.Substring(0, 1);
                                if (contractprefix == "Q")
                                {
                                    doGetSaleBasic lstSale = new doGetSaleBasic();
                                    lstSale = (from x in base.GetSaleBasic(itemsbilling.ContractCode)
                                               select x).ToList().FirstOrDefault();
                                    if (lstSale.InstallCompleteDate == null || lstSale.CustAcceptanceDate == null || itemsHeader.PaymentDate < lstSale.InstallCompleteDate || itemsHeader.PaymentDate < lstSale.CustAcceptanceDate)
                                    {
                                        DEFERREDREVENUE_INSTALLATION = DEFERREDREVENUE_INSTALLATION + itemsbilling.BillingAmount;
                                    }
                                    else
                                    {
                                        AR_INSTALLATIONFEES = AR_INSTALLATIONFEES + itemsbilling.BillingAmount;
                                    }
                                }
                                //else if (contractprefix == "S" || contractprefix == "N" || contractprefix == "M")
                                else
                                {
                                    doGetRentalContractBasic lstRental = new doGetRentalContractBasic();
                                    lstRental = (from x in base.GetRentalContractBasic(itemsbilling.ContractCode)
                                                 select x).ToList().FirstOrDefault();
                                    if (lstRental.FirstSecurityStartDate != null && itemsHeader.PaymentDate >= lstRental.FirstSecurityStartDate)
                                    {
                                        AR_INSTALLATIONFEES = AR_INSTALLATIONFEES + itemsbilling.BillingAmount;
                                    }
                                    else
                                    {
                                        DEFERREDREVENUE_INSTALLATION = DEFERREDREVENUE_INSTALLATION + itemsbilling.BillingAmount;
                                    }
                                }

                            }
                        }
                    }






                }
                #endregion


                if (itemsHeader.PaymentAmountCurrencyType != null)
                    totalAllCurrencyType = itemsHeader.PaymentAmountCurrencyType;
            }

            #region MatchParameter
            List<doMatchAccountCode> lstMatch = new List<doMatchAccountCode>();

            List<doGetAccountCode> getListAccCodeDebit = (from x in base.GetAccountCode()
                                                          where x.DebitCreditType.Equals("D")
                                                          select x).ToList();
            List<doGetAccountCode> getListAccCodeCredit = (from x in base.GetAccountCode()
                                                           where x.DebitCreditType.Equals("C")
                                                           select x).ToList();
            #region Debit
            foreach (var itemCodeDebit in getListAccCodeDebit)
            {
                //Debit

                if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_POSTTRADE)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = POSTTRADE.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_BANGKOKBANK_01)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = BANGKOKBANK_01.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_KASIKORNBANK_04)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = KASIKORNBANK_04.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_SIAMCOMMERCIALBANK_09)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = SIAMCOMMERCIALBANK_09.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_SIAMCOMMERCIALBANK_10)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = SIAMCOMMERCIALBANK_10.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_SIAMCOMMERCIALBANK_08)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = SIAMCOMMERCIALBANK_08.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_BANKOFAYUDTAYA_02)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = BANKOFAYUDTAYA_02.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_KRUNGTHAIBANK_06)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = KRUNGTHAIBANK_06.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_THAIMILITARYBANK_11)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = THAIMILITARYBANK_11.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_SIAMCITYBANK_07)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = SIAMCITYBANK_07.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_TISCOBANK_21)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = TISCOBANK_21.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_THANACHARTBANK_12)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = THANACHARTBANK_12.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_UNITEDOVERSEASBANK_15)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = UNITEDOVERSEASBANK_15.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_UNITEDOVERSEASBANK_16)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = UNITEDOVERSEASBANK_16.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_UNITEDOVERSEASBANK_13)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = UNITEDOVERSEASBANK_13.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_UNITEDOVERSEASBANK_14)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = UNITEDOVERSEASBANK_14.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_BANKTHAI_03)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = BANKTHAI_03.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_THEBANKOFTOKYO_18)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = THEBANKOFTOKYO_18.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_SUMITOMOMITSUIBANK_20)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = SUMITOMOMITSUIBANK_20.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_KASIKORNBANK_05)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = KASIKORNBANK_05.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_MIZUHOCORPORATEBANK_19)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = MIZUHOCORPORATEBANK_19.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_DEFERREDREVENUE)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = DEFERREDREVENUE.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_WITHOLDINGINCOMETAX)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = WITHOLDINGINCOMETAX.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_GUARANTEEBIDDING)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = GUARANTEEBIDDING.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_OTHERRECEIVABLE)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = OTHERRECEIVABLE.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_BANKINGFEES)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = BANKINGFEES.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_MISCELLANEOUSEFEES)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = MISCELLANEOUSEFEES.Value });
                }
                else if (itemCodeDebit.AccountCode == DebitOutPutTax.C_DEBIT_MISCELLANEOUSEXPENSES)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeDebit, ValueAmount = MISCELLANEOUSEXPENSES.Value });
                }
            }
            #endregion
            #region Credit
            //Credit
            foreach (var itemCodeCredit in getListAccCodeCredit)
            {
                if (itemCodeCredit.AccountCode == CreditOutPutTax.C_CREDIT_DEFERREDREVENUE_RENTALFEES)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeCredit, ValueAmount = DEFERREDREVENUE_RENTALFEES.Value });
                }
                else if (itemCodeCredit.AccountCode == CreditOutPutTax.C_CREDIT_DEFERREDREVENUE_SENTRYGUARD)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeCredit, ValueAmount = DEFERREDREVENUE_SENTRYGUARD.Value });
                }
                else if (itemCodeCredit.AccountCode == CreditOutPutTax.C_CREDIT_CONTRACTDEPOSIT)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeCredit, ValueAmount = CONTRACTDEPOSIT.Value });
                }
                else if (itemCodeCredit.AccountCode == CreditOutPutTax.C_CREDIT_AR_INSTALLATIONFEES)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeCredit, ValueAmount = AR_INSTALLATIONFEES.Value });
                }
                else if (itemCodeCredit.AccountCode == CreditOutPutTax.C_CREDIT_AR_SALES)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeCredit, ValueAmount = AR_SALES.Value });
                }
                else if (itemCodeCredit.AccountCode == CreditOutPutTax.C_CREDIT_AR_MAINENANCE)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeCredit, ValueAmount = AR_MAINENANCE.Value });
                }
                else if (itemCodeCredit.AccountCode == CreditOutPutTax.C_CREDIT_OUTPUTTAX)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeCredit, ValueAmount = OUTPUTTAX.Value });
                }
                else if (itemCodeCredit.AccountCode == CreditOutPutTax.C_CREDIT_OTHERINCOME)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeCredit, ValueAmount = OTHERINCOME.Value });
                }
                else if (itemCodeCredit.AccountCode == CreditOutPutTax.C_CREDIT_OTHERRECEIVABLE_CR)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeCredit, ValueAmount = OTHERRECEIVABLE_CR.Value });
                }
                else if (itemCodeCredit.AccountCode == CreditOutPutTax.C_CREDIT_DEFERREDREVENUE_MERCHANDISE)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeCredit, ValueAmount = DEFERREDREVENUE_MERCHANDISE.Value });
                }
                else if (itemCodeCredit.AccountCode == CreditOutPutTax.C_CREDIT_DEFERREDREVENUE_INSTALLATION)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeCredit, ValueAmount = DEFERREDREVENUE_INSTALLATION.Value });
                }
                else if (itemCodeCredit.AccountCode == CreditOutPutTax.C_CREDIT_DEFERREDREVENUE_OTHER)
                {
                    lstMatch.Add(new doMatchAccountCode() { AccountCodeInfo = itemCodeCredit, ValueAmount = DEFERREDREVENUE_OTHER.Value });
                }


            }

            #endregion


            var ss = lstMatch;

            #endregion

            const string TEMPLATE_NAME = "ICR050.xlsx";
            const string WSNAME_Working = "MatchR";
            const string DEFAULTCURRENCT = "Rupiah";


            #region paramExcel Const
            const int ROW_HEADER_DATE = 3;
            const int COL_HEADER_DATE = 7;
            const int ROW_HEADER_NO = 4;
            const int COL_HEADER_NO = 7;

            const int COL_ACCOUNT_CODE = 2;
            const int COL_ACCOUNT_NAME = 3;
            const int COL_DEBIT = 6;
            const int COL_CREDIT = 7;

            const int COL_START_TOTAL_DEBIT = 2;
            const int COL_START_DATA_DEBIT = 6;
            const int COL_START_DATA_CREDIT = 7;

            const int ROW_START_DATA = 9;


            const int TOTALTHAI_ROW = 31;

            const int ROW_CREATOR_NAME = 33;
            const int COL_CREATOR_NAME = 2;

            int rowindex = ROW_START_DATA;
            int rowStartDebit = ROW_START_DATA;
            int rowEndDebit = 0;

            int rowStartCredit = 0;
            int rowEndCredit = 0;
            #endregion

            var commonUtil = new CommonUtil();

            string strReportTempletePath = PathUtil.GetPathValue(PathUtil.PathName.ReportTempatePath, TEMPLATE_NAME);
            if (!File.Exists(strReportTempletePath))
            {
                throw new FileNotFoundException("Template file not found.", TEMPLATE_NAME);
            }

            var master = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
            var empinfo = master.GetActiveEmployee(paramSearch.CreateBy).FirstOrDefault();
            string creatorname = string.Format("{0} {1}", empinfo.EmpFirstNameLC, empinfo.EmpLastNameLC);

            string strOutputPath = PathUtil.GetTempFileName(".xlsx");

            using (SLDocument doc = new SLDocument(strReportTempletePath))
            {
                doc.AddWorksheet(WSNAME_Working);
                doc.SelectWorksheet(WSNAME_Working);

                doc.SetCellValue(ROW_HEADER_DATE, COL_HEADER_DATE, paramSearch.PaymentDate.Value.ToString("dd-MMM-yyyy"));
                doc.SetCellValue(ROW_HEADER_NO, COL_HEADER_NO, string.Format("{0}/{1}", paramSearch.CreateBy, paramSearch.GroupName));

                SLStyle styBorder = new SLStyle();
                styBorder.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;

                //Debit
                var lstMatchDebit = (from x in lstMatch
                                     where x.ValueAmount != 0
                                     && x.AccountCodeInfo.DebitCreditType == "D"
                                     orderby x.AccountCodeInfo.OrderSeq
                                     select x).ToList();

                foreach (var itemslstMatchDebit in lstMatchDebit)
                {
                    doc.SetCellValue(rowindex, COL_ACCOUNT_CODE, itemslstMatchDebit.AccountCodeInfo.AccountCode);
                    doc.SetCellValue(rowindex, COL_ACCOUNT_NAME, itemslstMatchDebit.AccountCodeInfo.AccountName);
                    doc.SetCellValue(rowindex, COL_DEBIT, itemslstMatchDebit.ValueAmount.Value);
                    doc.SetCellStyle(rowindex, COL_ACCOUNT_CODE, rowindex, COL_CREDIT + 1, styBorder);
                    rowindex++;
                }
                rowEndDebit = rowindex;
                //Credit
                rowStartCredit = rowindex;
                var lstMatchCredit = (from x in lstMatch
                                      where x.ValueAmount != 0
                                     && x.AccountCodeInfo.DebitCreditType == "C"
                                      orderby x.AccountCodeInfo.OrderSeq
                                      select x).ToList();

                foreach (var itemslstMatchCredit in lstMatchCredit)
                {
                    doc.SetCellValue(rowindex, COL_ACCOUNT_CODE, itemslstMatchCredit.AccountCodeInfo.AccountCode);
                    doc.SetCellValue(rowindex, COL_ACCOUNT_NAME, itemslstMatchCredit.AccountCodeInfo.AccountName);
                    doc.SetCellValue(rowindex, COL_CREDIT, itemslstMatchCredit.ValueAmount.Value);
                    doc.SetCellStyle(rowindex, COL_ACCOUNT_CODE, rowindex, COL_CREDIT + 1, styBorder);
                    rowindex++;
                }
                rowEndCredit = rowindex;

                //doc.SetCellValue(TOTALTHAI_ROW, COL_START_TOTAL_DEBIT, commonUtil.ToBahtText(Convert.ToDecimal(totalAll))); comment by jirawwat jannet on 2016-10-31
                // add by jirawat jannet on 2016-10-31
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                string totalAllCurrencyTypeName =  hand.getCurrencyName(totalAllCurrencyType);
                string priceWord = ReportUtil.NumberToEndlishWords(Convert.ToInt32(totalAll));
                if (priceWord.Trim() != string.Empty) priceWord = DEFAULTCURRENCT + " " + priceWord;
                doc.SetCellValue(TOTALTHAI_ROW, COL_START_TOTAL_DEBIT, priceWord);

                doc.SetCellValue(TOTALTHAI_ROW, COL_START_DATA_DEBIT, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(rowStartDebit, COL_START_DATA_DEBIT),
                    SLConvert.ToCellReference(rowEndDebit - 1, COL_START_DATA_DEBIT)
                ));

                doc.SetCellValue(TOTALTHAI_ROW, COL_START_DATA_CREDIT, string.Format(
                    "=SUM({0}:{1})",
                    SLConvert.ToCellReference(rowStartCredit, COL_START_DATA_CREDIT),
                    SLConvert.ToCellReference(rowEndCredit - 1, COL_START_DATA_CREDIT)
                ));

                doc.SetCellValue(ROW_CREATOR_NAME, COL_CREATOR_NAME, creatorname);

                doc.SelectWorksheet(doc.GetWorksheetNames().First());

                doc.SaveAs(strOutputPath);
            }

            return strOutputPath;
        }
    }
}
