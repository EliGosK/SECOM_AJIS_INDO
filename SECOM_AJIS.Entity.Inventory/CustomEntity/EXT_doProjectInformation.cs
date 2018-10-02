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
    [MetadataType(typeof(doProjectInformation_Meta))]
    public partial class doProjectInformation
    {
        public string ProjectManagerName { get; set; }
    }

}

namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class doProjectInformation_Meta
    {
        [LanguageMapping]
        public string ProjectManagerName { get; set; }
    }
}
