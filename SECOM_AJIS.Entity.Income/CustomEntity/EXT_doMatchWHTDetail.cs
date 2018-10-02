using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Income.MetaData;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data ojbect of payment information.
    /// </summary>
    public partial class doMatchWHTDetail
    {
        [LanguageMapping]
        public string VATRegistantName { get; set; }

        public decimal? WHTAmountVal
        {
            get
            {
                if (WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return WHTAmount;
                else if (WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return WHTAmountUsd;
                else return null;
            }
        }
        public string WHTAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.WHTAmountCurrencyType);
            }
        }
        public string WHTAmountShow
        {
            get
            {
                return string.Format("{0} {1}", WHTAmountCurrencyTypeName, WHTAmountVal.HasValue ? WHTAmountVal.Value.ToString("N2") : "0:00");
            }
        }
    }
}
