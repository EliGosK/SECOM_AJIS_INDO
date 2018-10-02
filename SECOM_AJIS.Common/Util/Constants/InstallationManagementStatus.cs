using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for installation management status
    /// </summary>
    public class InstallationManagementStatus
    {
        public static string C_INSTALL_MANAGE_STATUS_APPROVED { get; private set; }
        public static string C_INSTALL_MANAGE_STATUS_CANCELED { get; private set; }
        public static string C_INSTALL_MANAGE_STATUS_COMPLETED { get; private set; }
        public static string C_INSTALL_MANAGE_STATUS_PROCESSING { get; private set; }
        public static string C_INSTALL_MANAGE_STATUS_REJECTED { get; private set; }
        public static string C_INSTALL_MANAGE_STATUS_REQUEST_APPROVE { get; private set; }

    }
}
