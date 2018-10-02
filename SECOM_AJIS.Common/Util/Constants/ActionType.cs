using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of action type
    /// </summary>
    public class ActionType
    {
        public static string C_ACTION_TYPE_APPROVE { get; private set; }
        public static string C_ACTION_TYPE_CANCEL { get; private set; }
        public static string C_ACTION_TYPE_CHANGE { get; private set; }
        public static string C_ACTION_TYPE_DRAFT { get; private set; }
        public static string C_ACTION_TYPE_START { get; private set; }
    }
}
