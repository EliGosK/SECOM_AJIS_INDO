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
    public partial class tbs_BatchSchedule
    {
        #region Primitive Properties
    
        public virtual int ScheduleId
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
    
        public virtual string Type
        {
            get;
            set;
        }
    
        public virtual Nullable<byte> Condition
        {
            get;
            set;
        }
    
        public virtual System.TimeSpan Time
        {
            get;
            set;
        }
    
        public virtual bool Enable
        {
            get;
            set;
        }
    
        public virtual string Remark
        {
            get;
            set;
        }

        #endregion

    }
}
