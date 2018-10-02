using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Presentation.Master.Models.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Master.Models
{
    /// <summary>
    /// Parameter for screen MAS060.
    /// </summary>
    public class MAS060_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public List<doGroup> SearchResult { get; set; }
        [KeepSession]
        public bool HasAddPermission { get; set; }
        [KeepSession]
        public bool HasEditPermission { get; set; }
        [KeepSession]
        public bool HasDeletePermission { get; set; }
    }

    /// <summary>
    /// DO for stored information of customer group using as search condition.
    /// </summary>
    [MetadataType(typeof(MAS060_Search_MetaData))]
    public class MAS060_Search : doGroup
    {
    }

    /// <summary>
    /// DO for stored information of customer group using in case Add/Edit.
    /// </summary>
    [MetadataType(typeof(MAS060_AddEdit_MetaData))]
    public class MAS060_AddEdit : doGroup
    {
        public string CurrentMode { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Master.Models.MetaData
{
    public class MAS060_Search_MetaData
    {
        [AtLeast1FieldNotNullOrEmpty (MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string GroupCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty (MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string GroupName { get; set; }
    }

    public class MAS060_AddEdit_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER, 
                        Screen = "MAS060",
                        Parameter = "lblGroupNameEn", 
                        ControlName = "GroupNameEN")]
        public string GroupNameEN { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_MASTER, 
        //                Screen = "MAS060",
        //                Parameter = "lblGroupNameLc", 
        //                ControlName = "GroupNameLC")]
        //public string GroupNameLC { get; set; }
    }
}