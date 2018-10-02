using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data ojbect of Billing target’s unpaid information.
    /// </summary>
    public partial class doUnpaidBillingTarget
    {
        public string BillingClientName
        {
            get
            {
                string currentLang = CommonUtil.GetCurrentLanguage();
                if (currentLang == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return BillingClientNameEN;
                }
                else if (currentLang == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return BillingClientNameEN;
                }
                else
                {
                    return BillingClientNameLC;
                }
            }
        }
        //Show in grid
        public string BillingClientNameGrid
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}", this.BillingClientNameEN, this.BillingClientNameLC);
            }
        }

        public string BillingTargetNoShortFormat
        {
            get {
                return new CommonUtil().ConvertBillingCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string UnpaidTotalBalanceCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.UnpaidTotalBalanceCurrencyType);
            }
        }

        public string LocalCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(CurrencyUtil.C_CURRENCY_LOCAL);
            }
        }
        public string USCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(CurrencyUtil.C_CURRENCY_US);
            }
        }

        public decimal? UnpaidTotalBalanceValue
        {
            get
            {
                if (UnpaidTotalBalanceCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return UnpaidTotalBalance;
                else if (UnpaidTotalBalanceCurrencyType == CurrencyUtil.C_CURRENCY_US) return UnpaidTotalBalanceUsd;
                else return null;
            }
        }

        public string UnpaidTotalBalanceShow
        {
            get
            {
                return string.Format("{0} {1}<br />{2} {3}"
                    , LocalCurrencyTypeName, UnpaidTotalBalance.HasValue ? UnpaidTotalBalance.Value.ToString("N2") : "0:00"
                    , USCurrencyTypeName, UnpaidTotalBalanceUsd.HasValue ? UnpaidTotalBalanceUsd.Value.ToString("N2") : "0:00");
            }
        }
    }
}
