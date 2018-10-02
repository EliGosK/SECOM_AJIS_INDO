using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Contract;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class doRentalQuotationData
    {
        public doQuotationHeaderData doQuotationHeaderData { get; set; }
        public tbt_QuotationBasic dtTbt_QuotationBasic { get; set; }
        public doBeatGuardDetail doBeatGuardDetail { get; set; }
        public bool FirstInstallCompleteFlag { get; set; }
        public doLinkageSaleContractData doLinkageSaleContractData { get; set; }

        /* --- Grid Data --- */
        public List<doInstrumentDetail> InstrumentDetailList { get; set; }
        public List<doFacilityDetail> FacilityDetailList { get; set; }
        public List<doSentryGuardDetail> SentryGuardDetailList { get; set; }
        public List<doContractHeader> MaintenanceTargetList { get; set; }

        public List<doQuotationOperationType> OperationTypeList { get; set; }
    }
}
