using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Inventory.Models
{
    /// <summary>
    /// Parameter for screen IVS150.
    /// </summary>
    public class IVS150_ScreenParameter : ScreenParameter
    {
        
    }

    public class EnableButton
    {
        public bool EnableBtnStartingChecking { set; get; }
        public bool EnableBtnStopChecking { set; get; }
    }
}
