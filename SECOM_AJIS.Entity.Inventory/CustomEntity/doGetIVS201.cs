using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace SECOM_AJIS.DataEntity.Inventory
{
    /// <summary>
    /// Search criteria for view in IVS201.
    /// </summary>
    public class doGetIVS201
    {
        public string OfficeCode { get; set; }
        public string LocationCode { get; set; }
        public string AreaCode { get; set; }
        public string ShelfNoFrom { get; set; }
        public string ShelfNoTo { get; set; }
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string txtSearchShelfNoFrom { get; set; }
        public string txtSearchShelfNoTo { get; set; }
        public List<string> OfficeCodeList { get; set; }
        public List<string> LocationCodeList { get; set; }
    }
}

