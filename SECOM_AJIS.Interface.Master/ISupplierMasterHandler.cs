using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Common;


namespace SECOM_AJIS.DataEntity.Master
{
    public interface ISupplierMasterHandler
    {
        /// <summary>
        /// Get supplier and mapping language.
        /// </summary>
        /// <param name="strSupplierCode"></param>
        /// <param name="strSupplierName"></param>
        /// <returns></returns>
        tbm_Supplier GetSupplier(string strSupplierCode, string strSupplierName);
    }
}
