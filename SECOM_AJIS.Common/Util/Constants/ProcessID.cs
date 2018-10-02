


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for process id
    /// </summary>
    public class ProcessID
    {
        public static string C_PROCESS_ID_MANAGE_BILLING_DETAIL_MONTHLY_FEE { get; private set; }
        public static string C_PROCESS_ID_MANAGE_INVOICE { get; private set; }
        public static string C_INV_PROCESS_ID_GEN_BLANK_SLIP { get; private set; }
        public static string C_PROCESS_ID_PRINTING_SERVICE { get; private set; }
        public static string C_PROCESS_ID_GENERATE_ISSUE_LIST { get; private set; }
        public static string C_PROCESS_ID_GEN_ACCOUNT_CARRY_OVER_AND_PROFIT { get; private set; }

        public static string C_PROCESS_ID_GEN_AUTO_FILE { get; private set; }
        public static string C_INV_PROCESS_ID_AUTO_COMPLETE_CHECKING { get; private set; }

        //Income
        public static string C_PROCESS_ID_GENERATE_RECEIPT_AFTER_PAYMENT { get; private set; }
        public static string C_PROCESS_ID_GENERATE_RECEIPT_ADVANCE { get; private set; }
        public static string C_PROCESS_ID_GENERATE_DEBT_SUMMARY { get; private set; }
        public static string C_PROCESS_ID_DELETE_DEBT_TRACING { get; private set; }

        public static string C_PROCESS_ID_AUTO_RENEW { get; private set; }

        public static string C_PROCESS_ID_PRINTING_DEBT_NOTICE_SALE { get; private set; }
        public static string C_PROCESS_ID_PRINTING_DEBT_NOTICE_RENTAL { get; private set; }

        //Billing
        public static string C_PROCESS_ID_MANAGE_BILLING_BASIC { get; private set; }
        public static string C_PROCESS_ID_MANAGE_BILLING_DETAIL { get; private set; }
        public static string C_PROCESS_ID_MANAGE_BILLING_BASIC_START { get; private set; }
        public static string C_PROCESS_ID_MANAGE_BILLING_BASIC_RESUME { get; private set; }
        public static string C_PROCESS_ID_CALCULATE_DIFF { get; private set; }
        public static string C_PROCESS_ID_MANAGE_BILLING_BASIC_STOP { get; private set; }
        public static string C_PROCESS_ID_MANAGE_BILLING_BASIC_CANCEL { get; private set; }
        public static string C_PROCESS_ID_MANAGE_BILLING_BASIC_CHANGE { get; private set; }
    }
}
