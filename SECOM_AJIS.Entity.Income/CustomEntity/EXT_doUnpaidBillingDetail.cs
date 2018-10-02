using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Income.MetaData;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data ojbect of Unpaid invoice information.
    /// </summary>
    [MetadataType(typeof(doUnpaidBillingDetail_Meta))]
    public partial class doUnpaidBillingDetail
    {
        //Show in grid
        public string BillingCodeShortFormat
        {
            get {
                return new CommonUtil().ConvertBillingCode(this.BillingCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string GridBillingCode
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}", this.BillingCodeShortFormat, this.RunningNo);
            }
        }
        public string GridSiteName
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}", this.SiteNameEN, this.SiteNameLC);
            }
        }

        [LanguageMapping]
        public string BillingType { get; set; }
        [LanguageMapping]
        public string PaymentStatusDesc { get; set; }
        public string SiteNameJP
        {
            get
            {
                return this.SiteNameEN;
            }
        }
        [LanguageMapping]
        public string SiteName { get; set; }

        public decimal? BillingAmountVal
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return BillingAmount;
                else if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return BillingAmountUsd;
                else return null;
            }
        }
        public string BillingAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.BillingAmountCurrencyType);
            }
        }
        public string BillingAmountShow
        {
            get
            {
                return string.Format("{0} {1}", BillingAmountCurrencyTypeName, BillingAmountVal.HasValue ? BillingAmountVal.Value.ToString("N") : "0.00");
            }
        }
    }
}

namespace SECOM_AJIS.DataEntity.Income.MetaData
{
    /// <summary>
    /// Meta data for doUnpaidBillingDetail class
    /// </summary>
    public class doUnpaidBillingDetail_Meta
    {
        [GridToolTip("BillingType")]
        public string BillingTypeCode { get; set; }

        [GridToolTip("PaymentStatusDesc")]
        public string PaymentStatus { get; set; }
    }
}



