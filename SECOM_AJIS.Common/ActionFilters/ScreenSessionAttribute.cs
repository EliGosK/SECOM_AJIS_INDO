using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using System.Web;
using System.Web.Routing;
using SECOM_AJIS.Common.Controllers;
using System.Reflection;

namespace SECOM_AJIS.Common.ActionFilters
{
    public class ScreenSessionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Event on action executing for check session is expired?
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                bool checkScreenSession = true;
                string action = filterContext.RouteData.Values["action"].ToString();
                if (action == "CMS000"
                    || action == "CMS010"
                    || action == "Login"
                    || action == "CMS010_CHECK"
                    || action == "CMS020_Retrive"
                    || action == "GetMessage"
                    || action == "changeLanguageDsTransData"
                    || action == "Logout"
                    || action == "GetLanguageMessage"
                    || action == "ClearCurrentScreenSession"
                    || action == "CMS000_RedirectScreen"
                    || action.ToUpper().Contains("UPLOAD")
                    || action.ToUpper().Contains("ATTACH"))
                    checkScreenSession = false;

                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                if (dsTrans == null)
                {
                    //if (checkScreenSession)
                    //{
                        //Check Action from ajax caller or load view
                        string ajax = filterContext.HttpContext.Request["ajax"];
                        if ( (ajax == "1")
                            && action != "CMS010"
                            && action != "CMS010_CHECK"
                            && action != "Login")
                        {
                            RedirectObject o = new RedirectObject();
                            o.URL = CommonUtil.GenerateURL("Common", "CMS010") + "?timeout=1";

                            JsonResult jRes = new JsonResult();
                            jRes.Data = o;
                            filterContext.Result = jRes;
                        }
                        else if (checkScreenSession == true)
                        {
                            bool skip = false;
                            doDirectScreen dos = null;

                            /* *** This method will only in case url has send parameter (expect "k" parameter). *** */
                            #region Initial redirect method

                            if (action != "CMS020")
                            {
                                dos = new doDirectScreen();
                                dos.Controller = filterContext.RouteData.Values["controller"].ToString();
                                dos.ScreenID = action;

                                foreach (string key in filterContext.HttpContext.Request.QueryString.AllKeys)
                                {
                                    if (key == "k") 
                                    {
                                        skip = true;
                                        break;
                                    }
                                    string val = filterContext.HttpContext.Request[key];
                                    if (dos.Parameters == null)
                                    {
                                        dos.Parameters = new List<string>();
                                        dos.Values = new List<string>();
                                    }

                                    dos.Parameters.Add(key);
                                    dos.Values.Add(val);
                                }
                            }

                            #endregion

                            if (skip == false && dos != null)
                            {
                                CommonUtil.SetSession("DIRECT_SCREEN", dos);

                                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                                {
                                    controller = "Common",
                                    action = "CMS000"
                                }));
                            }
                            else
                            {
                                if (action == "CMS020")
                                {
                                    BaseController bController = filterContext.Controller as BaseController;
                                    if (bController != null)
                                    {
                                        filterContext.Result = bController.RedirectToLogin();
                                    }
                                }
                                else
                                {
                                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                                    {
                                        controller = "Common",
                                        action = "CMS010"
                                    }));
                                }
                            }
                        }

                        return;
                    //}
                }
                else
                {
                    BaseController bController = filterContext.Controller as BaseController;
                    if (bController != null)
                    {
                        ScreenParameter param = bController.GetScreenObject<object>() as ScreenParameter;
                        if (param != null)
                            dsTrans.dtTransHeader.ScreenID = param.ScreenID;
                        else if (checkScreenSession)
                        {
                            bool isInitial = false;
                            MethodInfo method = bController.GetType().GetMethod(action);
                            if (method != null)
                            {
                                if (method.GetCustomAttributes(typeof(InitializeAttribute), true).Length > 0)
                                    isInitial = true;
                            }

                            if (action.EndsWith("_Authority")
                                || isInitial == true)
                            {
                                dsTrans.dtTransHeader.ScreenID = action.Replace("_Authority", "");

                                /* *** This method will only in case url has send parameter (expect "k" parameter). *** */
                                #region Initial redirect method

                                if (action != "CMS020" && isInitial == true)
                                {
                                    doDirectScreen dos = new doDirectScreen();
                                    dos.Controller = filterContext.RouteData.Values["controller"].ToString();
                                    dos.ScreenID = action;

                                    bool skip = false;
                                    foreach (string key in filterContext.HttpContext.Request.QueryString.AllKeys)
                                    {
                                        if (key == "k")
                                        {
                                            skip = true;
                                            break;
                                        }
                                        string val = filterContext.HttpContext.Request[key];
                                        if (dos.Parameters == null)
                                        {
                                            dos.Parameters = new List<string>();
                                            dos.Values = new List<string>();
                                        }

                                        dos.Parameters.Add(key);
                                        dos.Values.Add(val);
                                    }

                                    if (skip == false)
                                    {
                                        CommonUtil.SetSession("DIRECT_SCREEN", dos);
                                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                                        {
                                            controller = "Common",
                                            action = "CMS000"
                                        }));
                                    }
                                }

                                #endregion
                            }
                            else
                            {
                                if (CommonUtil.dsTransData != null)
                                {
                                    RedirectObject o = new RedirectObject();
                                    o.URL = CommonUtil.GenerateURL("Common", "CMS020");

                                    JsonResult jRes = new JsonResult();
                                    jRes.Data = o;
                                    filterContext.Result = jRes;
                                }
                                else
                                {
                                    RedirectObject o = new RedirectObject();
                                    o.URL = CommonUtil.GenerateURL("Common", "CMS010") + "?timeout=1";

                                    JsonResult jRes = new JsonResult();
                                    jRes.Data = o;
                                    filterContext.Result = jRes;
                                    return;
                                }
                            }
                        }
                    }

                    DateTime dateTime=DateTime.Now;
                    dateTime = dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));
                    dsTrans.dtOperationData.ProcessDateTime = dateTime;
                    dsTrans.dtOperationData.GUID = Guid.NewGuid().ToString().ToUpper();
                    CommonUtil.dsTransData = dsTrans;
                }
                string actionUpper=action.ToUpper();
                if (actionUpper.Contains("EDIT")
                    || actionUpper.Contains("SAVE")
                    || actionUpper.Contains("UPDATE")
                    || actionUpper.Contains("DELETE"))
                {
                    string[] aItem = actionUpper.Split('_');
                    string screenID = aItem[0];
                    
                }
                base.OnActionExecuting(filterContext);
            }
            catch
            {
            }
        }
        
    }
}
