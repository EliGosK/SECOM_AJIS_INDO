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

namespace SECOM_AJIS.DataEntity.Billing
{
    public partial class tbt_TaxInvoice
    {
        #region Primitive Properties
    
        public virtual string TaxInvoiceNo
        {
            get;
            set;
        }
    
        public virtual string InvoiceNo
        {
            get;
            set;
        }
    
        public virtual Nullable<int> InvoiceOCC
        {
            get;
            set;
        }
    
        public virtual string ReceiptNo
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> TaxInvoiceDate
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> TaxInvoiceCanceledFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> TaxInvoiceIssuedFlag
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
