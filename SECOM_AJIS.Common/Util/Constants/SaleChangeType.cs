using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for sale change type
    /// </summary>
    public class SaleChangeType
    {
        public static string C_SALE_CHANGE_TYPE_NEW_SALE { get; private set; }
        public static string C_SALE_CHANGE_TYPE_ADD_SALE { get; private set; }
        public static string C_SALE_CHANGE_TYPE_CANCEL { get; private set; }
        public static string C_SALE_CHANGE_TYPE_CHANGE_NAME { get; private set; }
        public static string C_SALE_CHANGE_TYPE_EXCHANGE_INSTR { get; private set; }
        public static string C_SALE_CHANGE_TYPE_MOVE_INSTR { get; private set; }
        public static string C_SALE_CHANGE_TYPE_REMOVE_INSTR_PARTIAL { get; private set; }
        public static string C_SALE_CHANGE_TYPE_REMOVE_INSTR_ALL { get; private set; }
        public static string C_SALE_CHANGE_TYPE_CHANGE_WIRING { get; private set; } //Add by Jutarat A. on 21052013
    }
}
