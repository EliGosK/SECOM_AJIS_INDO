using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Payment matching description to show in viewing payment matching result.
    /// </summary>
    public partial class doPaymentMatchingDesc
    {
        [LanguageMapping]
        public string Description { get; set; }  
    }
}
