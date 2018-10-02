using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of employee
    /// </summary>
    public partial class dtEmployee
    {
        public string EmpFullName {
            get {
                return "(1) "+EmpFirstNameEN+" "+EmpLastNameEN+"<br/>(2) "+EmpFirstNameLC+" "+EmpLastNameLC;
            }
        }

        public string EmpFullNameEN
        {
            get
            {
                return String.Format("{0} {1}", EmpFirstNameEN, EmpLastNameEN);
            }
        }

        public string EmpFullNameLC
        {
            get
            {
                return String.Format("{0} {1}", EmpFirstNameLC, EmpLastNameLC);
            }
        }
    }
}
