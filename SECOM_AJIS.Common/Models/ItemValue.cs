using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models
{
    /// <summary>
    /// DO for item data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ItemValue<T>
    {
        private T objValue; public object Value { get { return this.objValue; } set { this.objValue = (T)value; } }
        private string strDisplay; public string Display { get { return this.strDisplay; } set { this.strDisplay = value; } }

    }
}
