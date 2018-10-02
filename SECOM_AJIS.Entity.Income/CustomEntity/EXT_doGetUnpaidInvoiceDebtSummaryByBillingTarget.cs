using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;

using System.ComponentModel.DataAnnotations;
using SECOM_AJIS.DataEntity.Income.MetaData;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Function for retrieving unpaid invoices data.
    /// </summary>
    [MetadataType(typeof(doGetUnpaidInvoiceDebtSummaryByBillingTarget_MetaData))]
    public partial class doGetUnpaidInvoiceDebtSummaryByBillingTarget
    {
        public string ToJson
        {
            get
            {
                return CommonUtil.CreateJsonString(this);
            }
        }

        public string PaymentMethod
        {
            get
            {
                if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return this.PaymentMethodEN;
                }
                else if (CommonUtil.GetCurrentLanguage() == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return this.PaymentMethodJP;
                }
                else
                {
                    return this.PaymentMethodLC;
                }
            }
        }

        public decimal? UnpaidAmountValue
        {
            get
            {
                if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return UnpaidAmount;
                else return UnpaidAmountUsd;
            }
        }
        public string UnpaidAmountString
        {
            get
            {
                return string.Format("{0} {1}", InvoiceAmountCurrencyTypeName, this.UnpaidAmountValue.HasValue ? this.UnpaidAmountValue.Value.ToString("#,##0.00") : "0.00");
            }
        }

        public string NoOfBillingDetailString
        {
            get
            {
                return string.Format("{0}", this.NoOfBillingDetail.HasValue ? this.NoOfBillingDetail.Value.ToString("#,##0") : "0");
            }
        }

        public string IncludeFirstFeeGridFormat
        {
            get;
            set;
        }


        public string TracingResultRegisteredString
        {
            get
            {
                return string.Format("{0}", this.TracingResultRegistered == 1 ? "Yes" : "No");
            }
        }

        public string OldestDelayedMonthString
        {
            get
            {
                return string.Format("{0}", this.OldestDelayedMonth.HasValue ? this.OldestDelayedMonth.Value.ToString("#,##0") : "0");
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
        public decimal? InvoiceAmountValue
        {
            get
            {
                if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return InvoiceAmount;
                else return InvoiceAmountUsd;
            }
        }
        public string InvoiceAmountString
        {
            get
            {

                return string.Format("{0} {1}", InvoiceAmountCurrencyTypeName, this.InvoiceAmountValue.HasValue ? this.InvoiceAmountValue.Value.ToString("#,##0.00") : "0.00");
            }
        }

        public string VatAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.VatAmountCurrencyType);
            }
        }
        public decimal? VatAmountValue
        {
            get
            {
                if (VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return VatAmount;
                else return VatAmountUsd;
            }
        }
        public string VatAmountString
        {
            get
            {
                return string.Format("{0} {1}", VatAmountCurrencyTypeName, this.VatAmountValue.HasValue ? this.VatAmountValue.Value.ToString("#,##0.00") : "0.00");
            }
        }
        decimal? decTempInvoiceAmount = 0;
        public string InvoiceAmount032String
        {
            get
            {
                decTempInvoiceAmount = 0;
                if (this.InvoiceAmount.HasValue)
                { decTempInvoiceAmount = decTempInvoiceAmount + this.InvoiceAmount; }

                if (this.VatAmount.HasValue)
                { decTempInvoiceAmount = decTempInvoiceAmount + this.VatAmount; }

                return string.Format("{0}", decTempInvoiceAmount.Value.ToString("#,##0.00"));
            }
        }
        public string InvoiceAmount032UsdString
        {
            get
            {
                decimal? val = 0;
                if (this.InvoiceAmountUsd.HasValue)
                { val = val + this.InvoiceAmountUsd; }

                if (this.VatAmountUsd.HasValue)
                { val = val + this.VatAmountUsd; }

                return string.Format("{0}", val.Value.ToString("#,##0.00"));
            }
        }
        public string WHTAmountString
        {
            get
            {
                return string.Format("{0}", this.WHTAmount.HasValue ? this.WHTAmount.Value.ToString("#,##0.00") : "0.00");
            }
        }

        public string OldestInvoiceExpectedPaymentDateValueLessThanToday
        {
            get
            {
                if (this.OldestInvoiceExpectedPaymentDate == null)
                {
                    return "N";
                }
                else if (this.OldestInvoiceExpectedPaymentDate.Value.Date < DateTime.Now.Date)
                {
                    return "Y";
                }
                else
                {
                    return "N";
                }
            }
        }

        public string OldestInvoiceExpectedPaymentDateFlag
        {
            get {
                if (this.OldestInvoiceExpectedPaymentDateValueLessThanToday == "Y")
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
        }

        public string DebtTracingRegisteredGridFormat
        {
            get;
            set;
        }

        public string IssueInvDateGridFormat
        {
            get {
                return CommonUtil.TextDate(this.IssueInvDate);
            }
        }

        public string OldestInvoiceExpectedPaymentDateGridFormat
        {
            get
            {
                return CommonUtil.TextDate(this.OldestInvoiceExpectedPaymentDate);
            }
        }

        [LanguageMapping]
        public string BillingType { get; set; } //Add by Jutarat A. on 26042013

    }
}

//Add by Jutarat A. on 26042013
namespace SECOM_AJIS.DataEntity.Income.MetaData
{
    public class doGetUnpaidInvoiceDebtSummaryByBillingTarget_MetaData
    {
        [GridToolTip("BillingType")]
        public string BillingTypeCode { get; set; }
    }
}
//End Add
