using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Text;

namespace SECOM_AJIS.Controllers
{
    public class SwtCommonController : Controller
    {
        public const string RESULT_FORMAT = "Case: {0}, Expected: {1}, Actual: {2}, Result: {3} <br>";
        public const string RESULT_FORMAT_LIST = "Case: {0}, Result: {1} <br>";
        public const string RESULT_FORMAT_LIST_DATA = "Case: {0}, Name: {1}, Result: {2} <br>";
        public const string RESULT_FORMAT_ERROR = "Case: {0}, Result: {1}, Error: {2} <br>";
        
        protected string CompareResult_String(string expected, string actual)
        {
            return (expected == actual ? "Pass" : "Fail");
        }
        protected string CompareResult_String(string[] expected, string[] actual)
        {
            string result = "Fail";

            if (expected != null && actual != null && expected.Length == actual.Length)
            {
                for (int i = 0; i < expected.Length; i++)
                {
                    result = CompareResult_String(expected[i], actual[i]);
                    if (result == "Fail")
                        break;
                }
            }

            return result;
        }

        protected string CompareResult_int(int expected, int actual)
        {
            return (expected == actual ? "Pass" : "Fail");
        }
        protected string CompareResult_bool(bool? expected, bool? actual)
        {
            return (expected.GetValueOrDefault() == actual.GetValueOrDefault() ? "Pass" : "Fail");
        }
        protected string CompareResult_bool(bool expected, bool actual)
        {
            return (expected == actual ? "Pass" : "Fail");
        }

        protected string CompareResult_Object(Object expected, Object actual, string[] fieldList = null) {
            if (expected == null && actual == null) {
                return "Pass";
            }

            if (fieldList == null) {
                return "Fail";
            }

            foreach (var fieldName in fieldList)
            {
                if (!CompareObject(actual, expected, fieldName))
                {
                    return "Fail";
                }
            }

            return "Pass";
        }
    
        #region QUP060
        protected bool CompareObjectList<T>(List<T> actual, List<T> expect, string fieldName = null)
        {
            if (actual.Count != expect.Count)
                return false;
            else
            {
                for (int i = 0; i < actual.Count; i++)
                {
                    bool result = CompareObject(actual[i], expect[i], fieldName);

                    if (result == false)
                        return false;
                }

                return true;
            }
        }
        protected bool CompareObject<T>(T actual, T expect, string fieldName = null)
        {
            if (actual == null)
            {
                if (expect == null)
                    return true;
                else
                    return false;
            }
            else
            {
                Type cType = typeof(T);
                PropertyInfo[] props = cType.GetProperties();

                foreach (PropertyInfo prop in props)
                {
                    if (fieldName == null || fieldName.IndexOf(prop.Name) >= 0)
                    {
                        bool result = false;
                        var actualVal = prop.GetValue(actual, null);
                        var expectVal = prop.GetValue(expect, null);

                        if (actualVal == null)
                        {
                            if (expectVal != null)
                                return false;
                            else
                                continue;
                        }
                        else
                        {
                            if (prop.PropertyType == typeof(String))
                            {
                                result = actualVal.Equals(expectVal);
                            }
                            else if (prop.PropertyType == typeof(DateTime?) || (prop.PropertyType == typeof(DateTime)))
                            {
                                result = ((DateTime)actualVal == (DateTime)expectVal);
                            }
                            else if (prop.PropertyType == typeof(bool?) || (prop.PropertyType == typeof(bool)))
                            {
                                result = ((bool)actualVal == (bool)expectVal);
                            }
                            else if (prop.PropertyType == typeof(int?) || (prop.PropertyType == typeof(int)))
                            {
                                result = ((int)actualVal == (int)expectVal);
                            }
                            else if (prop.PropertyType == typeof(decimal?) || (prop.PropertyType == typeof(decimal)))
                            {
                                result = ((decimal)actualVal == (decimal)expectVal);
                            }
                            else if (prop.PropertyType == typeof(TimeSpan?) || (prop.PropertyType == typeof(TimeSpan)))
                            {
                                result = ((TimeSpan)actualVal == (TimeSpan)expectVal);
                            }
                            else
                            {
                                result = (actualVal == expectVal);
                            }

                            if (result == false)
                                return false;
                            else
                                continue;
                        }
                    }
                }

                return true;
            }
        }
        #endregion

        protected string SetResult_String(string[] result)
        {
            string strResult = string.Empty;
            StringBuilder sbResultData = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {
                sbResultData.AppendFormat("{0}, ", result[i]);
            }

            if (sbResultData != null && sbResultData.Length > 1)
            {
                sbResultData.Remove(sbResultData.Length - 2, 2);
                strResult = sbResultData.ToString();
                if (strResult.Contains(","))
                {
                    strResult = string.Format("({0})", strResult);
                }
            }

            return strResult;
        }
    }
}