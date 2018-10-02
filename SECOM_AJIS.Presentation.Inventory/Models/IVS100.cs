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

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter of IVS100 screen
    /// </summary>
    public class IVS100_ScreenParameter : ScreenParameter
    {
        public string Location { get; set; }
        public string ApproveNo { get; set; }
        public string Memo { get; set; }
        public string RepairSubContractor { get; set; }
        [KeepSession]
        public doOffice office { get; set; }

        [KeepSession]
        public string slipNoReportPath { set; get; }
        public string slipNo { set; get; }
        public List<IVS100INST> ElemInstrument { get; set; }
    }

    /// <summary>
    /// DO of Instrument data
    /// </summary>
    public class IVS100INST
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string AreaCode { get; set; }
        public string AreaCodeName { get; set; }
        public string  ShelfNo { get; set; }
        public int InstrumentQty { get; set; }
        public int StockOutQty { get; set; }
        public string StockOutQty_view { get { return CommonUtil.TextNumeric(this.StockOutQty); } }      
        public string row_id { get; set; }
        public string StockOutQty_id { get; set; }
        public bool IsError { get; set; }

    }

    /// <summary>
    /// DO of Register data
    /// </summary>
    [MetadataType(typeof(IVS100RegisterCond_Meta))]
    public class IVS100RegisterCond
    {
        public string ApproveNo { get; set; }
        public string Memo { get; set; }
        public string RepairSubContractor { get; set; }
        public string SourceLoc { get; set; }
        public List<IVS100INST> StockInInstrument { get; set; }
    }

    /// <summary>
    /// DO of Search condition
    /// </summary>
    [MetadataType(typeof(IVS100SearchCond_Meta))]
    public class IVS100SearchCond
    {
        public string SourceLoc { get; set; }
        public string InstName { get; set; }
        public string InstCode { get; set; }
        public string InstArea { get; set; }

    }
}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class IVS100RegisterCond_Meta
    {[NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                Screen = "IVS100",
                Parameter = "lblApproveNo", 
                ControlName = "ApproveNo")]
        public string ApproveNo { get; set; }
        
    }

    public class IVS100SearchCond_Meta
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string InstCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string InstName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string InstArea { get; set; }
    }

}