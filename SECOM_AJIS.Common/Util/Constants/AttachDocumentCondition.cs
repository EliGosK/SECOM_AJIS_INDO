using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of attach document condition
    /// </summary>
    public class AttachDocumentCondition
    {
        public static int C_ATTACH_DOCUMENT_MAXIMUM_FILE_SIZE { get; private set; }
        public static int C_ATTACH_DOCUMENT_MAXIMUM_NUMBER { get; private set; }
        public static string C_CONFIG_TEMP_ATTACH_FILE_PATH { get; private set; }
        public static string C_CONFIG_ATTACH_FILE_PATH { get; private set; }
        public static string C_CONFIG_MODULE_AR { get; private set; }
        public static string C_CONFIG_MODULE_INCIDENT { get; private set; }
        public static string C_CONFIG_MODULE_PROJECT { get; private set; }
        public static string C_CONFIG_MODULE_INSTALLATION { get; private set; }
    }
}
