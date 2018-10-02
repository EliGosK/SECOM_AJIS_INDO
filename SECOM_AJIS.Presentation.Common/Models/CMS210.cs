using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Common.Models.MetaData;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter for CMS210
    /// </summary>
    public partial class CMS210_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        [NotNullOrEmpty]
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
        [NotNullOrEmpty]
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

   
}

namespace SECOM_AJIS.Presentation.Common.Models.MetaData
{
   
}

