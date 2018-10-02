using System;
using System.Web.WebPages;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using Microsoft.Security.Application;

namespace SECOM_AJIS.Common.Helpers
{
    public static partial class TextBoxHelper
    {
        public enum NUMERIC_UNIT_TYPE
        {
            CURRENCY,
            TIMES,
            PERSONS,
            MINUTES,
            HOURS
        }
        
        /// <summary>
        /// Generate textbox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CommonTextBox(this HtmlHelper helper, string id, object value = null, object attribute = null)
        {
            bool isViewMode = false;
            if (attribute != null)
            {
                PropertyInfo prop = attribute.GetType().GetProperty("isViewMode");
                if (prop != null)
                {
                    object val = prop.GetValue(attribute, null);
                    if (val != null)
                    {
                        bool mode = false;
                        if (bool.TryParse(val.ToString(), out mode))
                            isViewMode = mode;
                    }
                }
            }

            if (isViewMode)
            {
                var spanBuilder = new TagBuilder("div");
                spanBuilder.MergeAttribute("class", "label-view");
                // Akat K. 2011-08-01 : Use in case of ViewMode have to change data depend on button click
                spanBuilder.MergeAttribute("id", id);

                if (value != null)
                {
                    string txt = value.ToString();
                    string htmlEndcode = Encoder.HtmlEncode(txt);

                    // allow for BR tag (<br/>)
                    //htmlEndcode = htmlEndcode.Replace("&lt;br/&gt;", "<br/>").Replace("&lt;BR/&gt;", "BR/").Replace("&lt;br&gt;", "<br>").Replace("&lt;BR&gt;","<BR>");
                    htmlEndcode = htmlEndcode.Replace("&#10;", "<br/>");

                    if (txt.Trim() != string.Empty)
                        spanBuilder.InnerHtml = htmlEndcode;
                    else
                        spanBuilder.InnerHtml = "&nbsp;";
                }
                else
                    spanBuilder.InnerHtml = "&nbsp;";

                return MvcHtmlString.Create(spanBuilder.ToString(TagRenderMode.Normal));
            }
            else
            {
                var inputBuilder = new TagBuilder("input");
                inputBuilder.MergeAttribute("type", "text");
                inputBuilder.MergeAttribute("id", id);
                inputBuilder.MergeAttribute("name", id);

                if (value != null)
                    inputBuilder.MergeAttribute("value", value.ToString());
                if (attribute != null)
                    CommonUtil.SetHtmlTagAttribute(inputBuilder, attribute);

                return MvcHtmlString.Create(inputBuilder.ToString(TagRenderMode.SelfClosing));
            }
        }
        /// <summary>
        /// Generate textarea
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CommonTextArea(this HtmlHelper helper, string id, string value = null, object attribute = null)
        {
            var inputBuilder = new TagBuilder("textarea");
            inputBuilder.MergeAttribute("id", id);
            inputBuilder.MergeAttribute("name", id);

            if (value != null)
                inputBuilder.InnerHtml = value;
            if (attribute != null)
                CommonUtil.SetHtmlTagAttribute(inputBuilder, attribute);

            return MvcHtmlString.Create(inputBuilder.ToString(TagRenderMode.Normal));
        }
        /// <summary>
        /// Generate textbox in case of auto complete text
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString TextAutoComplete(this HtmlHelper helper, string id, string label, object attribute = null)
        {
            // build the <div> tag
            var divBuilder = new TagBuilder("div");
            if (attribute != null)
                CommonUtil.SetHtmlTagAttribute(divBuilder, attribute);

            // build the <div> tag
            var divSBuilder = new TagBuilder("div");
            divSBuilder.MergeAttribute("class", "ui-widget");

            // build the <label> tag
            var labelBuilder = new TagBuilder("label");
            labelBuilder.MergeAttribute("for", id);
            labelBuilder.InnerHtml = label;
            divSBuilder.InnerHtml += labelBuilder.ToString(TagRenderMode.Normal);

            //build the <input> tag
            var inputBuilder = new TagBuilder("input");
            inputBuilder.MergeAttribute("id", id);
            divSBuilder.InnerHtml += inputBuilder.ToString(TagRenderMode.SelfClosing);

            divBuilder.InnerHtml = divSBuilder.ToString(TagRenderMode.Normal);


            string anchorHtml = divBuilder.ToString(TagRenderMode.Normal);
            return MvcHtmlString.Create(anchorHtml);
        }
        /// <summary>
        /// Generate require field label
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static MvcHtmlString RequireFiled(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<span class=\"label-remark\">*</span>");
        }
        /// <summary>
        /// Generate textbox in case of numeric text
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString NumericTextBox(this HtmlHelper helper, string id, object value = null, object attribute = null)
        {
            bool isViewMode = false;
            if (attribute != null)
            {
                PropertyInfo prop = attribute.GetType().GetProperty("isViewMode");
                if (prop != null)
                {
                    bool mode = false;
                    if (bool.TryParse(prop.GetValue(attribute, null).ToString(), out mode))
                        isViewMode = mode;
                }
            }

            if (isViewMode)
            {
                var spanBuilder = new TagBuilder("div");
                spanBuilder.MergeAttribute("class", "label-view");

                if (value != null)
                {
                    string txt = value.ToString();
                    if (txt.Trim() != string.Empty)
                        spanBuilder.InnerHtml = txt;
                    else
                        spanBuilder.InnerHtml = "&nbsp;";
                }
                else
                    spanBuilder.InnerHtml = "&nbsp;";

                return MvcHtmlString.Create(spanBuilder.ToString(TagRenderMode.Normal));
            }
            else
            {
                var inputBuilder = new TagBuilder("input");
                inputBuilder.MergeAttribute("type", "text");
                inputBuilder.MergeAttribute("id", id);
                inputBuilder.MergeAttribute("name", id);
                inputBuilder.MergeAttribute("class", "numeric-box");

                if (value != null)
                    inputBuilder.MergeAttribute("value", value.ToString());
                if (attribute != null)
                    CommonUtil.SetHtmlTagAttribute(inputBuilder, attribute);

                return MvcHtmlString.Create(inputBuilder.ToString(TagRenderMode.SelfClosing));
            }
        }
        /// <summary>
        /// Generate textbox in case of numeric text (in clude unit)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString NumericTextBoxWithUnit(this HtmlHelper helper, string id, object value = null, object attribute = null, SECOM_AJIS.Common.Helpers.TextBoxHelper.NUMERIC_UNIT_TYPE unitType = SECOM_AJIS.Common.Helpers.TextBoxHelper.NUMERIC_UNIT_TYPE.CURRENCY, bool required = false)
        {
            bool isViewMode = false;
            if (attribute != null)
            {
                PropertyInfo prop = attribute.GetType().GetProperty("isViewMode");
                if (prop != null)
                {
                    bool mode = false;
                    if (bool.TryParse(prop.GetValue(attribute, null).ToString(), out mode))
                        isViewMode = mode;
                }
            }


            string txtNumeric = string.Empty;
            if (isViewMode)
            {
                var spanBuilder = new TagBuilder("div");
                spanBuilder.MergeAttribute("class", "label-view");

                if (value != null)
                {
                    string txt = value.ToString();
                    if (txt.Trim() != string.Empty)
                        spanBuilder.InnerHtml = txt;
                    else
                        spanBuilder.InnerHtml = "&nbsp;";
                }
                else
                    spanBuilder.InnerHtml = "&nbsp;";

                txtNumeric = spanBuilder.ToString(TagRenderMode.Normal);
            }
            else
            {
                var inputBuilder = new TagBuilder("input");
                inputBuilder.MergeAttribute("type", "text");
                inputBuilder.MergeAttribute("id", id);
                inputBuilder.MergeAttribute("name", id);
                inputBuilder.MergeAttribute("class", "numeric-box");

                if (value != null)
                    inputBuilder.MergeAttribute("value", value.ToString());
                if (attribute != null)
                    CommonUtil.SetHtmlTagAttribute(inputBuilder, attribute);

                txtNumeric = inputBuilder.ToString(TagRenderMode.SelfClosing);
            }

            string unit = "";
            if (unitType == SECOM_AJIS.Common.Helpers.TextBoxHelper.NUMERIC_UNIT_TYPE.CURRENCY)
                unit = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CommonResources", "lblTHB");
            if (unitType == SECOM_AJIS.Common.Helpers.TextBoxHelper.NUMERIC_UNIT_TYPE.TIMES)
                unit = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CommonResources", "lblTimes");
            else if (unitType == SECOM_AJIS.Common.Helpers.TextBoxHelper.NUMERIC_UNIT_TYPE.PERSONS)
                unit = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CommonResources", "lblPersons");
            else if (unitType == SECOM_AJIS.Common.Helpers.TextBoxHelper.NUMERIC_UNIT_TYPE.MINUTES)
                unit = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CommonResources", "lblMinutes");
            else if (unitType == SECOM_AJIS.Common.Helpers.TextBoxHelper.NUMERIC_UNIT_TYPE.HOURS)
                unit = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CommonResources", "lblHours");

            string div = string.Format("<div id=\"unit{0}\" class=\"label-unit\">{1}</div>", id, unit);

            string divr = string.Empty;
            if (required == true)
                divr = "&nbsp;<span class=\"label-remark\">*</span>";

            txtNumeric = string.Format("<div class=\"object-unit\">{0}</div>{1}{2}", txtNumeric, div, divr);

            return MvcHtmlString.Create(txtNumeric);
        }
        /// <summary>
        /// Generate textbox in case of numeric text (in clude unit)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        //public static MvcHtmlString NumericTextBoxWithMultipleCurrency(this HtmlHelper helper, string id, object value = null, string currency = "1", object attribute = null, MULTIPLE_CURRENCY_TYPE currencyType = MULTIPLE_CURRENCY_TYPE.COMBOBOX, bool required = false)
        //{
        //    bool isViewMode = false;
        //    if (attribute != null)
        //    {
        //        PropertyInfo prop = attribute.GetType().GetProperty("isViewMode");
        //        if (prop != null)
        //        {
        //            bool mode = false;
        //            if (bool.TryParse(prop.GetValue(attribute, null).ToString(), out mode))
        //                isViewMode = mode;
        //        }
        //    }

        //    Dictionary<string, string> currencies = new Dictionary<string, string>();
        //    currencies.Add("1", "Rp.");
        //    currencies.Add("2", "US$");

        //    string txtNumeric = string.Empty;
        //    string txtUnit = string.Empty;
        //    if (isViewMode)
        //    {
        //        var spanBuilder = new TagBuilder("div");
        //        spanBuilder.MergeAttribute("class", "label-view");

        //        if (value != null)
        //        {
        //            string txt = value.ToString();
        //            if (txt.Trim() != string.Empty)
        //                spanBuilder.InnerHtml = txt;
        //            else
        //                spanBuilder.InnerHtml = "&nbsp;";
        //        }
        //        else
        //            spanBuilder.InnerHtml = "&nbsp;";

        //        txtNumeric = spanBuilder.ToString(TagRenderMode.Normal);
        //        txtUnit = string.Format("<span>{0}</span>", currencies[currency]);
        //    }
        //    else
        //    {
        //        var inputBuilder = new TagBuilder("input");
        //        inputBuilder.MergeAttribute("type", "text");
        //        inputBuilder.MergeAttribute("id", id);
        //        inputBuilder.MergeAttribute("name", id);
        //        inputBuilder.MergeAttribute("class", "numeric-box");

        //        if (value != null)
        //            inputBuilder.MergeAttribute("value", value.ToString());
        //        if (attribute != null)
        //            CommonUtil.SetHtmlTagAttribute(inputBuilder, attribute);

        //        txtNumeric = inputBuilder.ToString(TagRenderMode.SelfClosing);

        //        if (currencyType == MULTIPLE_CURRENCY_TYPE.COMBOBOX)
        //        {
        //            var selectBuilder = new TagBuilder("select");
        //            selectBuilder.MergeAttribute("id", id + "MulC");
        //            selectBuilder.MergeAttribute("name", id + "MulC");

        //            if (attribute != null)
        //            {
        //                PropertyInfo prop = attribute.GetType().GetProperty("readonly");
        //                if (prop != null)
        //                {
        //                    Dictionary<string, string> dic = new Dictionary<string, string>();
        //                    dic.Add("disabled", prop.GetValue(attribute, null).ToString());
        //                    CommonUtil.SetHtmlTagAttribute(selectBuilder, dic);
        //                }
        //            }

        //            foreach (string key in currencies.Keys)
        //            {
        //                var optionBuilder = new TagBuilder("option");
        //                optionBuilder.MergeAttribute("value", key);
        //                optionBuilder.InnerHtml = currencies[key];

        //                string tt = optionBuilder.ToString(TagRenderMode.Normal);
        //                if (currency == key)
        //                {
        //                    string chk = "<option ";
        //                    int idx = tt.IndexOf(chk);
        //                    if (idx >= 0)
        //                        tt = tt.Substring(0, chk.Length) + "selected=\"true\" " + tt.Substring(chk.Length);
        //                }

        //                selectBuilder.InnerHtml += tt;
        //            }

        //            txtUnit = selectBuilder.ToString(TagRenderMode.Normal);
        //        }
        //        else
        //            txtUnit = string.Format("<span>{0}</span>", currencies[currency]);
        //    }

            
        //    string div = string.Format("<div class=\"combo-unit\">{0}</div>", txtUnit);
        //    string divr = string.Empty;
        //    if (required == true)
        //        divr = "&nbsp;<span class=\"label-remark\">*</span>";

        //    txtNumeric = string.Format("{0}<div class=\"object-unit\">{1}</div>{2}",
        //        div, txtNumeric, divr);

        //    return MvcHtmlString.Create(txtNumeric);
        //}
    }
}
