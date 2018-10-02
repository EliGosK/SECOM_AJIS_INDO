using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Globalization;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.Common.Util
{
    public partial class CommonUtil
    {
        /// <summary>
        /// Clone object data from T class to R class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static R CloneObject<T, R>(T obj)
            where T : class
            where R : new()
        {
            R nObj = new R();

            if (obj != null)
            {
                Type rT = typeof(R);
                PropertyInfo[] props = typeof(T).GetProperties();
                if (props != null)
                {
                    foreach (PropertyInfo prop in props)
                    {
                        PropertyInfo nprop = rT.GetProperty(prop.Name);
                        object val = prop.GetValue(obj, null);
                        if (nprop != null && val != null)
                        {
                            Type nt = nprop.PropertyType;
                            if (nprop.PropertyType.IsGenericType)
                                nt = nprop.PropertyType.GetGenericArguments()[0];

                            Type t = prop.PropertyType;
                            if (prop.PropertyType.IsGenericType)
                                t = prop.PropertyType.GetGenericArguments()[0];

                            if (nprop.CanWrite
                                && nt == t)
                                nprop.SetValue(nObj, val, null);
                        }
                    }
                }
            }
            return nObj;
        }
        /// <summary>
        /// Clone list of object data from T class to R class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<R> ClonsObjectList<T, R>(List<T> obj)
            where T : class
            where R : new()
        {
            try
            {
                List<R> lst = Activator.CreateInstance(typeof(List<R>)) as List<R>;
                if (obj != null)
                {
                    foreach (T o in obj)
                    {
                        R r = CloneObject<T, R>(o);
                        if (r != null)
                            lst.Add(r);
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Convert object to Json format
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static string CreateJsonString(object obj, params string[] cols)
        {
            string txt = string.Empty;

            if (obj != null)
            {
                PropertyInfo[] props = null;
                if (cols != null)
                {
                    if (cols.Length > 0)
                    {
                        List<PropertyInfo> pLst = new List<PropertyInfo>();
                        foreach (string col in cols)
                        {
                            PropertyInfo prop = obj.GetType().GetProperty(col);
                            if (prop != null)
                                pLst.Add(prop);
                        }
                        props = pLst.ToArray();
                    }
                }
                if (props == null)
                    props = obj.GetType().GetProperties();

                if (props != null)
                {
                    foreach (PropertyInfo prop in props)
                    {
                        if (prop.Name != "ToJson")
                        {
                            bool isObject = false;
                            string txtVal = string.Empty;
                            object val = prop.GetValue(obj, null);
                            if (val == null)
                            {
                                isObject = true;
                                txtVal = "null";
                            }
                            else
                            {
                                PropertyInfo jprop = val.GetType().GetProperty("ToJson");
                                if (jprop != null)
                                {
                                    isObject = true;
                                    txtVal = (string)jprop.GetValue(val, null);
                                }
                                else if (val.GetType().IsEnum)
                                {
                                    txtVal = ((int)val).ToString();
                                }
                                else if (val.GetType().IsGenericType)
                                {
                                    MethodInfo mt = val.GetType().GetMethod("ToArray");
                                    if (mt != null)
                                    {
                                        object[] lval = (object[])mt.Invoke(val, null);
                                        foreach (object gval in lval)
                                        {
                                            string itxt = string.Empty;
                                            if (gval.GetType() == typeof(string))
                                            {
                                                if (gval != null)
                                                    itxt = string.Format("\"{0}\"", gval.ToString().Replace("\"", "\\\""));
                                            }
                                            else
                                                itxt = CreateJsonString(gval);

                                            if (itxt != string.Empty)
                                            {
                                                if (txtVal != string.Empty)
                                                    txtVal += ",";
                                                txtVal += itxt;
                                            }
                                        }
                                        if (txtVal != string.Empty)
                                        {
                                            isObject = true;
                                            txtVal = string.Format("[{0}]", txtVal);
                                        }
                                    }
                                    else
                                    {
                                        PropertyInfo gprop = val.GetType().GetProperty("Value");
                                        if (gprop != null)
                                        {
                                            object gval = gprop.GetValue(val, null);
                                            if (gval != null)
                                                txtVal = gval.ToString();
                                        }
                                    }
                                }
                                else if (val.GetType().Namespace.StartsWith("SECOM_AJIS") == true)
                                {
                                    isObject = true;
                                    txtVal = CreateJsonString(val);
                                }
                                else
                                {
                                    txtVal = val.ToString().Replace("\"", "\\\"");

                                    if (val.GetType() == typeof(bool))
                                    {
                                        isObject = true;
                                        txtVal = txtVal.ToLower();
                                    }
                                }
                            }

                            if (txt != string.Empty)
                                txt += ",";
                            if (isObject)
                                txt += string.Format("\"{0}\":{1}", prop.Name, txtVal);
                            else
                                txt += string.Format("\"{0}\":\"{1}\"", prop.Name, txtVal);
                        }
                    }
                }
            }

            if (txt != string.Empty)
                txt = "{" + txt + "}";
            return txt;
        }
        /// <summary>
        /// Set value to object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        public static bool SetObjectValue(object obj, string propName, string value, string dateFormat = null )
        {
            try
            {
                if (CommonUtil.IsNullOrEmpty(value))
                    return true;

                if (obj != null)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(propName);

                    if (prop.PropertyType == typeof(string))
                    {
                        Dictionary<string, MaxTextLengthAttribute> miscAttr = CommonUtil.CreateAttributeDictionary<MaxTextLengthAttribute>(obj);
                        foreach (KeyValuePair<string, MaxTextLengthAttribute> attr in miscAttr)
                        {
                            if (attr.Key == propName)
                            {
                                if (value.Length > attr.Value.MaxLength)
                                    value = value.Substring(0, attr.Value.MaxLength);
                                break;
                            }
                        }
                    }
                    return SetObjectValue(prop, obj, value, dateFormat);
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Set value to object
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        public static bool SetObjectValue(PropertyInfo prop, object obj, string value,string dateFormat = null)
        {
            try
            {
                if (prop != null)
                {
                    if (prop.CanWrite == false)
                        return false;

                    if (prop.PropertyType == typeof(bool)
                        || prop.PropertyType == typeof(bool?))
                    {
                        bool b;
                        if (bool.TryParse(value, out b))
                            prop.SetValue(obj, b, null);
                        else if (value == "1")
                            prop.SetValue(obj, true, null);
                        else if (value == "0")
                            prop.SetValue(obj, false, null);
                        else
                            return false;
                    }
                    else if (prop.PropertyType == typeof(decimal)
                        || prop.PropertyType == typeof(decimal?))
                    {
                        decimal dec;
                        if (decimal.TryParse(value, out dec))
                            prop.SetValue(obj, dec, null);
                        else
                            return false;
                    }
                    else if (prop.PropertyType == typeof(int)
                            || prop.PropertyType == typeof(int?))
                    {
                        int num;
                        if (int.TryParse(value, out num))
                            prop.SetValue(obj, num, null);
                        else
                            return false;
                    }
                    else if (prop.PropertyType == typeof(DateTime)
                            || prop.PropertyType == typeof(DateTime?))
                    {
                        DateTime date;
                        if (DateTime.TryParse(value, out date))
                            prop.SetValue(obj, date, null);
                        else if (DateTime.TryParseExact(value, string.IsNullOrEmpty(dateFormat)==false?dateFormat:"yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                            prop.SetValue(obj, date, null);
                        else
                            return false;
                    }
                    else if (prop.PropertyType == typeof(TimeSpan)
                            || prop.PropertyType == typeof(TimeSpan?))
                    {
                        bool pass = false;
                        if (value.Length >= 5
                            && value.IndexOf(":") > 0)
                        {
                            string[] time = value.Split(":".ToCharArray());
                            if (time.Length <= 3)
                            {
                                int hh = 0;
                                int mm = 0;
                                int ss = 0;
                                if (int.TryParse(time[0], out hh)
                                    && int.TryParse(time[1], out mm)
                                    && int.TryParse(time.Length >= 3 ? time[2] : "0", out ss))
                                {
                                    if (hh < 24 && mm <= 59 && ss <= 59)
                                    {
                                        TimeSpan date;
                                        if (TimeSpan.TryParse(value, out date))
                                        {
                                            prop.SetValue(obj, date, null);
                                            pass = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (pass == false)
                            return false;
                    }
                    else if (prop.PropertyType.IsGenericType)
                    {
                        if (prop.PropertyType.GetGenericArguments()[0].Namespace.StartsWith("SECOM_AJIS") == false)
                            throw new Exception("Not implement convert method");
                    }
                    else if (prop.PropertyType.Namespace.StartsWith("SECOM_AJIS") == true)
                    {
                        //do not thing.
                        return true;
                    }
                    else
                        prop.SetValue(obj, value, null);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Convert list data to datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objDoList"></param>
        /// <returns></returns>
        public static DataTable ConvertDoListToDataTable<T>(List<T> objDoList)
        {

            DataTable dtOut = new DataTable();

            if (objDoList != null && objDoList.Count >= 0)
            {
                T objSource = System.Activator.CreateInstance<T>();
                if (objDoList.Count > 0)
                {
                    objSource = objDoList[0];
                }
                //Generate DataTable Column
                PropertyInfo[] pSourceInfo = objSource.GetType().GetProperties();
                foreach (PropertyInfo pInfo in pSourceInfo)
                {
                    string strPropertyType = string.Empty;
                    if (pInfo.PropertyType.FullName == objSource.GetType().ToString())
                    {
                        continue;
                    }

                    if (pInfo.PropertyType.IsGenericType && pInfo.PropertyType.Name.Contains("Nullable"))
                    {
                        Type tNullableType = Type.GetType(pInfo.PropertyType.FullName);
                        strPropertyType = tNullableType.GetGenericArguments()[0].FullName;
                    }
                    else if (!pInfo.PropertyType.IsGenericType)
                    {
                        strPropertyType = pInfo.PropertyType.FullName;
                    }
                    else
                    {
                        continue;
                    }
                    DataColumn col = new DataColumn(pInfo.Name, Type.GetType(strPropertyType));
                    dtOut.Columns.Add(col);
                }

                // Transfer Data from Do list to DataTable
                foreach (T obj in objDoList)
                {
                    DataRow row = dtOut.NewRow();
                    for (int idx = 0; idx < dtOut.Columns.Count; idx++)
                    {
                        PropertyInfo pDestInfo = obj.GetType().GetProperty(dtOut.Columns[idx].ColumnName);
                        Object objVal = pDestInfo.GetValue(obj, null);
                        row[dtOut.Columns[idx].ColumnName] = objVal == null ? DBNull.Value : objVal;
                    }
                    dtOut.Rows.Add(row);
                    dtOut.AcceptChanges();
                }
            }
            return dtOut;
        }
    }
}
