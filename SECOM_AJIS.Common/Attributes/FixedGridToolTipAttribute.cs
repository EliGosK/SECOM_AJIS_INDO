using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.CustomAttribute
{
    /// <summary>
    /// Attribte for fixed grid tool
    /// </summary>
    public class FixedGridToolTipAttribute : Attribute
    {
        public string ToolTipText { get; set; }
        public FixedGridToolTipAttribute(string strToolTipText)
        {
            ToolTipText = strToolTipText;
        }
    }

}
