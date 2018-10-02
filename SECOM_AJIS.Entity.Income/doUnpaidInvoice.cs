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
    public partial class doUnpaidInvoice
    {
        #region Primitive Properties
    
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
    
        public int NoOfBillingDetail
        {
            get;
            set;
        }
    
        public Nullable<decimal> InvoiceAmount
        {
            get;
            set;
        }
    
        public Nullable<decimal> VatAmount
        {
            get;
            set;
        }
    
        public Nullable<decimal> WHTAmount
        {
            get;
            set;
        }
    
        public Nullable<decimal> PaidAmount
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> IssueInvDate
        {
            get;
            set;
        }
    
        public Nullable<bool> ShowInvWHTFlag
        {
            get;
            set;
        }
    
        public string InvoicePaymentStatus
        {
            get;
            set;
        }
    
        public Nullable<decimal> RegisteredWHTAmount
        {
            get;
            set;
        }
    
        public Nullable<decimal> InvoiceAmountUsd
        {
            get;
            set;
        }
    
        public Nullable<decimal> VatAmountUsd
        {
            get;
            set;
        }
    
        public Nullable<decimal> WHTAmountUsd
        {
            get;
            set;
        }
    
        public Nullable<decimal> PaidAmountUsd
        {
            get;
            set;
        }
    
        public Nullable<decimal> RegisteredWHTAmountUsd
        {
            get;
            set;
        }
    
        public string InvoiceAmountCurrencyType
        {
            get;
            set;
        }
    
        public string VatAmountCurrencyType
        {
            get;
            set;
        }
    
        public string WHTAmountCurrencyType
        {
            get;
            set;
        }
    
        public string PaidAmountCurrencyType
        {
            get;
            set;
        }
    
        public string RegisteredWHTAmountCurrencyType
        {
            get;
            set;
        }

        #endregion

    }
}
