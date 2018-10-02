using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Installation;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter for Register data
    /// </summary>
    public class CTS110_RegisterRentalContractTargetData
    {
        public CTS110_InitialRegisterRentalContractTargetData InitialData { get; set; }
        public dsRentalContractData RegisterRentalContractData { get; set; }
    }

    /// <summary>
    /// Parameter for Initial data
    /// </summary>
    public class CTS110_InitialRegisterRentalContractTargetData
    {
        public string ContractCode { get; set; }
        public string OCCCode { get; set; }
        public doRentalContractBasicInformation doRentalContractBasicData { get; set; }
        public bool HasStopFee { get; set; }
        public tbt_RentalContractBasic RentalContractBasicData { get; set; }
        
        //public decimal? DefaultRemovalFee { get; set; }
        public List<doGetRemovalData> RemovalDataList { get; set; }

        public dsCancelContractQuotation CancelContractQuotationData { get; set; }
        public List<CTS110_CancelContractMemoDetailTemp> CancelContractMemoDetailTempData { get; set; }

        public List<CTS110_BillingTemp> BillingTempData { get; set; }
        public List<CTS110_BillingClientData> BillingClientData { get; set; }
        public List<CTS110_BillingTargetData> BillingTargetData { get; set; }

        public CTS110_BillingTemp BillingTempDataTemp { get; set; }
        public CTS110_BillingClientData BillingClientDataTemp { get; set; }
        public CTS110_BillingTargetData BillingTargetDataTemp { get; set; }
        public string SequenceTemp { get; set; }

        public decimal? TotalSlideAmount { get; set; }
        public decimal? TotalRefundAmount { get; set; }
        public decimal? TotalBillingAmount { get; set; }
        public decimal? TotalCounterBalAmount { get; set; }

        public decimal? TotalSlideAmountUsd { get; set; }
        public decimal? TotalRefundAmountUsd { get; set; }
        public decimal? TotalBillingAmountUsd { get; set; }
        public decimal? TotalCounterBalAmountUsd { get; set; }
    }

    /// <summary>
    /// Parameter of CTS110 screen
    /// </summary>
    public class CTS110_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public CTS110_RegisterRentalContractTargetData CTS110_Session { get; set; }

        public string ContractCode { get; set; }
    }

    /// <summary>
    /// DO of CancelContract grid
    /// </summary>
    public class CTS110_CancelContractConditionGridData
    {
        //Cancel contract condition list
        public string FeeType { get; set; }
        public string HandlingType { get; set; }
        //public decimal? Fee { get; set; }
        //public decimal? Tax { get; set; }
        public string Fee { get; set; }
        public string Tax { get; set; }
        public string Period { get; set; }
        public string Remark { get; set; }
        public string FeeTypeCode { get; set; }
        public string HandlingTypeCode { get; set; }
        public string Sequence { get; set; }

        //Cancel contract condition
        //public decimal? TotalSlideAmt { get; set; }
        //public decimal? TotalReturnAmt { get; set; }
        //public decimal? TotalBillingAmt { get; set; }
        //public decimal? TotalAmtAfterCounterBalance { get; set; }
        public string TotalSlideAmt { get; set; }
        public string TotalSlideAmtUsd { get; set; }
        public string TotalReturnAmt { get; set; }
        public string TotalReturnAmtUsd { get; set; }
        public string TotalBillingAmt { get; set; }
        public string TotalBillingAmtUsd { get; set; }
        public string TotalAmtAfterCounterBalance { get; set; }
        public string TotalAmtAfterCounterBalanceUsd { get; set; }
        public string ProcessAfterCounterBalanceType { get; set; }
        public string ProcessAfterCounterBalanceTypeUsd { get; set; }
        public string OtherRemarks { get; set; }
    }

    /// <summary>
    /// DO of Removal InstallationFee grid
    /// </summary>
    public class CTS110_RemovalInstallationFeeGridData
    {
        //Removal Installation Fee
        public string InstallationFee { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentMethod { get; set; }

        //Removal fee billing target
        public string BillingOCC { get; set; }
        public string BillingClientCode { get; set; }
        public string BillingOfficeCode { get; set; }
        public string BillingOfficeName { get; set; }
        public string BillingTargetCode { get; set; }
        public string BillingTargetName { get; set; }
        public string Sequence { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// DO of CancelContract Memo detail 
    /// </summary>
    public class CTS110_CancelContractMemoDetailTemp : tbt_CancelContractMemoDetail
    {
        public string Sequence { get; set; }
    }

    /// <summary>
    /// DO of BillingTemp
    /// </summary>
    public class CTS110_BillingTemp : tbt_BillingTemp
    {
        public string Sequence { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// DO of DO of BillingClient
    /// </summary>
    public class CTS110_BillingClientData : dtBillingClientData
    {
        public string Sequence { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// DO of BillingTarget
    /// </summary>
    public class CTS110_BillingTargetData : tbt_BillingTarget
    {
        public string BillingOCC { get; set; }
        public string Sequence { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// Parameter for RegisterCancel data
    /// </summary>
    public class CTS110_RegisterCancelData
    {
        public string ChangeType { get; set; }
        public Nullable<System.DateTime> ChangeImplementDate { get; set; }
        public string StopCancelReasonType { get; set; }
        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
        public string ProcessAfterCounterBalanceType { get; set; }
        public string ProcessAfterCounterBalanceTypeUsd { get; set; }
        public string OtherRemarks { get; set; }
        public decimal? RemovalAmount { get; set; }
        public string RemovalAmountCurrencyType { get; set; }
        public string PaymentMethod { get; set; }
        public string RemovalRowSequence { get; set; }
        public bool IsShowBillingTagetDetail { get; set; }
    }

    /// <summary>
    /// DO for validate HandlingType FeeType
    /// </summary>
    [MetadataType(typeof(CTS110_ValidateFeeTypeHandlingType_MetaData))]
    public class CTS110_ValidateFeeTypeHandlingType : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate Contract MaintenanceFee
    /// </summary>
    [MetadataType(typeof(CTS110_ValidateContractMaintenanceFee_MetaData))]
    public class CTS110_ValidateContractMaintenanceFee : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate DepositCard OtherFee
    /// </summary>
    [MetadataType(typeof(CTS110_ValidateDepositCardOtherFee_MetaData))]
    public class CTS110_ValidateDepositCardOtherFee : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate RemovalFee
    /// </summary>
    [MetadataType(typeof(CTS110_ValidateRemovalFee_MetaData))]
    public class CTS110_ValidateRemovalFee : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate ChangeFee
    /// </summary>
    [MetadataType(typeof(CTS110_ValidateChangeFee_MetaData))]
    public class CTS110_ValidateChangeFee : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate CancelFee
    /// </summary>
    [MetadataType(typeof(CTS110_ValidateCancelFee_MetaData))]
    public class CTS110_ValidateCancelFee : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for check authority of RentalContractBasic 
    /// </summary>
    [MetadataType(typeof(CTS110_doRentalContractBasicAuthority_MetaData))]
    public class CTS110_doRentalContractBasicAuthority : tbt_RentalContractBasic
    {

    }

    /// <summary>
    /// DO for validate BillingClient
    /// </summary>
    [MetadataType(typeof(CTS110_ValidateBillingClientData_MetaData))]
    public class CTS110_ValidateBillingClientData : dtBillingClientData
    {

    }

    /// <summary>
    /// DO for validate BillingTemp
    /// </summary>
    [MetadataType(typeof(CTS110_ValidateBillingTempData_MetaData))]
    public class CTS110_ValidateBillingTempData : tbt_BillingTemp
    {

    }

    /// <summary>
    /// DO for validate CancelContract
    /// </summary>
    [MetadataType(typeof(CTS110_ValidateCancelContract_MetaData))]
    public class CTS110_ValidateCancelContract : tbt_RentalSecurityBasic
    {

    }

    /// <summary>
    /// DO for validate CancelContract BefStart
    /// </summary>
    [MetadataType(typeof(CTS110_ValidateCancelContractBefStart_MetaData))]
    public class CTS110_ValidateCancelContractBefStart : tbt_RentalSecurityBasic
    {

    }

    /// <summary>
    /// DO for validate ProcessAfterCounterBalanceType
    /// </summary>
    [MetadataType(typeof(CTS110_ValidateProcessAfterCounterBalanceType_MetaData))]
    public class CTS110_ValidateProcessAfterCounterBalanceType : CTS110_RegisterCancelData
    {

    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS110_doRentalContractBasicAuthority_MetaData
    {
        [OperationOfficeInRole(Module = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0063)]
        public string OperationOfficeCode { get; set; }
    }

    public class CTS110_ValidateFeeTypeHandlingType_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblFeeType",
                ControlName = "ddlFeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblHandlingType",
                ControlName = "ddlHandlingType")]
        public string HandlingType { get; set; }
    }

    public class CTS110_ValidateContractMaintenanceFee_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblFeeType",
                ControlName = "ddlFeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblHandlingType",
                ControlName = "ddlHandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblFee",
                ControlName = "txtFee")]
        public Nullable<decimal> FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblTax",
                ControlName = "txtTax")]
        public Nullable<decimal> TaxAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblPeriod",
                ControlName = "dpPeriodFrom")]
        public Nullable<System.DateTime> StartPeriodDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS110",
            Parameter = "lblPeriod",
            ControlName = "dpPeriodTo")]
        public Nullable<System.DateTime> EndPeriodDate { get; set; }
    }

    public class CTS110_ValidateDepositCardOtherFee_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblFeeType",
                ControlName = "ddlFeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblHandlingType",
                ControlName = "ddlHandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblFee",
                ControlName = "txtFee")]
        public Nullable<decimal> FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblTax",
                ControlName = "txtTax")]
        public Nullable<decimal> TaxAmount { get; set; }
    }

    public class CTS110_ValidateRemovalFee_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblFeeType",
                ControlName = "ddlFeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblHandlingType",
                ControlName = "ddlHandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblFee",
                ControlName = "txtFee")]
        public Nullable<decimal> FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblTax",
                ControlName = "txtTax")]
        public Nullable<decimal> TaxAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblNormalFee",
                ControlName = "txtNormalFee")]
        public Nullable<decimal> NormalFeeAmount { get; set; }
    }

    public class CTS110_ValidateChangeFee_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblFeeType",
                ControlName = "ddlFeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblHandlingType",
                ControlName = "ddlHandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblFee",
                ControlName = "txtFee")]
        public Nullable<decimal> FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblTax",
                ControlName = "txtTax")]
        public Nullable<decimal> TaxAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblPeriod",
                ControlName = "dpPeriodFrom")]
        public Nullable<System.DateTime> StartPeriodDate { get; set; }
    }

    public class CTS110_ValidateCancelFee_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblFeeType",
                ControlName = "ddlFeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblHandlingType",
                ControlName = "ddlHandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblFee",
                ControlName = "txtFee")]
        public Nullable<decimal> FeeAmount { get; set; }
    }

    public class CTS110_ValidateBillingClientData_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS110",
            Parameter = "lblCustomerType",
            ControlName = "")]
        public string CustTypeCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS110",
            Parameter = "lblNameEnglish",
            ControlName = "FullNameEN")]
        public string NameEN { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS110",
            Parameter = "lblNameLocal",
            ControlName = "FullNameLC")]
        public string NameLC { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS110",
            Parameter = "lblAddressEnglish",
            ControlName = "AddressEN")]
        public string AddressEN { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS110",
            Parameter = "lblAddressLocal",
            ControlName = "AddressLC")]
        public string AddressLC { get; set; }
    }

    public class CTS110_ValidateBillingTempData_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblBillingOffice",
                ControlName = "BillingOffice")]
        public string BillingOfficeCode { get; set; }
    }

    public class CTS110_ValidateCancelContract_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblStopType")]
        public string ChangeType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblCancelDate",
                ControlName = "dpCancelDate")]
        public Nullable<System.DateTime> ChangeImplementDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblApprove1",
                ControlName = "txtApproveNo1")]
        public string ApproveNo1 { get; set; }
    }

    public class CTS110_ValidateCancelContractBefStart_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblCancelDate",
                ControlName = "dpCancelDate")]
        public Nullable<System.DateTime> ChangeImplementDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblCancelReason",
                ControlName = "ddlCancelReason")]
        public string StopCancelReasonType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblApprove1",
                ControlName = "txtApproveNo1")]
        public string ApproveNo1 { get; set; }
    }

    public class CTS110_ValidateProcessAfterCounterBalanceType_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblSelectProcessAfterCounterBalRp")]
        public string ProcessAfterCounterBalanceType { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS110",
                Parameter = "lblSelectProcessAfterCounterBalUsd")]
        public string ProcessAfterCounterBalanceTypeUsd { get; set; }
    }
}
