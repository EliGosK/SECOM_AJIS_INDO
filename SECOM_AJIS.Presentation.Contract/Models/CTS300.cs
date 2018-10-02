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
    public class CTS300_ScreenParameter : ScreenParameter
    {
        public string strIncidentRelevantType { get; set; }
        public string strIncidentRelevantCode { get; set; }
        public bool blnIncidentIsHeadOfficeFlag { get; set; } //Add by jutarat A. on 20082012
        public string MailTo { get; set; } //Add by jutarat A. on 07092012
    }

    /// <summary>
    /// Search result from retrieve data
    /// </summary>
    public class CTS300_ReturnData
    {
        public bool HasPermissionInCharge { get; set; }
        public object SearchResult { get; set; }
    }

    /// <summary>
    /// Customer data for display with nationality and business type
    /// </summary>
    public class CTS300_CustomerData : dtCustomerData
    {
        public string Nationality { get; set; }
        public string BusinessTypeName { get; set; }
    }

    /// <summary>
    /// Site data for display with usage
    /// </summary>
    public class CTS300_SiteData : dtSiteData
    {
        public string Usage { get; set; }
    }

    /// <summary>
    /// Contract data for display
    /// </summary>
    public class CTS300_ContractData
    {
        public string ContractName { get; set; }
        public string SiteName { get; set; }
        public string UserCode { get; set; }
        public string ContractCode { get; set; }
        public string ContractFrom { get; set; }
    }

    /// <summary>
    /// Attach file data
    /// </summary>
    public class CTS300_AttachFile
    {
        public string FileName { get; set; }
        public string FileID { get; set; }
    }

    /// <summary>
    /// Person in charge data for display in grid
    /// </summary>
    public class CTS300_PersonIncharge
    {
        public string OfficeCode { get; set; }
        public string DepartmentCode { get; set; }
        public string IncidentRoleCode { get; set; }
        public string EmpNo { get; set; }
    }

    /// <summary>
    /// Register new incident result
    /// </summary>
    public class CTS300_RegisResult
    {
        public bool IsCompleted { get; set; }
        public MessageModel ErrorMessage { get; set; }
        public string IncidentID { get; set; }
        public string IncidentNo { get; set; }
        public string RegisStatus { get; set; }
    }

    /// <summary>
    /// Recieved detail and reason type for display
    /// </summary>
    public class CTS300_PatternAndReason
    {
        public string RecievedDetail { get; set; }
        public string ReasonType { get; set; }
        public bool HaveReasonType { get; set; }
    }

    /// <summary>
    /// New incident object from screen
    /// </summary>
    public class CTS300_RegisData
    {
        public string RelevantType { get; set; }
        public string RelevantCode { get; set; }
        public List<CTS300_ContractData> ContractRelevant { get; set; } 
        public string CustomerRelateType { get; set; }
        public string SiteRelateType { get; set; }

        [NotNullOrEmpty(ControlName = "ReceivedDate", Screen = "CTS300", Controller = "Contract", Parameter = "lblReceivedDate")]
        public DateTime? ReceivedDate { get; set; }

        [NotNullOrEmpty(ControlName = "ReceivedTime", Screen = "CTS300", Controller = "Contract", Parameter = "lblReceivedTime")]
        public TimeSpan? ReceivedTime { get; set; }

        [NotNullOrEmpty(ControlName = "", Screen = "CTS300", Controller = "Contract", Parameter = "lblIncidentRelevantType")]
        public string ReceivedMethod { get; set; }

        [NotNullOrEmpty(ControlName = "ContractName", Screen = "CTS300", Controller = "Contract", Parameter = "lblContractName")]
        public string ContactName { get; set; }

        public string Department { get; set; }

        [NotNullOrEmpty(ControlName = "IncidentTitle", Screen = "CTS300", Controller = "Contract", Parameter = "lblIncidentTitle")]
        public string IncidentTitle { get; set; }

        [NotNullOrEmpty(ControlName = "IncidentTypeCode", Screen = "CTS300", Controller = "Contract", Parameter = "lblIncidentType")]
        public string IncidentType { get; set; }

        //[NotNullOrEmpty(ControlName = "ReasonTypeCode", Module = "Common"/*, Parameter = CommonUtil.GetLabelFromResource("Contract", "CTS300", "lblReasonType")*/)]
        public string ReasonType { get; set; }

        public bool IsSpecialInfo { get; set; }
        public bool IsImportance { get; set; }

        [NotNullOrEmpty(ControlName = "RecivedDetail", Screen = "CTS300", Controller = "Contract", Parameter = "lblRecivedDetail")]
        public string ReceivedDetail { get; set; }

        public string DueDateDeadLineType { get; set; }
        //[NotNullOrEmpty(ControlName = "DueDate", Module = "Common"/*, Parameter = CommonUtil.GetLabelFromResource("Contract", "CTS300", "rdoDueDate")*/)]
        public DateTime? DueDate_Date { get; set; }
        //[NotNullOrEmpty(ControlName = "ReceivedDueTime", Module = "Common"/*, Parameter = CommonUtil.GetLabelFromResource("Contract", "CTS300", "rdoDueDate")*/)]
        public TimeSpan? DueDate_Time { get; set; }
        //[NotNullOrEmpty(ControlName = "DeadLine", Module = "Common"/*, Parameter = CommonUtil.GetLabelFromResource("Contract", "CTS300", "rdoDeadLine")*/)]
        public DateTime? Deadline_Date { get; set; }
        //[NotNullOrEmpty(ControlName = "DeadLineTimeType", Module = "Common"/*, Parameter = CommonUtil.GetLabelFromResource("Contract", "CTS300", "rdoDeadLine")*/)]
        public string Deadline_Until { get; set; }

        public List<CTS300_PersonIncharge> InChargeList { get; set; }
    }
}
