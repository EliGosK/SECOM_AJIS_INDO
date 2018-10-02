using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Common
{
    [Serializable]
    public class doEmailSearchCondition
    {
        public string EmployeeName { set; get; }
        public string EmailEddress { set; get; }
        public string Office { set; get; }
        public string Department { set; get; }

        public int Counter { set; get; }
    
    }
}
