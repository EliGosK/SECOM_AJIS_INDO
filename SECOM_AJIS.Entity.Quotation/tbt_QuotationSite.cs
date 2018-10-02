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
    public partial class tbt_QuotationSite
    {
        #region Primitive Properties
    
        public virtual string QuotationTargetCode
        {
            get { return _quotationTargetCode; }
            set
            {
                if (_quotationTargetCode != value)
                {
                    if (tbt_QuotationTarget != null && tbt_QuotationTarget.QuotationTargetCode != value)
                    {
                        tbt_QuotationTarget = null;
                    }
                    _quotationTargetCode = value;
                }
            }
        }
        private string _quotationTargetCode;
    
        public virtual string SiteCode
        {
            get;
            set;
        }
    
        public virtual string SiteNameEN
        {
            get;
            set;
        }
    
        public virtual string SiteNameLC
        {
            get;
            set;
        }
    
        public virtual string SECOMContactPerson
        {
            get;
            set;
        }
    
        public virtual string PersonInCharge
        {
            get;
            set;
        }
    
        public virtual string PhoneNo
        {
            get;
            set;
        }
    
        public virtual string BuildingUsageCode
        {
            get;
            set;
        }
    
        public virtual string AddressFullEN
        {
            get;
            set;
        }
    
        public virtual string AddressEN
        {
            get;
            set;
        }
    
        public virtual string AlleyEN
        {
            get;
            set;
        }
    
        public virtual string RoadEN
        {
            get;
            set;
        }
    
        public virtual string SubDistrictEN
        {
            get;
            set;
        }
    
        public virtual string AddressFullLC
        {
            get;
            set;
        }
    
        public virtual string AddressLC
        {
            get;
            set;
        }
    
        public virtual string AlleyLC
        {
            get;
            set;
        }
    
        public virtual string RoadLC
        {
            get;
            set;
        }
    
        public virtual string SubDistrictLC
        {
            get;
            set;
        }
    
        public virtual string DistrictCode
        {
            get;
            set;
        }
    
        public virtual string ProvinceCode
        {
            get;
            set;
        }
    
        public virtual string ZipCode
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
    
        public virtual string SiteNo
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual tbt_QuotationTarget tbt_QuotationTarget
        {
            get { return _tbt_QuotationTarget; }
            set
            {
                if (!ReferenceEquals(_tbt_QuotationTarget, value))
                {
                    var previousValue = _tbt_QuotationTarget;
                    _tbt_QuotationTarget = value;
                    Fixuptbt_QuotationTarget(previousValue);
                }
            }
        }
        private tbt_QuotationTarget _tbt_QuotationTarget;

        #endregion

        #region Association Fixup
    
        private void Fixuptbt_QuotationTarget(tbt_QuotationTarget previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.tbt_QuotationSite, this))
            {
                previousValue.tbt_QuotationSite = null;
            }
    
            if (tbt_QuotationTarget != null)
            {
                tbt_QuotationTarget.tbt_QuotationSite = this;
                if (QuotationTargetCode != tbt_QuotationTarget.QuotationTargetCode)
                {
                    QuotationTargetCode = tbt_QuotationTarget.QuotationTargetCode;
                }
            }
        }

        #endregion

    }
}
