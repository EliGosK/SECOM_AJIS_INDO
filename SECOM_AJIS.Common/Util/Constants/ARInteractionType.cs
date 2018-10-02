using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of AR interaction type
    /// </summary>
    public class ARInteractionType
    {
        public static string C_AR_INTERACTION_TYPE_APPROVAL_REQUEST { get; private set; }
        public static string C_AR_INTERACTION_TYPE_APPROVE { get; private set; }
        public static string C_AR_INTERACTION_TYPE_AUDIT_REQUEST { get; private set; }
        public static string C_AR_INTERACTION_TYPE_INSTRUCTION { get; private set; }
        public static string C_AR_INTERACTION_TYPE_NEW_REGISTER { get; private set; }
        public static string C_AR_INTERACTION_TYPE_REJECT { get; private set; }
        public static string C_AR_INTERACTION_TYPE_RETURN { get; private set; }
        public static string C_AR_INTERACTION_TYPE_REGISTER_BY_ADMIN { get; private set; }
        public static string C_AR_INTERACTION_TYPE_RETURN_REQUEST { get; private set; }
    }
}
