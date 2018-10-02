using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Contract.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.DataEntity.Quotation;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// Parameter of CTS051 screen
    /// </summary>
    public class CTS051_ScreenParameter2 : ScreenSearchParameter
    {
        [KeepSession]
        public string ContractCode { get; set; }
        [KeepSession]
        public string ContractOCC { get; set; }
        [KeepSession]
        public string QuotationCode { get; set; }
        public string Alphabet { get; set; }
        public string ContractStatus { get; set; }
        public string ProductType { get; set; }
        public bool InstallationCompleteFlag { get; set; }
        public bool IsContractApprove { get; set; }
        public string StartType { get; set; }

        public List<dtBillingTemp_SetItem> newItemList { get; set; }
        public List<dtBillingTemp_SetItem> updateItemList { get; set; }
        public List<dtBillingTemp_SetItem> deleteItemList { get; set; }
    }

    /// <summary>
    /// DO for display Contract
    /// </summary>
    public class CTS051_DisplayContract
    {
        public string ContractCode { get; set; }
        public string UserCode { get; set; }
        public string CustCode { get; set; }
        public string EndUserCode { get; set; }
        public string SiteCode { get; set; }
        public bool IsImportantCustomer { get; set; }
        public string CustNameEN { get; set; }
        public string CustAddressEN { get; set; }
        public string SiteNameEN { get; set; }
        public string SiteAddressEN { get; set; }
        public string CustNameLC { get; set; }
        public string CustAddressLC { get; set; }
        public string SiteNameLC { get; set; }
        public string SiteAddressLC { get; set; }
        public string InstallationStatus { get; set; }
        public string OperationOffice { get; set; }
        public string QuotationCode { get; set; }
        public bool CanViewInstallStatus { get; set; }
    }

    /// <summary>
    /// DO for display ChangePlan
    /// </summary>
    public class CTS051_DisplayChangePlan
    {
        public string QuotationCode { get; set; }
        public string Alphabet { get; set; }
        public string ExpectedOperationDate { get; set; }

        public string ChangePlanNormalContractFeeCurrencyType { get; set; }
        public string ChangePlanNormalContractFee { get; set; }
        public string ChangePlanOrderContractFeeCurrencyType { get; set; }
        public string ChangePlanOrderContractFee { get; set; }

        public string ChangePlanNormalInstallationFeeCurrencyType { get; set; }
        public string ChangePlanNormalInstallationFee { get; set; }
        public string ChangePlanOrderInstallationFeeCurrencyType { get; set; }
        public string ChangePlanOrderInstallationFee { get; set; }
        public string ChangePlanApproveInstallationFeeCurrencyType { get; set; }
        public string ChangePlanApproveInstallationFee { get; set; }
        public string ChangePlanCompleteInstallationFeeCurrencyType { get; set; }
        public string ChangePlanCompleteInstallationFee { get; set; }
        public string ChangePlanStartInstallationFeeCurrencyType { get; set; }
        public string ChangePlanStartInstallationFee { get; set; }

        public string BillingTimingType { get; set; }
        public string NegotiationStaffEmpNo1 { get; set; }
        public string NegotiationStaffEmpName1 { get; set; }
        public string NegotiationStaffEmpNo2 { get; set; }
        public string NegotiationStaffEmpName2 { get; set; }

        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
        public string ApproveNo3 { get; set; }
        public string ApproveNo4 { get; set; }
        public string ApproveNo5 { get; set; }

        public bool ContractDurationFlag { get; set; }
        public string ContractDurationMonth { get; set; }
        public string AutoRenewMonth { get; set; }
        public string EndContractDate { get; set; }

        public List<string> DisableList { get; set; }
        public bool NeedFixTiming { get; set; }
    }

    /// <summary>
    /// Parameter for register ChangePlan
    /// </summary>
    public class CTS051_RegisterChangePlan
    {
        public string QuotationCode { get; set; }
        public string Alphabet { get; set; }
        public DateTime? ExpectedOperationDate { get; set; }

        public decimal? ChangePlanNormalContractFee { get; set; }
        public string ChangePlanNormalContractFeeCurrencyType { get; set; }

        public decimal? ChangePlanOrderContractFee { get; set; }
        public string ChangePlanOrderContractFeeCurrencyType { get; set; }

        public decimal? ChangePlanNormalInstallationFee { get; set; }
        public string ChangePlanNormalInstallationFeeCurrencyType { get; set; }

        public decimal? ChangePlanOrderInstallationFee { get; set; }
        public string ChangePlanOrderInstallationFeeCurrencyType { get; set; }

        public decimal? ChangePlanApproveInstallationFee { get; set; }
        public string ChangePlanApproveInstallationFeeCurrencyType { get; set; }

        public decimal? ChangePlanCompleteInstallationFee { get; set; }
        public string ChangePlanCompleteInstallationFeeCurrencyType { get; set; }

        public decimal? ChangePlanStartInstallationFee { get; set; }
        public string ChangePlanStartInstallationFeeCurrencyType { get; set; }
        
        public string NegotiationStaffEmpNo1 { get; set; }
        public string NegotiationStaffEmpNo2 { get; set; }

        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
        public string ApproveNo3 { get; set; }
        public string ApproveNo4 { get; set; }
        public string ApproveNo5 { get; set; }

        public bool ContractDurationFlag { get; set; }
        public int? ContractDurationMonth { get; set; }
        public int? AutoRenewMonth { get; set; }
        public DateTime? EndContractDate { get; set; }
        public bool DivideContractFeeBillingFlag { get; set; }
    }

    /// <summary>
    /// DO for display detail of BillingTarget
    /// </summary>
    public class CTS051_DisplayBillingTargetDetail : CTS051_ChangePlanBaseObject
    {
        public string BillingContractFee { get; set; }
        public string BillingContractFeeCurrencyType { get; set; }

        public string BillingInstallationFee { get; set; }
        public string BillingInstallationFeeCurrencyType { get; set; }

        public string PaymentInstallationFee { get; set; }
        
        public string BillingApproveInstallationFee { get; set; }
        public string BillingApproveInstallationFeeCurrencyType { get; set; }

        public string BillingCompleteInstallationFee { get; set; }
        public string BillingCompleteInstallationFeeCurrencyType { get; set; }

        public string PaymentCompleteInstallationFee { get; set; }
        
        public string BillingStartInstallationFee { get; set; }
        public string BillingStartInstallationFeeCurrencyType { get; set; }

        public string PaymentStartInstallationFee { get; set; }
        
        public bool IsBefore { get; set; }
        public List<string> HideList { get; set; }
        public List<string> DisableList { get; set; }
        public bool HasFee { get; set; }
    }

    /// <summary>
    /// DO for edit detail of BillingTarget
    /// </summary>
    public class CTS051_EditBillingTargetDetail : CTS051_ChangePlanBaseObject
    {
        public bool IsBefore { get; set; }
        public List<string> HideList { get; set; }
        public List<string> DisableList { get; set; }

        public decimal? BillingContractFee { get; set; }
        public string BillingContractFeeCurrencyType { get; set; }

        public decimal? BillingInstallationFee { get; set; }
        public string BillingInstallationFeeCurrencyType { get; set; }
        public string PaymentInstallationFee { get; set; }

        public decimal? BillingApproveInstallationFee { get; set; }
        public string BillingApproveInstallationFeeCurrencyType { get; set; }

        public decimal? BillingCompleteInstallationFee { get; set; }
        public string BillingCompleteInstallationFeeCurrencyType { get; set; }
        public string PaymentCompleteInstallationFee { get; set; }

        public decimal? BillingStartInstallationFee { get; set; }
        public string BillingStartInstallationFeeCurrencyType { get; set; }
        public string PaymentStartInstallationFee { get; set; }

        public decimal? AmountTotal { get; set; }
        public decimal? AmountTotalUS { get; set; }

        public string UID { get; set; }
        public bool HasFee { get; set; }
        public string OldOfficeCode { get; set; }
    }

    /// <summary>
    /// DO of ChangePlanBase
    /// </summary>
    public class CTS051_ChangePlanBaseObject
    {
        public string BillingTargetCode { get; set; }
        public string BillingClientCode { get; set; }

        public string FullNameEN { get; set; }
        public string BranchNameEN { get; set; }
        public string AddressEN { get; set; }

        public string FullNameLC { get; set; }
        public string BranchNameLC { get; set; }
        public string AddressLC { get; set; }

        public string Nationality { get; set; }
        public string PhoneNo { get; set; }
        public string BusinessTypeCode { get; set; }
        public string BusinessType { get; set; }
        public string IDNo { get; set; }
        public string BillingOffice { get; set; }
        public string BillingOCC { get; set; }
        public string CustomerType { get; set; }
        public string CompanyTypeCode { get; set; }
        public string RegionCode { get; set; }

        public int ObjectType { get; set; }
        public string UID { get; set; }

        public string OriginalBillingOCC { get; set; }
        public string OriginalBillingClientCode { get; set; }
        public string OriginalBillingOfficeCode { get; set; }

        //public int SequenceNo { get; set; }
    }

    /// <summary>
    /// DO of ChangePlan grid
    /// </summary>
    public class CTS051_ChangePlanGrid
    {
        public string BillingOCC { get; set; }
        public string BillingClientCode { get; set; }
        public string BillingOfficeCode { get; set; }
        public string BillingOfficeName { get; set; }
        public string BillingTargetCode { get; set; }
        public string BillingTargetName { get; set; }
        public string ContractFee { get; set; }
        public string InstallFee { get; set; }
        public string DepositFee { get; set; }
        public bool CanDelete { get; set; }
        public bool IsNew { get; set; }
        public bool HasUpdate { get; set; }

        public string UID { get; set; }
        public string OriginalBillingOCC { get; set; }
        public string OriginalBillingClientCode { get; set; }
        public string OriginalBillingOfficeCode { get; set; }
    }

    //public class CTS051_ChangePlanDeleteItem : CTS051_ChangePlanEditItem
    //{
        
    //}

    //public class CTS051_ChangePlanEditItem
    //{
    //    public string BillingTargetCode { get; set; }
    //    public string BillingClientCode { get; set; }
    //    public string BillingOCC { get; set; }
    //    public string BillingOfficeCode { get; set; }
    //    public bool IsNew { get; set; }
    //    public bool HasUpdate { get; set; }
    //    public string UID { get; set; }
    //}

    //public class CTS051_ChangePlanDetailObject : CTS051_ChangePlanBaseObject
    //{
    //    public decimal? BillingContractFee { get; set; }

    //    public decimal? BillingInstallationFee { get; set; }
    //    public string PaymentInstallationFee { get; set; }

    //    public decimal? BillingApproveInstallationFee { get; set; }

    //    public decimal? BillingCompleteInstallationFee { get; set; }
    //    public string PaymentCompleteInstallationFee { get; set; }

    //    public decimal? BillingStartInstallationFee { get; set; }
    //    public string PaymentStartInstallationFee { get; set; }

    //    public decimal? AmountTotal { get; set; }

    //    public decimal? BillingDepositFee { get; set; }
    //    public string PaymentDepositFee { get; set; }
    //    public string UID { get; set; }
    //    public string OldOfficeCode { get; set; }
    //}

    #region Old Object
    public class CTS051_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public string ContractCode { get; set; }

        //public CTS051_ScreenParameter ScreenParameter { get; set; }
        public doRentalContractBasicInformation DORentalContractBasicInformation { get; set; }
        public dsRentalContractData DSRentalContract { get; set; }
        public dsQuotationData DSQuotationData { get; set; }

        public dtBillingTempChangePlanData DTBillingTempChangePlan { get; set; }
        public List<dtBillingTempChangePlanData> ListDTBillingTempChangePlan { get; set; }

        public CTS051_DOChangePlanAndBillingTargetGridData DOChangePlanAndBillingTargetGrid { get; set; }
        public List<CTS051_DOChangePlanAndBillingTargetGridData> ListDOChangePlanAndBillingTargetGrid { get; set; }

        public CTS051_DOBillingTargetDetailData DOBillingTargetDetail { get; set; }
        public CTS051_DOBillingTargetDetailData DOBillingTargetDetailCopy { get; set; }
        public List<CTS051_DOBillingTargetDetailData> ListDOBillingTargetDetail { get; set; }

        public CTS051_DOBillingTargetDetailGridData DOBillingTargetDetailGrid { get; set; }
        public List<CTS051_DOBillingTargetDetailGridData> ListDOBillingTargetDetailGrid { get; set; }

        public CTS051_DTBillingClientDetailData DTBillingClientDetail { get; set; }
        public CTS051_DTBillingClientDetailData DTBillingClientDetailCopy { get; set; }
        public List<CTS051_DTBillingClientDetailData> ListDTBillingClientDetail { get; set; }
        public List<doMiscTypeCode> ListDOMiscTypeCode { get; set; }
    }

    public class CTS051_DOChangePlanAndBillingTargetGridData
    {
        //For condition ------------------------------------------

        public string ProductTypeCode { get; set; }
        public string ContractStatus { get; set; }

        //For change plan grid-------------------------------------

        public string ID { get; set; }                       //column
        public decimal? Normal { get; set; }                 //column
        public decimal? Order { get; set; }                  //column
        public decimal? ApproveContract { get; set; }        //column
        public decimal? CompleteInstallation { get; set; }   //column
        public decimal? StartService { get; set; }           //column

        public string NegotiationStaffEmpNo1 { get; set; }
        public string NegotiationStaffEmpName1 { get; set; }
        public string NegotiationStaffEmpNo2 { get; set; }
        public string NegotiationStaffEmpName2 { get; set; }

        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
        public string ApproveNo3 { get; set; }
        public string ApproveNo4 { get; set; }
        public string ApproveNo5 { get; set; }

        public DateTime ExpectOperationDate { get; set; }
        public bool IsChangeContractDuration { get; set; }
        public int? ContractDurationMonth { get; set; }
        public int? AutoRenewMonth { get; set; }
        public DateTime? ContractEndDate { get; set; }

        public string EmpNo { get; set; }
        public string EmpName { get; set; }

        public decimal? NormalContractFee { get; set; }
        public decimal? OrderContractFee { get; set; }
        public decimal? NormalInstallationFee { get; set; }
        public decimal? OrderInstallationFee { get; set; }
        public decimal? NormalDepositFee { get; set; }
        public decimal? OrderDepositFee { get; set; }

        public decimal? CompleteInstallationFee { get; set; }
        public decimal? StartInstallationFee { get; set; }

        //For billing target grid---------------------------------- 

        public bool DivideContractFeeBillingFlag { get; set; }
        public string DisplayAll { get; set; }

        public string BillingOCC { get; set; }         //column
        public string BillingClientCode { get; set; }  //column
        public string BillingOffice { get; set; }      //column
        public string BillingOfficeName { get; set; }      //column
        public string BillingTargetCode { get; set; }  //column
        public string BillingTargetName { get; set; }  //column
        public string ContractFee { get; set; }        //column
        public string InstallationFee { get; set; }    //column
        public string DepositFee { get; set; }         //column
        public string PayMethod { get; set; }          //column
        public string Sequence { get; set; }           //column
        public string Status { get; set; }             //column
        public bool Show { get; set; }                 //column
        public bool CanDelete { get; set; }

        public string uIDNew { get; set; }
        public string BillingClientCodeLong { get; set; }

        //----------------------------------------------------------       
    }

    public class CTS051_DTBillingClientDetailData : dtBillingClientData
    {
        public string BillingOffice { get; set; }
        public string Sequence { get; set; }
    }

    public class CTS051_DOBillingTargetDetailGridData
    {
        //For billing target detail grid---------------------------------

        public string Case { get; set; }
        public string Amount { get; set; }
        public string PayMethod { get; set; }
        public string Total { get; set; }
        public bool Show { get; set; }
        //----------------------------------------------------------
    }

    // Using in CTS130 Model
    public class CTS051_DOBillingTargetDetailData
    {
        //For billing target detail---------------------------------
        public string BillingTargetCode { get; set; }
        public string BillingClientCode { get; set; }

        public string BillingTargetCodeDetail { get; set; }
        public string BillingTargetCodeDetailLong { get; set; }
        public string BillingClientCodeDetail { get; set; }
        public string BillingClientCodeDetailLong { get; set; }
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
        public string BillingOfficeCode { get; set; }
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
    #endregion OldObject
}
