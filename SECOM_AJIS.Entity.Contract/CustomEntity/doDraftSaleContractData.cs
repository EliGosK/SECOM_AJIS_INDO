using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doDraftSaleContractData
    {
        public enum SALE_CONTRACT_MODE
        {
            QUOTATION,
            DRAFT,
            APPROVE
        }
        public enum PROCESS_TYPE
        {
            NEW = 1,
            ADD
        }

        public tbt_DraftSaleContract doTbt_DraftSaleContract { get; set; }
        public List<tbt_DraftSaleInstrument> doTbt_DraftSaleInstrument { get; set; }
        public List<tbt_DraftSaleEmail> doTbt_DraftSaleEmail { get; set; }
        public List<tbt_DraftSaleBillingTarget> doTbt_DraftSaleBillingTarget { get; set; }
        public List<tbt_RelationType> doTbt_RelationType { get; set; }

        public doCustomerWithGroup doPurchaserCustomer { get; set; }
        public doCustomerWithGroup doRealCustomer { get; set; }
        public doSite doSite { get; set; }

        public SALE_CONTRACT_MODE Mode { get; set; }
        public DateTime LastUpdateDateQuotationData { get; set; }
    }
}
