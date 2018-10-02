using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Common.MetaData;

namespace SECOM_AJIS.DataEntity.Common
{
     [MetadataType(typeof(dtTPL_MetaData))]
    public partial class dtTPL
    {
       
        public string PurgeStatusName { get; set; }
    }
}

namespace SECOM_AJIS.DataEntity.Common.MetaData
{
    public class dtTPL_MetaData
    {
        [BatchStatusMapping("BatchStatus")]
        public string PurgeStatusName { get; set; }

    }
}


