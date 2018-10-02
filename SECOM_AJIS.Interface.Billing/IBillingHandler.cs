using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Contract;

using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Billing
{
    public interface IBillingHandler
    {
        /// <summary>
        /// Custom round up
        /// </summary>
        /// <param name="dec"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        decimal RoundUp(decimal dec, int digits);
        /// <summary>
        /// To create/ update billing basic and billing target if there are not existing in DB when contract send command 
        /// </summary>
        /// <param name="doBillingTempBasicList"></param>
        /// <returns></returns>
        List<doBillingTempBasic> ManageBillingBasic(List<doBillingTempBasic> doBillingTempBasicList);
        /// <summary>
        /// Get billing basic data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="strBillingTargetCode"></param>
        /// <param name="strBillingClientCode"></param>
        /// <param name="strBillingOfficeCode"></param>
        /// <returns></returns>
        doTbt_BillingBasic GetBillingBasicData(string strContractCode, string strBillingOCC, string strBillingTargetCode, string strBillingClientCode, string strBillingOfficeCode); 
        /// <summary>
        /// Get billing basic data
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="billingTargetCode"></param>
        /// <param name="billingClientCode"></param>
        /// <param name="billingOfficeCode"></param>
        /// <returns></returns>
        List<doTbt_BillingBasic> GetBillingBasic(string contractCode, string billingOCC, string billingTargetCode, string billingClientCode, string billingOfficeCode, string c_CURRENCY_LOCAL, string c_CURRENCY_US);
        /// <summary>
        /// to mapping miscellaneous type
        /// </summary>
        /// <param name="BillingTypeCode"></param>
        /// <param name="BillingTimingType"></param>
        /// <param name="ProductTypeCode"></param>
        /// <returns></returns>
        string MappingBillingType(string BillingTypeCode, string BillingTimingType, string ProductTypeCode);
        /// <summary>
        /// To update tbt_BillingBasic
        /// </summary>
        /// <param name="billingBasic"></param>
        /// <returns></returns>
        int UpdateTbt_BillingBasic(tbt_BillingBasic billingBasic);

        /// <summary>
        /// To update list of tbt_BillingBasic
        /// </summary>
        /// <param name="billingBasicList"></param>
        /// <returns></returns>
        List<tbt_BillingBasic> UpdateTbt_BillingBasic(List<tbt_BillingBasic> billingBasicList); //Add by Jutarat A. on 07052013

        /// <summary>
        /// Get data of Tbt_BillingTypeDetail
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="billingTypeCode"></param>
        /// <returns></returns>
        List<tbt_BillingTypeDetail> GetTbt_BillingTypeDetail(string contractCode, string billingOCC, string billingTypeCode);
        /// <summary>
        /// Create billing type data
        /// </summary>
        /// <param name="doBTD"></param>
        /// <returns></returns>
        bool CreateBillingTypeDetail(tbt_BillingTypeDetail doBTD);
        /// <summary>
        /// To insert data to Tbt_BillingTypeDetail
        /// </summary>
        /// <param name="dotbt_BillingTypeDetail"></param>
        /// <returns></returns>
        int InsertTbt_BillingTypeDetail(tbt_BillingTypeDetail dotbt_BillingTypeDetail);
        /// <summary>
        /// Get data of tbt_BillingTarget
        /// </summary>
        /// <param name="billingTargetCode"></param>
        /// <param name="billingClientCode"></param>
        /// <param name="billingOfficeCode"></param>
        /// <returns></returns>
        List<tbt_BillingTarget> GetTbt_BillingTarget(string billingTargetCode, string billingClientCode, string billingOfficeCode);
        /// <summary>
        /// Create billing target
        /// </summary>
        /// <param name="doBLTG"></param>
        /// <returns></returns>
        string CreateBillingTarget(tbt_BillingTarget doBLTG);
        /// <summary>
        /// To insert data of Tbt_BillingTarget
        /// </summary>
        /// <param name="doTbt_BillingTarget"></param>
        /// <returns></returns>
        int InsertTbt_BillingTarget(tbt_BillingTarget doTbt_BillingTarget);
        /// <summary>
        /// Get billing basic list
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="c_CUST_TYPE"></param>
        /// <returns></returns>
        List<BillingBasicList> GetBillingBasicListData(string contractCode, string c_CUST_TYPE);
        /// <summary>
        /// Crate billibg basic
        /// </summary>
        /// <param name="doTbt_BillingBasic"></param>
        /// <returns></returns>
        string CreateBillingBasic(tbt_BillingBasic doTbt_BillingBasic);
        /// <summary>
        /// To insert data to Tbt_BillingBasic
        /// </summary>
        /// <param name="doBillingBasic"></param>
        /// <returns></returns>
        int InsertTbt_BillingBasic(tbt_BillingBasic doBillingBasic);
        /// <summary>
        /// Create monthly billing history
        /// </summary>
        /// <param name="doTbt_MonthlyBillingHistory"></param>
        /// <returns></returns>
        bool CreateMonthlyBillingHistory(tbt_MonthlyBillingHistory doTbt_MonthlyBillingHistory);
        /// <summary>
        /// Get Lastest data of billing history
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<tbt_MonthlyBillingHistory> GetLastBillingHistory(string contractCode, string billingOCC, string c_CURRENCY_LOCAL, string c_CURRENCY_US);
        /// <summary>
        /// To update data of tbt_MonthlyBillingHistory
        /// </summary>
        /// <param name="xml_doTbtMonthlyBillingHistory"></param>
        /// <returns></returns>
        List<tbt_MonthlyBillingHistory> UpdateTbt_MonthlyBillingHistoryData(string xml_doTbtMonthlyBillingHistory);
        /// <summary>
        /// To update data of tbt_MonthlyBillingHistory
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        List<tbt_MonthlyBillingHistory> UpdateTbt_MonthlyBillingHistoryData(List<tbt_MonthlyBillingHistory> data);
        /// <summary>
        /// To update data of tbt_MonthlyBillingHistory
        /// </summary>
        /// <param name="dolast"></param>
        /// <returns></returns>
        int UpdateTbt_MonthlyBillingHistory(tbt_MonthlyBillingHistory dolast);
        /// <summary>
        /// To insert data to tbt_MonthlyBillingHistory
        /// </summary>
        /// <param name="doInsertMonthlyBillingHistory"></param>
        /// <returns></returns>
        int InsertTbt_MonthlyBillingHistory(tbt_MonthlyBillingHistory doInsertMonthlyBillingHistory);
        /// <summary>
        /// To insert data to tbt_MonthlyBillingHistory
        /// </summary>
        /// <param name="xml_MonthlyBillingHistory"></param>
        /// <returns></returns>
        List<tbt_MonthlyBillingHistory> InsertTbt_MonthlyBillingHistory(string xml_MonthlyBillingHistory);
        /// <summary>
        /// To create billing detail when billing send command
        /// </summary>
        /// <param name="doBillingDetail"></param>
        /// <returns></returns>
        tbt_BillingDetail ManageBillingDetail(tbt_BillingDetail doBillingDetail);

        /// <summary>
        /// To create list of billing detail when billing send command
        /// </summary>
        /// <param name="doBillingDetailList"></param>
        /// <returns></returns>
        List<tbt_BillingDetail> ManageBillingDetail(List<tbt_BillingDetail> doBillingDetailList); //Add by Jutarat A. on 07052013

        /// <summary>
        /// Create data of Tbt_BillingDetail
        /// </summary>
        /// <param name="doBLDT"></param>
        /// <returns></returns>
        List<tbt_BillingDetail> CreateTbt_BillingDetail(tbt_BillingDetail doBLDT);
  
        /// <summary>
        /// To insert data to list of tbt_BillingDetail
        /// </summary>
        /// <param name="billingDetail"></param>
        /// <returns></returns>
        List<tbt_BillingDetail> CreateTbt_BillingDetail(List<tbt_BillingDetail> billingDetailList); //Add by Jutarat A. on 30042013

        /// <summary>
        /// Get data of Tbt_BillingBasic
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<tbt_BillingBasic> GetTbt_BillingBasic(string contractCode, string billingOCC);
        /// <summary>
        /// To create billing detail when contract send command
        /// </summary>
        /// <param name="doBillingTempDetailListData"></param>
        /// <returns></returns>
        List<doBillingTempDetail> ManageBillingDetailByContract(List<doBillingTempDetail> doBillingTempDetailListData);
        /// <summary>
        /// Get billing OCC by contract code
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        List<string> GetBillingOCC(string contractCode);
        /// <summary>
        /// Get next auto transfer date
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="strPaymentMethod"></param>
        /// <returns></returns>
        DateTime? GetNextAutoTransferDate(string strContractCode, string strBillingOCC, string strPaymentMethod);
        /// <summary>
        /// Get data of Tbt_AutoTransferBankAccount
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<tbt_AutoTransferBankAccount> GetTbt_AutoTransferBankAccount(string contractCode, string billingOCC);
        /// <summary>
        /// Get data of Tbt_CreditCard
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<tbt_CreditCard> GetTbt_CreditCard(string contractCode, string billingOCC);
        /// <summary>
        /// Get billing detail no
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<Nullable<int>> GetBillingDetailNo(string contractCode, string billingOCC);
        /// <summary>
        /// To update billing basic when contract send command for start service
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="dtStartServiceDate"></param>
        /// <param name="dtAdjustDate"></param>
        /// <returns></returns>
        bool ManageBillingBasicForStart(string strContractCode, DateTime? dtStartServiceDate, DateTime? dtAdjustDate);
        /// <summary>
        /// Calculate billing amount from billing history 
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="strCalDailyFee"></param>
        /// <param name="dtStartDate"></param>
        /// <param name="dtEndDate"></param>
        /// <returns></returns>
        decimal CalculateBillingAmount(string strContractCode, string strBillingOCC, string strCalDailyFee, DateTime? dtStartDate, DateTime? dtEndDate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="BillingAmount"></param>
        /// <param name="CalDailyFeeStatus"></param>
        /// <returns></returns>
        decimal CalCulateBillingAmountPerHistory(DateTime? FromDate, DateTime? ToDate, decimal BillingAmount, string CalDailyFeeStatus);
        //int CalculateMonthDifference(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Calculate billing amount per one billing history by depend on calculation fee status such as calendar, 30.4 and other
        /// </summary>
        /// <param name="toDate"></param>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        int CalculateDayDifference(DateTime? toDate, DateTime? fromDate);
        /// <summary>
        /// To get continues billing type by billing code
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="c_BILLING_TYPE_GROUP_CONTINUES"></param>
        /// <returns></returns>
        List<BillingTypeDetail> GetBillingTypeDetailContinues(string contractCode, string billingOCC, string c_BILLING_TYPE_GROUP_CONTINUES);
        /// <summary>
        /// To update billing basic when contract send command for resume service
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="dtResumeDate"></param>
        /// <returns></returns>
        bool ManageBillingBasicForResume(string strContractCode, DateTime dtResumeDate);
        /// <summary>
        /// Calculate difference monthly fee
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="BillingOCC"></param>
        /// <param name="ChangeFeeDate"></param>
        /// <param name="MonthlyBillingAmount"></param>
        /// <returns></returns>
        AdjustOnNextPeriod CalculateDifferenceMonthlyFee(string ContractCode, string BillingOCC, DateTime ChangeFeeDate, decimal MonthlyBillingAmount, string callerObject);
        /// <summary>
        /// Get data of Tbt_AutoTransferBankAccount for view
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        List<dtTbt_AutoTransferBankAccountForView> GetTbt_AutoTransferBankAccountForView(string strContractCode, string strBillingOCC);

        //BLP014 
        /// <summary>
        /// To update billing basic when contract send command for stop service
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="dtStopDate"></param>
        /// <param name="dStopFee"></param>
        /// <returns></returns>
        bool ManageBillingBasicForStop(string strContractCode, DateTime dtStopDate, decimal dStopFee);
        /// <summary>
        /// To update billing basic and billing target if there are not existing in DB when contract send command for change name and address
        /// </summary>
        /// <param name="doBillingTempBasicListData"></param>
        /// <returns></returns>
        List<doBillingTempBasic> ManageBillingBasicForChangeNameAndAddress(List<doBillingTempBasic> doBillingTempBasicListData);
        /// <summary>
        /// To update billing basic when contract send command for cancel service
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="dtCancelDate"></param>
        /// <returns></returns>
        bool ManageBillingBasicForCancel(string strContractCode, DateTime dtCancelDate);
        /// <summary>
        /// Get data of Tbt_BillingTarget for view
        /// </summary>
        /// <param name="billingTargetCode"></param>
        /// <param name="c_CUST_TYPE"></param>
        /// <returns></returns>
        List<dtTbt_BillingTargetForView> GetTbt_BillingTargetForView(string billingTargetCode, string c_CUST_TYPE);
        /// <summary>
        /// Get data of Tbt_BillingTarget for view
        /// </summary>
        /// <param name="strBillingTargetCode"></param>
        /// <param name="C_CUST_TYPE"></param>
        /// <returns></returns>
        dtTbt_BillingTargetForView GetTbt_BillingTargetForViewData(string strBillingTargetCode, string C_CUST_TYPE);
        /// <summary>
        /// To update data of Tbt_BillingTarget
        /// </summary>
        /// <param name="billingTarget"></param>
        /// <returns></returns>
        int UpdateTbt_BillingTarget(tbt_BillingTarget billingTarget);
        /// <summary>
        /// To insert data of Tbt_AutoTransferBankAccount
        /// </summary>
        /// <param name="doTbtAutoTransferBankAccountList"></param>
        /// <returns></returns>
        int InsertTbt_AutoTransferBankAccountData(List<tbt_AutoTransferBankAccount> doTbtAutoTransferBankAccountList);
        /// <summary>
        /// To insert data of Tbt_CreditCard
        /// </summary>
        /// <param name="doTbtCreditCardList"></param>
        /// <returns></returns>
        int InsertTbt_CreditCard(List<tbt_CreditCard> doTbtCreditCardList);

        //BLS071
        /// <summary>
        /// Get billing detail for combine (call store sp_BL_GetBillingDetailForCombine)
        /// </summary>
        /// <param name="billingTargetCode"></param>
        /// <param name="billingTypeCode"></param>
        /// <returns></returns>
        List<doBillingDetail> GetBillingDetailForCombine(string billingTargetCode, string billingTypeCode, string c_CURRENCY_LOCAL, string c_CURRENCY_US,string c_CURRENCY);

        //BLS040

        /// <summary>
        /// To delete data of Tbt_BillingTypeDetail
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="strBillingTypeCode"></param>
        /// <returns></returns>
        List<tbt_BillingTypeDetail> DeleteTbt_BillingTypeDetail(string strContractCode, string strBillingOCC, string strBillingTypeCode);
        /// <summary>
        /// To delete data of Tbt_CreditCard
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        List<tbt_CreditCard> DeleteTbt_CreditCard(string strContractCode, string strBillingOCC);
        /// <summary>
        /// To delete data of Tbt_AutoTransferBankAccount
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        List<tbt_AutoTransferBankAccount> DeleteTbt_AutoTransferBankAccount(string strContractCode, string strBillingOCC);
        /// <summary>
        /// To update data of Tbt_BillingTypeDetail
        /// </summary>
        /// <param name="lstUpdate"></param>
        /// <returns></returns>
        List<tbt_BillingTypeDetail> UpdateTbt_BillingTypeDetail(List<tbt_BillingTypeDetail> lstUpdate);
        /// <summary>
        /// To update data of Tbt_CreditCard
        /// </summary>
        /// <param name="lstUpdate"></param>
        /// <returns></returns>
        List<tbt_CreditCard> UpdateTbt_CreditCard(List<tbt_CreditCard> lstUpdate);
        /// <summary>
        /// To update data of Tbt_AutoTransferBankAccount
        /// </summary>
        /// <param name="lstUpdate"></param>
        /// <returns></returns>
        List<tbt_AutoTransferBankAccount> UpdateTbt_AutoTransferBankAccount(List<tbt_AutoTransferBankAccount> lstUpdate);
        /// <summary>
        /// Get billing history data list
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="BillingOCC"></param>
        /// <returns></returns>
        List<doTbt_MonthlyBillingHistoryList> GetBillingHistoryList(string ContractCode, string BillingOCC, string c_CURRENCY_LOCAL, string c_CURRENCY_US);
        /// <summary>
        /// Get billing type detail data list
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<doBillingTypeDetailList> GetBillingTypeDetailList(string contractCode, string billingOCC);

        //BLS050-060
        /// <summary>
        /// Get list of billing detail for cancel
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="paymentStatus"></param>
        /// <returns></returns>
        List<doGetBillingDetailForCancel> GetBillingDetailForCancelList(string contractCode, string billingOCC, string paymentStatus);
        /// <summary>
        ///  Check invoice for same account
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <param name="paymentMethod"></param>
        /// <returns></returns>
        string CheckInvoiceSameAccountData(string invoiceNo, Nullable<int> invoiceOCC, string paymentMethod);
        /// <summary>
        /// Get list of billing detail (continues fee)
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="paymentStatus"></param>
        /// <param name="c_BILLING_TYPE_GROUP_CONTINUES"></param>
        /// <returns></returns>
        List<doGetBillingDetailContinues> GetBillingDetailContinuesList(string contractCode, string billingOCC, string paymentStatus, string c_BILLING_TYPE_GROUP_CONTINUES);
        /// <summary>
        /// Get list of billing detail (continues fee)
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <param name="paymentStatus"></param>
        /// <returns></returns>
        List<doGetBillingDetailOfInvoice> GetBillingDetailOfInvoiceList(string invoiceNo, Nullable<int> invoiceOCC, string paymentStatus);
        /// <summary>
        /// Get list of billing detail (continues fee)
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        List<doGetBillingDetailOfInvoice> GetBillingDetailOfInvoiceList(string invoiceNo, Nullable<int> invoiceOCC);
        /// <summary>
        /// Get first monthly billing history of billing basic
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        tbt_MonthlyBillingHistory GetFirstBillingHistoryData(string contractCode, string billingOCC);
        /// <summary>
        /// Get unpaid invoice and billing detail of invoice 
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        List<doGetUnpaidInvoiceData> GetUnpaidInvoiceDataList(string invoiceNo);

        //List<tbt_BillingDetail> GetTbt_BillingDetailData(string strContractCode, string strBillingOCC, int intBillingDetailNo);
        
        /// <summary>
        /// Get data of Tbt_BillingDetail
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="intBillingDetailNo"></param>
        /// <returns></returns>
        List<tbt_BillingDetail> GetTbt_BillingDetailData(string strContractCode, string strBillingOCC, int? intBillingDetailNo);

        /// <summary>
        /// To delete data of Tbt_BillingDetail
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <param name="intBillingDetailNo"></param>
        /// <returns></returns>
        List<tbt_BillingDetail> DeleteTbt_BillingDetail(string strContractCode, string strBillingOCC, int intBillingDetailNo);
        /// <summary>
        /// To update data of Tbt_BillingDetail
        /// </summary>
        /// <param name="billingDetail"></param>
        /// <returns></returns>
        int Updatetbt_BillingDetail(tbt_BillingDetail billingDetail, DateTime? dtUpdateDate = null); //Modify (Add dtUpdateDate) by Jutarat A. on 25112013

        //Add by Jutarat A. on 29042013
        /// <summary>
        /// To update list of tbt_BillingDetail
        /// </summary>
        /// <param name="billingDetailList"></param>
        /// <returns></returns>
        List<tbt_BillingDetail> Updatetbt_BillingDetail(List<tbt_BillingDetail> billingDetailList, bool isCheckUpdateDate = false); //Modify (Add isCheckUpdateDate) by Jutarat A. on 25112013
      
        /// <summary>
        ///  To create invoice when billing send command
        /// </summary>
        /// <param name="dotbt_Invoice"></param>
        /// <param name="dotbt_BillingDetail"></param>
        /// <param name="isGenerateTaxInvoice"></param>
        /// <param name="isEncrypt"></param>
        /// <returns></returns>
        tbt_Invoice ManageInvoiceByCommand(tbt_Invoice dotbt_Invoice, List<tbt_BillingDetail> dotbt_BillingDetail, Boolean isGenerateTaxInvoice, bool isEncrypt = true, bool isGenerateReport = true, bool? isForceIssue = null);
        /// <summary>
        /// To update invoice payment status
        /// </summary>
        /// <param name="dotbt_Invoice"></param>
        /// <param name="dotbt_BillingDetailList"></param>
        /// <param name="strPaymentStatus"></param>
        /// <returns></returns>
        Boolean UpdateInvoicePaymentStatus(tbt_Invoice dotbt_Invoice, List<tbt_BillingDetail> dotbt_BillingDetailList, string strPaymentStatus, DateTime? dtUpdateDate = null); //Modify (Add dtUpdateDate) by Jutarat A. on 25112013
        
        /// <summary>
        /// Get tax invoice data
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        tbt_TaxInvoice GetTaxInvoiceData(string invoiceNo, int? invoiceOCC);
        /// <summary>
        /// Get tax invoice data list
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        List<tbt_TaxInvoice> GetTaxInvoiceDataList(string invoiceNo, int? invoiceOCC); //Add by Jutarat A. on 20122013

        /// <summary>
        /// Get data of Tbt_TaxInvoice
        /// </summary>
        /// <param name="taxinvoiceNo"></param>
        /// <returns></returns>
        List<tbt_TaxInvoice> GetTbt_TaxInvoice(string taxinvoiceNo);
        /// <summary>
        /// To update data of Tbt_TaxInvoice
        /// </summary>
        /// <param name="TaxInvoice"></param>
        /// <returns></returns>
        List<tbt_TaxInvoice> UpdateTbt_TaxInvoice(tbt_TaxInvoice TaxInvoice);

        /// <summary>
        /// To update list of tbt_TaxInvoice
        /// </summary>
        /// <param name="tbt_TaxInvoice"></param>
        /// <returns></returns>
        List<tbt_TaxInvoice> UpdateTbt_TaxInvoice(List<tbt_TaxInvoice> taxInvoiceList); //Add by Jutarat A. on 29072013

        /// <summary>
        /// Get deposit transation of billing basic by deposit status
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="depositStatus"></param>
        /// <returns></returns>
        List<tbt_Depositfee> GetDepositFee(string contractCode, string billingOCC, string invoiceNo, string depositStatus);
        /// <summary>
        /// Get data of Tbt_Depositfee
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="depositFeeNo"></param>
        /// <returns></returns>
        List<tbt_Depositfee> GetTbt_Depositfee(string contractCode, string billingOCC, int? depositFeeNo);
        /// <summary>
        /// Create data of Tbt_Depositfee
        /// </summary>
        /// <param name="dotbt_DepositFee"></param>
        /// <returns></returns>
        List<tbt_Depositfee> CreateTbt_Depositfee(tbt_Depositfee dotbt_DepositFee);

        /// <summary>
        /// To crate list of deposit fee
        /// </summary>
        /// <param name="depositFee"></param>
        /// <returns></returns>
        List<tbt_Depositfee> CreateTbt_Depositfee(List<tbt_Depositfee> depositFeeList); //Add by Jutarat A. on 30042013

        /// <summary>
        /// To update data of Tbt_Depositfee
        /// </summary>
        /// <param name="doDepositFee"></param>
        /// <returns></returns>
        List<tbt_Depositfee> UpdateTbt_Depositfee(tbt_Depositfee doDepositFee);
        
        //ICS050
        /// <summary>
        /// To force issue tax invoice
        /// </summary>
        /// <param name="doInvoice"></param>
        /// <param name="taxInvoiceDate"></param>
        /// <returns></returns>
        tbt_TaxInvoice ForceIssueTaxInvoice(doInvoice doInvoice, DateTime taxInvoiceDate);
        //ICP010
        /// <summary>
        /// Update receipt no. to deposit fee transaction after receipt generated.
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="receiptNo"></param>
        /// <param name="batchId"></param>
        /// <param name="batchDate"></param>
        /// <returns></returns>
        bool UpdateReceiptNoDepositFee(string invoiceNo, string receiptNo,string batchId,DateTime batchDate);


        //ICS060
        /// <summary>
        /// Update clear receipt no., invoice no of deposit fee transaction.
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="receiptNo"></param>
        /// <param name="batchId"></param>
        /// <param name="batchDate"></param>
        /// <returns></returns>
        bool UpdateReceiptNoDepositFeeCancelReceipt(string invoiceNo, string receiptNo, string batchId, DateTime batchDate);

        /// <summary>
        /// Force issue a receipt for unpaid invoice.
        /// </summary>
        /// <param name="doInvoice"></param>
        /// <param name="taxInvoiceDate"></param>
        /// <param name="batchId"></param>
        /// <param name="batchDate"></param>
        /// <returns></returns>
        tbt_TaxInvoice IssueTaxInvoice(doInvoice doInvoice, DateTime taxInvoiceDate, string batchId, DateTime batchDate);


        /// <summary>
        /// Get data of Tbt_Invoice
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        tbt_Invoice GetTbt_InvoiceData(string invoiceNo, int? invoiceOCC);

        List<tbt_Invoice> GetTbt_Invoice(string invoiceNo, Nullable<int> invoiceOCC); //Add by Jutarat A.on 05062013

        //CMS422

        /// <summary>
        /// Get data of Tbt_CreditCard for view
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<dtTbt_CreditCardForView> GetTbt_CreditCardForView(string contractCode, string billingOCC);

        /// <summary>
        /// To generate next running no. from specific type (invoice, tax invoice, receipt or payment transaction no.) and month-year for billing and income module.
        /// </summary>
        /// <param name="strRunningType"></param>
        /// <returns></returns>
        string GetNextRunningNoByTypeMonthYear(string strRunningType);
        /// <summary>
        /// To generate next running no. from specific type (invoice, tax invoice, receipt or payment transaction no.) and month-year for billing and income module.
        /// </summary>
        /// <param name="strRunningType"></param>
        /// <param name="userId"></param>
        /// <param name="processDate"></param>
        /// <returns></returns>
        string GetNextRunningNoByTypeMonthYear(string strRunningType, string userId, DateTime processDate);

        /// <summary>
        /// Get data of Tbt_BillingBasic for view
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<dtTbt_BillingBasicForView> GetTbt_BillingBasicForView(string contractCode, string billingOCC);
        /// <summary>
        /// Check difference auto transfer or credit card account no. and return  bank code
        /// </summary>
        /// <param name="InvoiceNo"></param>
        /// <param name="InvoiceOCC"></param>
        /// <param name="PaymentMethod"></param>
        /// <returns></returns>
        string CheckInvoiceSameAccount(string InvoiceNo, int? InvoiceOCC, string PaymentMethod);
        /// <summary>
        /// Get list of billing detail (continues fee)
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="paymentStatus"></param>
        /// <returns></returns>
        List<doGetBillingDetailContinues> GetBillingDetailContinuesList(string contractCode, string billingOCC, string paymentStatus);
        /// <summary>
        /// Get parcial fee of billing basic 
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<tbt_BillingDetail> GetBillingDetailPartialFee(string contractCode, string billingOCC);

        /// <summary>
        /// To get data for exprot auto transfer 
        /// </summary>
        /// <param name="BankCode"></param>
        /// <param name="AutoTransferDate"></param>
        /// <returns></returns>
        tbt_ExportAutoTransfer GetExportAutoTransfer(string BankCode, DateTime AutoTransferDate);
        /// <summary>
        ///  Get tax (VAT and WHT) of billing type for billing basic
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="billingTypeCode"></param>
        /// <param name="invoiceDate"></param>
        /// <returns></returns>
        doTax GetTaxChargedData(string contractCode, string billingOCC, string billingTypeCode, DateTime? invoiceDate);

        /// <summary>
        /// Get VAT of billing type
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="billingTypeCode"></param>
        /// <param name="invoiceDate"></param>
        /// <returns></returns>
        decimal? GetVATMaster(string billingTypeCode, DateTime? invoiceDate);

        /// <summary>
        /// To update invoice payment status without keep log
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <param name="strPaymentStatus"></param>
        /// <param name="strUpdateBy"></param>
        /// <param name="dtUpdateDate"></param>
        /// <returns></returns>
        bool UpdateInvoicePaymentStatusNolog(string strInvoiceNo, string strPaymentStatus, string strUpdateBy, DateTime? dtUpdateDate ,string AutoTransferFileName);
        /// <summary>
        /// Get invoice that has billing period between change date
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="changeFeeDate"></param>
        /// <returns></returns>
        List<tbt_Invoice> GetInvoiceOfChangeDate(string contractCode, string billingOCC, Nullable<System.DateTime> changeFeeDate);
        /// <summary>
        /// Get data of Tbt_BillingDetail by invoice
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        List<tbt_BillingDetail> GetTbt_BillingDetailOfInvoice(string invoiceNo, int? invoiceOCC);
        /// <summary>
        /// To get invoice with billing client name
        /// </summary>
        /// <param name="strInvoiceNo"></param>
        /// <returns></returns>
        doGetInvoiceWithBillingClientName GetInvoiceWithBillingClientName(string strInvoiceNo);

        /// <summary>
        /// To insert data to tbt_ExportAutoTransfer without log
        /// </summary>
        /// <param name="dotbt_ExportAutoTransfer"></param>
        /// <returns></returns>
        int CreateTbt_ExportAutoTransferNoLog(tbt_ExportAutoTransfer dotbt_ExportAutoTransfer);
        /// <summary>
        /// Get invoice data for generate bank file 
        /// </summary>
        /// <param name="strBankCode"></param>
        /// <param name="strAutoTransferDate"></param>
        /// <returns></returns>
        List<doGetInvoiceForGenBankFile> GetInvoiceForGenBankFile(string strBankCode, string strAutoTransferDate);
        /// <summary>
        /// Get bank data in case auto transfer for generate bank file 
        /// </summary>
        /// <returns></returns>
        List<doGetBankAutoTransferDateForGen> GetBankAutoTransferDateForGenList();


        //ICS084
        /// <summary>
        /// To update receipt no. to tax invoice table
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <param name="receiptNo"></param>
        /// <returns></returns>
        bool UpdateReceiptNo(string invoiceNo, int invoiceOCC, string receiptNo);
        /// <summary>
        /// Get invoice and billing detail of invoice 
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        doInvoice GetInvoice(string invoiceNo);
        /// <summary>
        /// Function for updating balance of deposit for billing basic.
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="ajustAmount"></param>
        /// <param name="depositBalanceAfterUpdate"></param>
        /// <returns></returns>
        bool UpdateBalanceDepositOfBillingBasic(string contractCode, string billingOCC
            , decimal? ajustAmount, decimal? ajustAmountUsd, string BalanceDepositCurrencyType
            , out decimal? depositBalanceAfterUpdate, out decimal? depositBalanceUsdAfterUpdate, out string balanceDepositAfterUpdateCurrencyType);

        //ICS090
        /// <summary>
        /// To check exist in receipt data by invoice
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        bool CheckExistReceiptForInvoice(string invoiceNo, int invoiceOCC);
        /// <summary>
        /// Function for checking whether invoice has been issued a tax invoice
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        bool CheckInvoiceIssuedTaxInvoice(string invoiceNo, int invoiceOCC);
        /// <summary>
        /// To update invoice correction reason
        /// </summary>
        /// <param name="doInvoice"></param>
        /// <param name="correctionReason"></param>
        /// <returns></returns>
        bool UpdateInvoiceCorrectionReason(doInvoice doInvoice, string correctionReason);

        //ICS040
        /// <summary>
        /// Register billing exemption by updating payment status to invoice and billing detail.
        /// </summary>
        /// <param name="doInvoiceExemption"></param>
        /// <returns></returns>
        bool RegisterInvoiceExemption(doInvoice doInvoiceExemption);

        //ICS060
        /// <summary>
        /// Mapping billing type code to invoice type: Deposit invoice , Service invoice ,Sales invoice
        /// </summary>
        /// <param name="billingTypeCode"></param>
        /// <returns></returns>
        string GetInvoiceType(string billingTypeCode);
        /// <summary>
        /// Cancel tax invoice by marking cancel flag.
        /// </summary>
        /// <param name="taxtInvoiceNo"></param>
        /// <returns></returns>
        bool CancelTaxInvoice(string taxtInvoiceNo);
        
        //ICP010
        /// <summary>
        /// To update receipt no. to tax invoice table
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <param name="receiptNo"></param>
        /// <param name="batchId"></param>
        /// <param name="batchDate"></param>
        /// <returns></returns>
        bool UpdateReceiptNo(string invoiceNo, int invoiceOCC, string receiptNo, string batchId, DateTime batchDate);


        //Refund
        /// <summary>
        /// Function for retrieving information of refund deposit payment 
        /// </summary>
        /// <param name="paymentTransNo"></param>
        /// <returns></returns>
        doRefundInfo GetRefundInfo(string paymentTransNo);
        //Deposit Fee

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
        bool InsertDepositFeePayment(string contractCode, string billingOCC, decimal? depositAmount, decimal? depositAmountUsd, string depositAmountCurrencyType, decimal? balanceDeposit, decimal? balanceDepositUsd, string balanceDepositCurrencyType, string invoiceNo, string receiptNo);
        /// <summary>
        /// Insert deposit fee transaction table after payment
        /// </summary>
        /// <param name="doRefundDeposit"></param>
        /// <param name="slidingAmount"></param>
        /// <param name="balanceDeposit"></param>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        bool InsertDepositFeeSlide(doRefundInfo doSlideRefund, decimal? slidingAmount, decimal? balanceDeposit
            , string contractCode, string billingOCC, string processAmountCurrencyType, decimal? processAmountUsd
            , string receivedFeeCurrencyType, decimal? receivedFeeUsd);
        /// <summary>
        /// Insert return deposit fee transaction after payment matching or deleted
        /// </summary>
        /// <param name="doRefundDeposit"></param>
        /// <param name="returnAmount"></param>
        /// <param name="balanceDeposit"></param>
        /// <returns></returns>
        bool InsertDepositFeeReturn(doRefundInfo doSlideRefund, decimal? returnAmount, decimal? returnAmountUsd, string returnAmountCurrenctyType
            , decimal? balanceDeposit, decimal? balanceDepositUsd, string balanceDepositCurrencyType);
        /// <summary>
        /// Insert deposit fee transaction of cancel of slide after cancel payment matching.
        /// </summary>
        /// <param name="doRefundDeposit"></param>
        /// <param name="cancelAmount"></param>
        /// <param name="balanceDeposit"></param>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        bool InsertDepositFeeCancelSlide(doRefundInfo doRefundDeposit, decimal cancelAmount, decimal balanceDeposit, string contractCode, string billingOCC, decimal balanceDepositUsd, string balanceDepositCurrencyType);
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
        bool InsertDepositFeeCancelPayment(string contractCode, string billingOCC, decimal cancelAmount, decimal balanceDeposit, string invoiceNo, string receiptNo, decimal cancelAmountUsd, string cancelAmountCurrencyType);
        /// <summary>
        /// Insert revenue deposit fee transaction after register revenue from deposit fee.
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="revenueAmount"></param>
        /// <param name="balanceDeposit"></param>
        /// <param name="revenueNo"></param>
        /// <returns></returns>
        bool InsertDepositFeeRevenue(string contractCode, string billingOCC, decimal revenueAmount, decimal revenueAmountUsd
            , string revenueAmountCurrencyType, decimal balanceDeposit, decimal balanceDepositUsd, string balanceDepositCurrencyType, string revenueNo);

        /// <summary>
        /// To update data of Tbt_Invoice
        /// </summary>
        /// <param name="xmlTbt_Invoice"></param>
        /// <returns></returns>
        List<tbt_Invoice> UpdateTbt_Invoice(string xmlTbt_Invoice);

        /// <summary>
        /// To update list of tbt_Invoice
        /// </summary>
        /// <param name="tbt_Invoice"></param>
        /// <returns></returns>
        List<tbt_Invoice> UpdateTbt_Invoice(List<tbt_Invoice> invoiceList); //Add by Jutarat A. on 29072013

        /// <summary>
        /// Process - get download auto transfer bank file
        /// </summary>
        /// <param name="secomAccountID"></param>
        /// <param name="autoTransferDateFrom"></param>
        /// <param name="autoTransferDateTo"></param>
        /// <param name="generateDateFrom"></param>
        /// <param name="generateDateTo"></param>
        /// <returns></returns>
        List<dtDownloadAutoTransferBankFile> GetDownloadAutoTransferBankFile(int? secomAccountID, DateTime? autoTransferDateFrom, DateTime? autoTransferDateTo, DateTime? generateDateFrom, DateTime? generateDateTo);
        
        /// <summary>
        /// Get contract information for billing
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        List<dtBillingContract> GetBillingContract(string contractCode, string c_CURRENCY_LOCAL, string c_CURRENCY_US);
        /// <summary>
        /// Get list of billing basic for rental contractby ContractCode
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        List<dtBillingBasicForRentalList> GetBillingBasicForRentalList(string contractCode);

        //ICS020

        /// <summary>
        /// Update auto transfer account last result from auto transfer import file.
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="lastestResult"></param>
        /// <returns></returns>
        bool UpdateAutoTransferAccountLastResult(string invoiceNo, string lastestResult);
        /// <summary>
        /// Function for checking that whether tax invoice has been issued for the invoice.
        /// </summary>
        /// <param name="doInvoice"></param>
        /// <returns></returns>
        bool CheckTaxInvoiceIssued(doInvoice doInvoice);


        // BLS090
        /// <summary>
        /// Process - update monthly billing amount
        /// </summary>
        /// <param name="billingBasicList"></param>
        /// <returns></returns>
        List<tbt_BillingBasic> UpdateMonthlyBillingAmount(List<tbt_BillingBasic> billingBasicList);

        // Select From ICS Module

        /// <summary>
        /// Function for retrieving balance of deposit by billing code.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        decimal? GetBalanceDepositValByBillingCode(string strContractCode, string strBillingOCC);
        /// <summary>
        /// Function for retrieving balance of deposit by billing code.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        List<doGetBalanceDepositByBillingCode> GetBalanceDepositByBillingCode(string strContractCode, string strBillingOCC);
        /// <summary>
        /// Get information of a billing code
        /// </summary>
        /// <param name="strBillingCode"></param>
        /// <returns></returns>
        List<doGetBillingCodeInfo> GetBillingCodeInfo(string strBillingCode);
        /// <summary>
        /// Get latest deposit transaction from deposit fee table.
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        List<tbt_Depositfee> GetLatestDepositFee(string strContractCode, string strBillingOCC);

        /// <summary>
        /// Get tax invoice
        /// </summary>
        /// <param name="strTaxInvoiceNo"></param>
        /// <returns></returns>
        List<doGetTaxInvoiceForIC> GetTaxInvoiceForIC(string strTaxInvoiceNo);

        /// <summary>
        /// To get last working day
        /// </summary>
        /// <param name="checkingDate"></param>
        /// <returns></returns>
        DateTime GetLastWorkingDay(DateTime? checkingDate);

        /// <summary>
        /// Get tax invoice data by invoice no
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="invoiceOCC"></param>
        /// <returns></returns>
        List<tbt_TaxInvoice> GetTaxInvoiceByInvoiceNo(string invoiceNo, Nullable<int> invoiceOCC);

        /// <summary>
        /// Get data of Tbt_BillingBasic
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<tbt_BillingBasic> GetTbt_BillingBasicListData(string strContractCode);

        /// <summary>
        /// To get list of billing detail that Issue invoice (Auto transfer,Credit card)
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<tbt_BillingDetail> GetBillingDetailAutoTransferList(string contractCode, string billingOCC);
        /// <summary>
        /// To get Auto transfer bank account data for view
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<dtAutoTransferBankAccountForView> GetAutoTransferBankAccountForView(string contractCode, string billingOCC);
        /// <summary>
        /// To get credit card data for view
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<dtCreditCardForView> GetCreditCardForView(string contractCode, string billingOCC);

        // BLS050 Turn

        /// <summary>
        /// To get billing basic data (for BLS050)
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        doBLS050GetBillingBasic BLS050_GetBillingBasic(string strContractCode, string strBillingOCC);
        /// <summary>
        /// Get list of billing detail for cancel
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBillingOCC"></param>
        /// <returns></returns>
        List<doBLS050GetBillingDetailForCancelList> BLS050_GetBillingDetailForCancelList(string strContractCode, string strBillingOCC);
        /// <summary>
        /// To get data of tbt_BillingTarget for view (BLS050)
        /// </summary>
        /// <param name="strBillingTargetCode"></param>
        /// <returns></returns>
        doBLS050GetTbt_BillingTargetForView BLS050_GetTbt_BillingTargetForView(string strBillingTargetCode);
        /// <summary>
        /// To update first issue
        /// </summary>
        /// <param name="DocumentNo"></param>
        /// <param name="BatchDate"></param>
        /// <returns></returns>
        List<tbt_Invoice> UpdateFirstIssue(string DocumentNo,string DocumentOCC, DateTime BatchDate, string UpdateBy);

        /// <summary>
        /// Get billing detail by primary key
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <param name="billingDetailNo"></param>
        /// <returns></returns>
        List<doGetBillingDetailOfInvoice> GetBillingDetailByKey(string contractCode, string billingOCC, Nullable<int> billingDetailNo,string C_CURRENCY_LOCAL, string C_CURRENCY_US);

        /// <summary>
        /// Get AutoTransferBankAccount data by contract
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<tbt_AutoTransferBankAccount> GetAutoTransferBankAccountByContract(string contractCode, string billingOCC);

        /// <summary>
        /// Get credit card data by contract
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="billingOCC"></param>
        /// <returns></returns>
        List<tbt_CreditCard> GetCreditCardByContract(string contractCode, string billingOCC);

        /// <summary>
        /// Insert data to table tbt_DepositFee
        /// </summary>
        /// <param name="xmlTbt_Depositfee"></param>
        /// <returns></returns>
        List<tbt_Depositfee> InsertTbt_Depositfee(string xmlTbt_Depositfee);


        // Add By Sommai P., Nov 11, 2013
        /// <summary>
        /// get billing basic by credit note no.
        /// </summary>
        tbt_BillingBasic GetBillingBasicByCreditNoteNo(string CreditNoteNo);

        /// <summary>
        /// Insert cancel refund deposit fee transaction after cancel credit note.
        /// </summary>
        bool InsertDepositFeeCancelRefund(string ContractCode, string BillingOCC, string CreditNoteNo, decimal CancelAmount, decimal CancelAmountUsd, string CancelAmountCurrencyType);

        //End Add

        // Akat K. 2014-05-21 Update Billing Basic
        /// <summary>
        /// get billing basic by credit note no.
        /// </summary>
        tbt_BillingBasic UpdateDebtTracingOffice(string billingTargetCode, string billingOfficeCode);

        List<string> GetTbt_InvoiceReprint();

        /// <summary>
        /// GEt invoice issue list for generate csv file of BLR060
        /// </summary>
        /// <param name="pdatGenerateDateFrom"></param>
        /// <param name="pdatGenerateDateTo"></param>
        /// <returns></returns>
        List<doGetRptInvoiceIssueList> GetRptInvoiceIssueList(Nullable<System.DateTime> pdatGenerateDateFrom, Nullable<System.DateTime> pdatGenerateDateTo);
    }
}
