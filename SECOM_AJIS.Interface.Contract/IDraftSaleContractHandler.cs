using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IDraftSaleContractHandler
    {
        /// <summary>
        /// Get entire draft sale contract
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="mode"></param>
        /// <param name="procType"></param>
        /// <returns></returns>
        doDraftSaleContractData GetEntireDraftSaleContract(doDraftSaleContractCondition cond, doDraftSaleContractData.SALE_CONTRACT_MODE mode, doDraftSaleContractData.PROCESS_TYPE procType);
        
        /// <summary>
        /// Get draft sale contract
        /// </summary>
        /// <param name="pchrQuotationTargetCode"></param>
        /// <returns></returns>
        List<tbt_DraftSaleContract> GetTbt_DraftSaleContract(string pchrQuotationTargetCode);

        /// <summary>
        /// To create draft sale contract data
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        bool CreateDraftSaleContractData(doDraftSaleContractData draft);

        /// <summary>
        /// To edit draft sale contract data
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        bool EditDraftSaleContractData(doDraftSaleContractData draft);

        /// <summary>
        /// Update draft sale contract
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        int UpdateTbt_DraftSaleContract(tbt_DraftSaleContract draft);
    }
}
