using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of table building usage
    /// </summary>
    public partial class tbm_BuildingUsage
    {
        [LanguageMapping]
        public string BuildingUsageName { get; set; }
    }
}
