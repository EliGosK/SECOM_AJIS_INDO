using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter for CMS250
    /// </summary>
    public class CMS250_ScreenParameter : ScreenParameter
    {
        public bool bExistCustOnlyFlag { get; set; }
    }
}
