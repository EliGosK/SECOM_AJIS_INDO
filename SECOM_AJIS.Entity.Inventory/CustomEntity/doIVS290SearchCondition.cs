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
    /// Search criteria for screen IVS289.
    /// </summary>
    public partial class doIVS290SearchCondition
    {
        public string SlipNoStart { get; set; }
        public string SlipNoEnd { get; set; }
        public DateTime? TransferDateStart { get; set; }
        public DateTime? TransferDateEnd { get; set; }
    }
}

