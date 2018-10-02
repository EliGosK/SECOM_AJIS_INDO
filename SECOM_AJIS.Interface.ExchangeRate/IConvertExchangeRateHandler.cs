using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.DataEntity.ExchangeRate
{
    public interface IConvertExchangeRateHandler
    {
        double? GetConvertedPrice(DateTime targetDate);
    }
}
