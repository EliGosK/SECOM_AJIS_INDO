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
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter for screen IVS260.
    /// </summary>
    public class IVS260_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public doOffice office { get; set; }
        public List<doPurchaseOrder> lstPurchaseOrder { get; set; }
        public doPurchaseOrder doPurchaseOrderData { get; set; } //Add by Jutarat A. on 11112013
        public List<doPurchaseOrderDetail> lstPOrderDetail { get; set; }
        public List<doPurchaseOrderDetail> lstInstrumentInGrid { get; set; }

        public decimal m_VatTHB { get; set; } //Add by Jutarat A. on 31102013
        public decimal m_WHT { get; set; }
        public doSpecifyPOrder260 doSpecifyPOrder { get; set; } //Add by Jutarat A. on 06112013

        public tbt_DocumentList PreparedDownloadDocument { get; set; }

        [KeepSession]
        public bool ViewOnlyMode { get; set; }
    }

    /// <summary>
    /// Do of Maintain Purchase Order
    /// </summary>
    public class IVS260_MaintainPurchaseOrderData //Add by Jutarat A. on 31102013
    {
        public doPurchaseOrder doPurchaseOrderData { get; set; }
        public doPurchaseOrderDetail doPOrderDetailData { get; set; }
    }

    /// <summary>
    /// DO of instrument for Add in screen IVS260.
    /// </summary>
    [MetadataType(typeof(doInstrument260_Meta))]
    public class doAddInstrument260 //Add by Jutarat A. on 01112013
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public decimal? OriginalUnitPrice { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? OrderQty { get; set; }
        public decimal Amount { get; set; }
        public string Amount_view { get { return Amount.ToString("#,##0.00"); } }

        public doInstrumentData dtNewInstrument { get; set; }
        public List<doPurchaseOrderDetail> InstrumentData { get; set; }

        public string Unit { get; set; }
    }

    /// <summary>
    /// Specify DO of Purchase order.
    /// </summary>
    [MetadataType(typeof(doSpecifyPOrder260_Meta))]
    public class doSpecifyPOrder260 //Add by Jutarat A. on 01112013
    {
        public string PurchaseOrderType { get; set; }
        public string TransportType { get; set; }
        public DateTime? AdjustDueDate { get; set; }
        public string Currency { get; set; }
        public string Memo { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal Vat { get; set; }

        public string TotalAmountDisplay { get { return TotalAmount.ToString("#,##0.00"); } }
        public string VatDisplay { get { return Vat.ToString("#,##0.00"); } }

        public List<doPurchaseOrderDetail> InstrumentData { get; set; }

        public decimal Discount { get; set; }
        public decimal WHT { get; set; }
    }

    /// <summary>
    /// Specify DO of Purchase order.
    /// </summary>
    [MetadataType(typeof(doSpecifyPOrder250_Domestic_Meta))]
    public class doSpecifyPOrder260_Domes : doSpecifyPOrder260 //Add by Jutarat A. on 04112013
    {
    }

}
namespace SECOM_AJIS.Presentation.Inventory.Models.MetaData
{
    public class doInstrument260_Meta //Add by Jutarat A. on 01112013
    {
        [NotNullOrEmpty(ControlName = "InstrumentCode", Parameter = "lblInstrumentCode", Controller = MessageUtil.MODULE_INVENTORY, Screen = "IVS260")]
        public string InstrumentCode { get; set; }

        [NotNullOrEmpty(ControlName = "UnitPrice", Parameter = "lblUnitPrice", Controller = MessageUtil.MODULE_INVENTORY, Screen = "IVS260")]
        public decimal? UnitPrice { get; set; }
        
        [NotNullOrEmpty(ControlName = "OrderQty", Parameter = "lblOrderQty", Controller = MessageUtil.MODULE_INVENTORY, Screen = "IVS260")]
        public int? OrderQty { get; set; }
    }

    public class doSpecifyPOrder260_Meta //Add by Jutarat A. on 01112013
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, Screen = "IVS260", Parameter = "lblPurchaseOrderType", ControlName = "DTPurchaseOrderType")]
        public string PurchaseOrderType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, Screen = "IVS260", Parameter = "lblTransportType", ControlName = "DTTransportType")]
        public string TransportType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, Screen = "IVS260", Parameter = "lblAdjustDueDate", ControlName = "AdjustDueDate")]
        public DateTime? AdjustDueDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, Screen = "IVS260", Parameter = "lblCurrency", ControlName = "Currency")]
        public string Currency { get; set; }
    }

    public class doSpecifyPOrder250_Domestic_Meta //Add by Jutarat A. on 04112013
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, Screen = "IVS260", Parameter = "lblPurchaseOrderType", ControlName = "DTPurchaseOrderType")]
        public string PurchaseOrderType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INVENTORY, Screen = "IVS260", Parameter = "lblAdjustDueDate", ControlName = "AdjustDueDate")]
        public DateTime? AdjustDueDate { get; set; }
    }

}