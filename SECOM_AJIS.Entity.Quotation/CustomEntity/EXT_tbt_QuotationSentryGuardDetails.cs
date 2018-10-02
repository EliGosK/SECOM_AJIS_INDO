using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation.MetaData;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(tbt_QuotationSentryGuardDetails_MetaData))]
    public partial class tbt_QuotationSentryGuardDetails
    {
    }

    
}
namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    public class tbt_QuotationSentryGuardDetails_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty]
        public string Alphabet { get; set; }
    }
}
