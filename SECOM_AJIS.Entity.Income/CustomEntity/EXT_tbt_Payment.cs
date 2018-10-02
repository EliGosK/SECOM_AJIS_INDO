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
using CSI.WindsorHelper;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data ojbect of payment information.
    /// </summary>
    [MetadataType(typeof(tbt_Payment_Meta))]
    public partial class tbt_Payment
    {
        public string SendingBranchNameEN { get; set; }
        public string SendingBranchNameLC { get; set; }
        public string SendingBankNameEN { get; set; }
        public string SendingBankNameLC { get; set; }
        public string SECOMBranchNameEN { get; set; }
        public string SECOMBranchNameLC { get; set; }
        public string SECOMBankNameLC { get; set; }
        public string SECOMBankNameEN { get; set; }

        public string SECOMBankFullName
        {
            get;
            set;
        }
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

        public string SendingBankFullName
        {
            get;
            set;
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

        public decimal? RefAdvanceReceiptAmountValue
        {
            get
            {
                if (RefAdvanceReceiptAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return RefAdvanceReceiptAmount;
                else return RefAdvanceReceiptAmountUsd;
            }
        }

        //Show Receipt info for grid
        public string ReceiptGrid
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {2} {1}"
                    , this.RefAdvanceReceiptNo
                    , this.RefAdvanceReceiptAmountValue.HasValue ? this.RefAdvanceReceiptAmountValue.Value.ToString("#,##0.00") : ""
                    , RefAdvanceReceiptAmountCurrencyTypeName);
            }
        }
        public decimal? PaymentAmountValue
        {
            get
            {
                if (PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return PaymentAmount;
                else return PaymentAmountUsd;
            }
        }
        // Show payment amount with currency
        public string PaymenyAmountShow
        {
            get
            {
                return string.Format("{0} {1}", PaymentAmountCurrencyTypeName
                    , PaymentAmountValue.HasValue ? PaymentAmountValue.Value.ToString("N2") : "");
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

        public string PaymentAmountCurrencyTypeName
        {
            get
            {
                if (string.IsNullOrEmpty(PaymentAmountCurrencyType)) return string.Empty;
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(PaymentAmountCurrencyType); 
            }
        }
        public string RefAdvanceReceiptAmountCurrencyTypeName
        {
            get
            {
                if (string.IsNullOrEmpty(RefAdvanceReceiptAmountCurrencyType)) return string.Empty;
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(RefAdvanceReceiptAmountCurrencyType);
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

        public string PaymentTypeDisplay
        {
            get;
            set;
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
    }
}

namespace SECOM_AJIS.DataEntity.Income.MetaData
{
    /// <summary>
    /// Meta data for tbt_Payment class
    /// </summary>
    public class tbt_Payment_Meta
    {
        [GridToolTip("PaymentTypeDisplay")]
        public string PaymentType { get; set; }
    }
}
