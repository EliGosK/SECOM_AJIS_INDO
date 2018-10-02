﻿using System;
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
    /// Screen parameter for CMS131
    /// </summary>
    public partial class CMS131_ScreenParameter : ScreenParameter
    {
        //============= In parameter=============//
        [KeepSession]
        public string strContractCode { get; set; }
        [KeepSession]
        public string strOCC { get; set; }
        //============= In parameter=============//

        [KeepSession]
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [KeepSession]
        public string OCC { get; set; }
        public string ContractTargetCode { get; set; }
        public string PurchaserCustCode { get; set; }
        public string RealCustomerCode { get; set; }
        public string SiteCode { get; set; }

        [NotNullOrEmpty]
        public string ServiceTypeCode { get; set; }
        public string MATargetContractCode { get; set; }
        public string ProductCode { get; set; }

        // additional
        public string CSCustCode { get; set; }
        public string RCCustCode { get; set; }
        public string Mode { get; set; }
    }

   
}

namespace SECOM_AJIS.Presentation.Common.Models.MetaData
{
   
}

