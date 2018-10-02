using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Do Of Screen parameter of CMS410
    /// </summary>
    public class CMS410_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string BillingTargetCode { get;set;}
    }
}
