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

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of CTS140 screen
    /// </summary>
    public class CTS140_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public string ContractCode { get; set; }
        public string ContractCodeLong
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            }
        }

        [KeepSession]
        public CTS140_DOContractRelateInformation DOContractRelateInformation { get; set; }
        [KeepSession]
        public CTS140_DOContractAgreementInformation DOContractAgreementInformation { get; set; }
        [KeepSession]
        public CTS140_DODepositInformation DODepositInformation { get; set; }
        [KeepSession]
        public CTS140_DOContractDocumentInformation DOContractDocumentInformation { get; set; }
        [KeepSession]
        public CTS140_DOProvideServiceInformation DOProvideServiceInformation { get; set; }
        [KeepSession]
        public CTS140_DOMaintenanceInformation DOMaintenanceInformation { get; set; }
        [KeepSession]
        public CTS140_DOSiteInformation DOSiteInformation { get; set; }
        [KeepSession]
        public CTS140_DOCancelContractCondition DOCancelContractCondition { get; set; }
        [KeepSession]
        public CTS140_DOValidateBusiness DOValidateBusiness { get; set; }
        [KeepSession]
        public dsRentalContractData DSRentalContract { get; set; }
        [KeepSession]
        public List<tbt_RelationType> ListRelationType { get; set; }
        [KeepSession]
        public List<tbt_RentalSecurityBasic> ListRentalSecurityBasic { get; set; }
        [KeepSession]
        public List<CTS140_DOMaintenanceGrid> ListDOMaintenanceGrid { get; set; }
        [KeepSession]
        public List<CTS140_DOMaintenanceGrid> ListDOMaintenanceGridEdit { get; set; }
        [KeepSession]
        public List<CTS140_DOCancelContractGrid> ListDOCancelContractGrid { get; set; }
        [KeepSession]
        public List<string> ListDOCancelContractGridRemove { get; set; }
    }

    /// <summary>
    /// DO of ContractRelate
    /// </summary>
    public class CTS140_DOContractRelateInformation
    {
        public string LastOrderContractFeeCurrencyType { get; set; }
        public decimal? LastOrderContractFee { get; set; }

        public DateTime? StartDealDate { get; set; }
        public DateTime? FirstSecurityStartDate { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public DateTime? LastChangeImplementDate { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeTypeCodeName { get; set; }
        public int? ContractDurationMonth { get; set; }
        public int? AutoRenewMonth { get; set; }
        public string LastOCC { get; set; }
        public string ProjectCode { get; set; }
    }

    /// <summary>
    /// DO of ContractAgreement
    /// </summary>
    public class CTS140_DOContractAgreementInformation
    {
        public DateTime? ApproveContractDate { get; set; }
        public string ContractOfficeCode { get; set; }
        public string QuotationTargetCode { get; set; }
        public string QuotationAlphabet { get; set; }
        public string PlanCode { get; set; }
        public string SalesmanEmpNo1 { get; set; }
        public string SalesmanEmpName1 { get; set; }
        public string SalesmanEmpNo2 { get; set; }
        public string SalesmanEmpName2 { get; set; }
        public string RelatedContractCode { get; set; }
        public string OldContractCode { get; set; }
        public string AcquisitionTypeCode { get; set; }
        public string IntroducerCode { get; set; }
        public string MotivationTypeCode { get; set; }
        public string SalesSupporterEmpNo { get; set; }
        public string SalesSupporterEmpName { get; set; }

        public string RelatedContractCodeShort { get; set; }
        public string QuotationTargetCodeShort { get; set; }
        public string OldContractCodeShort { get; set; }

        public DateTime? PaymentDateIncentive { get; set; }
    }

    /// <summary>
    /// DO of Deposit
    /// </summary>
    public class CTS140_DODepositInformation
    {
        public string NormalDepositFeeCurrencyType { get; set; }
        public decimal? NormalDepositFee { get; set; }
        public string OrderDepositFeeCurrencyType { get; set; }
        public decimal? OrderDepositFee { get; set; }
        public string ExemptedDepositFeeCurrencyType { get; set; }
        public decimal? ExemptedDepositFee { get; set; }
        public string CounterBalanceOriginContractCode { get; set; }
        public string CounterBalanceOriginContractCodeShort { get; set; }
    }

    /// <summary>
    /// DO of ContractDocument
    /// </summary>
    public class CTS140_DOContractDocumentInformation
    {
        public bool? IrregurationDocUsageFlag { get; set; }
        public string PODocAuditResult { get; set; }
        public string ContractDocAuditResult { get; set; }
        public string StartMemoAuditResult { get; set; }

        public string PODocAuditResultCodeName { get; set; }
        public string ContractDocAuditResultCodeName { get; set; }
        public string StartMemoAuditResultCodeName { get; set; }

        public DateTime? ContractDocReceiveDate { get; set; }
        public DateTime? PODocReceiveDate { get; set; }
        public DateTime? StartMemoReceiveDate { get; set; }
    }

    /// <summary>
    /// DO of ProvideService
    /// </summary>
    public class CTS140_DOProvideServiceInformation
    {
        public bool? FireMonitorFlag { get; set; }
        public bool? CrimePreventFlag { get; set; }
        public bool? EmergencyReportFlag { get; set; }
        public bool? FacilityMonitorFlag { get; set; }
        public string PhoneLineTypeCode1 { get; set; }
        public string PhoneLineOwnerCode1 { get; set; }
        public string PhoneLineTypeCode2 { get; set; }
        public string PhoneLineOwnerCode2 { get; set; }
        public string PhoneLineTypeCode3 { get; set; }
        public string PhoneLineOwnerCode3 { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string PhoneNo3 { get; set; }
    }

    /// <summary>
    /// DO of Maintenance
    /// </summary>
    public class CTS140_DOMaintenanceInformation
    {
        public string MaintenanceTypeCode { get; set; }
        public int? MaintenanceCycle { get; set; }
        public int? MaintenanceContractStartMonth { get; set; }
        public int? MaintenanceContractStartYear { get; set; }
        public string MaintenanceFeeTypeCode { get; set; }
    }

    /// <summary>
    /// DO of Maintenance grid
    /// </summary>
    [MetadataType(typeof(CTS140_DOMaintenanceGrid_Meta))]
    public class CTS140_DOMaintenanceGrid
    {
        CommonUtil comUtil = new CommonUtil();

        public string ContractCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductTypeCode { get; set; }
        public string ContractCodeLong { get { return comUtil.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG); } }
    }

    /// <summary>
    /// DO of Site
    /// </summary>
    public class CTS140_DOSiteInformation
    {
        public string BuildingTypeCode { get; set; }
        public string MainStructureTypeCode { get; set; }
        public string SiteBuildingArea { get; set; }
        public string SecurityAreaFrom { get; set; }
        public string SecurityAreaTo { get; set; }
        public int? NumOfBuilding { get; set; }
        public int? NumOfFloor { get; set; }
    }

    /// <summary>
    /// DO of Other
    /// </summary>
    public class CTS140_DOOtherInformation
    {
        public string Memo { get; set; }
    }

    /// <summary>
    /// DO for get ScreeenCode
    /// </summary>
    public class CTS140_DOGetScreeenCode
    {
        public string ContractCode { get; set; }
        public string ContractCodeLong
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            }
        }
    }

    /// <summary>
    /// DO for add CancelContract Condition
    /// </summary>
    [MetadataType(typeof(CTS140_DOADDCancelContractCondition_Meta))]
    public class CTS140_DOADDCancelContractCondition
    {
        public string BillingType { get; set; }
        public string FeeAmount { get; set; }
        public string ContractCode_CounterBalance { get; set; }
        public string HandlingType { get; set; }
        public string TaxAmount { get; set; }
        public string NormalFeeAmount { get; set; }
        public DateTime? StartPeriodDate { get; set; }
        public DateTime? EndPeriodDate { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// DO of CancelContract Condition FeeC1
    /// </summary>
    [MetadataType(typeof(CTS140_DOCancelContractCondition_FeeC1_Meta))]
    public class CTS140_DOCancelContractCondition_FeeC1 : CTS140_DOADDCancelContractCondition
    {
    }

    /// <summary>
    /// DO of CancelContract Condition FeeC2
    /// </summary>
    [MetadataType(typeof(CTS140_DOCancelContractCondition_FeeC2_Meta))]
    public class CTS140_DOCancelContractCondition_FeeC2 : CTS140_DOADDCancelContractCondition
    {
    }

    /// <summary>
    /// DO of CancelContract Condition FeeC3
    /// </summary>
    [MetadataType(typeof(CTS140_DOCancelContractCondition_FeeC3_Meta))]
    public class CTS140_DOCancelContractCondition_FeeC3 : CTS140_DOADDCancelContractCondition
    {
    }

    /// <summary>
    /// DO of CancelContract Condition FeeC4
    /// </summary>
    [MetadataType(typeof(CTS140_DOCancelContractCondition_FeeC4_Meta))]
    public class CTS140_DOCancelContractCondition_FeeC4 : CTS140_DOCancelContractCondition
    {
    }

    /// <summary>
    /// DO of CancelContract Condition FeeC5
    /// </summary>
    [MetadataType(typeof(CTS140_DOCancelContractCondition_FeeC5_Meta))]
    public class CTS140_DOCancelContractCondition_FeeC5 : CTS140_DOCancelContractCondition
    {
    }

    /// <summary>
    /// DO of CancelContract Condition
    /// </summary>
    public class CTS140_DOCancelContractCondition
    {
        public string BillingType { get; set; }

        public string FeeAmountCurrencyType { get; set; }
        public string FeeAmount { get; set; }

        public string ContractCode_CounterBalance { get; set; }
        public string HandlingType { get; set; }

        public string TaxAmountCurrencyType { get; set; }
        public string TaxAmount { get; set; }

        public string NormalFeeAmountCurrencyType { get; set; }
        public string NormalFeeAmount { get; set; }

        public DateTime? StartPeriodDate { get; set; }
        public DateTime? EndPeriodDate { get; set; }
        public string Remark { get; set; }
        public string ProcessAfterCounterBalanceType { get; set; }
        public string ProcessAfterCounterBalanceTypeUsd { get; set; }

        public string C_BILLING_TYPE_CONTRACT_FEE { get; set; }
        public string C_BILLING_TYPE_MAINTENANCE_FEE { get; set; }
        public string C_BILLING_TYPE_DEPOSIT_FEE { get; set; }
        public string C_BILLING_TYPE_REMOVAL_INSTALLATION_FEE { get; set; }
        public string C_BILLING_TYPE_CANCEL_CONTRACT_FEE { get; set; }
        public string C_BILLING_TYPE_CHANGE_INSTALLATION_FEE { get; set; }
        public string C_BILLING_TYPE_CARD_FEE { get; set; }
        public string C_BILLING_TYPE_OTHER_FEE { get; set; }

        public string C_HANDLING_TYPE_BILL_UNPAID_FEE { get; set; }
        public string C_HANDLING_TYPE_EXEMPT_UNPAID_FEE { get; set; }
        public string C_HANDLING_TYPE_RECEIVE_AS_REVENUE { get; set; }
        public string C_HANDLING_TYPE_REFUND { get; set; }
        public string C_HANDLING_TYPE_SLIDE { get; set; }

        public string C_PROC_AFT_COUNTER_BALANCE_TYPE_BILL { get; set; }
        public string C_PROC_AFT_COUNTER_BALANCE_TYPE_EXEMPT { get; set; }
        public string C_PROC_AFT_COUNTER_BALANCE_TYPE_RECEIVE { get; set; }
        public string C_PROC_AFT_COUNTER_BALANCE_TYPE_REFUND { get; set; }

        public string C_ContractFeeValidate { get; set; } //ไว้สำหรับ Validate นะครับเเต่มีเรื่องการเปลี่ยน ภาษาเข้ามาเกี่ยวด้วยก้อจะส่งเข้ามาจาก Hidden Field
        public string C_MaintenanceFeeValidate { get; set; } //ไว้สำหรับ Validate นะครับเเต่มีเรื่องการเปลี่ยน ภาษาเข้ามาเกี่ยวด้วยก้อจะส่งเข้ามาจาก Hidden Field

        public string OtherRemarks { get; set; }

        public string CancelContractDate { get; set; }
        public string RemovalInstallationCompleteDate { get; set; }
    }    

    /// <summary>
    /// DO of ProductName
    /// </summary>
    public class CTS140_DOProductName : tbm_Product
    {
        [LanguageMappingAttribute]
        public string ProductName { get; set; }
    }

    /// <summary>
    /// DO of Employee
    /// </summary>
    public class CTS140_DOEmployee : tbm_Employee
    {
        public string EmpName { get; set; }
    }

    /// <summary>
    /// DO for Validate business
    /// </summary>
    [MetadataType(typeof(CTS140_DOValidateBusiness_Meta))]
    public class CTS140_DOValidateBusiness
    {
        //Contract related information
        public DateTime? FirstSecurityStartDate { get; set; } //Add by Jutarat A. on 18102013
        public DateTime? StartDealDate { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public int? ContractDurationMonth { get; set; }
        public int? AutoRenewMonth { get; set; }
        public string ProjectCode { get; set; }

        public string ContractCode { get; set; }
        public string CounterBalanceOriginContractCode { get; set; }
        public string AcquisitionTypeCode { get; set; }
        public bool IsContractCancelShow { get; set; }
        public bool IsGenerateMAScheduleAgain { get; set; }
        public bool IsShowContractRelatedInformation { get; set; }

        //Contract agreement information
        public string ContractOfficeCode { get; set; }
        public string PlanCode { get; set; }
        public string SalesmanEmpNo1 { get; set; }
        public string SalesmanEmpNo2 { get; set; }
        public string OldContractCode { get; set; }
        public string IntroducerCode { get; set; }
        public string SalesSupporterEmpNo { get; set; }
        public string MotivationTypeCode { get; set; }
        public bool IsShowContractAgreementInformation { get; set; }
        public string QuotationNo { get; set; }

        //Deposit information
        public string NormalDepositFeeCurrencyType { get; set; }
        public string NormalDepositFee { get; set; }
        public string OrderDepositFeeCurrencyType { get; set; }
        public string OrderDepositFee { get; set; }
        public string ExemptedDepositFeeCurrencyType { get; set; }
        public string ExemptedDepositFee { get; set; }

        public bool IsShowDepositInformation { get; set; }

        //Contract document information
        public bool IrregurationDocUsageFlag { get; set; }
        public bool IsShowContractDocumentInformation { get; set; }

        //Provide service information
        public bool FireMonitorFlag { get; set; }
        public bool CrimePreventFlag { get; set; }
        public bool EmergencyReportFlag { get; set; }
        public bool FacilityMonitorFlag { get; set; }
        public string PhoneLineTypeCode1 { get; set; }
        public string PhoneLineOwnerCode1 { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneLineTypeCode2 { get; set; }
        public string PhoneLineOwnerCode2 { get; set; }
        public string PhoneNo2 { get; set; }
        public string PhoneLineTypeCode3 { get; set; }
        public string PhoneLineOwnerCode3 { get; set; }
        public string PhoneNo3 { get; set; }
        public bool IsShowProvideServiceInformation { get; set; }

        //Maintenance information
        public string MaintenanceTypeCode { get; set; }
        public string MaintenanceCycle { get; set; }
        public string MaintenanceContractStartMonth { get; set; }
        public string MaintenanceContractStartYear { get; set; }
        public string MaintenanceFeeTypeCode { get; set; }
        public bool IsShowMaintenanceInformation { get; set; }

        //Site information
        public string BuildingTypeCode { get; set; }
        public string SiteBuildingArea { get; set; }
        public string NumOfBuilding { get; set; }
        public string SecurityAreaFrom { get; set; }
        public string SecurityAreaTo { get; set; }
        public string NumOfFloor { get; set; }
        public string MainStructureTypeCode { get; set; }
        public bool IsShowSiteInformation { get; set; }

        //Cancel contract condition
        public string FeeType { get; set; }
        public string HandlingType { get; set; }
        public string FeeAmount { get; set; }
        public string TaxAmount { get; set; }
        public DateTime? StartPeriodDate { get; set; }
        public DateTime? EndPeriodDate { get; set; }
        public string Remark { get; set; }

        public string ProcessAfterCounterBalanceType { get; set; }
        public string ProcessAfterCounterBalanceTypeUsd { get; set; }

        public string OtherRemarks { get; set; }

        public decimal? TotalSlideAmt { get; set; }
        public decimal? TotalReturnAmt { get; set; }
        public decimal? TotalBillingAmt { get; set; }
        public decimal? TotalAmtAfterCounterBalance { get; set; }

        public decimal? TotalSlideAmtUsd { get; set; }
        public decimal? TotalReturnAmtUsd { get; set; }
        public decimal? TotalBillingAmtUsd { get; set; }
        public decimal? TotalAmtAfterCounterBalanceUsd { get; set; }

        public bool IsShowCancelContractCondition { get; set; }

        //Other information
        public string Memo { get; set; }

        public string GroupNameEN { get; set; }

        public string OperationOfficeCode { get; set; } //Add by Jutarat A. on 15082012
        public string RelatedContractCode { get; set; } //Add by Jutarat A. on 16082012
        public string UserCode { get; set; } //Add by Jutarat A. on 18102013

        public DateTime? PaymentDateIncentive { get; set; }
    }

    /// <summary>
    /// DO for Validate ProcessAfterCounterBalanceType
    /// </summary>
    [MetadataType(typeof(CTS140_ValidateProcessAfterCounterBalanceType_MetaData))]
    public class CTS140_ValidateProcessAfterCounterBalanceType : CTS140_DOValidateBusiness
    {
    }

    //Add by Jutarat A. on 16082012
    /// <summary>
    /// DO for Validate LinkageSale ContractCode
    /// </summary>
    [MetadataType(typeof(CTS140_ValidateLinkageSaleContractCode_MetaData))]
    public class CTS140_ValidateLinkageSaleContractCode : CTS140_DOValidateBusiness
    {
    }
    //End Add

    /// <summary>
    /// DO of CancelContract grid
    /// </summary>
    public class CTS140_DOCancelContractGrid : tbt_CancelContractMemoDetail
    {
        public string FeeAmountString { get; set; }
        public string TaxAmountString { get; set; }
        public string PeriodString { get; set; }
        public string StatusGrid { get; set; }
        public string RemarkString { get; set; }
        public string Sequence { get; set; }
    }

    /// <summary>
    /// DO of Total CancelContract
    /// </summary>
    public class CTS140_DOCancelContractTotal
    {
        public string TotalSlideAmt { get; set; }
        public string TotalRefundAmt { get; set; }
        public string TotalBillingAmt { get; set; }
        public string TotalAmtAfterCounterBalanceType { get; set; }

        public string TotalSlideAmtUsd { get; set; }
        public string TotalRefundAmtUsd { get; set; }
        public string TotalBillingAmtUsd { get; set; }
        public string TotalAmtAfterCounterBalanceTypeUsd { get; set; }

        public string Remark { get; set; }
        public bool IsShowReturnReceive { get; set; }
    }

    /// <summary>
    /// DO of Constant
    /// </summary>
    public class CTS140_DOConstantHideDIV
    {
        public string ContractStatus { get; set; }
        public string ProductTypeCode { get; set; }
        public string C_PROD_TYPE_AL { get; set; }
        public string C_PROD_TYPE_BE { get; set; }
        public string C_PROD_TYPE_MA { get; set; }
        public string C_PROD_TYPE_ONLINE { get; set; }
        public string C_PROD_TYPE_SALE { get; set; }
        public string C_PROD_TYPE_SG { get; set; }
        public string C_PROD_TYPE_RENTAL_SALE { get; set; }
        public string C_CONTRACT_STATUS_AFTER_START { get; set; }
        public string C_CONTRACT_STATUS_BEF_START { get; set; }
        public string C_CONTRACT_STATUS_CANCEL { get; set; }
        public string C_CONTRACT_STATUS_END { get; set; }
        public string C_CONTRACT_STATUS_FIXED_CANCEL { get; set; }
        public string C_CONTRACT_STATUS_STOPPING { get; set; }
    }

}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS140_DOValidateBusiness_Meta
    {
        //[NotNullOrEmpty(ControlName = "SalesmanEmpNo1", Controller = MessageUtil.MODULE_COMMON, Screen = "CTS140", MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "lblContractAgreementInformation")]
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblSaleMan1",
                ControlName = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }

        //[NotNullOrEmpty(ControlName = "ContractOfficeCode", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Contract office")]
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblContractOffice",
                ControlName = "ContractOfficeCode")]
        public string ContractOfficeCode { get; set; }

        //[NotNullOrEmpty(ControlName = "MaintenanceTypeCode", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Maintenance type")]
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblMaintenanceType",
                ControlName = "MaintenanceTypeCode")]
        public string MaintenanceTypeCode { get; set; }

        //[NotNullOrEmpty(ControlName = "MaintenanceCycle", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Maintenance cycle")]        
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblMaintenanceCycle",
                ControlName = "MaintenanceCycle")]
        public string MaintenanceCycle { get; set; }

        //[NotNullOrEmpty(ControlName = "MaintenanceContractStartMonth", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Maintenance contract start month")]
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblMaintenanceContractStartMonth",
                ControlName = "Month")]
        public string MaintenanceContractStartMonth { get; set; }

        //[NotNullOrEmpty(ControlName = "MaintenanceContractStartYear", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Maintenance contract start year")]
        //public string MaintenanceContractStartYear { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT, Screen = "CTS140", Parameter = "lblMaintenanceContractStartYear", ControlName = "Year")]
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblMaintenanceContractStartMonth",
                ControlName = "Year")]
        public string MaintenanceContractStartYear { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS140",
        //                Parameter = "lblMaintenanceCycle",
        //                ControlName = "MaintenanceContractStartMonth")]
        //public string GroupNameEN { get; set; }


        [CTS140_InstroducerNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS140",
                        Parameter = "lblIntroducerCode",
                        ControlName = "IntroducerCode")]
        public string IntroducerCode { get; set; }

        //Add by Jutarat A. on 15082012
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblOperationOffice",
                ControlName = "OperationOffice")]
        public string OperationOfficeCode { get; set; }
        //End Add
    }

    public class CTS140_ValidateProcessAfterCounterBalanceType_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblSelectProcessAfterCounterRp")]
        public string ProcessAfterCounterBalanceType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblSelectProcessAfterCounterUsd")]
        public string ProcessAfterCounterBalanceTypeUsd { get; set; }
    }

    //Add by Jutarat A. on 16082012
    public class CTS140_ValidateLinkageSaleContractCode_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblLinkageSaleContractCode",
                ControlName = "RelatedContractCode")]
        public string RelatedContractCode { get; set; }
    }
    //End Add

    public class CTS140_InstroducerNotNullOrEmptyAttribute : NotNullOrEmptyAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value))
            {
                CTS140_DOValidateBusiness cond = validationContext.ObjectInstance as CTS140_DOValidateBusiness;
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

    public class CTS140_DOADDCancelContractCondition_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblFeeType",
                ControlName = "FeeType",
                Order = 1)]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblHandlingType",
                ControlName = "HandlingType",
                Order = 2)]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblFee",
                ControlName = "FeeAmount",
                Order = 3)]
        public string FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblTax",
                ControlName = "TaxAmount",
                Order = 4)]
        public string TaxAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblNormalFee",
                ControlName = "NormalFeeAmount",
                Order = 5)]
        public string NormalFeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblPeriodFrom",
                ControlName = "StartPeriodDateContract",
                Order = 6)]
        public DateTime? StartPeriodDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblPeriodTo",
                ControlName = "EndPeriodDateContract",
                Order = 7)]
        public DateTime? EndPeriodDate { get; set; }
    }
    public class CTS140_DOCancelContractCondition_FeeC1_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblHandlingType",
                ControlName = "HandlingType",
                Order = 1)]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblFee",
                ControlName = "FeeAmount",
                Order = 2)]
        public string FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblTax",
                ControlName = "TaxAmount",
                Order = 3)]
        public string TaxAmount { get; set; }
        

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblPeriodFrom",
                ControlName = "StartPeriodDateContract",
                Order = 4)]
        public DateTime? StartPeriodDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblPeriodTo",
                ControlName = "EndPeriodDateContract",
                Order = 5)]
        public DateTime? EndPeriodDate { get; set; }
    }
    public class CTS140_DOCancelContractCondition_FeeC2_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblHandlingType",
                ControlName = "HandlingType",
                Order = 1)]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblFee",
                ControlName = "FeeAmount",
                Order = 2)]
        public string FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblTax",
                ControlName = "TaxAmount",
                Order = 3)]
        public string TaxAmount { get; set; }
    }
    public class CTS140_DOCancelContractCondition_FeeC3_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblHandlingType",
                ControlName = "HandlingType",
                Order = 1)]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblFee",
                ControlName = "FeeAmount",
                Order = 2)]
        public string FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblTax",
                ControlName = "TaxAmount",
                Order = 3)]
        public string TaxAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblNormalFee",
                ControlName = "NormalFeeAmount",
                Order=4)]
        public string NormalFeeAmount { get; set; }
    }
    public class CTS140_DOCancelContractCondition_FeeC4_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblHandlingType",
                ControlName = "HandlingType",
                Order = 1)]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblFee",
                ControlName = "FeeAmount",
                Order = 2)]
        public string FeeAmount { get; set; }
    }
    public class CTS140_DOCancelContractCondition_FeeC5_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblHandlingType",
                ControlName = "HandlingType",
                Order = 1)]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblFee",
                ControlName = "FeeAmount",
                Order = 2)]
        public string FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblTax",
                ControlName = "TaxAmount",
                Order = 3)]
        public string TaxAmount { get; set; }


        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblPeriodFrom",
                ControlName = "StartPeriodDateContract",
                Order = 4)]
        public DateTime? StartPeriodDate { get; set; }
    }

    public class CTS140_DOMaintenanceGrid_Meta
    {
        //[NotNullOrEmpty(ControlName = "ContractCode", Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Contract code")]
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS140",
                Parameter = "lblMaintenanceTargetContractCode",
                ControlName = "MaintenanceTargetContractCode")]
        public string ContractCode { get; set; }
    }
}
