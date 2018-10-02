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
    public partial class dtRentalContractBasicForView
    {
        [LanguageMapping]
        public string LastChangeTypeName { get; set; }

        [LanguageMapping]
        public string ProductName { get; set; }
        [LanguageMapping]
        public string SiteName { get; set; }
        [LanguageMapping]
        public string ContractTargetName { get; set; }
        [LanguageMapping]
        public string OfficeName { get; set; }
    }
}
