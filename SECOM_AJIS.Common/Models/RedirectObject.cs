using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Models
{
    /// <summary>
    /// DO for redirect url
    /// </summary>
    public class RedirectObject
    {
        public string URL { get; set; }
        public bool IsRedirectToLogin { get { return true; } }
    }
}
