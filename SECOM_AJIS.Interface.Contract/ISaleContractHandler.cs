using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Quotation;

namespace SECOM_AJIS.DataEntity.Contract
{
     public interface  ISaleContractHandler
     {
         /// <summary>
         /// Get sale basic for view data
         /// </summary>
         /// <param name="pchvContractCode"></param>
         /// <param name="pchrOCC"></param>
         /// <param name="pLatestOCCFlag"></param>
         /// <returns></returns>
         List<dtTbt_SaleBasicForView> GetTbt_SaleBasicForView(string pchvContractCode, string pchrOCC, Nullable<bool> pLatestOCCFlag);

         /// <summary>
         /// Get sale basic data
         /// </summary>
         /// <param name="pchvContractCode"></param>
         /// <param name="pchrOCC"></param>
         /// <param name="pbLastOCCFlag"></param>
         /// <returns></returns>
         List<tbt_SaleBasic> GetTbt_SaleBasic(string pchvContractCode, string pchrOCC, Nullable<bool> pbLastOCCFlag);

         /// <summary>
         /// To get sale contract data with accumulated instrument qty
         /// </summary>
         /// <param name="contractCode"></param>
         /// <param name="OCC"></param>
         /// <returns></returns>
         doSaleContractData GetSaleContractData(string contractCode, string OCC);

         /// <summary>
         /// To check linkage sale contract
         /// </summary>
         /// <param name="contractCode"></param>
         /// <param name="quotationTargetCode"></param>
         /// <returns></returns>
         doSaleContractData CheckLinkageSaleContract(string contractCode, string quotationTargetCode);

         /// <summary>
         /// To generate contract occurrence
         /// </summary>
         /// <param name="strContractCode"></param>
         /// <returns></returns>
         string GenerateContractOCC(string strContractCode);

         /// <summary>
         /// To generate contract counter of sale contract
         /// </summary>
         /// <param name="strContractCode"></param>
         /// <returns></returns>
         int GenerateContractCounter(string strContractCode);

         /// <summary>
         /// To get last OCC of sale contract
         /// </summary>
         /// <param name="strContractCode"></param>
         /// <returns></returns>
         string GetLastOCC(string strContractCode);

         /// <summary>
         /// Calling from billing module when registering customer acceptance
         /// </summary>
         /// <param name="strContractCode"></param>
         /// <param name="strSaleOCC"></param>
         /// <param name="dtpCustomerAcceptanceDate"></param>
         void UpdateCustomerAcceptance(string strContractCode, string strSaleOCC, DateTime? dtpCustomerAcceptanceDate);

         /// <summary>
         /// Update data in case new/add sale and will insert new occurrence in case other installation type.
         /// </summary>
         /// <param name="doComplete"></param>
         void CompleteInstallation(doCompleteInstallationData doComplete);

         /// <summary>
         /// Update process magement status to approve contract status in sale basic data.
         /// </summary>
         /// <param name="doComplete"></param>
         void CancelInstallation(doCompleteInstallationData doComplete);
         
         /// <summary>
         /// Get sale instrument subcontractor list for view data
         /// </summary>
         /// <param name="pContractCode"></param>
         /// <param name="pOCC"></param>
         /// <param name="pSubcontractorCode"></param>
         /// <returns></returns>
         List<dtTbt_SaleInstSubcontractorListForView> GetTbt_SaleInstSubcontractorListForView(string pContractCode, string pOCC, string pSubcontractorCode);

         /// <summary>
         /// Getting all part of specified contract for creaing a new occurrence or else
         /// </summary>
         /// <param name="strContractCode"></param>
         /// <param name="strOCC"></param>
         /// <returns></returns>
         dsSaleContractData GetEntireContract(string strContractCode, string strOCC);

         /// <summary>
         /// Insert sale basic data
         /// </summary>
         /// <param name="doInsert"></param>
         /// <returns></returns>
         List<tbt_SaleBasic> InsertTbt_SaleBasic(tbt_SaleBasic doInsert);

         /// <summary>
         /// Insert entire contract data
         /// </summary>
         /// <param name="saleContractData"></param>
         /// <returns></returns>
         dsSaleContractData InsertEntireContract(dsSaleContractData saleContractData);

         /// <summary>
         /// Update sale basic data
         /// </summary>
         /// <param name="dotbt_SaleBasic"></param>
         /// <returns></returns>
         List<tbt_SaleBasic> UpdateTbt_SaleBasic(tbt_SaleBasic dotbt_SaleBasic);

         /// <summary>
         /// Update sale instrument data
         /// </summary>
         /// <param name="dotbt_SaleInstrumentDetails"></param>
         /// <returns></returns>
         List<tbt_SaleInstrumentDetails> UpdateTbt_SaleInstrumentDetails(tbt_SaleInstrumentDetails dotbt_SaleInstrumentDetails);

         /// <summary>
         /// For registering change expected installation complete date of sale contract
         /// </summary>
         /// <param name="dsSaleContract"></param>
         /// <returns></returns>
         bool RegisterChangeExpectedInstallationCompleteDate(dsSaleContractData dsSaleContract);

         /// <summary>
         /// For register change plan of sale contract
         /// </summary>
         /// <param name="dsQuotation"></param>
         /// <param name="dsSaleContract"></param>
         /// <param name="listBillingTemp"></param>
         /// <returns></returns>
         bool RegisterChangePlan(dsQuotationData dsQuotation, dsSaleContractData dsSaleContract, List<tbt_BillingTemp> listBillingTemp);

         /// <summary>
         /// Replace contract data with quotation data. Using when create contract or change contract.
         /// This method can be run on client.
         /// </summary>
         /// <param name="dsQuotation"></param>
         /// <param name="dsSaleContract"></param>
         void MapFromQuotation(dsQuotationData dsQuotation, ref dsSaleContractData dsSaleContract);

         /// <summary>
         /// Get sale basic information for display on Installation page
         /// </summary>
         /// <param name="strContractCode"></param>
         /// <param name="strBuildingType"></param>
         /// <returns></returns>
         dtSaleBasic GetSaleBasicDataForInstall(string strContractCode, string strBuildingType);

         /// <summary>
         /// For register CQ-31
         /// </summary>
         /// <param name="contract"></param>
         void RegisterCQ31(dsSaleContractData contract);

         /// <summary>
         /// Get sale contract basic information
         /// </summary>
         /// <param name="strContractCode"></param>
         /// <returns></returns>
         List<dtSaleContractBasicForView> GetSaleContractBasicForView(string strContractCode);
         //void UpdateEntireContract(dsSaleContract contract);

         /// <summary>
         /// Get instrument installed before quantity
         /// </summary>
         /// <param name="strContractCode"></param>
         /// <returns></returns>
         List<dtInstrumentInstalledBefore> GetInstrumentInstalledBefore(string strContractCode);

         /// <summary>
         /// Get instrument additional installed quantity
         /// </summary>
         /// <param name="strContractCode"></param>
         /// <returns></returns>
         List<dtInstrumentAdditionalInstalled> GetInstrumentAdditionalInstalled(string strContractCode);

         /// <summary>
         /// In case installation type is new sale or add sale, if customer accpetance is registered, return true
         /// </summary>
         /// <param name="strContractCode"></param>
         /// <returns></returns>
         bool CheckCanReplaceInstallSlip(string strContractCode);

         /// <summary>
         /// Get sale basic data for issue invoice 
         /// </summary>
         /// <param name="ContractCode"></param>
         /// <param name="OCC"></param>
         /// <returns></returns>
         List<doGetSaleDataForIssueInvoice> GetSaleDataForIssueInvoice(string ContractCode, string OCC);

         /// <summary>
         /// To check the contract code is existing in sale contract or not
         /// </summary>
         /// <param name="strContractCode"></param>
         /// <returns></returns>
         List<bool?> IsContractExist(string strContractCode);

         /// <summary>
         /// To get previos OCC of sale contract
         /// </summary>
         /// <param name="strContractCode"></param>
         /// <param name="strCurrentOCC"></param>
         /// <returns></returns>
         List<string> GetPreviousOCC(string strContractCode, string strCurrentOCC);

        /// <summary>
        /// Get linkage sale basic data
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        List<tbt_SaleBasic> GetLinkageSaleContractData(string strContractCode);

        /// <summary>
        /// Get data of SaleInstrumentDetails
        /// </summary>
        List<tbt_SaleInstrumentDetails> GetTbt_SaleInstrumentDetails(string strContractCode, string strOCC); //Add by Jutarat A. on 31052013
     }
}
