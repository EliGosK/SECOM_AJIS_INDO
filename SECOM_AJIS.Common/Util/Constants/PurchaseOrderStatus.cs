

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for purchase order status
    /// </summary>
    public class PurchaseOrderStatus
    {
        public static string C_PURCHASE_ORDER_STATUS_COMPLETE_RECEIVE { get; private set; }
        public static string C_PURCHASE_ORDER_STATUS_PARTIAL_RECEIVE { get; private set; }
        public static string C_PURCHASE_ORDER_STATUS_WAIT_TO_RECEIVE { get; private set; }

    }
}
