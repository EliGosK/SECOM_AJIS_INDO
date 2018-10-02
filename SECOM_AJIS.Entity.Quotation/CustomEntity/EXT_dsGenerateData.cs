using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation.MetaData;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(dsGenerateData_MetaData))]
    public partial class dsGenerateData
    {
        
    }
}
#region Meta Data
namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    public class dsGenerateData_MetaData
    {
        [NotNullOrEmpty]
        public string ContractCode { get; set; }
        [NotNullOrEmpty]
        public int InstallationFee { get; set; }
    }

#endregion
}
