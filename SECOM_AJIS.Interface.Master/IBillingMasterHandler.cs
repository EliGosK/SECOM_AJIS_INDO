using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface IBillingMasterHandler
    {
        string ManageBillingClient(tbm_BillingClient billingClient);
        List<dtBillingClientData> GetBillingClient(string billingClientCode);
        tbm_BillingType GetTbm_BillingType(string BillingTypeCode);
        List<tbm_BillingType> GetTbm_BillingType();
        DateTime? GetAutoTransferDate(string strBankCode, string strAutoTransferDate);
        List<tbm_BillingType> GetBillingTypeOneTimeListData(string BillingServiceTypeCode);
        List<tbm_BillingType> GetBillingTypeList(string billingServiceTypeCode);

        /// <summary>
        /// Get list of auto transfer bank 
        /// </summary>
        /// <returns></returns>
        List<tbm_Bank> GetAutoTransferBank();
    }
}
