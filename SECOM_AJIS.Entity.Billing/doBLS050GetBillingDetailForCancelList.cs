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

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class doBLS050GetBillingDetailForCancelList
    {
        #region Primitive Properties
    
        public string ContractCode
        {
            get;
            set;
        }
    
        public string BillingOCC
        {
            get;
            set;
        }
    
        public int BillingDetailNo
        {
            get;
            set;
        }
    
        public string InvoiceNo
        {
            get;
            set;
        }
    
        public Nullable<int> InvoiceOCC
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> IssueInvDate
        {
            get;
            set;
        }
    
        public Nullable<bool> IssueInvFlag
        {
            get;
            set;
        }
    
        public string BillingTypeCode
        {
            get;
            set;
        }
    
        public Nullable<decimal> BillingAmount
        {
            get;
            set;
        }
    
        public Nullable<decimal> AdjustBillingAmount
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> BillingStartDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> BillingEndDate
        {
            get;
            set;
        }
    
        public string PaymentMethod
        {
            get;
            set;
        }
    
        public string PaymentStatus
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> AutoTransferDate
        {
            get;
            set;
        }
    
        public Nullable<bool> FirstFeeFlag
        {
            get;
            set;
        }
    
        public Nullable<int> DelayedMonth
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> CreateDate
        {
            get;
            set;
        }
    
        public string CreateBy
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> UpdateDate
        {
            get;
            set;
        }
    
        public string UpdateBy
        {
            get;
            set;
        }
    
        public string BillingTypeNameEN
        {
            get;
            set;
        }
    
        public string BillingTypeNameJP
        {
            get;
            set;
        }
    
        public string BillingTypeNameLC
        {
            get;
            set;
        }
    
        public string PaymentStatusNameEN
        {
            get;
            set;
        }
    
        public string PaymentStatusNameJP
        {
            get;
            set;
        }
    
        public string PaymentStatusNameLC
        {
            get;
            set;
        }
    
        public Nullable<decimal> BillingAmountUsd
        {
            get;
            set;
        }
    
        public string BillingAmountCurrencyType
        {
            get;
            set;
        }

        #endregion

    }
}
