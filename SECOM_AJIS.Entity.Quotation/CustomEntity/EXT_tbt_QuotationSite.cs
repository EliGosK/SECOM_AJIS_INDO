using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Quotation.MetaData;

namespace SECOM_AJIS.DataEntity.Quotation
{
    [MetadataType(typeof(tbt_QuotationSite_MetaData))]
    public partial class tbt_QuotationSite
    {
    }

    #region Meta Data
}
namespace SECOM_AJIS.DataEntity.Quotation.MetaData
{

    public class tbt_QuotationSite_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
        
        [CodeNotNullOtherNull("SiteCode")]
        [MaxTextLength(200)]
        public string SiteNameEN
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("SiteCode")]
        [MaxTextLength(200)]
        public string SiteNameLC
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("SiteCode")]
        [MaxTextLength(255)]
        public string AddressEN
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("SiteCode")]
        [MaxTextLength(255)]
        public string AddressLC
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("SiteCode")]
        [MaxTextLength(50)]
        public string RoadEN
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("SiteCode")]
        [MaxTextLength(50)]
        public string RoadLC
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("SiteCode")]
        [MaxTextLength(50)]
        public string SubDistrictLC
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("SiteCode")]
        [MaxTextLength(50)]
        public string SubDistrictEN
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("SiteCode")]
        public string BuildingUsageCode
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("SiteCode")]
        public string ProvinceCode
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("SiteCode")]
        public string DistrictCode
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("SiteCode")]
        public string AddressFullLC
        {
            get;
            set;
        }
        [CodeNotNullOtherNull("SiteCode")]
        public string AddressFullEN
        {
            get;
            set;
        }

        [MaxTextLength(100)]
        public string SECOMContactPerson { get; set; }
        [MaxTextLength(100)]
        public string PersonInCharge { get; set; }
        [MaxTextLength(50)]
        public string PhoneNo { get; set; }
        [MaxTextLength(50)]
        public string AlleyEN { get; set; }
        [MaxTextLength(50)]
        public string AlleyLC { get; set; }
        [MaxTextLength(10)]
        public string ZipCode { get; set; }
    }

    #endregion
}
