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
    /// Parameter for screen MAS100.
    /// </summary>
    public class MAS100_ScreenParameter : ScreenParameter
    {

        public List<doInstrumentExpansion> SearchInstrumentExpansion { get; set; }
        public List<doInstrumentExpansion> DelInstrumentExpansion { get; set; }
        public List<doInstrumentExpansion> AddInstrumentExpansion { get; set; }

        [KeepSession]
        public string ParentInstrumentCode { get; set; }
        [KeepSession]
        public string ParentInstrumentName { get; set; }
        [KeepSession]
        public bool HasAddPermission { get; set; }
        [KeepSession]
        public bool HasDeletePermission { get; set; }

        //public void xxx()
        //{
        //    List<doInstrumentExpansion> SearchInstrumentExpansion = new List<doInstrumentExpansion>();
        //    doInstrumentExpansion x = SearchInstrumentExpansion.Find(D => D.InstrumentCode == "1");
        //    SearchInstrumentExpansion.Remove(x);
        //}
        
    }

    //[MetadataType(typeof(MAS060_Search_MetaData))]
    //public class MAS060_Search : doGroup
    //{
    //}

    //[MetadataType(typeof(MAS060_AddEdit_MetaData))]
    //public class MAS060_AddEdit : doGroup
    //{
    //    public string CurrentMode { get; set; }
    //}
}
namespace SECOM_AJIS.Presentation.Master.Models.MetaData
{
    //public class MAS060_Search_MetaData
    //{
    //    [AtLeast1FieldNotNullOrEmpty (MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
    //    public string GroupCode { get; set; }
    //    [AtLeast1FieldNotNullOrEmpty (MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
    //    public string GroupName { get; set; }
    //}

    //public class MAS060_AddEdit_MetaData
    //{
    //    [NotNullOrEmpty (ControlName = "GroupNameEN")]
    //    public string GroupNameEN { get; set; }
    //    [NotNullOrEmpty(ControlName = "GroupNameLC")]
    //    public string GroupNameLC { get; set; }
    //}
}