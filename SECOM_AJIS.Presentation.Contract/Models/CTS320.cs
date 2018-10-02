using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of screen
    /// </summary>
    public class CTS320_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string strIncidentRelevantType { get; set; }

        [KeepSession]
        public string strIncidentRelevantCode { get; set; }
    }

    /// <summary>
    /// Grid header result
    /// </summary>
    public class CTS320_GridHeader
    {
        public string Code { get; set; }
        [LanguageMapping]
        public string Name { get; set; }

        public string NameEN { get; set; }
        public string NameLC { get; set; }
        public string NameJP { get; set; }
    }

    /// <summary>
    /// Retrieve code from screen
    /// </summary>
    public class CTS320_GetCodeCondition : ScreenParameter
    {
        public string SiteCode { get; set; }
        public string ContractCode { get; set; }
    }

    /// <summary>
    /// Condition for retrieve incident list
    /// </summary>
    public class CTS320_IncidentCondition : ScreenParameter
    {
        public string IncidentMode { get; set; }
        public string CustomerCode { get; set; }
        public string SiteCode { get; set; }
        public string ContractCode { get; set; }
        public string ProjectCode { get; set; }
        public string IncidentType { get; set; }
        public string DuedateDeadline { get; set; }
        public string IncidentStatus { get; set; }
    }

    /// <summary>
    /// Incident occurring site list result
    /// </summary>
    public class CTS320_IncidentOccurringSiteGridResult
    {
        public string No { get; set; }
        public string SiteCode { get; set; }
        public string SiteNameEN { get; set; }
        public string SiteNameLC { get; set; }
        public string Incident { get; set; }
        public string Delay { get; set; }
    }

    /// <summary>
    /// Incident occurring contract list result
    /// </summary>
    public class CTS320_IncidentOccurringContractGridResult
    {
        public string No { get; set; }
        public string ContractCode { get; set; }
        public string ContractNameEN { get; set; }
        public string ContractNameLC { get; set; }
        public string Incident { get; set; }
        public string Delay { get; set; }
    }

    /// <summary>
    /// Incident list result
    /// </summary>
    public class CTS320_IncidentGridResult
    {
        public string HeadCode { get; set; }
        public string HeadName { get; set; }
        public string IncidentStatus { get; set; }
        public string IncidentFlag { get; set; }
        public string IncidentID { get; set; }
        public string IncidentNo { get; set; }
        public string DueDateDeadline { get; set; }
        public string IncidentType { get; set; }
        public string ControlChief { get; set; }
        public string Correspondent { get; set; }
        public string RegisterDate { get; set; }
        public string CompleteDate { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// Container for return result data to screen
    /// </summary>
    public class CTS320_ViewResult
    {
        public bool IsValid { get; set; }
        public string IncidentMode { get; set; }
        public string strHeadCode { get; set; }
        public string strHeadName { get; set; }
        public string xmlIncidentGrid { get; set; }
        public string xmlOccurringGrid { get; set; }
    }

    /// <summary>
    /// Default value for screen
    /// </summary>
    public class CTS320_DefaultValue
    {
        public string DueDate { get; set; }
        public string Status { get; set; }
    }
}