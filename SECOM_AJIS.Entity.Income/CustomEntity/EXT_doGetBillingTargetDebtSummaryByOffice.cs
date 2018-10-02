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
    /// Data object of Function for retrieving debt summary data of each billing target in a billing office.
    /// </summary>
    public partial class doGetBillingTargetDebtSummaryByOffice
    {
        public string ToJson
        {
            get
            {
                return CommonUtil.CreateJsonString(this);
            }
        }

        public string BillingTargetCodeShort
        {
            get
            {
                CommonUtil c = new CommonUtil();
                return c.ConvertBillingTargetCode(this.BillingTargetCode, CommonUtil.CONVERT_TYPE.TO_SHORT);
            }
        }


        public string UnpaidInvoiceString
        {
            get
            {
                return string.Format("{0}", this.UnpaidInvoice.HasValue ? this.UnpaidInvoice.Value.ToString("#,##0") : "0");
            }
        }
        public string UnpaidDetailString
        {
            get
            {
                return string.Format("{0}", this.UnpaidDetail.HasValue ? this.UnpaidDetail.Value.ToString("#,##0") : "0");
            }
        }

        public string IncludeFirstFeeGridFormat
        {
            get;
            set;
        }

        public string DebtTracingRegisteredString
        {
            get
            {
                return string.Format("{0}", this.DebtTracingRegistered == 1 ? "Yes" : "No");
            }
        }
        public string DebtTracingRegisteredGridFormat
        {
            get;
            set;
        }

        public string OldestDelayMonthString
        {
            get
            {
                return string.Format("{0}", this.OldestDelayMonth.HasValue ? this.OldestDelayMonth.Value.ToString("#,##0") : "0");
            }
        }

        public string OldestBillingTargetExpectedPaymentDateValueLessThanToday
        {
            get
            {
                if (this.OldestBillingTargetExpectedPaymentDate == null)
                {
                    return "N";
                }
                else if (this.OldestBillingTargetExpectedPaymentDate.Value.Date < DateTime.Now.Date)
                {
                    return "Y";
                }
                else
                {
                    return "N";
                }
            }
        }

        public string OldestInvoiceExpectedPaymentDateValueLessThanToday
        {
            get
            {
                if (this.OldestInvoiceExpectedPaymentDate == null)
                {
                    return "N";
                }
                else if (this.OldestInvoiceExpectedPaymentDate.Value.Date < DateTime.Now.Date)
                {
                    return "Y";
                }
                else
                {
                    return "N";
                }
            }
        }


        public string BillingClientNameGrid
        {
            get
            {
                return string.Format("(1) {0}<br />(2) {1}", this.BillingClientNameEN, this.BillingClientNameLC);
            }
        }

        public string OldestBillingTargetExpectedPaymentDateFlag
        {
            get
            {
                if (this.OldestBillingTargetExpectedPaymentDateValueLessThanToday == "Y")
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
        }

        public string OldestInvoiceExpectedPaymentDateFlag
        {
            get
            {
                if (this.OldestInvoiceExpectedPaymentDateValueLessThanToday == "Y")
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
        }

        
        public decimal? UnpaidAmountValue
        {
            get
            {
                if (UnpaidAmountCurrencyType == CurrencyUtil.C_CURRENCY_LOCAL)
                    return UnpaidAmount;
                else
                    return UnpaidAmountUsd;
            }
        }
        public string UnpaidAmountString
        {
            get
            {
                // Commen by jirawat janne ton 2016-11-17
                //return string.Format("{0} {1}", UnpaidAmountCurrencyTypeName, this.UnpaidAmountValue.HasValue ? this.UnpaidAmountValue.Value.ToString("#,##0.00") : "0.00");

                return string.Format("{0} {1}<br />{2} {3}"
                                       , LocalCurrencyTypeName, this.UnpaidAmount.HasValue ? this.UnpaidAmount.Value.ToString("N2") : "0.00"
                                       , USCurrencyTypeName, this.UnpaidAmountUsd.HasValue ? this.UnpaidAmountUsd.Value.ToString("N2") : "0.00" );
            }
        }

        public string LocalCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(CurrencyUtil.C_CURRENCY_LOCAL);
            }
        }

        public string USCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(CurrencyUtil.C_CURRENCY_US);
            }
        }

        public string UnpaidAmountCurrencyTypeName
        {
            get
            {
                ICommonHandler hand = CSI.WindsorHelper.ServiceContainer.GetService<ICommonHandler>() as ICommonHandler;
                return hand.getCurrencyName(this.UnpaidAmountCurrencyType);
            }
        }
    }
}
