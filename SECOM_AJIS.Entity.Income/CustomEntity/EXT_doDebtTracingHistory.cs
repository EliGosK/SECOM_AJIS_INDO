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
    /// Data ojbect of debt tracing history list
    /// </summary>
    public partial class doDebtTracingHistory
    {
        [LanguageMapping]
        public string CallResultDesc
        {
            get;
            set;
        }

        [LanguageMapping]
        public string PaymentMethodDesc
        {
            get;
            set;
        }

        [LanguageMapping]
        public string PostponeReasonDesc
        {
            get;
            set;
        }

        [LanguageMapping]
        public string DebtTracingStatusDesc
        {
            get;
            set;
        }
        // add by jirawat jannet on 2016-10-31
        public string EstimatedAmountCurrencyTypeName
        {
            get
            {
                //ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //return hand.getCurrencyName(this.EstimatedAmountCurrencyType);
                if (EstimatedAmount >=0 ) return "Rp. " + EstimatedAmount.Value.ToString("#.##0.00");
                else return "Rp. 0";
            }
        }
        public string EstimatedAmountCurrencyTypeNameUsd
        {
            get
            {
                //ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                //return hand.getCurrencyName(this.EstimatedAmountCurrencyType);
                if (EstimatedAmountUsd >= 0) return "US$ " + EstimatedAmountUsd.Value.ToString("#.##0.00");
                else return "US$ 0";
            }
        }
        // add by jirawat jannet on 2016-10-31
        public decimal? EstimatedAmountVal
        {
            get
            {
                if (EstimatedAmountCurrencyType== CurrencyUtil.C_CURRENCY_LOCAL) return EstimatedAmount;
                else if (EstimatedAmountCurrencyType == CurrencyUtil.C_CURRENCY_US) return EstimatedAmountUsd;
                else return null;
            }
        }
        public string PaymentDetail
        {
            get
            {
                return string.Format(
                    "(1) {0}<br/>(2) {1}<br/>     {2}<br/>(3) {3}" // edit by jirawat jannet on 2016-10-31
                    , this.EstimatedDate.HasValue ? this.EstimatedDate.Value.ToString("dd-MMM-yyyy") : ""
                    , this.EstimatedAmountCurrencyTypeName // add by jirawat jannet on 2016-10-31
                    , this.EstimatedAmountCurrencyTypeNameUsd 
                                                           /* , this.EstimatedAmountVal.HasValue ? this.EstimatedAmountVal.Value.ToString("#,##0.00") : ""*/
                    , this.PaymentMethodDesc
                );
            }
        }

        [LanguageMapping]
        public string EmpFirstName { get; set; }

        [LanguageMapping]
        public string EmpLastName { get; set; }

        public string CallDateAndName
        {
            get
            {
                return string.Format(
                    "(1) {0}<br/>(2) {1} {2}<br/>(3) {3}"
                    , this.CallDate.ToString("dd-MMM-yyyy")
                    , this.EmpFirstName, this.EmpLastName, this.ServiceTypeDesc
                );
            }
        }

        [LanguageMapping]
        public string ServiceTypeDesc { get; set; }

    }
}
