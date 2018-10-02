using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Master;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using SECOM_AJIS.Presentation.Contract.Models.CustomAttribute;
using System.Reflection;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// DO for initial screen
    /// </summary>
    public class CTS020_ScreenParameter : ScreenParameter
    {
        public enum SCREEN_MODE
        {
            NEW = 0,
            EDIT,
            APPROVE
        }

        [KeepSession]
        public SCREEN_MODE ScreenMode
        {
            get
            {
                int mode = 0;
                if (CommonUtil.IsNullOrEmpty(this.SubObjectID) == false)
                    mode = int.Parse(this.SubObjectID);

                return (SCREEN_MODE)Enum.ToObject(typeof(SCREEN_MODE), mode);
            }
            set
            {
                this.SubObjectID = ((int)value).ToString();
            }
        }

        [KeepSession]
        public bool FirstLoad = true;

        [KeepSession]
        public string QuotationTargetCode { get; set; }

        [KeepSession]
        public doDraftSaleContractData.PROCESS_TYPE ProcessType { get; set; }

        [KeepSession]
        public string ContractCode { get; set; }

        [KeepSession]
        public doDraftSaleContractData doDraftSaleContractData { get; set; }
        [KeepSession]
        public doDraftSaleContractData doRegisterDraftSaleContractData { get; set; }
        [KeepSession]
        public bool OneShotFlag { get; set; }

        public CTS020_BillingTargetData BillingTarget { get; set; }
        public string PurchaserSignerTypeCode { get; set; }
        public string ContactPoint { get; set; }
        public bool IsExistDraftSale { get; set; }
    }
    /// <summary>
    /// DO for billing target data
    /// </summary>
    public class CTS020_BillingTargetData : tbt_DraftSaleBillingTarget
    {
        public string BillingTargetCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertBillingTargetCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string BillingClientCodeShort
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertBillingClientCode(this.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public tbm_BillingClient BillingClient { get; set; }

        public string SalePrice_ApprovalCurrencyType { get; set; }
        public decimal? SalePrice_Approval { get; set; }
        public decimal? SalePrice_ApprovalUsd { get; set; }

        public string SalePrice_PartialCurrencyType { get; set; }
        public decimal? SalePrice_Partial { get; set; }
        public decimal? SalePrice_PartialUsd { get; set; }

        public string SalePrice_AcceptanceCurrencyType { get; set; }
        public decimal? SalePrice_Acceptance { get; set; }
        public decimal? SalePrice_AcceptanceUsd { get; set; }

        public string SalePrice_PaymentMethod_Approval { get; set; }
        public string SalePrice_PaymentMethod_Partial { get; set; }
        public string SalePrice_PaymentMethod_Acceptance { get; set; }

        public string InstallationFee_ApprovalCurrencyType { get; set; }
        public decimal? InstallationFee_Approval { get; set; }
        public decimal? InstallationFee_ApprovalUsd { get; set; }

        public string InstallationFee_PartialCurrencyType { get; set; }
        public decimal? InstallationFee_Partial { get; set; }
        public decimal? InstallationFee_PartialUsd { get; set; }

        public string InstallationFee_AcceptanceCurrencyType { get; set; }
        public decimal? InstallationFee_Acceptance { get; set; }
        public decimal? InstallationFee_AcceptanceUsd { get; set; }

        public string InstallationFee_PaymentMethod_Approval { get; set; }
        public string InstallationFee_PaymentMethod_Partial { get; set; }
        public string InstallationFee_PaymentMethod_Acceptance { get; set; }


        public decimal? TotalPrice { get; set; }
        
        public string DocLanguage { get; set; } //Add by Jutarat A. on 20122013
    }
    /// <summary>
    /// DO for getting quotation data
    /// </summary>
    public class CTS020_RetrieveQuotationCondition : doDraftSaleContractCondition
    {
        public bool IsChangeQuotationSite { get; set; }
        public bool IsAddSale { get; set; }
    }
    /// <summary>
    /// DO for validate condition in case new contract
    /// </summary>
    [MetadataType(typeof(CTS020_RetrieveQuotationCondition_New_MetaData))]
    public class CTS020_RetrieveQuotationCondition_New : CTS020_RetrieveQuotationCondition
    {
    }
    /// <summary>
    /// DO for validate condition in case edit contract
    /// </summary>
    [MetadataType(typeof(CTS020_RetrieveQuotationCondition_Edit_MetaData))]
    public class CTS020_RetrieveQuotationCondition_Edit : CTS020_RetrieveQuotationCondition
    {
    }
    /// <summary>
    /// DO for validate condition in case change alphabet
    /// </summary>
    [MetadataType(typeof(CTS020_RetrieveQuotationCondition_EditChanged_MetaData))]
    public class CTS020_RetrieveQuotationCondition_EditChanged : CTS020_RetrieveQuotationCondition
    {
    }
    /// <summary>
    /// DO for send quotation data to screen
    /// </summary>
    public class CTS020_SpecifyQuotationData
    {
        public tbt_DraftSaleContract doTbt_DraftSaleContract { get; set; }
        public doCustomer doPurchaserCustomer { get; set; }
        public doCustomer doRealCustomer { get; set; }
        public doSite doSite { get; set; }
        public int ProcessType { get; set; } //Add by Jutarat A. on 16102012

    }
    //public class CTS020_FeeInformation
    //{
    //    public string Normal { get; set; }
    //    public string Order { get; set; }
    //    public string Approve { get; set; }
    //    public string Partial { get; set; }
    //    public string Acceptance { get; set; }
    //}
    /// <summary>
    /// DO for getting customer data
    /// </summary>
    public class CTS020_RetrieveCustomerCondition
    {
        [CTS020_CustConditionNotNullOrEmptyAttribute(ControlName = "CustCode")]
        public string CustCode { get; set; }
        public int CustType { get; set; }
    }
    /// <summary>
    /// DO for getting site data
    /// </summary>
    public class CTS020_RetrieveSiteCondition
    {
        [NotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0069, ControlName = "RCSearchCustCode")]
        public string CustCode { get; set; }
        [NotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0072, ControlName = "SiteCustCode")]
        public string SiteCustCode { get; set; }
        [NotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0070, ControlName = "SiteCustCodeNo")]
        public string SiteNo { get; set; }
    }
    /// <summary>
    /// DO for getting E-mail data
    /// </summary>
    public class CTS020_AddEmailAddressCondition
    {
        public bool FromPopUp { get; set; }
        public List<tbt_DraftSaleEmail> NewEmailAddressList { get; set; }
    }
    /// <summary>
    /// DO for getting billing target data
    /// </summary>
    [MetadataType(typeof(CTS020_RetrieveBillingTargetCondition_MetaData))]
    public class CTS020_RetrieveBillingTargetCondition : tbt_DraftSaleBillingTarget
    {
    }
    /// <summary>
    /// DO for getting billing client data
    /// </summary>
    [MetadataType(typeof(CTS020_RetrieveBillingClientCondition_MetaData))]
    public class CTS020_RetrieveBillingClientCondition : tbt_DraftSaleBillingTarget
    {
    }
    /// <summary>
    /// DO for update billing client is case of copy data
    /// </summary>
    public class CTS020_CopyBillingNameAddressCondition
    {
        public enum COPY_BILLING_NAME_ADDRESS_MODE
        {
            CONTRACT_TARGET = 1,
            BRANCH_CONTRACT_TARGET,
            REAL_CUSTOMERE,
            SITE
        }

        public int CopyModeID { get; set; }
        public COPY_BILLING_NAME_ADDRESS_MODE CopyMode
        {
            get
            {
                return (COPY_BILLING_NAME_ADDRESS_MODE)Enum.ToObject(typeof(COPY_BILLING_NAME_ADDRESS_MODE), this.CopyModeID);
            }
            set
            {
                this.CopyModeID = (int)value;
            }
        }

        public string BranchNameEN { get; set; }
        public string BranchAddressEN { get; set; }
        public string BranchNameLC { get; set; }
        public string BranchAddressLC { get; set; }
    }
    /// <summary>
    /// DO for validate register sale contract data
    /// </summary>
    [MetadataType(typeof(CTS020_RegisterSaleContractData_MetaData))]
    public class CTS020_RegisterSaleContractData : tbt_DraftSaleContract
    {
        public string QuotationTargetCodeLong
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertQuotationTargetCode(this.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            }
        }

        public bool IsConnectTargetCode { get; set; }
        public bool IsBranchChecked { get; set; }
    }
    /// <summary>
    /// DO for validate employee data
    /// </summary>
    [MetadataType(typeof(CTS020_RegisterSaleContractData_Employee_MetaData))]
    public class CTS020_RegisterSaleContractData_Employee : tbt_DraftSaleContract
    {
    }
    /// <summary>
    /// DO for validate billing target data
    /// </summary>
    [MetadataType(typeof(CTS020_UpdateBillingTargetData_MetaData))]
    public class CTS020_UpdateBillingTargetData : CTS020_BillingTargetData
    {

    }
    /// <summary>
    /// DO for validate billing target data
    /// </summary>
    [MetadataType(typeof(CTS020_UpdateBillingTargetData2_MetaData))]
    public class CTS020_UpdateBillingTargetData2 : CTS020_BillingTargetData
    {
        public string ApproveNo1 { get; set; }
    }
    /// <summary>
    /// DO for validate billing client data
    /// </summary>
    [MetadataType(typeof(CTS020_UpdateBillingClient_MetaData))]
    public class CTS020_UpdateBillingClient : tbm_BillingClient
    {
    }

    /// <summary>
    /// DO for validate billing client data (BranchName)
    /// </summary>
    [MetadataType(typeof(CTS020_UpdateBillingClientBranchName_MetaData))]
    public class CTS020_UpdateBillingClientBranchName : tbm_BillingClient //Add by Jutarat A. on 02012014
    {
    }

    /// <summary>
    /// DO for send register result to screen
    /// </summary>
    public class CTS020_RegisterResult
    {
        public string PurchaserCustCode { get; set; }
        public string PurchaserStatusCodeName { get; set; }
        public string PurchaserIDNo { get; set; }
        public string RealCustomerCustCode { get; set; }
        public string RealCustomerStatusCodeName { get; set; }
        public string RealCustomerIDNo { get; set; }
        public string SiteCode { get; set; }

        public string BillingTargetCode { get; set; }
        public string BillingClientCode { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Contract.Models.CustomAttribute
{
    /// <summary>
    /// Attribute for check customer is null or empty
    /// </summary>
    public class CTS020_CustConditionNotNullOrEmptyAttribute : NotNullOrEmptyAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value))
            {
                CTS020_RetrieveCustomerCondition cond = validationContext.ObjectInstance as CTS020_RetrieveCustomerCondition;
                if (cond != null)
                {
                    if (cond.CustType == 1)
                        this.MessageCode = MessageUtil.MessageList.MSG0067;
                    else
                        this.MessageCode = MessageUtil.MessageList.MSG0069;
                }

                return base.IsValid(value, validationContext);
            }

            return null;
        }
    }
    
    /// <summary>
    /// Attribute for check fee is follow by payment method
    /// </summary>
    public class CTS020_FeeRelateWithPaymentMethodAttribute : AValidatorAttribute
    {
        private string PaymentMethodField { get; set; }

        public CTS020_FeeRelateWithPaymentMethodAttribute(string PaymentMethodField)
        {
            this.PaymentMethodField = PaymentMethodField;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            tbt_DraftSaleBillingTarget obj = validationContext.ObjectInstance as tbt_DraftSaleBillingTarget;
            if (obj != null && value != null && value is decimal?)
            {
                PropertyInfo prop = obj.GetType().GetProperty(this.PaymentMethodField);
                decimal? num = (decimal?)value;
                if (prop != null && num > 0)
                {
                    if (CommonUtil.IsNullOrEmpty(prop.GetValue(obj, null)) == true)
                        return base.IsValid(value, validationContext);
                }
            }

            return null;
        }
    }
    /// <summary>
    /// Attribute for check approve payment method
    /// </summary>
    public class CTS020_ApprovePaymentMethodAttribute : AValidatorAttribute
    {
        public CTS020_ApprovePaymentMethodAttribute()
        {
            this.Module = MessageUtil.MODULE_CONTRACT;
            this.MessageCode = MessageUtil.MessageList.MSG3064;
            this.ControlName = "ApproveNo1";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            CTS020_UpdateBillingTargetData2 obj = validationContext.ObjectInstance as CTS020_UpdateBillingTargetData2;
            if (obj != null && CommonUtil.IsNullOrEmpty(value) == false)
            {
                if (value != SECOM_AJIS.Common.Util.ConstantValue.PaymentMethod.C_PAYMENT_METHOD_BANK_TRANSFER
                        && value != SECOM_AJIS.Common.Util.ConstantValue.PaymentMethod.C_PAYMENT_METHOD_AUTO_TRANSFER
                    && CommonUtil.IsNullOrEmpty(obj.ApproveNo1))
                {
                    return base.IsValid(value, validationContext);
                }

            }

            return null;
        }
    }
    /// <summary>
    /// Attribute for check branch is null or empty
    /// </summary>
    public class CTS020_BranchNotNullOrEmptyAttribute : NotNullOrEmptyAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value))
            {
                CTS020_RegisterSaleContractData cond = validationContext.ObjectInstance as CTS020_RegisterSaleContractData;
                if (cond != null)
                {
                    if (cond.IsBranchChecked)
                        return base.IsValid(value, validationContext);
                }
            }

            return null;
        }
    }
}
namespace SECOM_AJIS.Presentation.Contract.Models.MetaData
{
    /// <summary>
    /// Metadata for CTS020_RetrieveQuotationCondition_New DO
    /// </summary>
    public class CTS020_RetrieveQuotationCondition_New_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Parameter = "lblQuotationTargetCode",
                        ControlName = "QuotationTargetCode")]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Parameter = "lblAlphabet",
                        ControlName = "Alphabet")]
        public string Alphabet { get; set; }
    }
    /// <summary>
    /// Metadata for CTS020_RetrieveQuotationCondition_Edit DO
    /// </summary>
    public class CTS020_RetrieveQuotationCondition_Edit_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Parameter = "lblQuotationTargetCode",
                        ControlName = "QuotationTargetCode")]
        public string QuotationTargetCode { get; set; }
    }
    /// <summary>
    /// Metadata for CTS020_RetrieveQuotationCondition_EditChanged DO
    /// </summary>
    public class CTS020_RetrieveQuotationCondition_EditChanged_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Parameter = "lblQuotationTargetCode",
                        ControlName = "Alphabet")]
        public string Alphabet { get; set; }
    }
    /// <summary>
    /// Metadata for CTS020_RetrieveBillingTargetCondition DO
    /// </summary>
    public class CTS020_RetrieveBillingTargetCondition_MetaData
    {
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS020",
            Parameter = "lblBillingTargetCode", 
            ControlName = "BillingTargetCode")]
        public string BillingTargetCode { get; set; }
    }
    /// <summary>
    /// Metadata for CTS020_RetrieveBillingClientCondition DO
    /// </summary>
    public class CTS020_RetrieveBillingClientCondition_MetaData
    {
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS020",
            Parameter = "lblBillingClientCode", 
            ControlName = "BillingClientCode")]
        public string BillingClientCode { get; set; }
    }
    /// <summary>
    /// Metadata for CTS020_RegisterSaleContractData DO
    /// </summary>
    public class CTS020_RegisterSaleContractData_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Order = 1,
                        Parameter = "lblQuotationTargetCode")]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Order = 2,
                        Parameter = "lblQuotationTargetCode")]
        public string Alphabet { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Order = 3,
                        Parameter = "lblSaleType",
                        ControlName = "SaleType")]
        public string SaleType { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Order = 4,
                        Parameter = "lblExpectedCompleteInstallationDate",
                        ControlName = "ExpectedInstallCompleteDate")]
        public Nullable<System.DateTime> ExpectedInstallCompleteDate { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Order = 5,
                        Parameter = "lblExpectedCustomerAcceptanceDate",
                        ControlName = "ExpectedAcceptanceAgreeDate")]
        public Nullable<System.DateTime> ExpectedAcceptanceAgreeDate { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS020",
        //                Order = 6,
        //                Parameter = "lblOrderProductPrice",
        //                ControlName = "OrderProductPrice")]
        //public Nullable<decimal> OrderProductPrice { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS020",
        //                Order = 7,
        //                Parameter = "lblOrderInstallationFee",
        //                ControlName = "OrderInstallFee")]
        //public Nullable<decimal> OrderInstallFee { get; set; }
        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS020",
        //                Order = 8,
        //                Parameter = "lblOrderSalePrice",
        //                ControlName = "OrderSalePrice")]
        //public Nullable<decimal> OrderSalePrice { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Order = 9,
                        Parameter = "lblContractOffice",
                        ControlName = "ContractOfficeCode")]
        public string ContractOfficeCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Order = 10,
                        Parameter = "lblOperationOffice",
                        ControlName = "OperationOfficeCode")]
        public string OperationOfficeCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Order = 11,
                        Parameter = "lblSalesman1",
                        ControlName = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }

        [NotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Parameter = "lblContractSignerType",
                        ControlName = "CPPurchaserSignerTypeCode",
                        Order = 15)]
        public string PurchaserSignerTypeCode { get; set; }
        [CTS020_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Parameter = "lblBranchNameEN",
                        ControlName = "BranchNameEN",
                        Order = 16)]
        public string BranchNameEN { get; set; }
        [CTS020_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Parameter = "lblBranchAddressEN",
                        ControlName = "BranchAddressEN",
                        Order = 17)]
        public string BranchAddressEN { get; set; }
        // 2017.03.09 delete matsuda start
        //[CTS020_BranchNotNullOrEmpty(
        //                Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS020",
        //                Parameter = "lblBranchNameLC",
        //                ControlName = "BranchNameLC",
        //                Order = 18)]
        //public string BranchNameLC { get; set; }
        //[CTS020_BranchNotNullOrEmpty(
        //                Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS020",
        //                Parameter = "lblBranchAddressLC",
        //                ControlName = "BranchAddressLC",
        //                Order = 19)]
        //public string BranchAddressLC { get; set; }
        // 2017.03.09 delete matsuda end
    }
    /// <summary>
    /// Metadata for CTS020_RegisterSaleContractData_Employee DO
    /// </summary>
    public class CTS020_RegisterSaleContractData_Employee_MetaData
    {
        [EmployeeExist(Control = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo2")]
        public string SalesmanEmpNo2 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo3")]
        public string SalesmanEmpNo3 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo4")]
        public string SalesmanEmpNo4 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo5")]
        public string SalesmanEmpNo5 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo6")]
        public string SalesmanEmpNo6 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo7")]
        public string SalesmanEmpNo7 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo8")]
        public string SalesmanEmpNo8 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo9")]
        public string SalesmanEmpNo9 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo10")]
        public string SalesmanEmpNo10 { get; set; }
    }
    /// <summary>
    /// Metadata for CTS020_UpdateBillingTargetData DO
    /// </summary>
    public class CTS020_UpdateBillingTargetData_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS020",
                        Parameter = "lblBillingOffice",
                        ControlName = "BillingOfficeCode",
                        Order = 23)]
        public string BillingOfficeCode { get; set; }
    }
    /// <summary>
    /// Metadata for CTS020_UpdateBillingTargetData2 DO
    /// </summary>
    public class CTS020_UpdateBillingTargetData2_MetaData
    {
        [CTS020_FeeRelateWithPaymentMethod("PaymentMethod_Approval",
            Module = MessageUtil.MODULE_CONTRACT,
            MessageCode = MessageUtil.MessageList.MSG3210,
            ControlName = "PaymentMethod_Approval")]
        public decimal? SalePrice_Approval { get; set; }
        [CTS020_FeeRelateWithPaymentMethod("PaymentMethod_Partial",
            Module = MessageUtil.MODULE_CONTRACT,
            MessageCode = MessageUtil.MessageList.MSG3211,
            ControlName = "PaymentMethod_Partial")]
        public decimal? SalePrice_Partial { get; set; }
        [CTS020_FeeRelateWithPaymentMethod("PaymentMethod_Acceptance",
            Module = MessageUtil.MODULE_CONTRACT,
            MessageCode = MessageUtil.MessageList.MSG3212,
            ControlName = "PaymentMethod_Acceptance")]
        public decimal? SalePrice_Acceptance { get; set; }
    }
    /// <summary>
    /// Metadata for CTS020_UpdateBillingClient DO
    /// </summary>
    public class CTS020_UpdateBillingClient_MetaData
    {
        [CodeNullOtherNotNull("BillingClientCode", MessageCode = MessageUtil.MessageList.MSG0134)]
        public string CustTypeCode { get; set; }
        [CodeNullOtherNotNull("BillingClientCode", MessageCode = MessageUtil.MessageList.MSG0134)]
        public string NameEN { get; set; }
        //[CodeNullOtherNotNull("BillingClientCode", MessageCode = MessageUtil.MessageList.MSG0134)]
        //public string NameLC { get; set; }
        [CodeNullOtherNotNull("BillingClientCode", MessageCode = MessageUtil.MessageList.MSG0134)]
        public string AddressEN { get; set; }
        //[CodeNullOtherNotNull("BillingClientCode", MessageCode = MessageUtil.MessageList.MSG0134)]
        //public string AddressLC { get; set; }

        //Comment by Jutarat A. on 25122013
        /*//Add by Jutarat A. on 17122013
        [CodeNullOtherNotNull("BillingClientCode", MessageCode = MessageUtil.MessageList.MSG0134)]
        public string BranchNameEN { get; set; }
        [CodeNullOtherNotNull("BillingClientCode", MessageCode = MessageUtil.MessageList.MSG0134)]
        public string BranchNameLC { get; set; }
        //End Add*/
        //End Comment
    }

    /// <summary>
    /// Metadata for CTS020_UpdateBillingClientBranchName DO
    /// </summary>
    public class CTS020_UpdateBillingClientBranchName_MetaData //Add by Jutarat A. on 02012014
    {
        [CodeNullOtherNotNull("BillingClientCode", MessageCode = MessageUtil.MessageList.MSG0134)]
        public string BranchNameEN { get; set; }
        [CodeNullOtherNotNull("BillingClientCode", MessageCode = MessageUtil.MessageList.MSG0134)]
        public string BranchNameLC { get; set; }
        [CodeNullOtherNotNull("BillingClientCode", MessageCode = MessageUtil.MessageList.MSG0134)]
        public string IDNo { get; set; }
    }
}
