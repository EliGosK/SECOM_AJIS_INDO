using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;

using System.Data;

using System.IO;
using System.Diagnostics;
//using System.Windows.Forms;

using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.Common.Util
{
    // Generate CSV Files by SWAT Team <PLZ Donate if it's work>

    /// <summary>
    /// Example Call CSV Gen Files
    /// 
    /// path : Server Share Path
    /// List<doOBJECT> lstdoOBJECT
    /// 
    /// string path = ReportUtil.GetGeneratedReportPath("test.csv");
    /// 
    /// WriteCSVFile egWriteCSV = new WriteCSVFile();
    /// egWriteCSV.ExportToCSVFile<doOBJECT>(path, lstdoOBJECT);
    /// 
    /// </summary>

    #region Export CSV File (support any Data Object)

    public class WriteCSVFile
    {
        // --========================================--
        // For Low Level CSV Write File fn
        // Set, Get
        private static string l_CSVFilePath = null;

        // Set
        private static bool l_bolGenDateToCSVFile = true;
        private static bool l_bolWebMode = false;

        private static string l_ACT_CSVFilePath = "";
        private static string l_URLCSVFilePath = "";
        // --========================================--
        // For Gen CSV File From DB Object
        private static string l_CommaSeparatedChar = ",";
        private static string l_DateTimeColumnFormat = "ddMMyyyy";
        // Do Not Use Format like "#,##0.00" 
        // if " , " is SeparatedChar
        private static string l_DecimalColumnFormat = "###0.00";

        private static string l_CSVAfterGenFileName = "";
        public string CSVAfterGenFileName
        {
            get { return l_CSVAfterGenFileName; }
        }
        // --========================================--

        // For Popup Screen
        public bool bolCloseDisplayByProgram = false;
        // --========================================--
        // Converet DO to Datatable - DOToDataTable SET
        #region Convert Do to Datatable

        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }
        public DataTable DOToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                tb.Rows.Add(values);
            }
            return tb;
        }
        #endregion
        // Setting Init Value to Object
        #region InitValue
        public class InitCSVFiles
        {
            public static string CSI_CSVFilePath
            {
                get { return l_CSVFilePath; }
                set
                {
                    l_CSVFilePath = value;
                    l_ACT_CSVFilePath = "";
                }
            }
            public static bool CSI_GenDateToCSVFile
            {
                get { return l_bolGenDateToCSVFile; }
                set { l_bolGenDateToCSVFile = value; }
            }
            public static string CSI_ACT_GenCSVFileFullPath
            {
                get { return l_ACT_CSVFilePath; }
            }

        }
        #endregion
        // Execute Command
        #region execute CSV file
        public class CreateCSVFile
        {
            public static void WriteCSVFile(string strMessage)
            {

                string strCSVFilePath = "";
                string[] strTempCSVFilePath = null;
                string strCSVDirPath = "";

                int intLoop = 1;

                // remember Actual CSV File Name 
                // till it's change file by assign 
                // new CSVFilePath location

                //if (l_ACT_CSVFilePath == "")
                //{
                //    if (l_bolGenDateToCSVFile)
                //    {
                //        strCSVFilePath = l_CSVFilePath.Substring(0, l_CSVFilePath.Length - 4) +
                //            "_" +
                //            DateTime.Now.Year.ToString() +
                //            ("0" + DateTime.Now.Month.ToString()).Substring(DateTime.Now.Month.ToString().Length - 1, 2) +
                //            ("0" + DateTime.Now.Day.ToString()).Substring(DateTime.Now.Day.ToString().Length - 1, 2) +
                //            "-" +
                //            ("0" + DateTime.Now.Hour.ToString()).Substring(DateTime.Now.Hour.ToString().Length - 1, 2) +
                //            "-" +
                //            ("0" + DateTime.Now.Minute.ToString()).Substring(DateTime.Now.Minute.ToString().Length - 1, 2) +
                //            "-" +
                //            ("0" + DateTime.Now.Second.ToString()).Substring(DateTime.Now.Second.ToString().Length - 1, 2) +
                //            l_CSVFilePath.Substring(l_CSVFilePath.Length - 4, 4)
                //            ;
                //    }
                //    else
                //    {
                //        strCSVFilePath = l_CSVFilePath;
                //    }
                //    l_ACT_CSVFilePath = strCSVFilePath;
                //}
                //else
                //{
                //    strCSVFilePath = l_ACT_CSVFilePath;
                //}
                l_ACT_CSVFilePath = l_CSVFilePath;
                strCSVFilePath = l_ACT_CSVFilePath;

                // Log Assign Directory Check and Create if Not Found

                //strTempCSVFilePath = strCSVFilePath.Split("\\".ToCharArray());

                //strCSVDirPath = strTempCSVFilePath[0] + "\\";
                //while (intLoop < strTempCSVFilePath.Length)
                //{
                //    if (!Directory.Exists(strCSVDirPath))
                //    {
                //        Directory.CreateDirectory(strCSVDirPath);

                //    }
                //    strCSVDirPath = strCSVDirPath + strTempCSVFilePath[intLoop] + "\\";
                //    intLoop++;
                //}

                TextWriter lWriter = new StreamWriter(strCSVFilePath, true, System.Text.Encoding.GetEncoding("TIS-620"));
                lWriter.WriteLine(strMessage);
                lWriter.Close();
            }

        }
        #endregion
        public bool IfExitDeleteFile(string strCSVPart)
        {
            bool bolExportToCSVFileOperate = true;

            try
            {
                if (File.Exists(strCSVPart))
                {
                    File.Delete(strCSVPart);
                };
            }
            catch (Exception Ex)
            {
                bolExportToCSVFileOperate = false;

            }

            return bolExportToCSVFileOperate;
        }


        //Write Data Object to CSV File
        #region Write Data Object to CSV File
        public bool ExportToCSVFile<T>(string strCSVPart, List<T> items)
        {
            bool bolExportToCSVFileOperate = true;
            int intReportDataSetTableCount = 0;
            int[] intReportDataSetRowCount;
            DataSet ReportDataSet = new DataSet();
            ReportDataSet.Tables.Add(DOToDataTable(items));

            intReportDataSetTableCount = ReportDataSet.Tables.Count;
            if (intReportDataSetTableCount == 0)
            {
                bolExportToCSVFileOperate = false;
            }
            else
            {

                for (int j = 0; j < intReportDataSetTableCount; j++)
                {

                    WriteCSVFile.InitCSVFiles.CSI_GenDateToCSVFile = true;
                    WriteCSVFile.InitCSVFiles.CSI_CSVFilePath = strCSVPart;

                    //WriteCSVFileHeader(ReportDataSet.Tables[j]);
                    WriteCSVFileDetails(ReportDataSet.Tables[j]);
                    // Close by Program
                    bolCloseDisplayByProgram = true;
                    string[] tempCSVAfterGenFileName = WriteCSVFile.InitCSVFiles.CSI_ACT_GenCSVFileFullPath.Split("\\".ToCharArray(), StringSplitOptions.None);
                    l_CSVAfterGenFileName = tempCSVAfterGenFileName[tempCSVAfterGenFileName.Length - 1];

                }

                intReportDataSetRowCount = new int[intReportDataSetTableCount];
            }

            // this for out real file full path to program
            // xx = WriteCSVFile.InitCSVFiles.ACT_GenCSVFileFullPath;
            // End of CSV file
            WriteCSVFile.InitCSVFiles.CSI_CSVFilePath = "";
            return bolExportToCSVFileOperate;

        }

        public bool WriteCSVFileHeader(DataTable dt)
        {
            string strHeader = "";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                strHeader = strHeader + dt.Columns[i].ColumnName;
                if (!(i == dt.Columns.Count - 1))
                {
                    strHeader = strHeader + l_CommaSeparatedChar;
                }
            }
            WriteCSVFile.CreateCSVFile.WriteCSVFile(strHeader);
            return true;
        }

        public bool WriteCSVFileDetails(DataTable dt)
        {

            //Application.DoEvents();
            int iTotalRowCount = dt.Rows.Count;
            string strDisplayDescription = "Generate Total : " + iTotalRowCount + " Rows" + "\r\n";

            for (int j = 0; j < iTotalRowCount; j++)
            {
                string strDetailsTemp = "";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dt.Columns[i].DataType.FullName == "System.DateTime")
                    {
                        if (dt.Rows[j].ItemArray[i] == null || dt.Rows[j].ItemArray[i].Equals(DBNull.Value))
                        {
                            strDetailsTemp = strDetailsTemp + "";
                        }
                        else
                        {
                            // Edit by Narupon W. 9 May 2012
                            //strDetailsTemp = strDetailsTemp + Convert.ToDateTime(dt.Rows[j].ItemArray[i]).ToString(l_DateTimeColumnFormat).Replace(",", "");
                            strDetailsTemp = strDetailsTemp + "\"" + Convert.ToDateTime(dt.Rows[j].ItemArray[i]).ToString(l_DateTimeColumnFormat) + "\"";
                        }
                        if (!(i == dt.Columns.Count - 1))
                        {
                            strDetailsTemp = strDetailsTemp + l_CommaSeparatedChar;
                        }
                    }
                    else if (dt.Columns[i].DataType.FullName == "System.Decimal")
                    {
                        if (dt.Rows[j].ItemArray[i] == null || dt.Rows[j].ItemArray[i].Equals(DBNull.Value))
                        {
                            strDetailsTemp = strDetailsTemp + "";
                        }
                        else
                        {
                            // Edit by Narupon W. 9 May 2012
                            //strDetailsTemp = strDetailsTemp + Convert.ToDecimal(dt.Rows[j].ItemArray[i]).ToString(l_DecimalColumnFormat);
                            strDetailsTemp = strDetailsTemp + "\"" + Convert.ToDecimal(dt.Rows[j].ItemArray[i]).ToString(l_DecimalColumnFormat) + "\"";

                        }
                        if (!(i == dt.Columns.Count - 1))
                        {
                            strDetailsTemp = strDetailsTemp + l_CommaSeparatedChar;
                        }

                    }
                    else
                    {
                        if (dt.Rows[j].ItemArray[i] == null || dt.Rows[j].ItemArray[i].Equals(DBNull.Value))
                        {
                            strDetailsTemp = strDetailsTemp + "";
                        }
                        else
                        {
                            // Edit by Narupon W. 9 May 2012
                            //strDetailsTemp = strDetailsTemp + dt.Rows[j].ItemArray[i].ToString().Replace(",", "").Replace(Environment.NewLine, "");
                            strDetailsTemp = strDetailsTemp + "\"" + dt.Rows[j].ItemArray[i].ToString().Replace(Environment.NewLine, "") + "\"";
                        }
                        if (!(i == dt.Columns.Count - 1))
                        {
                            strDetailsTemp = strDetailsTemp + l_CommaSeparatedChar;
                        }
                    }
                }
                //Application.DoEvents();
                WriteCSVFile.CreateCSVFile.WriteCSVFile(strDetailsTemp);
            }
            return true;
        }
        #endregion

        //Open CSV file by Default Program on Windows OS
        public class OpenCSVFiles
        {
            public static bool OpenCSV()
            {
                try
                {
                    if (l_ACT_CSVFilePath == "")
                    {
                        return false;
                    }
                    else
                    {
                        if (l_bolWebMode)
                        {
                            Process p = new Process();
                            p.StartInfo.FileName = l_URLCSVFilePath;
                            p.StartInfo.Arguments = l_URLCSVFilePath;
                            p.Start();
                        }
                        else
                        {
                            Process p = new Process();
                            p.StartInfo.FileName = l_ACT_CSVFilePath;
                            p.StartInfo.Arguments = l_ACT_CSVFilePath;
                            p.Start();
                        }
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }

    #endregion

    #region Export CSV File (by Attribute)

    public class CSVReportUtil
    {
        public static string GenerateCSVData<T>(List<T> objDataList, bool includeRunningNo = false, bool includeHeader = true) where T : class
        {
            string strCSVResultData = string.Empty;
            const string C_STRING_TYPE_FULLNAME = "System.String";

            try
            {
                string strHeaderGrid = string.Empty;
                StringBuilder sbHeaderData = new StringBuilder();
                StringBuilder sbResultData = new StringBuilder();
                string strNo = CommonUtil.GetLabelFromResource(MessageUtil.MODULE_COMMON, "CommonResources", "headerNo");

                if (objDataList != null)
                {
                    PropertyInfo[] propInfos = typeof(T).GetProperties();
                    if (propInfos != null)
                    {
                        List<PropertyInfo> propList = new List<PropertyInfo>();
                        foreach (PropertyInfo prop in propInfos)
                        {
                            CSVMappingAttribute[] objAttr = prop.GetCustomAttributes(typeof(CSVMappingAttribute), true) as CSVMappingAttribute[];
                            if (objAttr != null && objAttr.Length > 0)
                                propList.Add(prop);
                        }

                        if (propList.Count > 0)
                        {
                            propInfos = propList.ToArray();
                            Array.Sort(propInfos, delegate (PropertyInfo first, PropertyInfo second)
                            {
                                int iResult = 0;

                                CSVMappingAttribute[] objAttr1 = first.GetCustomAttributes(typeof(CSVMappingAttribute), true) as CSVMappingAttribute[];
                                CSVMappingAttribute[] objAttr2 = second.GetCustomAttributes(typeof(CSVMappingAttribute), true) as CSVMappingAttribute[];
                                if ((objAttr1 != null && objAttr1.Length > 0) && (objAttr2 != null && objAttr2.Length > 0))
                                {
                                    if (objAttr1[0] == null && objAttr2[0] != null)
                                    {
                                        iResult = -1;
                                    }
                                    else if (objAttr1[0] == null && objAttr2[0] == null)
                                    {
                                        iResult = 0;
                                    }
                                    else if (objAttr1[0] != null && objAttr2[0] == null)
                                    {
                                        iResult = 1;
                                    }
                                    else
                                    {
                                        iResult = objAttr1[0].SequenceNo.CompareTo(objAttr2[0].SequenceNo);
                                    }
                                }

                                return iResult;
                            });

                            PropertyInfo pFirst = propInfos.First();

                            //Generate Header
                            if (includeHeader == true)
                            {
                                if (includeRunningNo)
                                {
                                    sbHeaderData.Append("\"" + strNo + "\"");
                                }
                                foreach (PropertyInfo prop in propInfos)
                                {
                                    CSVMappingAttribute[] objAttr = prop.GetCustomAttributes(typeof(CSVMappingAttribute), true) as CSVMappingAttribute[];
                                    if (objAttr == null || objAttr.Length <= 0)
                                        continue;

                                    if (includeRunningNo || pFirst != prop)
                                    {
                                        sbHeaderData.Append(",");
                                    }
                                    sbHeaderData.AppendFormat("\"{0}\"", String.IsNullOrEmpty(objAttr[0].HeaderName) ? prop.Name : objAttr[0].HeaderName);
                                }
                            }

                            //Generate Detail
                            int iNo = 0;
                            foreach (T objData in objDataList)
                            {
                                iNo++;

                                if (includeRunningNo)
                                {
                                    sbResultData.Append(iNo);
                                }

                                foreach (PropertyInfo prop in propInfos)
                                {
                                    CSVMappingAttribute[] objAttr = prop.GetCustomAttributes(typeof(CSVMappingAttribute), true) as CSVMappingAttribute[];
                                    if (objAttr == null || objAttr.Length <= 0)
                                        continue;

                                    string strValue = string.Empty;
                                    object o = prop.GetValue(objData, null);
                                    if (o != null)
                                    {
                                        //strValue = o.ToString();
                                        //if (prop.PropertyType.FullName == C_STRING_TYPE_FULLNAME)
                                        //    strValue = String.Format("=\"\"{0}\"\"", strValue.Replace("\"", "\"\""));

                                        CSVMappingAttribute.eValueOutputFormat operationFormat;
                                        if (objAttr[0].ValueOutputFormat == CSVMappingAttribute.eValueOutputFormat.Default)
                                        {
                                            if (prop.PropertyType.FullName == C_STRING_TYPE_FULLNAME)
                                                operationFormat = CSVMappingAttribute.eValueOutputFormat.Formula;
                                            else
                                                operationFormat = CSVMappingAttribute.eValueOutputFormat.Text;
                                        }
                                        else
                                        {
                                            operationFormat = objAttr[0].ValueOutputFormat;
                                        }


                                        strValue = (o == null ? "" : o.ToString());

                                        switch (operationFormat)
                                        {
                                            case CSVMappingAttribute.eValueOutputFormat.Formula:
                                                strValue = String.Format("\"=\"\"{0}\"\"\"", strValue.Replace("\"", "\"\"\"\""));
                                                break;
                                            case CSVMappingAttribute.eValueOutputFormat.Text:
                                            default:
                                                strValue = String.Format("\"{0}\"", strValue.Replace("\"", "\"\""));
                                                break;
                                            case CSVMappingAttribute.eValueOutputFormat.Raw:
                                                if (strValue.Contains(',') || strValue.Contains('"'))
                                                {
                                                    strValue = String.Format("\"{0}\"", strValue.Replace("\"", "\"\""));
                                                }
                                                break;
                                        }
                                    }

                                    //sbResultData.AppendFormat("\"{0}\",", strValue);
                                    if (includeRunningNo || pFirst != prop)
                                    {
                                        sbResultData.Append(",");
                                    }
                                    sbResultData.AppendFormat("{0}", strValue);
                                }

                                sbResultData.AppendLine();
                            }

                            if (sbResultData != null && sbResultData.Length > 0)
                            {
                                string strHeaderData = string.Empty;
                                string strResultData = sbResultData.ToString();

                                if (sbHeaderData != null && sbHeaderData.Length > 0)
                                {
                                    strHeaderData = sbHeaderData.ToString();
                                    //if (includeRunningNo)
                                    //    strHeaderData = String.Format("\"{0}\",{1}", strNo, strHeaderData);
                                }

                                strCSVResultData = String.Format("{0}{1}{2}", strHeaderData, Environment.NewLine, strResultData);
                                strCSVResultData = String.IsNullOrEmpty(strCSVResultData) ? string.Empty : strCSVResultData.Replace("<br/>", "").Replace("<BR/>", "");
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strCSVResultData;
        }

        public static string GenerateAccountingCSVData<T>(List<T> objDataList, bool includeRunningNo = false, bool includeHeader = true, string strNo = null) where T : class
        {
            string strCSVResultData = string.Empty;
            const string C_STRING_TYPE_FULLNAME = "System.String";

            try
            {
                string strHeaderGrid = string.Empty;
                StringBuilder sbHeaderData = new StringBuilder();
                StringBuilder sbResultData = new StringBuilder();


                if (objDataList != null)
                {
                    PropertyInfo[] propInfos = typeof(T).GetProperties();
                    if (propInfos != null)
                    {
                        List<PropertyInfo> propList = new List<PropertyInfo>();
                        foreach (PropertyInfo prop in propInfos)
                        {
                            CSVMappingAttribute[] objAttr = prop.GetCustomAttributes(typeof(CSVMappingAttribute), true) as CSVMappingAttribute[];
                            if (objAttr != null && objAttr.Length > 0)
                                propList.Add(prop);
                        }

                        if (propList.Count > 0)
                        {
                            propInfos = propList.ToArray();
                            Array.Sort(propInfos, delegate (PropertyInfo first, PropertyInfo second)
                            {
                                int iResult = 0;

                                CSVMappingAttribute[] objAttr1 = first.GetCustomAttributes(typeof(CSVMappingAttribute), true) as CSVMappingAttribute[];
                                CSVMappingAttribute[] objAttr2 = second.GetCustomAttributes(typeof(CSVMappingAttribute), true) as CSVMappingAttribute[];
                                if ((objAttr1 != null && objAttr1.Length > 0) && (objAttr2 != null && objAttr2.Length > 0))
                                {
                                    if (objAttr1[0] == null && objAttr2[0] != null)
                                    {
                                        iResult = -1;
                                    }
                                    else if (objAttr1[0] == null && objAttr2[0] == null)
                                    {
                                        iResult = 0;
                                    }
                                    else if (objAttr1[0] != null && objAttr2[0] == null)
                                    {
                                        iResult = 1;
                                    }
                                    else
                                    {
                                        iResult = objAttr1[0].SequenceNo.CompareTo(objAttr2[0].SequenceNo);
                                    }
                                }

                                return iResult;
                            });

                            PropertyInfo pFirst = propInfos.First();

                            //Generate Header
                            if (includeHeader == true)
                            {
                                if (includeRunningNo)
                                {
                                    sbHeaderData.Append("\"" + strNo + "\"");
                                }
                                foreach (PropertyInfo prop in propInfos)
                                {
                                    CSVMappingAttribute[] objAttr = prop.GetCustomAttributes(typeof(CSVMappingAttribute), true) as CSVMappingAttribute[];
                                    if (objAttr == null || objAttr.Length <= 0)
                                        continue;

                                    if (includeRunningNo || pFirst != prop)
                                    {
                                        sbHeaderData.Append(",");
                                    }
                                    sbHeaderData.AppendFormat("\"{0}\"", String.IsNullOrEmpty(objAttr[0].HeaderName) ? prop.Name : objAttr[0].HeaderName);
                                }
                            }

                            //Generate Detail
                            int iNo = 0;
                            foreach (T objData in objDataList)
                            {
                                iNo++;

                                if (includeRunningNo)
                                {
                                    sbResultData.Append(iNo);
                                }

                                foreach (PropertyInfo prop in propInfos)
                                {
                                    CSVMappingAttribute[] objAttr = prop.GetCustomAttributes(typeof(CSVMappingAttribute), true) as CSVMappingAttribute[];
                                    if (objAttr == null || objAttr.Length <= 0)
                                        continue;

                                    string strValue = string.Empty;
                                    object o = prop.GetValue(objData, null);
                                    if (o != null || objAttr[0].DisplayDefaultValue != CSVMappingAttribute.eDefaultValue.None)
                                    {
                                        //strValue = o.ToString();
                                        //if (prop.PropertyType.FullName == C_STRING_TYPE_FULLNAME)
                                        //    strValue = String.Format("=\"\"{0}\"\"", strValue.Replace("\"", "\"\""));

                                        CSVMappingAttribute.eValueOutputFormat operationFormat;
                                        if (objAttr[0].ValueOutputFormat == CSVMappingAttribute.eValueOutputFormat.Default)
                                        {
                                            if (prop.PropertyType.FullName == C_STRING_TYPE_FULLNAME)
                                                operationFormat = CSVMappingAttribute.eValueOutputFormat.Formula;
                                            else
                                                operationFormat = CSVMappingAttribute.eValueOutputFormat.Text;
                                        }
                                        else
                                        {
                                            operationFormat = objAttr[0].ValueOutputFormat;
                                        }

                                        strValue = (o == null ? "" : o.ToString());

                                        if (string.IsNullOrEmpty(strValue) == true)
                                        {
                                            if (objAttr[0].DisplayDefaultValue == CSVMappingAttribute.eDefaultValue.Dash)
                                                strValue = "-";
                                            else if (objAttr[0].DisplayDefaultValue == CSVMappingAttribute.eDefaultValue.Zero)
                                                strValue = "0";
                                        }


                                        switch (operationFormat)
                                        {
                                            case CSVMappingAttribute.eValueOutputFormat.Formula:
                                                strValue = String.Format("\"=\"\"{0}\"\"\"", strValue.Replace("\"", "\"\"\"\""));
                                                break;
                                            case CSVMappingAttribute.eValueOutputFormat.Text:
                                            default:
                                                strValue = String.Format("\"{0}\"", strValue.Replace("\"", "\"\""));
                                                break;
                                            case CSVMappingAttribute.eValueOutputFormat.Raw:
                                                if (strValue.Contains(',') || strValue.Contains('"'))
                                                {
                                                    strValue = String.Format("\"{0}\"", strValue.Replace("\"", "\"\""));
                                                }
                                                break;
                                        }
                                    }

                                    //sbResultData.AppendFormat("\"{0}\",", strValue);
                                    if (includeRunningNo || pFirst != prop)
                                    {
                                        sbResultData.Append(",");
                                    }
                                    sbResultData.AppendFormat("{0}", strValue);
                                }

                                sbResultData.AppendLine();
                            }

                            if (sbResultData != null && sbResultData.Length > 0)
                            {
                                string strHeaderData = string.Empty;
                                string strResultData = sbResultData.ToString();

                                if (sbHeaderData != null && sbHeaderData.Length > 0)
                                {
                                    strHeaderData = sbHeaderData.ToString();
                                    //if (includeRunningNo)
                                    //    strHeaderData = String.Format("\"{0}\",{1}", strNo, strHeaderData);
                                }

                                strCSVResultData = String.Format("{0}{1}{2}", strHeaderData, Environment.NewLine, strResultData);
                                strCSVResultData = String.IsNullOrEmpty(strCSVResultData) ? string.Empty : strCSVResultData.Replace("<br/>", "").Replace("<BR/>", "");
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strCSVResultData;
        }
    }

    #endregion

    #region Export CSV File (Using xaml template)

    // Create by Jirawat Jannet on 2016-12-13
    /// <summary>
    /// Creat Csv steam
    /// </summary>
    public static class CSVCreator
    {
        /// <summary>
        /// Create MemorySteam csv file
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="lst">Datas</param>
        /// <param name="file">Xml file</param>
        /// <param name="delimeter">delimeter (Default is , )</param>
        /// <param name="thereIsHeader">Do you want generate header text ? (Default is True)</param>
        /// <returns></returns>
        public static MemoryStream CreateCSVSteam<T>(List<T> lst, string file = null, string delimeter = ",", bool thereIsHeader = true) where T : class
        {
            try
            {
                if (!string.IsNullOrEmpty(file))
                {
                    // Load header
                    List<CSVCreatorColData> cols = generateCsvColDatas(file);

                    List<List<string>> datas = new List<List<string>>();

                    foreach (var c in cols)
                    {
                        List<string> rows = new List<string>();

                        if (thereIsHeader)
                            rows.Add(c.HeaderText.removeString(",", Environment.NewLine));

                        if (lst != null)
                        {
                            foreach (var l in lst)
                            {
                                rows.Add(convertColVal(GetPropValue(l, c.Id), c.DataType, c.DataFormat, c.Id).removeString(",", Environment.NewLine));
                            }
                        }

                        datas.Add(rows);
                    }

                    if(datas.Count <= 0) throw new Exception("There is no data.");

                    List<List<string>> lines = Transpose(datas);

                    StringBuilder text = new StringBuilder();

                    foreach (var rows in lines)
                    {
                        text.Append(string.Join(delimeter, rows) + Environment.NewLine);
                    }


                    return GenerateStreamFromString(text.ToString());
                }
                else
                {
                    throw new Exception("There is no require parameter \"file\".");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string removeString(this string str, params string[] removeString)
        {
            if (string.IsNullOrEmpty(str)) return str;
            if (removeString != null && removeString.Length > 0)
            {
                foreach (var r in removeString)
                {
                    str = str.Replace(r, string.Empty);
                }
            }

            return str;
        }

        private static List<List<T>> Transpose<T>(List<List<T>> lists)
        {
            var longest = lists.Any() ? lists.Max(l => l.Count) : 0;
            List<List<T>> outer = new List<List<T>>(longest);
            for (int i = 0; i < longest; i++)
                outer.Add(new List<T>(lists.Count));
            for (int j = 0; j < lists.Count; j++)
                for (int i = 0; i < longest; i++)
                    outer[i].Add(lists[j].Count > i ? lists[j][i] : default(T));
            return outer;
        }
        private static MemoryStream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        private static string convertColVal(object obj, string type, string format, string colName)
        {
            try
            {
                if (obj == null) return string.Empty;
                switch (type)
                {
                    case DATA_TYPE.bit:
                        bool b = (bool)obj;
                        return b ? "1" : "0";
                    case DATA_TYPE.date:
                        DateTime dt = (DateTime)obj;
                        return dt.ToString(format);
                    case DATA_TYPE.number:
                        decimal val = decimal.Parse(obj.ToString());
                        return val.ToString(format);
                    case DATA_TYPE.text:
                        return obj.ToString();
                    default:
                        return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Cannot convert data: {0} value: {1} to type: {2} format: {3} because: {4}"
                    , colName, obj.ToString(), type, format, ex.Message));
            }
        }
        private static List<CSVCreatorColData> generateCsvColDatas(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<response></response>");
            List<CSVCreatorColData> cols = new List<CSVCreatorColData>();
            int totalCol = 0;

            // Resource xml document (Text language)
            XmlDocument rDoc = generateResourceXmlDocument(file);
            // Template xml document
            XmlDocument hDoc = generateTemplateXmlDocument(file);

            if (hDoc.ChildNodes.Count >= 2)
            {
                doc.ChildNodes[0].InnerXml = hDoc.ChildNodes[1].InnerXml;

                //--- Get total "column" node, and update column header ---//
                foreach (XmlNode node in doc.ChildNodes[0].ChildNodes[0].ChildNodes)
                {
                    if (node.Name == "column")
                    {
                        string id = node.Attributes["id"].Value;
                        string type = node.Attributes["type"].Value;
                        string format = node.Attributes["format"] == null ? "" : node.Attributes["format"].Value;

                        if (string.IsNullOrEmpty(id)) continue;
                        if (string.IsNullOrEmpty(type)) type = DATA_TYPE.text;
                        if (string.IsNullOrEmpty(format)) format = string.Empty;

                        CSVCreatorColData col = new CSVCreatorColData();
                        col.Seq = totalCol;
                        col.Id = id;
                        col.DataType = type;
                        col.DataFormat = format;

                        XmlNode rNode = rDoc.SelectSingleNode(string.Format("root/data[@name=\"{0}\"]/value", node.InnerText));
                        if (rNode != null)
                        {
                            col.HeaderText = rNode.InnerText;
                        }
                        else if (node.InnerText.TrimStart(' ').StartsWith("<"))
                        {
                            //Do nothing
                        }
                        else
                        {
                            col.HeaderText = " ";
                        }

                        cols.Add(col);

                        totalCol += 1;
                    }
                }
            }

            return cols;
        }

        #region Xml document data generator

        private static XmlDocument generateResourceXmlDocument(string file)
        {
            string file_code = string.Empty;
            string[] pt = file.Split("\\".ToCharArray());
            if (pt.Length > 0)
            {
                string txt_p = pt[pt.Length - 1];
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

            XmlDocument Doc = new XmlDocument();
            Doc.Load(resourcePath);

            return Doc;
        }
        private static XmlDocument generateTemplateXmlDocument(string file)
        {
            string filePath = string.Format("{0}{1}\\{2}.xml",
                                                        CommonUtil.WebPath,
                                                        ConstantValue.CommonValue.GRID_TEMPLATE_FOLDER,
                                                        file);

            XmlDocument Doc = new XmlDocument();
            Doc.Load(filePath);

            return Doc;
        }

        #endregion


        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        #region Private class


        public class CSVCreatorColData
        {
            public int Seq { get; set; }
            public string Id { get; set; }
            public string HeaderText { get; set; }
            public string DataType { get; set; }
            public string DataFormat { get; set; }
        }
        private class DATA_TYPE
        {
            public const string text = "text";
            public const string number = "number";
            public const string date = "date";
            public const string bit = "bit";
        }


        #endregion
        
    }
    #endregion
}
