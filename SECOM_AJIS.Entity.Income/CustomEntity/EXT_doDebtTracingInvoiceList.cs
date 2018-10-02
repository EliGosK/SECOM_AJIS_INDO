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
    /// Data ojbect of debt tracing invoice list
    /// </summary>
    public partial class doDebtTracingInvoiceList
    {
        [LanguageMapping]
        public string PaymentMethodName
        {
            get;
            set;
        }

        [LanguageMapping]
        public string IsOverDueDesc { get; set; }

        // Add by Jirawat Jannet on 2016-10-31
        public string InvoiceAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.InvoiceAmountCurrencyType);
            }
        }
        // Add by Jirawat Jannet on 2016-10-31
        public decimal? InvoiceAmountVal
        {
            get
            {
                if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return InvoiceAmount;
                else if (InvoiceAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return InvoiceAmountUsd;
                else return null;
            }
        }
        // Add by Jirawat Jannet on 2016-10-31
        public string InvoiceAmountShow
        {
            get
            {
                return string.Format("{0} {1}", InvoiceAmountCurrencyTypeName, InvoiceAmountVal.HasValue ? InvoiceAmountVal.Value.ToString("N2") : "0.00");
            }
        }

        // Add by Jirawat Jannet on 2016-10-31
        public string InvoiceUnpaidCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.InvoiceUnpaidCurrencyType);
            }
        }
        // Add by Jirawat Jannet on 2016-10-31
        public decimal? InvoiceUnpaidAmountVal
        {
            get
            {
                if (InvoiceUnpaidCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return InvoiceUnpaidAmount;
                else if (InvoiceUnpaidCurrencyType == CurrencyUtil.C_CURRENCY_US) return InvoiceUnpaidAmountUsd;
                else return null;
            }
        }
        // Add by Jirawat Jannet on 2016-10-31
        public string InvoiceUnpaidAmountShow
        {
            get
            {
                return string.Format("{0} {1}", InvoiceUnpaidCurrencyTypeName, InvoiceUnpaidAmountVal.HasValue ? InvoiceUnpaidAmountVal.Value.ToString("N2") : "0.00");
            }
        }
    }
}
