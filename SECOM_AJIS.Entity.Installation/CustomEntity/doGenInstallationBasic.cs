using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Installation
{
    public class doGenInstallationBasic
    {
        public string ContractProjectCode { get; set; }
        public string OCC { get; set; }
        public string ServiceTypeCode { get; set; }
        public string InstallationStatus { get; set; }
        public string InstallationType { get; set; }
        public string OperationOfficeCode { get; set; }
        public string SecurityTypeCode { get; set; }
        public decimal NormalInstallFee { get; set; }
        public string ApproveNo1 { get; set; }
        public string  ApproveNo2 { get; set; }
        public decimal NormalContractFee { get; set; }
        public List<tbt_InstallationInstrumentDetails> doInstrumentDetails { get; set; } 
    }
}
