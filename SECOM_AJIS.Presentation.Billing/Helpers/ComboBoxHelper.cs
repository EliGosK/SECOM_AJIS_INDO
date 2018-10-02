using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
using System.Collections.Generic;
using System.Linq;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;

using SECOM_AJIS.DataEntity.Master;
using CSI.WindsorHelper;

using SECOM_AJIS.Common.Util.ConstantValue;
using System.Reflection;
using System.Globalization;
using SECOM_AJIS.Presentation.Common.Helpers;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.Presentation.Billing.Helpers
{
    public static partial class ComboBoxHelper
    {
        /// <summary>
        /// Generate billing office with special condition combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
    
        public static MvcHtmlString BillingOfficeWithSpecialConditionCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {            
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                // Get all office data
                list = handler.GetTbm_Office();
                var lstheadOffice = (from p in list
                                                where p.OfficeLevel == InventoryHeadOffice.C_OFFICELEVEL_HEAD
                                               select p.OfficeCode).ToList<string>();


                var bHasHeadOffice = (from p in CommonUtil.dsTransData.dtOfficeData
                                        where lstheadOffice.Contains(p.OfficeCode)
                                        select p
                                     ).Count() > 0;

                if (bHasHeadOffice)
                {
                    list = (from p in list 
                            where p.FunctionBilling == FunctionBilling.C_FUNC_BILLING_YES 
                            select p).ToList<tbm_Office>();
                }
                else
                {
                    var lstBelonging = (from p in CommonUtil.dsTransData.dtUserBelongingData
                                          select p.OfficeCode
                                         ).ToList<string>();

                    list = (from p in list
                            where lstBelonging.Contains(p.OfficeCode) && p.FunctionBilling == FunctionBilling.C_FUNC_BILLING_YES 
                            select p).ToList<tbm_Office>();
                }
                // Language mappping
                CommonUtil.MappingObjectLanguage<tbm_Office>(list);                
            }
            catch
            {
                list = new List<tbm_Office>();
            }

            return CommonUtil.CommonComboBox<tbm_Office>(id, list, "OfficeDisplay", "OfficeCode", attribute, false);

        }

        #region BLS031
        /// <summary>
        /// Generate bank code combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="autoTransferOnly"></param>
        /// <returns></returns>
        public static MvcHtmlString BankCodeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, bool autoTransferOnly = false)
        {
            List<tbm_Bank> list = new List<tbm_Bank>();
            string strDisplayName = "BankNameEN";
            try
            {
                if (autoTransferOnly == true)
                {
                    //return only support autotransfer
                    IBillingMasterHandler handler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                    list = handler.GetAutoTransferBank();
                }
                else
                {                    
                    //return all bank
                    IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                    list = handler.GetTbm_Bank();
                }


                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN
                    || CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "BankNameEN";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                    list = list.OrderBy(p => p.BankNameEN, StringComparer.Create(culture, false)).ToList();
                }
                else
                {
                    strDisplayName = "BankNameLC";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                    list = list.OrderBy(p => p.BankNameLC, StringComparer.Create(culture, false)).ToList();
                }
            }
            catch
            {
                list = new List<tbm_Bank>();
            }

            return CommonUtil.CommonComboBox<tbm_Bank>(id, list, strDisplayName, "BankCode", attribute, include_idx0);
        }

        /// <summary>
        /// Generate bank branch combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="BankCode"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString BankBranchComboBox(this HtmlHelper helper, string id, int? BankCode, object attribute = null, bool include_idx0 = true)
        {
            List<tbm_BankBranch> lst = new List<tbm_BankBranch>();
            try
            {
                IMasterHandler handle = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                lst = handle.GetTbm_BankBranch(null);

               
                //lst = (from p in lst orderby p.DistrictName ascending select p).ToList<tbm_District>();
            }
            catch
            {
                lst = new List<tbm_BankBranch>();
            }

            return CommonUtil.CommonComboBox<tbm_BankBranch>(id, lst, "BankBranchName", "BankBranchCode", attribute, false);
        }
        #endregion

        /// <summary>
        /// Generate billing type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingTypeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            List<tbm_BillingType> BillingTypeList = new List<tbm_BillingType>();
            try
            {
                IBillingMasterHandler billingMasterHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;
                BillingTypeList = billingMasterHandler.GetBillingTypeOneTimeListData(null);
                CommonUtil.MappingObjectLanguage<tbm_BillingType>(BillingTypeList);
            }
            catch
            {
                BillingTypeList = new List<tbm_BillingType>();
            }

            return CommonUtil.CommonComboBox<tbm_BillingType>(id, BillingTypeList, "BillingTypeCodeName", "BillingTypeCode", attribute);
        }

        /// <summary>
        /// Generate calculation daily fee combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CalculationDailyFeeComboBox2(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_CALC_DAILY_FEE_TYPE, attribute, "ValueDisplay");            
        }

        /// <summary>
        /// Generate billing flag combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingFlagComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            //return MiscTypeComboBox(helper, id, MiscType.C_BILLING_FLAG, attribute, "ValueDisplay");
            return MiscTypeComboBox(helper, id, MiscType.C_BILLING_FLAG, attribute, "ValueCodeDisplay");
        }

        // Akat K. 2014-05-20 : add new combobox
        /// <summary>
        /// Generate print advance date combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString PrintAdvanceDateComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            return MiscTypeComboBox(helper, id, MiscType.C_PRINT_ADVANCE_DATE, attribute, "ValueDisplay", false);
        }

        /// <summary>
        /// Generate misc type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="fieldName"></param>
        /// <param name="attribute"></param>
        /// <param name="display_field"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        private static MvcHtmlString MiscTypeComboBox(this HtmlHelper helper, string id, string fieldName, object attribute = null, string display_field = null, bool include_idx0 = true)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            try
            {
                List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = fieldName,
                        ValueCode = "%"
                    }
                };

                ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                lst = hand.GetMiscTypeCodeList(miscs);
            }
            catch
            {
            }

            if (lst == null)
                lst = new List<doMiscTypeCode>();

            string display = "ValueCodeDisplay";
            if (display_field != null)
                display = display_field;

            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, lst, display, "ValueCode", attribute, include_idx0);
        }


    }
}
