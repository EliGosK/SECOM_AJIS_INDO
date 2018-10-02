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
    [MetadataType(typeof(doOffice_Meta))]
    public partial class doOffice
    {
        public string OfficeName { get; set; }

        public string OfficeCodeName
        {
            get
            {
                return Common.Util.CommonUtil.TextCodeName(this.OfficeCode, this.OfficeName);
            }
        }

    }
}
namespace SECOM_AJIS.DataEntity.Inventory.MetaData
{
    public class doOffice_Meta
    {
        [LanguageMapping]
        public string OfficeName { get; set; }
    }
}
