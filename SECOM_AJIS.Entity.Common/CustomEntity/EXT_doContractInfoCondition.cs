using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
namespace SECOM_AJIS.DataEntity.Common
{
    /// <summary>
    /// DO for check validate contidion of screen CMS150.
    /// </summary>
    [MetadataType(typeof(doContractInfoCondition_MetaData))]
    public partial class doContractInfoCondition
    {
    }

    public class doContractInfoCondition_MetaData
    {   [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public string ServiceTypeCode { get; set; }
    }

}
