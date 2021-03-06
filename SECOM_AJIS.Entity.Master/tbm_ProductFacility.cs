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
    public partial class tbm_ProductFacility
    {
        #region Primitive Properties
    
        public virtual string ProductCode
        {
            get;
            set;
        }
    
        public virtual string FacilityCode
        {
            get { return _facilityCode; }
            set
            {
                if (_facilityCode != value)
                {
                    if (tbm_Instrument != null && tbm_Instrument.InstrumentCode != value)
                    {
                        tbm_Instrument = null;
                    }
                    _facilityCode = value;
                }
            }
        }
        private string _facilityCode;
    
        public virtual Nullable<bool> NotDefaultFacilityFlag
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

        #region Navigation Properties
    
        public virtual tbm_Instrument tbm_Instrument
        {
            get { return _tbm_Instrument; }
            set
            {
                if (!ReferenceEquals(_tbm_Instrument, value))
                {
                    var previousValue = _tbm_Instrument;
                    _tbm_Instrument = value;
                    Fixuptbm_Instrument(previousValue);
                }
            }
        }
        private tbm_Instrument _tbm_Instrument;

        #endregion

        #region Association Fixup
    
        private void Fixuptbm_Instrument(tbm_Instrument previousValue)
        {
            if (previousValue != null && previousValue.tbm_ProductFacility.Contains(this))
            {
                previousValue.tbm_ProductFacility.Remove(this);
            }
    
            if (tbm_Instrument != null)
            {
                if (!tbm_Instrument.tbm_ProductFacility.Contains(this))
                {
                    tbm_Instrument.tbm_ProductFacility.Add(this);
                }
                if (FacilityCode != tbm_Instrument.InstrumentCode)
                {
                    FacilityCode = tbm_Instrument.InstrumentCode;
                }
            }
        }

        #endregion

    }
}
