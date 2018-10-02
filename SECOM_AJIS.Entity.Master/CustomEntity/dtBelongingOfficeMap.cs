using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of belonging office map
    /// </summary>
    public partial class dtBelongingOfficeMap:dtBelongingOffice
    {
        [LanguageMapping]
        public string OfficeName { get; set; }
        [LanguageMapping]
        public string OfficeCodeName { get; set; }
    }
}
