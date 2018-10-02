using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util;
using System.Reflection;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Presentation.Quotation.Models.CustomAttribute;
using SECOM_AJIS.Presentation.Quotation.Models.MetaData;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.Presentation.Quotation.Models
{
    /// <summary>
    /// DO for initial screen
    /// </summary>
    public class QUS020_ScreenParameter : ScreenParameter
    {
        public QUS020_InitialQuotationTargetData InitialData { get; set; }
        public doRegisterQuotationTargetData RegisterData { get; set; }

        public string ImportKey { get; set; }
        public bool LoadImportSessionToDetail { get; set; }
    }
    /// <summary>
    /// DO for initial quotation target data
    /// </summary>
    public class QUS020_InitialQuotationTargetData
    {
        public enum INITIAL_OBJECT
        {
            INITIAL_DATA = 0,
            CONTRACT_TARGET_DATA,
            REAL_CUSTOMER_DATA,
            QUOTATION_SITE_DATA
        }

        public bool IsBranchChecked { get; set; }

        public INITIAL_OBJECT ObjectType
        {
            get
            {
                return (INITIAL_OBJECT)Enum.ToObject(typeof(INITIAL_OBJECT), this.ObjectTypeID);
            }
            set
            {
                this.ObjectTypeID = (int)value;
            }
        }
        public int ObjectTypeID { get; set; }

        public doCustomer doContractTargetData { get; set; }
        public doCustomer doRealCustomerData { get; set; }
        public doSite doQuotationSiteData { get; set; }

        public string BranchNameEN { get; set; }
        public string BranchAddressEN { get; set; }
        public string BranchNameLC { get; set; }
        public string BranchAddressLC { get; set; }
        public string ProductTypeCode { get; set; }
        public string QuotationOfficeCode { get; set; }
        public string OperationOfficeCode { get; set; }
        public string AcquisitionTypeCode { get; set; }
        public string IntroducerCode { get; set; }
        public string MotivationTypeCode { get; set; }
        public string OldContractCode { get; set; }
        public string QuotationStaffEmpNo { get; set; }
        
        public string ContractTargetMemo { get; set; }
        public string RealCustomerMemo { get; set; }

        public void SetObjectData(QUS020_InitialQuotationTargetData initialData)
        {
            if (initialData.ObjectType == INITIAL_OBJECT.CONTRACT_TARGET_DATA)
                this.doContractTargetData = initialData.doContractTargetData;
            else if (initialData.ObjectType == INITIAL_OBJECT.REAL_CUSTOMER_DATA)
            {
                if (this.doRealCustomerData != null)
                {
                    if (CommonUtil.IsNullOrEmpty(this.doRealCustomerData.CustCode) == false
                        && this.doQuotationSiteData != null)
                    {
                        if (CommonUtil.IsNullOrEmpty(this.doQuotationSiteData.SiteCode) == false)
                            this.doQuotationSiteData = null;
                    }
                }
                this.doRealCustomerData = initialData.doRealCustomerData;
            }
            else if (initialData.ObjectType == INITIAL_OBJECT.QUOTATION_SITE_DATA)
                this.doQuotationSiteData = initialData.doQuotationSiteData;
            else
            {
                this.doContractTargetData = initialData.doContractTargetData;
                this.doRealCustomerData = initialData.doRealCustomerData;
                this.doQuotationSiteData = initialData.doQuotationSiteData;
            }
        }
        public object GetObjectData(INITIAL_OBJECT ObjectType)
        {
            if (ObjectType == INITIAL_OBJECT.INITIAL_DATA)
                return this;
            else if (ObjectType == INITIAL_OBJECT.CONTRACT_TARGET_DATA)
                return this.doContractTargetData;
            else if (ObjectType == INITIAL_OBJECT.REAL_CUSTOMER_DATA)
                return this.doRealCustomerData;
            else if (ObjectType == INITIAL_OBJECT.QUOTATION_SITE_DATA)
                return this.doQuotationSiteData;
            return null;
        }
    }
    /// <summary>
    /// DO for getting customer data
    /// </summary>
    public class QUS020_RetrieveCustomerCondition
    {
        [QUS020_CustConditionNotNullOrEmpty(ControlName = "CustCode")]
        public string CustCode { get; set; }
        public int CustType { get; set; }
    }
    /// <summary>
    /// DO for getting site data
    /// </summary>
    public class QUS020_RetrieveSiteCondition
    {
        [NotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0051)]
        public string CustCode { get; set; }
        [NotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0072, ControlName = "SiteCustCode")]
        public string SiteCustCode { get; set; }
        [NotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0070, ControlName = "SiteCustCodeNo")]
        public string SiteNo { get; set; }
    }
    /// <summary>
    /// DO for validating customer data
    /// </summary>
    [MetadataType(typeof(QUS020_RegisterContractTarget_MetaData))]
    public class QUS020_doCustomer_ContractTarget : doCustomer
    {
    }
    /// <summary>
    /// DO for validating site data
    /// </summary>
    [MetadataType(typeof(QUS020_RegisterSite_MetaData))]
    public class QUS020_doSite : doSite
    {
    }

    #region Validate in case Register/Confirm

    /// <summary>
    /// DO for validating register quotation target data
    /// </summary>
    [MetadataType(typeof(QUS020_doRegisterQuotationTargetData_MetaData))]
    public class QUS020_doRegisterQuotationTargetData : doRegisterQuotationTargetData
    {
    }
    /// <summary>
    /// DO for validating quotation target data
    /// </summary>
    [MetadataType(typeof(QUS020_tbt_QuotationTarget_MetaData))]
    public class QUS020_tbt_QuotationTarget : tbt_QuotationTarget
    {
        public bool BranchContractFlag { get; set; }
    }

    #endregion
    #region Validate in case Import

    /// <summary>
    /// DO for validating the existence of data in combo
    /// </summary>
    [MetadataType(typeof(QUS020_tbt_QuotationTarget_InitCombo_MetaData))]
    public class QUS020_tbt_QuotationTarget_InitCombo : tbt_QuotationTarget
    {
    }

    #endregion
}
namespace SECOM_AJIS.Presentation.Quotation.Models.CustomAttribute
{
    /// <summary>
    /// Attribute for checking customer code is null or empty
    /// </summary>
    public class QUS020_CustConditionNotNullOrEmptyAttribute : NotNullOrEmptyAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value))
            {
                QUS020_RetrieveCustomerCondition cond = validationContext.ObjectInstance as QUS020_RetrieveCustomerCondition;
                if (cond != null)
                {
                    if (cond.CustType == 1)
                        this.MessageCode = MessageUtil.MessageList.MSG0067;
                    else if (cond.CustType == 2)
                        this.MessageCode = MessageUtil.MessageList.MSG0069;
                    else
                        this.MessageCode = MessageUtil.MessageList.MSG0051;
                }

                return base.IsValid(value, validationContext);
            }

            return null;
        }
    }
    /// <summary>
    /// Attribute for checking branch data are null or empty
    /// </summary>
    public class QUS020_BranchNotNullOrEmptyAttribute : NotNullOrEmptyAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value))
            {
                QUS020_tbt_QuotationTarget ncond = validationContext.ObjectInstance as QUS020_tbt_QuotationTarget;
                if (ncond != null)
                {
                    if (ncond.BranchContractFlag)
                        return base.IsValid(value, validationContext);
                }
            }

            return null;
        }
    }
    /// <summary>
    /// Attribute for checking introducer code is null or empty
    /// </summary>
    public class QUS020_InstroducerNotNullOrEmptyAttribute : NotNullOrEmptyAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value))
            {
                QUS020_InitialQuotationTargetData cond = validationContext.ObjectInstance as QUS020_InitialQuotationTargetData;
                if (cond != null)
                {
                    if (cond.AcquisitionTypeCode == SECOM_AJIS.Common.Util.ConstantValue.AcquisitionType.C_ACQUISITION_TYPE_CUST
                       || cond.AcquisitionTypeCode == SECOM_AJIS.Common.Util.ConstantValue.AcquisitionType.C_ACQUISITION_TYPE_INSIDE_COMPANY)
                        return base.IsValid(value, validationContext);
                }
                else
                {
                    tbt_QuotationTarget ncond = validationContext.ObjectInstance as tbt_QuotationTarget;
                    if (ncond.AcquisitionTypeCode == SECOM_AJIS.Common.Util.ConstantValue.AcquisitionType.C_ACQUISITION_TYPE_CUST
                        || ncond.AcquisitionTypeCode == SECOM_AJIS.Common.Util.ConstantValue.AcquisitionType.C_ACQUISITION_TYPE_INSIDE_COMPANY)
                        return base.IsValid(value, validationContext);
                }
            }

            return null;
        }
    }
}
namespace SECOM_AJIS.Presentation.Quotation.Models.MetaData
{
    /// <summary>
    /// Metadata for QUS020_RegisterContractTarget DO
    /// </summary>
    public class QUS020_RegisterContractTarget_MetaData
    {
        [CodeNullOtherNotNull("CustCode", 
                                Controller = MessageUtil.MODULE_QUOTATION,
                                Screen = "QUS020",
                                Order = 1,
                                Parameter = "lblContractTargetPorchaserInfo")]
        public string CustNameEN { get; set; }
        [CodeNullOtherNotNull("CustCode",
                                Controller = MessageUtil.MODULE_QUOTATION,
                                Screen = "QUS020",
                                Order = 1,
                                Parameter = "lblContractTargetPorchaserInfo")]
        public string CustNameLC { get; set; }
        //[CodeNullOtherNotNull("CustCode",
        //                        Controller = MessageUtil.MODULE_QUOTATION,
        //                        Screen = "QUS020",
        //                        Order = 1,
        //                        Parameter = "lblContractTargetPorchaserInfo")]
        public string CustFullNameEN { get; set; }
        [CodeNullOtherNotNull("CustCode", 
                                Controller = MessageUtil.MODULE_QUOTATION,
                                Screen = "QUS020",
                                Order = 1,
                                Parameter = "lblContractTargetPorchaserInfo")]
        public string CustFullNameLC { get; set; }
        //[CodeNullOtherNotNull("CustCode",
        //    Controller = MessageUtil.MODULE_QUOTATION,
        //                        Screen = "QUS020",
        //                        Order = 1,
        //                        Parameter = "lblContractTargetPorchaserInfo")]
        public string CustTypeCode { get; set; }
    }
    /// <summary>
    /// Metadata for QUS020_RegisterSite DO
    /// </summary>
    public class QUS020_RegisterSite_MetaData
    {
        [CodeNullOtherNotNull("SiteCode",
                                Controller = MessageUtil.MODULE_QUOTATION,
                                Screen = "QUS020",
                                Order = 6,
                                Parameter = "lblSiteInfo")]
        public string SiteNameEN { get; set; }
        [CodeNullOtherNotNull("SiteCode",
                                Controller = MessageUtil.MODULE_QUOTATION,
                                Screen = "QUS020",
                                Order = 6,
                                Parameter = "lblSiteInfo")]
        public string SiteNameLC { get; set; }
        [CodeNullOtherNotNull("SiteCode",
                                Controller = MessageUtil.MODULE_QUOTATION,
                                Screen = "QUS020",
                                Order = 6,
                                Parameter = "lblSiteInfo")]
        public string AddressEN { get; set; }
        [CodeNullOtherNotNull("SiteCode",
                                Controller = MessageUtil.MODULE_QUOTATION,
                                Screen = "QUS020",
                                Order = 6,
                                Parameter = "lblSiteInfo")]
        public string AddressLC { get; set; }
        //[CodeNullOtherNotNull("SiteCode",
        //                        Controller = MessageUtil.MODULE_QUOTATION,
        //                        Screen = "QUS020",
        //                        Order = 6,
        //                        Parameter = "lblSiteInfo")]
        //public string RoadEN { get; set; }
        //[CodeNullOtherNotNull("SiteCode",
        //                        Controller = MessageUtil.MODULE_QUOTATION,
        //                        Screen = "QUS020",
        //                        Order = 6,
        //                        Parameter = "lblSiteInfo")]
        //public string RoadLC { get; set; }
        [CodeNullOtherNotNull("SiteCode",
                                Controller = MessageUtil.MODULE_QUOTATION,
                                Screen = "QUS020",
                                Order = 6,
                                Parameter = "lblSiteInfo")]
        public string SubDistrictEN { get; set; }
        [CodeNullOtherNotNull("SiteCode",
                                Controller = MessageUtil.MODULE_QUOTATION,
                                Screen = "QUS020",
                                Parameter = "lblSiteInfo")]
        public string SubDistrictLC { get; set; }
        [CodeNullOtherNotNull("SiteCode",
                                Controller = MessageUtil.MODULE_QUOTATION,
                                Screen = "QUS020",
                                Order = 6,
                                Parameter = "lblSiteInfo")]
        public string BuildingUsageCode { get; set; }
        [CodeNullOtherNotNull("SiteCode",
                                Controller = MessageUtil.MODULE_QUOTATION,
                                Screen = "QUS020",
                                Order = 6,
                                Parameter = "lblSiteInfo")]
        public string ProvinceCode { get; set; }
        [CodeNullOtherNotNull("SiteCode",
                                Controller = MessageUtil.MODULE_QUOTATION,
                                Screen = "QUS020",
                                Order = 6,
                                Parameter = "lblSiteInfo")]
        public string DistrictCode { get; set; }
    }

    #region Validate in case Register/Confirm

    /// <summary>
    /// Metadata for QUS020_doRegisterQuotationTargetData DO
    /// </summary>
    public class QUS020_doRegisterQuotationTargetData_MetaData
    {
        [NotNullOrEmpty(
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS020",
                        Parameter = "lblContractTargetPorchaserInfo",
                        Order = 1)]
        public tbt_QuotationCustomer doTbt_QuotationCustomer1 { get; set; }
        [NotNullOrEmpty(
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS020",
                        Parameter = "lblSiteInfo",
                        Order = 6)]
        public tbt_QuotationSite doTbt_QuotationSite { get; set; }
    }
    /// <summary>
    /// Metadata for QUS020_tbt_QuotationTarget DO
    /// </summary>
    public class QUS020_tbt_QuotationTarget_MetaData
    {

        [QUS020_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS020",
                        Order = 2,
                        Parameter = "lblBranchNameEN",
                        ControlName = "BranchNameEN")]
        public string BranchNameEN { get; set; }
        [QUS020_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS020",
                        Order = 3,
                        Parameter = "lblBranchAddressEN",
                        ControlName = "BranchAddressEN")]
        public string BranchAddressEN { get; set; }
        [QUS020_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS020",
                        Order = 4,
                        Parameter = "lblBranchNameLC",
                        ControlName = "BranchNameLC")]
        public string BranchNameLC { get; set; }
        [QUS020_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS020",
                        Order = 5,
                        Parameter = "lblBranchAddressLC",
                        ControlName = "BranchAddressLC")]
        public string BranchAddressLC { get; set; }

        [NotNullOrEmpty(
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS020",
                        Parameter = "lblProductType",
                        Order = 7,
                        ControlName = "ProductTypeCode")]
        public string ProductTypeCode { get; set; }
        [NotNullOrEmpty(
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS020",
                        Parameter = "lblQuotationOffice",
                        Order = 8,
                        ControlName = "QuotationOfficeCode")]
        public string QuotationOfficeCode { get; set; }
        [NotNullOrEmpty(
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS020",
                        Parameter = "lblOperationOffice",
                        Order = 9,
                        ControlName = "OperationOfficeCode")]
        public string OperationOfficeCode { get; set; }
        [QUS020_InstroducerNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_QUOTATION,
                        Screen = "QUS020",
                        Order = 10,
                        Parameter = "lblIntroducerCode",
                        ControlName = "IntroducerCode")]
        public string IntroducerCode { get; set; }
    }

    #endregion
    #region Validate in case Import

    /// <summary>
    /// Metadata for QUS020_tbt_QuotationTarget_InitCombo DO
    /// </summary>
    public class QUS020_tbt_QuotationTarget_InitCombo_MetaData
    {
        [RelateObject("ProductTypeCode",
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS020",
            Parameter = "lblProductType",
            Order = 1,
            ControlName = "ProductTypeCode", 
            Module = MessageUtil.MODULE_COMMON, 
            MessageCode = MessageUtil.MessageList.MSG0066)]
        public string ProductTypeName { get; set; }
        [QuotationOfficeInRoleAttribute(
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS020",
            Parameter = "lblQuotationOffice",
            Order = 2,
            ControlName = "QuotationOfficeCode",
            Module = MessageUtil.MODULE_COMMON, 
            MessageCode = MessageUtil.MessageList.MSG0066)]
        public string QuotationOfficeCode { get; set; }
        [OperationOfficeInRoleAttribute(
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS020",
            Parameter = "lblOperationOffice",
            Order = 3,
            ControlName = "OperationOfficeCode",
            Module = MessageUtil.MODULE_COMMON,
            MessageCode = MessageUtil.MessageList.MSG0066)]
        public string OperationOfficeCode { get; set; }
        [AcquisitionTypeMapping("AcquisitionTypeName")]
        public string AcquisitionTypeCode { get; set; }
        [RelateObject("AcquisitionTypeCode",
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS020",
            Parameter = "lblAcquisitionType",
            Order = 4,
            ControlName = "AcquisitionTypeCode",
            Module = MessageUtil.MODULE_COMMON,
            MessageCode = MessageUtil.MessageList.MSG0066)]
        public string AcquisitionTypeName { get; set; }
        [MotivationTypeMapping("MotivationTypeName")]
        public string MotivationTypeCode { get; set; }
        [RelateObject("MotivationTypeCode",
            Controller = MessageUtil.MODULE_QUOTATION,
            Screen = "QUS020",
            Parameter = "lblPurchaseReasonMotivationType",
            Order = 5,
            ControlName = "MotivationTypeCode",
            Module = MessageUtil.MODULE_COMMON,
            MessageCode = MessageUtil.MessageList.MSG0066)]
        public string MotivationTypeName { get; set; }
    }

    #endregion
}
