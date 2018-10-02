using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;
using System.IO;

namespace SECOM_AJIS.Presentation.Common.Models
{
    public class CMS999_ScreenParameter : ScreenParameter
    {
        public Stream IVR010 { get; set; }
        public Stream IVR020 { get; set; }
        public Stream IVR030 { get; set; }
        public Stream IVR040 { get; set; }
        public Stream IVR050 { get; set; }
        public Stream IVR060 { get; set; }

        public Stream IVR070 { get; set; }
        public Stream IVR080 { get; set; }
        public Stream IVR090 { get; set; }
        public Stream IVR120 { get; set; }
        public Stream IVR130 { get; set; }
        public Stream IVR170 { get; set; }
        public Stream IVR180 { get; set; }

        public Stream IVR140 { get; set; }
        public Stream IVR150 { get; set; }
        public Stream IVR190 { get; set; }
        public Stream IVR191 { get; set; }
        public Stream IVR192 { get; set; }

        public Stream IVR210 { get; set; }

        public Stream ICR030 { get; set; }
        public Stream ICR040 { get; set; }
    }
}
