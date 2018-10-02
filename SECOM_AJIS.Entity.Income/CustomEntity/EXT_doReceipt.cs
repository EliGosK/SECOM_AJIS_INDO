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
    /// Data object of receipt information.
    /// </summary>
    public partial class doReceipt
    {
        public string SECOMBankFullName { get; set; }
        public bool AdvanceReceiptStatusFlag
        {
            get
            {
                if (this.AdvanceReceiptStatus == SECOM_AJIS.Common.Util.ConstantValue.AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_ISSUED
                    || this.AdvanceReceiptStatus == SECOM_AJIS.Common.Util.ConstantValue.AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_REGISTERED
                    || this.AdvanceReceiptStatus == SECOM_AJIS.Common.Util.ConstantValue.AdvanceReceiptStatus.C_INC_ADVANCE_RECEIPT_STATUS_PAID )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        //Display with format
        public string ReceiptAmountString
        {
            get
            {
                return string.Format("{0}", this.ReceiptAmountVal.HasValue ? this.ReceiptAmountVal.Value.ToString("0,0.00") : "0.00");
            }
        }
        public decimal? ReceiptAmountVal
        {
            get
            {
                if (ReceiptAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return ReceiptAmount;
                else if (ReceiptAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return ReceiptAmountUsd;
                else return null;
            }
        }
        public string ReceiptAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.ReceiptAmountCurrencyType);
            }
        }
        public string ReceiptAmountShow
        {
            get
            {
                return string.Format("{0} {1}", ReceiptAmountCurrencyTypeName, ReceiptAmountString);
            }
        }
        public string ReceiptDateDisplay
        {
            get
            {
                return CommonUtil.TextDate(this.ReceiptDate);
            }
        }
        public string PaymentDateDisplay
        {
            get
            {
                return CommonUtil.TextDate(this.PaymentDate);
            }
        }
        public string PaymentMatchingDateDisplay
        {
            get
            {
                return CommonUtil.TextDate(this.PaymentMatchingDate);
            }
        }


        private string _BillingTargetCodeShort;
        public string BillingTargetCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertBillingClientCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
            set { _BillingTargetCodeShort = value; }
        }
    }
}
