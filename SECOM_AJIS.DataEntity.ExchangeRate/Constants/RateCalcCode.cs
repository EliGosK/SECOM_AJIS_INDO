using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.DataEntity.ExchangeRate.ConstantValue
{
    public class RateCalcCode
    {
        public static string C_CONVERT_TYPE_TO_DOLLAR = "1";
        public static string C_CONVERT_TYPE_TO_RPIAH = "2";

        public static double C_NO_ERROR = 0;
        public static double C_ERROR_NO_RATE = 1;
        public static double C_ERROR_OVER_DIGIT = 2;
        public static double C_ERROR_OTHER = 9;
    }
}
