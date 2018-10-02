using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.DataEntity.Common
{
    public partial class doContractInfoCondition : ScreenParameter
    {
        public string ContractCode { get; set; }
        public string OCC { get; set; }
        public string ContractTargetCode { get; set; }
        public string PurchaserCustCode { get; set; }
        public string RealCustomerCode { get; set; }
        public string SiteCode { get; set; }
        public string ServiceTypeCode { get; set; }
        public string MATargetContractCode { get; set; }
        public string ProductCode { get; set; }

        // additional
        public string CSCustCode { get; set; }
        public string RCCustCode { get; set; }
        public string Mode { get; set; }

    }
}
