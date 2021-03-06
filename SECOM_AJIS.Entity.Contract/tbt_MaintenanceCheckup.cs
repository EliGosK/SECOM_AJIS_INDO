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
    public partial class tbt_MaintenanceCheckup
    {
        #region Primitive Properties
    
        public virtual string ContractCode
        {
            get;
            set;
        }
    
        public virtual string ProductCode
        {
            get;
            set;
        }
    
        public virtual System.DateTime InstructionDate
        {
            get;
            set;
        }
    
        public virtual string CheckupNo
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> ExpectedMaintenanceDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> MaintenanceDate
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> UsageTime
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> InstrumentMalfunctionFlag
        {
            get;
            set;
        }
    
        public virtual string Location
        {
            get;
            set;
        }
    
        public virtual string PICName
        {
            get;
            set;
        }
    
        public virtual string MaintEmpNo
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> NeedSalesmanFlag
        {
            get;
            set;
        }
    
        public virtual string MalfunctionDetail
        {
            get;
            set;
        }
    
        public virtual string Remark
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> DeleteFlag
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
    
        public virtual Nullable<decimal> MaintenanceFee
        {
            get;
            set;
        }
    
        public virtual string ApproveNo1
        {
            get;
            set;
        }
    
        public virtual string SubcontractCode
        {
            get;
            set;
        }
    
        public virtual string MaintenanceFeeCurrencyType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> MaintenanceFeeUsd
        {
            get;
            set;
        }

        #endregion

    }
}
