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
using System.IO;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter of IVS080 screen
    /// </summary>
    public class IVS080_ScreenParameter : ScreenParameter
    {
        public string Office { get; set; }
        public string Location { get; set; }
        public string SourceArea { get; set; }
        public string DestinationArea { get; set; }
        public string InstCode { get; set; }
        public string InstName { get; set; }
        public string ShelfNoFrom { get; set; }
        public string ShelfNoTo { get; set; }
        public string ApproveNo { get; set; }
        public string Memo { get; set; }
        [KeepSession]
        public string HeadOfficeCode { get; set; }
        [KeepSession]
        public string SrinakarinOfficeCode { get; set; }
        public string slipNoReportPath { set; get; }
        public string slipNo { set; get; }
        public List<IVS080INST> ElemInstrument { get; set; }
        public string ContractCode { get; set; }
        public DateTime? TransferDate { get; set; }
    }

    /// <summary>
    /// DO of Instrument data
    /// </summary>
    public class IVS080INST
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string SourceShelfNo { get; set; }
        public string ShelfNo { get; set; }
        public int InstrumentQty { get; set; }
        public string DestinationShelfNo { get; set; }
        public int TransferQty { get; set; }   
        public string row_id { get; set; }
        public bool IsError { get; set; }
        public string StockOutQty_id { get; set; }
        public string DestShelfNo_id { get; set; }

    }

    /// <summary>
    /// DO of Register data
    /// </summary>
    [MetadataType(typeof(IVS080RegisterCond_Meta))]
    public class IVS080RegisterCond
    {
        public string ApproveNo { get; set; }
        public string Memo { get; set; }
        public string Office { get; set; }
        public string SourceArea { get; set; }
        public string DestinationArea { get; set; }
        public List<IVS080INST> StockInInstrument { get; set; }
        public string ContractCode { get; set; }
        public DateTime? TransferDate { get; set; } 
    }

    /// <summary>
    /// DO of Search condition
    /// </summary>
    [MetadataType(typeof(IVS080SearchCond_Meta))]
    public class IVS080SearchCond
    {
        public string Office { get; set; }
        public string Location { get; set; }
        public string SourceArea { get; set; }
        public string DestinationArea { get; set; }
        public string InstCode { get; set; }
        public string InstName { get; set; }
        public string ShelfNoFrom { get; set; }
        public string ShelfNoTo { get; set; }

    }
}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class IVS080RegisterCond_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                Screen = "IVS080",
                Parameter = "lblApproveNo", 
                ControlName = "ApproveNo")]
        public string ApproveNo { get; set; }
    }

    public class IVS080SearchCond_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                        Screen =  "IVS080",
                        Parameter = "lblOffice",
                        ControlName = "Office")]
        public string Office { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                        Screen = "IVS080",
                        Parameter = "lblSourceArea",
                        ControlName = "SourceArea")]
        public string SourceArea { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                        Screen = "IVS080",
                        Parameter = "lblDestinationArea",
                        ControlName = "DestinationArea")]
        public string DestinationArea { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string InstCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string InstName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string ShelfNoFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string ShelfNoTo { get; set; }
    }

}