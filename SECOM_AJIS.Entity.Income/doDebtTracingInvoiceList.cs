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
    public partial class doDebtTracingInvoiceList
    {
        #region Primitive Properties
    
        public string BillingTargetCode
        {
            get;
            set;
        }
    
        public string InvoiceNo
        {
            get;
            set;
        }
    
        public int InvoiceOCC
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> IssueInvDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> BillingDueDate
        {
            get;
            set;
        }
    
        public string PaymentMethod
        {
            get;
            set;
        }
    
        public string PaymentMethodNameEN
        {
            get;
            set;
        }
    
        public string PaymentMethodNameJP
        {
            get;
            set;
        }
    
        public string PaymentMethodNameLC
        {
            get;
            set;
        }
    
        public Nullable<decimal> InvoiceAmount
        {
            get;
            set;
        }
    
        public Nullable<decimal> InvoiceUnpaidAmount
        {
            get;
            set;
        }
    
        public Nullable<int> ContractCount
        {
            get;
            set;
        }
    
        public int IsOverDue
        {
            get;
            set;
        }
    
        public string IsOverDueDescEN
        {
            get;
            set;
        }
    
        public string IsOverDueDescLC
        {
            get;
            set;
        }
    
        public string IsOverDueDescJP
        {
            get;
            set;
        }
    
        public Nullable<bool> FirstFeeFlag
        {
            get;
            set;
        }
    
        public Nullable<decimal> InvoiceAmountUsd
        {
            get;
            set;
        }
    
        public Nullable<decimal> InvoiceUnpaidAmountUsd
        {
            get;
            set;
        }
    
        public string InvoiceAmountCurrencyType
        {
            get;
            set;
        }
    
        public string InvoiceUnpaidCurrencyType
        {
            get;
            set;
        }

        #endregion

    }
}
