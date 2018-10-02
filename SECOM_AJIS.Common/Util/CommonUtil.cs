using System;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using System.Linq;
using System.Diagnostics;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.Xml;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text;


namespace SECOM_AJIS.Common.Util
{
    /// <summary>
    /// Common function
    /// </summary>
    public partial class CommonUtil
    {
        #region Validation

        /// <summary>
        /// Check object is null or empty
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(object obj)
        {
            if (obj != null)
            {
                if (obj is string)
                {
                    if (string.IsNullOrEmpty((string)obj) == false)
                        return false;
                }
                else if ((obj is DBNull) == false)
                {
                    return false;
                }
                else
                    return false;
            }

            return true;
        }

        #endregion
        #region Attribute

        /// <summary>
        /// Generate attribute list of object
        /// </summary>
        /// <typeparam name="ATTR"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, ATTR> CreateAttributeDictionary<ATTR>(object obj)
            where ATTR : class
        {
            try
            {
                return CreateAttributeDictionary<ATTR>(obj.GetType());
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Generate attribute list of object
        /// </summary>
        /// <typeparam name="ATTR"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<string, ATTR> CreateAttributeDictionary<ATTR>(Type t)
            where ATTR : class
        {
            try
            {
                Dictionary<string, ATTR> dicAttr = new Dictionary<string, ATTR>();
                if (t != null)
                {
                    #region Get Attribute from Meta Data Object

                    object[] mtAttr = t.GetCustomAttributes(typeof(MetadataTypeAttribute), true);
                    if (mtAttr.Length > 0)
                    {
                        MetadataTypeAttribute metadata = mtAttr[0] as MetadataTypeAttribute;
                        if (metadata.MetadataClassType != null)
                        {
                            foreach (PropertyInfo prop in metadata.MetadataClassType.GetProperties())
                            {
                                object[] objAttr = prop.GetCustomAttributes(typeof(ATTR), true);
                                if (objAttr.Length <= 0)
                                    continue;
                                dicAttr.Add(prop.Name, objAttr[0] as ATTR);
                            }
                        }
                    }

                    #endregion
                    #region Get Attribute from Object instance

                    foreach (PropertyInfo prop in t.GetProperties())
                    {
                        object[] objAttr = prop.GetCustomAttributes(typeof(ATTR), true);
                        if (objAttr.Length <= 0)
                            continue;
                        dicAttr.Add(prop.Name, objAttr[0] as ATTR);
                    }

                    #endregion
                }

                return dicAttr;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region Cryptography

        /// <summary>
        /// Encode text in format MD5
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string GetMD5Hash(string txt)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = provider.ComputeHash(UTF8Encoding.UTF8.GetBytes(txt));

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
        public static string HashPassword(string password)
        {
            try
            {
                var hash = System.Security.Cryptography.SHA1.Create();
                var encoder = new System.Text.ASCIIEncoding();
                var combined = encoder.GetBytes(password ?? "");
                return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #region Variables

        private XmlDocument resDoc;

        #endregion
        #region Connections

        /// <summary>
        /// Load xml from file
        /// </summary>
        /// <param name="file_code"></param>
        public void InitResourceXml(string file_code)
        {
            string lang = CommonUtil.GetCurrentLanguage();
            if (lang == ConstantValue.CommonValue.DEFAULT_LANGUAGE_EN)
                lang = string.Empty;
            else
                lang = "." + lang;

            string resourcePath = string.Format("{0}{1}\\{2}{3}.resx",
                                                            CommonUtil.WebPath,
                                                            ConstantValue.CommonValue.APP_GLOBAL_RESOURCE_FOLDER,
                                                            file_code,
                                                            lang);
            resDoc = new XmlDocument();
            resDoc.Load(resourcePath);
        }

        #endregion
        #region Varidation
        
        /// <summary>
        /// Check properties of object is null or empty
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string[] IsNullOrEmpty(object obj, params string[] param)
        {
            List<string> nullLst = new List<string>();
            if (obj != null && param != null)
            {
                if (param.Length > 0)
                {
                    for (int idx = 0; idx < param.Length; idx++)
                    {
                        PropertyInfo prop = obj.GetType().GetProperty(param[idx]);
                        if (prop != null)
                        {
                            object val = prop.GetValue(obj, null);
                            if (IsNullOrEmpty(val))
                                nullLst.Add(param[idx]);
                        }
                    }
                }
            }

            if (nullLst.Count > 0)
            {
                string[] res = new string[nullLst.Count];
                for (int idx = 0; idx < nullLst.Count; idx++)
                {
                    res[idx] = nullLst[idx];
                }
                return res;
            }

            return null;
        }
        /// <summary>
        /// Check all proprty in object is null or empty
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramIggnoreField"></param>
        /// <returns></returns>
        public static bool IsNullAllField(object obj, params string[] paramIggnoreField)
        {
            PropertyInfo[] props = obj.GetType().GetProperties();

            //Fix bug by Non A. 2/Mar/2012
            props = (from m in props where !paramIggnoreField.Contains<string>(m.Name) select m).ToArray<PropertyInfo>();

            ObjectMandatoryField f = new ObjectMandatoryField();
            for (int i = 0; i < props.Length; i++)
            {
                f.AddProperty(props[i].Name, props[i].Name);
            }

            string[][] strNullList = CheckMandatoryFiled(obj, f);

            if (strNullList != null)
            {
                if (strNullList.Length > 0)
                {
                    return (strNullList[0].Length == props.Length);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Check mandatory field
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string[][] CheckMandatoryFiled(object obj, params string[] field)
        {
            ObjectMandatoryField omf = new ObjectMandatoryField();
            if (field != null)
            {
                foreach (string f in field)
                {
                    MandatoryField mc = new MandatoryField();
                    mc.FieldName = f;
                    mc.ControlName = f;
                    mc.MandatoryMessage = f;
                    omf.AddProperty(mc);
                }
            }
            else if (obj != null && obj is object)
            {
                PropertyInfo[] props = obj.GetType().GetProperties();
                if (props != null)
                {
                    foreach (PropertyInfo prop in props)
                    {
                        MandatoryField mc = new MandatoryField();
                        mc.FieldName = prop.Name;
                        mc.ControlName = prop.Name;
                        mc.MandatoryMessage = prop.Name;
                        omf.AddProperty(mc);
                    }
                }
            }

            return CheckMandatoryFiled(obj, omf);
        }
        /// <summary>
        /// Check mandatory field
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="field"></param>
        /// <param name="return_ctrl"></param>
        /// <returns></returns>
        public static string[][] CheckMandatoryFiled(object obj, MandatoryField field, bool return_ctrl = false)
        {
            List<string> nullLst = new List<string>();
            List<string> nullCtrlLst = new List<string>();

            if (field is ListMandatoryField)
            {
                ListMandatoryField lfield = field as ListMandatoryField;

                if (IsNullOrEmpty(obj))
                {
                    nullLst.Add(field.MandatoryMessage);
                    nullCtrlLst.Add(field.FieldName);
                }
                else
                {
                    IList lst = null;
                    if (obj is HttpRequestBase)
                    {
                        lst = new List<string>();

                        HttpRequestBase req = obj as HttpRequestBase;
                        if (lfield.Rows != null)
                        {
                            foreach (MandatoryField mf in lfield.Rows)
                            {
                                lst.Add(req[mf.FieldName]);
                            }
                        }
                    }
                    else
                    {
                        lst = obj as IList;
                        if (lst.Count <= 0)
                        {
                            nullLst.Add(field.MandatoryMessage);
                            nullCtrlLst.Add(field.FieldName);
                        }
                    }

                    if (lst.Count > 0)
                    {
                        for (int idx = 0; idx < lst.Count; idx++)
                        {
                            if (lfield.Index != null)
                            {
                                if (idx > lfield.Index)
                                    break;
                                if (idx != lfield.Index)
                                    continue;
                            }

                            MandatoryField mf = lfield.Field;
                            if (mf == null && lfield.Rows != null)
                            {
                                if (idx < lfield.Rows.Count)
                                    mf = lfield.Rows[idx];
                            }
                            if (mf == null)
                                break;

                            string[][] sLst = CheckMandatoryFiled(lst[idx], mf, return_ctrl);
                            if (sLst != null)
                            {
                                if (lfield.Rows != null
                                    || lfield.Index != null)
                                {
                                    nullLst.AddRange(sLst[0]);

                                    if (sLst.Length > 1)
                                        nullCtrlLst.AddRange(sLst[1]);
                                }
                                else
                                {
                                    string txt = string.Format("Row {0} [{1}]", (idx + 1), TextList(sLst[0]));
                                    if (field.MandatoryMessage != null)
                                        txt = field.MandatoryMessage + " " + txt;
                                    nullLst.Add(txt);

                                    if (sLst.Length > 1)
                                        nullCtrlLst.Add(string.Format("{0}:{1}", (idx + 1), TextList(sLst[1])));

                                    if (lfield.IsBreakLoop)
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            else if (field is ObjectMandatoryField)
            {
                ObjectMandatoryField ofield = field as ObjectMandatoryField;
                if (ofield.Properties != null)
                {
                    if (obj == null)
                    {
                        nullLst.Add(ofield.MandatoryMessage);
                        nullCtrlLst.Add(ofield.ControlName);
                    }
                    else
                    {
                        foreach (MandatoryField mf in ofield.Properties)
                        {
                            PropertyInfo prop = obj.GetType().GetProperty(mf.FieldName);
                            if (prop != null)
                            {
                                object val = prop.GetValue(obj, null);
                                string[][] sLst = CheckMandatoryFiled(val, mf, return_ctrl);
                                if (sLst != null)
                                {
                                    nullLst.AddRange(sLst[0]);

                                    if (sLst.Length > 1)
                                        nullCtrlLst.AddRange(sLst[1]);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (field.FieldName == null || obj == null)
                {
                    if (IsNullOrEmpty(obj))
                    {
                        nullLst.Add(field.MandatoryMessage != null ? field.MandatoryMessage : "");
                        nullCtrlLst.Add(field.ControlName != null ? field.ControlName : field.FieldName != null ? field.FieldName : "");
                    }
                }
                else
                {
                    PropertyInfo prop = obj.GetType().GetProperty(field.FieldName);
                    if (prop != null)
                    {
                        object val = prop.GetValue(obj, null);
                        if (IsNullOrEmpty(val))
                        {
                            nullLst.Add(field.MandatoryMessage);
                            nullCtrlLst.Add(field.ControlName != null ? field.ControlName : field.FieldName);
                        }
                    }
                    else
                    {
                        if (IsNullOrEmpty(obj))
                        {
                            nullLst.Add(field.MandatoryMessage);
                            nullCtrlLst.Add(field.ControlName != null ? field.ControlName : field.FieldName);
                        }
                    }
                }
            }

            if (nullLst.Count > 0)
            {
                int length = 1;
                if (return_ctrl)
                    length = 2;

                string[][] res = new string[length][];
                res[0] = nullLst.ToArray();

                if (length == 2 && nullCtrlLst.Count > 0)
                    res[1] = nullCtrlLst.ToArray();

                return res;
            }

            return null;
        }

        #endregion
        #region Objects

        /// <summary>
        /// Mapping current language
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="obj"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static R ConvertObjectbyLanguage<T, R>(T obj, params string[] fields)
            where T : class
            where R : new()
        {
            object objRes = null;
            if (typeof(T) != typeof(R))
            {
                R nR = CloneObject<T, R>(obj);
                objRes = nR;
            }
            else
                objRes = obj;

            try
            {
                CommonUtil.LANGUAGE_LIST lang = CurrentLanguage();
                foreach (string field in fields)
                {
                    PropertyInfo propEN = typeof(T).GetProperty(field + CommonValue.LANGUAGE_EN);
                    PropertyInfo propJP = typeof(T).GetProperty(field + CommonValue.LANGUAGE_JP);
                    PropertyInfo propLC = typeof(T).GetProperty(field + CommonValue.LANGUAGE_LC);

                    if (objRes != null)
                    {
                        PropertyInfo prop = objRes.GetType().GetProperty(field);
                        if (prop != null)
                        {
                            object val = null;
                            if (propEN != null && lang == LANGUAGE_LIST.LANGUAGE_1
                                || (propJP == null && lang == LANGUAGE_LIST.LANGUAGE_2))
                                val = propEN.GetValue(obj, null);
                            else if (propJP != null && lang == LANGUAGE_LIST.LANGUAGE_2)
                                val = propJP.GetValue(obj, null);
                            else if (propLC != null)
                                val = propLC.GetValue(obj, null);

                            if (val != null)
                                prop.SetValue(objRes, val, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return (R)objRes;
        }
        /// <summary>
        /// Mapping current language
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="lst"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static List<R> ConvertObjectbyLanguage<T, R>(List<T> lst, params string[] fields)
            where T : class
            where R : new()
        {
            List<R> nLst = null;
            if (typeof(T) != typeof(R))
                nLst = new List<R>();

            try
            {
                if (lst != null)
                {
                    CommonUtil.LANGUAGE_LIST lang = CurrentLanguage();
                    foreach (T ilst in lst)
                    {
                        object obj = null;
                        foreach (string field in fields)
                        {
                            PropertyInfo propEN = typeof(T).GetProperty(field + CommonValue.LANGUAGE_EN);
                            PropertyInfo propJP = typeof(T).GetProperty(field + CommonValue.LANGUAGE_JP);
                            PropertyInfo propLC = typeof(T).GetProperty(field + CommonValue.LANGUAGE_LC);

                            if (nLst != null)
                            {
                                if (obj == null)
                                {
                                    /* --- Clone --- */
                                    R nR = CloneObject<T, R>(ilst);
                                    nLst.Add(nR);
                                    obj = nR;
                                }
                            }
                            else
                                obj = ilst;

                            if (obj != null)
                            {
                                PropertyInfo prop = obj.GetType().GetProperty(field);
                                if (prop != null)
                                {
                                    object val = null;
                                    if (propEN != null && lang == LANGUAGE_LIST.LANGUAGE_1
                                        || (propJP == null && lang == LANGUAGE_LIST.LANGUAGE_2))
                                        val = propEN.GetValue(ilst, null);
                                    else if (propJP != null && lang == LANGUAGE_LIST.LANGUAGE_2)
                                        val = propJP.GetValue(ilst, null);
                                    else if (propLC != null)
                                        val = propLC.GetValue(ilst, null);

                                    if (val != null)
                                        prop.SetValue(obj, val, null);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (nLst != null)
                return nLst;
            else
                return lst as List<R>;
        }
        /// <summary>
        /// Convert json data to object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="request"></param>
        public static void MappingObjectData(object obj, string name, HttpRequestBase request)
        {
            try
            {
                if ( obj != null && request != null)
                {
                    string format = "{0}";
                    if (name != null)
                        format = name + "[{0}]";

                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        string key = string.Format(format, prop.Name);
                        
                        if (prop.PropertyType.Namespace.StartsWith("SECOM_AJIS") == true)
                        {
                            object rObj = prop.GetValue(obj, null);
                            if (rObj == null)
                            {
                                rObj = Activator.CreateInstance(prop.PropertyType);
                                prop.SetValue(obj, rObj, null);
                            }
                            MappingObjectData(rObj, key, request);
                        }
                        else if (request[key] != null)
                        {
                            if (request[key] == "null")
                                SetObjectValue(prop, obj, null);
                            else
                                SetObjectValue(prop, obj, request[key]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Create json data from object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="colNames"></param>
        /// <returns></returns>
        public static string CreateJsonString<T>(T obj, string[] colNames) where T : class
        {
            string strJson = "";
            string strValue = "";
            PropertyInfo prop;

            for (int i = 0; i < colNames.Length; i++)
            {
                prop = obj.GetType().GetProperty(colNames[i]);
                strValue = prop.GetValue(obj, null).ToString();
                strJson += string.Format("\"{0}\":\"{1}\"", colNames[i], strValue);
                if (i < colNames.Length - 1)
                {
                    strJson += ",";
                }
            }

            if (colNames.Length > 0)
            {
                strJson = "{" + strJson + "}";
            }

            return strJson;
        }
        //public static string CreateJsonString<T>(T obj) where T : class
        //{
        //    string strJson = "";
        //    string strValue = "";

        //    PropertyInfo[] props = typeof(T).GetProperties();

        //    for (int i = 0; i < props.Length; i++)
        //    {
        //        if (props[i].Name != "ToJson")
        //        {
        //            strValue = props[i].GetValue(obj, null).ToString();
        //            strJson += string.Format("\"{0}\":\"{1}\"", props[i].Name, strValue);
        //            if (i < props.Length - 1)
        //            {
        //                strJson += ",";
        //            }
        //        }
        //    }

        //    if (props.Length > 0)
        //    {
        //        strJson = "{" + strJson + "}";
        //    }

        //    return strJson;
        //}

        #endregion
        #region DateTime

        /// <summary>
        /// Get first day of month
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonthFromDateTime(int month, int year)
        {
            return new DateTime(year, month, 1);
        }
        /// <summary>
        /// Get last day of month
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime LastDayOfMonthFromDateTime(int month, int year)
        {
            DateTime firstDayOfTheMonth = new DateTime(year, month, 1);
            return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }
        /// <summary>
        /// Get last day of month
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DateTime LastDayOfMonthFromDateTime(DateTime? d)
        {
            if (d.HasValue)
            {
                DateTime firstDayOfTheMonth = new DateTime(d.Value.Year, d.Value.Month, 1);
                return firstDayOfTheMonth.AddMonths(1).AddDays(-1);
            }
            else
            {
                return LastDayOfMonthFromDateTime( DateTime.Now);
            }
           
        }

        #endregion
        #region WindowEventLog

        /// <summary>
        /// Write window log
        /// </summary>
        /// <param name="eEventType"></param>
        /// <param name="strMessage"></param>
        public static void WriteWindowLog(string eEventType, string strMessage,int EventID)
        {

            string logName = "Application";
            string source = "SECOM-AJIS web application";

            EventLog objLog = new EventLog();

            try
            {
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, logName);
                }

                objLog.Source = source;
                objLog.Log = logName;


                if (eEventType == EventType.C_EVENT_TYPE_ERROR)
                {
                    objLog.WriteEntry(strMessage, EventLogEntryType.Error, EventID);
                }
                else if (eEventType == EventType.C_EVENT_TYPE_WARNING)
                {
                    objLog.WriteEntry(strMessage, EventLogEntryType.Warning, EventID);
                }
                else
                {
                    objLog.WriteEntry(strMessage, EventLogEntryType.Information, EventID);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        #endregion

        public static string TargetCurrency(string currencyType)
        {
            if (currencyType == CurrencyUtil.C_CURRENCY_LOCAL)
            {
                return "Rp.";
            }
            else if (currencyType == CurrencyUtil.C_CURRENCY_US)
            {
                return "US$";
            }
            else
            {
                return "-";
            }

        }

        public static decimal TargetFee(string currencyType, decimal? fee, decimal? feeUsd)
        {
            if (currencyType == CurrencyUtil.C_CURRENCY_LOCAL)
            {
                if (CommonUtil.IsNullOrEmpty(fee))
                {
                    return 0;
                }
                return (decimal)fee;
            }
            else if (currencyType == CurrencyUtil.C_CURRENCY_US)
            {
                if (CommonUtil.IsNullOrEmpty(feeUsd))
                {
                    return 0;
                }
                return (decimal)feeUsd;
            }
            else
            {
                return 0;
            }
        }

        public static string ContractOfTargetCurrency(decimal fee, string currencyType)
        {
            if(fee == 0)
            {
                return null;
            }
            else
            {
                return TargetCurrency(currencyType);
            }
        }

        public static decimal ContractOfTargetFee(decimal fee)
        {
            if(fee == 0)
            {
                return 0;

            }
            else
            {
                return fee ;
            }
        }
    }
}
