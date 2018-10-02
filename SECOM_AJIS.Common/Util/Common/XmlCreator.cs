using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Resources;
using SECOM_AJIS.Common.Models;
using System.Web;
using System.Text.RegularExpressions;
using System.Globalization;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.Common.Util
{
    public partial class CommonUtil
    {
        public enum GRID_EMPTY_TYPE
        {
            VIEW,
            SEARCH,
            INSERT
        }
        /// <summary>
        /// Get data from xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        private static string GetXmlValue<T>(T obj, PropertyInfo prop)
        {
            string val = string.Empty;
            if (prop.GetValue(obj, null) != null)
            {
                if (prop.PropertyType == typeof(DateTime))
                {
                    CultureInfo c = new CultureInfo("en-US");
                    DateTime dt = (DateTime)prop.GetValue(obj, null);
                    val = dt.ToString("yyyy/MM/dd", c);
                }
                else if (prop.PropertyType == typeof(DateTime?))
                {
                    DateTime? dt = (DateTime?)prop.GetValue(obj, null);
                    if (dt.HasValue)
                    {
                        CultureInfo c = new CultureInfo("en-US");
                        val = dt.Value.ToString("yyyy/MM/dd", c);
                    }
                }
                else if (prop.PropertyType == typeof(TimeSpan))
                {
                    TimeSpan dt = (TimeSpan)prop.GetValue(obj, null);
                    val = CommonUtil.TextTime(dt);
                }
                else if (prop.PropertyType == typeof(TimeSpan?))
                {
                    TimeSpan? dt = (TimeSpan?)prop.GetValue(obj, null);
                    if (dt.HasValue)
                        val = CommonUtil.TextTime(dt);
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    bool b = (bool)prop.GetValue(obj, null);
                    if (b)
                        val = "1";
                    else
                        val = "0";
                }
                else if (prop.PropertyType == typeof(bool?))
                {
                    bool? b = (bool?)prop.GetValue(obj, null);
                    if (b.HasValue)
                    {
                        if (b.Value)
                            val = "1";
                        else
                            val = "0";
                    }
                }
                else
                    val = prop.GetValue(obj, null).ToString();

                //val = HttpContext.Current.Server.HtmlEncode(val); //encode special character to html format
                val = HttpUtility.HtmlEncode(val);
                val = val.Replace("&lt;br&gt;", "<br>"); //decode tag <br> back 
                val = val.Replace("&lt;br/&gt;", "<br/>"); //decode tag <br/> back 
                val = val.Replace("&lt;b&gt;", "<b>"); //decode tag <b> back 
                val = val.Replace("&lt;/b&gt;", "</b>"); //decode tag </b> back 
                val = val.Replace("&lt;br /&gt;", "<br />"); //decode tag <br /> back
                val = val.Replace("&amp;quot;", "\\&quot;"); // decode -> \" , for support  JSON.parse()
                val = Regex.Replace(val, " {2,}", x => x.Value.Replace(" ", "&nbsp;")); //replace 2 or more space to &nbsp;
            }
            return val;
        }
        /// <summary>
        /// Convert xml to list
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetNumberFormatList(XmlDocument doc)
        {
            var dictResult = new Dictionary<string, string>();

            foreach (XmlNode nNode in doc.SelectNodes("/response/head/column"))
            {
                if (nNode.Attributes["type"].Value == "ron2" && nNode.Attributes["numberformat"] != null)
                {
                    dictResult.Add(nNode.Attributes["id"].Value, nNode.Attributes["numberformat"].Value);
                }
            }

            return dictResult;
        }
        /// <summary>
        /// Convert list to xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="file"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ConvertToXml<T>(List<T> lst, string file = null, GRID_EMPTY_TYPE type = GRID_EMPTY_TYPE.VIEW) where T: class
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<response></response>");

                #region Load Header

                int totalCol = 0;
                try
                {
                    if (file != null)
                    {
                        string file_code = string.Empty;
                        string[] pt = file.Split("\\".ToCharArray());
                        if (pt.Length > 0)
                        {
                            string txt_p = pt[pt.Length-1];
                            if (txt_p != string.Empty)
                                txt_p = txt_p.Substring(0, 6);

                            if (pt.Length > 1)
                            {
                                for (int i = 0; i <= pt.Length - 2; i++)
                                {
                                    if (file_code != string.Empty)
                                        file_code += "\\";
                                    file_code += pt[i];
                                }
                                if (file_code != string.Empty)
                                    file_code += "\\";
                            }
                            file_code += txt_p;
                        }
                        
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
                        XmlDocument rDoc = new XmlDocument();
                        rDoc.Load(resourcePath);
                        
                        string filePath = string.Format("{0}{1}\\{2}.xml", 
                                                            CommonUtil.WebPath,
                                                            ConstantValue.CommonValue.GRID_TEMPLATE_FOLDER, 
                                                            file);
                        XmlDocument hDoc = new XmlDocument();
                        hDoc.Load(filePath);

                        if (hDoc.ChildNodes.Count >= 2)
                        {
                            doc.ChildNodes[0].InnerXml = hDoc.ChildNodes[1].InnerXml;

                            //--- Get total "column" node, and update column header ---//
                            foreach (XmlNode node in doc.ChildNodes[0].ChildNodes[0].ChildNodes)
                            {
                                if (node.Name == "column")
                                {
                                    if (node.InnerText != "#cspan" && node.InnerText != "#rspan")
                                    {
                                        XmlNode rNode = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", node.InnerText));
                                        if (rNode != null)
                                        {
                                            node.InnerText = rNode.InnerText;
                                        }
                                        else if (node.InnerText.TrimStart(' ').StartsWith("<"))
                                        {
                                            //Do nothing
                                        }
                                        else
                                        {
                                            node.InnerText = "&nbsp;";
                                        }
                                    }

                                    totalCol += 1;
                                }
                                else if (node.Name == "afterInit")
                                {
                                    foreach (XmlNode aNode in node.ChildNodes)
                                    {
                                        if (aNode.Name == "call" && aNode.Attributes["command"] != null)
                                        {
                                            if (aNode.Attributes["command"].Value == "attachHeader")
                                            {
                                                foreach (XmlNode pNode in aNode.ChildNodes)
                                                {
                                                    if (pNode.InnerText != null)
                                                    {
                                                        string[] txts = pNode.InnerText.Split(",".ToCharArray());
                                                        foreach (string txt in txts)
                                                        {
                                                            if (txt != "#cspan" && txt != "#rspan")
                                                            {
                                                                XmlNode rNode = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", txt));
                                                                if (rNode != null)
                                                                    pNode.InnerText = pNode.InnerText.Replace(txt, rNode.InnerText);
                                                                else
                                                                    pNode.InnerText = pNode.InnerText.Replace(txt, "&nbsp;");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }

                #endregion
                #region Initial Grid

                XmlNode rowsNode = doc.CreateNode(XmlNodeType.Element, "rows", "");
                doc.ChildNodes[0].AppendChild(rowsNode);

                bool isRowMoreMaximum = false;
                if (lst != null)
                {
                    if (lst.Count > ConstantValue.CommonValue.MAX_GRID_ROWS
                        && type == GRID_EMPTY_TYPE.SEARCH)
                        isRowMoreMaximum = true;
                }
                
                MessageModel msg = null;
                if (isRowMoreMaximum)
                    msg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0052, ConstantValue.CommonValue.MAX_GRID_ROWS.ToString("N0"));
                else if (type == GRID_EMPTY_TYPE.SEARCH)
                    msg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                else if (type == GRID_EMPTY_TYPE.INSERT)
                    msg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0110);
                else
                    msg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0109);

                rowsNode.InnerXml = string.Format("<userdata name=\"msgdata\">{0}</userdata>", msg.ToJson);

                

                #endregion
                #region Set XML

                if (isRowMoreMaximum == false && lst != null)
                {
                    Type cType = typeof(T);

                    Dictionary<string, FixedGridToolTipAttribute> dicAttrFixedToolTip = CreateAttributeDictionary<FixedGridToolTipAttribute>(typeof(T));
                    Dictionary<string, GridToolTipAttribute> dicAttrToolTip = CreateAttributeDictionary<GridToolTipAttribute>(typeof(T));
                    Dictionary<string, string> dictNumberFormat = GetNumberFormatList(doc);

                    PropertyInfo[] props = cType.GetProperties();
                    foreach (T obj in lst)
                    {
                        XmlNode node = doc.CreateNode(XmlNodeType.Element, cType.Name, "");
                        rowsNode.AppendChild(node);

                        foreach (PropertyInfo prop in props)
                        {
                            XmlNode iNode = doc.CreateNode(XmlNodeType.Element, prop.Name, "");

                            if (dicAttrFixedToolTip.ContainsKey(prop.Name))
                            {
                                XmlAttribute attrTitle = doc.CreateAttribute("title");
                                attrTitle.Value = dicAttrFixedToolTip[prop.Name].ToolTipText;
                                iNode.Attributes.Append(attrTitle);
                            }
                            else if (dicAttrToolTip.ContainsKey(prop.Name))
                            {
                                XmlAttribute attrTitle = doc.CreateAttribute("title");
                                PropertyInfo propToolTip = cType.GetProperty(dicAttrToolTip[prop.Name].PropertyName);
                                if (propToolTip != null)
                                {
                                    attrTitle.Value = CommonUtil.GetXmlValue<T>(obj, propToolTip);
                                    iNode.Attributes.Append(attrTitle);
                                }
                            }

                            if (dictNumberFormat.ContainsKey(prop.Name))
                            {
                                XmlAttribute attrNumberFormat = doc.CreateAttribute("numberformat");
                                attrNumberFormat.Value = dictNumberFormat[prop.Name];
                                iNode.Attributes.Append(attrNumberFormat);
                            }

                            string val = CommonUtil.GetXmlValue<T>(obj, prop);
                            iNode.InnerText = val;
                            
                            node.AppendChild(iNode);
                        }
                    }
                }

                #endregion
                #region Check Data is exist?

                //Add Data not found row.
                //XmlNode nnode = doc.CreateNode(XmlNodeType.Element, "rows", "");
                //doc.ChildNodes[0].InsertBefore(nnode, doc.ChildNodes[0].FirstChild);

                //MessageModel msg = null;
                //if (isRowMoreMaximum)
                //    msg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0052);
                //else if (type == GRID_EMPTY_TYPE.SEARCH)
                //    msg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                //else if (type == GRID_EMPTY_TYPE.INSERT)
                //    msg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0110);
                //else
                //    msg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0109);

                //nnode.InnerXml = string.Format("<userdata name=\"msgdata\">{0}</userdata>",
                //                                    msg.ToJson);
                
                //if (doc.ChildNodes[0].ChildNodes.Count == 0
                //    || isRowMoreMaximum == true
                //    || ((file != null) && doc.ChildNodes[0].ChildNodes.Count == 1))
                //{
                //    //Add Data not found row.
                //    XmlNode node = doc.CreateNode(XmlNodeType.Element, "row", "");
                //    doc.ChildNodes[0].AppendChild(node);

                //    if (totalCol == 0)
                //        totalCol = 1;


                //    MessageModel msg = null;
                //    if (isRowMoreMaximum)
                //        msg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0052);
                //    else if (type == GRID_EMPTY_TYPE.SEARCH)
                //        msg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0001);
                //    else if (type == GRID_EMPTY_TYPE.INSERT)
                //        msg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0110);
                //    else
                //        msg = MessageUtil.GetMessage(MessageUtil.MODULE_COMMON, MessageUtil.MessageList.MSG0109);

                //    node.InnerXml = string.Format("<cell colspan=\"{0}\" rowspan=\"1\" align=\"left\">{1}</cell>",
                //                                        totalCol,
                //                                        msg.ToJson);
                //}
                

                StringWriter sw = new StringWriter();
                XmlTextWriter tx = new XmlTextWriter(sw);
                doc.WriteTo(tx);
                string xml = sw.ToString();

                #endregion

                return xml;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Convert list to xml for use in storeprocedure
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static string ConvertToXml_Store<T>(List<T> lst, params string[] fields) where T : class
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<rows></rows>");

                if (lst != null)
                {
                    Type cType = typeof(T);
                    PropertyInfo[] props = cType.GetProperties();

                    if (fields != null)
                    {
                        if (fields.Length > 0)
                        {
                            List<PropertyInfo> pLst = new List<PropertyInfo>();
                            foreach (string f in fields)
                            {
                                PropertyInfo prop = cType.GetProperty(f);
                                if (prop != null)
                                    pLst.Add(prop);
                            }
                            props = pLst.ToArray();
                        }
                    }

                    foreach (T obj in lst)
                    {
                        XmlNode node = doc.CreateNode(XmlNodeType.Element, "row", "");
                        doc.ChildNodes[0].AppendChild(node);

                        if (props == null)
                            continue;

                        foreach (PropertyInfo prop in props)
                        {
                            XmlNode iNode = doc.CreateNode(XmlNodeType.Element, prop.Name, "");

                            object value = prop.GetValue(obj, null);
                            if (value != null)
                            {
                                string val = string.Empty;
                                if (prop.PropertyType == typeof(bool))
                                {
                                    bool b = (bool)value;
                                    if (b)
                                        val = "1";
                                    else
                                        val = "0";
                                }
                                else if (prop.PropertyType == typeof(bool?))
                                {
                                    bool? b = (bool?)value;
                                    if (b.HasValue)
                                    {
                                        if (b.Value)
                                            val = "1";
                                        else
                                            val = "0";
                                    }
                                }
                                else if (prop.PropertyType == typeof(DateTime))
                                {
                                    DateTime dt = (DateTime)value;
                                    //val = dt.ToString("yyyy/MM/dd HH:mm:ss.fff", Thread.CurrentThread.CurrentUICulture);
                                    CultureInfo c = new CultureInfo("en-US");
                                    val = dt.ToString("yyyy/MM/dd HH:mm:ss.fff", c);
                                }
                                else if (prop.PropertyType == typeof(DateTime?))
                                {
                                    DateTime? dt = (DateTime?)value;
                                    if (dt.HasValue)
                                    {
                                        //val = dt.Value.ToString("yyyy/MM/dd HH:mm:ss.fff", Thread.CurrentThread.CurrentUICulture);
                                        CultureInfo c = new CultureInfo("en-US");
                                        val = dt.Value.ToString("yyyy/MM/dd HH:mm:ss.fff", c);
                                    }
                                }
                                else
                                    val = value.ToString();

                                iNode.InnerText = val;
                            }
                            else
                                iNode.InnerText = string.Empty;

                            node.AppendChild(iNode);
                        }
                    }
                }

                StringWriter sw = new StringWriter();
                XmlTextWriter tx = new XmlTextWriter(sw);
                doc.WriteTo(tx);

                string xml = sw.ToString();
                xml = xml.Replace("'", "''"); //Add by Nutnicha C. for support data with single quote

                return xml;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
