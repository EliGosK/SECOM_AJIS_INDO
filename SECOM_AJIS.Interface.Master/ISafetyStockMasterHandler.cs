using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface ISafetyStockMasterHandler
    {
        /// <summary>
        /// Retrieve SafetyStock data according to search condition
        /// </summary>
        /// <param name="InstrumentCode"></param>
        /// <returns></returns>
        List<doSafetyStock> GetSafetyStock(string InstrumentCode);

        /// <summary>
        /// Insert SafetyStock information (sp_MAMA_InsertSafetyStock)
        /// </summary>
        /// <param name="safetystock"></param>
        /// <returns></returns>
        List<tbm_SafetyStock> InsertSafetyStock(tbm_SafetyStock safetystock);

        /// <summary>
        /// Update SafetyStock information (sp_MA_UpdateSafetyStock)
        /// </summary>
        /// <param name="safetystock"></param>
        /// <returns></returns>
        List<tbm_SafetyStock> UpdateSafetyStock(tbm_SafetyStock safetystock);

    }
}
