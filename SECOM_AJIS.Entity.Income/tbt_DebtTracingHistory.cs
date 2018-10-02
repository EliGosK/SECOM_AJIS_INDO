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
    public partial class tbt_DebtTracingHistory
    {
        #region Primitive Properties
    
        public virtual int HistoryID
        {
            get;
            set;
        }
    
        public virtual string BillingTargetCode
        {
            get;
            set;
        }
    
        public virtual System.DateTime CallDate
        {
            get;
            set;
        }
    
        public virtual string CallResult
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> EstimatedDate
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> EstimatedAmount
        {
            get;
            set;
        }
    
        public virtual string PaymentMethod
        {
            get;
            set;
        }
    
        public virtual string PostponeReason
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> NextCallDate
        {
            get;
            set;
        }
    
        public virtual string Remark
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
    
        public virtual string DebtTracingStatus
        {
            get;
            set;
        }
    
        public virtual string ServiceTypeCode
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> EstimatedAmountUsd
        {
            get;
            set;
        }
    
        public virtual string EstimatedAmountCurrencyType
        {
            get;
            set;
        }

        #endregion

    }
}