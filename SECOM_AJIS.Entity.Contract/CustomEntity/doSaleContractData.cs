using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class doSaleContractData
    {
        public tbt_SaleBasic dtTbt_SaleBasic { get; set; }
        public List<dsSaleInstrumentDetails> dtTbt_SaleInstrumentDetails { get; set; }
    }
}
