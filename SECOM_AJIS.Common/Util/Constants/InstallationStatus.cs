using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for installation status
    /// </summary>
    public class InstallationStatus
    {
        public static string C_INSTALL_STATUS_NO_INSTALLATION { get; private set; }
        public static string C_INSTALL_STATUS_COMPLETED { get; private set; }
        public static string C_INSTALL_STATUS_INSTALL_CANCELLED { get; private set; }
        public static string C_INSTALL_STATUS_INSTALL_NOT_REGISTERED { get; private set; }
        public static string C_INSTALL_STATUS_INSTALL_NOT_REQUESTED { get; private set; }
        public static string C_INSTALL_STATUS_INSTALL_REQUESTED_PO_NOT_REGISTERED { get; private set; }
        public static string C_INSTALL_STATUS_INSTALL_REQUESTED_PO_REGISTERED { get; private set; }
        public static string C_INSTALL_STATUS_UNDER_INSTALLATION { get; private set; }
        public static string C_INSTALL_STATUS_COMPLETE { get; private set; }
        public static string C_INSTALL_STATUS_REQUEST { get; private set; }
        public static string C_INSTALL_STATUS_PO { get; private set; }
        public static string C_INSTALL_STATUS_SLIP { get; private set; }
        public static string C_INSTALL_STATUS_REGISTER { get; private set; }
        public static string C_INSTALL_STATUS_CANCEL { get; private set; }

    }
}
