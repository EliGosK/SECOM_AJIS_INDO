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

namespace SECOM_AJIS.Presentation.Master.Helpers
{
    public static partial class ComboBoxHelper
    {
        /// <summary>
        /// Generate province combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString ProvinceComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            List<tbm_Province> lst = new List<tbm_Province>();
            try
            {
                IMasterHandler handle = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                lst = handle.GetTbm_Province();

                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                    lst = lst.OrderBy(p => p.ProvinceName, StringComparer.Create(culture, false)).ToList();
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_JP);
                    lst = lst.OrderBy(p => p.ProvinceName, StringComparer.Create(culture, false)).ToList();
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_LC)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                    lst = lst.OrderBy(p => p.ProvinceName, StringComparer.Create(culture, false)).ToList();
                }

                //lst = (from p in lst orderby p.ProvinceName ascending select p).ToList<tbm_Province>();
            }
            catch
            {
                lst = new List<tbm_Province>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Province>(id, lst, "ProvinceName", "ProvinceCode", firstElement, attribute);
        }

        /// <summary>
        /// Generate district combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="province"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString DistrictComboBox(this HtmlHelper helper, string id, int? province, object attribute = null, string firstElement = null)
        {
            List<tbm_District> lst = new List<tbm_District>();
            try
            {
                IMasterHandler handle = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                lst = handle.GetTbm_District(null);

                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                    lst = lst.OrderBy(p => p.DistrictName, StringComparer.Create(culture, false)).ToList();
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_JP);
                    lst = lst.OrderBy(p => p.DistrictName, StringComparer.Create(culture, false)).ToList();
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_LC)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                    lst = lst.OrderBy(p => p.DistrictName, StringComparer.Create(culture, false)).ToList();
                }

                //lst = (from p in lst orderby p.DistrictName ascending select p).ToList<tbm_District>();
            }
            catch
            {
                lst = new List<tbm_District>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_District>(id, lst, "DistrictName", "DistrictCode", firstElement, attribute);
        }

        /// <summary>
        /// Generate district EN combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="province"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DistrictComboBoxEn(this HtmlHelper helper, string id, int? province, object attribute = null)
        {
            List<tbm_District> lst = new List<tbm_District>();
            try
            {
                IMasterHandler handle = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                lst = handle.GetTbm_District(null);


                var sortedList = from p in lst
                                 orderby p.DistrictNameEN ascending
                                 select p;

                lst = sortedList.ToList<tbm_District>();
            }
            catch
            {
                lst = new List<tbm_District>();
            }

            return CommonUtil.CommonComboBox<tbm_District>(id, lst, "DistrictNameEN", "DistrictCode", attribute);
        }

        /// <summary>
        /// Generate district LC combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="province"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DistrictComboBoxLc(this HtmlHelper helper, string id, int? province, object attribute = null)
        {
            List<tbm_District> lst = new List<tbm_District>();
            try
            {
                IMasterHandler handle = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                lst = handle.GetTbm_District(null);

                //var sortedList = from p in lst
                //                 orderby p.DistrictNameLC ascending
                //                 select p;
                //lst = sortedList.ToList<tbm_District>();

                CultureInfo culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                lst = lst.OrderBy(p => p.DistrictNameLC, StringComparer.Create(culture, false)).ToList();
            }
            catch
            {
                lst = new List<tbm_District>();
            }

            return CommonUtil.CommonComboBox<tbm_District>(id, lst, "DistrictNameLC", "DistrictCode", attribute);
        }

        // Office := Create by: Narupon W. ; Create Date: 16/Jun/2010 ; Update Date: 22/Jun/2010
        /// <summary>
        /// Generate office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString OfficeComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null, bool include_idx0 = true)
        {
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                list = handler.GetTbm_Office();

                foreach (var i in list)
                {
                    // Check user aplication launguage
                    if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                    {
                        i.OfficeNameEN = CommonUtil.TextCodeName(i.OfficeCode, i.OfficeNameEN);
                    }
                    else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                    {
                        i.OfficeNameEN = CommonUtil.TextCodeName(i.OfficeCode, i.OfficeNameJP);
                    }
                    else
                    {
                        i.OfficeNameEN = CommonUtil.TextCodeName(i.OfficeCode, i.OfficeNameLC);
                    }
                }

            }
            catch
            {
                list = new List<tbm_Office>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Office>(id, list, "OfficeNameEN", "OfficeCode", firstElement, attribute, include_idx0);
        }

        // Department := Create by: Narupon W. ; Create Date: 16/Jun/2010 ; Update Date: 22/Jun/2010
        /// <summary>
        /// Generate department combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString DepartmetComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            List<tbm_Department> list = new List<tbm_Department>();

            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                list = handler.GetTbm_Department();

                foreach (var i in list)
                {
                    i.DepartmentName = CommonUtil.TextCodeName(i.DepartmentCode, i.DepartmentName);
                }
            }
            catch
            {
                list = new List<tbm_Department>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Department>(id, list, "DepartmentName", "DepartmentCode", firstElement, attribute);

        }

        /// <summary>
        /// Generate customer type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CustomerTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode("CustomerType");

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute);

        }

        /// <summary>
        /// Generate financial combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString FinancialComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbs_MiscellaneousTypeCode> list = new List<tbs_MiscellaneousTypeCode>();
            string strDisplayName = "ValueDisplayEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbs_MiscellaneousTypeCode("FinancialMarketType");

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "ValueDisplayEN";
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "ValueDisplayJP";
                }
                else
                {
                    strDisplayName = "ValueDisplayLC";
                }

            }
            catch
            {
                list = new List<tbs_MiscellaneousTypeCode>();
            }

            return CommonUtil.CommonComboBox<tbs_MiscellaneousTypeCode>(id, list, strDisplayName, "ValueCode", attribute);

        }

        // CompanyType2 := Create by: Attawit C. Note: For CompanyType Combobox in special case
        /// <summary>
        /// Generate company type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CompanyType2ComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbm_CompanyType> list = new List<tbm_CompanyType>();
            string strDisplayName = "CompanyTypeNameEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                list = handler.GetTbm_CompanyType();

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "CompanyTypeNameEN";
                }
                else
                {
                    strDisplayName = "CompanyTypeNameLC";
                }

            }
            catch
            {
                list = new List<tbm_CompanyType>();
            }

            return CommonUtil.CommonComboBox<tbm_CompanyType>(id, list, strDisplayName, new string[] { "CustNamePrefixEN", "CustNamePrefixLC", "CustNameSuffixEN", "CustNameSuffixLC" }, attribute);

        }

        /// <summary>
        /// Generate nationality combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString NationalityComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<tbm_Region> list = new List<tbm_Region>();
            string strDisplayName = "NationalityEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                list = handler.GetTbm_Region();

                var listOther = (from p in list where p.RegionNameEN.ToUpper() == "OTHER" select p).ToList<tbm_Region>();

                if (listOther.Count > 0)
                {
                    list.Remove(listOther[0]);
                }

                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "NationalityEN";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);

                    list = list.OrderBy(p => p.NationalityEN, StringComparer.Create(culture, false)).ToList();

                    //list = (from p in list orderby p.NationalityEN ascending select p).ToList<tbm_Region>();
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "NationalityJP";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_JP);

                    list = list.OrderBy(p => p.NationalityJP, StringComparer.Create(culture, false)).ToList();

                    //list = (from p in list orderby p.NationalityJP ascending select p).ToList<tbm_Region>();
                }
                else
                {
                    strDisplayName = "NationalityLC";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);

                    list = list.OrderBy(p => p.NationalityLC, StringComparer.Create(culture, false)).ToList();

                    //list = (from p in list orderby p.NationalityLC ascending select p).ToList<tbm_Region>();
                }

                if (listOther.Count > 0)
                {
                    list.AddRange(listOther);
                }


            }
            catch
            {
                list = new List<tbm_Region>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Region>(id, list, strDisplayName, "RegionCode", firstElement, attribute, include_idx0);

        }

        /// <summary>
        /// Generate business type code combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString BusinessTypeComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<tbm_BusinessType> list = new List<tbm_BusinessType>();
            string strDisplayName = "BusinessTypeNameEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                list = handler.GetTbm_BusinessType();

                List<tbm_BusinessType> listOther = (from p in list where p.BusinessTypeNameEN.ToUpper() == "OTHER" select p).ToList<tbm_BusinessType>();

                if (listOther.Count > 0)
                {
                    list.Remove(listOther[0]);
                }

                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "BusinessTypeNameEN";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                    list = list.OrderBy(p => p.BusinessTypeNameEN, StringComparer.Create(culture, false)).ToList();

                    //list = (from p in list orderby p.BusinessTypeNameEN ascending select p).ToList<tbm_BusinessType>();
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "BusinessTypeNameJP";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_JP);
                    list = list.OrderBy(p => p.BusinessTypeNameJP, StringComparer.Create(culture, false)).ToList();

                    //list = (from p in list orderby p.BusinessTypeNameJP ascending select p).ToList<tbm_BusinessType>();
                }
                else
                {
                    strDisplayName = "BusinessTypeNameLC";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                    list = list.OrderBy(p => p.BusinessTypeNameLC, StringComparer.Create(culture, false)).ToList();

                    //list = (from p in list orderby p.BusinessTypeNameLC ascending select p).ToList<tbm_BusinessType>();
                }

                if (listOther.Count > 0)
                {
                    list.AddRange(listOther);
                }
            }
            catch
            {
                list = new List<tbm_BusinessType>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_BusinessType>(id, list, strDisplayName, "BusinessTypeCode", firstElement, attribute, include_idx0);

        }

        /// <summary>
        /// Generate usage combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString UsageComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            List<tbm_BuildingUsage> list = new List<tbm_BuildingUsage>();
            string strDisplayName = "BuildingUsageNameEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                list = handler.GetTbm_BiuldingUsage();

                List<tbm_BuildingUsage> listOther = (from p in list where p.BuildingUsageNameEN.ToUpper() == "OTHER" select p).ToList<tbm_BuildingUsage>();

                if (listOther.Count > 0)
                {
                    list.Remove(listOther[0]);
                }

                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "BuildingUsageNameEN";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                    list = list.OrderBy(p => p.BuildingUsageNameEN, StringComparer.Create(culture, false)).ToList();

                    //list = (from p in list orderby p.BuildingUsageNameEN ascending select p).ToList<tbm_BuildingUsage>();

                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "BuildingUsageNameJP";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_JP);
                    list = list.OrderBy(p => p.BuildingUsageNameJP, StringComparer.Create(culture, false)).ToList();

                    //list = (from p in list orderby p.BuildingUsageNameJP ascending select p).ToList<tbm_BuildingUsage>();
                }
                else
                {
                    strDisplayName = "BuildingUsageNameLC";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                    list = list.OrderBy(p => p.BuildingUsageNameLC, StringComparer.Create(culture, false)).ToList();

                    //list = (from p in list orderby p.BuildingUsageNameLC ascending select p).ToList<tbm_BuildingUsage>();
                }

                if (listOther.Count > 0)
                {
                    list.AddRange(listOther);
                }

            }
            catch
            {
                list = new List<tbm_BuildingUsage>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_BuildingUsage>(id, list, strDisplayName, "BuildingUsageCode", firstElement, attribute);

        }

        /// <summary>
        /// Generate province EN combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ProVinceEnComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbm_Province> list = new List<tbm_Province>();
            string strDisplayName = "";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                list = handler.GetTbm_Province();
                strDisplayName = "ProvinceNameEN";

                var sortedList = from p in list
                                 orderby p.ProvinceNameEN
                                 select p;

                list = sortedList.ToList<tbm_Province>();
            }
            catch
            {
                list = new List<tbm_Province>();
            }

            return CommonUtil.CommonComboBox<tbm_Province>(id, list, strDisplayName, "ProvinceCode", attribute);

        }

        /// <summary>
        /// Generate province LC combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ProVinceLCComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbm_Province> list = new List<tbm_Province>();
            string strDisplayName = "";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                list = handler.GetTbm_Province();
                strDisplayName = "ProvinceNameLC";

                //var sortedList = from p in list
                //                 orderby p.ProvinceNameLC
                //                 select p;

                //list = sortedList.ToList<tbm_Province>();

                CultureInfo culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                list = list.OrderBy(p => p.ProvinceNameLC, StringComparer.Create(culture, false)).ToList();
            }
            catch
            {
                list = new List<tbm_Province>();
            }

            return CommonUtil.CommonComboBox<tbm_Province>(id, list, strDisplayName, "ProvinceCode", attribute);

        }

        // CompanyType := Create by Narupon W. Note: For nomally company type combobox (companyTypeCode : companyTypeName)
        /// <summary>
        /// Generate company type combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString CompanyTypeComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            List<tbm_CompanyType> list = new List<tbm_CompanyType>();
            string strDisplayName = "CompanyTypeNameEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                list = handler.GetTbm_CompanyType();

                List<tbm_CompanyType> listOther = (from p in list where p.CompanyTypeNameEN.ToUpper() == "OTHER" select p).ToList<tbm_CompanyType>();

                if (listOther.Count > 0)
                {
                    list.Remove(listOther[0]);
                }


                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    strDisplayName = "CompanyTypeNameEN";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                    list = list.OrderBy(p => p.CompanyTypeNameEN, StringComparer.Create(culture, false)).ToList();

                    //list = (from p in list orderby p.CompanyTypeNameEN ascending select p).ToList<tbm_CompanyType>();
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_LC)
                {
                    strDisplayName = "CompanyTypeNameLC";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                    list = list.OrderBy(p => p.CompanyTypeNameLC, StringComparer.Create(culture, false)).ToList();

                    //list = (from p in list orderby p.CompanyTypeNameLC ascending select p).ToList<tbm_CompanyType>();
                }
                else
                {
                    strDisplayName = "CompanyTypeNameEN";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                    list = list.OrderBy(p => p.CompanyTypeNameEN, StringComparer.Create(culture, false)).ToList();

                    //list = (from p in list orderby p.CompanyTypeNameEN ascending select p).ToList<tbm_CompanyType>();
                }

                if (listOther.Count > 0)
                {
                    list.AddRange(listOther);
                }

            }
            catch
            {
                list = new List<tbm_CompanyType>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_CompanyType>(id, list, strDisplayName, "CompanyTypeCode", firstElement, attribute);

        }

        /// <summary>
        /// Generate group code combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString GroupCodeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<View_tbm_Group> list = new List<View_tbm_Group>();
            string strDisplayName = "GroupCodeName";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                list = handler.GetDorp_Group();

            }
            catch
            {
                list = new List<View_tbm_Group>();
            }

            return CommonUtil.CommonComboBox<View_tbm_Group>(id, list, strDisplayName, "GroupCode", attribute);

        }

        /// <summary>
        /// Generate bank name and branch name combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BankNameBranchNameComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<dtBankBranch> lst = new List<dtBankBranch>();
            try
            {
                IMasterHandler handle = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                lst = handle.GetBankBranch();

                CommonUtil.MappingObjectLanguage<dtBankBranch>(lst);

                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);  // JP -> EN in case have 2 language
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_LC)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                }

                if (culture != null)
                {
                    lst = lst.OrderBy(p => p.BankNameBankBranchName, StringComparer.Create(culture, false)).ToList();
                }


            }
            catch
            {

            }

            return CommonUtil.CommonComboBox<dtBankBranch>(id, lst, "BankNameBankBranchName", "BankCodeBankBranchCode", attribute);
        }


        /// <summary>
        /// Create Position Combo Box for normal order by PositionCode and for special case set selected index to Max PositionLevel
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute">if want to set selected to max position level you must assign attribute { selected = "" }</param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString PositionComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<tbm_Position> list = new List<tbm_Position>();
            string strDisplayName = "PositionName";
            try
            {
                IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
                list = handler.GetTbm_Position();

                //int maxPositionIdx = 0;

                for (int i = 0; i < list.Count; i++)
                {
                    list[i].PositionName = CommonUtil.TextCodeName(list[i].PositionCode, list[i].PositionName);
                    //if (list[i].PositionLevel > list[maxPositionIdx].PositionLevel) {
                    //    maxPositionIdx = i;
                    //}
                }

                //tbm_Position maxLevel = list[maxPositionIdx];
                //list.Remove(maxLevel);
                //list.Insert(0, maxLevel);
            }
            catch (Exception e)
            {
                list = new List<tbm_Position>();
            }
            // Akat K. : use position code trimmed
            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Position>(id, list, strDisplayName, "PositionCodeTrim", firstElement, attribute, include_idx0);
        }

        /// <summary>
        /// Generate product combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString ProductComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<View_tbm_Product> list = new List<View_tbm_Product>();
            try
            {
                IProductMasterHandler handler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;

                list = handler.GetTbm_ProductByLanguage(null, null);
            }
            catch
            {
                list = new List<View_tbm_Product>();
            }
            return CommonUtil.CommonComboBoxWithCustomFirstElement<View_tbm_Product>(id, list, "ProductCodeName", "ProductCode", firstElement, attribute, include_idx0);
            //return CommonUtil.CommonComboBox<View_tbm_Product>(id, list, "ProductCodeName", "ProductCode", attribute, include_idx0);
        }

        /// <summary>
        /// Generate product combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="empNo"></param>
        /// <param name="productCode"></param>
        /// <param name="productTypeCode"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ProductComboBox(this HtmlHelper helper, string id, string empNo, string productCode, string productTypeCode, object attribute = null)
        {
            List<View_tbm_Product> lst = new List<View_tbm_Product>();
            try
            {
                bool isFoundUser = false;

                IProductMasterHandler handler = ServiceContainer.GetService<IProductMasterHandler>() as IProductMasterHandler;
                IEmployeeMasterHandler empHandler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;

                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                if (dsTrans != null)
                {
                    if (dsTrans.dtUserPermissionData != null)
                    {
                        string permissionKey = ScreenID.C_SCREEN_ID_QTN_DETAIL + "." + FunctionID.C_FUNC_ID_PLANNER;
                        if (dsTrans.dtUserPermissionData.ContainsKey(permissionKey) == true)
                            isFoundUser = true;
                    }
                }
                if (isFoundUser)
                    lst = handler.GetTbm_ProductByLanguage(null, productTypeCode);
                else
                    lst = handler.GetActiveProductbyLanguage(productCode, productTypeCode);
            }
            catch
            {
                lst = new List<View_tbm_Product>();
            }
            return CommonUtil.CommonComboBox<View_tbm_Product>(id, lst, "ProductCodeName", "ProductCode", attribute);

        }

        /// <summary>
        /// Generate person in charge combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="officeCode"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString PersonInChargeCombo(this HtmlHelper helper, string id, string officeCode, object attribute = null, string firstElement = null)
        {
            List<dtBelongingEmpNo> result = new List<dtBelongingEmpNo>();
            string strDisplayName = "EmpValueCode";
            // Akat K. : no need to show any
            //try {
            //    IEmployeeMasterHandler handler = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
            //    result = handler.GetBelongingEmpNoByOffice(officeCode);
            //    CommonUtil.MappingObjectLanguage<dtBelongingEmpNo>(result);
            //} catch {
            //    result = new List<dtBelongingEmpNo>();
            //}
            return CommonUtil.CommonComboBoxWithCustomFirstElement<dtBelongingEmpNo>(id, result, strDisplayName, "EmpNo", firstElement, attribute);
        }

        /// <summary>
        /// Generate module combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString ModuleComboBox(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            List<tbm_Module> list = new List<tbm_Module>();

            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;

                list = handler.GetTbm_Module();

            }
            catch
            {
                list = new List<tbm_Module>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Module>(id, list, "ModuleName", "ModuleID", firstElement, attribute);

        }
        //public static MvcHtmlString BelongOfficeComboBox(this HtmlHelper helper, string id, object attribute = null, string empNo = null)
        //{
        //    List<dtBelongingOffice> lst = new List<dtBelongingOffice>();
        //    List<dtBelongingOffice> vw_lst = new List<dtBelongingOffice>();
        //    try
        //    {
        //        IEmployeeMasterHandler handle = ServiceContainer.GetService<IEmployeeMasterHandler>() as IEmployeeMasterHandler;
        //        lst = handle.GetBelongingOfficeList(empNo);

        //        // Select language
        //        vw_lst = CommonUtil.ConvertObjectbyLanguage<dtBelongingOffice, dtBelongingOffice>(lst, "OfficeCodeName");

        //        vw_lst = (from p in vw_lst orderby p.OfficeCodeName ascending select p).ToList<dtBelongingOffice>();
        //    }
        //    catch
        //    {
        //        lst = new List<dtBelongingOffice>();
        //    }

        //    return CommonUtil.CommonComboBox<dtBelongingOffice>(id, vw_lst, "OfficeCodeName", "OfficeCode", attribute);

        //}

        /// <summary>
        /// Generate operation office MA combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString OperationOfficeMAComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            List<OfficeDataDo> lst = new List<OfficeDataDo>();

            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                List<dtOffice> clst = handler.GetFunctionSecurity();
                if (clst.Count > 0)
                    lst = CommonUtil.ConvertObjectbyLanguage<dtOffice, OfficeDataDo>(clst, "OfficeName");
            }
            catch
            {
            }

            return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);
        }

        /// <summary>
        /// Generate operation office combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString OperationOfficeComboBoxWithFirstElement(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<OfficeDataDo> lst = new List<OfficeDataDo>();

            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                List<dtOffice> clst = handler.GetFunctionSecurity();
                if (clst.Count > 0)
                    lst = CommonUtil.ConvertObjectbyLanguage<dtOffice, OfficeDataDo>(clst, "OfficeName");
            }
            catch
            {
            }

            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", firstElement, attribute, include_idx0);
        }

        /// <summary>
        /// Generate bank combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString BankComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<tbm_Bank> list = new List<tbm_Bank>();

            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbm_Bank();

                foreach (var item in list)
                {
                    // Check user aplication launguage
                    if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                    {
                        item.BankName = item.BankNameEN;
                    }
                    else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                    {
                        item.BankName = item.BankNameEN;
                    }
                    else
                    {
                        item.BankName = item.BankNameLC;
                    }
                }

                CultureInfo culture = null;
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                    list = list.OrderBy(p => p.BankName, StringComparer.Create(culture, false)).ToList();
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN); // Note BankName have only EN , LC then JP set to EN
                    list = list.OrderBy(p => p.BankName, StringComparer.Create(culture, false)).ToList();
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_LC)
                {
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                    list = list.OrderBy(p => p.BankName, StringComparer.Create(culture, false)).ToList();
                }

            }
            catch
            {
            }

            //Modified by phumipat (3/Apr/2012) for extended custom first element
            //return CommonUtil.CommonComboBox<tbm_Bank>(id, list, "BankName", "BankCode", attribute, include_idx0);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Bank>(id, list, "BankName", "BankCode", firstElement, attribute, include_idx0);
        }

        /// <summary>
        /// Generate credit card company combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString CreditCardCompanyComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            List<tbm_CreditCardCompany> list = new List<tbm_CreditCardCompany>();
            string strDisplayName = "CreditCardCompanyNameEN";
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetTbm_CreditCardCompany();
                CultureInfo culture = null;

                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN
                    || CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    strDisplayName = "CreditCardCompanyNameEN";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_EN);
                    list = list.OrderBy(p => p.CreditCardCompanyNameEN, StringComparer.Create(culture, false)).ToList();
                }
                else
                {
                    strDisplayName = "CreditCardCompanyNameLC";
                    culture = new CultureInfo(CommonValue.DEFAULT_LANGUAGE_LC);
                    list = list.OrderBy(p => p.CreditCardCompanyNameLC, StringComparer.Create(culture, false)).ToList();
                }
            }
            catch
            {
                list = new List<tbm_CreditCardCompany>();
            }
            return CommonUtil.CommonComboBox<tbm_CreditCardCompany>(id, list, strDisplayName, "CreditCardCompanyCode", attribute, include_idx0);
        }

        #region Normal Office Combobox (New version)

        /// <summary>
        /// Generate all office normal combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString AllOfficeNormalCbo(this HtmlHelper helper, string id, object attribute = null, string firstElement = null, bool include_idx0 = true)
        {
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                list = handler.GetTbm_Office();
                CommonUtil.MappingObjectLanguage<tbm_Office>(list);
            }
            catch
            {
                list = new List<tbm_Office>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Office>(id, list, "OfficeDisplay", "OfficeCode", firstElement, attribute, include_idx0);
        }

        /// <summary>
        /// Generate contract normal combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString ContractOfficeNormalCbo(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            //IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            //var allOffice = officehandler.GetFunctionSale(FunctionSale.C_FUNC_SALE_NO);
            //var rawResult = from a in allOffice
            //                select new OfficeDataDo
            //                {
            //                    OfficeCode = a.OfficeCode,
            //                    OfficeName = String.Empty,
            //                    OfficeNameEN = a.OfficeNameEN,
            //                    OfficeNameJP = a.OfficeNameJP,
            //                    OfficeNameLC = a.OfficeNameLC
            //                };
            //var result = CommonUtil.ConvertObjectbyLanguage<OfficeDataDo, OfficeDataDo>(rawResult.ToList(), "OfficeName");
            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, result.ToList(), "OfficeCodeName", "OfficeCode", attribute);


            // Edit by Narupon W. 31/Jan/2012
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                // Get all office data
                list = handler.GetTbm_Office();

                // Filter ==> ContractOfficeNormalCbo where ==> FunctionSale <> C_FUNC_SALE_NO
                list = (from p in list where p.FunctionSale != FunctionSale.C_FUNC_SALE_NO select p).ToList<tbm_Office>();

                // Language mappping
                CommonUtil.MappingObjectLanguage<tbm_Office>(list);
            }
            catch
            {
                list = new List<tbm_Office>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Office>(id, list, "OfficeDisplay", "OfficeCode", firstElement, attribute);

        }

        /// <summary>
        /// Generate operation office normal combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString OperationOfficeNormalCbo(this HtmlHelper helper, string id, object attribute = null, string firstElement = null)
        {
            //IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            //var allOffice = officehandler.GetFunctionSecurity(FunctionSecurity.C_FUNC_SECURITY_NO);
            //var rawResult = from a in allOffice
            //                select new OfficeDataDo
            //                {
            //                    OfficeCode = a.OfficeCode,
            //                    OfficeName = String.Empty,
            //                    OfficeNameEN = a.OfficeNameEN,
            //                    OfficeNameJP = a.OfficeNameJP,
            //                    OfficeNameLC = a.OfficeNameLC
            //                };
            //var result = CommonUtil.ConvertObjectbyLanguage<OfficeDataDo, OfficeDataDo>(rawResult.ToList(), "OfficeName");
            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, result.ToList(), "OfficeCodeName", "OfficeCode", attribute);


            // Edit by Narupon W. 31/Jan/2012
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                // Get all office data
                list = handler.GetTbm_Office();

                // Filter ==> OperationOfficeNormalCbo where ==> FunctionSecurity <> C_FUNC_SECURITY_NO
                list = (from p in list where p.FunctionSecurity != FunctionSecurity.C_FUNC_SECURITY_NO select p).ToList<tbm_Office>();

                // Language mappping
                CommonUtil.MappingObjectLanguage<tbm_Office>(list);
            }
            catch
            {
                list = new List<tbm_Office>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Office>(id, list, "OfficeDisplay", "OfficeCode", firstElement, attribute);

        }

        /// <summary>
        /// Generate incident normal combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString IncidentOfficeNormalCbo(this HtmlHelper helper, string id, object attribute = null, string firstElement = null, bool include_idx0 = true)
        {
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                list = handler.GetIncidentOffice();
                CommonUtil.MappingObjectLanguage<tbm_Office>(list);
            }
            catch
            {
                list = new List<tbm_Office>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Office>(id, list, "OfficeDisplay", "OfficeCode", firstElement, attribute, include_idx0);
        }

        /// <summary>
        /// Generate AR office normal combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="firstElement"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString AROfficeNormalCbo(this HtmlHelper helper, string id, object attribute = null, string firstElement = null, bool include_idx0 = true)
        {
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                list = handler.GetAROffice();
                CommonUtil.MappingObjectLanguage<tbm_Office>(list);
            }
            catch
            {
                list = new List<tbm_Office>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Office>(id, list, "OfficeDisplay", "OfficeCode", firstElement, attribute, include_idx0);
        }

        /// <summary>
        /// Generate billing office normal combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingOfficeNormalCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true)
        {
            //List<OfficeDataDo> lst = new List<OfficeDataDo>();
            //try
            //{
            //    IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

            //    List<doFunctionBilling> clst = handler.GetFunctionBilling();
            //    if (clst.Count > 0)
            //        lst = CommonUtil.ConvertObjectbyLanguage<doFunctionBilling, OfficeDataDo>(clst, "OfficeName");
            //}
            //catch
            //{
            //}
            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, lst, "OfficeCodeName", "OfficeCode", attribute, include_idx0);

            // Edit by Narupon W. 31/Jan/2012
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                // Get all office data
                list = handler.GetTbm_Office();

                // Filter ==> BillingOfficeNormalCbo where ==> FunctionBilling <> C_FUNC_BILLING_NO
                list = (from p in list where p.FunctionBilling != FunctionBilling.C_FUNC_BILLING_NO select p).ToList<tbm_Office>();

                // Language mappping
                CommonUtil.MappingObjectLanguage<tbm_Office>(list);
            }
            catch
            {
                list = new List<tbm_Office>();
            }

            return CommonUtil.CommonComboBox<tbm_Office>(id, list, "OfficeDisplay", "OfficeCode", attribute);

        }

        /// <summary>
        /// Generate inventory office normal combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InventoryOfficeNormalCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            //IOfficeMasterHandler officehandler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            //var allOffice = officehandler.GetFunctionSecurity(FunctionLogistic.C_FUNC_LOGISTIC_NO);
            //var rawResult = from a in allOffice
            //                select new OfficeDataDo
            //                {
            //                    OfficeCode = a.OfficeCode,
            //                    OfficeName = String.Empty,
            //                    OfficeNameEN = a.OfficeNameEN,
            //                    OfficeNameJP = a.OfficeNameJP,
            //                    OfficeNameLC = a.OfficeNameLC
            //                };
            //var result = CommonUtil.ConvertObjectbyLanguage<OfficeDataDo, OfficeDataDo>(rawResult.ToList(), "OfficeName");
            //return CommonUtil.CommonComboBox<OfficeDataDo>(id, result.ToList(), "OfficeCodeName", "OfficeCode", attribute);

            // Edit by Narupon W. 31/Jan/2012
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                // Get all office data
                list = handler.GetTbm_Office();

                // Filter ==> InventoryOfficeNormalCbo where ==> FunctionLogistic <> C_FUNC_LOGISTIC_NO
                list = (from p in list where p.FunctionLogistic != FunctionLogistic.C_FUNC_LOGISTIC_NO select p).ToList<tbm_Office>();

                // Language mappping
                CommonUtil.MappingObjectLanguage<tbm_Office>(list);
            }
            catch
            {
                list = new List<tbm_Office>();
            }

            return CommonUtil.CommonComboBox<tbm_Office>(id, list, "OfficeDisplay", "OfficeCode", attribute);

        }

        /// <summary>
        /// Generate installation issue office normal combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InstallSipIssueOfficeNormalCbo(this HtmlHelper helper, string id, object attribute = null)
        {


            // Edit by Narupon W. 31/Jan/2012
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                // Get all office data
                list = handler.GetTbm_Office();

                // Filter ==> InstallSipIssueOfficeNormalCbo where ==> FunctionSecurity <> C_FUNC_SECURITY_NO
                list = (from p in list where p.FunctionSecurity != FunctionSecurity.C_FUNC_SECURITY_NO select p).ToList<tbm_Office>();

                // Language mappping
                CommonUtil.MappingObjectLanguage<tbm_Office>(list);
            }
            catch
            {
                list = new List<tbm_Office>();
            }

            return CommonUtil.CommonComboBox<tbm_Office>(id, list, "OfficeDisplay", "OfficeCode", attribute);

        }

        /// <summary>
        /// Generate inventory issue office normal combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString InventorySipIssueOfficeNormalCbo(this HtmlHelper helper, string id, object attribute = null)
        {

            // Edit by Narupon W. 31/Jan/2012
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                // Get all office data
                list = handler.GetTbm_Office();

                // Filter ==> InventorySipIssueOfficeNormalCbo where ==> FunctionLogistic <> C_FUNC_LOGISTIC_NO
                list = (from p in list where p.FunctionLogistic != FunctionLogistic.C_FUNC_LOGISTIC_NO select p).ToList<tbm_Office>();

                // Language mappping
                CommonUtil.MappingObjectLanguage<tbm_Office>(list);
            }
            catch
            {
                list = new List<tbm_Office>();
            }

            return CommonUtil.CommonComboBox<tbm_Office>(id, list, "OfficeDisplay", "OfficeCode", attribute);

        }

        /// <summary>
        /// Generate billing office normal combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BillingOfficeNormalCbo(this HtmlHelper helper, string id, object attribute = null)
        {
            IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
            List<doFunctionBilling> clst = handler.GetFunctionBilling();
            clst = CommonUtil.ConvertObjectbyLanguage<doFunctionBilling, doFunctionBilling>(clst, "OfficeName");

            string strDisplayMember = "OfficeCodeName";
            string strValueMember = "OfficeCode";

            return CommonUtil.CommonComboBox<doFunctionBilling>(id, clst, strDisplayMember, strValueMember, attribute);
        }

        public static MvcHtmlString BillingDebtTracingOfficeNormalCbo(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;

                list = handler.GetTbm_Office();
                list = (from p in list where p.FunctionDebtTracing == "1" select p).ToList<tbm_Office>();

                CommonUtil.MappingObjectLanguage<tbm_Office>(list);
            }
            catch
            {
                list = new List<tbm_Office>();
            }

            return CommonUtil.CommonComboBoxWithCustomFirstElement<tbm_Office>(id, list, "OfficeDisplay", "OfficeCode", firstElement, attribute, include_idx0);
        }

        // temp wait for move after complete
        // waroon h.
        /// <summary>
        /// Generate collection area checkbutton list
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="check_val"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CollectionAreaCheckList(this HtmlHelper helper,
                                                            string id,
                                                            string[] check_val = null,
                                                            object attribute = null)
        {

            List<tbm_Office> list = new List<tbm_Office>();
            try
            {
                IOfficeMasterHandler handler = ServiceContainer.GetService<IOfficeMasterHandler>() as IOfficeMasterHandler;
                if (handler != null)
                {
                    list = handler.GetTbm_Office();
                    list = (from p in list where p.FunctionSecurity != FunctionSecurity.C_FUNC_SECURITY_NO select p).ToList<tbm_Office>();
                    CommonUtil.MappingObjectLanguage<tbm_Office>(list);
                }
            }
            catch
            {

            }

            return CommonUtil.CommonCheckButtonList<tbm_Office>(id, null, list, "OfficeDisplay", "OfficeCode", false, check_val, attribute);
        }

        #endregion


        /// <summary>
        /// Generate SECOM bank account combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString SECOMAccountComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<doSECOMAccount> list = new List<doSECOMAccount>();
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetSECOMAccount();
            }
            catch
            {
            }
            return CommonUtil.CommonComboBoxWithCustomFirstElement<doSECOMAccount>(id, list, "Text", "SecomAccountID", firstElement, attribute, include_idx0);
        }


        /// <summary>
        /// Generate SECOM bank account (bank transfer) combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString SECOMAccountBankTransferComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<doSECOMAccount> list = new List<doSECOMAccount>();
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetSECOMAccountBankTransfer();
            }
            catch
            {
            }
            return CommonUtil.CommonComboBoxWithCustomFirstElement<doSECOMAccount>(id, list, "Text", "SecomAccountID", firstElement, attribute, include_idx0);
        }

        /// <summary>
        /// Generate SECOM bank account (auto transfer) combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString SECOMAccountAutoTransferComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<doSECOMAccount> list = new List<doSECOMAccount>();
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetSECOMAccountAutoTransfer();
            }
            catch
            {
            }
            return CommonUtil.CommonComboBoxWithCustomFirstElement<doSECOMAccount>(id, list, "Text", "SecomAccountID", firstElement, attribute, include_idx0);
        }

        /// <summary>
        /// Generate SECOM bank account (auto transfer) combobox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="firstElement"></param>
        /// <returns></returns>
        public static MvcHtmlString SECOMAccountDummyTransferComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<doSECOMAccount> list = new List<doSECOMAccount>();
            try
            {
                IMasterHandler handler = ServiceContainer.GetService<IMasterHandler>() as IMasterHandler;
                list = handler.GetSECOMAccountDummyTransfer();
            }
            catch
            {
            }
            return CommonUtil.CommonComboBoxWithCustomFirstElement<doSECOMAccount>(id, list, "Text", "SecomAccountID", firstElement, attribute, include_idx0);
        }
    }
}
