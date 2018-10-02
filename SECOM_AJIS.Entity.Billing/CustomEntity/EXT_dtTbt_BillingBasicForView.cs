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

    public partial class dtTbt_BillingBasicForView
    {
        [LanguageMapping]
        public string OfficeName { get; set; }

        //[LanguageMapping]
        //public string DebtTracingOfficeName { get; set; }

        //[LanguageMapping]
        //public string MethodName { get; set; }

        //public string CustTypeCodeName { get; set; }
        //public string StopBillingFlagName { get; set; }
      
    }
}

