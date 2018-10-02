using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of department
    /// </summary>
    public partial class dtDepartment
    {
        public string DepartmentNameCode
        {
            get
            {
                return SECOM_AJIS.Common.Util.CommonUtil.TextCodeName(DepartmentCode, DepartmentName);
            }
        }
    }
}
