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
    public partial class tbt_RentalSentryGuardDetails
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
    
        public virtual Nullable<System.TimeSpan> SecurityStartTime
        {
            get;
            set;
        }
    
        public virtual Nullable<System.TimeSpan> SecurityFinishTime
        {
            get;
            set;
        }
    
        public virtual string SentryGuardTypeCode
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> NumOfDate
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TimeUnitPrice
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> WorkHourPerMonth
        {
            get;
            set;
        }
    
        public virtual Nullable<int> NumOfSentryGuard
        {
            get;
            set;
        }
    
        public virtual System.DateTime CreateDate
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
    
        public virtual Nullable<decimal> TimeUnitPriceUsd
        {
            get;
            set;
        }
    
        public virtual string TimeUnitPriceCurrencyType
        {
            get;
            set;
        }

        #endregion

    }
}
