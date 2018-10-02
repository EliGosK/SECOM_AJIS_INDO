using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Common.CustomAttribute
{
    /// <summary>
    /// Attribute for mapping CSV data
    /// </summary>
    public class CSVMappingAttribute : Attribute
    {

        /// <summary>
        /// Determine format of output value.
        /// </summary>
        public enum eValueOutputFormat
        {
            /// <summary>
            /// Detect by value type name. 
            /// If value type is string, output same as eValueOutputFormat.Formula.
            /// Otherwise, output as eValueOutputFormat.Text.
            /// </summary>
            Default = 0,
            /// <summary>
            /// Output without Text Qualifier.
            /// If input value conatins any double quote or comma, output will be formatted same as eValueOutputFormat.Text.
            /// </summary>
            Raw,
            /// <summary>
            /// Wrapping with Text Qualifier. eg: "(value)".
            /// </summary>
            Text,
            /// <summary>
            /// Output to format like Excel's Formula.
            /// Warpping with Equal symbol and Text Qualifier. eg: ="(value)".
            /// </summary>
            Formula
        }

        public enum eDefaultValue
        {
            None = 0,
            Zero,
            Dash
        }

        public string Controller { get; set; }
        public string Screen { get; set; }
        public string LabelName { get; set; }
        private int _SequenceNo = int.MaxValue;
        public int SequenceNo
        {
            get
            {
                return _SequenceNo;
            }
            set
            {
                this._SequenceNo = value;
            }
        }
        public eValueOutputFormat ValueOutputFormat { get; set; }

        public eDefaultValue DisplayDefaultValue { get; set; }

        public CSVMappingAttribute()
        {

        }

        public CSVMappingAttribute(string strController, string strScreen, string strLabelName, int intSequenceNo, eValueOutputFormat outputFormat)
        {
            this.Controller = strController;
            this.Screen = strScreen;
            this.LabelName = strLabelName;
            this.SequenceNo = intSequenceNo;
            this.ValueOutputFormat = outputFormat;
        }

        public CSVMappingAttribute(string strController, string strScreen, string strLabelName, int intSequenceNo)
            : this(strController, strScreen, strLabelName, intSequenceNo, eValueOutputFormat.Default)
        {
        }

        public CSVMappingAttribute(string strHeaderName, int intSequenceNo)
        {
            this.HeaderName = strHeaderName;
            this.SequenceNo = intSequenceNo;
        }

        public CSVMappingAttribute(string strHeaderName, int intSequenceNo, eDefaultValue displayDefaultValue = eDefaultValue.None, eValueOutputFormat outputFormat = eValueOutputFormat.Default)
        {
            this.HeaderName = strHeaderName;
            this.SequenceNo = intSequenceNo;
            this.DisplayDefaultValue = displayDefaultValue;
            this.ValueOutputFormat = outputFormat;
        }

        private string _headerName = string.Empty;
        public string HeaderName
        {
            get
            {
                if (String.IsNullOrEmpty(this.LabelName) == false)
                {
                    string strHeader = CommonUtil.GetLabelFromResource(this.Controller, this.Screen, this.LabelName, true); //Add isDefaultLanguage by Jutarat A. on 07092012
                    if (String.IsNullOrEmpty(strHeader) == false)
                    {
                        strHeader = strHeader.Replace("<br/>", "");
                        strHeader = strHeader.Replace("<div>", "");
                        strHeader = strHeader.Replace("</div>", "");
                        strHeader = strHeader.Replace("<div style=\"display:none;\">", "");

                        _headerName = strHeader;
                    }
                }

                return _headerName;
            }

            set
            {
                this._headerName = value;
            }
        }
    }
}
