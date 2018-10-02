using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class doQuotationHeaderData
    {
        public doQuotationTarget doQuotationTarget { get; set; }
        public doQuotationCustomer doContractTarget { get; set; }
        public doQuotationCustomer doRealCustomer { get; set; }
        public doQuotationSite doQuotationSite { get; set; }
        public tbt_QuotationInstallationDetail doQuotationInstallationDetail { get; set; }
    }
}
