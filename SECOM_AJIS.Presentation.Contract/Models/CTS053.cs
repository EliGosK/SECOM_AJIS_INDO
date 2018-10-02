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
    /// Parameter of CTS053 screen
    /// </summary>
    public class CTS053_ScreenParameter : ScreenSearchParameter
    {
        [KeepSession]
        public string ContractCode { get; set; }
        public string OCC { get; set; }

        public List<dtEmailAddress> mailList { get; set; }

        public List<dtBillingTemp_SetItem> newItemList { get; set; }
        public List<dtBillingTemp_SetItem> updateItemList { get; set; }
        public List<dtBillingTemp_SetItem> deleteItemList { get; set; }
    }

    /// <summary>
    /// DO for display
    /// </summary>
    public class CTS053_DisplayObject
    {
        public CTS053_ContractDisplayData ContractData { get; set; }
        public CTS053_ChangeContractFeeDisplayData ChangeFeeData { get; set; }
    }

    /// <summary>
    /// Parameter for register ChangePlan
    /// </summary>
    public class CTS053_RegisterChangePlan
    {
        public DateTime? ChangeDateContractFee { get; set; }
        public string CurrentContractFeeCurrencyType { get; set; }
        public decimal? CurrentContractFee { get; set; }
        public string ChangeContractFeeCurrencyType { get; set; }
        public decimal? ChangeContractFee { get; set; }
        public bool ChangeFeeFlag { get; set; }
        public DateTime? ReturnToOriginalFeeDate { get; set; }
        public string NegotiationStaffEmpNo1 { get; set; }
        public string NegotiationStaffEmpNo2 { get; set; }
        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
        public string ApproveNo3 { get; set; }
        public string ApproveNo4 { get; set; }
        public string ApproveNo5 { get; set; }

        public bool DivideBillingContractFee { get; set; }
    }

    /// <summary>
    /// DO for display ontract
    /// </summary>
    public class CTS053_ContractDisplayData
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
        public string OperationOffice { get; set; }
    }

    /// <summary>
    /// DO for display ChangeContractFee
    /// </summary>
    public class CTS053_ChangeContractFeeDisplayData
    {
        public string ChangeDateContractFee { get; set; }
        public string CurrentContractFeeCurrencyType { get; set; }
        public string CurrentContractFee { get; set; }
        public string ChangeContractFeeCurrencyType { get; set; }
        public string ChangeContractFee { get; set; }
        public bool ChangeFeeFlag { get; set; }
        public string ReturnToOriginalFeeDate { get; set; }
        public string NegotiationStaffEmpNo1 { get; set; }
        public string NegotiationStaffEmpName1 { get; set; }
        public string NegotiationStaffEmpNo2 { get; set; }
        public string NegotiationStaffEmpName2 { get; set; }
        public string ApproveNo1 { get; set; }
        public string ApproveNo2 { get; set; }
        public string ApproveNo3 { get; set; }
        public string ApproveNo4 { get; set; }
        public string ApproveNo5 { get; set; }
        public bool IsDisableDivideFlag { get; set; }
    }

    /// <summary>
    /// DO of ChangePlan grid
    /// </summary>
    public class CTS053_ChangePlanGrid
    {
        public string BillingOCC { get; set; }
        public string BillingClientCode { get; set; }
        public string BillingOfficeCode { get; set; }
        public string BillingOfficeName { get; set; }
        public string BillingTargetCode { get; set; }
        public string BillingTargetName { get; set; }
        public string ContractFee { get; set; }
        public bool CanDelete { get; set; }
        public bool IsNew { get; set; }
        public bool HasUpdate { get; set; }

        public string UID { get; set; }
        public string OriginalBillingOCC { get; set; }
        public string OriginalBillingClientCode { get; set; }
        public string OriginalBillingOfficeCode { get; set; }
    }

    /// <summary>
    /// DO for display detail of BillingTarget
    /// </summary>
    public class CTS053_DisplayBillingTargetDetail : CTS053_ChangePlanBaseObject
    {
        public string BillingContractFeeCurrencyType { get; set; }
        public string BillingContractFee { get; set; }
        public bool HasFee { get; set; }
    }

    /// <summary>
    /// DO for edit detail of BillingTarget
    /// </summary>
    public class CTS053_EditBillingTargetDetail : CTS053_ChangePlanBaseObject
    {
        public bool IsBefore { get; set; }
        public List<string> HideList { get; set; }
        public List<string> DisableList { get; set; }

        public string BillingContractFeeCurrencyType { get; set; }
        public decimal? BillingContractFee { get; set; }

        public bool ChangeFeeFlag { get; set; }

        public string UID { get; set; }
        public bool HasFee { get; set; }
        public string OldOfficeCode { get; set; }
    }

    /// <summary>
    /// DO of ChangePlanBase
    /// </summary>
    public class CTS053_ChangePlanBaseObject
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
    }
}
