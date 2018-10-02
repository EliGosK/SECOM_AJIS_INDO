using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM_AJIS.DataEntity.Income
{
    public partial class doGetUnpaidBillingTargetByCodeWithExchange
    {
        //public decimal InvoiceAmountVal
        //{
        //    get
        //    {
        //        ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        decimal invoiceAmt = hand.ConvertCurrencyPrice(InvoiceAmount, CurrencyUtil.C_CURRENCY_LOCAL, UnpaidTotalBalanceCurrencyType, 0).Value;
        //        decimal invoiceAmtUsd = hand.ConvertCurrencyPrice(InvoiceAmountUsd, CurrencyUtil.C_CURRENCY_US, UnpaidTotalBalanceCurrencyType, 0).Value;
        //        return invoiceAmt + invoiceAmtUsd;
        //    }
        //}
        public decimal InvoiceAmountVal
        {
            get
            {
                return InvoiceAmount ?? 0;
            }
        }
        public decimal InvoiceAmountValUsd
        {
            get
            {
                return InvoiceAmountUsd ?? 0;
            }
        }

        //public decimal VatAmountVal
        //{
        //    get
        //    {
        //        ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        decimal vatAmt = hand.ConvertCurrencyPrice(VatAmount, CurrencyUtil.C_CURRENCY_LOCAL, UnpaidTotalBalanceCurrencyType, 0).Value;
        //        decimal vatAmtUsd = hand.ConvertCurrencyPrice(VatAmountUsd, CurrencyUtil.C_CURRENCY_US, UnpaidTotalBalanceCurrencyType, 0).Value;
        //        return vatAmt + vatAmtUsd;
        //    }
        //}
        public decimal VatAmountVal
        {
            get
            {
                return VatAmount ?? 0;
            }
        }
        public decimal VatAmountValUsd
        {
            get
            {
                return VatAmountUsd ?? 0;
            }
        }

        //public decimal PaidAmountIncVatVal
        //{
        //    get
        //    {
        //        ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        decimal paidAmt = hand.ConvertCurrencyPrice(PaidAmountIncVat, CurrencyUtil.C_CURRENCY_LOCAL, UnpaidTotalBalanceCurrencyType, 0).Value;
        //        decimal paidAmtUsd = hand.ConvertCurrencyPrice(PaidAmountIncVatUsd, CurrencyUtil.C_CURRENCY_US, UnpaidTotalBalanceCurrencyType, 0).Value;
        //        return paidAmt + paidAmtUsd;
        //    }
        //}
        public decimal PaidAmountIncVatVal
        {
            get
            {
                return PaidAmountIncVat ?? 0;
            }
        }
        public decimal PaidAmountIncVatValUsd
        {
            get
            {
                return PaidAmountIncVatUsd ?? 0;
            }
        }

        //public decimal RegisteredWHTAmountVal
        //{
        //    get
        //    {
        //        ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        decimal registeredWHTAmt = hand.ConvertCurrencyPrice(RegisteredWHTAmount, CurrencyUtil.C_CURRENCY_LOCAL, UnpaidTotalBalanceCurrencyType, 0).Value;
        //        decimal registeredWHTAmtUsd = hand.ConvertCurrencyPrice(RegisteredWHTAmountUsd, CurrencyUtil.C_CURRENCY_US, UnpaidTotalBalanceCurrencyType, 0).Value;
        //        return registeredWHTAmt + registeredWHTAmtUsd;
        //    }
        //}
        public decimal RegisteredWHTAmountVal
        {
            get
            {
                return RegisteredWHTAmount ?? 0;
            }
        }
        public decimal RegisteredWHTAmountValUsd
        {
            get
            {
                return RegisteredWHTAmountUsd ?? 0;
            }
        }

       
        public decimal UnpaidBalanceVal
        {
            get
            {
                return (InvoiceAmountVal + VatAmountVal) - (PaidAmountIncVatVal + RegisteredWHTAmountVal);
            }
        }
        public decimal UnpaidBalanceValUsd
        {
            get
            {
                return (InvoiceAmountValUsd + VatAmountValUsd) - (PaidAmountIncVatValUsd + RegisteredWHTAmountValUsd);
            }
        }


        //public decimal UnpaidBalanceLocalVal
        //{
        //    get
        //    {
        //        ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        return hand.ConvertCurrencyPrice(UnpaidBalanceVal, UnpaidTotalBalanceCurrencyType, CurrencyUtil.C_CURRENCY_LOCAL);
        //    }
        //}
        //public decimal UnpaidBalanceUsdVal
        //{
        //    get
        //    {
        //        ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        return hand.ConvertCurrencyPrice(UnpaidBalanceVal, UnpaidTotalBalanceCurrencyType, CurrencyUtil.C_CURRENCY_US);
        //    }
        //}

        public string LocalCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(CurrencyUtil.C_CURRENCY_LOCAL);
            }
        }
        public string USCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(CurrencyUtil.C_CURRENCY_US);
            }
        }


        public string UnpaidTotalBalanceShow
        {
            get
            {
                List<string> texts = new List<string>();
                string unpaidBalanceLocal = string.Format("{0} {1}", LocalCurrencyTypeName, UnpaidBalanceVal.ToString("N2"));
                string unpaidBalanceUsd = string.Format("{0} {1}", USCurrencyTypeName, UnpaidBalanceValUsd.ToString("N2"));

                if (UnpaidBalanceVal > 0)
                    texts.Add(unpaidBalanceLocal);
                if(UnpaidBalanceValUsd > 0)
                    texts.Add(unpaidBalanceUsd);

                return string.Join("<br />", texts);
            }
        }

        public string BillingClientName
        {
            get
            {
                string currentLang = CommonUtil.GetCurrentLanguage();
                if (currentLang == CommonValue.DEFAULT_LANGUAGE_EN)
                {
                    return BillingClientNameEN;
                }
                else if (currentLang == CommonValue.DEFAULT_LANGUAGE_JP)
                {
                    return BillingClientNameEN;
                }
                else
                {
                    return BillingClientNameLC;
                }
            }
        }
        public string BillingClientNameGrid
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}", this.BillingClientNameEN, this.BillingClientNameLC);
            }
        }
        public string BillingTargetNoShortFormat
        {
            get
            {
                return new CommonUtil().ConvertBillingCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string UnpaidTotalBalanceCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.UnpaidTotalBalanceCurrencyType);
            }
        }
    }
}
