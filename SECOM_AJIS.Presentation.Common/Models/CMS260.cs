using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter for CMS260
    /// </summary>
    public class CMS260_ScreenParameter : ScreenParameter
    {
        public string strRealCustomerCode { get; set; }

    }

   
}
