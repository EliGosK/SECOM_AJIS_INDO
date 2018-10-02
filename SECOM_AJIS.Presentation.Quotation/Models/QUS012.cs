using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Quotation.Models
{
    /// <summary>
    /// DO for initial screen
    /// </summary>
    public class QUS012_ScreenParameter :ScreenParameter
    {
        public doGetQuotationDataCondition Condition { get; set; }

        public doRentalQuotationData doRentalQuotationData { get; set; }
        public bool HideQuotationTarget { get; set; }

        public tbt_QuotationInstallationDetail doQuotationInstallationDetail { get; set; }
    }
}
