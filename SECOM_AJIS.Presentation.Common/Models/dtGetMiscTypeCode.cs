using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// DO for get MiscType data
    /// </summary>
    [Serializable]
    public class dtGetMiscTypeCode
    {
        private string strFieldName; 
        public string FieldName { get { return this.strFieldName; } set { this.strFieldName = value; } }
        private string strValueCode; 
        public string ValueCode { get { return this.strValueCode; } set { this.strValueCode = value; } } 
    }
}
