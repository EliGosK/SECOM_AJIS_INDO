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
    /// Parameter for screen IVS250.
    /// </summary>
    public class IVS250_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public doOffice office { get; set; }
        public List<doInstrument250> lstInstrument { get; set; }
        public doSpecifyPOrder250 SpecifyPOrder250 { get; set; }
        public tbm_Supplier Supplier { get; set; }
        [KeepSession]
        public decimal m_VatTHB { get; set; }
        public decimal m_WHT { get; set; }

        public string slipNo { get; set; }
        public string reportFilePath { get; set; }
    }

    /// <summary>
    /// Search condition for supplier.
    /// </summary>
    [MetadataType(typeof(doSupplierSearch_Meta))]
    public class doSupplierSearch
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
    }

    /// <summary>
    /// DO of instrument in screen IVS250.
    /// </summary>
    [MetadataType(typeof(doInstrument250_Meta))]
    public class doInstrument250
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string Memo { get; set; } //Add by Jutarat A. on 28102013
        public doInstrumentData dtNewInstrument { get; set; }
        public decimal? OriginalUnitPrice { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? OrderQty { get; set; }
        public decimal Amount { get; set; }
        public string Amount_view { get { return Amount.ToString("#,##0.00"); } }
        public string Unit { get; set; }
        public string UnitCtrlID { get; set; }
    }



    [MetadataType(typeof(doSpecifyPOrder250_Meta))]
    public class doSpecifyPOrder250
    {
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }

        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }

        public string PurchaseOrderType { get; set; }
        public string TransportType { get; set; }
        public DateTime? AdjustDueDate { get; set; }
        public string Currency { get; set; }
        public string Memo { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal Vat { get; set; }

        //Add by Jutarat A. on 30072013
        public string TotalAmountDisplay{ get { return TotalAmount.ToString("#,##0.00"); } }
        public string VatDisplay { get { return Vat.ToString("#,##0.00"); } }
        //End Add

        public List<doInstrument250> InstrumentData { get; set; } //Add by Jutarat A. on 28102013

        public decimal Discount { get; set; }
        public decimal WHT { get; set; }
    }

    /// <summary>
    /// DO of Purchase order.
    /// </summary>
    [MetadataType(typeof(doSpecifyPOrder250_Meta_Domestic))]
    public class doSpecifyPOrder250_Domes : doSpecifyPOrder250
    {
    }

    public class doPOrderAmount
    {
        public string Amount { get; set; }
        public decimal? AmountDecimal { get; set; }
        public string TotalAmount { get; set; }
        public string Vat { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class doSupplierSearch_Meta
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true, ControlName = "SupCode")]
        public string SupplierCode { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, UseControl = true, ControlName = "SupName")]
        public string SupplierName { get; set; }
    }

    public class doInstrument250_Meta
    {
        [NotNullOrEmpty(ControlName = "InstrumentCode", Parameter = "lblInstrumentCode", Controller = MessageUtil.MODULE_INVENTORY,
                   Screen = "IVS250")]
        public string InstrumentCode { get; set; }
        [NotNullOrEmpty(ControlName = "UnitPrice", Parameter = "lblUnitPrice", Controller = MessageUtil.MODULE_INVENTORY,
                   Screen = "IVS250")]
        public decimal? UnitPrice { get; set; }
        [NotNullOrEmpty(ControlName = "OrderQty", Parameter = "lblOrderQty", Controller = MessageUtil.MODULE_INVENTORY,
                   Screen = "IVS250")]
        public int? OrderQty { get; set; }
    }


    public class doSpecifyPOrder250_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
            Screen = "IVS250",
            Parameter = "lblPurchaseOrderType",
            ControlName = "PorderType")]
        public string PurchaseOrderType { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
            Screen = "IVS250",
            Parameter = "lblTransportType",
            ControlName = "TransportType")]
        public string TransportType { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
            Screen = "IVS250",
            Parameter = "lblAdjustDueDate",
            ControlName = "AdjustDueDate")]
        public DateTime? AdjustDueDate { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
            Screen = "IVS250",
            Parameter = "lblCurrency",
            ControlName = "Currency")]
        public string Currency { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
       Screen = "IVS250",
       Parameter = "lblSupplierCode",
       ControlName = "SupCode")]
        public string SupplierCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
       Screen = "IVS250",
       Parameter = "lblSupplierName",
       ControlName = "SupName")]
        public string SupplierName { get; set; }

    }

    public class doSpecifyPOrder250_Meta_Domestic
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
            Screen = "IVS250",
            Parameter = "lblPurchaseOrderType",
            ControlName = "PorderType")]
        public string PurchaseOrderType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
            Screen = "IVS250",
            Parameter = "lblAdjustDueDate",
            ControlName = "AdjustDueDate")]
        public DateTime? AdjustDueDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
       Screen = "IVS250",
       Parameter = "lblSupplierCode",
       ControlName = "SupCode")]
        public string SupplierCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY,
        Screen = "IVS250",
       Parameter = "lblSupplierName",
       ControlName = "SupName")]
        public string SupplierName { get; set; }
    }
}