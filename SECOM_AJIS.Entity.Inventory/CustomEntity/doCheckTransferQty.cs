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
    /// Data object for check transfer quantity.
    /// </summary>
    public class doCheckTransferQty
    {
        public string OfficeCode { get; set; }
        public string LocationCode { get; set; }
        public string AreaCode { get; set; }
        public string ShelfNo { get; set; }
        public string InstrumentCode { get; set; }
        public int TransferQty { get; set; }

    }



}