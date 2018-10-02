using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Common.Service;

namespace SECOM_AJIS.DataEntity.Income
{
    public partial class IncomeHandler : BizICDataEntities, IIncomeHandler
    {
        #region Unpaid Invoice
        /// <summary>
        /// Function for retrieving unpaid invoice information of specific invoice no.(sp_IC_GetUnpaidInvoice)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <returns></returns>
        public List<doUnpaidInvoice> GetUnpaidInvoice(string invoiceNo)
        {
            List<doUnpaidInvoice> result = base.GetUnpaidInvoice(
                invoiceNo
                , PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                , PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                , PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                , PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                , PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                , PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                );
            return result;
        }
        /// <summary>
        /// Function for retrieving unpaid invoice information of specific billing target code. (sp_IC_GetUnpaidInvoiceByBillingTarget)
        /// </summary>
        /// <param name="billingTargetCode">billing target code</param>
        /// <returns></returns>
        public List<doUnpaidInvoice> GetUnpaidInvoiceByBillingTarget(string billingTargetCode)
        {
            List<doUnpaidInvoice> result = base.GetUnpaidInvoiceByBillingTarget(
                billingTargetCode
                , PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT
                , PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT
                , PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT
                , PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK
                , PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK
                , PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK
                , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID
                , PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                );
            return result;
        }
        #endregion

        #region Unpaid billing
        /// <summary>
        /// Function for retrieving unpaid billing target information of specific search criteria. (sp_IC_SearchUnpaidBillingTarget)
        /// </summary>
        /// <param name="doSearch">search criteria</param>
        /// <returns></returns>
        public List<doUnpaidBillingTarget> SearchUnpaidBillingTarget(doUnpaidBillingTargetSearchCriteria doSearch)
        {
            List<doUnpaidBillingTarget> result = base.SearchUnpaidBillingTarget(
                doSearch.BillingClientName
                , doSearch.InvoiceAmountCurrencyType, doSearch.InvoiceAmountFrom, doSearch.InvoiceAmountTo
                , doSearch.IssueInvoiceDateFrom, doSearch.IssueInvoiceDateTo
                , doSearch.HaveCreditNoteIssued
                , doSearch.BillingDetailAmountCurrencyType, doSearch.BillingDetailAmountFrom, doSearch.BillingDetailAmountTo
                , doSearch.BillingType_ContractFee ?? false ? BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES : null
                , doSearch.BillingType_InstallationFee ?? false ? BillingTypeGroup.C_BILLING_TYPE_GROUP_INSTALL : null
                , doSearch.BillingType_DepositFee ?? false ? BillingTypeGroup.C_BILLING_TYPE_GROUP_DEPOSIT : null
                , doSearch.BillingType_SalePrice ?? false ? BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE : null
                , doSearch.BillingType_OtherFee ?? false ? BillingTypeGroup.C_BILLING_TYPE_GROUP_OTHER : null
                , doSearch.PaymentMethod_BankTransfer ?? false ? PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER : null
                , doSearch.PaymentMethod_Messenger ?? false ? PaymentMethod.C_PAYMENT_METHOD_MESSENGER_TRANSFER : null
                , doSearch.PaymentMethod_AutoTransfer ?? false ? PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER : null
                , doSearch.PaymentMethod_CreditCard ?? false ? PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER : null
                , doSearch.BillingCycle
                , doSearch.LastPaymentDayFrom, doSearch.LastPaymentDayTo
                , doSearch.ExpectedPaymentDateFrom, doSearch.ExpectedPaymentDateTo
                , PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                 PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                , CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
            return result;
        }
        public List<doGetUnpaidBillingTargetByCodeWithExchange> SearchUnpaidBillingTargetWithExchange(doUnpaidBillingTargetSearchCriteria doSearch)
        {
            List<doGetUnpaidBillingTargetByCodeWithExchange> result = base.SearchUnpaidBillingTargetWithExchange(
                doSearch.BillingClientName
                , doSearch.InvoiceAmountCurrencyType, doSearch.InvoiceAmountFrom, doSearch.InvoiceAmountTo
                , doSearch.IssueInvoiceDateFrom, doSearch.IssueInvoiceDateTo
                , doSearch.HaveCreditNoteIssued
                , doSearch.BillingDetailAmountCurrencyType, doSearch.BillingDetailAmountFrom, doSearch.BillingDetailAmountTo
                , doSearch.BillingType_ContractFee ?? false ? BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES : null
                , doSearch.BillingType_InstallationFee ?? false ? BillingTypeGroup.C_BILLING_TYPE_GROUP_INSTALL : null
                , doSearch.BillingType_DepositFee ?? false ? BillingTypeGroup.C_BILLING_TYPE_GROUP_DEPOSIT : null
                , doSearch.BillingType_SalePrice ?? false ? BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE : null
                , doSearch.BillingType_OtherFee ?? false ? BillingTypeGroup.C_BILLING_TYPE_GROUP_OTHER : null
                , doSearch.PaymentMethod_BankTransfer ?? false ? PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER : null
                , doSearch.PaymentMethod_Messenger ?? false ? PaymentMethod.C_PAYMENT_METHOD_MESSENGER_TRANSFER : null
                , doSearch.PaymentMethod_AutoTransfer ?? false ? PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER : null
                , doSearch.PaymentMethod_CreditCard ?? false ? PaymentMethod.C_PAYMENT_METHOD_CREDIT_CARD_TRANSFER : null
                , doSearch.BillingCycle
                , doSearch.LastPaymentDayFrom, doSearch.LastPaymentDayTo
                , doSearch.ExpectedPaymentDateFrom, doSearch.ExpectedPaymentDateTo
                , PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                 PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                , CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
            return result;
        }


        /// <summary>
        /// Function for retrieving unpaid billing target information of specific billing code. (sp_IC_GetUnpaidBillingTargetByBillingCode)
        /// </summary>
        /// <param name="billingCode">billing code</param>
        /// <returns></returns>
        public List<doUnpaidBillingTarget> GetUnpaidBillingTargetByBillingCode(string billingCode)
        {
            List<doUnpaidBillingTarget> result = base.GetUnpaidBillingTargetByBillingCode(billingCode,
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                 PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN);
            return result;
        }
        public List<doGetUnpaidBillingTargetByCodeWithExchange> GetUnpaidBillingTargetByBillingCodeWithExchange(string billingCode)
        {
            List<doGetUnpaidBillingTargetByCodeWithExchange> result = base.GetUnpaidBillingTargetByBillingCodeWithExchange(billingCode,
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                 PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN,
                CurrencyUtil.C_CURRENCY_LOCAL,
                CurrencyUtil.C_CURRENCY_US);
            return result;
        }
        /// <summary>
        /// Function for retrieving unpaid billing target information of specific billing target code. (sp_IC_GetUnpaidBillingTargetByCode)
        /// </summary>
        /// <param name="billingTargetCode">billing target code</param>
        /// <returns></returns>
        public List<doUnpaidBillingTarget> GetUnpaidBillingTargetByCode(string billingTargetCode)
        {
            List<doUnpaidBillingTarget> result = base.GetUnpaidBillingTargetByCode(billingTargetCode,
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                 PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN);
            return result;
        }
        public List<doGetUnpaidBillingTargetByCodeWithExchange> GetUnpaidBillingTargetByCodeWithExchange(string billingTargetCode)
        {
            List<doGetUnpaidBillingTargetByCodeWithExchange> result = base.GetUnpaidBillingTargetByCodeWithExchange(billingTargetCode,
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                 PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN,
                CurrencyUtil.C_CURRENCY_LOCAL,
                CurrencyUtil.C_CURRENCY_US);
            return result;
        }
        /// <summary>
        /// Function for retrieving billing target information by customer code (sp_IC_GetUnpaidBillingTargetByCustomerCode)
        /// </summary>
        /// <param name="customerCode">Customer Code</param>
        /// <returns></returns>
        public List<doUnpaidBillingTarget> GetUnpaidBillingTargetByCustomerCode(string customerCode) //Add by Jutarat A. on 09042013
        {
            List<doUnpaidBillingTarget> result = base.GetUnpaidBillingTargetByCustomerCode(customerCode,
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                 PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN);
            return result;
        }
        public List<doGetUnpaidBillingTargetByCodeWithExchange> GetUnpaidBillingTargetByCustomerCodeWithExchange(string customerCode)
        {
            List<doGetUnpaidBillingTargetByCodeWithExchange> result = base.GetUnpaidBillingTargetByCustomerCodeWithExchange(customerCode,
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                 PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN,
                CurrencyUtil.C_CURRENCY_LOCAL,
                CurrencyUtil.C_CURRENCY_US);
            return result;
        }
        /// <summary>
        /// Function for retrieving unpaid billing target information of specific invoice no. (sp_IC_GetUnpaidBillingTargetByInvoiceNo)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <returns></returns>
        public List<doUnpaidBillingTarget> GetUnpaidBillingTargetByInvoiceNo(string invoiceNo)
        {
            List<doUnpaidBillingTarget> result = base.GetUnpaidBillingTargetByInvoiceNo(invoiceNo,
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                 PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN);
            return result;
        }
        public List<doGetUnpaidBillingTargetByCodeWithExchange> GetUnpaidBillingTargetByInvoiceNoWithExchange(string invoiceNo)
        {
            List<doGetUnpaidBillingTargetByCodeWithExchange> result = base.GetUnpaidBillingTargetByInvoiceNoWithExchange(invoiceNo,
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                 PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN,
                CurrencyUtil.C_CURRENCY_LOCAL,
                CurrencyUtil.C_CURRENCY_US);
            return result;
        }
        /// <summary>
        /// Function for retrieving unpaid billing target information of specific receipt no. (sp_IC_GetUnpaidBillingTargetByReceiptNo)
        /// </summary>
        /// <param name="receiptNo">receipt no.</param>
        /// <returns></returns>
        public List<doUnpaidBillingTarget> GetUnpaidBillingTargetByReceiptNo(string receiptNo)
        {
            List<doUnpaidBillingTarget> result = base.GetUnpaidBillingTargetByReceiptNo(receiptNo,
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                 PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN);
            return result;
        }
        public List<doGetUnpaidBillingTargetByCodeWithExchange> GetUnpaidBillingTargetByReceiptNoWithExchange(string receiptNo)
        {
            List<doGetUnpaidBillingTargetByCodeWithExchange> result = base.GetUnpaidBillingTargetByReceiptNoWithExchange(receiptNo,
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                 PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN,
                CurrencyUtil.C_CURRENCY_LOCAL,
                CurrencyUtil.C_CURRENCY_US);
            return result;
        }

        /// <summary>
        /// Function for retrieving unpaid billing detail information of specific billing target code. (sp_IC_GetUnpaidBillingDetailByBillingTarget)
        /// </summary>
        /// <param name="billingTargetCode"></param>
        /// <returns></returns>
        public List<doUnpaidBillingDetail> GetUnpaidBillingDetailByBillingTarget(string billingTargetCode)
        {
            List<doUnpaidBillingDetail> result = base.GetUnpaidBillingDetailByBillingTarget(
                billingTargetCode, MiscType.C_PAYMENT_STATUS,
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN);
            if (result != null && result.Count > 0)
            {
                //Language Mapping
                CommonUtil.MappingObjectLanguage<doUnpaidBillingDetail>(result);
            }
            return result;
        }
        /// <summary>
        /// Function for retrieving unpaid billing detail information of specific invoice no. and invoice occ (sp_IC_GetUnpaidBillingDetailByInvoice)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="invoiceOCC">invoice occ</param>
        /// <returns></returns>
        public List<doUnpaidBillingDetail> GetUnpaidBillingDetailByInvoice(string invoiceNo, int? invoiceOCC)
        {
            List<doUnpaidBillingDetail> result = base.GetUnpaidBillingDetailByInvoice(
                invoiceNo, invoiceOCC, MiscType.C_PAYMENT_STATUS,
                PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT,
                PaymentStatus.C_PAYMENT_STATUS_GEN_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID,
                PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN
                );
            if (result != null && result.Count > 0)
            {
                //Language Mapping
                CommonUtil.MappingObjectLanguage<doUnpaidBillingDetail>(result);
            }
            return result;
        }
        #endregion

        #region Matching Payment / Cancel
        //ICS080: View
        public List<tbt_Invoice> GetInvoicePaymentMatchingList(string paymentTransNo)
        {
            List<doInvoicePaymentMatchingList> doInvoiceNo = base.GetInvoicePaymentMatchingList(paymentTransNo);
            List<tbt_Invoice> doTbt_Invoice = new List<tbt_Invoice>();
            if (doInvoiceNo != null)
            {
                IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
                foreach (doInvoicePaymentMatchingList item in doInvoiceNo)
                {
                    tbt_Invoice doInv = billingHandler.GetTbt_InvoiceData(item.invoiceno, item.InvoiceOCC);

                    if (doInv != null)
                        doTbt_Invoice.Add(doInv);
                }
            }
            return doTbt_Invoice;
        }

        //Matching
        //ICS084: Matching
        /// <summary>
        /// Function for retrieving payment matching result of a payment transaction from tbt_MatchPaymentHeader and tbt_MatchPaymentDetail. (sp_IC_GetPaymentMatchingResult, sp_IC_GetPaymentMatchingResult_Detail)
        /// </summary>
        /// <param name="paymentTransNo">payment transaction no.</param>
        /// <returns></returns>
        public override List<doPaymentMatchingResult> GetPaymentMatchingResult(string paymentTransNo)
        {
            List<doPaymentMatchingResult> result = base.GetPaymentMatchingResult(paymentTransNo);
            if (result != null)
            {
                foreach (var item in result)
                {
                    //Get matching result detail for NOT CANCEL
                    if (item.CancelFlag.Value != true)
                        item.PaymentMatchingResultDetail = base.GetPaymentMatchingResult_Detail(item.MatchID);
                }
            }
            return result;
        }
        /// <summary>
        /// Function to retrieve description to show in viewing payment matching result. (sp_IC_GetPaymentMatchingDesc)
        /// </summary>
        /// <param name="valueCode">payment matching code</param>
        /// <returns></returns>
        public doPaymentMatchingDesc GetPaymentMatchingDesc(string valueCode)
        {
            List<doPaymentMatchingDesc> result = base.GetPaymentMatchingDesc(valueCode
                , MiscType.C_PAYMENT_MATCHING_DESC);

            if (result != null && result.Count > 0)
            {
                //Language Mapping
                CommonUtil.MappingObjectLanguage<doPaymentMatchingDesc>(result);
                return result[0];
            }
            return null;
        }
        /// <summary>
        /// Function for checking whether an invoice is matching to only refund payment. (sp_IC_CheckAllMatchingToRefund)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="invoiceOCC">invoice occ</param>
        /// <returns></returns>
        public bool CheckAllMatchingToRefund(string invoiceNo, int invoiceOCC)
        {
            ObjectParameter pAllRefund = new ObjectParameter("OUT_ALL_REFUND", false);
            base.CheckAllMatchingToRefund(invoiceNo, invoiceOCC, PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND, pAllRefund);
            return Convert.ToBoolean(pAllRefund.Value);
        }
        /// <summary>
        /// Function for insert payment matching header information to the system.
        /// </summary>
        /// <param name="xmlTbt_MatchPaymentHeader">xml of payment matching header information</param>
        /// <returns></returns>
        public override List<tbt_MatchPaymentHeader> InsertTbt_MatchPaymentHeader(string xmlTbt_MatchPaymentHeader)
        {
            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            List<tbt_MatchPaymentHeader> saved = base.InsertTbt_MatchPaymentHeader(xmlTbt_MatchPaymentHeader);
            if (saved != null && saved.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_MATCH_PAYMENT_HEADER,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(saved);
                logHand.WriteTransactionLog(logData);
                return saved;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Function for insert payment matching detail information to the system.
        /// </summary>
        /// <param name="xmlTbt_MatchPaymentHeader">xml of payment matching header information</param>
        /// <returns></returns>
        public override List<tbt_MatchPaymentDetail> InsertTbt_MatchPaymentDetail(string xmlTbt_MatchPaymentDetail)
        {
            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            List<tbt_MatchPaymentDetail> saved = base.InsertTbt_MatchPaymentDetail(xmlTbt_MatchPaymentDetail);
            if (saved != null && saved.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Insert,
                    TableName = TableName.C_TBL_NAME_MATCH_PAYMENT_DETAIL,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(saved);
                logHand.WriteTransactionLog(logData);
                return saved;
            }
            else
            {
                return null;
            }
        }

        // Create by Jirawat Jannet on 2016-12-08
        // ปรับแก้ เพิ่มการแปลงสกุลเงินเข้ามา
        #region MatchPaymentInvoices Subset function

        public bool MatchPaymentInvoices(doMatchPaymentHeader matchHeader, tbt_Payment payment)
        {
            bool isSuccess = false;

            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            string matchID = billingHandler.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_MATCH_PAYMENT);

            #region Match
            #region Insert Match Header
            //Prepare
            matchHeader.MatchID = matchID;
            matchHeader.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            matchHeader.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            matchHeader.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            matchHeader.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            //Insert
            List<tbt_MatchPaymentHeader> sourceMatchHeader = new List<tbt_MatchPaymentHeader>();
            sourceMatchHeader.Add(CommonUtil.CloneObject<doMatchPaymentHeader, tbt_MatchPaymentHeader>(matchHeader));
            List<tbt_MatchPaymentHeader> resultMatchHeader = this.InsertTbt_MatchPaymentHeader(CommonUtil.ConvertToXml_Store<tbt_MatchPaymentHeader>(sourceMatchHeader));
            if (resultMatchHeader == null || resultMatchHeader.Count == 0)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7078, null);
            #endregion

            #region Insert Match Detail
            //Prepare
            foreach (var item in matchHeader.MatchPaymentDetail)
            {
                item.MatchID = matchID;
                item.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                item.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                item.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                item.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            }
            //Insert
            List<tbt_MatchPaymentDetail> sourceMatchDetail = CommonUtil.ClonsObjectList<doMatchPaymentDetail, tbt_MatchPaymentDetail>(matchHeader.MatchPaymentDetail);
            List<tbt_MatchPaymentDetail> resultMatchDetail = this.InsertTbt_MatchPaymentDetail(CommonUtil.ConvertToXml_Store<tbt_MatchPaymentDetail>(sourceMatchDetail));
            if (resultMatchDetail == null || resultMatchDetail.Count == 0)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7079, null);

            #endregion
            #endregion

            foreach (var item in resultMatchDetail)
            {
                #region Invoice
                #region Update Invoice
                //Prepare
                List<tbt_Invoice> invoiceList = new List<tbt_Invoice>();
                doInvoice doInvoice = billingHandler.GetInvoice(item.InvoiceNo);


                string updatePaymentStatus = string.Empty;

                #region Update InvoicePaymentStatus
                updatePaymentStatus = doInvoice.InvoicePaymentStatus;
                decimal unpaidAmount = (doInvoice.InvoiceAmount ?? 0) + (doInvoice.VatAmount ?? 0) - (doInvoice.PaidAmountIncVat ?? 0) - (doInvoice.RegisteredWHTAmount ?? 0);
                decimal unpaidAmountUsd = (doInvoice.InvoiceAmountUsd ?? 0) + (doInvoice.VatAmountUsd ?? 0) - (doInvoice.PaidAmountIncVatUsd ?? 0) - (doInvoice.RegisteredWHTAmountUsd ?? 0);
                if (item.MatchAmountIncWHT < unpaidAmount || item.MatchAmountIncWHTUsd < unpaidAmountUsd)
                {
                    #region Partially match
                    if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED      //09
                        || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN)
                    {
                        //Modify by Jutarat A. on 26112013
                        //updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN;  //97
                        if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;  //03
                        }
                        else
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN;  //97
                        }
                        //End Modify
                    }
                    else
                    {
                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID; //95
                    }
                    #endregion
                }
                else
                {
                    #region Fully match
                    if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN)
                    {
                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN;   //96
                    }
                    else
                    {
                        if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER)
                        {
                            #region Bank transfer
                            if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT)
                            {
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_BANK_PAID;
                            }
                            else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK)
                            {
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_AUTO_FAIL_BANK_PAID;
                            }
                            else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK)
                            {
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL_BANK_PAID;
                            }
                            else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK)
                            {
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_FAIL_BANK_PAID;
                            }
                            else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID)
                            {
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_BANK_PAID;
                            }
                            #endregion
                        }
                        else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CASH)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_CASH_PAID;
                        }
                        else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_NORMAL)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_CHEQUE_PAID;
                        }
                        else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CASHIER_CHEQUE)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_CASHIER_PAID;
                        }
                        else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE)
                        {
                            if (payment.EncashedFlag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_ENCASHED)
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED;
                            else if (payment.EncashedFlag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_RETURNED)
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED_RETURN;
                            else
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED;
                        }
                        else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED)
                        {
                            if (payment.EncashedFlag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_ENCASHED)
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED;
                            else if (payment.EncashedFlag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_RETURNED)
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED_RETURN;
                            else
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED;
                        }
                        else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_AUTO_PAID;
                        }
                        else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND)
                        {
                            #region Credit note refund
                            if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID)
                            {
                                bool isAllRefund = this.CheckAllMatchingToRefund(doInvoice.InvoiceNo, doInvoice.InvoiceOCC);
                                if (isAllRefund)
                                {
                                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_REFUND_PAID;	    //99
                                }
                                else
                                {
                                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_REFUND;   //98
                                }
                            }
                            else
                            {
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_REFUND_PAID;	    //99
                            }
                            #endregion
                        }
                        else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED)        //09
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN;  //96
                        }
                    }
                    #endregion
                }
                #endregion

                // Add by Jirawat Jannet on 2016-11-21
                // เพราะว่าใน store มีการส่ง currency ไป get ค่า amount มาตาม currency ทำให้ตอน update มันเพี้ยนไป
                #region VatAmount, InvoiceAmount, WHTAmount

                if (doInvoice.VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    doInvoice.VatAmount = doInvoice.VatAmount;
                    doInvoice.VatAmountUsd = null;
                }
                else
                {
                    doInvoice.VatAmount = null;
                    doInvoice.VatAmountUsd = doInvoice.VatAmountUsd;
                }

                if (doInvoice.InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    doInvoice.InvoiceAmount = doInvoice.InvoiceAmount;
                    doInvoice.InvoiceAmountUsd = null;
                }
                else
                {
                    doInvoice.InvoiceAmount = null;
                    doInvoice.InvoiceAmountUsd = doInvoice.InvoiceAmountUsd;
                }

                if (doInvoice.WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    doInvoice.WHTAmount = doInvoice.WHTAmount;
                    doInvoice.WHTAmountUsd = null;
                }
                else
                {
                    doInvoice.WHTAmount = null;
                    doInvoice.WHTAmountUsd = doInvoice.WHTAmountUsd;
                }


                #endregion

                // edit by Jirawat jannet @ 2016-10-07
                doInvoice.PaidAmountIncVatCurrencyType = item.WHTAmountCurrencyType;
                doInvoice.RegisteredWHTAmountCurrencyType = item.WHTAmountCurrencyType;

                if (doInvoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    doInvoice.PaidAmountIncVat = (doInvoice.PaidAmountIncVat ?? 0) + item.MatchAmountExcWHT;
                    doInvoice.RegisteredWHTAmount = (doInvoice.RegisteredWHTAmount ?? 0) + (item.WHTAmount ?? 0);
                }
                else
                {
                    doInvoice.PaidAmountIncVatUsd = (doInvoice.PaidAmountIncVatUsd ?? 0) + item.MatchAmountExcWHTUsd;
                    doInvoice.RegisteredWHTAmountUsd = (doInvoice.RegisteredWHTAmountUsd ?? 0) + (item.WHTAmountUsd ?? 0);
                }


                doInvoice.InvoicePaymentStatus = updatePaymentStatus;
                doInvoice.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                doInvoice.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                invoiceList.Add(CommonUtil.CloneObject<doInvoice, tbt_Invoice>(doInvoice));

                //Update
                List<tbt_Invoice> resultInvoice = billingHandler.UpdateTbt_Invoice(CommonUtil.ConvertToXml_Store<tbt_Invoice>(invoiceList));
                if (resultInvoice == null || resultInvoice.Count == 0)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7080, null);

                doInvoice = billingHandler.GetInvoice(item.InvoiceNo);  //Refresh
                if (doInvoice == null)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7080, null);
                #endregion

                #region Update Billing Detail
                //Prepare
                if (doInvoice.Tbt_BillingDetails != null)
                {
                    foreach (var billingDetail in doInvoice.Tbt_BillingDetails)
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
                #endregion

                #region Receipt
                #region Update Receipt No to tax invoice table
                //Comment on 23/Aug/2012 No need this process
                //if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CASH ||
                //    payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_NORMAL ||
                //    payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CASHIER_CHEQUE ||
                //    payment.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE ||
                //    payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED)
                //{
                //    if (item.MatchAmountIncWHT == unpaidAmount)
                //    {
                //        isSuccess = billingHandler.UpdateReceiptNo(doInvoice.InvoiceNo, doInvoice.InvoiceOCC, payment.RefAdvanceReceiptNo);
                //        if (!isSuccess)
                //            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7081, null);
                //    }
                //}
                #endregion

                #region Update receipt's advance receipt status for fully paid
                doReceipt receipt = this.GetReceiptByInvoiceNo(item.InvoiceNo, item.InvoiceOCC);
                if (item.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL
                         || item.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL_TO_FULL)
                {
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
                if (doInvoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT
                    && doInvoice.Tbt_BillingDetails != null && doInvoice.Tbt_BillingDetails.Count > 0
                    && (item.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL
                         || item.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL_TO_FULL))
                {
                    foreach (var doBillingDetail in doInvoice.Tbt_BillingDetails)
                    {
                        ContractCode = doBillingDetail.ContractCode;
                        BillingOCC = doBillingDetail.BillingOCC;
                        //decimal billingAmountIncVat = doBillingDetail.BillingAmount.GetValueOrDefault() * (1 + doInvoice.VatRate.GetValueOrDefault());
                        // Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
                        decimal? balanceDepositAfterUpdate = 0;
                        decimal? balanceDepositUsdAfterUpdate = 0;
                        string balanceDepositAfterUpdateCurrency = CurrencyUtil.C_CURRENCY_LOCAL;

                        isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(ContractCode, BillingOCC
                            , doBillingDetail.BillingAmount, doBillingDetail.BillingAmountUsd, doBillingDetail.BillingAmountCurrencyType
                            , out balanceDepositAfterUpdate, out balanceDepositUsdAfterUpdate, out balanceDepositAfterUpdateCurrency);
                        if (!isSuccess)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);

                        isSuccess = billingHandler.InsertDepositFeePayment(ContractCode, BillingOCC
                            , doBillingDetail.BillingAmount, doBillingDetail.BillingAmountUsd, doBillingDetail.BillingAmountCurrencyType
                            , balanceDepositAfterUpdate, balanceDepositUsdAfterUpdate, doBillingDetail.BillingAmountCurrencyType
                            , doInvoice.InvoiceNo, receipt == null ? null : receipt.ReceiptNo);
                        if (!isSuccess)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);
                    }
                }
                #endregion

                #region Insert deposit fee, in case of refund payment
                if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND)
                {
                    SECOM_AJIS.DataEntity.Billing.doRefundInfo doRefund = billingHandler.GetRefundInfo(payment.PaymentTransNo);

                    if (doRefund != null)
                    {
                        if (doRefund.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
                        {
                            //&& doInvoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
                            ContractCode = doInvoice.Tbt_BillingDetails[0].ContractCode;
                            BillingOCC = doInvoice.Tbt_BillingDetails[0].BillingOCC;

                            decimal? balanceDepositAfterUpdate = 0;
                            decimal? balanceDepositUsdAfterUpdate = 0;
                            string balanceDepositAfterUpdateCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                            //isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(doRefund.ContractCode, doRefund.BillingOCC, (item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (-1), out balanceDepositAfterUpdate);

                            //Modify by Jutarat A. on 10022014
                            //// Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
                            //// ((item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (1 + doInvoice.VatRate) --> Calculate only deposit fee, exclude VAT
                            //isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(doRefund.ContractCode, doRefund.BillingOCC, ((item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (1 + doInvoice.VatRate ?? 0)) * (-1), out balanceDepositAfterUpdate);
                            ////////

                            decimal? adjustAmount = null;
                            decimal? adjustAmountUsd = null;

                            if (item.MatchAmountIncWHTCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                                adjustAmount = (item.MatchAmountIncWHT / (1 + doInvoice.VatRate ?? 0)) * (-1);
                            else
                                adjustAmountUsd = (item.MatchAmountIncWHTUsd / (1 + doInvoice.VatRate ?? 0)) * (-1);


                            isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(doRefund.ContractCode, doRefund.BillingOCC
                                , adjustAmount, adjustAmountUsd, item.MatchAmountIncWHTCurrencyType
                                , out balanceDepositAfterUpdate, out balanceDepositUsdAfterUpdate, out balanceDepositAfterUpdateCurrencyType);
                            //End Modify

                            if (!isSuccess)
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);

                            if (balanceDepositAfterUpdate < 0)
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7111, null);

                            //isSuccess = billingHandler.InsertDepositFeeSlide(doRefund, item.MatchAmountIncWHT - item.WHTAmount ?? 0, balanceDepositAfterUpdate, ContractCode, BillingOCC);

                            ////Modify by Jutarat A. on 10022014
                            //// Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
                            //// ((item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (100 + (doInvoice.VatRate ?? 0))/100) --> Calculate only deposit fee, exclude VAT
                            //isSuccess = billingHandler.InsertDepositFeeSlide(doRefund, ((item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (1 + doInvoice.VatRate ?? 0)), balanceDepositAfterUpdate, ContractCode, BillingOCC);
                            ////////
                            decimal? slidingAmount = null;
                            decimal? slidingAmountUsd = null;

                            if (item.MatchAmountIncWHTCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                                slidingAmount = (item.MatchAmountIncWHT / (1 + doInvoice.VatRate ?? 0));
                            else
                                slidingAmountUsd = (item.MatchAmountIncWHTUsd / (1 + doInvoice.VatRate ?? 0));

                            string processAmountCurrencyType = item.WHTAmountCurrencyType;
                            isSuccess = billingHandler.InsertDepositFeeSlide(doRefund, slidingAmount, balanceDepositAfterUpdate, ContractCode, BillingOCC
                                                            , processAmountCurrencyType, slidingAmountUsd, item.MatchAmountIncWHTCurrencyType, balanceDepositUsdAfterUpdate);
                            //End Modify

                            if (!isSuccess)
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);


                        }
                        /*Skip on 28/May/2012
                        else if (doRefund.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT
                            && doInvoice.BillingTypeCode != BillingType.C_BILLING_TYPE_DEPOSIT)
                        {
                            isSuccess = billingHandler.InsertDepositFeeReturn(doRefund, item.MatchAmountIncWHT - item.WHTAmount ?? 0, balanceDepositAfterUpdate);
                            if (!isSuccess)
                                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);
                        }*/
                    }
                    else
                    {
                        //Not found refund info
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);
                    }
                }
                #endregion
                #endregion
            }

            //Formular Balance After processing: 
            //balanceAfterProcessing = payment.MatchableBalance - (sumOfMatchPaymentAmount - sumOfTotalWht)  + BankFee + OtherExpense - OtherIncome
            //Remark Group to single value for calculate: 
            //(sumOfMatchPaymentAmount - sumOfTotalWht)  - BankFee - OtherExpense + OtherIncome
            decimal adjustPayment = (matchHeader.TotalMatchAmount ?? 0) - (matchHeader.BankFeeAmount ?? 0)
                - (matchHeader.OtherExpenseAmount ?? 0)
                + (matchHeader.OtherIncomeAmount ?? 0);

            decimal adjustPaymentUsd = (matchHeader.TotalMatchAmountUsd ?? 0) - (matchHeader.BankFeeAmountUsd ?? 0)
                - (matchHeader.OtherExpenseAmountUsd ?? 0)
                + (matchHeader.OtherIncomeAmountUsd ?? 0); // add by jirawat jannet @ 2016-1-06


            #region Payment
            #region Update remaining matchable balance
            tbt_Payment paymentDB = this.GetPayment(payment.PaymentTransNo);
            if (paymentDB.UpdateDate != payment.UpdateDate)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, null);

            isSuccess = this.UpdatePaymentMatchableBalance(payment.PaymentTransNo, adjustPayment * (-1), adjustPaymentUsd * (-1), matchHeader.TotalMatchAmountCurrencyType);        //minus value
            if (!isSuccess)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
            #endregion

            #region Update bankFee Flag
            if ((matchHeader.BankFeeAmount ?? 0) > 0)
            {
                isSuccess = this.UpdatePaymentBankFeeFlag(payment.PaymentTransNo, true);
                if (!isSuccess)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
            }
            #endregion

            #region Update Income Flag
            if ((matchHeader.OtherIncomeAmount ?? 0) > 0)
            {
                isSuccess = this.UpdatePaymentOtherIncomeFlag(payment.PaymentTransNo, true);
                if (!isSuccess)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
            }
            #endregion

            #region Update Expense Flag
            if ((matchHeader.OtherExpenseAmount ?? 0) > 0)
            {
                isSuccess = this.UpdatePaymentOtherExpenseFlag(payment.PaymentTransNo, true);
                if (!isSuccess)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
            }
            #endregion
            #endregion

            //Success
            return true;
        }

        // Edit by Jirawat Jannet @ 2016-12-08
        // Edit by Jirawat Jannet @ 2016-10-06
        /// <summary>
        /// Match payment and invoice(s). Can match payment to multiple invoices. Match payment according to amount user input.
        /// </summary>
        /// <param name="matchHeader">match payment header</param>
        /// <param name="payment">payment information</param>
        /// <returns></returns>
        public bool MatchPaymentInvoices(doMatchPaymentHeader matchHeader, tbt_Payment payment, string FirstPaymentAmountCurrencyType)
        {
            bool isSuccess = false;

            IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            string matchID = billingHandler.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_MATCH_PAYMENT);

            // Match payment
            // ---------------------------------------
            List<tbt_MatchPaymentDetail> resultMatchDetail = MatchPaymentInvoices_InsertMatchPayment(ref matchID, ref matchHeader);


            foreach (var item in resultMatchDetail)
            {
                // Invoice
                // ---------------------------------------
                string updatePaymentStatus = string.Empty;
                doInvoice doInvoice = billingHandler.GetInvoice(item.InvoiceNo);
                List<tbt_Invoice> resultInvoice = new List<tbt_Invoice>();

                // Update invoice
                List<tbt_Invoice> invoiceList = MatchPaymentInvoices_UpdateInvoice(ref payment, ref doInvoice, ref updatePaymentStatus, item, billingHandler, out resultInvoice, FirstPaymentAmountCurrencyType);

                // Update Billing Detail
                MatchPaymentInvoices_UpdateBllingDetail(billingHandler, ref doInvoice, updatePaymentStatus, resultInvoice);


                // Receipt
                // ---------------------------------------
                doReceipt receipt = new doReceipt();

                MatchPaymentInvoices_UpdateReceipt(item, ref isSuccess, ref receipt);

                // Deposit
                // ---------------------------------------
                string ContractCode = string.Empty;
                string BillingOCC = string.Empty;

                // Insert deposit fee, in case of deposit fee invoice
                MatchPaymentInvoices_InsertDepositFee_InCaseOfDepositFeeInvoice(billingHandler, doInvoice, item, ref receipt, ref isSuccess, ref ContractCode, ref BillingOCC);

                // Insert deposit fee, in case of refund payment
                MatchPaymentInvoices_InsertDepositFee_InCaseOfRefundPayment(billingHandler, payment, doInvoice, item, ref isSuccess, ref ContractCode, ref BillingOCC);
            }


            // Payment
            // ---------------------------------------
            MatchPaymentInvoices_UpdatePayment(payment, matchHeader, isSuccess, FirstPaymentAmountCurrencyType);


            //Success
            return true;
        }

        #region Match payment

        private List<tbt_MatchPaymentDetail> MatchPaymentInvoices_InsertMatchPayment(ref string matchID, ref doMatchPaymentHeader matchHeader)
        {
            #region Insert Match Header
            //Prepare
            matchHeader = MatchPaymentInvoices_InsertMatchPayment_PrepareHeaderData(matchID, matchHeader);

            //Insert
            List<tbt_MatchPaymentHeader> sourceMatchHeader = new List<tbt_MatchPaymentHeader>();
            sourceMatchHeader.Add(CommonUtil.CloneObject<doMatchPaymentHeader, tbt_MatchPaymentHeader>(matchHeader));
            List<tbt_MatchPaymentHeader> resultMatchHeader = this.InsertTbt_MatchPaymentHeader(CommonUtil.ConvertToXml_Store<tbt_MatchPaymentHeader>(sourceMatchHeader));

            if (resultMatchHeader == null || resultMatchHeader.Count == 0)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7078, null);
            #endregion

            #region Insert Match Detail
            //Prepare
            foreach (var item in matchHeader.MatchPaymentDetail)
            {
                item.MatchID = matchID;
                item.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                item.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                item.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                item.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            }
            //Insert
            List<tbt_MatchPaymentDetail> sourceMatchDetail = CommonUtil.ClonsObjectList<doMatchPaymentDetail, tbt_MatchPaymentDetail>(matchHeader.MatchPaymentDetail);
            List<tbt_MatchPaymentDetail> resultMatchDetail = this.InsertTbt_MatchPaymentDetail(CommonUtil.ConvertToXml_Store<tbt_MatchPaymentDetail>(sourceMatchDetail));

            if (resultMatchDetail == null || resultMatchDetail.Count == 0)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7079, null);

            #endregion

            return resultMatchDetail;
        }
        private doMatchPaymentHeader MatchPaymentInvoices_InsertMatchPayment_PrepareHeaderData(string matchID, doMatchPaymentHeader matchHeader)
        {
            matchHeader.MatchID = matchID;
            matchHeader.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            matchHeader.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            matchHeader.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            matchHeader.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            return matchHeader;
        }

        #endregion

        #region Update invoice

        private List<tbt_Invoice> MatchPaymentInvoices_UpdateInvoice(ref tbt_Payment payment, ref doInvoice doInvoice, ref string updatePaymentStatus
            , tbt_MatchPaymentDetail item, IBillingHandler billingHandler, out List<tbt_Invoice> resultInvoice, string FirstPaymentAmountCurrencyType)
        {
            #region Update Invoice
            //Prepare
            List<tbt_Invoice> invoiceList = new List<tbt_Invoice>();


            // Update invoice payment status
            MatchPaymentInvoices_UpdateInvoicePaymentStatus(ref payment, ref updatePaymentStatus, ref item, ref doInvoice);

            // Add by Jirawat Jannet on 2016-11-21
            // เพราะว่าใน store มีการส่ง currency ไป get ค่า amount มาตาม currency ทำให้ตอน update มันเพี้ยนไป
            #region VatAmount, InvoiceAmount, WHTAmount

            if (doInvoice.VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
            {
                doInvoice.VatAmount = doInvoice.VatAmount;
                doInvoice.VatAmountUsd = null;
            }
            else
            {
                doInvoice.VatAmount = null;
                doInvoice.VatAmountUsd = doInvoice.VatAmountUsd;
            }

            if (doInvoice.InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
            {
                doInvoice.InvoiceAmount = doInvoice.InvoiceAmount;
                doInvoice.InvoiceAmountUsd = null;
            }
            else
            {
                doInvoice.InvoiceAmount = null;
                doInvoice.InvoiceAmountUsd = doInvoice.InvoiceAmountUsd;
            }

            if (doInvoice.WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
            {
                doInvoice.WHTAmount = doInvoice.WHTAmount;
                doInvoice.WHTAmountUsd = null;
            }
            else
            {
                doInvoice.WHTAmount = null;
                doInvoice.WHTAmountUsd = doInvoice.WHTAmountUsd;
            }


            #endregion

            // Calculate Paid amount and wth amount
            MatchPaymentInvoices_UpdateInvoice_CalculatPaidAmount(ref doInvoice, item, FirstPaymentAmountCurrencyType);

            doInvoice.InvoicePaymentStatus = updatePaymentStatus;
            doInvoice.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            doInvoice.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            invoiceList.Add(CommonUtil.CloneObject<doInvoice, tbt_Invoice>(doInvoice));

            //Update
            resultInvoice = billingHandler.UpdateTbt_Invoice(CommonUtil.ConvertToXml_Store<tbt_Invoice>(invoiceList));
            if (resultInvoice == null || resultInvoice.Count == 0)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7080, null);

            doInvoice = billingHandler.GetInvoice(item.InvoiceNo);  //Refresh
            if (doInvoice == null)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7080, null);
            #endregion

            return invoiceList;
        }

        private void MatchPaymentInvoices_UpdateInvoicePaymentStatus(ref tbt_Payment payment, ref string updatePaymentStatus, ref tbt_MatchPaymentDetail item, ref doInvoice doInvoice)
        {
            updatePaymentStatus = doInvoice.InvoicePaymentStatus;
            decimal unpaidAmount = (doInvoice.InvoiceAmount ?? 0) + (doInvoice.VatAmount ?? 0) - (doInvoice.PaidAmountIncVat ?? 0) - (doInvoice.RegisteredWHTAmount ?? 0);
            decimal unpaidAmountUsd = (doInvoice.InvoiceAmountUsd ?? 0) + (doInvoice.VatAmountUsd ?? 0) - (doInvoice.PaidAmountIncVatUsd ?? 0) - (doInvoice.RegisteredWHTAmountUsd ?? 0);
            if (item.MatchAmountIncWHT < unpaidAmount || item.MatchAmountIncWHTUsd < unpaidAmountUsd)
            {
                #region Partially match
                if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED      //09
                    || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN)
                {
                    //Modify by Jutarat A. on 26112013
                    //updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN;  //97
                    if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT)
                    {
                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;  //03
                    }
                    else
                    {
                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN;  //97
                    }
                    //End Modify
                }
                else
                {
                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID; //95
                }
                #endregion
            }
            else
            {
                #region Fully match
                if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN)
                {
                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN;   //96
                }
                else
                {
                    if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER)
                    {
                        #region Bank transfer
                        if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_BANK_PAID;
                        }
                        else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_AUTO_FAIL_BANK_PAID;
                        }
                        else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL_BANK_PAID;
                        }
                        else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_FAIL_BANK_PAID;
                        }
                        else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID)
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_BANK_PAID;
                        }
                        #endregion
                    }
                    else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CASH)
                    {
                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_CASH_PAID;
                    }
                    else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_NORMAL)
                    {
                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_CHEQUE_PAID;
                    }
                    else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CASHIER_CHEQUE)
                    {
                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_CASHIER_PAID;
                    }
                    else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE)
                    {
                        if (payment.EncashedFlag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_ENCASHED)
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED;
                        else if (payment.EncashedFlag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_RETURNED)
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED_RETURN;
                        else
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED;
                    }
                    else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED)
                    {
                        if (payment.EncashedFlag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_ENCASHED)
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED;
                        else if (payment.EncashedFlag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_RETURNED)
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED_RETURN;
                        else
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED;
                    }
                    else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
                    {
                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_AUTO_PAID;
                    }
                    else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND)
                    {
                        #region Credit note refund
                        if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID)
                        {
                            bool isAllRefund = this.CheckAllMatchingToRefund(doInvoice.InvoiceNo, doInvoice.InvoiceOCC);
                            if (isAllRefund)
                            {
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_REFUND_PAID;       //99
                            }
                            else
                            {
                                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_REFUND;   //98
                            }
                        }
                        else
                        {
                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_REFUND_PAID;       //99
                        }
                        #endregion
                    }
                    else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED)        //09
                    {
                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN;  //96
                    }
                }
                #endregion
            }
        }

        private void MatchPaymentInvoices_UpdateInvoice_CalculatPaidAmount(ref doInvoice doInvoice, tbt_MatchPaymentDetail item, string FirstPaymentAmountCurrencyType)
        {
            decimal PaidAmountIncVat = 0;
            decimal RegisteredWHTAmount = 0;

            decimal MatchAmountExcWHT = 0;
            decimal WHTAmount = 0;

            if (doInvoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_US)
                PaidAmountIncVat = doInvoice.PaidAmountIncVatUsd.ConvertCurrencyPrice(doInvoice.PaidAmountIncVatCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;
            else
                PaidAmountIncVat = doInvoice.PaidAmountIncVat.ConvertCurrencyPrice(doInvoice.PaidAmountIncVatCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;

            if (doInvoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                RegisteredWHTAmount = doInvoice.RegisteredWHTAmountUsd.ConvertCurrencyPrice(doInvoice.RegisteredWHTAmountCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;
            else
                RegisteredWHTAmount = doInvoice.RegisteredWHTAmount.ConvertCurrencyPrice(doInvoice.RegisteredWHTAmountCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;

            if (item.MatchAmountExcWHTCurrencyType == CurrencyUtil.C_CURRENCY_US)
                MatchAmountExcWHT = item.MatchAmountExcWHTUsd.ConvertCurrencyPrice(item.MatchAmountExcWHTCurrencyType, FirstPaymentAmountCurrencyType);
            else
                MatchAmountExcWHT = item.MatchAmountExcWHT.ConvertCurrencyPrice(item.MatchAmountExcWHTCurrencyType, FirstPaymentAmountCurrencyType);

            if (item.WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                WHTAmount = item.WHTAmountUsd.ConvertCurrencyPrice(item.WHTAmountCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;
            else
                WHTAmount = item.WHTAmount.ConvertCurrencyPrice(item.WHTAmountCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;

            PaidAmountIncVat += MatchAmountExcWHT;
            RegisteredWHTAmount += WHTAmount;

            doInvoice.PaidAmountIncVatCurrencyType = item.MatchAmountExcWHTCurrencyType;
            doInvoice.RegisteredWHTAmountCurrencyType = item.WHTAmountCurrencyType;

            if (doInvoice.PaidAmountIncVatCurrencyType == CurrencyUtil.C_CURRENCY_US)
            {
                doInvoice.PaidAmountIncVatUsd = PaidAmountIncVat.ConvertCurrencyPrice(FirstPaymentAmountCurrencyType, doInvoice.PaidAmountIncVatCurrencyType);
                doInvoice.PaidAmountIncVat = null;
            }
            else
            {
                doInvoice.PaidAmountIncVatUsd = null;
                doInvoice.PaidAmountIncVat = PaidAmountIncVat.ConvertCurrencyPrice(FirstPaymentAmountCurrencyType, doInvoice.PaidAmountIncVatCurrencyType);
            }

            if (doInvoice.RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
            {
                doInvoice.RegisteredWHTAmount = null;
                doInvoice.RegisteredWHTAmountUsd = RegisteredWHTAmount.ConvertCurrencyPrice(FirstPaymentAmountCurrencyType, doInvoice.RegisteredWHTAmountCurrencyType);
            }
            else
            {
                doInvoice.RegisteredWHTAmount = RegisteredWHTAmount.ConvertCurrencyPrice(FirstPaymentAmountCurrencyType, doInvoice.RegisteredWHTAmountCurrencyType);
                doInvoice.RegisteredWHTAmountUsd = null;
            }
        }

        #endregion

        #region Updat Billing Detail

        private void MatchPaymentInvoices_UpdateBllingDetail(IBillingHandler billingHandler, ref doInvoice doInvoice, string updatePaymentStatus, List<tbt_Invoice> resultInvoice)
        {
            //Prepare
            if (doInvoice.Tbt_BillingDetails != null)
            {
                foreach (var billingDetail in doInvoice.Tbt_BillingDetails)
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
        }

        #endregion

        #region Update receipt's advance receipt status for fully paid

        private void MatchPaymentInvoices_UpdateReceipt(tbt_MatchPaymentDetail item, ref bool isSuccess, ref doReceipt receipt)
        {
            #region Update receipt's advance receipt status for fully paid
            receipt = this.GetReceiptByInvoiceNo(item.InvoiceNo, item.InvoiceOCC);
            if (item.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL
                     || item.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL_TO_FULL)
            {
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
        }

        #endregion

        #region Deposit fee

        private void MatchPaymentInvoices_InsertDepositFee_InCaseOfDepositFeeInvoice(IBillingHandler billingHandler, doInvoice doInvoice, tbt_MatchPaymentDetail item
            , ref doReceipt receipt, ref bool isSuccess, ref string ContractCode, ref string BillingOCC)
        {
            if (doInvoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT
                && doInvoice.Tbt_BillingDetails != null && doInvoice.Tbt_BillingDetails.Count > 0
                && (item.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL
                     || item.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL_TO_FULL))
            {
                foreach (var doBillingDetail in doInvoice.Tbt_BillingDetails)
                {
                    ContractCode = doBillingDetail.ContractCode;
                    BillingOCC = doBillingDetail.BillingOCC;
                    //decimal billingAmountIncVat = doBillingDetail.BillingAmount.GetValueOrDefault() * (1 + doInvoice.VatRate.GetValueOrDefault());
                    // Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
                    decimal? balanceDepositAfterUpdate = 0;
                    decimal? balanceDepositUsdAfterUpdate = 0;
                    string balanceDepositAfterUpdateCurrency = CurrencyUtil.C_CURRENCY_LOCAL;

                    isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(ContractCode, BillingOCC
                        , doBillingDetail.BillingAmount, doBillingDetail.BillingAmountUsd, doBillingDetail.BillingAmountCurrencyType
                        , out balanceDepositAfterUpdate, out balanceDepositUsdAfterUpdate, out balanceDepositAfterUpdateCurrency);
                    if (!isSuccess)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);

                    isSuccess = billingHandler.InsertDepositFeePayment(ContractCode, BillingOCC
                        , doBillingDetail.BillingAmount, doBillingDetail.BillingAmountUsd, doBillingDetail.BillingAmountCurrencyType
                        , balanceDepositAfterUpdate, balanceDepositUsdAfterUpdate, doBillingDetail.BillingAmountCurrencyType
                        , doInvoice.InvoiceNo, receipt == null ? null : receipt.ReceiptNo);
                    if (!isSuccess)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);
                }
            }
        }


        private void MatchPaymentInvoices_InsertDepositFee_InCaseOfRefundPayment(IBillingHandler billingHandler, tbt_Payment payment, doInvoice doInvoice, tbt_MatchPaymentDetail item
            , ref bool isSuccess, ref string ContractCode, ref string BillingOCC)
        {
            if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND)
            {
                SECOM_AJIS.DataEntity.Billing.doRefundInfo doRefund = billingHandler.GetRefundInfo(payment.PaymentTransNo);

                if (doRefund != null)
                {
                    if (doRefund.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
                    {
                        //&& doInvoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
                        ContractCode = doInvoice.Tbt_BillingDetails[0].ContractCode;
                        BillingOCC = doInvoice.Tbt_BillingDetails[0].BillingOCC;

                        decimal? balanceDepositAfterUpdate = 0;
                        decimal? balanceDepositUsdAfterUpdate = 0;
                        string balanceDepositAfterUpdateCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                        //isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(doRefund.ContractCode, doRefund.BillingOCC, (item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (-1), out balanceDepositAfterUpdate);

                        //Modify by Jutarat A. on 10022014
                        //// Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
                        //// ((item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (1 + doInvoice.VatRate) --> Calculate only deposit fee, exclude VAT
                        //isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(doRefund.ContractCode, doRefund.BillingOCC, ((item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (1 + doInvoice.VatRate ?? 0)) * (-1), out balanceDepositAfterUpdate);
                        ////////

                        decimal? adjustAmount = null;
                        decimal? adjustAmountUsd = null;

                        if (item.MatchAmountIncWHTCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            adjustAmount = (item.MatchAmountIncWHT / (1 + doInvoice.VatRate ?? 0)) * (-1);
                        else
                            adjustAmountUsd = (item.MatchAmountIncWHTUsd / (1 + doInvoice.VatRate ?? 0)) * (-1);


                        isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(doRefund.ContractCode, doRefund.BillingOCC
                            , adjustAmount, adjustAmountUsd, item.MatchAmountIncWHTCurrencyType
                            , out balanceDepositAfterUpdate, out balanceDepositUsdAfterUpdate, out balanceDepositAfterUpdateCurrencyType);
                        //End Modify

                        if (!isSuccess)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);

                        if (balanceDepositAfterUpdate < 0)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7111, null);

                        //isSuccess = billingHandler.InsertDepositFeeSlide(doRefund, item.MatchAmountIncWHT - item.WHTAmount ?? 0, balanceDepositAfterUpdate, ContractCode, BillingOCC);

                        ////Modify by Jutarat A. on 10022014
                        //// Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
                        //// ((item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (100 + (doInvoice.VatRate ?? 0))/100) --> Calculate only deposit fee, exclude VAT
                        //isSuccess = billingHandler.InsertDepositFeeSlide(doRefund, ((item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (1 + doInvoice.VatRate ?? 0)), balanceDepositAfterUpdate, ContractCode, BillingOCC);
                        ////////
                        decimal? slidingAmount = null;
                        decimal? slidingAmountUsd = null;

                        if (item.MatchAmountIncWHTCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            slidingAmount = (item.MatchAmountIncWHT / (1 + doInvoice.VatRate ?? 0));
                        else
                            slidingAmountUsd = (item.MatchAmountIncWHTUsd / (1 + doInvoice.VatRate ?? 0));

                        string processAmountCurrencyType = item.WHTAmountCurrencyType;
                        isSuccess = billingHandler.InsertDepositFeeSlide(doRefund, slidingAmount, balanceDepositAfterUpdate, ContractCode, BillingOCC
                                                        , processAmountCurrencyType, slidingAmountUsd, item.MatchAmountIncWHTCurrencyType, balanceDepositUsdAfterUpdate);
                        //End Modify

                        if (!isSuccess)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);


                    }
                    /*Skip on 28/May/2012
                    else if (doRefund.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT
                        && doInvoice.BillingTypeCode != BillingType.C_BILLING_TYPE_DEPOSIT)
                    {
                        isSuccess = billingHandler.InsertDepositFeeReturn(doRefund, item.MatchAmountIncWHT - item.WHTAmount ?? 0, balanceDepositAfterUpdate);
                        if (!isSuccess)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);
                    }*/
                }
                else
                {
                    //Not found refund info
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);
                }
            }
        }

        #endregion

        #region Payment

        private void MatchPaymentInvoices_UpdatePayment(tbt_Payment payment, doMatchPaymentHeader matchHeader, bool isSuccess, string FirstPaymentAmountCurrencyType)
        {

            //Formular Balance After processing: 
            //balanceAfterProcessing = payment.MatchableBalance - (sumOfMatchPaymentAmount - sumOfTotalWht)  + BankFee + OtherExpense - OtherIncome
            //Remark Group to single value for calculate: 
            //(sumOfMatchPaymentAmount - sumOfTotalWht)  - BankFee - OtherExpense + OtherIncome

            decimal TotalMatchAmount = 0;
            decimal BankFeeAmount = 0;
            decimal OtherExpenseAmount = 0;
            decimal OtherIncomeAmount = 0;

            if (matchHeader.TotalMatchAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                TotalMatchAmount = matchHeader.TotalMatchAmountUsd.ConvertCurrencyPrice(matchHeader.TotalMatchAmountCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;
            else TotalMatchAmount = matchHeader.TotalMatchAmount.ConvertCurrencyPrice(matchHeader.TotalMatchAmountCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;

            if (matchHeader.BankFeeAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                BankFeeAmount = matchHeader.BankFeeAmountUsd.ConvertCurrencyPrice(matchHeader.BankFeeAmountCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;
            else BankFeeAmount = matchHeader.BankFeeAmount.ConvertCurrencyPrice(matchHeader.BankFeeAmountCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;

            if (matchHeader.OtherExpenseAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                OtherExpenseAmount = matchHeader.OtherExpenseAmountUsd.ConvertCurrencyPrice(matchHeader.OtherExpenseAmountCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;
            else OtherExpenseAmount = matchHeader.OtherExpenseAmount.ConvertCurrencyPrice(matchHeader.OtherExpenseAmountCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;

            if (matchHeader.OtherIncomeAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                OtherIncomeAmount = matchHeader.OtherIncomeAmountUsd.ConvertCurrencyPrice(matchHeader.OtherIncomeAmountCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;
            else OtherIncomeAmount = matchHeader.OtherIncomeAmount.ConvertCurrencyPrice(matchHeader.OtherIncomeAmountCurrencyType, FirstPaymentAmountCurrencyType, 0).Value;

            decimal adjustPayment = TotalMatchAmount - BankFeeAmount - OtherExpenseAmount + OtherIncomeAmount;
            decimal adjustPaymentUsd = 0;

            // เมื่อมีการสลับ currency type จะทำการ adjust matc balance ใน program แล้วนำไปบวกเพิ่มใน database โดยตรง 
            // เนื่องจาก field match balance เดิมจะมีค่าเป็น 0 เสมอ เนื่องจากเป็นการสลับ currency 
            if (payment.MatchableBalanceCurrencyType != matchHeader.TotalMatchAmountCurrencyType)
            {
                decimal MatchableBalance = 0;
                if (payment.MatchableBalanceCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    MatchableBalance = payment.MatchableBalanceUsd.ConvertCurrencyPrice(payment.MatchableBalanceCurrencyType, FirstPaymentAmountCurrencyType);
                else
                    MatchableBalance = payment.MatchableBalance.ConvertCurrencyPrice(payment.MatchableBalanceCurrencyType, FirstPaymentAmountCurrencyType);

                adjustPayment = (MatchableBalance - adjustPayment) * (-1); // adjust match balance แล้วทำการ * (-1) เพื่อทำให้ส่งค่าเข้าไปใน store procedure แล้วค่าเป็น +
            }

            // Set ค่าให้กับ field mactch balance ของ currency ที่เลือกปัจจุบัน 
            // และ set อีก field ของ currency ที่เหลือ เพื่อให้ส่งค่าเข้าไปยัง store procedure เป็น - เพื่อให้ field match balance ของ currency ที่ไม่ได้เลือก เป็น 0 เสมอ
            if (matchHeader.TotalMatchAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
            {
                adjustPaymentUsd = adjustPayment.ConvertCurrencyPrice(FirstPaymentAmountCurrencyType, matchHeader.TotalMatchAmountCurrencyType);
                adjustPayment = payment.MatchableBalance;
            }
            else
            {
                adjustPaymentUsd = payment.MatchableBalanceUsd;
                adjustPayment = adjustPayment.ConvertCurrencyPrice(FirstPaymentAmountCurrencyType, matchHeader.TotalMatchAmountCurrencyType);
            }

            #region Update remaining matchable balance
            tbt_Payment paymentDB = this.GetPayment(payment.PaymentTransNo);
            if (paymentDB.UpdateDate != payment.UpdateDate)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, null);

            isSuccess = this.UpdatePaymentMatchableBalance(payment.PaymentTransNo, adjustPayment * (-1), adjustPaymentUsd * (-1), matchHeader.TotalMatchAmountCurrencyType);        //minus value
            if (!isSuccess)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
            #endregion

            #region Update bankFee Flag
            if ((matchHeader.BankFeeAmount ?? 0) > 0)
            {
                isSuccess = this.UpdatePaymentBankFeeFlag(payment.PaymentTransNo, true);
                if (!isSuccess)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
            }
            #endregion

            #region Update Income Flag
            if ((matchHeader.OtherIncomeAmount ?? 0) > 0)
            {
                isSuccess = this.UpdatePaymentOtherIncomeFlag(payment.PaymentTransNo, true);
                if (!isSuccess)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
            }
            #endregion

            #region Update Expense Flag
            if ((matchHeader.OtherExpenseAmount ?? 0) > 0)
            {
                isSuccess = this.UpdatePaymentOtherExpenseFlag(payment.PaymentTransNo, true);
                if (!isSuccess)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
            }
            #endregion
        }

        #endregion

        #endregion

        #region Comment old code function MatchPaymentInvoice

       
        #region Comment old function MatchPaymentInvoices @ 2016-12-08 : เพิ่มการแปลงสกิลเงินไปมา

        

        #endregion

        // Comment by Jirawat Jannet @2016-10-06
        #region Comment old code function MatchPaymentInvoices

        /// <summary>
        /// Match payment and invoice(s). Can match payment to multiple invoices. Match payment according to amount user input.
        /// </summary>
        /// <param name="matchHeader">match payment header</param>
        /// <param name="payment">payment information</param>
        /// <returns></returns>
        //public bool MatchPaymentInvoices(doMatchPaymentHeader matchHeader, tbt_Payment payment)
        //{
        //    bool isSuccess = false;

        //    IBillingHandler billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
        //    string matchID = billingHandler.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_MATCH_PAYMENT);

        //    #region Match
        //    #region Insert Match Header
        //    //Prepare
        //    matchHeader.MatchID = matchID;
        //    matchHeader.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //    matchHeader.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //    matchHeader.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //    matchHeader.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //    //Insert
        //    List<tbt_MatchPaymentHeader> sourceMatchHeader = new List<tbt_MatchPaymentHeader>();
        //    sourceMatchHeader.Add(CommonUtil.CloneObject<doMatchPaymentHeader, tbt_MatchPaymentHeader>(matchHeader));
        //    List<tbt_MatchPaymentHeader> resultMatchHeader = this.InsertTbt_MatchPaymentHeader(CommonUtil.ConvertToXml_Store<tbt_MatchPaymentHeader>(sourceMatchHeader));
        //    if (resultMatchHeader == null || resultMatchHeader.Count == 0)
        //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7078, null);
        //    #endregion

        //    #region Insert Match Detail
        //    //Prepare
        //    foreach (var item in matchHeader.MatchPaymentDetail)
        //    {
        //        item.MatchID = matchID;
        //        item.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //        item.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //        item.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //        item.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //    }
        //    //Insert
        //    List<tbt_MatchPaymentDetail> sourceMatchDetail = CommonUtil.ClonsObjectList<doMatchPaymentDetail, tbt_MatchPaymentDetail>(matchHeader.MatchPaymentDetail);
        //    List<tbt_MatchPaymentDetail> resultMatchDetail = this.InsertTbt_MatchPaymentDetail(CommonUtil.ConvertToXml_Store<tbt_MatchPaymentDetail>(sourceMatchDetail));
        //    if (resultMatchDetail == null || resultMatchDetail.Count == 0)
        //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7079, null);

        //    #endregion
        //    #endregion

        //    foreach (var item in resultMatchDetail)
        //    {
        //        #region Invoice
        //        #region Update Invoice
        //        //Prepare
        //        List<tbt_Invoice> invoiceList = new List<tbt_Invoice>();
        //        doInvoice doInvoice = billingHandler.GetInvoice(item.InvoiceNo);


        //        string updatePaymentStatus = string.Empty;

        //        #region Update InvoicePaymentStatus
        //        updatePaymentStatus = doInvoice.InvoicePaymentStatus;
        //        decimal unpaidAmount = (doInvoice.InvoiceAmount ?? 0) + (doInvoice.VatAmount ?? 0) - (doInvoice.PaidAmountIncVat ?? 0) - (doInvoice.RegisteredWHTAmount ?? 0);
        //        decimal unpaidAmountUsd = (doInvoice.InvoiceAmountUsd ?? 0) + (doInvoice.VatAmountUsd ?? 0) - (doInvoice.PaidAmountIncVatUsd ?? 0) - (doInvoice.RegisteredWHTAmountUsd ?? 0);
        //        if (item.MatchAmountIncWHT < unpaidAmount || item.MatchAmountIncWHTUsd < unpaidAmountUsd)
        //        {
        //            #region Partially match
        //            if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED      //09
        //                || doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN)
        //            {
        //                //Modify by Jutarat A. on 26112013
        //                //updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN;  //97
        //                if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT)
        //                {
        //                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT;  //03
        //                }
        //                else
        //                {
        //                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN;  //97
        //                }
        //                //End Modify
        //            }
        //            else
        //            {
        //                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID; //95
        //            }
        //            #endregion
        //        }
        //        else
        //        {
        //            #region Fully match
        //            if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID_CN)
        //            {
        //                updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN;   //96
        //            }
        //            else
        //            {
        //                if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_BANK_TRANSFER)
        //                {
        //                    #region Bank transfer
        //                    if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT)
        //                    {
        //                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_BANK_PAID;
        //                    }
        //                    else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK)
        //                    {
        //                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_AUTO_FAIL_BANK_PAID;
        //                    }
        //                    else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK)
        //                    {
        //                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL_BANK_PAID;
        //                    }
        //                    else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK)
        //                    {
        //                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_FAIL_BANK_PAID;
        //                    }
        //                    else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID)
        //                    {
        //                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_BANK_PAID;
        //                    }
        //                    #endregion
        //                }
        //                else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CASH)
        //                {
        //                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_CASH_PAID;
        //                }
        //                else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_NORMAL)
        //                {
        //                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_CHEQUE_PAID;
        //                }
        //                else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CASHIER_CHEQUE)
        //                {
        //                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_CASHIER_PAID;
        //                }
        //                else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE)
        //                {
        //                    if (payment.EncashedFlag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_ENCASHED)
        //                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED;
        //                    else if (payment.EncashedFlag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_RETURNED)
        //                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED_RETURN;
        //                    else
        //                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED;
        //                }
        //                else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED)
        //                {
        //                    if (payment.EncashedFlag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_ENCASHED)
        //                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED;
        //                    else if (payment.EncashedFlag == EncashedFlagByte.C_PAYMENT_ENCASHED_FLAG_RETURNED)
        //                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED_RETURN;
        //                    else
        //                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED;
        //                }
        //                else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_AUTO_TRANSFER)
        //                {
        //                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_AUTO_PAID;
        //                }
        //                else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND)
        //                {
        //                    #region Credit note refund
        //                    if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_PARTIALLY_PAID)
        //                    {
        //                        bool isAllRefund = this.CheckAllMatchingToRefund(doInvoice.InvoiceNo, doInvoice.InvoiceOCC);
        //                        if (isAllRefund)
        //                        {
        //                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_REFUND_PAID;	    //99
        //                        }
        //                        else
        //                        {
        //                            updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_REFUND;   //98
        //                        }
        //                    }
        //                    else
        //                    {
        //                        updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_REFUND_PAID;	    //99
        //                    }
        //                    #endregion
        //                }
        //                else if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_DECREASED)        //09
        //                {
        //                    updatePaymentStatus = PaymentStatus.C_PAYMENT_STATUS_PAID_WITH_PARTIAL_CN;  //96
        //                }
        //            }
        //            #endregion
        //        }
        //        #endregion

        //        doInvoice.PaidAmountIncVatCurrencyType = doInvoice.VatAmountCurrencyType;
        //        doInvoice.PaidAmountIncVat = (doInvoice.PaidAmountIncVat ?? 0) + item.MatchAmountExcWHT;
        //        doInvoice.PaidAmountIncVatUsd = (doInvoice.PaidAmountIncVatUsd ?? 0) + item.MatchAmountExcWHTUsd;

        //        doInvoice.RegisteredWHTAmountCurrencyType = doInvoice.WHTAmountCurrencyType;
        //        doInvoice.RegisteredWHTAmount = (doInvoice.RegisteredWHTAmount ?? 0) + (item.WHTAmount ?? 0);
        //        doInvoice.RegisteredWHTAmountUsd = (doInvoice.RegisteredWHTAmountUsd ?? 0) + (item.WHTAmountUsd ?? 0);

        //        doInvoice.InvoicePaymentStatus = updatePaymentStatus;
        //        doInvoice.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //        doInvoice.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
        //        invoiceList.Add(CommonUtil.CloneObject<doInvoice, tbt_Invoice>(doInvoice));

        //        //Update
        //        List<tbt_Invoice> resultInvoice = billingHandler.UpdateTbt_Invoice(CommonUtil.ConvertToXml_Store<tbt_Invoice>(invoiceList));
        //        if (resultInvoice == null || resultInvoice.Count == 0)
        //            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7080, null);

        //        doInvoice = billingHandler.GetInvoice(item.InvoiceNo);  //Refresh
        //        if (doInvoice == null)
        //            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7080, null);
        //        #endregion

        //        #region Update Billing Detail
        //        //Prepare
        //        if (doInvoice.Tbt_BillingDetails != null)
        //        {
        //            foreach (var billingDetail in doInvoice.Tbt_BillingDetails)
        //            {
        //                billingDetail.PaymentStatus = updatePaymentStatus;
        //                billingDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
        //                billingDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

        //                //Update
        //                int result = billingHandler.Updatetbt_BillingDetail(billingDetail);
        //                if (resultInvoice.Count == 0)
        //                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7080, null);
        //            }
        //        }
        //        #endregion
        //        #endregion

        //        #region Receipt
        //        #region Update Receipt No to tax invoice table
        //        //Comment on 23/Aug/2012 No need this process
        //        //if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CASH ||
        //        //    payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_NORMAL ||
        //        //    payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CASHIER_CHEQUE ||
        //        //    payment.PaymentType == PaymentType.C_PAYMENT_TYPE_PROMISSORY_NOTE ||
        //        //    payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CHEQUE_POST_DATED)
        //        //{
        //        //    if (item.MatchAmountIncWHT == unpaidAmount)
        //        //    {
        //        //        isSuccess = billingHandler.UpdateReceiptNo(doInvoice.InvoiceNo, doInvoice.InvoiceOCC, payment.RefAdvanceReceiptNo);
        //        //        if (!isSuccess)
        //        //            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7081, null);
        //        //    }
        //        //}
        //        #endregion

        //        #region Update receipt's advance receipt status for fully paid
        //        doReceipt receipt = this.GetReceiptByInvoiceNo(item.InvoiceNo, item.InvoiceOCC);
        //        if (item.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL
        //                 || item.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL_TO_FULL)
        //        {
        //            if (receipt != null && (receipt.AdvanceReceiptStatus == AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_ISSUED
        //                        || receipt.AdvanceReceiptStatus == AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_REGISTERED))
        //            {
        //                isSuccess = this.UpdateAdvanceReceiptMatchPayment(receipt.ReceiptNo);
        //                if (!isSuccess)
        //                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7081, null);

        //                this.DeleteTbt_MoneyCollectionInfo(receipt.ReceiptNo);
        //            }
        //        }
        //        #endregion
        //        #endregion

        //        #region Deposit
        //        #region Insert deposit fee, in case of deposit fee invoice
        //        string ContractCode = string.Empty;
        //        string BillingOCC = string.Empty;
        //        if (doInvoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT
        //            && doInvoice.Tbt_BillingDetails != null && doInvoice.Tbt_BillingDetails.Count > 0
        //            && (item.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_FULL
        //                 || item.MatchStatus == PaymentMatchingStatus.C_PAYMENT_MATCHING_PARTIAL_TO_FULL))
        //        {
        //            foreach (var doBillingDetail in doInvoice.Tbt_BillingDetails)
        //            {
        //                ContractCode = doBillingDetail.ContractCode;
        //                BillingOCC = doBillingDetail.BillingOCC;
        //                //decimal billingAmountIncVat = doBillingDetail.BillingAmount.GetValueOrDefault() * (1 + doInvoice.VatRate.GetValueOrDefault());
        //                // Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
        //                decimal billingAmountExcVat = doBillingDetail.BillingAmount.GetValueOrDefault();
        //                decimal billingAmountExcVatUsd = doBillingDetail.BillingAmountUsd.GetValueOrDefault();// add by Jirawat Jannet @2016-10-05 ยังไม่ถูก แก้ด้วย
        //                decimal balanceDepositAfterUpdate = 0;

        //                isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(ContractCode, BillingOCC, billingAmountExcVat, billingAmountExcVatUsd, out balanceDepositAfterUpdate);
        //                if (!isSuccess)
        //                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);

        //                isSuccess = billingHandler.InsertDepositFeePayment(ContractCode, BillingOCC, billingAmountExcVat, balanceDepositAfterUpdate, doInvoice.InvoiceNo, receipt == null ? null : receipt.ReceiptNo);
        //                if (!isSuccess)
        //                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);
        //            }
        //        }
        //        #endregion

        //        #region Insert deposit fee, in case of refund payment
        //        if (payment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND)
        //        {
        //            SECOM_AJIS.DataEntity.Billing.doRefundInfo doRefund = billingHandler.GetRefundInfo(payment.PaymentTransNo);

        //            if (doRefund != null)
        //            {
        //                if (doRefund.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
        //                {
        //                    //&& doInvoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
        //                    ContractCode = doInvoice.Tbt_BillingDetails[0].ContractCode;
        //                    BillingOCC = doInvoice.Tbt_BillingDetails[0].BillingOCC;

        //                    decimal balanceDepositAfterUpdate = 0;
        //                    //isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(doRefund.ContractCode, doRefund.BillingOCC, (item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (-1), out balanceDepositAfterUpdate);

        //                    //Modify by Jutarat A. on 10022014
        //                    //// Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
        //                    //// ((item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (1 + doInvoice.VatRate) --> Calculate only deposit fee, exclude VAT
        //                    //isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(doRefund.ContractCode, doRefund.BillingOCC, ((item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (1 + doInvoice.VatRate ?? 0)) * (-1), out balanceDepositAfterUpdate);
        //                    ////////
        //                    decimal adjustAmount = (item.MatchAmountIncWHT / (1 + doInvoice.VatRate ?? 0)) * (-1);
        //                    decimal adjustAmountUsd = (item.MatchAmountIncWHTUsd / (1 + doInvoice.VatRate ?? 0)) * (-1);// add by Jirawat Jannet @2016-10-05 ยังไม่ถูก แก้ด้วย
        //                    isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(doRefund.ContractCode, doRefund.BillingOCC, adjustAmount, adjustAmountUsd, out balanceDepositAfterUpdate);
        //                    //End Modify

        //                    if (!isSuccess)
        //                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);

        //                    if (balanceDepositAfterUpdate < 0)
        //                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7111, null);

        //                    //isSuccess = billingHandler.InsertDepositFeeSlide(doRefund, item.MatchAmountIncWHT - item.WHTAmount ?? 0, balanceDepositAfterUpdate, ContractCode, BillingOCC);

        //                    ////Modify by Jutarat A. on 10022014
        //                    //// Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
        //                    //// ((item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (100 + (doInvoice.VatRate ?? 0))/100) --> Calculate only deposit fee, exclude VAT
        //                    //isSuccess = billingHandler.InsertDepositFeeSlide(doRefund, ((item.MatchAmountIncWHT - item.WHTAmount ?? 0) / (1 + doInvoice.VatRate ?? 0)), balanceDepositAfterUpdate, ContractCode, BillingOCC);
        //                    ////////
        //                    isSuccess = billingHandler.InsertDepositFeeSlide(doRefund, (item.MatchAmountIncWHT / (1 + doInvoice.VatRate ?? 0)), balanceDepositAfterUpdate, ContractCode, BillingOCC);
        //                    //End Modify

        //                    if (!isSuccess)
        //                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);


        //                }
        //                /*Skip on 28/May/2012
        //                else if (doRefund.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT
        //                    && doInvoice.BillingTypeCode != BillingType.C_BILLING_TYPE_DEPOSIT)
        //                {
        //                    isSuccess = billingHandler.InsertDepositFeeReturn(doRefund, item.MatchAmountIncWHT - item.WHTAmount ?? 0, balanceDepositAfterUpdate);
        //                    if (!isSuccess)
        //                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);
        //                }*/
        //            }
        //            else
        //            {
        //                //Not found refund info
        //                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7082, null);
        //            }
        //        }
        //        #endregion
        //        #endregion
        //    }

        //    //Formular Balance After processing: 
        //    //balanceAfterProcessing = payment.MatchableBalance - (sumOfMatchPaymentAmount - sumOfTotalWht)  + BankFee + OtherExpense - OtherIncome
        //    //Remark Group to single value for calculate: 
        //    //(sumOfMatchPaymentAmount - sumOfTotalWht)  - BankFee - OtherExpense + OtherIncome
        //    decimal adjustPayment = (matchHeader.TotalMatchAmount ?? 0) - (matchHeader.BankFeeAmount ?? 0)
        //        - (matchHeader.OtherExpenseAmount ?? 0)
        //        + (matchHeader.OtherIncomeAmount ?? 0);


        //    #region Payment
        //    #region Update remaining matchable balance
        //    tbt_Payment paymentDB = this.GetPayment(payment.PaymentTransNo);
        //    if (paymentDB.UpdateDate != payment.UpdateDate)
        //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019, null);

        //    isSuccess = this.UpdatePaymentMatchableBalance(payment.PaymentTransNo, adjustPayment * (-1));        //minus value
        //    if (!isSuccess)
        //        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
        //    #endregion

        //    #region Update bankFee Flag
        //    if ((matchHeader.BankFeeAmount ?? 0) > 0)
        //    {
        //        isSuccess = this.UpdatePaymentBankFeeFlag(payment.PaymentTransNo, true);
        //        if (!isSuccess)
        //            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
        //    }
        //    #endregion

        //    #region Update Income Flag
        //    if ((matchHeader.OtherIncomeAmount ?? 0) > 0)
        //    {
        //        isSuccess = this.UpdatePaymentOtherIncomeFlag(payment.PaymentTransNo, true);
        //        if (!isSuccess)
        //            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
        //    }
        //    #endregion

        //    #region Update Expense Flag
        //    if ((matchHeader.OtherExpenseAmount ?? 0) > 0)
        //    {
        //        isSuccess = this.UpdatePaymentOtherExpenseFlag(payment.PaymentTransNo, true);
        //        if (!isSuccess)
        //            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
        //    }
        //    #endregion
        //    #endregion

        //    //Success
        //    return true;
        //}

        #endregion

        #endregion

        //Cancel
        //ICS090: Cancel
        /// <summary>
        /// Function for retrieving payment matching header and detail of an invoice. (sp_IC_GetMatchPaymentHeaderByInvoice, sp_IC_GetMatchPaymentHeaderByInvoice_Detail)
        /// </summary>
        /// <param name="invoiceNo">invoice no.</param>
        /// <param name="invoiceOCC">invoice occ</param>
        /// <returns></returns>
        public List<doMatchPaymentHeader> GetMatchPaymentHeaderByInvoice(string invoiceNo, int invoiceOCC)
        {
            List<doMatchPaymentHeader> result = base.GetMatchPaymentHeaderByInvoice(invoiceNo, (int?)invoiceOCC);
            if (result != null)
            {
                foreach (var item in result)
                {
                    item.MatchPaymentDetail = base.GetMatchPaymentHeaderByInvoiceDetail(item.MatchID);
                }
            }
            return result;
        }
        /// <summary>
        /// Function for update match payment header information to DB. (sp_IC_UpdateTbt_MatchPaymentHeader)
        /// </summary>
        /// <param name="xmlTbt_MatchPaymentHeader">xml of match payment header</param>
        /// <returns></returns>
        public int UpdateTbt_MatchPaymentHeader(string xmlTbt_MatchPaymentHeader)
        {
            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            List<tbt_MatchPaymentHeader> saved = base.UpdateTbt_MatchPaymentHeader(xmlTbt_MatchPaymentHeader);
            if (saved != null && saved.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_MATCH_PAYMENT_HEADER,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(saved);
                logHand.WriteTransactionLog(logData);
                return saved.Count;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// Function for update match payment detail information to DB. (sp_IC_UpdateTbt_MatchPaymentDetail)
        /// </summary>
        /// <param name="xmlTbt_MatchPaymentDetail">xml of match payment detail</param>
        /// <returns></returns>
        public int UpdateTbt_MatchPaymentDetail(string xmlTbt_MatchPaymentDetail)
        {
            ILogHandler logHand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
            List<tbt_MatchPaymentDetail> saved = base.UpdateTbt_MatchPaymentDetail(xmlTbt_MatchPaymentDetail);
            if (saved != null && saved.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog()
                {
                    TransactionType = doTransactionLog.eTransactionType.Update,
                    TableName = TableName.C_TBL_NAME_MATCH_PAYMENT_DETAIL,
                    TableData = null
                };
                logData.TableData = CommonUtil.ConvertToXml(saved);
                logHand.WriteTransactionLog(logData);
                return saved.Count;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// Function to retrieve description for the payment status.
        /// </summary>
        /// <param name="paymentStatus">payment status</param>
        /// <returns></returns>
        public doMiscTypeCode GetpaymentStatusDesc(string paymentStatus)
        {
            if (string.IsNullOrEmpty(paymentStatus) == true)
                return null;

            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
            {
                new doMiscTypeCode()
                {
                    FieldName = MiscType.C_PAYMENT_STATUS,
                    ValueCode = paymentStatus
                }
            };
            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            doMiscTypeCode item = hand.GetMiscTypeCodeList(miscs).FirstOrDefault();   //.Where(d => d.ValueCode == paymentStatus).FirstOrDefault();
            return item;
        }

        /// <summary>
        /// Cancel payment matching information of specific invoice information and cancel information
        /// </summary>
        /// <param name="doInvoice">invoice information</param>
        /// <param name="correctionReason">correction reason</param>
        /// <param name="approveNo">approve no.</param>
        /// <returns></returns>
        public bool CancelPaymentMatching(doInvoice doInvoice, string correctionReason, string approveNo)
        {
            var billingHandler = ServiceContainer.GetService<IBillingHandler>() as IBillingHandler;
            doInvoice doInvoiceDB = billingHandler.GetInvoice(doInvoice.InvoiceNo);
            if ((doInvoiceDB.UpdateDate ?? doInvoiceDB.CreateDate) != (doInvoice.UpdateDate ?? doInvoice.CreateDate))
            {
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG0019, null);
            }

            tbt_Invoice tbt_Invoice = CommonUtil.CloneObject<doInvoice, tbt_Invoice>(doInvoice);
            bool isSuccess = false;

            //Comment by Jutarat A. on 25122013
            /*if (correctionReason == CorrectionReason.C_CORRECTION_REASON_ENCASH_MISTAKE)
            {
                //Encash
                #region Invoice
                isSuccess = billingHandler.UpdateInvoiceCorrectionReason(doInvoice, correctionReason);
                if (!isSuccess)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7099, null);


                #region Update InvoicePaymentStatus
                if (tbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED)
                {
                    isSuccess = billingHandler.UpdateInvoicePaymentStatus(tbt_Invoice, doInvoice.Tbt_BillingDetails, PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED);
                    if (!isSuccess)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7116, null);
                }
                else if (doInvoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED)
                {
                    isSuccess = billingHandler.UpdateInvoicePaymentStatus(tbt_Invoice, doInvoice.Tbt_BillingDetails, PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED);
                    if (!isSuccess)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7116, null);
                }
                #endregion
                #endregion
            
                List<doMatchPaymentHeader> doMatchPaymentHeader = this.GetMatchPaymentHeaderByInvoice(tbt_Invoice.InvoiceNo, tbt_Invoice.InvoiceOCC);
                foreach (var matchHeader in doMatchPaymentHeader)
                {
                    #region Match
                    if (matchHeader.MatchPaymentDetail != null)
                    {
                        foreach (var matchDetail in matchHeader.MatchPaymentDetail)
                        {
                            if (matchDetail.InvoiceNo == tbt_Invoice.InvoiceNo && matchDetail.InvoiceOCC == tbt_Invoice.InvoiceOCC)
                            {
                                if ((matchDetail.CancelFlag ?? false) == false)
                                {
                                    #region Update Match Detail
                                    matchDetail.CancelApproveNo = approveNo;
                                    matchDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                    matchDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                    List<tbt_MatchPaymentDetail> detailList = new List<tbt_MatchPaymentDetail>();
                                    detailList.Add(CommonUtil.CloneObject<doMatchPaymentDetail, tbt_MatchPaymentDetail>(matchDetail));
                                    int row = this.UpdateTbt_MatchPaymentDetail(CommonUtil.ConvertToXml_Store<tbt_MatchPaymentDetail>(detailList));
                                    if (row < 1)
                                    {
                                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7096, null);
                                    }
                                    #endregion

                                    //There is only one invoice 
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            else
            {*/
            //End Comment

            #region Invoice
            isSuccess = billingHandler.UpdateInvoiceCorrectionReason(doInvoice, correctionReason);
            if (!isSuccess)
                throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7099, null);


            #region Update InvoicePaymentStatus
            if (correctionReason == CorrectionReason.C_CORRECTION_REASON_MISTAKE
                || correctionReason == CorrectionReason.C_CORRECTION_REASON_ENCASH_MISTAKE) //Add by Jutarat A. on 25122013
            {
                isSuccess = billingHandler.UpdateInvoicePaymentStatus(tbt_Invoice, doInvoice.Tbt_BillingDetails, PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED);
                if (!isSuccess)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7099, null);
            }
            else if (correctionReason == CorrectionReason.C_CORRECTION_REASON_DISHONOR)
            {
                if (tbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_MATCHED)
                {
                    isSuccess = billingHandler.UpdateInvoicePaymentStatus(tbt_Invoice, doInvoice.Tbt_BillingDetails, PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL);
                    if (!isSuccess)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7099, null);
                }
                else if (tbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_MATCHED)
                {
                    isSuccess = billingHandler.UpdateInvoicePaymentStatus(tbt_Invoice, doInvoice.Tbt_BillingDetails, PaymentStatus.C_PAYMENT_STATUS_POST_FAIL);
                    if (!isSuccess)
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7099, null);
                }
            }
            #endregion
            #endregion

            List<doMatchPaymentHeader> doMatchPaymentHeader = this.GetMatchPaymentHeaderByInvoice(tbt_Invoice.InvoiceNo, tbt_Invoice.InvoiceOCC);
            foreach (var matchHeader in doMatchPaymentHeader)
            {
                #region Match
                decimal amountReturn = 0;
                string currencyType = ""; // add by jirawat jannet @2016-10-06

                if (matchHeader.MatchPaymentDetail != null)
                {
                    foreach (var matchDetail in matchHeader.MatchPaymentDetail)
                    {
                        if (matchDetail.InvoiceNo == tbt_Invoice.InvoiceNo && matchDetail.InvoiceOCC == tbt_Invoice.InvoiceOCC)
                        {
                            if ((matchDetail.CancelFlag ?? false) == false)
                            {
                                #region Update Match Detail
                                matchDetail.CancelFlag = true;
                                matchDetail.CancelApproveNo = approveNo;
                                matchDetail.CorrectionReason = correctionReason;
                                matchDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                matchDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                List<tbt_MatchPaymentDetail> detailList = new List<tbt_MatchPaymentDetail>();
                                detailList.Add(CommonUtil.CloneObject<doMatchPaymentDetail, tbt_MatchPaymentDetail>(matchDetail));
                                int row = this.UpdateTbt_MatchPaymentDetail(CommonUtil.ConvertToXml_Store<tbt_MatchPaymentDetail>(detailList));
                                if (row < 1)
                                {
                                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7096, null);
                                }
                                //Return amount
                                //amountReturn = matchDetail.MatchAmountExcWHT; comment by jirawat janne 2016-10-06
                                // add by jirawat jannet @ 2016-10-06
                                currencyType = matchDetail.MatchAmountExcWHTCurrencyType;
                                if (matchDetail.MatchAmountExcWHTCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                                    amountReturn = matchDetail.MatchAmountExcWHT;
                                else
                                    amountReturn = matchDetail.MatchAmountExcWHTUsd;
                                #endregion


                                #region Update deposit info
                                if (doInvoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
                                {
                                    foreach (var doBillingDetail in doInvoice.Tbt_BillingDetails)
                                    {
                                        string ContractCode = doBillingDetail.ContractCode;
                                        string BillingOCC = doBillingDetail.BillingOCC;
                                        //decimal billingAmountIncVat = doBillingDetail.BillingAmount.GetValueOrDefault() * (1 + doInvoice.VatRate.GetValueOrDefault());
                                        // Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
                                        decimal billingAmountExcVat = doBillingDetail.BillingAmount.GetValueOrDefault();
                                        decimal billingAmountExcVatUsd = doBillingDetail.BillingAmountUsd.GetValueOrDefault();// add by Jirawat Jannet @2016-10-05 ยังไม่ถูก แก้ด้วย
                                        decimal? balanceDeposit = 0;
                                        decimal? balanceDepositUsd = 0;
                                        string balanceDepositCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;

                                        isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(
                                                            ContractCode
                                                            , BillingOCC
                                                            , billingAmountExcVat * (-1)
                                                            , billingAmountExcVatUsd * (-1)
                                                            , doBillingDetail.BillingAmountCurrencyType
                                                            , out balanceDeposit
                                                            , out balanceDepositUsd
                                                            , out balanceDepositCurrencyType);  //Minus value
                                        if (!isSuccess)
                                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7100, null);

                                        if (balanceDeposit < 0)
                                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7111, null);

                                        isSuccess = billingHandler.InsertDepositFeeCancelPayment(
                                            ContractCode
                                            , BillingOCC
                                            , billingAmountExcVat
                                            , balanceDeposit.Value
                                            , doInvoice.InvoiceNo
                                            , null
                                            , billingAmountExcVatUsd
                                            , doBillingDetail.BillingAmountCurrencyType);
                                        if (!isSuccess)
                                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7100, null);
                                    }
                                }
                                #endregion


                                #region Cancel deposit fee in case of refund payment (slide)
                                tbt_Payment doPayment = this.GetPayment(matchHeader.PaymentTransNo);
                                if (doPayment.PaymentType == PaymentType.C_PAYMENT_TYPE_CREDITNOTE_REFUND)
                                {
                                    doRefundInfo doRefundInfo = billingHandler.GetRefundInfo(doPayment.PaymentTransNo);
                                    if (doRefundInfo.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
                                    {
                                        decimal? balanceDeposit = 0;
                                        decimal? balanceDepositUsd = 0;
                                        string balanceDepositCurrencyType = CurrencyUtil.C_CURRENCY_LOCAL;
                                        //isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(
                                        //            doRefundInfo.ContractCode
                                        //            , doRefundInfo.BillingOCC
                                        //            , matchDetail.MatchAmountIncWHT
                                        //            , out balanceDeposit);

                                        // Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
                                        // (matchDetail.MatchAmountIncWHT / (1 + doInvoice.VatRate ?? 0))--> Calculate only deposit fee, exclude VAT
                                        decimal adjustAmount = (matchDetail.MatchAmountIncWHT / (1 + doInvoice.VatRate ?? 0));
                                        decimal adjustAmountUsd = (matchDetail.MatchAmountIncWHTUsd / (1 + doInvoice.VatRate ?? 0)); // add by Jirawat Jannet @2016-10-05 ยังไม่ถูก แก้ด้วย
                                        isSuccess = billingHandler.UpdateBalanceDepositOfBillingBasic(
                                                    doRefundInfo.ContractCode
                                                    , doRefundInfo.BillingOCC
                                                    , adjustAmount
                                                    , adjustAmountUsd
                                                    , matchDetail.MatchAmountIncWHTCurrencyType
                                                    , out balanceDeposit
                                                    , out balanceDepositUsd
                                                    , out balanceDepositCurrencyType);

                                        if (!isSuccess)
                                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7100, null);

                                        //isSuccess = billingHandler.InsertDepositFeeCancelSlide(
                                        //        doRefundInfo
                                        //        , matchDetail.MatchAmountIncWHT
                                        //        , balanceDeposit
                                        //        , doInvoice.Tbt_BillingDetails[0].ContractCode
                                        //        , doInvoice.Tbt_BillingDetails[0].BillingOCC);

                                        // Balance of deposit fee is not include TAX Edit by Patcharee 11/02/2013
                                        isSuccess = billingHandler.InsertDepositFeeCancelSlide(
                                                doRefundInfo
                                                , (matchDetail.MatchAmountIncWHT / (1 + doInvoice.VatRate ?? 0))
                                                , balanceDeposit.Value
                                                , doInvoice.Tbt_BillingDetails[0].ContractCode
                                                , doInvoice.Tbt_BillingDetails[0].BillingOCC
                                                , balanceDepositUsd.Value
                                                , balanceDepositCurrencyType);
                                        if (!isSuccess)
                                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7100, null);
                                    }
                                }
                                #endregion

                                //There is only one invoice 
                                break;
                            }
                        }
                    }
                }

                #region Update Match Header
                bool isAllDetailCancelled = matchHeader.MatchPaymentDetail.Where(d => (d.CancelFlag ?? false) == false).Count() == 0;   //All canceled, No active
                if (isAllDetailCancelled)
                {
                    matchHeader.CancelFlag = true;
                    matchHeader.CancelApproveNo = approveNo;
                    matchHeader.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                    matchHeader.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                    List<tbt_MatchPaymentHeader> headerList = new List<tbt_MatchPaymentHeader>();
                    headerList.Add(CommonUtil.CloneObject<doMatchPaymentHeader, tbt_MatchPaymentHeader>(matchHeader));
                    int row = this.UpdateTbt_MatchPaymentHeader(CommonUtil.ConvertToXml_Store<tbt_MatchPaymentHeader>(headerList));
                    if (row < 1)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7097, null);
                    }

                    #region Payment
                    #region Update bankFee
                    if ((matchHeader.BankFeeAmount ?? 0) > 0)
                    {
                        // amountReturn -= matchHeader.BankFeeAmount.Value; // comment by jirawat jannet @ 2016-10-06
                        // add by jirawat jannet @ 2016-10-06
                        if (matchHeader.BankFeeAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            amountReturn -= matchHeader.BankFeeAmount.Value;
                        else
                            amountReturn = matchHeader.BankFeeAmountUsd.Value;

                        isSuccess = this.UpdatePaymentBankFeeFlag(matchHeader.PaymentTransNo, false);
                        if (!isSuccess)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7098, null);
                    }
                    #endregion

                    #region Update other expense
                    if ((matchHeader.OtherExpenseAmount ?? 0) > 0)
                    {
                        //amountReturn -= matchHeader.OtherExpenseAmount.Value; // comment by jirawat jannet @ 2016-10-06
                        // add by jirawat jannet @ 2016-10-06
                        if (matchHeader.OtherExpenseAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            amountReturn -= matchHeader.OtherExpenseAmount.Value;
                        else
                            amountReturn -= matchHeader.OtherExpenseAmountUsd.Value;

                        isSuccess = this.UpdatePaymentOtherExpenseFlag(matchHeader.PaymentTransNo, false);
                        if (!isSuccess)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7098, null);
                    }
                    #endregion

                    #region Update other income
                    if ((matchHeader.OtherIncomeAmount ?? 0) > 0)
                    {
                        //amountReturn += matchHeader.OtherIncomeAmount.Value; // comment by jirawat jannet @ 2016-10-06
                        // add by jirawat jannet @ 2016-10-06
                        if (matchHeader.OtherIncomeAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                            amountReturn += matchHeader.OtherIncomeAmount.Value;
                        else
                            amountReturn += matchHeader.OtherIncomeAmountUsd.Value;

                        isSuccess = this.UpdatePaymentOtherIncomeFlag(matchHeader.PaymentTransNo, false);
                        if (!isSuccess)
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7098, null);
                    }
                    #endregion
                    #endregion
                }
                #endregion
                #endregion

                #region Payment
                #region Update remaining matchable balance
                //isSuccess = this.UpdatePaymentMatchableBalance(matchHeader.PaymentTransNo, amountReturn); // comment by jirawat jannet @ 2016-10-06
                // add by jirawat jannet @ 2016-10-06
                decimal amountReturnLocal = currencyType == CurrencyUtil.C_CURRENCY_LOCAL ? amountReturn : 0;
                decimal amountReturnUsd = currencyType == CurrencyUtil.C_CURRENCY_US ? amountReturn : 0;

                isSuccess = this.UpdatePaymentMatchableBalance(matchHeader.PaymentTransNo, amountReturnLocal, amountReturnUsd, currencyType);


                if (!isSuccess)
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_INCOME, MessageUtil.MessageList.MSG7083, null);
                #endregion
                #endregion
            }
            //}


            //Success
            return true;
        }
        #endregion
    }
}