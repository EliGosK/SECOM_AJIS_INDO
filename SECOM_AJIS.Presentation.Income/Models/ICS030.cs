using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SECOM_AJIS.Common.Models;
using SECOM_AJIS.DataEntity.Common;
using System.ComponentModel.DataAnnotations;

using SECOM_AJIS.Common.CustomAttribute;
using SECOM_AJIS.Common.Util;
//using SECOM_AJIS.DataEntity.Master;
using SECOM_AJIS.DataEntity.Income;
using SECOM_AJIS.Presentation.Income.Models.MetaData;
namespace SECOM_AJIS.Presentation.Income.Models
{
    /// <summary>
    /// Parameter of ICS030 screen
    /// </summary>
    public class ICS030_ScreenParameter : ScreenParameter
    {
        // send data back to client
        //[KeepSession]
        //public List<doGetMoneyCollectionManagementInfo> doGetMoneyCollectionManagementInfo { set; get; }

        public List<OfficeDataDo> doOfficeDataDo { set; get; }
        public List<doGetBillingOfficeDebtSummary> doGetBillingOfficeDebtSummaryList { set; get; }
        public List<ICS030_DebtActualTableData> doNewGetBillingOfficeDebtSummaryList { set; get; }

        //public List<doGetBillingTargetDebtSummaryByOffice> doGetBillingTargetDebtSummaryByOfficeList { set; get; }
        //public List<doGetUnpaidInvoiceDebtSummaryByBillingTarget> doGetUnpaidInvoiceDebtSummaryByBillingTargetList { set; get; }

        //public List<doGetUnpaidDetailDebtSummary> doGetUnpaidDetailDebtSummaryByBillingTargetList { set; get; }
        //public List<doGetUnpaidDetailDebtSummary> doGetUnpaidDetailDebtSummaryByInvoiceList { set; get; }
        //public List<doGetUnpaidDetailDebtSummary> doGetUnpaidDetailDebtSummaryByBillingCodeList { set; get; }

        // send data from client to server
        public ICS030_RegisterData RegisterData { set; get; }
        public string conYes { set; get; }
        public string conNo { set; get; }

        public doTotalBillingOfficeDebt doTotalBillingOfficeDebt { set; get; } //Add by Jutarat A. on 05032014
        public doTotalBillingOfficeDebt doTotalBillingOfficeDebtLocal { set; get; } //Add by Jirawat Jannet A. on 2016-10-10
        public doTotalBillingOfficeDebt doTotalBillingOfficeDebtUs { set; get; } //Add by Jirawat Jannet A. on 2016-10-10
    }

    // register com sent data to server
    /// <summary>
    /// DO for Register Data
    /// </summary>
    public class ICS030_RegisterData
    {
        public DateTime RawdtpMonthYear { set; get; }
        public int intMonth { set; get; }
        public int intYear { set; get; }

        public string strOfficeCode { set; get; }
        public string strOfficeName { set; get; }

        public string strBillingTargetCode { set; get; }

        public string strInvoiceNo { set; get; }
        public string strInvoiceOCC { set; get; }
    }


    //Add by Jutarat A. on 05032012
    public class doTotalBillingOfficeDebt
    {
        public string Currency { get; set; }
        public Nullable<decimal> TotalUnpaidAmount { get; set; }
        //public Nullable<decimal> TotalUnpaidAmountUsd { get; set; }
        public Nullable<decimal> TotalUnpaidAmount2Month { get; set; }
        //public Nullable<decimal> TotalUnpaidAmount2MonthUsd { get; set; }
        public Nullable<decimal> TotalUnpaidAmount6Month { get; set; }
        //public Nullable<decimal> TotalUnpaidAmount6MonthUsd { get; set; }
        public Nullable<int> TotalUnpaidDetail { get; set; }
        //public Nullable<int> TotalUnpaidDetailUsd { get; set; }
        public Nullable<int> TotalUnpaidDetail2Month { get; set; }
        //public Nullable<int> TotalUnpaidDetail2MonthUsd { get; set; }
        public Nullable<int> TotalUnpaidDetail6Month { get; set; }
        //public Nullable<int> TotalUnpaidDetail6MonthUsd { get; set; }
        public Nullable<decimal> TotalTargetAmountAll { get; set; }
        //public Nullable<decimal> TotalTargetAmountAllUsd { get; set; }
        public Nullable<decimal> TotalTargetAmount2Month { get; set; }
        //public Nullable<decimal> TotalTargetAmount2MonthUsd { get; set; }
        public Nullable<int> TotalTargetDetailAll { get; set; }
        //public Nullable<int> TotalTargetDetailAllUsd { get; set; }
        public Nullable<int> TotalTargetDetail2Month { get; set; }
        //public Nullable<int> TotalTargetDetail2MonthUsd { get; set; }

        public string TotalUnpaidAmountString { get { return CommonUtil.TextNumeric(TotalUnpaidAmount); } }
        //public string TotalUnpaidAmountUsdString { get { return CommonUtil.TextNumeric(TotalUnpaidAmountUsd); } }

        public string TotalUnpaidAmount2MonthString { get { return CommonUtil.TextNumeric(TotalUnpaidAmount2Month); } }
        //public string TotalUnpaidAmount2MonthUsdString { get { return CommonUtil.TextNumeric(TotalUnpaidAmount2MonthUsd); } }

        public string TotalUnpaidAmount6MonthString { get { return CommonUtil.TextNumeric(TotalUnpaidAmount6Month); } }
        //public string TotalUnpaidAmount6MonthUsdString { get { return CommonUtil.TextNumeric(TotalUnpaidAmount6MonthUsd); } }

        public string TotalUnpaidDetailString { get { return CommonUtil.TextNumeric(TotalUnpaidDetail); } }
        //public string TotalUnpaidDetailUsdString { get { return CommonUtil.TextNumeric(TotalUnpaidDetailUsd); } }

        public string TotalUnpaidDetail2MonthString { get { return CommonUtil.TextNumeric(TotalUnpaidDetail2Month); } }
        //public string TotalUnpaidDetail2MonthUsdString { get { return CommonUtil.TextNumeric(TotalUnpaidDetail2MonthUsd); } }

        public string TotalUnpaidDetail6MonthString { get { return CommonUtil.TextNumeric(TotalUnpaidDetail6Month); } }
        //public string TotalUnpaidDetail6MonthUsdString { get { return CommonUtil.TextNumeric(TotalUnpaidDetail6MonthUsd); } }

        public string TotalTargetAmountAllString { get { return CommonUtil.TextNumeric(TotalTargetAmountAll); } }
        //public string TotalTargetAmountAllUsdString { get { return CommonUtil.TextNumeric(TotalTargetAmountAllUsd); } }

        public string TotalTargetAmount2MonthString { get { return CommonUtil.TextNumeric(TotalTargetAmount2Month); } }
        //public string TotalTargetAmount2MonthUsdString { get { return CommonUtil.TextNumeric(TotalTargetAmount2MonthUsd); } }

        public string TotalTargetDetailAllString { get { return CommonUtil.TextNumeric(TotalTargetDetailAll); } }
        //public string TotalTargetDetailAllUsdString { get { return CommonUtil.TextNumeric(TotalTargetDetailAllUsd); } }

        public string TotalTargetDetail2MonthString { get { return CommonUtil.TextNumeric(TotalTargetDetail2Month); } }
        //public string TotalTargetDetail2MonthUsdString { get { return CommonUtil.TextNumeric(TotalTargetDetail2MonthUsd); } }


        public Nullable<decimal> TargetAmountAll { get { return TotalTargetAmountAll > 0 ? ((TotalUnpaidAmount * 100) / TotalTargetAmountAll) : null; } }
        //public Nullable<decimal> TargetAmountAllUsd { get { return TotalTargetAmountAllUsd > 0 ? ((TotalUnpaidAmountUsd * 100) / TotalTargetAmountAllUsd) : null; } }

        public Nullable<decimal> TargetDetailAll { get { return TotalTargetDetailAll > 0 ? ((TotalUnpaidDetail * 100) / TotalTargetDetailAll) : null; } }
        //public Nullable<decimal> TargetDetailAllUsd { get { return TotalTargetDetailAllUsd > 0 ? ((TotalUnpaidDetailUsd * 100) / TotalTargetDetailAllUsd) : null; } }

        public Nullable<decimal> TargetAmount2Month { get { return TotalTargetAmount2Month > 0 ? ((TotalUnpaidAmount2Month * 100) / TotalTargetAmount2Month) : null; } }
        //public Nullable<decimal> TargetAmount2MonthUsd { get { return TotalTargetAmount2MonthUsd > 0 ? ((TotalUnpaidAmount2MonthUsd * 100) / TotalTargetAmount2MonthUsd) : null; } }

        public Nullable<decimal> TargetDetail2Month { get { return TotalTargetDetail2Month > 0 ? ((TotalUnpaidDetail2Month * 100) / TotalTargetDetail2Month) : null; } }
        //public Nullable<decimal> TargetDetail2MonthUsd { get { return TotalTargetDetail2MonthUsd > 0 ? ((TotalUnpaidDetail2MonthUsd * 100) / TotalTargetDetail2MonthUsd) : null; } }

        public string TargetAmountAllString { get { return CommonUtil.TextNumeric(TargetAmountAll); } }
        //public string TargetAmountAllUsdString { get { return CommonUtil.TextNumeric(TargetAmountAllUsd); } }

        public string TargetDetailAllString { get { return CommonUtil.TextNumeric(TargetDetailAll); } }
        //public string TargetDetailAllUsdString { get { return CommonUtil.TextNumeric(TargetDetailAllUsd); } }

        public string TargetAmount2MonthString { get { return CommonUtil.TextNumeric(TargetAmount2Month); } }
        //public string TargetAmount2MonthUsdString { get { return CommonUtil.TextNumeric(TargetAmount2MonthUsd); } }

        public string TargetDetail2MonthString { get { return CommonUtil.TextNumeric(TargetDetail2Month); } }
        //public string TargetDetail2MonthUsdString { get { return CommonUtil.TextNumeric(TargetDetail2MonthUsd); } }
    }
    //End Add

    public class ICS030_DebtActualTableData
    {
        public string BillingOffice { get; set; }
        public string Currency { get; set; }
        public string AllUnpaidActual { get; set; }
        public string AllUnpaidTarget { get; set; }
        public string AllUnpaidCompareTotarget { get; set; }
        public string UnpaidOver2MonthActual { get; set; }
        public string UnpaidOver2MonthTarget { get; set; }
        public string UnpaidOver2MonthCompareTotarget { get; set; }
        public string UnpaidOver6Month { get; set; }
        public string BillingOfficeCode { get; set; }
        public string BillingOfficeName { get; set; }
        public string DisableLinkOfficeFlag { get; set; }

        public decimal? UnpaidAmount { get; set; }
        public decimal? UnpaidAmount2Month { get; set; }
        public decimal? UnpaidAmount6Month { get; set; }
        public int? UnpaidDetail { get; set; }
        public int? UnpaidDetail2Month { get; set; }
        public int? UnpaidDetail6Month { get; set; }
        public decimal? TargetAmountAll { get; set; }
        public decimal? TargetAmount2Month { get; set; }
        public int? TargetDetailAll { get; set; }
        public int? TargetDetail2Month { get; set; }
    }

}
