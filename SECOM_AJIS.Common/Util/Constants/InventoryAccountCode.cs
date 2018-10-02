using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for inventory account code
    /// </summary>
    public class InventoryAccountCode
    {
        public static string C_INV_ACCOUNT_CODE_INSTOCK { get; private set; }
        public static string C_INV_ACCOUNT_CODE_INPROCESS { get; private set; }
        public static string C_INV_ACCOUNT_CODE_INSTALLED { get; private set; }
        public static string C_INV_ACCOUNT_CODE_SPECIAL { get; private set; }
        public static string C_INV_ACCOUNT_CODE_SALE { get; private set; }
        public static string C_INV_ACCOUNT_CODE_ELIMINATE { get; private set; }
        public static string C_INV_ACCOUNT_CODE_ADJUST { get; private set; }
        public static string C_INV_ACCOUNT_CODE_LOSSBYMA { get; private set; }
    }
}
