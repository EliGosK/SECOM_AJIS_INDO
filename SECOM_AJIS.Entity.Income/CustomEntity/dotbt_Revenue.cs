using SECOM_AJIS.Common.Util.ConstantValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.DataEntity.Income
{
    public partial class tbt_Revenue
    {
        public decimal RevenueAmountIncVATVal
        {
            get
            {
                if (RevenueAmountIncVATCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return RevenueAmountIncVAT.Value;
                else if (RevenueAmountIncVATCurrencyType == CurrencyUtil.C_CURRENCY_US) return RevenueAmountIncVATUsd;
                else return 0;
            }
        }
        public decimal? RevenueVATAmountVal
        {
            get
            {
                if (RevenueVATAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return RevenueVATAmount;
                else if (RevenueVATAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return RevenueVATAmountUsd;
                else return null;
            }
        }
    }
}
