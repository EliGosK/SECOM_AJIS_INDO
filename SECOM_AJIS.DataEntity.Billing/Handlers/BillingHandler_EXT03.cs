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
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;

using SECOM_AJIS.DataEntity.Billing.CustomEntity;
using System.Data.Objects;

// Used by Income
namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class BillingHandler : BizBLDataEntities, IBillingHandler
    {
        #region Invoice
        /// <summary>
        /// Get invoice and billing detail of invoice 
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        public doInvoice GetInvoice(string invoiceNo)
        {
            doInvoice invoice = base.GetInvoice(invoiceNo,CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US).FirstOrDefault();
            if (invoice != null)
            {
                invoice.Tbt_BillingDetails = GetTbt_BillingDetailOfInvoice(invoiceNo, invoice.InvoiceOCC);
            }
            return invoice;
        }
        /// <summary>
        /// To update data to tbt_Invoice
        /// </summary>
        /// <param name="xmlTbt_Invoice"></param>
        /// <returns></returns>
        public override List<tbt_Invoice> UpdateTbt_Invoice(string xmlTbt_Invoice)
        {
            List<tbt_Invoice> result = base.UpdateTbt_Invoice(xmlTbt_Invoice);
            if (result.Count > 0)
            {
                //Updated
                return result;
            }
            //No updated data
            return null;
        }

        /// <summary>
        /// Function for checking that whether tax invoice has been issued for the invoice.
        /// </summary>
        /// <param name="doInvoice"></param>
        /// <returns></returns>
        public bool CheckTaxInvoiceIssued(doInvoice doInvoice)
        {
            ObjectParameter paramIsIssued = new ObjectParameter("IsIssued", false);
            base.CheckTaxInvoiceIssued(doInvoice.InvoiceNo, doInvoice.InvoiceOCC, paramIsIssued);
            bool isIssued = Convert.ToBoolean(paramIsIssued.Value);
            return isIssued;
        }

        //ICS090
        /// <summary>
        /// To check exist in receipt data by invoice
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        public bool CheckExistReceiptForInvoice(string invoiceNo, int invoiceOCC)
        {
            bool result = false;
            ObjectParameter isExist = new ObjectParameter("IS_EXIST", false);
            base.CheckExistReceiptForInvoice(invoiceNo, (int?)invoiceOCC, isExist);
            result = (bool)isExist.Value;
            return result;
        }
        /// <summary>
        /// Function for checking whether invoice has been issued a tax invoice
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        public bool CheckInvoiceIssuedTaxInvoice(string invoiceNo, int invoiceOCC)
        {
            bool result = false;
            ObjectParameter isIssued = new ObjectParameter("IS_ISSUED", false);
            base.CheckInvoiceIssuedTaxInvoice(invoiceNo, (int?)invoiceOCC, isIssued);
            result = (bool)isIssued.Value;
            return result;
        }
        /// <summary>
        /// To update invoice correction reason
        /// </summary>
        /// <param name="doInvoice"></param>
        /// <param name="correctionReason"></param>
        /// <returns></returns>
        public bool UpdateInvoiceCorrectionReason(doInvoice doInvoice, string correctionReason)
        {
            List<tbt_Invoice> result = base.UpdateInvoiceCorrectionReason(doInvoice.InvoiceNo,(int?) doInvoice.InvoiceOCC, correctionReason
                , CommonUtil.dsTransData.dtUserData.EmpNo
                , CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                );
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_INVOICE,
                    TableData = CommonUtil.ConvertToXml(result)
                };
                logHand.WriteTransactionLog(logData);
                //Updated 
                return true;
            }

            //No updated data
            return false;
        }

        //ICS040

        /// <summary>
        /// Register billing exemption by updating payment status to invoice and billing detail.
        /// </summary>
        /// <param name="doInvoiceExemption"></param>
        /// <returns></returns>
        public bool RegisterInvoiceExemption(doInvoice doInvoiceExemption)
        {
            if (doInvoiceExemption == null)
                return false;

            //Invoice
            List<tbt_Invoice> result = base.RegisterInvoiceExemption(doInvoiceExemption.InvoiceNo, doInvoiceExemption.InvoiceOCC,
                PaymentStatus.C_PAYMENT_STATUS_BILLING_EXEMPTION, doInvoiceExemption.ExemptApproveNo,
                CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
            if (result == null || result.Count == 0)
                return false;

            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            doTransactionLog logData = new doTransactionLog()
            {
                TransactionType = doTransactionLog.eTransactionType.Update,
                TableName = TableName.C_TBL_NAME_INVOICE,
                TableData = null
            };
            logData.TableData = CommonUtil.ConvertToXml(result);
            logHand.WriteTransactionLog(logData);


            //Billing details
            if (doInvoiceExemption.Tbt_BillingDetails != null && doInvoiceExemption.Tbt_BillingDetails.Count > 0)
            {
                List<tbt_BillingDetail> billingDetailResult = base.RegisterBillingDetailExemption(doInvoiceExemption.InvoiceNo, doInvoiceExemption.InvoiceOCC,
                    PaymentStatus.C_PAYMENT_STATUS_BILLING_EXEMPTION,
                    CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                if (billingDetailResult == null || billingDetailResult.Count == 0
                    || billingDetailResult.Count != billingDetailResult.Count)
                    return false;

                logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_INVOICE,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(billingDetailResult);
                logHand.WriteTransactionLog(logData);
            }


            //Success
            return true;
        }

        //ICS060

        /// <summary>
        /// Mapping billing type code to invoice type: Deposit invoice , Service invoice ,Sales invoice
        /// </summary>
        /// <param name="billingTypeCode"></param>
        /// <returns></returns>
        public string GetInvoiceType(string billingTypeCode)
        {
            if (billingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRODUCT_PRICE
                || billingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE
                || billingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_DOWN
                || billingTypeCode == BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL
                || billingTypeCode == BillingType.C_BILLING_TYPE_CARD)
            {
                return InvoiceType.C_INVOICE_TYPE_SALE;
            }
            else if (billingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
            {
                return InvoiceType.C_INVOICE_TYPE_DEPOSIT;
            }
            else
            {
                return InvoiceType.C_INVOICE_TYPE_SERVICE;
            }
        }

        /// <summary>
        /// To update first issue
        /// </summary>
        /// <param name="xmlTbt_Invoice"></param>
        /// <returns></returns>
        public List<tbt_Invoice> UpdateFirstIssue(string DocumentNo,string DocumentOCC, DateTime BatchDate,string UpdateBy)
        {
            List<tbt_Invoice> result = base.UpdateFirstIssue(DocumentNo, DocumentOCC, BatchDate, BatchDate, UpdateBy);
            if (result.Count > 0)
            {
                //Updated
                return result;
            }
            //No updated data
            return null;
        }
        #endregion

        #region TaxInvoice
        //ICS084 : call by screen

        /// <summary>
        /// To update receipt no. to tax invoice table
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        public bool UpdateReceiptNo(string invoiceNo, int invoiceOCC, string receiptNo)
        {
            List<tbt_TaxInvoice> result = base.UpdateReceiptNo(invoiceNo, invoiceOCC, receiptNo
                 , CommonUtil.dsTransData.dtUserData.EmpNo
                 , CommonUtil.dsTransData.dtOperationData.ProcessDateTime
                 );
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_RECEIPT,
                    TableData = CommonUtil.ConvertToXml(result)
                };
                logHand.WriteTransactionLog(logData);
                //Updated 
                return true;
            }

            //No updated data
            return false;
        }

        //ICP010 : Call by batch process

        /// <summary>
        /// To update receipt no. to tax invoice table
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <param name="receiptNo"></param>
        /// <param name="batchId"></param>
        /// <param name="batchDate"></param>
        /// <returns></returns>
        public bool UpdateReceiptNo(string invoiceNo, int invoiceOCC, string receiptNo, string batchId, DateTime batchDate)
        {
            List<tbt_TaxInvoice> result = base.UpdateReceiptNo(invoiceNo, invoiceOCC, receiptNo
                , batchId
                , batchDate
                );
            if (result != null && result.Count > 0)
            {
                //No logging

                //Updated 
                return true;
            }

            //No updated data
            return false;
        }

        /// <summary>
        /// Cancel tax invoice by marking cancel flag.
        /// </summary>
        /// <param name="taxtInvoiceNo"></param>
        /// <returns></returns>
        public bool CancelTaxInvoice(string taxtInvoiceNo)
        {
            List<tbt_TaxInvoice> result = base.CancelTaxInvoice(taxtInvoiceNo
                    , CommonUtil.dsTransData.dtUserData.EmpNo
                    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_TAX_INVOICE,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(result);
                logHand.WriteTransactionLog(logData);
                return true;
            }
            return false;
        }
        
        //ICS050

        /// <summary>
        /// To force issue tax invoice
        /// </summary>
        /// <param name="doInvoice"></param>
        /// <param name="taxInvoiceDate"></param>
        /// <returns></returns>
        public tbt_TaxInvoice ForceIssueTaxInvoice(doInvoice doInvoice, DateTime taxInvoiceDate)
        {
            string runningNo = this.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_TAX_INVOICE
                , CommonUtil.dsTransData.dtUserData.EmpNo
                , taxInvoiceDate/*CommonUtil.dsTransData.dtOperationData.ProcessDateTime*/); //Edit by Patcharee T. For get Tax invoice no. in month of taxInvoiceDate 11-Jun-2013
            tbt_TaxInvoice doTaxInvoice = new tbt_TaxInvoice()
            {
                TaxInvoiceNo = runningNo,
                InvoiceNo = doInvoice.InvoiceNo,
                InvoiceOCC = (int?)doInvoice.InvoiceOCC,
                ReceiptNo = null,
                TaxInvoiceDate = taxInvoiceDate,
                TaxInvoiceCanceledFlag = false,
                TaxInvoiceIssuedFlag = true,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
            };

            //Modify by Jutarat A. on 30072013
            //List<tbt_TaxInvoice> doTaxInvoices = new List<tbt_TaxInvoice>();
            //doTaxInvoices.Add(doTaxInvoice);
            //List<tbt_TaxInvoice> result = this.InsertTbt_TaxInvoice(CommonUtil.ConvertToXml_Store<tbt_TaxInvoice>(doTaxInvoices));
            List<tbt_TaxInvoice> result = CreateTbt_TaxInvoice(doTaxInvoice);
            //End Modify

            if (result != null && result.Count > 0)
            {
                //success
                return result[0];
            }

            //no insert data
            return null;
        }

        //ICP010

        /// <summary>
        /// Force issue a receipt for unpaid invoice.
        /// </summary>
        /// <param name="doInvoice"></param>
        /// <param name="taxInvoiceDate"></param>
        /// <param name="batchId"></param>
        /// <param name="batchDate"></param>
        /// <returns></returns>
        public tbt_TaxInvoice IssueTaxInvoice(doInvoice doInvoice, DateTime taxInvoiceDate, string batchId, DateTime batchDate)
        {
            string runningNo = this.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_TAX_INVOICE, batchId, taxInvoiceDate/*batchDate*/); //Edit by Patcharee T. For issue Tax invoice no. in month of taxInvoiceDate

            //Determin receipt issue flag
            bool receiptIssueFlag = doInvoice.IssueReceiptTiming == IssueRecieptTime.C_ISSUE_REC_TIME_NOT_ISSUE ? false : true;

            tbt_TaxInvoice doTaxInvoice = new tbt_TaxInvoice()
            {
                TaxInvoiceNo = runningNo,
                InvoiceNo = doInvoice.InvoiceNo,
                InvoiceOCC = (int?)doInvoice.InvoiceOCC,
                ReceiptNo = null,
                TaxInvoiceDate = taxInvoiceDate,
                TaxInvoiceCanceledFlag = false,
                TaxInvoiceIssuedFlag = receiptIssueFlag,
                CreateDate = batchDate,
                CreateBy = batchId,
                UpdateDate = batchDate,
                UpdateBy = batchId
            };
            List<tbt_TaxInvoice> doTaxInvoices = new List<tbt_TaxInvoice>();
            doTaxInvoices.Add(doTaxInvoice);
            List<tbt_TaxInvoice> result = this.InsertTbt_TaxInvoice(CommonUtil.ConvertToXml_Store<tbt_TaxInvoice>(doTaxInvoices));
            if (result != null && result.Count > 0)
            {
                //success
                return result[0];
            }
            //no insert data
            return null;
        }
        #endregion

        #region BillingBasic

        /// <summary>
        /// Function for updating balance of deposit for billing basic.
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="ajustAmount"></param>
        /// <param name="depositBalanceAfterUpdate"></param>
        /// <returns></returns>
        public bool UpdateBalanceDepositOfBillingBasic(string contractCode, string billingOCC
            , decimal? ajustAmount, decimal? ajustAmountUsd, string BalanceDepositCurrencyType
            , out decimal? depositBalanceAfterUpdate, out decimal? depositBalanceUsdAfterUpdate, out string balanceDepositAfterUpdateCurrencyType)
        {
            List<tbt_BillingBasic> result = base.UpdateBalanceDepositOfBillingBasic(contractCode, billingOCC, ajustAmount
                , CommonUtil.dsTransData.dtUserData.EmpNo
                , CommonUtil.dsTransData.dtOperationData.ProcessDateTime, ajustAmountUsd, BalanceDepositCurrencyType);
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_BILLING_BASIC,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(result);
                logHand.WriteTransactionLog(logData);

                //return desposit balance
                depositBalanceAfterUpdate = result[0].BalanceDeposit;
                depositBalanceUsdAfterUpdate = result[0].BalanceDepositUsd;
                balanceDepositAfterUpdateCurrencyType = result[0].BalanceDepositCurrencyType;
                return true;
            }
            depositBalanceAfterUpdate = null;
            balanceDepositAfterUpdateCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
            depositBalanceUsdAfterUpdate = null;
            return false;
        }
        #endregion

        #region Refund
        /// <summary>
        /// Function for retrieving information of refund deposit payment 
        /// </summary>
        /// <param name="paymentTransNo"></param>
        /// <returns></returns>
        public doRefundInfo GetRefundInfo(string paymentTransNo)
        {
            doRefundInfo result = base.GetRefundInfo(paymentTransNo).FirstOrDefault();
            return result;
        }
        #endregion

        #region Deposit Fee
        /// <summary>
        /// Insert deposit fee transaction table after payment
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="depositAmount"></param>
        /// <param name="balanceDeposit"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        public bool InsertDepositFeePayment(string contractCode, string billingOCC, decimal? depositAmount, decimal? depositAmountUsd, string depositAmountCurrencyType, decimal? balanceDeposit, decimal? balanceDepositUsd, string balanceDepositCurrencyType, string invoiceNo, string receiptNo)
        {
            tbt_Depositfee doDepositFee = new tbt_Depositfee()
            {
                ContractCode = contractCode,
                BillingOCC = billingOCC,
                DepositFeeNo = 0,  //Max running + 1 Move logic to sp_BL_InsertTbt_Depositfee
                ProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                DepositStatus = DepositStatus.C_DEPOSIT_STATUS_PAYMENT,

                ProcessAmount = depositAmount,
                ProcessAmountUsd = depositAmountUsd,
                ProcessAmountCurrencyType = depositAmountCurrencyType,

                ReceivedFee = balanceDeposit,
                ReceivedFeeUsd = balanceDepositUsd,
                ReceivedFeeCurrencyType = balanceDepositCurrencyType,

                InvoiceNo = (string.IsNullOrEmpty(receiptNo)? invoiceNo: null),
                ReceiptNo = receiptNo,
                CreditNoteNo = null,
                SlideBillingCode = null,
                RevenueNo = null,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
            };
            var result = CreateTbt_Depositfee(doDepositFee);
            return result.Count > 0;
        }
        /// <summary>
        /// Insert deposit fee transaction table after payment
        /// </summary>
        /// <param name="doSlideRefund"></param>
        /// <param name="slidingAmount"></param>
        /// <param name="balanceDeposit"></param>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        public bool InsertDepositFeeSlide(doRefundInfo doSlideRefund, decimal? slidingAmount, decimal? balanceDeposit
            , string contractCode, string billingOCC, string processAmountCurrencyType, decimal? processAmountUsd
            , string receivedFeeCurrencyType, decimal? receivedFeeUsd)
        {
            tbt_Depositfee doDepositFee = new tbt_Depositfee()
            {
                ContractCode = doSlideRefund.ContractCode,
                BillingOCC = doSlideRefund.BillingOCC,
                DepositFeeNo = 0,  //Max running + 1 Move logic to sp_BL_InsertTbt_Depositfee,
                ProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                DepositStatus = DepositStatus.C_DEPOSIT_STATUS_SLIDE,
                ProcessAmount = slidingAmount,
                ReceivedFee = balanceDeposit,
                InvoiceNo = null,
                ReceiptNo = null,
                CreditNoteNo = doSlideRefund.CreditNoteNo,
                SlideBillingCode = contractCode + "-" + billingOCC,
                RevenueNo = null,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,

                ProcessAmountCurrencyType = processAmountCurrencyType,
                ProcessAmountUsd = processAmountUsd,
                ReceivedFeeCurrencyType = receivedFeeCurrencyType,
                ReceivedFeeUsd = receivedFeeUsd
            };
            var result = CreateTbt_Depositfee(doDepositFee);
            return result.Count > 0;
        }
        /// <summary>
        /// Insert return deposit fee transaction after payment matching or deleted
        /// </summary>
        /// <param name="doSlideRefund"></param>
        /// <param name="returnAmount"></param>
        /// <param name="balanceDeposit"></param>
        /// <returns></returns>
        public bool InsertDepositFeeReturn(doRefundInfo doSlideRefund, decimal? returnAmount, decimal? returnAmountUsd, string returnAmountCurrenctyType
            , decimal? balanceDeposit, decimal? balanceDepositUsd, string balanceDepositCurrencyType)
        {
            tbt_Depositfee doDepositFee = new tbt_Depositfee()
            {
                ContractCode = doSlideRefund.ContractCode,
                BillingOCC = doSlideRefund.BillingOCC,
                DepositFeeNo = 0,  //Max running + 1 Move logic to sp_BL_InsertTbt_Depositfee,
                ProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                DepositStatus = DepositStatus.C_DEPOSIT_STATUS_RETURN,
                ProcessAmount = returnAmount,
                ReceivedFee = balanceDeposit,
                InvoiceNo = null,
                ReceiptNo = null,
                CreditNoteNo = doSlideRefund.CreditNoteNo,
                SlideBillingCode = null,
                RevenueNo = null,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                ProcessAmountUsd = returnAmountUsd,
                ProcessAmountCurrencyType = returnAmountCurrenctyType,
                ReceivedFeeUsd = balanceDepositUsd,
                ReceivedFeeCurrencyType = balanceDepositCurrencyType
            };
            var result = CreateTbt_Depositfee(doDepositFee);
            return result.Count > 0;
        }
        /// <summary>
        /// Insert deposit fee transaction of cancel of slide after cancel payment matching.
        /// </summary>
        /// <param name="doSlideRefund"></param>
        /// <param name="cancelAmount"></param>
        /// <param name="balanceDeposit"></param>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        public bool InsertDepositFeeCancelSlide(doRefundInfo doSlideRefund, decimal cancelAmount, decimal balanceDeposit, string contractCode, string billingOCC, decimal cancelAmountUsd, string cancelAmountCurrencyType)
        {
            tbt_Depositfee doDepositFee = new tbt_Depositfee()
            {
                ContractCode = doSlideRefund.ContractCode,
                BillingOCC = doSlideRefund.BillingOCC,
                DepositFeeNo = 0,  //Max running + 1 Move logic to sp_BL_InsertTbt_Depositfee,
                ProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                DepositStatus = DepositStatus.C_DEPOSIT_STATUS_CANCEL_SLIDE,
                ProcessAmount = cancelAmount,
                ProcessAmountUsd = cancelAmountUsd,
                ProcessAmountCurrencyType = cancelAmountCurrencyType,
                ReceivedFee = balanceDeposit,
                InvoiceNo = null,
                ReceiptNo = null,
                CreditNoteNo = doSlideRefund.CreditNoteNo,
                SlideBillingCode = contractCode + "-" + billingOCC,
                RevenueNo = null,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
            };
            var result = CreateTbt_Depositfee(doDepositFee);
            return result.Count > 0;
        }
        /// <summary>
        /// Insert deposit fee transaction of cancel of payment after cancel match payment.
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="cancelAmount"></param>
        /// <param name="balanceDeposit"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        public bool InsertDepositFeeCancelPayment(string contractCode, string billingOCC, decimal cancelAmount, decimal balanceDeposit, string invoiceNo, string receiptNo, decimal cancelAmountUsd, string cancelAmountCurrencyType)
        {
            tbt_Depositfee doDepositFee = new tbt_Depositfee()
            {
                ContractCode = contractCode,
                BillingOCC = billingOCC,
                DepositFeeNo = 0,  //Max running + 1 Move logic to sp_BL_InsertTbt_Depositfee
                ProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                DepositStatus = DepositStatus.C_DEPOSIT_STATUS_CANCEL_PAYMENT,
                ProcessAmount = cancelAmount,
                ProcessAmountUsd = cancelAmountUsd,
                ProcessAmountCurrencyType = cancelAmountCurrencyType,
                ReceivedFee = balanceDeposit,
                InvoiceNo = (string.IsNullOrEmpty(receiptNo) ? invoiceNo : null),
                ReceiptNo = receiptNo,
                CreditNoteNo = null,
                SlideBillingCode = null,
                RevenueNo = null,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
            };
            var result = CreateTbt_Depositfee(doDepositFee);
            return result.Count > 0;
        }

        /// <summary>
        /// Insert revenue deposit fee transaction after register revenue from deposit fee.
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="cancelAmount"></param>
        /// <param name="balanceDeposit"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        public bool InsertDepositFeeRevenue(string contractCode, string billingOCC, decimal revenueAmount, decimal revenueAmountUsd
            , string revenueAmountCurrencyType, decimal balanceDeposit, decimal balanceDepositUsd, string balanceDepositCurrencyType, string revenueNo)
        {
            tbt_Depositfee doDepositFee = new tbt_Depositfee()
            {
                ContractCode = contractCode,
                BillingOCC = billingOCC,
                DepositFeeNo = 0,  //Max running + 1 Move logic to sp_BL_InsertTbt_Depositfee
                ProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                DepositStatus = DepositStatus.C_DEPOSIT_STATUS_REVENUE,
                ProcessAmount = revenueAmount,
                ProcessAmountUsd = revenueAmountUsd,
                ProcessAmountCurrencyType = revenueAmountCurrencyType,
                ReceivedFee = balanceDeposit,
                ReceivedFeeUsd = balanceDepositUsd,
                ReceivedFeeCurrencyType = balanceDepositCurrencyType,
                InvoiceNo = null,
                ReceiptNo = null,
                CreditNoteNo = null,
                SlideBillingCode = null,
                RevenueNo = revenueNo,
                CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime
            };
            var result = CreateTbt_Depositfee(doDepositFee);
            return result.Count > 0;
        }
        
        
        //ICP010
        /// <summary>
        /// Update receipt no. to deposit fee transaction after receipt generated.
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="receiptNo"></param>
        /// <param name="batchId"></param>
        /// <param name="batchDate"></param>
        /// <returns></returns>
        public bool UpdateReceiptNoDepositFee(string invoiceNo, string receiptNo, string batchId, DateTime batchDate)
        {
            List<tbt_Depositfee> result = base.UpdateReceiptNoDepositFee(invoiceNo, receiptNo
                , batchId
                , batchDate
                , DepositStatus.C_DEPOSIT_STATUS_PAYMENT);
            if (result != null && result.Count > 0)
            {
                //No logging for batch process
                //ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                //doTransactionLog logData = new doTransactionLog()
                //{
                //    TransactionType = doTransactionLog.eTransactionType.Update,
                //    TableName = TableName.C_TBL_NAME_DEPOSIT_FEE,
                //    TableData = null
                //};
                //logData.TableData = CommonUtil.ConvertToXml(result);
                //logHand.WriteTransactionLog(logData);
                return true;
            }
            return false;
        }

        //ICS060
        /// <summary>
        /// Update clear receipt no., invoice no of deposit fee transaction.
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="receiptNo"></param>
        /// <param name="batchId"></param>
        /// <param name="batchDate"></param>
        /// <returns></returns>
        public bool UpdateReceiptNoDepositFeeCancelReceipt(string invoiceNo, string receiptNo, string batchId, DateTime batchDate)
        {
            List<tbt_Depositfee> result = base.UpdateReceiptNoDepositFeeCancelReceipt(invoiceNo, receiptNo
                , batchId
                , batchDate
                , DepositStatus.C_DEPOSIT_STATUS_PAYMENT);
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_DEPOSIT_FEE,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(result);
                logHand.WriteTransactionLog(logData);
                return true;
            }
            return false;
        }

        #endregion

        #region AutoTransferBankAccount
        //ICS020
        /// <summary>
        /// Update auto transfer account last result from auto transfer import file.
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="lastestResult"></param>
        /// <returns></returns>
        public bool UpdateAutoTransferAccountLastResult(string invoiceNo, string lastestResult)
        {
            List<tbt_AutoTransferBankAccount> result = base.UpdateAutoTransferAccountLastResult(invoiceNo,lastestResult
                ,CommonUtil.dsTransData.dtUserData.EmpNo
                ,CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
            if (result != null && result.Count > 0)
            {
                ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_AUTO_TRANSFER_ACC,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(result);
                logHand.WriteTransactionLog(logData);
                return true;
            }
            return false;
        }
        #endregion

        public List<string> GetTbt_InvoiceReprint()
        {
            BLDataEntities context = new BLDataEntities();
            return context.tbt_InvoiceReprint.Select(d => d.InvoiceNo).ToList();
        }
    }
}
