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
    public partial class tbt_CancelContractMemoDetail
    {
        #region Primitive Properties
    
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
    
        public virtual int SequenceNo
        {
            get;
            set;
        }
    
        public virtual string BillingType
        {
            get;
            set;
        }
    
        public virtual string HandlingType
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> StartPeriodDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> EndPeriodDate
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> FeeAmount
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TaxAmount
        {
            get;
            set;
        }
    
        public virtual string ContractCode_CounterBalance
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
    
        public virtual Nullable<decimal> NormalFeeAmount
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> FeeAmountUsd
        {
            get;
            set;
        }
    
        public virtual string FeeAmountCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TaxAmountUsd
        {
            get;
            set;
        }
    
        public virtual string TaxAmountCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> NormalFeeAmountUsd
        {
            get;
            set;
        }
    
        public virtual string NormalFeeAmountCurrencyType
        {
            get;
            set;
        }

        #endregion

    }
}
