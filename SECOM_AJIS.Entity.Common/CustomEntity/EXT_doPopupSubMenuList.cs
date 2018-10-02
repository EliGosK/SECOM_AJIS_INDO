using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Common
{
    public partial class doPopupSubMenuList
    {
        [LanguageMapping]
        public string PopupSubmenuName { get; set; }
        [LanguageMapping]
        public string ObjectName { get; set; }
        [LanguageMapping]
        public string ObjectDescription { get; set; }
    }
}
