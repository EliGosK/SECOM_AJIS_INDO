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
    public partial class doPaymentMatchingResult
    {
        #region Primitive Properties
    
        public string MatchID
        {
            get;
            set;
        }
    
        public System.DateTime MatchDate
        {
            get;
            set;
        }
    
        public string PaymentTransNo
        {
            get;
            set;
        }
    
        public Nullable<decimal> TotalMatchAmount
        {
            get;
            set;
        }
    
        public Nullable<decimal> BankFeeAmount
        {
            get;
            set;
        }
    
        public Nullable<bool> SpecialProcessFlag
        {
            get;
            set;
        }
    
        public string ApproveNo
        {
            get;
            set;
        }
    
        public Nullable<decimal> OtherExpenseAmount
        {
            get;
            set;
        }
    
        public Nullable<decimal> OtherIncomeAmount
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
    
        public Nullable<bool> CancelFlag
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> CreateDate
        {
            get;
            set;
        }
    
        public Nullable<decimal> TotalMatchAmountUsd
        {
            get;
            set;
        }
    
        public string TotalMatchAmountCurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> BankFeeAmountUsd
        {
            get;
            set;
        }
    
        public string BankFeeAmountCurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> OtherExpenseAmountUsd
        {
            get;
            set;
        }
    
        public string OtherExpenseAmountCurrencyType
        {
            get;
            set;
        }
    
        public Nullable<decimal> OtherIncomeAmountUsd
        {
            get;
            set;
        }
    
        public string OtherIncomeAmountCurrencyType
        {
            get;
            set;
        }

        #endregion

    }
}
