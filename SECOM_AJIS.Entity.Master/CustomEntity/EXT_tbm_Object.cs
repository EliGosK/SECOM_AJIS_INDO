using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    public partial class tbm_Object
    {
        [LanguageMapping]
        public string ObjectName { get; set; }
        
        [LanguageMapping]
        public string ObjectDescription { get; set; }

    }
}
