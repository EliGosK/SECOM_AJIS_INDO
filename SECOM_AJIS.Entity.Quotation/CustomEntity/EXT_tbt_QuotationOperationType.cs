using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation.MetaData;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(tbt_QuotationOperationType_MetaData))]
    public partial class tbt_QuotationOperationType
    {
    }
}

namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    public class tbt_QuotationOperationType_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty]
        public string Alphabet { get; set; }
        [NotNullOrEmpty]
        public string OperationTypeCode { get; set; }

    }
}
