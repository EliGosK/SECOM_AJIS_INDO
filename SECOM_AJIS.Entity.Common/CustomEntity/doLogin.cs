using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Common
{
    /// <summary>
    /// Parameter for Login function
    /// </summary>
    public class doLogin
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_COMMON,
                Screen = "CMS010",
                Parameter = "lblEmpNo",
                ControlName = "EmpNo")]
        public string EmployeeNo { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_COMMON,
                Screen = "CMS010",
                Parameter = "lblPassword",
                ControlName = "Password")]
        public string Password { get; set; } 
    }
}
