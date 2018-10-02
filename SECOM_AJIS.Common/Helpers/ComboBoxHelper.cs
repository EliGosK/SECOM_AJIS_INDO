using System;
using System.Web.WebPages;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

using System.Data;
using System.Collections.Generic;
using System.Linq;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Common.Helpers
{
    public static partial class ComboBoxHelper
    {
        /// <summary>
        /// Generate blank combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BlankComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<ItemValue<object>> ls = new List<ItemValue<object>>();
            return CommonUtil.CommonComboBox<ItemValue<object>>(id, ls, "Display", "Value", attribute);
        }
        /// <summary>
        /// Generate blank combobox (include first element)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BlankComboBoxWithFirstRow(this HtmlHelper helper, string id, object attribute = null)
        {
            List<ItemValue<object>> ls = new List<ItemValue<object>>();
            string dummyValue = "-XXX-";

            ls.Add(new ItemValue<object>()
                {
                    Display = "",
                    Value = dummyValue
                });
            return MvcHtmlString.Create(CommonUtil.CommonComboBox<ItemValue<object>>(id, ls, "Display", "Value", attribute).ToString().Replace("<option value=\"-XXX-\"></option>", ""));
        }
        /// <summary>
        /// Generate blank combobox (include first element)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString BlankComboBoxWithFirstElement(this HtmlHelper helper, string id, object attribute = null, string firstElement = null, bool include_idx0 = true)
        {
            List<ItemValue<object>> ls = new List<ItemValue<object>>();
            return CommonUtil.CommonComboBoxWithCustomFirstElement<ItemValue<object>>(id, ls, "Display", "Value", firstElement, attribute, include_idx0);
        }
        /// <summary>
        /// Generate billing office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingOfficeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return null;
        }
        /// <summary>
        /// Generate month combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="includeIdx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString MonthComboBox(this HtmlHelper helper, string id, object attribute = null, bool includeIdx0 = true, string firstElement = null)
        {
            DateTime month = new DateTime(2000, 1, 1, 0, 0, 0);
            List<ItemValue<int?>> ls = new List<ItemValue<int?>>();

            ItemValue<int?> item;

            for (int i = 0; i < 12; i++)
            {
                item = new ItemValue<int?>();
                item.Value = i + 1;
                item.Display = month.ToString("MMMM");
                ls.Add(item);

                month = month.AddMonths(1);
            }


            return CommonUtil.CommonComboBoxWithCustomFirstElement<ItemValue<int?>>(id, ls, "Display", "Value", firstElement,attribute, includeIdx0);

        }
        /// <summary>
        /// Generate month combobox (display value as number)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="includeIdx0"></param>
        /// <returns></returns>
        public static MvcHtmlString MonthComboBoxDispalyAsNumber(this HtmlHelper helper, string id, object attribute = null, bool includeIdx0 = true)
        {
            DateTime month = new DateTime(2000, 1, 1, 0, 0, 0);
            List<ItemValue<int?>> ls = new List<ItemValue<int?>>();

            ItemValue<int?> item;

            for (int i = 0; i < 12; i++)
            {
                item = new ItemValue<int?>();
                item.Value = i + 1;
                item.Display = month.ToString("MM");
                ls.Add(item);

                month = month.AddMonths(1);
            }

            //return CommonUtil.CommonComboBoxWithCustomFirstElement<doMiscTypeCode>(id, lst, display, "ValueCode", firstElement, attribute, include_idx0);
            return CommonUtil.CommonComboBox<ItemValue<int?>>(id, ls, "Display", "Value", attribute, includeIdx0, CommonUtil.eFirstElementType.None);

        }
        /// <summary>
        /// Generate year combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="includeIdx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString YearComboBox(this HtmlHelper helper, string id, object attribute = null, bool includeIdx0 = true, string firstElement = null)
        {
            DateTime today = DateTime.Now;
            List<ItemValue<int?>> ls = new List<ItemValue<int?>>();
            ItemValue<int?> item;

            // Year +- 5
            today = today.AddYears(5);
            for (int i = 0; i <= 10; i++)
            {
                item = new ItemValue<int?>();
                item.Value = today.Year;
                item.Display = today.Year.ToString();
                ls.Add(item);

                today = today.AddYears(-1);
            }
            return CommonUtil.CommonComboBox<ItemValue<int?>>(id, ls, "Display", "Value", attribute, includeIdx0, CommonUtil.eFirstElementType.None);
            //return CommonUtil.CommonComboBoxWithCustomFirstElement<ItemValue<int?>>(id, ls, "Display", "Value", firstElement, attribute, includeIdx0);
        }
        /// <summary>
        /// Generate time combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString TimeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            DateTime t = DateTime.Now;
            DateTime today = new DateTime(t.Year, t.Month, t.Day, 0, 0, 0);
            List<ItemValue<DateTime>> ls = new List<ItemValue<DateTime>>();
            ItemValue<DateTime> item;

            for (int i = 0; i < 48; i++)
            {
                item = new ItemValue<DateTime>();
                item.Value = today;
                item.Display = today.ToString("HH:mm");
                ls.Add(item);

                today = today.AddMinutes(30);
            }

            return CommonUtil.CommonComboBox<ItemValue<DateTime>>(id, ls, "Display", "Value", attribute);
        }
        /// <summary>
        /// Generate quotation office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString QuotationOfficeComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            List<OfficeDataDo> nLst = new List<OfficeDataDo>();

            try
            {
                dsTransDataModel dstrans = CommonUtil.dsTransData;
                if (dstrans != null)
                {
                    List<OfficeDataDo> lst = new List<OfficeDataDo>();
                    List<OfficeDataDo> clst = dstrans.dtOfficeData;
                    if (clst != null)
                    {
                        foreach (OfficeDataDo off in clst)
                        {
                            if (off.FunctionQuatation != SECOM_AJIS.Common.Util.ConstantValue.FunctionQuotation.C_FUNC_QUOTATION_NO)
                                lst.Add(off);
                        }
                    }

                    nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
                    CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);
                }
            }
            catch
            {
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", firstElement, attribute);
        }
        /// <summary>
        /// Generate operation office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString OperationOfficeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            List<OfficeDataDo> nLst = new List<OfficeDataDo>();

            try
            {
                List<OfficeDataDo> lst = new List<OfficeDataDo>();
                List<OfficeDataDo> clst = CommonUtil.dsTransData.dtOfficeData;
                if (clst != null)
                {
                    foreach (OfficeDataDo off in clst)
                    {
                        if (off.FunctionSecurity != SECOM_AJIS.Common.Util.ConstantValue.FunctionSecurity.C_FUNC_SECURITY_NO)
                            lst.Add(off);
                    }

                    nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
                    CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);
                }
            }
            catch
            {
            }

            return CommonUtil.CommonComboBox<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);
        }
        /// <summary>
        /// Generate operation office combobox (include first element)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="firstElement"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString OperationOfficeComboBoxWithFirstElement(this HtmlHelper helper, string id, string firstElement, object attribute = null)
        {
            List<OfficeDataDo> clst = CommonUtil.dsTransData.dtOfficeData;
            List<OfficeDataDo> lst = new List<OfficeDataDo>();
            if (clst != null)
            {
                foreach (OfficeDataDo off in clst)
                {
                    if (off.FunctionQuatation != SECOM_AJIS.Common.Util.ConstantValue.FunctionSecurity.C_FUNC_SECURITY_NO)
                        lst.Add(off);
                }
            }

            List<OfficeDataDo> nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
            CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);

            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", firstElement, attribute);
        }
        /// <summary>
        /// Generate operation office combobox (only security flag = on) 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString OperationOfficeSecurityFlagOnComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<OfficeDataDo> clst = CommonUtil.dsTransData.dtOfficeData;
            List<OfficeDataDo> lst = new List<OfficeDataDo>();
            if (clst != null)
            {
                foreach (OfficeDataDo off in clst)
                {
                    if (off.FunctionSecurity != SECOM_AJIS.Common.Util.ConstantValue.FunctionSecurity.C_FUNC_SECURITY_NO)
                        lst.Add(off);
                }

                //if (lst.Count > 0)
                //{
                //    OfficeDataDo officeData = new OfficeDataDo();
                //    officeData.OfficeName = "office";
                //    officeData.OfficeCode = "ALL";
                //    lst.Insert(0, officeData);
                //}
            }

            List<OfficeDataDo> nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
            CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);

            return CommonUtil.CommonComboBox<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", attribute);
        }
        /// <summary>
        /// Generate contract office combobox (only sale flag = on)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ContractOfficeSaleFlagOnComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<OfficeDataDo> clst = CommonUtil.dsTransData.dtOfficeData;
            List<OfficeDataDo> lst = new List<OfficeDataDo>();
            if (clst != null)
            {
                foreach (OfficeDataDo off in clst)
                {
                    if (off.FunctionSale == SECOM_AJIS.Common.Util.ConstantValue.FunctionSale.C_FUNC_SALE_YES)
                        lst.Add(off);
                }

                //if (lst.Count > 0)
                //{
                //    OfficeDataDo officeData = new OfficeDataDo();
                //    officeData.OfficeName = "office";
                //    officeData.OfficeCode = "ALL";
                //    lst.Add(officeData);
                //}
            }

            List<OfficeDataDo> nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
            CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);

            return CommonUtil.CommonComboBox<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", attribute);
        }

        #region Office With Authority Combobox (New version)

        /// <summary>
        /// Generate quotation office combobox (only has authority)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString QuotationOfficeAuthorityCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<OfficeDataDo> nLst = new List<OfficeDataDo>();

            try
            {
                List<OfficeDataDo> lst = (from p in CommonUtil.dsTransData.dtOfficeData
                       where p.FunctionQuatation != FunctionQuotation.C_FUNC_QUOTATION_NO
                       orderby p.OfficeCode ascending
                       select p).ToList<OfficeDataDo>();

                nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);
            }
            catch
            {
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", firstElement, attribute, include_idx0);
        }
        /// <summary>
        /// Generate operation office combobox (only has authority)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString OperationOfficeAuthorityCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<OfficeDataDo> nLst = new List<OfficeDataDo>();

            try
            {
                List<OfficeDataDo> lst = (from p in CommonUtil.dsTransData.dtOfficeData where p.FunctionSecurity != FunctionSecurity.C_FUNC_SECURITY_NO 
                       orderby p.OfficeCode ascending
                       select p).ToList<OfficeDataDo>();

                nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);
            }
            catch
            {
            }

            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", firstElement, attribute, include_idx0);
        }
        public static MvcHtmlString OperationOfficeShowAllAuthorityCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<OfficeDataDo> nLst = new List<OfficeDataDo>();

            try
            {
                // Show office all not condition
                List<OfficeDataDo> lst = (from p in CommonUtil.dsTransData.dtOfficeData
                                         // where p.FunctionSecurity != FunctionSecurity.C_FUNC_SECURITY_NO
                                          orderby p.OfficeCode ascending
                                          select p).ToList<OfficeDataDo>();

                nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);
            }
            catch
            {
            }

            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", firstElement, attribute, include_idx0);
        }
        /// <summary>
        /// Generate contract office combobox (only has authority)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString ContractOfficeAuthorityCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {

            List<OfficeDataDo> nLst = new List<OfficeDataDo>();
            try
            {
                List<OfficeDataDo> lst = (from p in CommonUtil.dsTransData.dtOfficeData
                                          where p.FunctionSale != FunctionSale.C_FUNC_SALE_NO 
                       orderby p.OfficeCode ascending
                       select p).ToList<OfficeDataDo>();

                nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);
            }
            catch
            {
            }


            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", firstElement, attribute, include_idx0);
        }
        /// <summary>
        /// Generate billing office combobox (only has authority)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingOfficeAuthorityCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<OfficeDataDo> nLst = new List<OfficeDataDo>();

            try
            {
                List<OfficeDataDo> lst = (from p in CommonUtil.dsTransData.dtOfficeData
                       where p.FunctionBilling != FunctionBilling.C_FUNC_BILLING_NO
                       orderby p.OfficeCode ascending
                       select p).ToList<OfficeDataDo>();

                nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);

            }
            catch
            {
            }
            //--- modify by Siripoj S. 23-04-2012
            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", firstElement,attribute, include_idx0);
            //--- End Modify
        }
        /// <summary>
        /// Generate install sip issue office combobox (only has authority)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString InstallSipIssueOfficeAuthorityCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<OfficeDataDo> nLst = new List<OfficeDataDo>();

            try
            {
                List<OfficeDataDo> lst = (from p in CommonUtil.dsTransData.dtOfficeData
                       where p.FunctionSecurity != FunctionSecurity.C_FUNC_SECURITY_NO
                       orderby p.OfficeCode ascending
                       select p).ToList<OfficeDataDo>();

                nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);
            }
            catch
            {
            }

            // Edit by Narupon W. 9 May 2012
            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);

            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", firstElement, attribute, include_idx0);
        }
        /// <summary>
        /// Generate inverntory sip issue office combobox (only has authority)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString InventorySipIssueOfficeAuthorityCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<OfficeDataDo> nLst = new List<OfficeDataDo>();

            try
            {
                List<OfficeDataDo> lst = (from p in CommonUtil.dsTransData.dtOfficeData
                       where p.FunctionLogistic != FunctionLogistic.C_FUNC_LOGISTIC_NO
                       orderby p.OfficeCode ascending
                       select p).ToList<OfficeDataDo>();

                nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);
            }
            catch
            {
            }

            // Edit by Narupon W. 9 May 2012
            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);

            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", firstElement, attribute, include_idx0);
        }
        /// <summary>
        /// Generate inventory office combobox (only has authority)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryOfficeAuthorityComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {

            List<OfficeDataDo> nLst = new List<OfficeDataDo>();
            try
            {
                List<OfficeDataDo> lst = (from p in CommonUtil.dsTransData.dtOfficeData
                       where p.FunctionLogistic != LogisticFunction.C_OFFICELOGISTIC_NONE   
                       orderby p.OfficeCode ascending
                       select p).ToList<OfficeDataDo>();

                nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);
            }
            catch
            {
            }
            // Edit by Narupon W. 9 May 2012
            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);

            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", firstElement, attribute, include_idx0);
        }
        /// <summary>
        /// Generate billing debt tracing combobox (only has authority)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingDebtTracingAuthorityComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {

            List<OfficeDataDo> nLst = new List<OfficeDataDo>();
            try
            {
                //List<OfficeDataDo> lst = (from p in CommonUtil.dsTransData.dtOfficeData
                //       where p.FunctionLogistic == FunctionDebtTracing.C_FUNC_DEBTTRACING_YES
                //       orderby p.OfficeCode ascending
                //       select p).ToList<OfficeDataDo>();

                List<OfficeDataDo> lst = (from p in CommonUtil.dsTransData.dtOfficeData
                                          where p.FunctionDebtTracing == FunctionDebtTracing.C_FUNC_DEBTTRACING_YES
                                          orderby p.OfficeCode ascending
                                          select p).ToList<OfficeDataDo>();

                nLst = CommonUtil.ClonsObjectList<OfficeDataDo, OfficeDataDo>(lst);
                CommonUtil.MappingObjectLanguage<OfficeDataDo>(nLst);
            }
            catch
            {
            }


            // Edit by Narupon W. 9 May 2012
            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);

            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, nLst, "OfficeCodeName", "OfficeCode", firstElement, attribute, include_idx0);
        }

        #endregion
    }
}
