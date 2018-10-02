using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for document audit result
    /// </summary>
    public class DocAuditResult
    {
        public static string C_DOC_AUDIT_RESULT_APPROVAL_DOCUMENT { get; private set; }
        public static string C_DOC_AUDIT_RESULT_HQ_RECEIVED { get; private set; }
        public static string C_DOC_AUDIT_RESULT_NEED_FOLLOW { get; private set; }
        public static string C_DOC_AUDIT_RESULT_NO_NEED_TO_RECEIVE { get; private set; }
        public static string C_DOC_AUDIT_RESULT_NOT_RECEIVED { get; private set; }
        public static string C_DOC_AUDIT_RESULT_OTHER { get; private set; }
        public static string C_DOC_AUDIT_RESULT_RECEIVED { get; private set; }
        public static string C_DOC_AUDIT_RESULT_RETURNED { get; private set; }
    }
}
