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
    /// Parameter of IVS210 screen
    /// </summary>
    public class IVS210_ScreenParameter : ScreenParameter
    {
        public string Location { get; set; }       
        public string InstCode { get; set; }
        public string InstName { get; set; }
        public string InstArea { get; set; }
        public string ShelfNoFrom { get; set; }
        public string ShelfNoTo { get; set; }
        [KeepSession]
        public doOffice Office { get; set; }
        public Stream StreamReport { get; set; }
        public List<IVS210INST> ElemInstrument { get; set; }
    }

    /// <summary>
    /// DO of Instrument data 
    /// </summary>
    public class IVS210INST
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string SourceShelfNo { get; set; }
        public string ShelfNo { get; set; }
        public string ShelfTypeCode { get; set; }
        public int InstrumentQty { get; set; }
        public string DestinationShelfNo { get; set; }
        public int TransferQty { get; set; }
        public string AreaCode { get; set; }
        public string row_id { get; set; }
        public bool IsError { get; set; }
        public string StockOutQty_id { get; set; }
        public string DestShelfNo_id { get; set; }

    }

    /// <summary>
    /// DO of Register data
    /// </summary>
    public class IVS210RegisterCond
    {
        public string Office { get; set; }
        public string Location { get; set; }
        public List<IVS210INST> StockInInstrument { get; set; }
    }

    /// <summary>
    /// DO of Search condition
    /// </summary>
    [MetadataType(typeof(IVS210SearchCond_Meta))]
    public class IVS210SearchCond
    {
        public string Office { get; set; }
        public string Location { get; set; }
        public string InstCode { get; set; }
        public string InstName { get; set; }
        public string InstArea{ get; set; }
        public string ShelfNoFrom { get; set; }
        public string ShelfNoTo { get; set; }

    }
}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{    public class IVS210SearchCond_Meta
    {       
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string InstCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string InstName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string InstArea { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string ShelfNoFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string ShelfNoTo { get; set; }
    }

}
