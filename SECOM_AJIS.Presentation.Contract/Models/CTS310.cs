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

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of screen
    /// </summary>
    public class CTS310_ScreenParameter : ScreenParameter
    {
        public string callerID { get; set; }

        [KeepSession]
        public string incidentOfficeCode { get; set; }

        [KeepSession]
        public string screenMode { get { return this.SubObjectID; } }

        [KeepSession]
        public bool isAdmin { get; set; }
    }

    /// <summary>
    /// Search condition by role
    /// </summary>
    public class SearchIncidentListByRoleCondition
    {
        public string strIncidentRole { get; set; }
        public string strIncidentStatus { get; set; }
        public int? intAddDate { get; set; }
    }

    /// <summary>
    /// Search condition by criteria
    /// </summary>
    [MetadataType(typeof(SearchIncidentCondition_MetaData))]
    public class SearchIncidentCondition : doSearchIncidentListCondition
    {
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class SearchIncidentCondition_MetaData
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ContractTargetPurchaserName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ContractCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string UserCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ContractOfficeCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string OperationOfficeCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ContractStatus { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ContractType { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string CustomerName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string CustomerGroupName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string SiteName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ProjectName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string IncidentNo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string IncidentTitle { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string IncidentType { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string IncidentStatusHandling { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string IncidentStatusComplete { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string IncidentOfficeCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string SpecfyPeriod { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public DateTime? SpecifyPeriodFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public DateTime? SpecifyPeriodTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string Registrant { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string Correspondent { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ControlChief { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string Chief { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string Assistant { get; set; }
    }
}
