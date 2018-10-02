using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.Presentation.Common.Models
{
    /// <summary>
    /// DO for get Contract data
    /// </summary>
    public class dsContractData
    {
        public List<tbt_RentalContractBasic> dtRCB { get; set; }
        public List<tbt_SaleBasic> dtSB { get; set; }
    }

    /// <summary>
    /// DO for store search bar data
    /// </summary>
    public class dtSearchBarData
    {
        public string ContractCode { get; set; }
        public string ServiceType { get; set; }
        public string InvoiceNo { get; set; }
        public string ProjectCode { get; set; }
        public string Mode { get; set; }
    }
}
