using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using System.IO;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// Screen parameter for CMS030
    /// </summary>
    public class CMS030_ScreenParameter : ScreenParameter
    {
        public string DocumentNo { get; set; }
        public string DocumentOCC { get; set; }
        public string DocumentCode { get; set; }
        public string FileName { get; set; }
        public Stream StreamReport { get; set; }
    }
}
