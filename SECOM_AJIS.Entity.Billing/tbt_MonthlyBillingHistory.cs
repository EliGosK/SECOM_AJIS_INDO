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
    public partial class tbt_MonthlyBillingHistory
    {
        #region Primitive Properties
    
        public virtual string ContractCode
        {
            get;
            set;
        }
    
        public virtual string BillingOCC
        {
            get;
            set;
        }
    
        public virtual int HistoryNo
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> MonthlyBillingAmount
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> BillingStartDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> BillingEndDate
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
    
        public virtual Nullable<decimal> MonthlyBillingAmountUsd
        {
            get;
            set;
        }
    
        public virtual string MonthlyBillingAmountCurrencyType
        {
            get;
            set;
        }

        #endregion

    }
}
