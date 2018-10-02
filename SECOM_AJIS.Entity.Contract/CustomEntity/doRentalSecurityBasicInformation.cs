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
    [MetadataType(typeof(doRentalContractBasicInformation_C_MetaData))]
    public partial class doRentalSecurityBasicInformation
    {
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class doRentalSecurityBasicInformation_C_MetaData : doRentalSecurityBasicInformation
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }

        [NotNullOrEmpty]
        public string OCC { get; set; }
    }
}
