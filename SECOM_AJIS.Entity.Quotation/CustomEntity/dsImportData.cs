using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class dsImportData : ScreenParameter
    {
        public string QuotationTargetCode { get; set; }

        public List<tbt_QuotationCustomer> dtTbt_QuotationCustomer { get; set; }
        public List<tbt_QuotationSite> dtTbt_QuotationSite { get; set; }
        public List<tbt_QuotationTarget> dtTbt_QuotationTarget { get; set; }
        public List<tbt_QuotationBasic> dtTbt_QuotationBasic { get; set; }
        public List<tbt_QuotationOperationType> dtTbt_QuotationOperationType { get; set; }
        public List<tbt_QuotationInstrumentDetails> dtTbt_QuotationInstrumentDetails { get; set; }
        public List<tbt_QuotationFacilityDetails> dtTbt_QuotationFacilityDetails { get; set; }
        public List<tbt_QuotationBeatGuardDetails> dtTbt_QuotationBeatGuardDetails { get; set; }
        public List<tbt_QuotationSentryGuardDetails> dtTbt_QuotationSentryGuardDetails { get; set; }
        public List<tbt_QuotationMaintenanceLinkage> dtTbt_QuotationMaintenanceLinkage { get; set; }

        public string ToJson
        {
            get
            {
                return SECOM_AJIS.Common.Util.CommonUtil.CreateJsonString(this);
            }
        }
    }
}
