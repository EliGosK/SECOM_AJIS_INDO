using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Inventory.MetaData;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Inventory
{
    public class doIVS030SearchCondition
    {
        public string ContractCode { get; set; }
        public DateTime? CompleteDateFrom { get; set; }
        public DateTime? CompleteDateTo { get; set; }
        public string InstallationSlipNo { get; set; }
        public string SubContractorName { get; set; }
        public string ProjectCode { get; set; }
    }
}

