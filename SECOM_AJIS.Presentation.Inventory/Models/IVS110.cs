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

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter of IVS110 screen
    /// </summary>
    public class IVS110_ScreenParameter : ScreenParameter
    {
        public string SlipNo { get; set; }
        [KeepSession]
        public doOffice office { get; set; }    
        public bool IsError { get; set; }
        public List<IVS110INST> ElemInstrument { get; set; }
    }

    /// <summary>
    /// DO of Instrument data
    /// </summary>
    public class IVS110INST
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string DestinationAreaCode { get; set; }
        public string DestinationShelfNo { get; set; }
        public Nullable<int> TransferQty { get; set; }
        public string row_id { get; set; }
        public bool IsError { get; set; }

    }

    /// <summary>
    /// DO of Register data
    /// </summary>
    public class IVS110ConfirmCond
    {
        public string SlipNo { get; set; }
        public List<IVS110INST> StockInInstrument { get; set; }
    }

    /// <summary>
    /// DO of Search condition
    /// </summary>
    [MetadataType(typeof(IVS110SearchCond_Meta))]
    public class IVS110SearchCond
    {
        public string SlipNo { get; set; }

    }
}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class IVS110SearchCond_Meta
    {
        [NotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, ControlName = "SlipNo")]
        public string SlipNo { get; set; }
    }
}