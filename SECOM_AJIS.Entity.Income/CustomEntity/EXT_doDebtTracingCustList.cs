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
    /// Data ojbect of debt tracing billing target list
    /// </summary>
    public partial class doDebtTracingCustList
    {
        [LanguageMapping]
        public string BillingOfficeName
        {
            get;
            set;
        }

        public string BillingOfficeDisplay
        {
            get
            {
                return CommonUtil.TextCodeName(this.BillingOfficeCode, this.BillingOfficeName);
            }
        }

        [LanguageMapping]
        public string BillingClientName
        {
            get;
            set;
        }

        public string BillingClientDisplay
        {
            get
            {
                string shortcode = new CommonUtil().ConvertBillingClientCode(this.BillingClientCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
                return CommonUtil.TextCodeName(shortcode, this.BillingClientName);
            }
        }

        public string BillingTargetCodeShort
        {
            get
            {
                return new CommonUtil().ConvertBillingTargetCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }
        
        [LanguageMapping]
        public string DebtTracingStatusDesc
        {
            get;
            set;
        }

        public string ContractAndInvoiceCount
        {
            get
            {
                return string.Format("{0} / {1} / {2}"
                    , (this.ContractCount ?? 0)
                    , (this.InvoiceOverDueCount ?? 0)
                    , (this.InvoiceNotDueCount ?? 0)
                );
            }
        }

        // Add by Jirawat Jannet on 2016-10-31
        //public string AmountCurrencyTypeName
        //{
        //    get
        //    {
        //        ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        return hand.getCurrencyName(this.AmountCurrencyType);
        //    }
        //}
        // Add by Jirawat Jannet on 2016-10-31
        //public decimal? AmountVal
        //{
        //    get
        //    {
        //        if (AmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return Amount;
        //        else if (AmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return AmountUsd;
        //        else return null;
        //    }
        //}
        // Add by Jirawat Jannet on 2016-10-31
        public string AmountShow
        {
            get
            {
                return string.Format("{0} {1}<br />{2} {3}"
                    , CurrencyLocalName, Amount.HasValue ? Amount.Value.ToString("N2") : "0.00"
                    , CurrencyUsName, AmountUsd.HasValue ? AmountUsd.Value.ToString("N2") : "0.00");
            }
        }
        // Add by Jirawat Jannet on 2016-10-31
        //public string UnpaidAmountCurrencyTypeName
        //{
        //    get
        //    {
        //        ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
        //        return hand.getCurrencyName(this.UnpaidAmountCurrencyType);
        //    }
        //}
        // Add by Jirawat Jannet on 2016-10-31
        //public decimal? UnpaidAmountVal
        //{
        //    get
        //    {
        //        if (UnpaidAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL) return UnpaidAmount;
        //        else if (UnpaidAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return UnpaidAmountUsd;
        //        else return null;
        //    }
        //}
        // Add by Jirawat Jannet on 2016-10-31
        public string UnpaidAmountShow
        {
            get
            {
                return string.Format("{0} {1}<br />{2} {3}"
                    , CurrencyLocalName, UnpaidAmount.HasValue ? UnpaidAmount.Value.ToString("N2") : "0.00"
                    , CurrencyUsName, UnpaidAmountUsd.HasValue ? UnpaidAmountUsd.Value.ToString("N2") : "0.00");
            }
        }

        // Add by Jirawat Jannet on 2016-11-08
        public string CurrencyLocalName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(CurrencyUtil.C_CURRENCY_LOCAL);
            }
        }
        // Add by Jirawat Jannet on 2016-11-08
        public string CurrencyUsName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(CurrencyUtil.C_CURRENCY_US);
            }
        }
    }
}
