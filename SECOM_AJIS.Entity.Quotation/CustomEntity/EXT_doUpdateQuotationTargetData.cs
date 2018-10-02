using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation.MetaData;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(doUpdateQuotationTargetData_MetaData))]
    public partial class doUpdateQuotationTargetData
    {
    }
}
namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    public class doUpdateQuotationTargetData_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }

    }
}
