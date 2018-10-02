using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of AR role
    /// </summary>
    public class ARRole
    {
        public static string C_AR_ROLE_APPROVER { get; private set; }
        public static string C_AR_ROLE_AUDITOR { get; private set; }
        public static string C_AR_ROLE_REQUESTER { get; private set; }
        public static string C_AR_ROLE_CHIEF_OF_RELATED_OFFICE { get; private set; }
        public static string C_AR_ROLE_RECEIPIENT { get; private set; } //Add by Jutarat A. on 29082012
    }
}
