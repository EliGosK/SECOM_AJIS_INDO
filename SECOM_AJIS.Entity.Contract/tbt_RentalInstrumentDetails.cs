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
    public partial class tbt_RentalInstrumentDetails
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
    
        public virtual string InstrumentCode
        {
            get;
            set;
        }
    
        public virtual string InstrumentTypeCode
        {
            get;
            set;
        }
    
        public virtual Nullable<int> InstrumentQty
        {
            get;
            set;
        }
    
        public virtual Nullable<int> AdditionalInstrumentQty
        {
            get;
            set;
        }
    
        public virtual Nullable<int> RemovalInstrumentQty
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

        #endregion

    }
}
