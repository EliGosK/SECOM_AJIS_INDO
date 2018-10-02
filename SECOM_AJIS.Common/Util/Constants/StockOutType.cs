using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for stock out type
    /// </summary>
    public class StockOutType
    {      
        public static string C_STOCK_OUT_TYPE_BRINGING_TO_DEPOT { get; private set; }
        public static string C_STOCK_OUT_TYPE_BRINGING_TO_SIT { get; private set; }
        public static string C_STOCK_OUT_TYPE_BRINGING_TO_SUBCONTRACT_OFFICE { get; private set; }
        public static string C_STOCK_OUT_TYPE_GOING_TO_WH_FOR_TAKING { get; private set; }
        public static string C_STOCK_OUT_TYPE_SPECIAL_ADJUSTMENT { get; private set; }


    }
}
