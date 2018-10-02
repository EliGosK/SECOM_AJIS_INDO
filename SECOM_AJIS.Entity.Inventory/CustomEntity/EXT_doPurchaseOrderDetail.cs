using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory.MetaData;

namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(doPurchaseOrderDetail_Meta))]
    public partial class doPurchaseOrderDetail
    {

        public string SupplierName { get; set; }
        public string CurrencyName { get; set; }
        public string PurchaseOrderStatusName { get; set; }
        public string TransportTypeName { get; set; }
        public int NewReceiveQty { get; set; }
        public string InstrumentArea { get; set; }
        public string row_id { get; set; }

        public string RegionName { get; set; }
        public string ModifyOrderQtyID { get; set; }

        public bool IsShowRemove { get; set; } //Add by Jutarat A. on 29102013

        public string UnitCtrlID { get; set; }
    }

}
namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class doPurchaseOrderDetail_Meta
    {
        [LanguageMapping]
        public string SupplierName { get; set; }
        [LanguageMapping]
        public string CurrencyName { get; set; }
        [LanguageMapping]
        public string PurchaseOrderStatusName { get; set; }
        [LanguageMapping]
        public string RegionName { get; set; }
        [LanguageMapping]
        public string TransportTypeName { get; set; }
    }

}