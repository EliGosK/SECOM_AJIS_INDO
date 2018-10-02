using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of screen
    /// </summary>
    public class CTS330_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string strIncidentID { get; set; }
        public CTS330_SessionData sessObj { get; set; }
        public Dictionary<int, string> newAttachLst { get; set; }
        public Dictionary<int, string> delAttachLst { get; set; }
        public bool blnIncidentIsHeadOfficeFlag { get; set; } //Add by jutarat A. on 20082012
    }

    /// <summary>
    /// Return type of combobox item
    /// </summary>
    public class CTS330_ComboboxData
    {
        public string CBBMarkup { get; set; }
        public bool IsEnablePIC { get; set; }
    }

    /// <summary>
    /// Container for incident data and permission in memory
    /// </summary>
    public class CTS330_SessionData
    {
        public dsIncidentDetail incidentData { get; set; }
        public bool IsSpecialView { get; set; }
        public bool IsEditable { get; set; }
        public doHasIncidentPermission incidentPermit { get; set; }
    }

    /// <summary>
    /// Person in charge item for display in grid
    /// </summary>
    public class CTS330_PersonIncharge
    {
        public string OfficeCode { get; set; }
        public string OfficeName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string IncidentRoleCode { get; set; }
        public string IncidentRoleName { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string RemoveBtn { get; set; }
    }

    /// <summary>
    /// History log from screen
    /// </summary>
    public class CTS330_HistoryLog
    {
        public string OfficeCode { get; set; }
        public string DepartmentCode { get; set; }
        public string IncidentRoleCode { get; set; }

        public string EmpNo { get; set; }
        public string Flag { get; set; }
        public string FunctionType
        {
            get
            {
                if (Flag == "1")
                    return Common.Util.ConstantValue.FunctionID.C_FUNC_ID_ADD;
                else if (Flag == "2")
                    return Common.Util.ConstantValue.FunctionID.C_FUNC_ID_EDIT;
                else if (Flag == "3")
                    return Common.Util.ConstantValue.FunctionID.C_FUNC_ID_DEL;
                else
                    return String.Empty;
            }
        }
    }

    /// <summary>
    /// Incident data for edit from screen
    /// </summary>
    public class CTS330_EntryData
    {
        public List<CTS330_PersonIncharge> IncidentRoleList { get; set; }
        public List<CTS330_HistoryLog> HistoryList { get; set; }
        public List<CTS330_HistoryLog> OriginList { get; set; }

        [NotNullOrEmpty(ControlName = "InteractionType", Controller = "Contract", Parameter = "lblInteractionType", Screen = "CTS330")]
        public string InteractionType { get; set; }
        public string StatusAfterUpdate { get; set; }
        public string CurrentRespondingDetail { get; set; }
        public string DueDateDeadLineType { get; set; }
        public DateTime? DueDate_Date { get; set; }
        public TimeSpan? DueDate_Time { get; set; }
        public DateTime? Deadline_Date { get; set; }
        public string Deadline_Until { get; set; }
    }

    //public class CTS330_CustNameData
    //{
    //    public string Code { get; set; }
    //    public string NameEN { get; set; }
    //    public string NameLC { get; set; }
    //    [LanguageMapping]
    //    public string Name { get; set; }
    //}

    /// <summary>
    /// Incident data for display on screen
    /// </summary>
    public class CTS330_DisplayData
    {
        public bool IsViewMode { get; set; }
        public int ContractDataFrom { get; set; }
        public bool CanChangeStatus { get; set; }
        public bool CanViewPIC { get; set; }
        public bool CanModPIC { get; set; }
        //public string[] ExceptedEmp { get; set; }
        public string ExceptedEmp { get; set; }

        public string IncidentRelevantType { get; set; }
        public string IncidentNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerNameEN { get; set; }
        public string CustomerNameLC { get; set; }
        public string SiteCode { get; set; }
        public string SiteNameEN { get; set; }
        public string SiteNameLC { get; set; }
        public string SiteAddressEN { get; set; }
        public string ContractCode { get; set; }
        public string UserCode { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string Registrant { get; set; }

        public string StartDealDate { get; set; }
        public string MonthlyContractFeeCurrencyType { get; set; }
        public string MonthlyContractFee { get; set; }
        public string ContractDurationFrom { get; set; }
        public string ContractDurationTo { get; set; }
        public string FirstOperationDate { get; set; }
        public string ContractDuration { get; set; }
        public string LastChangeType { get; set; }
        public string AutoRenew { get; set; }
        public string LastOperationDate { get; set; }
        public string OldContractCode { get; set; }
        public string SecurityTypeCode { get; set; }
        public string OperationOffice { get; set; }
        public string ProductName { get; set; }

        public string ProcessManagementStatus { get; set; }
        public string ExpectedCompleteInstallationDate { get; set; }
        public string MaintenanceContractCode { get; set; }
        public string CompleteInstallationDate { get; set; }
        public string SaleType { get; set; }
        public string ExpectedCustomerAcceptanceDate { get; set; }
        public string CustomerAcceptanceDate { get; set; }

        public string IncidentTitle { get; set; }
        public string IncidentType { get; set; }
        public string ReasonType { get; set; }
        public bool IsConfidential { get; set; }
        public bool IsImportance { get; set; }
        public string ContractDetail { get; set; }
        public string ReceivedDetails { get; set; }
        public string RespondingProgress_Normal { get; set; }
        public string RespondingProgress_All { get; set; }
        public string InteractionType { get; set; }
        public string StatusAfterUpdate { get; set; }
        public string CurrentRespondingDetail { get; set; }
        public string DueDateDeadLineType { get; set; }
        public string DueDate_Date { get; set; }
        public string DueDate_Time { get; set; }
        public string Deadline_Date { get; set; }
        public string Deadline_Until { get; set; }
        public bool HasChief { get; set; }
    }
}
