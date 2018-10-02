using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    public interface IProductMasterHandler
    {
        List<tbm_Product> GetTbm_Product(string pchrProductCode, string pcharProductTypeCode);

        /// <summary>
        /// Get product and mapping language.
        /// </summary>
        /// <param name="pchrProductCode"></param>
        /// <param name="pcharProductTypeCode"></param>
        /// <returns></returns>
        List<View_tbm_Product> GetTbm_ProductByLanguage(string pchrProductCode, string pcharProductTypeCode);

        List<tbm_Product> GetActiveProduct(string pcharProductCode, string pcharProductTypeCode);

        /// <summary>
        /// Get active product and mapping language.
        /// </summary>
        /// <param name="pchrProductCode"></param>
        /// <param name="pcharProductTypeCode"></param>
        /// <returns></returns>
        List<View_tbm_Product> GetActiveProductbyLanguage(string pcharProductCode, string pcharProductTypeCode);

        List<tbm_ProductInstrument> GetTbm_ProductInstrument(string pchrProductCode);
        List<tbm_ProductFacility> GetTbm_ProductFacility(string pchrProductCode);
    }
}
