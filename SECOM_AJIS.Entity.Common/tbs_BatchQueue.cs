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

namespace SECOM_AJIS.DataEntity.Common
{
    public partial class tbs_BatchQueue
    {
        #region Primitive Properties
    
        public virtual int RunId
        {
            get;
            set;
        }
    
        public virtual Nullable<int> ScheduleId
        {
            get;
            set;
        }
    
        public virtual string BatchCode
        {
            get;
            set;
        }
    
        public virtual string BatchName
        {
            get;
            set;
        }
    
        public virtual System.DateTime NextRun
        {
            get;
            set;
        }
    
        public virtual string Status
        {
            get;
            set;
        }
    
        public virtual string Remark
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> StartTime
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> EndTime
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> LastUpdate
        {
            get;
            set;
        }

        #endregion

    }
}
