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
    /// Parameter for screen IVS011.
    /// </summary>
    public class IVS011_ScreenParameter : ScreenParameter
    {
        public List<doInventorySlipDetailList> lstInventory { get; set; }
        [KeepSession]
        public doOffice office { get; set; }
    }

    public class IVS011INST
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public int TransferQty { get; set; }
        public string SourceAreaCode { get; set; }
        public string AreaCodeName { get; set; }
        public string row_id { get; set; }
    }

    /// <summary>
    /// List of inventory in cancel slip.
    /// </summary>
    public class IVS011Cancel
    {
        public List<IVS011INST> InstrumentList { get; set; }
    }

}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{



    //    public class doStockInstrumentCond_Meta
    //    {
    //        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, ControlName = "InstCode", Screen = "IVS010", Parameter = "lblInstrumentCode")]
    //        public string InstrumentCode { get; set; }
    //        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, ControlName = "InstrumentQty", Screen = "IVS010", Parameter = "lblStockInInstrumentQty")]
    //        public string InstrumentQty { get; set; }
    //        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, ControlName = "InstrumentArea", Screen = "IVS010", Parameter = "lblInstrumentArea")]
    //        public string InstrumentArea { get; set; }
    //    }
    //    public class doRegStockInSpecial_Meta
    //    {
    //        [NotNullOrEmpty(ControlName = "SpcApproveNo")]
    //        public string ApproveNo { get; set; }
    //    }

    //    public class doRegStockInPurchase_Meta
    //    {
    //        [NotNullOrEmpty(ControlName = "DetSuppDeliveryOrderNo")]
    //        public string SupplierDeliveryOrderNo { get; set; }

    //    }
}