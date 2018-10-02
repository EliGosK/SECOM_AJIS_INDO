using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using System.Web;
using System.Web.Routing;
using SECOM_AJIS.Common.Controllers;
using System.Reflection;
using NLog;

namespace SECOM_AJIS.Common.ActionFilters
{
    public class AutoRetryAttribute : ActionFilterAttribute
    {
        private static Dictionary<string, object> _SyncLockKey = new Dictionary<string,object>();
        public static int MAX_RETRY = 3;

        private object SyncLockKey(string key)
        {
            lock (_SyncLockKey)
            {
                object oKey = null;
                if (_SyncLockKey.TryGetValue(key, out oKey))
                {
                    return oKey;
                }
                else
                {
                    oKey = new object();
                    _SyncLockKey.Add(key, oKey);
                    return oKey;
                }
            }
        }

        private object SyncLockKey(RouteData route)
        {
            return SyncLockKey(string.Format(@"{0}\{1}", route.Values["controller"], route.Values["action"]));
        }


        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    if (filterContext.Result is JsonResult)
        //    {
        //        JsonResult json = filterContext.Result as JsonResult;
        //        if (json != null && json.Data is ObjectResultData)
        //        {
        //            ObjectResultData data = json.Data as ObjectResultData;
        //            if (data != null && data.AutoRetry)
        //            {
        //                int iCount = 0;
        //                if (filterContext.RouteData.DataTokens.ContainsKey("retry_count"))
        //                {
        //                    iCount = (int)filterContext.RouteData.DataTokens["retry_count"];
        //                }

        //                if (iCount < _MaxRetry)
        //                {
        //                    iCount++;

        //                    filterContext.HttpContext.Response.ClearContent();
        //                    filterContext.HttpContext.Server.ClearError();

        //                    string controllerName = filterContext.RouteData.Values["controller"].ToString();
        //                    IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();

        //                    RequestContext req = new RequestContext(filterContext.RequestContext.HttpContext, filterContext.RouteData);
        //                    if (req.RouteData.DataTokens.ContainsKey("retry_count"))
        //                    {
        //                        req.RouteData.DataTokens["retry_count"] = iCount;
        //                    }
        //                    else
        //                    {
        //                        req.RouteData.DataTokens.Add("retry_count", iCount);
        //                    }
        //                    req.HttpContext.Request.InputStream.Position = 0;

        //                    int iWait = (new Random()).Next(1, 10) * 100;
        //                    System.Threading.Thread.Sleep(iWait);

        //                    lock (this.SyncLockKey(req.RouteData))
        //                    {
        //                        IController c = factory.CreateController(req, controllerName);
        //                        c.Execute(req);
        //                        filterContext.Canceled = true;
        //                    }
        //                }
        //                else
        //                {
        //                    Logger logger = LogManager.GetCurrentClassLogger();
        //                    var logEventInfo = new LogEventInfo(LogLevel.Error, "databaselog", "logMessage");
        //                    logEventInfo.Properties["ErrorDescription"] = "Retry over limit.";
        //                    logEventInfo.Properties["CreateDate"] = DateTime.Now;
        //                    string userID = "SYSTEM";
        //                    try
        //                    {
        //                        userID = CommonUtil.dsTransData.dtUserData.EmpNo;
        //                    }
        //                    catch (Exception)
        //                    {

        //                    }
        //                    logEventInfo.Properties["UserID"] = userID;
        //                    logEventInfo.Exception = filterContext.HttpContext.Server.GetLastError();

        //                    logger.Log(logEventInfo);
        //                }
        //            }
        //        }
        //    }
        //    base.OnActionExecuted(filterContext);
        //}

        /// <summary>
        /// Event on action executing for check AutoRetry flag from ObjectResultData and re-execute request context.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Result is JsonResult)
            {
                JsonResult json = filterContext.Result as JsonResult;
                if (json != null && json.Data is ObjectResultData)
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            {
                    ObjectResultData data = json.Data as ObjectResultData;
                    if (data != null && data.AutoRetry)
                    {
                        int iCount = 0;
                        if (filterContext.RouteData.DataTokens.ContainsKey("retry_count"))
                        {
                            iCount = (int)filterContext.RouteData.DataTokens["retry_count"];
                        }

                        if (iCount < MAX_RETRY)
                        {
                            iCount++;

                            filterContext.HttpContext.Response.ClearContent();
                            filterContext.HttpContext.Server.ClearError();

                            string controllerName = filterContext.RouteData.Values["controller"].ToString();
                            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();

                            RequestContext req = new RequestContext(filterContext.RequestContext.HttpContext, filterContext.RouteData);
                            if (req.RouteData.DataTokens.ContainsKey("retry_count"))
                            {
                                req.RouteData.DataTokens["retry_count"] = iCount;
                            }
                            else
                            {
                                req.RouteData.DataTokens.Add("retry_count", iCount);
                            }
                            req.HttpContext.Request.InputStream.Position = 0;

                            int iWait = (new Random()).Next(1, 10) * 100;
                            System.Threading.Thread.Sleep(iWait);

                            lock (this.SyncLockKey(req.RouteData))
                            {
                                IController c = factory.CreateController(req, controllerName);
                                c.Execute(req);
                                filterContext.Cancel = true;
                            }
                        }
                        else
                        {
                            Logger logger = LogManager.GetCurrentClassLogger();
                            var logEventInfo = new LogEventInfo(LogLevel.Error, "databaselog", "logMessage");
                            logEventInfo.Properties["ErrorDescription"] = "Retry over limit.";
                            logEventInfo.Properties["CreateDate"] = DateTime.Now;
                            string userID = "SYSTEM";
                            try
                            {
                                userID = CommonUtil.dsTransData.dtUserData.EmpNo;
                            }
                            catch (Exception)
                            {

                            }
                            logEventInfo.Properties["UserID"] = userID;
                            logEventInfo.Exception = filterContext.HttpContext.Server.GetLastError();

                            logger.Log(logEventInfo);
                        }
                    }
                }
            }
            base.OnResultExecuting(filterContext);
        }
    }

}
