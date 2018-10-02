using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class tbt_BillingDetail
    {
        public Nullable<DateTime> StartOperationDate
        {
            get;
            set;
        }
        public string InvoiceDescriptionEN { get; set; }
        public string InvoiceDescriptionLC { get; set; }

        public string SiteNameEN { get; set; }
        public string SiteNameLC { get; set; }

        public string BillingTypeGroup { get; set; }

        
    }
}
