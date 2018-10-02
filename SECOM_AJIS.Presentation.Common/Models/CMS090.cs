using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Presentation.Common.Models.MetaData;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Parameter for screeb CMS090
    /// </summary>
    public class CMS090_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public bool hasPermission_CMS100 { get; set; }
    }
}
