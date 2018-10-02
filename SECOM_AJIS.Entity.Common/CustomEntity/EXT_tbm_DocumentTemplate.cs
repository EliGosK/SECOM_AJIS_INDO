using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Common
{

    public partial class tbm_DocumentTemplate
    {
        [LanguageMapping]
        public string DocumentName { get; set; }
    }
}
