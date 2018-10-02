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
    [MetadataType(typeof(doContractSite_MetaData))]
    public class doContractSite
    {
        public virtual string SiteCode { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{
    public class doContractSite_MetaData
    {
        [NotNullOrEmpty]
        public string SiteCode { get; set; }
    }
}

