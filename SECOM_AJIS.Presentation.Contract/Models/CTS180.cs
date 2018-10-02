using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.IO;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of CTS180 screen
    /// </summary>
    public class CTS180_ScreenParameter : ScreenParameter
    {
        public List<dtContractDocumentList> ContractDocumentList { get; set; }

        public string DocumentCode { get; set; }
        public List<tbt_ContractDocument> dtTbt_ContractDocument { get; set; }
        public List<tbs_ContractDocTemplate> dtTbs_ContractDocTemplate { get; set; }
        public List<tbt_DocContractReport> dtTbt_DocContractReport { get; set; }
        public List<tbt_DocChangeMemo> dtTbt_DocChangeMemo { get; set; }
        public List<tbt_DocChangeNotice> dtTbt_DocChangeNotice { get; set; }
        public List<tbt_DocConfirmCurrentInstrumentMemo> dtTbt_DocConfirmCurrentInstrumentMemo { get; set; }
        public List<tbt_DocCancelContractMemo> dtTbt_DocCancelContractMemo { get; set; }
        public List<tbt_DocCancelContractMemoDetail> dtTbt_DocCancelContractMemoDetail { get; set; }
        public List<tbt_DocChangeFeeMemo> dtTbt_DocChangeFeeMemo { get; set; }

        public List<CTS110_CancelContractMemoDetailTemp> CancelContractMemoDetailTempData { get; set; }

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
    /// DO of DocumentList grid
    /// </summary>
    public class CTS180_DocumentListGridData : dtContractDocumentList
    {
        public string ContractQuatationCode
        {
            get
            {
                return String.IsNullOrEmpty(ContractCode) ? QuotationTargetCodeShort : ContractCodeShort;
            }
        }

        private string _contractTarget = string.Empty;
        public string ContractTarget
        {
            get
            {
                //if (String.IsNullOrEmpty(ContractTargetCustCode) == false)
                //    _contractTarget = String.Format("(1) {0}<br/>(2) {1}<br/>(3) {2}", ContractTargetCustCode, ContractTargetCustFullNameEN, ContractTargetCustFullNameLC);
                //else
                //    _contractTarget = String.Format("(1) {0}<br/>(2) {1}<br/>(3) {2}", PurchaserCustCode, PurchaserCustFullNameEN, PurchaserCustFullNameLC);

                if (String.IsNullOrEmpty(ContractTargetCustCode) == false)
                    _contractTarget = CommonUtil.TextLineFormat(ContractTargetCustCode, ContractTargetCustFullNameEN, ContractTargetCustFullNameLC);
                else
                    _contractTarget = CommonUtil.TextLineFormat(PurchaserCustCode, PurchaserCustFullNameEN, PurchaserCustFullNameLC);

                return _contractTarget;
            }
        }

        private string _site = string.Empty;
        public string Site
        {
            get
            {
                //if (String.IsNullOrEmpty(SiteCode) == false)
                //    _site = String.Format("(1) {0}<br/>(2) {1}<br/>(3) {2}", SiteCode, SiteNameEN, SiteNameLC);

                if (String.IsNullOrEmpty(SiteCode) == false)
                    _site = CommonUtil.TextLineFormat(SiteCode, SiteNameEN, SiteNameLC);

                return _site;
            }
        }

        public string OCCAlphabet
        {
            get
            {
                return String.IsNullOrEmpty(OCC) ? Alphabet : OCC;
            }
        }

        public string Office
        {
            get
            {
                //string strContractOfficeName = String.IsNullOrEmpty(ContractOfficeName) ? string.Empty : String.Format("(1) {0} ", ContractOfficeName);
                //string strOperationOfficeName = String.IsNullOrEmpty(OperationOfficeName) ? string.Empty : String.Format("<br/>(2) {0}", OperationOfficeName);
                //return String.Format("{0}{1}", strContractOfficeName, strOperationOfficeName);

                return CommonUtil.TextLineFormat(ContractOfficeName, OperationOfficeName);
            }
        }

        public bool IsEnableSelect { get; set; }

        public string Document
        {
            get
            {
                //return String.Format("(1) {0}<br/>(2) {1}<br/>(3) {2}", this.DocumentName, this.DocStatusName, this.DocAuditResultName);
                return CommonUtil.TextLineFormat(this.DocumentName, this.DocStatusName, this.DocAuditResultName);
            }
        }
    }

    /// <summary>
    /// DO of ContractReport grid
    /// </summary>
    public class CTS180_ContractReportGridData
    {
        public string Fee { get; set; }
        public string Order { get; set; }
        public string ApproveContract { get; set; }
        public string CompleteInstallation { get; set; }
        public string StartService { get; set; }
    }

    /// <summary>
    /// Parameter for Register data
    /// </summary>
    public class CTS180_RegisterContractDocumentData
    {
        public string ContractCodeShort { get; set; }
        public string DocumentLanguage { get; set; }
        public string ContractTargetNameLC { get; set; }
        public string RealCustomerNameLC { get; set; }
        public string SiteAddressLC { get; set; }
        public string ContractTargetNameEN { get; set; }
        public string RealCustomerNameEN { get; set; }
        public string SiteAddressEN { get; set; }
        public string SiteNameEN { get; set; }
        public string SiteNameLC { get; set; }

        public string PlanCode { get; set; }
        public string ProductCode { get; set; }
        public string PhoneLineTypeCode { get; set; }
        public string PhoneLineOwnerTypeCode { get; set; }
        public bool? FireSecurityFlag { get; set; }
        public bool? CrimePreventFlag { get; set; }
        public bool? EmergencyReportFlag { get; set; }

        public string ContractFeeCurrencyType { get; set; }
        public decimal? ContractFee { get; set; }
        public string ContractFeeControlName { get; set; }
        public string NegotiationTotalInstallFeeCurrencyType { get; set; }
        public decimal? NegotiationTotalInstallFee { get; set; }
        public string InstallFee_ApproveContractCurrencyType { get; set; }
        public decimal? InstallFee_ApproveContract { get; set; }
        public string InstallFee_CompleteInstallCurrencyType { get; set; }
        public decimal? InstallFee_CompleteInstall { get; set; }
        public string InstallFee_StartServiceCurrencyType { get; set; }
        public decimal? InstallFee_StartService { get; set; }
        public string DepositFeeCurrencyType { get; set; }
        public decimal? DepositFee { get; set; }

        public string DepositFeePhase { get; set; }

        public int? PaymentCycle { get; set; }
        public string ContractFeePayMethod { get; set; }
        public int? CreditTerm { get; set; }
        public int? ContractDurationMonth { get; set; }
        public int? AutoRenewMonth { get; set; }

        public string CustomerSignatureName { get; set; }
        public bool? SECOMSignatureFlag { get; set; }
        public string EmpName { get; set; }
        public string EmpPosition { get; set; }

        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string OldContractFeeCurrencyType { get; set; }
        public decimal? OldContractFee { get; set; }
        public string NewContractFeeCurrencyType { get; set; }
        public decimal? NewContractFee { get; set; }
        public string ChangeContent { get; set; }

        public Nullable<System.DateTime> RealInvestigationDate { get; set; }

        public Nullable<System.DateTime> StartServiceDate { get; set; }
        public Nullable<System.DateTime> CancelContractDate { get; set; }
        public decimal? TotalSlideAmt { get; set; }
        public decimal? TotalReturnAmt { get; set; }
        public decimal? TotalBillingAmt { get; set; }
        public decimal? TotalAmtAfterCounterBalance { get; set; }

        public string ProcessAfterCounterBalanceType { get; set; }
        public string ProcessAfterCounterBalanceTypeUsd { get; set; }
        public string AutoTransferBillingType { get; set; }

        public string BankTransferBillingType { get; set; }
        public string BankTransferBillingTypeUsd { get; set; }
        
        public string OtherRemarks { get; set; }
        public decimal? AutoTransferBillingAmt { get; set; }
        public decimal? BankTransferBillingAmt { get; set; }
        public decimal? BankTransferBillingAmtUsd { get; set; }

        public Nullable<System.DateTime> ChangeContractFeeDate { get; set; }
        public Nullable<System.DateTime> ReturnToOriginalFeeDate { get; set; }

    }

    /// <summary>
    /// DO of CancelContract grid
    /// </summary>
    public class CTS180_CancelContractMemoDetailData : tbt_CancelContractMemoDetail
    {
        public string CancelContractType { get; set; }
    }

    /// <summary>
    /// DO for search ContractDoc Condition
    /// </summary>
    [MetadataType(typeof(CTS180_doSearchContractDocCondition_MetaData))]
    public class CTS180_doSearchContractDocCondition : doSearchContractDocCondition
    {

    }

    /// <summary>
    /// DO for validate Contract
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateContract_MetaData))]
    public class CTS180_ValidateContract : CTS180_RegisterContractDocumentData
    {

    }

    /// <summary>
    /// DO for validate ChangeMemo
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateChangeMemo_MetaData))]
    public class CTS180_ValidateChangeMemo : CTS180_RegisterContractDocumentData
    {

    }

    /// <summary>
    /// DO for validate CancelContract
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateCancelContract_MetaData))]
    public class CTS180_ValidateCancelContract : CTS180_RegisterContractDocumentData
    {

    }

    /// <summary>
    /// DO for validate AutoTransfer
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateAutoTransfer_MetaData))]
    public class CTS180_ValidateAutoTransfer : CTS180_RegisterContractDocumentData
    {

    }

    /// <summary>
    /// DO for validate BankTransfer
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateBankTransfer_MetaData))]
    public class CTS180_ValidateBankTransfer : CTS180_RegisterContractDocumentData
    {

    }
    [MetadataType(typeof(CTS180_ValidateBankTransferUsd_MetaData))]
    public class CTS180_ValidateBankTransferUsd : CTS180_RegisterContractDocumentData
    {

    }

    /// <summary>
    /// DO for validate CancelQuotationContract
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateCancelQuotationContract_MetaData))]
    public class CTS180_ValidateCancelQuotationContract : CTS180_RegisterContractDocumentData
    {

    }

    /// <summary>
    /// DO for validate ChangeFee Memo
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateChangeFeeMemo_MetaData))]
    public class CTS180_ValidateChangeFeeMemo : CTS180_RegisterContractDocumentData
    {

    }

    /// <summary>
    /// DO for validate ProcessAfterCounterBalanceType
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateProcessAfterCounterBalanceType_MetaData))]
    public class CTS180_ValidateProcessAfterCounterBalanceType : CTS180_RegisterContractDocumentData
    {

    }
    #region ValidateCancelCC

    /// <summary>
    /// DO for validate FeeType HandlingTypeCC
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateFeeTypeHandlingTypeCC_MetaData))]
    public class CTS180_ValidateFeeTypeHandlingTypeCC : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate Contract MaintenanceFeeCC
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateContractMaintenanceFeeCC_MetaData))]
    public class CTS180_ValidateContractMaintenanceFeeCC : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate DepositCard OtherFeeCC
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateDepositCardOtherFeeCC_MetaData))]
    public class CTS180_ValidateDepositCardOtherFeeCC : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate RemovalFeeCC
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateRemovalFeeCC_MetaData))]
    public class CTS180_ValidateRemovalFeeCC : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate ChangeFeeCC
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateChangeFeeCC_MetaData))]
    public class CTS180_ValidateChangeFeeCC : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate CancelFeeCC
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateCancelFeeCC_MetaData))]
    public class CTS180_ValidateCancelFeeCC : tbt_CancelContractMemoDetail
    {

    }

    #endregion

    #region ValidateCancelQC

    /// <summary>
    /// DO for validate FeeType HandlingTypeQC
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateFeeTypeHandlingTypeQC_MetaData))]
    public class CTS180_ValidateFeeTypeHandlingTypeQC : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate Contract MaintenanceFeeQC
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateContractMaintenanceFeeQC_MetaData))]
    public class CTS180_ValidateContractMaintenanceFeeQC : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate DepositCard OtherFeeQC
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateDepositCardOtherFeeQC_MetaData))]
    public class CTS180_ValidateDepositCardOtherFeeQC : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate RemovalFeeQC
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateRemovalFeeQC_MetaData))]
    public class CTS180_ValidateRemovalFeeQC : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate ChangeFeeQC
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateChangeFeeQC_MetaData))]
    public class CTS180_ValidateChangeFeeQC : tbt_CancelContractMemoDetail
    {

    }

    /// <summary>
    /// DO for validate CancelFeeQC
    /// </summary>
    [MetadataType(typeof(CTS180_ValidateCancelFeeQC_MetaData))]
    public class CTS180_ValidateCancelFeeQC : tbt_CancelContractMemoDetail
    {

    }

    #endregion
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS180_doSearchContractDocCondition_MetaData
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string DocStatus { get; set; }

        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string ContractCode { get; set; }

        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string QuotationTargetCode { get; set; }

        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string ProjectCode { get; set; }

        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string OCC { get; set; }

        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string Alphabet { get; set; }

        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string ContractOfficeCode { get; set; }

        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string OperationOfficeCode { get; set; }

        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string NegotiationStaffEmpNo { get; set; }

        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string NegotiationStaffEmpName { get; set; }

        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006)]
        public string DocumentCode { get; set; }
    }

    public class CTS180_ValidateContract_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblContractFee",
                ControlName = "OrderFee0")]
        public decimal? ContractFee { get; set; }
    }

    public class CTS180_ValidateChangeMemo_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblChangeDate",
                ControlName = "CM_ChangeDate")]
        public Nullable<System.DateTime> EffectiveDate { get; set; }
    }

    public class CTS180_ValidateCancelContract_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblCancelDate",
                ControlName = "CC_CancelDate")]
        public Nullable<System.DateTime> CancelContractDate { get; set; }
    }

    public class CTS180_ValidateAutoTransfer_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblAutoTransferBillingAmount",
                ControlName = "CC_AutoTransferBillingAmount")]
        public decimal? AutoTransferBillingAmt { get; set; }
    }

    public class CTS180_ValidateBankTransfer_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblBankTransferBillingAmountRp",
                ControlName = "CC_BankTransferBillingAmount")]
        public decimal? BankTransferBillingAmt { get; set; }
    }
    public class CTS180_ValidateBankTransferUsd_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblBankTransferBillingAmountUsd",
                ControlName = "CC_BankTransferBillingAmountUsd")]
        public decimal? BankTransferBillingAmtUsd { get; set; }
    }

    public class CTS180_ValidateCancelQuotationContract_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblCancelDate",
                ControlName = "QC_CancelDate")]
        public Nullable<System.DateTime> CancelContractDate { get; set; }
    }

    public class CTS180_ValidateChangeFeeMemo_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblChangeContractFeeDate",
                ControlName = "CHF_ChangeContractFeeDate")]
        public Nullable<System.DateTime> ChangeContractFeeDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblNewContractFee",
                ControlName = "CHF_NewContractFee")]
        public decimal? NewContractFee { get; set; }
    }

    #region ValidateCancelCC

    public class CTS180_ValidateFeeTypeHandlingTypeCC_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFeeType",
                ControlName = "CC_FeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblHandlingType",
                ControlName = "CC_HandlingType")]
        public string HandlingType { get; set; }
    }

    public class CTS180_ValidateContractMaintenanceFeeCC_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFeeType",
                ControlName = "CC_FeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblHandlingType",
                ControlName = "CC_HandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFee",
                ControlName = "CC_Fee")]
        public Nullable<decimal> FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblTax",
                ControlName = "CC_Tax")]
        public Nullable<decimal> TaxAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblPeriod",
                ControlName = "CC_PeriodFrom")]
        public Nullable<System.DateTime> StartPeriodDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS180",
            Parameter = "lblPeriod",
            ControlName = "CC_PeriodTo")]
        public Nullable<System.DateTime> EndPeriodDate { get; set; }
    }

    public class CTS180_ValidateDepositCardOtherFeeCC_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFeeType",
                ControlName = "CC_FeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblHandlingType",
                ControlName = "CC_HandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFee",
                ControlName = "CC_Fee")]
        public Nullable<decimal> FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblTax",
                ControlName = "CC_Tax")]
        public Nullable<decimal> TaxAmount { get; set; }
    }

    public class CTS180_ValidateRemovalFeeCC_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFeeType",
                ControlName = "CC_FeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblHandlingType",
                ControlName = "CC_HandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFee",
                ControlName = "CC_Fee")]
        public Nullable<decimal> FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblTax",
                ControlName = "CC_Tax")]
        public Nullable<decimal> TaxAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblNormalFee",
                ControlName = "CC_NormalFee")]
        public Nullable<decimal> NormalFeeAmount { get; set; }
    }

    public class CTS180_ValidateChangeFeeCC_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFeeType",
                ControlName = "CC_FeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblHandlingType",
                ControlName = "CC_HandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFee",
                ControlName = "CC_Fee")]
        public Nullable<decimal> FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblTax",
                ControlName = "CC_Tax")]
        public Nullable<decimal> TaxAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblPeriod",
                ControlName = "CC_PeriodFrom")]
        public Nullable<System.DateTime> StartPeriodDate { get; set; }
    }

    public class CTS180_ValidateCancelFeeCC_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFeeType",
                ControlName = "CC_FeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblHandlingType",
                ControlName = "CC_HandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFee",
                ControlName = "CC_Fee")]
        public Nullable<decimal> FeeAmount { get; set; }
    }

    public class CTS180_ValidateProcessAfterCounterBalanceType_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblSelectProcessAfterCounterBalRp")]
        public string ProcessAfterCounterBalanceType { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblSelectProcessAfterCounterBalUsd")]
        public string ProcessAfterCounterBalanceTypeUsd { get; set; }
    }

    #endregion

    #region ValidateCancelQC

    public class CTS180_ValidateFeeTypeHandlingTypeQC_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFeeType",
                ControlName = "QC_FeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblHandlingType",
                ControlName = "QC_HandlingType")]
        public string HandlingType { get; set; }
    }

    public class CTS180_ValidateContractMaintenanceFeeQC_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFeeType",
                ControlName = "QC_FeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblHandlingType",
                ControlName = "QC_HandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFee",
                ControlName = "QC_Fee")]
        public Nullable<decimal> FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblTax",
                ControlName = "QC_Tax")]
        public Nullable<decimal> TaxAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblPeriod",
                ControlName = "QC_PeriodFrom")]
        public Nullable<System.DateTime> StartPeriodDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS180",
            Parameter = "lblPeriod",
            ControlName = "QC_PeriodTo")]
        public Nullable<System.DateTime> EndPeriodDate { get; set; }
    }

    public class CTS180_ValidateDepositCardOtherFeeQC_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFeeType",
                ControlName = "QC_FeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblHandlingType",
                ControlName = "QC_HandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFee",
                ControlName = "QC_Fee")]
        public Nullable<decimal> FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblTax",
                ControlName = "QC_Tax")]
        public Nullable<decimal> TaxAmount { get; set; }
    }

    public class CTS180_ValidateRemovalFeeQC_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFeeType",
                ControlName = "QC_FeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblHandlingType",
                ControlName = "QC_HandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFee",
                ControlName = "QC_Fee")]
        public Nullable<decimal> FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblTax",
                ControlName = "QC_Tax")]
        public Nullable<decimal> TaxAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblNormalFee",
                ControlName = "QC_NormalFee")]
        public Nullable<decimal> NormalFeeAmount { get; set; }
    }

    public class CTS180_ValidateChangeFeeQC_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFeeType",
                ControlName = "QC_FeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblHandlingType",
                ControlName = "QC_HandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFee",
                ControlName = "QC_Fee")]
        public Nullable<decimal> FeeAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblTax",
                ControlName = "QC_Tax")]
        public Nullable<decimal> TaxAmount { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblPeriod",
                ControlName = "QC_PeriodFrom")]
        public Nullable<System.DateTime> StartPeriodDate { get; set; }
    }

    public class CTS180_ValidateCancelFeeQC_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFeeType",
                ControlName = "QC_FeeType")]
        public string BillingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblHandlingType",
                ControlName = "QC_HandlingType")]
        public string HandlingType { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                Screen = "CTS180",
                Parameter = "lblFee",
                ControlName = "QC_Fee")]
        public Nullable<decimal> FeeAmount { get; set; }
    }

    #endregion
}
