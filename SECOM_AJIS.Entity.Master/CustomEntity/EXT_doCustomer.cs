using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Master.MetaData;
using SECOM_AJIS.DataEntity.Master.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Master
{
    /// <summary>
    /// Do Of customer
    /// </summary>
    [MetadataType(typeof(doCustomer_MetaData))]
    public partial class doCustomer
    {
        public string CustCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertCustCode(this.CustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string SiteCustCode
        {
            get
            {
                if (this.CustCode != null)
                    return this.CustCode.Replace(CustomerCode.C_CUST_CODE_PREFIX, SiteCode.C_SITE_CODE_PREFIX);
                else
                    return null;
            }
        }
        public string SiteCustCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertSiteCode(this.SiteCustCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        [LanguageMapping]
        public string CustStatusName { get; set; }
        public string CustStatusCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.CustStatus, this.CustStatusName);
            }
        }
        [LanguageMapping]
        public string CustTypeName { get; set; }
        public string CustTypeCodeName
        {
            get
            {
                return CommonUtil.TextCodeName(this.CustTypeCode, this.CustTypeName);
            }
        }
        [LanguageMapping]
        public string CompanyTypeName { get; set; }
        [LanguageMapping]
        public string FinancialMaketTypeName { get; set; }
        [LanguageMapping]
        public string BusinessTypeName { get; set; }
        [LanguageMapping]
        public string Nationality { get; set; }

        public bool? ValidateCustomerData { get; set; }
        public string ToJson
        {
            get
            {
                return CommonUtil.CreateJsonString(this);
            }
        }

        public bool IsNameLCChanged { get; set; }
    }

    /// <summary>
    /// Do Of validate customer
    /// </summary>
    [MetadataType(typeof(ValidateCustomer_MetaData))]
    public class ValidateCustomer : doCustomer
    {
    }

    /// <summary>
    /// Do Of validate customer code null
    /// </summary>
    [MetadataType(typeof(ValidateCustomer_CodeNull_MetaData))]
    public class ValidateCustomer_CodeNull : doCustomer
    {
    }

    /// <summary>
    /// Do Of validate customer full
    /// </summary>
    [MetadataType(typeof(ValidateCustomer_Full_MetaData))]
    public class ValidateCustomer_Full : doCustomer
    {
    }

    /// <summary>
    /// Do Of customer with group
    /// </summary>
    public class doCustomerWithGroup : doCustomer
    {
        public List<dtCustomeGroupData> CustomerGroupData { get; set; }
    }

    /// <summary>
    /// Do Of customer address
    /// </summary>
    public class doCustomerAddress
    {
        [AddressFullMapping("AlleyPrefix", SetEachLanguage = true)]
        public string AlleyPrefixCode
        {
            get { return AddressFull.C_ADDRESS_ALLEY_PREFIX; }
        }
        [AddressFullMapping("AlleySuffix", SetEachLanguage = true)]
        public string AlleySuffixCode
        {
            get { return AddressFull.C_ADDRESS_ALLEY_SUFFIX; }
        }
        [AddressFullMapping("RoadPrefix", SetEachLanguage = true)]
        public string RoadPrefixCode
        {
            get { return AddressFull.C_ADDRESS_ROAD_PREFIX; }
        }
        [AddressFullMapping("RoadSuffix", SetEachLanguage = true)]
        public string RoadSuffixCode
        {
            get { return AddressFull.C_ADDRESS_ROAD_SUFFIX; }
        }
        [AddressFullMapping("SubDistrictPrefix", SetEachLanguage = true)]
        public string SubDistrictPrefixCode
        {
            get { return AddressFull.C_ADDRESS_SUB_DISTRICT_PREFIX; }
        }
        [AddressFullMapping("SubDistrictSuffix", SetEachLanguage = true)]
        public string SubDistrictSuffixCode
        {
            get { return AddressFull.C_ADDRESS_SUB_DISTRICT_SUFFIX; }
        }
        [AddressFullMapping("DistrictPrefix", SetEachLanguage = true)]
        public string DistrictPrefixCode
        {
            get { return AddressFull.C_ADDRESS_DISTRICT_PREFIX; }
        }
        [AddressFullMapping("DistrictSuffix", SetEachLanguage = true)]
        public string DistrictSuffixCode
        {
            get { return AddressFull.C_ADDRESS_DISTRICT_SUFFIX; }
        }
        [AddressFullMapping("ProvincePrefix", SetEachLanguage = true)]
        public string ProvincePrefixCode
        {
            get { return AddressFull.C_ADDRESS_PROVINCE_PREFIX; }
        }
        [AddressFullMapping("ProvinceSuffix", SetEachLanguage = true)]
        public string ProvinceSuffixCode
        {
            get { return AddressFull.C_ADDRESS_PROVINCE_SUFFIX; }
        }

        public string AlleyPrefixEN { get; set; }
        public string AlleyPrefixLC { get; set; }
        public string AlleySuffixEN { get; set; }
        public string AlleySuffixLC { get; set; }
        public string RoadPrefixEN { get; set; }
        public string RoadPrefixLC { get; set; }
        public string RoadSuffixEN { get; set; }
        public string RoadSuffixLC { get; set; }
        public string SubDistrictPrefixEN { get; set; }
        public string SubDistrictPrefixLC { get; set; }
        public string SubDistrictSuffixEN { get; set; }
        public string SubDistrictSuffixLC { get; set; }
        public string DistrictPrefixEN { get; set; }
        public string DistrictPrefixLC { get; set; }
        public string DistrictSuffixEN { get; set; }
        public string DistrictSuffixLC { get; set; }
        public string ProvincePrefixEN { get; set; }
        public string ProvincePrefixLC { get; set; }
        public string ProvinceSuffixEN { get; set; }
        public string ProvinceSuffixLC { get; set; }
    }
}
namespace SECOM_AJIS.DataEntity.Master.CustomAttribute
{
    /// <summary>
    /// Do Of id no not null or empty
    /// </summary>
    public class IDNoNotNullOrEmptyAttribute : NotNullOrEmptyAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value))
            {
                doCustomer mo = validationContext.ObjectInstance as doCustomer;
                if (mo != null)
                {
                    if (mo.DummyIDFlag == true)
                        return null;
                }

                return base.IsValid(value, validationContext);
            }
            return null;
        }
    }
}
namespace SECOM_AJIS.DataEntity.Master.MetaData
{
    /// <summary>
    /// Do Of customer meta data
    /// </summary>
    public class doCustomer_MetaData
    {
        [CustTypeMapping("CustTypeName")]
        public string CustTypeCode { get; set; }
        [FinancialMarketTypeMapping("FinancialMaketTypeName")]
        public string FinancialMarketTypeCode { get; set; }
    }

    /// <summary>
    /// Do Of validate customer meta data
    /// </summary>
    public class ValidateCustomer_MetaData
    {
        [NotNullOrEmpty]
        [CodeHasValue("CustTypeName")]
        public string CustTypeCode { get; set; }
        [NotNullOrEmpty]
        public string CustNameEN { get; set; }
        //[NotNullOrEmpty]
        public string CustNameLC { get; set; }

        [CodeHasValue("CompanyTypeName")]
        public string CompanyTypeCode { get; set; }
        [CodeHasValue("FinancialMaketTypeName")]
        public string FinancialMarketTypeCode { get; set; }
        [CodeHasValue("Nationality")]
        public string RegionCode { get; set; }
        [CodeHasValue("BusinessTypeName")]
        public string BusinessTypeCode { get; set; }
        [CodeNotNullOtherNotNull("ProvinceCode")]
        public string ProvinceNameEN { get; set; }
        [CodeNotNullOtherNotNull("ProvinceCode")]
        public string ProvinceNameLC { get; set; }
        [CodeNotNullOtherNotNull("DistrictCode")]
        public string DistrictNameEN { get; set; }
        [CodeNotNullOtherNotNull("DistrictCode")]
        public string DistrictNameLC { get; set; }
    }

    /// <summary>
    /// Do Of validate customer code null
    /// </summary>
    public class ValidateCustomer_CodeNull_MetaData
    {
        [NotNullOrEmpty]
        [CodeHasValue("CustTypeName")]
        public string CustTypeCode { get; set; }
        [NotNullOrEmpty]
        public string CustNameEN { get; set; }
        //[NotNullOrEmpty]
        public string CustNameLC { get; set; }

        [CodeHasValue("CompanyTypeName")]
        public string CompanyTypeCode { get; set; }
        [CodeHasValue("FinancialMaketTypeName")]
        public string FinancialMarketTypeCode { get; set; }

        [NotNullOrEmpty]
        [CodeHasValue("Nationality")]
        public string RegionCode { get; set; }

        [CodeHasValue("BusinessTypeName")]
        public string BusinessTypeCode { get; set; }
        [CodeNotNullOtherNotNull("ProvinceCode")]
        public string ProvinceNameEN { get; set; }
        [CodeNotNullOtherNotNull("ProvinceCode")]
        public string ProvinceNameLC { get; set; }
        [CodeNotNullOtherNotNull("DistrictCode")]
        public string DistrictNameEN { get; set; }
        [CodeNotNullOtherNotNull("DistrictCode")]
        public string DistrictNameLC { get; set; }
    }

    /// <summary>
    /// Do Of validate customer full meta data
    /// </summary>
    public class ValidateCustomer_Full_MetaData
    {
        [NotNullOrEmpty]
        [CodeHasValue("CustTypeName")]
        public string CustTypeCode { get; set; }
        [IDNoNotNullOrEmpty]
        public string IDNo { get; set; }
        [NotNullOrEmpty]
        public string CustNameEN { get; set; }
        //[NotNullOrEmpty]
        public string CustNameLC { get; set; }

        [CodeHasValue("CompanyTypeName")]
        public string CompanyTypeCode { get; set; }
        [CodeHasValue("FinancialMaketTypeName")]
        public string FinancialMarketTypeCode { get; set; }
        [NotNullOrEmpty]
        [CodeHasValue("Nationality")]
        public string RegionCode { get; set; }
        [NotNullOrEmpty]
        [CodeHasValue("BusinessTypeName")]
        public string BusinessTypeCode { get; set; }
        [NotNullOrEmpty]
        public string AddressEN { get; set; }
        // 2017.02.15 delete matsuda start
        //[NotNullOrEmpty]
        //public string AddressLC { get; set; }
        // 2017.02.15 delete matsuda end
        //[NotNullOrEmpty]
        //public string RoadEN { get; set; }
        //[NotNullOrEmpty]
        //public string RoadLC { get; set; }
        [NotNullOrEmpty]
        public string SubDistrictEN { get; set; }
        // 2017.02.15 delete matsuda start
        //[NotNullOrEmpty]
        //public string SubDistrictLC { get; set; }
        // 2017.02.15 delete matsuda end
        [NotNullOrEmpty]
        [CodeNotNullOtherNotNull("ProvinceCode")]
        public string ProvinceNameEN { get; set; }
        [NotNullOrEmpty]
        [CodeNotNullOtherNotNull("ProvinceCode")]
        public string ProvinceNameLC { get; set; }
        [NotNullOrEmpty]
        [CodeNotNullOtherNotNull("DistrictCode")]
        public string DistrictNameEN { get; set; }
        [NotNullOrEmpty]
        [CodeNotNullOtherNotNull("DistrictCode")]
        public string DistrictNameLC { get; set; }
    }
}
