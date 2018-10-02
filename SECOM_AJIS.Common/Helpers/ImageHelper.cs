using System;
using System.Web.WebPages;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Collections.Generic;
using System.Threading;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Common.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Generate image link
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="imagePath"></param>
        /// <param name="alt"></param>
        /// <param name="link_attribute"></param>
        /// <param name="img_attribute"></param>
        /// <returns></returns>
        public static MvcHtmlString ActionImageLink(this HtmlHelper helper, string controller, string action,
                                                    string imagePath, string alt, object link_attribute=null, object img_attribute=null)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);
            
            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", url.Content(imagePath));
            imgBuilder.MergeAttribute("alt", alt);

            if (img_attribute != null)
                CommonUtil.SetHtmlTagAttribute(imgBuilder, img_attribute);

            string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);


            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(action, controller));

            if (link_attribute != null)
                CommonUtil.SetHtmlTagAttribute(anchorBuilder, link_attribute);

            // include the <img> tag inside
            anchorBuilder.InnerHtml = imgHtml;

            string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }
    }
}
