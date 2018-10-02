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
using SECOM_AJIS.Presentation.Income.Models.MetaData;
using SECOM_AJIS.DataEntity.Billing;

namespace SECOM_AJIS.Presentation.Income.Models
{
    #region enum
    /// <summary>
    /// Screen mode for ICS_080 screen
    /// </summary>
    public enum ICS080_SCREEN_MODE
    {
        Match = 0,
        View,
        Delete
    }

    public enum ICS080_MATCHING_DETAIL_SECTION : int
    {
        Payment = 0,
        MatchingResult
    }

    public enum ICS080_MATCHING_DETAIL_SORTING : int
    { 
        PaymentAmount = 0,
        CancelMatchAmount,
        CancelWHTAmount,
        CancelBankFee,
        CancelOtherExpense,
        CancelOtherIncome,
        MatchAmount,
        WHTAmount,
        BankFee,
        OtherExpense,
        OtherIncome
    }
    #endregion

    /// <summary>
    /// Data object for ICS_080 screen
    /// </summary>
    public class ICS080_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public ICS080_SCREEN_MODE ScreenMode
        {
            get
            {
                int mode = 0;
                if (CommonUtil.IsNullOrEmpty(this.SubObjectID) == false)
                    mode = int.Parse(this.SubObjectID);

                return (ICS080_SCREEN_MODE)Enum.ToObject(typeof(ICS080_SCREEN_MODE), mode);
            }
            set
            {
                this.SubObjectID = ((int)value).ToString();
            }
        }
        public ICS080_PaymentSearchCriteria PaymentSearchCriteria { get; set; }
        public string AllPaymentTypeExceptCreditNoteDecreased { get; set; } //Add by Jutarat A. on 06082013
    }

    public class ICS080_PaymentMatchingResult
    {
        public doPayment doPayment { get; set; }
        public List<tbt_Invoice> doTbt_Invoice { get; set; }
        public bool IsPermissionEncash { get; set; }
        public bool IsEncashable { get; set; }
        public bool IsEncashed { get; set; }
    }

    /// <summary>
    /// Meta data of ICS080_PaymentSearchCriteria data object
    /// </summary>
    [MetadataType(typeof(ICS080_PaymentSearchCriteria_MetaData))]
    public class ICS080_PaymentSearchCriteria : doPaymentSearchCriteria
    {
    }
}

namespace SECOM_AJIS.Presentation.Income.Models.MetaData
{
    /// <summary>
    /// Meta data of ICS080_PaymentSearchCriteria data object
    /// </summary>
    public class ICS080_PaymentSearchCriteria_MetaData
    {
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string PaymentType { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string Status { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public int? SECOMAccountID { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string PaymentTransNo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string Payer { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public DateTime? PaymentDateFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public DateTime? PaymentDateTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public decimal? MatchableBalanceFrom { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public decimal? MatchableBalanceTo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string InvoiceNo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string ReceiptNo { get; set; }
        [AtLeast1FieldNotNullOrEmpty(MessageCode = MessageUtil.MessageList.MSG0006, Module = MessageUtil.MODULE_COMMON)]
        public string SendingBank { get; set; }
    }
}
