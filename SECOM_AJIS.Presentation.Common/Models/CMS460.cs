using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;



namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter of CMS460
    /// </summary>
    public class CMS460_ScreenParameter : ScreenParameter
    {
       
    }

    /// <summary>
    /// Screen input validate of CMS460
    /// </summary>
    public class CMS460_ScreenInputValidate 
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_COMMON,
                        Screen = "CMS460",
                        Parameter = "lblIssuedate",
                        ControlName = "IssueDate")]
        public DateTime? IssueDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_COMMON,
                       Screen = "CMS460",
                       Parameter = "lblManagementNo",
                       ControlName = "ManagementNoFrom")]
        public int? ManagementNoFrom { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_COMMON,
                       Screen = "CMS460",
                       Parameter = "lblManagementNo",
                       ControlName = "ManagementNoTo")]
        public int? ManagementNoTo { get; set; }


    }

}
