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
    public partial class dtAR
    {
        [LanguageMapping]
        public string ARStatusName { get; set; }

        [LanguageMapping]
        public string ARTypeName { get; set; }

        [LanguageMapping]
        public string RegistrantName { get; set; }
    }
}
