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
    /// Data object of Function for retrieving debt target data.
    /// </summary>
    public partial class doWHTReportForAccount
    {
        public string CustomerNameAndTaxID
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}"
                    , (string.IsNullOrEmpty(this.CustomerName) ? "-" : this.CustomerName)
                    , (string.IsNullOrEmpty(this.TaxID) ? "-" : this.TaxID)
                );
            }
        }
        public string WHTDocNoAndWHTDocDate
        {
            get {
                return string.Format("(1) {0}<br/>(2) {1}"
                    , (string.IsNullOrEmpty(this.WHTDocNo) ? "-" : this.WHTDocNo)
                    , (this.WHTDocDate == null ? "-" : this.WHTDocDate.ToString("dd-MM-yyyy"))
                );
            }
        }

        public string AmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.AmountCurrencyType);
            }
        }
        public decimal? WHTAmountVal
        {
            get
            {
                if (AmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return WHTAmount;
                else if (AmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return WHTAmountUsd;
                else return null;
            }
        }
        public string WHTAmountShow
        {
            get
            {
                return string.Format("{0} {1}", AmountCurrencyTypeName, WHTAmountVal.HasValue ? WHTAmountVal.Value.ToString("N2") : "0:00");
            }
        }

        public decimal? MatchingAmountVal
        {
            get
            {
                if (AmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return MatchingAmount;
                else if (AmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return MatchingAmountUsd;
                else return null;
            }
        }
        public string MatchingAmountShow
        {
            get
            {
                return string.Format("{0} {1}", AmountCurrencyTypeName, MatchingAmountVal.HasValue ? MatchingAmountVal.Value.ToString("N2") : "0:00");
            }
        }
            

        public decimal? DiffVal
        {
            get
            {
                if (AmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return Diff;
                else if (AmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return DiffUsd;
                else return null;
            }
        }
        public string DiffShow
        {
            get
            {
                return string.Format("{0}", DiffVal.HasValue ? DiffVal.Value.ToString("N2") : "0");
            }
        }
    }
}