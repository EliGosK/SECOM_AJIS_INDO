using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;

using SECOM_AJIS.Presentation.Common.Models.MetaData;

namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Data object for ICS_081 screen
    /// </summary>
    public class ICS081_ScreenParameter : ScreenParameter
    {
        public string PaymentTransNo { get; set; }
    }

    /// <summary>
    /// Meta data for ICS081_UnpaidBillingTargetSearchCriteria data object
    /// </summary>
    [MetadataType(typeof(ICS081_UnpaidBillingTargetSearchCriteria_MetaData))]
    public class ICS081_UnpaidBillingTargetSearchCriteria : doUnpaidBillingTargetSearchCriteria
    {
    }
}


namespace SECOM_AJIS.Presentation.Common.Models.MetaData
{
    /// <summary>
    ///  data for ICS081_UnpaidBillingTargetSearchCriteria data object
    /// </summary>
    public class ICS081_UnpaidBillingTargetSearchCriteria_MetaData
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string BillingClientName { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public decimal? InvoiceAmountFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public decimal? InvoiceAmountTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public DateTime? IssueInvoiceDateFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public DateTime? IssueInvoiceDateTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public bool? HaveCreditNoteIssued { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public decimal? BillingDetailAmountFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public decimal? BillingDetailAmountTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public int? BillingCycle { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public int? LastPaymentDayFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public int? LastPaymentDayTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public DateTime? ExpectedPaymentDateFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public DateTime? ExpectedPaymentDateTo { get; set; }
        
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public bool? BillingType_ContractFee { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public bool? BillingType_InstallationFee { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public bool? BillingType_DepositFee { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public bool? BillingType_SalePrice { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public bool? BillingType_OtherFee { get; set; }
       
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public bool? PaymentMethod_BankTransfer { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public bool? PaymentMethod_Messenger { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public bool? PaymentMethod_AutoTransfer { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public bool? PaymentMethod_CreditCard { get; set; }
    }
}
