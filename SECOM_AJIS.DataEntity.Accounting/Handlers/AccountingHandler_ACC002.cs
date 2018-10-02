using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;

namespace SECOM_AJIS.DataEntity.Accounting.Handlers
{
    public partial class AccountingHandler
    {

        public List<dtAccountDataOfAgingReport> GetAccountDataOfAgingReport(DateTime? dtTargetPeriod, DateTime? dtLastEndDate, string strHQCode)
        {
            ACDataEntities context = new ACDataEntities();
            return context.GetAccountDataOfAgingReport(dtTargetPeriod, dtLastEndDate, strHQCode, MiscType.C_RENTAL_CHANGE_TYPE, MiscType.C_SALE_CHANGE_TYPE, MiscType.C_PAYMENT_METHOD, MiscType.C_SHOW_DUEDATE).ToList();
        }

        public List<dtAccountingBusinessDateOfAgingReport> GetAccountingBusinessDateOfAgingReport(Nullable<System.DateTime> targetPeriodTo)
        {
            ACDataEntities context = new ACDataEntities();
            return context.GetAccountingBusinessDateOfAgingReport(targetPeriodTo).ToList();
        }

        public doGenerateDocumentResult ACC002_AgingReport(DocumentContext context)
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

            using (TransactionScope scope = new TransactionScope())
            {

            try
            {
                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                #region Do process
                //Get business date parameter
                dtAccountingBusinessDateOfAgingReport businessDate = this.GetAccountingBusinessDateOfAgingReport(context.TargetPeriodTo).FirstOrDefault();
                dtAccountingConfig folder = this.getAccountingConfig(AccountingConfig.C_ACC_CONFIG_GROUP_CSV, AccountingConfig.C_ACC_CONFIG_NAME_FOLDER);

                #region Generate report
                //Get account data
                List<dtAccountDataOfAgingReport> accountData = this.GetAccountDataOfAgingReport(businessDate.TargetPeriod.Value, businessDate.LastEndDate.Value, context.UserHQCode);

                if (accountData.Count == 0)
                {
                    result.ErrorFlag = true;
                    result.ErrorCode = MessageUtil.MessageList.MSG8005;
                    return result;
                }

                for (int i = 0; i < accountData.Count(); i++)
                {
                    accountData[i].Currencies = new List<doMiscTypeCode>(tmpCurrencies);
                }
                #region Write text report
                //Format: ..\YYYYMM\[CSV.Folder]\ACC002YYYYMMDDHQCD.csv
                string textContent = CSVReportUtil.GenerateAccountingCSVData<dtAccountDataOfAgingReport>(accountData, true, true, "No.");
                string documentNo = string.Format(@"{0}{1}{2}", context.DocumentCode, businessDate.TargetPeriod.Value.ToString("yyyyMMdd"), context.UserHQCode);
                string outputfilepath = string.Format(@"{0}{1}\{2}\{3}.csv", businessDate.ReportYear, businessDate.ReportMonth, folder.ConfigValue, documentNo);
                string fullOutputFilePath = PathUtil.GetPathValue(PathUtil.PathName.GeneratedReportPath, outputfilepath);

                WriteCSVFile egWriteCSV = new WriteCSVFile();

                egWriteCSV.IfExitDeleteFile(fullOutputFilePath);

                using (StreamWriter sw = new StreamWriter(fullOutputFilePath, false, Encoding.GetEncoding("TIS-620")))
                {
                    sw.WriteLine(textContent);
                    sw.Close();
                }

                #endregion

                #region Keep daily genereated document
                List<tbt_AccountingDocumentList> genComplete = new List<tbt_AccountingDocumentList>();
                genComplete.Add(new tbt_AccountingDocumentList()
                {
                    DocumentNo = documentNo,
                    DocumentCode = context.DocumentCode,
                    TargetPeriodFrom = businessDate.TargetPeriod.Value,
                    TargetPeriodTo = businessDate.TargetPeriod.Value,
                    GenerateHQCode = context.UserHQCode,
                    ReportMonth = Convert.ToInt32(businessDate.ReportMonth),
                    ReportYear = Convert.ToInt32(businessDate.ReportYear),
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
                    result.Total += accountData.Count;
                    result.Complete = result.Total;
                result.ResultDocumentNoList = documentNo;
                    return result;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw;
                }
                #endregion
                #endregion
            }
        }

    }
}
