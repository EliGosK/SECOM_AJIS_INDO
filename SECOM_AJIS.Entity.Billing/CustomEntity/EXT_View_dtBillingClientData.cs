using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Master;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Billing.CustomEntity
{
   public class do_View_dtBillingClientData : View_dtBillingClientData        
    {
        [LanguageMapping]
        public string CustTypeName { get; set; }

        [LanguageMapping]
        public string BusinessTypeName { get; set; }

        [LanguageMapping]
        public string Nationality { get; set; }

        [LanguageMapping]
        public string CompanyTypeName { get; set; }

    }
}
