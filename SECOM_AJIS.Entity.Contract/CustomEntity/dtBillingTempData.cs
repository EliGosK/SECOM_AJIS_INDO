using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    [MetadataType(typeof(dtBillingTempData_MetaData))]
    public class dtBillingTempData : tbt_BillingTemp
    {

    }

    [MetadataType(typeof(dtBillingTempChangePlanData_MetaData))]
    public class dtBillingTempChangePlanData : tbt_BillingTemp
    {
        public string Sequence { get; set; }
        public string Status { get; set; }
        public int DataComeFrom { get; set; }
        public bool isShowOnGrid { get; set; }
        public string uIDNew { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{

    public class dtBillingTempData_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public string OCC { get; set; }

        [NotNullOrEmpty]
        public int Sequence { get; set; }
    }

    public class dtBillingTempChangePlanData_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
    }
}

