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

namespace SECOM_AJIS.DataEntity.Installation
{
    public partial class tbt_InstallationInstrumentDetails
    {
        #region Primitive Properties
    
        public virtual string ContractCode
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
    
        public virtual Nullable<int> ContractInstalledQty
        {
            get;
            set;
        }
    
        public virtual Nullable<int> ContractRemovedQty
        {
            get;
            set;
        }
    
        public virtual Nullable<int> ContractMovedQty
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
