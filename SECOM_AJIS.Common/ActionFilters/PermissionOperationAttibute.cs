using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Reflection;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Common.ActionFilters
{
    public class PermissionOperationAttibute : ActionFilterAttribute
    {
        public string Function { get; set; }

        /// <summary>
        /// Event on action executing for check user permission
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //filterContext.HttpContext.Response.Write("Before action:" + Function);
            try
            {
                PropertyInfo pinfo = typeof(FunctionID).GetProperty(Function);
                string functionId=pinfo.GetValue(null,null).ToString();

                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                if (dsTrans.dtUserPermissionData != null)
                {
                                                  
                    string screenId = filterContext.RouteData.Values["action"].ToString();
                    screenId = screenId.Substring(0, 6);
                    string permissionKey = screenId + "." + functionId;
                    if (!dsTrans.dtUserPermissionData.ContainsKey(permissionKey) == true)
                        return;

                    base.OnActionExecuting(filterContext);
                }
            }
            catch (NullReferenceException e)
            {
                
            }
        } 
    }
}
