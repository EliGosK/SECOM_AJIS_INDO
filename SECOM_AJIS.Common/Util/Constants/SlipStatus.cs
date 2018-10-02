using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for slip status
    /// </summary>
    public class SlipStatus
    {
        public static string C_SLIP_STATUS_INSTALL_SLIP_CANCELED { get; private set; }
        public static string C_SLIP_STATUS_NO_NEED_TO_RETURN { get; private set; }
        public static string C_SLIP_STATUS_NO_NEED_TO_STOCK_OUT { get; private set; }
        public static string C_SLIP_STATUS_NOT_STOCK_OUT { get; private set; }
        public static string C_SLIP_STATUS_PARTIAL_STOCK_OUT { get; private set; }
        public static string C_SLIP_STATUS_REPLACED { get; private set; }
        public static string C_SLIP_STATUS_RETURNED { get; private set; }
        public static string C_SLIP_STATUS_STOCK_OUT { get; private set; }
        public static string C_SLIP_STATUS_WAIT_FOR_RETURN { get; private set; }

    }
}
