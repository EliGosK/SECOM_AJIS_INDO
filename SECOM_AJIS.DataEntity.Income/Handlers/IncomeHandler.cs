using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Transactions;

using CSI.WindsorHelper;
using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Controllers;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Billing;
using System.Data.Objects;
using System.Reflection;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Income
{
    public partial class IncomeHandler : BizICDataEntities, IIncomeHandler
    {
        #region Import Payment
        //ICS020
        /// <summary>
        /// Function for loading payment content data file.
        /// </summary>
        /// <param name="importID">import id</param>
        /// <param name="csvFilePath">csv file path</param>
        /// <param name="secomBankBranch">SECOM bank/brach id</param>
        /// <param name="paymentType">payment type</param>
        /// <returns></returns>
        public int LoadPaymentDataFile(Guid importID, string csvFilePath, int secomBankBranch, string paymentType, string bankCode, string currencyType)
        {
            #region Csv
            //Read csv
            System.IO.StreamReader rd = new System.IO.StreamReader(csvFilePath, System.Text.Encoding.UTF8);
            List<string> lines = new List<string>();
            while (rd.EndOfStream == false)
                lines.Add(rd.ReadLine());
            rd.Close();

            //Mapping Csv data to tbt_tmpImportContent
            List<CsvParseSetPropertyError> setPropertyError = new List<CsvParseSetPropertyError>();
            List<tbt_tmpImportContent> tmpImportContent = new List<tbt_tmpImportContent>();
            try
            {
                //Convet csv content to do object
                tmpImportContent = CSVImportUtil.GenerateModelofCsvData<tbt_tmpImportContent>(lines, out setPropertyError, "dd-MMM-yyyy");
            }
            catch
            {
                //All import csv error, display msg7018
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7018, null);
            }

            if (setPropertyError != null && setPropertyError.Count > 0)
            {
                string ErrorMsg = "Invalid data format." + Environment.NewLine;
                foreach (CsvParseSetPropertyError item in setPropertyError)
                {
                    ErrorMsg += string.Format("Error at Line: {0}, Column Name: {1}[{2}], Value: {3}" + Environment.NewLine,
                        item.Line, item.CsvColumnName, item.Column, item.Value);
                }
                throw new Exception(ErrorMsg);
            }
            #endregion

            // add by Jirawat Jannet @ 2016-10-05
            #region Update currency data

            foreach (var item in tmpImportContent)
            {
                decimal? paymentAmount = item.PaymentAmount;
                decimal? whtAmount = item.WHTAmount;
                item.PaymentAmountCurrencyType = currencyType;
                item.WHTAmountCurrencyType = currencyType;

                if (currencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    item.PaymentAmount = paymentAmount;
                    item.WHTAmount = whtAmount;
                    item.PaymentAmountUsd = null;
                    item.WHTAmountUsd = null;
                }
                else
                {
                    item.PaymentAmount = null;
                    item.WHTAmount = null;
                    item.PaymentAmountUsd = paymentAmount;
                    item.WHTAmountUsd = whtAmount;
                }
            }

            #endregion

            #region Insert into db
            //Update all rows
            foreach (var item in tmpImportContent)
            {
                item.ImportID = importID;
                item.ValidationResult = null;

                if (paymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
                {
                    if (String.IsNullOrEmpty(item.AutoTransferResult) == false) //Add by Jutarat A. on 12092013
                    {
                        //if (item.AutoTransferResult == AutoTransferResultWord.C_AUTO_TRANSFER_RESULT_WORD_FAIL)
                        if (item.AutoTransferResult.ToUpper() == AutoTransferResultWord.C_AUTO_TRANSFER_RESULT_WORD_FAIL.ToUpper()) //Modify by Jutarat A. on 12092013
                        {
                            item.AutoTransferResult = AutoTransferResult.C_AUTO_TRANSFER_RESULT_FAIL;
                        }
                        //else if (item.AutoTransferResult == AutoTransferResultWord.C_AUTO_TRANSFER_RESULT_WORD_OK)
                        else if (item.AutoTransferResult.ToUpper() == AutoTransferResultWord.C_AUTO_TRANSFER_RESULT_WORD_OK.ToUpper()) //Modify by Jutarat A. on 12092013
                        {
                            item.AutoTransferResult = AutoTransferResult.C_AUTO_TRANSFER_RESULT_OK;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(item.PayerBankAccNo))
                    item.PayerBankAccNo = item.PayerBankAccNo.Replace("-", "");

                item.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                item.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                item.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                item.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            }

            //Insert
            InsertTbt_tmpImportContent(tmpImportContent);
            #endregion

            #region Validate
            //Check all records have the same payment type
            int nPaymentType = tmpImportContent.Select(d => d.PaymentType).Distinct().Count();
            if (nPaymentType > 1)
                return -2;

            //Check selected payment type is correct
            string tmpPaymentType = tmpImportContent.Select(d => d.PaymentType).Distinct().First();
            if (!tmpPaymentType.Equals(paymentType))
                return -3;

            //For auto transfer
            if (paymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
            {
                //No need, check all records have the same payment date
                //int nPaymentDate = tmpImportContent.Select(d => d.PaymentDate).Distinct().Count();
                //if (nPaymentDate > 1)
                //    return -5;

                int nBankNotMatch = tmpImportContent.Where(d => (string.IsNullOrEmpty(d.SendingBankCode) == false && d.SendingBankCode.Equals(bankCode) == false)).ToList().Count();
                if (nBankNotMatch > 0)
                    return -6;
            }


            foreach (var d in tmpImportContent)
            {
                //Validate required fields
                if (string.IsNullOrEmpty(d.PaymentType) || (!d.PaymentDate.HasValue)
                             || string.IsNullOrEmpty(d.RefInvoiceNo) || ((d.PaymentAmount ?? 0) <= 0 && (d.PaymentAmountUsd ?? 0) <= 0)
                             || (!d.WHTAmount.HasValue && !d.WHTAmountUsd.HasValue))
                    return -4;          //Missing required field

                if (paymentType == PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER)
                {
                    if (!string.IsNullOrEmpty(d.AutoTransferResult) || !string.IsNullOrEmpty(d.FailureReason))
                        return -1;      //Invalid file format
                }
                else if (paymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
                {
                    if (string.IsNullOrEmpty(d.AutoTransferResult))
                        return -4;      //Missing required field

                    if (d.AutoTransferResult == AutoTransferResult.C_AUTO_TRANSFER_RESULT_FAIL && string.IsNullOrEmpty(d.FailureReason))
                        return -4;      //Missing required field

                    if (d.AutoTransferResult == AutoTransferResult.C_AUTO_TRANSFER_RESULT_OK && !string.IsNullOrEmpty(d.FailureReason))
                        return -1;      //Invalid file format
                }
            }
            #endregion

            //Success
            return 1;
        }

        //Confirm SA (P.karn) on 12/Apr/2012
        //No need logging 
        //public override List<tbt_tmpImportContent> InsertTbt_tmpImportContent(string xmldoTmpImportContents)
        //{
        //    ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
        //    List<tbt_tmpImportContent> saved = base.InsertTbt_tmpImportContent(xmldoTmpImportContents);
        //    doTransactionLog logData = new doTransactionLog()
        //    {
        //        TransactionType = doTransactionLog.eTransactionType.Insert,
        //        TableName = TableName.C_TBL_NAME_TMP_IMPORT_CONTENT,
        //        TableData = null
        //    };
        //    logData.TableData = CommonUtil.ConvertToXml(saved);
        //    logHand.WriteTransactionLog(logData);
        //    return saved;
        //}
        /// <summary>
        /// Function for insert payment content data to DB. (sp_IC_InsertTbt_tmpImportContent)
        /// </summary>
        /// <param name="tmpImportContents">payment content data</param>
        /// <returns></returns>
        public List<tbt_tmpImportContent> InsertTbt_tmpImportContent(List<tbt_tmpImportContent> tmpImportContents)
        {
            List<tbt_tmpImportContent> result = this.InsertTbt_tmpImportContent(CommonUtil.ConvertToXml_Store<tbt_tmpImportContent>(tmpImportContents));
            return result;
        }
        /// <summary>
        /// Function for retrieving payment content data of specific import id. (sp_IC_GetTbt_tmpImportContent)
        /// </summary>
        /// <param name="importID">import id</param>
        /// <returns></returns>
        public List<tbt_tmpImportContent> GetTbt_tmpImportContent(Guid importID)
        {
            List<tbt_tmpImportContent> result = base.GetTbt_tmpImportContent((Guid?)importID);
            if (result != null && result.Count > 0)
            {
                //Language Mapping
                CommonUtil.MappingObjectLanguage<tbt_tmpImportContent>(result);
            }
            return result;
        }

        /// <summary>
        /// Function for validating that the import contain all invoice in export file. (sp_IC_ValidateWholeFile)
        /// </summary>
        /// <param name="importID">import id</param>
        /// <returns>The remaining invoice no.</returns>
        public List<doValidateWholeFile> ValidateWholeFile(Guid importID)
        {
            return base.ValidateWholeFile((Guid?)importID);
        }

        /// <summary>
        /// Function for validating business for auto transfer file content. (sp_IC_ValidateAutoTransferContent)
        /// </summary>
        /// <param name="importID">import id</param>
        public void ValidateAutoTransferContent(Guid importID, int secomAccountID)
        {
            base.ValidateAutoTransferContent((Guid?)importID
                , (int?)secomAccountID
                , ValidationImportResult.C_PAYMENT_IMPORT_NO_ERROR
                , ValidationImportResult.C_PAYMENT_IMPORT_ERROR_INVOICE_AMOUNT_UNMATCH
                , ValidationImportResult.C_PAYMENT_IMPORT_ERROR_PAY_DATE_UNMATCH
                , ValidationImportResult.C_PAYMENT_IMPORT_ERROR_BANK_UNMATCH
                , PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                , PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT);
        }
        /// <summary>
        /// Function for validating business for bank transfer file content. (sp_IC_ValidateBankTransferContent)
        /// </summary>
        /// <param name="importID">import id</param>
        public void ValidateBankTransferContent(Guid importID)
        {
            base.ValidateBankTransferContent((Guid?)importID
                , ValidationImportResult.C_PAYMENT_IMPORT_NO_ERROR
                , ValidationImportResult.C_PAYMENT_IMPORT_ERROR_INVALID_INVOICE
                , ValidationImportResult.C_PAYMENT_IMPORT_ERROR_IMPORTED_INVOICE
                , ValidationImportResult.C_PAYMENT_IMPORT_ERROR_PAID_INVOICE
                , ValidationImportResult.C_PAYMENT_IMPORT_ERROR_INCORRECT_STATUS
                , PaymentSystemMethod.C_INC_PAYMENT_IMPORT
                , PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                , PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                , PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                , PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                , PaymentStatus.C_PAYMENT_STATUS_CANCEL
                , PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED
                , PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL
                , PaymentStatus.C_PAYMENT_STATUS_POST_FAIL
                , PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                , PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT);
        }

        /// <summary>
        /// Function for checking that auto transfer has been imported. (sp_IC_CheckAutoTransferFileImported)
        /// </summary>
        /// <param name="importID">import id</param>
        /// <param name="secomAccountID">SECOM account id</param>
        /// <returns></returns>
        public bool CheckAutoTransferFileImported(Guid importID, int secomAccountID)
        {
            bool result = false;
            ObjectParameter isImport = new ObjectParameter("IS_IMPORTED", false);
            base.CheckAutoTransferFileImported((Guid?)importID, secomAccountID
                , PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER
                , isImport);
            result = (bool)isImport.Value;
            return result;
        }

        /// <summary>
        /// Function for insert payment import file data to DB. (sp_IC_InsertTbt_PaymentImportFile)
        /// </summary>
        /// <param name="importFiles">payment import file data</param>
        /// <returns></returns>
        public override List<tbt_PaymentImportFile> InsertTbt_PaymentImportFile(string xmldoPaymentImportFile)
        {
            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            List<tbt_PaymentImportFile> saved = base.InsertTbt_PaymentImportFile(xmldoPaymentImportFile);
            if (saved != null && saved.Count > 0) //Add by Jutarat A. on 30052013
            {
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_PAYMENT_IMPORT_FILE,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(saved);
                logHand.WriteTransactionLog(logData);
            }
            return saved;
        }
        /// <summary>
        /// Function for insert payment import file data to DB. (sp_IC_InsertTbt_PaymentImportFile)
        /// </summary>
        /// <param name="importFiles">payment import file data</param>
        /// <returns></returns>
        public List<tbt_PaymentImportFile> InsertTbt_PaymentImportFile(List<tbt_PaymentImportFile> importFiles)
        {
            List<tbt_PaymentImportFile> result = this.InsertTbt_PaymentImportFile(CommonUtil.ConvertToXml_Store<tbt_PaymentImportFile>(importFiles));
            return null;
        }


        //Match payment (Auto)
        /// <summary>
        /// Match payment and invoice. The amount matched is equal to payment amount. Other amount is set automatically.
        /// </summary>
        /// <param name="doPayment">payment information</param>
        /// <param name="doInvoice">invoice information</param>
        /// <returns></returns>
        public bool MatchPaymentInvoiceAuto(tbt_Payment doPayment, tbt_Invoice doTbt_Invoice)
        {
            if (doPayment == null || doTbt_Invoice == null)
                return false;

            bool isSuccess = false;

            //Validate business
            //if (doPayment.MatchableBalance != ((doTbt_Invoice.InvoiceAmount ?? 0) + (doTbt_Invoice.VatAmount ?? 0) - (doTbt_Invoice.WHTAmount ?? 0)))
            //    throw new Exception(MessageUtil.GetMessage(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7001, null).Message);


            //Generate MatchID
            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            string matchID = billingHandler.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_MATCH_PAYMENT);

            #region Match Header
            tbt_MatchPaymentHeader matchHeader = new tbt_MatchPaymentHeader()
            {
                MatchID = matchID,
                MatchDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                PaymentTransNo = doPayment.PaymentTransNo,
                TotalMatchAmount = doPayment.PaymentAmount,
                BankFeeAmount = 0,
                SpecialProcessFlag = false,
                ApproveNo = null,
                OtherExpenseAmount = 0,
                OtherIncomeAmount = 0,
                CancelFlag = false,
                CancelApproveNo = null,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
            };
            //Save
            List<tbt_MatchPaymentHeader> sourceMatchHeader = new List<tbt_MatchPaymentHeader>();
            sourceMatchHeader.Add(matchHeader);
            List<tbt_MatchPaymentHeader> resultMatchHeader = this.InsertTbt_MatchPaymentHeader(CommonUtil.ConvertToXml_Store<tbt_MatchPaymentHeader>(sourceMatchHeader));
            if (resultMatchHeader == null || resultMatchHeader.Count == 0)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7078, null);
            #endregion


            #region Match Detail
            decimal matchAmountIncWHT = 0;
            decimal? whtAmount = 0;
            if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER || doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
            {
                //Auto/Bank transfer
                matchAmountIncWHT = doPayment.PaymentAmount.ConvertTo<decimal>(false, 0) + (doTbt_Invoice.WHTAmount ?? 0);
                whtAmount = doTbt_Invoice.WHTAmount;
            }
            else if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED)
            {
                //CN
                matchAmountIncWHT = doPayment.PaymentAmount.ConvertTo<decimal>(false, 0);
                whtAmount = 0;
            }
            else
            {
                //Payment type incorrect
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7079, null);
            }

            string matchStatus = string.Empty;
            if ((doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER || doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER
                    || doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED)
                && (doTbt_Invoice.PaidAmountIncVat ?? 0) == 0 && (doTbt_Invoice.RegisteredWHTAmount ?? 0) == 0
                    && doPayment.PaymentAmount == (doTbt_Invoice.InvoiceAmount ?? 0) + (doTbt_Invoice.VatAmount ?? 0) - (doTbt_Invoice.WHTAmount ?? 0))
            {
                //Match full
                matchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL;
            }
            else if ((doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED
                && (doTbt_Invoice.PaidAmountIncVat ?? 0) > 0
                && doPayment.PaymentAmount == (doTbt_Invoice.InvoiceAmount ?? 0) + (doTbt_Invoice.VatAmount ?? 0) - (doTbt_Invoice.PaidAmountIncVat ?? 0) - (doTbt_Invoice.RegisteredWHTAmount ?? 0)))
            {
                //Match to full (CN only)
                matchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL_TO_FULL;
            }
            else if ((doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED
                && doPayment.PaymentAmount < (doTbt_Invoice.InvoiceAmount ?? 0) + (doTbt_Invoice.VatAmount ?? 0) - (doTbt_Invoice.PaidAmountIncVat ?? 0) - (doTbt_Invoice.RegisteredWHTAmount ?? 0)))
            {
                //Partially match (CN only)
                matchStatus = PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL;
            }
            else
            {
                //Not found case
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7079, null);
            }

            tbt_MatchPaymentDetail matchDetail = new tbt_MatchPaymentDetail()
            {
                MatchID = matchID,
                InvoiceNo = doTbt_Invoice.InvoiceNo,
                InvoiceOCC = doTbt_Invoice.InvoiceOCC,
                MatchAmountExcWHT = doPayment.PaymentAmount.ConvertTo<decimal>(false, 0),
                MatchAmountIncWHT = matchAmountIncWHT,
                WHTAmount = whtAmount,
                MatchStatus = matchStatus,
                CancelFlag = false,
                CancelApproveNo = null,
                CorrectionReason = null,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
            };
            //Save
            List<tbt_MatchPaymentDetail> matchDetailList = new List<tbt_MatchPaymentDetail>();
            matchDetailList.Add(matchDetail);
            List<tbt_MatchPaymentDetail> resultMatchDetail = this.InsertTbt_MatchPaymentDetail(CommonUtil.ConvertToXml_Store<tbt_MatchPaymentDetail>(matchDetailList));
            if (resultMatchDetail == null || resultMatchDetail.Count == 0)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7079, null);
            #endregion


            #region Invoice
            decimal unpaidAmount = 0;
            decimal? registeredWhtAmount = null;
            if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER || doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
            {
                unpaidAmount = (doTbt_Invoice.InvoiceAmount ?? 0) + (doTbt_Invoice.VatAmount ?? 0) - (doTbt_Invoice.WHTAmount ?? 0);
                registeredWhtAmount = doTbt_Invoice.WHTAmount;
            }
            else if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED)
            {
                unpaidAmount = (doTbt_Invoice.InvoiceAmount ?? 0) + (doTbt_Invoice.VatAmount ?? 0) - (doTbt_Invoice.PaidAmountIncVat ?? 0) - (doTbt_Invoice.RegisteredWHTAmount ?? 0);
                registeredWhtAmount = doTbt_Invoice.RegisteredWHTAmount;
            }

            #region Update InvoicePaymentStatus
            string updatePaymentStatus = updatePaymentStatus = doTbt_Invoice.InvoicePaymentStatus;

            if (doPayment.PaymentAmount < unpaidAmount)
            {
                #region Partially match
                if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED      //09
                    || doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN)
                {
                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN;  //97
                }
                else
                {
                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID; //95
                }
                #endregion
            }
            else
            {
                #region Fully match or match to full
                if (doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN)
                {
                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN;   //96
                }
                else
                {
                    if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER)
                    {
                        #region Bank transfer
                        if (doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_BANK_PAID;
                        }
                        else if (doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_AUTO_FAIL_BANK_PAID;
                        }
                        else if (doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL_BANK_PAID;
                        }
                        else if (doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_FAIL_BANK_PAID;
                        }
                        else if (doTbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_BANK_PAID;
                        }
                        #endregion
                    }
                    else if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
                    {
                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_AUTO_PAID;
                    }
                    else if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED)        //09
                    {
                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN;      //96
                    }
                }
                #endregion
            }
            #endregion

            doTbt_Invoice.InvoicePaymentStatus = updatePaymentStatus;
            doTbt_Invoice.PaidAmountIncVat = (doTbt_Invoice.PaidAmountIncVat ?? 0) + doPayment.PaymentAmount;
            doTbt_Invoice.RegisteredWHTAmount = registeredWhtAmount;
            doTbt_Invoice.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            doTbt_Invoice.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            //Save
            List<tbt_Invoice> invoiceList = new List<tbt_Invoice>();
            invoiceList.Add(doTbt_Invoice);
            List<tbt_Invoice> resultInvoice = billingHandler.UpdateTbt_Invoice(CommonUtil.ConvertToXml_Store<tbt_Invoice>(invoiceList));
            if (resultInvoice == null || resultInvoice.Count == 0)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7080, null);
            #endregion


            #region Update Billing Detail
            //Prepare
            List<tbt_BillingDetail> doTbt_billingDetails = billingHandler.GetTbt_BillingDetailOfInvoice(doTbt_Invoice.InvoiceNo, doTbt_Invoice.InvoiceOCC);
            if (doTbt_billingDetails != null)
            {
                foreach (var billingDetail in doTbt_billingDetails)
                {
                    billingDetail.PaymentStatus = updatePaymentStatus;
                    billingDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    billingDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                    //Update
                    int result = billingHandler.Updatetbt_BillingDetail(billingDetail);
                    if (resultInvoice.Count == 0)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7080, null);
                }
            }
            #endregion


            #region Receipt
            //No need Update Receipt No to tax invoice table

            #region Update receipt's advance receipt status for fully paid
            if (matchDetail.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL || matchDetail.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL_TO_FULL)
            {
                doReceipt receipt = this.GetReceiptByInvoiceNo(doTbt_Invoice.InvoiceNo, doTbt_Invoice.InvoiceOCC);
                if (receipt != null && (receipt.AdvanceReceiptStatus == AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_ISSUED
                                 || receipt.AdvanceReceiptStatus == AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_REGISTERED))
                {
                    isSuccess = this.UpdateAdvanceReceiptMatchPayment(receipt.ReceiptNo);
                    if (!isSuccess)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7081, null);

                    this.DeleteTbt_MoneyCollectionInfo(receipt.ReceiptNo);
                }
            }
            #endregion
            #endregion

            #region Deposit
            #region Insert deposit fee, in case of deposit fee invoice
            string ContractCode = string.Empty;
            string BillingOCC = string.Empty;
            if (doTbt_billingDetails != null && doTbt_billingDetails.Count > 0)
            {
                ContractCode = doTbt_billingDetails[0].ContractCode;
                BillingOCC = doTbt_billingDetails[0].BillingOCC;
            }

            if (doTbt_Invoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
            {
                decimal? balanceDepositAfterUpdate = 0;
                string balanceDepositAfterUpdateCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                decimal? balanceDepositUsdAfterUpdate = 0;
                // decimal adjustAmount = (doTbt_Invoice.InvoiceAmount??0)+(doTbt_Invoice.VatAmount??0);
                decimal? adjustAmount = doTbt_Invoice.InvoiceAmount;  // Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013 // edit by jirawat jannet @2016-10-06
                decimal? adjustAmountUsd = doTbt_Invoice.InvoiceAmountUsd; // add by Jirawat Jannet @ 2016-10-06
                isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(ContractCode, BillingOCC
                    , adjustAmount, adjustAmountUsd, doTbt_Invoice.InvoiceAmountCurrencyType
                    , out balanceDepositAfterUpdate, out balanceDepositUsdAfterUpdate, out balanceDepositAfterUpdateCurrencyType); // edit by Jirawat Jannet @ 2016-10-06
                if (!isSuccess)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);

                isSuccess = billingHandler.InsertDepositFeePayment(ContractCode, BillingOCC
                    , adjustAmount, adjustAmountUsd, doTbt_Invoice.InvoiceAmountCurrencyType
                    , balanceDepositAfterUpdate, balanceDepositUsdAfterUpdate, doTbt_Invoice.InvoiceAmountCurrencyType
                    , doTbt_Invoice.InvoiceNo, null);
                if (!isSuccess)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);
            }
            #endregion

            //No need Insert deposit fee, in case of refund payment
            #endregion

            #region Payment
            //Update remaining matchable balance
            tbt_Payment paymentDB = this.GetPayment(doPayment.PaymentTransNo);
            if (paymentDB.UpdateDate != doPayment.UpdateDate)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7019, null);

            //Update
            //bool isUpdate = this.UpdatePaymentMatchableBalance(paymentDB.PaymentTransNo, paymentDB.PaymentAmount.ConvertTo<decimal>(false, 0) * (-1));   //Minus value  // Comment by jirawat jannet 2016-10-06

            // add by jirawat jannet @ 2016-10-06
            bool isUpdate = this.UpdatePaymentMatchableBalance(paymentDB.PaymentTransNo
                , paymentDB.PaymentAmount.ConvertTo<decimal>(false, 0) * (-1), paymentDB.PaymentAmountUsd.ConvertTo<decimal>(false, 0) * (-1)
                , paymentDB.MatchableBalanceCurrencyType);

            if (!isUpdate)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
            #endregion

            //Success
            return true;
        }
        #endregion

        public List<doPaymentForWHT> SearchPaymentForWHT(doPaymentForWHTSearchCriteria param)
        {
            List<doPaymentForWHT> result = base.SearchPaymentForWHT(
                param.PaymentDateFrom,
                param.PaymentDateTo,
                param.PaymentTransNo,
                param.PayerName,
                param.VATRegistantName,
                param.InvoiceNo,
                param.ContractCode,
                param.WHTNo,
                param.IDNo,
                CurrencyUtil.C_CURRENCY_LOCAL,
                CurrencyUtil.C_CURRENCY_US
            );
            if (result != null && result.Count > 0)
            {
                CommonUtil.MappingObjectLanguage<doPaymentForWHT>(result);
            }
            return result;
        }

        public new string GenerateWHTNo(DateTime? matchingdate)
        {
            List<string> result = base.GenerateWHTNo(matchingdate);
            return result.FirstOrDefault();
        }

        public List<tbt_IncomeWHT> InsertTbt_IncomeWHT(List<tbt_IncomeWHT> wht)
        {
            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            string xml = CommonUtil.ConvertToXml_Store<tbt_IncomeWHT>(wht);
            List<tbt_IncomeWHT> saved = base.InsertTbt_IncomeWHT(xml);
            if (saved != null && saved.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_INCOME_WHT,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(saved);
                logHand.WriteTransactionLog(logData);
            }
            return saved;
        }

        public List<tbt_IncomeWHT> UpdateTbt_IncomeWHT(List<tbt_IncomeWHT> wht)
        {
            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            string xml = CommonUtil.ConvertToXml_Store<tbt_IncomeWHT>(wht);
            List<tbt_IncomeWHT> saved = base.UpdateTbt_IncomeWHT(xml);
            if (saved != null && saved.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_INCOME_WHT,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(saved);
                logHand.WriteTransactionLog(logData);
            }
            return saved;
        }

        public List<tbt_IncomeWHT> GetTbt_IncomeWHT(string WHTNo)
        {
            List<tbt_IncomeWHT> result = base.GetTbt_IncomeWHT(WHTNo).ToList();
            return result;
        }

        public List<tbt_Payment> UpdateWHTNoToPayment(string WHTNo, List<string> lstPaymentTransNo, string UpdateBy, DateTime UpdateDate)
        {
            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            string transnolist = (lstPaymentTransNo == null ? null : string.Join(",", lstPaymentTransNo.Select(d => "\"" + d + "\"")));
            List<tbt_Payment> saved = base.UpdateWHTNoToPayment(WHTNo, transnolist, UpdateBy, UpdateDate);
            if (saved != null && saved.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_PAYMENT,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(saved);
                logHand.WriteTransactionLog(logData);
            }
            return saved;
        }

        public List<doIncomeWHT> SearchIncomeWHT(doIncomeWHTSearchCriteria param)
        {
            List<doIncomeWHT> result = base.SearchIncomeWHT(
                param.WHTNo,
                param.PaymentTransNo,
                param.PayerName,
                param.VATRegistantName,
                param.DocumentDateFrom,
                param.DocumentDateTo,
                CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US
            );
            if (result != null && result.Count > 0)
            {
                CommonUtil.MappingObjectLanguage(result);
            }
            return result;
        }

        public doWHTYearMonth GetWHTYearMonth()
        {
            List<doWHTYearMonth> result = base.GetWHTYearMonth();
            if (result != null && result.Count > 0)
            {
                return result[0];
            }
            else
            {
                var thismonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                return new doWHTYearMonth()
                {
                    MINYEARMONTH = thismonth.AddMonths(-23),
                    MAXYEARMONTH = thismonth.AddMonths(1)
                };
            }
        }

        public List<doWHTReportForAccount> GetWHTReportForAccount(DateTime? yearMonthFrom, DateTime? yearMonthTo)
        {
            List<doWHTReportForAccount> result = base.GetWHTReportForAccount(yearMonthFrom, yearMonthTo,CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
            if (result != null && result.Count > 0)
            {
                CommonUtil.MappingObjectLanguage(result);
            }
            return result;
        }

        public List<doWHTReportForIMS> GetWHTReportForIMS(DateTime? yearMonthFrom, DateTime? yearMonthTo)
        {
            List<doWHTReportForIMS> result = base.GetWHTReportForIMS(yearMonthFrom, yearMonthTo);
            if (result != null && result.Count > 0)
            {
                CommonUtil.MappingObjectLanguage(result);
            }
            return result;
        }

        public List<doDebtTracingCustList> GetDebtTracingCustList(string empNo, doDebtTracingCustListSearchCriteria param)
        {
            var lstSubStatus = new List<string>();
            var permission = this.GetTbm_DebtTracingPermission(empNo);
            if (permission != null && permission.Count > 0)
            {
                if (param.ShowOutstanding ?? false) lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ_OUTSTANDING);
                if (param.ShowPending ?? false)
                {
                    lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ_PENDING_CALL);
                    lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ_PENDING_MATCH);
                }
                if (param.ShowNotDue ?? false) lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ_NOTDUE);
                if (param.ShowBranch ?? false)
                {
                    lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_OUTSTANDING);
                    lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_PENDING_CALL);
                    lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_PENDING_MATCH);
                    lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_NOTDUE);
                }
                if (param.ShowLawsuit ?? false) lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_LAWSUIT);
            }
            else
            {
                if (param.ShowOutstanding ?? false) lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_OUTSTANDING);
                if (param.ShowPending ?? false)
                {
                    lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_PENDING_CALL);
                    lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_PENDING_MATCH);
                }
                if (param.ShowNotDue ?? false) lstSubStatus.Add(DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_NOTDUE);
            }

            string substatus = null;
            if (lstSubStatus.Count > 0)
            {
                substatus = string.Join(",", lstSubStatus);
            }

            string contractcodelong = new CommonUtil().ConvertContractCode(param.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);

            var result = base.GetDebtTracingCustList(
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN,
                PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED_RETURN,
                PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED_RETURN,
                empNo,
                param.BillingOfficeCode,
                substatus,
                contractcodelong,
                param.BillingClientName,
                param.InvoiceNo,
                param.FirstFeeFlag
            );

            if (result != null && result.Count > 0)
            {
                CommonUtil.MappingObjectLanguage(result);
            }
            return result;
        }

        public List<tbt_DebtTracingHistory> InsertTbt_DebtTracingHistory(List<tbt_DebtTracingHistory> doInsertList)
        {
            try
            {
                if (doInsertList == null || doInsertList.Count <= 0)
                    return null;

                foreach (var row in doInsertList)
                {
                    row.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    row.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    row.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    row.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }

                var insertList = base.InsertTbt_DebtTracingHistory(
                    CommonUtil.ConvertToXml_Store<tbt_DebtTracingHistory>(doInsertList)
                );

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DEBT_TRACING_HISTORY;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<tbt_DebtTracingHistoryDetail> InsertTbt_DebtTracingHistoryDetail(List<tbt_DebtTracingHistoryDetail> doInsertList)
        {
            try
            {
                if (doInsertList == null || doInsertList.Count <= 0)
                    return null;

                var insertList = base.InsertTbt_DebtTracingHistoryDetail(
                    CommonUtil.ConvertToXml_Store<tbt_DebtTracingHistoryDetail>(doInsertList)
                );

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DEBT_TRACING_HISTORY_DETAIL;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<tbt_DebtTracingCustCondition> InsertTbt_DebtTracingCustCondition(List<tbt_DebtTracingCustCondition> doInsertList)
        {
            try
            {
                if (doInsertList == null || doInsertList.Count <= 0)
                    return null;

                foreach (var row in doInsertList)
                {
                    row.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    row.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    row.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    row.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }

                var insertList = base.InsertTbt_DebtTracingCustCondition(
                    CommonUtil.ConvertToXml_Store<tbt_DebtTracingCustCondition>(doInsertList)
                );

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DEBT_TRACING_CUST_CONDITION;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<tbt_DebtTracingCustCondition> UpdateTbt_DebtTracingCustCondition(List<tbt_DebtTracingCustCondition> doUpdateList)
        {
            try
            {
                if (doUpdateList == null || doUpdateList.Count <= 0)
                    return null;

                foreach (var row in doUpdateList)
                {
                    row.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                    row.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                }

                var updateList = base.UpdateTbt_DebtTracingCustCondition(
                    CommonUtil.ConvertToXml_Store<tbt_DebtTracingCustCondition>(doUpdateList)
                );

                //Insert Log
                if (updateList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_DEBT_TRACING_CUST_CONDITION;
                    logData.TableData = CommonUtil.ConvertToXml(updateList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updateList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<doGenerateDebtTracingNotice> GenerateDebtTracingNotice(DateTime batchDate, string documentType, string contractType, DateTime updateDate, string updateBy)
        {
            try
            {
                var docno = base.GenerateDebtTracingNotice(batchDate, documentType, contractType, updateDate, updateBy);
                return docno;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<doGetMatchGroupNamePayment> getMatchGroupNameCbo(DateTime? paymentDate, string empno)
        {
            try
            {

                var list = base.GetMatchGroupNamePayment(empno, paymentDate);
                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<doGetICR050> GetListIRC050(doMatchRReport doMatchReport)
        {
            try
            {

                var list = base.GetICR050(doMatchReport.PaymentDate, doMatchReport.GroupName, doMatchReport.CreateBy);
                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SaveDebtTracingInput(doDebtTracingInput input, bool isHQUser)
        {
            try
            {
                var handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var masHandler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                #region tbt_DebtTracingHistory
                var tmpHist = new tbt_DebtTracingHistory()
                {
                    BillingTargetCode = input.BillingTargetCode,
                    CallDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Date,
                    CallResult = input.Result,
                    Remark = input.Remark,
                    ServiceTypeCode = input.ServiceTypeCode,
                };

                if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_WAIT_MATCH)
                {
                    tmpHist.NextCallDate = input.NextCallDate;
                    if (isHQUser)
                    {
                        tmpHist.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ_OUTSTANDING;
                    }
                    else
                    {
                        tmpHist.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_OUTSTANDING;
                    }
                }
                else if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_POSTPONE)
                {
                    tmpHist.NextCallDate = input.NextCallDate;
                    tmpHist.PostponeReason = input.PostponeReason;
                    if (isHQUser)
                    {
                        tmpHist.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ_OUTSTANDING;
                    }
                    else
                    {
                        tmpHist.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_OUTSTANDING;
                    }
                }
                else if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_WAIT_FOR_PAYMENT)
                {
                    tmpHist.NextCallDate = input.NextCallDate;
                    tmpHist.EstimatedDate = input.EstimateDate;

                    //tmpHist.EstimatedAmount = input.Amount; Comment by Jirawat jannet on 2016-10-31

                    // Comment by Jirawat Jannet on 2016-11-08
                    //// Add by jirawat jannet on 2016-10-31
                    //tmpHist.EstimatedAmountCurrencyType = null;
                    //tmpHist.EstimatedAmount = null; ;
                    //tmpHist.EstimatedAmountUsd = null;
                    //if (input.Amount != null)
                    //{
                    //    tmpHist.EstimatedAmountCurrencyType = input.AmountCurrencyType;
                    //    if (input.AmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    //    {
                    //        tmpHist.EstimatedAmount = input.Amount;
                    //        tmpHist.EstimatedAmountUsd = null;
                    //    }
                    //    else if (input.AmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    //    {
                    //        tmpHist.EstimatedAmount = null;
                    //        tmpHist.EstimatedAmountUsd = input.Amount;
                    //    }
                    //}
                    //// End add

                    // Add by Jirawat Jannet on 2016-11-08
                    tmpHist.EstimatedAmountCurrencyType = null;
                    tmpHist.EstimatedAmount = input.Amount;
                    tmpHist.EstimatedAmountUsd = input.AmountUsd;


                    tmpHist.PaymentMethod = input.PaymentMethod;
                    if (isHQUser)
                    {
                        tmpHist.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ_PENDING_CALL;
                    }
                    else
                    {
                        tmpHist.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_PENDING_CALL;
                    }
                }
                else if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_BRANCH)
                {
                    tmpHist.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR_PENDING_CALL;
                }
                else if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_HQ)
                {
                    tmpHist.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ_PENDING_CALL;
                }
                else if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_LAWSUIT)
                {
                    tmpHist.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_LAWSUIT;
                }
                else
                {
                    throw new ArgumentNullException("input.Result");
                }

                var his = handler.InsertTbt_DebtTracingHistory(new List<tbt_DebtTracingHistory>() { tmpHist }).FirstOrDefault();
                if (his == null)
                {
                    throw new ApplicationException("Cannot get HistoryID from InsertTbt_DebtTracingHistory()");
                }
                #endregion

                #region tbt_DebtTracingHistoryDetail
                if (input.InvoiceNoList != null && input.InvoiceNoList.Count > 0)
                {
                    var tmpHistDetail = input.InvoiceNoList.Select(d =>
                        new tbt_DebtTracingHistoryDetail()
                        {
                            HistoryID = his.HistoryID,
                            InvoiceNo = d
                        }
                    ).ToList(); 

                    var histdetail = handler.InsertTbt_DebtTracingHistoryDetail(tmpHistDetail);
                }
                #endregion

                #region tbt_DebtTracingCustCondition
                var currentCC = handler.GetTbt_DebtTracingCustCondition(input.BillingTargetCode);
                var lstCCInsert = new List<tbt_DebtTracingCustCondition>();
                var lstCCUpdate = new List<tbt_DebtTracingCustCondition>();

                if (input.ServiceTypeCode == null || input.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE)
                {
                    tbt_DebtTracingCustCondition tmpCCSale = currentCC.Where(cc => cc.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_SALE).FirstOrDefault();
                    if (tmpCCSale == null)
                    {
                        tmpCCSale = new tbt_DebtTracingCustCondition()
                        {
                            BillingTargetCode = input.BillingTargetCode,
                            ServiceTypeCode = ServiceType.C_SERVICE_TYPE_SALE,
                            DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ,
                            Due = 1,
                            LastResult = input.Result,
                            CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                        };
                        lstCCInsert.Add(tmpCCSale);
                    }
                    else
                    {
                        tmpCCSale.LastResult = input.Result;
                        lstCCUpdate.Add(tmpCCSale);
                    }

                    if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_WAIT_MATCH
                        || input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_POSTPONE
                        || input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_WAIT_FOR_PAYMENT
                        )
                    {
                        tmpCCSale.PendingToDate = input.NextCallDate;
                    }
                    else if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_BRANCH)
                    {
                        tmpCCSale.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR;
                    }
                    else if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_HQ)
                    {
                        tmpCCSale.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ;
                    }
                    else if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_LAWSUIT)
                    {
                        tmpCCSale.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_LAWSUIT;
                    }
                }

                if (input.ServiceTypeCode == null || input.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL)
                {
                    tbt_DebtTracingCustCondition tmpCCRental = currentCC.Where(cc => cc.ServiceTypeCode == ServiceType.C_SERVICE_TYPE_RENTAL).FirstOrDefault();
                    if (tmpCCRental == null)
                    {
                        tmpCCRental = new tbt_DebtTracingCustCondition()
                        {
                            BillingTargetCode = input.BillingTargetCode,
                            ServiceTypeCode = ServiceType.C_SERVICE_TYPE_RENTAL,
                            DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ,
                            Due = 1,
                            LastResult = input.Result,
                            CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                        };
                        lstCCInsert.Add(tmpCCRental);
                    }
                    else
                    {
                        tmpCCRental.LastResult = input.Result;
                        lstCCUpdate.Add(tmpCCRental);
                    }

                    if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_WAIT_MATCH
                        || input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_POSTPONE
                        || input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_WAIT_FOR_PAYMENT
                        )
                    {
                        tmpCCRental.PendingToDate = input.NextCallDate;
                    }
                    else if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_BRANCH)
                    {
                        tmpCCRental.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_BR;
                    }
                    else if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_HQ)
                    {
                        tmpCCRental.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_HQ;
                    }
                    else if (input.Result == DebtTracingResult.C_DEBT_TRACE_RESULT_LAWSUIT)
                    {
                        tmpCCRental.DebtTracingStatus = DebtTracingSubStatus.C_DEBT_TRACE_SUBSTATUS_LAWSUIT;
                    }
                }

                if (lstCCInsert.Count > 0) handler.InsertTbt_DebtTracingCustCondition(lstCCInsert);
                if (lstCCUpdate.Count > 0) handler.UpdateTbt_DebtTracingCustCondition(lstCCUpdate);

                #endregion

            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
