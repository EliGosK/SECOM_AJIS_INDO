using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;


namespace SECOM_AJIS.DataEntity.Contract
{
    public class doBillingBasicForMAResultBasedFeePayment
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
    }
}
