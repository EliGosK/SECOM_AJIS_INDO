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

namespace SECOM_AJIS.DataEntity.Master
{
    public partial class tbm_SecomBankAccount
    {
        #region Primitive Properties
    
        public virtual int SecomAccountID
        {
            get;
            set;
        }
    
        public virtual string BankCode
        {
            get;
            set;
        }
    
        public virtual string BankBranchCode
        {
            get;
            set;
        }
    
        public virtual string AccountNo
        {
            get;
            set;
        }
    
        public virtual string AccountName
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> AutoTransferFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> BankTransferFlag
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

        #endregion

    }
}