using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Inventory.MetaData;


namespace SECOM_AJIS.DataEntity.Inventory
{
    [MetadataType(typeof(dtResultInventorySlipDetail_Meta))]
    public partial class dtResultInventorySlipDetail
    {
        public string SourceAreaName { get; set; }
        public string DestAreaName { get; set; }
    }

}

namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class dtResultInventorySlipDetail_Meta
    {
        [LanguageMapping]
        public string SourceAreaName { get; set; }
        [LanguageMapping]
        public string DestAreaName { get; set; }
    }

}
