using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.Presentation.Master.Models
{
    /// <summary>
    /// Parameter for screen MAS080.
    /// </summary>
    public class MAS080_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public bool hasPermissionAdd { get; set; }
        [KeepSession]
        public bool hasPermissionEdit { get; set; }
        [KeepSession]
        public bool hasPermissionDelete { get; set; }
        [KeepSession]
        public List<dtPermissionHeader> PermissionList { get; set; }
    }

    /// <summary>
    /// DO of data for save employee
    /// </summary>
    public class MAS080_dtEmpNo : dtEmpNo
    {
        public string ModifyMode { get; set; }
    }
}
