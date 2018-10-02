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
    /// Search criteria for screen IVS200.
    /// </summary>
    public partial class doGetIVS200
    {
        public string OfficeCode { get; set; }
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public bool HaveOrder { get; set; }
        public bool BelowSafety { get; set; }
        public bool Minus { get; set; }
    }

    public partial class doGetIVS200_Detail
    {
        public string InstrumentCode { get; set; }
    }

}
