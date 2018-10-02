using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of instrument search condition
    /// </summary>
    public class doInstrumentSearchCondition
    {
        public string InstrumentCode { get; set; }
        public string InstrumentName { get; set; }
        public string Maker { get; set; }
        public string SupplierCode { get; set; }
        public string LineUpTypeCode { get; set; }
        public List<int?> InstrumentFlag { get; set; }
        public List<string> ExpansionType { get; set; }
        public Nullable<int> SaleFlag { get; set; }
        public Nullable<int> RentalFlag { get; set; }
        //public Nullable<int> Alarm { get; set; }
        public List<string> InstrumentType { get; set; }
    }
}
