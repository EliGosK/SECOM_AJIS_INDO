using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Common.Helpers
{
    public static class LabelHelper
    {
        /// <summary>
        /// Generate currency unit label
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MvcHtmlString LabelCurrencyUnit(this HtmlHelper helper, string id)
        {
            string unit = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CommonResources", "lblTHB");
            string div = string.Format("<div id=\"unit{0}\" class=\"label-unit\">{1}</div>", id, unit);
            return MvcHtmlString.Create(div);
        }
        /// <summary>
        /// Generate time unit label
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MvcHtmlString LabelTimesUnit(this HtmlHelper helper, string id)
        {
            string unit = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CommonResources", "lblTimes");
            string div = string.Format("<div id=\"unit{0}\" class=\"label-unit\">{1}</div>", id, unit);
            return MvcHtmlString.Create(div);
        }
        /// <summary>
        /// Generate date format label
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MvcHtmlString LabelDateFormat(this HtmlHelper helper, string id)
        {
            string format = "(ddMMyyyy)";

            string div = string.Format("<div id=\"unitDate{0}\" class=\"label-unit\">{1}</div>", id, format);
            return MvcHtmlString.Create(div);
        }
        /// <summary>
        /// Generate minutes unit label
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MvcHtmlString LabelMinutesUnit(this HtmlHelper helper, string id)
        {
            string unit = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CommonResources", "lblMinutes");
            string div = string.Format("<div id=\"unit{0}\" class=\"label-unit\">{1}</div>", id, unit);
            return MvcHtmlString.Create(div);
        }
        /// <summary>
        /// Generate decode label
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="labelText"></param>
        /// <returns></returns>
        public static MvcHtmlString LabelDecode(this HtmlHelper helper, string labelText)
        {
            string outText = System.Web.HttpContext.Current.Server.HtmlDecode(labelText);
            return MvcHtmlString.Create(outText);
        }
    }
}
