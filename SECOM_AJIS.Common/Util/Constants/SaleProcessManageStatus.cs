using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for sale process manage status
    /// </summary>
    public class SaleProcessManageStatus
    {
        public static string C_SALE_PROCESS_STATUS_GENCODE { get; private set; }
        public static string C_SALE_PROCESS_STATUS_SHIP { get; private set; }
        public static string C_SALE_PROCESS_STATUS_ACCEPT_NOTCOMPLETE { get; private set; }
        public static string C_SALE_PROCESS_STATUS_COMPLETE_NOTACCEPT { get; private set; }
        public static string C_SALE_PROCESS_STATUS_COMPLETE_ACCEPT { get; private set; }
        public static string C_SALE_PROCESS_STATUS_CANCEL { get; private set; }
        public static string C_SALE_PROCESS_STATUS_REMOVEALLINSTRUMENT { get; private set; }
    }
}
