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
    /// Validation class of screen BLS030
    /// </summary>
    public class BLS030_ValidateData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "BLS030",
                        Parameter = "lblContractProjectCode",
                        ControlName = "ContractCodeProjectCode")]
        public string ContractProjectCodeShort { get; set; }
        
        public string ContractProjectCodeLong { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_INSTALLATION,
                        Screen = "BLS030",
                        Parameter = "lblBillingTargetCode",
                        ControlName = "BillingTargetRunningNo")]
        public string BillingClientCode { get; set; }
    } 

    /// <summary>
    /// Screen parameter for BLS030
    /// </summary>
    public class BLS030_ScreenParameter : ScreenParameter
    {       
        [KeepSession]
        public string ContractProjectCodeShort { get; set; }
        [KeepSession]
        public string ContractProjectCodeLong { get; set; }
      
        public string BillingClientCode { get; set; }
       
        public string BillingTargetRunningNo { get; set; }
        public string BillingTargetCode { get; set; }
        public string ServiceTypeCode { get; set; }
        public string ProductTypeCode { get; set; }
        public dtTbt_BillingTargetForView doBillingTarget { get; set; }
        public List<tbt_BillingTypeDetail> doBillingTypeList { get; set; }
        public BLS030_doAutoTransferBankAccount doAutoTransferBankAccount { get; set; }
        public BLS030_doCreditCard doCreditCard { get; set; }
        public string PaymentMethod { get; set; }
        public string AccountName { get; set; }
        public string CardName { get; set; }       
        public string SortingType { get; set; }
        public string VATUnchargedFlag { get; set; }
        public string strBillingServiceTypeCode { get; set; }
        
    }


    //public class BLS030_BillingType
    //{
    //    public string BillingTypeCode { get; set; }
    //    public string BillingTypeNameEN { get; set; }
    //    public string BillingTypeNameLC { get; set; }
    //    public string BillingTypeGroup { get; set; }
    //}

    /// <summary>
    /// Data boject of Auto transfer bank account
    /// </summary>
    public class BLS030_doAutoTransferBankAccount 
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
    public class BLS030_doCreditCard
    { 
        public string CreditCardCompanyCode { get; set; }
        public string CreditCardType { get; set; }
        public string CreditCardNo { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }

        // CardName
        public string CardName { get; set; }

    }


}

