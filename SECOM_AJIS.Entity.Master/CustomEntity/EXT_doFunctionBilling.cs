using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// do Of funtion billing
    /// </summary>
    public partial class doFunctionBilling
    {
        [LanguageMapping]
        public string OfficeName { get; set; }

        public string OfficeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(OfficeCode, OfficeName);
            }
        }
    }
}
