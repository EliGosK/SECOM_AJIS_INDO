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
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;

// Used by Waroon
namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class BillingHandler : BizBLDataEntities, IBillingHandler
    {
        /// <summary>
        /// Get list of billing detail for cancel
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="paymentStatus"></param>
        /// <returns></returns>
        public List<doGetBillingDetailForCancel> GetBillingDetailForCancelList(string contractCode, string billingOCC, string paymentStatus)
        {
            try
            {
                List<doGetBillingDetailForCancel> result = base.GetBillingDetailForCancel(contractCode, billingOCC, paymentStatus, null, null);
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Check invoice for same account
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <param name="paymentMethod"></param>
        /// <returns></returns>
        public string CheckInvoiceSameAccountData(string invoiceNo, Nullable<int> invoiceOCC, string paymentMethod)
        {
            // expire - PLZ use CheckInvoiceSameAccount fn
            try
            {
                List<doCheckInvoiceSameAccount> result = base.CheckInvoiceSameAccount(invoiceNo, invoiceOCC, paymentMethod);
                if (result.Count > 0)
                {
                    return result[0].BankCode.ToString();
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get list of billing detail (continues fee)
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="paymentStatus"></param>
        /// <param name="c_BILLING_TYPE_GROUP_CONTINUES"></param>
        /// <returns></returns>
        public List<doGetBillingDetailContinues> GetBillingDetailContinuesList(string contractCode, string billingOCC, string paymentStatus, string c_BILLING_TYPE_GROUP_CONTINUES)
        {
            try
            {
                List<doGetBillingDetailContinues> result = base.GetBillingDetailContinues(contractCode, billingOCC, paymentStatus, c_BILLING_TYPE_GROUP_CONTINUES);
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get list of billing detail of invoice
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        public List<doGetBillingDetailOfInvoice> GetBillingDetailOfInvoiceList(string invoiceNo, Nullable<int> invoiceOCC)
        {
            try
            {
                //C_PAYMENT_STATUS_CANCEL
                //--08 (Normally cancelled)
                //C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED
                //--18 (Cancel payment matching)
                //C_PAYMENT_STATUS_NOTE_FAIL
                //--28 (Cancel payment by promissory note (Cancel payment matching)
                //C_PAYMENT_STATUS_POST_FAIL
                //--28 Cancelled payment by post date cheque (Cancel payment matching)

                string strPaymentStatus = "," + PaymentStatus.C_PAYMENT_STATUS_CANCEL + ","
                    + PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED + ","
                    + PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL + ","
                    + PaymentStatus.C_PAYMENT_STATUS_POST_FAIL + ",";

                List<doGetBillingDetailOfInvoice> result = base.GetBillingDetailOfInvoice(invoiceNo, invoiceOCC, strPaymentStatus, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get list of billing detail of invoice
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <param name="paymentStatus"></param>
        /// <returns></returns>
        public List<doGetBillingDetailOfInvoice> GetBillingDetailOfInvoiceList(string invoiceNo, Nullable<int> invoiceOCC, string paymentStatus)
        {

            try
            {

                if (paymentStatus.Substring(0, 1) != ",")
                {
                    paymentStatus = "," + paymentStatus + ",";
                }

                List<doGetBillingDetailOfInvoice> result = base.GetBillingDetailOfInvoice(invoiceNo, invoiceOCC, paymentStatus, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Get first monthly billing history of billing basic
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        public tbt_MonthlyBillingHistory GetFirstBillingHistoryData(string contractCode, string billingOCC)
        {
            try
            {
                List<tbt_MonthlyBillingHistory> result = base.GetFirstBillingHistory(contractCode, billingOCC, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get unpaid invoice and billing detail of invoice 
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoicePaymentStatus"></param>
        /// <returns></returns>
        public List<doGetUnpaidInvoiceData> GetUnpaidInvoiceDataList(string invoiceNo)
        {
            try
            {

                List<string> lstPaymentStatus = new List<string>();
                lstPaymentStatus.Add(PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT);
                lstPaymentStatus.Add(PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT);
                lstPaymentStatus.Add(PaymentStatus.C_PAYMENT_STATUS_COUNTER_BAL);
                lstPaymentStatus.Add(PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK);
                lstPaymentStatus.Add(PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK);
                lstPaymentStatus.Add(PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK);

                string strPaymentStatus = CommonUtil.CreateCSVString(lstPaymentStatus);

                List<doGetUnpaidInvoiceData> result = base.GetUnpaidInvoiceData(invoiceNo, strPaymentStatus);
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get tax (VAT and WHT) of billing type for billing basic
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="billingTypeCode"></param>
        /// <param name="invoiceDate"></param>
        /// <returns></returns>
        public doTax GetTaxChargedData(string contractCode, string billingOCC, string billingTypeCode, DateTime? invoiceDate)
        {
            try
            {
                List<doTax> result = base.GetTaxCharged(contractCode, billingOCC, billingTypeCode, invoiceDate);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get tax (VAT and WHT) of billing type for billing basic (ignore VATUnchargedFlag of Billing basic)
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="billingTypeCode"></param>
        /// <param name="invoiceDate"></param>
        /// <returns></returns>
        public decimal? GetVATMaster(string billingTypeCode, DateTime? invoiceDate)
        {
            decimal? vat = null;
            doVATMaster doVat = base.GetVATMaster(billingTypeCode, invoiceDate).FirstOrDefault();
            if (doVat != null)
                vat = doVat.VATRate;

            return vat;
        }

        /// <summary>
        /// To get invoice no.
        /// </summary>
        /// <param name="strRunningType"></param>
        /// <returns></returns>
        public string GetInvoiceNo(string strRunningType)
        {
            try
            {
                // return result in format 00000xxxxx
                // eg 0000001
                //

                List<string> result = base.GetInvoiceRunningNo(strRunningType
                    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Year.ToString()
                    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Month.ToString()
                    , CommonUtil.dsTransData.dtUserData.EmpNo
                    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return "";
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To get invoice OCC by invoice no.
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <returns></returns>
        public int GetInvoiceOCC(string strInvoiceNo)
        {
            try
            {
                List<string> result = base.GetInvoiceOCC(strInvoiceNo);
                if (result.Count > 0)
                {
                    return Convert.ToInt32(result[0]);
                }
                else
                    return 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To create invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public List<tbt_Invoice> CreateTbt_Invoice(tbt_Invoice invoice)
        {
            List<tbt_Invoice> invoiceList = new List<tbt_Invoice>();
            invoiceList.Add(invoice);
            var inserted_invoice = base.InsertTbt_Invoice(CommonUtil.ConvertToXml_Store<tbt_Invoice>(invoiceList));

            //Insert Log
            if (inserted_invoice.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                logData.TableName = TableName.C_TBL_NAME_INVOICE;
                logData.TableData = CommonUtil.ConvertToXml(inserted_invoice);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return inserted_invoice;

        }

        /// <summary>
        /// To create tax invoice
        /// </summary>
        /// <param name="taxInvoice"></param>
        /// <returns></returns>
        public List<tbt_TaxInvoice> CreateTbt_TaxInvoice(tbt_TaxInvoice taxInvoice)
        {
            List<tbt_TaxInvoice> taxinvoiceList = new List<tbt_TaxInvoice>();
            taxinvoiceList.Add(taxInvoice);
            var inserted_taxInvoice = base.InsertTbt_TaxInvoice(CommonUtil.ConvertToXml_Store<tbt_TaxInvoice>(taxinvoiceList));

            //Insert Log
            if (inserted_taxInvoice.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                logData.TableName = TableName.C_TBL_NAME_TAX_INVOICE;
                logData.TableData = CommonUtil.ConvertToXml(inserted_taxInvoice);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return inserted_taxInvoice;

        }

        /// <summary>
        /// To update tbt_Invoice
        /// </summary>
        /// <param name="Invoice"></param>
        /// <returns></returns>
        public int UpdateTbt_Invoice(tbt_Invoice Invoice)
        {
            try
            {
                //set updateDate and updateBy
                Invoice.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                Invoice.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_Invoice> tbt_Invoice = new List<tbt_Invoice>();
                tbt_Invoice.Add(Invoice);
                List<tbt_Invoice> updatedList = base.UpdateTbt_Invoice(CommonUtil.ConvertToXml_Store<tbt_Invoice>(tbt_Invoice));

                //Insert Log
                if (updatedList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_INVOICE;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //Add by Jutarat A. on 29072013
        /// <summary>
        /// To update list of tbt_Invoice
        /// </summary>
        /// <param name="tbt_Invoice"></param>
        /// <returns></returns>
        public List<tbt_Invoice> UpdateTbt_Invoice(List<tbt_Invoice> invoiceList)
        {
            try
            {
                //set updateDate and updateBy
                if (invoiceList != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_Invoice invoiceData in invoiceList)
                    {
                        invoiceData.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        invoiceData.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_Invoice> updatedList = base.UpdateTbt_Invoice(CommonUtil.ConvertToXml_Store<tbt_Invoice>(invoiceList));

                //Insert Log
                if (updatedList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_INVOICE;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //End Add

        /// <summary>
        /// To update tbt_Invoice without keep log
        /// </summary>
        /// <param name="Invoice"></param>
        /// <returns></returns>
        public int UpdateTbt_InvoiceNoLog(tbt_Invoice Invoice)
        {
            try
            {
                ////set updateDate and updateBy
                //Invoice.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //Invoice.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_Invoice> tbt_Invoice = new List<tbt_Invoice>();
                tbt_Invoice.Add(Invoice);
                List<tbt_Invoice> updatedList = base.UpdateTbt_Invoice(CommonUtil.ConvertToXml_Store<tbt_Invoice>(tbt_Invoice));



                return updatedList.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Get tax invoice data
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        public tbt_TaxInvoice GetTaxInvoiceData(string invoiceNo, int? invoiceOCC)
        {
            try
            {
                List<tbt_TaxInvoice> result = base.GetTaxInvoice(invoiceNo, invoiceOCC);
                if (result.Count > 0)
                {
                    //Modify by Jutarat A. on 20122013
                    //return result[0];
                    List<tbt_TaxInvoice> doCheckTaxInvoiceList = (from t in result
                                                                 where t.TaxInvoiceCanceledFlag == false
                                                                 select t).ToList<tbt_TaxInvoice>();

                    if (doCheckTaxInvoiceList != null && doCheckTaxInvoiceList.Count > 0)
                        return doCheckTaxInvoiceList[0];
                    else
                        return null;
                    //End Modify
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get tax invoice data list
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        public List<tbt_TaxInvoice> GetTaxInvoiceDataList(string invoiceNo, int? invoiceOCC) //Add by Jutarat A. on 20122013
        {
            try
            {
                List<tbt_TaxInvoice> result = base.GetTaxInvoice(invoiceNo, invoiceOCC);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To update tbt_TaxInvoice
        /// </summary>
        /// <param name="TaxInvoice"></param>
        /// <returns></returns>
        public List<tbt_TaxInvoice> UpdateTbt_TaxInvoice(tbt_TaxInvoice TaxInvoice)
        {
            try
            {
                //set updateDate and updateBy
                TaxInvoice.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                TaxInvoice.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_TaxInvoice> tbt_TaxInvoice = new List<tbt_TaxInvoice>();
                tbt_TaxInvoice.Add(TaxInvoice);
                List<tbt_TaxInvoice> updatedList = base.UpdateTbt_TaxInvoice(CommonUtil.ConvertToXml_Store<tbt_TaxInvoice>(tbt_TaxInvoice));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_TAX_INVOICE;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //Add by Jutarat A. on 29072013
        /// <summary>
        /// To update list of tbt_TaxInvoice
        /// </summary>
        /// <param name="tbt_TaxInvoice"></param>
        /// <returns></returns>
        public List<tbt_TaxInvoice> UpdateTbt_TaxInvoice(List<tbt_TaxInvoice> taxInvoiceList)
        {
            try
            {
                //set updateDate and updateBy
                if (taxInvoiceList != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_TaxInvoice taxInvoiceData in taxInvoiceList)
                    {
                        taxInvoiceData.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        taxInvoiceData.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                List<tbt_TaxInvoice> updatedList = base.UpdateTbt_TaxInvoice(CommonUtil.ConvertToXml_Store<tbt_TaxInvoice>(taxInvoiceList));

                //Insert Log
                if (updatedList.Count > 0)
                {

                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_TAX_INVOICE;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //End Add

        /// <summary>
        /// Get deposit transation of billing basic by deposit status
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="depositStatus"></param>
        /// <returns></returns>
        public List<tbt_Depositfee> GetDepositFee(string contractCode, string billingOCC, string invoiceNo, string depositStatus)
        {
            try
            {
                List<tbt_Depositfee> result = base.GetDepositFee(contractCode, billingOCC, invoiceNo, depositStatus, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To crate deposit fee
        /// </summary>
        /// <param name="depositFee"></param>
        /// <returns></returns>
        public List<tbt_Depositfee> CreateTbt_Depositfee(tbt_Depositfee depositFee)
        {
            //Add by Jutarat A. on 22082013
            depositFee.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            depositFee.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            depositFee.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
            depositFee.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
            //End Add

            List<tbt_Depositfee> depositFeeList = new List<tbt_Depositfee>();
            depositFeeList.Add(depositFee);
            var insertList = base.InsertTbt_Depositfee(CommonUtil.ConvertToXml_Store<tbt_Depositfee>(depositFeeList));

            //Insert Log
            if (insertList.Count > 0)
            {
                doTransactionLog logData = new doTransactionLog();
                logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                logData.TableName = TableName.C_TBL_NAME_DEPOSIT_FEE;
                logData.TableData = CommonUtil.ConvertToXml(insertList);
                ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                hand.WriteTransactionLog(logData);
            }

            return insertList;

        }

        //Add by Jutarat A. on 30042013
        /// <summary>
        /// To crate list of deposit fee
        /// </summary>
        /// <param name="depositFee"></param>
        /// <returns></returns>
        public List<tbt_Depositfee> CreateTbt_Depositfee(List<tbt_Depositfee> depositFeeList)
        {
            try
            {
                if (depositFeeList != null)
                {
                    dsTransDataModel dsTrans = CommonUtil.dsTransData;
                    foreach (tbt_Depositfee depositFee in depositFeeList)
                    {
                        depositFee.CreateDate = dsTrans.dtOperationData.ProcessDateTime;
                        depositFee.CreateBy = dsTrans.dtUserData.EmpNo;
                        depositFee.UpdateDate = dsTrans.dtOperationData.ProcessDateTime;
                        depositFee.UpdateBy = dsTrans.dtUserData.EmpNo;
                    }
                }

                var insertList = base.InsertTbt_Depositfee(CommonUtil.ConvertToXml_Store<tbt_Depositfee>(depositFeeList));

                //Insert Log
                if (insertList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Insert;
                    logData.TableName = TableName.C_TBL_NAME_DEPOSIT_FEE;
                    logData.TableData = CommonUtil.ConvertToXml(insertList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return insertList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //End Add

        /// <summary>
        /// To update tbt_DepositFee
        /// </summary>
        /// <param name="doDepositFee"></param>
        /// <returns></returns>
        public List<tbt_Depositfee> UpdateTbt_Depositfee(tbt_Depositfee doDepositFee)
        {
            try
            {
                //set updateDate and updateBy
                doDepositFee.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                doDepositFee.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_Depositfee> tbt_DepositFee = new List<tbt_Depositfee>();
                tbt_DepositFee.Add(doDepositFee);
                List<tbt_Depositfee> updatedList = base.UpdateTbt_Depositfee(CommonUtil.ConvertToXml_Store<tbt_Depositfee>(tbt_DepositFee));

                //Insert Log
                if (updatedList.Count > 0)
                {
                    doTransactionLog logData = new doTransactionLog();
                    logData.TransactionType = doTransactionLog.eTransactionType.Update;
                    logData.TableName = TableName.C_TBL_NAME_DEPOSIT_FEE;
                    logData.TableData = CommonUtil.ConvertToXml(updatedList);
                    ILogHandler hand = ServiceContainer.GetService<ILogHandler>() as ILogHandler;
                    hand.WriteTransactionLog(logData);
                }

                return updatedList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// To get data tbt_Invoice
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        public tbt_Invoice GetTbt_InvoiceData(string invoiceNo, int? invoiceOCC)
        {
            try
            {
                List<tbt_Invoice> result = base.GetTbt_Invoice(invoiceNo, invoiceOCC);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To update invoice payment status
        /// </summary>
        /// <param name="dotbt_Invoice"></param>
        /// <param name="dotbt_BillingDetailList"></param>
        /// <param name="strPaymentStatus"></param>
        /// <returns></returns>
        public Boolean UpdateInvoicePaymentStatus(tbt_Invoice dotbt_Invoice, List<tbt_BillingDetail> dotbt_BillingDetailList, string strPaymentStatus, DateTime? dtUpdateDate = null) //Modify (Add dtUpdateDate) by Jutarat A. on 25112013
        {
            List<tbt_BillingDetail> updatedBillingDetailList = null; //Add by Jutarat A. on 29042013

            try
            {
                if (dotbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_NOTE_ENCASHED
                    || dotbt_Invoice.InvoicePaymentStatus == PaymentStatus.C_PAYMENT_STATUS_POST_ENCASHED)
                {
                    #region Encash
                    tbt_TaxInvoice _dotbt_TaxInvoice = new tbt_TaxInvoice();
                    tbt_Invoice _dotbt_Invoice = GetTbt_Invoice(dotbt_Invoice.InvoiceNo, dotbt_Invoice.InvoiceOCC).FirstOrDefault();
                    if (_dotbt_Invoice == null)
                        return false;

                    //Add by Jutarat A. on 25112013
                    if (dtUpdateDate != null
                        && DateTime.Compare(dtUpdateDate.GetValueOrDefault(), _dotbt_Invoice.UpdateDate.GetValueOrDefault()) != 0)
                    {
                        throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                    }
                    //End Add

                    _dotbt_Invoice.InvoicePaymentStatus = strPaymentStatus;
                    if (UpdateTbt_Invoice(_dotbt_Invoice) == 0)
                        return false;

                    foreach (tbt_BillingDetail dotbt_BillingDetail in dotbt_BillingDetailList)
                    {
                        dotbt_BillingDetail.PaymentStatus = strPaymentStatus;
                        //Comment by Jutarat A. on 29042013
                        //if (Updatetbt_BillingDetail(dotbt_BillingDetail) == 0) 
                        //    return false;
                        //End Comment
                    }

                    updatedBillingDetailList = Updatetbt_BillingDetail(dotbt_BillingDetailList); //Add by Jutarat A. on 29042013

                    #endregion
                }
                else
                {
                    #region Other case

                    //Add by Jutarat A. on 29072013
                    List<tbt_Invoice> updatedInvoiceList = new List<tbt_Invoice>();
                    List<tbt_TaxInvoice> updatedTaxInvoiceList = new List<tbt_TaxInvoice>();
                    //End Add

                   // tbt_TaxInvoice _dotbt_TaxInvoice = new tbt_TaxInvoice();
                    List<tbt_Invoice> _dotbt_InvoiceList = GetTbt_Invoice(dotbt_Invoice.InvoiceNo, dotbt_Invoice.InvoiceOCC);
                    foreach (tbt_Invoice _dotbt_Invoice in _dotbt_InvoiceList)
                    {
                        //Add by Jutarat A. on 25112013
                        if (dtUpdateDate != null
                            && DateTime.Compare(dtUpdateDate.GetValueOrDefault(), _dotbt_Invoice.UpdateDate.GetValueOrDefault()) != 0)
                        {
                            throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0019);
                        }
                        //End Add

                        _dotbt_Invoice.InvoicePaymentStatus = strPaymentStatus;
                        //Modify by Jutarat A. on 29072013
                        //if (UpdateTbt_Invoice(_dotbt_Invoice) == 0)
                        //{
                        //    // error here
                        //};
                        updatedInvoiceList.Add(_dotbt_Invoice);
                        //End Modify

                        //_dotbt_TaxInvoice = GetTaxInvoiceData(_dotbt_Invoice.InvoiceNo, _dotbt_Invoice.InvoiceOCC);
                        //if (_dotbt_TaxInvoice != null &&
                        //    _dotbt_TaxInvoice.TaxInvoiceCanceledFlag == false)
                        //{
                        //    _dotbt_TaxInvoice.TaxInvoiceCanceledFlag = true;

                            //Modify by Jutarat A. on 29072013
                            //var updated_taxInvoice = UpdateTbt_TaxInvoice(_dotbt_TaxInvoice);
                            //if (updated_taxInvoice.Count == 0)
                            //{
                            //    // error here
                            //};
                        //    updatedTaxInvoiceList.Add(_dotbt_TaxInvoice);
                            //End Modify
                        //}

                        //Add by Jutarat A. on 29072013
                        if (updatedInvoiceList.Count > 0)
                            UpdateTbt_Invoice(updatedInvoiceList);

                      //  if (updatedTaxInvoiceList.Count > 0)
                        //    UpdateTbt_TaxInvoice(updatedTaxInvoiceList);
                        //End Add
                    }


                    List<tbt_Depositfee> depositFee_list = new List<tbt_Depositfee>();
                    foreach (tbt_BillingDetail dotbt_BillingDetail in dotbt_BillingDetailList)
                    {
                        dotbt_BillingDetail.PaymentStatus = strPaymentStatus;
                        //Comment by Jutarat A. on 29042013
                        //if (Updatetbt_BillingDetail(dotbt_BillingDetail) == 0) 
                        //    return false;
                        //End Comment
                    }

                    updatedBillingDetailList = Updatetbt_BillingDetail(dotbt_BillingDetailList); //Add by Jutarat A. on 29042013

                    if (dotbt_Invoice.BillingTypeCode == BillingType.C_BILLING_TYPE_DEPOSIT)
                    {
                        depositFee_list = GetDepositFee(null, null, dotbt_Invoice.InvoiceNo, DepositStatus.C_DEPOSIT_STATUS_ISSUE_INVOICE);
                        if (depositFee_list != null)
                        {
                            //Add by Jutarat A. on 29072013
                            tbt_Depositfee insertedDepositFee = new tbt_Depositfee();
                            List<tbt_Depositfee> insertedDepositFeeList = new List<tbt_Depositfee>();
                            //End Add

                            foreach (tbt_Depositfee deposit in depositFee_list)
                            {
                                deposit.ProcessDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                deposit.DepositStatus = DepositStatus.C_DEPOSIT_STATUS_CANCEL;
                                //Modify by Jutarat A. on 29072013
                                //deposit.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                //deposit.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                                //deposit.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                                //deposit.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;

                                //var Inserted_depositFee = CreateTbt_Depositfee(deposit);
                                insertedDepositFee = CommonUtil.CloneObject<tbt_Depositfee, tbt_Depositfee>(deposit);
                                insertedDepositFeeList.Add(insertedDepositFee);
                                //End Modify
                            }

                            //Add by Jutarat A. on 29072013
                            if (insertedDepositFeeList.Count > 0)
                                CreateTbt_Depositfee(insertedDepositFeeList);
                            //End Add
                        }
                    };
                    #endregion
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // BLP031

        /// <summary>
        /// To create invoice when billing send command
        /// </summary>
        /// <param name="dtInvoice"></param>
        /// <param name="dtBillingDetailList"></param>
        /// <param name="isGenerateTaxInvoice"></param>
        /// <param name="isEncrypt"></param>
        /// <returns></returns>
        public tbt_Invoice ManageInvoiceByCommand(tbt_Invoice dtInvoice, List<tbt_BillingDetail> dtBillingDetailList, Boolean isGenerateTaxInvoice, bool isEncrypt = true, bool isGenerateReport = true, bool? isForceIssue = null)
        {


            //----- Revise by Narupon W. 17 May 2012
            try
            {
                IIncomeHandler incomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                tbt_Invoice dtReturn = new tbt_Invoice();

                using (TransactionScope scope = new TransactionScope()) //Add by Jutarat A. on 29042013
                {
                // Get dVATRate , dWHTRate
                decimal? dVATRate = 0M;
                decimal? dWHTRate = 0M;
                if (dtBillingDetailList.Count > 0)
                {
                    var dtTaxCharge = this.GetTaxChargedData(dtBillingDetailList[0].ContractCode,
                                                                        dtBillingDetailList[0].BillingOCC,
                                                                        dtBillingDetailList[0].BillingTypeCode,
                                                                        dtInvoice.IssueInvDate);

                    if (dtTaxCharge != null)
                    {
                        dVATRate = Convert.ToDecimal(dtTaxCharge.VATRate);
                        dWHTRate = Convert.ToDecimal(dtTaxCharge.WHTRate);
                    }
                }

                // Validate data in dtBillingDetailList (BillingTypeCode , PaymentMethod) must be same value in dtBillingDetailList
                var list_BillingTypeCode = (from p in dtBillingDetailList select p.BillingTypeCode).Distinct().ToList();
                var list_PaymentMethod = (from p in dtBillingDetailList select p.PaymentMethod).Distinct().ToList();

                if (list_BillingTypeCode.Count > 1)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6059);
                }

                if (list_PaymentMethod.Count > 1)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6060);
                }

                if ((CommonUtil.IsNullOrEmpty(dtInvoice.InvoiceNo) == false) && (CommonUtil.IsNullOrEmpty(dtInvoice.InvoiceOCC) == true))
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0007, new string[] { "InvoiceOCC" });
                }

                // Get tbm_BillingType
                IBillingMasterHandler handlerBiingMaster = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                var billingType = handlerBiingMaster.GetTbm_BillingType(dtInvoice.BillingTypeCode);

                // Calculate amount (Invoice , TaxInvoice)
                var dSumBillingAmount = dtBillingDetailList.Sum(p => p.BillingAmount);
                var dSumBillingAmountUS = dtBillingDetailList.Sum(p => p.BillingAmountUsd);
                var dBillingCurrency = dtBillingDetailList.Select(m => m.BillingAmountCurrencyType).FirstOrDefault();

                dtInvoice.InvoiceAmountCurrencyType = dBillingCurrency;
                if (dtInvoice.InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    dtInvoice.InvoiceAmount = dSumBillingAmount;
                    dtInvoice.InvoiceAmountUsd = null;
                }
                else
                {
                    dtInvoice.InvoiceAmount = null;
                    dtInvoice.InvoiceAmountUsd = dSumBillingAmountUS;
                }

                if (dBillingCurrency == CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    dtInvoice.VatAmount = this.RoundUp((dSumBillingAmount ?? 0M) * (dVATRate ?? 0M), 2);
                    dtInvoice.WHTAmount = this.RoundUp((dSumBillingAmount ?? 0M) * (dWHTRate ?? 0M), 2);
                }
                else 
                {
                    dtInvoice.VatAmountUsd = this.RoundUp((dSumBillingAmountUS ?? 0M) * (dVATRate ?? 0M), 2);
                    dtInvoice.WHTAmountUsd = this.RoundUp((dSumBillingAmountUS ?? 0M) * (dWHTRate ?? 0M), 2);
                }

                if ((dtInvoice.WHTAmount != null && dtInvoice.WHTAmount > 0) || (dtInvoice.WHTAmountUsd != null && dtInvoice.WHTAmountUsd > 0))
                    dtInvoice.WHTAmountCurrencyType = dBillingCurrency;
                else
                    dtInvoice.WHTAmountCurrencyType = null;

                if ((dtInvoice.VatAmount != null && dtInvoice.VatAmount > 0) || (dtInvoice.VatAmountUsd != null && dtInvoice.VatAmountUsd > 0))
                    dtInvoice.VatAmountCurrencyType = dBillingCurrency;
                else
                    dtInvoice.VatAmountCurrencyType = null;

                dtInvoice.VatRate = dVATRate;
                dtInvoice.WHTRate = dWHTRate;

                // Generate new InvoiceNo
                bool isInvoiceNoExistInTaxInvoice = false;
                bool isInvoiceNoExistInReceipt = false;
                List<tbt_TaxInvoice> ExistInTaxInvoiceList = new List<tbt_TaxInvoice>();
                doReceipt ExistInReceipt = new doReceipt();
                if (CommonUtil.IsNullOrEmpty(dtInvoice.InvoiceNo))
                {
                    dtInvoice.InvoiceNo = GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_INVOICE, CommonUtil.dsTransData.dtUserData.EmpNo, dtInvoice.IssueInvDate.Value); //Edit by Patcharee T. For get invoice no. in month of InvoiceDate 11-Jun-2013
                    dtInvoice.InvoiceOCC = 1;
                }
                else
                {
                    // Check for this InvoiceNo is exist in tbt_TaxInvoice ?
                    ExistInTaxInvoiceList = base.GetTaxInvoiceByInvoiceNo(dtInvoice.InvoiceNo, dtInvoice.InvoiceOCC);

                    //ExistInTaxInvoiceList = ExistInTaxInvoiceList.OrderByDescending(t => t.TaxInvoiceNo).ToList<tbt_TaxInvoice>(); //Add by Jutarat A. on 30072013
                    ExistInTaxInvoiceList = ExistInTaxInvoiceList.OrderByDescending(t => t.CreateDate).ToList<tbt_TaxInvoice>(); //Modify by Jutarat A. on 07102013
                    if (ExistInTaxInvoiceList.Count > 0)
                    {
                        isInvoiceNoExistInTaxInvoice = true;
                    }

                    // Check for this InvoiceNo is exist in tbt_Receipt ?
                    ExistInReceipt = incomeHandler.GetReceiptByInvoiceNo(dtInvoice.InvoiceNo, dtInvoice.InvoiceOCC);
                    if (ExistInReceipt != null)
                    {
                        isInvoiceNoExistInReceipt = true;
                    }
                }

                // === Invoice ===
                // check over Digit before create tbt_invoice

                if (dtInvoice.InvoiceAmount > 999999999999.99M)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6084);
                }
                if (dtInvoice.InvoiceAmountUsd > 999999999999.99M)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6084);
                }
                if (dtInvoice.VatAmount > 999999999999.99M)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6085);
                }
                if (dtInvoice.WHTAmount > 999999999999.99M)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6086);
                }
                if (dtInvoice.WHTAmountUsd > 999999999999.99M)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6086);
                }
                if (dtInvoice.VatAmountUsd > 999999999999.99M)
                {
                    throw ApplicationErrorException.ThrowErrorException(MessageUtil.MODULE_BILLING, MessageUtil.MessageList.MSG6085);
                }

                // Prepate tbt_invoice
                string strInvoiceNo = string.Empty;
                tbt_Invoice invoice = new tbt_Invoice()
                {
                    InvoiceNo = dtInvoice.InvoiceNo,
                    InvoiceOCC = dtInvoice.InvoiceOCC,
                    IssueInvDate = dtInvoice.IssueInvDate,
                    AutoTransferDate = dtInvoice.AutoTransferDate,
                    BillingTargetCode = dtInvoice.BillingTargetCode,
                    BillingTypeCode = dtInvoice.BillingTypeCode,
                    InvoiceAmount = dtInvoice.InvoiceAmount,
                    PaidAmountIncVat = (int?)0,
                    VatRate = dtInvoice.VatRate,
                    VatAmount = dtInvoice.VatAmount,
                    WHTRate = dtInvoice.WHTRate,
                    WHTAmount = dtInvoice.WHTAmount,
                    RegisteredWHTAmount = (int?)0,
                    InvoicePaymentStatus = dtInvoice.InvoicePaymentStatus,
                    IssueInvFlag = dtInvoice.IssueInvFlag,
                    FirstIssueInvDate = dtInvoice.FirstIssueInvDate,//Convert.ToBoolean(dtInvoice.IssueInvFlag) == true ? dtInvoice.IssueInvDate : (DateTime?)null,
                    FirstIssueInvFlag = dtInvoice.FirstIssueInvFlag,//Convert.ToBoolean(dtInvoice.IssueInvFlag) == true ? true : false,
                    PaymentMethod = dtInvoice.PaymentMethod,
                    CorrectReason = null,
                    RefOldInvoiceNo = dtInvoice.RefOldInvoiceNo,

                    CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                    UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                    UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo,

                    VatAmountCurrencyType = dtInvoice.VatAmountCurrencyType,
                    WHTAmountCurrencyType = dtInvoice.WHTAmountCurrencyType,
                    VatAmountUsd = dtInvoice.VatAmountUsd,
                    WHTAmountUsd = dtInvoice.WHTAmountUsd,
                    InvoiceAmountUsd = dtInvoice.InvoiceAmountUsd,
                    InvoiceAmountCurrencyType = dtInvoice.InvoiceAmountCurrencyType
                };

                // CREATE !!
                var inserted_invoice = CreateTbt_Invoice(invoice);
                if (inserted_invoice.Count > 0)
                {
                    strInvoiceNo = inserted_invoice[0].InvoiceNo;
                    dtReturn = inserted_invoice[0];
                }

                // === TaxInvoice ===
                string strTaxInvoiceNo = string.Empty;
                if (isInvoiceNoExistInTaxInvoice)
                {
                    // Update InviceOCC
                    if (ExistInTaxInvoiceList.Count > 0 && inserted_invoice.Count > 0)
                    {
                        ExistInTaxInvoiceList[0].InvoiceOCC = inserted_invoice[0].InvoiceOCC;
                        ExistInTaxInvoiceList[0].TaxInvoiceCanceledFlag = false;

                        // UPDATE !!
                        var updated_taxInvoice = this.UpdateTbt_TaxInvoice(ExistInTaxInvoiceList[0]);
                        strTaxInvoiceNo = ExistInTaxInvoiceList[0].TaxInvoiceNo;
                    }
                }
                else
                {
                    // Create tbt_TaxInvoice
                    if (billingType.BillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_SALE || isGenerateTaxInvoice == true)
                    {
                        // Prepare tbt_TaxInvoice
                        tbt_TaxInvoice taxInvoice = new tbt_TaxInvoice()
                        {
                            TaxInvoiceNo = this.GetNextRunningNoByTypeMonthYear(RunningType.C_RUNNING_TYPE_TAX_INVOICE, CommonUtil.dsTransData.dtUserData.EmpNo, dtReturn.IssueInvDate.Value), //Edit by Patcharee T. For get invoice no. in month of InvoiceDate 11-Jun-2013
                            InvoiceNo = dtReturn.InvoiceNo,
                            InvoiceOCC = dtReturn.InvoiceOCC,
                            ReceiptNo = null,
                            TaxInvoiceDate = dtReturn.IssueInvDate,
                            TaxInvoiceCanceledFlag = false,
                            TaxInvoiceIssuedFlag = dtReturn.IssueInvFlag,

                            CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                            UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                        };

                        // CREATE !!
                        var inserted_taxInvoice = this.CreateTbt_TaxInvoice(taxInvoice);

                        if (inserted_taxInvoice.Count > 0)
                        {
                            strTaxInvoiceNo = inserted_taxInvoice[0].TaxInvoiceNo;
                        }
                    }
                }

                // === Receipt ===
                if (isInvoiceNoExistInReceipt)
                {
                    // Update InviceOCC
                    if (ExistInReceipt != null)
                    {
                        tbt_Receipt updateReceipt = new tbt_Receipt();

                        updateReceipt.ReceiptNo = ExistInReceipt.ReceiptNo;
                        updateReceipt.ReceiptDate = ExistInReceipt.ReceiptDate;
                        updateReceipt.BillingTargetCode = ExistInReceipt.BillingTargetCode;
                        updateReceipt.InvoiceNo = ExistInReceipt.InvoiceNo;
                        updateReceipt.InvoiceOCC = inserted_invoice[0].InvoiceOCC;
                        updateReceipt.ReceiptAmount = ExistInReceipt.ReceiptAmount;
                        updateReceipt.AdvanceReceiptStatus = ExistInReceipt.AdvanceReceiptStatus;
                        updateReceipt.ReceiptIssueFlag = ExistInReceipt.ReceiptIssueFlag;
                        updateReceipt.CancelFlag = ExistInReceipt.CancelFlag;
                        updateReceipt.CreateDate = ExistInReceipt.CreateDate;
                        updateReceipt.CreateBy = ExistInReceipt.CreateBy;
                        updateReceipt.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        updateReceipt.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        //UPDATE !!
                        var updated_Receipt = incomeHandler.UpdateTbt_Receipt(updateReceipt);
                    }
                }

                // === BillingDetail ===
                //Add by Jutarat A. on 30042013
                tbt_BillingDetail insertedBillingDetail = new tbt_BillingDetail();
                List<tbt_BillingDetail> insertedBillingDetailList = new List<tbt_BillingDetail>();

                tbt_BillingDetail updatedBillingDetail = new tbt_BillingDetail();
                List<tbt_BillingDetail> updatedBillingDetailList = new List<tbt_BillingDetail>();

                tbt_Depositfee insertedDepositFee = new tbt_Depositfee();
                List<tbt_Depositfee> insertedDepositFeeList = new List<tbt_Depositfee>();
                //End Add

                foreach (var item in dtBillingDetailList)
                {

                    // Prepare tbt_BillingDetail
                    tbt_BillingDetail billingDetail = new tbt_BillingDetail()
                    {
                        InvoiceNo = dtReturn.InvoiceNo,
                        InvoiceOCC = dtReturn.InvoiceOCC,
                        IssueInvDate = dtReturn.IssueInvDate,
                        IssueInvFlag = dtReturn.IssueInvFlag,
                        BillingTypeCode = item.BillingTypeCode,
                        BillingAmount = item.BillingAmount,
                        AdjustBillingAmount = item.AdjustBillingAmount,
                        BillingStartDate = item.BillingStartDate,
                        BillingEndDate = item.BillingEndDate,
                        PaymentMethod = dtReturn.PaymentMethod,
                        PaymentStatus = dtReturn.InvoicePaymentStatus,
                        AutoTransferDate = dtReturn.AutoTransferDate,
                        FirstFeeFlag = item.FirstFeeFlag,
                        DelayedMonth = item.DelayedMonth,
                        ForceIssueFlag = item.ForceIssueFlag,
                        ContractOCC = item.ContractOCC,

                        BillingAmountCurrencyType = item.BillingAmountCurrencyType,
                        BillingAmountUsd = item.BillingAmountUsd

                    };

                    if (CommonUtil.IsNullOrEmpty(item.BillingDetailNo) == true || item.BillingDetailNo == 0)
                    {
                        // Create BillingDetail
                        billingDetail.ContractCode = item.ContractCode;
                        billingDetail.BillingOCC = item.BillingOCC;

                        billingDetail.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        billingDetail.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                        billingDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        billingDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        //Modify by Jutarat A. on 30042013
                        //// CREATE !!
                        //var inserted_billingDetasdil = this.CreateTbt_BillingDetail(billingDetail);
                        insertedBillingDetail = CommonUtil.CloneObject<tbt_BillingDetail, tbt_BillingDetail>(billingDetail);
                        insertedBillingDetailList.Add(insertedBillingDetail);
                        //End Modify
                    }
                    else
                    {
                        // Update BillingDetail
                        billingDetail.ContractCode = item.ContractCode;
                        billingDetail.BillingOCC = item.BillingOCC;
                        billingDetail.BillingDetailNo = item.BillingDetailNo;

                        billingDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                        billingDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                        //Modify by Jutarat A. on 30042013
                        //// UPDATE !!
                        //var updated_billingDetail = this.Updatetbt_BillingDetail(billingDetail);
                        updatedBillingDetail = CommonUtil.CloneObject<tbt_BillingDetail, tbt_BillingDetail>(billingDetail);
                        updatedBillingDetailList.Add(updatedBillingDetail);
                        //End Modify
                    }


                    // === DepositFee ===
                    if (billingType.BillingTypeGroup == BillingTypeGroup.C_BILLING_TYPE_GROUP_DEPOSIT)
                    {
                        // Prepare tbt_DepositFee
                        tbt_Depositfee depositFee = new tbt_Depositfee()
                        {
                            ContractCode = item.ContractCode,
                            BillingOCC = item.BillingOCC,

                            ProcessDate = dtReturn.IssueInvDate,
                            DepositStatus = DepositStatus.C_DEPOSIT_STATUS_ISSUE_INVOICE,
                            //ProcessAmount = Convert.ToDecimal(item.BillingAmount) + this.RoundUp((item.BillingAmount ?? 0M) * (dVATRate ?? 0M), 2),
                            ProcessAmount = Convert.ToDecimal(item.BillingAmount),
                            InvoiceNo = dtReturn.InvoiceNo,
                            ProcessAmountUsd = Convert.ToDecimal(item.BillingAmountUsd),    //Modify by Pachara S. on 20170315
                            ProcessAmountCurrencyType = item.BillingAmountCurrencyType      //Modify by Pachara S. on 20170315

                            //Comment by Jutarat A. on 30042013
                            //,CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            //CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo,
                            //UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime,
                            //UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo
                            //End Comment
                        };

                        //Modify by Jutarat A. on 30042013
                        //// CREATE !!
                        //var inserted_depositFee = this.CreateTbt_Depositfee(depositFee);
                        insertedDepositFee = CommonUtil.CloneObject<tbt_Depositfee, tbt_Depositfee>(depositFee);
                        insertedDepositFeeList.Add(insertedDepositFee);
                        //End Modify
                    }
                }

                //Add by Jutarat A. on 30042013
                if (insertedBillingDetailList != null && insertedBillingDetailList.Count > 0)
                {
                    this.CreateTbt_BillingDetail(insertedBillingDetailList);
                }

                if (updatedBillingDetailList != null && updatedBillingDetailList.Count > 0)
                {
                    this.Updatetbt_BillingDetail(updatedBillingDetailList);
                }

                if (insertedDepositFeeList != null && insertedDepositFeeList.Count > 0)
                {
                    this.CreateTbt_Depositfee(insertedDepositFeeList);
                }
                //End Add

                IBillingDocumentHandler handlerBillingDocument = ServiceContainer.GetService<IBillingDocumentHandler>() as IBillingDocumentHandler;

                // ==== Report (Invoice) ===
                // ==== Report (Tax) ===
                string strInvoiceFilePath = string.Empty;
                // string strTaxInvoiceFilePath = string.Empty;


                if (isGenerateReport)
                {
                    if (isEncrypt) // (isEncrypt = true) is normal case
                    {
                        //if (string.IsNullOrEmpty(strInvoiceNo) == false && string.IsNullOrEmpty(strTaxInvoiceNo) == false)
                        //{
                        //    var mergeFile = handlerBillingDocument.GenerateBLR010_BLR020FilePath(strInvoiceNo, strTaxInvoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                        //    dtReturn.FilePath = mergeFile;
                        //}
                        //else 
                        if (string.IsNullOrEmpty(strInvoiceNo) == false)
                        {
                            strInvoiceFilePath = handlerBillingDocument.GenerateBLR010FilePath(strInvoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                            dtReturn.FilePath = strInvoiceFilePath;
                        }
                        // Comment by Jirawat Jannet
                        //else if (string.IsNullOrEmpty(strTaxInvoiceNo) == false)
                        //{
                        //    strTaxInvoiceFilePath = handlerBillingDocument.GenerateBLR020FilePath(strTaxInvoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
                        //    dtReturn.FilePath = strTaxInvoiceFilePath;
                        //}
                    }
                    else // (isEncrypt = false) is spacial case
                    {
                        if (string.IsNullOrEmpty(strInvoiceNo) == false)// && string.IsNullOrEmpty(strTaxInvoiceNo) == false)
                        {

                            List<string> mergeList = new List<string>();
                            string blr010 = handlerBillingDocument.GenerateBLR010FilePath(strInvoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, true);
                            mergeList.Add(blr010);
                            // Comment by Jirawat Jannet
                            //string blr020 = handlerBillingDocument.GenerateBLR020FilePath(strTaxInvoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, true);
                            //mergeList.Add(blr020);

                            string blr050 = handlerBillingDocument.GenerateBLR050FilePath(strInvoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, true);
                            mergeList.Add(blr050);

                            string mergeOutputFilename = PathUtil.GetTempFileName(".pdf");
                            string encryptOutputFileName = PathUtil.GetTempFileName(".pdf");


                            bool isSuccess = ReportUtil.MergePDF(mergeList.ToArray(), mergeOutputFilename, false, encryptOutputFileName, null);

                            dtReturn.FilePath = mergeOutputFilename;
                        }
                        //else if (string.IsNullOrEmpty(strInvoiceNo) == false)
                        //{
                        //    strInvoiceFilePath = handlerBillingDocument.GenerateBLR010FilePath(strInvoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, true);

                        //    dtReturn.FilePath = strInvoiceFilePath;
                        //}
                        // Comment by Jirawat Jannet
                        //else if (string.IsNullOrEmpty(strTaxInvoiceNo) == false)
                        //{
                        //    strTaxInvoiceFilePath = handlerBillingDocument.GenerateBLR020FilePath(strTaxInvoiceNo, CommonUtil.dsTransData.dtUserData.EmpNo, CommonUtil.dsTransData.dtOperationData.ProcessDateTime, true);

                        //    dtReturn.FilePath = strTaxInvoiceFilePath;
                        //}
                    }
                }

                scope.Complete();
                }

                return dtReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        //Call by screen

        /// <summary>
        /// To generate next running no. from specific type (invoice, tax invoice, receipt or payment transaction no.) and month-year for billing and income module.
        /// </summary>
        /// <param name="strRunningType"></param>
        /// <returns></returns>
        public string GetNextRunningNoByTypeMonthYear(string strRunningType)
        {
            return GetNextRunningNoByTypeMonthYear(strRunningType
                , CommonUtil.dsTransData.dtUserData.EmpNo
                , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);
        }

        //Call by batch process

        /// <summary>
        /// To generate next running no. from specific type (invoice, tax invoice, receipt or payment transaction no.) and month-year for billing and income module.
        /// </summary>
        /// <param name="strRunningType"></param>
        /// <param name="userId"></param>
        /// <param name="processDate"></param>
        /// <returns></returns>
        public string GetNextRunningNoByTypeMonthYear(string strRunningType, string userId, DateTime processDate)
        {
            try
            {
                string strPrefix = "";

                if (strRunningType == RunningType.C_RUNNING_TYPE_INVOICE)
                {
                    strPrefix = BillingIncomeDocPrefix.C_INVOICE_PREFIX;
                }
                if (strRunningType == RunningType.C_RUNNING_TYPE_TAX_INVOICE)
                {
                    strPrefix = BillingIncomeDocPrefix.C_TAX_INVOICE_PREFIX;
                }
                if (strRunningType == RunningType.C_RUNNING_TYPE_RECEIPT)
                {
                    strPrefix = BillingIncomeDocPrefix.C_RECEIPT_PREFIX;
                }
                if (strRunningType == RunningType.C_RUNNING_TYPE_CREDIT_NOTE)
                {
                    strPrefix = BillingIncomeDocPrefix.C_CREDIT_NOTE_PREFIX;
                }
                if (strRunningType == RunningType.C_RUNNING_TYPE_DEBT_TRACING_NOTICE1)
                {
                    strPrefix = BillingIncomeDocPrefix.C_DEBT_TRACING_NOTICE1;
                }
                if (strRunningType == RunningType.C_RUNNING_TYPE_DEBT_TRACING_NOTICE2)
                {
                    strPrefix = BillingIncomeDocPrefix.C_DEBT_TRACING_NOTICE2;
                }

                if (strRunningType == RunningType.C_RUNNING_TYPE_REVENUE)
                {
                    strPrefix = "";
                }
                if (strRunningType == RunningType.C_RUNNING_TYPE_PAYMENT_TRANS)
                {
                    strPrefix = "";
                }
                //Add by budd 21/Mar/2012
                if (strRunningType == RunningType.C_RUNNING_TYPE_MATCH_PAYMENT)
                {
                    strPrefix = "";
                }

                //List<string> result = base.GetInvoiceRunningNo(strRunningType
                //    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Year.ToString()
                //    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime.Month.ToString()
                //    , CommonUtil.dsTransData.dtUserData.EmpNo
                //    , CommonUtil.dsTransData.dtOperationData.ProcessDateTime);

                List<string> result = base.GetInvoiceRunningNo(strRunningType
                    , processDate.Year.ToString("0000")
                    , processDate.Month.ToString("00")
                    , userId
                    , DateTime.Now);

                if (result.Count > 0)
                {
                    string strMonth = processDate.Month.ToString("00");

                    return processDate.Year.ToString()
                        + strMonth
                        + strPrefix
                        + result[0];
                    // strYear + strMonth + strPrefix + strRunningNo(5 digit)
                }
                else
                    return string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check difference auto transfer or credit card account no. and return  bank code
        /// </summary>
        /// <param name="InvoiceNo"></param>
        /// <param name="InvoiceOCC"></param>
        /// <param name="PaymentMethod"></param>
        /// <returns></returns>
        public string CheckInvoiceSameAccount(string InvoiceNo, int? InvoiceOCC, string PaymentMethod)
        {
            List<doCheckInvoiceSameAccountNo> _doCheckInvoiceSameAccountNoList = new List<doCheckInvoiceSameAccountNo>();
            try
            {
                string strReturn = null;
                if (PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_AUTO_TRANFER)
                {
                    _doCheckInvoiceSameAccountNoList = base.CheckInvoiceSameAccountNo_AUTO_TRANFER(InvoiceNo, InvoiceOCC);
                }
                else if (PaymentMethod == PaymentMethodType.C_PAYMENT_METHOD_CREDIT_CARD)
                {
                    _doCheckInvoiceSameAccountNoList = base.CheckInvoiceSameAccountNo_CREDIT_CARD(InvoiceNo, InvoiceOCC);
                }

                if (_doCheckInvoiceSameAccountNoList != null)
                {
                    if (_doCheckInvoiceSameAccountNoList.Count == 1)
                    {
                        strReturn = _doCheckInvoiceSameAccountNoList[0].BankCode;
                    }
                }

                return strReturn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get list of billing detail (continues fee)
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="paymentStatus"></param>
        /// <returns></returns>
        public List<doGetBillingDetailContinues> GetBillingDetailContinuesList(string contractCode, string billingOCC, string paymentStatus)
        {
            try
            {
                //C_PAYMENT_STATUS_CANCEL
                //--08 (Normally cancelled)
                //C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED
                //--18 (Cancel payment matching)
                //C_PAYMENT_STATUS_NOTE_FAIL
                //--28 (Cancel payment by promissory note (Cancel payment matching)
                //C_PAYMENT_STATUS_POST_FAIL
                if (paymentStatus == string.Empty)
                {
                    paymentStatus = "," + PaymentStatus.C_PAYMENT_STATUS_CANCEL
                        + "," + PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED
                        + "," + PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL
                        + "," + PaymentStatus.C_PAYMENT_STATUS_POST_FAIL
                        + ",";
                }
                List<doGetBillingDetailContinues> result = base.GetBillingDetailContinues(
                    contractCode, billingOCC, paymentStatus, BillingTypeGroup.C_BILLING_TYPE_GROUP_CONTINUES);
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Get parcial fee of billing basic 
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        public List<tbt_BillingDetail> GetBillingDetailPartialFee(string contractCode, string billingOCC)
        {
            try
            {
                //C_PAYMENT_STATUS_CANCEL
                //--08 (Normally cancelled)
                //C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED
                //--18 (Cancel payment matching)
                //C_PAYMENT_STATUS_NOTE_FAIL
                //--28 (Cancel payment by promissory note (Cancel payment matching)
                //C_PAYMENT_STATUS_POST_FAIL
                //--28 Cancelled payment by post date cheque (Cancel payment matching)

                string paymentStatus = "," + PaymentStatus.C_PAYMENT_STATUS_CANCEL
                     + "," + PaymentStatus.C_PAYMENT_STATUS_PAYMENT_MATCH_CANCELLED
                     + "," + PaymentStatus.C_PAYMENT_STATUS_NOTE_FAIL
                     + "," + PaymentStatus.C_PAYMENT_STATUS_POST_FAIL
                     + ",";

                List<tbt_BillingDetail> result = base.GetBillingDetailPartialFeeList(contractCode, billingOCC, BillingType.C_BILLING_TYPE_SALE_PRICE_PARTIAL, paymentStatus,CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);

                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// To get data for exprot auto transfer 
        /// </summary>
        /// <param name="BankCode"></param>
        /// <param name="AutoTransferDate"></param>
        /// <returns></returns>
        public tbt_ExportAutoTransfer GetExportAutoTransfer(string BankCode, DateTime AutoTransferDate)
        {
            try
            {
                List<tbt_ExportAutoTransfer> result = base.GetExportAutoTransfer(BankCode, AutoTransferDate);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// To update invoice payment status without keep log
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strPaymentStatus"></param>
        /// <param name="strUpdateBy"></param>
        /// <param name="dtUpdateDate"></param>
        /// <returns></returns>
        public bool UpdateInvoicePaymentStatusNolog(string strInvoiceNo, string strPaymentStatus, string strUpdateBy, DateTime? dtUpdateDate, string AutoTransferFileName)
        {

            bool bolReturn = false;
            try
            {
                // Invoice CC = null = Last Invoice OCC
                List<tbt_Invoice> _dotbt_InvoiceList = base.GetTbt_Invoice(strInvoiceNo, null);
                tbt_BillingDetail _doUpdatetbt_BillingDetail = new tbt_BillingDetail();
                if (_dotbt_InvoiceList != null)
                {
                    if (_dotbt_InvoiceList.Count != 0)
                    {
                        tbt_Invoice _dotbt_Invoice = _dotbt_InvoiceList[0];

                        _dotbt_Invoice.InvoicePaymentStatus = strPaymentStatus;
                        _dotbt_Invoice.UpdateDate = dtUpdateDate;
                        _dotbt_Invoice.UpdateBy = strUpdateBy;

                        // tt
                        _dotbt_Invoice.AutoTransferFile = AutoTransferFileName;


                        if (UpdateTbt_InvoiceNoLog(_dotbt_Invoice) != 0)
                        {
                            List<doGetBillingDetailOfInvoice> _doGetBillingDetailOfInvoiceList = GetBillingDetailOfInvoice(
                                _dotbt_Invoice.InvoiceNo
                                , _dotbt_Invoice.InvoiceOCC
                                , _dotbt_Invoice.InvoicePaymentStatus, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                            int intCountEffectRow = 0;
                            foreach (doGetBillingDetailOfInvoice _doGetBillingDetailOfInvoice in _doGetBillingDetailOfInvoiceList)
                            {
                                _doUpdatetbt_BillingDetail = new tbt_BillingDetail();
                                _doUpdatetbt_BillingDetail.ContractCode = _doGetBillingDetailOfInvoice.ContractCode;
                                _doUpdatetbt_BillingDetail.BillingOCC = _doGetBillingDetailOfInvoice.BillingOCC;
                                _doUpdatetbt_BillingDetail.BillingDetailNo = _doGetBillingDetailOfInvoice.BillingDetailNo;
                                _doUpdatetbt_BillingDetail.InvoiceNo = _doGetBillingDetailOfInvoice.InvoiceNo;
                                _doUpdatetbt_BillingDetail.InvoiceOCC = _doGetBillingDetailOfInvoice.InvoiceOCC;
                                _doUpdatetbt_BillingDetail.IssueInvDate = _doGetBillingDetailOfInvoice.IssueInvDate;
                                _doUpdatetbt_BillingDetail.IssueInvFlag = _doGetBillingDetailOfInvoice.IssueInvFlag;
                                _doUpdatetbt_BillingDetail.BillingTypeCode = _doGetBillingDetailOfInvoice.BillingTypeCode;
                                _doUpdatetbt_BillingDetail.BillingAmount = _doGetBillingDetailOfInvoice.BillingAmount;
                                _doUpdatetbt_BillingDetail.AdjustBillingAmount = _doGetBillingDetailOfInvoice.AdjustBillingAmount;
                                _doUpdatetbt_BillingDetail.BillingStartDate = _doGetBillingDetailOfInvoice.BillingStartDate;
                                _doUpdatetbt_BillingDetail.BillingEndDate = _doGetBillingDetailOfInvoice.BillingEndDate;
                                _doUpdatetbt_BillingDetail.PaymentMethod = _doGetBillingDetailOfInvoice.PaymentMethod;
                                _doUpdatetbt_BillingDetail.PaymentStatus = strPaymentStatus;
                                _doUpdatetbt_BillingDetail.AutoTransferDate = _doGetBillingDetailOfInvoice.AutoTransferDate;
                                _doUpdatetbt_BillingDetail.FirstFeeFlag = _doGetBillingDetailOfInvoice.FirstFeeFlag;
                                _doUpdatetbt_BillingDetail.DelayedMonth = _doGetBillingDetailOfInvoice.DelayedMonth;
                                _doUpdatetbt_BillingDetail.UpdateDate = DateTime.Now;
                                _doUpdatetbt_BillingDetail.UpdateBy = strUpdateBy;
                                _doUpdatetbt_BillingDetail.ForceIssueFlag = _doGetBillingDetailOfInvoice.ForceIssueFlag;
                                _doUpdatetbt_BillingDetail.ContractOCC = _doGetBillingDetailOfInvoice.ContractOCC;

                                if (UpdateTbt_BillingDetailNoLog(_doUpdatetbt_BillingDetail) != 0)
                                {
                                    intCountEffectRow = intCountEffectRow + 1;
                                };

                            }

                            if (_doGetBillingDetailOfInvoiceList.Count == intCountEffectRow)
                            {
                                bolReturn = true;
                            }

                        }

                    }

                }

                return bolReturn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To update tbt_BillingDetail without keep log
        /// </summary>
        /// <param name="billingDetail"></param>
        /// <returns></returns>
        public int UpdateTbt_BillingDetailNoLog(tbt_BillingDetail billingDetail)
        {
            try
            {
                ////set updateDate and updateBy
                //billingDetail.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //billingDetail.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_BillingDetail> tbt_BillingDetail = new List<tbt_BillingDetail>();
                tbt_BillingDetail.Add(billingDetail);
                List<tbt_BillingDetail> updatedList = base.UpdateTbt_BillingDetailData(CommonUtil.ConvertToXml_Store<tbt_BillingDetail>(tbt_BillingDetail));

                return updatedList.Count;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// To get invoice with billing client name
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <returns></returns>
        public doGetInvoiceWithBillingClientName GetInvoiceWithBillingClientName(string strInvoiceNo)
        {
            try
            {
                List<doGetInvoiceWithBillingClientName> result = base.GetInvoiceWithBillingClientName(strInvoiceNo);
                if (result != null)
                {
                    if (result.Count > 0)
                    {
                        return result[0];
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Function for retrieving balance of deposit by billing code.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        public decimal? GetBalanceDepositValByBillingCode(string strContractCode, string strBillingOCC)
        {
            try
            {
                List<doGetBalanceDepositByBillingCode> result = base.GetBalanceDepositByBillingCode(strContractCode, strBillingOCC, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                if (result.Count > 0)
                {
                    return result[0].BalanceDeposit;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get information of a billing code
        /// </summary>
        /// <param name="strBillingCode"></param>
        /// <returns></returns>
        public List<doGetBillingCodeInfo> GetBillingCodeInfo(string strBillingCode)
        {
            try
            {
                List<doGetBillingCodeInfo> result = base.GetBillingCodeInfo(strBillingCode);
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get latest deposit transaction from deposit fee table.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        public List<tbt_Depositfee> GetLatestDepositFee(string strContractCode, string strBillingOCC)
        {
            try
            {
                List<tbt_Depositfee> result = base.GetLatestDepositFee(strContractCode, strBillingOCC);
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get tax invoice
        /// </summary>
        /// <param name="strTaxInvoiceNo"></param>
        /// <returns></returns>
        public List<doGetTaxInvoiceForIC> GetTaxInvoiceForIC(string strTaxInvoiceNo)
        {
            try
            {
                List<doGetTaxInvoiceForIC> result = base.GetTaxInvoiceForIC(strTaxInvoiceNo);
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // for 050 screen turn

        /// <summary>
        /// To get billing basic data (for BLS050)
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        public doBLS050GetBillingBasic BLS050_GetBillingBasic(string strContractCode, string strBillingOCC)
        {
            try
            {
                List<doBLS050GetBillingBasic> result = base.BLS050_GetBillingBasic(strContractCode, strBillingOCC, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return new doBLS050GetBillingBasic();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To get Billing detail for cancel list (for BLS050)
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        public List<doBLS050GetBillingDetailForCancelList> BLS050_GetBillingDetailForCancelList(string strContractCode, string strBillingOCC)
        {
            try
            {
                string strPaymentStatus = ","
                    + PaymentStatus.C_PAYMENT_STATUS_DETAIL_BANK_COLLECT + ","
                     + PaymentStatus.C_PAYMENT_STATUS_DETAIL_AUTO_CREDIT + ","
                      + PaymentStatus.C_PAYMENT_STATUS_INV_BANK_COLLECT + ","
                       + PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT + ","
                        + PaymentStatus.C_PAYMENT_STATUS_FAIL_AUTO_INV_BANK + ","
                         + PaymentStatus.C_PAYMENT_STATUS_FAIL_NOTE_INV_BANK + ","
                          + PaymentStatus.C_PAYMENT_STATUS_FAIL_CHEQUE_INV_BANK + ","
                           + PaymentStatus.C_PAYMENT_STATUS_COUNTER_BAL
                           + ",";

                List<doBLS050GetBillingDetailForCancelList> result = base.BLS050_GetBillingDetailForCancelList(strContractCode, strBillingOCC, strPaymentStatus, CurrencyUtil.C_CURRENCY_LOCAL, CurrencyUtil.C_CURRENCY_US);
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return new List<doBLS050GetBillingDetailForCancelList>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To get data of tbt_BillingTarget for view (BLS050)
        /// </summary>
        /// <param name="strBillingTargetCode"></param>
        /// <returns></returns>
        public doBLS050GetTbt_BillingTargetForView BLS050_GetTbt_BillingTargetForView(string strBillingTargetCode)
        {
            try
            {
                List<doBLS050GetTbt_BillingTargetForView> result = base.BLS050_GetTbt_BillingTargetForView(strBillingTargetCode);
                if (result.Count > 0)
                {
                    return result[0];
                }
                else
                    return new doBLS050GetTbt_BillingTargetForView();
            }
            catch (Exception)
            {
                throw;
            }
        }
        // process region

        /// <summary>
        /// Get bank data in case auto transfer for generate bank file 
        /// </summary>
        /// <returns></returns>
        public List<doGetBankAutoTransferDateForGen> GetBankAutoTransferDateForGenList()
        {
            try
            {
                List<doGetBankAutoTransferDateForGen> result = base.GetBankAutoTransferDateForGen();
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get invoice data for generate bank file 
        /// </summary>
        /// <param name="strBankCode"></param>
        /// <param name="strAutoTransferDate"></param>
        /// <returns></returns>
        public List<doGetInvoiceForGenBankFile> GetInvoiceForGenBankFile(string strBankCode, string strAutoTransferDate)
        {
            try
            {
                // strAutoTransferDate = day of date format '0#'
                //PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT = '03'

                List<doGetInvoiceForGenBankFile> result = base.GetInvoiceForGenBankFile(
                    strBankCode
                    , strAutoTransferDate
                    , PaymentStatus.C_PAYMENT_STATUS_INV_AUTO_CREDIT);
                if (result.Count > 0)
                {
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To insert data to tbt_ExportAutoTransfer without log
        /// </summary>
        /// <param name="dotbt_ExportAutoTransfer"></param>
        /// <returns></returns>
        public int CreateTbt_ExportAutoTransferNoLog(tbt_ExportAutoTransfer dotbt_ExportAutoTransfer)
        {
            try
            {
                ////set CreateDate, CreateBy, UpdateDate and UpdateBy
                //dotbt_ExportAutoTransfer.CreateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //dotbt_ExportAutoTransfer.CreateBy = CommonUtil.dsTransData.dtUserData.EmpNo;
                //dotbt_ExportAutoTransfer.UpdateDate = CommonUtil.dsTransData.dtOperationData.ProcessDateTime;
                //dotbt_ExportAutoTransfer.UpdateBy = CommonUtil.dsTransData.dtUserData.EmpNo;

                List<tbt_ExportAutoTransfer> insertList = base.InsertTbt_ExportAutoTransfer(
                    dotbt_ExportAutoTransfer.BankCode
                    , dotbt_ExportAutoTransfer.BankBranchCode
                    , dotbt_ExportAutoTransfer.GenerateDate
                    , dotbt_ExportAutoTransfer.AutoTransferDate
                    , dotbt_ExportAutoTransfer.FilePath
                    , dotbt_ExportAutoTransfer.FileName
                    , dotbt_ExportAutoTransfer.CreateDate
                    , dotbt_ExportAutoTransfer.CreateBy
                    , dotbt_ExportAutoTransfer.UpdateDate
                    , dotbt_ExportAutoTransfer.UpdateBy);

                return insertList.Count;
            }
            catch (Exception Ex)
            {
                throw;
            }
        }

    }



}
