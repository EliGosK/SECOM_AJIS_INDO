using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SpreadsheetLight;
using System.Drawing;

namespace SECOM_AJIS.DataEntity.Accounting.Handlers
{
    public partial class AccountingHandler
    {
        public List<dtACC011_Sheet1> GetAccountDataOfACC011_Sheet1(DateTime? dtTargetPeriodFrom, DateTime? dtTargetPeriodTo, string strHQCode)
        {
            ACDataEntities context = new ACDataEntities();
            return context.GetAccountDataOfACC011_Sheet1(dtTargetPeriodFrom, dtTargetPeriodTo, strHQCode).ToList();
        }

        public List<dtACC011_Sheet2> GetAccountDataOfACC011_Sheet2(DateTime? dtTargetPeriodFrom, DateTime? dtTargetPeriodTo, string strHQCode)
        {
            ACDataEntities context = new ACDataEntities();
            return context.GetAccountDataOfACC011_Sheet2(dtTargetPeriodFrom, dtTargetPeriodTo, strHQCode).ToList();
        }

        public List<dtACC011_Sheet3> GetAccountDataOfACC011_Sheet3(DateTime? dtTargetPeriodFrom, DateTime? dtTargetPeriodTo, string strHQCode)
        {
            ACDataEntities context = new ACDataEntities();
            return context.GetAccountDataOfACC011_Sheet3(dtTargetPeriodFrom, dtTargetPeriodTo, strHQCode).ToList();
        }

        public List<dtACC011_Sheet4> GetAccountDataOfACC011_Sheet4(DateTime? dtTargetPeriodFrom, DateTime? dtTargetPeriodTo, string strHQCode)
        {
            ACDataEntities context = new ACDataEntities();
            return context.GetAccountDataOfACC011_Sheet4(dtTargetPeriodFrom, dtTargetPeriodTo, strHQCode).ToList();
        }
        public List<dtACC011_Sheet5> GetAccountDataOfACC011_Sheet5(DateTime? dtTargetPeriodFrom, DateTime? dtTargetPeriodTo, string strHQCode)
        {
            ACDataEntities context = new ACDataEntities();
            return context.GetAccountDataOfACC011_Sheet5(dtTargetPeriodFrom, dtTargetPeriodTo, strHQCode).ToList();
        }
        public List<dtACC011_Sheet6> GetAccountDataOfACC011_Sheet6(DateTime? dtTargetPeriodFrom, DateTime? dtTargetPeriodTo, string strHQCode)
        {
            ACDataEntities context = new ACDataEntities();
            return context.GetAccountDataOfACC011_Sheet6(dtTargetPeriodFrom, dtTargetPeriodTo, strHQCode).ToList();
        }
        public doGenerateDocumentResult ACC011_PaymentListReport(DocumentContext context)
        {
            doGenerateDocumentResult result = new doGenerateDocumentResult()
            {
                ErrorFlag = false,
                Total = 0,
                Complete = 0,
                Failed = 0,
                ResultDocumentNoList = string.Empty
            };

            using (TransactionScope scope = new TransactionScope())
            {
            try
            {
                #region Write text report
                dtAccountingConfig folder = this.getAccountingConfig(AccountingConfig.C_ACC_CONFIG_GROUP_CSV, AccountingConfig.C_ACC_CONFIG_NAME_FOLDER);
                string documentNo = string.Format(@"{0}{1}{2}{3}", context.DocumentCode, context.TargetPeriodFrom.Value.ToString("yyyyMMdd"), context.TargetPeriodTo.Value.ToString("yyyyMMdd"), context.UserHQCode);
                string outputfilepath = string.Format(@"{0}\{1}\{2}.xlsx", context.TargetPeriodTo.Value.ToString("yyyyMM"), folder.ConfigValue, documentNo);
                string fullOutputFilePath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, outputfilepath);

                int dataNotFoundCount = 0;

                using (SLDocument doc = new SLDocument())
                {

                    //Sheet1
                    {
                        result = this.GenerateACC011_Sheet1(context, doc, "Sheet1");
                        if (result.ErrorFlag == true)
                        {
                            if (result.ErrorCode == MessageUtil.MessageList.MSG8005)
                            {
                                dataNotFoundCount++;
                            }
                            else
                            {
                                return result;
                            }
                        }
                        else
                        {
                            result.Complete += 1;
                        }
                    }

                    //Sheet2
                    {
                        result = this.GenerateACC011_Sheet2(context, doc, "Sheet2");
                        if (result.ErrorFlag == true)
                        {
                            if (result.ErrorCode == MessageUtil.MessageList.MSG8005)
                            {
                                dataNotFoundCount++;
                            }
                            else
                            {
                                return result;
                            }
                        }
                        else
                        {
                            result.Complete += 1;
                        }
                    }
                    //Sheet3
                    {
                        result = this.GenerateACC011_Sheet3(context, doc, "Sheet3");
                        if (result.ErrorFlag == true)
                        {
                            if (result.ErrorCode == MessageUtil.MessageList.MSG8005)
                            {
                                dataNotFoundCount++;
                            }
                            else
                            {
                                return result;
                            }
                        }
                        else
                        {
                            result.Complete += 1;
                        }
                    }
                    //Sheet4
                    {
                        result = this.GenerateACC011_Sheet4(context, doc, "Sheet4");
                        if (result.ErrorFlag == true)
                        {
                            if (result.ErrorCode == MessageUtil.MessageList.MSG8005)
                            {
                                dataNotFoundCount++;
                            }
                            else
                            {
                                return result;
                            }
                        }
                        else
                        {
                            result.Complete += 1;
                        }
                    }
                    //Sheet5
                    {
                        result = this.GenerateACC011_Sheet5(context, doc, "Sheet5");
                        if (result.ErrorFlag == true)
                        {
                            if (result.ErrorCode == MessageUtil.MessageList.MSG8005)
                            {
                                dataNotFoundCount++;
                            }
                            else
                            {
                                return result;
                            }
                        }
                        else
                        {
                            result.Complete += 1;
                        }
                    }
                    //Sheet6
                    {
                        result = this.GenerateACC011_Sheet6(context, doc, "Sheet6");
                        if (result.ErrorFlag == true)
                        {
                            if (result.ErrorCode == MessageUtil.MessageList.MSG8005)
                            {
                                dataNotFoundCount++;
                            }
                            else
                            {
                                return result;
                            }
                        }
                        else
                        {
                            result.Complete += 1;
                        }
                    }
                    if (dataNotFoundCount >= 6)
                    {
                        return result;
                    }
                    else
                    {
                        result.ErrorFlag = false;
                    }

                    WriteCSVFile egWriteCSV = new WriteCSVFile();

                    egWriteCSV.IfExitDeleteFile(fullOutputFilePath);

                    doc.SaveAs(fullOutputFilePath);
                }
                #endregion

                #region Keep daily genereated document
                List<tbt_AccountingDocumentList> genComplete = new List<tbt_AccountingDocumentList>();
                genComplete.Add(new tbt_AccountingDocumentList()
                {
                    DocumentNo = documentNo,
                    DocumentCode = context.DocumentCode,
                    TargetPeriodFrom = context.TargetPeriodFrom.Value,
                    TargetPeriodTo = context.TargetPeriodTo.Value,
                    GenerateHQCode = context.UserHQCode,
                    ReportMonth = Convert.ToInt32(context.TargetPeriodTo.Value.ToString("MM")),
                    ReportYear = Convert.ToInt32(context.TargetPeriodTo.Value.ToString("yyyy")),
                    FilePath = outputfilepath,
                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                    CreateDate = DateTime.Now,
                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                    UpdateDate = DateTime.Now
                });
                this.Insert_tbt_AccountingDocumentList(CommonUtil.ConvertToXml_Store<tbt_AccountingDocumentList>(genComplete));
                #endregion

                scope.Complete();
                //Record info
                result.Complete = result.Total;
                result.ResultDocumentNoList = documentNo;

                return result;
            }
            catch (Exception ex)
            {
                    scope.Dispose();
                    throw;
            }
            }
        }

        private doGenerateDocumentResult GenerateACC011_Sheet1(DocumentContext context, SLDocument doc, string sheetname)
        {
            #region Prepare
            doGenerateDocumentResult result = new doGenerateDocumentResult()
            {
                ErrorFlag = false,
                Total = 0,
                Complete = 0,
                Failed = 0,
                ResultDocumentNoList = string.Empty
            };
            #endregion

            #region Constants
            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 3;
            int columncount = 0;
            int COL_ROWNUMBER = ++columncount;
            int COL_FULLNAMEEN = ++columncount;
            int COL_CONTRACTCODE = ++columncount;
            int COL_BILLINGTARGETCODE = ++columncount;
            int COL_BILLINGTYPECODE = ++columncount;
            int COL_INVOICENO = ++columncount;
            int COL_ISSUEINVDATE = ++columncount;
            int COL_BILLINGAMOUNTCURRENCY = ++columncount;
            int COL_BILLINGAMOUNT = ++columncount;
            int COL_VAT = ++columncount;
            int COL_TAXINVOICENO = ++columncount;
            int COL_TAXINVOICEDATE = ++columncount;
            int COL_FIRSTSECURITYSTARTDATE = ++columncount;
            int COL_PAYMENTDATE = ++columncount;
            int COL_MATCHDATE = ++columncount;
            #endregion

            if (doc.GetSheetNames().Contains(sheetname))
            {
                doc.DeleteWorksheet(sheetname);
            }
            doc.AddWorksheet(sheetname);
            doc.SelectWorksheet(sheetname);

            doc.SetCellStyle(ROW_HEADER, COL_HEADER_TITLE, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 12, Bold = true }
            });
            doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, "Payment List (BillingTypeCode 01 02 07)");

            doc.SetCellStyle(ROW_TBLHDR, COL_HEADER_TITLE, ROW_TBLHDR, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = true }
            });
            doc.SetCellValue(ROW_TBLHDR, COL_ROWNUMBER, "#");
            doc.SetCellValue(ROW_TBLHDR, COL_FULLNAMEEN, "FULLNAMEEN");
            doc.SetCellValue(ROW_TBLHDR, COL_CONTRACTCODE, "CONTRACTNO.");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTARGETCODE, "BILLINGCUSTOMERCODE");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTYPECODE, "BILLINGTYPECODE");
            doc.SetCellValue(ROW_TBLHDR, COL_INVOICENO, "INVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_ISSUEINVDATE, "ISSUEINVDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGAMOUNTCURRENCY, "BILLINGAMOUNTCURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGAMOUNT, "BILLINGAMOUNT");
            doc.SetCellValue(ROW_TBLHDR, COL_VAT, "VAT");
            doc.SetCellValue(ROW_TBLHDR, COL_TAXINVOICENO, "TAXINVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_TAXINVOICEDATE, "TAXINVOICEDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_FIRSTSECURITYSTARTDATE, "FIRSTSECURITYSTARTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_PAYMENTDATE, "PAYMENTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_MATCHDATE, "MATCHDATE");

            var reportdata = this.GetAccountDataOfACC011_Sheet1(context.TargetPeriodFrom, context.TargetPeriodTo, context.UserHQCode);
            if (reportdata.Count == 0)
            {
                result.ErrorFlag = true;
                result.ErrorCode = MessageUtil.MessageList.MSG8005;
                return result;
            }

            result.Complete = reportdata.Count;
            result.Total = reportdata.Count;

            int rowindex = ROW_TBLHDR + 1;
            doc.SetCellStyle(rowindex, COL_HEADER_TITLE, rowindex + reportdata.Count - 1, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = false }
            });
            foreach (var rowdata in reportdata)
            {
                if (rowdata.RowNumber != null) doc.SetCellValue(rowindex, COL_ROWNUMBER, rowdata.RowNumber.Value);
                doc.SetCellValue(rowindex, COL_FULLNAMEEN, rowdata.FullNameEN);
                doc.SetCellValue(rowindex, COL_CONTRACTCODE, rowdata.ContractCode);
                doc.SetCellValue(rowindex, COL_BILLINGTARGETCODE, rowdata.BillingTargetCode);
                doc.SetCellValue(rowindex, COL_BILLINGTYPECODE, rowdata.BillingTypeCode);
                doc.SetCellValue(rowindex, COL_INVOICENO, rowdata.InvoiceNo);
                if (rowdata.IssueInvDate != null) doc.SetCellValue(rowindex, COL_ISSUEINVDATE, rowdata.IssueInvDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.BillingAmountCurrencyType != null) doc.SetCellValue(rowindex, COL_BILLINGAMOUNTCURRENCY, rowdata.BillingAmountCurrencyType);
                if (rowdata.BillingAmount != null) doc.SetCellValue(rowindex, COL_BILLINGAMOUNT, rowdata.BillingAmount.Value);
                if (rowdata.VAT != null) doc.SetCellValue(rowindex, COL_VAT, rowdata.VAT.Value);
                doc.SetCellValue(rowindex, COL_TAXINVOICENO, rowdata.TaxInvoiceNo);
                if (rowdata.TaxInvoiceDate != null) doc.SetCellValue(rowindex, COL_TAXINVOICEDATE, rowdata.TaxInvoiceDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.FirstSecurityStartDate != null) doc.SetCellValue(rowindex, COL_FIRSTSECURITYSTARTDATE, rowdata.FirstSecurityStartDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.PaymentDate != null) doc.SetCellValue(rowindex, COL_PAYMENTDATE, rowdata.PaymentDate.Value.ToString("dd-MMM-yyyy"));
                doc.SetCellValue(rowindex, COL_MATCHDATE, rowdata.MatchDate.ToString("dd-MMM-yyyy"));

                rowindex++;
            }


            return result;
        }

        private doGenerateDocumentResult GenerateACC011_Sheet2(DocumentContext context, SLDocument doc, string sheetname)
        {
            #region Prepare
            doGenerateDocumentResult result = new doGenerateDocumentResult()
            {
                ErrorFlag = false,
                Total = 0,
                Complete = 0,
                Failed = 0,
                ResultDocumentNoList = string.Empty
            };
            #endregion

            #region Constants
            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 3;
            int columncount = 0;
            int COL_ROWNUMBER = ++columncount;
            int COL_FULLNAMEEN = ++columncount;
            int COL_CONTRACTCODE = ++columncount;
            int COL_BILLINGTARGETCODE = ++columncount;
            int COL_BILLINGTYPECODE = ++columncount;
            int COL_INVOICENO = ++columncount;
            int COL_ISSUEINVDATE = ++columncount;
            int COL_BILLINGAMOUNTCURRENCY = ++columncount;
            int COL_BILLINGAMOUNT = ++columncount;
            int COL_VAT = ++columncount;
            int COL_TAXINVOICENO = ++columncount;
            int COL_TAXINVOICEDATE = ++columncount;
            int COL_FIRSTSECURITYSTARTDATE = ++columncount;
            int COL_PAYMENTDATE = ++columncount;
            int COL_MATCHDATE = ++columncount;
            #endregion

            if (doc.GetSheetNames().Contains(sheetname))
            {
                doc.DeleteWorksheet(sheetname);
            }
            doc.AddWorksheet(sheetname);
            doc.SelectWorksheet(sheetname);

            doc.SetCellStyle(ROW_HEADER, COL_HEADER_TITLE, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 12, Bold = true }
            });
            doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, "Payment List (BillingTypeCode 11 12 17)");

            doc.SetCellStyle(ROW_TBLHDR, COL_HEADER_TITLE, ROW_TBLHDR, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = true }
            });
            doc.SetCellValue(ROW_TBLHDR, COL_ROWNUMBER, "#");
            doc.SetCellValue(ROW_TBLHDR, COL_FULLNAMEEN, "FULLNAMEEN");
            doc.SetCellValue(ROW_TBLHDR, COL_CONTRACTCODE, "CONTRACTNO.");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTARGETCODE, "BILLINGCUSTOMERCODE");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTYPECODE, "BILLINGTYPECODE");
            doc.SetCellValue(ROW_TBLHDR, COL_INVOICENO, "INVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_ISSUEINVDATE, "ISSUEINVDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGAMOUNTCURRENCY, "BILLINGAMOUNTCURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGAMOUNT, "BILLINGAMOUNT");
            doc.SetCellValue(ROW_TBLHDR, COL_VAT, "VAT");
            doc.SetCellValue(ROW_TBLHDR, COL_TAXINVOICENO, "TAXINVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_TAXINVOICEDATE, "TAXINVOICEDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_FIRSTSECURITYSTARTDATE, "FIRSTSECURITYSTARTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_PAYMENTDATE, "PAYMENTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_MATCHDATE, "MATCHDATE");

            var reportdata = this.GetAccountDataOfACC011_Sheet2(context.TargetPeriodFrom, context.TargetPeriodTo, context.UserHQCode);
            if (reportdata.Count == 0)
            {
                result.ErrorFlag = true;
                result.ErrorCode = MessageUtil.MessageList.MSG8005;
                return result;
            }

            result.Complete = reportdata.Count;
            result.Total = reportdata.Count;

            int rowindex = ROW_TBLHDR + 1;
            doc.SetCellStyle(rowindex, COL_HEADER_TITLE, rowindex + reportdata.Count - 1, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = false }
            });
            foreach (var rowdata in reportdata)
            {
                if (rowdata.RowNumber != null) doc.SetCellValue(rowindex, COL_ROWNUMBER, rowdata.RowNumber.Value);
                doc.SetCellValue(rowindex, COL_FULLNAMEEN, rowdata.FullNameEN);
                doc.SetCellValue(rowindex, COL_CONTRACTCODE, rowdata.ContractCode);
                doc.SetCellValue(rowindex, COL_BILLINGTARGETCODE, rowdata.BillingTargetCode);
                doc.SetCellValue(rowindex, COL_BILLINGTYPECODE, rowdata.BillingTypeCode);
                doc.SetCellValue(rowindex, COL_INVOICENO, rowdata.InvoiceNo);
                if (rowdata.IssueInvDate != null) doc.SetCellValue(rowindex, COL_ISSUEINVDATE, rowdata.IssueInvDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.BillingAmountCurrencyType != null) doc.SetCellValue(rowindex, COL_BILLINGAMOUNTCURRENCY, rowdata.BillingAmountCurrencyType);
                if (rowdata.BillingAmount != null) doc.SetCellValue(rowindex, COL_BILLINGAMOUNT, rowdata.BillingAmount.Value);
                if (rowdata.VAT != null) doc.SetCellValue(rowindex, COL_VAT, rowdata.VAT.Value);
                doc.SetCellValue(rowindex, COL_TAXINVOICENO, rowdata.TaxInvoiceNo);
                if (rowdata.TaxInvoiceDate != null) doc.SetCellValue(rowindex, COL_TAXINVOICEDATE, rowdata.TaxInvoiceDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.FirstSecurityStartDate != null) doc.SetCellValue(rowindex, COL_FIRSTSECURITYSTARTDATE, rowdata.FirstSecurityStartDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.PaymentDate != null) doc.SetCellValue(rowindex, COL_PAYMENTDATE, rowdata.PaymentDate.Value.ToString("dd-MMM-yyyy"));
                doc.SetCellValue(rowindex, COL_MATCHDATE, rowdata.MatchDate.ToString("dd-MMM-yyyy"));

                rowindex++;
            }

            return result;
        }

        private doGenerateDocumentResult GenerateACC011_Sheet3(DocumentContext context, SLDocument doc, string sheetname)
        {
            #region Prepare
            doGenerateDocumentResult result = new doGenerateDocumentResult()
            {
                ErrorFlag = false,
                Total = 0,
                Complete = 0,
                Failed = 0,
                ResultDocumentNoList = string.Empty
            };
            #endregion

            #region Constants
            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 3;
            int columncount = 0;
            int COL_ROWNUMBER = ++columncount;
            int COL_FULLNAMEEN = ++columncount;
            int COL_CONTRACTCODE = ++columncount;
            int COL_BILLINGTARGETCODE = ++columncount;
            int COL_BILLINGTYPECODE = ++columncount;
            int COL_INVOICENO = ++columncount;
            int COL_ISSUEINVDATE = ++columncount;
            int COL_BILLINGAMOUNTCURRENCY = ++columncount;
            int COL_BILLINGAMOUNT = ++columncount;
            int COL_VAT = ++columncount;
            int COL_TAXINVOICENO = ++columncount;
            int COL_TAXINVOICEDATE = ++columncount;
            int COL_FIRSTSECURITYSTARTDATE = ++columncount;
            int COL_PAYMENTDATE = ++columncount;
            int COL_MATCHDATE = ++columncount;
            #endregion

            if (doc.GetSheetNames().Contains(sheetname))
            {
                doc.DeleteWorksheet(sheetname);
            }
            doc.AddWorksheet(sheetname);
            doc.SelectWorksheet(sheetname);

            doc.SetCellStyle(ROW_HEADER, COL_HEADER_TITLE, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 12, Bold = true }
            });
            doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, "Payment List (BillingTypeCode 31 32 37)");

            doc.SetCellStyle(ROW_TBLHDR, COL_HEADER_TITLE, ROW_TBLHDR, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = true }
            });
            doc.SetCellValue(ROW_TBLHDR, COL_ROWNUMBER, "#");
            doc.SetCellValue(ROW_TBLHDR, COL_FULLNAMEEN, "FULLNAMEEN");
            doc.SetCellValue(ROW_TBLHDR, COL_CONTRACTCODE, "CONTRACTNO.");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTARGETCODE, "BILLINGCUSTOMERCODE");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTYPECODE, "BILLINGTYPECODE");
            doc.SetCellValue(ROW_TBLHDR, COL_INVOICENO, "INVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_ISSUEINVDATE, "ISSUEINVDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGAMOUNTCURRENCY, "BILLINGAMOUNTCURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGAMOUNT, "BILLINGAMOUNT");
            doc.SetCellValue(ROW_TBLHDR, COL_VAT, "VAT");
            doc.SetCellValue(ROW_TBLHDR, COL_TAXINVOICENO, "TAXINVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_TAXINVOICEDATE, "TAXINVOICEDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_FIRSTSECURITYSTARTDATE, "FIRSTSECURITYSTARTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_PAYMENTDATE, "PAYMENTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_MATCHDATE, "MATCHDATE");

            var reportdata = this.GetAccountDataOfACC011_Sheet3(context.TargetPeriodFrom, context.TargetPeriodTo, context.UserHQCode);

            if (reportdata.Count == 0)
            {
                result.ErrorFlag = true;
                result.ErrorCode = MessageUtil.MessageList.MSG8005;
                return result;
            }

            result.Complete = reportdata.Count;
            result.Total = reportdata.Count;

            int rowindex = ROW_TBLHDR + 1;
            doc.SetCellStyle(rowindex, COL_HEADER_TITLE, rowindex + reportdata.Count - 1, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = false }
            });
            foreach (var rowdata in reportdata)
            {
                if (rowdata.RowNumber != null) doc.SetCellValue(rowindex, COL_ROWNUMBER, rowdata.RowNumber.Value);
                doc.SetCellValue(rowindex, COL_FULLNAMEEN, rowdata.FullNameEN);
                doc.SetCellValue(rowindex, COL_CONTRACTCODE, rowdata.ContractCode);
                doc.SetCellValue(rowindex, COL_BILLINGTARGETCODE, rowdata.BillingTargetCode);
                doc.SetCellValue(rowindex, COL_BILLINGTYPECODE, rowdata.BillingTypeCode);
                doc.SetCellValue(rowindex, COL_INVOICENO, rowdata.InvoiceNo);
                if (rowdata.IssueInvDate != null) doc.SetCellValue(rowindex, COL_ISSUEINVDATE, rowdata.IssueInvDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.BillingAmountCurrencyType != null) doc.SetCellValue(rowindex, COL_BILLINGAMOUNTCURRENCY, rowdata.BillingAmountCurrencyType);
                if (rowdata.BillingAmount != null) doc.SetCellValue(rowindex, COL_BILLINGAMOUNT, rowdata.BillingAmount.Value);
                if (rowdata.VAT != null) doc.SetCellValue(rowindex, COL_VAT, rowdata.VAT.Value);
                doc.SetCellValue(rowindex, COL_TAXINVOICENO, rowdata.TaxInvoiceNo);
                if (rowdata.TaxInvoiceDate != null) doc.SetCellValue(rowindex, COL_TAXINVOICEDATE, rowdata.TaxInvoiceDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.FirstSecurityStartDate != null) doc.SetCellValue(rowindex, COL_FIRSTSECURITYSTARTDATE, rowdata.FirstSecurityStartDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.PaymentDate != null) doc.SetCellValue(rowindex, COL_PAYMENTDATE, rowdata.PaymentDate.Value.ToString("dd-MMM-yyyy"));
                doc.SetCellValue(rowindex, COL_MATCHDATE, rowdata.MatchDate.ToString("dd-MMM-yyyy"));

                rowindex++;
            }

            return result;
        }

        private doGenerateDocumentResult GenerateACC011_Sheet4(DocumentContext context, SLDocument doc, string sheetname)
        {
            #region Prepare
            doGenerateDocumentResult result = new doGenerateDocumentResult()
            {
                ErrorFlag = false,
                Total = 0,
                Complete = 0,
                Failed = 0,
                ResultDocumentNoList = string.Empty
            };
            #endregion

            #region Constants
            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 3;
            int columncount = 0;
            int COL_ROWNUMBER = ++columncount;
            int COL_FULLNAMEEN = ++columncount;
            int COL_CONTRACTCODE = ++columncount;
            int COL_BILLINGTARGETCODE = ++columncount;
            int COL_BILLINGTYPECODE = ++columncount;
            int COL_INVOICENO = ++columncount;
            int COL_ISSUEINVDATE = ++columncount;
            int COL_BILLINGAMOUNTCURRENCY = ++columncount;
            int COL_BILLINGAMOUNT = ++columncount;
            int COL_VAT = ++columncount;
            int COL_TAXINVOICENO = ++columncount;
            int COL_TAXINVOICEDATE = ++columncount;
            int COL_FIRSTSECURITYSTARTDATE = ++columncount;
            int COL_PAYMENTDATE = ++columncount;
            int COL_MATCHDATE = ++columncount;
            #endregion

            if (doc.GetSheetNames().Contains(sheetname))
            {
                doc.DeleteWorksheet(sheetname);
            }
            doc.AddWorksheet(sheetname);
            doc.SelectWorksheet(sheetname);

            doc.SetCellStyle(ROW_HEADER, COL_HEADER_TITLE, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 12, Bold = true }
            });
            doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, "Payment List (BillingTypeCode 45 46 47 85)");

            doc.SetCellStyle(ROW_TBLHDR, COL_HEADER_TITLE, ROW_TBLHDR, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = true }
            });
            doc.SetCellValue(ROW_TBLHDR, COL_ROWNUMBER, "#");
            doc.SetCellValue(ROW_TBLHDR, COL_FULLNAMEEN, "FULLNAMEEN");
            doc.SetCellValue(ROW_TBLHDR, COL_CONTRACTCODE, "CONTRACTNO.");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTARGETCODE, "BILLINGCUSTOMERCODE");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTYPECODE, "BILLINGTYPECODE");
            doc.SetCellValue(ROW_TBLHDR, COL_INVOICENO, "INVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_ISSUEINVDATE, "ISSUEINVDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGAMOUNTCURRENCY, "BILLINGAMOUNTCURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGAMOUNT, "BILLINGAMOUNT");
            doc.SetCellValue(ROW_TBLHDR, COL_VAT, "VAT");
            doc.SetCellValue(ROW_TBLHDR, COL_TAXINVOICENO, "TAXINVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_TAXINVOICEDATE, "TAXINVOICEDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_FIRSTSECURITYSTARTDATE, "FIRSTSECURITYSTARTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_PAYMENTDATE, "PAYMENTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_MATCHDATE, "MATCHDATE");

            var reportdata = this.GetAccountDataOfACC011_Sheet4(context.TargetPeriodFrom, context.TargetPeriodTo, context.UserHQCode);

            if (reportdata.Count == 0)
            {
                result.ErrorFlag = true;
                result.ErrorCode = MessageUtil.MessageList.MSG8005;
                return result;
            }

            result.Complete = reportdata.Count;
            result.Total = reportdata.Count;

            int rowindex = ROW_TBLHDR + 1;
            doc.SetCellStyle(rowindex, COL_HEADER_TITLE, rowindex + reportdata.Count - 1, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = false }
            });
            foreach (var rowdata in reportdata)
            {
                if (rowdata.RowNumber != null) doc.SetCellValue(rowindex, COL_ROWNUMBER, rowdata.RowNumber.Value);
                doc.SetCellValue(rowindex, COL_FULLNAMEEN, rowdata.FullNameEN);
                doc.SetCellValue(rowindex, COL_CONTRACTCODE, rowdata.ContractCode);
                doc.SetCellValue(rowindex, COL_BILLINGTARGETCODE, rowdata.BillingTargetCode);
                doc.SetCellValue(rowindex, COL_BILLINGTYPECODE, rowdata.BillingTypeCode);
                doc.SetCellValue(rowindex, COL_INVOICENO, rowdata.InvoiceNo);
                if (rowdata.IssueInvDate != null) doc.SetCellValue(rowindex, COL_ISSUEINVDATE, rowdata.IssueInvDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.BillingAmountCurrencyType != null) doc.SetCellValue(rowindex, COL_BILLINGAMOUNTCURRENCY, rowdata.BillingAmountCurrencyType);
                if (rowdata.BillingAmount != null) doc.SetCellValue(rowindex, COL_BILLINGAMOUNT, rowdata.BillingAmount.Value);
                if (rowdata.VAT != null) doc.SetCellValue(rowindex, COL_VAT, rowdata.VAT.Value);
                doc.SetCellValue(rowindex, COL_TAXINVOICENO, rowdata.TaxInvoiceNo);
                if (rowdata.TaxInvoiceDate != null) doc.SetCellValue(rowindex, COL_TAXINVOICEDATE, rowdata.TaxInvoiceDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.FirstSecurityStartDate != null) doc.SetCellValue(rowindex, COL_FIRSTSECURITYSTARTDATE, rowdata.FirstSecurityStartDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.PaymentDate != null) doc.SetCellValue(rowindex, COL_PAYMENTDATE, rowdata.PaymentDate.Value.ToString("dd-MMM-yyyy"));
                doc.SetCellValue(rowindex, COL_MATCHDATE, rowdata.MatchDate.ToString("dd-MMM-yyyy"));

                rowindex++;
            }

            return result;
        }

        private doGenerateDocumentResult GenerateACC011_Sheet5(DocumentContext context, SLDocument doc, string sheetname)
        {
            #region Prepare
            doGenerateDocumentResult result = new doGenerateDocumentResult()
            {
                ErrorFlag = false,
                Total = 0,
                Complete = 0,
                Failed = 0,
                ResultDocumentNoList = string.Empty
            };
            #endregion

            #region Constants
            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 3;
            int columncount = 0;
            int COL_ROWNUMBER = ++columncount;
            int COL_FULLNAMEEN = ++columncount;
            int COL_CONTRACTCODE = ++columncount;
            int COL_BILLINGTARGETCODE = ++columncount;
            int COL_BILLINGTYPECODE = ++columncount;
            int COL_INVOICENO = ++columncount;
            int COL_ISSUEINVDATE = ++columncount;
            int COL_BILLINGAMOUNTCURRENCY = ++columncount;
            int COL_BILLINGAMOUNT = ++columncount;
            int COL_VAT = ++columncount;
            int COL_TAXINVOICENO = ++columncount;
            int COL_TAXINVOICEDATE = ++columncount;
            int COL_FIRSTSECURITYSTARTDATE = ++columncount;
            int COL_PAYMENTDATE = ++columncount;
            int COL_MATCHDATE = ++columncount;
            #endregion

            if (doc.GetSheetNames().Contains(sheetname))
            {
                doc.DeleteWorksheet(sheetname);
            }
            doc.AddWorksheet(sheetname);
            doc.SelectWorksheet(sheetname);

            doc.SetCellStyle(ROW_HEADER, COL_HEADER_TITLE, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 12, Bold = true }
            });
            doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, "Payment List (BillingTypeCode 60 61 62 71 81 82 91)");

            doc.SetCellStyle(ROW_TBLHDR, COL_HEADER_TITLE, ROW_TBLHDR, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = true }
            });
            doc.SetCellValue(ROW_TBLHDR, COL_ROWNUMBER, "#");
            doc.SetCellValue(ROW_TBLHDR, COL_FULLNAMEEN, "FULLNAMEEN");
            doc.SetCellValue(ROW_TBLHDR, COL_CONTRACTCODE, "CONTRACTNO.");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTARGETCODE, "BILLINGCUSTOMERCODE");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTYPECODE, "BILLINGTYPECODE");
            doc.SetCellValue(ROW_TBLHDR, COL_INVOICENO, "INVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_ISSUEINVDATE, "ISSUEINVDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGAMOUNTCURRENCY, "BILLINGAMOUNTCURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGAMOUNT, "BILLINGAMOUNT");
            doc.SetCellValue(ROW_TBLHDR, COL_VAT, "VAT");
            doc.SetCellValue(ROW_TBLHDR, COL_TAXINVOICENO, "TAXINVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_TAXINVOICEDATE, "TAXINVOICEDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_FIRSTSECURITYSTARTDATE, "FIRSTSECURITYSTARTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_PAYMENTDATE, "PAYMENTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_MATCHDATE, "MATCHDATE");

            var reportdata = this.GetAccountDataOfACC011_Sheet5(context.TargetPeriodFrom, context.TargetPeriodTo, context.UserHQCode);

            if (reportdata.Count == 0)
            {
                result.ErrorFlag = true;
                result.ErrorCode = MessageUtil.MessageList.MSG8005;
                return result;
            }

            result.Complete = reportdata.Count;
            result.Total = reportdata.Count;

            int rowindex = ROW_TBLHDR + 1;
            doc.SetCellStyle(rowindex, COL_HEADER_TITLE, rowindex + reportdata.Count - 1, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = false }
            });
            foreach (var rowdata in reportdata)
            {
                if (rowdata.RowNumber != null) doc.SetCellValue(rowindex, COL_ROWNUMBER, rowdata.RowNumber.Value);
                doc.SetCellValue(rowindex, COL_FULLNAMEEN, rowdata.FullNameEN);
                doc.SetCellValue(rowindex, COL_CONTRACTCODE, rowdata.ContractCode);
                doc.SetCellValue(rowindex, COL_BILLINGTARGETCODE, rowdata.BillingTargetCode);
                doc.SetCellValue(rowindex, COL_BILLINGTYPECODE, rowdata.BillingTypeCode);
                doc.SetCellValue(rowindex, COL_INVOICENO, rowdata.InvoiceNo);
                if (rowdata.IssueInvDate != null) doc.SetCellValue(rowindex, COL_ISSUEINVDATE, rowdata.IssueInvDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.BillingAmountCurrencyType != null) doc.SetCellValue(rowindex, COL_BILLINGAMOUNTCURRENCY, rowdata.BillingAmountCurrencyType);
                if (rowdata.BillingAmount != null) doc.SetCellValue(rowindex, COL_BILLINGAMOUNT, rowdata.BillingAmount.Value);
                if (rowdata.VAT != null) doc.SetCellValue(rowindex, COL_VAT, rowdata.VAT.Value);
                doc.SetCellValue(rowindex, COL_TAXINVOICENO, rowdata.TaxInvoiceNo);
                if (rowdata.TaxInvoiceDate != null) doc.SetCellValue(rowindex, COL_TAXINVOICEDATE, rowdata.TaxInvoiceDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.FirstSecurityStartDate != null) doc.SetCellValue(rowindex, COL_FIRSTSECURITYSTARTDATE, rowdata.FirstSecurityStartDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.PaymentDate != null) doc.SetCellValue(rowindex, COL_PAYMENTDATE, rowdata.PaymentDate.Value.ToString("dd-MMM-yyyy"));
                doc.SetCellValue(rowindex, COL_MATCHDATE, rowdata.MatchDate.ToString("dd-MMM-yyyy"));

                rowindex++;
            }

            return result;
        }

        private doGenerateDocumentResult GenerateACC011_Sheet6(DocumentContext context, SLDocument doc, string sheetname)
        {
            #region Prepare
            doGenerateDocumentResult result = new doGenerateDocumentResult()
            {
                ErrorFlag = false,
                Total = 0,
                Complete = 0,
                Failed = 0,
                ResultDocumentNoList = string.Empty
            };
            #endregion

            #region Constants
            const int ROW_HEADER = 1;
            const int COL_HEADER_TITLE = 1;

            const int ROW_TBLHDR = 3;
            int columncount = 0;
            int COL_ROWNUMBER = ++columncount;
            int COL_FULLNAMEEN = ++columncount;
            int COL_CONTRACTCODE = ++columncount;
            int COL_BILLINGTARGETCODE = ++columncount;
            int COL_BILLINGTYPECODE = ++columncount;
            int COL_INVOICENO = ++columncount;
            int COL_ISSUEINVDATE = ++columncount;
            int COL_BILLINGAMOUNTCURRENCY = ++columncount;
            int COL_BILLINGAMOUNT = ++columncount;
            int COL_VAT = ++columncount;
            int COL_TAXINVOICENO = ++columncount;
            int COL_TAXINVOICEDATE = ++columncount;
            int COL_FIRSTSECURITYSTARTDATE = ++columncount;
            int COL_PAYMENTDATE = ++columncount;
            int COL_MATCHDATE = ++columncount;
            #endregion

            if (doc.GetSheetNames().Contains(sheetname))
            {
                doc.DeleteWorksheet(sheetname);
            }
            doc.AddWorksheet(sheetname);
            doc.SelectWorksheet(sheetname);

            doc.SetCellStyle(ROW_HEADER, COL_HEADER_TITLE, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 12, Bold = true }
            });
            doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, "Payment List (BillingTypeCode 99)");

            doc.SetCellStyle(ROW_TBLHDR, COL_HEADER_TITLE, ROW_TBLHDR, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = true }
            });
            doc.SetCellValue(ROW_TBLHDR, COL_ROWNUMBER, "#");
            doc.SetCellValue(ROW_TBLHDR, COL_FULLNAMEEN, "FULLNAMEEN");
            doc.SetCellValue(ROW_TBLHDR, COL_CONTRACTCODE, "CONTRACTNO.");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTARGETCODE, "BILLINGCUSTOMERCODE");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTYPECODE, "BILLINGTYPECODE");
            doc.SetCellValue(ROW_TBLHDR, COL_INVOICENO, "INVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_ISSUEINVDATE, "ISSUEINVDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGAMOUNTCURRENCY, "BILLINGAMOUNTCURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGAMOUNT, "BILLINGAMOUNT");
            doc.SetCellValue(ROW_TBLHDR, COL_VAT, "VAT");
            doc.SetCellValue(ROW_TBLHDR, COL_TAXINVOICENO, "TAXINVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_TAXINVOICEDATE, "TAXINVOICEDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_FIRSTSECURITYSTARTDATE, "FIRSTSECURITYSTARTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_PAYMENTDATE, "PAYMENTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_MATCHDATE, "MATCHDATE");

            var reportdata = this.GetAccountDataOfACC011_Sheet6(context.TargetPeriodFrom, context.TargetPeriodTo, context.UserHQCode);

            if (reportdata.Count == 0)
            {
                result.ErrorFlag = true;
                result.ErrorCode = MessageUtil.MessageList.MSG8005;
                return result;
            }

            result.Complete = reportdata.Count;
            result.Total = reportdata.Count;

            int rowindex = ROW_TBLHDR + 1;
            doc.SetCellStyle(rowindex, COL_HEADER_TITLE, rowindex + reportdata.Count - 1, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = false }
            });
            foreach (var rowdata in reportdata)
            {
                if (rowdata.RowNumber != null) doc.SetCellValue(rowindex, COL_ROWNUMBER, rowdata.RowNumber.Value);
                doc.SetCellValue(rowindex, COL_FULLNAMEEN, rowdata.FullNameEN);
                doc.SetCellValue(rowindex, COL_CONTRACTCODE, rowdata.ContractCode);
                doc.SetCellValue(rowindex, COL_BILLINGTARGETCODE, rowdata.BillingTargetCode);
                doc.SetCellValue(rowindex, COL_BILLINGTYPECODE, rowdata.BillingTypeCode);
                doc.SetCellValue(rowindex, COL_INVOICENO, rowdata.InvoiceNo);
                if (rowdata.IssueInvDate != null) doc.SetCellValue(rowindex, COL_ISSUEINVDATE, rowdata.IssueInvDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.BillingAmountCurrencyType != null) doc.SetCellValue(rowindex, COL_BILLINGAMOUNTCURRENCY, rowdata.BillingAmountCurrencyType);
                if (rowdata.BillingAmount != null) doc.SetCellValue(rowindex, COL_BILLINGAMOUNT, rowdata.BillingAmount.Value);
                if (rowdata.VAT != null) doc.SetCellValue(rowindex, COL_VAT, rowdata.VAT.Value);
                doc.SetCellValue(rowindex, COL_TAXINVOICENO, rowdata.TaxInvoiceNo);
                if (rowdata.TaxInvoiceDate != null) doc.SetCellValue(rowindex, COL_TAXINVOICEDATE, rowdata.TaxInvoiceDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.FirstSecurityStartDate != null) doc.SetCellValue(rowindex, COL_FIRSTSECURITYSTARTDATE, rowdata.FirstSecurityStartDate.Value.ToString("dd-MMM-yyyy"));
                if (rowdata.PaymentDate != null) doc.SetCellValue(rowindex, COL_PAYMENTDATE, rowdata.PaymentDate.Value.ToString("dd-MMM-yyyy"));
                doc.SetCellValue(rowindex, COL_MATCHDATE, rowdata.MatchDate.ToString("dd-MMM-yyyy"));

                rowindex++;
            }

            return result;
        }
    }
}
