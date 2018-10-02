using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Billing.MetaData;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Billing
{
    [MetadataType(typeof(doInvoice_MetaData))]
    public partial class doInvoice
    {
        public List<tbt_BillingDetail> Tbt_BillingDetails { get; set; }

        public string IssueInvDateDisplay
        {
            get
            {
                return this.IssueInvDate == null ? string.Empty : CommonUtil.TextDate(this.IssueInvDate);
            }
        }

        public string BillingClientNameGridDisplay
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}", this.BillingClientNameEN, this.BillingClientNameLC);
            }
        }
        public string InvoiceAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.InvoiceAmountCurrencyType);
            }
        }
        public decimal BillingAmountIncVATGridDisplay
        {
            get
            {
                return (this.InvoiceAmountVal ?? 0) + (this.VatAmountVal ?? 0);
            }
        }
        public string BillingAmountIncVATGridDisplayCurrencyType
        {
            get
            {
                return InvoiceAmountCurrencyType;
            }
        }
        public decimal? InvoiceAmountVal
        {
            get
            {
                if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return InvoiceAmount;
                else if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return InvoiceAmountUsd;
                else return null;
            }
        }
        public decimal? VatAmountVal
        {
            get
            {
                if (VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return VatAmount;
                else if (VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return VatAmountUsd;
                else return null;
            }
        }
        public string BillingAmountIncVATGridDisplayString
        {
            get
            {
                return string.Format("{0} {1}", InvoiceAmountCurrencyTypeName, BillingAmountIncVATGridDisplay.ToString("N2"));
            }
        }
        public int InvoiceDetailQtyGridDisplay
        {
            get
            {
                return this.Tbt_BillingDetails == null ? 0 : this.Tbt_BillingDetails.Count;
            }
        }

        public string BillingTargetCodeShortFormat
        {
            get
            {
                return new CommonUtil().ConvertBillingTargetCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }

        public string FilePath { set; get; }
        public string InvoicePaymentStatusDisplayName { set; get; }

    }
}

namespace SECOM_AJIS.DataEntity.Billing.MetaData
{
    public class doInvoice_MetaData
    {
        [PaymentStatuaMapping("InvoicePaymentStatusDisplayName")]
        public string InvoicePaymentStatus { get; set; }

    }
}
