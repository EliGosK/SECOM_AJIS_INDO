using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class dtCreditCardForView
    {

        [LanguageMapping]
        public string CreditCardTypeName { get; set; }

        [LanguageMapping]
        public string CreditCardCompanyName { get; set; }

    }
}
