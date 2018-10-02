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
    /// Data object of Payment matching result detail of a payment transaction information.
    /// </summary>
    public partial class doPaymentMatchingResultDetail
    {
        public DateTime GridMatchDate { get; set; }
        public string GridBillingClientNameOrCustomValue { get; set; }
        public string GridPartialPayment { get; set; }

        public string BillingClientName
        {
            get
            {
                string currentLanguage = CommonUtil.GetCurrentLanguage();
                if (currentLanguage == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return BillingClientNameEN;
                }
                else if (currentLanguage == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return BillingClientNameEN;
                }
                else
                {
                    return BillingClientNameLC;
                }
            }
        }
        public string GirdBusinessDecorateFlag { get; set; }

        public string GridMatchDateDisplay
        {
            get
            {
                return CommonUtil.TextDate(this.GridMatchDate);
            }
        }
        public string MatchAmountIncWHTCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.MatchAmountIncWHTCurrencyType);
            }
        }

        public decimal? MatchAmountIncWHTValue
        {
            get
            {
                if (MatchAmountIncWHTCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return MatchAmountIncWHT;
                else if (MatchAmountIncWHTCurrencyType == CurrencyUtil.C_CURRENCY_US) return MatchAmountIncWHTUsd;
                else return null;
            }
        }
        public bool IsMatchAmountIncWHTShowDash { get; set; }
        public string MatchAmountIncWHTShow
        {
            get
            {
                if (IsMatchAmountIncWHTShowDash)
                {
                    if (MatchAmountIncWHTValue != null)
                        return string.Format("{0} {1}", MatchAmountIncWHTCurrencyTypeName, MatchAmountIncWHTValue.Value.ToString("N2"));
                    else
                        return string.Empty;
                }
                else
                {
                    return string.Format("{0} {1}", MatchAmountIncWHTCurrencyTypeName, MatchAmountIncWHTValue?.ToString("N2"));
                }
            }
        }



        public string BillingTargetCodeShortFormat
        {
            get {
                return new CommonUtil().ConvertBillingTargetCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }


        //Sorting Helper for ICS080 in matching result grid
        public int MatchingDetailSection { get; set; }
        public int MatchingDetailSorting { get; set; }
    }
}
