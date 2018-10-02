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
    public partial class tbt_QuotationBeatGuardDetails
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
    
        public virtual int NumOfDayTimeWd
        {
            get;
            set;
        }
    
        public virtual int NumOfNightTimeWd
        {
            get;
            set;
        }
    
        public virtual int NumOfDayTimeSat
        {
            get;
            set;
        }
    
        public virtual int NumOfNightTimeSat
        {
            get;
            set;
        }
    
        public virtual int NumOfDayTimeSun
        {
            get;
            set;
        }
    
        public virtual int NumOfNightTimeSun
        {
            get;
            set;
        }
    
        public virtual int NumOfBeatStep
        {
            get;
            set;
        }
    
        public virtual Nullable<int> FreqOfGateUsage
        {
            get;
            set;
        }
    
        public virtual Nullable<int> NumOfClockKey
        {
            get;
            set;
        }
    
        public virtual decimal NumOfDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.TimeSpan> NotifyTime
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
            if (previousValue != null && ReferenceEquals(previousValue.tbt_QuotationBeatGuardDetails, this))
            {
                previousValue.tbt_QuotationBeatGuardDetails = null;
            }
    
            if (tbt_QuotationBasic != null)
            {
                tbt_QuotationBasic.tbt_QuotationBeatGuardDetails = this;
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
