using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Contract
{
    public class doDraftRentalContractData
    {
        public enum RENTAL_CONTRACT_MODE
        {
            QUOTATION,
            DRAFT,
            APPROVE,
            OTHER
        }

        public tbt_DraftRentalContract doTbt_DraftRentalContrat { get; set; }
        public tbt_DraftRentalBEDetails doTbt_DraftRentalBEDetails { get; set; }
        public tbt_DraftRentalSentryGuard doTbt_DraftRentalSentryGuard { get; set; }
        public List<tbt_DraftRentalSentryGuardDetails> doTbt_DraftRentalSentryGuardDetails { get; set; }
        public List<tbt_DraftRentalBillingTarget> doTbt_DraftRentalBillingTarget { get; set; }
        public List<tbt_DraftRentalEmail> doTbt_DraftRentalEmail { get; set; }
        public List<tbt_DraftRentalInstrument> doTbt_DraftRentalInstrument { get; set; }
        public tbt_DraftRentalMaintenanceDetails doTbt_DraftRentalMaintenanceDetails { get; set; }
        public List<tbt_DraftRentalOperationType> doTbt_DraftRentalOperationType { get; set; }
        public List<tbt_RelationType> doTbt_RelationType { get; set; }
        public string QuotationNo { get; set; }

        public doCustomerWithGroup doContractCustomer { get; set; }
        public doCustomerWithGroup doRealCustomer { get; set; }
        public doSite doSite { get; set; }

        public RENTAL_CONTRACT_MODE Mode { get; set; }
        public DateTime LastUpdateDateQuotationData { get; set; }
    }
}
