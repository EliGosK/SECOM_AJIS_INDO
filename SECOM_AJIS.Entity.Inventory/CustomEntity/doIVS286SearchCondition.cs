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
    /// Search criteria for screen IVS286.
    /// </summary>
    public partial class doIVS286SearchCondition
    {

        public string ReportType { get; set; }
        public string ContractCode { get; set; }
        public string ProcessDate { get; set; }
        public string[] ContractCodeSelected { get; set; }

    }
}

