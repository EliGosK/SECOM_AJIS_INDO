using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class doInitImportData
    {
        public doCustomer doContractTargetData {get;set;}
        public doCustomer doRealCustomerData { get; set; }
        public doSite doQuotationSiteData { get; set; }
        public tbt_QuotationTarget doQuotationTargetData { get; set; }

        public string ToJson
        {
            get
            {
                return CommonUtil.CreateJsonString(this);
            }
        }
    }
}
