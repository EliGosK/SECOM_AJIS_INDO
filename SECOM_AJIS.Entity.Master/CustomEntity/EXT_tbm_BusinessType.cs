using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of table business type
    /// </summary>
    public partial class tbm_BusinessType
    {
        [LanguageMapping]
        public string BusinessTypeName { get; set; }
    }
}
