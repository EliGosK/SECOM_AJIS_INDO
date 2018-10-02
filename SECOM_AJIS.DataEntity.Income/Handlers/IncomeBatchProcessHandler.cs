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
using System.Configuration;

using CSI.WindsorHelper;
using SECOM_AJIS.Common;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Data.Objects;

namespace SECOM_AJIS.DataEntity.Income
{
    public partial class IncomeBatchProcessHandler : BizICDataEntities
    {
        #region Get data
        /// <summary>
        /// Get paid invoice information which no receipt 
        /// </summary>
        /// <returns></returns>
        public List<doPaidInvoiceNoReceipt> GetPaidInvoiceNoReceipt()
        {
            List<doPaidInvoiceNoReceipt> result = base.GetPaidInvoiceNoReceipt(
                PaymentStatus.C_PAYMENT_STATUS_BANK_PAID,
                PaymentStatus.C_PAYMENT_STATUS_AUTO_PAID,
                PaymentStatus.C_PAYMENT_STATUS_CASH_PAID,
                PaymentStatus.C_PAYMENT_STATUS_CHEQUE_PAID,
                PaymentStatus.C_PAYMENT_STATUS_CASHIER_PAID,
                PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED,
                PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED,
                PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED,
                PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED,
                PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN,
                PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_REFUND,
                PaymentStatus.C_PAYMENT_STATUS_REFUND_PAID,

                PaymentStatus.C_PAYMENT_STATUS_AUTO_FAIL_BANK_PAID,
                PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL_BANK_PAID,
                PaymentStatus.C_PAYMENT_STATUS_POST_FAIL_BANK_PAID
                );
            return result;
        }
        /// <summary>
        /// Get invoice advance receipt information
        /// </summary>
        /// <returns></returns>
        public List<doInvoiceAdvanceReceipt> GetInvoiceAdvanceReceipt()
        {
            List<doInvoiceAdvanceReceipt> result = base.GetInvoiceAdvanceReceipt(
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE,
                IssueRecieptTime.C_ISSUE_REC_TIME_SAME_INV,
                PaymentMethodType.C_PAYMENT_METHOD_MESSENGER);
            return result;
        }
        #endregion

        #region Batch process handler
        //IncomeHandler-BatchGenReceiptAfterPayment
        /// <summary>
        /// Batch Process to generate tax invoice pdf report, receipt pdf report after payment on shared report folder
        /// </summary>
        /// <param name="UserId">employee no.</param>
        /// <param name="BatchDate">process datetime</param>
        /// <returns></returns>
        public doBatchProcessResult ICP010_BatchGenReceiptAfterPaymentProcess(string UserId, DateTime BatchDate)
        {
            #region Prepare
            //Handler
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            IBillingDocumentHandler billingDocumentHandler = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;
            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            IIncomeDocumentHandler incomeDocumentHandler = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;

            //Get data
            List<doPaidInvoiceNoReceipt> paidInvoices = this.GetPaidInvoiceNoReceipt();

            //Initial batch result
            doBatchProcessResult result = new doBatchProcessResult();
            result.Result = FlagType.C_FLAG_ON;
            result.BatchStatus = null;
            result.Total = paidInvoices.Count;
            result.Failed = 0;
            result.Complete = 0;
            result.ErrorMessage = string.Empty;
            result.BatchUser = UserId;
            #endregion

            if (paidInvoices.Count > 0)
            {
                foreach (doPaidInvoiceNoReceipt doPaidInvoice in paidInvoices)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        try
                        {
                            doInvoice doInvoice = billingHandler.GetInvoice(doPaidInvoice.InvoiceNo);

                            if (doPaidInvoice.BillingTypeGroup != BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE
                                   && (doPaidInvoice.IssueReceiptTiming == IssueRecieptTime.C_ISSUE_REC_TIME_NOT_ISSUE
                                       || doPaidInvoice.IssueReceiptTiming == IssueRecieptTime.C_ISSUE_REC_TIME_AFTER_PAYMENT)
                                )
                            {
                                #region Issue tax invoice

                                tbt_TaxInvoice doCheckTaxInvoice = billingHandler.GetTaxInvoiceData(doInvoice.InvoiceNo, doInvoice.InvoiceOCC); //Add by Jutarat A. on 04072013

                                if (doCheckTaxInvoice != null) //Add by Jutarat A. on 14112013
                                {
                                    //Add by Jutarat A. on 17102013
                                    List<tbt_TaxInvoice> doCheckTaxInvoiceList = new List<tbt_TaxInvoice>();
                                    doCheckTaxInvoiceList.Add(doCheckTaxInvoice);

                                    doCheckTaxInvoiceList = (from t in doCheckTaxInvoiceList
                                                             where t.TaxInvoiceCanceledFlag == false
                                                             select t).ToList<tbt_TaxInvoice>();

                                    if (doCheckTaxInvoiceList != null && doCheckTaxInvoiceList.Count > 0)
                                    {
                                        doCheckTaxInvoice = CommonUtil.CloneObject<tbt_TaxInvoice, tbt_TaxInvoice>(doCheckTaxInvoiceList[0]);
                                    }
                                    else
                                    {
                                        doCheckTaxInvoice = null;
                                    }
                                    //End Add
                                }

                                if (doCheckTaxInvoice == null) //Add by Jutarat A. on 04072013 (Check not exist TaxInvoice)
                                {
                                    tbt_TaxInvoice doTaxInvoice = billingHandler.IssueTaxInvoice(
                                            doInvoice,
                                            doPaidInvoice.PaymentDate,
                                            ProcessID.C_PROCESS_ID_GENERATE_RECEIPT_AFTER_PAYMENT,
                                            BatchDate);
                                    if (doTaxInvoice != null)
                                    {
                                        // Comment by Jirawat Jannet
                                        //BLR020 TaxInvoice report
                                        //billingDocumentHandler.GenerateBLR020FilePath(
                                        //    doTaxInvoice.TaxInvoiceNo
                                        //    , ProcessID.C_PROCESS_ID_GENERATE_RECEIPT_AFTER_PAYMENT
                                        //    , BatchDate);
                                        throw new Exception("กำลังดำเนินการแก้ไข report BLR020");
                                    }
                                    else
                                    {
                                        throw new Exception("Error generate tax invoice");
                                    }
                                }

                                #endregion

                                #region Issue receipt
                                tbt_Receipt doReceipt = incomeHandler.IssueReceipt(doInvoice, doPaidInvoice.PaymentDate, ProcessID.C_PROCESS_ID_GENERATE_RECEIPT_AFTER_PAYMENT, BatchDate, false); //Add (isWriteTransLog) by Jutarat A. on 07062013
                                if (doReceipt != null)
                                {
                                    //ICR010 Receipt report
                                    incomeDocumentHandler.GenerateICR010FilePath(doReceipt.ReceiptNo
                                        , ProcessID.C_PROCESS_ID_GENERATE_RECEIPT_AFTER_PAYMENT
                                        , BatchDate);

                                    if (doInvoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
                                    {
                                        bool isSuccess = billingHandler.UpdateReceiptNoDepositFee(doPaidInvoice.InvoiceNo, doReceipt.ReceiptNo, ProcessID.C_PROCESS_ID_GENERATE_RECEIPT_AFTER_PAYMENT, BatchDate);
                                        if (!isSuccess)
                                            throw new Exception("Error update receipt no to deposit table");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Error generate receipt");
                                }
                                #endregion
                            }
                            else
                            {
                                #region Issued receipt only

                                DateTime receiptDate = doPaidInvoice.PaymentDate;

                                if (doPaidInvoice.BillingTypeGroup != BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE
                                    && doPaidInvoice.IssueReceiptTiming == IssueRecieptTime.C_ISSUE_REC_TIME_SAME_INV
                                    && (doPaidInvoice.PaymentMethod == PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER
                                        || doPaidInvoice.PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_MESSENGER))
                                {
                                    receiptDate = doPaidInvoice.IssueInvDate.Value;
                                }
                                tbt_Receipt doReceipt = incomeHandler.IssueReceipt(doInvoice, receiptDate, ProcessID.C_PROCESS_ID_GENERATE_RECEIPT_AFTER_PAYMENT, BatchDate, false); //Add (isWriteTransLog) by Jutarat A. on 07062013
                                if (doReceipt != null)
                                {
                                    //ICR010 Receipt report
                                    incomeDocumentHandler.GenerateICR010FilePath(doReceipt.ReceiptNo
                                        , ProcessID.C_PROCESS_ID_GENERATE_RECEIPT_AFTER_PAYMENT
                                        , BatchDate);

                                    if (doInvoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
                                    {
                                        bool isSuccess = billingHandler.UpdateReceiptNoDepositFee(doPaidInvoice.InvoiceNo, doReceipt.ReceiptNo, ProcessID.C_PROCESS_ID_GENERATE_RECEIPT_AFTER_PAYMENT, BatchDate);
                                        if (!isSuccess)
                                            throw new Exception("Error update receipt no to deposit table");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Error generate receipt");
                                }
                                #endregion
                            }

                            //Success
                            scope.Complete();
                            result.Complete++;
                        }
                        catch (Exception ex)
                        {
                            scope.Dispose();
                            result.Failed++;
                            result.ErrorMessage += string.Format("Invoice no. {0} has Error : {1} {2}\n", doPaidInvoice.InvoiceNo, ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                        }
                    }
                }
                //Update batch result,     at lease one transaction fail => batch fail
                result.Result = (result.Failed == 0);
            }
            return result;
        }

        //IncomeHandler-BatchGenAdvanceReceipt
        /// <summary>
        /// Batch Process to generate advance receipt pdf report on shared report folder
        /// </summary>
        /// <param name="UserId">employee no.</param>
        /// <param name="BatchDate">process datetime</param>
        /// <returns></returns>
        public doBatchProcessResult ICP011_BatchGenAdvanceReceiptProcess(string UserId, DateTime BatchDate)
        {
            #region Prepare
            //Handler
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            IBillingDocumentHandler billingDocumentHandler = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;
            IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
            IIncomeDocumentHandler incomeDocumentHandler = ServiceContainer.GetService<IIncomeDocumentHandler>() as IIncomeDocumentHandler;

            //Get data
            List<doInvoiceAdvanceReceipt> invoiceAdvanceReceipts = this.GetInvoiceAdvanceReceipt();

            //Result
            doBatchProcessResult result = new doBatchProcessResult();
            result.Result = FlagType.C_FLAG_ON;
            result.BatchStatus = null;
            result.Total = invoiceAdvanceReceipts.Count;
            result.Failed = 0;
            result.Complete = 0;
            result.ErrorMessage = string.Empty;
            result.BatchUser = UserId;
            #endregion

            if (invoiceAdvanceReceipts.Count > 0)
            {
                foreach (doInvoiceAdvanceReceipt doAdvanceReceipt in invoiceAdvanceReceipts)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        try
                        {
                            doInvoice doInvoice = CommonUtil.CloneObject<doInvoiceAdvanceReceipt, doInvoice>(doAdvanceReceipt);
                            tbt_Receipt doReceipt = incomeHandler.IssueAdvanceReceipt(doInvoice, doInvoice.IssueInvDate.Value
                                , ProcessID.C_PROCESS_ID_GENERATE_RECEIPT_ADVANCE, BatchDate);
                            if (doReceipt != null)
                            {
                                //ICR010 Receipt report
                                incomeDocumentHandler.GenerateICR010FilePath(doReceipt.ReceiptNo
                                    , ProcessID.C_PROCESS_ID_GENERATE_RECEIPT_ADVANCE
                                    , BatchDate);
                            }
                            else
                            {
                                throw new Exception("Error issue receipt");
                            }

                            //Success
                            scope.Complete();
                            result.Complete++;
                        }
                        catch (Exception ex)
                        {
                            scope.Dispose();
                            result.Failed++;
                            result.ErrorMessage += string.Format("Invoice no. {0} has Error : {1} {2}\n", doAdvanceReceipt.InvoiceNo, ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                        }
                    }
                }
                //Update batch result,     at lease one transaction fail => batch fail
                result.Result = (result.Failed == 0);
            }
            return result;
        }

        //IncomeHandler-BatchGenDebtSummary
        /// <summary>
        /// Batch Process to generate debt summary information. call "sp_IC_BatchGenDebtSummary" store procedure
        /// </summary>a
        /// <param name="UserId">employee no.</param>
        /// <param name="BatchDate">process datetime</param>
        /// <returns></returns>
        public doBatchProcessResult ICP030_BatchGenDebtSummaryProcess(string UserId, DateTime BatchDate)
        {
            #region Prepare
            //Result
            doBatchProcessResult result = new doBatchProcessResult();
            result.Result = FlagType.C_FLAG_ON;
            result.BatchStatus = null;
            result.Total = 0;
            result.Complete = 0;
            result.Failed = 0;
            result.ErrorMessage = string.Empty;
            result.BatchUser = UserId;
            ObjectParameter pTotoalEffectRow = new ObjectParameter("TotalEffectRow", typeof(int));
            #endregion

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Call store procedure
                    this.BatchGenDebtSummary(ProcessID.C_PROCESS_ID_GENERATE_DEBT_SUMMARY
                        , BatchDate
                        , BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES
                        , PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                        , PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                        , PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                        , PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                        , PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                        , PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                        , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                        , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                        , pTotoalEffectRow);

                    //Success
                    scope.Complete();
                    result.Result = FlagType.C_FLAG_ON;
                    result.Total = (int)pTotoalEffectRow.Value;
                    result.Complete = (int)pTotoalEffectRow.Value;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    result.Result = FlagType.C_FLAG_OFF;
                    result.Failed = 0;
                    result.Total = 0;
                    result.ErrorMessage += string.Format("Process has Error : {0} {1}\n", ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                }
            }
            return result;
        }

        //IncomeHandler-BatchDeleteDebtTracing
        /// <summary>
        /// Batch Process to delete unused deb tracing information. call "sp_IC_BatchDeleteDebtTracing" store procedure
        /// </summary>
        /// <param name="UserId">employee no.</param>
        /// <param name="BatchDate">process datetime</param>
        /// <returns></returns>
        public doBatchProcessResult ICP050_BatchDeleteDebtTracingProcess(string UserId, DateTime BatchDate)
        {
            #region Prepare
            //Result
            doBatchProcessResult result = new doBatchProcessResult();
            result.Result = FlagType.C_FLAG_ON;
            result.BatchStatus = null;
            result.Total = 0;
            result.Complete = 0;
            result.Failed = 0;
            result.ErrorMessage = string.Empty;
            result.BatchUser = UserId;
            ObjectParameter pTotalRow = new ObjectParameter("TotalRow", typeof(int));
            ObjectParameter pCompletedRow = new ObjectParameter("CompletedRow", typeof(int));
            #endregion

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Call store procedure
                    this.BatchDeleteDebtTracing(
                        PaymentStatus.C_PAYMENT_STATUS_BANK_PAID
                        , PaymentStatus.C_PAYMENT_STATUS_AUTO_PAID
                        , PaymentStatus.C_PAYMENT_STATUS_CANCEL
                        , PaymentStatus.C_PAYMENT_STATUS_CASH_PAID
                        , PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED
                        , PaymentStatus.C_PAYMENT_STATUS_BILLING_EXEMPTION
                        , PaymentStatus.C_PAYMENT_STATUS_CHEQUE_PAID
                        , PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED
                        , PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED
                        , PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL
                        , PaymentStatus.C_PAYMENT_STATUS_CASHIER_PAID
                        , PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED
                        , PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED
                        , PaymentStatus.C_PAYMENT_STATUS_POST_FAIL

                        , PaymentStatus.C_PAYMENT_STATUS_AUTO_FAIL_BANK_PAID
                        , PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL_BANK_PAID
                        , PaymentStatus.C_PAYMENT_STATUS_POST_FAIL_BANK_PAID

                        , PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN
                        , PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_REFUND
                        , PaymentStatus.C_PAYMENT_STATUS_REFUND_PAID
                        , pTotalRow
                        , pCompletedRow);

                    //Success
                    scope.Complete();
                    result.Result = FlagType.C_FLAG_ON;
                    result.Total = (int)pTotalRow.Value;
                    result.Complete = (int)pCompletedRow.Value;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    result.Result = FlagType.C_FLAG_OFF;
                    result.Total = 0;
                    result.Failed = 0;
                    result.ErrorMessage += string.Format("Process has Error : {0} {1}\n", ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                }
            }
            return result;
        }
        #endregion

        internal doBatchProcessResult BatchGenerateDebtTracingNoticeProcess(string UserId, DateTime BatchDate, string strServiceTypeCode)
        {
            string BatchUserID = (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE ? "ICP060" : "ICP070");
            try
            {
                var handler = new IncomeHandler();
                var dochandler = new IncomeDocumentHandler();

                handler.GenerateDebtTracingNotice(BatchDate, null, strServiceTypeCode, BatchDate, BatchUserID);
                List<doDebtTracingDocNoForGenerate> lstDocNo = handler.GetDebtTracingDocNoForGenerate(BatchDate, strServiceTypeCode);

                var result = new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_ON,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_SUCCEEDED,
                    Total = lstDocNo.Count,
                    Complete = 0,
                    Failed = 0,
                    ErrorMessage = null,
                    BatchUser = BatchUserID,
                    BatchDate = BatchDate
                };

                foreach (var doc in lstDocNo)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        try
                        {
                            string reportpath = null;
                            if (doc.DocumentType == RunningType.C_RUNNING_TYPE_DEBT_TRACING_NOTICE1)
                            {
                                reportpath = dochandler.GenerateICR030FilePath(doc.DocumentNo, BatchUserID, BatchDate);
                            }
                            else if (doc.DocumentType == RunningType.C_RUNNING_TYPE_DEBT_TRACING_NOTICE2)
                            {
                                reportpath = dochandler.GenerateICR040FilePath(doc.DocumentNo, BatchUserID, BatchDate);
                            }

                            handler.UpdateDebtTracingNoticeGenerateFlag(doc.DocumentNo);

                            if (string.IsNullOrEmpty(reportpath))
                            {
                                result.Failed++;
                            }
                            else
                            {
                                result.Complete++;
                            }
                            scope.Complete();
                        }
                        catch (Exception ex)
                        {
                            result.Failed++;
                            result.ErrorMessage = ex.Message;
                            scope.Dispose();
                            return result;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return new doBatchProcessResult()
                {
                    Result = FlagType.C_FLAG_OFF,
                    BatchStatus = BatchStatus.C_BATCH_STATUS_FAILED,
                    Total = 1,
                    Complete = 0,
                    Failed = 1,
                    ErrorMessage = ex.Message, //msg.Message,
                    BatchUser = BatchUserID,
                    BatchDate = BatchDate
                };
            }
        }

        internal doBatchProcessResult ICP060_BatchGenerateDebtTracingNoticeSaleProcess(string UserId, DateTime BatchDate)
        {
            return BatchGenerateDebtTracingNoticeProcess(UserId, BatchDate, ServiceType.C_SERVICE_TYPE_SALE);
        }

        internal doBatchProcessResult ICP070_BatchGenerateDebtTracingNoticeRentalProcess(string UserId, DateTime BatchDate)
        {
            return BatchGenerateDebtTracingNoticeProcess(UserId, BatchDate, ServiceType.C_SERVICE_TYPE_RENTAL);
        }

        internal doBatchProcessResult BatchPrintDebtTracingNoticeProcess(string UserId, DateTime BatchDate, string strServiceTypeCode)
        {
            doBatchProcessResult result = new doBatchProcessResult();
            IncomeHandler incomeHandler = new IncomeHandler();
            ILogHandler logHandler = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            ICommonHandler comHandler = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

            string pathFoxit = ConfigurationManager.AppSettings["PrintPDFFoxit"];
            string pathFileName = string.Empty;
            string processID = (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE ? ProcessID.C_PROCESS_ID_PRINTING_DEBT_NOTICE_SALE : ProcessID.C_PROCESS_ID_PRINTING_DEBT_NOTICE_RENTAL);
            string printflag = (strServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE ? PrintingFlag.C_PRINTING_FLAG_ICP080 : PrintingFlag.C_PRINTING_FLAG_ICP090);

            List<doDebtTracingDocNoForPrinting> lstDocNo = incomeHandler.GetDebtTracingDocNoForPrinting(BatchDate, strServiceTypeCode);
            if (lstDocNo != null && lstDocNo.Count > 0)
            {
                result.Result = false;
                result.Total = lstDocNo.Count;
                result.Complete = 0;
                result.Failed = 0;
                result.ErrorMessage = string.Empty;

                string strErrorMessage = string.Empty;
                bool bResult = comHandler.AllocatePrintingProcess(printflag, processID, ref strErrorMessage);
                if (bResult)
                {
                    foreach (doDebtTracingDocNoForPrinting doc in lstDocNo)
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            try
                            {
                                doDocumentDownloadLog cond = new doDocumentDownloadLog();
                                cond.DocumentNo = doc.DocumentNo;
                                cond.DocumentCode = doc.DocumentCode;
                                cond.DocumentOCC = doc.DocumentOCC;
                                cond.DownloadDate = BatchDate;
                                cond.DownloadBy = processID;
                                logHandler.WriteDocumentDownloadLog(cond);

                                incomeHandler.UpdateDebtTracingNoticePrintFlag(doc.DocumentNo);
                                comHandler.PrintPDF(doc.FilePathDocument);

                                scope.Complete();
                                result.Complete++;
                            }
                            catch (Exception ex)
                            {
                                scope.Dispose();
                                result.Failed++;
                                result.ErrorMessage += string.Format("DocumentNo {0} DocumentCode {1} DocumentOCC {2} has Error : {3} {4}\n"
                                    , doc.DocumentNo
                                    , doc.DocumentCode
                                    , doc.DocumentOCC
                                    , ex.Message
                                    , ex.InnerException != null ? ex.InnerException.Message : ""
                                );
                                break;
                            }
                        }
                    }

                    bResult = comHandler.ResetPrintingProcess(processID);
                }
                else
                {
                    result.Failed = result.Total;
                    if (String.IsNullOrEmpty(strErrorMessage) == false)
                        result.ErrorMessage = strErrorMessage;
                }
            }

            result.Result = (result.Failed == 0);
            result.BatchStatus = (result.Result ? BatchStatus.C_BATCH_STATUS_SUCCEEDED : BatchStatus.C_BATCH_STATUS_FAILED);
            return result;
        }

        internal doBatchProcessResult ICP080_BatchPrintDebtTracingNoticeSaleProcess(string UserId, DateTime BatchDate)
        {
            return BatchPrintDebtTracingNoticeProcess(UserId, BatchDate, ServiceType.C_SERVICE_TYPE_SALE);
        }

        internal doBatchProcessResult ICP090_BatchPrintDebtTracingNoticeRentalProcess(string UserId, DateTime BatchDate)
        {
            return BatchPrintDebtTracingNoticeProcess(UserId, BatchDate, ServiceType.C_SERVICE_TYPE_RENTAL);
        }
    }

    #region Batch process class
    public class ICP010_BatchGenReceiptAfterPayment : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            IncomeBatchProcessHandler batch = new IncomeBatchProcessHandler();
            return batch.ICP010_BatchGenReceiptAfterPaymentProcess(UserId, BatchDate);
        }
    }
    public class ICP011_BatchGenAdvanceReceipt : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            IncomeBatchProcessHandler batch = new IncomeBatchProcessHandler();
            return batch.ICP011_BatchGenAdvanceReceiptProcess(UserId, BatchDate);
        }
    }
    public class ICP030_BatchGenDebtSummary : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            IncomeBatchProcessHandler batch = new IncomeBatchProcessHandler();
            return batch.ICP030_BatchGenDebtSummaryProcess(UserId, BatchDate);
        }
    }
    public class ICP050_BatchDeleteDebtTracing : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            IncomeBatchProcessHandler batch = new IncomeBatchProcessHandler();
            return batch.ICP050_BatchDeleteDebtTracingProcess(UserId, BatchDate);
        }
    }
    public class ICP060_BatchGenerateDebtTracingNoticeSale : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            IncomeBatchProcessHandler batch = new IncomeBatchProcessHandler();
            return batch.ICP060_BatchGenerateDebtTracingNoticeSaleProcess(UserId, BatchDate);
        }
    }
    public class ICP070_BatchGenerateDebtTracingNoticeRental : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            IncomeBatchProcessHandler batch = new IncomeBatchProcessHandler();
            return batch.ICP070_BatchGenerateDebtTracingNoticeRentalProcess(UserId, BatchDate);
        }
    }
    public class ICP080_BatchPrintDebtTracingNoticeSale : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            IncomeBatchProcessHandler batch = new IncomeBatchProcessHandler();
            return batch.ICP080_BatchPrintDebtTracingNoticeSaleProcess(UserId, BatchDate);
        }
    }
    public class ICP090_BatchPrintDebtTracingNoticeRental : IBatchProcess
    {
        public doBatchProcessResult WorkProcess(string UserId, DateTime BatchDate)
        {
            IncomeBatchProcessHandler batch = new IncomeBatchProcessHandler();
            return batch.ICP090_BatchPrintDebtTracingNoticeRentalProcess(UserId, BatchDate);
        }
    }
    #endregion
}
