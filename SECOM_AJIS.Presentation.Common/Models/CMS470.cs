using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter of CMS470
    /// </summary>
    public class CMS470_ScreenParameter : ScreenParameter
    {

        [KeepSession]
        public string ContractCode { set; get; }

        [KeepSession]
        public string BillingOCC { set; get; }

    }


}
