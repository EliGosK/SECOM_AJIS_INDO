using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Presentation.Inventory.Models.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter for screen IVS050.
    /// </summary>
    public class IVS050_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public doOffice office { get; set; }

        public string Location { get; set; }
        public string ApproveNo { get; set; }
        public string Memo { get; set; }
        public List<IVS040INST> ElemInstrument { get; set; }
        public string[][] PrevInstTransferArr { get; set; }
        public string slipNo { get; set; }
        public string reportFilePath { get; set; }
    }

    /// <summary>
    /// Search condition for screen IVS050.
    /// </summary>
    [MetadataType(typeof(IVS050SearchCondition_Meta))]
    public class IVS050SearchCondition
    {
        public string sourceLoc { get; set; }
        public string InstrumentName { get; set; }
        public string InstrumentCode { get; set; }
        public string AreaCode { get; set; }
    }

}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{

    public class IVS050SearchCondition_Meta
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string InstrumentName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string InstrumentCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string AreaCode { get; set; }
    }

}