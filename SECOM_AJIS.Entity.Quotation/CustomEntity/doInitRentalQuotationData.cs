using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class doInitRentalQuotationData
    {
        public tbt_QuotationBasic doQuotationBasic { get; set; }
        public List<doQuotationOperationType> OperationTypeList { get; set; }
        public List<doInstrumentDetail> InstrumentDetailList { get; set; }
        public List<doFacilityDetail> FacilityDetailList { get; set; }
        public doLinkageSaleContractData doLinkageSaleContractData { get; set; }
        public doBeatGuardDetail doBeatGuardDetail { get; set; }
        public List<doContractHeader> MaintenanceTargetList { get; set; }
        public List<doSentryGuardDetail> SentryGuardDetailList { get; set; }
    }
}
