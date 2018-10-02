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

namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Screen caller for ICS_084 screen
    /// </summary>
    public enum ICS084_ScreenCaller
    {
        ICS080 = 0,
        ICS081
    }

    /// <summary>
    /// Data object for adjust invoice information
    /// </summary>
    public class ICS084_MatchInvoiceData
    {
        public string InvoiceNo { get; set; }
        public decimal? KeyInMatchAmountIncWHT { get; set; }
        public string KeyInMatchAmountIncWHTCurrencyType { get; set; }
        public decimal? KeyInWHTAmount { get; set; }
        public string KeyInWHTAmountCurrencyType { get; set; }
        public string KeyInMatchAmountIncWHT_ID { get; set; }
        public string KeyInWHTAmount_ID { get; set; }
        public string InvoiceAmountCurrencyType { get; set; }
    }

    /// <summary>
    /// Data object for ICS_084 screen
    /// </summary>
    public class ICS084_ScreenParameter : ScreenParameter
    {
        [KeepSession]
        public ICS084_ScreenCaller ScreenCaller { get; set; }
        [KeepSession]
        public string PaymentTransNo { get; set; }
        [KeepSession]
        public string InvoiceNo { get; set; }
        //Important: this parameter is kept billing target code "Long Format"
        [KeepSession]
        public string BillingTargetCode { get; set; }
        [KeepSession]
        public List<doUnpaidInvoice> UnpaidInvoice { get; set; }
        [KeepSession]
        public doPayment doPayment { get; set; }

        public bool SpecialProcess { get; set; }
        public string ApproveNo { get; set; }
        public decimal? BankFee { get; set; }
        public string BankFeeCurrencyType { get; set; }
        public decimal? OtherExpense { get; set; }
        public string OtherExpenseCurrencyType { get; set; }
        public decimal? OtherIncome { get; set; }
        public string OtherIncomeCurrencyType { get; set; }
        public decimal? ExchangeLoss { get; set; }
        public string ExchangeLossCurrencyType { get; set; }
        public decimal? ExchangeGain { get; set; }
        public string ExchangeGainCurrencyType { get; set; }
        public List<ICS084_MatchInvoiceData> MatchInvoiceData { get; set; }
        public decimal? BalanceAfterProcessing { get; set; }
        public string BalanceAfterProcessingCurrencyType { get; set; } // add by jirawat jannet on 2016-11-21

        public tbt_Payment PaymentData { get; set; } //Add by Jutarat A. on 13062013
        public string FirstPaymentAmountCurrencyType { get; set; } // add by jirawat jannet on 2016-11-01
    }

    /// <summary>
    /// Data object for register adjust payment matching
    /// </summary>
    public class ICS084_AdjustMatchPaymentRegister
    {
        public string ResultFlag { get; set; }
        public decimal? BalanceAfterProcessing { get; set; }
        public List<string> ConfirmMessageID { get; set; }
        public string BalanceAfterProcessingCurrencyType { get; set; }
    }
}
