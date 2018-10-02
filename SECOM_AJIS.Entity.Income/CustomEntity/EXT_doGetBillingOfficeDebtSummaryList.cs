using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using SECOM_AJIS.Common;
using SECOM_AJIS.Common.Util;
using SECOM_AJIS.Common.Util.ConstantValue;
using SECOM_AJIS.Common.CustomAttribute;

namespace SECOM_AJIS.DataEntity.Income
{
    /// <summary>
    /// Data object of Function for retrieving debt summary data of each billing office.
    /// </summary>
    public partial class doGetBillingOfficeDebtSummary
    {
        [LanguageMapping]
        public string BillingOfficeName { get; set; }

        public string UnpaidAmountString
        {
            get
            {
                return string.Format("{0}", this.UnpaidAmount.HasValue ? this.UnpaidAmount.Value.ToString("#,##0.00") : "0.00");
            }
        }
        public string UnpaidAmountUsdString
        {
            get
            {
                return string.Format("{0}", this.UnpaidAmountUsd.HasValue ? this.UnpaidAmountUsd.Value.ToString("#,##0.00") : "0.00");
            }
        }

        public string UnpaidAmount2MonthString
        {
            get
            {
                return string.Format("{0}", this.UnpaidAmount2Month.HasValue ? this.UnpaidAmount2Month.Value.ToString("#,##0.00") : "0.00");
            }
        }
        public string UnpaidAmount2MonthUsdString
        {
            get
            {
                return string.Format("{0}", this.UnpaidAmount2MonthUsd.HasValue ? this.UnpaidAmount2MonthUsd.Value.ToString("#,##0.00") : "0.00");
            }
        }

        public string UnpaidAmount6MonthString
        {
            get
            {
                return string.Format("{0}", this.UnpaidAmount6Month.HasValue ? this.UnpaidAmount6Month.Value.ToString("#,##0.00") : "0.00");
            }
        }
        public string UnpaidAmount6MonthUsdString
        {
            get
            {
                return string.Format("{0}", this.UnpaidAmount6MonthUsd.HasValue ? this.UnpaidAmount6MonthUsd.Value.ToString("#,##0.00") : "0.00");
            }
        }


        public string UnpaidDetailString
        {
            get
            {
                return string.Format("{0}", this.UnpaidDetail.HasValue ? this.UnpaidDetail.Value.ToString("#,##0") : "0");
            }
        }
        public string UnpaidDetailUsdString
        {
            get
            {
                return string.Format("{0}", this.UnpaidDetailUsd.HasValue ? this.UnpaidDetailUsd.Value.ToString("#,##0") : "0");
            }
        }

        public string UnpaidDetail2MonthString
        {
            get
            {
                return string.Format("{0}", this.UnpaidDetail2Month.HasValue ? this.UnpaidDetail2Month.Value.ToString("#,##0") : "0");
            }
        }
        public string UnpaidDetail2MonthUsdString
        {
            get
            {
                return string.Format("{0}", this.UnpaidDetail2MonthUsd.HasValue ? this.UnpaidDetail2MonthUsd.Value.ToString("#,##0") : "0");
            }
        }

        public string UnpaidDetail6MonthString
        {
            get
            {
                return string.Format("{0}", this.UnpaidDetail6Month.HasValue ? this.UnpaidDetail6Month.Value.ToString("#,##0") : "0");
            }
        }
        public string UnpaidDetail6MonthUsdString
        {
            get
            {
                return string.Format("{0}", this.UnpaidDetail6MonthUsd.HasValue ? this.UnpaidDetail6MonthUsd.Value.ToString("#,##0") : "0");
            }
        }

        public string OldestDelayMonthString
        {
            get
            {
                return string.Format("{0}", this.OldestDelayMonth.HasValue ? this.OldestDelayMonth.Value.ToString("#,##0") : "0");
            }
        }
        public string OldestDelayMonthUsdString
        {
            get
            {
                return string.Format("{0}", this.OldestDelayMonthUsd.HasValue ? this.OldestDelayMonthUsd.Value.ToString("#,##0") : "0");
            }
        }

        public string TargetAmountAllString
        {
            get
            {
                return string.Format("{0}", this.TargetAmountAll.HasValue ? this.TargetAmountAll.Value.ToString("#,##0.00") : "0.00");
            }
        }
        public string TargetAmountAllUsdString
        {
            get
            {
                return string.Format("{0}", this.TargetAmountAllUsd.HasValue ? this.TargetAmountAllUsd.Value.ToString("#,##0.00") : "0.00");
            }
        }

        public string TargetAmount2MonthString
        {
            get
            {
                return string.Format("{0}", this.TargetAmount2Month.HasValue ? this.TargetAmount2Month.Value.ToString("#,##0.00") : "0.00");
            }
        }
        public string TargetAmount2MonthUsdString
        {
            get
            {
                return string.Format("{0}", this.TargetAmount2MonthUsd.HasValue ? this.TargetAmount2MonthUsd.Value.ToString("#,##0.00") : "0.00");
            }
        }

        public string TargetDetailAllString
        {
            get
            {
                return string.Format("{0}", this.TargetDetailAll.HasValue ? this.TargetDetailAll.Value.ToString("#,##0") : "0");
            }
        }
        public string TargetDetailAllUsdString
        {
            get
            {
                return string.Format("{0}", this.TargetDetailAllUsd.HasValue ? this.TargetDetailAllUsd.Value.ToString("#,##0") : "0");
            }
        }

        public string TargetDetail2MonthString
        {
            get
            {
                return string.Format("{0}", this.TargetDetail2Month.HasValue ? this.TargetDetail2Month.Value.ToString("#,##0") : "0");
            }
        }
        public string TargetDetail2MonthUsdString
        {
            get
            {
                return string.Format("{0}", this.TargetDetail2MonthUsd.HasValue ? this.TargetDetail2MonthUsd.Value.ToString("#,##0") : "0");
            }
        }

        public string AmountCompareToTargetAllString
        {
            get
            {
                return string.Format("{0}", this.AmountCompareToTargetAll.HasValue ? this.AmountCompareToTargetAll.Value.ToString("#,##0.00") : "0.00");
            }
        }
        public string AmountCompareToTargetAllUsdString
        {
            get
            {
                return string.Format("{0}", this.AmountCompareToTargetAllUsd.HasValue ? this.AmountCompareToTargetAllUsd.Value.ToString("#,##0.00") : "0.00");
            }
        }

        public string AmountCompareToTarget2MonthString
        {
            get
            {
                return string.Format("{0}", this.AmountCompareToTarget2Month.HasValue ? this.AmountCompareToTarget2Month.Value.ToString("#,##0.00") : "0.00");
            }
        }
        public string AmountCompareToTarget2MonthUsdString
        {
            get
            {
                return string.Format("{0}", this.AmountCompareToTarget2MonthUsd.HasValue ? this.AmountCompareToTarget2MonthUsd.Value.ToString("#,##0.00") : "0.00");
            }
        }

        public string DetailCompareToTargetAllString
        {
            get
            {
                return string.Format("{0}", this.DetailCompareToTargetAll.HasValue ? this.DetailCompareToTargetAll.Value.ToString("#,##0.0") : "0.0");
            }
        }
        public string DetailCompareToTargetAllUsdString
        {
            get
            {
                return string.Format("{0}", this.DetailCompareToTargetAllUsd.HasValue ? this.DetailCompareToTargetAllUsd.Value.ToString("#,##0.0") : "0.0");
            }
        }

        public string DetailCompareToTarget2MonthString
        {
            get
            {
                return string.Format("{0}", this.DetailCompareToTarget2Month.HasValue ? this.DetailCompareToTarget2Month.Value.ToString("#,##0.0") : "0.0");
            }
        }
        public string DetailCompareToTarget2MonthUsdString
        {
            get
            {
                return string.Format("{0}", this.DetailCompareToTarget2MonthUsd.HasValue ? this.DetailCompareToTarget2MonthUsd.Value.ToString("#,##0.0") : "0.0");
            }
        }

        //Add by Jutarat A. on 05032012
        public int GroupTotal { get; set; }

        public string TargetAmountAllShow { get { return CommonUtil.TextNumeric(TargetAmountAll > 0 ? ((UnpaidAmount * 100) / TargetAmountAll) : null); } }
        public string TargetAmountAllUsdShow { get { return CommonUtil.TextNumeric(TargetAmountAllUsd > 0 ? ((UnpaidAmountUsd * 100) / TargetAmountAllUsd) : null); } }
        public string TargetDetailAllShow { get { return CommonUtil.TextNumeric(TargetDetailAll > 0 ? ((UnpaidDetail * 100) / TargetDetailAll) : null); } }
        public string TargetDetailAllUsdShow { get { return CommonUtil.TextNumeric(TargetDetailAllUsd > 0 ? ((UnpaidDetailUsd * 100) / TargetDetailAllUsd) : null); } }
        public string TargetAmount2MonthShow { get { return CommonUtil.TextNumeric(TargetAmount2Month > 0 ? ((UnpaidAmount2Month * 100) / TargetAmount2Month) : null); } }
        public string TargetAmount2MonthUsdShow { get { return CommonUtil.TextNumeric(TargetAmount2MonthUsd > 0 ? ((UnpaidAmount2MonthUsd * 100) / TargetAmount2MonthUsd) : null); } }
        public string TargetDetail2MonthShow { get { return CommonUtil.TextNumeric(TargetDetail2Month > 0 ? ((UnpaidDetail2Month * 100) / TargetDetail2Month) : null); } }
        public string TargetDetail2MonthUsdShow { get { return CommonUtil.TextNumeric(TargetDetail2MonthUsd > 0 ? ((UnpaidDetail2MonthUsd * 100) / TargetDetail2MonthUsd) : null); } }

        public string BillingOffice { get { return string.Format("{0}<br> {1}", BillingOfficeCode, BillingOfficeName); } }
        public string AllUnpaidActual { get { return string.Format("{0}<br>{1}", UnpaidAmountString, UnpaidDetailString); } }
        public string AllUnpaidTarget { get { return string.Format("{0}<br>{1}", TargetAmountAllString, TargetDetailAllString); } }
        public string AllUnpaidCompareTotarget { get { return string.Format("{0}<br>{1}", TargetAmountAllShow, TargetDetailAllShow); } }
        public string UnpaidOver2MonthActual { get { return string.Format("{0}<br>{1}", UnpaidAmount2MonthString, UnpaidDetail2MonthString); } }
        public string UnpaidOver2MonthTarget { get { return string.Format("{0}<br>{1}", TargetAmount2MonthString, TargetDetail2MonthString); } }
        public string UnpaidOver2MonthCompareTotarget { get { return string.Format("{0}<br>{1}", TargetAmount2MonthShow, TargetDetail2MonthShow); } }
        public string UnpaidOver6Month { get { return string.Format("{0}<br>{1}", UnpaidAmount6MonthString, UnpaidDetail6MonthString); } }  
    
        public string DisableLinkOfficeFlag { get; set; }
        //End Add
    }
}
