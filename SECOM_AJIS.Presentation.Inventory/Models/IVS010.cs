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
    /// Parameter for screen IVS010.
    /// </summary>
    public class IVS010_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public doOffice office { get; set; }
        public doRegStockInPurchase RegPurchaseData { get; set; }
        public doRegStockInSpecial RegSpecialData { get; set; }
        public string PurchaseOrderStatus { get; set; }
    }

    /// <summary>
    /// DO for register stock in.
    /// </summary>
    [MetadataType(typeof(doRegStockInPurchase_Meta))]
    public class doRegStockInPurchase
    {
        public string PurchaseOrderNo { get; set; }
        public string ApproveNo { get; set; }
        public string Memo { get; set; }
        public string SupplierDeliveryOrderNo { get; set; }
        public int numKeyEnter { get; set; }
        public DateTime? StockInDate { get; set; }
        public List<StockInIntrument> StockInInstrument { get; set; }
        public string VoucherNo { get; set; }
        public DateTime? VoucherDate { get; set; }
    }
    [MetadataType(typeof(doRegStockInSpecial_Meta))]
    public class doRegStockInSpecial
    {

        public string ApproveNo { get; set; }
        public string Memo { get; set; }
        public string SupplierDeliveryOrderNo { get; set; }
        public int numKeyEnter { get; set; }
        public DateTime? StockInDate { get; set; }
        public List<StockInInstrumentSpecial> StockInstrumentSPC { get; set; }
        public string VoucherNo { get; set; }
        public DateTime? VoucherDate { get; set; }
    }

    /// <summary>
    /// Do for store stock instrument information.
    /// </summary>
    [MetadataType(typeof(doStockInstrumentCond_Meta))]
    public class doStockInstrumentCond
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string InstrumentQty { get; set; }
        public string InstrumentArea { get; set; }

    }
    public class StockInIntrument
    {
        public string InstrumentCode { get; set; }
        public int PurchaseQty { get; set; }
        public int RemainQty { get; set; }
        public string InstrumentArea { get; set; }
        public int NewReceiveQty { get; set; }
        public string NewReceiveQtyID { get; set; }
        public string InstrumentAreaID { get; set; }
        public string row_id { get; set; }
    }
    public class StockInInstrumentSpecial
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public int StockInQty { get; set; }
        public string InstrumentArea { get; set; }
        public string StockInQtyID { get; set; }
        public string row_id { get; set; }
    }

}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class doStockInstrumentCond_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, ControlName = "InstCode", Screen = "IVS010", Parameter = "lblInstrumentCode")]
        public string InstrumentCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, Screen = "IVS010", Parameter = "headerInstrumentName")]
        public string InstrumentName { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, ControlName = "InstrumentQty", Screen = "IVS010", Parameter = "lblStockInInstrumentQty")]
        public string InstrumentQty { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, ControlName = "InstrumentArea", Screen = "IVS010", Parameter = "lblInstrumentArea")]
        public string InstrumentArea { get; set; }
    }
    public class doRegStockInSpecial_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, ControlName = "SpcApproveNo", Screen = "IVS010", Parameter = "lblApproveNo")]
        public string ApproveNo { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, ControlName = "txtSpecialStockInDate", Screen = "IVS010", Parameter = "lblStockInDate")]
        public DateTime? StockInDate { get; set; }
    }

    public class doRegStockInPurchase_Meta
    {
        [NotNullOrEmpty(ControlName = "DetSuppDeliveryOrderNo", Controller = MessageUtil.MODULE_INVENTORY, Screen = "IVS010", Parameter = "lblSupplierDeliveryOrderNo")]
        public string SupplierDeliveryOrderNo { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, ControlName = "txtPOStockInDate", Screen = "IVS010", Parameter = "lblStockInDate")]
        public DateTime? StockInDate { get; set; }

    }
}