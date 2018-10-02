using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Income.MetaData;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data ojbect of payment information.
    /// </summary>
    [MetadataType(typeof(doPayment_Meta))]
    public partial class doPayment
    {
        [LanguageMapping]
        public string PaymentTypeDisplay { get; set; }

        public string SECOMBankName
        {
            get
            {
                string currentLanguage = CommonUtil.GetCurrentLanguage();
                if (currentLanguage == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return SECOMBankNameEN;
                }
                else if (currentLanguage == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return SECOMBankNameEN;
                }
                else
                {
                    return SECOMBankNameLC;
                }
            }
        }
        public string SECOMBranchName
        {
            get
            {
                string currentLanguage = CommonUtil.GetCurrentLanguage();
                if (currentLanguage == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return SECOMBranchNameEN;
                }
                else if (currentLanguage == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return SECOMBranchNameEN;
                }
                else
                {
                    return SECOMBranchNameLC;
                }
            }
        }
        public string SECOMBankFullName
        {
            get
            {
                return string.Format("{0} / {1}", this.SECOMBankName, this.SECOMBranchName);
            }
        }

        public string SendingBankName
        {
            get
            {
                string currentLanguage = CommonUtil.GetCurrentLanguage();
                if (currentLanguage == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return SendingBankNameEN;
                }
                else if (currentLanguage == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return SendingBankNameEN;
                }
                else
                {
                    return SendingBankNameLC;
                }
            }
        }
        public string SendingBranchName
        {
            get
            {
                string currentLanguage = CommonUtil.GetCurrentLanguage();
                if (currentLanguage == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return SendingBranchNameEN;
                }
                else if (currentLanguage == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return SendingBranchNameEN;
                }
                else
                {
                    return SendingBranchNameLC;
                }
            }
        }
        public string SendingBankFullName
        {
            get
            {
                return string.Format("{0} / {1}", this.SendingBankName, this.SendingBranchName);
            }
        }

        public string PaymentAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.PaymentAmountCurrencyType);
            }
        }
        public decimal? PaymentAmountVal
        {
            get
            {
                if (this.PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return PaymentAmount;
                else return PaymentAmountUsd;
            }
        }
        public string PaymentAmountShow
        {
            get
            {
                return string.Format("{0} {1}", PaymentAmountCurrencyTypeName, PaymentAmountVal.HasValue ? PaymentAmountVal.Value.ToString("N2"):"0.00");
            }
        }

        public string MatchableBalanceCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.MatchableBalanceCurrencyType);
            }
        }
        public decimal MatchableBalanceVal
        {
            get
            {
                if (MatchableBalanceCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return MatchableBalance;
                else return MatchableBalanceUsd;
            }
        }
        public string MatchableBalanceShow
        {
            get
            {
                return string.Format("{0} {1}", MatchableBalanceCurrencyTypeName, MatchableBalanceVal.ToString("N2"));
            }
        }


        //Show Receipt info for grid
        public string ReceiptGrid
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}", this.RefAdvanceReceiptNo, this.RefAdvanceReceiptAmount.HasValue ? this.RefAdvanceReceiptAmount.Value.ToString("#,##0.00") : "");
            }
        }

        //Show SECOM Bank info for grid
        public string SECOMBankGrid
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}", this.SECOMBankName, this.SECOMBranchName);
            }
        }

        //Show Sending info for grid
        public string SendingGrid
        {
            get
            {
                return string.Format("(1) {0}<br />(2) {1}", this.SendingBankName, this.SendingBranchName);
            }
        }

        //Show Sending info for grid
        public string InvoiceReceiptGrid
        {
            get
            {
                return string.Format("(1) {0}<br />(2) {1}", this.RefInvoiceNo, this.RefAdvanceReceiptNo);
            }
        }

        //Show Posdate cheque/promissory note no., date
        public string PromissoryNoteGrid
        {
            get
            {
                return string.Format("(1) {0}<br />(2) {1}", this.DocNo, CommonUtil.TextDate(this.DocDate));
            }
        }

        public Nullable<DateTime> PaymentDateNull { get; set; }

        public string PaymentDateDisplay
        {
            get {
                return CommonUtil.TextDate(this.PaymentDate);
            }
        }
        public string DocDateDisplay
        {
            get
            {
                return CommonUtil.TextDate(this.DocDate);
            }
        }

        public string PayerBankAccNoDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(this.PayerBankAccNo))
                {
                    return string.Format("{0}-{1}-{2}-{3}",
                        this.PayerBankAccNo.Substring(0, 3),
                        this.PayerBankAccNo.Substring(3, 1),
                        this.PayerBankAccNo.Substring(4, 5),
                        this.PayerBankAccNo.Substring(9, 1)
                        );
                }
                else
                {
                    return "";
                }
            }
        }
        public string MatchableBalanceGrid
        {
            get
            {
                return this.MatchableBalance.ToString("#,##0.00");
            }
        }

        [LanguageMapping]
        public string ChequeStatus { get; set; }
        [LanguageMapping]
        public string ChequeReturnReasonDesc { get; set; }

        public string PayerAndMatchR
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}", this.Payer, this.MatchRGroupName);
            }
        }
    }
}

namespace SECOM_AJIS.DataEntity.Income.MetaData
{
    /// <summary>
    /// Meta data for tbt_Payment class
    /// </summary>
    public class doPayment_Meta
    {
        [GridToolTip("PaymentTypeDisplay")]
        public string PaymentType { get; set; }
    }
}
