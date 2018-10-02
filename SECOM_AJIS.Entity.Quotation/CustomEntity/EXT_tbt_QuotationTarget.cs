using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation.MetaData;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(tbt_QuotationTarget_MetaData))]
    public partial class tbt_QuotationTarget
    {
        public string ProductTypeName { get; set; }
        public string AcquisitionTypeName { get; set; }
        public string MotivationTypeName { get; set; }
    }

    #region Meta Data
}
namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    public class tbt_QuotationTarget_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty]
        public string ProductTypeCode { get; set; }
        [NotNullOrEmpty]
        public string QuotationOfficeCode { get; set; }
        [NotNullOrEmpty]
        public string OperationOfficeCode { get; set; }

        [MaxTextLength(11)]
        public string IntroducerCode { get; set; }
        [MaxTextLength(100)]
        public string BranchNameEN { get; set; }
        [MaxTextLength(100)]
        public string BranchNameLC { get; set; }
        [MaxTextLength(250)]
        public string BranchAddressEN { get; set; }
        [MaxTextLength(250)]
        public string BranchAddressLC { get; set; }
        [MaxTextLength(300)]
        public string ContractTargetMemo { get; set; }
        [MaxTextLength(300)]
        public string RealCustomerMemo { get; set; }
    }

    #endregion
}
