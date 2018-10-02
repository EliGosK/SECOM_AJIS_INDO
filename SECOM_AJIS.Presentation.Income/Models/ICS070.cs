using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Billing;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Presentation.Income.Models.MetaData;
using System.IO;
namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Parameter of ICS070 screen
    /// </summary>
    public class ICS070_ScreenParameter : ScreenParameter
    {
        // send data back to client
        [KeepSession]
        public doGetCreditNote doGetCreditNote { set; get; }

        public doGetTaxInvoiceForIC doGetTaxInvoiceForIC { set; get; }
        public string strBillingCode { set; get; }
        public List<doBillingDetail> doBillingDetailList { set; get; }
        public decimal? decBalanceDeposit { set; get; }

        public string decBalanceDepositCurrencyType { get; set; }

        public doGetBillingCodeInfo doGetBillingCodeInfo { set; get; }
        public doGetBillingDetailOfInvoice doGetBillingDetailOfInvoice { set; get; }



        // send data back to client add 1,2,3 case
        public string strBillingTargetCode { set; get; }
        public string strInvoiceNo { set; get; }
        public string strInvoiceOCC { set; get; }
        public bool bolNotRetrieveFlag { set; get; }
        public string strCreditNoteType { set; get; }
        public string strRegContent { set; get; }

        public List<string> WarningMessage { set; get; }

        // send data from client to server
        public ICS070_RegisterData RegisterData { set; get; }
    }

    // register com sent data to server
    /// <summary>
    /// DO of Register Header 
    /// </summary>
    public class ICS070_HeaderRegisterData
    {
        public string rdoProcessType { set; get; }
        public string txtCancelCreditNoteNo { set; get; }

        public string txtExceptDepositFeeTaxInvoiceNoForCreditNote { set; get; }
        public string txtDepositFeeTaxInvoiceNoForCreditNote { set; get; }

        public string txtDepositFeeBillingCode { set; get; }
        public string txtExceptDepositFeeBillingCode { set; get; }
        public string txtRevenueBillingCode { set; get; }

    }
    /// <summary>
    /// DO of Register Input (1) 
    /// </summary>
    public class ICS070_InputRegisterData1
    {

        public bool chkExceptDepositFeeNotRetrieve { set; get; }

        public DateTime? dtpExceptDepositFeeTaxInvoiceDate { set; get; }
        public DateTime? dtpExceptDepositFeeCreditNoteDate { set; get; }

        public string strExceptDepositFeeTaxInvoiceBillingType { set; get; }

        public string strExceptDepositFeeTaxInvoiceNoForCreditNote { set; get; }
        public string strExceptDepositFeeBillingCode { set; get; }
        public string strExceptDepositFeeTaxInvoiceAmountIncludeVat { set; get; }
        public string strExceptDepositFeeTaxInvoiceAmountIncludeVatCurrencyType { get; set; } // add by Jirawat Jannet @ 2016-10-19
        public string strExceptDepositFeeTaxInvoiceAmount { set; get; }
        public string strExceptDepositFeeTaxInvoiceAmountCurrencyType { get; set; } // add by Jirawat Jannet @ 2016-10-19
        public string strExceptDepositFeeAccumulatedPaymentAmount { set; get; }
        public string strExceptDepositFeeAccumulatedPaymentAmountCurrencyType { set; get; } // add by Jirawat Jannet @ 2016-10-19
        public string strExceptDepositFeeCreditNoteAmountIncludeVat { set; get; }
        public string strExceptDepositFeeCreditNoteAmountIncludeVatCurrencyType { set; get; } // add by Jirawat Jannet @ 2016-10-19
        public string strExceptDepositFeeCreditNoteAmount { set; get; }
        public string strExceptDepositFeeCreditNoteAmountCurrencyType { set; get; } // add by Jirawat Jannet @ 2016-10-19 
        public string strExceptDepositFeeApproveNo { set; get; }
        public string straExceptDepositFeeIssueReason { set; get; }

    }

    /// <summary>
    /// DO of Register Input (2) 
    /// </summary>
    public class ICS070_InputRegisterData2
    {

        public bool chkDepositFeeNotRetrieve { set; get; }
 
        public DateTime? dtpDepositFeeTaxInvoiceDate { set; get; }
        public DateTime? dtpDepositFeeCreditNoteDate { set; get; }
 
        public string strDepositFeeTaxInvoiceBillingType { set; get; }
 
        public string strDepositFeeTaxInvoiceNoForCreditNote { set; get; }
        public string strDepositFeeBillingCode { set; get; }
        public string strDepositFeeBalanceOfDepositFee { set; get; }
        public string strDepositFeeBalanceOfDepositFeeCurrencyType { set; get; } // add by Jirawat Jannet @ 2016-10-19 
        public string strDepositFeeTaxInvoiceAmountIncludeVat { set; get; }
        public string strDepositFeeTaxInvoiceAmountIncludeVatCurrencyType { set; get; } // add by Jirawat Jannet @ 2016-10-19 
        public string strDepositFeeTaxInvoiceAmount { set; get; }
        public string strDepositFeeTaxInvoiceAmountCurrencyType { set; get; } // add by Jirawat Jannet @ 2016-10-19 
        public string strDepositFeeAccumulatedPaymentAmount { set; get; }
        public string strDepositFeeAccumulatedPaymentAmountCurrencyType { set; get; } // add by Jirawat Jannet @ 2016-10-19 
        public string strDepositFeeCreditNoteAmountIncludeVat { set; get; }
        public string strDepositFeeCreditNoteAmountIncludeVatCurrencyType { set; get; } // add by Jirawat Jannet @ 2016-10-19 
        public string strDepositFeeCreditNoteAmount { set; get; }
        public string strDepositFeeCreditNoteAmountCurrencyType { set; get; } // add by Jirawat Jannet @ 2016-10-19 
        public string strDepositFeeApproveNo { set; get; }
        public string straDepositFeeIssueReason { set; get; }

    }

    /// <summary>
    /// DO of Register Input (3) 
    /// </summary>
    public class ICS070_InputRegisterData3
    {

        public string strRevenueBillingCode { set; get; }
        public string strRevenueBalanceOfDepositFee { set; get; }
        public string strRevenueBalanceOfDepositFeeCurrencyType { set; get; } // add by Jirawat Jannet @ 2016-10-19 
        public string strRevenueRevenueDepositFee { set; get; }
        public string strRevenueRevenueDepositFeeCurrencyType { set; get; } // add by Jirawat Jannet @ 2016-10-19 
        public string strRevenueRevenueVatAmount { set; get; }
        public string strRevenueRevenueVatAmountCurrencyType { set; get; } // add by Jirawat Jannet @ 2016-10-19 

    }

    /// <summary>
    /// DO of Register Detail (Section1)
    /// </summary>
    public class ICS070_DetailRegisterDataSection1
    {
        public string strRegistrationContents { set; get; }
        public string strAmountIncludingVat { set; get; }
        public string strAmountIncludingVatCurrencyType { get; set; }
        public string strAmountIncludingVatCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.strAmountIncludingVatCurrencyType);
            }
        }
        public string strVatAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.strVatAmountCurrencyType);
            }
        }
        public string strVatAmount { set; get; }
        public string strVatAmountCurrencyType { get; set; }
        public DateTime? dtpCreditNoteDate { set; get; }
        public string strBillingCode { set; get; }
        public string strCreditNoteNo { set; get; }
        public string strBillingClientName { set; get; }
        //------------------------------------------
        public string strCreditNoteType { set; get; }
        public string strTaxInvoiceNo { set; get; }
        public string strBillingTargetCode { set; get; }
        public string strCreditAmountIncVAT { set; get; }
        public string strCreditVATAmount { set; get; }
        public string strBillingTypeCode { set; get; }
        public string strApproveNo { set; get; }
        public string strIssueReason { set; get; }
        public string strTaxInvoiceAmount { set; get; }
        public string strTaxInvoiceVATAmount { set; get; }
        public DateTime? dtpTaxInvoiceDate { set; get; }
        public string strPaymentTransNo { set; get; }
        public string strCancelFlag { set; get; }
        public string strRevenueNo { set; get; }
        public DateTime? dtpIssueDate { set; get; }
        public string strRevenueAmountIncVAT { set; get; }
        public string strRevenueAmountIncVATCurrencyType { set; get; }
        public string strRevenueVATAmount { set; get; }
        public string strRevenueVATAmountCurrencyType { set; get; }
        public string strInvoiceNo { set; get; }
        public string strInvoiceOCC { set; get; }
        public string strNotRetrieveFlag { set; get; }
        public string strBillingClientNameEN { set; get; }
        public string rowid { set; get; }

        public string FilePath { set; get; } //Add by Jutarat A. on 02102013
    }

    /// <summary>
    /// DO for Register Data
    /// </summary>
    public class ICS070_RegisterData
    {
        public ICS070_HeaderRegisterData Header { set; get; }
        public ICS070_InputRegisterData1 Input1 { set; get; }
        public ICS070_InputRegisterData2 Input2 { set; get; }
        public ICS070_InputRegisterData3 Input3 { set; get; }
        public List<ICS070_DetailRegisterDataSection1> Detail1 { set; get; }
        
    }

}


