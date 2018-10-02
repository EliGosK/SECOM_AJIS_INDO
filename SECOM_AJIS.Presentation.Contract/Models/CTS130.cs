using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of CTS130 screen
    /// </summary>
    public class CTS130_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public string ContractCode { get; set; }

        [KeepSession]
        public string LastOCC { get; set; }

        [KeepSession]
        public string ServiceTypeCode { get; set; }

        public dsRentalContractData RentalContractData { get; set; }
        public dsSaleContractData SaleContractData { get; set; }

        public List<doBillingTargetDetail> doBillingTargetDetailData { get; set; }
        public doRentalContractBasicInformation doRentalContractBasicInfo { get; set; }
        public doSaleContractBasicInformation doSaleContractBasicInfo { get; set; }

        public List<doCustomerWithGroup> ContractTargetPurchaserData { get; set; }
        public List<doCustomerWithGroup> RealCustomerData { get; set; }
        public List<doSite> SiteData { get; set; }

        public List<doCustomerWithGroup> ContractTargetPurchaserDataTemp { get; set; }
        public List<doCustomerWithGroup> RealCustomerDataTemp { get; set; }
        public List<doSite> SiteDataTemp { get; set; }

        public List<CTS110_BillingClientData> BillingClientOriginalData { get; set; }
        public List<CTS110_BillingTargetData> BillingTargetOriginalData { get; set; }
        public List<CTS110_BillingClientData> BillingClientData { get; set; }
        public List<CTS110_BillingTargetData> BillingTargetData { get; set; }
        public CTS110_BillingClientData BillingClientDataTemp { get; set; }
        public CTS110_BillingTargetData BillingTargetDataTemp { get; set; }
        public string SequenceTemp { get; set; }
    }

    /// <summary>
    /// Parameter of Site Condition
    /// </summary>
    public class CTS130_SiteCondition
    {
        public string RealCustomerCode { get; set; }
        public string SiteCustCode { get; set; }
        public string SiteNo { get; set; }
    }

    /// <summary>
    /// Parameter of copy SiteName Condition
    /// </summary>
    public class CTS130_CopySiteNameCondition
    {
        public string CopyType { get; set; }
        public bool BranchContractFlag { get; set; }
        public string BranchNameEN { get; set; }
        public string BranchNameLC { get; set; }
    }

    /// <summary>
    /// Parameter of copy BillingName Condition
    /// </summary>
    public class CTS130_CopyBillingNameCondition
    {
        public string CopyType { get; set; }
        public string PurchaserCustCode { get; set; }
        public string RealCustCode { get; set; }
        public string SiteCode { get; set; }
        public string BranchNameEN { get; set; }
        public string BranchNameLC { get; set; }
        public string BranchAddressEN { get; set; }
        public string BranchAddressLC { get; set; }
    }

    /// <summary>
    /// Parameter for Register ChangeName and Address
    /// </summary>
    public class CTS130_RegisterChangeNameAddressData
    {
        public string ChangeNameReasonType { get; set; }
        public bool BranchContractFlag { get; set; }
        public string BranchNameEN { get; set; }
        public string BranchNameLC { get; set; }
        public string BranchAddressEN { get; set; }
        public string BranchAddressLC { get; set; }
        public string ContractTargetSignerTypeCode { get; set; }
        public string ContactPoint { get; set; }
        public bool IsShowBillingTagetDetail { get; set; }
    }

    /// <summary>
    /// DO for validate NameReasonType
    /// </summary>
    [MetadataType(typeof(CTS130_ValidateNameReasonType_MetaData))]
    public class CTS130_ValidateNameReasonType
    {
        public string ChangeNameReasonType { get; set; }
    }

    /// <summary>
    /// DO for validate PurchaserCustCode
    /// </summary>
    [MetadataType(typeof(CTS130_ValidatePurchaserCustCode_MetaData))]
    public class CTS130_ValidatePurchaserCustCode : doCustomerWithGroup
    {

    }

    /// <summary>
    /// DO for validate PurchaserCustName
    /// </summary>
    [MetadataType(typeof(CTS130_ValidatePurchaserCustName_MetaData))]
    public class CTS130_ValidatePurchaserCustName : doCustomerWithGroup
    {

    }

    /// <summary>
    /// DO for validate ContractTargetSignerTypeCode
    /// </summary>
    [MetadataType(typeof(CTS130_ValidateContractTargetSignerTypeCode_MetaData))]
    public class CTS130_ValidateContractTargetSignerTypeCode : tbt_RentalContractBasic
    {

    }

    /// <summary>
    /// DO for validate PurchaserBranch
    /// </summary>
    [MetadataType(typeof(CTS130_ValidatePurchaserBranch_MetaData))]
    public class CTS130_ValidatePurchaserBranch
    {
        public string BranchNameEN { get; set; }
        public string BranchNameLC { get; set; }
        public string BranchAddressEN { get; set; }
        public string BranchAddressLC { get; set; }
    }

    /// <summary>
    /// DO for validate RealCustomer CustCode
    /// </summary>
    [MetadataType(typeof(CTS130_ValidateRealCustomerCustCode_MetaData))]
    public class CTS130_ValidateRealCustomerCustCode : doCustomerWithGroup
    {

    }

    /// <summary>
    /// DO for validate RealCustomer CustName
    /// </summary>
    [MetadataType(typeof(CTS130_ValidateRealCustomerCustName_MetaData))]
    public class CTS130_ValidateRealCustomerCustName : doCustomerWithGroup
    {

    }

    /// <summary>
    /// DO for validate SiteCode
    /// </summary>
    [MetadataType(typeof(CTS130_ValidateSiteCode_MetaData))]
    public class CTS130_ValidateSiteCode : doSite
    {

    }

    /// <summary>
    /// DO for validate SiteName
    /// </summary>
    [MetadataType(typeof(CTS130_ValidateSiteName_MetaData))]
    public class CTS130_ValidateSiteName : doSite
    {

    }

    /// <summary>
    /// DO for validate BillingTemp
    /// </summary>
    [MetadataType(typeof(CTS130_ValidateBillingTempData_MetaData))]
    public class CTS130_ValidateBillingTempData : tbt_BillingTemp
    {

    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS130_ValidateNameReasonType_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblNameAddressChangeReason",
                ControlName = "ChangeReasonType")]
        public string ChangeNameReasonType { get; set; }
    }

    public class CTS130_ValidatePurchaserCustCode_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblCustomerCode",
                ControlName = "PC_CustomerCode")]
        public string CustCode { get; set; }
    }

    public class CTS130_ValidatePurchaserCustName_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblNameEnglish",
                ControlName = "PC_NameEnglish")]
        public string CustNameEN { get; set; }

        // 2017.03.09 delete matsuda start
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //        Screen = "CTS130",
        //        Parameter = "lblNameLocal",
        //        ControlName = "PC_NameLocal")]
        //public string CustNameLC { get; set; }
        // 2017.03.09 delete matsuda end

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblCustomerType",
                ControlName = "PC_CustomerType")]
        public string CustTypeCode { get; set; }
    }

    public class CTS130_ValidateContractTargetSignerTypeCode_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblContractSignerType",
                ControlName = "PC_ContractSignerType")]
        public string ContractTargetSignerTypeCode { get; set; }
    }

    public class CTS130_ValidatePurchaserBranch_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblBranchNameEnglish",
                ControlName = "PC_BranchNameEnglish")]
        public string BranchNameEN { get; set; }

        // 2017.03.09 delete matsuda start
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //        Screen = "CTS130",
        //        Parameter = "lblBranchNameLocal",
        //        ControlName = "PC_BranchNameLocal")]
        //public string BranchNameLC { get; set; }
        // 2017.03.09 delete matsuda end

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblBranchAddressEnglish",
                ControlName = "PC_BranchAddressEnglish")]
        public string BranchAddressEN { get; set; }

        // 2017.03.09 delete matsuda start
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //        Screen = "CTS130",
        //        Parameter = "lblBranchAddressLocal",
        //        ControlName = "PC_BranchAddressLocal")]
        //public string BranchAddressLC { get; set; }
        // 2017.03.09 delete matsuda end
    }

    public class CTS130_ValidateRealCustomerCustCode_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblCustomerCode",
                ControlName = "RC_CustomerCode")]
        public string CustCode { get; set; }
    }

    public class CTS130_ValidateRealCustomerCustName_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblNameEnglish",
                ControlName = "RC_NameEnglish")]
        public string CustNameEN { get; set; }

        // 2017.03.09 delete matsuda start
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //        Screen = "CTS130",
        //        Parameter = "lblNameLocal",
        //        ControlName = "RC_NameLocal")]
        //public string CustNameLC { get; set; }
        // 2017.03.09 delete matsuda end

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblCustomerType",
                ControlName = "RC_CustomerType")]
        public string CustTypeCode { get; set; }
    }

    public class CTS130_ValidateSiteCode_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblSiteCode",
                ControlName = "ST_SiteCode")]
        public string SiteCode { get; set; }
    }

    public class CTS130_ValidateSiteName_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblNameEnglish",
                ControlName = "ST_NameEnglish")]
        public string SiteNameEN { get; set; }

        // 2017.03.09 delete matsuda start
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //        Screen = "CTS130",
        //        Parameter = "lblNameLocal",
        //        ControlName = "ST_NameLocal")]
        //public string SiteNameLC { get; set; }
        // 2017.03.09 delete matsuda end

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblAddressEnglish",
                ControlName = "ST_AddressEnglish")]
        public string AddressEN { get; set; }

        // 2017.03.09 delete matsuda start
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //        Screen = "CTS130",
        //        Parameter = "lblAddressLocal",
        //        ControlName = "ST_AddressLocal")]
        //public string AddressLC { get; set; }
        // 2017.03.09 delete matsuda end

        //[NotNullOrEmpty]
        //public string RoadEN { get; set; }

        //[NotNullOrEmpty]
        //public string RoadLC { get; set; }

        [NotNullOrEmpty]
        public string SubDistrictEN { get; set; }

        // 2017.03.09 delete matsuda start
        //[NotNullOrEmpty]
        //public string SubDistrictLC { get; set; }
        // 2017.03.09 delete matsuda end

        [NotNullOrEmpty]
        public string BuildingUsageCode { get; set; }

        [NotNullOrEmpty]
        public string ProvinceCode { get; set; }

        [NotNullOrEmpty]
        public string DistrictCode { get; set; }
    }

    public class CTS130_ValidateBillingTempData_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS130",
                Parameter = "lblBillingOffice",
                ControlName = "BT_BillingOffice")]
        public string BillingOfficeCode { get; set; }
    }
}