using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.DataEntity.Common;
using SECOM_AJIS.Common.Util.ConstantValue;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Function for retrieving money collection management information.
    /// </summary>
    public partial class doGetMoneyCollectionManagementInfo
    {
        [LanguageMapping]
        public string CollectionAreaName { get; set; }

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
                return string.Format("{0} {1}", ReceiptAmountCurrencyTypeName, ReceiptAmountVal.HasValue ? ReceiptAmountVal.Value.ToString("N2") : "0.00");
            }
        }
    }
}
