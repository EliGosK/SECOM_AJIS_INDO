using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class dsQuotationData
    {
        public tbt_QuotationTarget dtTbt_QuotationTarget {get;set;}
        public List<tbt_QuotationCustomer> dtTbt_QuotationCustomer {get;set;}
        public tbt_QuotationSite dtTbt_QuotationSite {get;set;}
        public tbt_QuotationBasic dtTbt_QuotationBasic {get;set;}
        public List<tbt_QuotationOperationType> dtTbt_QuotationOperationType {get;set;}
        public List<tbt_QuotationInstrumentDetails> dtTbt_QuotationInstrumentDetails {get;set;}
        public List<tbt_QuotationFacilityDetails> dtTbt_QuotationFacilityDetails {get;set;}
        public tbt_QuotationBeatGuardDetails dtTbt_QuotationBeatGuardDetails {get;set;}
        public List<tbt_QuotationSentryGuardDetails> dtTbt_QuotationSentryGuardDetails {get;set;}
        public List<tbt_QuotationMaintenanceLinkage> dtTbt_QuotationMaintenanceLinkage { get; set; }
    }
}
