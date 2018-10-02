using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doARPermission
    {
        public bool AssignApproverFlag { get; set; }
        public bool AssignAuditorFlag { get; set; }
        public bool ViewARDetailFlag { get; set; }
        public bool EditARDetailFlag { get; set; }
    }
}
