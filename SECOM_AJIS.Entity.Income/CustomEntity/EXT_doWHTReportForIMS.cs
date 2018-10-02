using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Common;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Function for retrieving debt target data.
    /// </summary>
    public partial class doWHTReportForIMS
    {
        public string CustomerNameAndTaxID
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}"
                    , (string.IsNullOrEmpty(this.CustomerName) ? "-" : this.CustomerName)
                    , (string.IsNullOrEmpty(this.TaxID) ? "-" : this.TaxID)
                );
            }
        }
        public string InvoiceNoAndTaxInvoiceNo
        {
            get
            {
                return string.Format("(1) {0}<br/>(2) {1}"
                    , (string.IsNullOrEmpty(this.InvoiceNo) ? "-" : this.InvoiceNo)
                    , (string.IsNullOrEmpty(this.TaxInvoiceNo) ? "-" : this.TaxInvoiceNo)
                );
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
        public decimal? ReceivedWHTAmountVal
        {
            get
            {
                if (PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return ReceivedWHTAmount;
                else if (PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return ReceivedWHTAmountUsd;
                else return null;
            }
        }
        public string ReceivedWHTAmountShow
        {
            get
            {
                return string.Format("{0} {1}", PaymentAmountCurrencyTypeName, ReceivedWHTAmountVal.HasValue ? ReceivedWHTAmountVal.Value.ToString("N2") : "0.00");
            }
        }
    }
}