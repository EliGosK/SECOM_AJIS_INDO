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
    [MetadataType(typeof(doUpdateStockOutInstrumentData_MetaData))]
    public class doUpdateStockOutInstrumentData
    {
        public virtual string ProjectCode { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{

    public class doUpdateStockOutInstrumentData_MetaData
    {
        [NotNullOrEmpty]
        public string ProjectCode { get; set; }
    }
}

