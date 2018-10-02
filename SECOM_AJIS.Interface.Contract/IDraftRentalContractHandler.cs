using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract 
{
    public interface IDraftRentalContractHandler
    {
        /// <summary>
        /// Get entire draft rental contract data
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        doDraftRentalContractData GetEntireDraftRentalContract(doDraftRentalContractCondition cond, doDraftRentalContractData.RENTAL_CONTRACT_MODE mode);

        /// <summary>
        /// Get draft rental contract data
        /// </summary>
        /// <param name="pchrQuotationTargetCode"></param>
        /// <returns></returns>
        List<tbt_DraftRentalContract> GetTbt_DraftRentalContract(string pchrQuotationTargetCode);

        /// <summary>
        /// Get draft rental contract information 
        /// </summary>
        /// <param name="strQuotationTargetCode"></param>
        /// <returns></returns>
        List<doDraftRentalContractInformation> GetDraftRentalContractInformationData(string strQuotationTargetCode);

        /// <summary>
        /// To create draft rental contract data
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        bool CreateDraftRentalContractData(doDraftRentalContractData draft);
        
        /// <summary>
        /// To edit draft rental contract data
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        bool EditDraftRentalContractData(doDraftRentalContractData draft);

        /// <summary>
        /// To edit draft rental contract data for CTS010
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        bool EditDraftRentalContractDataForCTS010(doDraftRentalContractData draft);

        /// <summary>
        /// Update draft rental contract data
        /// </summary>
        /// <param name="draft"></param>
        /// <returns></returns>
        int UpdateTbt_DraftRentalContract(tbt_DraftRentalContract draft);
    }
}
