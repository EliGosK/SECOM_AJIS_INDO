using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class tbs_ARTypeTitle
    {
        [LanguageMapping]
        public string ARTitleName { get; set; }
    }
}
