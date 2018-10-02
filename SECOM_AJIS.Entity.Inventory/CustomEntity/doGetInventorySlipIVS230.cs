using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace SECOM_AJIS.DataEntity.Inventory
{
    /// <summary>
    /// Search condition for get inventory slip in screen IVS230.
    /// </summary>
    public class doGetInventorySlipIVS230
    {
        public string InventorySlipno { get; set; }
        public string SlipStatus { get; set; }
        public List<string> OfficeCode { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string EmpNo { get; set; }
        public string ProjectCode { get; set; }
        public string StockOutType { get; set; }

        public string ContractCode { get; set; }
        public string InstrumentCode { get; set; }

        // Akat K. add
        public string cboSearchCreateOffice { get; set; }
    }
}

