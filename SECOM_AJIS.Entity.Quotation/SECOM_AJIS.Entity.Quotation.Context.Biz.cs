//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public partial class BizQUDataEntities
    {
        #region Methods
    		public virtual List<dtSearchQuotationTargetListResult> SearchQuotationTargetList(string quotationTargetCode, string productTypeCode, string quotationOfficeCode, string operationOfficeCode, string contractTargetCode, string contractTargetName, string contractTargetAddr, string siteCode, string siteName, string siteAddr, string empNo, string empName, Nullable<System.DateTime> quotationDateFrom, Nullable<System.DateTime> quotationDateTo, string c_CUST_PART_TYPE_CONTRACT_TARGET, string c_TARGET_CODE_TYPE_QTN_CODE, string c_CONTRACT_TRANS_STATUS_CONTRACT_APP, string c_TARGET_CODE_TYPE_CONTRACT_CODE, string xmlOfficeData)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.SearchQuotationTargetList(quotationTargetCode, productTypeCode, quotationOfficeCode, operationOfficeCode, contractTargetCode, contractTargetName, contractTargetAddr, siteCode, siteName, siteAddr, empNo, empName, quotationDateFrom, quotationDateTo, c_CUST_PART_TYPE_CONTRACT_TARGET, c_TARGET_CODE_TYPE_QTN_CODE, c_CONTRACT_TRANS_STATUS_CONTRACT_APP, c_TARGET_CODE_TYPE_CONTRACT_CODE, xmlOfficeData).ToList();
    		}
    		public virtual List<dtSearchQuotationListResult> SearchQuotationList(string pC_CUST_PART_TYPE_CONTRACT_TARGET, string pQuotationTargetCode, string pAlphabet, string pProductTypeCode, string pLockStatus, string pQuotationOfficeCode, string pOperationOfficeCode, string pContractTargetCode, string pContractTargetName, string pContractTargetAddr, string pSiteCode, string pSiteName, string pSiteAddr, string pEmpNo, string pEmpName, Nullable<System.DateTime> pQuotationDateFrom, Nullable<System.DateTime> pQuotationDateTo, string pServiceTypeCode, string pTargetCodeTypeCode, string pContractTransferStatus)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.SearchQuotationList(pC_CUST_PART_TYPE_CONTRACT_TARGET, pQuotationTargetCode, pAlphabet, pProductTypeCode, pLockStatus, pQuotationOfficeCode, pOperationOfficeCode, pContractTargetCode, pContractTargetName, pContractTargetAddr, pSiteCode, pSiteName, pSiteAddr, pEmpNo, pEmpName, pQuotationDateFrom, pQuotationDateTo, pServiceTypeCode, pTargetCodeTypeCode, pContractTransferStatus).ToList();
    		}
    		public virtual List<tbt_QuotationTarget> GetTbt_QuotationTarget(string quotationTargetCode, string serviceTypeCode, string targetCodeTypeCode, string contractCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetTbt_QuotationTarget(quotationTargetCode, serviceTypeCode, targetCodeTypeCode, contractCode).ToList();
    		}
    		public virtual List<Nullable<int>> CountQuotationBasicSQL(string pchvQuotationTargetCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.CountQuotationBasicSQL(pchvQuotationTargetCode).ToList();
    		}
    		public virtual List<tbt_QuotationBasic> LockAll(string pchr_C_LOCK_STATUS_LOCK, string pchr_C_LOCK_STATUS_UNLOCK, Nullable<System.DateTime> pdtmProcessDateTime, string pchrEmpNo, string pchvQuotationTargetCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.LockAll(pchr_C_LOCK_STATUS_LOCK, pchr_C_LOCK_STATUS_UNLOCK, pdtmProcessDateTime, pchrEmpNo, pchvQuotationTargetCode).ToList();
    		}
    		public virtual List<tbt_QuotationBasic> LockBackward(string pchr_C_LOCK_STATUS_LOCK, string pchr_C_LOCK_STATUS_UNLOCK, Nullable<System.DateTime> pdtmProcessDateTime, string pchrEmpNo, string pchvQuotationTargetCode, string pchrAlphabet)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.LockBackward(pchr_C_LOCK_STATUS_LOCK, pchr_C_LOCK_STATUS_UNLOCK, pdtmProcessDateTime, pchrEmpNo, pchvQuotationTargetCode, pchrAlphabet).ToList();
    		}
    		public virtual List<tbt_QuotationBasic> LockIndividual(string pchvnQuotationTargetCode, string pchrAlphabet, string pchrC_LOCK_STATUS_LOCK, string pchr_C_LOCK_STATUS_UNLOCK, Nullable<System.DateTime> pdatProcessDateTime, string pchvEmpno)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.LockIndividual(pchvnQuotationTargetCode, pchrAlphabet, pchrC_LOCK_STATUS_LOCK, pchr_C_LOCK_STATUS_UNLOCK, pdatProcessDateTime, pchvEmpno).ToList();
    		}
    		public virtual List<tbt_QuotationBasic> GetQuotationBasicData(string pchvQuotationTargetCode, string pchrAlphabet)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetQuotationBasicData(pchvQuotationTargetCode, pchrAlphabet).ToList();
    		}
    		public virtual List<tbt_QuotationCustomer> GetTbt_QuotationCustomer(string quotationTargetCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetTbt_QuotationCustomer(quotationTargetCode).ToList();
    		}
    		public virtual List<tbt_QuotationSite> GetTbt_QuotationSite(string quotationTargetCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetTbt_QuotationSite(quotationTargetCode).ToList();
    		}
    		public virtual List<tbt_QuotationInstrumentDetails> GetTbt_QuotationInstrumentDetails(string quotationTargetCode, string alphabet)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetTbt_QuotationInstrumentDetails(quotationTargetCode, alphabet).ToList();
    		}
    		public virtual List<tbt_QuotationOperationType> GetTbt_QuotationOperationType(string quotationTargetCode, string alphabet)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetTbt_QuotationOperationType(quotationTargetCode, alphabet).ToList();
    		}
    		public virtual List<tbt_QuotationFacilityDetails> GetTbt_QuotationFacilityDetails(string quotationTargetCode, string alphabet)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetTbt_QuotationFacilityDetails(quotationTargetCode, alphabet).ToList();
    		}
    		public virtual List<tbt_QuotationSentryGuardDetails> GetTbt_QuotationSentryGuardDetails(string quotationTargetCode, string alphabet)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetTbt_QuotationSentryGuardDetails(quotationTargetCode, alphabet).ToList();
    		}
    		public virtual List<tbt_QuotationBeatGuardDetails> GetTbt_QuotationBeatGuardDetails(string quotationTargetCode, string alphabet)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetTbt_QuotationBeatGuardDetails(quotationTargetCode, alphabet).ToList();
    		}
    		public virtual List<tbt_QuotationMaintenanceLinkage> GetTbt_QuotationMaintenanceLinkage(string quotationTargetCode, string alphabet)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetTbt_QuotationMaintenanceLinkage(quotationTargetCode, alphabet).ToList();
    		}
    		public virtual List<doQuotationTarget> GetQuotationTarget(string pchr_C_ACQUISITION_TYPE, string pchr_C_MOTIVATION_TYPE, string pchrQuotationTargetCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetQuotationTarget(pchr_C_ACQUISITION_TYPE, pchr_C_MOTIVATION_TYPE, pchrQuotationTargetCode).ToList();
    		}
    		public virtual List<doQuotationSite> GetQuotationSite(string pchrQuotationTargetCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetQuotationSite(pchrQuotationTargetCode).ToList();
    		}
    		public virtual List<doQuotationCustomer> GetQuotationCustomer(string pchrQuotationTargetCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetQuotationCustomer(pchrQuotationTargetCode).ToList();
    		}
    		public virtual List<doInstrumentDetail> GetInstrumentDetail(string pchr_C_LINE_UP_TYPE, string pchr_C_PROD_TYPE_SALE, string pchrQuotationTargetCode, string pchrAlphabet, string pchrProductTypeCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetInstrumentDetail(pchr_C_LINE_UP_TYPE, pchr_C_PROD_TYPE_SALE, pchrQuotationTargetCode, pchrAlphabet, pchrProductTypeCode).ToList();
    		}
    		public virtual List<tbt_QuotationBasic> UpdateQuotationBasic(Nullable<System.DateTime> pdatProcessDateTime, string pchvEmpno, string pchrAlphabet, string pchrContractTransferStatus, string pchvnQuotationTargetCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.UpdateQuotationBasic(pdatProcessDateTime, pchvEmpno, pchrAlphabet, pchrContractTransferStatus, pchvnQuotationTargetCode).ToList();
    		}
    		public virtual List<tbt_QuotationTarget> UpdateQuotationTarget(string pchrQuotationOfficeCode, string pchrLastAlphabet, string pchrContractTransferStatus, string pchrContractCode, Nullable<System.DateTime> pchrTransferDate, string pchrTransferAlphabet, Nullable<System.DateTime> pdtmUpdateDate, string pchvUpdateBy, string pchvQuotationTargetCode, string operationOfficeCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.UpdateQuotationTarget(pchrQuotationOfficeCode, pchrLastAlphabet, pchrContractTransferStatus, pchrContractCode, pchrTransferDate, pchrTransferAlphabet, pdtmUpdateDate, pchvUpdateBy, pchvQuotationTargetCode, operationOfficeCode).ToList();
    		}
    		public virtual List<dtBatchProcessResult> DeleteQuotation(Nullable<bool> pbit_C_FLAG_ON, Nullable<bool> pbit_C_FLAG_OFF)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.DeleteQuotation(pbit_C_FLAG_ON, pbit_C_FLAG_OFF).ToList();
    		}
    		public virtual List<tbt_QuotationOperationType> InsertQuotationOperationType(string pchvQuotationTargetCode, string pchrAlphabet, string pchrOperationTypeCode, Nullable<System.DateTime> pdtmCreateDate, string pchvCreateBy, Nullable<System.DateTime> pdtmUpdateDate, string pchvUpdateBy)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.InsertQuotationOperationType(pchvQuotationTargetCode, pchrAlphabet, pchrOperationTypeCode, pdtmCreateDate, pchvCreateBy, pdtmUpdateDate, pchvUpdateBy).ToList();
    		}
    		public virtual List<tbt_QuotationInstrumentDetails> InsertQuotationInstrumentDetails(string pchvQuotationTargetCode, string pchrAlphabet, string pchvInstrumentCode, Nullable<int> pintInstrumentQty, Nullable<int> pintAddedQty, Nullable<int> pintRemovedQty, Nullable<System.DateTime> pdtmCreateDate, string pchvCreateBy, Nullable<System.DateTime> pdtmUpdateDate, string pchvUpdateBy)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.InsertQuotationInstrumentDetails(pchvQuotationTargetCode, pchrAlphabet, pchvInstrumentCode, pintInstrumentQty, pintAddedQty, pintRemovedQty, pdtmCreateDate, pchvCreateBy, pdtmUpdateDate, pchvUpdateBy).ToList();
    		}
    		public virtual List<tbt_QuotationFacilityDetails> InsertQuotationFacilityDetails(string pchvQuotationTargetCode, string pchrAlphabet, string pchvFacilityCode, Nullable<int> pintFacilityQty, Nullable<System.DateTime> pdtmCreateDate, string pchvCreateBy, Nullable<System.DateTime> pdtmUpdateDate, string pchvUpdateBy)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.InsertQuotationFacilityDetails(pchvQuotationTargetCode, pchrAlphabet, pchvFacilityCode, pintFacilityQty, pdtmCreateDate, pchvCreateBy, pdtmUpdateDate, pchvUpdateBy).ToList();
    		}
    		public virtual List<tbt_QuotationBasic> InsertQuotationBasic(string xml_doTbtQuotationBasic)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.InsertQuotationBasic(xml_doTbtQuotationBasic).ToList();
    		}
    		public virtual List<tbt_QuotationTarget> InsertQuotationTarget(string xml_QuotationTarget)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.InsertQuotationTarget(xml_QuotationTarget).ToList();
    		}
    		public virtual List<tbt_QuotationCustomer> InsertQuotationCustomer(string xml_doTbt_QuotationCustomer)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.InsertQuotationCustomer(xml_doTbt_QuotationCustomer).ToList();
    		}
    		public virtual List<tbt_QuotationSite> InsertQuotationSite(string xml_doTbt_QuotationSite)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.InsertQuotationSite(xml_doTbt_QuotationSite).ToList();
    		}
    		public virtual List<doQuotationOperationType> GetQuotationOperationType(string pchrQuotationTargetCode, string pchrAlphabet, string pcharC_OPERATION_TYPE)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetQuotationOperationType(pchrQuotationTargetCode, pchrAlphabet, pcharC_OPERATION_TYPE).ToList();
    		}
    		public virtual List<doFacilityDetail> GetFacilityDetail(string pchrQuotationTargetCode, string pchrAlphabet)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetFacilityDetail(pchrQuotationTargetCode, pchrAlphabet).ToList();
    		}
    		public virtual List<doBeatGuardDetail> GetBeatGuardDetail(string pchrQuotationTargetCode, string pchrAlphabet, string pcharC_NUM_OF_DATE)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetBeatGuardDetail(pchrQuotationTargetCode, pchrAlphabet, pcharC_NUM_OF_DATE).ToList();
    		}
    		public virtual List<doSentryGuardDetail> GetSentryGuardDetail(string pchrQuotationTargetCode, string pchrAlphabet, string pcharC_SG_TYPE)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetSentryGuardDetail(pchrQuotationTargetCode, pchrAlphabet, pcharC_SG_TYPE).ToList();
    		}
    		public virtual List<doDefaultInstrument> GetDefaultInstrument(string pchrProductCode, string pchrProductTypeCode, string c_LINE_UP_TYPE, string c_LINE_UP_TYPE_STOP_SALE, string c_LINE_UP_TYPE_LOGICAL_DELETE, string c_PROD_TYPE_SALE, string c_INST_TYPE_GENERAL, string c_EXPANSION_TYPE_PARENT, Nullable<bool> blnSaleFlag, Nullable<bool> blnRentalFlag)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetDefaultInstrument(pchrProductCode, pchrProductTypeCode, c_LINE_UP_TYPE, c_LINE_UP_TYPE_STOP_SALE, c_LINE_UP_TYPE_LOGICAL_DELETE, c_PROD_TYPE_SALE, c_INST_TYPE_GENERAL, c_EXPANSION_TYPE_PARENT, blnSaleFlag, blnRentalFlag).ToList();
    		}
    		public virtual List<tbt_QuotationBeatGuardDetails> InsertQuotationBeatGuardDetails(string xml_doTbt_QuotationSite)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.InsertQuotationBeatGuardDetails(xml_doTbt_QuotationSite).ToList();
    		}
    		public virtual List<tbt_QuotationSentryGuardDetails> InsertQuotationSentryGuardDetails(string pchrQuotationTargetCode, string pchrAlphabet, Nullable<int> pintRunningNo, string pchrSentryGuardTypeCode, Nullable<decimal> pdecNumOfDate, Nullable<System.TimeSpan> ptSecurityStartTime, Nullable<System.TimeSpan> ptSecurityFinishTime, Nullable<decimal> pdecWorkHourPerMonth, Nullable<decimal> pdecCostPerHour, Nullable<int> pintNumOfSentryGuard, Nullable<System.DateTime> pdtCreateDate, string pchrCreateBy, Nullable<System.DateTime> pdtUpdateDate, string pdtUpdateBy, Nullable<decimal> pdecCostPerHourUsd, string pdecCostPerCurrencyType)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.InsertQuotationSentryGuardDetails(pchrQuotationTargetCode, pchrAlphabet, pintRunningNo, pchrSentryGuardTypeCode, pdecNumOfDate, ptSecurityStartTime, ptSecurityFinishTime, pdecWorkHourPerMonth, pdecCostPerHour, pintNumOfSentryGuard, pdtCreateDate, pchrCreateBy, pdtUpdateDate, pdtUpdateBy, pdecCostPerHourUsd, pdecCostPerCurrencyType).ToList();
    		}
    		public virtual List<tbt_QuotationMaintenanceLinkage> InsertQuotationMaintenanceLinkage(string pchrQuotationTargetCode, string pchrAlphabet, string pchrContractCode, Nullable<System.DateTime> pdtCreateDate, string pchrCreateBy, Nullable<System.DateTime> pdtUpdateDate, string pchrUpdateBy)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.InsertQuotationMaintenanceLinkage(pchrQuotationTargetCode, pchrAlphabet, pchrContractCode, pdtCreateDate, pchrCreateBy, pdtUpdateDate, pchrUpdateBy).ToList();
    		}
    		public virtual List<doDefaultFacility> GetDefaultFacility(string pchrProductCode, string c_LINE_UP_TYPE_STOP_SALE, string c_LINE_UP_TYPE_LOGICAL_DELETE, string c_INST_TYPE_MONITOR)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetDefaultFacility(pchrProductCode, c_LINE_UP_TYPE_STOP_SALE, c_LINE_UP_TYPE_LOGICAL_DELETE, c_INST_TYPE_MONITOR).ToList();
    		}
    		public virtual List<Nullable<int>> IsUsedSite(string pchrSiteCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.IsUsedSite(pchrSiteCode).ToList();
    		}
    		public virtual List<Nullable<int>> IsUsedCustomer(string pchrCustCode)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.IsUsedCustomer(pchrCustCode).ToList();
    		}
    		public virtual List<tbt_QuotationInstrumentDetails> ConvertQuotationParentToChildInstrument(string pQuotationTargetCode, string pAlphabet)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.ConvertQuotationParentToChildInstrument(pQuotationTargetCode, pAlphabet).ToList();
    		}
    		public virtual List<tbt_QuotationInstallationDetail> GetTbt_QuotationInstallationDetail(string quotationTargetCode, string alphabet)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetTbt_QuotationInstallationDetail(quotationTargetCode, alphabet).ToList();
    		}
    		public virtual List<tbt_QuotationInstallationDetail> InsertTbt_QuotationInstallationDetail(string xml_Tbt_QuotationInstallationDetail)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.InsertTbt_QuotationInstallationDetail(xml_Tbt_QuotationInstallationDetail).ToList();
    		}
    		public virtual List<tbt_QuotationInstallationDetail> UpdateTbt_QuotationInstallationDetail(string xml_Tbt_QuotationInstallationDetail)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.UpdateTbt_QuotationInstallationDetail(xml_Tbt_QuotationInstallationDetail).ToList();
    		}
    		public virtual List<doQuotationOneShotFlag> GetQuotationOneShotFlag(string quotationTargetCode, string alphabet)
    		{
    			QUDataEntities context = new QUDataEntities();
    			return context.GetQuotationOneShotFlag(quotationTargetCode, alphabet).ToList();
    		}

        #endregion

    }
}
