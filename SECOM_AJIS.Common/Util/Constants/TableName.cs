using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for table name
    /// </summary>
    public class TableName
    {
        public static string C_TBL_NAME_QTN_BASIC { get; private set; }
        public static string C_TBL_NAME_QTN_CUST { get; private set; }
        public static string C_TBL_NAME_QTN_FAC { get; private set; }
        public static string C_TBL_NAME_QTN_INST { get; private set; }
        public static string C_TBL_NAME_QTN_OPER_TYPE { get; private set; }
        public static string C_TBL_NAME_QTN_SITE { get; private set; }
        public static string C_TBL_NAME_QTN_TARGET { get; private set; }
        public static string C_TBL_NAME_AR_APVNO_RUNNO { get; private set; }
        public static string C_TBL_NAME_AR_NO_RUNNO { get; private set; }
        public static string C_TBL_NAME_INCIDENT_RUNNO { get; private set; }
        public static string C_TBL_NAME_BILLING_TEMP { get; private set; }
        public static string C_TBL_NAME_BILLING_CLIENT { get; private set; }
        public static string C_TBL_NAME_QTN_BE { get; private set; }
        public static string C_TBL_NAME_QTN_SG { get; private set; }
        public static string C_TBL_NAME_QTN_MA { get; private set; }
        public static string C_TBL_NAME_SALE_BASIC { get; private set; }
        public static string C_TBL_NAME_SALE_INST_DET { get; private set; }
        public static string C_TBL_NAME_SALE_INST_SUB { get; private set; }
        public static string C_TBL_NAME_SUBCONTRACTOR { get; private set; }
        public static string C_TBL_NAME_SHELF { get; private set; }
        public static string C_TBL_NAME_SAFETY_STOCK { get; private set; }
        public static string C_TBL_NAME_PROJ_STOCKOUT_INST { get; private set; }
        public static string C_TBL_NAME_CAN_ContractMemo { get; private set; }
        public static string C_TBL_NAME_CAN_ContractMemo_Detail { get; private set; }
        public static string C_TBL_NAME_RNT_BE_DET { get; private set; }
        public static string C_TBL_NAME_RNT_CONTRACT_BASIC { get; private set; }
        public static string C_TBL_NAME_RNT_INST { get; private set; }
        public static string C_TBL_NAME_RNT_MA { get; private set; }
        public static string C_TBL_NAME_RNT_OPER_TYPE { get; private set; }
        public static string C_TBL_NAME_RNT_SECURITY_BASIC { get; private set; }
        public static string C_TBL_NAME_RNT_SG { get; private set; }
        public static string C_TBL_NAME_RNT_SG_DET { get; private set; }
        public static string C_TBL_NAME_RNT_SUBCONT { get; private set; }
        public static string C_TBL_NAME_INSTRUMENT { get; private set; }
        public static string C_TBL_NAME_INSTRUMENT_EXPANSION { get; private set; }
        public static string C_TBL_NAME_CON_EMAIL { get; private set; }
        public static string C_TBL_NAME_MAIN_CHKUP { get; private set; }
        public static string C_TBL_NAME_MAIN_CHKUP_DET { get; private set; }
        public static string C_TBL_NAME_RELATION_TYPE { get; private set; }
        public static string C_TBL_NAME_GROUP { get; private set; }
        public static string C_TBL_NAME_CUSTOMER { get; private set; }
        public static string C_TBL_NAME_SITE { get; private set; }
        public static string C_TBL_NAME_DRF_RNT_CONTRACT { get; private set; }
        public static string C_TBL_NAME_DRF_RNT_BE { get; private set; }
        public static string C_TBL_NAME_DRF_RNT_BILLING { get; private set; }
        public static string C_TBL_NAME_DRF_RNT_EMAIL { get; private set; }
        public static string C_TBL_NAME_DRF_RNT_INST { get; private set; }
        public static string C_TBL_NAME_DRF_RNT_MA { get; private set; }
        public static string C_TBL_NAME_DRF_RNT_OPER_TYPE { get; private set; }
        public static string C_TBL_NAME_DRF_RNT_SG { get; private set; }
        public static string C_TBL_NAME_DRF_RNT_SG_DET { get; private set; }
        public static string C_TBL_NAME_CONTRACT_DOC { get; private set; }
        public static string C_TBL_NAME_DOC_CAN_CONTRACT_MEMO { get; private set; }
        public static string C_TBL_NAME_DOC_CAN_CONTRAL_MEMO_DET { get; private set; }
        public static string C_TBL_NAME_DOC_CHANGE_MEMO { get; private set; }
        public static string C_TBL_NAME_DOC_CHANGE_NOTICE { get; private set; }
        public static string C_TBL_NAME_DOC_CHANGE_FREE_MEMO { get; private set; }
        public static string C_TBL_NAME_DOC_CONTRACT_RPT { get; private set; }
        public static string C_TBL_NAME_DOC_CONF_CUR_INSTUMENT_MEMO { get; private set; }
        public static string C_TBL_NAME_DOC_INSTUMENT_DET { get; private set; }
        public static string C_TBL_NAME_DOC_START_MEMO { get; private set; } //Add by Jutarat A. on 22042013
        public static string C_TBL_NAME_DRF_SALE_BILLING { get; private set; }
        public static string C_TBL_NAME_DRF_SALE_CONTRACT { get; private set; }
        public static string C_TBL_NAME_DRF_SALE_EMAIL { get; private set; }
        public static string C_TBL_NAME_DRF_SALE_INST { get; private set; }
        public static string C_TBL_NAME_PRJ { get; private set; }
        public static string C_TBL_NAME_PRJ_BRA_STOCKOUT { get; private set; }
        public static string C_TBL_NAME_PRJ_CUST { get; private set; }
        public static string C_TBL_NAME_PRJ_EXP_INST { get; private set; }
        public static string C_TBL_NAME_PRJ_OTH_COMP { get; private set; }
        public static string C_TBL_NAME_PRJ_PRJ_SYSTEM { get; private set; }
        public static string C_TBL_NAME_PRJ_STOCKOUT { get; private set; }
        public static string C_TBL_NAME_PRJ_STOCKOUT_MEMO { get; private set; }
        public static string C_TBL_NAME_PRJ_SUP_STAFF { get; private set; }
        public static string C_TBL_NAME_INCIDENT { get; private set; }
        public static string C_TBL_NAME_INCIDENT_ROLE { get; private set; }
        public static string C_TBL_NAME_EMPLOYEE { get; private set; }
        public static string C_TBL_NAME_BELONGING { get; private set; }
        public static string C_TBL_NAME_PERMISSION_GROUP { get; private set; }
        public static string C_TBL_NAME_PERMISSION_DETAIL { get; private set; }
        public static string C_TBL_NAME_PERMISSION_IND_DETAIL { get; private set; }
        public static string C_TBL_NAME_PERMISSION_IND { get; private set; }

        public static string C_TBL_NAME_INS_BASIC  { get; private set; }
        public static string C_TBL_NAME_INS_EMAIL  { get; private set; }
        public static string C_TBL_NAME_INS_INST  { get; private set; }	 
        public static string C_TBL_NAME_INS_HIS  { get; private set; }
        public static string C_TBL_NAME_INS_HIS_DET  { get; private set; }
        public static string C_TBL_NAME_INS_MA  { get; private set; }
        public static string C_TBL_NAME_INS_MEMO  { get; private set; }
        public static string C_TBL_NAME_INS_PO_MA  { get; private set; }
        public static string C_TBL_NAME_INS_SLIP  { get; private set; }
        public static string C_TBL_NAME_INS_SLIP_DET { get; private set; }
        public static string C_TBL_NAME_INCIDENT_HISTORY { get; private set; }
        public static string C_TBL_NAME_INCIDENT_HISTORY_DETAIL { get; private set; }

        public static string C_TBL_NAME_AR { get; private set; }
        public static string C_TBL_NAME_AR_ROLE { get; private set; }
        public static string C_TBL_NAME_AR_FEE_ADJUSTMENT { get; private set; }
        public static string C_TBL_NAME_AR_HISTORY { get; private set; }
        public static string C_TBL_NAME_AR_HISTORY_DETAIL { get; private set; }

        public static string C_TBL_NAME_BILLING_BASIC { get; private set; }
        public static string C_TBL_NAME_BILLING_DETAIL { get; private set; }
        public static string C_TBL_NAME_BILLING_TARGET { get; private set; }
        public static string C_TBL_NAME_BILLING_TYPE_DETAIL { get; private set; }
        public static string C_AR_STATUS_RETURNED_REQUEST { get; private set; }
        public static string C_AR_STATUS_WAIT_FOR_APPROVAL { get; private set; }
        public static string C_TBL_NAME_MONTHLY_HISTORY { get; private set; }

        public static string C_TBL_NAME_ACCOUNT_STOCK_MOVING { get; private set; }

        public static string C_TBL_NAME_INV_SLIP { get; private set; }
        public static string C_TBL_NAME_INV_SLIP_DETAIL { get; private set; }
        public static string C_TBL_NAME_INV_BOOKING { get; private set; }
        public static string C_TBL_NAME_INV_BOOKING_DETAIL { get; private set; }
        public static string C_TBL_NAME_INV_CHECKING_SCHEDULE { get; private set; }
        public static string C_TBL_NAME_INV_CHECKING_SLIP { get; private set; }
        public static string C_TBL_NAME_INV_CHECKING_SLIP_DETAIL { get; private set; }
        public static string C_TBL_NAME_INV_CHECKING_TMP { get; private set; }
        public static string C_TBL_NAME_INV_CURRENT { get; private set; }
        public static string C_TBL_NAME_INV_DEPRECIATION { get; private set; }
        public static string C_TBL_NAME_INV_PURCHASE { get; private set; }
        public static string C_TBL_NAME_INV_PURCHASE_DETAIL { get; private set; }
        public static string C_TBL_NAME_INV_ACC_INSTOCK { get; private set; }
        public static string C_TBL_NAME_INV_ACC_INPROCESS { get; private set; }
        public static string C_TBL_NAME_INV_ACC_INSTALLED { get; private set; }
        public static string C_TBL_NAME_INV_ACC_SAMPLE_INSTOCK { get; private set; }
        public static string C_TBL_NAME_INV_ACC_SAMPLE_INPROCESS { get; private set; }
        public static string C_TBL_NAME_INV_ACC_STOCK_MOVING { get; private set; }
        public static string C_TBL_NAME_INV_PROJECT_WIP { get; private set; }
        public static string C_TBL_NAME_AUTO_BANK { get; private set; }
        public static string C_TBL_NAME_CREDIT_CARD { get; private set; }
        public static string C_TBL_NAME_CREDIT_NOTE { get; private set; } //Add by Jutarat A. on 26042013
        public static string C_TBL_NAME_INS_ATTH_FILE { get; private set; }

        public static string C_TBL_NAME_PAYMENT { get; private set; }
        public static string C_TBL_NAME_MATCH_PAYMENT_HEADER { get; private set; }
        public static string C_TBL_NAME_MATCH_PAYMENT_DETAIL { get; private set; }
        public static string C_TBL_NAME_RECEIPT { get; private set; }
        public static string C_TBL_NAME_TMP_IMPORT_CONTENT { get; private set; }
        public static string C_TBL_NAME_PAYMENT_IMPORT_FILE { get; private set; }

        public static string C_TBL_NAME_AUTO_TRANSFER_ACC { get; private set; }
        public static string C_TBL_NAME_DEPOSIT_FEE { get; private set; }

        public static string C_TBL_NAME_INVOICE { get; private set; }
        public static string C_TBL_NAME_TAX_INVOICE { get; private set; }
        public static string C_TBL_NAME_DEBT_TARGET { get; private set; }

        //Add by Jutarat A. on 30052013
        public static string C_TBL_NAME_MONEY_COLLECTION_INFO { get; private set; }
        public static string C_TBL_NAME_BILLING_TARGET_DEBT_TRACING { get; private set; }
        public static string C_TBL_NAME_INVOICE_DEBT_TRACING { get; private set; }
        public static string C_TBL_NAME_REVENUE { get; private set; }
        //End Add

        public static string C_TBL_NAME_INCOME_WHT { get; set; }
        public static string C_TBL_NAME_QTN_INSTALL_DTL { get; private set; }

        public static string C_TBL_NAME_DEBT_TRACING_HISTORY { get; set; }
        public static string C_TBL_NAME_DEBT_TRACING_HISTORY_DETAIL { get; set; }
        public static string C_TBL_NAME_DEBT_TRACING_CUST_CONDITION { get; set; }

    }
}
