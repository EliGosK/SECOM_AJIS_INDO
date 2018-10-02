using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace SECOM_AJIS.DataEntity.Inventory
{
    /// <summary>
    /// Search condition for search installation slip.
    /// </summary>
    public class doSearchInstallationSlipCond
    {
        public string InstallationSlipNo { get; set; }
        public DateTime? ExpectedStockOutDateFrom { get; set; }
        public DateTime? ExpectedStockOutDateTo { get; set; }
        public string ContractCode { get; set; }
        public string ProjectCode { get; set; }
        public string OperationOfficeCode { get; set; }
        public string SubContractorName { get; set; }
        public string[] OperationOfficeCodeList { get; set; }
    }
}

