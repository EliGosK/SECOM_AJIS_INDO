﻿using System;
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
    /// Search criteria for screen IVS281.
    /// </summary>
    public partial class doIVS281SearchCondition
    {

        public string ReportType { get; set; }
        public string SlipNoStart { get; set; }
        public string SlipNoEnd { get; set; }
        public DateTime? StockOutDateStart { get; set; }
        public DateTime? StockOutDateEnd { get; set; }
        public string ContractCode { get; set; }
        public DateTime? OperateDateStart { get; set; }
        public DateTime? OperateDateEnd { get; set; }
        public string CustName { get; set; }

    }
}

