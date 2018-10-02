using CSI.WindsorHelper;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECOM_AJIS.Common.Helpers.TextBoxHelper;

namespace SECOM_AJIS.Presentation.Common.Helpers
{
    public static partial class TextBoxHelper
    {
        public enum MULTIPLE_CURRENCY_TYPE
        {
            COMBOBOX,
            LABEL
        }

        /// <summary>
        /// Generate textbox in case of numeric text (in clude unit)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString NumericTextBoxWithMultipleCurrency(this HtmlHelper helper, string id, object value = null, string currency = "1", object attribute = null, MULTIPLE_CURRENCY_TYPE currencyType = MULTIPLE_CURRENCY_TYPE.COMBOBOX, bool required = false)
        {
            if (string.IsNullOrEmpty(currency))
                currency = CurrencyUtil.C_CURRENCY_LOCAL;
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

            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_CURRENCT,
                        ValueCode = "%"
                    }
                };

            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            lst = hand.GetMiscTypeCodeList(miscs).ToList();

            Dictionary<string, string> currencies = new Dictionary<string, string>();
            foreach (var l in lst)
            {
                currencies.Add(l.ValueCode, l.ValueDisplayEN);
            }
            //currencies.Add("1", "Rp.");
            //currencies.Add("2", "US$");

            string txtNumeric = string.Empty;
            string txtUnit = string.Empty;
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
                txtUnit = string.Format("<span>{0}</span>", currencies[currency]);
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

                if (currencyType == MULTIPLE_CURRENCY_TYPE.COMBOBOX)
                {
                    var selectBuilder = new TagBuilder("select");
                    selectBuilder.MergeAttribute("id", id + "CurrencyType");
                    selectBuilder.MergeAttribute("name", id + "CurrencyType");

                    if (attribute != null)
                    {
                        PropertyInfo prop = attribute.GetType().GetProperty("readonly");
                        if (prop != null)
                        {
                            string val = prop.GetValue(attribute, null).ToString();
                            bool rd = true;
                            if (bool.TryParse(val, out rd) == false)
                                rd = true;
                            if (rd == true)
                            {
                                Dictionary<string, string> dic = new Dictionary<string, string>();
                                dic.Add("disabled", val);
                                CommonUtil.SetHtmlTagAttribute(selectBuilder, dic);
                            }
                        }
                    }

                    foreach (string key in currencies.Keys)
                    {
                        var optionBuilder = new TagBuilder("option");
                        optionBuilder.MergeAttribute("value", key);
                        optionBuilder.InnerHtml = currencies[key];

                        string tt = optionBuilder.ToString(TagRenderMode.Normal);
                        if (currency == key)
                        {
                            string chk = "<option ";
                            int idx = tt.IndexOf(chk);
                            if (idx >= 0)
                                tt = tt.Substring(0, chk.Length) + "selected=\"true\" " + tt.Substring(chk.Length);
                        }

                        selectBuilder.InnerHtml += tt;
                    }

                    txtUnit = selectBuilder.ToString(TagRenderMode.Normal);
                }
                else
                {
                    TagBuilder spanBuilder = new TagBuilder("span");
                    spanBuilder.MergeAttribute("id", id + "CurrencyType");
                    spanBuilder.MergeAttribute("name", id + "CurrencyType");
                    spanBuilder.AddCssClass("label-currency");

                    if (attribute != null)
                    {
                        PropertyInfo prop = attribute.GetType().GetProperty("readonly");
                        if (prop != null)
                        {
                            string val = prop.GetValue(attribute, null).ToString();
                            bool rd = true;
                            if (bool.TryParse(val, out rd) == false)
                                rd = true;
                            if (rd == true)
                            {
                                //Dictionary<string, string> dic = new Dictionary<string, string>();
                                //dic.Add("disabled", val);
                                //CommonUtil.SetHtmlTagAttribute(spanBuilder, dic);
                                spanBuilder.MergeAttribute("data-readonly", "1");
                            }
                        }
                    }

                    foreach (string key in currencies.Keys)
                    {
                        var ispanBuilder = new TagBuilder("span");
                        ispanBuilder.SetInnerText(currencies[key]);
                        ispanBuilder.MergeAttribute("data-type", key);

                        if (currency != key)
                            ispanBuilder.MergeAttribute("style", "display:none;");

                        spanBuilder.InnerHtml += ispanBuilder.ToString(TagRenderMode.Normal);
                    }

                    txtUnit = spanBuilder.ToString(TagRenderMode.Normal);
                }
            }


            string div = string.Format("<div class=\"combo-unit\">{0}</div>", txtUnit);
            string divr = string.Empty;
            if (required == true)
                divr = "<span class=\"label-remark\">&nbsp;*</span>";

            txtNumeric = string.Format("{0}<div class=\"object-unit\">{1}{2}</div>",
                div, txtNumeric, divr);

            return MvcHtmlString.Create(txtNumeric);
        }

        public static MvcHtmlString MultipleCurrency(this HtmlHelper helper, string id, object value = null, string currency = "1", object attribute = null, MULTIPLE_CURRENCY_TYPE currencyType = MULTIPLE_CURRENCY_TYPE.COMBOBOX, bool required = false)
        {
            List<doMiscTypeCode> lst = new List<doMiscTypeCode>();
            List<doMiscTypeCode> miscs = new List<doMiscTypeCode>()
                {
                    new doMiscTypeCode()
                    {
                        FieldName = MiscType.C_CURRENCT,
                        ValueCode = "%"
                    }
                };

            ICommonHandler hand = ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
            lst = hand.GetMiscTypeCodeList(miscs).ToList();

            Dictionary<string, string> currencies = new Dictionary<string, string>();
            foreach (var l in lst)
            {
                currencies.Add(l.ValueCode, l.ValueDisplayEN);
            }
            //currencies.Add("1", "Rp.");
            //currencies.Add("2", "US$");

            string txtNumeric = string.Empty;
            string txtUnit = string.Empty;

            if (currencyType == MULTIPLE_CURRENCY_TYPE.COMBOBOX)
            {
                var selectBuilder = new TagBuilder("select");
                selectBuilder.MergeAttribute("id", id + "CurrencyType");
                selectBuilder.MergeAttribute("name", id + "CurrencyType");

                if (attribute != null)
                {
                    PropertyInfo prop = attribute.GetType().GetProperty("readonly");
                    if (prop != null)
                    {
                        string val = prop.GetValue(attribute, null).ToString();
                        bool rd = true;
                        if (bool.TryParse(val, out rd) == false)
                            rd = true;
                        if (rd == true)
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.Add("disabled", val);
                            CommonUtil.SetHtmlTagAttribute(selectBuilder, dic);
                        }
                    }
                }

                foreach (string key in currencies.Keys)
                {
                    var optionBuilder = new TagBuilder("option");
                    optionBuilder.MergeAttribute("value", key);
                    optionBuilder.InnerHtml = currencies[key];

                    string tt = optionBuilder.ToString(TagRenderMode.Normal);
                    if (currency == key)
                    {
                        string chk = "<option ";
                        int idx = tt.IndexOf(chk);
                        if (idx >= 0)
                            tt = tt.Substring(0, chk.Length) + "selected=\"true\" " + tt.Substring(chk.Length);
                    }

                    selectBuilder.InnerHtml += tt;
                }

                txtUnit = selectBuilder.ToString(TagRenderMode.Normal);
            }
            else
            {
                TagBuilder spanBuilder = new TagBuilder("span");
                spanBuilder.MergeAttribute("id", id + "CurrencyType");
                spanBuilder.MergeAttribute("name", id + "CurrencyType");
                spanBuilder.AddCssClass("label-currency");

                if (attribute != null)
                {
                    PropertyInfo prop = attribute.GetType().GetProperty("readonly");
                    if (prop != null)
                    {
                        string val = prop.GetValue(attribute, null).ToString();
                        bool rd = true;
                        if (bool.TryParse(val, out rd) == false)
                            rd = true;
                        if (rd == true)
                        {
                            //Dictionary<string, string> dic = new Dictionary<string, string>();
                            //dic.Add("disabled", val);
                            //CommonUtil.SetHtmlTagAttribute(spanBuilder, dic);
                            spanBuilder.MergeAttribute("data-readonly", "1");
                        }
                    }
                }

                foreach (string key in currencies.Keys)
                {
                    var ispanBuilder = new TagBuilder("span");
                    ispanBuilder.SetInnerText(currencies[key]);
                    ispanBuilder.MergeAttribute("data-type", key);

                    if (currency != key)
                        ispanBuilder.MergeAttribute("style", "display:none;");

                    spanBuilder.InnerHtml += ispanBuilder.ToString(TagRenderMode.Normal);
                }

                txtUnit = spanBuilder.ToString(TagRenderMode.Normal);
            }

            return MvcHtmlString.Create(txtUnit);
        }
    }
}
