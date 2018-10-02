using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for misc type
    /// </summary>
    public class MiscType
    {
        public static string C_ACQUISITION_TYPE { get; private set; }
        public static string C_ADDRESS_FULL { get; private set; }
        public static string C_ALL_CHANGE_TYPE { get; private set; }
        public static string C_APPROVE_STATUS { get; private set; }
        public static string C_AR_INTERACTION_TYPE { get; private set; }
        public static string C_AR_ROLE { get; private set; }
        public static string C_AR_SEARCH_PERIOD { get; private set; }
        public static string C_AR_SEARCH_STATUS { get; private set; }
        public static string C_AR_STATUS { get; private set; }
        public static string C_AR_SUMMARY_PERIOD { get; private set; }
        public static string C_AR_TYPE { get; private set; }
        public static string C_BATCH_LAST_RESULT { get; private set; }
        public static string C_BATCH_STATUS { get; private set; }
        public static string C_BILLING_CYCLE { get; private set; }
        public static string C_BILLING_TIMING { get; private set; }
        public static string C_BILLING_TIMING_FOR_CUSTOMER_DISP { get; private set; }
        public static string C_BILLING_TYPE { get; private set; }
        public static string C_BUILDING_TYPE { get; private set; }
        public static string C_CALC_DAILY_FEE_TYPE { get; private set; }
        public static string C_CHANGE_NAME_REASON_TYPE { get; private set; }
        public static string C_CONTRACT_DOC_STATUS { get; private set; }
        public static string C_CONTRACT_SIGNER_TYPE { get; private set; }
        public static string C_CONTRACT_TRANS_STATUS { get; private set; }
        public static string C_CUST_ROLE_TYPE { get; private set; }
        public static string C_CUST_STATUS { get; private set; }
        public static string C_CUST_TYPE { get; private set; }
        public static string C_DEADLINE_TIME_TYPE { get; private set; }
        public static string C_DISPATCH_TYPE { get; private set; }
        public static string C_DISTRIBUTED_TYPE { get; private set; }
        public static string C_DOC_AUDIT_RESULT { get; private set; }
        public static string C_DOCUMENT_TYPE { get; private set; }
        public static string C_EXPANSION_TYPE { get; private set; }
        public static string C_FINANCIAL_MARKET_TYPE { get; private set; }
        public static string C_HANDLING_TYPE { get; private set; }
        public static string C_INCIDENT_INTERACTION_TYPE { get; private set; }
        public static string C_INCIDENT_ROLE { get; private set; }
        public static string C_INCIDENT_SEARCH_DUEDATE { get; private set; }
        public static string C_INCIDENT_SEARCH_PERIOD { get; private set; }
        public static string C_INCIDENT_SEARCH_STATUS { get; private set; }
        public static string C_INCIDENT_STATUS { get; private set; }
        public static string C_INCIDENT_SUMMARY_PERIOD { get; private set; }
        public static string C_INCIDENT_TYPE { get; private set; }
        public static string C_INSTALL_STATUS { get; private set; }
        public static string C_INSTALL_TYPE { get; private set; }
        public static string C_INSTRUMENT_TYPE { get; private set; }
        public static string C_INSURANCE_TYPE { get; private set; }
        public static string C_LINE_UP_TYPE { get; private set; }
        public static string C_LOCK_STATUS { get; private set; }
        public static string C_MA_CYCLE { get; private set; }
        public static string C_MA_FEE_TYPE { get; private set; }
        public static string C_MA_TARGET_PROD_TYPE { get; private set; }
        public static string C_MA_TYPE { get; private set; }
        public static string C_MAIN_STRUCTURE_TYPE { get; private set; }
        public static string C_MOTIVATION_TYPE { get; private set; }
        public static string C_NUM_OF_DATE { get; private set; }
        public static string C_OPERATION_TYPE { get; private set; }
        public static string C_PAYMENT_METHOD { get; private set; }
        public static string C_PAYMENT_METHOD_FOR_CUSTOMER_DISP { get; private set; }
        public static string C_PHONE_LINE_OWNER_TYPE { get; private set; }
        public static string C_PHONE_LINE_TYPE { get; private set; }
        public static string C_PROC_AFT_COUNTER_BALANCE_TYPE { get; private set; }
        public static string C_PROJECT_STATUS { get; private set; }
        public static string C_REASON_TYPE { get; private set; }
        public static string C_RENTAL_CHANGE_TYPE { get; private set; }
        public static string C_SALE_CHANGE_TYPE { get; private set; }
        public static string C_SALE_PROC_MANAGE_STATUS { get; private set; }
        public static string C_SALE_TYPE { get; private set; }
        public static string C_SG_AREA_TYPE { get; private set; }
        public static string C_SG_TYPE { get; private set; }
        public static string C_START_TYPE { get; private set; }
        public static string C_STOP_CANCEL_REASON_TYPE { get; private set; }
        public static string C_CHANGE_REASON_TYPE { get; private set; }
        public static string C_NEW_BLD_MGMT_FLAG { get; private set; }
        

        // additional 29 Jul 2011
        public static string C_RENTAL_INSTALL_TYPE { get; private set; }
        public static string C_RENTAL_IMPLEMENT_TYPE { get; private set; }               
        public static string C_SALE_INSTALL_TYPE { get; private set; }

        // Add for CTS370
        public static string C_AR_SEARCH_DUEDATE { get; private set; }

        public static string C_INSTALL_MANAGE_STATUS { get; private set; }

        public static string C_CUSTOMER_REASON { get; private set; }
        public static string C_SECOM_REASON { get; private set; }

        public static string C_INSTALL_FEE_BILLING_TYPE { get; private set; }
        public static string C_STOCK_OUT_TYPE { get; private set; }
        public static string C_INSTALLATION_BY { get; private set; }
        public static string C_DOC_LANGUAGE { get; private set; }


       
        public static string C_AUTO_TRANSFER_BILLING_TYPE { get; private set; }
        public static string C_BANK_TRANSFER_BILLING_TYPE { get; private set; }
       
        public static string C_CONTRACT_BILLING_TYPE { get; private set; }
       
        public static string C_COVER_LETTER_DOC_CODE { get; private set; }
      
        //public static string C_INSALL_FEE_BILLING_TYPE { get; private set; }
        public static string C_INST_TYPE { get; private set; }
        public static string C_INSTALL_ADJUST_CONTENTS { get; private set; }
        public static string C_INSTALL_ADJUSTMENT { get; private set; }
        public static string C_INSTALL_COMPLAIN { get; private set; }
        public static string C_INSTALL_IE_EVALUATION { get; private set; }
    
        public static string C_INV_AREA { get; private set; }
        public static string C_INV_LOC { get; private set; }
        public static string C_INV_SHELF_TYPE { get; private set; }
      
        public static string C_SLIP_STATUS { get; private set; }
       
        public static string C_PURCHASE_ORDER_TYPE { get; private set; }
        public static string C_TRANSPORT_TYPE { get; private set; }
        public static string C_CURRENCT { get; private set; }
        public static string C_CURRENCY_TYPE { get; private set; }
        public static string C_PURCHASE_ORDER_STATUS { get; private set; }
        public static string C_INV_STOCKIN_TYPE { get; private set; }
        public static string C_INV_REGISTER_ASSET { get; private set; }
       
        public static string C_INV_CHECKING_STATUS { get; private set; }

        public static string C_INSTALL_BEFORE_CHANGE_REASON { get; private set; }
        public static string C_INSTALL_BEFORE_CHANGE_REQUESTER { get; private set; }

        public static string C_SUBCONTRACTOR_LEVEL { get; private set; }
        public static string C_SUBCONTRACTOR_SKILL_LEVEL { get; private set; }

        public static string C_ISSUE_INV_TIME { get; private set; }
        public static string C_ISSUE_INV_DATE { get; private set; }
        public static string C_INV_FORMAT { get; private set; }
        public static string C_SIG_TYPE { get; private set; }
        public static string C_SHOW_DUEDATE { get; private set; }
        public static string C_ISSUE_REC_TIME { get; private set; }
        public static string C_SHOW_BANK_ACC { get; private set; }
        public static string C_SHOW_ISSUE_DATE { get; private set; }
        public static string C_SEP_INV { get; private set; }
        public static string C_DEDUCT_TYPE { get; private set; }
        public static string C_AUTO_TRANSFER_DATE { get; private set; }
        public static string C_ACCOUNT_TYPE { get; private set; }

        public static string C_CREDIT_CARD_TYPE { get; private set; }
        public static string C_CREDIT_CARD_COMPANY { get; private set; }
        public static string C_BILLING_FLAG { get; private set; }
        public static string C_SHOW_AUTO_TRANSFER_RESULT { get; private set; }

        public static string C_PAYMENT_TYPE { get; private set; }

        public static string C_ISSUE_INV { get; private set; }
        public static string C_BILLING_INV_FORMAT { get; private set; }
        public static string C_INV_SLIP_STATUS { get; private set; }

        public static string C_DEPOSIT_STATUS { get; private set; }

        public static string C_PAYMENT_STATUS { get; private set; }
        public static string C_PAYMENT_STATUS_SEARCH { get; private set; }
        public static string C_USED_PAYMENT_METHOD { get; private set; }

        public static string C_PAYMENT_MATCHING_DESC { get; private set; }
        public static string C_ADJUST_TYPE { get; private set; }
        public static string C_INCIDENT_RECEIVED_METHOD { get; private set; }

        public static string C_INV_ACCOUNT_CODE { get; private set; }

        public static string C_RENTAL_COVER_LETTER_DOC_CODE { get; private set; }
        public static string C_SALE_COVER_LETTER_DOC_CODE { get; private set; }

        public static string C_CORRECTION_REASON { get; private set; }
        public static string C_INSTALL_CHANGE_REASON { get; private set; }
        public static string C_ISSUE_INVOICE { get; private set; }
        public static string C_ISSUE_PAYMENT { get; private set; }
        public static string C_FLAG_DISPLAY { get; private set; }

        public static string C_PERMISSION_TYPE { get; private set; }

        public static string C_INV_PROCESS_TYPE { get; private set; }
        public static string C_INC_MISC_WORD { get; private set; }
        
        public static string C_CANCEL_RECEIPT_TARGET { get; private set; }

        public static string C_PROJECT_OFFICE_DUMMY { get; private set; }

        // Akat K. 2014-05-20 : constant for new combobox
        public static string C_PRINT_ADVANCE_DATE { get; private set; }

        public static string C_STOCK_REPORT_TYPE { get; private set; }

        public static string C_UNIT { get; private set; }

        public static string C_ELIMINATE_TRANSFER_TYPE { get; set; }
        public static string C_SUB_CONTRATOR { get; private set; }

        public static string C_DEBT_TRACING_RESULT { get; private set; }
        public static string C_DEBT_TRACING_POSTPONE_REASON { get; private set; }
        public static string C_CHEQUE_RETURN_REASON { get; private set; }
        public static string C_DEBT_TRACING_PAYMENT_METHOD { get; private set; }
        public static string C_DEBT_TRACING_STATUS { get; set; }

        //2017/02/10 add by shibahara start
        public static string C_PAYMENT_MATCHING_STATUS { get; set; }
        //2017/02/10 add by shibahara end

    }
}

