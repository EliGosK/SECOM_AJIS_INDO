using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation.MetaData;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(tbt_QuotationCustomer_MetaData))]
    public partial class tbt_QuotationCustomer
    {
    }
}
namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{
    #region Meta Data

    public class tbt_QuotationCustomer_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty]
        public string CustPartTypeCode { get; set; }

        [CodeNotNullOtherNull("CustCode")]
        [MaxTextLength(100)]
        public string CustNameEN
        {
            get;
            set;
        }
        //[CodeNotNullOtherNull("CustCode")]
        [MaxTextLength(100)]
        public string CustNameLC
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("CustCode")]
        public string CustFullNameEN
        {
            get;
            set;
        }
        //[CodeNotNullOtherNull("CustCode")]
        public string CustFullNameLC
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("CustCode")]
        public string CustTypeCode
        {
            get;
            set;
        }

        [MaxTextLength(100)]
        public string RepPersonName { get; set; }
        [MaxTextLength(100)]
        public string ContactPersonName { get; set; }
        [MaxTextLength(100)]
        public string SECOMContactPerson { get; set; }
        [MaxTextLength(20)]
        public string PhoneNo { get; set; }
        [MaxTextLength(20)]
        public string FaxNo { get; set; }
        [MaxTextLength(20)]
        public string IDNo { get; set; }
        [MaxTextLength(100)]
        public string URL { get; set; }
        [MaxTextLength(255)]
        public string AddressEN { get; set; }
        [MaxTextLength(50)]
        public string AlleyEN { get; set; }
        [MaxTextLength(50)]
        public string RoadEN { get; set; }
        [MaxTextLength(50)]
        public string SubDistrictEN { get; set; }
        [MaxTextLength(255)]
        public string AddressLC { get; set; }
        [MaxTextLength(50)]
        public string AlleyLC { get; set; }
        [MaxTextLength(50)]
        public string RoadLC { get; set; }
        [MaxTextLength(50)]
        public string SubDistrictLC { get; set; }
        [MaxTextLength(10)]
        public string ZipCode { get; set; }
    }

    #endregion
}
