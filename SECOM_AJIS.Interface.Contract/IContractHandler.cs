using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models.EmailTemplates;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IContractHandler
    {
        /// <summary>
        /// Get contract header from data
        /// </summary>
        /// <param name="contracts"></param>
        /// <returns></returns>
        List<doContractHeader> GetContractHeaderData(List<tbt_SaleBasic> contracts);

        /// <summary>
        /// Get contract header by language from data
        /// </summary>
        /// <param name="contracts"></param>
        /// <returns></returns>
        List<doContractHeader> GetContractHeaderDataByLanguage(List<tbt_SaleBasic> contracts);

        /// <summary>
        /// To check whether all contract have the same site
        /// </summary>
        /// <param name="contracts"></param>
        /// <returns></returns>
        bool IsSameSite(List<string> contracts);

        /// <summary>
        /// To get site name from provided contract code.
        /// </summary>
        /// <param name="strContractCodeList"></param>
        /// <param name="bLastestOCCFlag"></param>
        /// <returns></returns>
        List<dsGetSiteContractList> GetSiteContractList(string strContractCodeList, bool bLastestOCCFlag);

        /// <summary>
        /// To check maintenance target contract
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strQuotationTargetCode"></param>
        /// <returns></returns>
        doContractHeader CheckMaintenanceTargetContract(string strContractCode, string strQuotationTargetCode);

        /// <summary>
        /// To check maintenance target contract list
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="bLastestOCCFlag"></param>
        /// <returns></returns>
        List<dtGetMaintenanceTargetContract> GetMaintenanceTargetContract(string strContractCode, bool bLastestOCCFlag);

        /// <summary>
        /// To check maintenance target contract list
        /// </summary>
        /// <param name="contracts"></param>
        /// <param name="quotationTargetCode"></param>
        /// <returns></returns>
        List<doContractHeader> CheckMaintenanceTargetContractList(List<string> contracts, string quotationTargetCode);

        /// <summary>
        /// To generate contract code
        /// </summary>
        /// <param name="strProductTypeCode"></param>
        /// <returns></returns>
        string GenerateContractCode(string strProductTypeCode);

        /// <summary>
        /// To generate notify email for change contract fee
        /// </summary>
        /// <param name="doNotifyEmail"></param>
        /// <returns></returns>
        doNotifyChangeFeeContract GenerateNotifyEmail(doNotifyChangeFeeContract doNotifyEmail);
        
        /// <summary>
        /// Update data in case new/add sale and will insert new occurrence in case other installation type.
        /// </summary>
        /// <param name="doComplete"></param>
        void CompleteInstallation(doCompleteInstallationData doComplete);

        /// <summary>
        /// To send notify email for change contract fee
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="BatchDate"></param>
        /// <returns></returns>
        doBatchProcessResult SendNotifyEmailForChangeFee(string UserId, DateTime BatchDate);

        /// <summary>
        /// Get unsent notify email
        /// </summary>
        /// <returns></returns>
        List<tbt_ContractEmail> GetUnsentNotifyEmail();

        /// <summary>
        /// Check site code whether be used in draft contract, contract , AR and incident 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <returns></returns>
        bool IsUsedSiteData(string siteCode);

        /// <summary>
        /// Check customer code whether be used in draft contract, contract , AR, incident or Project
        /// </summary>
        /// <param name="custCode"></param>
        /// <returns></returns>
        bool IsUsedCustomerData(string custCode);

        /// <summary>
        /// Insert contract email data
        /// </summary>
        /// <param name="listEmail"></param>
        /// <returns></returns>
        int InsertTbt_ContractEmail(List<tbt_ContractEmail> listEmail);

        /// <summary>
        /// Auto renew batch process
        /// </summary>
        /// <returns></returns>
        List<AutoRenewProcess_Result> AutoRenewProcessBatch();

        /// <summary>
        /// Update contract email data
        /// </summary>
        /// <param name="listEmail"></param>
        /// <returns></returns>
        int UpdateTbt_ContractEmail(List<tbt_ContractEmail> listEmail); //Add by Jutarat A. on 14012014

        /// <summary>
        /// Get cancel contract memo.
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <returns></returns>
        List<tbt_CancelContractMemo> GetTbt_CancelContractMemo(string pContractCode, string pOCC);

        /// <summary>
        /// GetContralLastOCC
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <returns></returns>
        List<string> GetContractLastOCC(string ContractCode);

        bool SendEmailChangePlanBeforeStart(doChangePlanBeforeStartEmail templateObj);


        List<sp_CT_GetSaleBasicOneShotFlag_Result> GetSaleBasicOneShotFlag(string strContractCode);

        List<dtUnreceivedContractDocuemntCTR095> GetUnreceivedContractDocuemntCTR095(DateTime? GenerateDateFrom, DateTime? GenerateDateTo);
    }
}
