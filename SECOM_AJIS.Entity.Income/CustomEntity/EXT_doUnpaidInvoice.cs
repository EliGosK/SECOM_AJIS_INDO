using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Unpaid invoice information.
    /// </summary>
    public partial class doUnpaidInvoice
    {
        //public string PaidAmountCurrencyTypeName
        //{
        //    get
        //    {
        //        ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        return hand.getCurrencyName(this.PaidAmountCurrencyType);
        //    }
        //}

        public string BillingAmountIncTaxShow
        {
            get
            {
                if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && PaidAmountCurrencyType  == CurrencyUtil.C_CURRENCY_US)
                    return "-";
                else
                    return string.Format("{0} {1}", InvoiceAmountCurrencyTypeName, BillingAmountIncTax.ToString("N2"));
            }
        }
        public decimal BillingAmountIncTax
        {
            get
            {
                double errorCode = 0;
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                decimal invoiceAmountValue = hand.ConvertCurrencyPrice(InvoiceAmountValue, InvoiceAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                decimal vatAmountValue = hand.ConvertCurrencyPrice(VatAmountValue, VatAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;

                return invoiceAmountValue + vatAmountValue;
            }
        }

        public decimal BillingAmountIncTaxPaidCurrency
        {
            get
            {
                double errorCode = 0;
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.ConvertCurrencyPrice(BillingAmountIncTax, InvoiceAmountCurrencyType, PaidAmountCurrencyType, DateTime.Now, ref errorCode);
            }
        }

        public decimal UnpaidAmount
        {
            get
            {
                return (this.InvoiceAmount ?? 0) + (this.VatAmount ?? 0)
                    - (this.RegisteredWHTAmount ?? 0) - (this.PaidAmount ?? 0);
            }
        }
        public string UnpaidAmountCurrencyType
        {
            get
            {
                return PaidAmountCurrencyType;
                //if (PaidAmountCurrencyType != null) return PaidAmountCurrencyType;
                //else return InvoiceAmountCurrencyType;
            }
        }

        public decimal UnpaidAmountUsd
        {
            get
            {
                return (this.InvoiceAmountUsd ?? 0) + (this.VatAmountUsd ?? 0)
                    - (this.RegisteredWHTAmountUsd ?? 0) - (this.PaidAmountUsd ?? 0);
            }
        }
        public decimal PaidAmountDisplayOnGrid
        { 
            get
            {
                double errorCode = 0;
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                decimal paidAmountValue = 0;
                decimal registeredWHTAmountValue = 0;

                if (PaidAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    paidAmountValue = hand.ConvertCurrencyPrice(PaidAmountUsd, PaidAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                else
                    paidAmountValue = hand.ConvertCurrencyPrice(PaidAmount, PaidAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;

                if (RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    registeredWHTAmountValue = hand.ConvertCurrencyPrice(RegisteredWHTAmountUsd, RegisteredWHTAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                else
                    registeredWHTAmountValue = hand.ConvertCurrencyPrice(RegisteredWHTAmount, RegisteredWHTAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;


                return hand.ConvertCurrencyPrice((paidAmountValue + registeredWHTAmountValue), InvoiceAmountCurrencyType, PaidAmountCurrencyType, DateTime.Now, ref errorCode);
            }
        }

        public decimal? KeyInMatchAmountIncWHT { get; set; }
        
        public decimal? KeyInWHTAmount { get; set; }

        public bool IsToMatchableProcess { get; set; }

        //Add by Jutarat A. on 17042013
        public decimal MatchAmountIncWHT
        {
            get
            {
                double errorCode = 0;
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (string.IsNullOrEmpty(PaidAmountCurrencyType))
                    PaidAmountCurrencyType = InvoiceAmountCurrencyType;

                decimal invoiceAmount = hand.ConvertCurrencyPrice(InvoiceAmountValue, InvoiceAmountCurrencyType, PaidAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                decimal vatAmount = hand.ConvertCurrencyPrice(VatAmountValue, VatAmountCurrencyType, PaidAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                decimal paidAmountValue = hand.ConvertCurrencyPrice(PaidAmountValue, PaidAmountCurrencyType, PaidAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                decimal registeredWHTAmountValue = hand.ConvertCurrencyPrice(RegisteredWHTAmountValue, RegisteredWHTAmountCurrencyType, PaidAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;


                return invoiceAmount + vatAmount - paidAmountValue - registeredWHTAmountValue;

                //return (this.InvoiceAmountValue ?? 0) + (this.VatAmountValue ?? 0) - (this.PaidAmountValue ?? 0) - (this.RegisteredWHTAmountValue ?? 0);
            }
        }
        //End Add

        //Add by Jutarat A. on 26042013
        public decimal WHTAmountDefault
        {
            get
            {
                return ((this.WHTAmountValue ?? 0) - (this.RegisteredWHTAmountValue ?? 0)) < 0 ? 0 : ((this.WHTAmountValue ?? 0) - (this.RegisteredWHTAmountValue ?? 0));
            }
        }
        //End Add

        // Add by Jirawat Jannet @ 2016-10-27

        public decimal? InvoiceAmountValue
        {
            get
            {
                if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return InvoiceAmount;
                else if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return InvoiceAmountUsd;
                else return null;
            }
        }
        public decimal? VatAmountValue
        {
            get
            {
                if (VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return VatAmount;
                else if (VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return VatAmountUsd;
                else return null;
            }
        }
        public decimal? WHTAmountValue
        {
            get
            {
                if (WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return WHTAmount;
                else if (WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return WHTAmountUsd;
                else return null;
            }
        }
        public decimal? PaidAmountValue
        {
            get
            {
                if (PaidAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return PaidAmount;
                else if (PaidAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return PaidAmountUsd;
                else return null;
            }
        }
        public decimal? RegisteredWHTAmountValue
        {
            get
            {
                if (RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return RegisteredWHTAmount;
                else if (RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return RegisteredWHTAmountUsd;
                else return null;
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
        public string VatAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.VatAmountCurrencyType);
            }
        }
        public string WHTAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.WHTAmountCurrencyType);
            }
        }
        public string PaidAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.PaidAmountCurrencyType);
            }
        }
        public string RegisteredWHTAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.RegisteredWHTAmountCurrencyType);
            }
        }

        public string InvoiceAmountShow
        {
            get
            {
                if ((InvoiceAmountValue ?? 0) > 0)
                    return string.Format("{0} {1}", InvoiceAmountCurrencyTypeName
                        , InvoiceAmountValue.HasValue ? InvoiceAmountValue.Value.ToString("N2") : "0.00");
                else
                    return "0.00";
            }
        }
        public string VatAmountShow
        {
            get
            {
                if ((VatAmountValue ?? 0) > 0)
                    return string.Format("{0} {1}", VatAmountCurrencyTypeName, VatAmountValue.HasValue ? VatAmountValue.Value.ToString("N2") : "0.00");
                else return "0.00";
            }
        }
        public string WHTAmountShow
        {
            get
            {
                if ((WHTAmountValue ?? 0) > 0)
                    return string.Format("{0} {1}", WHTAmountCurrencyTypeName, WHTAmountValue.HasValue ? WHTAmountValue.Value.ToString("N2") : "0.00");
                else return "0.00";
            }
        }
        public string PaidAmountShow
        {
            get
            {
                if ((PaidAmountValue ?? 0) > 0)
                    return string.Format("{0} {1}", PaidAmountCurrencyTypeName, PaidAmountValue.HasValue ? PaidAmountValue.Value.ToString("N2") : "0.00");
                else return "0.00";
            }
        }
        public string RegisteredWHTAmountShow
        {
            get
            {
                if ((RegisteredWHTAmountValue ?? 0) > 0)
                    return string.Format("{0} {1}", RegisteredWHTAmountCurrencyTypeName, RegisteredWHTAmountValue.HasValue ? RegisteredWHTAmountValue.Value.ToString("N2") : "0.00");
                else return "0.00";
            }
        }
        public string PaidAmountDisplayOnGridShow
        {
            get
            {
                if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && PaidAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    return "-";
                else
                {
                    if (PaidAmountDisplayOnGrid > 0)
                        return string.Format("{0} {1}", PaidAmountCurrencyTypeName, PaidAmountDisplayOnGrid.ToString("N2"));
                    else return "0.00";
                }
            }
        }
        public string UnpaidAmountShow
        {
            get
            {
                double errorCode = 0;
                ICommonHandler comHandler = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL && PaidAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    return "-";
                else if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_US && PaidAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                {
                    string localStr = string.Format("{0} {1}", PaidAmountCurrencyTypeName, UnpaidAmountVal.ToString("N2"));
                    string usStr = string.Format("{0} {1}", comHandler.getCurrencyName(CurrencyUtil.C_CURRENCY_US)
                                                          , comHandler.ConvertCurrencyPrice(UnpaidAmountVal, PaidAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode).ToString("N2"));

                    return string.Format("{0}<br />{1}", localStr, usStr);
                }
                else
                {
                    if (UnpaidAmountVal > 0)
                        return string.Format("{0} {1}", InvoiceAmountCurrencyTypeName, UnpaidAmountVal.ToString("N2"));
                    else return "0.00";
                }
            }
        }

        public decimal UnpaidAmountVal
        {
            get
            {
                double errorCode = 0;
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;

                decimal invoiceAmount = 0;
                decimal vatAmount = 0;
                decimal registeredWHTAmount = 0;
                decimal paidAmount = 0;

                if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    invoiceAmount = hand.ConvertCurrencyPrice(InvoiceAmountUsd, InvoiceAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                else invoiceAmount = hand.ConvertCurrencyPrice(InvoiceAmount, InvoiceAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;

                if (VatAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    vatAmount = hand.ConvertCurrencyPrice(VatAmountUsd, VatAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                else vatAmount = hand.ConvertCurrencyPrice(VatAmount, VatAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;

                if (RegisteredWHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    registeredWHTAmount = hand.ConvertCurrencyPrice(RegisteredWHTAmountUsd, RegisteredWHTAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                else registeredWHTAmount = hand.ConvertCurrencyPrice(RegisteredWHTAmount, RegisteredWHTAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;

                if (PaidAmountCurrencyType == CurrencyUtil.C_CURRENCY_US)
                    paidAmount = hand.ConvertCurrencyPrice(PaidAmountUsd, PaidAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;
                else paidAmount = hand.ConvertCurrencyPrice(PaidAmount, PaidAmountCurrencyType, InvoiceAmountCurrencyType, DateTime.Now, ref errorCode, 0).Value;

                if (string.IsNullOrEmpty(PaidAmountCurrencyType)) PaidAmountCurrencyType = InvoiceAmountCurrencyType; // ถ้าไม่มีการจ่ายเงินมาก่อน ให้ใช้ currency ของ Invoice Amount

                return hand.ConvertCurrencyPrice((invoiceAmount + vatAmount - registeredWHTAmount - paidAmount), InvoiceAmountCurrencyType, PaidAmountCurrencyType, DateTime.Now, ref errorCode);
            }
        }

        // End add
    }
}
