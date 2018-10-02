using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public interface IQuotationHandler
    {
        #region Get Methods

        /// <summary>
        /// Search quotation target data (Call stored procedure: sp_QU_SearchQuotationTargetList)
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        List<dtSearchQuotationTargetListResult> SearchQuotationTargetList(doSearchQuotationTargetListCondition Cond);
        /// <summary>
        /// Search quotation data (Call stored procedure: sp_QU_SearchQuotationList)
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        List<dtSearchQuotationListResult> SearchQuotationList(doSearchQuotationListCondition Cond);
        /// <summary>
        /// Get data from tbt_QuotationTarget (Call stored procedure: sp_QU_GetTbt_QuotationTarget)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<tbt_QuotationTarget> GetTbt_QuotationTarget(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get data from tbt_QuotationTarget (Call stored procedure: sp_QU_GetTbt_QuotationTarget)
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        List<tbt_QuotationTarget> GetTbt_QuotationTargetByContractCode(string contractCode);
        /// <summary>
        /// Get quotation data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        dsQuotationData GetQuotationData(doGetQuotationDataCondition cond);
        /// <summary>
        /// Delete quotation data (Call stored procedure: sp_QU_DeleteQuotation)
        /// </summary>
        /// <returns></returns>
        List<dtBatchProcessResult> DeleteQuotation();
        /// <summary>
        /// Get quotation basic data (Call stroed procedure: sp_QU_GetQuotationBasicData)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<tbt_QuotationBasic> GetQuotationBasicData(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get instrument detail data (Call stored procedure: sp_QU_GetInstrumentDetail)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<doInstrumentDetail> GetInstrumentDetail(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get data from tbt_QuotationCustomer (Call stored procedure: sp_QU_GetTbt_QuotationCustomer)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<tbt_QuotationCustomer> GetTbt_QuotationCustomer(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get data from tbt_QuotationSite (Call stored procedure: sp_QU_GetTbt_QuotationSite)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<tbt_QuotationSite> GetTbt_QuotationSite(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get data from tbt_QuotationInstrumentDetails (Call stored procedure: sp_QU_GetTbt_QuotationInstrumentDetials)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<tbt_QuotationInstrumentDetails> GetTbt_QuotationInstrumentDetails(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get data from tbt_QuotationOperationType (Call stored procedure: sp_QU_GetTbt_QuotationOperationType)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<tbt_QuotationOperationType> GetTbt_QuotationOperationType(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get data from tbt_QuotationFacilityDetails (Call stored procedure: sp_QU_GetTbt_QuotationFacilityDetails)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<tbt_QuotationFacilityDetails> GetTbt_QuotationFacilityDetails(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get data from tbt_QuotationSentryGuardDetails (Call stored procedure: sp_QU_GetTbt_QuotationSentryGuardDetails)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<tbt_QuotationSentryGuardDetails> GetTbt_QuotationSentryGuardDetails(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get data from tbt_QuotationBeatGuardDetails (Call stored procedure: sp_QU_GetTbt_QuotationBeatGuardDetails)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<tbt_QuotationBeatGuardDetails> GetTbt_QuotationBeatGuardDetails(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get quotation operation type data (Call stored procedure: sp_QU_GetQuotationOperationType
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<doQuotationOperationType> GetQuotationOperationType(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get facility detail data (Call stored procedure: sp_QU_GetFacilityDetail)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<doFacilityDetail> GetFacilityDetail(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get beat guard detail data (Call stored procedure: sp_QU_GetBeatGuardDetail)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        doBeatGuardDetail GetBeatGuardDetail(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get sentry guard detail data (Call stored procedure: sp_QU_GetSentryGuardDetail)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<doSentryGuardDetail> GetSentryGuardDetail(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get data from tbt_QuotationMaintenanceLinkage (Call stored procedure: sp_QU_GetTbt_QuotationMaintenanceLinkage)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<tbt_QuotationMaintenanceLinkage> GetTbt_QuotationMaintenanceLinkage(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get sale quotation data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        doSaleQuotationData GetSaleQuotationData(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get rental quotation data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        doRentalQuotationData GetRentalQuotationData(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get quotation header data
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        doQuotationHeaderData GetQuotationHeaderData(doGetQuotationDataCondition cond);
        /// <summary>
        /// Get default instrument data (Call stored procedure: sp_QU_GetDefaultInstrument)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<doDefaultInstrument> GetDefaultInstrument(doDefaultInstrumentCondition cond);
        /// <summary>
        /// Get default facility data (Call stroed procedure: sp_QU_GetDefaultFacility)
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        List<doDefaultFacility> GetDefaultFacility(doDefaultFacilityCondition cond);
        /// <summary>
        /// Check site data is aleady used (Call stored procedure: sp_QU_IsUsedSite)
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        bool IsUsedSiteData(string siteCode);
        /// <summary>
        /// Check customer data is aleady used (Call stored procedure: sp_QU_IsUsedCustomer)
        /// </summary>
        /// <param name="custCode"></param>
        /// <returns></returns>
        bool IsUsedCustomerData(string custCode);
        /// <summary>
        /// Get data from tbt_QuotationInstallationDetail (Call stored procedure: sp_QU_GetTbt_QuotationInstallationDetail)
        /// </summary>
        /// <param name="quotationTargetCode"></param>
        /// <param name="alphabet"></param>
        /// <returns></returns>
        List<tbt_QuotationInstallationDetail> GetTbt_QuotationInstallationDetail(string quotationTargetCode, string alphabet);

        doQuotationOneShotFlag GetQuotationOneShotFlagData(doGetQuotationDataCondition cond);

        #endregion
        #region Initial Methods

        /// <summary>
        /// Generate Quotation
        /// </summary>
        /// <param name="GenerateData"></param>
        /// <returns></returns>
        string GenerateQuotation(dsGenerateData GenerateData);
        /// <summary>
        /// Generate quotation target code
        /// </summary>
        /// <param name="strProductTypeCode"></param>
        /// <returns></returns>
        string GenerateQuotationTargetCode(string productTypeCode);
        /// <summary>
        /// Generate quotation alphabet
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <returns></returns>
        string GenerateQuotationAlphabet(string strQuotationTargetCode);
        /// <summary>
        /// Get linkage sale contract data
        /// </summary>
        /// <param name="strSaleContractCode"></param>
        /// <returns></returns>
        doLinkageSaleContractData GetLinkageSaleContractData(string strSaleContractCode);
        /// <summary>
        /// Initial linkage sale contract data
        /// </summary>
        /// <param name="doSaleContractData"></param>
        /// <returns></returns>
        doLinkageSaleContractData InitLinkageSaleContractData(doSaleContractData doSaleContractData);

        #endregion
        #region Insert Methods

        /// <summary>
        /// Create quotation target data
        /// </summary>
        /// <param name="doRegQuoTarData"></param>
        /// <returns></returns>
        int CreateQuotationTargetData(doRegisterQuotationTargetData doRegQuoTarData);
        /// <summary>
        /// Insert quotation beat guard detail data (Call stored procedure: sp_QU_InsertQuotationBeatGuardDetails)
        /// </summary>
        /// <param name="tbt_QuotationBeatGuardDetails"></param>
        /// <returns></returns>
        int InsertQuotationBeatGuardDetails(tbt_QuotationBeatGuardDetails tbt_QuotationBeatGuardDetails);
        /// <summary>
        /// Insert quotation operation type data (Call stored procedure: sp_QU_InsertQuotationOperationType)
        /// </summary>
        /// <param name="doTbt_QuotationOperationType"></param>
        /// <returns></returns>
        int InsertQuotationOperationType(tbt_QuotationOperationType doTbt_QuotationOperationType);
        /// <summary>
        /// Insert quotation instrument details data (Call stored procedure: sp_QU_InsertQuotationInstrumentDetails)
        /// </summary>
        /// <param name="doTbt_QuotationInstrumentDetails"></param>
        /// <returns></returns>
        int InsertQuotationInstrumentDetails(tbt_QuotationInstrumentDetails doTbt_QuotationInstrumentDetails);
        /// <summary>
        /// Insert quotation facility details data (Call stored procedure: sp_QU_InsertQuotationFacilityDetails)
        /// </summary>
        /// <param name="doTbt_QuotationFacilityDetails"></param>
        /// <returns></returns>
        int InsertQuotationFacilityDetails(tbt_QuotationFacilityDetails doTbt_QuotationFacilityDetails);
        /// <summary>
        /// Insert quotation basic data (Call stored procedure: sp_QU_InsertQuotationBasic)
        /// </summary>
        /// <param name="doTbt_QuotationBasic"></param>
        /// <returns></returns>
        int InsertQuotationBasic(tbt_QuotationBasic doTbt_QuotationBasic);
        /// <summary>
        /// Insert quotation target data (Call stored procedure: sp_QU_InsertQuotationTarget)
        /// </summary>
        /// <param name="doTbt_QuotationTarget"></param>
        /// <returns>List<tbt_QuotationTarget> </returns>
        int InsertQuotationTarget(tbt_QuotationTarget doTbt_QuotationTarget);
        /// <summary>
        /// Insert quotation customer data (Call stored procedure: sp_QU_InsertQuotationCustomer)
        /// </summary>
        /// <param name="doTbt_QuotationCustomer"></param>
        /// <returns></returns>
        int InsertQuotationCustomer(tbt_QuotationCustomer doTbt_QuotationCustomer);
        /// <summary>
        /// Insert quotation site data (Call stored procedure: sp_QU_InsertQuotationSite)
        /// </summary>
        /// <param name="doRegQuoTarData"></param>
        /// <returns></returns>
        int InsertQuotationSite(tbt_QuotationSite doTbt_QuotationSite);
        /// <summary>
        /// Insert quotation sentry guard details data (Call stored procedure: sp_QU_InsertQuotationSentryGuardDetails)
        /// </summary>
        /// <param name="tbt_QuotationSentryGuardDetails"></param>
        /// <returns></returns>
        int InsertQuotationSentryGuardDetails(tbt_QuotationSentryGuardDetails tbt_QuotationSentryGuardDetails);
        /// <summary>
        /// Insert quotation maintenance linkage data (Call stored procedure: sp_QU_InsertQuotationMaintenanceLinkage)
        /// </summary>
        /// <param name="tbt_QuotationMaintenanceLinkage"></param>
        /// <returns></returns>
        int InsertQuotationMaintenanceLinkage(tbt_QuotationMaintenanceLinkage tbt_QuotationMaintenanceLinkage);
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int InsertQuotationInstallationDetail(List<tbt_QuotationInstallationDetail> datalist);

        #endregion
        #region Update Methods

        /// <summary>
        /// Lock quotation
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <param name="strAlphabet"></param>
        /// <param name="strLockStyleCode"></param>
        /// <returns></returns>
        bool LockQuotation(string strQuotationTargetCode, string strAlphabet, string strLockStyleCode);
        /// <summary>
        /// Update quotation target data (Call stored procedure: sp_QU_UpdateQuotationTarget)
        /// </summary>
        /// <param name="UpdateQuotationTargetData"></param>
        /// <returns></returns>
        int UpdateQuotationTarget(doUpdateQuotationTargetData UpdateQuotationTargetData);
        /// <summary>
        /// Update quotation data
        /// </summary>
        /// <param name="uData"></param>
        /// <returns></returns>
        int UpdateQuotationData(doUpdateQuotationData uData);
        /// <summary>
        /// Update quotation installation detail (Call stored procedure: sp_QU_UpdateTbt_QuotationInstallationDetail)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int UpdateQuotationInstallationDetail(List<tbt_QuotationInstallationDetail> datalist);
        #endregion

        #region QUP030 Mock Lock

        /// <summary>
        /// Lock all quotation (Call stored procedure: sp_QU_LockAll)
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <returns></returns>
        List<tbt_QuotationBasic> LockAll(string strQuotationTargetCode);
        /// <summary>
        /// Lock backward quotation (Call stored procedure: sp_QU_LockBackward)
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <param name="strAlphabet"></param>
        /// <returns></returns>
        List<tbt_QuotationBasic> LockBackward(string strQuotationTargetCode, string strAlphabet);
        /// <summary>
        /// Lock individual quotation (Call stored procedure: sp_QU_LockIndividual)
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <param name="strAlphabet"></param>
        /// <returns></returns>
        List<tbt_QuotationBasic> LockIndividual(string strQuotationTargetCode, string strAlphabet);

        #endregion

        /// <summary>
        /// Convert quotation parent to child instrument
        /// </summary>
        /// <param name="pQuotationTargetCode"></param>
        /// <param name="pAlphabet"></param>
        /// <returns></returns>
        List<tbt_QuotationInstrumentDetails> ConvertQuotationParentToChildInstrument(string pQuotationTargetCode, string pAlphabet);

    }
}
