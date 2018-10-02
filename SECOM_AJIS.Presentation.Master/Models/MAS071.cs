using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Presentation.Master.Models.MetaData;
using SECOM_AJIS.Common.Models;
namespace SECOM_AJIS.Presentation.Master.Models
{
    /// <summary>
    /// Parameter for screen MAS070.
    /// </summary>
    public class MAS071_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public bool hasPermissionAdd { get; set; }
        [KeepSession]
        public bool hasPermissionEdit { get; set; }
        [KeepSession]
        public bool hasPermissionDelete { get; set; }
    }
}
