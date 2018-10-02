using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for function id
    /// </summary>
    public class FunctionID
    {
        public static string C_FUNC_ID_ADD { get; private set; }
        public static string C_FUNC_ID_APPROVE { get; private set; }
        public static string C_FUNC_ID_CANCEL { get; private set; }
        public static string C_FUNC_ID_COMPLETE { get; private set; }
        public static string C_FUNC_ID_DEL { get; private set; }
        public static string C_FUNC_ID_DOWNLOAD { get; private set; }
        public static string C_FUNC_ID_EDIT { get; private set; }
        public static string C_FUNC_ID_EDIT_FEE { get; private set; }
        public static string C_FUNC_ID_OPERATE { get; private set; }
        public static string C_FUNC_ID_OPERATE_NO_CONTROL_AUTHORITY { get; private set; }
        public static string C_FUNC_ID_PROJECT_LAST_COMPLETE { get; private set; }
        public static string C_FUNC_ID_REDOWNLOAD { get; private set; }
        public static string C_FUNC_ID_SPECIAL_ADD_INCIDENT_ROLE { get; private set; }
        public static string C_FUNC_ID_SPECIAL_VIEW_CONFIDENTIAL { get; private set; }
        public static string C_FUNC_ID_SPECIAL_VIEW_EDIT_AR { get; private set; }
        public static string C_FUNC_ID_SPECIAL_VIEW_EDIT_INCIDENT { get; private set; }
        public static string C_FUNC_ID_VIEW { get; private set; }
        public static string C_FUNC_ID_VIEW_AR_OFFICE { get; private set; }
        public static string C_FUNC_ID_VIEW_INCIDENT_LIST { get; private set; }
        public static string C_FUNC_ID_VIEW_INCIDENT_OFFICE { get; private set; }
        public static string C_FUNC_ID_CAREFUL_SPECIAL { get; private set; }
        public static string C_FUNC_ID_REGISTER_AUTO_TRANSFER { get; private set; }
        public static string C_FUNC_ID_REGISTER_CREDIT_CARD { get; private set; }
        public static string C_FUNC_ID_SET_CAREFUL_SPECIAL { get; private set; }
        public static string C_FUNC_ID_CANCEL_BILLING_DETAIL { get; private set; }
        public static string C_FUNC_ID_SPECIAL_CREATE { get; private set; }
        public static string C_FUNC_ID_SPECIAL_STOCK_IN { get; private set; }
        public static string C_FUNC_ID_PLANNER { get; private set; }
        public static string C_FUNC_ID_INSTALL_ADVANCE_PAY { get; private set; }
        public static string C_FUNC_ID_SPECIAL_CANCEL { get; private set; }
        public static string C_FUNC_ID_ENCASH { get; private set; }
        public static string C_FUNC_ID_SPECIAL_STOCK_OUT_MATERIAL { get; set; }
        public static string C_FUNC_ID_TRANSFER_TO_BRANCH { get; set; }

        public const string FUNC_ID_ADD = "C_FUNC_ID_ADD";
        public const string FUNC_ID_APPROVE = "C_FUNC_ID_APPROVE";
        public const string FUNC_ID_CANCEL = "C_FUNC_ID_CANCEL";
        public const string FUNC_ID_COMPLETE = "C_FUNC_ID_COMPLETE";
        public const string FUNC_ID_DEL = "C_FUNC_ID_DEL";
        public const string FUNC_ID_DOWNLOAD = "C_FUNC_ID_DOWNLOAD";
        public const string FUNC_ID_EDIT = "C_FUNC_ID_EDIT";
        public const string FUNC_ID_EDIT_FEE = "C_FUNC_ID_EDIT_FEE";
        public const string FUNC_ID_OPERATE = "C_FUNC_ID_OPERATE";
        public const string FUNC_ID_OPERATE_NO_CONTROL_AUTHORITY = "C_FUNC_ID_OPERATE_NO_CONTROL_AUTHORITY";
        public const string FUNC_ID_PROJECT_LAST_COMPLETE = "C_FUNC_ID_PROJECT_LAST_COMPLETE";
        public const string FUNC_ID_REDOWNLOAD = "C_FUNC_ID_REDOWNLOAD";
        public const string FUNC_ID_SPECIAL_ADD_INCIDENT_ROLE = "C_FUNC_ID_SPECIAL_ADD_INCIDENT_ROLE";
        public const string FUNC_ID_SPECIAL_VIEW_CONFIDENTIAL = "C_FUNC_ID_SPECIAL_VIEW_CONFIDENTIAL";
        public const string FUNC_ID_SPECIAL_VIEW_EDIT_AR = "C_FUNC_ID_SPECIAL_VIEW_EDIT_AR";
        public const string FUNC_ID_SPECIAL_VIEW_EDIT_INCIDENT = "C_FUNC_ID_SPECIAL_VIEW_EDIT_INCIDENT";
        public const string FUNC_ID_VIEW = "C_FUNC_ID_VIEW";
        public const string FUNC_ID_VIEW_AR_OFFICE = "C_FUNC_ID_VIEW_AR_OFFICE";
        public const string FUNC_ID_VIEW_INCIDENT_LIST = "C_FUNC_ID_VIEW_INCIDENT_LIST";
        public const string FUNC_ID_VIEW_INCIDENT_OFFICE = "C_FUNC_ID_VIEW_INCIDENT_OFFICE";
        public const string FUNC_ID_CAREFUL_SPECIAL = "C_FUNC_ID_CAREFUL_SPECIAL";
        public const string FUNC_ID_REGISTER_AUTO_TRANSFER = "C_FUNC_ID_REGISTER_AUTO_TRANSFER";
        public const string FUNC_ID_REGISTER_CREDIT_CARD = "C_FUNC_ID_REGISTER_CREDIT_CARD";
        public const string FUNC_ID_ENCASH = "C_FUNC_ID_ENCASH";
        public const string FUNC_ID_SPECIAL_STOCK_OUT_MATERIAL = "C_FUNC_ID_SPECIAL_STOCK_OUT_MATERIAL";
        public const string FUNC_ID_TRANSFER_TO_BRANCH = "C_FUNC_ID_TRANSFER_TO_BRANCH";
    }
}
