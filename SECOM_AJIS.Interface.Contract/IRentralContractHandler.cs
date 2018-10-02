using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.DataEntity.Contract 
{
    public interface IRentralContractHandler
    {
        /// <summary>
        /// Get rental contract data
        /// </summary>
        /// <returns></returns>
        dsRentalContractData GetRentalContractData();

        /// <summary>
        /// Get rental contract basic data
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="strUserCode"></param>
        /// <returns></returns>
        List<tbt_RentalContractBasic> GetTbt_RentalContractBasic(string contractCode ,string strUserCode);

        /// <summary>
        /// Get rental contract basic for view
        /// </summary>
        /// <param name="pchvContractCode"></param>
        /// <returns></returns>
        List<dtTbt_RentalContractBasicForView> GetTbt_RentalContractBasicForView(string pchvContractCode);

        /// <summary>
        /// Get rental contract basic for view
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <returns></returns>
        List<dtTbt_RentalSecurityBasicForView> GetTbt_RentalSecurityBasicForView(string pContractCode ,string pOCC);

        /// <summary>
        /// Get relate contract list
        /// </summary>
        /// <param name="pchrRelationType"></param>
        /// <param name="pchvstrContractCode"></param>
        /// <param name="pchrOCC"></param>
        /// <returns></returns>
        List<dtRelatedContract> GetRelatedContractList(string pchrRelationType, string pchvstrContractCode, string pchrOCC);

        /// <summary>
        /// Get maintenance detail for view data
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="oCC"></param>
        /// <returns></returns>
        List<dtTbt_RentalMaintenanceDetailsForView> GetTbt_RentalMaintenanceDetailsForView(string contractCode, string oCC);

        /// <summary>
        /// Get cancel contract memo detail for view
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="oCC"></param>
        /// <returns></returns>
        List<dtTbt_CancelContractMemoDetailForView> GetTbt_CancelContractMemoDetailForView(string contractCode, string oCC);

        /// <summary>
        /// Get rental operation type list for view data
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <returns></returns>
        List<dtTbt_RentalOperationTypeListForView> GetTbt_RentalOperationTypeListForView(string pContractCode, string pOCC);

        /// <summary>
        /// Get rental instrument subcontractor list for view data
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <returns></returns>
        List<dtTbt_RentalInstSubContractorListForView> GetTbt_RentalInstSubContractorListForView(string pContractCode, string pOCC);

        /// <summary>
        /// Get rental sentry guard for view
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <returns></returns>
        List<dtTbt_RentalSentryGuardForView> GetTbt_RentalSentryGuardForView(string pContractCode, string pOCC);

        /// <summary>
        /// Get be detail for view data
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <returns></returns>
        List<dtTbt_RentalBEDetailsForView> GetTbt_RentalBEDetailsForView(string pContractCode, string pOCC);

        /// <summary>
        /// Get rental instrument detail list for view data
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <param name="pInstrumentCode"></param>
        /// <param name="pInstrumentTypeCode"></param>
        /// <returns></returns>
        List<dtTbt_RentalInstrumentDetailsListForView> GetTbt_RentalInstrumentDetailsListForView(string pContractCode, string pOCC, string pInstrumentCode, string pInstrumentTypeCode);

        /// <summary>
        /// Get rental sentry guard detail list for view data
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <param name="pSequenceNo"></param>
        /// <returns></returns>
        List<dtTbt_RentalSentryGuardDetailsListForView> GetTbt_RentalSentryGuardDetailsListForView(string pContractCode, string pOCC, Nullable<int> pSequenceNo);

        /// <summary>
        /// Get sale instrument subcontractor list for view data
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <param name="pSubcontractorCode"></param>
        /// <returns></returns>
        List<dtTbt_SaleInstSubcontractorListForView> GetTbt_SaleInstSubcontractorListForView(string pContractCode, string pOCC, string pSubcontractorCode);
       
        //List<dtSaleInstruDetailListForView> GetSaleInstruDetailListForView(string pContractCode, string pOCC);

        /// <summary>
        /// Get rental security basic data
        /// </summary>
        /// <param name="pchvContractCode"></param>
        /// <param name="pchrOCC"></param>
        /// <returns></returns>
        List<tbt_RentalSecurityBasic> GetTbt_RentalSecurityBasic(string pchvContractCode, string pchrOCC);

        /// <summary>
        /// Get rental BE detail data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_RentalBEDetails> GetTbt_RentalBEDetails(string strContractCode, string strOCC);

        /// <summary>
        /// Get rental instrument detail data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_RentalInstrumentDetails> GetTbt_RentalInstrumentDetails(string strContractCode, string strOCC);

        /// <summary>
        ///  Get rental instrument subcontractor data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_RentalInstSubcontractor> GetTbt_RentalInstSubContractor(string strContractCode, string strOCC);

        /// <summary>
        /// Get rental maintenance detail data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_RentalMaintenanceDetails> GetTbt_RentalMaintenanceDetails(string strContractCode, string strOCC);

        /// <summary>
        /// Get rental operation type data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_RentalOperationType> GetTbt_RentalOperationType(string strContractCode, string strOCC);

        /// <summary>
        /// Get rental sentry guard data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_RentalSentryGuard> GetTbt_RentalSentryGuard(string strContractCode, string strOCC);

        /// <summary>
        /// Get rental sentry guard detail data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_RentalSentryGuardDetails> GetTbt_RentalSentryGuardDetails(string strContractCode, string strOCC);

        /// <summary>
        ///  Get cancel contract memo data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_CancelContractMemo> GetTbt_CancelContractMemo(string strContractCode, string strOCC);

        /// <summary>
        /// Get cancel contract memo detail data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_CancelContractMemoDetail> GetTbt_CancelContractMemoDetail(string strContractCode, string strOCC);

        /// <summary>
        /// Getting last occurrence of implemented contract. If there is no implemented occurrence, return as null
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        string GetLastImplementedOCC(string strContractCode);

        /// <summary>
        /// Getting last occurrence of unimplemented contract. If there is no unimplemented occurrence, return null
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        string GetLastUnimplementedOCC(string strContractCode);

        /// <summary>
        /// Getting previous OCC number
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        string GetPreviousImplementedOCC(string contractCode, string occ);

        /// <summary>
        /// Getting previous OCC number
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        string GetPreviousUnimplementedOCC(string contractCode, string occ);

        /// <summary>
        /// Getting next OCC number
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occ"></param>
        /// <returns></returns>
        string GetNextImplementedOCC(string contractCode, string occ);
        //List<string> GetContractCounterNo(string strContractCode, string strLastOCC);

        /// <summary>
        /// Get the contract fee before change fee
        /// </summary>
        /// <param name="paramContractCode"></param>
        /// <param name="paramOCC"></param>
        /// <param name="dsRentalContract"></param>
        /// <returns></returns>
        decimal? GetContractFeeBeforeChange(string paramContractCode, string paramOCC, dsRentalContractData dsRentalContract);
          
        /// <summary>
        /// To check first installation flag
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        bool IsFirstInstallComplete(string strContractCode);

        /// <summary>
        /// Getting all part of specified contract for creaing a new occurrence or else
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        dsRentalContractData GetEntireContract(string strContractCode, string strOCC);
        
        /// <summary>
        /// Insert entire contract data
        /// </summary>
        /// <param name="dsData"></param>
        /// <returns></returns>
        dsRentalContractData InsertEntireContract(dsRentalContractData dsData);

        /// <summary>
        /// Insert entire contract data for CTS010
        /// </summary>
        /// <param name="dsData"></param>
        void InsertEntireContractForCTS010(dsRentalContractData dsData);

        dsRentalContractData DeleteEntireOCC(string strContractCode, string strOCC, DateTime dtRentalContractBasicUpdateDate);

        /// <summary>
        /// Replace the summary fields in contract basic with current OCC.
        /// </summary>
        /// <param name="dsRental"></param>
        /// <returns></returns>
        dsRentalContractData UpdateSummaryFields(ref dsRentalContractData dsRental);

        /// <summary>
        /// Update data in case new/add sale and will insert new occurrence in case other installation type.
        /// </summary>
        /// <param name="doComplete"></param>
        void CompleteInstallation(doCompleteInstallationData doComplete);
                
        /// <summary>
        /// To generate contract occurrence
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="bImplementFlag"></param>
        /// <returns></returns>
        string GenerateContractOCC(string strContractCode, bool bImplementFlag);

        /// <summary>
        /// To generate contract counter
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        int GenerateContractCounter(string strContractCode);

        /// <summary>
        /// Auto renew contract
        /// </summary>
        void AutoRenew();

        /// <summary>
        /// Get contract data where expire next month of BatchDate
        /// </summary>
        /// <param name="BatchDate"></param>
        /// <returns></returns>
        List<doContractAutoRenew> GetContractExpireNextMonth(DateTime BatchDate);

        /// <summary>
        /// For registering change expected operation date of rental contract
        /// </summary>
        /// <param name="dsRental"></param>
        /// <returns></returns>
        int RegisterExpectedOperationDate(dsRentalContractData dsRental);
        //bool RegisterChangePlan(dsRentalContractData dsRentalContract,dsQuotationData dsQuotation, List<dtBillingTempChangePlanData> listBillingTemp,List<dtBillingClientData> listBillingClient, bool contractDurationChangeFlag);

        /// <summary>
        /// For register change plan of rental contract
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <param name="dsQuotation"></param>
        /// <param name="newItemList"></param>
        /// <param name="updateItemList"></param>
        /// <param name="deleteItemList"></param>
        /// <param name="contractDurationChangeFlag"></param>
        /// <param name="lastOCCForBilling"></param>
        /// <returns></returns>
        bool RegisterChangePlan(dsRentalContractData dsRentalContract, dsQuotationData dsQuotation, List<dtBillingTemp_SetItem> newItemList, List<dtBillingTemp_SetItem> updateItemList, List<dtBillingTemp_SetItem> deleteItemList, bool contractDurationChangeFlag, string lastOCCForBilling);

        /// <summary>
        /// For register maintain contract data (CP-34) (Correct)
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <param name="isSendNotifyEmail"></param>
        /// <returns></returns>
        bool RegisterCP34Correct(dsRentalContractData dsRentalContract, bool isSendNotifyEmail);

        /// <summary>
        /// For register maintain contract data (CP-34) (Delete)
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <returns></returns>
        bool RegisterCP34Delete(dsRentalContractData dsRentalContract);

        /// <summary>
        /// For register maintain contract data (CP-34) (Insert)
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <returns></returns>
        bool RegisterCP34Insert(dsRentalContractData dsRentalContract);

        /// <summary>
        /// Replace contract data with quotation data. Using when create contract or change contract.
        /// This method can be run on client.
        /// </summary>
        /// <param name="dsQuotation"></param>
        /// <param name="dsRentalContract"></param>
        void MapFromQuotation(dsQuotationData dsQuotation, ref dsRentalContractData dsRentalContract);

        /// <summary>
        /// Replace contract data with quotation data. Using when create contract or change contract.
        /// This method can be run on client.
        /// </summary>
        /// <param name="dsQuotation"></param>
        /// <param name="dsRentalContract"></param>
        /// <param name="needSumInstrumentQty"></param>
        void MapFromQuotation(dsQuotationData dsQuotation, ref dsRentalContractData dsRentalContract, bool needSumInstrumentQty);

        /// <summary>
        /// For cancel unoperated contract
        /// </summary>
        /// <param name="dsRental"></param>
        void CancelUnoperatedContract(dsRentalContractData dsRental);

        /// <summary>
        /// Get last cancel contract quotation data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        dsCancelContractQuotation GetLastCancelContractQuotation(string strContractCode);

        //bool RegisterChangeContractFee(dsRentalContractData dsRentalContract, List<dtEmailAddress> listDTEmailAddress, List<dtBillingTempChangePlanData> listDTBillingTempChangePlan, List<dtBillingClientData> listDTBillingClient);

        /// <summary>
        /// For register change contract fee of rental contract
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <param name="listDTEmailAddress"></param>
        /// <param name="newItemList"></param>
        /// <param name="updateItemList"></param>
        /// <param name="deleteItemList"></param>
        /// <param name="changeFeeDate"></param>
        /// <returns></returns>
        int RegisterChangeContractFee(dsRentalContractData dsRentalContract, List<dtEmailAddress> listDTEmailAddress, List<dtBillingTemp_SetItem> newItemList, List<dtBillingTemp_SetItem> updateItemList, List<dtBillingTemp_SetItem> deleteItemList, DateTime? changeFeeDate, decimal? ChangeContractFee);

        /// <summary>
        /// For registering instrument modification of rental contract
        /// </summary>
        /// <param name="dsQuotation"></param>
        /// <param name="dsRentalContract"></param>
        void RegisterModifyInstrument(dsQuotationData dsQuotation, dsRentalContractData dsRentalContract);

        /// <summary>
        /// For register maintain contract data (CP-33)
        /// </summary>
        /// <param name="dsRentalContract"></param>
        /// <param name="listRelationType"></param>
        /// <param name="listContractCode"></param>
        /// <param name="isUpdateRemovalFeeToBillingTemp"></param>
        /// <param name="isGenerateMAScheduleAgain"></param>
        void RegisterCP33(dsRentalContractData dsRentalContract, List<tbt_RelationType> listRelationType, List<string> listContractCode, bool isUpdateRemovalFeeToBillingTemp, bool isGenerateMAScheduleAgain, bool isUpdateBilling); //Modify (Add isUpdateBilling) by Jutarat A. on 18102013

        /// <summary>
        /// Get contract doc template data
        /// </summary>
        /// <param name="strDocumentCode"></param>
        /// <returns></returns>
        List<tbs_ContractDocTemplate> GetTbsContractDocTemplate(string strDocumentCode);

        /// <summary>
        /// Gete cancel contract quotationdata
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        dsCancelContractQuotation GetCancelContractQuotation(string strContractCode, string strOCC);

        /// <summary>
        /// Create contract memo when register cancel contract
        /// </summary>
        /// <param name="dsCancelContract"></param>
        /// <returns></returns>
        dsCancelContractQuotation CreateCancelContractMemo(dsCancelContractQuotation dsCancelContract);

        /// <summary>
        /// Delete cancel contract memo data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOcc"></param>
        /// <returns></returns>
        bool DeleteCancelContractMemo(string strContractCode, string strOcc);

        /// <summary>
        /// Create contract doc data
        /// </summary>
        /// <param name="dsContractDoc"></param>
        /// <returns></returns>
        dsContractDocData CreateContractDocData(dsContractDocData dsContractDoc);

        /// <summary>
        /// Get rental contract baisc information data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<doRentalContractBasicInformation> GetRentalContractBasicInformation(string strContractCode);

        /// <summary>
        /// Get the billing basic which pay the maintenance fee in result based. Null is returned if there is many billing target or no billing target
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        tbt_BillingBasic GetBillingBasicForMAResultBasedFeePayment(string ContractCode);

        /// <summary>
        /// In case rental contract start service, calculate all fee of umimplement 
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<doSummaryFee> SumFeeUnimplementData(string strContractCode);

        /// <summary>
        /// Update rental security basic data
        /// </summary>
        /// <param name="dotbt_RentalSecurityBasic"></param>
        /// <returns></returns>
        List<tbt_RentalSecurityBasic> UpdateTbt_RentalSecurityBasic(tbt_RentalSecurityBasic dotbt_RentalSecurityBasic);

        /// <summary>
        /// Update rental security basic core data
        /// </summary>
        /// <param name="dotbt_RentalContractBasic"></param>
        /// <returns></returns>
        List<tbt_RentalContractBasic> UpdateTbt_RentalContractBasicCore(tbt_RentalContractBasic dotbt_RentalContractBasic);

        /// <summary>
        /// Get rental contract basic information for display on Installation page
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strBuildingType"></param>
        /// <returns></returns>
        dtRentalContractBasicForInstall GetRentalContractBasicDataForInstall(string strContractCode, string strBuildingType);

        /// <summary>
        /// Get rental contract basic information
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strUserCode"></param>
        /// <returns></returns>
        List<dtRentalContractBasicForView> GetRentalContractBasicForView(string strContractCode, string strUserCode);

        /// <summary>
        /// Check cancel contract before start service
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        bool CheckCancelContractBeforeStartService(string strContractCode);

        /// <summary>
        /// Check rental contract basic is CP-12 is registered
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strLastImplementOCC"></param>
        /// <returns></returns>
        bool CheckCP12(string strContractCode,string strLastImplementOCC);

        /// <summary>
        /// Get relation type data
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <param name="pRelatedContractCode"></param>
        /// <returns></returns>
        List<tbt_RelationType> GetTbt_RelationType(string pContractCode, string pOCC, string pRelatedContractCode);

        /// <summary>
        /// Get max update date of MA target contract for checking with the create date of quotation when register change plan
        /// </summary>
        /// <param name="paramMAContractCode"></param>
        /// <param name="paramOCC"></param>
        /// <returns></returns>
        List<DateTime?> GetMaxUpdateDateOfMATargetContract(string paramMAContractCode, string paramOCC);

        /// <summary>
        /// Get billing temp from both billing temp table and billing basic
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<dtBillingTempChangePlanData> GetBillingTempForChangePlan(string strContractCode, string strOCC);

        /// <summary>
        /// Get billing temp from both billing temp table and billing basic
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<dtBillingTempChangeFeeData> GetBillingTempForChangeFee(string strContractCode);

        /// <summary>
        /// Getting installation compete date when installation type is remove all.
        /// </summary>
        /// <param name="contractCode"></param>
        /// <returns></returns>
        DateTime? GetRemovalInstallCompleteDate(string contractCode);

        List<dtQuotationNoData> GetQuotationNo(string quotationTargetCode, string alphabet);
    }
}
