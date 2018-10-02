using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Presentation.Quotation.Models.MetaData;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Models;

namespace SECOM_AJIS.Presentation.Quotation.Models
{
    /// <summary>
    /// DO for initial screen
    /// </summary>
    public class QUS050_ScreenParameter : ScreenParameter
    {
        public dsImportData ImportData { get; set; }
    }
    /// <summary>
    /// DO for validating quotation target data
    /// </summary>
    [MetadataType(typeof(QUS050_tbt_QuotationTarget_MetaData))]
    public class QUS050_tbt_QuotationTarget : tbt_QuotationTarget
    {
    }
    /// <summary>
    /// DO for validating quotation target detail data
    /// </summary>
    [MetadataType(typeof(QUS050_tbt_QuotationTarget_D_MetaData))]
    public class QUS050_tbt_QuotationTarget_D : tbt_QuotationTarget
    {
    }
    /// <summary>
    /// DO for validating quotation basic data
    /// </summary>
    [MetadataType(typeof(QUS050_tbt_QuotationBasic_MetaData))]
    public class QUS050_tbt_QuotationBasic : tbt_QuotationBasic
    {
    }
    /// <summary>
    /// DO for validating customer data in case of beat guard
    /// </summary>
    [MetadataType(typeof(QUS050_tbt_QuotationCustomer_BC_MetaData))]
    public class QUS050_tbt_QuotationCustomer_BC : tbt_QuotationCustomer
    {
    }
    /// <summary>
    /// DO for validating site data in case of beat guard
    /// </summary>
    [MetadataType(typeof(QUS050_tbt_QuotationSite_BC_MetaDate))]
    public class QUS050_tbt_QuotationSite_BC : tbt_QuotationSite
    {
    }
}
namespace SECOM_AJIS.Presentation.Quotation.Models.MetaData
{
    /// <summary>
    /// Metadata for QUS050_tbt_QuotationTarget DO
    /// </summary>
    public class QUS050_tbt_QuotationTarget_MetaData
    {
        [NotNullOrEmpty]
        public string ProductTypeCode { get; set; }
        [NotNullOrEmpty]
        public string QuotationOfficeCode { get; set; }
    }
    /// <summary>
    /// Metadata for QUS050_tbt_QuotationTarget_D DO
    /// </summary>
    public class QUS050_tbt_QuotationTarget_D_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
    }
    /// <summary>
    /// Metadata for QUS050_tbt_QuotationBasic DO
    /// </summary>
    public class QUS050_tbt_QuotationBasic_MetaData
    {
        [NotNullOrEmpty]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty]
        public string ProductCode { get; set; }
    }
    /// <summary>
    /// Metadata for QUS050_tbt_QuotationCustomer_BC DO
    /// </summary>
    public class QUS050_tbt_QuotationCustomer_BC_MetaData
    {
        [CodeNotNullOtherNull("CustCode")]
        public string CustNameEN { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string CustNameLC { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string RepPersonName { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string ContactPersonName { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string SECOMContactPerson { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string CustTypeCode { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string CompanyTypeCode { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string FinancialMarketTypeCode { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string BusinessTypeCode { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string PhoneNo { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string IDNo { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string RegionCode { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string URL { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string AddressEN { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string AlleyEN { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string RoadEN { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string SubDistrictEN { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string AddressLC { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string AlleyLC { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string RoadLC { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string SubDistrictLC { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string DistrictCode { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string ProvinceCode { get; set; }
        [CodeNotNullOtherNull("CustCode")]
        public string ZipCode { get; set; }
    }
    /// <summary>
    /// Metadata for QUS050_tbt_QuotationSite_BC DO
    /// </summary>
    public class QUS050_tbt_QuotationSite_BC_MetaDate
    {
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string SiteNameEN { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string SiteNameLC { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string SECOMContactPerson { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string PersonInCharge { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string PhoneNo { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string BuildingUsageCode { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string AddressEN { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string AlleyEN { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string RoadEN { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string SubDistrictEN { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string AddressLC { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string AlleyLC { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string RoadLC { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string SubDistrictLC { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string DistrictCode { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string ProvinceCode { get; set; }
        [CodeNotNullOtherNull("SiteNo", Module = MessageUtil.MODULE_QUOTATION, MessageCode = MessageUtil.MessageList.MSG2022)]
        public string ZipCode { get; set; }
    }
}

