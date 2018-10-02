using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Presentation.Common
{
    /// <summary>
    /// DO for view customer group of screen CMS100
    /// </summary>
    [Serializable]
    public class doCMS100_ViewCustomerGroup
    {
        public string GroupCode { get; set; }
        public string StrPreFix { get; set; }
    }
}
