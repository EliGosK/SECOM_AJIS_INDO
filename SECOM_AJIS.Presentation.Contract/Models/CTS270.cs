using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using System.IO;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// DO for validate required field
    /// </summary>
    public class CTS270_CheckReqField
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS270",
                        Parameter = "cboCheckupInstruction",
                        ControlName = "CheckupInstructionMonthFrom")]
        public int? CheckupInstructionMonthFrom { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS270",
                        Parameter = "cboCheckupInstruction",
                        ControlName = "CheckupInstructionYearFrom")]
        public int? CheckupInstructionYearFrom { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS270",
                        Parameter = "cboCheckupInstruction",
                        ControlName = "CheckupInstructionMonthTo")]
        public int? CheckupInstructionMonthTo { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS270",
                        Parameter = "cboCheckupInstruction",
                        ControlName = "CheckupInstructionYearTo")]
        public int? CheckupInstructionYearTo { get; set; }
    }
    /// <summary>
    /// DO for download data
    /// </summary>
    public class CTS270_Download
    {
        public String ContractCode { get; set; }
        public String ProductCode { get; set; }
        public DateTime? InstructionDate { get; set; }
        public bool IsMACheckupList { get; set; }
    }
    /// <summary>
    /// DO for CTS270 screen
    /// </summary>
    public class dsCTS270Data
    {
        private doSearchMACheckupCriteria _doSearchCondition;
        private List<dtSearchMACheckupResult> _dtSearchResultList;

        public doSearchMACheckupCriteria doSearchCondition
        {
            get { return this._doSearchCondition; }
            set { this._doSearchCondition = value; }
        }
        public List<dtSearchMACheckupResult> dtSearchResult
        {
            get { return this._dtSearchResultList; }
            set { this._dtSearchResultList = value; }
        }
    }
    /// <summary>
    /// DO for initial screen
    /// </summary>
    public class CTS270_ScreenParameter : ScreenParameter
    {
        public bool isBtnClick { get; set; }
        public int CurrentIndex { get; set; }
        public int CurrentSortColIndex {get;set;}
        public string CurrentSortType { get; set; }

        [KeepSession]
        public bool hasRegisterPermission { get; set; }

        [KeepSession]
        public bool hasDeletePermision { get; set; }

        [KeepSession]
        public bool hasSlipDownloadPermision { get; set; }

        [KeepSession]
        public bool hasListDownloadPermision { get; set; }

        [KeepSession]
        public bool hasDownloadButton { get; set; }

        public string csvRpt { get; set; }
        
        //public Stream pdfRpt { get; set; }
        public byte[] pdfRpt { get; set; }
        
        public dsCTS270Data data { get; set; }
        public List<CTS270_CheckResultItem> TempList { get; set; }
    }
    /// <summary>
    /// DO for check result item data
    /// </summary>
    public class CTS270_CheckResultItem
    {
        public String ContractCode { get; set; }
        public String ProductCode { get; set; }
        public DateTime? InstructionDate { get; set; }

        public bool CheckedFlag { get; set; }
        public string KeyIndex { get; set; }
    }
}
