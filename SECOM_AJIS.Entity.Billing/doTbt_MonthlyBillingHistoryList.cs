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
    public partial class doTbt_MonthlyBillingHistoryList
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
    
        public int HistoryNo
        {
            get;
            set;
        }
    
        public Nullable<decimal> MonthlyBillingAmount
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
    
        public Nullable<decimal> MonthlyBillingAmountUsd
        {
            get;
            set;
        }
    
        public string MonthlyBillingAmountCurrencyType
        {
            get;
            set;
        }

        #endregion

    }
}
