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
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;

namespace SECOM_AJIS.DataEntity.Accounting.Handlers
{
    public partial class AccountingHandler
    {
        public List<dtACC009> GetAccountDataOfACC009(DateTime? dtTargetPeriodFrom, DateTime? dtTargetPeriodTo, string strHQCode)
        {
            ACDataEntities context = new ACDataEntities();
            return context.GetAccountDataOfACC009(dtTargetPeriodFrom, dtTargetPeriodTo, strHQCode).ToList();
        }

        public doGenerateDocumentResult ACC009_NewOperationRentalReport(DocumentContext context)
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

                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                Currencies = new List<doMiscTypeCode>(tmpCurrencies);

                using (SLDocument doc = new SLDocument())
                    {
                        //Sheet1
                        {
                        result = this.GenerateACC009_Sheet1(context, doc, "Sheet1");
                        if (result.ErrorFlag == true)
                        {
                            return result;
                        }
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

        private doGenerateDocumentResult GenerateACC009_Sheet1(DocumentContext context, SLDocument doc, string sheetname)
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
            int COL_BILLINGTARGETNAMEEN = ++columncount;
            int COL_SITENAMEEN = ++columncount;
            int COL_CONTRACTCODE = ++columncount;
            int COL_BILLINGTARGETCODE = ++columncount;
            int COL_PLANCODE = ++columncount;
            int COL_ORDERDEPOSITFEECURRENCY = ++columncount;
            int COL_ORDERDEPOSITFEE = ++columncount;
            int COL_ORDERINSTALLFEECURRENCY = ++columncount;
            int COL_ORDERINSTALLFEE = ++columncount;
            int COL_INSTALLFEEAMOUNTCURRENCY = ++columncount;
            int COL_INSTALLFEEAMOUNT = ++columncount;
            int COL_INSTALLFEEINVOICENO = ++columncount;
            int COL_ORDERCONTRACTFEECURRENCY = ++columncount;
            int COL_ORDERCONTRACTFEE = ++columncount;
            int COL_CONTRACTFEEAMOUNTCURRENCY = ++columncount;
            int COL_CONTRACTFEEAMOUNT = ++columncount;
            int COL_INVOICENO = ++columncount;
            int COL_BILLINGCYCLE = ++columncount;
            int COL_OFFICENAMEEN = ++columncount;
            int COL_FIRSTSECURITYSTARTDATE = ++columncount;
            int COL_PRODUCTNAMEEN = ++columncount;
            int COL_SALESMANNAME = ++columncount;
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
            doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, "New operation rental report");

            doc.SetCellStyle(ROW_TBLHDR, COL_HEADER_TITLE, ROW_TBLHDR, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = true }
            });
            doc.SetCellValue(ROW_TBLHDR, COL_ROWNUMBER, "NO.");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTARGETNAMEEN, "BILLINGCUSTOMERNAME");
            doc.SetCellValue(ROW_TBLHDR, COL_SITENAMEEN, "PREMISE'SNAME");
            doc.SetCellValue(ROW_TBLHDR, COL_CONTRACTCODE, "CONTRACTNO.");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGTARGETCODE, "BILLINGCUSTOMERCODE");
            doc.SetCellValue(ROW_TBLHDR, COL_PLANCODE, "PLANCODE");
            doc.SetCellValue(ROW_TBLHDR, COL_ORDERDEPOSITFEECURRENCY, "ORDERDEPOSITFEECURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_ORDERDEPOSITFEE, "ORDERDEPOSITFEE");
            doc.SetCellValue(ROW_TBLHDR, COL_ORDERINSTALLFEECURRENCY, "ORDERINSTALLFEECURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_ORDERINSTALLFEE, "ORDERINSTALLFEE");
            doc.SetCellValue(ROW_TBLHDR, COL_INSTALLFEEAMOUNTCURRENCY, "INSTALLFEEAMOUNTCURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_INSTALLFEEAMOUNT, "INSTALLFEEAMOUNT");
            doc.SetCellValue(ROW_TBLHDR, COL_INSTALLFEEINVOICENO, "INSTALLFEEINVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_ORDERCONTRACTFEECURRENCY, "ORDERCONTRACTFEECURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_ORDERCONTRACTFEE, "ORDERCONTRACTFEE");
            doc.SetCellValue(ROW_TBLHDR, COL_CONTRACTFEEAMOUNTCURRENCY, "CONTRACTFEEAMOUNTCURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_CONTRACTFEEAMOUNT, "CONTRACTFEEAMOUNT");
            doc.SetCellValue(ROW_TBLHDR, COL_INVOICENO, "INVOICENO");
            doc.SetCellValue(ROW_TBLHDR, COL_BILLINGCYCLE, "BILLINGCYCLE");
            doc.SetCellValue(ROW_TBLHDR, COL_OFFICENAMEEN, "OFFICENAMEEN");
            doc.SetCellValue(ROW_TBLHDR, COL_FIRSTSECURITYSTARTDATE, "FIRSTSECURITYSTARTDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_PRODUCTNAMEEN, "PRODUCTNAMEEN");
            doc.SetCellValue(ROW_TBLHDR, COL_SALESMANNAME, "SALESMANNAME");

            List<dtACC009> accountData = this.GetAccountDataOfACC009(context.TargetPeriodFrom, context.TargetPeriodTo, context.UserHQCode);
            if (accountData.Count == 0)
            {
                result.ErrorFlag = true;
                result.ErrorCode = MessageUtil.MessageList.MSG8005;
                return result;
            }

            result.Complete = accountData.Count;
            result.Total = accountData.Count;

            int rowindex = ROW_TBLHDR + 1;
            doc.SetCellStyle(rowindex, COL_HEADER_TITLE, rowindex + accountData.Count - 1, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = false }
            });
            foreach (var rowdata in accountData)
            {
                if (rowdata.No != null) doc.SetCellValue(rowindex, COL_ROWNUMBER, rowdata.No.Value);
                doc.SetCellValue(rowindex, COL_BILLINGTARGETNAMEEN, rowdata.BillingTargetNameEN);
                doc.SetCellValue(rowindex, COL_SITENAMEEN, rowdata.SiteNameEN);
                doc.SetCellValue(rowindex, COL_CONTRACTCODE, rowdata.ContractCode);
                doc.SetCellValue(rowindex, COL_BILLINGTARGETCODE, rowdata.BillingTargetCode);
                doc.SetCellValue(rowindex, COL_PLANCODE, rowdata.PlanCode);
                if (rowdata.OrderDepositFeeCurrencyType != null) doc.SetCellValue(rowindex, COL_ORDERCONTRACTFEECURRENCY, this.CurrencyName(rowdata.OrderContractFeeCurrencyType));
                if (rowdata.OrderDepositFee != null) doc.SetCellValue(rowindex, COL_ORDERDEPOSITFEE, rowdata.OrderDepositFee.Value);
                if (rowdata.OrderInstallFeeCurrencyType != null) doc.SetCellValue(rowindex, COL_ORDERINSTALLFEECURRENCY, this.CurrencyName(rowdata.OrderInstallFeeCurrencyType));
                if (rowdata.OrderInstallFee != null) doc.SetCellValue(rowindex, COL_ORDERINSTALLFEE, rowdata.OrderInstallFee.Value);
                if (rowdata.InstallFeeAmountCurrencyType != null) doc.SetCellValue(rowindex, COL_INSTALLFEEAMOUNTCURRENCY, this.CurrencyName(rowdata.InstallFeeAmountCurrencyType));
                if (rowdata.InstallFeeAmount != null) doc.SetCellValue(rowindex, COL_INSTALLFEEAMOUNT, rowdata.InstallFeeAmount.Value);
                doc.SetCellValue(rowindex, COL_INSTALLFEEINVOICENO, rowdata.InstallFeeInvoiceNo);
                if (rowdata.OrderContractFeeCurrencyType != null)doc.SetCellValue(rowindex, COL_ORDERCONTRACTFEECURRENCY, this.CurrencyName(rowdata.OrderContractFeeCurrencyType));
                if (rowdata.OrderContractFee != null) doc.SetCellValue(rowindex, COL_ORDERCONTRACTFEE, rowdata.OrderContractFee.Value);
                if (rowdata.ContractFeeAmountCurrencyType != null) doc.SetCellValue(rowindex, COL_CONTRACTFEEAMOUNTCURRENCY, this.CurrencyName(rowdata.ContractFeeAmountCurrencyType));
                if (rowdata.ContractFeeAmount != null) doc.SetCellValue(rowindex, COL_CONTRACTFEEAMOUNT, rowdata.ContractFeeAmount.Value);
                doc.SetCellValue(rowindex, COL_INVOICENO, rowdata.InvoiceNo);
                if (rowdata.BillingCycle != null) doc.SetCellValue(rowindex, COL_BILLINGCYCLE, rowdata.BillingCycle.Value);
                doc.SetCellValue(rowindex, COL_OFFICENAMEEN, rowdata.OfficeNameEN);
                if (rowdata.FirstSecurityStartDate != null) doc.SetCellValue(rowindex, COL_FIRSTSECURITYSTARTDATE, rowdata.FirstSecurityStartDate.Value);
                doc.SetCellValue(rowindex, COL_PRODUCTNAMEEN, rowdata.ProductNameEN);
                doc.SetCellValue(rowindex, COL_SALESMANNAME, rowdata.SalesmanName);

                rowindex++;
            }
            result.Total += accountData.Count;

            return result;
        }
    }
}
