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
    /// Data ojbect of debt tracing invoice detail
    /// </summary>
    public partial class doDebtTracingInvoiceDetail
    {
        [LanguageMapping]
        public string BillingTypeName
        {
            get;
            set;
        }

        public string ContractCodeShort
        {
            get
            {
                return new CommonUtil().ConvertContractCode(this.ContractCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        public string BillingAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.BillingAmountCurrencyType);
            }
        }
        public decimal? BillingAmountVal
        {
            get
            {
                if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return BillingAmount;
                else if (BillingAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return BillingAmountUsd;
                else return null;
            }
        }
        public string BillingAmountShow
        {
            get
            {
                return string.Format("{0} {1}", BillingAmountCurrencyTypeName, BillingAmountVal.HasValue ? BillingAmountVal.Value.ToString("N2") : "0.00");
            }
        }
    }
}
