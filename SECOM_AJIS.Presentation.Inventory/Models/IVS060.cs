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
    /// Parameter for screen IVS060.
    /// </summary>
    public class IVS060_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public doOffice office { get; set; }
        [KeepSession]
        public doOffice SrinakarinOfficeCode { get; set; }
        public string SourceOffice { get; set; }
        public string DestinationOffice { get; set; }
        public string Memo { get; set; }
        public List<IVS060INST> lstTransferInstrument { get; set; }
        public string[][] PrevInstTransferArr { get; set; }
        public string slipNo { get; set; }
        public string reportFilePath { get; set; }

    }

    /// <summary>
    /// DO of instrument.
    /// </summary>
    public class IVS060INST : IVS040INST
    {
    }

    public class IVS060TransferGrid
    {
        public string controlID { get; set; }
        public string instrumentCode { get; set; }
        public string areaCode { get; set; }
        public int? transferQTY { get; set; }
    }

    /// <summary>
    /// DO of confirm screen.
    /// </summary>
    public class IVS060TransferDO
    {
        public string memo { get; set; }
        public List<IVS060TransferGrid> transferQTYList { get; set; }
    }

    /// <summary>
    /// Search condition for screen IVS060.
    /// </summary>
    [MetadataType(typeof(IVS060_SearchInstCond_Meta))]
    public class IVS060_SearchInstCond
    {
        public string SourceOffice { get; set; }
        public string DestinationOffice { get; set; }
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string AreaCode { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class IVS060_SearchInstCond_Meta
    {

        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, ControlName = "InstCode")]
        public string InstrumentCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, ControlName = "InstName")]
        public string InstrumentName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, ControlName = "InstArea")]
        public string AreaCode { get; set; }
    }

}