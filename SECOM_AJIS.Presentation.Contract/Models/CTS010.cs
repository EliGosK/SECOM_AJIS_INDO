using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Quotation;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Contract;
using SECOM_AJIS.Presentation.Contract.Models.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Presentation.Contract.Models.MetaData;
using System.Reflection;

namespace SECOM_AJIS.Presentation.Contract.Models
{
    /// <summary>
    /// DO for initial screen
    /// </summary>
    public class CTS010_ScreenParameter : ScreenParameter
    {
        public enum SCREEN_MODE
        {
            NEW = 0,
            EDIT,
            APPROVE
        }

        [KeepSession]
        public bool FirstLoad = true;

        [KeepSession]
        public string QuotationTargetCode { get; set; }

        [KeepSession]
        public string ContractCode { get; set; }

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
        public doDraftRentalContractData doDraftRentalContractData { get; set; }
        [KeepSession]
        public doDraftRentalContractData doRegisterDraftRentalContractData { get; set; }

        public List<CTS010_BillingTargetData> BillingTargetList { get; set; }
        public CTS010_BillingTargetData TempBillingTargetData { get; set; }
    }
    /// <summary>
    /// DO for getting quotation data
    /// </summary>
    public class CTS010_RetrieveQuotationCondition : doDraftRentalContractCondition
    {
        public bool IsChangeQuotationSite { get; set; }
    }
    /// <summary>
    /// DO for validate condition in case new contract
    /// </summary>
    [MetadataType(typeof(CTS010_RetrieveQuotationCondition_New_MetaData))]
    public class CTS010_RetrieveQuotationCondition_New : CTS010_RetrieveQuotationCondition
    {
    }
    /// <summary>
    /// DO for validate condition in case edit contract
    /// </summary>
    [MetadataType(typeof(CTS010_RetrieveQuotationCondition_Edit_MetaData))]
    public class CTS010_RetrieveQuotationCondition_Edit : CTS010_RetrieveQuotationCondition
    {
    }
    /// <summary>
    /// DO for validate condition in case change alphabet
    /// </summary>
    [MetadataType(typeof(CTS010_RetrieveQuotationCondition_EditChanged_MetaData))]
    public class CTS010_RetrieveQuotationCondition_EditChanged : CTS010_RetrieveQuotationCondition
    {
    }
    /// <summary>
    /// DO for send quotation data to screen
    /// </summary>
    public class CTS010_SpecifyQuotationData
    {
        public tbt_DraftRentalContract doTbt_DraftRentalContrat { get; set; }
        public doCustomer doContractCustomer { get; set; }
        public doCustomer doRealCustomer { get; set; }
        public doSite doSite { get; set; }

        public bool IsProductTypeAL
        {
            get
            {
                if (doTbt_DraftRentalContrat != null)
                {
                    return doTbt_DraftRentalContrat.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_AL;
                }
                return false;
            }
        }
        public bool IsProductTypeRentalSale
        {
            get
            {
                if (doTbt_DraftRentalContrat != null)
                {
                    return doTbt_DraftRentalContrat.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_RENTAL_SALE;
                }
                return false;
            }
        }
        public bool IsProductSaleOnline
        {
            get
            {
                if (doTbt_DraftRentalContrat != null)
                {
                    return doTbt_DraftRentalContrat.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_ONLINE;
                }
                return false;
            }
        }
        public bool IsProductBeatGuard
        {
            get
            {
                if (doTbt_DraftRentalContrat != null)
                {
                    return doTbt_DraftRentalContrat.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_BE;
                }
                return false;
            }
        }
        public bool IsProductSentryGuard
        {
            get
            {
                if (doTbt_DraftRentalContrat != null)
                {
                    return doTbt_DraftRentalContrat.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_SG;
                }
                return false;
            }
        }
        public bool IsProductMaintenance
        {
            get
            {
                if (doTbt_DraftRentalContrat != null)
                {
                    return doTbt_DraftRentalContrat.ProductTypeCode == SECOM_AJIS.Common.Util.ConstantValue.ProductType.C_PROD_TYPE_MA;
                }
                return false;
            }
        }
    }
    /// <summary>
    /// DO for getting customer data
    /// </summary>
    public class CTS010_RetrieveCustomerCondition
    {
        [CTS010_CustConditionNotNullOrEmptyAttribute(ControlName = "CustCode")]
        public string CustCode { get; set; }
        public int CustType { get; set; }
    }
    /// <summary>
    /// DO for getting site data
    /// </summary>
    public class CTS010_RetrieveSiteCondition
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
    public class CTS010_AddEmailAddressCondition
    {
        public bool FromPopUp { get; set; }
        public List<tbt_DraftRentalEmail> NewEmailAddressList { get; set; }
    }
    //public class CTS010_FeeInformation
    //{
    //    public string Normal { get; set; }
    //    public string Order { get; set; }
    //    public string Approve { get; set; }
    //    public string Complete { get; set; }
    //    public string Start { get; set; }
    //}
    /// <summary>
    /// DO for billing target data
    /// </summary>
    public class CTS010_BillingTargetData : tbt_DraftRentalBillingTarget
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
        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }

        public string ContractFeeCurrencyType { get; set; }
        public decimal? ContractFee { get; set; }
        public decimal? ContractFeeUsd { get; set; }

        public string InstallFee_ApprovalCurrencyType { get; set; }
        public decimal? InstallFee_Approval { get; set; }
        public decimal? InstallFee_ApprovalUsd { get; set; }

        public string InstallFee_CompleteInstallationCurrencyType { get; set; }
        public decimal? InstallFee_CompleteInstallation { get; set; }
        public decimal? InstallFee_CompleteInstallationUsd { get; set; }

        public string InstallFee_StartServiceCurrencyType { get; set; }
        public decimal? InstallFee_StartService { get; set; }
        public decimal? InstallFee_StartServiceUsd { get; set; }

        public string DepositFeeCurrencyType { get; set; }
        public decimal? DepositFee { get; set; }
        public decimal? DepositFeeUsd { get; set; }

        public decimal? TotalFee { get; set; }
        public decimal? TotalFeeUsd { get; set; }

        public string PaymentMethod_Approval { get; set; }
        public string PaymentMethod_CompleteInstallation { get; set; }
        public string PaymentMethod_StartService { get; set; }
        public string PaymentMethod_Deposit { get; set; }

        public string BillingClientName
        {
            get
            {
                string name = string.Empty;
                if (this.BillingClient != null)
                {
                    name = string.Format("(1) {0}<br/>(2) {1}",
                                    this.BillingClient.FullNameEN,
                                    this.BillingClient.FullNameLC);
                }
                return name;
            }
        }
        public string BillingOfficeName { get; set; }

        public string TextContractFee
        {
            get
            {
                string txt;
                if (this.ContractFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    txt = CommonUtil.TextNumeric(this.ContractFee);
                else
                    txt = CommonUtil.TextNumeric(this.ContractFeeUsd);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.ContractFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }

                return txt;
            }
        }
        public string TextInstallationFee
        {
            get
            {
                string txtApproval;
                if (this.InstallFee_ApprovalCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    txtApproval = CommonUtil.TextNumeric(this.InstallFee_Approval);
                else
                    txtApproval = CommonUtil.TextNumeric(this.InstallFee_ApprovalUsd);

                if (txtApproval == "")
                    txtApproval = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.InstallFee_ApprovalCurrencyType);
                        if (curr != null)
                            txtApproval = string.Format("{0} {1}", curr.ValueDisplayEN, txtApproval);
                    }
                }

                string txtComplete;
                if (this.InstallFee_CompleteInstallationCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    txtComplete = CommonUtil.TextNumeric(this.InstallFee_CompleteInstallation);
                else
                    txtComplete = CommonUtil.TextNumeric(this.InstallFee_CompleteInstallationUsd);

                if (txtComplete == "")
                    txtComplete = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.InstallFee_CompleteInstallationCurrencyType);
                        if (curr != null)
                            txtComplete = string.Format("{0} {1}", curr.ValueDisplayEN, txtComplete);
                    }
                }

                string txtStart;
                if (this.InstallFee_StartServiceCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    txtStart = CommonUtil.TextNumeric(this.InstallFee_StartService);
                else
                    txtStart = CommonUtil.TextNumeric(this.InstallFee_StartServiceUsd);

                if (txtStart == "")
                    txtStart = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.InstallFee_StartServiceCurrencyType);
                        if (curr != null)
                            txtStart = string.Format("{0} {1}", curr.ValueDisplayEN, txtStart);
                    }
                }

                return string.Format(
@"<div class='text-installfee'>
    <p>
        <span>(1)</span>
        <span class='fee-value'>{0}</span>
    </p>
    <p>
        <span>(2)</span>
        <span class='fee-value'>{1}</span>
    </p>
    <p>
        <span>(3)</span>
        <span class='fee-value'>{2}</span>
    </p>
</div>",
                                    txtApproval,
                                    txtComplete,
                                    txtStart);
            }
        }
        public string TextDepositFee
        {
            get
            {
                string txt;
                if (this.DepositFeeCurrencyType == SECOM_AJIS.Common.Util.ConstantValue.CurrencyUtil.C_CURRENCY_LOCAL)
                    txt = CommonUtil.TextNumeric(this.DepositFee);
                else
                    txt = CommonUtil.TextNumeric(this.DepositFeeUsd);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.DepositFeeCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }

                return txt;
            }
        }
        public string BillingTargetKeyIndex
        {
            get
            {
                return CommonUtil.TextList(new string[] { this.BillingClientCode, this.BillingOfficeCode }, ":");
            }
        }
        public bool IsBillingTarget(string key)
        {
            if (CommonUtil.IsNullOrEmpty(key) == false)
            {
                if (key == BillingTargetKeyIndex)
                    return true;
            }
            return false;
        }
        public bool IsBillingTarget(string BillingClientCode, string BillingOfficeCode)
        {
            string key = CommonUtil.TextList(new string[] { BillingClientCode, BillingOfficeCode }, ":");
            return IsBillingTarget(key);
        }

        public string DocLanguage { get; set; } //Add by Jutarat A. on 18122013
    }
    /// <summary>
    /// DO for getting billing target data
    /// </summary>
    [MetadataType(typeof(CTS010_RetrieveBillingTargetCondition_MetaData))]
    public class CTS010_RetrieveBillingTargetCondition : tbt_DraftRentalBillingTarget
    {
    }
    /// <summary>
    /// DO for getting billing target data
    /// </summary>
    [MetadataType(typeof(CTS010_RetrieveBillingClientCondition_MetaData))]
    public class CTS010_RetrieveBillingClientCondition : tbt_DraftRentalBillingTarget
    {
    }
    /// <summary>
    /// DO for update billing client is case of copy data
    /// </summary>
    public class CTS010_CopyBillingNameAddressCondition
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
    /// DO for validate billing target data
    /// </summary>
    [MetadataType(typeof(CTS010_UpdateBillingTargetData_MetaData))]
    public class CTS010_UpdateBillingTargetData : CTS010_BillingTargetData
    {
        public enum UPDATE_MODE
        {
            NEW = 0,
            UPDATE
        }

        public int UpdateModeID { get; set; }
        public UPDATE_MODE UpdateMode
        {
            get
            {
                return (UPDATE_MODE)Enum.ToObject(typeof(UPDATE_MODE), this.UpdateModeID);
            }
            set
            {
                this.UpdateModeID = (int)value;
            }
        }
        public int RowIndex { get; set; }
    }
    /// <summary>
    /// DO for validate billing target data
    /// </summary>
    [MetadataType(typeof(CTS010_UpdateBillingTargetData2_MetaData))]
    public class CTS010_UpdateBillingTargetData2 : CTS010_BillingTargetData
    {
    }
    /// <summary>
    /// DO for validate billing client
    /// </summary>
    [MetadataType(typeof(CTS010_UpdateBillingClient_MetaData))]
    public class CTS010_UpdateBillingClient : tbm_BillingClient
    {
    }
    /// <summary>
    /// DO for send result data to screen
    /// </summary>
    public class CTS010_UpdateBillingResult
    {
        public string TotalContractFee { get; set; }
        public string TotalContractFeeUsd { get; set; }
        public string TotalInstallationFee { get; set; }
        public string TotalInstallationFeeUsd { get; set; }
        public string TotalDepositFee { get; set; }
        public string TotalDepositFeeUsd { get; set; }
    }

    #region Register class

    /// <summary>
    /// DO for register rental contract data
    /// </summary>
    public class CTS010_RegisterRentalContractData : tbt_DraftRentalContract
    {
        public string QuotationTargetCodeLong
        {
            get
            {
                CommonUtil cmm = new CommonUtil();
                return cmm.ConvertQuotationTargetCode(this.QuotationTargetCode, CommonUtil.CONVERT_TYPE.TO_LONG);
            }
        }

        public List<string> OperationType { get; set; }

        public bool IsContractCodeForDepositFeeSlide { get; set; }
        public bool IsBranchChecked { get; set; }
        public bool IsBillingEditMode { get; set; }
    }
    /// <summary>
    /// DO for validate register rental contract data in case of alarm
    /// </summary>
    [MetadataType(typeof(CTS010_RegisterRentalContractData_AL_MetaData))]
    public class CTS010_RegisterRentalContractData_AL : CTS010_RegisterRentalContractData
    {
    }
    /// <summary>
    /// DO for validate register rentral contract data (expect alarm)
    /// </summary>
    [MetadataType(typeof(CTS010_RegisterRentalContractData_OTHER_MetaData))]
    public class CTS010_RegisterRentalContractData_OTHER : CTS010_RegisterRentalContractData
    {
    }

    #endregion
    #region Validate business

    /// <summary>
    /// 
    /// </summary>
    [MetadataType(typeof(CTS010_RegisterRentalContracrData_Employee_MetaData))]
    public class CTS010_RegisterRentalContracrData_Employee : tbt_DraftRentalContract
    {
    }
    /// <summary>
    /// DO for validate register contract customer
    /// </summary>
    [MetadataType(typeof(CTS010_RegisterContractCutomer_MetaData))]
    public class CTS010_RegisterContractCutomer : doCustomer
    {
    }
    /// <summary>
    /// DO for validate register site data
    /// </summary>
    [MetadataType(typeof(CTS010_RegisterSiteData_MetaData))]
    public class CTS010_RegisterSiteData : doSite
    {
    }

    #endregion

    /// <summary>
    /// DO for send register result to screen
    /// </summary>
    public class CTS010_RegisterResult
    {
        public string ContractTargetCustCode { get; set; }
        public string ContractTargetStatusCodeName { get; set; }
        public string RealCustomerCustCode { get; set; }
        public string RealCustomerStatusCodeName { get; set; }
        public string SiteCode { get; set; }
        public string ContractTargetIDNo { get; set; }
        public string RealCustomerIDNo { get; set; }
    }
}
namespace SECOM_AJIS.Presentation.Contract.Models.CustomAttribute
{
    /// <summary>
    /// Attribute for check customer is null or empty
    /// </summary>
    public class CTS010_CustConditionNotNullOrEmptyAttribute : NotNullOrEmptyAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value))
            {
                CTS010_RetrieveCustomerCondition cond = validationContext.ObjectInstance as CTS010_RetrieveCustomerCondition;
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
    public class CTS010_FeeRelateWithPaymentMethodAttribute : AValidatorAttribute
    {
        private string PaymentMethodField { get; set; }

        public CTS010_FeeRelateWithPaymentMethodAttribute(string PaymentMethodField)
        {
            this.PaymentMethodField = PaymentMethodField;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            tbt_DraftRentalBillingTarget obj = validationContext.ObjectInstance as tbt_DraftRentalBillingTarget;
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
    /// Attribute for check branch is null or empty
    /// </summary>
    public class CTS010_BranchNotNullOrEmptyAttribute : NotNullOrEmptyAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (CommonUtil.IsNullOrEmpty(value))
            {
                CTS010_RegisterRentalContractData cond = validationContext.ObjectInstance as CTS010_RegisterRentalContractData;
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
    /// Metadata for CTS010_RetrieveQuotationCondition_New DO
    /// </summary>
    public class CTS010_RetrieveQuotationCondition_New_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblQuotationTargetCode",
                        ControlName = "QuotationTargetCode")]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblAlphabet",
                        ControlName = "Alphabet")]
        public string Alphabet { get; set; }
    }
    /// <summary>
    /// Metadata for CTS010_RetrieveQuotationCondition_Edit DO
    /// </summary>
    public class CTS010_RetrieveQuotationCondition_Edit_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblQuotationTargetCode",
                        ControlName = "QuotationTargetCode")]
        public string QuotationTargetCode { get; set; }
    }
    /// <summary>
    /// Metadata for CTS010_RetrieveQuotationCondition_EditChanged DO
    /// </summary>
    public class CTS010_RetrieveQuotationCondition_EditChanged_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblQuotationTargetCode",
                        ControlName = "Alphabet")]
        public string Alphabet { get; set; }
    }
    /// <summary>
    /// Metadata for CTS010_RetrieveBillingTargetCondition DO
    /// </summary>
    public class CTS010_RetrieveBillingTargetCondition_MetaData
    {
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS010",
            Parameter = "lblBillingTargetCode",
            ControlName = "BillingTargetCode")]
        public string BillingTargetCode { get; set; }
    }
    /// <summary>
    /// Metadata for CTS010_RetrieveBillingClientCondition DO
    /// </summary>
    public class CTS010_RetrieveBillingClientCondition_MetaData
    {
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS010",
            Parameter = "lblBillingClientCode",
            ControlName = "BillingClientCode")]
        public string BillingClientCode { get; set; }
    }
    /// <summary>
    /// Metadata for CTS010_UpdateBillingTargetData DO
    /// </summary>
    public class CTS010_UpdateBillingTargetData_MetaData
    {
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS010",
            Parameter = "lblBillingOffice",
            ControlName = "BillingOfficeCode",
            Order = 2)]
        public string BillingOfficeCode { get; set; }
    }
    /// <summary>
    /// Metadata for  CTS010_UpdateBillingTargetData2 DO
    /// </summary>
    public class CTS010_UpdateBillingTargetData2_MetaData
    {
        [CTS010_FeeRelateWithPaymentMethod("PaymentMethod_Approval",
            Module = MessageUtil.MODULE_CONTRACT,
            MessageCode = MessageUtil.MessageList.MSG3037,
            ControlName = "PaymentMethod_Approval")]
        public decimal? InstallFee_Approval { get; set; }
        [CTS010_FeeRelateWithPaymentMethod("PaymentMethod_CompleteInstallation",
            Module = MessageUtil.MODULE_CONTRACT,
            MessageCode = MessageUtil.MessageList.MSG3029,
            ControlName = "PaymentMethod_CompleteInstallation")]
        public decimal? InstallFee_CompleteInstallation { get; set; }
        [CTS010_FeeRelateWithPaymentMethod("PaymentMethod_StartService",
            Module = MessageUtil.MODULE_CONTRACT,
            MessageCode = MessageUtil.MessageList.MSG3030,
            ControlName = "PaymentMethod_StartService")]
        public decimal? InstallFee_StartService { get; set; }
        [CTS010_FeeRelateWithPaymentMethod("PaymentMethod_Deposit",
            Module = MessageUtil.MODULE_CONTRACT,
            MessageCode = MessageUtil.MessageList.MSG3031,
            ControlName = "PaymentMethod_Deposit")]
        public decimal? DepositFee { get; set; }
    }
    /// <summary>
    /// Metadata for CTS010_UpdateBillingClient DO
    /// </summary>
    public class CTS010_UpdateBillingClient_MetaData
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

    #region Register meta class

    /// <summary>
    /// Metadata for CTS010_RegisterRentalContractData_AL DO
    /// </summary>
    public class CTS010_RegisterRentalContractData_AL_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 1,
                        Parameter = "lblQuotationTargetCode")]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 2,
                        Parameter = "lblQuotationTargetCode")]
        public string Alphabet { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 3,
                        Parameter = "lblExpectedInstallationDate",
                        ControlName = "ExpectedInstallCompleteDate")]
        public Nullable<System.DateTime> ExpectedInstallCompleteDate { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 4,
                        Parameter = "lblExpectedOperationDate",
                        ControlName = "ExpectedStartServiceDate")]
        public Nullable<System.DateTime> ExpectedStartServiceDate { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS010",
        //                Order = 5,
        //                Parameter = "lblMSGContractFee",
        //                ControlName = "OrderContractFee")]
        //public Nullable<decimal> OrderContractFee { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS010",
        //                Order = 6,
        //                Parameter = "lblMSGInstallationFee",
        //                ControlName = "OrderInstallFee")]
        //public Nullable<decimal> OrderInstallFee { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS010",
        //                Order = 7,
        //                Parameter = "lblMSGApproveContract",
        //                ControlName = "OrderInstallFee_ApproveContract")]
        //public Nullable<decimal> OrderInstallFee_ApproveContract { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS010",
        //                Order = 8,
        //                Parameter = "lblMSGCompleteInstallation",
        //                ControlName = "OrderInstallFee_CompleteInstall")]
        //public Nullable<decimal> OrderInstallFee_CompleteInstall { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS010",
        //                Order = 9,
        //                Parameter = "lblMSGStartService",
        //                ControlName = "OrderInstallFee_StartService")]
        //public Nullable<decimal> OrderInstallFee_StartService { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS010",
        //                Order = 10,
        //                Parameter = "lblMSGOrderDepositFee",
        //                ControlName = "OrderDepositFee")]
        //public Nullable<decimal> OrderDepositFee { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 12,
                        Parameter = "lblBillingCycle",
                        ControlName = "BillingCycle")]
        public Nullable<int> BillingCycle { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 13,
                        Parameter = "lblPaymentMethod",
                        ControlName = "PayMethod")]
        public string PayMethod { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 14,
                        Parameter = "lblCreditTerm",
                        ControlName = "CreditTerm")]
        public Nullable<int> CreditTerm { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 15,
                        Parameter = "lblCalculationDailyFee",
                        ControlName = "CalDailyFeeStatus")]
        public string CalDailyFeeStatus { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 16,
                        Parameter = "lblContractOffice",
                        ControlName = "ContractOfficeCode")]
        public string ContractOfficeCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                       Screen = "CTS010",
                       Order = 17,
                       Parameter = "lblOperationOfficeCode",
                       ControlName = "OperationOfficeCode")]
        public string OperationOfficeCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 18,
                        Parameter = "lblSalesman1",
                        ControlName = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }

        [NotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblContractSignerType",
                        ControlName = "CPContractTargetSignerTypeCode",
                        Order = 22)]
        public string ContractTargetSignerTypeCode { get; set; }
        [CTS010_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblBranchNameEN",
                        ControlName = "BranchNameEN",
                        Order = 23)]
        public string BranchNameEN { get; set; }
        [CTS010_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblBranchAddressEN",
                        ControlName = "BranchAddressEN",
                        Order = 24)]
        public string BranchAddressEN { get; set; }
        [CTS010_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblBranchNameLC",
                        ControlName = "BranchNameLC",
                        Order = 25)]
        public string BranchNameLC { get; set; }
        [CTS010_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblBranchAddressLC",
                        ControlName = "BranchAddressLC",
                        Order = 26)]
        public string BranchAddressLC { get; set; }
    }
    /// <summary>
    /// Metadata for CTS010_RegisterRentalContractData_OTHER DO
    /// </summary>
    public class CTS010_RegisterRentalContractData_OTHER_MetaData
    {
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 1,
                        Parameter = "lblQuotationTargetCode")]
        public string QuotationTargetCode { get; set; }
        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 2,
                        Parameter = "lblQuotationTargetCode")]
        public string Alphabet { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 4,
                        Parameter = "lblExpectedOperationDate",
                        ControlName = "ExpectedStartServiceDate")]
        public Nullable<System.DateTime> ExpectedStartServiceDate { get; set; }

        //[NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
        //                Screen = "CTS010",
        //                Order = 5,
        //                Parameter = "lblMSGContractFee",
        //                ControlName = "OrderContractFee")]
        //public Nullable<decimal> OrderContractFee { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 12,
                        Parameter = "lblBillingCycle",
                        ControlName = "BillingCycle")]
        public Nullable<int> BillingCycle { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 13,
                        Parameter = "lblPaymentMethod",
                        ControlName = "PayMethod")]
        public string PayMethod { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 14,
                        Parameter = "lblCreditTerm",
                        ControlName = "CreditTerm")]
        public Nullable<int> CreditTerm { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 15,
                        Parameter = "lblCalculationDailyFee",
                        ControlName = "CalDailyFeeStatus")]
        public string CalDailyFeeStatus { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 16,
                        Parameter = "lblContractOffice",
                        ControlName = "ContractOfficeCode")]
        public string ContractOfficeCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                       Screen = "CTS010",
                       Order = 17,
                       Parameter = "lblOperationOfficeCode",
                       ControlName = "OperationOfficeCode")]
        public string OperationOfficeCode { get; set; }

        [NotNullOrEmpty(Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Order = 18,
                        Parameter = "lblSalesman1",
                        ControlName = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }

        [NotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblContractSignerType",
                        ControlName = "CPContractTargetSignerTypeCode",
                        Order = 22)]
        public string ContractTargetSignerTypeCode { get; set; }
        [CTS010_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblBranchNameEN",
                        ControlName = "BranchNameEN",
                        Order = 23)]
        public string BranchNameEN { get; set; }
        [CTS010_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblBranchAddressEN",
                        ControlName = "BranchAddressEN",
                        Order = 24)]
        public string BranchAddressEN { get; set; }
        [CTS010_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblBranchNameLC",
                        ControlName = "BranchNameLC",
                        Order = 24)]
        public string BranchNameLC { get; set; }
        [CTS010_BranchNotNullOrEmpty(
                        Controller = MessageUtil.MODULE_CONTRACT,
                        Screen = "CTS010",
                        Parameter = "lblBranchAddressLC",
                        ControlName = "BranchAddressLC",
                        Order = 25)]
        public string BranchAddressLC { get; set; }
    }

    #endregion
    #region Validate business meta data

    /// <summary>
    /// Metadata for CTS010_RegisterRentalContracrData_Employee DO
    /// </summary>
    public class CTS010_RegisterRentalContracrData_Employee_MetaData
    {
        [EmployeeExist(Control = "SalesmanEmpNo1")]
        public string SalesmanEmpNo1 { get; set; }
        [EmployeeExist(Control = "SalesmanEmpNo2")]
        public string SalesmanEmpNo2 { get; set; }
        [EmployeeExist(Control = "SalesSupporterEmpNo")]
        public string SalesSupporterEmpNo { get; set; }
    }
    /// <summary>
    /// Metadata for CTS010_RegisterContractCutomer DO
    /// </summary>
    public class CTS010_RegisterContractCutomer_MetaData
    {
        [CodeNullOtherNotNull("CustCode",
                                Controller = MessageUtil.MODULE_CONTRACT,
                                Screen = "CTS010",
                                Parameter = "lblContractTarget")]
        public string CustNameEN { get; set; }
        //[CodeNullOtherNotNull("CustCode",
        //                        Controller = MessageUtil.MODULE_CONTRACT,
        //                        Screen = "CTS010",
        //                        Parameter = "lblContractTarget")]
        //public string CustNameLC { get; set; }
        [CodeNullOtherNotNull("CustCode",
                                Controller = MessageUtil.MODULE_CONTRACT,
                                Screen = "CTS010",
                                Parameter = "lblContractTarget")]
        public string CustFullNameEN { get; set; }
        //[CodeNullOtherNotNull("CustCode",
        //                        Controller = MessageUtil.MODULE_CONTRACT,
        //                        Screen = "CTS010",
        //                        Parameter = "lblContractTarget")]
        //public string CustFullNameLC { get; set; }
        [CodeNullOtherNotNull("CustCode",
                                Controller = MessageUtil.MODULE_CONTRACT,
                                Screen = "CTS010",
                                Parameter = "lblContractTarget")]
        public string CustTypeCode { get; set; }
    }
    /// <summary>
    /// Metadata for CTS010_RegisterSiteData DO
    /// </summary>
    public class CTS010_RegisterSiteData_MetaData
    {
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS010",
            Parameter = "lblSiteInfo")]
        public string SiteNameEN { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS010",
            Parameter = "lblSiteInfo")]
        public string AddressEN { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_CONTRACT,
        //    Screen = "CTS010",
        //    Parameter = "lblSiteInfo")]
        //public string RoadEN { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS010",
            Parameter = "lblSiteInfo")]
        public string SubDistrictEN { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS010",
            Parameter = "lblSiteInfo")]
        public string DistrictNameEN { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS010",
            Parameter = "lblSiteInfo")]
        public string ProvinceNameEN { get; set; }

        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS010",
            Parameter = "lblSiteInfo")]
        public string SiteNameLC { get; set; }
        // 2017.02.15 delete matsuda start
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_CONTRACT,
        //    Screen = "CTS010",
        //    Parameter = "lblSiteInfo")]
        //public string AddressLC { get; set; }
        // 2017.02.15 delete matsuda end
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_CONTRACT,
        //    Screen = "CTS010",
        //    Parameter = "lblSiteInfo")]
        //public string RoadLC { get; set; }
        //[NotNullOrEmpty(
        //    Controller = MessageUtil.MODULE_CONTRACT,
        //    Screen = "CTS010",
        //    Parameter = "lblSiteInfo")]
        //public string SubDistrictLC { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS010",
            Parameter = "lblSiteInfo")]
        public string DistrictNameLC { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS010",
            Parameter = "lblSiteInfo")]
        public string ProvinceNameLC { get; set; }
        [NotNullOrEmpty(
            Controller = MessageUtil.MODULE_CONTRACT,
            Screen = "CTS010",
            Parameter = "lblSiteInfo")]
        public string BuildingUsageCode { get; set; }
    }

    #endregion
}

