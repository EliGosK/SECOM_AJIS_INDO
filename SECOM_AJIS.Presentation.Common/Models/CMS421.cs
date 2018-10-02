﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter of CMS421
    /// </summary>
    public class CMS421_ScreenParameter : ScreenParameter
    {
        //------IN parameter ----//
        [KeepSession]
        public string ContractCode { set; get; }
        [KeepSession]
        public string BillingOCC { set; get; }
        //------IN parameter ----//

    }
}
