using System;
using System.Web.Mvc;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using SECOM_AJIS.Common.Models;
using Microsoft.Security.Application;

namespace SECOM_AJIS.Common.Util
{
    public partial class CommonUtil
    {
        public enum eFirstElementType
        {
            Select,
            All,
            Short,
            None
        }
        private const string COMBO_FIRSTELEMTXT_SELECT = "SELECT";
        private const string COMBO_FIRSTELEMTXT_ALL = "ALL";
        private const string COMBO_FIRSTELEMTXT_NONE = "NONE";
        private const string COMBO_FIRSTELEMTXT_CUSTOM_SELECT = "CUSTOM_SELECT";
        private const string COMBO_FIRSTELEMTXT_CUSTOM_NONE = "----";

        /// <summary>
        /// Set attribute in html tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attribute"></param>
        public static void SetHtmlTagAttribute(TagBuilder tag, object attribute)
        {
            PropertyInfo[] prop = attribute.GetType().GetProperties();
            if (prop != null)
            {
                if (prop.Length > 0)
                {
                    foreach (PropertyInfo p in prop)
                    {
                        try
                        {
                            string attr = p.Name;
                            string val = p.GetValue(attribute, null).ToString().Trim();

                            tag.MergeAttribute(attr, val, true);
                        }
                        catch
                        {
                        }
                    }
                }
            }

        }
        /// <summary>
        /// Set attribute in html tag
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attribute"></param>
        public static void SetHtmlTagAttribute(TagBuilder tag, Dictionary<string, string> attribute)
        {
            if (attribute != null)
            {
                foreach (KeyValuePair<string, string> attr in attribute)
                {
                    tag.MergeAttribute(attr.Key, attr.Value, true);
                }
            }
        }
        /// <summary>
        /// Generate custom combobox (include first element)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="lst"></param>
        /// <param name="display"></param>
        /// <param name="value"></param>
        /// <param name="firstElement"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        public static MvcHtmlString CommonComboBoxWithCustomFirstElement<T>(string id, List<T> lst, string display, string value, string firstElement, object attribute = null, bool include_idx0 = true) where T : class
        {
            string currentLang = CommonUtil.GetCurrentLanguage();
            string sVal = null;

            //MessageModel all = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0120); // ---All---
            //MessageModel select = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0113); // ---Select--

            string strFirstElemText;

            if (string.IsNullOrWhiteSpace(firstElement) || firstElement.ToUpper() == COMBO_FIRSTELEMTXT_SELECT)
            {
                //strFirstElemText = CommonUtil.GetLabelFromResource("Common", "CMS030", "lblComboboxSelect");
                strFirstElemText = CommonUtil.GetLabelFromResource("Common", "CommonResources", "lblComboboxSelect");

            }
            else if (firstElement.ToUpper() == COMBO_FIRSTELEMTXT_ALL)
            {
                //strFirstElemText = CommonUtil.GetLabelFromResource("Common", "CMS030", "lblComboboxAll");
                strFirstElemText = CommonUtil.GetLabelFromResource("Common", "CommonResources", "lblComboboxAll");
            }
            else if (firstElement.ToUpper() == COMBO_FIRSTELEMTXT_CUSTOM_SELECT)
            {
                //strFirstElemText = CommonUtil.GetLabelFromResource("Common", "CMS030", "lblComboboxAll");
                strFirstElemText = CommonUtil.GetLabelFromResource("Common", "CommonResources", "lblComboboxCUSTOM_SELECT");
            }
            else if (firstElement.ToUpper() == COMBO_FIRSTELEMTXT_NONE)
            {
                //strFirstElemText = CommonUtil.GetLabelFromResource("Common", "CMS030", "lblComboboxAll");
                strFirstElemText = COMBO_FIRSTELEMTXT_CUSTOM_NONE;
            }
            else
            {
                strFirstElemText = firstElement;
            }

            var selectBuilder = new TagBuilder("select");
            selectBuilder.MergeAttribute("id", id);
            selectBuilder.MergeAttribute("name", id);

            if (attribute != null)
            {
                PropertyInfo[] prop = attribute.GetType().GetProperties();
                if (prop != null)
                {
                    if (prop.Length > 0)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        foreach (PropertyInfo p in prop)
                        {
                            object o = p.GetValue(attribute, null);
                            if (o != null)
                            {
                                if (p.Name == "selected")
                                    //sVal = (string)o;
                                    sVal = o.ToString();
                                else
                                    //dic.Add(p.Name, (String)o);
                                    dic.Add(p.Name,  o.ToString());
                            }
                        }

                        SetHtmlTagAttribute(selectBuilder, dic);
                    }
                }

            }

            if (include_idx0)
            {
                var fOptionBuilder = new TagBuilder("option");
                fOptionBuilder.MergeAttribute("value", "");
                fOptionBuilder.InnerHtml = strFirstElemText;
                selectBuilder.InnerHtml += fOptionBuilder.ToString(TagRenderMode.Normal);
            }

            if (lst != null)
            {
                foreach (T et in lst)
                {
                    PropertyInfo propD = et.GetType().GetProperty(display);
                    PropertyInfo propV = et.GetType().GetProperty(value);
                    PropertyInfo propS = et.GetType().GetProperty("Selected");

                    if (propD != null && propV != null)
                    {
                        if (propV.GetValue(et, null) != null && propD.GetValue(et, null) != null)
                        {


                            var optionBuilder = new TagBuilder("option");
                            optionBuilder.MergeAttribute("value", Encoder.HtmlEncode(propV.GetValue(et, null).ToString()));
                            optionBuilder.InnerHtml = Encoder.HtmlEncode(propD.GetValue(et, null).ToString());

                            string tt = optionBuilder.ToString(TagRenderMode.Normal);
                           
                            if (sVal != null)
                            {
                                if (sVal == propV.GetValue(et, null).ToString())
                                {
                                    string chk = "<option ";
                                    int idx = tt.IndexOf(chk);
                                    if (idx >= 0)
                                        tt = tt.Substring(0, chk.Length) + "selected=\"true\" " + tt.Substring(chk.Length);
                                }
                            }
                            if (propS != null)
                            {
                                if ((bool)propS.GetValue(et, null) == true)
                                {
                                    string chk = "<option ";
                                    int idx = tt.IndexOf(chk);
                                    if (idx >= 0)
                                        tt = tt.Substring(0, chk.Length) + "selected=\"true\" " + tt.Substring(chk.Length);
                                }
                            }

                            selectBuilder.InnerHtml += tt;
                        }
                    }
                }

            }

            return MvcHtmlString.Create(selectBuilder.ToString(TagRenderMode.Normal));
        }
        //public static MvcHtmlString CommonComboBox<T>(string id, List<T> lst, string display, string value, object attribute = null, bool include_idx0 = true) where T : class
        //{
        //    return CommonComboBoxWithCustomFirstElement(id, lst, display, value, null, attribute, include_idx0);
        //}
        /// <summary>
        /// Generate custom combobox
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="lst"></param>
        /// <param name="display"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <param name="idx0_type"></param>
        /// <returns></returns>
        public static MvcHtmlString CommonComboBox<T>(string id, List<T> lst, string display, string value, object attribute = null, bool include_idx0 = true, eFirstElementType idx0_type = eFirstElementType.Select) where T : class
        {
            string strFirstElemTxt;
            if (idx0_type == eFirstElementType.All)
            {
                strFirstElemTxt = COMBO_FIRSTELEMTXT_ALL;
            }
            else if (idx0_type == eFirstElementType.Select)
            {
                strFirstElemTxt = COMBO_FIRSTELEMTXT_SELECT;
            }
            else if (idx0_type == eFirstElementType.None)
            {
                strFirstElemTxt = COMBO_FIRSTELEMTXT_NONE;
            }
            else
            {
                strFirstElemTxt = null;
            }
            return CommonComboBoxWithCustomFirstElement(id, lst, display, value, strFirstElemTxt, attribute, include_idx0);
        }
        /// <summary>
        /// Generate custom cobobox
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="lst"></param>
        /// <param name="display"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <param name="include_idx0"></param>
        /// <returns></returns>
        [Obsolete("Dont use this method.", false)]
        public static MvcHtmlString CommonComboBox<T>(string id, List<T> lst, string display, string[] value, object attribute = null, bool include_idx0 = true) where T : class
        {
            string currentLang = CommonUtil.GetCurrentLanguage();

            var selectBuilder = new TagBuilder("select");
            selectBuilder.MergeAttribute("id", id);
            selectBuilder.MergeAttribute("name", id);

            if (attribute != null)
                SetHtmlTagAttribute(selectBuilder, attribute);

            if (lst != null)
            {
                if (lst.Count > 0 && include_idx0)
                {
                    var fOptionBuilder = new TagBuilder("option");
                    fOptionBuilder.MergeAttribute("value", "");

                    //MessageModel select = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0113);
                    string strSelect = CommonUtil.GetLabelFromResource("Common", "CMS030", "lblComboboxSelect");

                    fOptionBuilder.InnerHtml = strSelect;

                    selectBuilder.InnerHtml += fOptionBuilder.ToString(TagRenderMode.Normal);
                }
                foreach (T et in lst)
                {
                    PropertyInfo propD = et.GetType().GetProperty(display);
                    if (propD != null)
                    {
                        if (propD.GetValue(et, null) != null)
                        {
                            bool valIsSet = false;
                            if (value != null)
                            {
                                if (value.Length > 0)
                                    valIsSet = true;
                            }

                            string valSet = string.Empty;
                            if (valIsSet)
                            {
                                foreach (string val in value)
                                {
                                    PropertyInfo propV = et.GetType().GetProperty(val);
                                    if (propV != null)
                                    {
                                        if (propV.GetValue(et, null) != null)
                                        {
                                            if (valSet != string.Empty)
                                                valSet += ",";
                                            valSet += propV.GetValue(et, null).ToString();
                                        }
                                    }
                                }

                            }

                            var optionBuilder = new TagBuilder("option");
                            optionBuilder.MergeAttribute("value", valSet);
                            optionBuilder.InnerHtml = propD.GetValue(et, null).ToString();

                            selectBuilder.InnerHtml += optionBuilder.ToString(TagRenderMode.Normal);

                        }
                    }
                }
            }

            return MvcHtmlString.Create(selectBuilder.ToString(TagRenderMode.Normal));
        }
        /// <summary>
        /// Generate check list box
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="header"></param>
        /// <param name="lst"></param>
        /// <param name="display"></param>
        /// <param name="value"></param>
        /// <param name="isHorizontal"></param>
        /// <param name="check_val"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CommonCheckButtonList<T>(string id,
                                                              string header,
                                                              List<T> lst,
                                                              string display,
                                                              string value,
                                                              bool isHorizontal = false,
                                                              string[] check_val = null,
                                                              object attribute = null,
                                                              bool isRequired = false)
        {
            var divBuilder = new TagBuilder("div");
            divBuilder.MergeAttribute("id", id);
            divBuilder.MergeAttribute("class", "fieldset-table");

            var idivBuilder = new TagBuilder("div");

            if (isHorizontal == false)
                idivBuilder.MergeAttribute("class", "ctrl-list");
            else
                idivBuilder.MergeAttribute("class", "ctrl-list-horizontal");

            bool isDisabled = false;
            if (attribute != null)
            {
                Dictionary<string, string> iDic = attribute.GetType().GetProperties().ToDictionary(p => p.Name, p => (string)p.GetValue(attribute, null));
                if (iDic.ContainsKey("width"))
                    divBuilder.MergeAttribute("style", "width:" + iDic["width"]);

                if (iDic.ContainsKey("height"))
                    idivBuilder.MergeAttribute("style", "height:" + iDic["height"] + ";");
                else
                    idivBuilder.MergeAttribute("style", "height:100%;");

                if (iDic.ContainsKey("disabled"))
                    isDisabled = true;
            }

            if (lst != null)
            {
                if (lst.Count > 0)
                {
                    int idx = 1;
                    var ulBuilder = new TagBuilder("ul");
                    foreach (T et in lst)
                    {
                        PropertyInfo propD = et.GetType().GetProperty(display);
                        PropertyInfo propV = et.GetType().GetProperty(value);
                        if (propD != null && propV != null)
                        {
                            if (propV.GetValue(et, null) != null && propD.GetValue(et, null) != null)
                            {
                                string chk_id = string.Format("{0}_{1}", id, idx);
                                string txtDisplay = string.Format("<span>{0}</span>", propD.GetValue(et, null).ToString());
                                string val = propV.GetValue(et, null).ToString();

                                var inputBuilder = new TagBuilder("input");
                                inputBuilder.MergeAttribute("type", "checkbox");
                                inputBuilder.MergeAttribute("id", chk_id);
                                inputBuilder.MergeAttribute("name", chk_id);
                                inputBuilder.MergeAttribute("value", val);

                                if (check_val != null)
                                {
                                    if (check_val.Contains<string>(val))
                                        inputBuilder.MergeAttribute("checked", "checked");
                                }
                                if (isDisabled)
                                    inputBuilder.MergeAttribute("disabled", "disabled");

                                var liBuilder = new TagBuilder("li");
                                liBuilder.InnerHtml = inputBuilder.ToString(TagRenderMode.SelfClosing) + txtDisplay;
                                ulBuilder.InnerHtml += liBuilder.ToString(TagRenderMode.Normal);

                                idx++;
                            }
                        }
                    }

                    idivBuilder.InnerHtml += ulBuilder.ToString(TagRenderMode.Normal);
                }
            }

            if (header != null)
            {
                var hdivBuilder = new TagBuilder("div");
                hdivBuilder.MergeAttribute("class", "fieldset-header");
                hdivBuilder.InnerHtml = string.Format("<span>{0}</span>", header);
                if (isRequired)
                {
                    hdivBuilder.InnerHtml += "<span class=\"label-remark\">*</span>";
                }
                divBuilder.InnerHtml += hdivBuilder.ToString(TagRenderMode.Normal);
            }

            divBuilder.InnerHtml += idivBuilder.ToString(TagRenderMode.Normal);

            var cdivBuilder = new TagBuilder("div");
            cdivBuilder.MergeAttribute("class", "usr-clear-style");
            cdivBuilder.InnerHtml = "&nbsp;";
            divBuilder.InnerHtml += cdivBuilder.ToString(TagRenderMode.Normal);


            return MvcHtmlString.Create(divBuilder.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Generate link list box
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="header"></param>
        /// <param name="lst"></param>
        /// <param name="display"></param>
        /// <param name="value"></param>
        /// <param name="isHorizontal"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CommonLinkList<T>(string id,
                                                      string header,
                                                      List<T> lst,
                                                      string display,
                                                      string value,
                                                      bool isHorizontal = false,
                                                      object attribute = null)
        {
            var divBuilder = new TagBuilder("div");
            divBuilder.MergeAttribute("id", id);
            divBuilder.MergeAttribute("class", "fieldset-table");

            var idivBuilder = new TagBuilder("div");

            if (isHorizontal == false)
                idivBuilder.MergeAttribute("class", "ctrl-list");
            else
                idivBuilder.MergeAttribute("class", "ctrl-list-horizontal");

            bool isDisabled = false;
            if (attribute != null)
            {
                Dictionary<string, string> iDic = attribute.GetType().GetProperties().ToDictionary(p => p.Name, p => (string)p.GetValue(attribute, null));
                if (iDic.ContainsKey("width"))
                    divBuilder.MergeAttribute("style", "width:" + iDic["width"]);

                if (iDic.ContainsKey("height"))
                    idivBuilder.MergeAttribute("style", "height:" + iDic["height"] + ";");
                else
                    idivBuilder.MergeAttribute("style", "height:100%;");

                if (iDic.ContainsKey("disabled"))
                    isDisabled = true;
            }

            if (lst != null)
            {
                if (lst.Count > 0)
                {
                    int idx = 1;
                    var ulBuilder = new TagBuilder("ul");
                    foreach (T et in lst)
                    {
                        PropertyInfo propD = et.GetType().GetProperty(display);
                        PropertyInfo propV = et.GetType().GetProperty(value);
                        if (propD != null && propV != null)
                        {
                            if (propV.GetValue(et, null) != null && propD.GetValue(et, null) != null)
                            {
                                string lnk_id = string.Format("{0}_{1}", id, idx);
                                string txtDisplay = propD.GetValue(et, null).ToString();
                                string val = propV.GetValue(et, null).ToString();

                                var aBuilder = new TagBuilder("a");
                                aBuilder.MergeAttribute("id", lnk_id);
                                aBuilder.MergeAttribute("name", lnk_id);

                                if (isDisabled == false)
                                    aBuilder.MergeAttribute("href", "#");

                                aBuilder.InnerHtml = txtDisplay + string.Format("<input type=\"hidden\" value=\"{0}\" />", val);

                                var liBuilder = new TagBuilder("li");
                                liBuilder.InnerHtml = aBuilder.ToString(TagRenderMode.Normal);
                                ulBuilder.InnerHtml += liBuilder.ToString(TagRenderMode.Normal);

                                idx++;
                            }
                        }
                    }

                    idivBuilder.InnerHtml += ulBuilder.ToString(TagRenderMode.Normal);
                }
            }

            if (header != null)
            {
                var hdivBuilder = new TagBuilder("div");
                hdivBuilder.MergeAttribute("class", "fieldset-header");
                hdivBuilder.InnerHtml = string.Format("<span>{0}</span>", header);
                divBuilder.InnerHtml += hdivBuilder.ToString(TagRenderMode.Normal);
            }

            divBuilder.InnerHtml += idivBuilder.ToString(TagRenderMode.Normal);

            var cdivBuilder = new TagBuilder("div");
            cdivBuilder.MergeAttribute("class", "usr-clear-style");
            cdivBuilder.InnerHtml = "&nbsp;";
            divBuilder.InnerHtml += cdivBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(divBuilder.ToString(TagRenderMode.Normal));
        }
    }
}
