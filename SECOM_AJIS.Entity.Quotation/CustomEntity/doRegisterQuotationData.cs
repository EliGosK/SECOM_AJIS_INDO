using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class doRegisterQuotationData
    {
        public doQuotationHeaderData doQuotationHeaderData { get; set; }
        public tbt_QuotationBasic doTbt_QuotationBasic { get; set; }
        public tbt_QuotationInstallationDetail doTbt_QuotationInstallationDetail { get; set; }
        public List<tbt_QuotationOperationType> OperationList { get; set; }
        public List<tbt_QuotationInstrumentDetails> InstrumentList { get; set; }
        public List<tbt_QuotationFacilityDetails> FacilityList { get; set; }
        public tbt_QuotationBeatGuardDetails doTbt_QuotationBeatGuardDetails { get; set; }
        public List<tbt_QuotationSentryGuardDetails> SentryGuardList { get; set; }
        public List<tbt_QuotationMaintenanceLinkage> MaintenanceList { get; set; }
    }
}
