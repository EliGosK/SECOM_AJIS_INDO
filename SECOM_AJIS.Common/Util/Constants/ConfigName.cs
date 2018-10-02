using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO of config name
    /// </summary>
    public class ConfigName
    {
        public static string C_CONFIG_BACKUP_REPORT_PATH { get; private set; }
        public static string C_CONFIG_DEFAULT_BILLING_CYCLE { get; private set; }
        public static string C_CONFIG_DEFAULT_MA_CYCLE { get; private set; }
        public static string C_CONFIG_DEPARTMENT_NOTIFY_CHANGE_FEE { get; private set; }
        public static string C_CONFIG_RESUME_SERVICE_TIME { get; private set; }
        public static string C_CONFIG_SUSPEND_FLAG { get; private set; }
        public static string C_CONFIG_SUSPEND_SERVICE_TIME { get; private set; }
        public static string C_EMAIL_SUFFIX { get; private set; }

        public static string C_CONFIG_ATTACH_FILE_PATH { get; private set; }      
        public static string C_CONFIG_TEMP_ATTACH_FILE_PATH { get; private set; }
        public static string C_CONFIG_INSTALL_WARRANTY_COND { get; private set; }
        public static string C_CONFIG_DEPRECIATION_PERIOD_CONTRACT { get; private set; }
        public static string C_CONFIG_DEPRECIATION_PERIOD_REVENUE { get; private set; }
        public static string C_CONFIG_SCRAP_VALUE { get; private set; }
        public static string C_CONFIG_WILDCARD { get; private set; }
        public static string C_CONFIG_DOC_OCC_DEFAULT { get; private set; }
        public static string C_VAT_THB { get; private set; }
        public static string C_WHT { get; private set; }
        public static string C_INV_AREA_SHORT { get; private set; }
        public static string C_INV_SLIP_PREFIX { get; private set; }

        public static string C_CONFIG_PAYMENT_DATA_FILE_PATH { get; private set; }
        public static string C_CONFIG_PRINTING_FLAG { get; private set; } //Add by Jutarat A. on 17092013

        public static string C_CONFIG_INSTALLATION_REQUEST_EMAIL { get; private set; }

        public static string C_CONFIG_SPECIAL_STOCKOUT_MATERIAL { get; set; }
        public static string C_CONFIG_INV_STOCK_LOC { get; private set; }
        public static string C_CONFIG_INV_WIP_LOC { get; private set; }

        public static string C_CONFIG_PURCHASE_EMAIL { get; set; }
        public static string C_CONFIG_BLS050_N_BILLINGTYPEFOROCC { get; set; }
        public static string C_CONFIG_BLS050_Q_BILLINGTYPEFOROCC { get; set; }

        public static string C_CONFIG_DEBT_TRACING_WAIT_MATCHING_DAY { get; set; }

        public static string C_CONFIG_CHANGEPLAN_BEFORE_START_EMAIL { get; set; }

        public static string C_CONFIG_ACCOUNTING_BUSINESS_DAYS { get; set; }
    }
}
