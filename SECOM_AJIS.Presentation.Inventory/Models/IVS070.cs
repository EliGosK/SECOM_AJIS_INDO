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
    /// Parameter for screen IVS070.
    /// </summary>
    public class IVS070_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public doOffice office { get; set; }

        public string Memo { get; set; }
        public string SlipNo { get; set; }
        public string SourceOffice { get; set; }
        public string DestinationOffice { get; set; }
        public List<doInventorySlipDetail> lstInventoryDetail { get; set; }
        public string[][] PrevInstTransferArr { get; set; }
    }

    /// <summary>
    /// DO for validate slip.
    /// </summary>
    [MetadataType(typeof(IVS070_RetrieveCond_Meta))]
    public class IVS070_RetrieveCond
    {
        public string SlipNo { get; set; }
    }

    public class IVS070INST
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string DestinationAreaCode { get; set; }
        public string DestinationShelfNo { get; set; }
        public string row_id { get; set; }
        public int TransferQty { get; set; }
    }

    /// <summary>
    /// DO for register receive instrument.
    /// </summary>
    public class IVS070RegisterCond
    {        
        public List<IVS070INST> StockInInstrument { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class IVS070_RetrieveCond_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
                   Screen = "IVS070",
                   Parameter = "lblInventorySlipNo",
                   ControlName = "SlipNo")]
        public string SlipNo { get; set; }
    }

}