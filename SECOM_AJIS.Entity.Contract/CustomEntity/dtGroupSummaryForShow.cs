using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Contract.MetaData;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class dtGroupSummaryForShow
    {
        public string RowHeader { get; set; }
        public string RowPrefix { get; set; }
        public string ContractAlarm { get; set; }
        public string ContractMaintenance { get; set; }
        public string ContractGuard { get; set; }
        public string ContractSale { get; set; }
        public string CustomerAlarm { get; set; }
        public string CustomerMaintenance { get; set; }
        public string CustomerGuard { get; set; }
        public string CustomerSale { get; set; }
    }
}



