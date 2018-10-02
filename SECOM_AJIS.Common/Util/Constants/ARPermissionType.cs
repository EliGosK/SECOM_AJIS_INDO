using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of AR permission type
    /// </summary>
    public class ARPermissionType
    {
        public static string C_AR_PERMISSION_TYPE_REQUESTER { get; private set; }
        public static string C_AR_PERMISSION_TYPE_APPROVER { get; private set; }
        public static string C_AR_PERMISSION_TYPE_AUDITOR { get; private set; }
        public static string C_AR_PERMISSION_TYPE_RECEIPIENT { get; private set; } //Add by Jutarat A. on 29082012

        public static string C_AR_PERMISSION_TYPE_BELONG_SAME_OFFICE { get; private set; }
        public static string C_AR_PERMISSION_TYPE_BELONG_SAME_BRANCH { get; private set; }
        public static string C_AR_PERMISSION_TYPE_BELONG_HQ { get; private set; }

        public static string C_AR_PERMISSION_TYPE_OFFICE_CHIEF { get; private set; }
        public static string C_AR_PERMISSION_TYPE_RELATED_OFFICE { get; private set; }
        public static string C_AR_PERMISSION_TYPE_EXECUTIVE { get; private set; }
        public static string C_AR_PERMISSION_TYPE_PROJECT_MANAGER { get; private set; }
        public static string C_AR_PERMISSION_TYPE_GENERAL { get; private set; }
    }
}