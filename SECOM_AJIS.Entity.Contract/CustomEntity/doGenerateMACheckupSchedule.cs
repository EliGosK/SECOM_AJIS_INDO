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
    //[MetadataType(typeof(doGenerateMACheckupSchedule_MetaData))]
    public class doGenerateMACheckupSchedule
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public string MAProcessType { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{

    //public class doGenerateMACheckupSchedule_MetaData
    //{
    //    [NotNullOrEmpty]
    //    public string ContractCode { get; set; }
    //}

}

