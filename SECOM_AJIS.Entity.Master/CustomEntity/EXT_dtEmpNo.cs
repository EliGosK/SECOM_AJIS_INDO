using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of employee no
    /// </summary>
    public partial class dtEmpNo
    {
        [LanguageMapping]
        public string EmpFirstName { get; set; }

        [LanguageMapping]
        public string EmpLastName { get; set; }
    }
}