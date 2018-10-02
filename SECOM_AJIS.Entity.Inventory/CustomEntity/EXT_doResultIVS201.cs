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
    [MetadataType(typeof(doResultIVS201_Meta))]
    public partial class doResultIVS201
    {
        public string OfficeName { get; set; }
        public string LocationName { get; set; }
        public string AreaName { get; set; }
    }

}

namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class doResultIVS201_Meta
    {
        [LanguageMapping]
        public string OfficeName { get; set; }
        [LanguageMapping]
        public string LocationName { get; set; }
        [LanguageMapping]
        public string AreaName { get; set; }
        [GridToolTip("AreaName")]
        public string AreaNameShort { get; set; }
    }
}
