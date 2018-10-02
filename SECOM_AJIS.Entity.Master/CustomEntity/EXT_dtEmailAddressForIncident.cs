using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Master
{
    public partial class dtEmailAddressForIncident
    {
        [LanguageMapping]
        public string EmpFirstName { get; set; }

        [LanguageMapping]
        public string EmpLastName { get; set; }

    }
}
