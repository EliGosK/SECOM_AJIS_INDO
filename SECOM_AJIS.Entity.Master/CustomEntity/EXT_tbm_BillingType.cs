using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of billint type
    /// </summary>
    public partial class tbm_BillingType
    {
        [LanguageMapping]
        public string BillingTypeName { get; set; }

        public string BillingTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.BillingTypeCode, this.BillingTypeName);
            }
        }
    }
}
