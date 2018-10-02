using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.DataEntity.Income
{
    public partial class doReturnedCheque
    {
        public string PaymentAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.PaymentAmountCurrencyType);
            }
        }
        public decimal? PaymentAmountVal
        {
            get
            {
                if (PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return PaymentAmount;
                else if (PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return PaymentAmountUsd;
                else return null;
            }
        }
        public string PaymentAmountShow
        {
            get
            {
                return string.Format("{0} {1}", PaymentAmountCurrencyTypeName, PaymentAmountVal.HasValue ? PaymentAmountVal.Value.ToString("N2") : "0.00");
            }
        }
    }
}
