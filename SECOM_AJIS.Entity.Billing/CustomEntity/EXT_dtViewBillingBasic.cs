using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.DataEntity.Billing.MetaData;

namespace SECOM_AJIS.DataEntity.Billing
{
    [MetadataType(typeof(dtViewBillingBasic_MetaData))]
    public partial class dtViewBillingBasic
    {
        [LanguageMapping]
        public string OfficeName { get; set; }

        [LanguageMapping]
        public string DebtTracingOfficeName { get; set; }

        [LanguageMapping]
        public string MethodName { get; set; }

        public string CustTypeCodeName { get; set; }
        public string StopBillingFlagName { get; set; }
        public string PaymentMethodName { get; set; }

        public string StopBillingFlagConvert
        {
            get
            {
                string StopBillingFlagConvert = "";
                if (this.StopBillingFlag.HasValue)
                {
                    StopBillingFlagConvert = this.StopBillingFlag.Value == true ? "1" : "0";
                }
                else
                {
                    StopBillingFlagConvert = "0";
                }

                return StopBillingFlagConvert;
            }
        }

        public string StopBillingFlagCodeName
        {
            get
            {
                string code = "";
                if (this.StopBillingFlag.HasValue)
                {
                    code = this.StopBillingFlag.Value == true ? "1" : "0";
                }
                else
                {
                    code = "0";
                }

                return CommonUtil.TextCodeName(code, this.StopBillingFlagName);
            }
        }

        public string DocAuditResultName { get; set; }

        public string CalDailyFeeStatusName { get; set; }

        public List<DataEntity.Common.doMiscTypeCode> Currencies { get; set; }
        public string TextTransferMonthlyBillingAmount
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.MonthlyBillingAmount);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.MonthlyBillingAmountCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferMonthlyFeeBeforeStop
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.MonthlyFeeBeforeStop);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.MonthlyFeeBeforeStopCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferBalanceDeposit
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.BalanceDeposit);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.BalanceDepositCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }

        public string TextTransferAdjustBillingPeriodAmount
        {
            get
            {
                string txt = CommonUtil.TextNumeric(this.AdjustBillingPeriodAmount);

                if (txt == "")
                    txt = "-";
                else
                {
                    if (this.Currencies != null)
                    {
                        DataEntity.Common.doMiscTypeCode curr = this.Currencies.Find(x => x.ValueCode == this.AdjustBillingPeriodAmountCurrencyType);
                        if (curr != null)
                            txt = string.Format("{0} {1}", curr.ValueDisplayEN, txt);
                    }
                }
                return txt;
            }
        }
    }    
}

namespace SECOM_AJIS.DataEntity.Billing.MetaData
{
    public class dtViewBillingBasic_MetaData
    {
        [CustTypeMappingAttribute("CustTypeCodeName")]
        public string CustTypeCode { get; set; }

        [StopBillingFlagMappingAttribute("StopBillingFlagName")]
        public string StopBillingFlagConvert { get; set; }

        [PaymentMethodMappingAttribute("PaymentMethodName")]
        public string PaymentMethod { get; set; }

        [DocAuditResultMappingAttribute("DocAuditResultName")]
        public string DocAuditResult { get; set; }

        [CalDailyFeeStatusMappingAttribute("CalDailyFeeStatusName")]
        public string CalDailyFeeStatus { get; set; }
    }
}