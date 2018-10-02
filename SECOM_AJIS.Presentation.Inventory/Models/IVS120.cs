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
    /// Parameter of IVS120 screen
    /// </summary>
    public class IVS120_ScreenParameter : ScreenParameter
    {
        public string Location { get; set; }
        public string Memo { get; set; }
        [KeepSession]
        public doOffice office { get; set; }

        [KeepSession]
        public string slipNoReportPath { set; get; }
        public string slipNo { set; get; }
        public List<IVS120INST> ElemInstrument { get; set; }
    }

    /// <summary>
    /// DO of Instrument data
    /// </summary>
    public class IVS120INST
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
    public class IVS120RegisterCond
    {
        public string Memo { get; set; }
        public string SourceLoc { get; set; }
        public List<IVS120INST> StockInInstrument { get; set; }
    }

    /// <summary>
    /// DO of Search condition
    /// </summary>
    [MetadataType(typeof(IVS120SearchCond_Meta))]
    public class IVS120SearchCond
    {
        public string SourceLoc { get; set; }
        public string InstName { get; set; }
        public string InstCode { get; set; }
        public string InstArea { get; set; }

    }
}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class IVS120SearchCond_Meta
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string InstCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string InstName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true)]
        public string InstArea { get; set; }
    }

}