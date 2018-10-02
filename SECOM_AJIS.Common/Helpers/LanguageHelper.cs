using System;
using System.Web.WebPages;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Collections.Generic;
using System.Threading;

using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Common.Helpers
{
    public static class LanguageHelper
    {
        /// <summary>
        /// Generate language url
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="cultureName"></param>
        /// <param name="languageRouteName"></param>
        /// <param name="strictSelected"></param>
        /// <returns></returns>
        public static LanguageModel LanguageUrl(this HtmlHelper helper, string cultureName,  
                                                string languageRouteName = "lang", bool strictSelected = false)
        {
            // set the input language to lower
            cultureName = cultureName.ToLower();
            
            // retrieve the route values from the view context
            var routeValues = new RouteValueDictionary(helper.ViewContext.RouteData.Values);
            
            // copy the query strings into the route values to generate the link
            var queryString = helper.ViewContext.HttpContext.Request.QueryString;
            
            foreach (string key in queryString)
            {
                if (queryString[key] != null && !string.IsNullOrWhiteSpace(key))
                {
                    if (routeValues.ContainsKey(key))
                    {
                        routeValues[key] = queryString[key];
                    }
                    else
                    {
                        routeValues.Add(key, queryString[key]);
                    }
                }
            }
            
            var actionName = routeValues["action"].ToString();
            var controllerName = routeValues["controller"].ToString();
            
            // set the language into route values
            routeValues[languageRouteName] = cultureName;
            
            // generate the language specify url
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
            var url = urlHelper.RouteUrl("Localization", routeValues);
            
            // check whether the current thread ui culture is this language
            var current_lang_name = Thread.CurrentThread.CurrentUICulture.Name.ToLower();
            var isSelected = strictSelected ? current_lang_name == cultureName : current_lang_name.StartsWith(cultureName);
            
            return new LanguageModel()
                        {  
                            Url = url,
                            ActionName = actionName,
                            ControllerName = controllerName,
                            RouteValues = routeValues,
                            IsSelected = isSelected
                        };
        }
        /// <summary>
        /// Generate language selector link
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="cultureName"></param>
        /// <param name="selectedText"></param>
        /// <param name="unselectedText"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="languageRouteName"></param>
        /// <param name="strictSelected"></param>
        /// <returns></returns>
        public static MvcHtmlString LanguageSelectorLink(this HtmlHelper helper, string cultureName, 
                                                            string selectedText, string unselectedText,
                                                            IDictionary<string, object> htmlAttributes, 
                                                            string languageRouteName = "lang", bool strictSelected = false)
        {
            var language = helper.LanguageUrl(cultureName, languageRouteName, strictSelected);
            var link = helper.RouteLink(language.IsSelected ? selectedText : unselectedText,
                                        "Localization", language.RouteValues, htmlAttributes);
            
            return link;
        }
        /// <summary>
        /// Generate language selector image link
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="cultureName"></param>
        /// <param name="imagePath"></param>
        /// <param name="alt"></param>
        /// <param name="languageRouteName"></param>
        /// <param name="strictSelected"></param>
        /// <returns></returns>
        public static MvcHtmlString LanguageSelectorImageLink(this HtmlHelper helper,string id, string cultureName,
                                                            string imagePath, string alt,
                                                            string languageRouteName = "lang", bool strictSelected = false)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);
            var language = helper.LanguageUrl(cultureName, languageRouteName, strictSelected);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", url.Content(imagePath));
            imgBuilder.MergeAttribute("alt", alt);
            imgBuilder.MergeAttribute("id", id);
            imgBuilder.MergeAttribute("name", id);
            string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);


            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(language.ActionName, language.RouteValues));
            // include the <img> tag inside
            anchorBuilder.InnerHtml = imgHtml;

            string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }
    }
}
