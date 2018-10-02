using SECOM_AJIS.DataEntity.ExchangeRate.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.RateCalcuration.Util
{
    public class RateCalc
    {
        public static void doCalc(DateTime I_TargetDate, string I_ConvertType, decimal I_Amount, ref decimal O_Amount, ref double O_ErrorCode)
        {
            ConvertExchangeRateHandler handler = new ConvertExchangeRateHandler();
            decimal convertedAmount = handler.ConvertAmountByBankRate(I_TargetDate, I_ConvertType, I_Amount, ref O_ErrorCode);
            O_Amount = convertedAmount;
        }
    }
}
