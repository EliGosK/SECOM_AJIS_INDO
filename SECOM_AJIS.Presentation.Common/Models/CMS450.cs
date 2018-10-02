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
    /// Screen parameter of CMS450
    /// </summary>
    public class CMS450_ScreenParameter : ScreenParameter
    {
        CommonUtil cm = new CommonUtil();
        [KeepSession]
        public string ContractCode { set; get; }

        [KeepSession]
        public string BillingOCC { set; get; }

        [KeepSession]
        public string BillingTargetCode { set; get; }

        [KeepSession]
        public string InvoiceNo { set; get; }

      
        
    }

  
}
