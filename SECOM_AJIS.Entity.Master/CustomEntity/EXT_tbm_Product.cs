using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    public partial class tbm_Product
    {
        [LanguageMapping]
        public string ProductName { get; set; }
    }
}
