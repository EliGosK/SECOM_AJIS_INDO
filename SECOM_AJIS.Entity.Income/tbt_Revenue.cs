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
    public partial class tbt_Revenue
    {
        #region Primitive Properties
    
        public virtual string RevenueNo
        {
            get;
            set;
        }
    
        public virtual string BillingCode
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> IssueDate
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> RevenueAmountIncVAT
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> RevenueVATAmount
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
    
        public virtual decimal RevenueAmountIncVATUsd
        {
            get;
            set;
        }
    
        public virtual string RevenueAmountIncVATCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> RevenueVATAmountUsd
        {
            get;
            set;
        }
    
        public virtual string RevenueVATAmountCurrencyType
        {
            get;
            set;
        }

        #endregion

    }
}
