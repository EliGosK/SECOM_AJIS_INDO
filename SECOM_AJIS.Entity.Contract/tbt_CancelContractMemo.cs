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
    public partial class tbt_CancelContractMemo
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
    
        public virtual Nullable<bool> QuotationFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TotalSlideAmt
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TotalReturnAmt
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TotalBillingAmt
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TotalAmtAfterCounterBalance
        {
            get;
            set;
        }
    
        public virtual string ProcessAfterCounterBalanceType
        {
            get;
            set;
        }
    
        public virtual string AutoTransferBillingType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> AutoTransferBillingAmt
        {
            get;
            set;
        }
    
        public virtual string BankTransferBillingType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> BankTransferBillingAmt
        {
            get;
            set;
        }
    
        public virtual string CustomerSignatureName
        {
            get;
            set;
        }
    
        public virtual string OtherRemarks
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
    
        public virtual Nullable<decimal> TotalSlideAmtUsd
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TotalReturnAmtUsd
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TotalBillingAmtUsd
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TotalAmtAfterCounterBalanceUsd
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> AutoTransferBillingAmtUsd
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> BankTransferBillingAmtUsd
        {
            get;
            set;
        }
    
        public virtual string ProcessAfterCounterBalanceTypeUsd
        {
            get;
            set;
        }

        #endregion

    }
}