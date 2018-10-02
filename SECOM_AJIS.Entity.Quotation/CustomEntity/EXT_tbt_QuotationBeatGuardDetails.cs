using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(tbt_QuotationBeatGuardDetails_MetaData))]
    public partial class tbt_QuotationBeatGuardDetails
    {
    }

    public class tbt_QuotationBeatGuardDetails_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty]
        public string Alphabet { get; set; }
    }
}
