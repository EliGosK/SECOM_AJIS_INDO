//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SECOM_AJIS.DataEntity.Contract
{
    public partial class tbt_ContractDocument
    {
        #region Primitive Properties
    
        public virtual int DocID
        {
            get;
            set;
        }
    
        public virtual string QuotationTargetCode
        {
            get;
            set;
        }
    
        public virtual string Alphabet
        {
            get;
            set;
        }
    
        public virtual string ContractCode
        {
            get;
            set;
        }
    
        public virtual string OCC
        {
            get;
            set;
        }
    
        public virtual string ContractDocOCC
        {
            get;
            set;
        }
    
        public virtual string DocNo
        {
            get;
            set;
        }
    
        public virtual string DocumentCode
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> SECOMSignatureFlag
        {
            get;
            set;
        }
    
        public virtual string EmpName
        {
            get;
            set;
        }
    
        public virtual string EmpPosition
        {
            get;
            set;
        }
    
        public virtual string ContractTargetCustCode
        {
            get;
            set;
        }
    
        public virtual string ContractTargetNameLC
        {
            get;
            set;
        }
    
        public virtual string ContractTargetNameEN
        {
            get;
            set;
        }
    
        public virtual string ContractTargetAddressLC
        {
            get;
            set;
        }
    
        public virtual string ContractTargetAddressEN
        {
            get;
            set;
        }
    
        public virtual string SiteNameLC
        {
            get;
            set;
        }
    
        public virtual string SiteNameEN
        {
            get;
            set;
        }
    
        public virtual string SiteAddressLC
        {
            get;
            set;
        }
    
        public virtual string SiteAddressEN
        {
            get;
            set;
        }
    
        public virtual string ContractOfficeCode
        {
            get;
            set;
        }
    
        public virtual string OperationOfficeCode
        {
            get;
            set;
        }
    
        public virtual string NegotiationStaffEmpNo
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> IssuedDate
        {
            get;
            set;
        }
    
        public virtual string DocEditFlag
        {
            get;
            set;
        }
    
        public virtual string DocEditor
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> DocEditDate
        {
            get;
            set;
        }
    
        public virtual string DocStatus
        {
            get;
            set;
        }
    
        public virtual string DocAuditResult
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> CollectDocDate
        {
            get;
            set;
        }
    
        public virtual string SubjectEN
        {
            get;
            set;
        }
    
        public virtual string SubjectLC
        {
            get;
            set;
        }
    
        public virtual string RelatedNo1
        {
            get;
            set;
        }
    
        public virtual string RelatedNo2
        {
            get;
            set;
        }
    
        public virtual string AttachDoc1
        {
            get;
            set;
        }
    
        public virtual string AttachDoc2
        {
            get;
            set;
        }
    
        public virtual string AttachDoc3
        {
            get;
            set;
        }
    
        public virtual string AttachDoc4
        {
            get;
            set;
        }
    
        public virtual string AttachDoc5
        {
            get;
            set;
        }
    
        public virtual string ApproveNo1
        {
            get;
            set;
        }
    
        public virtual string ApproveNo2
        {
            get;
            set;
        }
    
        public virtual string ApproveNo3
        {
            get;
            set;
        }
    
        public virtual string ContactMemo
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> QuotationFee
        {
            get;
            set;
        }
    
        public virtual string CreateOfficeCode
        {
            get;
            set;
        }
    
        public virtual string ProductCode
        {
            get;
            set;
        }
    
        public virtual string PhoneLineTypeCode
        {
            get;
            set;
        }
    
        public virtual string PhoneLineOwnerTypeCode
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> ContractFee
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> DepositFee
        {
            get;
            set;
        }
    
        public virtual string ContractFeePayMethod
        {
            get;
            set;
        }
    
        public virtual Nullable<int> CreditTerm
        {
            get;
            set;
        }
    
        public virtual Nullable<int> PaymentCycle
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> FireSecurityFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> CrimePreventFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> EmergencyReportFlag
        {
            get;
            set;
        }
    
        public virtual string BusinessTypeCode
        {
            get;
            set;
        }
    
        public virtual string BuildingUsageCode
        {
            get;
            set;
        }
    
        public virtual Nullable<int> ContractDurationMonth
        {
            get;
            set;
        }
    
        public virtual string OldContractCode
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> CreateDate
        {
            get;
            set;
        }
    
        public virtual string CreateBy
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> UpdateDate
        {
            get;
            set;
        }
    
        public virtual string UpdateBy
        {
            get;
            set;
        }
    
        public virtual string CoverLetterDocCode
        {
            get;
            set;
        }
    
        public virtual string RealCustomerCustCode
        {
            get;
            set;
        }
    
        public virtual string RealCustomerNameLC
        {
            get;
            set;
        }
    
        public virtual string RealCustomerNameEN
        {
            get;
            set;
        }
    
        public virtual string OperationOfficeNameEN
        {
            get;
            set;
        }
    
        public virtual string OperationOfficeNameLC
        {
            get;
            set;
        }
    
        public virtual string CreateOfficeNameEN
        {
            get;
            set;
        }
    
        public virtual string CreateOfficeNameLC
        {
            get;
            set;
        }
    
        public virtual string ProductNameEN
        {
            get;
            set;
        }
    
        public virtual string ProductNameLC
        {
            get;
            set;
        }
    
        public virtual string PhoneLineTypeNameEN
        {
            get;
            set;
        }
    
        public virtual string PhoneLineTypeNameLC
        {
            get;
            set;
        }
    
        public virtual string PhoneLineOwnerTypeNameEN
        {
            get;
            set;
        }
    
        public virtual string PhoneLineOwnerTypeNameLC
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> FacilityMonitorFlag
        {
            get;
            set;
        }
    
        public virtual string BusinessTypeNameEN
        {
            get;
            set;
        }
    
        public virtual string BusinessTypeNameLC
        {
            get;
            set;
        }
    
        public virtual string BuildingUsageNameEN
        {
            get;
            set;
        }
    
        public virtual string BuildingUsageNameLC
        {
            get;
            set;
        }
    
        public virtual string QuotationFeeCurrencyType
        {
            get;
            set;
        }
    
        public virtual string ContractFeeCurrencyType
        {
            get;
            set;
        }
    
        public virtual string DepositFeeCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> QuotationFeeUsd
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> ContractFeeUsd
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> DepositFeeUsd
        {
            get;
            set;
        }

        #endregion

    }
}