using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models
{
    /// <summary>
    /// DO for input redirect URL
    /// </summary>
    public class doDirectScreen
    {
        public string Controller { get; set; }
        public string ScreenID { get; set; }
        public List<string> Parameters { get; set; }
        public List<string> Values { get; set; }
    }
}
