using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Data;
using System.IO;
using System.Diagnostics;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.Common.Util
{
    #region Generate Model from CSV File (by Attribute)

    /// <summary>
    /// CSV management
    /// </summary>
    public partial class CSVImportUtil
    {
        /// <summary>
        /// Generate model object from csv file by using ImportCsvModelMapping.xml
        /// List<CsvParseSetPropertyError> setPropertyError = new List<CsvParseSetPropertyError>();
        /// List<tbt_tmpImportContent> tmpImportContent = CSVReportUtil2.GenerateModelofCsvData<tbt_tmpImportContent>(session.CsvData, out setPropertyError);
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="csvData">Input: csv data, seperated by each line</param>
        /// <param name="setPropertyError">Output: Unable set property (Error)</param>
        /// <returns>List of model object</returns>
        public static List<T> GenerateModelofCsvData<T>(List<string> csvData, out List<CsvParseSetPropertyError> setPropertyError,string dateFormat = null) where T : class
        {
            List<T> resultModels = new List<T>();
            Type type = typeof(T);
            bool canSetValue = false;
            setPropertyError = new List<CsvParseSetPropertyError>();

            var csvQuery = from line in csvData
                           let data = line.CsvSplit()
                           select data;

            if (csvQuery.Count() > 1)
            {
                string propertyName = "";
                string value = "";
                PropertyInfo[] props = type.GetProperties();
                PropertyInfo prop = null;
                string[] csvColName = csvQuery.ElementAt(0);       //first line in csv file
                int xmlModelIndex = 0;
                bool isMatchModelName = false;

                //Get Xml template file
                XmlDocument doc = new XmlDocument();
                string filePath = CommonUtil.WebPath + SECOM_AJIS.Common.Util.ConstantValue.CommonValue.IMPORT_CSV_MODEL_MAPPING_FILE;
                doc.Load(filePath);
                XmlNodeList nodes = doc.SelectNodes("models/model");

                #region Validate
                //Check Model name
                for (int m = 0; m < nodes.Count; m++)
                {
                    if (nodes[m].Attributes["name"].Value == typeof(T).Name)
                    {
                        xmlModelIndex = m;
                        isMatchModelName = true;
                        break;
                    }
                }
                if (!isMatchModelName)
                    throw new CsvParseException("Was not found model name in xml config file.");

                //Check no. of column
                int csvColumnCount = csvColName.Count();
                int xmlColumnCount = nodes[xmlModelIndex].ChildNodes.Count;
                if (csvColumnCount != xmlColumnCount)
                    throw new CsvParseException("No. of column in csv file isn't equal no. of column in xml config file.");
                

                //Check column name 
                for (int c = 0; c < xmlColumnCount; c++)
                {
                    string xmlColName = nodes[xmlModelIndex].ChildNodes[c].Attributes["name"].Value;
                    if (xmlColName == null)
                        throw new CsvParseException("Column name in xml config file is empty.");

                    prop = props.Where(p => p.Name.Equals(xmlColName)).FirstOrDefault();
                    if (prop == null)
                        throw new CsvParseException("Column name in xml config file is invalid.");
                }
                #endregion

                #region Set value
                for (int l = 1; l < csvQuery.Count(); l++)     //Start at line 2
                {
                    T item = Activator.CreateInstance<T>();
                    for (int c = 0; c < xmlColumnCount; c++)
                    {
                        propertyName = nodes[xmlModelIndex].ChildNodes[c].Attributes["name"].Value;
                        value = csvQuery.ElementAt(l)[c];
                        prop = props.Where(p => p.Name.Equals(propertyName)).First();

                        canSetValue = CommonUtil.SetObjectValue(item, propertyName, value != string.Empty ? value : null, dateFormat);
                        if (canSetValue == false)
                        {
                            setPropertyError.Add(new CsvParseSetPropertyError()
                            {
                                Line = l,
                                Column = c + 1,
                                CsvColumnName = csvQuery.ElementAt(0)[c],
                                Value = value,
                                PropertyName = propertyName,
                                PropertyType = prop.GetType()
                            });
                        }
                    }
                    resultModels.Add(item);
                }
                #endregion
            }
            return resultModels;
        }
    }
    /// <summary>
    /// Exception of CSV convertor
    /// </summary>
    public class CsvParseException : Exception
    {
        public CsvParseException(string message) : base(message) { }
    }
    /// <summary>
    /// Exception of CSV convertor
    /// </summary>
    public class CsvParseSetPropertyError
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string CsvColumnName { get; set; }
        public string Value { get; set; }
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
    }
    /// <summary>
    /// Exception of CSV convertor
    /// </summary>
    public static class CsvParseExtensions
    {
        private enum CsvParseState
        {
            AtBeginningOfToken,
            InNonQuotedToken,
            InQuotedToken,
            ExpectingComma,
            InEscapedCharacter
        };

        public static string[] CsvSplit(this String source)
        {
            List<string> splitString = new List<string>();
            List<int> slashesToRemove = null;
            CsvParseState state = CsvParseState.AtBeginningOfToken;
            char[] sourceCharArray = source.ToCharArray();
            int tokenStart = 0;
            int len = sourceCharArray.Length;
            for (int i = 0; i < len; ++i)
            {
                switch (state)
                {
                    case CsvParseState.AtBeginningOfToken:
                        if (sourceCharArray[i] == '"')
                        {
                            state = CsvParseState.InQuotedToken;
                            slashesToRemove = new List<int>();
                            continue;
                        }
                        if (sourceCharArray[i] == ',')
                        {
                            splitString.Add("");
                            tokenStart = i + 1;
                            continue;
                        }
                        state = CsvParseState.InNonQuotedToken;
                        continue;
                    case CsvParseState.InNonQuotedToken:
                        if (sourceCharArray[i] == ',')
                        {
                            splitString.Add(
                                source.Substring(tokenStart, i - tokenStart));
                            state = CsvParseState.AtBeginningOfToken;
                            tokenStart = i + 1;
                        }
                        continue;
                    case CsvParseState.InQuotedToken:
                        if (sourceCharArray[i] == '"')
                        {
                            state = CsvParseState.ExpectingComma;
                            continue;
                        }
                        if (sourceCharArray[i] == '\\')
                        {
                            state = CsvParseState.InEscapedCharacter;
                            slashesToRemove.Add(i - tokenStart);
                            continue;
                        }
                        continue;
                    case CsvParseState.ExpectingComma:
                        if (sourceCharArray[i] != ',')
                            throw new CsvParseException("Expecting comma");
                        string stringWithSlashes =
                            source.Substring(tokenStart, i - tokenStart);
                        foreach (int item in slashesToRemove.Reverse<int>())
                            stringWithSlashes =
                                stringWithSlashes.Remove(item, 1);
                        splitString.Add(
                            stringWithSlashes.Substring(1,
                                stringWithSlashes.Length - 2));
                        state = CsvParseState.AtBeginningOfToken;
                        tokenStart = i + 1;
                        continue;
                    case CsvParseState.InEscapedCharacter:
                        state = CsvParseState.InQuotedToken;
                        continue;
                }
            }
            switch (state)
            {
                case CsvParseState.AtBeginningOfToken:
                    splitString.Add("");
                    return splitString.ToArray();
                case CsvParseState.InNonQuotedToken:
                    splitString.Add(
                        source.Substring(tokenStart,
                            source.Length - tokenStart));
                    return splitString.ToArray();
                case CsvParseState.InQuotedToken:
                    throw new CsvParseException("Expecting ending quote");
                case CsvParseState.ExpectingComma:
                    string stringWithSlashes =
                        source.Substring(tokenStart, source.Length - tokenStart);
                    foreach (int item in slashesToRemove.Reverse<int>())
                        stringWithSlashes = stringWithSlashes.Remove(item, 1);
                    splitString.Add(
                        stringWithSlashes.Substring(1,
                            stringWithSlashes.Length - 2));
                    return splitString.ToArray();
                case CsvParseState.InEscapedCharacter:
                    throw new CsvParseException("Expecting escaped character");
            }
            throw new CsvParseException("Unexpected error");
        }
    }
    
    #endregion
}
