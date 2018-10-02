using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Presentation.Common
{
    /// <summary>
    /// DO for store search instrument of screen CMS170
    /// </summary>
    [Serializable]
    public class doCMS170_SearchInstrument
    {
        public string InstumentCode { get; set; }
        public string InstumentName { get; set; }
        public string Maker { get; set; }
        public string InstrumentType_1 { get; set; }
        public string InstrumentType_2 { get; set; }
        public string InstrumentType_3 { get; set; }
        public string InstrumentFlag_1 { get; set; }
        public string InstrumentFlag_2 { get; set; }
        public string ExpantionType_1 { get; set; }
        public string ExpantionType_2 { get; set; }
        public string ProductType_1 { get; set; }
        public string ProductType_2 { get; set; }
        public string LineUpType { get; set; }
        public string SupplierType { get; set; }
        
    }
}
