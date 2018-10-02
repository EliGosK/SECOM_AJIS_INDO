using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    public partial class tbm_Region
    {
        [LanguageMapping]
        public string Nationality { get; set; }
        [LanguageMapping]
        public string RegionName { get; set; }
    }
}