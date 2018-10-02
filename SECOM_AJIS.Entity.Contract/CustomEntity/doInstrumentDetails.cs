using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doInstrumentDetails
    {
        public string InstrumentCode { get; set; }
        public int InstrumentQty { get; set; }
        public int AddQty { get; set; }
        public int RemoveQty { get; set; }
        public string InstrumentTypeCode { get; set; }
    }
}
