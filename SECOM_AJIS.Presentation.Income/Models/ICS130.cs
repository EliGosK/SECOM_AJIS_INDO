﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
//using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Presentation.Income.Models.MetaData;
namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Parameter of ICS130 screen
    /// </summary>
    public class ICS130_ScreenParameter : ScreenParameter
    {
        public enum eCacheReportType
        {
            Account,
            IMS
        }
        public eCacheReportType? CachedReportType { get; set; }
        public DateTime? CachedPeriodFrom { get; set; }
        public DateTime? CachedPeriodTo { get; set; }
        public List<doWHTReportForAccount> CachedAccountReport { get; set; }
        public List<doWHTReportForIMS> CachedIMSReport { get; set; }
    }
}

