using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public interface IUserControlHandler
    {
        /// <summary>
        /// Get rental contract basic information to show in user control
        /// </summary>
        /// <param name="strContrancCode"></param>
        /// <returns></returns>
        doRentalContractBasicInformation GetRentalContactBasicInformationData(string strContrancCode);

        /// <summary>
        /// Get rental security basic information to show in user control
        /// </summary>
        /// <param name="strContrancCode"></param>
        /// <param name="occCode"></param>
        /// <returns></returns>
        doRentalSecurityBasicInformation GetRentalSecurityBasicInformationData(string strContrancCode, string occCode);

        /// <summary>
        /// Get sale contract basic information to show in user control
        /// </summary>
        /// <param name="contractCode"></param>
        /// <param name="occCode"></param>
        /// <returns></returns>
        List<doSaleContractBasicInformation> GetSaleContractBasicInformationData(string contractCode, string occCode);
    }
}
