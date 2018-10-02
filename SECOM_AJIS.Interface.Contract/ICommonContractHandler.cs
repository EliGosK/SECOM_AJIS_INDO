using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Contract 
{
    public interface ICommonContractHandler
    {
        /// <summary>
        /// Get billing temp list for view
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pOCC"></param>
        /// <returns></returns>
        List<dtTbt_BillingTempListForView> GetTbt_BillingTempListForView(string pContractCode, string pOCC);

        /// <summary>
        /// Get billing temp
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> GetTbt_BillingTemp(string strContractCode, string strOCC);

        /// <summary>
        /// Get billing temp from both billing temp table and billing basic
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <param name="strOCC"></param>
        /// <returns></returns>
        List<tbt_BillingTemp> GetTbt_BillingTargetForEditing(string strContractCode, string strOCC);

        /// <summary>
        /// Get sub contractor data
        /// </summary>
        /// <param name="pSubContractorCode"></param>
        /// <returns></returns>
        List<tbm_SubContractor> GetTbm_SubContractorData(string pSubContractorCode);

        /// <summary>
        /// Delete contract email
        /// </summary>
        /// <param name="contractEmailID"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        List<tbt_ContractEmail> DeleteTbt_ContractEmail(int contractEmailID, string empNo = null);

        /// <summary>
        /// Delete the contract email of specified type which not sent
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="emailType"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        List<tbt_ContractEmail> DeleteTbt_ContractEmailUnsentContractEmail(string contractCode, string emailType, bool flag);

        /// <summary>
        /// Update contract email
        /// </summary>
        /// <param name="doUpdate"></param>
        /// <returns></returns>
        List<tbt_ContractEmail> UpdateTbt_ContractEmail(tbt_ContractEmail doUpdate);        

        /// <summary>
        /// Insert relation type
        /// </summary>
        /// <param name="doRelationType"></param>
        /// <returns></returns>
        List<tbt_RelationType> InsertTbt_RelationType(tbt_RelationType doRelationType);

        /// <summary>
        /// Insert relation type (list)
        /// </summary>
        /// <param name="relationTypeList"></param>
        /// <returns></returns>
        int InsertTbt_RelationType(List<tbt_RelationType> relationTypeList);

        /// <summary>
        /// Update relation type
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="relationTypeList"></param>
        /// <returns></returns>
        int UpdateTbt_RelationType(string ContractCode, List<tbt_RelationType> relationTypeList);

        /// <summary>
        /// Delete relation type
        /// </summary>
        /// <param name="pchrContractCode"></param>
        /// <returns></returns>
        List<tbt_RelationType> DeleteTbtRelationType(string pchrContractCode);

        /// <summary>
        /// To generate doRelationType from list of MA target contract. Using for update relation type from quotation
        /// </summary>
        /// <param name="ContractCode"></param>
        /// <param name="MATargetContract"></param>
        /// <param name="BeforeStartFlag"></param>
        /// <returns></returns>
        List<tbt_RelationType> GenerateMaintenanceRelationType(string ContractCode, List<string> MATargetContract, bool BeforeStartFlag = false);

        /// <summary>
        /// Get the records from Tbt_RelationType which link from the specified contract code
        /// </summary>
        /// <param name="pSubContractorCode"></param>
        /// <param name="paramOCC"></param>
        /// <param name="paramRelationType"></param>
        /// <returns></returns>
        List<tbt_RelationType> GetContractLinkageRelation(string pSubContractorCode, string paramOCC, string paramRelationType);

        /// <summary>
        /// Get contract document header
        /// </summary>
        /// <param name="pContractCode"></param>
        /// <param name="pQuotationTargetCode"></param>
        /// <param name="pOCC_Alphabet"></param>
        /// <param name="pContractDocOCC"></param>
        /// <returns></returns>
        List<dtContractDocHeader> GetContractDocHeader(string pContractCode, string pQuotationTargetCode, string pOCC_Alphabet, string pContractDocOCC);

        /// <summary>
        /// Insert contract customer history
        /// </summary>
        /// <param name="docLst"></param>
        /// <returns></returns>
        List<tbt_ContractCustomerHistory> InsertTbt_ContractCustomerHistory(List<tbt_ContractCustomerHistory> docLst);

        /// <summary>
        /// Check exist contract code (Both rental and sale) 
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        bool IsContractExistInRentalOrSale(string strContractCode);

        /// <summary>
        /// Get service type code and product type code by contract code or project code
        /// </summary>
        /// <param name="strCode"></param>
        /// <returns></returns>
        List<doServiceProductTypeCode> GetServiceProductTypeCode(string strCode);

        /// <summary>
        /// Check installation complete remove all 
        /// </summary>
        /// <param name="strContractCode"></param>
        /// <returns></returns>
        bool IsCompleteRemoveAll(string strContractCode);

        void UpdateOperationOffice(string contractCode, string operationOfficeCode);
    }
}
