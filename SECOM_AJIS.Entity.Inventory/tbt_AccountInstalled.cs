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

namespace SECOM_AJIS.DataEntity.Inventory
{
    public partial class tbt_AccountInstalled
    {
        #region Primitive Properties
    
        public virtual string OfficeCode
        {
            get;
            set;
        }
    
        public virtual string LocationCode
        {
            get;
            set;
        }
    
        public virtual string LotNo
        {
            get;
            set;
        }
    
        public virtual string InstrumentCode
        {
            get;
            set;
        }
    
        public virtual Nullable<int> InstrumentQty
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> AccquisitionCost
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
    
        public virtual Nullable<decimal> AccquisitionCostUsd
        {
            get;
            set;
        }
    
        public virtual string AccquisitionCostCurrencyType
        {
            get;
            set;
        }

        #endregion

    }
}
