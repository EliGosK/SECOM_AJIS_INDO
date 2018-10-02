using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using SECOM_AJIS.Presentation.Contract.Models;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Quotation;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// DO for validate retrieved data 
    /// </summary>
    [MetadataType(typeof(CTS052_DOValidateRetrieveData_Meta))]
    public class CTS052_DOValidateRetrieveData
    {
        public string Alphabet { get; set; }
    }

    /// <summary>
    /// DO of screen output
    /// </summary>
    public class CTS052_ScreenOutputObject
    {
        public string ContractCodeShort { get; set; }
        public string ContractCode { get; set; }
        public string OCC { get; set; }
        public string ContractStatus { get; set; }
        public string Alphabet { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string DisplayAll { get; set; }
        public string BillingClientCode { get; set; }
        public string BillingOffice { get; set; }
        public string PaymentMethod { get; set; }
        public string Sequence { get; set; }
        public string ServiceTypeCode { get; set; }
        public string TargetCodeType { get; set; }
        public string EndContractDate { get; set; }
        public string InstallationStatusCode { get; set; }
        public string UserCode { get; set; }
        public string CustomerCode { get; set; }
        public string RealCustomerCode { get; set; }
        public string SiteCode { get; set; }
        public string CustFullNameEN { get; set; }
        public string AddressFullEN { get; set; }
        public string SiteName { get; set; }
        public string SiteAddress { get; set; }
        public string CustFullNameLC { get; set; }
        public string AddressFullLC { get; set; }
        public string SiteNameLC { get; set; }
        public string SiteAddressLC { get; set; }
        public string InstallationStatus { get; set; }
        public string OfficeName { get; set; }
        public string QuotationTargetCode { get; set; }
        public string ExpectOperationDate { get; set; }
        public string NegotiationStaffEmpNo1 { get; set; }
        public bool ImportantFlag { get; set; }

    }

    /// <summary>
    /// DO for validate business
    /// </summary>
    [MetadataType(typeof(CTS052_DOValidateBusinessData_Meta))]
    public class CTS052_DOValidateBusinessData
    {
        public string QuotationTargetCode { get; set; }
        public string Alphabet { get; set; }
        public DateTime? ChangeImplementDate { get; set; }

        public string NegotiationStaffEmpNo1 { get; set; }
        public string NegotiationStaffEmpNo2 { get; set; }

        public string NegotiationStaffEmpName1 { get; set; }
        public string NegotiationStaffEmpName2 { get; set; }

        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
    }

    /// <summary>
    /// DO of Quotation
    /// </summary>
    [MetadataType(typeof(CTS052_DOQuotation_Meta))]
    public class CTS052_DOQuotation
    {
        public string QuotationTargetCode { get; set; }
        public string Alphabet { get; set; }
        public DateTime? ChangeImplementDate { get; set; }
        public string ChangeImplementDateShow { get; set; }

        public string ContractFeeCurrencyType { get; set; }
        public string ContractFee { get; set; }
        public string OrderContractFeeCurrencyType { get; set; }
        public string OrderContractFee { get; set; }

        public string NegotiationStaffEmpNo1 { get; set; }
        public string NegotiationStaffEmpNo2 { get; set; }

        public string NegotiationStaffEmpName1 { get; set; }
        public string NegotiationStaffEmpName2 { get; set; }

        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
    }

    /// <summary>
    /// DO of active employee
    /// </summary>
    public class CTS052_DOGetActiveEmployeeData
    {
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
    }

    /// <summary>
    /// DO of retrieved BillingClient 
    /// </summary>
    [MetadataType(typeof(CTS052_DORetrieveBillingClientData_Meta))]
    public class CTS052_DORetrieveBillingClientData
    {
        public string BillingClientCode { get; set; }
        public string Mode { get; set; }
    }

    /// <summary>
    /// DO for detail of BillingClient
    /// </summary>
    public class CTS052_DTBillingClientDetailData : dtBillingClientData
    {
        public string BillingOffice { get; set; }
        public string Sequence { get; set; }
    }

    /// <summary>
    /// Parameter of CTS052 screen
    /// </summary>
    public class CTS052_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public string ContractCode { get; set; }

        public CTS052_DOValidateBusinessData DOValidateBusiness { get; set; }
        public CTS052_DOQuotation DOQuotation { get; set; }
        public dsRentalContractData DSRentalContract { get; set; }
        public dsQuotationData DSQuotation { get; set; }
        public List<tbt_RentalMaintenanceDetails> ListRentalMaintenanceDetails { get; set; }
        public doRentalContractBasicInformation DORentalContractBasicInformation { get; set; } 
    }
}

namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    public class CTS052_DOValidateBusinessData_Meta
    {
        [NotNullOrEmpty(ControlName = "Alphabet", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblAlphabet", Screen = "CTS052")]
        public string Alphabet { get; set; }

        [NotNullOrEmpty(ControlName = "ChangeImplementDate", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblRealInvestigationDate", Screen = "CTS052")]
        public DateTime? ChangeImplementDate { get; set; }

        [NotNullOrEmpty(ControlName = "NegotiationStaffEmpNo1", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblNegotiationStaff1", Screen = "CTS052")]
        public string NegotiationStaffEmpNo1 { get; set; }

        [NotNullOrEmpty(ControlName = "ApproveNo1", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblApproveNo1", Screen = "CTS052")]
        public string ApproveNo1 { get; set; }
    }

    public class CTS052_DORetrieveBillingClientData_Meta
    {
        [NotNullOrEmpty(ControlName = "BillingClientCodeDetail", Controller = MessageUtil.MODULE_COMMON, MessageCode = MessageUtil.MessageList.MSG0007, Parameter = "Billing client code")]
        public string BillingClientCode { get; set; }
    }

    public class CTS052_DOValidateRetrieveData_Meta
    {
        [NotNullOrEmpty(ControlName = "Alphabet", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblAlphabet", Screen = "CTS052")]
        public string Alphabet { get; set; }      
    }

    public class CTS052_DOQuotation_Meta
    {
        [NotNullOrEmpty(ControlName = "Alphabet", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblAlphabet", Screen = "CTS052")]
        public string Alphabet { get; set; }

        [NotNullOrEmpty(ControlName = "ChangeImplementDate", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblRealInvestigationDate", Screen = "CTS052")]
        public DateTime? ChangeImplementDate { get; set; }

        [NotNullOrEmpty(ControlName = "ApproveNo1", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblApproveNo1", Screen = "CTS052")]
        public string ApproveNo1 { get; set; }

        [NotNullOrEmpty(ControlName = "NegotiationStaffEmpNo1", Controller = MessageUtil.MODULE_CONTRACT, Parameter = "lblNegotiationStaff1", Screen = "CTS052")]
        public string NegotiationStaffEmpNo1 { get; set; }
    }
}