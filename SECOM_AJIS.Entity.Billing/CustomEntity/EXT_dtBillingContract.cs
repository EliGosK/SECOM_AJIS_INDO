using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Billing.MetaData;

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class dtBillingContract
    {

        public decimal? TotalFee { set; get; }
        public string TotalFeeForDisplay { set; get; }
        public List<dtBillingBasicForRentalList> details { get; set; }
    }
}
