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
    public class CTS360_ScreenParameter : ScreenParameter
    {
        public string screenMode { get; set; }

        [KeepSession]
        public string AROfficeCode { get; set; }
    }

    /// <summary>
    /// Search condition by criteria
    /// </summary>
    [MetadataType(typeof(SearchARCondition_MetaData))]
    public class SearchARCondition : doSearchARListCondition
    {
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    
    public class SearchARCondition_MetaData
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string QuotationTargetCode { get; set; }
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
        public string RequestNo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ApproveNo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ARTitle { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ARType { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ARStatusHandling { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ARStatusComplete { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string AROfficeCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string SpecfyPeriod { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public DateTime SpecifyPeriodFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public DateTime SpecifyPeriodTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string Requester { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string Approver { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string Auditor { get; set; }
    }
}
