using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    public class ProductMasterHandler : BizMADataEntities, IProductMasterHandler
    {
        #region Override Methods

        public override List<tbm_Product> GetActiveProduct(string pcharProductCode, string pcharProductTypeCode)
        {
            try
            {
                //MandatoryField mf = new MandatoryField();
                //mf.MandatoryMessage = "ProductTypeCode";
                //ApplicationErrorException.CheckMandatoryField(pcharProductTypeCode, mf);

                return base.GetActiveProduct(pcharProductCode, pcharProductTypeCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #region Methods

        public List<View_tbm_Product> GetTbm_ProductByLanguage(string pchrProductCode, string pcharProductTypeCode)
        {
            try
            {
                List<View_tbm_Product> lst = CommonUtil.ConvertObjectbyLanguage<tbm_Product, View_tbm_Product>(
                    base.GetTbm_Product(pchrProductCode, pcharProductTypeCode),
                    "ProductName");

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<View_tbm_Product> GetActiveProductbyLanguage(string pcharProductCode, string pcharProductTypeCode)
        {
            try
            {
                return SECOM_AJIS.Common.Util.CommonUtil.ConvertObjectbyLanguage<tbm_Product, View_tbm_Product>(
                    this.GetActiveProduct(pcharProductCode, pcharProductTypeCode),
                    "ProductName");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
