using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Parameter for screen CMS300
    /// </summary>
    [MetadataType(typeof(CMS300_ScreenParameter_MetaData))]
    public class CMS300_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string ContractCode { get; set; }
        [KeepSession]
        public string OCC { get; set; }
        [KeepSession]
        public string ContractTargetCode { get; set; }
        [KeepSession]
        public string PurchaserCustCode { get; set; }
        [KeepSession]
        public string RealCustomerCode { get; set; }
        [KeepSession]
        public string SiteCode { get; set; }
        [KeepSession]
        public string ServiceTypeCode { get; set; }
        [KeepSession]
        public string MATargetContractCode { get; set; }
        [KeepSession]
        public string ProductCode { get; set; }

        // additional
        [KeepSession]
        public string CSCustCode { get; set; }
        [KeepSession]
        public string RCCustCode { get; set; }
        [KeepSession]
        public string Mode { get; set; }
    }

    public class CMS300_ScreenParameter_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public string ServiceTypeCode { get; set; }
    }
}
