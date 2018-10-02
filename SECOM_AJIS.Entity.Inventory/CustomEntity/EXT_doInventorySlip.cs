using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class doInventorySlip
    {
        [LanguageMapping]
        public string SourceLocationName { get; set; }

        [LanguageMapping]
        public string DestinationLocationName { get; set; }
        [LanguageMapping]
        public string SourceOfficeName { get; set; }
        [LanguageMapping]
        public string DestinationOfficeName { get; set; }
    }
}
namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class doInventorySlip_Meta
    {
    }

}
