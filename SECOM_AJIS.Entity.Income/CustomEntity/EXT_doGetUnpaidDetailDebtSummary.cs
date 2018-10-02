using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Function for retrieving unpaid detail dept summary data.
    /// </summary>
    public partial class doGetUnpaidDetailDebtSummary
    {
        //[LanguageMapping]
        // have no idea why LanguageMapping not working in this custom do ?
        public string SiteName
        {
            get
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return this.SiteNameEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return this.SiteNameEN;
                }
                else
                {
                    return this.SiteNameLC;
                }
            }
        }

        public string BillingType
        {
            get
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return this.BillingTypeEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return this.BillingTypeJP;
                }
                else
                {
                    return this.BillingTypeLC;
                }
            }
        }
        public string PaymentStatusDesc
        {
            get
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return this.PaymentStatusDescEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return this.PaymentStatusDescJP;
                }
                else
                {
                    return this.PaymentStatusDescLC;
                }
            }
        }

        private string _BillingCodeShort;
        public string BillingCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();

                return c.ConvertBillingClientCode(this.BillingCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _BillingCodeShort = value; }
        }

        public string DelayedMonthString
        {
            get
            {
                return string.Format("{0}", this.DelayedMonth.HasValue ? this.DelayedMonth.Value.ToString("#,##0") : "0");
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

        public decimal? BillingAmountValue
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return BillingAmount;
                else
                    return BillingAmountUsd;
            }
        }
    
        public string BillingAmountString
        {
            get
            {
                return string.Format("{0} {1}", BillingAmountCurrencyTypeName, this.BillingAmountValue.HasValue ? this.BillingAmountValue.Value.ToString("#,##0.00") : "0.00");
            }
        }
        public string FirstFeeFlagString
        {
            get
            {
                return string.Format("{0}", this.FirstFeeFlag == true ? "Yes" : "No");
            }
        }
        //public string DebtTracingRegisteredString
        //{
        //    get
        //    {
        //        return string.Format("{0}", this.DebtTracingRegistered == 1 ? "Yes" : "No");
        //    }
        //}
        public string CreditNoteIssuedFlagString
        {
            get
            {
                return string.Format("{0}", this.CreditNoteIssuedFlag == 1 ? "Yes" : "No");
            }
        }

        public string SiteNameGridFormat
        {
            get
            { 
                return string.Format("(1) {0}<br />(2) {1}",this.SiteNameEN, this.SiteNameLC);
            }
        }

        public string DebtTracingRegisteredGridFormat
        {
            get;
            set;
        }

        public string BillingPeriod
        {
            get { 
                string period = CommonUtil.TextDate(this.BillingStartDate);
                if (this.BillingEndDate.HasValue)
                    period += " ~ " + CommonUtil.TextDate(this.BillingEndDate);
                return period;
            }
        }

        public string ToJson
        {
            get
            {
                return CommonUtil.CreateJsonString(this);
            }
        }
    }
}
