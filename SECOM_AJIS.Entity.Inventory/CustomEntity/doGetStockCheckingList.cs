using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    /// <summary>
    /// Search condition for get stock checking list.
    /// </summary>
    public class doGetStockCheckingList
    {

        public string CheckingYearMonth { get; set; }
        public string OfficeCode { get; set; }
        public string OfficeText { get; set; }
        public string LocationCode { get; set; }
        public string LocationText { get; set; }
        public string AreaCode { get; set; }
        public string AreaText { get; set; }
        public string ShelfNoFrom { get; set; }
        public string ShelfNoTo { get; set; }
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }

    }



}