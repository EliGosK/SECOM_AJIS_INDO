using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Contract;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Enum of ContractType
    /// </summary>
    public enum CTS160_CONTRACT_TYPE
    {
        RENTAL,
        DRAFT_RENTAL,
        SALE
    }

    /// <summary>
    /// Parameter of CTS160 screen
    /// </summary>
    public class CTS160_ScreenParameter : ScreenSearchParameter
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
        public string OCCAlphabet { get; set; }
        [KeepSession]
        public CTS160_CONTRACT_TYPE ContractType { get; set; }

        public dsRentalContractData dsRentalContractData { get; set; }
        public doDraftRentalContractData dsEntireDraftContract { get; set; }
        public doSaleContractData doSaleContractData { get; set; }
        public doRentalContractBasicInformation doRentalContractBasicInformation { get; set; }
        public doDraftRentalContractInformation doDraftRentalContractInformation { get; set; }
        public doSaleContractBasicInformation doSaleContractBasicInformation { get; set; }
        public doCustomer doContractTargetCustomer { get; set; }
        public doCustomer doRealCustomer { get; set; }
        public doSite doSiteData { get; set; }
        public List<dtTbt_BillingTempListForView> dtBillingTemp { get; set; }
        public List<tbt_BillingBasic> dtTbt_BillingBasic { get; set; }

        public CTS160_CoverLetterInformation CoverLetterInfo { get; set; } //Add by Jutarat A. on 19122012
    }

    /// <summary>
    /// DO of DocumentTemplate
    /// </summary>
    public class CTS160_DocumentTemplateData
    {
        public CTS160_CONTRACT_TYPE ContractType { get; set; }
        public string ContractCode { get; set; }
        public string OCCAlphabet { get; set; }
        public string ContractTargetNameEN { get; set; }
        public string RealCustomerNameEN { get; set; }
        public string SiteNameEN { get; set; }
        public string ContractTargetNameLC { get; set; }
        public string RealCustomerNameLC { get; set; }
        public string SiteNameLC { get; set; }
        public string ChangeTypeName { get; set; }
    }

    /// <summary>
    /// DO of DocumentState
    /// </summary>
    public class CTS160_DocumentState
    {
        public bool ContractReportFlag { get; set; }
        public bool MemorandumFlag { get; set; }
        public bool NoticeFlag { get; set; }
        public bool CoverLetterFlag { get; set; }
    }

    /// <summary>
    /// DO of DocumentTemplateCondition
    /// </summary>
    public class CTS160_DocumentTemplateCondition : CTS160_DocumentState
    {
        public string ContractLanguage { get; set; }
        public string DocumentName { get; set; }
        public bool SignatureFlag { get; set; }
    }

    /// <summary>
    /// DO of CoverLetter Information
    /// </summary>
    public class CTS160_CoverLetterInformation
    {
        public string IssuePerson { get; set; }
        public string IssueOffice { get; set; }
        public string Subject { get; set; }
        public string CIContractTargetNameEN { get; set; }
        public string CIContractTargetNameLC { get; set; }
        public string CISiteNameEN { get; set; }
        public string CISiteNameLC { get; set; }
        public string CIContractCode { get; set; }
        public string CIOCC { get; set; }
        public string AttachDocument1 { get; set; }
        public string AttachDocument2 { get; set; }
        public string AttachDocument3 { get; set; }
        public string AttachDocument4 { get; set; }
        public string AttachDocument5 { get; set; }
        public string RelatedOCCIncidentNo1 { get; set; }
        public string RelatedOCCIncidentNo2 { get; set; }
        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
        public string ApproveNo3 { get; set; }
        public string ContractMemo { get; set; }
        public string Product { get; set; }
        [PhoneLineTypeMapping("LineType")]
        public string LineTypeCode { get; set; }
        public string LineType { get; set; }
        [PhoneLineOwnerTypeMapping("TelephoneOwner")]
        public string TelephoneOwnerCode { get; set; }
        public string TelephoneOwner { get; set; }

        public decimal? QuotationFee { get; set; }
        public string QuotationFeeCurrencyType { get; set; }

        public decimal? ContractFee { get; set; }
        public string ContractFeeCurrencyType { get; set; }

        public decimal? ReceivedDepositFee { get; set; }
        public string ReceivedDepositFeeCurrencyType { get; set; }

        [PaymentMethodMapping("PaymentMethod")]
        public string PaymentMethodCode { get; set; }
        public string PaymentMethod { get; set; }
        public int? BillingCycle { get; set; }
        public int? CreditTerm { get; set; }
        public bool? FireMonitoringFlag { get; set; }
        public bool? CrimePreventionFlag { get; set; }
        public bool? EmergencyReportFlag { get; set; }
        public bool? FacilityMonitoringFlag { get; set; }
        public string BusinessType { get; set; }
        public string Usage { get; set; }
        public int? ContractDuration { get; set; }
        public string OldContract { get; set; }
        public string OperationOfficeCode { get; set; }
        public string OperationOffice { get; set; }
    }

    /// <summary>
    /// DO of InstrumentDetail
    /// </summary>
    public class CTS160_InstrumentDetail
    {
        public string InstrumentCode { get; set; }
        public int? InstrumentQty { get; set; }
    }


















    //public class CTS160_ScreenParameter : ScreenParameter
    //{
    //    [KeepSession]
    //    public string ContractCode { get; set; }

    //    public string QuotationTargetCode { get; set; }

    //    [KeepSession]
    //    public string OCCAlphabet { get; set; }

    //    [KeepSession]
    //    public dsRentalContractData RentalContractData { get; set; }

    //    public doDraftRentalContractData DraftRentalContractData { get; set; }

    //    [KeepSession]
    //    public doRentalContractBasicInformation doRentalContractBasicData { get; set; }

    //    [KeepSession]
    //    public List<doCustomer> ContractTargetCustomerList { get; set; }

    //    [KeepSession]
    //    public List<doCustomer> RealCustomerList { get; set; }

    //    [KeepSession]
    //    public List<doSite> SiteList { get; set; }

    //    [KeepSession]
    //    public List<dtTbt_BillingTempListForView> BillingTempListForView { get; set; }

    //    public List<tbm_DocumentTemplate> DocumentTemplateData { get; set; }
    //    public CTS160_DocumentTemplateCondition DocumentTemplateCondition { get; set; }
    //    public CTS160_CoverLetterData CoverLetterData { get; set; }
    //    public List<tbt_BillingBasic> BillingBasicData;

    //    public doDraftRentalContractInformation doDraftRentalContractData { get; set; }
    //}

    //public class CTS160_SpecifyContractCondition
    //{
    //    CommonUtil comUtil = new CommonUtil();

    //    private string _contractCode;
    //    public string ContractCode
    //    {
    //        get { return String.IsNullOrEmpty(_contractCode) == false ? _contractCode.ToUpper() : string.Empty; }
    //        set { _contractCode = value; }
    //    }

    //    public string ContractCodeLong
    //    {
    //        get
    //        {
    //            return comUtil.ConvertContractCode(ContractCode, CommonUtil.CONVERT_TYPE.TO_LONG);
    //        }
    //    }

    //    private string _quotationTargetCode;
    //    public string QuotationTargetCode
    //    {
    //        get
    //        {
    //            if (String.IsNullOrEmpty(_quotationTargetCode) == false)
    //                _quotationTargetCode = _quotationTargetCode.ToUpper();

    //            return _quotationTargetCode;
    //        }
    //        set
    //        {
    //            _quotationTargetCode = value;
    //        }
    //    }

    //    public string QuotationTargetCodeLong
    //    {
    //        get
    //        {
    //            return comUtil.ConvertQuotationTargetCode(QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
    //        }
    //    }

    //    private string _OCCAlphabet;
    //    public string OCCAlphabet
    //    {
    //        get
    //        {
    //            if (String.IsNullOrEmpty(_OCCAlphabet) == false)
    //                _OCCAlphabet = _OCCAlphabet.ToUpper();

    //            return _OCCAlphabet;
    //        }
    //        set
    //        {
    //            _OCCAlphabet = value;
    //        }
    //    }
    //}

    //public class CTS160_SpecifyContractData : doRentalContractBasicInformation
    //{
    //    public string ChangeType { get; set; }
    //    public string DraftStatus { get; set; }
    //    public string SecOCC { get; set; }
    //}

    //public class CTS160_DocumentTemplateCondition
    //{
    //    public bool ContractFlag { get; set; }
    //    public bool MemorandumFlag { get; set; }
    //    public bool NoticeFlag { get; set; }
    //    public bool CoverLetterFlag { get; set; }
    //    public string DocumentName { get; set; }
    //    public string ContractDocumentLanguage { get; set; }
    //    public bool ShowSignature { get; set; }
    //}

    //public class CTS160_CoverLetterData
    //{
    //    public string EmpFullName { get; set; }

    //    public string MainOfficeCode { get; set; }
    //    public string MainOfficeName { get; set; }

    //    public string DocumentCode { get; set; }

    //    [LanguageMapping]
    //    public string DocumentName { get; set; }
    //    public string DocumentNameEN { get; set; }
    //    public string DocumentNameLC { get; set; }
    //    public string DocumentNameJP { get { return DocumentNameEN; } }

    //    public string ContractTargetNameEN { get; set; }
    //    public string ContractTargetNameLC { get; set; }
    //    public string SiteNameEN { get; set; }
    //    public string SiteNameLC { get; set; }

    //    public string ContractCode { get; set; }
    //    public string OCC { get; set; }

    //    public string RelatedNo1 { get; set; }
    //    public string RelatedNo2 { get; set; }
    //    public string AttachDoc1 { get; set; }
    //    public string AttachDoc2 { get; set; }
    //    public string AttachDoc3 { get; set; }
    //    public string AttachDoc4 { get; set; }
    //    public string AttachDoc5 { get; set; }
    //    public string ApproveNo1 { get; set; }
    //    public string ApproveNo2 { get; set; }
    //    public string ApproveNo3 { get; set; }
    //    public string ContactMemo { get; set; }

    //    public string ProductCode { get; set; }

    //    [LanguageMapping]
    //    public string ProductName { get; set; }
    //    public string ProductNameEN { get; set; }
    //    public string ProductNameLC { get; set; }
    //    public string ProductNameJP { get; set; }

    //    public string PhoneLineTypeCode { get; set; }

    //    [LanguageMapping]
    //    public string PhoneLineTypeName { get; set; }
    //    public string PhoneLineTypeNameEN { get; set; }
    //    public string PhoneLineTypeNameLC { get; set; }
    //    public string PhoneLineTypeNameJP { get; set; }

    //    public string PhoneLineOwnerTypeCode { get; set; }

    //    [LanguageMapping]
    //    public string PhoneLineOwnerTypeName { get; set; }
    //    public string PhoneLineOwnerTypeNameEN { get; set; }
    //    public string PhoneLineOwnerTypeNameLC { get; set; }
    //    public string PhoneLineOwnerTypeNameJP { get; set; }

    //    public string OrderContractFee { get; set; }
    //    public string NormalContractFee { get; set; }
    //    public string OrderAdditionalDepositFee { get; set; }

    //    public string OrderContractFeePayMethod { get; set; }

    //    [LanguageMapping]
    //    public string OrderContractFeePayMethodName { get; set; }
    //    public string OrderContractFeePayMethodNameEN { get; set; }
    //    public string OrderContractFeePayMethodNameLC { get; set; }
    //    public string OrderContractFeePayMethodNameJP { get; set; }

    //    public int? CreditTerm { get; set; }
    //    public int? BillingCycle { get; set; }

    //    public bool? FireMonitorFlag { get; set; }
    //    public bool? CrimePreventFlag { get; set; }
    //    public bool? EmergencyReportFlag { get; set; }
    //    public bool? FacilityMonitorFlag { get; set; }

    //    [LanguageMapping]
    //    public string BusinessTypeName { get; set; }
    //    public string BusinessTypeNameEN { get; set; }
    //    public string BusinessTypeNameLC { get; set; }
    //    public string BusinessTypeNameJP { get; set; }

    //    [LanguageMapping]
    //    public string BuildingUsageName { get; set; }
    //    public string BuildingUsageNameEN { get; set; }
    //    public string BuildingUsageNameLC { get; set; }
    //    public string BuildingUsageNameJP { get; set; }

    //    public int? ContractDurationMonth { get; set; }
    //    public string OldContractCode { get; set; }
    //    public string ContractFeePayMethod { get; set; }
    //    public string OperationOfficeName { get; set; }
    //}

    //public class CTS160_InstrumentGridData : tbt_RentalInstrumentDetails
    //{
    //    public string InstrumentQtyForShow
    //    {
    //        get
    //        {
    //            return CommonUtil.TextNumeric(this.InstrumentQty);
    //        }
    //    }
    //}

}

namespace SECOM_AJIS.DataEntity.Contract.MetaData
{


}
