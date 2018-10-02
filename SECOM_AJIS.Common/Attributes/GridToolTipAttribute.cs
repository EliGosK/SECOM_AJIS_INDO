using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.CustomAttribute
{
    /// <summary>
    /// Attribute for show tooltip in grid
    /// </summary>
    public class GridToolTipAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public GridToolTipAttribute(string strPropName)
        {
            PropertyName = strPropName;
        }
    }

}
