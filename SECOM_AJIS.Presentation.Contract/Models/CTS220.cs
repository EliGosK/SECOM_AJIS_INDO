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
    /// Parameter of CTS220 screen
    /// </summary>
    public class CTS220_Parameter
    {
        public string contractCode { get; set; }
    }

    /// <summary>
    /// DO of Employee
    /// </summary>
    public class CTS220_DOEmployee : tbm_Employee
    {
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
    }

    /// <summary>
    /// DO of ProductName
    /// </summary>
    public class CTS220_DOProductName : tbm_Product
    {
        [LanguageMappingAttribute]
        public string ProductName { get; set; }
    }

    /// <summary>
    /// DO of SelectProcess
    /// </summary>
    [MetadataType(typeof(CTS220_DOSelectProcess_Meta))]
    public class CTS220_DOSelectProcess
    {
        public string ContractCode { get; set; }
        public string OCC { get; set; }
        public string ProcessType { get; set; }
    }

    /// <summary>
    /// DO for validate business
    /// </summary>
    public class CTS220_ValidateBusiness
    {
        public string ContractCode { get; set; }
        public string OCC { get; set; }

        public string ContractFeeCurrencyType { get; set; }
        public decimal? ContractFee { get; set; }
        public string ContractFeeOnStopCurrencyType { get; set; }
        public decimal? ContractFeeOnStop { get; set; } //Add by Jutarat A. on 14082012
        public string ProcessType { get; set; }
        public DateTime? ChangeImplementDate { get; set; }
        public string ChangeType { get; set; }
        public string ChangeReasonType { get; set; }
        public string SecurityMemo { get; set; }
        //public string OperationTypeCode { get; set; }
        public string InsuranceTypeCode { get; set; }

        public string InsuranceCoverageAmountCurrencyType { get; set; }
        public decimal? InsuranceCoverageAmount { get; set; }
        public string MonthlyInsuranceFeeCurrencyType { get; set; }
        public decimal? MonthlyInsuranceFee { get; set; }

        public string DocumentCode { get; set; }
        public string PlanCode { get; set; }

        public string MaintenanceFee1CurrencyType { get; set; }
        public decimal? MaintenanceFee1 { get; set; }
        public string AdditionalFee1CurrencyType { get; set; }
        public decimal? AdditionalFee1 { get; set; }
        public string AdditionalFee2CurrencyType { get; set; }
        public decimal? AdditionalFee2 { get; set; }
        public string AdditionalFee3CurrencyType { get; set; }
        public decimal? AdditionalFee3 { get; set; }

        public string InstallationTypeCode { get; set; }
        public DateTime? InstallationCompleteDate { get; set; }

        public string NormalInstallFeeCurrencyType { get; set; }
        public decimal? NormalInstallFee { get; set; }
        public string OrderInstallFeeCurrencyType { get; set; }
        public decimal? OrderInstallFee { get; set; }
        public string OrderInstallFee_ApproveContractCurrencyType { get; set; }
        public decimal? OrderInstallFee_ApproveContract { get; set; }
        public string OrderInstallFee_CompleteInstallCurrencyType { get; set; }
        public decimal? OrderInstallFee_CompleteInstall { get; set; }
        public string OrderInstallFee_StartServiceCurrencyType { get; set; }
        public decimal? OrderInstallFee_StartService { get; set; }

        public DateTime? ReturnToOriginalFeeDate { get; set; }
        public DateTime? ExpectedResumeDate { get; set; }
        public string SalesmanEmpNo1 { get; set; }
        public string SalesmanEmpNo2 { get; set; }
        public string NegotiationStaffEmpNo1 { get; set; }
        public string NegotiationStaffEmpNo2 { get; set; }
        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
        public bool IsSendNotifyEmail { get; set; }

        public bool IsEnableReasonType { get; set; }
        public List<string> OperationType { get; set; }

        public string NormalContractFeeCurrencyType { get; set; }
        public decimal? NormalContractFee { get; set; } //Add by Jutarat A. on 06022014

        public string InstallFeePaidBySECOMCurrencyType { get; set; }
        public decimal? InstallFeePaidBySECOM { get; set; }
        public string InstallFeeRevenueBySECOMCurrencyType { get; set; }
        public decimal? InstallFeeRevenueBySECOM { get; set; }
    }

    /// <summary>
    /// DO of Product
    /// </summary>
    public class CTS220_DOProductInformation
    {
        public string OrderContractFeeCurrencyType { get; set; }
        public string OrderContractFee { get; set; }
        public string ContractFeeOnStopCurrencyType { get; set; }
        public string ContractFeeOnStop { get; set; } //Add by Jutarat A. on 14082012
        public DateTime? ChangeImplementDate { get; set; }
        public string SecurityTypeCode { get; set; }
        public string ProductCode { get; set; }
        public string ChangeType { get; set; }
        public string ChangeReasonType { get; set; }
        public string SaleManEmpNo1 { get; set; }
        public string SaleManEmpName1 { get; set; }

        public string SaleManEmpNo2 { get; set; }
        public string SaleManEmpName2 { get; set; }

        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string CreateByName { get; set; }
        public string SecurityMemo { get; set; }
        public string OperationTypeCode { get; set; }
    }

    /// <summary>
    /// DO of InsuranceType
    /// </summary>
    public class CTS220_DOInsuranceType
    {
        public string InsuranceTypeCode { get; set; }
        public string InsuranceCoverageAmountCurrencyType { get; set; }
        public string InsuranceCoverageAmount { get; set; }
        public string MonthlyInsuranceFeeCurrencyType { get; set; }
        public string MonthlyInsuranceFee { get; set; }
    }

    /// <summary>
    /// DO of FutureDate
    /// </summary>
    public class CTS220_DOFutureDate
    {
        public DateTime? ExpectedResumeDate { get; set; }
        public DateTime? ReturnToOriginalFeeDate { get; set; }
    }

    /// <summary>
    /// DO of Quotation
    /// </summary>
    public class CTS220_DOQuotation
    {
        public string QuotationTargetCode { get; set; }
        public string QuotationAlphabet { get; set; }
        public string PlanCode { get; set; }
        public DateTime? PlanApproveDate { get; set; }
        public string PlanApproverEmpNo { get; set; }
        public string PlanApproverEmpName { get; set; }
        public string NormalContractFeeCurrencyType { get; set; }
        public string NormalContractFee { get; set; }
        public string MaintenanceFee1CurrencyType { get; set; }
        public string MaintenanceFee1 { get; set; }
        public string AdditionalFee1CurrencyType { get; set; }
        public string AdditionalFee1 { get; set; }
        public string AdditionalFee2CurrencyType { get; set; }
        public string AdditionalFee2 { get; set; }
        public string AdditionalFee3CurrencyType { get; set; }
        public string AdditionalFee3 { get; set; }
    }

    /// <summary>
    /// DO of Installation
    /// </summary>
    public class CTS220_DOInstallation
    {
        public string InstallationTypeCode { get; set; }
        public DateTime? InstallationCompleteDate { get; set; }

        public string NormalInstallFeeCurrencyType { get; set; }
        public string NormalInstallFee { get; set; }
        public string OrderInstallFeeCurrencyType { get; set; }
        public string OrderInstallFee { get; set; }
        public string OrderInstallFee_ApproveContractCurrencyType { get; set; }
        public string OrderInstallFee_ApproveContract { get; set; }
        public string OrderInstallFee_CompleteInstallCurrencyType { get; set; }
        public string OrderInstallFee_CompleteInstall { get; set; }
        public string OrderInstallFee_StartServiceCurrencyType { get; set; }
        public string OrderInstallFee_StartService { get; set; }
        public string InstallFeePaidBySECOMCurrencyType { get; set; }
        public string InstallFeePaidBySECOM { get; set; }
        public string InstallFeeRevenueBySECOMCurrencyType { get; set; }
        public string InstallFeeRevenueBySECOM { get; set; }
        public string InstallationSlipNoCurrencyType { get; set; }
        public string InstallationSlipNo { get; set; }
    }

    /// <summary>
    /// DO of SubContractor
    /// </summary>
    [MetadataType(typeof(CTS220_DOSubContract_Meta))]
    public class CTS220_DOSubContract
    {
        public string SubContractCode { get; set; }
        public string ProcessType { get; set; }
    }

    /// <summary>
    /// Parameter of CTS220 screen
    /// </summary>
    public class CTS220_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public string ContractCode { get; set; }

        [KeepSession]
        public CTS220_Parameter ScreenParameter { get; set; }

        public CTS220_ValidateBusiness DOValidateBusiness { get; set; }
        public dsRentalContractData DSRentalContract { get; set; }
        public dsRentalContractData DSRentalContractPrevious { get; set; }
        public dsRentalContractData DSRentalContractNext { get; set; }
        public dsRentalContractData DSRentalContractShow { get; set; }

        public List<tbt_RentalSecurityBasic> ListRentalSecurityBasic { get; set; }
        public List<dtTbt_RentalInstSubContractorListForView> ListSubContractor { get; set; }
        public bool IsSendNotifyEmail { get; set; }

        public string ProcessType { get; set; }
    }

    /// <summary>
    /// DO for validate require field
    /// </summary>
    [MetadataType(typeof(CTS220_ValidateRequireField_MetaData))]
    public class CTS220_ValidateRequireField
    {
        public DateTime? ChangeImplementDate { get; set; }
        public string SalesmanEmpNo1 { get; set; }
    }

    /// <summary>
    /// DO for validate ChangeReasonType
    /// </summary>
    [MetadataType(typeof(CTS220_ValidateChangeReasonType_MetaData))]
    public class CTS220_ValidateChangeReasonType
    {
        public string ChangeReasonType { get; set; }
    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS220_DOSelectProcess_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT, Screen = "CTS220", Parameter = "lblOccurence", ControlName = "Occurence")]
        public string OCC { get; set; }
    }

    public class CTS220_DOSubContract_Meta
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT, Screen = "CTS220", Parameter = "lblSubContractorName", ControlName = "SubcontractCode")]
        public string SubContractCode { get; set; }
    }

    public class CTS220_ValidateRequireField_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS220",
            Parameter = "lblChangeOperationDate",
            ControlName = "ChangeImplementDate")]
        public DateTime? ChangeImplementDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS220",
            Parameter = "lblSalesMan1",
            ControlName = "SaleManEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }
    }

    public class CTS220_ValidateChangeReasonType_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS220",
            Parameter = "lblReason",
            ControlName = "ChangeReasonType")]
        public string ChangeReasonType { get; set; }
    }
}
