using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class dtIncidentRole
    {
        [LanguageMapping]
        public string EmpFirstName { get; set; }
        [LanguageMapping]
        public string EmpLastName { get; set; }
        [LanguageMapping]
        public string IncidentRoleTypeName { get; set; }
        [LanguageMapping]
        public string OfficeName { get; set; }
    }
}
