using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation.MetaData;

namespace SECOM_AJIS.DataEntity.Quotation
{
    public partial class tbt_QuotationInstrumentDetails
    {
    }

    [MetadataType(typeof(tbt_QuotaionInstrumentDetail_MetaData))]
    public class InsertQuotationInstrumentDetailsValidator : tbt_QuotationInstrumentDetails
    {
    }
}
namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    public class tbt_QuotaionInstrumentDetail_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty]
        public string Alphabet { get; set; }
        [NotNullOrEmpty]
        public string InstrumentCode { get; set; }
        [NotNullOrEmpty]
        public int? InstrumentQty { get; set; }
    }
}
