using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for module id
    /// </summary>
    public class ModuleID
    {
        public static int C_MODULE_ID_BILLING { get; private set; }
        public static int C_MODULE_ID_COMMON { get; private set; }
        public static int C_MODULE_ID_CONTRACT { get; private set; }
        public static int C_MODULE_ID_INCOME { get; private set; }
        public static int C_MODULE_ID_INSTALLATION { get; private set; }
        public static int C_MODULE_ID_INVENTORY { get; private set; }
        public static int C_MODULE_ID_MASTER { get; private set; }
        public static int C_MODULE_ID_QUOTATION { get; private set; }
    }
}
