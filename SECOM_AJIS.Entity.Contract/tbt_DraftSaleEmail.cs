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
    public partial class tbt_DraftSaleEmail
    {
        #region Primitive Properties
    
        public virtual string QuotationTargetCode
        {
            get;
            set;
        }
    
        public virtual int DraftSaleEmailID
        {
            get;
            set;
        }
    
        public virtual string Alphabet
        {
            get;
            set;
        }
    
        public virtual string ToEmpNo
        {
            get;
            set;
        }
    
        public virtual string SendFlag
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
