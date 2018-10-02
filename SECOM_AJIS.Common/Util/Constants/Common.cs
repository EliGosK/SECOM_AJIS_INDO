using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of common constants
    /// </summary>
    public class CommonValue
    {
        public const string LANGUAGE_EN = "EN";
        public const string LANGUAGE_JP = "JP";
        public const string LANGUAGE_LC = "LC";

        public static string DEFAULT_LANGUAGE_EN { get; private set; }
        public static string DEFAULT_LANGUAGE_JP { get; private set; }
        public static string DEFAULT_LANGUAGE_LC { get; private set; }
        public static string DEFAULT_SHORT_LANGUAGE_EN { get; private set; }
        public static string DEFAULT_SHORT_LANGUAGE_JP { get; private set; }
        public static string DEFAULT_SHORT_LANGUAGE_LC { get; private set; }

        public static string SESSION_DSCTS030DATA_KEY { get; private set; }
        public static string SESSION_DSCTS270DATA_KEY { get; private set; }

        public static string SESSION_DSTRANSDATA_KEY { get; private set; }
        public static string SESSION_DTSYSTEMSTATUS_KEY { get; private set; }
        public static string SESSION_SCREEN_PARAMETER_KEY { get; private set; }
        public static string SYSTEM_MESSAGE_CODE { get; private set; }
        public static string MESSAGE_NOTFOUND { get; private set; }
        public static string MESSAGE_ASSEMBLY { get; private set; }
        public static string MESSAGE_NAMESPACE { get; private set; }
        public static string ASSEMBLY_FOLDER { get; private set; }
        public static string APP_GLOBAL_RESOURCE_FOLDER { get; private set; }
        public static string TEMPLATE_FOLDER { get; private set; }

        public static string GRID_TEMPLATE_FOLDER { get; private set; }
        public static int MAX_GRID_ROWS { get; private set; }
        public static int ROWS_PER_PAGE_FOR_VIEWPAGE { get; private set; }
        public static int ROWS_PER_PAGE_FOR_SEARCHPAGE { get; private set; }
        public static int ROWS_PER_PAGE_FOR_INVENTORY_CHECKING { get; private set; }

        public static int MENU_MAX_LENGTH_EN { get; private set; }
        public static int MENU_MAX_LENGTH_JP { get; private set; }
        public static int MENU_MAX_LENGTH_LC { get; private set; }

        public static string PERMITTED_IPADDR_FOLDER { get; private set; }
        public static string PERMITTED_IPADDR_FILE { get; private set; }

        public static int GROUP_CODE_LONG_DIGIT { get; private set; }
        public static int GROUP_CODE_SHORT_DIGIT { get; private set; }
        public static int CUST_CODE_LONG_DIGIT { get; private set; }
        public static int CUST_CODE_SHORT_DIGIT { get; private set; }
        public static int SITE_CODE_LONG_DIGIT { get; private set; }
        public static int SITE_CODE_SHORT_DIGIT { get; private set; }
        public static int CONTRACT_CODE_LONG_DIGIT { get; private set; }
        public static int CONTRACT_CODE_SHORT_DIGIT { get; private set; }
        public static int CONTRACT_REALCODE_LONG_DIGIT { get; private set; }
        public static int CONTRACT_REALCODE_SHORT_DIGIT { get; private set; }
        public static int PROJECT_CODE_LONG_DIGIT { get; private set; }
        public static int PROJECT_CODE_SHORT_DIGIT { get; private set; }
        public static int BILLING_TARGET_CODE_LONG_DIGIT { get; private set; }
        public static int BILLING_TARGET_CODE_SHORT_DIGIT { get; private set; }
        public static int BILLING_CODE_LONG_DIGIT { get; private set; }
        public static int BILLING_CODE_SHORT_DIGIT { get; private set; }
        public static int QUOTATION_TARGET_CODE_LONG_DIGIT { get; private set; }
        public static int QUOTATION_TARGET_CODE_SHORT_DIGIT { get; private set; }

        // additional
        public static int BILLING_CLIENT_CODE_LONG_DIGIT { get; private set; }
        public static int BILLING_CLIENT_CODE_SHORT_DIGIT { get; private set; }

        //------------------maxlength for input code
        public static string C_MAXLENGTH_CUSTOMER_CODE { get {return (CUST_CODE_SHORT_DIGIT+1).ToString(); }  }
        public static string C_MAXLENGTH_SITE_CODE { get { return (SITE_CODE_SHORT_DIGIT + 6).ToString(); } }
        public static string C_MAXLENGTH_CONTRACT_CODE { get { return (CONTRACT_CODE_SHORT_DIGIT + 2).ToString(); } }
        public static string C_MAXLENGTH_QUOTATION_TARGE_CODE { get { return (CONTRACT_CODE_SHORT_DIGIT + 3).ToString(); } }
        public static string C_MAXLENGTH_BILLING_CLIENT_CODE { get { return (BILLING_CLIENT_CODE_SHORT_DIGIT + 0).ToString(); } }
        public static string C_MAXLENGTH_BILLING_TARGE_CODE { get { return (BILLING_TARGET_CODE_SHORT_DIGIT + 0).ToString(); } }
        public static string C_MAXLENGTH_BILLING_CODE { get { return (BILLING_CODE_SHORT_DIGIT + 5).ToString(); } }
        //------------------maxlength for input code


        public static string CURRENCY_UNIT { get; private set; }
        public static string TIMES_UNIT { get; private set; }
        public static string PERSON_UNIT { get; private set; }

        public static string IMPORT_TEMPLATE_FILE { get; private set; }
        public static string IMPORT_CSV_MODEL_MAPPING_FILE { get; private set; }

        //Added by Nattapong N.
        public static string SESSION_MENU_KEY { get; private set; }
        public static string DOMAIN_NAME { get; set; }


        public static string QUOTATION_SERVICE_TYPE_FIRE_MONITORING { get; private set; }
        public static string QUOTATION_SERVICE_TYPE_CRIME_PREVENTION { get; private set; }
        public static string QUOTATION_SERVICE_TYPE_EMERGENCY_REPORT { get; private set; }
        public static string QUOTATION_SERVICE_TYPE_FACILITY_MONITORING { get; private set; }

        public static string RUN_BATCH_ALL_JOB_NAME { get; private set; }
        public static string RUN_BATCH_ALL_BATCHCODE { get; private set; }

        public static string C_DUMMY_ID_PREFIX { get; private set; }
        public static string C_SITE_CODE_PREFIX { get; private set; }
        public static string C_CUST_CODE_PREFIX { get; private set; }
        //Added by Natthavat S.
        public static string ALLOW_TYPE_FILE { get; private set; }

        public static string C_MAX_AMOUNT { get; set; }

        public static decimal C_MAX_MONTHLY_FEE_DB { get; private set; }
        public static int C_MAX_BILLING_CYCLE { get; private set; }
        public static decimal C_MAX_MONTHLY_FEE_INPUT
        {
            get
            {
                return C_MAX_MONTHLY_FEE_DB; // add by jirawat jannet on 2016-11-23
                // comment by jirawat jannet on 2016-11-23
                //return Math.Floor(C_MAX_MONTHLY_FEE_DB / C_MAX_BILLING_CYCLE);
            }
        }

        public static int C_AR_MAXIMUM_RECEIPIENT { get; private set; } //Add by Jutarat A. on 29082012
    }
}
