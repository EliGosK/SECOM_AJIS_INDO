using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;

using SECOM_AJIS.Common.ActionFilters;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using System.Text;
using System.Web.Routing;
using System.Reflection;
using System.IO;

namespace SECOM_AJIS.Common.Controllers
{
    [HandleError]
    [Localization]
    [ScreenSession]
    [AutoRetry]
    public abstract class BaseController : Controller
    {
        protected override void  OnException(ExceptionContext filterContext)
        {
 	         base.OnException(filterContext);
        }

        #region Permission

        /// <summary>
        /// Check user permission by ScreenID and Function.
        /// </summary>
        /// <param name="ScreenID">Screen ID</param>
        /// <param name="FunctionID">Funtion ID</param>
        /// <returns></returns>
        protected bool CheckUserPermission(string ScreenID, string FunctionID)
        {
            try
            {
                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                if (dsTrans == null && ScreenID != null && FunctionID != null)
                    return false;

                if (dsTrans.dtUserPermissionData != null)
                {
                    string permissionKey = ScreenID + "." + FunctionID;
                    // 2016.06 modify tanaka start
                    return true;
                    //if (dsTrans.dtUserPermissionData.ContainsKey(permissionKey) == true)
                    //    return true;
                    // 20.16.06 modify tanaka end

                }
            }
            catch (Exception)
            {
            }

            return false;
        }
        /// <summary>
        /// Check user permission by ScreenID only.
        /// </summary>
        /// <param name="ScreenID">Screen ID</param>
        /// <returns></returns>
        protected bool CheckUserPermission(string ScreenID)
        {
             try
            {
                dsTransDataModel dsTrans = CommonUtil.dsTransData;
                if (dsTrans == null)
                    return false;

                if (dsTrans.dtUserPermissionData != null)
                {
                    var hasPermit = from a in dsTrans.dtUserPermissionData where a.Key.StartsWith(ScreenID) select a;
                    return (hasPermit.Count() > 0);
                }

            }
            catch 
            {
            }

            return false;
        }

        #endregion
        #region Parameter

        /// <summary>
        /// Get data from session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        protected T GetScreenParameter<T>(string key = null) where T : new()
        {
            try
            {
                T param = default(T);

                string rkey = key;
                if (rkey == null)
                    rkey = GetCurrentKey();
                HttpCookie cookie = this.HttpContext.Request.Cookies[rkey];
                if (cookie != null)
                {
                    string screenID = string.Empty;
                    string subObjectID = "0";
                    string callerKey = string.Empty;
                    string callerScreenID = string.Empty;
                    string callerModule = string.Empty;
                    bool isPopup = false;
                    string time = string.Empty;
                    if (cookie.HasKeys)
                    {
                        screenID = cookie.Values["ScreenID"];
                        callerModule = cookie.Values["CallerModule"];
                        callerScreenID = cookie.Values["CallerScreenID"];
                        callerKey = cookie.Values["CallerKey"];
                        isPopup = (cookie.Values["IsPopup"] == "1");
                        time = cookie.Values["TimeKey"];

                        if (CommonUtil.IsNullOrEmpty(cookie.Values["SubObjectID"]) == false)
                            subObjectID = cookie.Values["SubObjectID"];
                    }

                    if (CommonUtil.IsNullOrEmpty(screenID) == false
                        && CommonUtil.IsNullOrEmpty(time) == false)
                    {
                        string skey = string.Format("{0}.{1}", screenID, time);
                        param = CommonUtil.ScreenObject.GetScreenParameter<T>(skey);
                    }
                    if (param != null)
                    {
                        ScreenParameter sparam = param as ScreenParameter;
                        if (sparam != null)
                        {
                            sparam.ScreenID = screenID;
                            sparam.CallerScreenID = callerScreenID;
                            sparam.CallerModule = callerModule;
                            sparam.CallerKey = callerKey;
                            sparam.IsPopup = isPopup;
                            sparam.SubObjectID = subObjectID;
                        }
                    }
                }
                else if (rkey != null)
                {
                    throw new Exception("Cookie is missing");
                }

                return param;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Update data in session
        /// </summary>
        /// <param name="param"></param>
        /// <param name="key"></param>
        protected void UpdateScreenParameter(ScreenParameter param, string key = null)
        {
            try
            {
                string rkey = key;
                if (rkey == null)
                    rkey = GetCurrentKey();
                HttpCookie cookie = this.HttpContext.Request.Cookies[rkey];
                if (cookie != null)
                {
                    string screenID = string.Empty;
                    string time = string.Empty;
                    if (cookie.HasKeys)
                    {
                        screenID = cookie.Values["ScreenID"];
                        time = cookie.Values["TimeKey"];
                    }

                    string skey = string.Format("{0}.{1}", screenID, time);
                    CommonUtil.ScreenObject.SetScreenParameter(skey, param);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region Initial

        /// <summary>
        /// Initial screen environment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="param"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected ActionResult InitialScreenEnvironment<T>(string action, ScreenParameter param, ObjectResultData result = null) where T : new()
        {
            ObjectResultData res = result;
            if (res == null)
                res = new ObjectResultData();

            try
            {
                if (res.IsError == false)
                {
                    string key = null;
                    if (action != SECOM_AJIS.Common.Util.ConstantValue.ScreenID.C_SCREEN_ID_MAIN)
                    {
                        #region Generate KEY

                        key = CommonUtil.GetMD5Hash(action + DateTime.Now.Ticks.ToString());

                        #endregion
                        #region Create Object for this session

                        if (param == null)
                            param = new T() as ScreenParameter;
                        param.Key = key;
                        param.Module = this.GetType().Name.Replace("Controller", "");
                        param.ScreenID = action;

                        if (CommonUtil.IsNullOrEmpty(param.SubObjectID))
                            param.SubObjectID = "0";

                        CommonUtil.ScreenObject.SetScreenParameter(key, param);

                        #endregion
                    }

                    #region Call screen URL

                    res.ResultData = CallScreenURL(param);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Get data from session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetScreenObject<T>(string key = null) where T : new()
        {
            try
            {
                string rkey = key;
                if (rkey == null)
                    rkey = GetCurrentKey();

                if (CommonUtil.IsNullOrEmpty(rkey) == false)
                {
                    T obj = CommonUtil.ScreenObject.GetScreenParameter<T>(rkey);
                    if (obj != null && obj is ScreenParameter)
                    {
                        ScreenParameter po = obj as ScreenParameter;
                        if (po.CallerKey != null)
                        {
                            ScreenParameter cpo = (ScreenParameter)CommonUtil.ScreenObject.GetScreenParameter<object>(po.CallerKey);
                            if (cpo != null)
                                cpo.IsLoaded = false;
                        }
                    }

                    return obj;
                }

                return default(T);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Update data in session
        /// </summary>
        /// <param name="param"></param>
        /// <param name="key"></param>
        protected void UpdateScreenObject(ScreenParameter param, string key = null)
        {
            try
            {
                string rkey = key;
                if (rkey == null)
                    rkey = GetCurrentKey();

                if (CommonUtil.IsNullOrEmpty(rkey) == false)
                    CommonUtil.ScreenObject.SetScreenParameter(rkey, param);
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion
        #region Actions

        /// <summary>
        /// Reset session data
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetSessionData()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ScreenParameter param = GetScreenObject<object>() as ScreenParameter;
                if (param != null)
                {
                    ScreenParameter nparam = ScreenParameter.ResetScreenParameter(param);
                    if (nparam != null)
                        UpdateScreenObject(nparam);
                }

                res.ResultData = true;
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        /// <summary>
        /// Clear data from session
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearCurrentScreenSession()
        {
            try
            {
                ScreenParameter param = GetScreenObject<object>() as ScreenParameter;
                if (param != null)
                {
                    string methodName = string.Format("{0}_ClearSession", param.ScreenID);
                    MethodInfo method = this.GetType().GetMethod(methodName);
                    if (method != null)
                        method.Invoke(this, null);
                    else
                        UpdateScreenObject(null);
                }
            }
            catch (Exception)
            {
            }

            return Json(this.HttpContext.Session.Keys.Count);
        }
        /// <summary>
        /// Check it has data in session?
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckSession()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ScreenParameter param = GetScreenObject<object>() as ScreenParameter;
                if (param != null)
                {
                    if (param.BackStep == true)
                    {
                        param.BackStep = false;
                        res.ResultData = true;
                    }
                }
            }
            catch (Exception ex)
            {
                res.AddErrorMessage(ex);
            }

            return Json(res);
        }
        public ActionResult RedirectToLogin()
        {
            return RedirectToAction("CMS010", "Common", new { timeout = 1 });
        }

        #endregion
        #region Parameter Keys

        /// <summary>
        /// Get current session's key
        /// </summary>
        /// <returns></returns>
        protected string GetCurrentKey()
        {
            return this.HttpContext.Request["k"];
        }

        #endregion
        #region Methods

        /// <summary>
        /// Method for generate URL
        /// </summary>
        /// <param name="param"></param>
        /// <param name="CallerScreen"></param>
        /// <returns></returns>
        protected string CallScreenURL(ScreenParameter param, bool CallerScreen = false)
        {
            try
            {
                if (param == null)
                    return null;

                string key = param.Key;
                string module = param.Module;
                string screenID = param.ScreenID;
                if (CallerScreen == true)
                {
                    key = param.CallerKey;
                    module = param.CallerModule;
                    screenID = param.CallerScreenID;
                }

                if (CommonUtil.IsNullOrEmpty(key) == false)
                {
                    return string.Format("/{0}/{1}?k={2}",
                                            module,
                                            screenID,
                                            key);
                }
                else
                {
                    return string.Format("/{0}/{1}",
                                            module,
                                            screenID);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Fixed Json

        /// <summary>
        /// Override Json method
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected internal new JsonResult Json(object data)
        {
            return new LargeJsonResult { Data = data };
        }

        #endregion

        #region DownloadFile

        /// <summary>
        /// Convert string to stream
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private MemoryStream ConvertStringToStream(string strData)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(strData);
            writer.Flush();

            return stream;
        }

        /// <summary>
        /// Download file in CSV format
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="strCSVData"></param>
        protected void DownloadCSVFile(string strFileName, string strCSVData)
        {
            //Modiify by Jutarat A. 24072012
            //MemoryStream streamData = new MemoryStream();
            //if (String.IsNullOrEmpty(strCSVData) == false)
            //{
            //    streamData = this.ConvertStringToStream(strCSVData);

            //    Response.Clear();
            //    Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", strFileName));
            //    Response.ContentType = "text/csv";
            //    Response.Charset = "UTF-8";
            //    Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");

            //    //add the BOM
            //    byte[] bBOM = new byte[] { 0xEF, 0xBB, 0xBF };
            //    byte[] bContent = streamData.ToArray();
            //    byte[] bToWrite = new byte[bBOM.Length + bContent.Length];

            //    //combile the BOM and the content
            //    bBOM.CopyTo(bToWrite, 0);
            //    bContent.CopyTo(bToWrite, bBOM.Length);

            //    //write to the client
            //    Response.Write(Encoding.UTF8.GetString(bToWrite));
            //    Response.Flush();
            //    Response.End();

            //    streamData.Dispose();
            //}
            if (String.IsNullOrEmpty(strCSVData) == false)
            {
                if (String.IsNullOrEmpty(strFileName) == false)
                    strFileName = strFileName.Replace(' ', '_');

                Response.Clear();
                Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", strFileName));
                Response.ContentType = "text/csv";
                Response.Charset = "TIS-620";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("TIS-620");

                byte[] b = Response.ContentEncoding.GetBytes(strCSVData);
                char[] c = Response.ContentEncoding.GetChars(b);

                //write to the client
                Response.Write(c, 0, c.Length);
                Response.Flush();
                Response.End();
            }
            //End Modify
        }

        /// <summary>
        /// Download file in CSV format
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="btCSVData"></param>
        protected void DownloadCSVFile(string strFileName, byte[] btCSVData)
        {
            if (btCSVData != null)
            {
                if (String.IsNullOrEmpty(strFileName) == false)
                    strFileName = strFileName.Replace(' ', '_');

                Response.Clear();
                Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", strFileName));
                Response.ContentType = "text/csv";
                Response.Charset = "TIS-620";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("TIS-620");

                //write to the client
                Response.BinaryWrite(btCSVData);
                Response.Flush();
                Response.End();
            }
        }

        /// <summary>
        /// Download file in PDF format
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="btPDFData"></param>
        protected void DownloadPDFFile(string strFileName, byte[] btPDFData)
        {
            if (btPDFData != null)
            {
                if (String.IsNullOrEmpty(strFileName) == false)
                    strFileName = strFileName.Replace(' ', '_');

                Response.Clear();
                Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", strFileName));
                Response.ContentType = "application/pdf";
                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                Response.BinaryWrite(btPDFData);
                Response.Flush();
                Response.End();
            }
        }

        /// <summary>
        /// Download all file
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="btCSVData"></param>
        protected void DownloadAllFile(string strFileName, Stream stStreamReport)
        {
            if (stStreamReport != null)
            {
                if (String.IsNullOrEmpty(strFileName) == false)
                    strFileName = strFileName.Replace(' ', '_');

                MemoryStream msStreamReport = new MemoryStream();
                stStreamReport.CopyTo(msStreamReport);

                Response.Clear();
                Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", strFileName));
                Response.ContentType = "application/octet-stream";
                //Response.Charset = "UTF-8";
                //Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");

                //write to the client
                Response.BinaryWrite(msStreamReport.ToArray());
                Response.Flush();
                Response.End();
            }
        }

        public ActionResult InitialDownloadFile()
        {
            ObjectResultData res = new ObjectResultData();
            try
            {
                ScreenParameter param = GetScreenObject<object>() as ScreenParameter;
                if (param != null)
                {
                    param.IsLoaded = false;
                    param.BackStep = true;
                }

                res.ResultData = true;
            }
            catch
            {
            }

            return Json(res);
        }

        #endregion

    }


}
