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

namespace SECOM_AJIS.DataEntity.Income
{
    public partial class doDebtTracingCustList
    {
        #region Primitive Properties
    
        public string BillingOfficeCode
        {
            get;
            set;
        }
    
        public string BillingOfficeNameEN
        {
            get;
            set;
        }
    
        public string BillingOfficeNameJP
        {
            get;
            set;
        }
    
        public string BillingOfficeNameLC
        {
            get;
            set;
        }
    
        public string BillingTargetCode
        {
            get;
            set;
        }
    
        public string BillingClientCode
        {
            get;
            set;
        }
    
        public string BillingClientNameEN
        {
            get;
            set;
        }
    
        public string BillingClientNameLC
        {
            get;
            set;
        }
    
        public Nullable<int> InvoiceCount
        {
            get;
            set;
        }
    
        public Nullable<int> ContractCount
        {
            get;
            set;
        }
    
        public Nullable<decimal> Amount
        {
            get;
            set;
        }
    
        public Nullable<decimal> UnpaidAmount
        {
            get;
            set;
        }
    
        public string DebtTracingSubStatus
        {
            get;
            set;
        }
    
        public string ContactPersonName
        {
            get;
            set;
        }
    
        public string PhoneNo
        {
            get;
            set;
        }
    
        public string DebtTracingStatusDescEN
        {
            get;
            set;
        }
    
        public string DebtTracingStatusDescJP
        {
            get;
            set;
        }
    
        public string DebtTracingStatusDescLC
        {
            get;
            set;
        }
    
        public int IsHQBranch
        {
            get;
            set;
        }
    
        public string ServiceTypeCode
        {
            get;
            set;
        }
    
        public Nullable<int> InvoiceOverDueCount
        {
            get;
            set;
        }
    
        public Nullable<int> InvoiceNotDueCount
        {
            get;
            set;
        }
    
        public Nullable<decimal> AmountUsd
        {
            get;
            set;
        }
    
        public Nullable<decimal> UnpaidAmountUsd
        {
            get;
            set;
        }

        #endregion

    }
}