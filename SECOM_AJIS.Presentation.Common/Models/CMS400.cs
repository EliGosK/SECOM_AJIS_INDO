using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen paramegter of CMS400
    /// </summary>
    public class CMS400_ScreenParameter : ScreenParameter
    {
        
        [KeepSession]
        public string BillingTargetCode { set; get; }

        [KeepSession]
        public bool isEnableBtnShowInvoiceList { set; get; }

        [KeepSession]
        public bool ShowInvoiceList { set; get; }
        
        [KeepSession]
        public dtTbt_BillingTargetForView dtBillingTargetForView { set; get; }

        [KeepSession]
        public List<dtViewBillingBasicList> doBasicList { set; get; }

        [KeepSession]
        public List<dtViewBillingInvoiceListOfLastInvoiceOcc> dtViewBillingInvoiceListOfLastInvoiceOccList { set; get; }
        
    }
}
