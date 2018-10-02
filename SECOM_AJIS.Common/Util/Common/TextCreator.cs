using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Data;
using System.Xml;

namespace SECOM_AJIS.Common.Util
{
    public partial class CommonUtil
    {
        /// <summary>
        /// Generate text in format (first name + " " + last name)
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public static string TextFullName(string firstName, string lastName)
        {
            string txt = string.Empty;
            if (IsNullOrEmpty(firstName) == false)
            {
                txt = firstName;
                if (IsNullOrEmpty(lastName) == false)
                    txt += " " + lastName;
            }

            return txt;
        }
        /// <summary>
        /// Generate text in format (prefix + text + suffix)
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string TextWithPrefixSuffix(string txt, string prefix, string suffix)
        {
            string res = string.Empty;
            if (IsNullOrEmpty(txt) == false)
            {
                res = txt;
                if (IsNullOrEmpty(prefix) == false)
                    res = prefix + res;
                if (IsNullOrEmpty(suffix) == false)
                    res = res + suffix;
            }

            return res;
        }
        /// <summary>
        /// Generate text in format (code + ": " + name)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="middle_text"></param>
        /// <returns></returns>
        public static string TextCodeName(string code, string name, string middle_text = ": ")
        {
            string txt = string.Empty;
            if (IsNullOrEmpty(code) == false)
            {
                txt = code;
                if (IsNullOrEmpty(name) == false)
                {
                    if (middle_text != null)
                        txt += middle_text;
                    txt += name;
                }
            }

            return txt;
        }
        /// <summary>
        /// Generate numeric text
        /// </summary>
        /// <param name="objNumber"></param>
        /// <param name="decp"></param>
        /// <returns></returns>
        public static string TextNumeric(object objNumber, int decp = 2, string defaultValue = "")
        {
            string txt = string.Empty;
            if (objNumber != null)
            {
                if (objNumber is decimal? || objNumber is decimal)
                {
                    decimal dec = 0;
                    if (objNumber is decimal?)
                        dec = (objNumber as decimal?).Value;
                    else
                        dec = (decimal)objNumber;

                    txt = dec.ToString("N" + decp.ToString());
                }
                else if (objNumber is int? || objNumber is int)
                {
                    int num = 0;
                    if (objNumber is int?)
                        num = (objNumber as int?).Value;
                    else
                        num = (int)objNumber;

                    txt = num.ToString("N0");
                }
            }
            else
            {
                return defaultValue;
            }

            return txt;
        }
        /// <summary>
        /// Generate date text in format (dd-mmm-yyyy)
        /// </summary>
        /// <param name="objDate"></param>
        /// <returns></returns>
        public static string TextDate(object objDate)
        {
            string txt = string.Empty;
            if (objDate != null)
            {
                if (objDate is DateTime? || objDate is DateTime)
                {
                    DateTime date = DateTime.Now;
                    if (objDate is DateTime?)
                        date = (objDate as DateTime?).Value;
                    else
                        date = (DateTime)objDate;

                    txt = date.ToString("dd-MMM-yyyy");
                }
            }

            return txt;
        }
        /// <summary>
        /// Generate time text in format (hh:mm)
        /// </summary>
        /// <param name="objDate"></param>
        /// <returns></returns>
        public static string TextTime(object objDate)
        {
            string txt = string.Empty;
            if (objDate != null)
            {
                if (objDate is TimeSpan? || objDate is TimeSpan)
                {
                    TimeSpan time = new TimeSpan();
                    if (objDate is TimeSpan?)
                        time = (objDate as TimeSpan?).Value;
                    else
                        time = (TimeSpan)objDate;

                    txt = string.Format("{0:D2}:{1:D2}", time.Hours, time.Minutes);
                }
            }

            return txt;
        }
        /// <summary>
        /// Generate text from list (x1, x2, x3, ...)
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="split_txt"></param>
        /// <returns></returns>
        public static string TextList(string[] lst, string split_txt = null)
        {
            string txt = string.Empty;
            if (lst != null)
            {
                foreach (string s in lst)
                {
                    if (IsNullOrEmpty(s) == false)
                    {
                        if (txt != string.Empty)
                        {
                            if (split_txt == null)
                                txt += ", ";
                            else
                                txt += split_txt;
                        }
                        txt += s;
                    }
                }
            }

            return txt;
        }
        /// <summary>
        /// Convert number to thai format
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public string ToBahtText(decimal amount)
        {
            string bahtTxt, n, bahtTH = "";

            bahtTxt = amount.ToString("####.00");
            string[] num = { "ศูนย์", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า", "สิบ" };
            string[] rank = { "", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน", "ล้าน" };
            string[] temp = bahtTxt.Split('.');
            string intVal = temp[0];
            string decVal = temp[1];

            if (Convert.ToDouble(bahtTxt) == 0)
                bahtTH = "ศูนย์บาทถ้วน";
            else
            {
                for (int i = 0; i < intVal.Length; i++)
                {
                    n = intVal.Substring(i, 1);
                    if (n != "0")
                    {
                        if (((i == (intVal.Length - 1) || ((i == ((intVal.Length % 7)) && intVal.Length > 7)))) && (n == "1") && (intVal.Length > 1))
                            bahtTH += "เอ็ด";
                        else if (((i == (intVal.Length - 2)) || ((i == ((intVal.Length % 7) - 1)) && intVal.Length > 7)) && (n == "2"))
                            bahtTH += "ยี่";
                        else if ((i == (intVal.Length - 2)) && (n == "1"))
                            bahtTH += "";
                        else
                            bahtTH += num[Convert.ToInt32(n)];

                        if ((intVal.Length > 7) && (i < intVal.Length - 7))
                            bahtTH += rank[(((intVal.Length - i) - 1) % 7) + 1];
                        else
                            bahtTH += rank[(intVal.Length - i) - 1];
                    }
                    // Akat K. : fix missing 'ล้าน'
                    if (n == "0" && intVal.Length > 7 && ((intVal.Length - i) - 1) == 6)
                    {
                        bahtTH += rank[(intVal.Length - i) - 1];
                    }
                }
                if (intVal.Length > 0)
                    bahtTH += "บาท";

                if (decVal == "00")
                    bahtTH += "ถ้วน";
                else
                {
                    for (int i = 0; i < decVal.Length; i++)
                    {
                        n = decVal.Substring(i, 1);
                        if (n != "0")
                        {
                            // Akat K. Modify
                            //if ((i == decVal.Length - 1) && (n == "1"))
                            if ((i == decVal.Length - 1) && (n == "1") && (decVal.Substring(0, 1) != "0"))
                                bahtTH += "เอ็ด";
                            else if ((i == (decVal.Length - 2)) && (n == "2"))
                                bahtTH += "ยี่";
                            else if ((i == (decVal.Length - 2)) && (n == "1"))
                                bahtTH += "";
                            else
                                bahtTH += num[Convert.ToInt32(n)];

                            bahtTH += rank[(decVal.Length - i) - 1];
                        }
                    }
                    bahtTH += "สตางค์";
                }
            }
            return bahtTH;
        }
        /// <summary>
        /// Create CSV
        /// </summary>
        /// <param name="lstValues"></param>
        /// <returns></returns>
        public static string CreateCSVString(List<string> lstValues)
        {
            string csv = "";
            int isOk = 0;
            // ,xx,yy,zz,
            foreach (var item in lstValues)
            {
                if (item != null)
                {
                    csv += string.Format(",{0}", item);
                    isOk++;
                }

            }

            if (isOk > 0)
            {
                csv += ",";
            }

            return csv;
        }
        /// <summary>
        /// Create header of in grid in format CSV
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string CsvHeaderGrid(string header)
        {
            string csvHeader = "";
            string xml = CommonUtil.ConvertToXml<object>(null, header);
            XmlDocument rDoc = new XmlDocument();
            rDoc.LoadXml(xml);
            foreach (XmlNode node in rDoc.ChildNodes[0].ChildNodes[0].ChildNodes)
            {
                if (node.Name == "column")
                {
                    if (node.InnerText != "#cspan" && node.InnerText != "#rspan")
                    {
                        //csvHeader += node.InnerText + ",";
                        csvHeader = String.Format("{0}\"{1}\",", csvHeader, node.InnerText);
                    }
                }
            }
            if (csvHeader.Length > 0)
            {
                csvHeader = csvHeader.Substring(0, csvHeader.Length - 1);
                csvHeader = csvHeader.Replace("<br/>", "");
                csvHeader = csvHeader.Replace("<div>", "");
                csvHeader = csvHeader.Replace("</div>", "");
                csvHeader = csvHeader.Replace("<div style=\"display:none;\">", "");
            }
            return csvHeader;
        }
        /// <summary>
        /// Create CSV
        /// </summary>
        /// <param name="arrValues"></param>
        /// <returns></returns>
        public static string CreateCSVString(string[] arrValues)
        {
            string csv = "";
            int isOk = 0;
            // ,xx,yy,zz,

            for (int i = 0; i < arrValues.Length; i++)
            {
                if (String.IsNullOrEmpty(arrValues[i]) == false)
                {
                    csv += string.Format(",{0}", arrValues[i]);
                    isOk++;
                }

            }


            if (isOk > 0)
            {
                csv += ",";
            }

            return csv;
        }
        /// <summary>
        /// Generate text with newline
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string TextLineFormat(params string[] param)
        {
            string txt = string.Empty;
            if (param != null)
            {
                for (int idx = 1; idx <= param.Length; idx++)
                {
                    if (CommonUtil.IsNullOrEmpty(param[idx-1]))
                        param[idx-1] = "-";

                    if (txt != string.Empty)
                        txt += "<br/>";
                    txt += string.Format("({0}) {1}", idx, param[idx-1]);
                }
            }

            return txt;
        }
        /// <summary>
        /// Generate text from list
        /// </summary>
        /// <typeparam name="TVal"></typeparam>
        /// <param name="key"></param>
        /// <param name="mapping"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string TextName<TVal>(string key, Dictionary<string, TVal> mapping, string field) where TVal : class
        {
            string name = null;
            if (key != null && mapping != null)
            {
                if (mapping.ContainsKey(key))
                {
                    TVal val = mapping[key];
                    if (val != null)
                    {
                        PropertyInfo prop = val.GetType().GetProperty(field);
                        if (prop != null)
                        {
                            if (prop.GetValue(val, null) != null)
                                name = prop.GetValue(val, null).ToString();
                        }
                    }
                }
            }

            return name;
        }
        /// <summary>
        /// Generate text from list
        /// </summary>
        /// <typeparam name="TVal"></typeparam>
        /// <param name="key"></param>
        /// <param name="fieldValue"></param>
        /// <param name="miscLst"></param>
        /// <returns></returns>
        public static string TextName<TVal>(string key, string fieldValue, List<TVal> miscLst) where TVal : class
        {
            string name = null;
            if (key != null && miscLst != null)
            {
                PropertyInfo propField = typeof(TVal).GetProperty("FieldName");
                PropertyInfo propValue = typeof(TVal).GetProperty("ValueCode");
                PropertyInfo propDisplay = typeof(TVal).GetProperty("ValueDisplay");
                if (propField != null
                    && propValue != null
                    && propDisplay != null)
                {
                    foreach (TVal misc in miscLst)
                    {
                        object field = propField.GetValue(misc, null);
                        object value = propValue.GetValue(misc, null);
                        if (field != null && value != null)
                        {
                            if (field.ToString() == key
                                && value.ToString() == fieldValue)
                            {
                                object display = propDisplay.GetValue(misc, null);
                                if (display != null)
                                    name = display.ToString();
                                break;
                            }
                        }

                    }
                }
            }

            return name;
        }
        /// <summary>
        /// Generate text from list in format (code + ": " + name)
        /// </summary>
        /// <typeparam name="TVal"></typeparam>
        /// <param name="key"></param>
        /// <param name="mapping"></param>
        /// <param name="field"></param>
        /// <param name="middle_text"></param>
        /// <returns></returns>
        public static string TextCodeName<TVal>(string key, Dictionary<string, TVal> mapping, string field, string middle_text = null)
                                                                                                where TVal : class
        {
            return TextCodeName(key,
                                TextName<TVal>(key, mapping, field),
                                middle_text);
        }
        /// <summary>
        /// Generate text from list in format (x1, x2, x3, ...)
        /// </summary>
        /// <param name="txt_field"></param>
        /// <param name="nullLst"></param>
        /// <returns></returns>
        public static string GetTxtFieldValidateData(string txt_field, string[] nullLst)
        {
            foreach (string f in nullLst)
            {
                if (f != null && f != string.Empty)
                {
                    if (txt_field != string.Empty)
                        txt_field += ", ";
                    txt_field += f;
                }
            }

            return txt_field;
        }
    }
}
