using System;
using SECOM_AJIS.Common.CustomAttribute;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Common
{
    /// <summary>
    /// DO for store search condition of screen CMS090
    /// </summary>
    [Serializable]
    public class doCMS090_SearchCondition
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string GroupCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string GroupName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        //[NotMoreThan(RelateProperty = "NumberOfCustomerTo", MessageCode = MessageUtil.MessageList.MSG0080, Module = MessageUtil.MODULE_COMMON)]
        public int? NumberOfCustomerFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public int? NumberOfCustomerTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        //[NotMoreThan(RelateProperty = "NumberOfSiteTo", MessageCode = MessageUtil.MessageList.MSG0080, Module = MessageUtil.MODULE_COMMON)]
        public int? NumberOfSiteFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public int? NumberOfSiteTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string OfficeInCharge { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string PersonInCharge { get; set; }
    }
}
