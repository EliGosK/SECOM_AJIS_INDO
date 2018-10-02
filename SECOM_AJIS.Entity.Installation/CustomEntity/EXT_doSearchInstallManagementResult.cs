using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Installation
{
    public partial class doSearchInstallManagementResult
    {
        public string IEStaffName
        {
            get;
            set;
        }

        public string Subcontractor
        {
            get;
            set;
        }
        public string IntsallationDate
        {
            get;
            set;
        }
        public string Site
        {
            get;
            set;
        }
        public string OperationOffice
        {
            get;
            set;
        }
        
    }
}
