using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Transactions;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;

namespace SECOM_AJIS.DataEntity.Accounting.Handlers
{
    public partial class AccountingHandler
    {
        public List<dtAccountDataOfVATReport> GetAccountDataOfVATReport(DateTime? dtTargetPeriodFrom, DateTime? dtTargetPeriodTo, string strHQCode)
        {
            ACDataEntities context = new ACDataEntities();
            return context.GetAccountDataOfVATReport(dtTargetPeriodFrom, dtTargetPeriodTo, strHQCode).ToList();
        }

        public List<dtAccountingBusinessDateOfVATReport> GetAccountingBusinessDateOfVATReport(Nullable<System.DateTime> targetPeriodFrom, Nullable<System.DateTime> targetPeriodTo)
        {
            ACDataEntities context = new ACDataEntities();
            return context.GetAccountingBusinessDateOfVATReport(targetPeriodFrom, targetPeriodTo).ToList();
        }

        public doGenerateDocumentResult ACC008_VATReport(DocumentContext context)
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

                #region Do process
                //Get business date parameter
                dtAccountingBusinessDateOfVATReport businessDate = this.GetAccountingBusinessDateOfVATReport(context.TargetPeriodFrom, context.TargetPeriodTo).FirstOrDefault();
                dtAccountingConfig folder = this.getAccountingConfig(AccountingConfig.C_ACC_CONFIG_GROUP_CSV, AccountingConfig.C_ACC_CONFIG_NAME_FOLDER);

                List<dtAccountingConfig> flagDisplay = this.getAccountingConfigList(AccountingConfig.C_ACC_CONFIG_GROUP_FLAG_DISPLAY, null);

                ICommonHandler comHand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                List<doMiscTypeCode> tmpCurrencies = comHand.GetMiscTypeCodeList(new List<doMiscTypeCode>()
                                                        {
                                                            new doMiscTypeCode()
                                                            {
                                                                FieldName = MiscType.C_CURRENCT,
                                                                ValueCode = "%"
                                                            }
                                                        }).ToList();

                #region Generate report
                //Get account data
                List<dtAccountDataOfVATReport> accountData = this.GetAccountDataOfVATReport(businessDate.TargetPeriodFrom.Value, businessDate.TargetPeriodTo.Value, context.UserHQCode);

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
                for (int i = 0; i < accountData.Count(); i++)
                {
                    accountData[i].FlagDisplay = flagDisplay;
                }

                #region Write text report
                //Format: ..\YYYYMM\[CSV.Folder]\ACC002YYYYMMDDHQCD.csv
                string textContent = CSVReportUtil.GenerateAccountingCSVData<dtAccountDataOfVATReport>(accountData, true, true, "No.");
                string documentNo = string.Format(@"{0}{1}{2}{3}", context.DocumentCode, businessDate.TargetPeriodFrom.Value.ToString("yyyyMMdd"), businessDate.TargetPeriodTo.Value.ToString("yyyyMMdd"), context.UserHQCode);
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
                    TargetPeriodFrom = businessDate.TargetPeriodFrom.Value,
                    TargetPeriodTo = businessDate.TargetPeriodTo.Value,
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
