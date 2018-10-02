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
    public class doInsertDepreciationData
    {
        public string ContractCode	 { get; set; }
        public string InstrumentCode	 { get; set; }
        public string StartYearMonth	 { get; set; }
        public int DepreciationPeriod { get; set; }
        public decimal MovingAveragePrice { get; set; }
        public string StartType { get; set; }
    }
}