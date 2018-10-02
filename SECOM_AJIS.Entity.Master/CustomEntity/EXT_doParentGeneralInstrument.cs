using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of parent general instrument
    /// </summary>
    public partial class doParentGeneralInstrument
    {
        [LanguageMapping]
        public string LineUpTypeName { get; set; }
    }
}
