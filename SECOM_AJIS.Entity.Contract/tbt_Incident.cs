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
    public partial class tbt_Incident
    {
        #region Primitive Properties
    
        public virtual int IncidentID
        {
            get;
            set;
        }
    
        public virtual string IncidentNo
        {
            get;
            set;
        }
    
        public virtual string IncidentOfficeCode
        {
            get;
            set;
        }
    
        public virtual string IncidentDepartmentCode
        {
            get;
            set;
        }
    
        public virtual string InteractionType
        {
            get;
            set;
        }
    
        public virtual string IncidentStatus
        {
            get;
            set;
        }
    
        public virtual string IncidentRelavantType
        {
            get;
            set;
        }
    
        public virtual string CustCode
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> RelatedToAllSiteFlag
        {
            get;
            set;
        }
    
        public virtual string SiteCode
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> RelatedToAllContractFlag
        {
            get;
            set;
        }
    
        public virtual string ProjectCode
        {
            get;
            set;
        }
    
        public virtual string ContractCode
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> CompletedDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> ReceivedDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.TimeSpan> ReceivedTime
        {
            get;
            set;
        }
    
        public virtual string ReceivedMethod
        {
            get;
            set;
        }
    
        public virtual string ContactPerson
        {
            get;
            set;
        }
    
        public virtual string ContactPersonDep
        {
            get;
            set;
        }
    
        public virtual string IncidentTitle
        {
            get;
            set;
        }
    
        public virtual string IncidentType
        {
            get;
            set;
        }
    
        public virtual string ReasonType
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> ConfidentialFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> ImportanceFlag
        {
            get;
            set;
        }
    
        public virtual string ReceivedDetail
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> DueDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.TimeSpan> DueDateTime
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> DeadLine
        {
            get;
            set;
        }
    
        public virtual string DeadLineTime
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
    
        public virtual Nullable<bool> hasRespondingDetailFlag
        {
            get;
            set;
        }

        #endregion

    }
}
