using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of customer data
    /// </summary>
    public partial class dtCustomerData
    {
        [LanguageMapping]
        public string BusinessTypeName { get; set; }
        [LanguageMapping]
        public string Nationality { get; set; }
    }
}
