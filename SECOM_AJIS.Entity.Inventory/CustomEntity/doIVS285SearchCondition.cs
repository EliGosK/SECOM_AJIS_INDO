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
    /// Search criteria for screen IVS285.
    /// </summary>
    public partial class doIVS285SearchCondition
    {

        public string ReportType { get; set; }
        public string InstrumentCode { get; set; }
        public string[] InstrumentCodeSelected { get; set; }
        public string YearMonth { get; set; }

    }
}

