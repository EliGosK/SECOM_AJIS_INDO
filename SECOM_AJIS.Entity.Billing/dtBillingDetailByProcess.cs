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
    public partial class dtBillingDetailByProcess
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
    
        public string BillingTypeCode
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> LastBillingDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> StartOperationDate
        {
            get;
            set;
        }
    
        public string PaymentMethod
        {
            get;
            set;
        }
    
        public string CalDailyFeeStatus
        {
            get;
            set;
        }
    
        public Nullable<int> BillingCycle
        {
            get;
            set;
        }
    
        public Nullable<decimal> AdjustBillingPeriodAmount
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> AutoTransferDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> AdjustEndDate
        {
            get;
            set;
        }
    
        public Nullable<System.DateTime> IssueInvDate
        {
            get;
            set;
        }

        #endregion

    }
}