using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

using CSI.WindsorHelper;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Presentation.Income.Models;

namespace SECOM_AJIS.Presentation.Income.Helpers
{
    public static partial class ComboBoxHelper
    {
        /// <summary>
        /// Generate html of combobox for day list (1-31)
        /// </summary>
        /// <param name="helper">html helper</param>
        /// <param name="id">contorl id</param>
        /// <param name="attribute">html attribute</param>
        /// <param name="include_idx0">is display first combobox item</param>
        /// <param name="firstElement">first combobox item</param>
        /// <returns></returns>
        public static MvcHtmlString DayComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<SelectListItem> dayList = new List<SelectListItem>();
            try
            {
                for (int d = 1; d < 32; d++)
                    dayList.Add(new SelectListItem()
                    {
                        Text = d.ToString(),
                        Value = d.ToString()
                    });
            }
            catch
            { }
            //return CommonUtil.CommonComboBox<SelectListItem>(id, dayList, "Text", "Value", attribute);
            return CommonUtil.CommonComboBoxWithCustomFirstElement<SelectListItem>(id, dayList, "Text", "Value", firstElement, attribute, include_idx0);
        }

        // use in ICS030
        // select backward 25 month
        /// <summary>
        /// Generate html of combobox for month/year list
        /// </summary>
        /// <param name="helper">html helper</param>
        /// <param name="id">control id</param>
        /// <param name="attribute">html attribute</param>
        /// <param name="include_idx0">is display first combobox item</param>
        /// <param name="firstElement">first combobox item</param>
        /// <returns></returns>
        public static MvcHtmlString MonthYearComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<SelectListItem> MonthYearList = new List<SelectListItem>();
            try
            {
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;

                //int j = iincomeHandler.GetWorkingDayNoOfMonth(DateTime.Now);
                int j = DateTime.Now.Day;
                if (iincomeHandler.GetWorkingDayNoOfMonth(DateTime.Now) <= 6)
                {
                    j = 1;
                }
                else
                {
                    j = 0;
                }

                for (int i = 0; i < 25; i++)
                {
                    if (i == j)
                    {
                        MonthYearList.Add(new SelectListItem()
                        {
                            Selected = true,
                            Text = ToMonthName(DateTime.Now.AddMonths((-1) * i)) + "-" + DateTime.Now.AddMonths((-1) * i).Year.ToString(),
                            Value = DateTime.Now.AddMonths((-1) * i).ToShortDateString()
                        });
                    }
                    else
                    {
                        MonthYearList.Add(new SelectListItem()
                        {
                            Selected = false,
                            Text = ToMonthName(DateTime.Now.AddMonths((-1) * i)) + "-" + DateTime.Now.AddMonths((-1) * i).Year.ToString(),
                            Value = DateTime.Now.AddMonths((-1) * i).ToShortDateString()
                        });
                    }
                };
            }
            catch
            { }
            return CommonUtil.CommonComboBox<SelectListItem>(id, MonthYearList, "Text", "Value", attribute, false);
        }
        /// <summary>
        /// Retrieve month name of specific datetime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }

        // temp DDS not complete
        /// <summary>
        /// Generate html of combobox for tracing result information list
        /// </summary>
        /// <param name="helper">html helper</param>
        /// <param name="id">control id</param>
        /// <param name="attribute">html attribute</param>
        /// <returns></returns>
        public static MvcHtmlString TracingResultComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            //return MiscTypeComboBox(helper, id, MiscType.C_BILLING_FLAG, attribute, "ValueDisplay");
            return MiscTypeComboBox(helper, id, "TracingResult", attribute, "ValueDisplay");
        }
        /// <summary>
        /// Generate html of combobox for miscellaneousTypeCode infomation of specific field name
        /// </summary>
        /// <param name="helper">html helper</param>
        /// <param name="id">control id</param>
        /// <param name="fieldName">field name</param>
        /// <param name="attribute">html attribute</param>
        /// <param name="display_field">specific display field name</param>
        /// <param name="include_idx0">is display first combobox item</param>
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

        /// <summary>
        /// Generate html of combobox for billing type information which display billing type code name
        /// </summary>
        /// <param name="helper">html helper</param> 
        /// <param name="id">control id</param>
        /// <param name="attribute">html attribute</param>
        /// <returns></returns>
        public static MvcHtmlString BillingTypeComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbm_BillingType> vw_list = new List<tbm_BillingType>();
            IBillingMasterHandler masHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;

            List<tbm_BillingType> listOrder = masHandler.GetTbm_BillingType();

            vw_list = CommonUtil.ConvertObjectbyLanguage<tbm_BillingType, tbm_BillingType>(listOrder, "BillingTypeName");

            return CommonUtil.CommonComboBox<tbm_BillingType>(id, vw_list, "BillingTypeCodeName", "BillingTypeCode", attribute);
        }
        /// <summary>
        /// Generate html of combobox for billing type information which display billing type name
        /// </summary>
        /// <param name="helper">html helper</param>
        /// <param name="id">control id</param>
        /// <param name="attribute">html attribute</param>
        /// <returns></returns>
        public static MvcHtmlString BillingTypeNameComboBox(this HtmlHelper helper, string id, object attribute = null)
        {
            List<tbm_BillingType> vw_list = new List<tbm_BillingType>();
            IBillingMasterHandler masHandler = ServiceContainer.GetService<IBillingMasterHandler>() as IBillingMasterHandler;

            List<tbm_BillingType> listOrder = masHandler.GetTbm_BillingType();

            vw_list = CommonUtil.ConvertObjectbyLanguage<tbm_BillingType, tbm_BillingType>(listOrder, "BillingTypeName");

            return CommonUtil.CommonComboBox<tbm_BillingType>(id, vw_list, "BillingTypeName", "BillingTypeCode", attribute);
        }

        public static MvcHtmlString WHTMonthYearComboBox(this HtmlHelper helper, string id, object attribute = null, bool include_idx0 = true, string firstElement = null)
        {
            List<SelectListItem> MonthYearList = new List<SelectListItem>();
            try
            {
                IIncomeHandler iincomeHandler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                var yearmonth = iincomeHandler.GetWHTYearMonth();
                var thismonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                if (yearmonth.MINYEARMONTH == null) yearmonth.MINYEARMONTH = thismonth;
                if (yearmonth.MAXYEARMONTH == null) yearmonth.MAXYEARMONTH = thismonth;

                for (DateTime dt = yearmonth.MAXYEARMONTH.Value; dt >= yearmonth.MINYEARMONTH; dt = dt.AddMonths(-1))
                {
                    MonthYearList.Add(new SelectListItem()
                    {
                        Selected = (dt == thismonth),
                        Text = ToMonthName(dt) + "-" + dt.Year.ToString(),
                        Value = dt.ToShortDateString()
                    });
                };
            }
            catch
            { }
            return CommonUtil.CommonComboBox<SelectListItem>(id, MonthYearList, "Text", "Value", attribute, false);
        }


        /// <summary>
        /// Billing Office from tbm_DebtTracingPermission
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DebtTracingPermissionCombo(this HtmlHelper helper, string id, object attribute = null)
        {
            List<doDebtTracingPermission> list = new List<doDebtTracingPermission>();

            if (CommonUtil.dsTransData != null)
            {
                var handler = ServiceContainer.GetService<IIncomeHandler>() as IIncomeHandler;
                list = handler.GetDebtTracingPermission(CommonUtil.dsTransData.dtUserData.EmpNo);
                CommonUtil.MappingObjectLanguage<doDebtTracingPermission>(list);
            }

            string strDisplayMember = "OfficeDisplay";
            string strValueMember = "OfficeCode";

            return CommonUtil.CommonComboBox<doDebtTracingPermission>(id, list, strDisplayMember, strValueMember, attribute, true, CommonUtil.eFirstElementType.All);
        }

        /// <summary>
        /// Debt tracing result
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DebtTracingResultCombo(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_DEBT_TRACING_RESULT);
            List<doMiscTypeCode> dtMisc = comh.GetMiscTypeCodeListByFieldName(lst);

            if (CommonUtil.dsTransData != null)
            {
                if (!CommonUtil.dsTransData.ContainsPermission(ScreenID.C_SCREEN_ID_DEBT_TRACING, FunctionID.C_FUNC_ID_TRANSFER_TO_BRANCH))
                {
                    dtMisc = dtMisc.Where(d => 
                        d.ValueCode != DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_BRANCH
                        && d.ValueCode != DebtTracingResult.C_DEBT_TRACE_RESULT_TRANSFER_TO_HQ
                    ).ToList();
                }
            }

            CommonUtil.eFirstElementType idx0_type = CommonUtil.eFirstElementType.Select;
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtMisc, "ValueCodeDisplay", "ValueCode", attribute, true, idx0_type);
        }

        /// <summary>
        /// Debt tracing result
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString DebtTracingPostponeReasonCombo(this HtmlHelper helper, string id, object attribute = null)
        {
            ICommonHandler comh = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            List<string> lst = new List<string>();
            lst.Add(MiscType.C_DEBT_TRACING_POSTPONE_REASON);
            List<doMiscTypeCode> dtTransportType = comh.GetMiscTypeCodeListByFieldName(lst);

            CommonUtil.eFirstElementType idx0_type = CommonUtil.eFirstElementType.Select;
            return CommonUtil.CommonComboBox<doMiscTypeCode>(id, dtTransportType, "ValueCodeDisplay", "ValueCode", attribute, true, idx0_type);
        }
    }
}
