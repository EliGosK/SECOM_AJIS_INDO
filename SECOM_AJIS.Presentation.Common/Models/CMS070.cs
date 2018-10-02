using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Common.Models.MetaData;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Common.Models
{

    /// <summary>
    /// Parameter for search customer by code
    /// </summary>
    [MetadataType(typeof(CMS070_CustomerByCode_MetaData))]
    public class CMS070_CustomerByCode : doSearchInfoCondition
    {
    }

    /// <summary>
    /// Parameter for search contract by code
    /// </summary>
    [MetadataType(typeof(CMS070_ContractByCode_MetaData))]
    public class CMS070_ContractByCode : doSearchInfoCondition
    {
    }

    /// <summary>
    /// Parameter for screen CMS070
    /// </summary>
    public class CMS070_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public bool hasPermission_CMS080 { get; set; }
        [KeepSession]
        public bool hasPermission_CMS190 { get; set; }
        [KeepSession]
        public bool hasPermission_CMS280 { get; set; }

        public string radioDefault { get; set; }
    }
}

namespace SECOM_AJIS.Presentation.Common.Models.MetaData
{
    public class CMS070_CustomerByCode_MetaData
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string CustomerCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string GroupCode { get; set; }
    }

    public class CMS070_ContractByCode_MetaData
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string CustomerCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string GroupCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string SiteCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ContractCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string UserCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string PlanCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ProjectCode { get; set; }
    }
}
