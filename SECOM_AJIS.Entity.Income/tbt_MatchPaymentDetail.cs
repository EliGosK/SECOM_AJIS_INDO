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
    public partial class tbt_MatchPaymentDetail
    {
        #region Primitive Properties
    
        public virtual string MatchID
        {
            get;
            set;
        }
    
        public virtual string InvoiceNo
        {
            get;
            set;
        }
    
        public virtual int InvoiceOCC
        {
            get;
            set;
        }
    
        public virtual decimal MatchAmountExcWHT
        {
            get;
            set;
        }
    
        public virtual decimal MatchAmountIncWHT
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> WHTAmount
        {
            get;
            set;
        }
    
        public virtual string MatchStatus
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> CancelFlag
        {
            get;
            set;
        }
    
        public virtual string CorrectionReason
        {
            get;
            set;
        }
    
        public virtual string CreateBy
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> CreateDate
        {
            get;
            set;
        }
    
        public virtual string UpdateBy
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> UpdateDate
        {
            get;
            set;
        }
    
        public virtual string CancelApproveNo
        {
            get;
            set;
        }
    
        public virtual string MatchAmountExcWHTCurrencyType
        {
            get;
            set;
        }
    
        public virtual string MatchAmountIncWHTCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> WHTAmountUsd
        {
            get;
            set;
        }
    
        public virtual string WHTAmountCurrencyType
        {
            get;
            set;
        }
    
        public virtual decimal MatchAmountIncWHTUsd
        {
            get;
            set;
        }
    
        public virtual decimal MatchAmountExcWHTUsd
        {
            get;
            set;
        }

        #endregion

    }
}