using System;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Common.ActionFilters
{
    public class LocalizationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Event on action executing for set currect language
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (filterContext.RouteData.Values["lang"] != null &&
                    !string.IsNullOrWhiteSpace(filterContext.RouteData.Values["lang"].ToString()))
                {
                    // set the culture from the route data (url)
                    //var lang = filterContext.RouteData.Values["lang"].ToString();

                    try
                    {
                        var lang = filterContext.RouteData.Values["lang"].ToString();
                        Thread.CurrentThread.CurrentUICulture = ConvertToCultureInfo(ref lang);
                    }
                    catch
                    {
                        //for redirect to 404 by Rachatanawee
                        //{
                        RedirectObject o = new RedirectObject();
                        o.URL = "/Error/Http404";

                        JsonResult jRes = new JsonResult();
                        jRes.Data = o;
                        filterContext.Result = jRes;
                        base.OnActionExecuting(filterContext);
                        //}
                        var lang = CommonValue.DEFAULT_LANGUAGE_EN;
                        Thread.CurrentThread.CurrentUICulture = ConvertToCultureInfo(ref lang);
                    }
                }
                else
                {
                    // load the culture info from the cookie
                    var cookie = filterContext.HttpContext.Request.Cookies["ShaunXu.MvcLocalization.CurrentUICulture"];
                    var langHeader = string.Empty;
                    if (cookie != null)
                    {
                        // set the culture by the cookie content
                        langHeader = cookie.Value;
                        Thread.CurrentThread.CurrentUICulture = ConvertToCultureInfo(ref langHeader);
                    }
                    else
                    {
                        // set the culture by the location if not speicified
                        langHeader = filterContext.HttpContext.Request.UserLanguages[0];
                        Thread.CurrentThread.CurrentUICulture = ConvertToCultureInfo(ref langHeader);
                    }

                    // set the lang value into route data
                    filterContext.RouteData.Values["lang"] = langHeader;
                }

                // save the location into cookie
                HttpCookie _cookie = new HttpCookie("ShaunXu.MvcLocalization.CurrentUICulture",
                                                    Thread.CurrentThread.CurrentUICulture.Name);
                _cookie.Expires = DateTime.Now.AddDays(1);
                filterContext.HttpContext.Response.SetCookie(_cookie);

                if (CommonUtil.dsTransData != null)
                {
                    if (CommonUtil.dsTransData.dtTransHeader.Language != Thread.CurrentThread.CurrentUICulture.Name)
                        HttpContext.Current.Session.Remove("Menu");
                    CommonUtil.dsTransData.dtTransHeader.Language = Thread.CurrentThread.CurrentUICulture.Name;
                }
            }
            catch (Exception)
            {
            }

            base.OnActionExecuting(filterContext);
        }
        /// <summary>
        /// Method for convert cultureinfo format
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private CultureInfo ConvertToCultureInfo(ref string lang)
        {
            if (lang != null)
            {
                string fLang = null;

                if (lang == CommonValue.DEFAULT_LANGUAGE_EN
                    || lang.ToLower() == CommonValue.DEFAULT_SHORT_LANGUAGE_EN)
                {
                    fLang = CommonValue.DEFAULT_LANGUAGE_EN;
                    lang = CommonValue.DEFAULT_SHORT_LANGUAGE_EN;
                }
                else if (lang == CommonValue.DEFAULT_LANGUAGE_JP
                    || lang.ToLower() == CommonValue.DEFAULT_SHORT_LANGUAGE_JP)
                {
                    fLang = CommonValue.DEFAULT_LANGUAGE_JP;
                    lang = CommonValue.DEFAULT_SHORT_LANGUAGE_JP;
                }
                else if (lang == CommonValue.DEFAULT_LANGUAGE_LC
                   || lang.ToLower() == CommonValue.DEFAULT_SHORT_LANGUAGE_LC)
                {
                    fLang = CommonValue.DEFAULT_LANGUAGE_LC;
                    lang = CommonValue.DEFAULT_SHORT_LANGUAGE_LC;
                }

                if (fLang != null)
                    return CultureInfo.CreateSpecificCulture(fLang);
            }

            return null;
        }
    }
}
