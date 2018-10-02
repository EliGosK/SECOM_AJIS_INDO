using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of active employee list
    /// </summary>
    public partial class doActiveEmployeeList
    {
        public string EmpFirstName { get; set; }
        public string EmpLastName { get; set; }
        public string EmpFullName
        {
            get
            {
                return CommonUtil.TextList(new string[] { this.EmpFirstName, this.EmpLastName }, " ");
            }
        }
    }
}
