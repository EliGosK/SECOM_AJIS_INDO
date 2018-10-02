using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of screen
    /// </summary>
    public class CTS390_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public bool hasPermission360 { get; set; }
    }

    /// <summary>
    /// Summary period for search
    /// </summary>
    public class doSummaryARPeriod
    {
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
        public DateTime? current { get; set; }
    }
}
