using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.CustomAttribute
{
    /// <summary>
    /// Attribute for mapping instrument data
    /// </summary>
    public class InstrumentMappingAttribute : Attribute
    {
        public string[] InstrumentField { get; set; }

        public InstrumentMappingAttribute(string[] InstrumentField)
        {
            this.InstrumentField = InstrumentField;
        }
    }
}
