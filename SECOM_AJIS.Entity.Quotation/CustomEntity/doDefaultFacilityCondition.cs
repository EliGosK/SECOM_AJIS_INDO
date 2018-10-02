using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class doDefaultFacilityCondition
    {
        [NotNullOrEmpty]
        public string ProductCode { get; set; }
    }
}
