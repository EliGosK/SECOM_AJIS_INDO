using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Income.MetaData;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data ojbect of payment information.
    /// </summary>
    public partial class doDebtTracingPermission
    {
        [LanguageMapping]
        public string OfficeName
        {
            get;
            set;
        }

        public string OfficeDisplay
        {
            get
            {
                return CommonUtil.TextCodeName(OfficeCode, OfficeName);
            }
        }
    }
}
