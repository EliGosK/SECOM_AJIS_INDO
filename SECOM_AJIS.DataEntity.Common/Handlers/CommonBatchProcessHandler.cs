using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net.Mail;
using System.Configuration;
using System.Transactions;
using System.Diagnostics;
using System.Security.Cryptography;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using CSI.WindsorHelper;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.DataEntity.Common
{
    public class CommonBatchProcessHandler : BizCMDataEntities
    {
        #region Get data



        #endregion

        #region Batch Process

        // CMP120
        /// <summary>
        /// Batch Printing Service Process (call sp_CM_DocumentListForPrining)
        /// </summary>
        /// <param name="UserId">User id</param>
        /// <param name="BatchDate">Batch date</param>
        /// <returns></returns>
        public doBatchProcessResult CMP120_PrintingServiceProcess(string UserId, DateTime BatchDate)
        {
            doBatchProcessResult result = new doBatchProcessResult();
            DocumentHandler documentHandler = new DocumentHandler();
            ILogHandler logHandler = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler; //Add by Jutarat A. on 17092013

            //--- variable           
            string pathFoxit = ConfigurationManager.AppSettings["PrintPDFFoxit"];
            string pathFileName = string.Empty;

            List<dtDocumentListForPrining> dtDocumentListForPriningData = documentHandler.GetDocumentListForPrining(DocumentType.C_DOCUMENT_TYPE_INCOME, DocumentType.C_DOCUMENT_TYPE_COMMON, BatchDate, null, null);
            if (dtDocumentListForPriningData != null && dtDocumentListForPriningData.Count > 0)
            {
                // Initial value of doBatchProcessResult
                result.Result = false;
                result.Total = dtDocumentListForPriningData.Count;
                result.Complete = 0;
                result.Failed = 0;
                result.ErrorMessage = string.Empty;

                //logHandler.SearchFoxitProcess("CMP120"); //Add by Jutarat A. on 20082013
            //}

                //Add by Jutarat A. on 17092013
                string strErrorMessage = string.Empty;
                bool bResult = comHandler.AllocatePrintingProcess(PrintingFlag.C_PRINTING_FLAG_CMP120, ProcessID.C_PROCESS_ID_PRINTING_SERVICE, ref strErrorMessage);
                if (bResult)
                //End Add
                {
                    foreach (dtDocumentListForPrining list in dtDocumentListForPriningData)
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            try
                            {
                                //3.1
                                doDocumentDownloadLog cond = new doDocumentDownloadLog();
                                cond.DocumentNo = list.DocumentNo;
                                cond.DocumentCode = list.DocumentCode;
                                cond.DocumentOCC = list.DocumentOCC;
                                cond.DownloadDate = BatchDate;
                                cond.DownloadBy = ProcessID.C_PROCESS_ID_PRINTING_SERVICE;
                                logHandler.WriteDocumentDownloadLog(cond);

                                //============ Teerapong 23/07/2012 ===============
                                billingHandler.UpdateFirstIssue(list.DocumentNo, cond.DocumentOCC, BatchDate, UserId);
                                //=================================================

                                //3.2 Send data to specified printer to print documents                    
                                //PrintPDF(list.FilePathDocument);
                                comHandler.PrintPDF(list.FilePathDocument); //Modify by Jutarat A. on 17092013

                                //3.3
                                scope.Complete();
                                result.Complete++;
                            }
                            catch (Exception ex)
                            {
                                scope.Dispose();
                                result.Failed++;
                                result.ErrorMessage += string.Format("DocumentNo {0} DocumentCode {1} DocumentOCC {2} has Error : {3} {4}\n", list.DocumentNo, list.DocumentCode, list.DocumentOCC, ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");

                                break; //Add by Jutarat A. on 03072013
                            }
                        }
                    }

                    bResult = comHandler.ResetPrintingProcess(ProcessID.C_PROCESS_ID_PRINTING_SERVICE); //Add by Jutarat A. on 17092013
                }
                //Add by Jutarat A. on 17092013
                else
                {
                    result.Failed = result.Total;
                    if (String.IsNullOrEmpty(strErrorMessage) == false)
                        result.ErrorMessage = strErrorMessage;
                }
                //End Add
            }

            result.Result = (result.Failed == 0);
            result.BatchStatus = (result.Result ? BatchStatus.C_BATCH_STATUS_SUCCEEDED : BatchStatus.C_BATCH_STATUS_FAILED);
            return result;
        }

        // CMP130
        /// <summary>
        /// Batch Generate Issue List Process (sp_CM_GetBillingOffice, sp_CM_GetMaxmanagementNo, sp_CM_GetNextIssueListNo, )
        /// </summary>
        /// <param name="UserId">User id</param>
        /// <param name="BatchDate">Batch date</param>
        /// <returns></returns>
        public doBatchProcessResult CMP130_GenerateIssueListProcess(string UserId, DateTime BatchDate)
        {
            doBatchProcessResult result = new doBatchProcessResult();
            DocumentHandler documentHandler = new DocumentHandler();
            List<dtBillingOffice> dtBillingOfficeList = documentHandler.GetBillingOffice(DocumentType.C_DOCUMENT_TYPE_INCOME, (DateTime?)BatchDate);
            
            if (dtBillingOfficeList.Count > 0)
            {
                // Initial value of doBatchProcessResult
                result.Result = false;

                //Comment by Jutarat A. on 04012013 (Move to set data after process)
                //result.Total = dtBillingOfficeList.Count;
                //result.Complete = 0;
                //result.Failed = 0;
                //End Comment

                result.ErrorMessage = string.Empty;
            }

            //Add by Jutarat A. on 04012013
            int iTotalCount = 0; 
            int iSuccessCount = 0;
            //End Add

            foreach (dtBillingOffice list in dtBillingOfficeList)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //Add by Jutarat A. on 04012013
                    bool hasIssueList = false;
                    bool isSuccess = false;
                    //End Add

                    try
                    {
                        //1.2.2 Get max No
                        List<int?> iMaxManagementNo = documentHandler.GetMaxManagementNo(DocumentType.C_DOCUMENT_TYPE_INCOME, list.Issuedate);

                        if (iMaxManagementNo.Count > 0)
                        {
                            //Name Code IssueListNo - yyyymmdd
                            string strIssueNo = documentHandler.GetNextIssueListNo("IssueListNo-" + DateTime.Now.ToString("yyyyMMdd"));
                            string pdfReport = documentHandler.GenerateCMR010FilePath(strIssueNo, list.BillingOfficeCode, iMaxManagementNo[0], ProcessID.C_PROCESS_ID_GENERATE_ISSUE_LIST, list.Issuedate.Value, ref hasIssueList, ref isSuccess); //Modify by Jutarat A. (Add isSuccess) on 04012013

                            //Complete
                            scope.Complete();
                            result.Complete++;
                        }
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        result.Failed++;
                        result.ErrorMessage += string.Format("BillingOfficeCode {0} has Error : {1} {2}\n", list.BillingOfficeCode, ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
                    }

                    //Add by Jutarat A. on 04012013
                    if (hasIssueList)
                    {
                        iTotalCount++;

                        if (isSuccess)
                            iSuccessCount++;
                    }
                    //End Add
                }
            }

            //Add by Jutarat A. on 04012013
            result.Total = iTotalCount;
            result.Complete = iSuccessCount;
            result.Failed = (iTotalCount - iSuccessCount);
            //End Add

            result.Result = (result.Failed == 0);
            result.BatchStatus = (result.Result?BatchStatus.C_BATCH_STATUS_SUCCEEDED:BatchStatus.C_BATCH_STATUS_FAILED);
            return result;
        }

        // CMP140
        /// <summary>
        /// CMP140: Batch generate account data of carry-over & Profit Process
        /// </summary>
        /// <param name="UserId">User id</param>
        /// <param name="BatchDate">Batch date</param>
        /// <returns></returns>
        public doBatchProcessResult CMP140_GenerateAccountDataOfCarryOverAndProfitProcess(string UserId, DateTime BatchDate)
        {
            #region Prepare
            doBatchProcessResult result = new doBatchProcessResult()
            {
                Total = 0,
                Complete = 0,
                Failed = 0,
                ErrorMessage = string.Empty
            };
            CommonHandler commonHandler = new CommonHandler();
            #endregion

            #region Do process 
            //Get business date parameter
            dtBusinessDateForAccountDataOfCarryOverAndProfit businessDate = commonHandler.GetBusinessDateForAccountDataOfCarryOverAndProfitProcess((DateTime?)BatchDate).FirstOrDefault();
            if (BatchDate.Date >= businessDate.FiveBusinessDate.Value.Date && businessDate.RecordCount == 0)
            {
                #region Generate account data of carry-over & Profit
                //Get account data by each product type code
                using (TransactionScope scope = new TransactionScope())
                {
                    //Product type code list
                    string[] productTypeCodes = { GroupProductType.C_GROUP_PRODUCT_TYPE_N, 
                                                   GroupProductType.C_GROUP_PRODUCT_TYPE_SG ,
                                                   GroupProductType.C_GROUP_PRODUCT_TYPE_MA
                                               };
                    //current process product type code
                    string processProductTypeCode = string.Empty;
                    try
                    {
                        IDocumentHandler documentHandler = ServiceContainer.GetService<IDocumentHandler>() as IDocumentHandler;
                        foreach (string productTypeCode in productTypeCodes)
                        {
                            //Set current process productTypeCode
                            processProductTypeCode = productTypeCode;

                            //Get account data
                            List<dtAccountDataOfCarryOverAndProfit> accountData = commonHandler.GetAccountDataOfCarryOverAndProfit(
                                businessDate.StartTargetDate.Value,
                                businessDate.EndTargetDate.Value,
                                businessDate.FiveBusinessDate.Value,
                                productTypeCode);

                            //Freeze data to tbt_ManageCarryOverProfit
                            List<tbt_ManageCarryOverProfit> dtManageCarryPreInsert = new List<tbt_ManageCarryOverProfit>();
                            foreach (dtAccountDataOfCarryOverAndProfit dtAccountData in accountData)
                            {
                                tbt_ManageCarryOverProfit dtManageCarryItem = new tbt_ManageCarryOverProfit()
                                {
                                    ReportYear = businessDate.ReportYear,
                                    ReportMonth = businessDate.ReportMonth,
                                    ProductType = productTypeCode,
                                    ContractCode = dtAccountData.ContractCode,
                                    BillingOCC = dtAccountData.BillingOCC,
                                    BillingTargetCode = dtAccountData.BillingTargetCode,
                                    BillingOfficeCode = dtAccountData.BillingOfficeCode,
                                    BillingOfficeName = dtAccountData.BillingOfficeName,
                                    BillingTargetName = dtAccountData.BillingTargetName,
                                    FirstOperationDate = dtAccountData.FirstOperationDate,
                                    MonthlyBillingAmount = dtAccountData.MonthlyBillingAmount,
                                    LastAccumulatedReceiveAmount = dtAccountData.LastAccumulatedReceiveAmount,
                                    LastAccumulatedUnpaid = dtAccountData.LastAccumulatedUnpaid,
                                    ReceiveAmount = dtAccountData.ReceiveAmount,
                                    IncomeRentalFee = dtAccountData.IncomeRentalFee,
                                    AccumulatedReceiveAmount = dtAccountData.AccumulatedReceiveAmount,
                                    AccumulatedUnpaid = dtAccountData.AccumulatedUnpaid,
                                    IncomeVat = dtAccountData.IncomeVat,
                                    UnpaidPeriod = dtAccountData.UnpaidPeriod,
                                    IncomeDate = dtAccountData.IncomeDate,
                                    ContractStatus = dtAccountData.ContractStatus,

                                    CalDailyFeeStatus = dtAccountData.CalDailyFeeStatus,
                                    StartTargetDate = businessDate.StartTargetDate,
                                    EndTargetDate = businessDate.EndTargetDate,
                                    FiveBusinessDate = businessDate.FiveBusinessDate,

                                    CreateDate = BatchDate,
                                    CreateBy = ProcessID.C_PROCESS_ID_GEN_ACCOUNT_CARRY_OVER_AND_PROFIT,
                                    UpdateDate = BatchDate,
                                    UpdateBy = ProcessID.C_PROCESS_ID_GEN_ACCOUNT_CARRY_OVER_AND_PROFIT
                                };
                                dtManageCarryPreInsert.Add(dtManageCarryItem);
                            }
                            //Insert freeze data into db
                            commonHandler.InsertTbt_ManageCarryOverProfit(CommonUtil.ConvertToXml_Store<tbt_ManageCarryOverProfit>(dtManageCarryPreInsert));

                            //Update billing history 
                            commonHandler.UpdateBillingHistoryOfManageCarryOverProfit(businessDate.ReportYear, businessDate.ReportMonth, productTypeCode, (DateTime?) BatchDate, ProcessID.C_PROCESS_ID_GEN_ACCOUNT_CARRY_OVER_AND_PROFIT);

                            //Genereate CMR020 report (Csv formatted)
                            documentHandler.GenerateCMR020FilePath(businessDate.ReportYear, businessDate.ReportMonth, productTypeCode, ProcessID.C_PROCESS_ID_GEN_ACCOUNT_CARRY_OVER_AND_PROFIT, BatchDate);

                            //Record info
                            result.Total += accountData.Count;
                        }

                        //Completed
                        scope.Complete();
                        result.Complete = result.Total;
                    }
                    catch (Exception ex)
                    {
                        //All failed
                        scope.Dispose();
                        result.Failed = result.Total;
                        result.ErrorMessage += string.Format("Product type code {0} has Error : {1} {2}\n", processProductTypeCode, ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
                        result.Result = FlagType.C_FLAG_OFF;
                        result.BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED;
                        return result;
                    }
                }
                #endregion
            }
            else
            {
                if (businessDate.RecordCount > 0)
                {
                    #region Already generated data
                    result.Result = FlagType.C_FLAG_ON;
                    result.BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED;
                    result.ErrorMessage = "Generate data already";
                    return result;
                    #endregion
                }
            }
            #endregion
            
            //All success, include no process case, no error case
            result.Result = FlagType.C_FLAG_ON;
            result.BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED;
            return result;
        }


        //Print PDF by Foxit
        public void PrintPDF(string strPathFilename)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = ConfigurationManager.AppSettings["PrintPDFFoxit"];
                //process.StartInfo.Arguments = "/p " + @strPathFilename;
                process.StartInfo.Arguments = string.Format("/t \"{0}\" \"{1}\"", strPathFilename, ConfigurationManager.AppSettings["PrinterName"]); //Modify by Jutarat A. on 12072013
                process.StartInfo.Verb = "Print";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                //Modify by Jutarat A. on 12072013
                //process.WaitForExit();
                int intPrintTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["PrintTimeOut"]);
                if (process.WaitForExit(intPrintTimeOut) == false) //Wait a maximum of 1 min for the process to finish
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                        process.Dispose();
                    }

                    throw new Exception("Print Timeout");
                }
                //End Modify
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion
    }

    #region Class process

    #region CMP120
    public class CMP120_PrintingService : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            CommonBatchProcessHandler batch = new CommonBatchProcessHandler();
            return batch.CMP120_PrintingServiceProcess(UserId, BatchDate);
        }
    }
    #endregion

    #region CMP130
    public class CMP130_GenerateIssueList : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            CommonBatchProcessHandler batch = new CommonBatchProcessHandler();
            return batch.CMP130_GenerateIssueListProcess(UserId, BatchDate);
        }
    }
    #endregion

    #region CMP140
    public class CMP140_GenerateAccountDataOfCarryOverAndProfit : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            CommonBatchProcessHandler batch = new CommonBatchProcessHandler();
            return batch.CMP140_GenerateAccountDataOfCarryOverAndProfitProcess(UserId, BatchDate);
        }
    }
    #endregion
    #endregion
}
