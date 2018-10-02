using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Debt summary information of a billing target.
    /// </summary>
    public partial class doBillingTargetDebtSummary : doGetBillingTargetDebtSummaryByOffice
    {
        public int TracingResultRegistered
        {
            get {return DebtTracingRegistered ;}
            set { DebtTracingRegistered = value; }
        }
    }
}
