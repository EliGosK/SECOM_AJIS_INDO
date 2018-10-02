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

namespace SECOM_AJIS.DataEntity.Quotation
{
    public partial class tbt_QuotationFacilityDetails
    {
        #region Primitive Properties
    
        public virtual string QuotationTargetCode
        {
            get { return _quotationTargetCode; }
            set
            {
                if (_quotationTargetCode != value)
                {
                    if (tbt_QuotationBasic != null && tbt_QuotationBasic.QuotationTargetCode != value)
                    {
                        tbt_QuotationBasic = null;
                    }
                    _quotationTargetCode = value;
                }
            }
        }
        private string _quotationTargetCode;
    
        public virtual string Alphabet
        {
            get { return _alphabet; }
            set
            {
                if (_alphabet != value)
                {
                    if (tbt_QuotationBasic != null && tbt_QuotationBasic.Alphabet != value)
                    {
                        tbt_QuotationBasic = null;
                    }
                    _alphabet = value;
                }
            }
        }
        private string _alphabet;
    
        public virtual string FacilityCode
        {
            get;
            set;
        }
    
        public virtual Nullable<int> FacilityQty
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
    
        public virtual tbt_QuotationBasic tbt_QuotationBasic
        {
            get { return _tbt_QuotationBasic; }
            set
            {
                if (!ReferenceEquals(_tbt_QuotationBasic, value))
                {
                    var previousValue = _tbt_QuotationBasic;
                    _tbt_QuotationBasic = value;
                    Fixuptbt_QuotationBasic(previousValue);
                }
            }
        }
        private tbt_QuotationBasic _tbt_QuotationBasic;

        #endregion

        #region Association Fixup
    
        private void Fixuptbt_QuotationBasic(tbt_QuotationBasic previousValue)
        {
            if (previousValue != null && previousValue.tbt_QuotationFacilityDetails.Contains(this))
            {
                previousValue.tbt_QuotationFacilityDetails.Remove(this);
            }
    
            if (tbt_QuotationBasic != null)
            {
                if (!tbt_QuotationBasic.tbt_QuotationFacilityDetails.Contains(this))
                {
                    tbt_QuotationBasic.tbt_QuotationFacilityDetails.Add(this);
                }
                if (QuotationTargetCode != tbt_QuotationBasic.QuotationTargetCode)
                {
                    QuotationTargetCode = tbt_QuotationBasic.QuotationTargetCode;
                }
                if (Alphabet != tbt_QuotationBasic.Alphabet)
                {
                    Alphabet = tbt_QuotationBasic.Alphabet;
                }
            }
        }

        #endregion

    }
}
