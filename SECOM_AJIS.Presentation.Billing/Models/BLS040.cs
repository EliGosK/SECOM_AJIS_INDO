using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Common.MetaData;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Billing;

using SECOM_AJIS.Presentation.Billing.Models;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;

namespace SECOM_AJIS.Presentation.Billing.Models
{    
    /// <summary>
    /// Validation class of screen BLS040
    /// </summary>
    public class BLS040_ValidateData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "BLS040",
                        Parameter = "lblBillingCode",
                        ControlName = "ContractCodeProjectCode")]
        public string ContractProjectCodeShort { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "BLS040",
                        Parameter = "lblBillingCode",
                        ControlName = "BillingOCC")]
        public string BillingOCC { get; set; }
        
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "BLS040",
                        Parameter = "lblBillingTargetCode",
                        ControlName = "BillingTargetNo")]
        public string BillingTargetNo { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "BLS040",
                        Parameter = "lblBillingTargetCode",
                        ControlName = "BillingClientCode")]
        public string BillingClientCode { get; set; }
    } 

    /// <summary>
    /// Screen parameter for BLS040
    /// </summary>
    public class BLS040_ScreenParameter : ScreenParameter
    {
        public string ContractCode { get; set; }
        public string ContractProjectCodeShort { get; set; }
        [KeepSession]
        public string ContractProjectCodeLong { get; set; }
        [KeepSession]
        public string BillingClientCode { get; set; }
        [KeepSession]
        public string BillingClientCodeShort { get; set; }
        [KeepSession]
        public string BillingTargetRunningNo { get; set; }
        public string BillingOCC { get; set; }
        public string ServiceTypeCode { get; set; }
        public string ProductTypeCode { get; set; }

        public doTbt_BillingBasic doBillingBasic { get; set; }
        public dtTbt_BillingTargetForView doBillingTarget { get; set; }
        //public dtTbt_BillingTargetForView doBillingTargetNew { get; set; }
        public List<doTbt_MonthlyBillingHistoryList> doTbt_MonthlyBillingHistoryList { get; set; }
        public tbt_AutoTransferBankAccount doTbt_AutoTransferBankAccount { get; set; }
        public dtAutoTransferBankAccountForView dtAutoTransferForView { get; set; }

        public tbt_CreditCard doTbt_CreditCard { get; set; }
        public dtCreditCardForView dtCreditCardForView { get; set; }

        public List<doBillingTypeDetailList> doBillingTypeDetailListPrevious { get; set; }
        public List<doBillingTypeDetailList> doBillingTypeDetailList { get; set; }
        public BLS040_doAutoTransferBankAccount doAutoTransferBankAccount { get; set; }
        public BLS040_doCreditCard doCreditCard { get; set; }

        public string PaymentMethod { get; set; }
        public string AccountName { get; set; }
        public string CardName { get; set; }       
        public string SortingType { get; set; }
        public string VATUnchargedFlag { get; set; }
        public string strBillingServiceTypeCode { get; set; }

        public bool EnableCarefulSpecial { get; set; }
        public bool EnableAutoTransfer { get; set; }
        public bool EnableCreditCard { get; set; }
        public bool IsQCodeContract { get; set; }    
    }

    /// <summary>
    /// Data object of Changing data list
    /// </summary>
    public class BLS040_ChangingDateList
    {
        public DateTime? BillingStartDate0 { get; set; }
        public DateTime? BillingStartDate1 { get; set; }
        public DateTime? BillingStartDate2 { get; set; }
        public DateTime? BillingStartDate3 { get; set; }
        public DateTime? BillingStartDate4 { get; set; }
        public DateTime? BillingStartDate5 { get; set; }
    }

    //Add by Jutarat A. on 25042013
    /// <summary>
    /// Data object of Monthly Billing Amount list
    /// </summary>
    public class BLS040_MonthlyBillingAmountList
    {
        public decimal? MonthlyBillingAmount0 { get; set; }
        public decimal? MonthlyBillingAmount1 { get; set; }
        public decimal? MonthlyBillingAmount2 { get; set; }
        public decimal? MonthlyBillingAmount3 { get; set; }
        public decimal? MonthlyBillingAmount4 { get; set; }
        public decimal? MonthlyBillingAmount5 { get; set; }

        public string MonthlyBillingAmount0MulC { get; set; }
        public string MonthlyBillingAmount1MulC { get; set; }
        public string MonthlyBillingAmount2MulC { get; set; }
        public string MonthlyBillingAmount3MulC { get; set; }
        public string MonthlyBillingAmount4MulC { get; set; }
        public string MonthlyBillingAmount5MulC { get; set; }
    }
    //End Add

    //public class BLS040_BillingType
    //{
    //    public string BillingTypeCode { get; set; }
    //    public string BillingTypeNameEN { get; set; }
    //    public string BillingTypeNameLC { get; set; }
    //    public string BillingTypeGroup { get; set; }
    //}


    /// <summary>
    /// Data object of Auto transfer bank account
    /// </summary>
    public class BLS040_doAutoTransferBankAccount 
    {        
        public string BankCode { get; set; }
        public string BankBranchCode { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string AutoTransferDate { get; set; }
    }

    /// <summary>
    /// Data object of Credit card
    /// </summary>
    public class BLS040_doCreditCard
    { 
        public string CreditCardCompanyCode { get; set; }
        public string CreditCardType { get; set; }
        public string CreditCardNo { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }

    }


}

