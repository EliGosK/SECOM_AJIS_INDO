using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Helpers;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Master;



namespace SECOM_AJIS.Presentation.Quotation.Helpers
{
    public static partial class ComboBoxHelper
    {
        /// <summary>
        /// Generate product type combobox for screen QUS010
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="strServiceTypeCode"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString ProductTypeComboboxQUS010(this HtmlHelper helper, string id, string strServiceTypeCode, object attribute = null, string firstElement = null)
        {
            if (strServiceTypeCode == "")
                strServiceTypeCode = null;
            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<tbs_ProductType> lst = hand.GetTbs_ProductType(strServiceTypeCode, null);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbs_ProductType>(id, lst, "ProductTypeCodeName", "ProductTypeCode", firstElement, attribute);
        }
        /// <summary>
        /// Generate operation office combobox for screen QUS010
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="strServiceTypeCode"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString QuotationOfficeComboQUS010(this HtmlHelper helper, string id, object attribute = null)
        {

            IOfficeMasterHandler hand = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            List<dtOffice> lstOffice = hand.GetFunctionQuatation(FunctionQuotation.C_FUNC_QUOTATION_NO);
            List<View_dtOffice> lst_view = CommonUtil.ConvertObjectbyLanguage<dtOffice, View_dtOffice>(lstOffice, "OfficeName");

            return CommonUtil.CommonComboBox<View_dtOffice>(id, lst_view, "ValueCodeDisplay", "OfficeCode", attribute);
        }
        /// <summary>
        /// Generate operation offfice combobox for screen QUS010
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString OperationOfficeComboQUS010(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {

            IOfficeMasterHandler hand = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            List<dtOffice> lstOffice = hand.GetFunctionSecurity(FunctionSecurity.C_FUNC_SECURITY_NO);
            List<View_dtOffice> lst_view = CommonUtil.ConvertObjectbyLanguage<dtOffice, View_dtOffice>(lstOffice, "OfficeName");

            return CommonUtil.CommonComboBoxWithCustomFirstElement<View_dtOffice>(id, lst_view, "ValueCodeDisplay", "OfficeCode", firstElement, attribute);

        }
        /// <summary>
        /// Generate lock status combobox for screen QUS010
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString LockStatusComboQUS010(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<doMiscTypeCode> lstMiscTypeCode = new List<doMiscTypeCode>();
            doMiscTypeCode MiscTypeCode = new doMiscTypeCode();
            MiscTypeCode.FieldName = MiscType.C_LOCK_STATUS;
            MiscTypeCode.ValueCode = "%";
            lstMiscTypeCode.Add(MiscTypeCode);
            List<doMiscTypeCode> MiscLock = hand.GetMiscTypeCodeList(lstMiscTypeCode);

            return CommonUtil.CommonComboBoxWithCustomFirstElement<doMiscTypeCode>(id, MiscLock, "ValueCodeDisplay", "ValueCode", firstElement, attribute);

        }
    }
}
