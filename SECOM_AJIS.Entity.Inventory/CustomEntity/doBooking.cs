using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Inventory
{
    /// <summary>
    /// Data object of booking.
    /// </summary>
    public class doBooking
    {
        public string ContractCode { get; set; }
        public DateTime ExpectedStartServiceDate { get; set; }
        public List<string> InstrumentCode { get; set; }
        public List<int> InstrumentQty { get; set; }
        public bool blnExistContractCode { get; set; }
        public bool blnFirstInstallCompleteFlag { get; set; }
    }
}
