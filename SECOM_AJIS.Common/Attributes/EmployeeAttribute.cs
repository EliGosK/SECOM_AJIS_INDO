using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.CustomAttribute
{
    /// <summary>
    /// Attribute for mapping employee name to selected object
    /// </summary>
    public class EmployeeMappingAttribute : Attribute
    {
        public string EmployeeNameField { get; set; }

        public EmployeeMappingAttribute(string EmployeeNameField)
        {
            this.EmployeeNameField = EmployeeNameField;
        }
    }
    /// <summary>
    /// Attribute for check employee is exist?
    /// </summary>
    public class EmployeeExistAttribute : Attribute
    {
        public string Parameter { get; set; }
        public string Control { get; set; }
    }
}
