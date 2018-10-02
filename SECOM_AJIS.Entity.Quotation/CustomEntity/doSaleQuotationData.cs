using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Master;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public class doSaleQuotationData
    {
        public doQuotationHeaderData doQuotationHeaderData{get;set;}
        public tbt_QuotationBasic dtTbt_QuotationBasic {get;set;}
        public List<doInstrumentDetail> InstrumentDetailList { get; set; }
    }
}
