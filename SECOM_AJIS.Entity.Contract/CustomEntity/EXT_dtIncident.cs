using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtIncident
    {
        [LanguageMapping]
        public string RegistrantName { get; set; }
        [LanguageMapping]
        public string IncidentTypeName { get; set; }
        [LanguageMapping]
        public string ReasonTypeName { get; set; }

        public string InteractionType { get; set; }
    }
}
