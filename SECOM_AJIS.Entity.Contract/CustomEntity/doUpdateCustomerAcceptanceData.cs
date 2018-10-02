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
    [MetadataType(typeof(doUpdateCustomerAcceptanceData_MetaData))]
    public class doUpdateCustomerAcceptanceData
    {
        public virtual string ContractCode { get; set; }
        public virtual DateTime? Date { get; set; }
        public virtual string OCC { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{

    public class doUpdateCustomerAcceptanceData_MetaData
    {
        [NotNullOrEmpty(Parameter = "Contract code", Order = 1)]
        public string ContractCode { get; set; }

        [NotNullOrEmpty(Parameter = "Customer acceptance date", Order = 3)]
        public string Date { get; set; }

        [NotNullOrEmpty(Parameter = "Sale OCC", Order = 2)]
        public string OCC { get; set; }
    }
}

