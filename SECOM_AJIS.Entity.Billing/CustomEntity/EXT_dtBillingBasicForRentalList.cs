using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Billing.MetaData;

namespace SECOM_AJIS.DataEntity.Billing
{

    public partial class dtBillingBasicForRentalList
    {
        public string ToJson { get { return CommonUtil.CreateJsonString(this); } }
        public string NewMonthlyBillingAmountCurrency { get; set; }
    }
}


