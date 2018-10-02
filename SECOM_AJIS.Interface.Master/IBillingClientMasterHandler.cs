using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface IBillingClientMasterHandler
    {
        #region Master II
        /// <summary>
        /// Retrieve billing client according to condition (sp_MA_GetTbm_BillingClient)
        /// </summary>
        /// <param name="BillingClientCode"></param>
        /// <returns></returns>
        List<tbm_BillingClient> GetTbm_BillingClient(string BillingClientCode);

        /// <summary>
        /// Update billing client information (sp_MA_UpdateBillingClient)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        List<tbm_BillingClient> UpdateBillingClient(tbm_BillingClient data);
        #endregion
    }
}
