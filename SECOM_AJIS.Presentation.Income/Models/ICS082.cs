using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;

using SECOM_AJIS.Presentation.Common.Models.MetaData;

namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Data object for ICS_082 screen
    /// </summary>
    public class ICS082_ScreenParameter : ScreenParameter
    {
        public string BillingTargetCode { get; set; }
    }
}