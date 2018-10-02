using System;
using System.Web.WebPages;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using System.Reflection;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Xml;

namespace SECOM_AJIS.Common.Helpers
{
    public static class ButtonHelper
    {
        /// <summary>
        /// Generate link
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="display"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CommonLink(this HtmlHelper helper, string id, string display, object attribute = null)
        {
            var aBuilder = new TagBuilder("a");
            aBuilder.MergeAttribute("id", id);
            aBuilder.MergeAttribute("name", id);
            aBuilder.MergeAttribute("href", "#");

            if (attribute != null)
                CommonUtil.SetHtmlTagAttribute(aBuilder, attribute);

            aBuilder.InnerHtml = display;

            return MvcHtmlString.Create(aBuilder.ToString(TagRenderMode.Normal));

        }
        /// <summary>
        /// Generate button
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="display"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CommonButton(this HtmlHelper helper, string id, string display, object attribute=null)
        {
            var buttonBuilder = new TagBuilder("button");
            buttonBuilder.MergeAttribute("id", id);
            buttonBuilder.MergeAttribute("name", id);

            if (attribute != null)
                CommonUtil.SetHtmlTagAttribute(buttonBuilder, attribute);

            buttonBuilder.InnerHtml = display;

            return MvcHtmlString.Create(buttonBuilder.ToString(TagRenderMode.Normal));

        }
        /// <summary>
        /// Generate radio button
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="display"></param>
        /// <param name="value"></param>
        /// <param name="check"></param>
        /// <param name="group"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CommonRadioButton( this HtmlHelper helper, 
                                                        string id, 
                                                        string display, 
                                                        string value = null, 
                                                        bool? check = false,
                                                        string group = null, 
                                                        object attribute = null)
        {
            var inputBuilder = new TagBuilder("input");
            inputBuilder.MergeAttribute("type", "radio");
            inputBuilder.MergeAttribute("id", id);
            inputBuilder.MergeAttribute("name", (group != null) ? group : id);

            if (value != null)
                inputBuilder.MergeAttribute("value", value);

            object attr = attribute;
            if (check == true)
            {
                if (attribute == null)
                    attr = new { @checked = "checked" };
                else
                {
                    Dictionary<string, string> iDic = attribute.GetType().GetProperties().ToDictionary(p => p.Name, p => (string)p.GetValue(attribute, null));
                    if (iDic.ContainsKey("checked") == false)
                        iDic.Add("checked", "checked");

                    CommonUtil.SetHtmlTagAttribute(inputBuilder, iDic);
                    attr = null;
                }
            }
            if (attr != null)
            {
                //attribute
                CommonUtil.SetHtmlTagAttribute(inputBuilder, attr);
            }

            string txtDisplay = string.Format("<span id='span" + id + "'>{0}</span>", display);
            return MvcHtmlString.Create(inputBuilder.ToString(TagRenderMode.SelfClosing) + txtDisplay);

        }
        /// <summary>
        /// Generate checkbox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="display"></param>
        /// <param name="value"></param>
        /// <param name="check"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString CommonCheckButton( this HtmlHelper helper, 
                                                        string id, 
                                                        string display, 
                                                        string value = null, 
                                                        bool? check = false,
                                                        object attribute = null)
        {
            var inputBuilder = new TagBuilder("input");
            inputBuilder.MergeAttribute("type", "checkbox");
            inputBuilder.MergeAttribute("id", id);
            inputBuilder.MergeAttribute("name", id);

            if (value != null)
                inputBuilder.MergeAttribute("value", value);

            object attr = attribute;
            if (check == true)
            {
                if (attribute == null)
                    attr = new { @checked = "checked" };
                else
                {
                    Dictionary<string, string> iDic = attribute.GetType().GetProperties().ToDictionary(p => p.Name, p => (string)p.GetValue(attribute, null));
                    if (iDic.ContainsKey("checked") == false)
                        iDic.Add("checked", "checked");

                    CommonUtil.SetHtmlTagAttribute(inputBuilder, iDic);
                    attr = null;
                }
            }
            if (attr != null)
            {
                //attribute
                CommonUtil.SetHtmlTagAttribute(inputBuilder, attr);
            }

            string txtDisplay = string.Format("<span>{0}</span>", display);
            return MvcHtmlString.Create(inputBuilder.ToString(TagRenderMode.SelfClosing) + txtDisplay);

        }
        /// <summary>
        /// Generate checkbox list
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="isHorizontal"></param>
        /// <param name="check_val"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString TestCheckButtonList(this HtmlHelper helper,
                                                        string id,
                                                        bool isHorizontal = false,
                                                        string[] check_val = null,
                                                        object attribute = null)
        {
            List<TestModel> lst = new List<TestModel>();
            lst.Add(new TestModel() { Code = "U001", DisplayName = "P001"});
            lst.Add(new TestModel() { Code = "U002", DisplayName = "P002" });
            lst.Add(new TestModel() { Code = "U003", DisplayName = "P003" });
            lst.Add(new TestModel() { Code = "U004", DisplayName = "P004" });
            lst.Add(new TestModel() { Code = "U005", DisplayName = "P005" });
            lst.Add(new TestModel() { Code = "U006", DisplayName = "P006" });
            lst.Add(new TestModel() { Code = "U007", DisplayName = "P007" });

            return CommonUtil.CommonCheckButtonList<TestModel>(id, null, lst, "DisplayName", "Code", isHorizontal, check_val, attribute);
        }
        /// <summary>
        /// Generate service type checkbox list
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="header"></param>
        /// <param name="isHorizontal"></param>
        /// <param name="check_val"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ServiceTypeCheckButtonList(this HtmlHelper helper,
                                                                    string id,
                                                                    string header,
                                                                    bool isHorizontal = false,
                                                                    string[] check_val = null,
                                                                    object attribute = null,
                                                                    bool isRequired = false)
        {
            string lang = CommonUtil.GetCurrentLanguage();
            if (lang == SECOM_AJIS.Common.Util.ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                lang = string.Empty;
            else
                lang = "." + lang;

            string resourcePath = string.Format("{0}{1}\\{2}\\{3}{4}.resx",
                                                            CommonUtil.WebPath,
                                                            SECOM_AJIS.Common.Util.ConstantValue.CommonValue.APP_GLOBAL_RESOURCE_FOLDER,
                                                            "Common",
                                                            "CommonResources",
                                                            lang);
            XmlDocument rDoc = new XmlDocument();
            rDoc.Load(resourcePath);

            List<ComboObjectModel> lst = new List<ComboObjectModel>();
            lst.Add(new ComboObjectModel() { Code = CommonValue.QUOTATION_SERVICE_TYPE_FIRE_MONITORING, DisplayName = "lblServiceTypeFireMonitoring" });
            lst.Add(new ComboObjectModel() { Code = CommonValue.QUOTATION_SERVICE_TYPE_CRIME_PREVENTION, DisplayName = "lblServiceTypeCrimePrevention" });
            lst.Add(new ComboObjectModel() { Code = CommonValue.QUOTATION_SERVICE_TYPE_EMERGENCY_REPORT, DisplayName = "lblServiceTypeEmergencyReport" });
            lst.Add(new ComboObjectModel() { Code = CommonValue.QUOTATION_SERVICE_TYPE_FACILITY_MONITORING, DisplayName = "lblServiceTypeFacilityMonitoring" });

            foreach (ComboObjectModel p in lst)
            {
                XmlNode rNode = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", p.DisplayName));
                if (rNode != null)
                   p.DisplayName = rNode.InnerText;
            }

            return CommonUtil.CommonCheckButtonList<ComboObjectModel>(id, header, lst, "DisplayName", "Code", isHorizontal, check_val, attribute, isRequired);
        }
        


        public static MvcHtmlString DemoLinkList(this HtmlHelper helper,
                                                 string id,
                                                 string header,
                                                 bool isHorizontal = false,
                                                 object attribute = null)
        {
            List<TestModel> lst = new List<TestModel>();
            lst.Add(new TestModel() { Code = "1", DisplayName = "0001" });
            lst.Add(new TestModel() { Code = "2", DisplayName = "0002" });
            lst.Add(new TestModel() { Code = "3", DisplayName = "0003" });
            lst.Add(new TestModel() { Code = "4", DisplayName = "0004" });

            return CommonUtil.CommonLinkList<TestModel>(id, header, lst, "DisplayName", "Code", isHorizontal, attribute);
        }
    }
}
