using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using CSI.WindsorHelper;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of import payment content information.
    /// </summary>
    public partial class  tbt_tmpImportContent
    {
        public string ImportErrorReason { get; set; }
        public string GridPaymentDateDisplay
        {
            get
            {
                return CommonUtil.TextDate(this.PaymentDate);
            }
        }
        public string GridAutoTransferResultDisplay
        {
            get
            {
                if (this.AutoTransferResult == SECOM_AJIS.Common.Util.ConstantValue.AutoTransferResult.C_AUTO_TRANSFER_RESULT_FAIL)
                    return AutoTransferResultWord.C_AUTO_TRANSFER_RESULT_WORD_FAIL;
                else if (this.AutoTransferResult == SECOM_AJIS.Common.Util.ConstantValue.AutoTransferResult.C_AUTO_TRANSFER_RESULT_OK)
                    return AutoTransferResultWord.C_AUTO_TRANSFER_RESULT_WORD_OK;
                else
                    return string.Empty;
            }
        }

        public string PaymentAmountShow
        {
            get
            {
                if (PaymentAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return string.Format("{0} {1}", PaymentAmountCurrencyTypeName, PaymentAmount?.ToString("N2"));
                else
                    return string.Format("{0} {1}", PaymentAmountCurrencyTypeName, PaymentAmountUsd?.ToString("N2"));
            }
        }

        public string WHTAmountShow
        {
            get
            {
                if (WHTAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return string.Format("{0} {1}", WHTAmountCurrencyTypeName, WHTAmount?.ToString("N2"));
                else
                    return string.Format("{0} {1}", WHTAmountCurrencyTypeName, WHTAmountUsd?.ToString("N2"));
            }
        }

        public string PaymentAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(PaymentAmountCurrencyType);
            }
        }
        public string WHTAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(WHTAmountCurrencyType);
            }
        }
    }
}
