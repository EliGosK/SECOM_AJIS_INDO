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
    public partial class tbt_ContractEmail
    {
        #region Primitive Properties
    
        public virtual int ContractEmailID
        {
            get;
            set;
        }
    
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
    
        public virtual string EmailType
        {
            get;
            set;
        }
    
        public virtual string ToEmpNo
        {
            get;
            set;
        }
    
        public virtual string EmailFrom
        {
            get;
            set;
        }
    
        public virtual string EmailSubject
        {
            get;
            set;
        }
    
        public virtual string EmailContent
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> SendDate
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> SendFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<int> FailSendingCounter
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