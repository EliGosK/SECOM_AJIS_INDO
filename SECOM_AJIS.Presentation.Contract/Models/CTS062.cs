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
    /// Parameter of CTS062 screen
    /// </summary>
    public class CTS062_Parameter
    {
        public string contractCode { get; set; }
    }

    /// <summary>
    /// DO of ScreenOutput
    /// </summary>
    public class CTS062_ScreenOutputObject
    {
        public string LastOCC { get; set; }
        public bool CanOperate { get; set; }
        public string ExpectedInstallCompleteDate { get; set; }
        public string InstallationStatusCode { get; set; }
        public string ContractCode { get; set; }
        public string ContractCodeShort { get; set; }
        public string PurchaserCustCode { get; set; }
        public string RealCustomerCustCode { get; set; }
        public string SiteCode { get; set; }
        public bool ImportantFlag { get; set; }
        public string PurchaserNameEN { get; set; }
        public string PurchaserAddressEN { get; set; }
        public string SiteNameEN { get; set; }
        public string SiteAddressEN { get; set; }
        public string PurchaserNameLC { get; set; }
        public string PurchaserAddressLC { get; set; }
        public string SiteNameLC { get; set; }
        public string SiteAddressLC { get; set; }
        public string InstallationStatusCodeName { get; set; }
        public string OperationOfficeName { get; set; }
        public string TargetCodeType { get; set; }
        public string ServiceTypeCode { get; set; }

    }

    /// <summary>
    /// DO of retrieved data
    /// </summary>
    [MetadataType(typeof(CTS062_DORetrieveData_MetaData))]
    public class CTS062_DORetrieveData
    {
        public string QuotationTargetCode { get; set; }
        public string Alphabet { get; set; }
    }

    /// <summary>
    /// Parameter for register data
    /// </summary>
    [MetadataType(typeof(CTS062_DORegisterData_MetaData))]
    public class CTS062_DORegisterData
    {
        public string Alphabet { get; set; }
        public DateTime? ExpectedInstallCompleteDate { get; set; }
        public DateTime? ExpectedCustAcceptanceDate { get; set; }

        //public string NormalSalePrice { get; set; }
        //public string OrderSalePrice { get; set; }
        //public string PartialFee { get; set; }
        //public string BillingAmt_Acceptance { get; set; }
        //public string BillingAmt_ApproveContract { get; set; }
        //public string BillingAmt_CompleteInstallation { get; set; }

        //public string PaymethodCompleteInstallation { get; set; }

        public string OrderProductPriceCurrencyType { get; set; }
        public string OrderProductPrice { get; set; }
        public string BillingAmt_ApproveContractCurrencyType { get; set; }
        public string BillingAmt_ApproveContract { get; set; }
        public string BillingAmt_PartialFeeCurrencyType { get; set; }
        public string BillingAmt_PartialFee { get; set; }
        public string BillingAmt_AcceptanceCurrencyType { get; set; }
        public string BillingAmt_Acceptance { get; set; }

        public string OrderInstallFeeCurrencyType { get; set; }
        public string OrderInstallFee { get; set; }
        public string BillingAmtInstallation_ApproveContractCurrencyType { get; set; }
        public string BillingAmtInstallation_ApproveContract { get; set; }
        public string BillingAmtInstallation_PartialFeeCurrencyType { get; set; }
        public string BillingAmtInstallation_PartialFee { get; set; }
        public string BillingAmtInstallation_AcceptanceCurrencyType { get; set; }
        public string BillingAmtInstallation_Acceptance { get; set; }
        
        public string NegotiationStaffEmpNo1 { get; set; }
        public string NegotiationStaffEmpNo2 { get; set; }

        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
        public string ApproveNo3 { get; set; }
        public string ApproveNo4 { get; set; }
        public string ApproveNo5 { get; set; }

        public bool? ConnectionFlag { get; set; }
        public string ConnectTargetCode { get; set; }

        public string DistributedType { get; set; }
        public string DistributedCode { get; set; }

        public string SalePrice_ApprovalCurrencyType { get; set; }
        public string SalePrice_Approval { get; set; }
        public string SalePrice_PartialCurrencyType { get; set; }
        public string SalePrice_Partial { get; set; }
        public string SalePrice_AcceptanceCurrencyType { get; set; }
        public string SalePrice_Acceptance { get; set; }
        public string SalePrice_PaymentMethod_Acceptance { get; set; }

        public string InstallationFee_ApprovalCurrencyType { get; set; }
        public string InstallationFee_Approval { get; set; }
        public string InstallationFee_PartialCurrencyType { get; set; }
        public string InstallationFee_Partial { get; set; }
        public string InstallationFee_AcceptanceCurrencyType { get; set; }
        public string InstallationFee_Acceptance { get; set; }
        public string InstallationFee_PaymentMethod_Acceptance { get; set; }
    }

    /// <summary>
    /// DO of ChangePlan
    /// </summary>
    public class CTS062_DOChangePlan
    {
        public string ID { get; set; } //COLUMN

        public string NormalPriceCurrencyType { get; set; }
        public string NormalPrice { get; set; } //COLUMN NORMAL
        public string OrderPriceCurrencyType { get; set; }
        public string OrderPrice { get; set; } //COLUMN
        public string BillingAmt_ApproveContractCurrencyType { get; set; }
        public string BillingAmt_ApproveContract { get; set; }
        public string BillingAmt_PartialFeeCurrencyType { get; set; }
        public string BillingAmt_PartialFee { get; set; }
        public string BillingAmt_AcceptanceCurrencyType { get; set; }
        public string BillingAmt_Acceptance { get; set; }
    }

    /// <summary>
    /// DO of ChangePlan detail
    /// </summary>
    public class CTS062_DOChangePlanDetail
    {
        public string Alphabet { get; set; }
        public string ProductName { get; set; }
        public bool? ConnectionFlag { get; set; }
        public string ConnectTargetCode { get; set; }

        public string DistributedType { get; set; }
        public string DistributedCode { get; set; }

        public DateTime? ExpectedInstallCompleteDate { get; set; }
        public DateTime? ExpectedCustAcceptanceDate { get; set; }

        public string SalesmanEmpNo1 { get; set; } 
        public string SalesmanEmpNo2 { get; set; } 
        public string SalesmanEmpNo3 { get; set; } 
        public string SalesmanEmpNo4 { get; set; } 
        public string SalesmanEmpNo5 { get; set; } 
        public string SalesmanEmpNo6 { get; set; } 
        public string SalesmanEmpNo7 { get; set; } 
        public string SalesmanEmpNo8 { get; set; }
        public string SalesmanEmpNo9 { get; set; }
        public string SalesmanEmpNo10 { get; set; }

        public string SalesmanEmpName1 { get; set; } 
        public string SalesmanEmpName2 { get; set; } 
        public string SalesmanEmpName3 { get; set; } 
        public string SalesmanEmpName4 { get; set; } 
        public string SalesmanEmpName5 { get; set; } 
        public string SalesmanEmpName6 { get; set; } 
        public string SalesmanEmpName7 { get; set; } 
        public string SalesmanEmpName8 { get; set; } 
        public string SalesmanEmpName9 { get; set; } 
        public string SalesmanEmpName10 { get; set; }

        public string NegotiationStaffEmpNo1 { get; set; }
        public string NegotiationStaffEmpNo2 { get; set; }
        public string NegotiationStaffEmpName1 { get; set; }
        public string NegotiationStaffEmpName2 { get; set; }

        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
        public string ApproveNo3 { get; set; }
        public string ApproveNo4 { get; set; }
        public string ApproveNo5 { get; set; }
    }

    /// <summary>
    /// DO of BillingTarget detail
    /// </summary>
    public class CTS062_DOBillingTargetDetailData
    {
        //For billing target detail---------------------------------

        public string BillingTargetCodeDetail { get; set; }
        public string BillingClientCodeDetail { get; set; }
        public string FullNameEN { get; set; }
        public string BranchNameEN { get; set; }
        public string AddressEN { get; set; }
        public string FullNameLC { get; set; }
        public string BranchNameLC { get; set; }
        public string AddressLC { get; set; }
        public string Nationality { get; set; }
        public string PhoneNo { get; set; }
        public string BusinessType { get; set; }
        public string IDNo { get; set; }
        public string BillingOffice { get; set; }
        public string BillingOCC { get; set; }
        public string Sequence { get; set; }
        public string Status { get; set; }

        public string BillingContractFeeDetail { get; set; }
        public string BillingDepositFee { get; set; }
        public string BillingInstallationApprovalFee { get; set; }
        public string BillingInstallationCompleteFee { get; set; }
        public string BillingInstallationStartServiceFee { get; set; }
        public string BillingTotalFee { get; set; }

        public string PayMethodCompleteFee { get; set; }
        public string PayMethodStartServiceFee { get; set; }
        public string PayMethodDepositFee { get; set; }
        //----------------------------------------------------------
    }

    /// <summary>
    /// DO of BillingTarget detail grid
    /// </summary>
    public class CTS062_DOBillingTargetDetailGridData
    {
        public string ID { get; set; }
        public string AmountCurrencyType { get; set; }
        public string Amount { get; set; }
        public string PayMethod { get; set; }
    }

    /// <summary>
    /// DO for validate business
    /// </summary>
    public class CTS062_DOValidateBusiness
    {
        public string NormalSalePrice { get; set; }
        //public string OrderSalePrice { get; set; }
        public string BillingAmt_ApproveContract { get; set; }
        public string PartialFee { get; set; }
        public string BillingAmt_Acceptance { get; set; }
        //public string PaymethodCompleteInstallation { get; set; }

        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
        public string ApproveNo3 { get; set; }
        public string ApproveNo4 { get; set; }
        public string ApproveNo5 { get; set; }

        public string NegotiationStaffEmpNo1 { get; set; }
        public string NegotiationStaffEmpNo2 { get; set; }

        public DateTime? ExpectedInstallCompleteDate { get; set; }
        public DateTime? ExpectedCustAcceptanceDate { get; set; }

        public string DistributedType { get; set; }
        public string DistributedCode { get; set; }

    }

    /// <summary>
    /// DO for show data
    /// </summary>
    public class CTS062_DOShowData
    {
        public string ProductName { get; set; }
        public string SaleType { get; set; }
        public string SaleManName { get; set; }
    }
    
    /// <summary>
    /// DO of ProductName
    /// </summary>
    public class CTS062_DOProductName : tbm_Product
    {
        [LanguageMappingAttribute]
        public string ProductName { get; set; }
    }

    /// <summary>
    /// DO of Employee
    /// </summary>
    public class CTS062_DOEmployee : tbm_Employee
    {
        public string EmpName { get; set; }
    }

    /// <summary>
    /// DO of LastOCC
    /// </summary>
    public class CTS062_DOLastOCC
    {
        public string LastOCC { get; set; }
    }

    /// <summary>
    /// DO of BillingClient
    /// </summary>
    public class CTS062_DTBillingClientData : dtBillingClientData
    {
    }

    /// <summary>
    /// Parameter of CTS062 screen
    /// </summary>
    public class CTS062_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public CTS062_Parameter ScreenParameter { get; set; }

        [KeepSession]
        public CTS062_DOValidateBusiness DOValidateBusiness { get; set; }
        [KeepSession]
        public CTS062_DOLastOCC DOLastOCC { get; set; }
        [KeepSession]
        public dsSaleContractData DSSaleContract { get; set; }
        [KeepSession]
        public dsQuotationData DSQuotation { get; set; }
        [KeepSession]
        public List<tbt_BillingTemp> ListBillingTemp { get; set; }
        [KeepSession]
        public List<dtBillingClientData> ListDTBillingClient { get; set; }
        [KeepSession]
        public List<doMiscTypeCode> ListDOMiscTypeCode { get; set; }

        [KeepSession]
        public string ContractCode { get; set; }
    }

    /// <summary>
    /// DO for check valid retrieved data
    /// </summary>
    public class CTS062_RetrieveValid
    {
        public bool CanRetrieve { get; set; }
        public string MessageCode { get; set; }
    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS062_DORetrieveData_MetaData
    {
        [NotNullOrEmpty(ControlName = "Alphabet", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblAlphabet", Screen = "CTS062")]
        public string Alphabet { get; set; }
    }  

    public class CTS062_DORegisterData_MetaData
    {
        [NotNullOrEmpty(ControlName = "ExpectedInstallCompleteDate", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblExpectedInstallationCompleteDate", Screen = "CTS062")]      
        public DateTime? ExpectedInstallCompleteDate { get; set; }

        [NotNullOrEmpty(ControlName = "ExpectedCustAcceptanceDate", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblExpectedCustomerAcceptanceDate", Screen = "CTS062")]      
        public DateTime? ExpectedCustAcceptanceDate { get; set; }

        [NotNullOrEmpty(ControlName = "OrderProductPrice", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblOrderProductPrice", Screen = "CTS062")]
        public string OrderProductPrice { get; set; }

        [NotNullOrEmpty(ControlName = "OrderInstallFee", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblOrderInstallFee", Screen = "CTS062")]
        public string OrderInstallFee { get; set; }

        ////[NotNullOrEmpty(ControlName = "OrderSalePrice", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblOrderSalePrice", Screen = "CTS062")]
        ////public string OrderSalePrice { get; set; }

        [NotNullOrEmpty(ControlName = "NegotiationStaffEmpNo1", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblNegotiationStaff1", Screen = "CTS062")]
        public string NegotiationStaffEmpNo1 { get; set; }

        //[NotNullOrEmpty(ControlName = "PaymethodCompleteInstallation", Module = MessageUtil.MODULE_COMMON, Parameter = "Payment method complete installation")]
        //public string PaymethodCompleteInstallation { get; set; }       

    }
}