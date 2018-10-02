using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(tbt_QuotationFacilityDetails_MetaData))]
    public partial class tbt_QuotationFacilityDetails
    {

    }

    #region Meta Data

    public class tbt_QuotationFacilityDetails_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty]
        public string Alphabet { get; set; }
        [NotNullOrEmpty]
        public string FacilityCode { get; set; }
        [NotNullOrEmpty]
        public string FacilityQty { get; set; }
    }

    #endregion
}
