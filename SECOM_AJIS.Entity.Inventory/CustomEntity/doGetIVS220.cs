using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace SECOM_AJIS.DataEntity.Inventory
{
    /// <summary>
    /// Search criteria for view in IVS220.
    /// </summary>
    public class doGetIVS220
    {
        public List<string> OfficeCode { get; set; }
        public string LocationCode { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string AreaCode { get; set; }
        public string InventorySlipNo { get; set; }
        public string ContractCode { get; set; }
        public string SupplierName { get; set; }
        public string TransferType { get; set; }
    }
}

