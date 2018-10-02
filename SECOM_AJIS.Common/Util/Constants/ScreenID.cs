using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SECOM_AJIS.Common.Util.ConstantValue
{
    /// <summary>
    /// DO for screen id
    /// </summary>
    public class ScreenID
    {
        public static string C_SCREEN_ID_REGIST_BILL_TARGET { get; private set; }
        public static string C_SCREEN_ID_EDIT_BILL_TARGET { get; private set; }
        public static string C_SCREEN_ID_REGIST_BILL_BASIC { get; private set; }
        public static string C_SCREEN_ID_EDIT_BILL_BASIC { get; private set; }
        public static string C_SCREEN_ID_MANAGE_DETAIL { get; private set; }
        public static string C_SCREEN_ID_MANAGE_BILLING { get; private set; }
        public static string C_SCREEN_ID_MANAGE_INVOICE { get; private set; }
        public static string C_SCREEN_ID_DOWNLOD_AUTO { get; private set; }
        public static string C_SCREEN_ID_EDIT_MONTHLY { get; private set; }
        public static string C_SCREEN_ID_MAIN { get; private set; }
        public static string C_SCREEN_ID_DOWNLOAD_DOC { get; private set; }
        public static string C_SCREEN_ID_SUSPEND_RESUME { get; private set; }
        public static string C_SCREEN_ID_RUN_BATCH { get; private set; }
        public static string C_SCREEN_ID_SEARCH_INFORMATION { get; private set; }
        public static string C_SCREEN_ID_VIEW_CUSTOMER_INFO { get; private set; }
        public static string C_SCREEN_ID_SEARCH_CUST_GROUP { get; private set; }
        public static string C_SCREEN_ID_VIEW_CUSTOMER_GROUP { get; private set; }
        public static string C_SCREEN_ID_VIEW_CONTRACT_BASIC { get; private set; }
        public static string C_SCREEN_ID_VIEW_SECURITY_BASIC { get; private set; }
        public static string C_SCREEN_ID_VIEW_SECURITY_DETAIL { get; private set; }
        public static string C_SCREEN_ID_VIEW_HISTORY_DIGEST { get; private set; }
        public static string C_SCREEN_ID_VIEW_SALE_CONTRACT { get; private set; }
        public static string C_SCREEN_ID_SEARCH_INSTRUMENT { get; private set; }
        public static string C_SCREEN_ID_VIEW_CONTRACT_DIGEST { get; private set; }
        public static string C_SCREEN_ID_PURGE_LOG { get; private set; }
        public static string C_SCREEN_ID_VIEW_SITE_INFO { get; private set; }
        public static string C_SCREEN_ID_SEARCH_PROJECT { get; private set; }
        public static string C_SCREEN_ID_VIEW_BILLING_TARGET_INFORMATION { get; private set; }
        public static string C_SCREEN_ID_FN99 { get; private set; }
        public static string C_SCREEN_ID_FQ99 { get; private set; }
        public static string C_SCREEN_ID_SEARCH_APPROVE { get; private set; }
        public static string C_SCREEN_ID_SEARCH_APPROVE_RESULT { get; private set; }
        public static string C_SCREEN_ID_CP12_CHANGE_PLAN { get; private set; }
        public static string C_SCREEN_ID_CP12_MODIFY_INSTRUMENT_QTY { get; private set; }
        public static string C_SCREEN_ID_CP12_CHANGE_FEE { get; private set; }
        public static string C_SCREEN_ID_CP12_CHANGE_EXPECTED_OPR_DATE { get; private set; }
        public static string C_SCREEN_ID_CP12_CANCEL_UNIMPLEMENTED_CONTRACT { get; private set; }
        public static string C_SCREEN_ID_CQ12_CHANGE_COMPLETE_INSTALLATION_DATE { get; private set; }
        public static string C_SCREEN_ID_CQ12_CHANGE_PLAN { get; private set; }
        public static string C_SCREEN_ID_CP05 { get; private set; }
        public static string C_SCREEN_ID_PRE_CP14 { get; private set; }
        public static string C_SCREEN_ID_CANCEL_SALE_CONTRACT { get; private set; }
        public static string C_SCREEN_ID_CP13 { get; private set; }
        public static string C_SCREEN_ID_CANCEL_RENTAL_CONTRACT { get; private set; }
        public static string C_SCREEN_ID_CP99 { get; private set; }
        public static string C_SCREEN_ID_CP16 { get; private set; }
        public static string C_SCREEN_ID_CP33 { get; private set; }
        public static string C_SCREEN_ID_GENERATE_CONTRACT_DOCUMENT { get; private set; }
        public static string C_SCREEN_ID_MAINTAIN_CONTRACT_DOCUMENT { get; private set; }
        public static string C_SCREEN_ID_CP32 { get; private set; }
        public static string C_SCREEN_ID_ISSUE_CONTRACT_DOCUMENT { get; private set; }
        public static string C_SCREEN_ID_CP34 { get; private set; }
        public static string C_SCREEN_ID_PROJ_NEW { get; private set; }
        public static string C_SCREEN_ID_PROJ_CHANGE { get; private set; }
        public static string C_SCREEN_ID_PROJ_VIEW { get; private set; }
        public static string C_SCREEN_ID_SEARCH_MAINTENANCE_CHECKUP { get; private set; }
        public static string C_SCREEN_ID_REGISTER_MAINTENANCE_CHECKUP { get; private set; }
        public static string C_SCREEN_ID_REGISTER_INCIDENT { get; private set; }
        public static string C_SCREEN_ID_SEARCH_INCIDENT { get; private set; }
        public static string C_SCREEN_ID_SUMMARY_INCIDENT { get; private set; }
        public static string C_SCREEN_ID_REGISTER_AR { get; private set; }
        public static string C_SCREEN_ID_SEARCH_AR { get; private set; }
        public static string C_SCREEN_ID_SUMMARY_AR { get; private set; }
        public static string C_SCREEN_ID_REGISTER_PAYMENT { get; private set; }
        public static string C_SCREEN_ID_IMPORT_PAYMENT_DATA { get; private set; }
        public static string C_SCREEN_ID_MANAGE_DEBT_TRACING { get; private set; }
        public static string C_SCREEN_ID_SET_UNPAID_TARGET { get; private set; }
        public static string C_SCREEN_ID_DEBT_TRACING_INFO { get; private set; }
        public static string C_SCREEN_ID_REGISTER_BILLING_EXEMPTION { get; private set; }
        public static string C_SCREEN_ID_FORCE_ISSUE_RECEIPT { get; private set; }
        public static string C_SCREEN_ID_CANCEL_RECEIPT { get; private set; }
        public static string C_SCREEN_ID_REGISTER_CREDIT_NOTE { get; private set; }
        public static string C_SCREEN_ID_MANAGE_PAYMENT_INFO { get; private set; }
        public static string C_SCREEN_ID_MATCH_PAYMENT { get; private set; }
        public static string C_SCREEN_ID_MATCH_PAYMENT_BY_INVOICE { get; private set; }
        public static string C_SCREEN_ID_CANCEL_PAYMENT_MATCHING { get; private set; }
        public static string C_SCREEN_ID_MANAGE_MONEY_COLLECTION { get; private set; }
        public static string C_SCREEN_ID_MONEY_COLLECTION_MANAGEMENT_INFO { get; private set; }
        public static string C_SCREEN_ID_MATCH_WHT { get; private set; }
        public static string C_SCREEN_ID_WHT_MANAGEMENT { get; private set; }
        public static string C_SCREEN_ID_DEBT_TRACING { get; set; }
        public static string C_SCREEN_ID_MATCH_R_REPORT { get; set; }
        public static string C_SCREEN_ID_INSTALL_REQUEST { get; private set; }
        public static string C_SCREEN_ID_INSTALL_SLIP { get; private set; }
        public static string C_SCREEN_ID_INSTALL_PO { get; private set; }
        public static string C_SCREEN_ID_INSTALL_COMPLETE { get; private set; }
        public static string C_SCREEN_ID_INSTALL_CANCEL { get; private set; }
        public static string C_SCREEN_ID_INSTALL_SEACH_MANAGE { get; private set; }
        public static string C_SCREEN_ID_INSTALL_MANAGE { get; private set; }
        public static string C_SCREEN_ID_INSTALL_REPORT { get; private set; }   
        public static string C_INV_SCREEN_ID_STOCKIN { get; private set; }
        public static string C_INV_SCREEN_ID_STOCKIN_CANCEL { get; private set; }
        public static string C_INV_SCREEN_ID_STOCKIN_ASSET { get; private set; }
        public static string C_INV_SCREEN_ID_STOCKOUT { get; private set; }
        public static string C_INV_SCREEN_ID_STOCKOUT_PARTIAL { get; private set; }
        public static string C_INV_SCREEN_ID_RECEIVE_RETURN { get; private set; }
        public static string C_INV_SCREEN_ID_PRE_ELIMINATION { get; private set; }
        public static string C_INV_SCREEN_ID_ELIMINATION { get; private set; }
        public static string C_INV_SCREEN_ID_TRANSFER_OFFICE { get; private set; }
        public static string C_INV_SCREEN_ID_TRANSFER_OFFICE_RECEIVE { get; private set; }
        public static string C_INV_SCREEN_ID_WITHIN_WH { get; private set; }
        public static string C_INV_SCREEN_ID_SPECIAL_STOCKOUT { get; private set; }
        public static string C_INV_SCREEN_ID_REPAIR_REQUEST { get; private set; }
        public static string C_INV_SCREEN_ID_REPAIR_REQUEST_RECEIVE { get; private set; }
        public static string C_INV_SCREEN_ID_REPAIR_RETURN { get; private set; }
        public static string C_INV_SCREEN_ID_REPAIR_RETURN_RECEIVE { get; private set; }
        public static string C_INV_SCREEN_ID_CHECKING_RETURN { get; private set; }
        public static string C_INV_SCREEN_ID_START_STOP_CHECKING { get; private set; }
        public static string C_INV_SCREEN_ID_CHECKING_INSTRUMENT { get; private set; }
        public static string C_INV_SCREEN_ID_COMPARE_CHECKING { get; private set; }
        public static string C_INV_SCREEN_ID_TRANSFER_BUFFER { get; private set; }
        public static string C_INV_SCREEN_ID_FIX_ADJUSTMENT { get; private set; }
        public static string C_INV_SCREEN_ID_INQUIRE_INSTRUMENT { get; private set; }
        public static string C_INV_SCREEN_ID_VIEW_INSTRUMENT { get; private set; }
        public static string C_INV_SCREEN_ID_MOVE_SHELF { get; private set; }
        public static string C_INV_SCREEN_ID_INQUIRE_IN_OUT { get; private set; }
        public static string C_INV_SCREEN_ID_INQUIRE_TRANSFER { get; private set; }
        public static string C_INV_SCREEN_ID_PICKING_LIST { get; private set; }
        public static string C_INV_SCREEN_ID_REGISTER_PURCHASE_ORDER { get; private set; }
        public static string C_INV_SCREEN_ID_MAINTAIN_PURCHASE_ORDER { get; private set; }
        public static string C_INV_SCREEN_ID_PROJECT_STOCKOUT { get; private set; }
        public static string C_INV_SCREEN_ID_IN_STOCK_REPORT { get; private set; }
        public static string C_INV_SCREEN_ID_OUT_STOCK_REPORT { get; private set; }
        public static string C_INV_SCREEN_ID_RETURN_STOCK_REPORT { get; private set; }
        public static string C_INV_SCREEN_ID_PHYSICAL_STOCK_MOVEMENT_WH { get; private set; }
        public static string C_INV_SCREEN_ID_STOCK_INPROCESS_TO_INSTALL { get; private set; }
        public static string C_INV_SCREEN_ID_PHYSICAL_STOCK_WH_MONTHLY { get; private set; }
        public static string C_INV_SCREEN_ID_STOCK_INPROCESS { get; private set; }
        public static string C_INV_SCREEN_ID_EQUIPMENT_LIST { get; private set; }
        public static string C_SCREEN_ID_MAINTAIN_CUST_INFO { get; private set; }
        public static string C_SCREEN_ID_MAINTAIN_BILLING_CLIENT_INFO { get; private set; }
        public static string C_SCREEN_ID_MAINTAIN_CUST_GROUP_INFO { get; private set; }
        public static string C_SCREEN_ID_MAINTAIN_USER_INFO { get; private set; }
        public static string C_SCREEN_ID_MAINTAIN_PERMISSION_GROUP_INFO { get; private set; }
        public static string C_SCREEN_ID_MAINTAIN_INSTRUMENT_INFO { get; private set; }
        public static string C_SCREEN_ID_MAINTAIN_INSTRUMENT_EXPANSION_INFO { get; private set; }
        public static string C_SCREEN_ID_MAINTAIN_SUBCONTRACTOR_INFO { get; private set; }
        public static string C_SCREEN_ID_MAINTAIN_SHELF_INFO { get; private set; }
        public static string C_SCREEN_ID_MAINTAIN_SAFETY_STOCK_INFO { get; private set; }
        public static string C_SCREEN_ID_SEARCH_QTN { get; private set; }
        public static string C_SCREEN_ID_QTN_TARGET { get; private set; }
        public static string C_SCREEN_ID_QTN_DETAIL { get; private set; }
        
        public static string C_SCREEN_ID_VIEW_CONTRACT_BILLING { get; private set; }
        public static string C_SCREEN_ID_REGISTER_CQ31 { get; private set; }
        public static string C_SCREEN_ID_SEARCH_NEARLY_EXPIRED_WARRANTY_SALE_CONTRACT { get; private set; }
        public static string C_SCREEN_ID_INCIDENT_LIST { get; private set; }
        public static string C_SCREEN_ID_VIEW_EDIT_INCIDENT { get; private set; }
        public static string C_SCREEN_ID_AR_LIST { get; private set; }
        public static string C_SCREEN_ID_VIEW_EDIT_AR { get; private set; }
        public static string C_SCREEN_ID_VIEW_AUTO_TRANSFER_INFORMATION { get; private set; }
        public static string C_SCREEN_ID_VIEW_BILLING_BASIC_INFORMATION { get; private set; }

        public static string C_SCREEN_ID_SEARCH_BILLING_INFORMATION { get; private set; }
        public static string C_SCREEN_ID_VIEW_CREDIT_CARD_INFORMATION { get; private set; }
        public static string C_SCREEN_ID_VIEW_BILLING_DETAIL { get; private set; }
        public static string C_SCREEN_ID_VIEW_DIPOSIT_INFORMATION { get; private set; }
        public static string C_SCREEN_ID_REPRINT_BILLING_RELATED_DOCUMENT { get; private set; }
        public static string C_SCREEN_ID_VIEW_INSTALLATION { get; private set; }
        public static string C_SCREEN_ID_EDIT_CARRY_OVER_AND_PROFIT { get; private set; }
        
        public static string C_SCREEN_ID_OTHER_ACCOUNTING_REPORT { get; private set; }
        
        

    }
}
