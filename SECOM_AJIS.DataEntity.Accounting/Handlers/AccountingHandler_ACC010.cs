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
        public List<dtACC010> GetAccountDataOfACC010(DateTime? dtTargetPeriodFrom, DateTime? dtTargetPeriodTo, string strHQCode)
        {
            ACDataEntities context = new ACDataEntities();
            return context.GetAccountDataOfACC010(dtTargetPeriodFrom, dtTargetPeriodTo, strHQCode).ToList();
        }

        public doGenerateDocumentResult ACC010_NewOperationSalesReport(DocumentContext context)
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

                using (SLDocument doc = new SLDocument())
                {
                    //Sheet1
                    {
                        result = this.GenerateACC010_Sheet1(context, doc, "Sheet1");
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

        private doGenerateDocumentResult GenerateACC010_Sheet1(DocumentContext context, SLDocument doc, string sheetname)
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
            int COL_CUSTFULLNAMEEN = ++columncount;
            int COL_CONTRACTCODE = ++columncount;
            int COL_SITENAMEEN = ++columncount;
            int COL_PLANCODE = ++columncount;
            int COL_OFFICENAMEEN = ++columncount;
            int COL_CUSTACCEPTANCEDATE = ++columncount;
            int COL_PRODUCTNAMEEN = ++columncount;
            int COL_SALESMANNAME = ++columncount;
            int COL_ORDERPRODUCTPRICECURRENCY = ++columncount;
            int COL_ORDERPRODUCTPRICE = ++columncount;
            int COL_INSTRUMENTCOSTCURRENCY = ++columncount;
            int COL_INSTRUMENTCOST = ++columncount;
            int COL_ORDERINSTALLFEECURRENCY = ++columncount;
            int COL_ORDERINSTALLFEE = ++columncount;
            int COL_PAYTOSUBCONTRACTORCURRENCY = ++columncount;
            int COL_PAYTOSUBCONTRACTOR = ++columncount;
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
            doc.SetCellValue(ROW_HEADER, COL_HEADER_TITLE, "New operation sales report");

            doc.SetCellStyle(ROW_TBLHDR, COL_HEADER_TITLE, ROW_TBLHDR, columncount, new SLStyle()
            {
                Font = new SLFont() { FontName = "Tahoma", FontSize = 10, Bold = true }
            });
            doc.SetCellValue(ROW_TBLHDR, COL_ROWNUMBER, "NO.");
            doc.SetCellValue(ROW_TBLHDR, COL_CUSTFULLNAMEEN, "CUSTFULLNAMEEN");
            doc.SetCellValue(ROW_TBLHDR, COL_CONTRACTCODE, "CONTRACTNO.");
            doc.SetCellValue(ROW_TBLHDR, COL_SITENAMEEN, "PREMISE'SNAMEEN");
            doc.SetCellValue(ROW_TBLHDR, COL_PLANCODE, "PLANCODE");
            doc.SetCellValue(ROW_TBLHDR, COL_OFFICENAMEEN, "OFFICENAMEEN");
            doc.SetCellValue(ROW_TBLHDR, COL_CUSTACCEPTANCEDATE, "CUSTACCEPTANCEDATE");
            doc.SetCellValue(ROW_TBLHDR, COL_PRODUCTNAMEEN, "PRODUCTNAMEEN");
            doc.SetCellValue(ROW_TBLHDR, COL_SALESMANNAME, "SALESMANNAME");
            doc.SetCellValue(ROW_TBLHDR, COL_ORDERPRODUCTPRICECURRENCY, "ORDERPRODUCTPRICECURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_ORDERPRODUCTPRICE, "ORDERPRODUCTPRICE");
            doc.SetCellValue(ROW_TBLHDR, COL_INSTRUMENTCOSTCURRENCY, "INSTRUMENTCOSTCURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_INSTRUMENTCOST, "INSTRUMENTCOST");
            doc.SetCellValue(ROW_TBLHDR, COL_ORDERINSTALLFEECURRENCY, "ORDERINSTALLFEECURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_ORDERINSTALLFEE, "ORDERINSTALLFEE");
            doc.SetCellValue(ROW_TBLHDR, COL_PAYTOSUBCONTRACTORCURRENCY, "PAYTOSUBCONTRACTORCURRENCY");
            doc.SetCellValue(ROW_TBLHDR, COL_PAYTOSUBCONTRACTOR, "PAYTOSUBCONTRACTOR");

            var accountData = this.GetAccountDataOfACC010(context.TargetPeriodFrom, context.TargetPeriodTo, context.UserHQCode);
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
                if (rowdata.RowNumber != null) doc.SetCellValue(rowindex, COL_ROWNUMBER, rowdata.RowNumber.Value);
                doc.SetCellValue(rowindex, COL_CUSTFULLNAMEEN, rowdata.CustFullNameEN);
                doc.SetCellValue(rowindex, COL_CONTRACTCODE, rowdata.ContractCode);
                doc.SetCellValue(rowindex, COL_SITENAMEEN, rowdata.SiteNameEN);
                doc.SetCellValue(rowindex, COL_PLANCODE, rowdata.PlanCode);
                doc.SetCellValue(rowindex, COL_OFFICENAMEEN, rowdata.OfficeNameEN);
                if (rowdata.CustAcceptanceDate != null) doc.SetCellValue(rowindex, COL_CUSTACCEPTANCEDATE, rowdata.CustAcceptanceDate.Value.ToString("dd-MMM-yyyy"));
                doc.SetCellValue(rowindex, COL_PRODUCTNAMEEN, rowdata.ProductNameEN);
                doc.SetCellValue(rowindex, COL_SALESMANNAME, rowdata.SalesmanName);
                if (rowdata.OrderProductPriceCurrencyType != null) doc.SetCellValue(rowindex, COL_ORDERPRODUCTPRICECURRENCY, rowdata.OrderProductPriceCurrencyType);
                if (rowdata.OrderProductPrice != null) doc.SetCellValue(rowindex, COL_ORDERPRODUCTPRICE, rowdata.OrderProductPrice.Value);
                if (rowdata.InstrumentCostCurrencyType != null) doc.SetCellValue(rowindex, COL_INSTRUMENTCOSTCURRENCY, rowdata.InstrumentCostCurrencyType);
                if (rowdata.InstrumentCost != null) doc.SetCellValue(rowindex, COL_INSTRUMENTCOST, rowdata.InstrumentCost.Value);
                if (rowdata.OrderInstallFee != null) doc.SetCellValue(rowindex, COL_ORDERINSTALLFEE, rowdata.OrderInstallFee.Value);
                if (rowdata.OrderInstallFeeCurrencyType != null) doc.SetCellValue(rowindex, COL_ORDERINSTALLFEECURRENCY, rowdata.OrderInstallFeeCurrencyType);
                if (rowdata.PayToSubcontractor != null) doc.SetCellValue(rowindex, COL_PAYTOSUBCONTRACTOR, rowdata.PayToSubcontractor.Value);

                rowindex++;
            }

            doc.Filter(ROW_TBLHDR, COL_ROWNUMBER, rowindex-1, COL_PAYTOSUBCONTRACTOR);

            doc.SetCellValue(SLConvert.ToCellReference(rowindex, COL_ORDERPRODUCTPRICE), string.Format("=SUBTOTAL({0})", SLConvert.ToCellRange(ROW_TBLHDR+1, COL_ORDERPRODUCTPRICE, rowindex-1, COL_ORDERPRODUCTPRICE)));

            return result;
        }
    }
}
