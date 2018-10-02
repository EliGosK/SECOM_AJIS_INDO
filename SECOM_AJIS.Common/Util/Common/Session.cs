using System;
using System.Web;
using SECOM_AJIS.Common.Models;
using System.Collections.Generic;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;

namespace SECOM_AJIS.Common.Util
{
    public partial class CommonUtil
    {
        /// <summary>
        /// DO of session
        /// </summary>
        public class ScreenObject
        {
            public static void SetScreenParameter(string key, ScreenParameter param)
            {
                try
                {
                    string sessionKey = string.Format("{0}.{1}", ConstantValue.CommonValue.SESSION_SCREEN_PARAMETER_KEY, key);
                    SetSession(sessionKey, param);
                }
                catch(Exception)
                {
                    throw;
                }
            }
            public static T GetScreenParameter<T>(string key) where T:new()
            {
                try
                {
                    T param = default(T);
                    if (CommonUtil.IsNullOrEmpty(key) == false)
                    {
                        string sessionKey = string.Format("{0}.{1}", ConstantValue.CommonValue.SESSION_SCREEN_PARAMETER_KEY, key);
                        param = GetSession<T>(sessionKey);
                    }
                    return param;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// system information of current page
        /// </summary>
        public static dsTransDataModel dsTransData
        {
            get
            {
                try
                {
                    if (HttpContext.Current == null)
                    {
                        return _dsTransTemp;
                    }
                    string sessionKey = CommonValue.SESSION_DSTRANSDATA_KEY;
                    return GetSession<dsTransDataModel>(sessionKey);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            set
            {
                try
                {
                    if (HttpContext.Current == null)
                    {
                        _dsTransTemp = value;
                        return;
                    }
                    string sessionKey = CommonValue.SESSION_DSTRANSDATA_KEY;
                    SetSession(sessionKey, value);
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }
        /// <summary>
        /// List of menu's name
        /// </summary>
        public static List<MenuName> MenuNameList
        {
            get
            {
                try
                {
                    string sessionKey = CommonValue.SESSION_MENU_KEY;
                    List<MenuName> data = GetSession<List<MenuName>>(sessionKey);

                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            set
            {
                try
                {
                    string sessionKey = CommonValue.SESSION_MENU_KEY;
                    SetSession(sessionKey, value);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        /// <summary>
        /// Generate session key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string SessionKey(string key)
        {
            try
            {
                if (HttpContext.Current != null)
                    return string.Format("{0};{1}", key, HttpContext.Current.Session.SessionID);
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Get object from session by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetSession<T>(string key)
        {
            try
            {
                string skey = SessionKey(key);

                if (HttpContext.Current != null && skey != null)
                {
                    object obj = HttpContext.Current.Session[skey];
                    if (obj != null
                        && obj is T)
                        return (T)obj;
                }

                return default(T);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Set object to session
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void SetSession(string key, object obj)
        {
            try
            {
                string skey = SessionKey(key);

                if (obj == null)
                    ClearSession(key);
                else
                {
                    if (HttpContext.Current.Session[skey] == null)
                        HttpContext.Current.Session.Add(skey, obj);
                    else
                        HttpContext.Current.Session[skey] = obj;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Remove object from session
        /// </summary>
        /// <param name="key"></param>
        public static void ClearSession(string key)
        {
            try
            {
                if (key != null)
                {
                    string skey = SessionKey(key);
                    HttpContext.Current.Session.Remove(skey);
                }
                else
                    HttpContext.Current.Session.RemoveAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static dsTransDataModel _dsTransTemp = null;

        public static void SetTransDataForJobScheduler(string userId, DateTime? batchDate)
        {
            dsTransDataModel dsTransTemp = new dsTransDataModel();

            dsTransTemp.dtTransHeader = new TransHeaderDo();
            dsTransTemp.dtTransHeader.Language = CommonValue.DEFAULT_LANGUAGE_EN;

            dsTransTemp.dtUserData = new UserDataDo()
            {
                EmpNo = userId,
                EmpFirstNameEN = userId,
                EmpFirstNameLC = userId,
                EmpLastNameEN = userId,
                EmpLastNameLC = userId,
            };
            dsTransTemp.dtUserBelongingData = new List<UserBelongingData>();
            dsTransTemp.dtOfficeData = new List<OfficeDataDo>();
            dsTransTemp.dtUserPermissionData = new Dictionary<string, UserPermissionDataDo>();

            dsTransTemp.dtOperationData = new OperationDataDo();
            dsTransTemp.dtOperationData.ProcessDateTime = (batchDate ?? DateTime.Now);

            dsTransTemp.dtCommonSearch = new CommonSearchDo();

            CommonUtil.dsTransData = dsTransTemp;
        }
    }
}
